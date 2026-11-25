<#
.SYNOPSIS
    Comprehensive automated test suite for SimpleRDP.
    Runs every test in its own isolated temporary folder so your real config.json and rdp-files are never touched.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File tests\run-tests.ps1
#>

param(
    [string]$ExePath = (Join-Path $PSScriptRoot "..\bin\Release\SimpleRDP.exe")
)

$ErrorActionPreference = "Stop"

$ExePath = (Resolve-Path $ExePath).Path
if (-not (Test-Path $ExePath)) {
    Write-Host "ERROR: SimpleRDP.exe not found at $ExePath. Build Release first." -ForegroundColor Red
    exit 1
}

Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing
[System.Reflection.Assembly]::LoadFrom($ExePath) | Out-Null

$passed = 0
$failed = 0
$sw = [System.Diagnostics.Stopwatch]::StartNew()

function Run-Test {
    param(
        [string]$Name,
        [scriptblock]$Body
    )

    $tempDir = Join-Path ([System.IO.Path]::GetTempPath()) ("SimpleRDP_Test_" + [Guid]::NewGuid().ToString("N"))
    New-Item -ItemType Directory -Force -Path $tempDir | Out-Null
    $prevCwd = [System.Environment]::CurrentDirectory
    [System.Environment]::CurrentDirectory = $tempDir

    try {
        & $Body $tempDir
        Write-Host "  [PASS] $Name" -ForegroundColor Green
        $script:passed++
    } catch {
        Write-Host "  [FAIL] $Name" -ForegroundColor Red
        $msg = if ($_.Exception.InnerException) { $_.Exception.InnerException.ToString() } else { $_.Exception.Message }
        Write-Host "         $msg" -ForegroundColor Yellow
        $script:failed++
    } finally {
        [System.Environment]::CurrentDirectory = $prevCwd
        Remove-Item -Recurse -Force $tempDir -ErrorAction SilentlyContinue
    }
}

function Assert-True {
    param([bool]$Condition, [string]$Message)
    if (-not $Condition) { throw $Message }
}

function Assert-Equal {
    param($Expected, $Actual, [string]$Message)
    if ($Expected -ne $Actual) {
        throw "$Message (expected: '$Expected', actual: '$Actual')"
    }
}

function Get-PrivateField {
    param($Object, [string]$FieldName)
    $field = $Object.GetType().GetField($FieldName, [System.Reflection.BindingFlags]"NonPublic,Instance")
    if (-not $field) { throw "Private field '$FieldName' not found." }
    return $field.GetValue($Object)
}

function Invoke-PrivateMethod {
    param($Object, [string]$MethodName, [object[]]$MethodArgs = @())
    $method = $Object.GetType().GetMethod($MethodName, [System.Reflection.BindingFlags]"NonPublic,Instance")
    if (-not $method) { throw "Private method '$MethodName' not found." }
    $rawArgs = New-Object object[] $MethodArgs.Length
    for ($i = 0; $i -lt $MethodArgs.Length; $i++) {
        $val = $MethodArgs[$i]
        $rawArgs[$i] = if ($val -is [System.Management.Automation.PSObject]) { $val.psobject.BaseObject } else { $val }
    }
    return $method.Invoke($Object, $rawArgs)
}

function New-HiddenMainForm {
    $form = New-Object SimpleRDP.MainForm
    $form.StartPosition = [System.Windows.Forms.FormStartPosition]::Manual
    $form.Location = New-Object System.Drawing.Point(-2000, -2000)
    $form.Show()
    [System.Windows.Forms.Application]::DoEvents()
    return $form
}

function Invoke-TreeDragDrop {
    param($Form, $TreeView, $DraggedNode, [System.Drawing.Point]$ClientPoint)
    $screenPt = $TreeView.PointToScreen($ClientPoint)
    $rawNode = if ($DraggedNode -is [System.Management.Automation.PSObject]) { $DraggedNode.psobject.BaseObject } else { $DraggedNode }
    $dataObj = New-Object System.Windows.Forms.DataObject
    $dataObj.SetData([System.Windows.Forms.TreeNode], $rawNode)
    $dragArgs = New-Object System.Windows.Forms.DragEventArgs(
        $dataObj,
        0,
        $screenPt.X,
        $screenPt.Y,
        [System.Windows.Forms.DragDropEffects]::Move,
        [System.Windows.Forms.DragDropEffects]::Move
    )
    Invoke-PrivateMethod $Form "treeViewProfiles_DragDrop" @($TreeView, $dragArgs) | Out-Null
    [System.Windows.Forms.Application]::DoEvents()
}

