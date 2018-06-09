namespace AppsDownloader.Windows
{
    sealed partial class MainForm
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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("listViewGroup0", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("listViewGroup1", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("listViewGroup2", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("listViewGroup3", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("listViewGroup4", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup6 = new System.Windows.Forms.ListViewGroup("listViewGroup5", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup7 = new System.Windows.Forms.ListViewGroup("listViewGroup6", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup8 = new System.Windows.Forms.ListViewGroup("listViewGroup7", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup9 = new System.Windows.Forms.ListViewGroup("listViewGroup8", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup10 = new System.Windows.Forms.ListViewGroup("listViewGroup9", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup11 = new System.Windows.Forms.ListViewGroup("listViewGroup10", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup12 = new System.Windows.Forms.ListViewGroup("listViewGroup11", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup13 = new System.Windows.Forms.ListViewGroup("listViewGroup12", System.Windows.Forms.HorizontalAlignment.Left);
            this.appStatus = new System.Windows.Forms.Label();
            this.downloadReceivedLabel = new System.Windows.Forms.Label();
            this.urlStatus = new System.Windows.Forms.Label();
            this.appStatusLabel = new System.Windows.Forms.Label();
            this.fileStatusLabel = new System.Windows.Forms.Label();
            this.settingsAreaBorder = new System.Windows.Forms.Panel();
            this.settingsArea = new System.Windows.Forms.Panel();
            this.highlightInstalledCheck = new System.Windows.Forms.CheckBox();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.showColorsCheck = new System.Windows.Forms.CheckBox();
            this.showGroupsCheck = new System.Windows.Forms.CheckBox();
            this.buttonAreaBorder = new System.Windows.Forms.Panel();
            this.urlStatusLabel = new System.Windows.Forms.Label();
            this.timeStatusLabel = new System.Windows.Forms.Label();
            this.buttonAreaPanel = new System.Windows.Forms.Panel();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.startBtn = new System.Windows.Forms.Button();
            this.timeStatus = new System.Windows.Forms.Label();
            this.statusAreaLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.statusAreaRightPanel = new System.Windows.Forms.TableLayoutPanel();
            this.downloadReceived = new System.Windows.Forms.Label();
            this.downloadSpeed = new System.Windows.Forms.Label();
            this.downloadSpeedLabel = new System.Windows.Forms.Label();
            this.statusAreaLeftPanel = new System.Windows.Forms.TableLayoutPanel();
            this.fileStatus = new System.Windows.Forms.Label();
            this.statusAreaBorder = new System.Windows.Forms.Panel();
            this.downloadProgress = new System.Windows.Forms.Panel();
            this.downloadHandler = new System.Windows.Forms.Timer(this.components);
            this.downloadStarter = new System.Windows.Forms.Timer(this.components);
            this.statusAreaPanel = new System.Windows.Forms.Panel();
            this.appsList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.appMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.appMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.appMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.appMenuItemSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.appMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.appMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.searchResultBlinker = new System.Windows.Forms.Timer(this.components);
            this.settingsArea.SuspendLayout();
            this.buttonAreaPanel.SuspendLayout();
            this.statusAreaLayoutPanel.SuspendLayout();
            this.statusAreaRightPanel.SuspendLayout();
            this.statusAreaLeftPanel.SuspendLayout();
            this.statusAreaPanel.SuspendLayout();
            this.appMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // appStatus
            // 
            this.appStatus.BackColor = System.Drawing.Color.Transparent;
            this.appStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appStatus.ForeColor = System.Drawing.Color.PaleTurquoise;
            this.appStatus.Location = new System.Drawing.Point(130, 1);
            this.appStatus.Name = "appStatus";
            this.appStatus.Size = new System.Drawing.Size(232, 20);
            this.appStatus.TabIndex = 0;
            this.appStatus.Text = "Example";
            this.appStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // downloadReceivedLabel
            // 
            this.downloadReceivedLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.downloadReceivedLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.downloadReceivedLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.downloadReceivedLabel.Location = new System.Drawing.Point(4, 1);
            this.downloadReceivedLabel.Name = "downloadReceivedLabel";
            this.downloadReceivedLabel.Size = new System.Drawing.Size(119, 20);
            this.downloadReceivedLabel.TabIndex = 0;
            this.downloadReceivedLabel.Text = "Downloaded:";
            this.downloadReceivedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // urlStatus
            // 
            this.urlStatus.BackColor = System.Drawing.Color.Transparent;
            this.urlStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.urlStatus.ForeColor = System.Drawing.Color.PaleTurquoise;
            this.urlStatus.Location = new System.Drawing.Point(130, 43);
            this.urlStatus.Name = "urlStatus";
            this.urlStatus.Size = new System.Drawing.Size(232, 20);
            this.urlStatus.TabIndex = 0;
            this.urlStatus.Text = "example.com";
            this.urlStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // appStatusLabel
            // 
            this.appStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appStatusLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.appStatusLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.appStatusLabel.Location = new System.Drawing.Point(4, 1);
            this.appStatusLabel.Name = "appStatusLabel";
            this.appStatusLabel.Size = new System.Drawing.Size(119, 20);
            this.appStatusLabel.TabIndex = 0;
            this.appStatusLabel.Text = "Application:";
            this.appStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fileStatusLabel
            // 
            this.fileStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileStatusLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.fileStatusLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.fileStatusLabel.Location = new System.Drawing.Point(4, 22);
            this.fileStatusLabel.Name = "fileStatusLabel";
            this.fileStatusLabel.Size = new System.Drawing.Size(119, 20);
            this.fileStatusLabel.TabIndex = 0;
            this.fileStatusLabel.Text = "File:";
            this.fileStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // settingsAreaBorder
            // 
            this.settingsAreaBorder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.settingsAreaBorder.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.settingsAreaBorder.Location = new System.Drawing.Point(0, 503);
            this.settingsAreaBorder.Name = "settingsAreaBorder";
            this.settingsAreaBorder.Size = new System.Drawing.Size(744, 1);
            this.settingsAreaBorder.TabIndex = 0;
            // 
            // settingsArea
            // 
            this.settingsArea.BackColor = System.Drawing.Color.Transparent;
            this.settingsArea.Controls.Add(this.highlightInstalledCheck);
            this.settingsArea.Controls.Add(this.searchBox);
            this.settingsArea.Controls.Add(this.showColorsCheck);
            this.settingsArea.Controls.Add(this.showGroupsCheck);
            this.settingsArea.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.settingsArea.Location = new System.Drawing.Point(0, 504);
            this.settingsArea.Name = "settingsArea";
            this.settingsArea.Size = new System.Drawing.Size(744, 32);
            this.settingsArea.TabIndex = 0;
            // 
            // highlightInstalledCheck
            // 
            this.highlightInstalledCheck.AutoSize = true;
            this.highlightInstalledCheck.Checked = true;
            this.highlightInstalledCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.highlightInstalledCheck.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.highlightInstalledCheck.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.highlightInstalledCheck.Location = new System.Drawing.Point(313, 7);
            this.highlightInstalledCheck.Name = "highlightInstalledCheck";
            this.highlightInstalledCheck.Size = new System.Drawing.Size(127, 19);
            this.highlightInstalledCheck.TabIndex = 3;
            this.highlightInstalledCheck.Text = "Highlight Installed";
            this.highlightInstalledCheck.UseVisualStyleBackColor = true;
            this.highlightInstalledCheck.CheckedChanged += new System.EventHandler(this.HighlightInstalledCheck_CheckedChanged);
            // 
            // searchBox
            // 
            this.searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchBox.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.searchBox.Location = new System.Drawing.Point(517, 5);
            this.searchBox.Multiline = true;
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(218, 22);
            this.searchBox.TabIndex = 4;
            this.searchBox.TextChanged += new System.EventHandler(this.SearchBox_TextChanged);
            this.searchBox.Enter += new System.EventHandler(this.SearchBox_Enter);
            this.searchBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchBox_KeyDown);
            // 
            // showColorsCheck
            // 
            this.showColorsCheck.AutoSize = true;
            this.showColorsCheck.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showColorsCheck.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.showColorsCheck.Location = new System.Drawing.Point(150, 7);
            this.showColorsCheck.Name = "showColorsCheck";
            this.showColorsCheck.Size = new System.Drawing.Size(132, 19);
            this.showColorsCheck.TabIndex = 2;
            this.showColorsCheck.Text = "Show Group Colors";
            this.showColorsCheck.UseVisualStyleBackColor = true;
            this.showColorsCheck.CheckedChanged += new System.EventHandler(this.ShowColorsCheck_CheckedChanged);
            // 
            // showGroupsCheck
            // 
            this.showGroupsCheck.AutoSize = true;
            this.showGroupsCheck.Checked = true;
            this.showGroupsCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showGroupsCheck.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showGroupsCheck.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.showGroupsCheck.Location = new System.Drawing.Point(14, 7);
            this.showGroupsCheck.Name = "showGroupsCheck";
            this.showGroupsCheck.Size = new System.Drawing.Size(100, 19);
            this.showGroupsCheck.TabIndex = 1;
            this.showGroupsCheck.Text = "Show Groups";
            this.showGroupsCheck.UseVisualStyleBackColor = true;
            this.showGroupsCheck.CheckedChanged += new System.EventHandler(this.ShowGroupsCheck_CheckedChanged);
            // 
            // buttonAreaBorder
            // 
            this.buttonAreaBorder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.buttonAreaBorder.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonAreaBorder.Location = new System.Drawing.Point(0, 536);
            this.buttonAreaBorder.Name = "buttonAreaBorder";
            this.buttonAreaBorder.Size = new System.Drawing.Size(744, 1);
            this.buttonAreaBorder.TabIndex = 0;
            // 
            // urlStatusLabel
            // 
            this.urlStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.urlStatusLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.urlStatusLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.urlStatusLabel.Location = new System.Drawing.Point(4, 43);
            this.urlStatusLabel.Name = "urlStatusLabel";
            this.urlStatusLabel.Size = new System.Drawing.Size(119, 20);
            this.urlStatusLabel.TabIndex = 0;
            this.urlStatusLabel.Text = "Source:";
            this.urlStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timeStatusLabel
            // 
            this.timeStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeStatusLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.timeStatusLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.timeStatusLabel.Location = new System.Drawing.Point(4, 43);
            this.timeStatusLabel.Name = "timeStatusLabel";
            this.timeStatusLabel.Size = new System.Drawing.Size(119, 20);
            this.timeStatusLabel.TabIndex = 0;
            this.timeStatusLabel.Text = "Time Elapsed:";
            this.timeStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonAreaPanel
            // 
            this.buttonAreaPanel.BackgroundImage = global::AppsDownloader.Properties.Resources.diagonal_pattern;
            this.buttonAreaPanel.Controls.Add(this.cancelBtn);
            this.buttonAreaPanel.Controls.Add(this.startBtn);
            this.buttonAreaPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonAreaPanel.Location = new System.Drawing.Point(0, 537);
            this.buttonAreaPanel.Name = "buttonAreaPanel";
            this.buttonAreaPanel.Size = new System.Drawing.Size(744, 48);
            this.buttonAreaPanel.TabIndex = 0;
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.BackColor = System.Drawing.SystemColors.Control;
            this.cancelBtn.Location = new System.Drawing.Point(651, 12);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 24);
            this.cancelBtn.TabIndex = 101;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = false;
            this.cancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // startBtn
            // 
            this.startBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.startBtn.BackColor = System.Drawing.SystemColors.Control;
            this.startBtn.Enabled = false;
            this.startBtn.Location = new System.Drawing.Point(559, 12);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(75, 24);
            this.startBtn.TabIndex = 100;
            this.startBtn.Text = "OK";
            this.startBtn.UseVisualStyleBackColor = false;
            this.startBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // timeStatus
            // 
            this.timeStatus.BackColor = System.Drawing.Color.Transparent;
            this.timeStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeStatus.ForeColor = System.Drawing.Color.PaleTurquoise;
            this.timeStatus.Location = new System.Drawing.Point(130, 43);
            this.timeStatus.Name = "timeStatus";
            this.timeStatus.Size = new System.Drawing.Size(232, 20);
            this.timeStatus.TabIndex = 0;
            this.timeStatus.Text = "00:00.000";
            this.timeStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusAreaLayoutPanel
            // 
            this.statusAreaLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.statusAreaLayoutPanel.ColumnCount = 2;
            this.statusAreaLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.statusAreaLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.statusAreaLayoutPanel.Controls.Add(this.statusAreaRightPanel, 1, 0);
            this.statusAreaLayoutPanel.Controls.Add(this.statusAreaLeftPanel, 0, 0);
            this.statusAreaLayoutPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusAreaLayoutPanel.Location = new System.Drawing.Point(0, 5);
            this.statusAreaLayoutPanel.Name = "statusAreaLayoutPanel";
            this.statusAreaLayoutPanel.RowCount = 1;
            this.statusAreaLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.statusAreaLayoutPanel.Size = new System.Drawing.Size(744, 70);
            this.statusAreaLayoutPanel.TabIndex = 0;
            // 
            // statusAreaRightPanel
            // 
            this.statusAreaRightPanel.BackColor = System.Drawing.Color.Transparent;
            this.statusAreaRightPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.statusAreaRightPanel.ColumnCount = 2;
            this.statusAreaRightPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.statusAreaRightPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.statusAreaRightPanel.Controls.Add(this.timeStatus, 1, 2);
            this.statusAreaRightPanel.Controls.Add(this.timeStatusLabel, 0, 2);
            this.statusAreaRightPanel.Controls.Add(this.downloadReceived, 1, 0);
            this.statusAreaRightPanel.Controls.Add(this.downloadSpeed, 1, 1);
            this.statusAreaRightPanel.Controls.Add(this.downloadReceivedLabel, 0, 0);
            this.statusAreaRightPanel.Controls.Add(this.downloadSpeedLabel, 0, 1);
            this.statusAreaRightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusAreaRightPanel.Location = new System.Drawing.Point(375, 3);
            this.statusAreaRightPanel.Name = "statusAreaRightPanel";
            this.statusAreaRightPanel.RowCount = 3;
            this.statusAreaRightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.statusAreaRightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.statusAreaRightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.statusAreaRightPanel.Size = new System.Drawing.Size(366, 64);
            this.statusAreaRightPanel.TabIndex = 0;
            // 
            // downloadReceived
            // 
            this.downloadReceived.BackColor = System.Drawing.Color.Transparent;
            this.downloadReceived.Dock = System.Windows.Forms.DockStyle.Fill;
            this.downloadReceived.ForeColor = System.Drawing.Color.PaleTurquoise;
            this.downloadReceived.Location = new System.Drawing.Point(130, 1);
            this.downloadReceived.Name = "downloadReceived";
            this.downloadReceived.Size = new System.Drawing.Size(232, 20);
            this.downloadReceived.TabIndex = 0;
            this.downloadReceived.Text = "0.00 bytes / 0.00 bytes";
            this.downloadReceived.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // downloadSpeed
            // 
            this.downloadSpeed.BackColor = System.Drawing.Color.Transparent;
            this.downloadSpeed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.downloadSpeed.ForeColor = System.Drawing.Color.PaleTurquoise;
            this.downloadSpeed.Location = new System.Drawing.Point(130, 22);
            this.downloadSpeed.Name = "downloadSpeed";
            this.downloadSpeed.Size = new System.Drawing.Size(232, 20);
            this.downloadSpeed.TabIndex = 0;
            this.downloadSpeed.Text = "0.00 bit/s";
            this.downloadSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // downloadSpeedLabel
            // 
            this.downloadSpeedLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.downloadSpeedLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.downloadSpeedLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.downloadSpeedLabel.Location = new System.Drawing.Point(4, 22);
            this.downloadSpeedLabel.Name = "downloadSpeedLabel";
            this.downloadSpeedLabel.Size = new System.Drawing.Size(119, 20);
            this.downloadSpeedLabel.TabIndex = 0;
            this.downloadSpeedLabel.Text = "Speed:";
            this.downloadSpeedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusAreaLeftPanel
            // 
            this.statusAreaLeftPanel.BackColor = System.Drawing.Color.Transparent;
            this.statusAreaLeftPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.statusAreaLeftPanel.ColumnCount = 2;
            this.statusAreaLeftPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.statusAreaLeftPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.statusAreaLeftPanel.Controls.Add(this.urlStatusLabel, 0, 2);
            this.statusAreaLeftPanel.Controls.Add(this.urlStatus, 0, 2);
            this.statusAreaLeftPanel.Controls.Add(this.appStatus, 1, 0);
            this.statusAreaLeftPanel.Controls.Add(this.fileStatus, 1, 1);
            this.statusAreaLeftPanel.Controls.Add(this.appStatusLabel, 0, 0);
            this.statusAreaLeftPanel.Controls.Add(this.fileStatusLabel, 0, 1);
            this.statusAreaLeftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusAreaLeftPanel.Location = new System.Drawing.Point(3, 3);
            this.statusAreaLeftPanel.Name = "statusAreaLeftPanel";
            this.statusAreaLeftPanel.RowCount = 3;
            this.statusAreaLeftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.statusAreaLeftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.statusAreaLeftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.statusAreaLeftPanel.Size = new System.Drawing.Size(366, 64);
            this.statusAreaLeftPanel.TabIndex = 0;
            // 
            // fileStatus
            // 
            this.fileStatus.BackColor = System.Drawing.Color.Transparent;
            this.fileStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileStatus.ForeColor = System.Drawing.Color.PaleTurquoise;
            this.fileStatus.Location = new System.Drawing.Point(130, 22);
            this.fileStatus.Name = "fileStatus";
            this.fileStatus.Size = new System.Drawing.Size(232, 20);
            this.fileStatus.TabIndex = 0;
            this.fileStatus.Text = "example.7z";
            this.fileStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusAreaBorder
            // 
            this.statusAreaBorder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.statusAreaBorder.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusAreaBorder.Location = new System.Drawing.Point(0, 585);
            this.statusAreaBorder.Name = "statusAreaBorder";
            this.statusAreaBorder.Size = new System.Drawing.Size(744, 1);
            this.statusAreaBorder.TabIndex = 0;
            // 
            // downloadProgress
            // 
            this.downloadProgress.BackColor = System.Drawing.Color.Gainsboro;
            this.downloadProgress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.downloadProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.downloadProgress.Location = new System.Drawing.Point(0, 0);
            this.downloadProgress.Name = "downloadProgress";
            this.downloadProgress.Size = new System.Drawing.Size(744, 5);
            this.downloadProgress.TabIndex = 0;
            // 
            // downloadHandler
            // 
            this.downloadHandler.Interval = 10;
            this.downloadHandler.Tick += new System.EventHandler(this.DownloadHandler_Tick);
            // 
            // downloadStarter
            // 
            this.downloadStarter.Tick += new System.EventHandler(this.DownloadStarter_Tick);
            // 
            // statusAreaPanel
            // 
            this.statusAreaPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(46)))));
            this.statusAreaPanel.Controls.Add(this.downloadProgress);
            this.statusAreaPanel.Controls.Add(this.statusAreaLayoutPanel);
            this.statusAreaPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusAreaPanel.Location = new System.Drawing.Point(0, 586);
            this.statusAreaPanel.Name = "statusAreaPanel";
            this.statusAreaPanel.Size = new System.Drawing.Size(744, 75);
            this.statusAreaPanel.TabIndex = 0;
            this.statusAreaPanel.Visible = false;
            // 
            // appsList
            // 
            this.appsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appsList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.appsList.CheckBoxes = true;
            this.appsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.appsList.ContextMenuStrip = this.appMenu;
            this.appsList.FullRowSelect = true;
            listViewGroup1.Header = "listViewGroup0";
            listViewGroup1.Name = "listViewGroup0";
            listViewGroup2.Header = "listViewGroup1";
            listViewGroup2.Name = "listViewGroup1";
            listViewGroup3.Header = "listViewGroup2";
            listViewGroup3.Name = "listViewGroup2";
            listViewGroup4.Header = "listViewGroup3";
            listViewGroup4.Name = "listViewGroup3";
            listViewGroup5.Header = "listViewGroup4";
            listViewGroup5.Name = "listViewGroup4";
            listViewGroup6.Header = "listViewGroup5";
            listViewGroup6.Name = "listViewGroup5";
            listViewGroup7.Header = "listViewGroup6";
            listViewGroup7.Name = "listViewGroup6";
            listViewGroup8.Header = "listViewGroup7";
            listViewGroup8.Name = "listViewGroup7";
            listViewGroup9.Header = "listViewGroup8";
            listViewGroup9.Name = "listViewGroup8";
            listViewGroup10.Header = "listViewGroup9";
            listViewGroup10.Name = "listViewGroup9";
            listViewGroup11.Header = "listViewGroup10";
            listViewGroup11.Name = "listViewGroup10";
            listViewGroup12.Header = "listViewGroup11";
            listViewGroup12.Name = "listViewGroup11";
            listViewGroup13.Header = "listViewGroup12";
            listViewGroup13.Name = "listViewGroup12";
            this.appsList.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3,
            listViewGroup4,
            listViewGroup5,
            listViewGroup6,
            listViewGroup7,
            listViewGroup8,
            listViewGroup9,
            listViewGroup10,
            listViewGroup11,
            listViewGroup12,
            listViewGroup13});
            this.appsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.appsList.LabelWrap = false;
            this.appsList.Location = new System.Drawing.Point(0, 0);
            this.appsList.MultiSelect = false;
            this.appsList.Name = "appsList";
            this.appsList.Size = new System.Drawing.Size(744, 579);
            this.appsList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.appsList.TabIndex = 0;
            this.appsList.TabStop = false;
            this.appsList.UseCompatibleStateImageBehavior = false;
            this.appsList.View = System.Windows.Forms.View.Details;
            this.appsList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.AppsList_ItemCheck);
            this.appsList.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.AppsList_ItemChecked);
            this.appsList.Enter += new System.EventHandler(this.AppsList_Enter);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Application";
            this.columnHeader1.Width = 189;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Description";
            this.columnHeader2.Width = 270;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Version";
            this.columnHeader3.Width = 81;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Download Size";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Installed Size";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Source";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader6.Width = 115;
            // 
            // appMenu
            // 
            this.appMenu.BackColor = System.Drawing.SystemColors.Control;
            this.appMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.appMenuItem1,
            this.appMenuItem2,
            this.appMenuItemSeparator1,
            this.appMenuItem3,
            this.toolStripSeparator1,
            this.appMenuItem4});
            this.appMenu.Name = "addMenu";
            this.appMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.appMenu.ShowItemToolTips = false;
            this.appMenu.Size = new System.Drawing.Size(224, 126);
            this.appMenu.Opening += new System.ComponentModel.CancelEventHandler(this.AppMenu_Opening);
            // 
            // appMenuItem1
            // 
            this.appMenuItem1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.appMenuItem1.Name = "appMenuItem1";
            this.appMenuItem1.Size = new System.Drawing.Size(223, 22);
            this.appMenuItem1.Text = "Check";
            this.appMenuItem1.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // appMenuItem2
            // 
            this.appMenuItem2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.appMenuItem2.Name = "appMenuItem2";
            this.appMenuItem2.Size = new System.Drawing.Size(223, 22);
            this.appMenuItem2.Text = "Check All";
            this.appMenuItem2.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // appMenuItemSeparator1
            // 
            this.appMenuItemSeparator1.Name = "appMenuItemSeparator1";
            this.appMenuItemSeparator1.Size = new System.Drawing.Size(220, 6);
            // 
            // appMenuItem3
            // 
            this.appMenuItem3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.appMenuItem3.Name = "appMenuItem3";
            this.appMenuItem3.Size = new System.Drawing.Size(223, 22);
            this.appMenuItem3.Text = "Open in Browser";
            this.appMenuItem3.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(220, 6);
            // 
            // appMenuItem4
            // 
            this.appMenuItem4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.appMenuItem4.Name = "appMenuItem4";
            this.appMenuItem4.Size = new System.Drawing.Size(223, 22);
            this.appMenuItem4.Text = "Show advanced information";
            this.appMenuItem4.Click += new System.EventHandler(this.AppMenuItem_Click);
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // searchResultBlinker
            // 
            this.searchResultBlinker.Interval = 300;
            this.searchResultBlinker.Tick += new System.EventHandler(this.SearchResultBlinker_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(84)))), ((int)(((byte)(86)))));
            this.ClientSize = new System.Drawing.Size(744, 661);
            this.Controls.Add(this.settingsAreaBorder);
            this.Controls.Add(this.settingsArea);
            this.Controls.Add(this.buttonAreaBorder);
            this.Controls.Add(this.buttonAreaPanel);
            this.Controls.Add(this.statusAreaBorder);
            this.Controls.Add(this.statusAreaPanel);
            this.Controls.Add(this.appsList);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(760, 125);
            this.Name = "MainForm";
            this.Opacity = 0D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Apps Downloader";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.ResizeBegin += new System.EventHandler(this.MainForm_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.MainForm_ResizeEnd);
            this.settingsArea.ResumeLayout(false);
            this.settingsArea.PerformLayout();
            this.buttonAreaPanel.ResumeLayout(false);
            this.statusAreaLayoutPanel.ResumeLayout(false);
            this.statusAreaRightPanel.ResumeLayout(false);
            this.statusAreaLeftPanel.ResumeLayout(false);
            this.statusAreaPanel.ResumeLayout(false);
            this.appMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label appStatus;
        private System.Windows.Forms.Label downloadReceivedLabel;
        private System.Windows.Forms.Label urlStatus;
        private System.Windows.Forms.Label appStatusLabel;
        private System.Windows.Forms.Label fileStatusLabel;
        private System.Windows.Forms.Panel settingsAreaBorder;
        private System.Windows.Forms.Panel settingsArea;
        private System.Windows.Forms.CheckBox highlightInstalledCheck;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.CheckBox showColorsCheck;
        private System.Windows.Forms.CheckBox showGroupsCheck;
        private System.Windows.Forms.Panel buttonAreaBorder;
        private System.Windows.Forms.Label urlStatusLabel;
        private System.Windows.Forms.Label timeStatusLabel;
        private System.Windows.Forms.Panel buttonAreaPanel;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button startBtn;
        private System.Windows.Forms.Label timeStatus;
        private System.Windows.Forms.TableLayoutPanel statusAreaLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel statusAreaRightPanel;
        private System.Windows.Forms.Label downloadReceived;
        private System.Windows.Forms.Label downloadSpeed;
        private System.Windows.Forms.Label downloadSpeedLabel;
        private System.Windows.Forms.TableLayoutPanel statusAreaLeftPanel;
        private System.Windows.Forms.Label fileStatus;
        private System.Windows.Forms.Panel statusAreaBorder;
        private System.Windows.Forms.Panel downloadProgress;
        private System.Windows.Forms.Timer downloadHandler;
        private System.Windows.Forms.Timer downloadStarter;
        private System.Windows.Forms.Panel statusAreaPanel;
        private System.Windows.Forms.ListView appsList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ContextMenuStrip appMenu;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem2;
        private System.Windows.Forms.ToolStripSeparator appMenuItemSeparator1;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem3;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Timer searchResultBlinker;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem appMenuItem4;
    }
}

