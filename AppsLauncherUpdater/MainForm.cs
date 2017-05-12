namespace Updater
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
    using LangResources;
    using Properties;
    using SilDev;
    using SilDev.Forms;
    using Timer = System.Windows.Forms.Timer;

    public partial class MainForm : Form
    {
        private static readonly string HomeDir = PathEx.Combine(PathEx.LocalDir, "..");
        private static readonly Guid UpdateGuid = Guid.NewGuid();
        private static readonly string UpdateDir = PathEx.Combine(Path.GetTempPath(), $"PortableAppsSuite-{{{UpdateGuid}}}");
        private static readonly List<string> DownloadMirrors = new List<string>();
        private readonly NetEx.AsyncTransfer _transfer = new NetEx.AsyncTransfer();
        private readonly string _updatePath = Path.Combine(UpdateDir, "Update.7z");
        private int _downloadFinishedCount;
        private bool _ipv4, _ipv6;
        private string _hashInfo, _releaseLastStamp, _snapshotLastStamp;

        public MainForm()
        {
            InitializeComponent();
            Icon = Resources.PortableApps_green_64;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Lang.SetControlLang(this);

            // Check internet connection
            if (!(_ipv4 = NetEx.InternetIsAvailable()) && !(_ipv6 = NetEx.InternetIsAvailable(true)))
            {
                Environment.ExitCode = 1;
                Application.Exit();
                return;
            }

            // Get update infos from GitHub if enabled
            if (Ini.Read("Settings", "UpdateChannel", 0) > 0)
            {
                if (!_ipv4 && _ipv6)
                {
                    Environment.ExitCode = 1;
                    Application.Exit();
                    return;
                }
                try
                {
                    var path = PathEx.AltCombine(Resources.GitRawProfileUri, Resources.GitSnapshotsPath, "Last.ini");
                    if (!NetEx.FileIsAvailable(path, 60000))
                        throw new PathNotFoundException(path);
                    var data = NetEx.Transfer.DownloadString(path);
                    if (string.IsNullOrWhiteSpace(data))
                        throw new ArgumentNullException(nameof(data));
                    _snapshotLastStamp = Ini.Read("Info", "LastStamp", data);
                    if (string.IsNullOrWhiteSpace(_snapshotLastStamp))
                        throw new ArgumentNullException(_snapshotLastStamp);
                    path = PathEx.AltCombine(Resources.GitRawProfileUri, Resources.GitSnapshotsPath, $"{_snapshotLastStamp}.ini");
                    if (!NetEx.FileIsAvailable(path, 60000))
                        throw new PathNotFoundException(path);
                    data = NetEx.Transfer.DownloadString(path);
                    if (string.IsNullOrWhiteSpace(data))
                        throw new ArgumentNullException(nameof(data));
                    _hashInfo = data;
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                }
            }

            // Get update infos if not already set
            if (string.IsNullOrWhiteSpace(_hashInfo))
            {
                // Get available download mirrors
                var dnsInfo = string.Empty;
                for (var i = 0; i < 3; i++)
                {
                    if (!_ipv4 && _ipv6)
                    {
                        dnsInfo = Resources.IPv6DNS;
                        break;
                    }
                    try
                    {
                        var path = PathEx.AltCombine(Resources.GitRawProfileUri, Resources.GitDnsPath);
                        if (!NetEx.FileIsAvailable(path, 60000))
                            throw new PathNotFoundException(path);
                        var data = NetEx.Transfer.DownloadString(path);
                        if (string.IsNullOrWhiteSpace(data))
                            throw new ArgumentNullException(nameof(data));
                        dnsInfo = data;
                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex);
                    }
                    if (string.IsNullOrWhiteSpace(dnsInfo) && i < 2)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    break;
                }
                if (!string.IsNullOrWhiteSpace(dnsInfo))
                    foreach (var section in Ini.GetSections(dnsInfo))
                    {
                        var addr = Ini.Read(section, _ipv4 ? "addr" : "ipv6", dnsInfo);
                        if (string.IsNullOrEmpty(addr))
                            continue;
                        var domain = Ini.Read(section, "domain", dnsInfo);
                        if (string.IsNullOrEmpty(domain))
                            continue;
                        var ssl = Ini.Read(section, "ssl", false, dnsInfo);
                        domain = PathEx.AltCombine(ssl ? "https:" : "http:", domain);
                        if (!DownloadMirrors.ContainsEx(domain))
                            DownloadMirrors.Add(domain);
                    }
                if (DownloadMirrors.Count == 0)
                {
                    Environment.ExitCode = 1;
                    Application.Exit();
                    return;
                }

                // Get file hashes
                foreach (var mirror in DownloadMirrors)
                {
                    try
                    {
                        var path = PathEx.AltCombine(mirror, Resources.ReleasePath, "Last.ini");
                        if (!NetEx.FileIsAvailable(path, 60000))
                            throw new PathNotFoundException(path);
                        var data = NetEx.Transfer.DownloadString(path);
                        if (string.IsNullOrWhiteSpace(data))
                            throw new ArgumentNullException(nameof(data));
                        _releaseLastStamp = Ini.ReadOnly("Info", "LastStamp", data);
                        if (string.IsNullOrWhiteSpace(_releaseLastStamp))
                            throw new ArgumentNullException(nameof(_releaseLastStamp));
                        path = PathEx.AltCombine(mirror, Resources.ReleasePath, $"{_releaseLastStamp}.ini");
                        if (!NetEx.FileIsAvailable(path, 60000))
                            throw new PathNotFoundException(path);
                        data = NetEx.Transfer.DownloadString(path);
                        if (string.IsNullOrWhiteSpace(data))
                            throw new ArgumentNullException(nameof(data));
                        _hashInfo = data;
                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex);
                    }
                    if (!string.IsNullOrWhiteSpace(_hashInfo))
                        break;
                }
            }
            if (string.IsNullOrWhiteSpace(_hashInfo))
            {
                Environment.ExitCode = 1;
                Application.Exit();
                return;
            }

            // Compare hashes
            var updateAvailable = false;
            try
            {
                foreach (var key in Ini.GetKeys("SHA256", _hashInfo))
                {
                    var file = Path.Combine(HomeDir, $"{key}.exe");
                    if (!File.Exists(file))
                        file = PathEx.Combine(PathEx.LocalDir, $"{key}.exe");
                    if (Ini.Read("SHA256", key, _hashInfo).EqualsEx(Crypto.EncryptFileToSha256(file)))
                        continue;
                    updateAvailable = true;
                    break;
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                Environment.ExitCode = 1;
                Application.Exit();
                return;
            }

            // Install updates
            if (updateAvailable)
                if (MessageBox.Show(Lang.GetText(nameof(en_US.UpdateAvailableMsg)), Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    // Update changelog
                    if (DownloadMirrors.Count > 0)
                    {
                        var changes = string.Empty;
                        foreach (var mirror in DownloadMirrors)
                        {
                            var path = PathEx.AltCombine(mirror, Resources.ReleasePath, "ChangeLog.txt");
                            if (string.IsNullOrWhiteSpace(path))
                                continue;
                            if (!NetEx.FileIsAvailable(path, 60000))
                                continue;
                            changes = NetEx.Transfer.DownloadString(path);
                            if (!string.IsNullOrWhiteSpace(changes))
                                break;
                        }
                        if (!string.IsNullOrWhiteSpace(changes))
                        {
                            changeLog.Font = new Font("Consolas", 8.25f);
                            changeLog.Text = changes.FormatNewLine();
                            var colorMap = new Dictionary<Color, string[]>
                            {
                                {
                                    Color.PaleGreen, new[]
                                    {
                                        " PORTABLE APPS SUITE",
                                        " UPDATED:",
                                        " CHANGES:"
                                    }
                                },
                                {
                                    Color.SkyBlue, new[]
                                    {
                                        " Global:",
                                        " Apps Launcher:",
                                        " Apps Downloader:",
                                        " Apps Suite Updater:"
                                    }
                                },
                                {
                                    Color.Khaki, new[]
                                    {
                                        "Version History:"
                                    }
                                },
                                {
                                    Color.Plum, new[]
                                    {
                                        "{", "}",
                                        "(", ")",
                                        "|",
                                        ".",
                                        "-"
                                    }
                                },
                                {
                                    Color.Tomato, new[]
                                    {
                                        " * "
                                    }
                                },
                                {
                                    Color.Black, new[]
                                    {
                                        new string('_', 84)
                                    }
                                }
                            };
                            foreach (var line in changeLog.Text.Split('\n'))
                            {
                                DateTime d;
                                if (line.Length < 1 || !DateTime.TryParseExact(line.Trim(' ', ':'), "d MMMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                                    continue;
                                changeLog.MarkText(line, Color.Khaki);
                            }
                            foreach (var color in colorMap)
                                foreach (var s in color.Value)
                                    changeLog.MarkText(s, color.Key);
                        }
                    }
                    else
                    {
                        changeLog.Dock = DockStyle.None;
                        changeLog.Size = new Size(changeLogPanel.Width, TextRenderer.MeasureText(changeLog.Text, changeLog.Font).Height);
                        changeLog.Location = new Point(0, changeLogPanel.Height / 2 - changeLog.Height - 16);
                        changeLog.SelectAll();
                        changeLog.SelectionAlignment = HorizontalAlignment.Center;
                        changeLog.DeselectAll();
                    }
                    ShowInTaskbar = true;
                    return;
                }

            // Exit the application if no updates were found
            Environment.ExitCode = 2;
            Application.Exit();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (!ShowInTaskbar)
                return;
            Opacity = 1d;
            Refresh();
        }

        private void ChangeLog_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                Process.Start(e.LinkText);
                WindowState = FormWindowState.Minimized;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        private void UpdateBtn_Click(object sender, EventArgs e)
        {
            var owner = sender as Button;
            if (owner == null)
                return;
            owner.Enabled = false;
            string downloadPath = null;
            if (!string.IsNullOrWhiteSpace(_snapshotLastStamp))
                try
                {
                    downloadPath = PathEx.AltCombine(Resources.GitRawProfileUri, Resources.GitSnapshotsPath, $"{_snapshotLastStamp}.7z");
                    if (!NetEx.FileIsAvailable(downloadPath, 60000))
                        throw new PathNotFoundException(downloadPath);
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                    downloadPath = null;
                }
            if (string.IsNullOrWhiteSpace(downloadPath))
                try
                {
                    var exist = false;
                    foreach (var mirror in DownloadMirrors)
                    {
                        downloadPath = PathEx.AltCombine(mirror, Resources.ReleasePath, $"{_releaseLastStamp}.7z");
                        exist = NetEx.FileIsAvailable(downloadPath, 60000);
                        if (exist)
                            break;
                    }
                    if (!exist)
                        throw new PathNotFoundException(downloadPath);
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                    downloadPath = null;
                }
            if (!string.IsNullOrWhiteSpace(downloadPath))
                try
                {
                    if (_updatePath.ContainsEx(HomeDir))
                        throw new NotSupportedException();
                    var updDir = Path.GetDirectoryName(UpdateDir);
                    if (!string.IsNullOrEmpty(updDir))
                        foreach (var dir in Directory.GetDirectories(updDir, "PortableAppsSuite-{*}", SearchOption.TopDirectoryOnly))
                            Directory.Delete(dir, true);
                    if (!Directory.Exists(UpdateDir))
                        Directory.CreateDirectory(UpdateDir);
                    foreach (var file in new[] { "7z.dll", "7zG.exe" })
                    {
                        var path = PathEx.Combine(PathEx.LocalDir, "Helper\\7z");
                        if (Environment.Is64BitOperatingSystem)
                            path = Path.Combine(path, "x64");
                        path = Path.Combine(path, file);
                        File.Copy(path, Path.Combine(UpdateDir, file));
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(ex, true);
                    return;
                }
            try
            {
                _transfer.DownloadFile(downloadPath, _updatePath);
                checkDownload.Enabled = true;
            }
            catch (Exception ex)
            {
                Log.Write(ex, true);
            }
        }

        private void CheckDownload_Tick(object sender, EventArgs e)
        {
            var owner = sender as Timer;
            if (owner == null)
                return;
            statusLabel.Text = _transfer.TransferSpeedAd + @" - " + _transfer.DataReceived;
            statusBar.Value = _transfer.ProgressPercentage;
            if (!_transfer.IsBusy)
                _downloadFinishedCount++;
            if (_downloadFinishedCount == 10)
                statusBar.JumpToEnd();
            if (_downloadFinishedCount < 100)
                return;
            owner.Enabled = false;
            string helperPath = null;
            try
            {
                helperPath = Path.GetDirectoryName(_updatePath);
                if (string.IsNullOrEmpty(helperPath))
                    return;
                helperPath = Path.Combine(helperPath, "UpdateHelper.bat");
                var helper = string.Format(Resources.BatchDummy, UpdateGuid, HomeDir, Guid.NewGuid());
                File.WriteAllText(helperPath, helper);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            try
            {
                if (string.IsNullOrEmpty(helperPath))
                    throw new ArgumentNullException(nameof(helperPath));
                var lastStamp = _releaseLastStamp;
                if (string.IsNullOrWhiteSpace(lastStamp))
                    lastStamp = _snapshotLastStamp;
                if (!Ini.Read("MD5", lastStamp, _hashInfo).EqualsEx(Crypto.EncryptFileToMd5(_updatePath)))
                    throw new InvalidOperationException();
                AppsSuite_CloseAll();
                ProcessEx.Start(helperPath, true, ProcessWindowStyle.Hidden);
                Application.Exit();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                MessageBoxEx.Show(this, Lang.GetText(nameof(en_US.InstallErrorMsg)), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.ExitCode = 1;
                CancelBtn_Click(cancelBtn, EventArgs.Empty);
            }
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (_transfer.IsBusy)
                    _transfer.CancelAsync();
                if (Directory.Exists(UpdateDir))
                    Directory.Delete(UpdateDir, true);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            Application.Exit();
        }

        private void ProgressLabel_TextChanged(object sender, EventArgs e)
        {
            try
            {
                statusTableLayoutPanel.ColumnStyles[0].Width = progressLabel.Width + 8;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        private void VirusTotalBtn_Click(object sender, EventArgs e)
        {
            var owner = sender as Label;
            if (owner == null)
                return;
            owner.Enabled = false;
            try
            {
                foreach (var key in Ini.GetKeys("SHA256", _hashInfo))
                {
                    Process.Start(string.Format(Resources.VirusTotalUri, Ini.Read("SHA256", key, _hashInfo)));
                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            owner.Enabled = true;
        }

        private void WebBtn_Click(object sender, EventArgs e) =>
            Process.Start(Resources.DevUri);

        private void AppsSuite_CloseAll()
        {
            var fileList = new List<string>();
            fileList.AddRange(Directory.GetFiles(HomeDir, "*.exe", SearchOption.TopDirectoryOnly));
            fileList.AddRange(Directory.GetFiles(PathEx.LocalDir, "*.exe", SearchOption.AllDirectories).Where(s => !PathEx.LocalPath.EqualsEx(s)));
            var taskList = fileList.SelectMany(s => Process.GetProcessesByName(Path.GetFileNameWithoutExtension(s))).ToList();
            if (!ProcessEx.Terminate(taskList))
                MessageBoxEx.Show(this, Lang.GetText(nameof(en_US.InstallErrorMsg)), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