Write-Host ""
Write-Host "Running SimpleRDP Extensive Test Suite against: $ExePath" -ForegroundColor Cyan
Write-Host "========================================================================"

# 1. Legacy flat config.json backward compatibility
Run-Test "01. Legacy flat config.json loads and saves without group noise" {
    param($dir)
    $legacyJson = @'
{
  "Profiles": [
    {
      "Name": "Office PC",
      "Filename": "",
      "Computer": "10.0.0.5",
      "Username": "admin",
      "EncryptedPassword": "",
      "Domain": "CORP"
    }
  ],
  "KeepOpening": true,
  "FormHeight": 489
}
'@
    [System.IO.File]::WriteAllText((Join-Path $dir "config.json"), $legacyJson)

    $cfg = [SimpleRDP.Config]::GetConfig()
    Assert-Equal 1 $cfg.Profiles.Count "Profile count mismatch"
    Assert-True (-not $cfg.Profiles[0].IsGroup) "Legacy profile should not be a group."
    Assert-Equal "Office PC" $cfg.Profiles[0].Name "Profile name mismatch"

    $cfg.Save()
    $saved = [System.IO.File]::ReadAllText((Join-Path $dir "config.json"))
    Assert-True ($saved -notmatch '"IsGroup"') "Regular profile should not serialize IsGroup."
    Assert-True ($saved -notmatch '"Children"') "Regular profile should not serialize Children."
}

# 2. First-run empty folder behavior
Run-Test "02. First-run creates config.json, rdp-files/, and starter profile" {
    param($dir)
    $form = New-HiddenMainForm
    try {
        Assert-True ([System.IO.File]::Exists((Join-Path $dir "config.json"))) "config.json should be created."
        Assert-True ([System.IO.Directory]::Exists((Join-Path $dir "rdp-files"))) "rdp-files directory should be created."
        $tv = Get-PrivateField $form "treeViewProfiles"
        Assert-Equal 1 $tv.Nodes.Count "Should auto-create 1 starter profile."
        Assert-Equal "<New Profile>" $tv.Nodes[0].Text "Starter profile default name mismatch"
        Assert-True $form.EditMode "Starter profile should start in EditMode."
    } finally {
        $form.Close(); $form.Dispose()
    }
}

# 3. Password encryption & RDP password encoding
Run-Test "03. Password encryption, special chars, empty password, and RDP hex" {
    $p = New-Object SimpleRDP.Profile
    Assert-Equal "" $p.Password "Default password should be empty"

    $secret = 'P@ssw0rd! äöü € #123'
    $p.Password = $secret
    Assert-True ($p.EncryptedPassword -ne "" -and $p.EncryptedPassword -ne $secret) "Password must be encrypted."
    Assert-Equal $secret $p.Password "Decrypted password mismatch"

    $rdpHex = $p.GetRDPEncryptedPassword()
    Assert-True ($rdpHex.Length -gt 20 -and $rdpHex -match '^[0-9A-Fa-f]+$') "RDP encrypted password should be valid hex."
}

# 4. PrepareRdpFile preserves custom mstsc settings when updating credentials
Run-Test "04. PrepareRdpFile preserves custom .rdp settings (resolution, audio)" {
    $cfg = [SimpleRDP.Config]::GetConfig()
    $p = New-Object SimpleRDP.Profile
    $p.Name = "Custom RDP"
    $p.Computer = "10.0.0.10"
    $p.Username = "user1"
    $p.Domain = "DOM1"
    $p.PrepareRdpFile()

    # Simulate user editing options via mstsc /edit
    $lines = [System.IO.File]::ReadAllLines($p.Filename) | ForEach-Object {
        if ($_ -like "desktopwidth:i:*") { "desktopwidth:i:1920" }
        elseif ($_ -like "desktopheight:i:*") { "desktopheight:i:1080" }
        else { $_ }
    }
    $lines += "audiomode:i:2"
    [System.IO.File]::WriteAllLines($p.Filename, $lines)

    # Now update profile credentials and re-run PrepareRdpFile
    $p.Computer = "10.0.0.99"
    $p.Username = "user2"
    $p.PrepareRdpFile()

    $updated = [System.IO.File]::ReadAllText($p.Filename)
    Assert-True ($updated -match "full address:s:10\.0\.0\.99") "Updated computer missing."
    Assert-True ($updated -match "username:s:user2") "Updated username missing."
    Assert-True ($updated -match "desktopwidth:i:1920") "Custom desktopwidth was lost!"
    Assert-True ($updated -match "desktopheight:i:1080") "Custom desktopheight was lost!"
    Assert-True ($updated -match "audiomode:i:2") "Custom audiomode was lost!"
}

