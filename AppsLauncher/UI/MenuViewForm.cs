using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;

namespace AppsLauncher
{
    public partial class MenuViewForm : Form
    {
        protected override void WndProc(ref Message m)
        {
            const uint HTLEFT = 10;
            const uint HTRIGHT = 11;
            const uint HTBOTTOMRIGHT = 17;
            const uint HTBOTTOM = 15;
            const uint HTBOTTOMLEFT = 16;
            const uint HTTOP = 12;
            const uint HTTOPRIGHT = 14;
            bool handled = false;
            if (m.Msg == 0x0084 || m.Msg == 0x0200)
            {
                Size wndSize = Size;
                Point scrPoint = new Point(m.LParam.ToInt32());
                Point clntPoint = PointToClient(scrPoint);
                Dictionary<uint, Rectangle> hitBoxes = new Dictionary<uint, Rectangle>();
                switch (SilDev.Taskbar.GetLocation())
                {
                    case SilDev.Taskbar.Location.LEFT:
                    case SilDev.Taskbar.Location.TOP:
                        hitBoxes.Add(HTRIGHT, new Rectangle(wndSize.Width - 8, 8, 8, wndSize.Height - 2 * 8));
                        hitBoxes.Add(HTBOTTOMRIGHT, new Rectangle(wndSize.Width - 8, wndSize.Height - 8, 8, 8));
                        hitBoxes.Add(HTBOTTOM, new Rectangle(8, wndSize.Height - 8, wndSize.Width - 2 * 8, 8));
                        break;
                    case SilDev.Taskbar.Location.RIGHT:
                        hitBoxes.Add(HTLEFT, new Rectangle(0, 8, 8, wndSize.Height - 2 * 8));
                        hitBoxes.Add(HTBOTTOMLEFT, new Rectangle(0, wndSize.Height - 8, 8, 8));
                        hitBoxes.Add(HTBOTTOM, new Rectangle(8, wndSize.Height - 8, wndSize.Width - 2 * 8, 8));
                        break;
                    default:
                        hitBoxes.Add(HTRIGHT, new Rectangle(wndSize.Width - 8, 8, 8, wndSize.Height - 2 * 8));
                        hitBoxes.Add(HTTOPRIGHT, new Rectangle(wndSize.Width - 8, 0, 8, 8));
                        hitBoxes.Add(HTTOP, new Rectangle(8, 0, wndSize.Width - 2 * 8, 8));
                        break;
                }
                foreach (KeyValuePair<uint, Rectangle> hitBox in hitBoxes)
                {
                    if (hitBox.Value.Contains(clntPoint))
                    {
                        m.Result = (IntPtr)hitBox.Key;
                        handled = true;
                        break;
                    }
                }
            }
            if (!handled)
                base.WndProc(ref m);
        }

        double WindowOpacity = .95f;
        int WindowFadeInDuration = 1;
        bool AppStartEventCalled = false;
        string SearchText = string.Empty;

        public MenuViewForm()
        {
            InitializeComponent();
#if !x86
            Text = $"{Text} (64-bit)";
#endif
            Icon = Properties.Resources.PortableApps_blue;
            BackColor = Color.FromArgb(255, Main.Colors.Layout.R, Main.Colors.Layout.G, Main.Colors.Layout.B);
            tableLayoutPanel1.BackgroundImage = Main.LayoutBackground;
            tableLayoutPanel1.BackColor = Main.Colors.Layout;
            appsListView.ForeColor = Main.Colors.ControlText;
            appsListView.BackColor = Main.Colors.Control;
            aboutBtn.BackgroundImage = Main.ImageGrayScaleSwitch($"{aboutBtn.Name}BackgroundImage", aboutBtn.BackgroundImage);
            searchBox.ForeColor = Main.Colors.ControlText;
            searchBox.BackColor = Main.Colors.Control;
            foreach (Button btn in new Button[] { downloadBtn, settingsBtn })
            {
                btn.ForeColor = Main.Colors.ButtonText;
                btn.BackColor = Main.Colors.Button;
                btn.FlatAppearance.MouseDownBackColor = Main.Colors.Button;
                btn.FlatAppearance.MouseOverBackColor = Main.Colors.ButtonHover;
            }
            logoBox.Image = Main.ImageFilter(Properties.Resources.PortableApps_Logo_gray, logoBox.Height, logoBox.Height);
            if (!searchBox.Focus())
                searchBox.Select();
        }

