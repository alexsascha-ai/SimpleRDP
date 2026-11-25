using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;

namespace SimpleRDP {
    public partial class MainForm : Form {

        private const int IconConnectionIndex = 0;
        private const int IconFolderClosedIndex = 1;
        private const int IconFolderOpenIndex = 2;

        private Config _config;
        private bool _editMode = false;
        private Profile? selectedProfile = null;
        private int _initialHeight;
        private bool _suppressRedirectionWarningEvent = false;
        private bool _suppressTreeEvents = false;
        private const string RdpPolicySubKey = @"SOFTWARE\Policies\Microsoft\Windows NT\Terminal Services\Client";
        private const string RedirectionWarningValueName = "RedirectionWarningDialogVersion";

        public MainForm() {
            InitializeComponent();
            InitializeTreeIcons();
            _config = Config.GetConfig();
        }

        private void InitializeTreeIcons() {
            imageListTree.Images.Clear();
            imageListTree.Images.Add(CreateConnectionBitmap(size: 20));
            imageListTree.Images.Add(CreateFolderBitmap(open: false, size: 20));
            imageListTree.Images.Add(CreateFolderBitmap(open: true, size: 20));
        }

        private static Bitmap CreateConnectionBitmap(int size) {
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp)) {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // Monitor stand
                using (var standBrush = new SolidBrush(Color.FromArgb(108, 122, 137))) {
                    g.FillRectangle(standBrush, 7.5f, 13.5f, 3.5f, 2.5f);
                    g.FillRectangle(standBrush, 5.5f, 15.5f, 7.5f, 1.8f);
                }

                // Monitor outer bezel
                using (var bezelBrush = new SolidBrush(Color.FromArgb(46, 58, 77))) {
                    g.FillRectangle(bezelBrush, 1.5f, 2f, 15.5f, 12f);
                }

                // Monitor screen
                using (var screenBrush = new SolidBrush(Color.FromArgb(74, 101, 214)))
                using (var accentBrush = new SolidBrush(Color.FromArgb(118, 142, 232))) {
                    g.FillRectangle(screenBrush, 3.2f, 3.7f, 12.1f, 8.6f);
                    g.FillRectangle(accentBrush, 4.4f, 5.0f, 5.2f, 1.3f);
                    g.FillRectangle(accentBrush, 4.4f, 7.1f, 3.5f, 1.3f);
                }

                // Bottom-right circular badge with >_
                using (var whiteBrush = new SolidBrush(Color.White))
                using (var badgeBrush = new SolidBrush(Color.FromArgb(29, 92, 224)))
                using (var glyphPen = new Pen(Color.White, 1.4f) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round }) {
                    g.FillEllipse(whiteBrush, 9.5f, 9.5f, 9.5f, 9.5f);
                    g.FillEllipse(badgeBrush, 10.5f, 10.5f, 7.5f, 7.5f);
                    g.DrawLines(glyphPen, new[] {
                        new PointF(12.2f, 12.6f),
                        new PointF(14.1f, 14.2f),
                        new PointF(12.2f, 15.8f)
                    });
                    g.DrawLine(glyphPen, 14.7f, 15.9f, 16.3f, 15.9f);
                }
            }
            return bmp;
        }

        private static Bitmap CreateFolderBitmap(bool open, int size) {
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp)) {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                using (var backBrush = new SolidBrush(Color.FromArgb(228, 174, 58)))
                using (var frontBrush = new SolidBrush(open ? Color.FromArgb(250, 212, 102) : Color.FromArgb(245, 198, 78)))
                using (var borderPen = new Pen(Color.FromArgb(182, 130, 32), 1.2f)) {
                    // Folder tab + back
                    var backPath = new GraphicsPath();
                    backPath.AddPolygon(new[] {
                        new PointF(2f, 4f),
                        new PointF(8f, 4f),
                        new PointF(10f, 6.5f),
                        new PointF(17.5f, 6.5f),
                        new PointF(17.5f, 16.5f),
                        new PointF(2f, 16.5f)
                    });
                    g.FillPath(backBrush, backPath);
                    g.DrawPath(borderPen, backPath);

                    if (open) {
                        // Tilted open front flap
                        var frontPath = new GraphicsPath();
                        frontPath.AddPolygon(new[] {
                            new PointF(4f, 9f),
                            new PointF(18.5f, 9f),
                            new PointF(16.8f, 16.5f),
                            new PointF(2f, 16.5f)
                        });
                        g.FillPath(frontBrush, frontPath);
                        g.DrawPath(borderPen, frontPath);
                    } else {
                        // Closed front flap
                        var frontRect = new RectangleF(2f, 7.5f, 15.5f, 9f);
                        g.FillRectangle(frontBrush, frontRect);
                        g.DrawRectangle(borderPen, frontRect.X, frontRect.Y, frontRect.Width, frontRect.Height);
                    }
                }
            }
            return bmp;
        }

        private void MainForm_Load(object sender, EventArgs e) {
            if (_config.FormHeight >= this.MinimumSize.Height) {
                this.Height = _config.FormHeight;
            }
            _initialHeight = this.Height;

            if (_config.Profiles.Count == 0) {
                var profile = new Profile { JustAdded = true };
                _config.Profiles.Add(profile);
                PopulateTree(profile);
            } else {
                PopulateTree();
            }

            checkBoxKeepOpening.Checked = _config.KeepOpening;
            LoadRedirectionWarningSetting();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            int currentHeight = this.WindowState == FormWindowState.Normal ? this.Height : this.RestoreBounds.Height;
            if (currentHeight != _initialHeight && currentHeight >= this.MinimumSize.Height) {
                _config.FormHeight = currentHeight;
                _config.Save();
            }
        }

        private bool IsRedirectionWarningDialogEnabled() {
            try {
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var key = baseKey.OpenSubKey(RdpPolicySubKey, false)) {
                    if (key != null) {
                        var val = key.GetValue(RedirectionWarningValueName);
                        if (val is int intVal && intVal == 1) {
                            return true;
                        }
                    }
                }
            } catch {
            }
            return false;
        }

        private void LoadRedirectionWarningSetting() {
            _suppressRedirectionWarningEvent = true;
            checkBoxRedirectionWarning.Checked = IsRedirectionWarningDialogEnabled();
            _suppressRedirectionWarningEvent = false;
        }

        private void checkBoxRedirectionWarning_CheckedChanged(object sender, EventArgs e) {
            if (_suppressRedirectionWarningEvent) {
                return;
            }

            bool enable = checkBoxRedirectionWarning.Checked;
            try {
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var key = baseKey.CreateSubKey(RdpPolicySubKey, true)) {
                    if (key != null) {
                        if (enable) {
                            key.SetValue(RedirectionWarningValueName, 1, RegistryValueKind.DWord);
                        } else {
                            key.DeleteValue(RedirectionWarningValueName, false);
                        }
                        return;
                    }
                }
            } catch (UnauthorizedAccessException) {
            } catch (System.Security.SecurityException) {
            } catch {
            }

            try {
                string args = enable
                    ? $"add \"HKLM\\{RdpPolicySubKey}\" /v \"{RedirectionWarningValueName}\" /t REG_DWORD /d 1 /f /reg:64"
                    : $"delete \"HKLM\\{RdpPolicySubKey}\" /v \"{RedirectionWarningValueName}\" /f /reg:64";

                var psi = new ProcessStartInfo {
                    FileName = "reg.exe",
                    Arguments = args,
                    Verb = "runas",
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                var proc = Process.Start(psi);
                proc?.WaitForExit();
            } catch (Exception ex) {
                MessageBox.Show("Failed to update registry setting: " + ex.Message, "SimpleRDP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            LoadRedirectionWarningSetting();
        }

        private void PopulateTree(Profile? selectTarget = null) {
            _suppressTreeEvents = true;
            treeViewProfiles.BeginUpdate();
            try {
                treeViewProfiles.Nodes.Clear();

                foreach (var item in _config.Profiles) {
                    var node = CreateTreeNode(item);
                    treeViewProfiles.Nodes.Add(node);

                    if (item.IsGroup) {
                        if (item.Children != null) {
                            foreach (var child in item.Children) {
                                node.Nodes.Add(CreateTreeNode(child));
                            }
                        }

                        if (item.IsExpanded) {
                            node.Expand();
                        } else {
                            node.Collapse();
                        }
                        UpdateGroupNodeIcon(node, item.IsExpanded);
                    }
                }

                TreeNode? nodeToSelect = selectTarget != null
                    ? FindNodeByProfile(treeViewProfiles.Nodes, selectTarget)
                    : (treeViewProfiles.Nodes.Count > 0 ? treeViewProfiles.Nodes[0] : null);

                if (nodeToSelect == null && treeViewProfiles.Nodes.Count > 0) {
                    nodeToSelect = treeViewProfiles.Nodes[0];
                }

                treeViewProfiles.SelectedNode = nodeToSelect;
                nodeToSelect?.EnsureVisible();
            } finally {
                treeViewProfiles.EndUpdate();
                _suppressTreeEvents = false;
            }

            SelectProfile(true);
        }

        private static TreeNode CreateTreeNode(Profile profile) {
            int iconIndex = profile.IsGroup
                ? (profile.IsExpanded ? IconFolderOpenIndex : IconFolderClosedIndex)
                : IconConnectionIndex;

            return new TreeNode(profile.Name) {
                Tag = profile,
                ImageIndex = iconIndex,
                SelectedImageIndex = iconIndex
            };
        }

        private static void UpdateGroupNodeIcon(TreeNode node, bool isExpanded) {
            int iconIndex = isExpanded ? IconFolderOpenIndex : IconFolderClosedIndex;
            node.ImageIndex = iconIndex;
            node.SelectedImageIndex = iconIndex;
        }

        private static TreeNode? FindNodeByProfile(TreeNodeCollection nodes, Profile target) {
            foreach (TreeNode node in nodes) {
                if (ReferenceEquals(node.Tag, target)) {
                    return node;
                }
                if (node.Nodes.Count > 0) {
                    var found = FindNodeByProfile(node.Nodes, target);
                    if (found != null) {
                        return found;
                    }
                }
            }
            return null;
        }

        private List<Profile>? FindParentList(Profile target, out Profile? parentGroup) {
            if (_config.Profiles.Contains(target)) {
                parentGroup = null;
                return _config.Profiles;
            }

            foreach (var item in _config.Profiles) {
                if (item.IsGroup && item.Children != null && item.Children.Contains(target)) {
                    parentGroup = item;
                    return item.Children;
                }
            }

            parentGroup = null;
            return null;
        }

        private int GetTotalItemCount() {
            int count = _config.Profiles.Count;
            foreach (var item in _config.Profiles) {
                if (item.IsGroup && item.Children != null) {
                    count += item.Children.Count;
                }
            }
            return count;
        }

        public bool EditMode {
            get => _editMode;
            set {
                _editMode = value;
                bool hasSelection = selectedProfile != null;
                bool isGroup = selectedProfile?.IsGroup ?? false;

                buttonEdit.Visible = !value;
                buttonEdit.Enabled = !value && hasSelection;
                buttonSave.Visible = value;
                buttonCancel.Visible = value;

                buttonOptions.Enabled = !value && hasSelection && !isGroup;
                buttonConnect.Enabled = !value && hasSelection && !isGroup;
                buttonClone.Enabled = hasSelection && !isGroup;
                buttonDelete.Enabled = hasSelection;

                textBoxName.Enabled = value && hasSelection;
                textBoxComputer.Enabled = value && hasSelection && !isGroup;
                textBoxUsername.Enabled = value && hasSelection && !isGroup;
                textBoxPassword.Enabled = value && hasSelection && !isGroup;
                textBoxDomain.Enabled = value && hasSelection && !isGroup;
            }
        }

        private void AddNewProfile() {
            var profile = new Profile { JustAdded = true };

            var current = GetSelectedProfile();
            if (current != null && current.IsGroup) {
                current.Children.Add(profile);
                current.IsExpanded = true;
            } else if (current != null) {
                var parentList = FindParentList(current, out var parentGroup);
                if (parentGroup != null && parentList != null) {
                    parentList.Add(profile);
                    parentGroup.IsExpanded = true;
                } else {
                    _config.Profiles.Add(profile);
                }
            } else {
                _config.Profiles.Add(profile);
            }

            PopulateTree(profile);
            textBoxName.Focus();
            textBoxName.SelectAll();
        }

        private void AddNewGroup() {
            var group = new Profile {
                IsGroup = true,
                IsExpanded = true,
                JustAdded = true
            };

            _config.Profiles.Add(group);
            PopulateTree(group);
            textBoxName.Focus();
            textBoxName.SelectAll();
        }

        private void buttonMoreOptions_Click(object sender, EventArgs e) {
            var profile = GetSelectedProfile();
            if (profile == null || profile.IsGroup) {
                return;
            }

            if (string.IsNullOrEmpty(profile.Filename)) {
                profile.PrepareRdpFile();
            }

            ProcessStartInfo startInfo = new ProcessStartInfo {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = "mstsc.exe",
                Arguments = "/edit " + profile.Filename,
            };

            try {
                var exeProcess = Process.Start(startInfo) ?? throw new InvalidOperationException();
                exeProcess.WaitForExit();
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e) {
            var profile = GetSelectedProfile();
            if (profile == null || profile.IsGroup) {
                return;
            }

            if (string.IsNullOrWhiteSpace(profile.Computer)) {
                MessageBox.Show("Invalid connection");
                return;
            }

            profile.PrepareRdpFile();

            ProcessStartInfo startInfo = new ProcessStartInfo {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = "mstsc.exe",
                Arguments = profile.Filename,
            };

            try {
                var exeProcess = Process.Start(startInfo) ?? throw new InvalidOperationException();
                exeProcess.WaitForExit();

                if (!_config.KeepOpening) {
                    this.Close();
                }

            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }

        private Profile? GetSelectedProfile() {
            return treeViewProfiles.SelectedNode?.Tag as Profile;
        }

        private void SelectProfile(bool force = false) {
            var profile = GetSelectedProfile();

            if (ReferenceEquals(profile, selectedProfile) && !force) {
                return;
            }

            selectedProfile = profile;

            if (profile == null) {
                EditMode = false;
                textBoxName.Text = "";
                textBoxComputer.Text = "";
                textBoxUsername.Text = "";
                textBoxPassword.Text = "";
                textBoxDomain.Text = "";
                return;
            }

            EditMode = profile.JustAdded;

            textBoxName.Text = profile.Name;
            if (profile.IsGroup) {
                textBoxComputer.Text = "";
                textBoxUsername.Text = "";
                textBoxPassword.Text = "";
                textBoxDomain.Text = "";
            } else {
                textBoxComputer.Text = profile.Computer;
                textBoxUsername.Text = profile.Username;
                textBoxPassword.Text = profile.Password;
                textBoxDomain.Text = profile.Domain;
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e) {
            if (GetSelectedProfile() == null) {
                return;
            }
            EditMode = true;
            textBoxName.Focus();
            textBoxName.SelectAll();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            var profile = GetSelectedProfile();
            if (profile == null) {
                EditMode = false;
                return;
            }

            if (profile.JustAdded && GetTotalItemCount() > 1) {
                var parentList = FindParentList(profile, out var parentGroup);
                profile.Delete();
                parentList?.Remove(profile);
                EditMode = false;
                PopulateTree(parentGroup);
            } else {
                EditMode = false;
                SelectProfile(true);
            }
        }

        private void buttonNew_Click(object sender, EventArgs e) {
            AddNewProfile();
        }

        private void buttonNewGroup_Click(object sender, EventArgs e) {
            AddNewGroup();
        }

        private void buttonClone_Click(object sender, EventArgs e) {
            var selected = GetSelectedProfile();
            if (selected == null || selected.IsGroup) {
                return;
            }

            var clone = selected.Clone();
            var parentList = FindParentList(selected, out _);
            if (parentList != null) {
                int idx = parentList.IndexOf(selected);
                parentList.Insert(idx + 1, clone);
            } else {
                _config.Profiles.Add(clone);
            }

            PopulateTree(clone);
            textBoxName.Focus();
            textBoxName.SelectAll();
        }

        private void buttonDelete_Click(object sender, EventArgs e) {
            var selected = GetSelectedProfile();
            if (selected == null) {
                return;
            }

            if (selected.IsGroup) {
                string prompt = selected.Children.Count > 0
                    ? $"Are you sure to delete the group \"{selected.Name}\"?\n\nIts {selected.Children.Count} connection(s) will be moved to the main list."
                    : $"Are you sure to delete the group \"{selected.Name}\"?";

                var confirmResult = MessageBox.Show(prompt, "Confirm", MessageBoxButtons.YesNo);
                if (confirmResult != DialogResult.Yes) {
                    return;
                }

                int groupIndex = _config.Profiles.IndexOf(selected);
                Profile? nextSelect = selected.Children.Count > 0 ? selected.Children[0] : null;
                _config.Profiles.Remove(selected);

                if (selected.Children.Count > 0) {
                    int insertAt = groupIndex >= 0 ? Math.Min(groupIndex, _config.Profiles.Count) : _config.Profiles.Count;
                    _config.Profiles.InsertRange(insertAt, selected.Children);
                }

                _config.Save();

                if (_config.Profiles.Count == 0) {
                    AddNewProfile();
                } else {
                    PopulateTree(nextSelect);
                }
                return;
            }

            var confirmProfile = MessageBox.Show(
                "Are you sure to delete this profile?",
                "Confirm",
                MessageBoxButtons.YesNo);

            if (confirmProfile == DialogResult.Yes) {
                var parentList = FindParentList(selected, out var parentGroup);
                int oldIdx = parentList?.IndexOf(selected) ?? -1;

                selected.Delete();
                parentList?.Remove(selected);
                _config.Save();

                if (_config.Profiles.Count == 0) {
                    AddNewProfile();
                } else {
                    Profile? nextSelect = null;
                    if (parentList != null && parentList.Count > 0) {
                        nextSelect = parentList[Math.Min(Math.Max(0, oldIdx), parentList.Count - 1)];
                    } else if (parentGroup != null) {
                        nextSelect = parentGroup;
                    }
                    PopulateTree(nextSelect);
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e) {
            var profile = GetSelectedProfile();
            if (profile == null) {
                return;
            }

            profile.JustAdded = false;
            profile.Name = textBoxName.Text.Trim();

            if (!profile.IsGroup) {
                profile.Computer = textBoxComputer.Text;
                profile.Username = textBoxUsername.Text;
                profile.Password = textBoxPassword.Text;
                profile.Domain = textBoxDomain.Text;
                profile.PrepareRdpFile();
            }

            _config.Save();
            EditMode = false;

            if (treeViewProfiles.SelectedNode != null) {
                treeViewProfiles.SelectedNode.Text = profile.Name;
            }
        }

        private void checkBoxKeepOpening_CheckedChanged(object sender, EventArgs e) {
            _config.KeepOpening = checkBoxKeepOpening.Checked;
            _config.Save();
        }

        private void buttonAbout_Click(object sender, EventArgs e) {
            About about = new About();
            about.ShowDialog(this);
        }

        private void treeViewProfiles_AfterSelect(object sender, TreeViewEventArgs e) {
            if (_suppressTreeEvents) {
                return;
            }
            SelectProfile();
        }

        private void treeViewProfiles_AfterExpand(object sender, TreeViewEventArgs e) {
            if (_suppressTreeEvents || e.Node == null) {
                return;
            }

            if (e.Node.Tag is Profile group && group.IsGroup) {
                UpdateGroupNodeIcon(e.Node, true);
                if (!group.IsExpanded) {
                    group.IsExpanded = true;
                    _config.Save();
                }
            }
        }

        private void treeViewProfiles_AfterCollapse(object sender, TreeViewEventArgs e) {
            if (_suppressTreeEvents || e.Node == null) {
                return;
            }

            if (e.Node.Tag is Profile group && group.IsGroup) {
                UpdateGroupNodeIcon(e.Node, false);
                if (group.IsExpanded) {
                    group.IsExpanded = false;
                    _config.Save();
                }
            }
        }

        private void treeViewProfiles_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
            if (e.Button == MouseButtons.Right && e.Node != null) {
                treeViewProfiles.SelectedNode = e.Node;
            }
        }

        private void treeViewProfiles_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
            if (e.Button != MouseButtons.Left || EditMode) {
                return;
            }

            var hit = treeViewProfiles.HitTest(e.Location);
            if (hit.Node == null) {
                return;
            }

            var clickableAreas = TreeViewHitTestLocations.Image | TreeViewHitTestLocations.Label | TreeViewHitTestLocations.RightOfLabel;
            if ((hit.Location & clickableAreas) == 0) {
                return;
            }

            if (hit.Node.Tag is Profile profile && !profile.IsGroup) {
                treeViewProfiles.SelectedNode = hit.Node;
                buttonConnect_Click(sender, e);
            }
        }

        private void treeViewProfiles_KeyDown(object sender, KeyEventArgs e) {
            if (EditMode) {
                return;
            }

            var selectedNode = treeViewProfiles.SelectedNode;
            if (selectedNode == null) {
                return;
            }

            if (e.KeyCode == Keys.F2) {
                buttonEdit_Click(sender, EventArgs.Empty);
                e.Handled = true;
            } else if (e.KeyCode == Keys.Delete) {
                buttonDelete_Click(sender, EventArgs.Empty);
                e.Handled = true;
            } else if (e.KeyCode == Keys.Enter) {
                if (selectedNode.Tag is Profile profile) {
                    if (profile.IsGroup) {
                        selectedNode.Toggle();
                    } else {
                        buttonConnect_Click(sender, EventArgs.Empty);
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void contextMenuTree_Opening(object sender, CancelEventArgs e) {
            var selected = GetSelectedProfile();
            bool hasSelection = selected != null;
            bool isGroup = selected?.IsGroup ?? false;
            bool hasGroups = _config.Profiles.Any(p => p.IsGroup);

            menuNewConnection.Enabled = !EditMode;
            menuNewGroup.Enabled = !EditMode;
            menuEdit.Enabled = !EditMode && hasSelection;
            menuEdit.Text = isGroup ? "Rename Group" : "Edit";
            menuClone.Enabled = !EditMode && hasSelection && !isGroup;
            menuDelete.Enabled = !EditMode && hasSelection;
            menuExpandAll.Enabled = hasGroups;
            menuCollapseAll.Enabled = hasGroups;
        }

        private void menuExpandAll_Click(object sender, EventArgs e) {
            SetAllGroupsExpanded(true);
        }

        private void menuCollapseAll_Click(object sender, EventArgs e) {
            SetAllGroupsExpanded(false);
        }

        private void SetAllGroupsExpanded(bool expanded) {
            bool changed = false;
            _suppressTreeEvents = true;
            treeViewProfiles.BeginUpdate();
            try {
                foreach (TreeNode node in treeViewProfiles.Nodes) {
                    if (node.Tag is Profile group && group.IsGroup) {
                        if (group.IsExpanded != expanded) {
                            group.IsExpanded = expanded;
                            changed = true;
                        }
                        if (expanded) {
                            node.Expand();
                        } else {
                            node.Collapse();
                        }
                        UpdateGroupNodeIcon(node, expanded);
                    }
                }
            } finally {
                treeViewProfiles.EndUpdate();
                _suppressTreeEvents = false;
            }

            if (changed) {
                _config.Save();
            }
        }

        private void treeViewProfiles_ItemDrag(object sender, ItemDragEventArgs e) {
            if (EditMode) {
                return;
            }

            if (e.Button == MouseButtons.Left && e.Item is TreeNode draggedNode) {
                treeViewProfiles.SelectedNode = draggedNode;
                DoDragDrop(draggedNode, DragDropEffects.Move);
                treeViewProfiles.ClearDropIndicator();
            }
        }

        private bool ComputeDropTarget(
            TreeNode draggedNode,
            Point clientPoint,
            out TreeNode? targetNode,
            out TreeDropPosition position,
            out bool childIndent) {

            targetNode = null;
            position = TreeDropPosition.None;
            childIndent = false;

            if (!(draggedNode.Tag is Profile draggedProfile)) {
                return false;
            }

            TreeNode? hitNode = treeViewProfiles.GetNodeAt(clientPoint);
            if (hitNode == null) {
                position = TreeDropPosition.AppendToRoot;
                return true;
            }

            // Dragging a group: only allow top-level reordering
            if (draggedProfile.IsGroup) {
                TreeNode rootHit = hitNode.Parent ?? hitNode;
                if (ReferenceEquals(rootHit, draggedNode)) {
                    return false;
                }

                targetNode = rootHit;
                childIndent = false;

                if (hitNode.Parent != null) {
                    position = TreeDropPosition.Below;
                } else {
                    int midY = rootHit.Bounds.Top + rootHit.Bounds.Height / 2;
                    position = clientPoint.Y < midY ? TreeDropPosition.Above : TreeDropPosition.Below;
                }
                return true;
            }

            // Dragging a connection
            if (ReferenceEquals(hitNode, draggedNode)) {
                return false;
            }

            if (hitNode.Tag is Profile hitProfile && hitProfile.IsGroup) {
                var bounds = hitNode.Bounds;
                int edgeZone = Math.Max(5, bounds.Height / 4);

                if (clientPoint.Y < bounds.Top + edgeZone) {
                    targetNode = hitNode;
                    position = TreeDropPosition.Above;
                    childIndent = false;
                } else if (clientPoint.Y > bounds.Bottom - edgeZone) {
                    if (hitNode.IsExpanded && hitNode.Nodes.Count > 0) {
                        targetNode = hitNode.Nodes[0];
                        position = TreeDropPosition.Above;
                        childIndent = true;
                    } else {
                        targetNode = hitNode;
                        position = TreeDropPosition.Below;
                        childIndent = false;
                    }
                } else {
                    targetNode = hitNode;
                    position = TreeDropPosition.Into;
                    childIndent = false;
                }
                return true;
            }

            // Target is another connection (at root or inside a group)
            int mid = hitNode.Bounds.Top + hitNode.Bounds.Height / 2;
            targetNode = hitNode;
            position = clientPoint.Y < mid ? TreeDropPosition.Above : TreeDropPosition.Below;
            childIndent = hitNode.Parent != null;
            return true;
        }

        private void treeViewProfiles_DragOver(object sender, DragEventArgs e) {
            if (EditMode || e.Data == null || !e.Data.GetDataPresent(typeof(TreeNode))) {
                e.Effect = DragDropEffects.None;
                treeViewProfiles.ClearDropIndicator();
                return;
            }

            var draggedNode = e.Data.GetData(typeof(TreeNode)) as TreeNode;
            if (draggedNode == null || draggedNode.TreeView != treeViewProfiles) {
                e.Effect = DragDropEffects.None;
                treeViewProfiles.ClearDropIndicator();
                return;
            }

            Point clientPoint = treeViewProfiles.PointToClient(new Point(e.X, e.Y));
            treeViewProfiles.ScrollIfNeeded(clientPoint);

            if (ComputeDropTarget(draggedNode, clientPoint, out var targetNode, out var position, out var childIndent)) {
                e.Effect = DragDropEffects.Move;
                treeViewProfiles.SetDropIndicator(targetNode, position, childIndent);
            } else {
                e.Effect = DragDropEffects.None;
                treeViewProfiles.ClearDropIndicator();
            }
        }

        private void treeViewProfiles_DragLeave(object sender, EventArgs e) {
            treeViewProfiles.ClearDropIndicator();
        }

        private void treeViewProfiles_DragDrop(object sender, DragEventArgs e) {
            treeViewProfiles.ClearDropIndicator();

            if (EditMode || e.Data == null || !e.Data.GetDataPresent(typeof(TreeNode))) {
                return;
            }

            var draggedNode = e.Data.GetData(typeof(TreeNode)) as TreeNode;
            if (draggedNode == null || draggedNode.TreeView != treeViewProfiles || !(draggedNode.Tag is Profile draggedProfile)) {
                return;
            }

            Point clientPoint = treeViewProfiles.PointToClient(new Point(e.X, e.Y));
            if (!ComputeDropTarget(draggedNode, clientPoint, out var targetNode, out var position, out _)) {
                return;
            }

            var sourceList = FindParentList(draggedProfile, out _);
            if (sourceList == null) {
                return;
            }

            List<Profile>? destList;
            int destIndex;

            if (position == TreeDropPosition.AppendToRoot) {
                destList = _config.Profiles;
                destIndex = _config.Profiles.Count;
            } else if (position == TreeDropPosition.Into && targetNode?.Tag is Profile destGroup && destGroup.IsGroup) {
                destList = destGroup.Children;
                destIndex = destGroup.Children.Count;
                destGroup.IsExpanded = true;
            } else if (targetNode?.Tag is Profile targetProfile) {
                destList = FindParentList(targetProfile, out _);
                if (destList == null) {
                    return;
                }
                int targetIdx = destList.IndexOf(targetProfile);
                if (targetIdx < 0) {
                    return;
                }
                destIndex = position == TreeDropPosition.Above ? targetIdx : targetIdx + 1;
            } else {
                return;
            }

            int sourceIdx = sourceList.IndexOf(draggedProfile);
            if (sourceIdx < 0) {
                return;
            }

            if (ReferenceEquals(sourceList, destList) && (destIndex == sourceIdx || destIndex == sourceIdx + 1)) {
                return;
            }

            sourceList.RemoveAt(sourceIdx);
            if (ReferenceEquals(sourceList, destList) && sourceIdx < destIndex) {
                destIndex--;
            }

            destIndex = Math.Max(0, Math.Min(destIndex, destList.Count));
            destList.Insert(destIndex, draggedProfile);

            _config.Save();
            PopulateTree(draggedProfile);
        }
    }
}