# 5. Profile Clone, Delete, and Group safety guards
Run-Test "05. Profile Clone copies .rdp independently; Group Delete/Prepare are no-ops" {
    $cfg = [SimpleRDP.Config]::GetConfig()
    $p = New-Object SimpleRDP.Profile
    $p.Name = "Prod"
    $p.Computer = "10.1.2.3"
    $p.PrepareRdpFile()

    $clone = $p.Clone()
    Assert-Equal "Prod (Copy)" $clone.Name "Clone name mismatch"
    Assert-True ($clone.Filename -ne $p.Filename) "Clone must use separate .rdp file."
    Assert-True ([System.IO.File]::Exists($clone.Filename)) "Cloned .rdp file should exist."

    $p.Delete()
    Assert-True (-not [System.IO.File]::Exists($p.Filename)) "Original .rdp should be deleted."
    Assert-True ([System.IO.File]::Exists($clone.Filename)) "Deleting original must not delete clone's .rdp."
    $clone.Delete()

    $g = New-Object SimpleRDP.Profile
    $g.IsGroup = $true
    $g.Name = "MyGroup"
    $g.PrepareRdpFile()
    Assert-Equal "" $g.Filename "Group PrepareRdpFile should be a no-op"
    $g.Delete()
}

# 6. Grouped JSON omits connection fields and normalizes null Children
Run-Test "06. Grouped JSON omits connection fields and normalizes null Children" {
    param($dir)
    $rawJson = @'
{
  "Profiles": [
    {
      "Name": "Empty Group",
      "IsGroup": true,
      "IsExpanded": false,
      "Children": null
    }
  ],
  "KeepOpening": true,
  "FormHeight": 489
}
'@
    [System.IO.File]::WriteAllText((Join-Path $dir "config.json"), $rawJson)
    $cfg = [SimpleRDP.Config]::GetConfig()
    Assert-True ($null -ne $cfg.Profiles[0].Children) "Null Children should be normalized to empty list."
    Assert-Equal 0 $cfg.Profiles[0].Children.Count "Normalized Children should be empty."

    $cfg.Save()
    $saved = [System.IO.File]::ReadAllText((Join-Path $dir "config.json"))
    Assert-True ($saved -notmatch '"Computer"') "Group JSON should not serialize Computer."
    Assert-True ($saved -notmatch '"EncryptedPassword"') "Group JSON should not serialize EncryptedPassword."
}

# 7. UI: Create and Save a new connection
Run-Test "07. UI: Create, fill, and Save a new connection" {
    $form = New-HiddenMainForm
    try {
        (Get-PrivateField $form "textBoxName").Text = "Finance Server"
        (Get-PrivateField $form "textBoxComputer").Text = "10.20.30.40"
        (Get-PrivateField $form "textBoxUsername").Text = "jdoe"
        (Get-PrivateField $form "textBoxPassword").Text = "TopSecret1"
        (Get-PrivateField $form "textBoxDomain").Text = "FIN"

        Invoke-PrivateMethod $form "buttonSave_Click" @($form, [EventArgs]::Empty) | Out-Null
        [System.Windows.Forms.Application]::DoEvents()

        Assert-True (-not $form.EditMode) "EditMode should turn off after Save."
        $tv = Get-PrivateField $form "treeViewProfiles"
        Assert-Equal "Finance Server" $tv.SelectedNode.Text "Tree node text should update on Save"

        $reloaded = [SimpleRDP.Config]::GetConfig()
        Assert-Equal 1 $reloaded.Profiles.Count "Saved profile count mismatch"
        Assert-Equal "10.20.30.40" $reloaded.Profiles[0].Computer "Saved computer mismatch"
        Assert-Equal "TopSecret1" $reloaded.Profiles[0].Password "Saved password mismatch"
        Assert-True ([System.IO.File]::Exists($reloaded.Profiles[0].Filename)) "Saved .rdp file should exist."
    } finally {
        $form.Close(); $form.Dispose()
    }
}

