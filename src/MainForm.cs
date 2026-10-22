using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace SimpleRDP {
    public partial class MainForm : Form {

        private Config _config;
        private bool _editMode = false;
        private Profile selectedProfile = null;
        private int _initialHeight;
        private bool _suppressRedirectionWarningEvent = false;
        private const string RdpPolicySubKey = @"SOFTWARE\Policies\Microsoft\Windows NT\Terminal Services\Client";
        private const string RedirectionWarningValueName = "RedirectionWarningDialogVersion";

        public MainForm() {
            InitializeComponent();
            _config = Config.GetConfig();
        }
        
        private void MainForm_Load(object sender, EventArgs e) {
            if (_config.FormHeight >= this.MinimumSize.Height) {
                this.Height = _config.FormHeight;
            }
            _initialHeight = this.Height;

            listBox.DataSource = _config.Profiles;

            if (_config.Profiles.Count == 0) {
                AddNewProfile();
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

        public bool EditMode {
            get => _editMode;
            set {
                buttonEdit.Visible = !value;
                buttonSave.Visible = value;
                buttonCancel.Visible = value;
                buttonOptions.Enabled = !value;

                buttonConnect.Enabled = !value;

                textBoxName.Enabled = value;
                textBoxComputer.Enabled = value;
                textBoxUsername.Enabled = value;
                textBoxPassword.Enabled = value;
                textBoxDomain.Enabled = value;
            }
        }

        private void AddNewProfile() {
            var profile = new Profile();
            profile.JustAdded = true;
            _config.Profiles.Add(profile);
            listBox.SelectedIndex = _config.Profiles.Count - 1;
        }

        private void buttonMoreOptions_Click(object sender, EventArgs e) {
            ProcessStartInfo startInfo = new ProcessStartInfo {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = "mstsc.exe",
                Arguments = "/edit " + GetSelectedProfile().Filename,
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

            if (String.IsNullOrWhiteSpace(profile.Computer) || String.IsNullOrWhiteSpace(profile.Computer)) {
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

        private void listBox_SelectedValueChanged(object sender, EventArgs e) {
            SelectProfile();
        }

        private Profile GetSelectedProfile() {
            return (Profile) listBox.SelectedItem;
        }

        private void SelectProfile(bool force = false) {
            var profile = (Profile) listBox.SelectedItem;

            // Avoid click empty area reset value
            if (profile == selectedProfile && !force) {
                return;
            }

            selectedProfile = profile;

            EditMode = profile.JustAdded;

            textBoxName.Text = profile.Name ;
            textBoxComputer.Text = profile.Computer;
            textBoxUsername.Text = profile.Username ;
            textBoxPassword.Text = profile.Password;
            textBoxDomain.Text = profile.Domain;
        }

        private void buttonEdit_Click(object sender, EventArgs e) {
            EditMode = true;
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            EditMode = false;

            var profile = GetSelectedProfile();

            if (profile.JustAdded && _config.Profiles.Count > 1) {
                buttonDelete_Click(null, null);
            } else {
                SelectProfile(true);
            }
        }

        private void buttonNew_Click(object sender, EventArgs e) {
            AddNewProfile();
        }

        private void buttonDelete_Click(object sender, EventArgs e) {
            // show confirm dialog
            var confirmResult = MessageBox.Show(
                "Are you sure to delete this profile?",
                "Confirm",
                MessageBoxButtons.YesNo);

            // if confirm delete
            if (confirmResult == DialogResult.Yes) {
                var selectedItems = (Profile) listBox.SelectedItem;
                selectedItems.Delete();
                _config.Profiles.Remove(selectedItems);
                _config.Save();

                if (_config.Profiles.Count == 0) {
                    AddNewProfile();
                    SelectProfile(true);
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e) {
            var profile = (Profile) listBox.SelectedItem;

            profile.JustAdded = false;

            profile.Name = textBoxName.Text;
            profile.Computer = textBoxComputer.Text;
            profile.Username = textBoxUsername.Text;
            profile.Password = textBoxPassword.Text;
            profile.Domain = textBoxDomain.Text;

            profile.PrepareRdpFile();

            _config.Save();
            EditMode = false;

            // Refresh the list
            listBox.DisplayMember = null;
            listBox.DisplayMember = "Name";
        }

        private void checkBoxKeepOpening_CheckedChanged(object sender, EventArgs e) {
            _config.KeepOpening = checkBoxKeepOpening.Checked;
            _config.Save();
        }

        private void buttonAbout_Click(object sender, EventArgs e) {
            About about = new About();
            about.ShowDialog(this);
        }

        private void listBox_MouseDoubleClick(object sender, MouseEventArgs e) {
            buttonConnect_Click(sender, e);
        }

        /**
         * From https://stackoverflow.com/questions/8333282/how-can-i-include-icons-in-my-listbox
         */
        private void listBox_DrawItem(object sender, DrawItemEventArgs e) {
            if (e.Index == -1)
                return;

            e.DrawBackground();
            Brush myBrush = Brushes.Black;


            var iconWidth = listBox.ItemHeight;
            var iconMargin = 4;
            var textMargin = (iconWidth - 18) / 2;
            var rect = new Rectangle(e.Bounds.X + iconMargin, e.Bounds.Y, iconWidth, iconWidth);
            //assuming the icon is already added to project resources

            e.Graphics.DrawIcon(SimpleRDP.Properties.Resources.icon, rect);

            var profile = (Profile)listBox.Items[e.Index];

            e.Graphics.DrawString(
                profile.Name,
                e.Font,
                myBrush,
                new Rectangle(e.Bounds.X + iconMargin * 2 + iconWidth, e.Bounds.Y + textMargin, e.Bounds.Width, e.Bounds.Height),
                StringFormat.GenericDefault
            );

            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();
        }

    }
}
