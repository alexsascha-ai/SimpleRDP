namespace SimpleRDP {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.checkBoxKeepOpening = new System.Windows.Forms.CheckBox();
            this.checkBoxRedirectionWarning = new System.Windows.Forms.CheckBox();
            this.buttonAbout = new System.Windows.Forms.Button();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.textBoxComputer = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonNew = new System.Windows.Forms.Button();
            this.buttonNewGroup = new System.Windows.Forms.Button();
            this.buttonClone = new System.Windows.Forms.Button();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxDomain = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonOptions = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.treeViewProfiles = new SimpleRDP.DraggableTreeView();
            this.imageListTree = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuNewConnection = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNewGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuClone = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuExpandAll = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCollapseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.contextMenuTree.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxKeepOpening
            // 
            this.checkBoxKeepOpening.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxKeepOpening.Location = new System.Drawing.Point(13, 386);
            this.checkBoxKeepOpening.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxKeepOpening.Name = "checkBoxKeepOpening";
            this.checkBoxKeepOpening.Size = new System.Drawing.Size(360, 24);
            this.checkBoxKeepOpening.TabIndex = 1;
            this.checkBoxKeepOpening.Text = "Keep opening SimpleRDP";
            this.checkBoxKeepOpening.UseVisualStyleBackColor = true;
            this.checkBoxKeepOpening.CheckedChanged += new System.EventHandler(this.checkBoxKeepOpening_CheckedChanged);
            // 
            // checkBoxRedirectionWarning
            // 
            this.checkBoxRedirectionWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxRedirectionWarning.Location = new System.Drawing.Point(13, 412);
            this.checkBoxRedirectionWarning.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxRedirectionWarning.Name = "checkBoxRedirectionWarning";
            this.checkBoxRedirectionWarning.Size = new System.Drawing.Size(360, 24);
            this.checkBoxRedirectionWarning.TabIndex = 20;
            this.checkBoxRedirectionWarning.Text = "Classic RDP warning dialog";
            this.checkBoxRedirectionWarning.UseVisualStyleBackColor = true;
            this.checkBoxRedirectionWarning.CheckedChanged += new System.EventHandler(this.checkBoxRedirectionWarning_CheckedChanged);
            // 
            // buttonAbout
            // 
            this.buttonAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAbout.Location = new System.Drawing.Point(532, 398);
            this.buttonAbout.Margin = new System.Windows.Forms.Padding(4);
            this.buttonAbout.Name = "buttonAbout";
            this.buttonAbout.Size = new System.Drawing.Size(100, 28);
            this.buttonAbout.TabIndex = 2;
            this.buttonAbout.Text = "About";
            this.buttonAbout.UseVisualStyleBackColor = true;
            this.buttonAbout.Click += new System.EventHandler(this.buttonAbout_Click);
            // 
            // buttonConnect
            // 
            this.buttonConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonConnect.Location = new System.Drawing.Point(403, 398);
            this.buttonConnect.Margin = new System.Windows.Forms.Padding(4);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(121, 28);
            this.buttonConnect.TabIndex = 3;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // textBoxComputer
            // 
            this.textBoxComputer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxComputer.Location = new System.Drawing.Point(123, 32);
            this.textBoxComputer.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxComputer.Name = "textBoxComputer";
            this.textBoxComputer.Size = new System.Drawing.Size(209, 22);
            this.textBoxComputer.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 36);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 21);
            this.label1.TabIndex = 5;
            this.label1.Text = "Computer:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 73);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 23);
            this.label2.TabIndex = 6;
            this.label2.Text = "User name:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 110);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 23);
            this.label3.TabIndex = 7;
            this.label3.Text = "Password:";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 146);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(108, 23);
            this.label4.TabIndex = 8;
            this.label4.Text = "Domain:";
            // 
            // buttonNew
            // 
            this.buttonNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonNew.Location = new System.Drawing.Point(13, 351);
            this.buttonNew.Margin = new System.Windows.Forms.Padding(4);
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new System.Drawing.Size(56, 28);
            this.buttonNew.TabIndex = 9;
            this.buttonNew.Text = "New";
            this.buttonNew.UseVisualStyleBackColor = true;
            this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
            // 
            // buttonNewGroup
            // 
            this.buttonNewGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonNewGroup.Location = new System.Drawing.Point(72, 351);
            this.buttonNewGroup.Margin = new System.Windows.Forms.Padding(4);
            this.buttonNewGroup.Name = "buttonNewGroup";
            this.buttonNewGroup.Size = new System.Drawing.Size(63, 28);
            this.buttonNewGroup.TabIndex = 10;
            this.buttonNewGroup.Text = "Group";
            this.buttonNewGroup.UseVisualStyleBackColor = true;
            this.buttonNewGroup.Click += new System.EventHandler(this.buttonNewGroup_Click);
            // 
            // buttonClone
            // 
            this.buttonClone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonClone.Location = new System.Drawing.Point(138, 351);
            this.buttonClone.Margin = new System.Windows.Forms.Padding(4);
            this.buttonClone.Name = "buttonClone";
            this.buttonClone.Size = new System.Drawing.Size(62, 28);
            this.buttonClone.TabIndex = 11;
            this.buttonClone.Text = "Clone";
            this.buttonClone.UseVisualStyleBackColor = true;
            this.buttonClone.Click += new System.EventHandler(this.buttonClone_Click);
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxUsername.Location = new System.Drawing.Point(123, 69);
            this.textBoxUsername.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(209, 22);
            this.textBoxUsername.TabIndex = 10;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPassword.Location = new System.Drawing.Point(124, 106);
            this.textBoxPassword.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(208, 22);
            this.textBoxPassword.TabIndex = 11;
            this.textBoxPassword.UseSystemPasswordChar = true;
            // 
            // textBoxDomain
            // 
            this.textBoxDomain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDomain.Location = new System.Drawing.Point(124, 143);
            this.textBoxDomain.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxDomain.Name = "textBoxDomain";
            this.textBoxDomain.Size = new System.Drawing.Size(208, 22);
            this.textBoxDomain.TabIndex = 12;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBoxDomain);
            this.groupBox1.Controls.Add(this.textBoxComputer);
            this.groupBox1.Controls.Add(this.textBoxPassword);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBoxUsername);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(279, 96);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(353, 190);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection";
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(277, 294);
            this.buttonSave.Margin = new System.Windows.Forms.Padding(4);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(100, 28);
            this.buttonSave.TabIndex = 14;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDelete.Location = new System.Drawing.Point(203, 351);
            this.buttonDelete.Margin = new System.Windows.Forms.Padding(4);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(66, 28);
            this.buttonDelete.TabIndex = 15;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonOptions
            // 
            this.buttonOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOptions.Location = new System.Drawing.Point(532, 294);
            this.buttonOptions.Margin = new System.Windows.Forms.Padding(4);
            this.buttonOptions.Name = "buttonOptions";
            this.buttonOptions.Size = new System.Drawing.Size(100, 28);
            this.buttonOptions.TabIndex = 16;
            this.buttonOptions.Text = "Options";
            this.buttonOptions.UseVisualStyleBackColor = true;
            this.buttonOptions.Click += new System.EventHandler(this.buttonMoreOptions_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = global::SimpleRDP.Properties.Resources.banner;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(645, 48);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 17;
            this.pictureBox1.TabStop = false;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(385, 294);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 28);
            this.buttonCancel.TabIndex = 18;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Location = new System.Drawing.Point(277, 294);
            this.buttonEdit.Margin = new System.Windows.Forms.Padding(4);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(100, 28);
            this.buttonEdit.TabIndex = 19;
            this.buttonEdit.Text = "Edit";
            this.buttonEdit.UseVisualStyleBackColor = true;
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxName.Location = new System.Drawing.Point(279, 62);
            this.textBoxName.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(353, 22);
            this.textBoxName.TabIndex = 13;
            // 
            // imageListTree
            // 
            this.imageListTree.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageListTree.ImageSize = new System.Drawing.Size(20, 20);
            this.imageListTree.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // contextMenuTree
            // 
            this.contextMenuTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuNewConnection,
            this.menuNewGroup,
            this.menuEdit,
            this.menuClone,
            this.menuDelete,
            this.menuSeparator1,
            this.menuExpandAll,
            this.menuCollapseAll});
            this.contextMenuTree.Name = "contextMenuTree";
            this.contextMenuTree.Size = new System.Drawing.Size(166, 164);
            this.contextMenuTree.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuTree_Opening);
            // 
            // menuNewConnection
            // 
            this.menuNewConnection.Name = "menuNewConnection";
            this.menuNewConnection.Size = new System.Drawing.Size(165, 22);
            this.menuNewConnection.Text = "New Connection";
            this.menuNewConnection.Click += new System.EventHandler(this.buttonNew_Click);
            // 
            // menuNewGroup
            // 
            this.menuNewGroup.Name = "menuNewGroup";
            this.menuNewGroup.Size = new System.Drawing.Size(165, 22);
            this.menuNewGroup.Text = "New Group";
            this.menuNewGroup.Click += new System.EventHandler(this.buttonNewGroup_Click);
            // 
            // menuEdit
            // 
            this.menuEdit.Name = "menuEdit";
            this.menuEdit.Size = new System.Drawing.Size(165, 22);
            this.menuEdit.Text = "Edit / Rename";
            this.menuEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // menuClone
            // 
            this.menuClone.Name = "menuClone";
            this.menuClone.Size = new System.Drawing.Size(165, 22);
            this.menuClone.Text = "Clone";
            this.menuClone.Click += new System.EventHandler(this.buttonClone_Click);
            // 
            // menuDelete
            // 
            this.menuDelete.Name = "menuDelete";
            this.menuDelete.Size = new System.Drawing.Size(165, 22);
            this.menuDelete.Text = "Delete";
            this.menuDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // menuSeparator1
            // 
            this.menuSeparator1.Name = "menuSeparator1";
            this.menuSeparator1.Size = new System.Drawing.Size(162, 6);
            // 
            // menuExpandAll
            // 
            this.menuExpandAll.Name = "menuExpandAll";
            this.menuExpandAll.Size = new System.Drawing.Size(165, 22);
            this.menuExpandAll.Text = "Expand All";
            this.menuExpandAll.Click += new System.EventHandler(this.menuExpandAll_Click);
            // 
            // menuCollapseAll
            // 
            this.menuCollapseAll.Name = "menuCollapseAll";
            this.menuCollapseAll.Size = new System.Drawing.Size(165, 22);
            this.menuCollapseAll.Text = "Collapse All";
            this.menuCollapseAll.Click += new System.EventHandler(this.menuCollapseAll_Click);
            // 
            // treeViewProfiles
            // 
            this.treeViewProfiles.AllowDrop = true;
            this.treeViewProfiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left)));
            this.treeViewProfiles.ContextMenuStrip = this.contextMenuTree;
            this.treeViewProfiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeViewProfiles.FullRowSelect = true;
            this.treeViewProfiles.HideSelection = false;
            this.treeViewProfiles.ImageIndex = 0;
            this.treeViewProfiles.ImageList = this.imageListTree;
            this.treeViewProfiles.Indent = 19;
            this.treeViewProfiles.ItemHeight = 28;
            this.treeViewProfiles.Location = new System.Drawing.Point(13, 62);
            this.treeViewProfiles.Margin = new System.Windows.Forms.Padding(4);
            this.treeViewProfiles.Name = "treeViewProfiles";
            this.treeViewProfiles.SelectedImageIndex = 0;
            this.treeViewProfiles.ShowLines = false;
            this.treeViewProfiles.ShowPlusMinus = true;
            this.treeViewProfiles.ShowRootLines = true;
            this.treeViewProfiles.Size = new System.Drawing.Size(256, 281);
            this.treeViewProfiles.TabIndex = 0;
            this.treeViewProfiles.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeViewProfiles_AfterCollapse);
            this.treeViewProfiles.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeViewProfiles_AfterExpand);
            this.treeViewProfiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewProfiles_AfterSelect);
            this.treeViewProfiles.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeViewProfiles_ItemDrag);
            this.treeViewProfiles.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewProfiles_NodeMouseClick);
            this.treeViewProfiles.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewProfiles_NodeMouseDoubleClick);
            this.treeViewProfiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeViewProfiles_DragDrop);
            this.treeViewProfiles.DragLeave += new System.EventHandler(this.treeViewProfiles_DragLeave);
            this.treeViewProfiles.DragOver += new System.Windows.Forms.DragEventHandler(this.treeViewProfiles_DragOver);
            this.treeViewProfiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeViewProfiles_KeyDown);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(645, 442);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.buttonEdit);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.buttonOptions);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonClone);
            this.Controls.Add(this.buttonNewGroup);
            this.Controls.Add(this.buttonNew);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.buttonAbout);
            this.Controls.Add(this.checkBoxRedirectionWarning);
            this.Controls.Add(this.checkBoxKeepOpening);
            this.Controls.Add(this.treeViewProfiles);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(15, 15);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(663, 5528);
            this.MinimumSize = new System.Drawing.Size(663, 489);
            this.Name = "MainForm";
            this.Text = "SimpleRDP";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.contextMenuTree.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox textBoxName;

        private System.Windows.Forms.Button buttonEdit;

        private SimpleRDP.DraggableTreeView treeViewProfiles;
        private System.Windows.Forms.ImageList imageListTree;
        private System.Windows.Forms.ContextMenuStrip contextMenuTree;
        private System.Windows.Forms.ToolStripMenuItem menuNewConnection;
        private System.Windows.Forms.ToolStripMenuItem menuNewGroup;
        private System.Windows.Forms.ToolStripMenuItem menuEdit;
        private System.Windows.Forms.ToolStripMenuItem menuClone;
        private System.Windows.Forms.ToolStripMenuItem menuDelete;
        private System.Windows.Forms.ToolStripSeparator menuSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuExpandAll;
        private System.Windows.Forms.ToolStripMenuItem menuCollapseAll;

        private System.Windows.Forms.Button buttonCancel;

        private System.Windows.Forms.PictureBox pictureBox1;

        private System.Windows.Forms.Button buttonOptions;

        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonDelete;

        private System.Windows.Forms.GroupBox groupBox1;

        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxDomain;

        private System.Windows.Forms.Button buttonNew;
        private System.Windows.Forms.Button buttonNewGroup;
        private System.Windows.Forms.Button buttonClone;

        private System.Windows.Forms.TextBox textBoxComputer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;

        private System.Windows.Forms.Button buttonConnect;

        private System.Windows.Forms.Button buttonAbout;

        private System.Windows.Forms.CheckBox checkBoxKeepOpening;
        private System.Windows.Forms.CheckBox checkBoxRedirectionWarning;

        #endregion
    }
}