# 8. UI: Create Group, add Connection inside Group, and Rename Group
Run-Test "08. UI: Create Group, add Connection inside it, and Rename Group" {
    $cfg = [SimpleRDP.Config]::GetConfig()
    $c0 = New-Object SimpleRDP.Profile; $c0.Name = "Existing"; $c0.Computer = "10.0.0.1"
    $cfg.Profiles.Add($c0)
    $cfg.Save()

    $form = New-HiddenMainForm
    try {
        # Create new group
        Invoke-PrivateMethod $form "buttonNewGroup_Click" @($form, [EventArgs]::Empty) | Out-Null
        Assert-True $form.EditMode "New group should start in EditMode."
        Assert-True (Get-PrivateField $form "textBoxName").Enabled "Group name box should be enabled."
        Assert-True (-not (Get-PrivateField $form "textBoxComputer").Enabled) "Computer box must stay disabled for groups."

        (Get-PrivateField $form "textBoxName").Text = "Work"
        Invoke-PrivateMethod $form "buttonSave_Click" @($form, [EventArgs]::Empty) | Out-Null

        # With group selected, click New -> should add connection INSIDE Work group
        Invoke-PrivateMethod $form "buttonNew_Click" @($form, [EventArgs]::Empty) | Out-Null
        (Get-PrivateField $form "textBoxName").Text = "Build Server"
        (Get-PrivateField $form "textBoxComputer").Text = "10.0.0.50"
        Invoke-PrivateMethod $form "buttonSave_Click" @($form, [EventArgs]::Empty) | Out-Null

        # Select Work group again and rename it
        $tv = Get-PrivateField $form "treeViewProfiles"
        $tv.SelectedNode = $tv.Nodes[1]
        [System.Windows.Forms.Application]::DoEvents()
        Invoke-PrivateMethod $form "buttonEdit_Click" @($form, [EventArgs]::Empty) | Out-Null
        (Get-PrivateField $form "textBoxName").Text = "Work Renamed"
        Invoke-PrivateMethod $form "buttonSave_Click" @($form, [EventArgs]::Empty) | Out-Null

        $reloaded = [SimpleRDP.Config]::GetConfig()
        Assert-Equal 2 $reloaded.Profiles.Count "Expected 2 top-level items"
        Assert-Equal "Work Renamed" $reloaded.Profiles[1].Name "Group rename did not persist"
        Assert-Equal 1 $reloaded.Profiles[1].Children.Count "New connection should be inside group"
        Assert-Equal "Build Server" $reloaded.Profiles[1].Children[0].Name "Child connection name mismatch"
    } finally {
        $form.Close(); $form.Dispose()
    }
}

# 9. UI: Cancel restores edits and removes unsaved JustAdded item
Run-Test "09. UI: Cancel restores edited values and removes unsaved new items" {
    $cfg = [SimpleRDP.Config]::GetConfig()
    $c = New-Object SimpleRDP.Profile; $c.Name = "Stable"; $c.Computer = "10.0.0.2"
    $cfg.Profiles.Add($c)
    $cfg.Save()

    $form = New-HiddenMainForm
    try {
        # Edit existing and cancel
        Invoke-PrivateMethod $form "buttonEdit_Click" @($form, [EventArgs]::Empty) | Out-Null
        (Get-PrivateField $form "textBoxName").Text = "Changed"
        (Get-PrivateField $form "textBoxComputer").Text = "9.9.9.9"
        Invoke-PrivateMethod $form "buttonCancel_Click" @($form, [EventArgs]::Empty) | Out-Null

        Assert-True (-not $form.EditMode) "EditMode should be false after Cancel."
        Assert-Equal "Stable" (Get-PrivateField $form "textBoxName").Text "Name should revert on Cancel"
        Assert-Equal "10.0.0.2" (Get-PrivateField $form "textBoxComputer").Text "Computer should revert on Cancel"

        # Add new profile and cancel -> should remove it
        Invoke-PrivateMethod $form "buttonNew_Click" @($form, [EventArgs]::Empty) | Out-Null
        $tv = Get-PrivateField $form "treeViewProfiles"
        Assert-Equal 2 $tv.Nodes.Count "Should temporarily have 2 nodes."
        Invoke-PrivateMethod $form "buttonCancel_Click" @($form, [EventArgs]::Empty) | Out-Null
        Assert-Equal 1 $tv.Nodes.Count "Cancelling JustAdded profile should remove it."
    } finally {
        $form.Close(); $form.Dispose()
    }
}

