namespace AppsLauncher.UI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using SilDev;
    using SilDev.Forms;

    public partial class AboutForm : Form
    {
        private static readonly object BwLocker = new object();
        private static int? _updExitCode = 0;

        public AboutForm()
        {
            InitializeComponent();

            logoPanel.BackColor = Main.Colors.Base;

            updateBtn.Image = ResourcesEx.GetSystemIcon(ResourcesEx.ImageresIconIndex.Network, Main.SystemResourcePath)?.ToBitmap();
            updateBtn.ForeColor = Main.Colors.ButtonText;
            updateBtn.BackColor = Main.Colors.Button;
            updateBtn.FlatAppearance.MouseDownBackColor = Main.Colors.Button;
            updateBtn.FlatAppearance.MouseOverBackColor = Main.Colors.ButtonHover;

            aboutInfoLabel.ActiveLinkColor = Main.Colors.Base;
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            Icon = ResourcesEx.GetSystemIcon(ResourcesEx.ImageresIconIndex.HelpShield, Main.SystemResourcePath);

            Lang.SetControlLang(this);

            var title = Lang.GetText("AboutFormTitle");
            if (!string.IsNullOrWhiteSpace(title))
                Text = $@"{title} Portable Apps Suite";

            Main.SetFont(this);

            AddFileInfoLabels();

            updateBtnPanel.Width = TextRenderer.MeasureText(updateBtn.Text, updateBtn.Font).Width + 32;

            aboutInfoLabel.BorderStyle = BorderStyle.None;
            aboutInfoLabel.Text = string.Format(Lang.GetText(aboutInfoLabel), "Si13n7 Developments", Lang.GetText("aboutInfoLabelLinkLabel1"), Lang.GetText("aboutInfoLabelLinkLabel2"));
            aboutInfoLabel.Links.Clear();
            aboutInfoLabel.LinkText("Si13n7 Developments", "http://www.si13n7.com");
            aboutInfoLabel.LinkText(Lang.GetText("aboutInfoLabelLinkLabel1"), "http://paypal.si13n7.com");
            aboutInfoLabel.LinkText(Lang.GetText("aboutInfoLabelLinkLabel2"), "https://support.si13n7.com");
            copyrightLabel.Text = string.Format(copyrightLabel.Text, DateTime.Now.Year);
        }

        private void AddFileInfoLabels()
        {
            var verInfoList = new List<FileVersionInfo>();
            string[][] strArray =
            {
                new[]
                {
                    "AppsLauncher.exe",
                    "AppsLauncher64.exe",
                    "Binaries\\AppsDownloader.exe",
                    "Binaries\\AppsDownloader64.exe",
                    "Binaries\\Updater.exe"
                },
                new[]
                {
                    "Binaries\\SilDev.CSharpLib.dll",
                    "Binaries\\SilDev.CSharpLib64.dll"
                },
                new[]
                {
                    "Binaries\\Helper\\7z\\7zG.exe",
                    "Binaries\\Helper\\7z\\x64\\7zG.exe"
                }
            };
            var verArray = new Version[strArray.Length];
            for (var i = 0; i < strArray.Length; i++)
                foreach (var f in strArray[i])
                    try
                    {
                        if (verArray[i] == null)
                            verArray[i] = Version.Parse("0.0.0.0");
                        var fvi = FileVersionInfo.GetVersionInfo(PathEx.Combine(PathEx.LocalDir, f));
                        Version ver;
                        if (Version.TryParse(fvi.ProductVersion, out ver) && verArray[i] < ver)
                            verArray[i] = ver;
                        verInfoList.Add(fvi);
                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex);
                    }

            var bottom = 0;
            foreach (var fvi in verInfoList)
                try
                {
                    var nam = new Label
                    {
                        AutoSize = true,
                        BackColor = Color.Transparent,
                        Font = new Font("Segoe UI", 12.25f, FontStyle.Bold, GraphicsUnit.Point),
                        ForeColor = Color.PowderBlue,
                        Location = new Point(aboutInfoLabel.Left, bottom == 0 ? 15 : bottom + 10),
                        Text = fvi.FileDescription
                    };
                    mainPanel.Controls.Add(nam);
                    Version reqVer;
                    switch (Path.GetFileName(fvi.FileName)?.ToLower())
                    {
                        case "sildev.csharplib.dll":
                        case "sildev.csharplib64.dll":
                            reqVer = verArray[1];
                            break;
                        case "7zg.exe":
                            reqVer = verArray[2];
                            break;
                        default:
                            reqVer = verArray[0];
                            break;
                    }

                    Version curVer;
                    if (!Version.TryParse(fvi.ProductVersion, out curVer))
                        curVer = Version.Parse("0.0.0.0");
                    var ver = new Label
                    {
                        AutoSize = true,
                        BackColor = nam.BackColor,
                        Font = new Font(nam.Font.FontFamily, 8.25f, FontStyle.Regular, nam.Font.Unit),
                        ForeColor = reqVer == curVer ? Color.PaleGreen : Color.Firebrick,
                        Location = new Point(nam.Left + 3, nam.Bottom),
                        Text = fvi.ProductVersion
                    };
                    mainPanel.Controls.Add(ver);
                    var sep = new Label
                    {
                        AutoSize = true,
                        BackColor = nam.BackColor,
                        Font = ver.Font,
                        ForeColor = copyrightLabel.ForeColor,
                        Location = new Point(ver.Right, nam.Bottom),
                        Text = @"|"
                    };
                    mainPanel.Controls.Add(sep);
                    var pat = new Label
                    {
                        AutoSize = true,
                        BackColor = nam.BackColor,
                        Font = ver.Font,
                        ForeColor = ver.ForeColor,
                        Location = new Point(sep.Right, nam.Bottom),
                        Text = fvi.FileName.Replace(PathEx.LocalDir, string.Empty).TrimStart('\\')
                    };
                    mainPanel.Controls.Add(pat);
                    bottom = pat.Bottom;
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                }

            Height += bottom;
        }

        private void AboutForm_FormClosing(object sender, FormClosingEventArgs e) =>
            e.Cancel = updateChecker.IsBusy;

        private void updateBtn_Click(object sender, EventArgs e)
        {
            updateBtn.Enabled = false;
            if (!updateChecker.IsBusy)
                updateChecker.RunWorkerAsync();
            closeToUpdate.Enabled = true;
        }

        private void updateChecker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (var p = ProcessEx.Start("%CurDir%\\Binaries\\Updater.exe", false, false))
            {
                if (!p?.HasExited != true)
                    return;
                p?.WaitForExit();
                lock (BwLocker)
                {
                    _updExitCode = p?.ExitCode;
                }
            }
        }

        private void updateChecker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) =>
            updateBtn.Enabled = true;

        private void closeToUpdate_Tick(object sender, EventArgs e)
        {
            if (updateChecker.IsBusy)
                return;
            closeToUpdate.Enabled = false;
            MsgBoxEx.Show(this, _updExitCode > 0 ? Lang.GetText("OperationCanceledMsg") : Lang.GetText("NoUpdatesFoundMsg"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void aboutInfoLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Link.LinkData is Uri)
                Process.Start(e.Link.LinkData.ToString());
        }
    }
}
