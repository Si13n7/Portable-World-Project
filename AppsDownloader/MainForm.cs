﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace AppsDownloader
{
    public partial class MainForm : Form
    {
        static string HomeDir = Path.GetFullPath(string.Format("{0}\\..", Application.StartupPath));
        static string DownloadServer = string.Empty;
        static string AppsDBPath = string.Empty;
        static List<string> WebInfoSections = new List<string>();

        static string IniPath = Path.Combine(Application.StartupPath, "AppsDownloader.ini");
        static string SWSrv = SilDev.Initialization.ReadValue("Shareware", "Srv", IniPath);
        static string SWUsr = SilDev.Initialization.ReadValue("Shareware", "Usr", IniPath);
        static string SWPwd = SilDev.Initialization.ReadValue("Shareware", "Pwd", IniPath);

        static bool UpdateSearch = Environment.CommandLine.Contains("7fc552dd-328e-4ed8-b3c3-78f4bf3f5b0e");

        public MainForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.PortableApps_gray;
            AppList.Select();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Lang.SetControlLang(this);
            bool InternetIsAvailable = SilDev.Network.InternetIsAvailable();
            if (!InternetIsAvailable)
            {
                if (!UpdateSearch)
                    SilDev.MsgBox.Show(this, Lang.GetText("InternetIsNotAvailableMsg"), Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                Environment.Exit(Environment.ExitCode);
            }
            DownloadServer = SilDev.Network.GetTheBestServer("raw.githubusercontent.com/Si13n7/_ServerInfos/master/Server-DNS.ini", InternetIsAvailable);
            if (string.IsNullOrWhiteSpace(DownloadServer))
            {
                if (!UpdateSearch)
                    SilDev.MsgBox.Show(Lang.GetText("NoServerAvailableMsg"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                Environment.Exit(Environment.ExitCode);
            }
            try
            {
                AppsDBPath = Path.Combine(Application.StartupPath, "AppInfo.ini");
                SilDev.Network.DownloadFile("https://raw.githubusercontent.com/Si13n7/Portable-World-Project/master/AppsDownloader/AppDB/AppInfo.ini", AppsDBPath);
                if (!File.Exists(AppsDBPath))
                    throw new Exception("Server connection failed.");
                Thread TipThread = new Thread(() => new TipForm(Text, Lang.GetText("DatabaseAccessMsg"), 0, FormStartPosition.CenterScreen).ShowDialog());
                TipThread.Start();
                string ExternDBPath = Path.Combine(Application.StartupPath, "AppInfo.7z");
                if (File.Exists(ExternDBPath))
                    File.Delete(ExternDBPath);
                SilDev.Network.DownloadFile(SilDev.Crypt.Base64.Decrypt("aHR0cDovL3BvcnRhYmxlYXBwcy5jb20vdXBkYXRlci91cGRhdGUuN3o="), ExternDBPath);
                WebInfoSections = SilDev.Initialization.GetSections(AppsDBPath);
                if (File.Exists(ExternDBPath))
                {
                    SilDev.Run.App(new ProcessStartInfo() { FileName = "%CurrentDir%\\7z\\_7zHelper.bat", Arguments = string.Format("x \"\"\"{0}\"\"\" -o\"\"\"{1}\"\"\" -y", ExternDBPath, Application.StartupPath), WindowStyle = ProcessWindowStyle.Hidden }, 0);
                    File.Delete(ExternDBPath);
                    ExternDBPath = Path.Combine(Application.StartupPath, "update.ini");
                    if (File.Exists(ExternDBPath))
                    {
                        foreach (string section in SilDev.Initialization.GetSections(ExternDBPath))
                        {
                            string cat = SilDev.Initialization.ReadValue(section, "Category", ExternDBPath);
                            string nam = SilDev.Initialization.ReadValue(section, "Name", ExternDBPath);
                            if (WebInfoSections.Contains(section) || string.IsNullOrWhiteSpace(cat) || string.IsNullOrWhiteSpace(nam) || nam.Contains("PortableApps.com"))
                                continue;
                            string pat = SilDev.Initialization.ReadValue(section, "DownloadPath", ExternDBPath);
                            pat = string.Format("{0}/{1}", string.IsNullOrWhiteSpace(pat) ? "http://downloads.sourceforge.net/portableapps" : pat, SilDev.Initialization.ReadValue(section, "DownloadFile", ExternDBPath));
                            if (!pat.EndsWith(".paf.exe", StringComparison.OrdinalIgnoreCase))
                                continue;
                            File.AppendAllText(AppsDBPath, Environment.NewLine);
                            string ver = SilDev.Initialization.ReadValue(section, "DisplayVersion", ExternDBPath);
                            string siz = SilDev.Initialization.ReadValue(section, "DownloadSize", ExternDBPath);
                            string des = SilDev.Initialization.ReadValue(section, "Description", ExternDBPath);
                            string adv = SilDev.Initialization.ReadValue(section, "Advanced", ExternDBPath);
                            SilDev.Initialization.WriteValue(section, "Category", cat, AppsDBPath);
                            SilDev.Initialization.WriteValue(section, "Name", nam, AppsDBPath);
                            SilDev.Initialization.WriteValue(section, "ArchivePath", pat, AppsDBPath);
                            SilDev.Initialization.WriteValue(section, "Version", ver, AppsDBPath);
                            SilDev.Initialization.WriteValue(section, "Size", siz, AppsDBPath);
                            SilDev.Initialization.WriteValue(section, "Description", des, AppsDBPath);
                            SilDev.Initialization.WriteValue(section, "Website", "PortableApps.com", AppsDBPath);
                            if (adv.ToLower() == "true")
                                SilDev.Initialization.WriteValue(section, "Advanced", true, AppsDBPath);
                        }
                        File.Delete(ExternDBPath);
                        WebInfoSections = SilDev.Initialization.GetSections(AppsDBPath);
                    }
                }
                TipThread.Abort();
                if (!UpdateSearch)
                {
                    appDBStatus.Text = string.Format(Lang.GetText(appDBStatus), WebInfoSections.Count);
                    SetAppList(WebInfoSections);
                    if (AppList.Items.Count == 0)
                        throw new Exception("No available apps found.");
                    return;
                }
            }
            catch (Exception ex)
            {
                SilDev.Log.Debug(ex);
                Environment.Exit(Environment.ExitCode);
            }
            try
            {
                string UpdateInfo = SilDev.Network.DownloadString(string.Format("{0}/Portable%20World/.free/index_virustotal.txt", DownloadServer));
                if (string.IsNullOrWhiteSpace(UpdateInfo))
                    throw new Exception("Server connection failed.");
                Dictionary<string, string> hashList = new Dictionary<string, string>();
                foreach (string line in UpdateInfo.Split(','))
                {
                    string[] split = line.Replace(Environment.NewLine, string.Empty).Trim().Split(' ');
                    if (split.Length != 2)
                        continue;
                    hashList.Add(split[1], split[0]);
                }
                if (hashList.Count == 0)
                    throw new Exception("No update data found.");
                List<string> InstalledApps = GetInstalledApps(0);
                List<string> OutdatedApps = new List<string>();
                foreach (string dir in InstalledApps)
                {
                    string section = Path.GetFileName(dir);
                    if (!WebInfoSections.Contains(section))
                        continue;
                    string file = SilDev.Initialization.ReadValue(section, "VerCheck", AppsDBPath);
                    if (string.IsNullOrWhiteSpace(file))
                    {
                        string appIniPath = Path.Combine(dir, "App\\AppInfo\\appinfo.ini");
                        if (!File.Exists(appIniPath))
                            continue;
                        string localVersion = SilDev.Initialization.ReadValue("Version", "DisplayVersion", appIniPath);
                        string onlineVersion = SilDev.Initialization.ReadValue(section, "Version", AppsDBPath);
                        if (string.IsNullOrWhiteSpace(localVersion) || string.IsNullOrWhiteSpace(onlineVersion))
                            continue;
                        if (localVersion != onlineVersion)
                            OutdatedApps.Add(section);
                        continue;
                    }
                    string appPath = SilDev.Initialization.ReadValue(dir, "ArchivePath", AppsDBPath);
                    appPath = Path.Combine(appPath, string.Format("Apps\\{0}", appPath.Replace("/", "\\").Replace(".7z", string.Empty)));
                    string filePath = Path.Combine(appPath, file);
                    if (!File.Exists(filePath))
                        continue;
                    if (SilDev.Crypt.SHA.EncryptFile(appPath, SilDev.Crypt.SHA.CryptKind.SHA256) != hashList[file])
                        OutdatedApps.Add(dir);
                }
                if (OutdatedApps.Count == 0)
                    throw new Exception("No updates available.");
                SetAppList(OutdatedApps);
                if (!SilDev.Elevation.IsAdministrator)
                {
                    if (SilDev.MsgBox.Show(this, string.Format(Lang.GetText("UpdatesAvailableMsg0"), AppList.Items.Count, AppList.Items.Count > 1 ? Lang.GetText("UpdatesAvailableMsg1") : Lang.GetText("UpdatesAvailableMsg2")), Text, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                    {
                        SilDev.Elevation.RestartAsAdministrator("7fc552dd-328e-4ed8-b3c3-78f4bf3f5b0e");
                        throw new Exception("Restart as administrator.");
                    }
                    throw new Exception("Update canceled.");
                }
                foreach (ListViewItem item in AppList.Items)
                    item.Checked = true;
            }
            catch (Exception ex)
            {
                SilDev.Log.Debug(ex);
                Environment.Exit(Environment.ExitCode);
            }
        }

        private void SetAppList(List<string> _list)
        {
            

            foreach (string section in _list)
            {
                string[] vars = new string[]
                {
                    SilDev.Initialization.ReadValue(section, "Category", AppsDBPath),
                    SilDev.Initialization.ReadValue(section, "Name", AppsDBPath),
                    SilDev.Initialization.ReadValue(section, "Description", AppsDBPath),
                    SilDev.Initialization.ReadValue(section, "Version", AppsDBPath),
                    SilDev.Initialization.ReadValue(section, "Size", AppsDBPath),
                    SilDev.Initialization.ReadValue(section, "Website", AppsDBPath),
                    SilDev.Initialization.ReadValue(section, "Advanced", AppsDBPath),
                };
                ListViewItem item = new ListViewItem(vars[1]);
                item.Name = section;
                item.SubItems.Add(!string.IsNullOrWhiteSpace(vars[2]) ? vars[2] : string.Empty);
                item.SubItems.Add(!string.IsNullOrWhiteSpace(vars[3]) ? vars[3] : "0.0.0.0");
                item.SubItems.Add(!string.IsNullOrWhiteSpace(vars[4]) ? string.Format("{0} MB", vars[4]) : ">0 MB");
                item.SubItems.Add(!string.IsNullOrWhiteSpace(vars[5]) ? vars[5] : string.Empty);
                if (section.EndsWith("###") && (string.IsNullOrEmpty(SWSrv) || string.IsNullOrEmpty(SWSrv) || string.IsNullOrEmpty(SWSrv)))
                    continue;
                if (!string.IsNullOrWhiteSpace(vars[0]))
                {
                    try
                    {
                        ListViewGroup group = new ListViewGroup(vars[0]);
                        if (!section.EndsWith("###"))
                        {
                            for (int i = 0; i < AppList.Groups.Count; i++)
                            {
                                if (string.IsNullOrWhiteSpace(vars[6]) && AppList.Groups[i].Header == vars[0] || AppList.Groups[i].Header == "*Advanced")
                                {
                                    AppList.Items.Add(item).Group = AppList.Groups[i];
                                    break;
                                }
                            }
                        }
                        else
                            AppList.Items.Add(item).Group = AppList.Groups[AppList.Groups.Count - 1];
                    }
                    catch (Exception ex)
                    {
                        SilDev.Log.Debug(ex);
                    }
                }
            }
            ShowColors();
        }

        private List<string> GetInstalledApps(int _index)
        {
            List<string> list = new List<string>();
            try
            {
                list.AddRange(Directory.GetDirectories(Path.Combine(HomeDir, "Apps"), "*", SearchOption.TopDirectoryOnly).Where(s => !s.StartsWith(".")).ToArray());
                list.AddRange(Directory.GetDirectories(Path.Combine(HomeDir, "Apps\\.free"), "*", SearchOption.TopDirectoryOnly));
                if (_index == 1)
                    list.AddRange(Directory.GetDirectories(Path.Combine(HomeDir, "Apps\\.repack"), "*", SearchOption.TopDirectoryOnly));
                if (_index > 1)
                    list.AddRange(Directory.GetDirectories(Path.Combine(HomeDir, "Apps\\.share"), "*", SearchOption.TopDirectoryOnly));
            }
            catch (Exception ex)
            {
                SilDev.Log.Debug(ex);
            }
            return list;
        }

        private List<string> GetInstalledApps()
        {
            return GetInstalledApps(1);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            SilDev.WinAPI.SetForegroundWindow(Handle);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (AppList.CheckedItems.Count > 0 && SilDev.MsgBox.Show(this, Lang.GetText("AreYouSureMsg"), Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }
            Environment.ExitCode = 1;
            Environment.Exit(Environment.ExitCode);
        }

        private void AppList_Enter(object sender, EventArgs e)
        {
            ShowColors();
        }

        private void AppList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            OKBtn.Enabled = AppList.CheckedItems.Count > 0;
        }

        private void ShowGroupsCheck_CheckedChanged(object sender, EventArgs e)
        {
            AppList.ShowGroups = ShowGroupsCheck.Checked;
        }

        private void ShowColorsCheck_CheckedChanged(object sender, EventArgs e)
        {
            ShowColors();
        }

        private void ShowColors()
        {
            try
            {
                foreach (ListViewItem item in AppList.Items)
                {
                    item.ForeColor = AppList.ForeColor;
                    item.BackColor = AppList.BackColor;
                }
                foreach (ListViewItem item in AppList.Items)
                {
                    if (ShowColorsCheck.Checked)
                    {
                        item.ForeColor = AppList.ForeColor;
                        switch (item.Group.Header)
                        {
                            case "Accessibility":
                                item.BackColor = ColorTranslator.FromHtml("#00CC99");
                                break;
                            case "Development":
                                item.BackColor = ColorTranslator.FromHtml("#66CCFF");
                                break;
                            case "Education":
                                item.BackColor = ColorTranslator.FromHtml("#FFFF99");
                                break;
                            case "Graphics and Pictures":
                                item.BackColor = ColorTranslator.FromHtml("#00CC33");
                                break;
                            case "Internet":
                                item.BackColor = ColorTranslator.FromHtml("#FF9999");
                                break;
                            case "Music and Video":
                                item.BackColor = ColorTranslator.FromHtml("#FF3399");
                                break;
                            case "Office":
                                item.BackColor = ColorTranslator.FromHtml("#FFCC99");
                                break;
                            case "Security":
                                item.BackColor = ColorTranslator.FromHtml("#6666FF");
                                break;
                            case "Utilities":
                                item.BackColor = ColorTranslator.FromHtml("#6699FF");
                                break;
                            case "*Advanced":
                                item.BackColor = ColorTranslator.FromHtml("#FF3300");
                                break;
                            case "*Shareware":
                                item.BackColor = ColorTranslator.FromHtml("#FF0000");
                                break;
                        }
                    }
                    else
                    {
                        item.ForeColor = AppList.ForeColor;
                        item.BackColor = AppList.BackColor;
                        foreach (ListViewItem.ListViewSubItem sub in item.SubItems)
                        {
                            if (sub.Text.EndsWith("si13n7.com", StringComparison.OrdinalIgnoreCase))
                            {
                                item.BackColor = Color.LightSteelBlue;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SilDev.Log.Debug(ex);
            }
        }

        private void SearchBox_Enter(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                string tmp = SearchBox.Text;
                SearchBox.Text = string.Empty;
                SearchBox.Text = tmp;
            }
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                AppList.Select();
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(SearchBox.Text))
                {
                    string search = SearchBox.Text.ToLower();
                    string[] split = null;
                    if (search.Contains("*") && !search.StartsWith("*") && !search.EndsWith("*"))
                        split = search.Split('*');
                    bool match = false;
                    for (int i = 0; i < 2; i++)
                    {
                        foreach (ListViewItem item in AppList.Items)
                        {
                            if (i < 1 && split != null && split.Length == 2)
                            {
                                var regex = new Regex(string.Format(".*{0}(.*){1}.*", split[0], split[1]), RegexOptions.IgnoreCase);
                                match = regex.IsMatch(item.Name);
                            }
                            else
                            {
                                match = item.ToString().StartsWith(search, StringComparison.OrdinalIgnoreCase);
                                if (i > 0 && !match)
                                    match = item.Name.ToLower().Contains(search);
                            }
                            if (match)
                            {
                                ShowColors();
                                if (ShowColorsCheck.Checked)
                                {
                                    item.ForeColor = AppList.BackColor;
                                    item.BackColor = AppList.ForeColor;
                                }
                                else
                                {
                                    item.ForeColor = SystemColors.HighlightText;
                                    item.BackColor = SystemColors.Highlight;
                                }
                                item.Selected = true;
                                item.EnsureVisible();
                                AppList.EnsureVisible(item.Index);
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SilDev.Log.Debug(ex);
            }
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            if (AppList.CheckedItems.Count == 0)
                return;

            AppList.HideSelection = true;
            AppList.Enabled = false;

            OKBtn.Enabled = false;
            CancelBtn.Enabled = false;

            DLSpeed.Visible = true;
            DLPercentage.Visible = true;
            DLLoaded.Visible = true;

            MultiDownloader.Enabled = true;
        }

        int count = 0;
        bool LastDownload = false;
        private void MultiDownloader_Tick(object sender, EventArgs e)
        {
            foreach (ListViewItem item in AppList.CheckedItems)
            {
                string archivePath = SilDev.Initialization.ReadValue(item.Name, "ArchivePath", AppsDBPath);
                string localArchivePath = string.Empty;
                if (!archivePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    localArchivePath = Path.Combine(HomeDir, string.Format("Apps\\{0}", archivePath.Replace("/", "\\")));
                else
                {
                    string[] tmp = archivePath.Split('/');
                    localArchivePath = Path.Combine(HomeDir, string.Format("Apps\\{0}", tmp[tmp.Length -1]));
                }
                if (File.Exists(localArchivePath) || CheckDownload.Enabled)
                    continue;
                if (!Directory.Exists(Path.GetDirectoryName(localArchivePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(localArchivePath));
                count = 0;
                Text = string.Format("Downloading: {0}", item.Text);
                if (!archivePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    if (item.Group.Header == "*Shareware")
                        SilDev.Network.DownloadFileAsync(string.Format("{0}/{1}", SWSrv.EndsWith("/") ? SWSrv.Substring(0, SWSrv.Length - 1) : SWSrv, archivePath), localArchivePath, SWUsr, SWPwd);
                    else
                        SilDev.Network.DownloadFileAsync(string.Format("{0}/Portable%20World/{1}", DownloadServer, archivePath), localArchivePath);
                }
                else
                    SilDev.Network.DownloadFileAsync(archivePath, localArchivePath);
                CheckDownload.Enabled = true;
                MultiDownloader.Enabled = false;
                if (item == AppList.CheckedItems[AppList.CheckedItems.Count - 1])
                    LastDownload = true;
            }
        }
        
        private void CheckDownload_Tick(object sender, EventArgs e)
        {
            DLSpeed.Text = SilDev.Network.DownloadInfo.GetTransferSpeed;
            DLPercentage.Value = SilDev.Network.DownloadInfo.GetProgressPercentage;
            DLLoaded.Text = SilDev.Network.DownloadInfo.GetDataReceived;
            if (!SilDev.Network.AsyncIsBusy())
                count++;
            if (count == 1)
            {
                DLPercentage.Maximum = 1000;
                DLPercentage.Value = 1000;
                DLPercentage.Value--;
                DLPercentage.Maximum = 100;
                DLPercentage.Value = 100;
            }
            if (count >= 10)
            {
                CheckDownload.Enabled = false;
                if (!LastDownload)
                {
                    MultiDownloader.Enabled = true;
                    return;
                }
                DLSpeed.Visible = false;
                DLLoaded.Visible = false;
                List<string> archiveList = new List<string>();
                archiveList.AddRange(Directory.GetFiles(Path.Combine(HomeDir, "Apps"), "*.paf.exe", SearchOption.TopDirectoryOnly));
                archiveList.AddRange(Directory.GetFiles(Path.Combine(HomeDir, "Apps\\.repack"), "*.7z", SearchOption.TopDirectoryOnly));
                archiveList.AddRange(Directory.GetFiles(Path.Combine(HomeDir, "Apps\\.free"), "*.7z", SearchOption.TopDirectoryOnly));
                archiveList.AddRange(Directory.GetFiles(Path.Combine(HomeDir, "Apps\\.share"), "*.7z", SearchOption.TopDirectoryOnly));
                foreach (string file in archiveList)
                {
                    string appDir = string.Empty;
                    if (file.EndsWith(".paf.exe", StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (string dir in GetInstalledApps())
                        {
                            if (Path.GetFileName(file).StartsWith(Path.GetFileName(dir), StringComparison.OrdinalIgnoreCase))
                            {
                                appDir = dir;
                                break;
                            }
                        }
                    }
                    else
                        appDir = file.Replace(".7z", string.Empty);
                    try
                    {
                        foreach (string f in Directory.GetFiles(appDir, "*.exe", SearchOption.AllDirectories))
                        {
                            foreach (Process p in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(f)))
                            {
                                p.CloseMainWindow();
                                p.WaitForExit(100);
                                if (!p.HasExited)
                                    p.Kill();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        SilDev.Log.Debug(ex);
                    }
                    if (file.EndsWith(".7z", StringComparison.OrdinalIgnoreCase))
                        SilDev.Run.App(new ProcessStartInfo() { FileName = "%CurrentDir%\\7z\\_7zHelper.bat", Arguments = string.Format("x \"\"\"{0}\"\"\" -o\"\"\"{1}\"\"\" -y", file, appDir), WindowStyle = ProcessWindowStyle.Minimized }, 0);
                    else
                        SilDev.Run.App(new ProcessStartInfo() { FileName = file, WorkingDirectory = Path.Combine(HomeDir, "Apps") }, 0);
                    File.Delete(file);
                }
                SilDev.MsgBox.Show(this, string.Format(Lang.GetText("SuccessfullyDownloadMsg0"), AppList.CheckedItems.Count > 1 ? "Apps" : "App", UpdateSearch ? Lang.GetText("SuccessfullyDownloadMsg1") : Lang.GetText("SuccessfullyDownloadMsg2")), Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(Environment.ExitCode);
            }
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UrlStatus_Click(object sender, EventArgs e)
        {
            Process.Start("http:\\www.si13n7.com");
        }
    }
}