# 10. UI: Clone inside a group places clone directly below original
Run-Test "10. UI: Clone inside a group inserts copy right beneath original" {
    $cfg = [SimpleRDP.Config]::GetConfig()
    $g = New-Object SimpleRDP.Profile; $g.Name = "Cluster"; $g.IsGroup = $true; $g.IsExpanded = $true
    $n1 = New-Object SimpleRDP.Profile; $n1.Name = "Node 1"; $n1.Computer = "10.0.1.1"
    $n2 = New-Object SimpleRDP.Profile; $n2.Name = "Node 2"; $n2.Computer = "10.0.1.2"
    $g.Children.Add($n1); $g.Children.Add($n2)
    $cfg.Profiles.Add($g)
    $cfg.Save()

    $form = New-HiddenMainForm
    try {
        $tv = Get-PrivateField $form "treeViewProfiles"
        $tv.SelectedNode = $tv.Nodes[0].Nodes[0] # Node 1
        [System.Windows.Forms.Application]::DoEvents()

        Invoke-PrivateMethod $form "buttonClone_Click" @($form, [EventArgs]::Empty) | Out-Null
        Invoke-PrivateMethod $form "buttonSave_Click" @($form, [EventArgs]::Empty) | Out-Null

        $reloaded = [SimpleRDP.Config]::GetConfig()
        Assert-Equal 3 $reloaded.Profiles[0].Children.Count "Group should now have 3 children"
        Assert-Equal "Node 1" $reloaded.Profiles[0].Children[0].Name "Child 0 mismatch"
        Assert-Equal "Node 1 (Copy)" $reloaded.Profiles[0].Children[1].Name "Clone should be inserted at index 1"
        Assert-Equal "Node 2" $reloaded.Profiles[0].Children[2].Name "Child 2 mismatch"
    } finally {
        $form.Close(); $form.Dispose()
    }
}

# 11. UI: Context menu states + Expand All / Collapse All
Run-Test "11. UI: Context menu states and Expand All / Collapse All" {
    $cfg = [SimpleRDP.Config]::GetConfig()
    $g1 = New-Object SimpleRDP.Profile; $g1.Name = "G1"; $g1.IsGroup = $true; $g1.IsExpanded = $false
    $g1.Children.Add((New-Object SimpleRDP.Profile -Property @{ Name = "C1"; Computer = "10.0.0.1" }))
    $g2 = New-Object SimpleRDP.Profile; $g2.Name = "G2"; $g2.IsGroup = $true; $g2.IsExpanded = $false
    $g2.Children.Add((New-Object SimpleRDP.Profile -Property @{ Name = "C2"; Computer = "10.0.0.2" }))
    $cfg.Profiles.Add($g1); $cfg.Profiles.Add($g2)
    $cfg.Save()

    $form = New-HiddenMainForm
    try {
        # Check context menu for selected group
        $cancelArgs = New-Object System.ComponentModel.CancelEventArgs
        Invoke-PrivateMethod $form "contextMenuTree_Opening" @($form, $cancelArgs) | Out-Null
        Assert-Equal "Rename Group" (Get-PrivateField $form "menuEdit").Text "Group menu should say Rename Group"
        Assert-True (-not (Get-PrivateField $form "menuClone").Enabled) "Clone menu should be disabled for group"
        Assert-True (Get-PrivateField $form "menuExpandAll").Enabled "Expand All should be enabled"

        # Expand All
        Invoke-PrivateMethod $form "menuExpandAll_Click" @($form, [EventArgs]::Empty) | Out-Null
        $tv = Get-PrivateField $form "treeViewProfiles"
        Assert-True ($tv.Nodes[0].IsExpanded -and $tv.Nodes[1].IsExpanded) "Both groups should be expanded."
        Assert-True ($tv.Nodes[0].ImageIndex -eq 2 -and $tv.Nodes[1].ImageIndex -eq 2) "Both icons should be open folder."

        $reloaded = [SimpleRDP.Config]::GetConfig()
        Assert-True ($reloaded.Profiles[0].IsExpanded -and $reloaded.Profiles[1].IsExpanded) "Expand All must persist."

        # Collapse All
        Invoke-PrivateMethod $form "menuCollapseAll_Click" @($form, [EventArgs]::Empty) | Out-Null
        Assert-True (-not $tv.Nodes[0].IsExpanded -and -not $tv.Nodes[1].IsExpanded) "Both groups should be collapsed."
        $reloaded2 = [SimpleRDP.Config]::GetConfig()
        Assert-True (-not $reloaded2.Profiles[0].IsExpanded -and -not $reloaded2.Profiles[1].IsExpanded) "Collapse All must persist."
    } finally {
        $form.Close(); $form.Dispose()
    }
}

