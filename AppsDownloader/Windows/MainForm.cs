namespace AppsDownloader.Windows
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using LangResources;
    using Libraries;
    using Properties;
    using SilDev;
    using SilDev.Forms;

    public partial class MainForm : Form
    {
        private static readonly object DownloadStarter = new object(),
                                       DownloadHandler = new object();

        private readonly ListView _appsListClone = new ListView();
        private readonly NotifyBox _notifyBox = new NotifyBox();

        private readonly Dictionary<ListViewItem, AppTransferor> _transferManager = new Dictionary<ListViewItem, AppTransferor>();
        private readonly Stopwatch _transferStopwatch = new Stopwatch();
        private readonly List<AppData> _transferFails = new List<AppData>();
        private KeyValuePair<ListViewItem, AppTransferor> _currentTransfer;
        private Task _transferTask;

        private int _currentTransferFinishTick,
                    _searchResultBlinkCount;

        public MainForm()
        {
            InitializeComponent();

            appsList.ListViewItemSorter = new ListViewEx.AlphanumericComparer();
            searchBox.DrawSearchSymbol(searchBox.ForeColor);
            if (!appsList.Focus())
                appsList.Select();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            FormEx.Dockable(this);

            Icon = Resources.PortableApps_purple;

            MinimumSize = Settings.Window.Size.Minimum;
            MaximumSize = Settings.Window.Size.Maximum;
            if (Settings.Window.Size.Width > Settings.Window.Size.Minimum.Width)
                Width = Settings.Window.Size.Width;
            if (Settings.Window.Size.Height > Settings.Window.Size.Minimum.Height)
                Height = Settings.Window.Size.Height;
            WinApi.NativeHelper.CenterWindow(Handle);
            if (Settings.Window.State == FormWindowState.Maximized)
                WindowState = FormWindowState.Maximized;

            Text = Settings.Title;
            Language.SetControlLang(this);
            for (var i = 0; i < appsList.Columns.Count; i++)
                appsList.Columns[i].Text = Language.GetText($"columnHeader{i + 1}");
            for (var i = 0; i < appsList.Groups.Count; i++)
                appsList.Groups[i].Header = Language.GetText(appsList.Groups[i].Name);
            for (var i = 0; i < appMenu.Items.Count; i++)
                appMenu.Items[i].Text = Language.GetText(appMenu.Items[i].Name);
            var statusLabels = new[]
            {
                appStatusLabel,
                fileStatusLabel,
                urlStatusLabel,
                downloadReceivedLabel,
                downloadSpeedLabel,
                timeStatusLabel
            };
            foreach (var label in statusLabels)
                label.Text = Language.GetText(label.Name);

            showGroupsCheck.Checked = Settings.ShowGroups;
            showColorsCheck.Left = showGroupsCheck.Right + 4;
            showColorsCheck.Checked = Settings.ShowGroupColors;
            highlightInstalledCheck.Left = showColorsCheck.Right + 4;
            highlightInstalledCheck.Checked = Settings.HighlightInstalled;

            appsList.SetDoubleBuffer();
            appMenu.EnableAnimation();
            appMenu.SetFixedSingle();
            statusAreaLeftPanel.SetDoubleBuffer();
            statusAreaRightPanel.SetDoubleBuffer();

            if (!Settings.ActionGuid.IsUpdateInstance)
                _notifyBox.Show(Language.GetText(nameof(en_US.DatabaseAccessMsg)), Settings.Title, NotifyBox.NotifyBoxStartPosition.Center);

            if (!Network.InternetIsAvailable)
            {
                if (!Settings.ActionGuid.IsUpdateInstance)
                    MessageBoxEx.Show(Language.GetText(nameof(en_US.InternetIsNotAvailableMsg)), Settings.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ApplicationExit(1);
                return;
            }

            if (!Settings.ActionGuid.IsUpdateInstance && !AppSupply.GetMirrors(AppSupply.Suppliers.Internal).Any())
            {
                MessageBoxEx.Show(Language.GetText(nameof(en_US.NoServerAvailableMsg)), Settings.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ApplicationExit(1);
                return;
            }

            try
            {
                Settings.CacheData.UpdateAppImages();
                Settings.CacheData.UpdateAppInfo();
                if (!Settings.CacheData.AppInfo.Any())
                    throw new InvalidOperationException("No apps found.");

                if (Settings.ActionGuid.IsUpdateInstance)
                {
                    var appUpdates = AppSupply.FindOutdatedApps();
                    if (!appUpdates.Any())
                        throw new WarningException("No updates available.");

                    AppsListSetContent(appUpdates.ToArray());
                    if (appsList.Items.Count == 0)
                        throw new InvalidOperationException("No apps available.");

                    var asterisk = string.Format(Language.GetText(appsList.Items.Count == 1 ? nameof(en_US.AppUpdateAvailableMsg) : nameof(en_US.AppUpdatesAvailableMsg)), appsList.Items.Count);
                    if (MessageBoxEx.Show(asterisk, Settings.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) != DialogResult.Yes)
                        throw new WarningException("Update canceled.");

                    foreach (var item in appsList.Items.Cast<ListViewItem>())
                        item.Checked = true;
                }
                else
                {
                    if (Settings.CacheData.AppInfo.Any())
                        AppsListSetContent();

                    if (appsList.Items.Count == 0)
                        throw new InvalidOperationException("No apps available.");
                }
            }
            catch (WarningException ex)
            {
                Log.Write(ex.Message);
                ApplicationExit(2);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                if (!Settings.ActionGuid.IsUpdateInstance)
                    MessageBoxEx.Show(Language.GetText(nameof(en_US.NoServerAvailableMsg)), Settings.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ApplicationExit(1);
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            _notifyBox?.Close();
            TopMost = false;
            Refresh();
            var timer = new Timer(components)
            {
                Interval = 1,
                Enabled = true
            };
            timer.Tick += (o, args) =>
            {
                if (Opacity < 1d)
                {
                    AppsListResizeColumns();
                    Opacity += .05d;
                    return;
                }
                timer.Dispose();
            };
        }

        private void MainForm_ResizeBegin(object sender, EventArgs e)
        {
            appsList.BeginUpdate();
            appsList.Visible = false;
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            appsList.EndUpdate();
            appsList.Visible = true;
            AppsListResizeColumns();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_transferManager.Any() && MessageBoxEx.Show(this, Language.GetText(nameof(en_US.AreYouSureMsg)), Settings.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            Settings.Window.State = WindowState;
            Settings.Window.Size.Width = Width;
            Settings.Window.Size.Height = Height;

            if (downloadHandler.Enabled)
                downloadHandler.Enabled = false;
            if (downloadStarter.Enabled)
                downloadStarter.Enabled = false;

            foreach (var appTransferor in _transferManager.Values)
                appTransferor.Transfer.CancelAsync();

            var appInstaller = AppSupply.FindAppInstaller();
            if (appInstaller.Any())
                appInstaller.ForEach(file => ProcessEx.SendHelper.WaitThenDelete(file));
        }

        private void AppsList_Enter(object sender, EventArgs e) =>
            AppsListShowColors(false);

        private void AppsList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var appData = Settings.CacheData.AppInfo?.FirstOrDefault(x => x.Key.EqualsEx(appsList.Items[e.Index].Name));
            if (appData == default(AppData))
                return;

            if (!Network.IPv4IsAvalaible && Network.IPv6IsAvalaible && !appsList.Items[e.Index].Checked && appData.DownloadCollection.Any())
            {
                var innerData = appData.DownloadCollection.First().Value;
                if (innerData?.Any() == true)
                {
                    var shortHost = innerData.First().Item1.GetShortHost();
                    switch (shortHost)
                    {
                        case AppSupply.SupplierHosts.PortableApps:
                        case AppSupply.SupplierHosts.SourceForge:
                            var message = string.Format(Language.GetText(nameof(en_US.AppInternetProtocolWarningMsg)), shortHost);
                            MessageBox.Show(message, Settings.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                    }
                }
            }

            if (appData.Requirements?.Any() != true)
                return;
            var installedApps = AppSupply.FindInstalledApps();
            foreach (var requirement in appData.Requirements)
            {
                if (installedApps.Contains(requirement))
                    continue;
                foreach (var item in appsList.Items.Cast<ListViewItem>())
                {
                    if (!item.Name.Equals(requirement))
                        continue;
                    item.Checked = e.NewValue == CheckState.Checked;
                    break;
                }
            }
        }

        private void AppsList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (!downloadStarter.Enabled && !downloadHandler.Enabled && !_transferManager.Values.Any(x => x.Transfer.IsBusy))
                startBtn.Enabled = appsList.CheckedItems.Count > 0;
        }

        private void AppsListSetContent(string[] keys = null)
        {
            var index = 0;
            var appImages = Settings.CacheData.AppImages ?? new Dictionary<string, Image>();
            if (Shareware.Enabled)
                foreach (var srv in Shareware.GetAddresses())
                {
                    var usr = Shareware.GetUser(srv);
                    var pwd = Shareware.GetPassword(srv);
                    var url = PathEx.AltCombine(srv, "AppImages.dat");
                    if (Log.DebugMode > 0)
                        Log.Write($"Shareware: Looking for '{{{Shareware.FindAddressKey(srv).ToHexa()}}}/AppImages.dat'.");
                    if (!NetEx.FileIsAvailable(url, usr, pwd, 60000))
                        continue;
                    var swAppImages = NetEx.Transfer.DownloadData(url, usr, pwd)?.DeserializeObject<Dictionary<string, Image>>();
                    if (swAppImages == null)
                        continue;
                    foreach (var pair in swAppImages)
                    {
                        if (appImages.ContainsKey(pair.Key))
                            continue;
                        appImages.Add(pair.Key, pair.Value);
                    }
                }

            appsList.BeginUpdate();
            appsList.Items.Clear();
            foreach (var info in Settings.CacheData.AppInfo)
            {
                if (!Shareware.Enabled && info.ServerKey != null || keys?.ContainsEx(info.Key) == false)
                    continue;

                var url = info.DownloadCollection.First().Value.First().Item1;
                if (string.IsNullOrWhiteSpace(url))
                    continue;

                var src = AppSupply.GetHost(AppSupply.Suppliers.Internal);
                if (url.StartsWithEx("http"))
                    if (url.ContainsEx(AppSupply.GetHost(AppSupply.Suppliers.PortableApps)) && url.ContainsEx("/redirect/"))
                        src = AppSupply.GetHost(AppSupply.Suppliers.SourceForge);
                    else
                    {
                        src = url.GetShortHost();
                        if (string.IsNullOrEmpty(src))
                            continue;
                    }

                var item = new ListViewItem(info.Name)
                {
                    Name = info.Key
                };
                item.SubItems.Add(info.Description);
                item.SubItems.Add(info.DisplayVersion);
                item.SubItems.Add(info.DownloadSize.FormatSize(Reorganize.SizeOptions.Trim));
                item.SubItems.Add(info.InstallSize.FormatSize(Reorganize.SizeOptions.Trim));
                item.SubItems.Add(src);
                item.ImageIndex = index;

                if (appImages?.ContainsKey(info.Key) == true)
                {
                    var img = appImages[info.Key];
                    if (img != null)
                        imageList.Images.Add(info.Key, img);
                }

                if (!imageList.Images.ContainsKey(info.Key))
                {
                    if (Log.DebugMode == 0)
                        continue;
                    Log.Write($"Cache: Could not find target '{Settings.CachePaths.AppImages}:{info.Key}'.");
                    info.Advanced = true;
                    imageList.Images.Add(info.Key, Resources.PortableAppsBox);
                }

                if (info.ServerKey == null)
                    foreach (var group in appsList.Groups.Cast<ListViewGroup>())
                    {
                        var enName = Language.GetText("en-US", group.Name);
                        if ((info.Advanced || !enName.EqualsEx(info.Category)) && !enName.EqualsEx("*Advanced"))
                            continue;
                        appsList.Items.Add(item).Group = group;
                        break;
                    }
                else
                    appsList.Items.Add(item).Group = appsList.Groups.Cast<ListViewGroup>().Last();

                index++;
            }

            if (Log.DebugMode > 0)
                Log.Write($"Interface: {appsList.Items.Count} {(appsList.Items.Count == 1 ? "App" : "Apps")} has been added!");

            appsList.SmallImageList = imageList;
            appsList.EndUpdate();
            AppsListShowColors();

            _appsListClone.Groups.Clear();
            foreach (var group in appsList.Groups.Cast<ListViewGroup>())
                _appsListClone.Groups.Add(new ListViewGroup
                {
                    Name = group.Name,
                    Header = group.Header
                });
            _appsListClone.Items.Clear();
            foreach (var item in appsList.Items.Cast<ListViewItem>())
                _appsListClone.Items.Add((ListViewItem)item.Clone());
        }

        private void AppsListReset()
        {
            if (_appsListClone.Items.Count == appsList.Items.Count)
                for (var i = 0; i < appsList.Items.Count; i++)
                {
                    var item = appsList.Items[i];
                    var clone = _appsListClone.Items[i];
                    foreach (var group in appsList.Groups.Cast<ListViewGroup>())
                        if (clone.Group.Name.Equals(group.Name))
                        {
                            if (!clone.Group.Name.Equals(item.Group.Name))
                                group.Items.Add(item);
                            break;
                        }
                }
            AppsListShowColors(false);
            appsList.Sort();
        }

        private void AppsListResizeColumns()
        {
            if (appsList.Columns.Count < 5)
                return;
            var staticColumnsWidth = SystemInformation.VerticalScrollBarWidth + 2;
            for (var i = 3; i < appsList.Columns.Count; i++)
                staticColumnsWidth += appsList.Columns[i].Width;
            var dynamicColumnsWidth = 0;
            while (dynamicColumnsWidth < appsList.Width - staticColumnsWidth)
                dynamicColumnsWidth++;
            for (var i = 0; i < 3; i++)
                appsList.Columns[i].Width = (int)Math.Ceiling(dynamicColumnsWidth / 100d * (i == 0 ? 35d : i == 1 ? 50d : 15d));
        }

        private void AppsListShowColors(bool searchResultColor = true)
        {
            if (searchResultBlinker.Enabled)
                searchResultBlinker.Enabled = false;
            var installed = new List<string>();
            if (!highlightInstalledCheck.Checked)
                highlightInstalledCheck.Text = Language.GetText(highlightInstalledCheck);
            else
            {
                installed = AppSupply.FindInstalledApps();
                highlightInstalledCheck.Text = $@"{Language.GetText(highlightInstalledCheck)} ({installed.Count})";
            }
            appsList.SetDoubleBuffer(false);
            appsList.BeginUpdate();
            try
            {
                var darkList = appsList.BackColor.R + appsList.BackColor.G + appsList.BackColor.B < byte.MaxValue;
                foreach (var item in appsList.Items.Cast<ListViewItem>())
                {
                    if (highlightInstalledCheck.Checked && installed.ContainsEx(item.Name))
                    {
                        item.Font = new Font(appsList.Font, FontStyle.Italic);
                        item.ForeColor = darkList ? Color.FromArgb(0xc0, 0xff, 0xc0) : Color.FromArgb(0x20, 0x40, 0x20);
                        if (searchResultColor && item.Group.Name.EqualsEx("listViewGroup0"))
                        {
                            item.BackColor = SystemColors.Highlight;
                            continue;
                        }
                        item.BackColor = darkList ? Color.FromArgb(0x20, 0x40, 0x20) : Color.FromArgb(0xc0, 0xff, 0xc0);
                        continue;
                    }
                    item.Font = appsList.Font;
                    if (searchResultColor && item.Group.Name.EqualsEx("listViewGroup0"))
                    {
                        item.ForeColor = SystemColors.HighlightText;
                        item.BackColor = SystemColors.Highlight;
                        continue;
                    }
                    item.ForeColor = appsList.ForeColor;
                    item.BackColor = appsList.BackColor;
                }
                if (!showColorsCheck.Checked)
                    return;
                foreach (ListViewItem item in appsList.Items)
                {
                    var backColor = item.BackColor;
                    switch (item.Group.Name)
                    {
                        case "listViewGroup0": // Search Result
                            continue;
                        case "listViewGroup1": // Accessibility
                            item.BackColor = Color.FromArgb(0xff, 0xff, 0x99);
                            break;
                        case "listViewGroup2": // Education
                            item.BackColor = Color.FromArgb(0xff, 0xff, 0xcc);
                            break;
                        case "listViewGroup3": // Development
                            item.BackColor = Color.FromArgb(0x77, 0x77, 0x99);
                            break;
                        case "listViewGroup4": // Office
                            item.BackColor = Color.FromArgb(0x88, 0xbb, 0xdd);
                            break;
                        case "listViewGroup5": // Internet
                            item.BackColor = Color.FromArgb(0xcc, 0x88, 0x66);
                            break;
                        case "listViewGroup6": // Graphics and Pictures	
                            item.BackColor = Color.FromArgb(0xff, 0xcc, 0xff);
                            break;
                        case "listViewGroup7": // Music and Video
                            item.BackColor = Color.FromArgb(0xcc, 0xcc, 0xff);
                            break;
                        case "listViewGroup8": // Security
                            item.BackColor = Color.FromArgb(0x66, 0xcc, 0x99);
                            break;
                        case "listViewGroup9": // Utilities
                            item.BackColor = Color.FromArgb(0x77, 0xbb, 0xbb);
                            break;
                        case "listViewGroup11": // *Advanced
                            item.BackColor = Color.FromArgb(0xff, 0x66, 0x66);
                            break;
                        case "listViewGroup12": // *Shareware
                            item.BackColor = Color.FromArgb(0xff, 0x66, 0xff);
                            break;
                    }
                    if (item.BackColor == backColor)
                        continue;
                    if (item.ForeColor != Color.Black)
                        item.ForeColor = Color.Black;

                    // adjust bright colors if a dark Windows theme style is used
                    var lightItem = item.BackColor.R + item.BackColor.G + item.BackColor.B > byte.MaxValue * 2;
                    if (darkList && lightItem)
                        item.BackColor = Color.FromArgb((byte)(item.BackColor.R * 2), (byte)(item.BackColor.G / 2), (byte)(item.BackColor.B / 2));

                    // highlight installed apps
                    if (highlightInstalledCheck.Checked && installed.ContainsEx(item.Name))
                        item.BackColor = Color.FromArgb(item.BackColor.R, (byte)(item.BackColor.G + 24), item.BackColor.B);
                }
            }
            finally
            {
                appsList.EndUpdate();
                appsList.SetDoubleBuffer();
            }
        }

        private void AppMenu_Opening(object sender, CancelEventArgs e)
        {
            if (appsList.SelectedItems.Count == 0)
            {
                e.Cancel = true;
                return;
            }
            var isChecked = appsList.SelectedItems[0].Checked;
            appMenuItem1.Text = isChecked ? Language.GetText(nameof(appMenuItem1) + "u") : Language.GetText(nameof(appMenuItem1));
            appMenuItem2.Text = isChecked ? Language.GetText(nameof(appMenuItem2) + "u") : Language.GetText(nameof(appMenuItem2));
        }

        private void AppMenuItem_Click(object sender, EventArgs e)
        {
            if (appsList.SelectedItems.Count == 0)
                return;
            var owner = sender as ToolStripMenuItem;
            switch (owner?.Name)
            {
                case "appMenuItem1":
                    appsList.SelectedItems[0].Checked = !appsList.SelectedItems[0].Checked;
                    break;
                case "appMenuItem2":
                    var isChecked = !appsList.SelectedItems[0].Checked;
                    for (var i = 0; i < appsList.Items.Count; i++)
                    {
                        if (showGroupsCheck.Checked && appsList.SelectedItems[0].Group != appsList.Items[i].Group)
                            continue;
                        appsList.Items[i].Checked = isChecked;
                    }
                    break;
                case "appMenuItem3":
                    var appData = Settings.CacheData.AppInfo.FirstOrDefault(x => x.Key.EqualsEx(appsList.SelectedItems[0].Name));
                    var website = appData?.Website;
                    if (website?.StartsWithEx("https://", "http://") != true)
                        return;
                    Process.Start(website);
                    break;
            }
        }

        private void ShowGroupsCheck_CheckedChanged(object sender, EventArgs e)
        {
            Settings.ShowGroups = (sender as CheckBox)?.Checked == true;
            appsList.ShowGroups = Settings.ShowGroups;
        }

        private void ShowColorsCheck_CheckedChanged(object sender, EventArgs e)
        {
            Settings.ShowGroupColors = (sender as CheckBox)?.Checked == true;
            AppsListShowColors();
        }

        private void HighlightInstalledCheck_CheckedChanged(object sender, EventArgs e)
        {
            Settings.HighlightInstalled = (sender as CheckBox)?.Checked == true;
            AppsListShowColors();
        }

        private void SearchBox_Enter(object sender, EventArgs e)
        {
            var owner = sender as TextBox;
            if (string.IsNullOrWhiteSpace(owner?.Text))
                return;
            var tmp = owner.Text;
            owner.Text = string.Empty;
            owner.Text = tmp;
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;
            SearchReset();
            var owner = sender as TextBox;
            if (string.IsNullOrWhiteSpace(owner?.Text))
                return;
            appsList.SetDoubleBuffer(false);
            try
            {
                foreach (var item in appsList.Items.Cast<ListViewItem>())
                {
                    var description = item.SubItems[1];
                    if (description.Text.ContainsEx(owner.Text))
                    {
                        foreach (var group in appsList.Groups.Cast<ListViewGroup>())
                            if (group.Name.EqualsEx("listViewGroup0"))
                            {
                                if (!group.Items.Contains(item))
                                    group.Items.Add(item);
                                if (!item.Selected)
                                    item.EnsureVisible();
                            }
                        continue;
                    }
                    if (!item.Text.ContainsEx(owner.Text))
                        continue;
                    foreach (var group in appsList.Groups.Cast<ListViewGroup>())
                        if (group.Name.EqualsEx("listViewGroup0"))
                        {
                            if (!group.Items.Contains(item))
                                group.Items.Add(item);
                            if (!item.Selected)
                                item.EnsureVisible();
                        }
                }
            }
            finally
            {
                appsList.SetDoubleBuffer();
            }
            AppsListShowColors();
            if (_searchResultBlinkCount > 0)
                _searchResultBlinkCount = 0;
            if (!searchResultBlinker.Enabled)
                searchResultBlinker.Enabled = true;
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            var separators = Environment.NewLine.ToArray();
            if (!searchBox.Text.ContainsEx(separators))
                return;
            var text = searchBox.Text;
            searchBox.Text = text.RemoveChar(separators);
            searchBox.SelectionStart = searchBox.TextLength;
            searchBox.ScrollToCaret();
        }

        private void SearchResultBlinker_Tick(object sender, EventArgs e)
        {
            if (!(sender is Timer owner))
                return;
            if (owner.Enabled && _searchResultBlinkCount >= 5)
                owner.Enabled = false;
            appsList.SetDoubleBuffer(false);
            try
            {
                foreach (var group in appsList.Groups.Cast<ListViewGroup>())
                {
                    if (!group.Name.EqualsEx("listViewGroup0"))
                        continue;
                    if (group.Items.Count > 0)
                        foreach (var item in appsList.Items.Cast<ListViewItem>())
                        {
                            if (!item.Group.Name.Equals(group.Name))
                                continue;
                            if (!searchResultBlinker.Enabled || item.BackColor != SystemColors.Highlight)
                            {
                                item.BackColor = SystemColors.Highlight;
                                owner.Interval = 200;
                            }
                            else
                            {
                                item.BackColor = appsList.BackColor;
                                owner.Interval = 100;
                            }
                        }
                    else
                        owner.Enabled = false;
                }
            }
            finally
            {
                appsList.SetDoubleBuffer();
            }
            if (owner.Enabled)
                _searchResultBlinkCount++;
        }

        private void SearchReset()
        {
            if (!showGroupsCheck.Checked)
                showGroupsCheck.Checked = true;
            AppsListReset();
            AppsListShowColors(false);
            appsList.Sort();
            foreach (var group in appsList.Groups.Cast<ListViewGroup>())
            {
                if (group.Items.Count == 0)
                    continue;
                foreach (var item in group.Items.Cast<ListViewItem>())
                {
                    item.EnsureVisible();
                    return;
                }
            }
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            if (!(sender is Button owner))
                return;

            var transferIsBusy = _transferManager.Values.Any(x => x.Transfer.IsBusy);
            if (!owner.Enabled || appsList.Items.Count == 0 || transferIsBusy)
                return;

            Settings.SkipWriteValue = true;

            owner.Enabled = false;
            searchBox.Text = string.Empty;

            appsList.BeginUpdate();
            foreach (var item in appsList.Items.Cast<ListViewItem>())
            {
                if (item.Checked)
                    continue;
                item.Remove();
            }
            appsList.EndUpdate();

            foreach (var filePath in AppSupply.FindAppInstaller())
                FileEx.TryDelete(filePath);

            appsList.HideSelection = !owner.Enabled;
            appsList.Enabled = owner.Enabled;
            appsList.Sort();

            appMenu.Enabled = owner.Enabled;
            showGroupsCheck.Checked = owner.Enabled;
            showGroupsCheck.Enabled = owner.Enabled;
            showColorsCheck.Enabled = owner.Enabled;
            highlightInstalledCheck.Enabled = owner.Enabled;
            searchBox.Enabled = owner.Enabled;
            cancelBtn.Enabled = owner.Enabled;

            _transferManager.Clear();
            var totalSize = 0L;
            foreach (var item in appsList.CheckedItems.Cast<ListViewItem>())
            {
                var appData = Settings.CacheData.AppInfo.FirstOrDefault(x => x.Key.EqualsEx(item.Name));
                if (appData == null)
                    continue;

                if (appData.DownloadCollection.Count > 1 && !appData.Settings.ArchiveLangConfirmed)
                    try
                    {
                        var result = DialogResult.None;
                        while (result != DialogResult.OK)
                            using (Form dialog = new LangSelectionForm(appData))
                            {
                                result = dialog.ShowDialog();
                                if (result == DialogResult.OK)
                                    break;
                                if (MessageBoxEx.Show(this, Language.GetText(nameof(en_US.AreYouSureMsg)), Settings.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                                    continue;
                                ApplicationExit();
                                return;
                            }
                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex);
                    }

                _transferManager.Add(item, new AppTransferor(appData));

                unchecked
                {
                    totalSize += appData.DownloadSize;
                    totalSize += appData.InstallSize;
                }
            }

            var freeSpace = Settings.FreeDiskSpace;
            if (totalSize > freeSpace)
            {
                var warning = string.Format(Language.GetText(nameof(en_US.NotEnoughDiskSpaceMsg)), totalSize.FormatSize(), freeSpace.FormatSize());
                switch (MessageBoxEx.Show(this, warning, Settings.Title, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning))
                {
                    case DialogResult.Abort:
                        _transferManager.Clear();
                        ApplicationExit();
                        break;
                    case DialogResult.Retry:
                        owner.Enabled = !owner.Enabled;
                        appsList.HideSelection = !owner.Enabled;
                        appsList.Enabled = owner.Enabled;
                        appMenu.Enabled = owner.Enabled;
                        cancelBtn.Enabled = owner.Enabled;
                        return;
                }
            }

            downloadStarter.Enabled = !owner.Enabled;
        }

        private void CancelBtn_Click(object sender, EventArgs e) =>
            ApplicationExit();

        private void DownloadStarter_Tick(object sender, EventArgs e)
        {
            if (!(sender is Timer owner))
                return;
            lock (DownloadStarter)
            {
                owner.Enabled = false;

                if (_transferManager.Any(x => x.Value.Transfer.IsBusy))
                    return;

                try
                {
                    _currentTransfer = _transferManager.First(x => !_transferFails.Contains(x.Value.AppData) && (x.Key.Checked || x.Value.Transfer.HasCanceled));
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                    ApplicationExit(1);
                    return;
                }

                var listViewItem = _currentTransfer.Key;
                appStatus.Text = listViewItem.Text;
                urlStatus.Text = $@"{listViewItem.SubItems[listViewItem.SubItems.Count - 1].Text} ";
                Text = $@"{string.Format(Language.GetText(nameof(en_US.titleStatus)), _transferManager.Keys.Count(x => !x.Checked), _transferManager.Keys.Count)} - {appStatus.Text}";

                if (!_transferStopwatch.IsRunning)
                    _transferStopwatch.Start();

                var appTransferor = _currentTransfer.Value;
                _transferTask?.Dispose();
                _transferTask = Task.Run(delegate { appTransferor.StartDownload(); });

                downloadHandler.Enabled = !owner.Enabled;
            }
        }

        private void DownloadHandler_Tick(object sender, EventArgs e)
        {
            if (!(sender is Timer owner))
                return;
            lock (DownloadHandler)
            {
                var appTransferor = _currentTransfer.Value;
                DownloadProgressUpdate(appTransferor.Transfer.ProgressPercentage);

                if (!statusAreaBorder.Visible || !statusAreaPanel.Visible)
                {
                    SuspendLayout();
                    statusAreaBorder.SuspendLayout();
                    statusAreaPanel.SuspendLayout();
                    statusAreaBorder.Visible = true;
                    statusAreaPanel.Visible = true;
                    statusAreaPanel.ResumeLayout();
                    statusAreaBorder.ResumeLayout();
                    ResumeLayout();
                }

                statusAreaLeftPanel.SuspendLayout();
                fileStatus.Text = appTransferor.Transfer.FileName;
                if (string.IsNullOrEmpty(fileStatus.Text))
                    fileStatus.Text = Language.GetText(nameof(en_US.InitStatusText));
                fileStatus.Text = fileStatus.Text.Trim(fileStatus.Font, fileStatus.Width);
                statusAreaLeftPanel.ResumeLayout();

                statusAreaRightPanel.SuspendLayout();
                downloadReceived.Text = appTransferor.Transfer.DataReceived;
                if (downloadReceived.Text.EqualsEx("0.00 bytes / 0.00 bytes"))
                    downloadReceived.Text = Language.GetText(nameof(en_US.InitStatusText));
                downloadSpeed.Text = appTransferor.Transfer.TransferSpeedAd;
                if (downloadSpeed.Text.EqualsEx("0.00 bit/s"))
                    downloadSpeed.Text = Language.GetText(nameof(en_US.InitStatusText));
                timeStatus.Text = _transferStopwatch.Elapsed.ToString("mm\\:ss\\.fff");
                statusAreaRightPanel.ResumeLayout();

                if (_transferTask?.Status == TaskStatus.Running || appTransferor.Transfer.IsBusy)
                {
                    _currentTransferFinishTick = 0;
                    return;
                }

                if (_currentTransferFinishTick < (int)Math.Floor(1000d / owner.Interval))
                {
                    _currentTransferFinishTick++;
                    return;
                }

                owner.Enabled = false;
                if (!appTransferor.DownloadStarted)
                    _transferFails.Add(appTransferor.AppData);
                if (appsList.CheckedItems.Count > 0)
                {
                    var listViewItem = _currentTransfer.Key;
                    listViewItem.Checked = false;
                    if (appsList.CheckedItems.Count > 0)
                    {
                        downloadStarter.Enabled = !owner.Enabled;
                        return;
                    }
                }

                Text = Settings.Title;
                _transferStopwatch.Stop();
                TaskBar.Progress.SetState(Handle, TaskBar.Progress.Flags.Indeterminate);

                var windowState = WindowState;
                WindowState = FormWindowState.Minimized;

                var installed = new List<ListViewItem>();
                installed.AddRange(_transferManager.Where(x => x.Value.StartInstall()).Select(x => x.Key));
                if (installed.Any())
                    foreach (var key in installed)
                        _transferManager.Remove(key);
                if (_transferManager.Any())
                    _transferFails.AddRange(_transferManager.Values.Select(x => x.AppData));

                WindowState = windowState;

                _transferManager.Clear();
                _currentTransfer = default(KeyValuePair<ListViewItem, AppTransferor>);

                if (_transferFails.Any())
                {
                    TaskBar.Progress.SetState(Handle, TaskBar.Progress.Flags.Error);
                    var warning = string.Format(Language.GetText(_transferFails.Count == 1 ? nameof(en_US.AppDownloadErrorMsg) : nameof(en_US.AppsDownloadErrorMsg)), _transferFails.Select(x => x.Name).Join(Environment.NewLine));
                    switch (MessageBoxEx.Show(this, warning, Settings.Title, MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning))
                    {
                        case DialogResult.Retry:
                            foreach (var appData in _transferFails)
                            {
                                var item = appsList.Items.Cast<ListViewItem>().FirstOrDefault(x => x.Name.EqualsEx(appData.Key));
                                if (item == default(ListViewItem))
                                    continue;
                                item.Checked = true;
                            }
                            _transferFails.Clear();

                            SuspendLayout();
                            appsList.Enabled = true;
                            appsList.HideSelection = !appsList.Enabled;

                            downloadSpeed.Text = string.Empty;
                            DownloadProgressUpdate(0);
                            downloadReceived.Text = string.Empty;

                            showGroupsCheck.Enabled = appsList.Enabled;
                            showColorsCheck.Enabled = appsList.Enabled;
                            highlightInstalledCheck.Enabled = appsList.Enabled;

                            searchBox.Enabled = appsList.Enabled;

                            startBtn.Enabled = appsList.Enabled;
                            cancelBtn.Enabled = appsList.Enabled;

                            statusAreaBorder.Visible = !appsList.Enabled;
                            statusAreaPanel.Visible = !appsList.Enabled;
                            ResumeLayout();

                            StartBtn_Click(startBtn, EventArgs.Empty);
                            return;
                        default:
                            if (Settings.ActionGuid.IsUpdateInstance)
                                foreach (var appData in _transferFails)
                                {
                                    appData.Settings.NoUpdates = true;
                                    appData.Settings.NoUpdatesTime = DateTime.Now;
                                }
                            break;
                    }
                }

                TaskBar.Progress.SetValue(Handle, 100, 100);

                var information = Language.GetText(Settings.ActionGuid.IsUpdateInstance ? installed.Count == 1 ? nameof(en_US.AppUpdatedMsg) : nameof(en_US.AppsUpdatedMsg) : installed.Count == 1 ? nameof(en_US.AppDownloadedMsg) : nameof(en_US.AppsDownloadedMsg));
                MessageBoxEx.Show(this, information, Settings.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);

                ApplicationExit();
            }
        }

        private void DownloadProgressUpdate(int value)
        {
            var color = PanelEx.FakeProgressBar.Update(downloadProgress, value);
            appStatus.ForeColor = color;
            fileStatus.ForeColor = color;
            urlStatus.ForeColor = color;
            downloadReceived.ForeColor = color;
            downloadSpeed.ForeColor = color;
            timeStatus.ForeColor = color;
        }

        protected static void ApplicationExit(int exitCode = 0)
        {
            if (exitCode > 0)
            {
                Environment.ExitCode = exitCode;
                Environment.Exit(Environment.ExitCode);
            }
            Application.Exit();
        }

        protected override void WndProc(ref Message m)
        {
            var previous = (int)WindowState;
            base.WndProc(ref m);
            var current = (int)WindowState;
            if (previous == 1 || current == 1 || previous == current)
                return;
            AppsListResizeColumns();
        }
    }
}