        private void MenuViewForm_Load(object sender, EventArgs e)
        {
            Lang.SetControlLang(this);
            for (int i = 0; i < appMenu.Items.Count; i++)
                appMenu.Items[i].Text = Lang.GetText(appMenu.Items[i].Name);

            if (!Directory.Exists(Main.AppsPath))
                Main.RepairAppsLauncher();

            WindowOpacity = SilDev.Ini.ReadDouble("Settings", "WindowOpacity", 95);
            if (WindowOpacity >= 20d && WindowOpacity <= 100d)
                WindowOpacity /= 100d;
            else
                WindowOpacity = .95d;

            WindowFadeInDuration = SilDev.Ini.ReadInteger("Settings", "WindowFadeInDuration", 1);
            if (WindowFadeInDuration < 1)
                WindowFadeInDuration = 1;
            int opacity = (int)(WindowOpacity * 100d);
            if (WindowFadeInDuration > opacity)
                WindowFadeInDuration = opacity;

            int WindowWidth = SilDev.Ini.ReadInteger("Settings", "WindowWidth", MinimumSize.Width);
            if (WindowWidth > MinimumSize.Width && WindowWidth < Screen.PrimaryScreen.WorkingArea.Width)
                Width = WindowWidth;
            if (WindowWidth > Screen.PrimaryScreen.WorkingArea.Width)
                Width = Screen.PrimaryScreen.WorkingArea.Width;

            int WindowHeight = SilDev.Ini.ReadInteger("Settings", "WindowHeight", MinimumSize.Height);
            if (WindowHeight > MinimumSize.Height && WindowHeight < Screen.PrimaryScreen.WorkingArea.Height)
                Height = WindowHeight;
            if (WindowHeight > Screen.PrimaryScreen.WorkingArea.Height)
                Height = Screen.PrimaryScreen.WorkingArea.Height;

            MenuViewForm_Update();

            if (!searchBox.Focus())
                searchBox.Select();

            if (!fadeInTimer.Enabled)
                fadeInTimer.Enabled = true;
        }

        private void MenuViewForm_Deactivate(object sender, EventArgs e)
        {
            if (Application.OpenForms.Count > 1 || appMenu.Focus() || AppStartEventCalled)
                return;
            if (!ClientRectangle.Contains(PointToClient(MousePosition)))
                Close();
        }

        private void MenuViewForm_ResizeBegin(object sender, EventArgs e)
        {
            if (!appsListView.Scrollable)
                appsListView.Scrollable = true;
            tableLayoutPanel1.BackgroundImage = null;
        }

        private void MenuViewForm_ResizeEnd(object sender, EventArgs e)
        {
            if (!searchBox.Focus())
                searchBox.Select();
            SilDev.Ini.Write("Settings", "WindowWidth", Width);
            SilDev.Ini.Write("Settings", "WindowHeight", Height);
            tableLayoutPanel1.BackgroundImage = Main.LayoutBackground;
        }

        private void MenuViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            AppStartEventCalled = true;
            if (Opacity != 0)
                Opacity = 0;

            if (File.Exists(SilDev.Ini.File()))
            {
                string tmpIniPath = $"{SilDev.Ini.File()}.tmp";
                if (!File.Exists(tmpIniPath))
                {
                    try
                    {
                        File.Create(tmpIniPath).Close();
                    }
                    catch (Exception ex)
                    {
                        SilDev.Log.Debug(ex);
                    }
                }
                if (File.Exists(tmpIniPath))
                {
                    List <string> sections = new List<string>();
                    sections.Add("History");
                    sections.Add("Host");
                    sections.Add("Settings");
                    if (Main.AppConfigs.Count > 0)
                    {
                        Main.AppConfigs.Sort();
                        sections.AddRange(Main.AppConfigs);
                    }
                    foreach (string section in sections)
                    {
                        List<string> keys = SilDev.Ini.GetKeys(section);
                        if (keys.Count == 0)
                            continue;
                        foreach (string key in keys)
                        {
                            string value = SilDev.Ini.Read(section, key);
                            if (string.IsNullOrWhiteSpace(value))
                                continue;
                            SilDev.Ini.Write(section, key, value, tmpIniPath);
                        }
                        File.AppendAllText(tmpIniPath, Environment.NewLine);
                    }
                    try
                    {
                        File.WriteAllText(SilDev.Ini.File(), File.ReadAllText(tmpIniPath));
                        File.Delete(tmpIniPath);
                    }
                    catch (Exception ex)
                    {
                        SilDev.Log.Debug(ex);
                    }
                }
            }