# 12. End-to-end Drag-and-Drop: move into group, reorder inside group, move out, reorder groups
Run-Test "12. Drag-and-Drop end-to-end: move into/out of group and reorder" {
    $cfg = [SimpleRDP.Config]::GetConfig()
    $gA = New-Object SimpleRDP.Profile; $gA.Name = "Group A"; $gA.IsGroup = $true; $gA.IsExpanded = $false
    $gA.Children.Add((New-Object SimpleRDP.Profile -Property @{ Name = "A1"; Computer = "10.0.1.1" }))
    $gB = New-Object SimpleRDP.Profile; $gB.Name = "Group B"; $gB.IsGroup = $true; $gB.IsExpanded = $true
    $gB.Children.Add((New-Object SimpleRDP.Profile -Property @{ Name = "B1"; Computer = "10.0.2.1" }))
    $rootC = New-Object SimpleRDP.Profile; $rootC.Name = "RootConn"; $rootC.Computer = "10.0.3.1"
    $cfg.Profiles.Add($gA); $cfg.Profiles.Add($gB); $cfg.Profiles.Add($rootC)
    $cfg.Save()

    $form = New-HiddenMainForm
    try {
        $tv = Get-PrivateField $form "treeViewProfiles"

        # A) Drag RootConn INTO collapsed Group A -> auto-expands Group A and adds RootConn
        $nodeGA = $tv.Nodes[0]
        $nodeRootC = $tv.Nodes[2]
        $midGA = New-Object System.Drawing.Point(($nodeGA.Bounds.Left + 10), ($nodeGA.Bounds.Top + [int]($nodeGA.Bounds.Height / 2)))
        Invoke-TreeDragDrop $form $tv $nodeRootC $midGA

        $c1 = [SimpleRDP.Config]::GetConfig()
        Assert-Equal 2 $c1.Profiles.Count "Root should now have 2 groups"
        Assert-True $c1.Profiles[0].IsExpanded "Group A should auto-expand when receiving a drop"
        Assert-Equal 2 $c1.Profiles[0].Children.Count "Group A should have 2 children"
        Assert-Equal "RootConn" $c1.Profiles[0].Children[1].Name "RootConn should be second child of Group A"

        # B) Reorder inside Group A: drag RootConn (child 1) ABOVE A1 (child 0)
        $nodeA1 = $tv.Nodes[0].Nodes[0]
        $nodeMovedRootC = $tv.Nodes[0].Nodes[1]
        $topA1 = New-Object System.Drawing.Point(($nodeA1.Bounds.Left + 10), ($nodeA1.Bounds.Top + 2))
        Invoke-TreeDragDrop $form $tv $nodeMovedRootC $topA1

        $c2 = [SimpleRDP.Config]::GetConfig()
        Assert-Equal "RootConn" $c2.Profiles[0].Children[0].Name "RootConn should now be first in Group A"
        Assert-Equal "A1" $c2.Profiles[0].Children[1].Name "A1 should now be second in Group A"

        # C) Drag RootConn OUT of Group A into empty space at bottom -> back to root
        $nodeMovedRootC2 = $tv.Nodes[0].Nodes[0]
        $emptyBottom = New-Object System.Drawing.Point(20, ($tv.ClientSize.Height - 5))
        Invoke-TreeDragDrop $form $tv $nodeMovedRootC2 $emptyBottom

        $c3 = [SimpleRDP.Config]::GetConfig()
        Assert-Equal 3 $c3.Profiles.Count "RootConn should be back at root"
        Assert-Equal "RootConn" $c3.Profiles[2].Name "RootConn should be last at root"
        Assert-Equal 1 $c3.Profiles[0].Children.Count "Group A should be back to 1 child"

        # D) Drag Group A BELOW Group B -> reorders groups with children intact
        $nodeGA2 = $tv.Nodes[0]
        $nodeGB2 = $tv.Nodes[1]
        $bottomGB = New-Object System.Drawing.Point(($nodeGB2.Bounds.Left + 10), ($nodeGB2.Bounds.Bottom - 2))
        Invoke-TreeDragDrop $form $tv $nodeGA2 $bottomGB

        $c4 = [SimpleRDP.Config]::GetConfig()
        Assert-Equal "Group B" $c4.Profiles[0].Name "Group B should now be first"
        Assert-Equal "Group A" $c4.Profiles[1].Name "Group A should now be second"
        Assert-Equal "A1" $c4.Profiles[1].Children[0].Name "Group A's children must stay intact"

        # E) Verify DragDrop is ignored while in EditMode
        $form.EditMode = $true
        $nodeGB3 = $tv.Nodes[0]
        $nodeGA3 = $tv.Nodes[1]
        $bottomGA = New-Object System.Drawing.Point(($nodeGA3.Bounds.Left + 10), ($nodeGA3.Bounds.Bottom - 2))
        Invoke-TreeDragDrop $form $tv $nodeGB3 $bottomGA
        $c5 = [SimpleRDP.Config]::GetConfig()
        Assert-Equal "Group B" $c5.Profiles[0].Name "DragDrop must be blocked in EditMode"
    } finally {
        $form.Close(); $form.Dispose()
    }
}

