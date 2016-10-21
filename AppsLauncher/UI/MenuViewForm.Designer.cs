namespace AppsLauncher.UI
{
    partial class MenuViewForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.appMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.appMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.appMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.appMenuItemSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.appMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.appMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.appMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.appMenuItemSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.appMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.appMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.fadeInTimer = new System.Windows.Forms.Timer(this.components);
            this.settingsBtn = new System.Windows.Forms.Button();
            this.closeBtn = new System.Windows.Forms.Button();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.aboutBtn = new System.Windows.Forms.PictureBox();
            this.profileBtn = new System.Windows.Forms.PictureBox();
            this.logoBox = new System.Windows.Forms.PictureBox();
            this.appsCount = new System.Windows.Forms.Label();
            this.title = new System.Windows.Forms.Label();
            this.appsListViewPanel = new System.Windows.Forms.Panel();
            this.appsListView = new System.Windows.Forms.ListView();
            this.sizeGrip = new System.Windows.Forms.Panel();
            this.downloadBtnPanel = new System.Windows.Forms.Panel();
            this.downloadBtn = new System.Windows.Forms.Button();
            this.layoutPanel = new System.Windows.Forms.Panel();
            this.settingsBtnPanel = new System.Windows.Forms.Panel();
            this.appMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aboutBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.profileBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.logoBox)).BeginInit();
            this.appsListViewPanel.SuspendLayout();
            this.downloadBtnPanel.SuspendLayout();
            this.layoutPanel.SuspendLayout();
            this.settingsBtnPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // appMenu
            // 
            this.appMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.appMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.appMenuItem1,
            this.appMenuItem2,
            this.appMenuItemSeparator1,
            this.appMenuItem3,
            this.appMenuItem4,
            this.appMenuItem5,
            this.appMenuItemSeparator2,
            this.appMenuItem6,
            this.appMenuItem7});
            this.appMenu.Name = "addMenu";
            this.appMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.appMenu.Size = new System.Drawing.Size(212, 170);
            this.appMenu.Opening += new System.ComponentModel.CancelEventHandler(this.AppMenu_Opening);
            this.appMenu.Opened += new System.EventHandler(this.AppMenu_Opened);
            this.appMenu.Paint += new System.Windows.Forms.PaintEventHandler(this.AppMenu_Paint);
            this.appMenu.MouseLeave += new System.EventHandler(this.AppMenu_MouseLeave);
            // 
            // appMenuItem1
            // 
            this.appMenuItem1.ForeColor = System.Drawing.Color.Silver;
            this.appMenuItem1.Name = "appMenuItem1";
            this.appMenuItem1.Size = new System.Drawing.Size(211, 22);
            this.appMenuItem1.Text = "Run";
            this.appMenuItem1.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // appMenuItem2
            // 
            this.appMenuItem2.ForeColor = System.Drawing.Color.Silver;
            this.appMenuItem2.Name = "appMenuItem2";
            this.appMenuItem2.Size = new System.Drawing.Size(211, 22);
            this.appMenuItem2.Text = "Run as administrator";
            this.appMenuItem2.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.appMenuItemSeparator1.Name = "toolStripSeparator2";
            this.appMenuItemSeparator1.Size = new System.Drawing.Size(208, 6);
            // 
            // appMenuItem3
            // 
            this.appMenuItem3.ForeColor = System.Drawing.Color.Silver;
            this.appMenuItem3.Name = "appMenuItem3";
            this.appMenuItem3.Size = new System.Drawing.Size(211, 22);
            this.appMenuItem3.Text = "Open app location";
            this.appMenuItem3.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // appMenuItem4
            // 
            this.appMenuItem4.ForeColor = System.Drawing.Color.Silver;
            this.appMenuItem4.Name = "appMenuItem4";
            this.appMenuItem4.Size = new System.Drawing.Size(211, 22);
            this.appMenuItem4.Text = "Create a Desktop Shortcut";
            this.appMenuItem4.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // appMenuItem5
            // 
            this.appMenuItem5.ForeColor = System.Drawing.Color.Silver;
            this.appMenuItem5.Name = "appMenuItem5";
            this.appMenuItem5.Size = new System.Drawing.Size(211, 22);
            this.appMenuItem5.Text = "Pin to Taskbar";
            this.appMenuItem5.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.appMenuItemSeparator2.Name = "toolStripSeparator3";
            this.appMenuItemSeparator2.Size = new System.Drawing.Size(208, 6);
            // 
            // appMenuItem6
            // 
            this.appMenuItem6.ForeColor = System.Drawing.Color.Silver;
            this.appMenuItem6.Name = "appMenuItem6";
            this.appMenuItem6.Size = new System.Drawing.Size(211, 22);
            this.appMenuItem6.Text = "Rename";
            this.appMenuItem6.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // appMenuItem7
            // 
            this.appMenuItem7.ForeColor = System.Drawing.Color.Silver;
            this.appMenuItem7.Name = "appMenuItem7";
            this.appMenuItem7.Size = new System.Drawing.Size(211, 22);
            this.appMenuItem7.Text = "Delete";
            this.appMenuItem7.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // imgList
            // 
            this.imgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imgList.ImageSize = new System.Drawing.Size(16, 16);
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // fadeInTimer
            // 
            this.fadeInTimer.Interval = 1;
            this.fadeInTimer.Tick += new System.EventHandler(this.FadeInTimer_Tick);
            // 
            // settingsBtn
            // 
            this.settingsBtn.BackColor = System.Drawing.SystemColors.ControlDark;
            this.settingsBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsBtn.FlatAppearance.BorderSize = 0;
            this.settingsBtn.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Highlight;
            this.settingsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.settingsBtn.Location = new System.Drawing.Point(0, 0);
            this.settingsBtn.Name = "settingsBtn";
            this.settingsBtn.Size = new System.Drawing.Size(132, 22);
            this.settingsBtn.TabIndex = 3;
            this.settingsBtn.TabStop = false;
            this.settingsBtn.Text = "Setting";
            this.settingsBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.settingsBtn.UseVisualStyleBackColor = false;
            this.settingsBtn.Click += new System.EventHandler(this.OpenNewFormBtn_Click);
            this.settingsBtn.MouseEnter += new System.EventHandler(this.ImageButton_MouseEnterLeave);
            this.settingsBtn.MouseLeave += new System.EventHandler(this.ImageButton_MouseEnterLeave);
            // 
            // closeBtn
            // 
            this.closeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeBtn.BackgroundImage = global::AppsLauncher.Properties.Resources.horizontal_pattern;
            this.closeBtn.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.closeBtn.FlatAppearance.BorderSize = 0;
            this.closeBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkRed;
            this.closeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeBtn.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeBtn.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.closeBtn.Location = new System.Drawing.Point(200, 292);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(21, 21);
            this.closeBtn.TabIndex = 1;
            this.closeBtn.TabStop = false;
            this.closeBtn.Text = "X";
            this.closeBtn.UseVisualStyleBackColor = false;
            this.closeBtn.Visible = false;
            this.closeBtn.Click += new System.EventHandler(this.CloseBtn_Click);
            // 
            // searchBox
            // 
            this.searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.searchBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.searchBox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchBox.Location = new System.Drawing.Point(4, 292);
            this.searchBox.Multiline = true;
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(192, 21);
            this.searchBox.TabIndex = 3;
            this.searchBox.TextChanged += new System.EventHandler(this.SearchBox_TextChanged);
            this.searchBox.Enter += new System.EventHandler(this.SearchBox_Enter);
            this.searchBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchBox_KeyDown);
            this.searchBox.Leave += new System.EventHandler(this.SearchBox_Leave);
            // 
            // aboutBtn
            // 
            this.aboutBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.aboutBtn.BackColor = System.Drawing.Color.Transparent;
            this.aboutBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.aboutBtn.Cursor = System.Windows.Forms.Cursors.Help;
            this.aboutBtn.Location = new System.Drawing.Point(314, 1);
            this.aboutBtn.Name = "aboutBtn";
            this.aboutBtn.Size = new System.Drawing.Size(23, 23);
            this.aboutBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.aboutBtn.TabIndex = 6;
            this.aboutBtn.TabStop = false;
            this.aboutBtn.Click += new System.EventHandler(this.OpenNewFormBtn_Click);
            this.aboutBtn.MouseEnter += new System.EventHandler(this.ImageButton_MouseEnterLeave);
            this.aboutBtn.MouseLeave += new System.EventHandler(this.ImageButton_MouseEnterLeave);
            // 
            // profileBtn
            // 
            this.profileBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.profileBtn.BackColor = System.Drawing.Color.Transparent;
            this.profileBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.profileBtn.Location = new System.Drawing.Point(311, 210);
            this.profileBtn.Name = "profileBtn";
            this.profileBtn.Size = new System.Drawing.Size(20, 20);
            this.profileBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.profileBtn.TabIndex = 8;
            this.profileBtn.TabStop = false;
            this.profileBtn.Click += new System.EventHandler(this.ProfileBtn_Click);
            this.profileBtn.MouseEnter += new System.EventHandler(this.ImageButton_MouseEnterLeave);
            this.profileBtn.MouseLeave += new System.EventHandler(this.ImageButton_MouseEnterLeave);
            // 
            // logoBox
            // 
            this.logoBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.logoBox.BackColor = System.Drawing.Color.Transparent;
            this.logoBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.logoBox.Location = new System.Drawing.Point(199, 33);
            this.logoBox.Name = "logoBox";
            this.logoBox.Size = new System.Drawing.Size(116, 64);
            this.logoBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logoBox.TabIndex = 5;
            this.logoBox.TabStop = false;
            // 
            // appsCount
            // 
            this.appsCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.appsCount.BackColor = System.Drawing.Color.Transparent;
            this.appsCount.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appsCount.ForeColor = System.Drawing.SystemColors.ControlText;
            this.appsCount.Location = new System.Drawing.Point(199, 98);
            this.appsCount.Name = "appsCount";
            this.appsCount.Size = new System.Drawing.Size(116, 13);
            this.appsCount.TabIndex = 2;
            this.appsCount.Text = "0 apps found!";
            this.appsCount.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // title
            // 
            this.title.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.title.AutoSize = true;
            this.title.BackColor = System.Drawing.Color.Transparent;
            this.title.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title.Location = new System.Drawing.Point(202, 9);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(110, 19);
            this.title.TabIndex = 9;
            this.title.Text = "Apps Launcher";
            // 
            // appsListViewPanel
            // 
            this.appsListViewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appsListViewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.appsListViewPanel.Controls.Add(this.appsListView);
            this.appsListViewPanel.Location = new System.Drawing.Point(4, 4);
            this.appsListViewPanel.Name = "appsListViewPanel";
            this.appsListViewPanel.Size = new System.Drawing.Size(192, 284);
            this.appsListViewPanel.TabIndex = 10;
            // 
            // appsListView
            // 
            this.appsListView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.appsListView.BackColor = System.Drawing.SystemColors.Window;
            this.appsListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.appsListView.ContextMenuStrip = this.appMenu;
            this.appsListView.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.appsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appsListView.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appsListView.ForeColor = System.Drawing.SystemColors.WindowText;
            this.appsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.appsListView.HideSelection = false;
            this.appsListView.LabelWrap = false;
            this.appsListView.Location = new System.Drawing.Point(0, 0);
            this.appsListView.MultiSelect = false;
            this.appsListView.Name = "appsListView";
            this.appsListView.ShowGroups = false;
            this.appsListView.Size = new System.Drawing.Size(190, 282);
            this.appsListView.TabIndex = 0;
            this.appsListView.TileSize = new System.Drawing.Size(128, 30);
            this.appsListView.UseCompatibleStateImageBehavior = false;
            this.appsListView.View = System.Windows.Forms.View.List;
            this.appsListView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.AppsListView_AfterLabelEdit);
            this.appsListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AppsListView_KeyDown);
            this.appsListView.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AppsListView_KeyPress);
            this.appsListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.AppsListView_MouseClick);
            this.appsListView.MouseEnter += new System.EventHandler(this.AppsListView_MouseEnter);
            this.appsListView.MouseLeave += new System.EventHandler(this.AppsListView_MouseLeave);
            this.appsListView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AppsListView_MouseMove);
            // 
            // sizeGrip
            // 
            this.sizeGrip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sizeGrip.BackColor = System.Drawing.Color.Transparent;
            this.sizeGrip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.sizeGrip.Location = new System.Drawing.Point(326, 306);
            this.sizeGrip.Name = "sizeGrip";
            this.sizeGrip.Size = new System.Drawing.Size(12, 12);
            this.sizeGrip.TabIndex = 11;
            this.sizeGrip.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SizeGrip_MouseDown);
            this.sizeGrip.MouseEnter += new System.EventHandler(this.SizeGrip_MouseEnter);
            // 
            // downloadBtnPanel
            // 
            this.downloadBtnPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadBtnPanel.BackColor = System.Drawing.Color.Transparent;
            this.downloadBtnPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.downloadBtnPanel.Controls.Add(this.downloadBtn);
            this.downloadBtnPanel.Location = new System.Drawing.Point(200, 236);
            this.downloadBtnPanel.Name = "downloadBtnPanel";
            this.downloadBtnPanel.Size = new System.Drawing.Size(134, 24);
            this.downloadBtnPanel.TabIndex = 2;
            // 
            // downloadBtn
            // 
            this.downloadBtn.BackColor = System.Drawing.SystemColors.ControlDark;
            this.downloadBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.downloadBtn.FlatAppearance.BorderSize = 0;
            this.downloadBtn.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Highlight;
            this.downloadBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.downloadBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.downloadBtn.Location = new System.Drawing.Point(0, 0);
            this.downloadBtn.Name = "downloadBtn";
            this.downloadBtn.Size = new System.Drawing.Size(132, 22);
            this.downloadBtn.TabIndex = 2;
            this.downloadBtn.TabStop = false;
            this.downloadBtn.Text = "Get More";
            this.downloadBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.downloadBtn.UseVisualStyleBackColor = false;
            this.downloadBtn.Click += new System.EventHandler(this.DownloadBtn_Click);
            this.downloadBtn.MouseEnter += new System.EventHandler(this.ImageButton_MouseEnterLeave);
            this.downloadBtn.MouseLeave += new System.EventHandler(this.ImageButton_MouseEnterLeave);
            // 
            // layoutPanel
            // 
            this.layoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.layoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.layoutPanel.Controls.Add(this.settingsBtnPanel);
            this.layoutPanel.Controls.Add(this.downloadBtnPanel);
            this.layoutPanel.Controls.Add(this.sizeGrip);
            this.layoutPanel.Controls.Add(this.appsListViewPanel);
            this.layoutPanel.Controls.Add(this.title);
            this.layoutPanel.Controls.Add(this.appsCount);
            this.layoutPanel.Controls.Add(this.logoBox);
            this.layoutPanel.Controls.Add(this.profileBtn);
            this.layoutPanel.Controls.Add(this.aboutBtn);
            this.layoutPanel.Controls.Add(this.searchBox);
            this.layoutPanel.Controls.Add(this.closeBtn);
            this.layoutPanel.Location = new System.Drawing.Point(1, 1);
            this.layoutPanel.Name = "layoutPanel";
            this.layoutPanel.Size = new System.Drawing.Size(338, 318);
            this.layoutPanel.TabIndex = 9;
            // 
            // settingsBtnPanel
            // 
            this.settingsBtnPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsBtnPanel.BackColor = System.Drawing.Color.Transparent;
            this.settingsBtnPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.settingsBtnPanel.Controls.Add(this.settingsBtn);
            this.settingsBtnPanel.Location = new System.Drawing.Point(200, 264);
            this.settingsBtnPanel.Name = "settingsBtnPanel";
            this.settingsBtnPanel.Size = new System.Drawing.Size(134, 24);
            this.settingsBtnPanel.TabIndex = 3;
            // 
            // MenuViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SlateGray;
            this.ClientSize = new System.Drawing.Size(340, 320);
            this.Controls.Add(this.layoutPanel);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(340, 320);
            this.Name = "MenuViewForm";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Apps Launcher";
            this.TopMost = true;
            this.Deactivate += new System.EventHandler(this.MenuViewForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MenuViewForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MenuViewForm_FormClosed);
            this.Load += new System.EventHandler(this.MenuViewForm_Load);
            this.ResizeBegin += new System.EventHandler(this.MenuViewForm_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.MenuViewForm_ResizeEnd);
            this.Resize += new System.EventHandler(this.MenuViewForm_Resize);
            this.appMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.aboutBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.profileBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.logoBox)).EndInit();
            this.appsListViewPanel.ResumeLayout(false);
            this.downloadBtnPanel.ResumeLayout(false);
            this.layoutPanel.ResumeLayout(false);
            this.layoutPanel.PerformLayout();
            this.settingsBtnPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip appMenu;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem2;
        private System.Windows.Forms.ToolStripSeparator appMenuItemSeparator1;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem4;
        private System.Windows.Forms.ToolStripSeparator appMenuItemSeparator2;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem6;
        private System.Windows.Forms.Timer fadeInTimer;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem5;
        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.Button settingsBtn;
        private System.Windows.Forms.Button closeBtn;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.PictureBox profileBtn;
        private System.Windows.Forms.PictureBox logoBox;
        private System.Windows.Forms.Label appsCount;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.Panel appsListViewPanel;
        private System.Windows.Forms.ListView appsListView;
        private System.Windows.Forms.Panel sizeGrip;
        private System.Windows.Forms.Panel downloadBtnPanel;
        private System.Windows.Forms.Button downloadBtn;
        private System.Windows.Forms.Panel layoutPanel;
        private System.Windows.Forms.Panel settingsBtnPanel;
        private System.Windows.Forms.PictureBox aboutBtn;
    }
}