            bool StartMenuIntegration = SilDev.Ini.ReadBoolean("Settings", "StartMenuIntegration");
            if (StartMenuIntegration)
            {
                try
                {
                    List<string> list = new List<string>();
                    for (int i = 0; i < appsListView.Items.Count; i++)
                        list.Add(appsListView.Items[i].Text);
                    Main.StartMenuFolderUpdate(list);
                }
                catch (Exception ex)
                {
                    SilDev.Log.Debug(ex);
                }
            }
        }

        private void MenuViewForm_FormClosed(object sender, FormClosedEventArgs e) =>
            Main.SearchForUpdates();

        private void MenuViewForm_Update(bool setWindowLocation = true)
        {
            try
            {
                Main.CheckAvailableApps();
                appsListView.BeginUpdate();
                appsListView.Items.Clear();
                if (!appsListView.Scrollable)
                    appsListView.Scrollable = true;
                imgList.Images.Clear();
                string CacheFile = Path.Combine(Application.StartupPath, "Assets\\icon.tmp");
                try
                {
                    if (!File.Exists(CacheFile))
                        File.Create(CacheFile).Close();
                }
                catch (Exception ex)
                {
                    SilDev.Log.Debug(ex);
                    if (!SilDev.Elevation.IsAdministrator)
                        SilDev.Elevation.RestartAsAdministrator(Main.CmdLine);
                }
                byte[] IcoDb = null;
                try
                {
                    IcoDb = File.ReadAllBytes(Path.Combine(Application.StartupPath, "Assets\\icon.db"));
                }
                catch (Exception ex)
                {
                    SilDev.Log.Debug(ex);
                }
                Image DefaultExeIcon = Main.ImageFilter(Properties.Resources.executable, 16, 16);
                for (int i = 0; i < Main.AppsList.Count; i++)
                {
                    appsListView.Items.Add(Main.AppsList[i], i);
                    string nameHash = SilDev.Crypt.MD5.EncryptString(Main.AppsDict[Main.AppsList[i]]);
                    try
                    {
                        if (File.Exists(CacheFile))
                        {
                            Image img = SilDev.Ini.ReadImage("Cache", nameHash, CacheFile);
                            if (img != null)
                            {
                                if (SilDev.Log.DebugMode > 1 && Main.CmdLineArray.Contains("{0CA7046C-4776-4DB0-913B-D8F81964F8EE}"))
                                {
                                    try
                                    {
                                        string imgDir = SilDev.Run.EnvironmentVariableFilter("%CurrntDir%\\Assets\\Images");
                                        if (!Directory.Exists(imgDir))
                                            Directory.CreateDirectory(imgDir);
                                        img.Save(Path.Combine(imgDir, nameHash));
                                        string imgIni = Path.Combine(imgDir, "_list.ini");
                                        if (!File.Exists(imgIni))
                                            File.Create(imgIni).Close();
                                        SilDev.Ini.Write("list", nameHash, Main.AppsDict[Main.AppsList[i]], imgIni);
                                    }
                                    catch (Exception ex)
                                    {
                                        SilDev.Log.Debug(ex);
                                    }
                                }
                                imgList.Images.Add(nameHash, img);
                            }
                        }
                        if (imgList.Images.ContainsKey(nameHash))
                            continue;
                        if (IcoDb != null)
                        {
                            using (MemoryStream stream = new MemoryStream(IcoDb))
                            {
                                try
                                {
                                    using (ZipArchive archive = new ZipArchive(stream))
                                    {
                                        foreach (ZipArchiveEntry entry in archive.Entries)
                                        {
                                            if (entry.Name != nameHash)
                                                continue;
                                            Image img = Image.FromStream(entry.Open());
                                            imgList.Images.Add(nameHash, img);
                                            SilDev.Ini.Write("Cache", nameHash, img, CacheFile);
                                            break;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    SilDev.Log.Debug(ex);
                                }
                            }
                        }
                        if (imgList.Images.ContainsKey(nameHash))
                            continue;
                        string appPath = Main.GetAppPath(Main.AppsDict[Main.AppsList[i]]);
                        string imgPath = Path.Combine(Path.GetDirectoryName(appPath), $"{Path.GetFileNameWithoutExtension(appPath)}.png");
                        if (!File.Exists(imgPath))
                        {
                            if (imgList.Images.ContainsKey(nameHash))
                                continue;
                            if (!File.Exists(imgPath))
                                imgPath = Path.Combine(Path.GetDirectoryName(appPath), "App\\AppInfo\\appicon_16.png");
                            if (File.Exists(imgPath))
                            {
                                Image imgFromFile = Main.ImageFilter(Image.FromFile(imgPath), 16, 16);
                                imgList.Images.Add(nameHash, imgFromFile);
                                SilDev.Ini.Write("Cache", nameHash, imgFromFile, CacheFile);
                            }
                            if (imgList.Images.ContainsKey(nameHash))
                                continue;
                            Icon ico = Main.IconResourceFromFile(appPath);
                            if (ico != null)
                            {
                                Image imgFromIcon = Main.ImageFilter(ico.ToBitmap(), 16, 16);
                                imgList.Images.Add(nameHash, imgFromIcon);
                                SilDev.Ini.Write("Cache", nameHash, imgFromIcon, CacheFile);
                                continue;
                            }
                            throw new Exception();
                        }
                        else
                        {
                            Image img = Image.FromFile(imgPath);
                            imgList.Images.Add(nameHash, img);
                            SilDev.Ini.Write("Cache", nameHash, img, CacheFile);
                        }
                    }
                    catch
                    {
                        imgList.Images.Add(nameHash, DefaultExeIcon);
                    }
                }
                appsListView.SmallImageList = imgList;
                if (setWindowLocation)
                {
                    int defaultPos = SilDev.Ini.ReadInteger("Settings", "DefaultPosition", 0);
                    if (defaultPos == 0)
                    {
                        switch (SilDev.Taskbar.GetLocation())
                        {
                            case SilDev.Taskbar.Location.LEFT:
                                Left = Screen.PrimaryScreen.WorkingArea.X;
                                Top = 0;
                                break;
                            case SilDev.Taskbar.Location.TOP:
                                Left = 0;
                                Top = Screen.PrimaryScreen.WorkingArea.Y;
                                break;
                            case SilDev.Taskbar.Location.RIGHT:
                                Left = Screen.PrimaryScreen.WorkingArea.Width - Width;
                                Top = 0;
                                break;
                            default:
                                Left = 0;
                                Top = Screen.PrimaryScreen.WorkingArea.Height - Height;
                                break;
                        }
                    }
                    else
                    {
                        Point newLocation = GetWindowStartPos(new Point(Width, Height));
                        Left = newLocation.X;
                        Top = newLocation.Y;
                    }
                }
                appsListView.EndUpdate();
                appsCount.Text = string.Format(Lang.GetText(appsCount), appsListView.Items.Count, appsListView.Items.Count == 1 ? Lang.GetText("App") : Lang.GetText("Apps"));
            }
            catch (Exception ex)
            {
                SilDev.Log.Debug(ex);
                Environment.Exit(Environment.ExitCode);
            }
        }

        private Point GetWindowStartPos(Point point)
        {
            Point p = new Point();
            int defaultPos = SilDev.Ini.ReadInteger("Settings", "DefaultPosition", 0);
            if (defaultPos == 0)
            {
                switch (SilDev.Taskbar.GetLocation())
                {
                    case SilDev.Taskbar.Location.LEFT:
                        p.X = Cursor.Position.X - (point.X / 2);
                        p.Y = Cursor.Position.Y;
                        break;
                    case SilDev.Taskbar.Location.TOP:
                        p.X = Cursor.Position.X - (point.X / 2);
                        p.Y = Cursor.Position.Y;
                        break;
                    case SilDev.Taskbar.Location.RIGHT:
                        p.X = Screen.PrimaryScreen.WorkingArea.Width - point.X;
                        p.Y = Cursor.Position.Y;
                        break;
                    default:
                        p.X = Cursor.Position.X - (point.X / 2);
                        p.Y = Cursor.Position.Y - point.Y;
                        break;
                }
                if (p.X + point.X > Screen.PrimaryScreen.WorkingArea.Width)
                    p.Y = Screen.PrimaryScreen.WorkingArea.Width - point.X;
                if (p.Y + point.Y > Screen.PrimaryScreen.WorkingArea.Height)
                    p.Y = Screen.PrimaryScreen.WorkingArea.Height - point.Y;
            }
            else
            {
                int maxWidth = Screen.PrimaryScreen.WorkingArea.Width - point.X;
                p.X = Cursor.Position.X > point.X / 2 && Cursor.Position.X < maxWidth ? Cursor.Position.X - point.X / 2 : Cursor.Position.X > maxWidth ? maxWidth : Cursor.Position.X;
                int maxHeight = Screen.PrimaryScreen.WorkingArea.Height - point.Y;
                p.Y = Cursor.Position.Y > point.Y / 2 && Cursor.Position.Y < maxHeight ? Cursor.Position.Y - point.Y / 2 : Cursor.Position.Y > maxHeight ? maxHeight : Cursor.Position.Y;
            }
            return p;
        }

        private void fadeInTimer_Tick(object sender, EventArgs e)
        {
            if (Opacity < WindowOpacity)
                Opacity += WindowOpacity / WindowFadeInDuration;
            else
            {
                fadeInTimer.Enabled = false;
                if (SilDev.WinAPI.SafeNativeMethods.GetForegroundWindow() != Handle)
                    SilDev.WinAPI.SafeNativeMethods.SetForegroundWindow(Handle);
            }
        }

        private void appsListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;
            if (appsListView.SelectedItems.Count > 0)
            {
                AppStartEventCalled = true;
                if (Opacity != 0)
                    Opacity = 0;
                Main.StartApp(appsListView.SelectedItems[0].Text, true);
            }
        }

        private void appsListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2 && appsListView.SelectedItems.Count > 0)
            {
                if (!appsListView.LabelEdit)
                    appsListView.LabelEdit = true;
                appsListView.SelectedItems[0].BeginEdit();
            }
        }

        private void appsListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Label))
            {
                try
                {
                    string appPath = Main.GetAppPath(Main.AppsDict[appsListView.SelectedItems[0].Text]);
                    string appIniPath = Path.Combine(Path.GetDirectoryName(appPath), $"{Path.GetFileName(Path.GetDirectoryName(appPath))}.ini");
                    if (!File.Exists(appIniPath))
                        File.Create(appIniPath).Close();
                    SilDev.Ini.Write("AppInfo", "Name", e.Label, appIniPath);
                }
                catch (Exception ex)
                {
                    SilDev.Log.Debug(ex);
                }
                MenuViewForm_Update();
            }
        }

        private void appMenu_Opening(object sender, CancelEventArgs e) =>
            e.Cancel = appsListView.SelectedItems.Count == 0;

        private void appMenu_Opened(object sender, EventArgs e)
        {
            ContextMenuStrip cms = (ContextMenuStrip)sender;
            cms.Left -= 48;
            cms.Top -= 10;
        }

        private void appMenu_Paint(object sender, PaintEventArgs e)
        {
            ContextMenuStrip cms = (ContextMenuStrip)sender;
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddRectangle(new RectangleF(2, 2, cms.Width - 4, cms.Height - 4));
                cms.Region = new Region(gp);
            }
        }

        private void appMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem i = (ToolStripMenuItem)sender;
            switch (i.Name)
            {
                case "appMenuItem1":
                case "appMenuItem2":
                case "appMenuItem3":
                    if (Opacity != 0)
                        Opacity = 0;
                    switch (i.Name)
                    {
                        case "appMenuItem1":
                        case "appMenuItem2":
                            Main.StartApp(appsListView.SelectedItems[0].Text, true, i.Name == "appMenuItem2");
                            break;
                        case "appMenuItem3":
                            Main.OpenAppLocation(appsListView.SelectedItems[0].Text, true);
                            break;
                    }
                    break;
                case "appMenuItem4":
                    SilDev.MsgBox.MoveCursorToMsgBoxAtOwner = !ClientRectangle.Contains(PointToClient(MousePosition));
                    if (SilDev.Data.CreateShortcut(Main.GetAppPath(Main.AppsDict[appsListView.SelectedItems[0].Text]), Path.Combine("%DesktopDir%", appsListView.SelectedItems[0].Text)))
                        SilDev.MsgBox.Show(this, Lang.GetText("appMenuItem4Msg0"), Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    else
                        SilDev.MsgBox.Show(this, Lang.GetText("appMenuItem4Msg1"), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case "appMenuItem5":
                    SilDev.MsgBox.MoveCursorToMsgBoxAtOwner = !ClientRectangle.Contains(PointToClient(MousePosition));
                    bool pinned = false;
                    string appPath = Main.GetAppPath(Main.AppsDict[appsListView.SelectedItems[0].Text]);
                    if (Environment.OSVersion.Version.Major < 10)
                        pinned = SilDev.Data.PinToTaskbar(appPath);
                    else
                    {
                        // Temporary solution for Windows 10
                        int pid = SilDev.Run.App(new ProcessStartInfo()
                        {
                            Arguments = $"\"{appPath}\" c:5386",
                            FileName = "%CurrentDir%\\Binaries\\Helper\\syspin\\syspin.exe",
                            WindowStyle = ProcessWindowStyle.Hidden
                        });
                        pinned = pid > 0;
                    }
                    if (pinned)
                        SilDev.MsgBox.Show(this, Lang.GetText("appMenuItem4Msg0"), Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    else
                        SilDev.MsgBox.Show(this, Lang.GetText("appMenuItem4Msg1"), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case "appMenuItem6":
                    if (appsListView.SelectedItems.Count > 0)
                    {
                        if (!appsListView.LabelEdit)
                            appsListView.LabelEdit = true;
                        appsListView.SelectedItems[0].BeginEdit();
                    }
                    break;
                case "appMenuItem7":
                    SilDev.MsgBox.MoveCursorToMsgBoxAtOwner = !ClientRectangle.Contains(PointToClient(MousePosition));
                    if (SilDev.MsgBox.Show(this, string.Format(Lang.GetText("appMenuItem7Msg"), appsListView.SelectedItems[0].Text), Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        SilDev.MsgBox.MoveCursorToMsgBoxAtOwner = !ClientRectangle.Contains(PointToClient(MousePosition));
                        try
                        {
                            string appDir = Path.GetDirectoryName(Main.GetAppPath(Main.AppsDict[appsListView.SelectedItems[0].Text]));
                            if (Directory.Exists(appDir))
                            {
                                Directory.Delete(appDir, true);
                                MenuViewForm_Update(false);
                                SilDev.MsgBox.Show(this, Lang.GetText("OperationCompletedMsg"), Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                        }
                        catch (Exception ex)
                        {
                            SilDev.MsgBox.Show(this, Lang.GetText("OperationFailedMsg"), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            SilDev.Log.Debug(ex);
                        }
                    }
                    else
                    {
                        SilDev.MsgBox.MoveCursorToMsgBoxAtOwner = !ClientRectangle.Contains(PointToClient(MousePosition));
                        SilDev.MsgBox.Show(this, Lang.GetText("OperationCanceledMsg"), Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    break;
            }
            if (SilDev.MsgBox.MoveCursorToMsgBoxAtOwner)
                SilDev.MsgBox.MoveCursorToMsgBoxAtOwner = false;
        }

        private void appMenu_MouseLeave(object sender, EventArgs e) =>
            appMenu.Close();

        private void ImageButton_MouseEnterLeave(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                Button b = (Button)sender;
                if (b.BackgroundImage != null)
                    b.BackgroundImage = Main.ImageGrayScaleSwitch($"{b.Name}BackgroundImage", b.BackgroundImage);
                if (b.Image != null)
                    b.Image = Main.ImageGrayScaleSwitch($"{b.Name}Image", b.Image);
                return;
            }
            if (sender is PictureBox)
            {
                PictureBox pb = (PictureBox)sender;
                if (pb.BackgroundImage != null)
                    pb.BackgroundImage = Main.ImageGrayScaleSwitch($"{pb.Name}BackgroundImage", pb.BackgroundImage);
                if (pb.Image != null)
                    pb.Image = Main.ImageGrayScaleSwitch($"{pb.Name}Image", pb.Image);
            }
        }

        private void openNewFormBtn_Click(object sender, EventArgs e)
        {
            Form form;
            if (sender is Button)
                form = new SettingsForm(appsListView.SelectedItems.Count > 0 ? appsListView.SelectedItems[0].Text : string.Empty);
            else if (sender is PictureBox)
                form = new AboutForm();
            else
                return;
            if (TopMost)
                TopMost = false;
            bool result = false;
            try
            {
                using (Form dialog = form)
                {
                    Point point = GetWindowStartPos(new Point(dialog.Width, dialog.Height));
                    if (point != new Point(0, 0))
                    {
                        dialog.StartPosition = FormStartPosition.Manual;
                        dialog.Left = point.X;
                        dialog.Top = point.Y;
                    }
                    result = dialog.ShowDialog() == DialogResult.Yes;
                }
            }
            catch (Exception ex)
            {
                SilDev.Log.Debug(ex);
            }
            if (result && sender is Button)
            {
                SilDev.Run.App(new ProcessStartInfo()
                {
                    Arguments = "{17762FDA-39B3-4224-9525-B1A4DF75FA02}",
                    FileName = Application.ExecutablePath
                });
                Close();
            }
            else
            {
                if (!TopMost)
                    TopMost = true;
                if (SilDev.WinAPI.SafeNativeMethods.GetForegroundWindow() != Handle)
                    SilDev.WinAPI.SafeNativeMethods.SetForegroundWindow(Handle);
                if (!searchBox.Focus())
                    searchBox.Select();
            }
        }

        private void profileBtn_Click(object sender, EventArgs e)
        {
            SilDev.Run.App(new ProcessStartInfo()
            {
                Arguments = Path.Combine(Application.StartupPath, "Documents"),
                FileName = "%WinDir%\\explorer.exe"
            });
            Close();
        }

        private void downloadBtn_Click(object sender, EventArgs e)
        {
            Main.SkipUpdateSearch = true;
#if x86
            SilDev.Run.App(new ProcessStartInfo() { FileName = "%CurrentDir%\\Binaries\\AppsDownloader.exe" });
#else
            SilDev.Run.App(new ProcessStartInfo() { FileName = "%CurrentDir%\\Binaries\\AppsDownloader64.exe" });
#endif
            Close();
        }

        private void searchBox_Enter(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Font = new Font("Segoe UI", 8.25F);
            tb.ForeColor = Main.Colors.ControlText;
            tb.Text = SearchText;
        }

        private void searchBox_Leave(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Font = new Font("Comic Sans MS", 8.25F, FontStyle.Italic);
            tb.ForeColor = SystemColors.GrayText;
            SearchText = tb.Text;
            string text = Lang.GetText(tb).Replace(" ", string.Empty).ToLower();
            tb.Text = $"{text.Substring(0, 1).ToUpper()}{text.Substring(1)}";
        }

        private void searchBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (appsListView.SelectedItems.Count > 0)
                {
                    AppStartEventCalled = true;
                    Main.StartApp(appsListView.SelectedItems[0].Text, true);
                }
                return;
            }
            ((TextBox)sender).Refresh();
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            List<string> itemList = new List<string>();
            foreach (ListViewItem item in appsListView.Items)
            {
                item.ForeColor = Main.Colors.ControlText;
                item.BackColor = Main.Colors.Control;
                itemList.Add(item.Text);
            }
            if (string.IsNullOrWhiteSpace(tb.Text) || tb.Font.Italic)
                return;
            foreach (ListViewItem item in appsListView.Items)
            {
                if (item.Text == Main.SearchMatchItem(tb.Text, itemList))
                {
                    item.ForeColor = SystemColors.Control;
                    item.BackColor = SystemColors.HotTrack;
                    item.Selected = true;
                    item.Focused = true;
                    break;
                }
            }
        }

        private void closeBtn_Click(object sender, EventArgs e) =>
            Application.Exit();
    }
}