# 13. KeepOpening toggle & Window height persistence (save on resize, restore on reopen)
Run-Test "13. KeepOpening checkbox and window height save/restore across sessions" {
    $cfg = [SimpleRDP.Config]::GetConfig()
    $cfg.Profiles.Add((New-Object SimpleRDP.Profile -Property @{ Name = "Host"; Computer = "127.0.0.1" }))
    $cfg.KeepOpening = $true
    $cfg.Save()

    $form1 = New-HiddenMainForm
    try {
        $chk = Get-PrivateField $form1 "checkBoxKeepOpening"
        $chk.Checked = $false
        [System.Windows.Forms.Application]::DoEvents()
        $form1.Height = 610
        [System.Windows.Forms.Application]::DoEvents()
    } finally {
        $form1.Close(); $form1.Dispose()
    }

    $reloaded = [SimpleRDP.Config]::GetConfig()
    Assert-True (-not $reloaded.KeepOpening) "KeepOpening=false should persist."
    Assert-Equal 610 $reloaded.FormHeight "FormHeight 610 should persist."

    # Reopen and verify restored height
    $form2 = New-HiddenMainForm
    try {
        Assert-Equal 610 $form2.Height "Reopened form should restore saved height 610."
        Assert-True (-not (Get-PrivateField $form2 "checkBoxKeepOpening").Checked) "KeepOpening should restore unchecked."
    } finally {
        $form2.Close(); $form2.Dispose()
    }
}

# 14. About dialog version & MIT license text
Run-Test "14. About dialog displays correct version and MIT license" {
    $owner = New-HiddenMainForm
    $about = New-Object SimpleRDP.About
    try {
        $about.Owner = $owner
        $about.StartPosition = [System.Windows.Forms.FormStartPosition]::Manual
        $about.Location = New-Object System.Drawing.Point(-2000, -2000)
        $about.Show()
        [System.Windows.Forms.Application]::DoEvents()

        $expectedVersion = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($ExePath).FileVersion
        $lblName = Get-PrivateField $about "labelName"
        $txtLicense = Get-PrivateField $about "textBoxLicense"

        Assert-Equal ("SimpleRDP v" + $expectedVersion) $lblName.Text "About version label mismatch"
        Assert-True ($txtLicense.Text -match "Modified 2026 by Alex") "License missing Alex notice."
        Assert-True ($txtLicense.Text -match "MIT License") "License missing MIT header."
        Assert-True ($txtLicense.Text -match "Copyright \(c\) 2025 Louis Lam") "License missing original copyright."
    } finally {
        $about.Close(); $about.Dispose()
        $owner.Close(); $owner.Dispose()
    }
}

$sw.Stop()
Write-Host "========================================================================"
Write-Host ("Summary: {0} passed, {1} failed ({2:N2}s)" -f $passed, $failed, $sw.Elapsed.TotalSeconds) -ForegroundColor $(if ($failed -eq 0) { "Green" } else { "Red" })
Write-Host ""

if ($failed -gt 0) {
    exit 1
}
