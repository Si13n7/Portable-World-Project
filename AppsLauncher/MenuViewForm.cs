﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace AppsLauncher
{
    public partial class MenuViewForm : Form
    {
        bool CloseAtDeactivateEvent = true;

        public MenuViewForm()
        {
            InitializeComponent();
#if !x86
            Text = string.Format("{0} (64-bit)", Text);
#endif
            Icon = Properties.Resources.PortableApps_blue;
            label1.BackColor = Main.LayoutColor;
            label2.BackColor = Main.LayoutColor;
            label3.BackColor = Main.LayoutColor;
            label4.BackColor = Main.LayoutColor;
            tableLayoutPanel1.BackColor = Main.LayoutColor;
            downloadBtn.FlatAppearance.MouseOverBackColor = Main.LayoutColor;
            settingsBtn.FlatAppearance.MouseOverBackColor = Main.LayoutColor;
            logoBox.Image = ImageHighQualityResize(Properties.Resources.PortableApps_Logo_gray, logoBox.Height, logoBox.Height);
            searchBox.Select();
        }

        private void MenuViewForm_Load(object sender, EventArgs e)
        {
            Lang.SetControlLang(this);
            for (int i = 0; i < appMenu.Items.Count; i++)
                appMenu.Items[i].Text = Lang.GetText(appMenu.Items[i].Name);
            if (!Directory.Exists(Main.AppsPath))
                Main.RepairAppsLauncher();
            MenuViewForm_Update();
            CloseAtDeactivateEvent = false;
            Main.CheckUpdates();
            CloseAtDeactivateEvent = true;
        }

        private void MenuViewForm_Activated(object sender, EventArgs e)
        {
            CloseAtDeactivateEvent = true;
        }

        private void MenuViewForm_Deactivate(object sender, EventArgs e)
        {
            if (CloseAtDeactivateEvent && SilDev.Log.DebugMode < 1)
                Close();
        }

        private void MenuViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            int StartMenuIntegration = 0;
            int.TryParse(SilDev.Initialization.ReadValue("Settings", "StartMenuIntegration"), out StartMenuIntegration);
            if (StartMenuIntegration > 0)
            {
                List<string> list = new List<string>();
                for (int i = 0; i < appsListView.Items.Count; i++)
                    list.Add(appsListView.Items[i].Text);
                Main.StartMenuFolderUpdate(list);
            }
        }

        private void MenuViewForm_Update()
        {
            Main.CheckAvailableApps();
            appsListView.BeginUpdate();
            appsListView.Items.Clear();
            imgList.Images.Clear();
            string ImageCacheDir = Path.Combine(Application.StartupPath, "Assets\\cache");
            Image DefaultExeIcon = ImageHighQualityResize(Properties.Resources.executable, 16, 16);
            for (int i = 0; i < Main.AppsList.Count; i++)
            {
                appsListView.Items.Add(Main.AppsList[i], i);
                try
                {
                    string appPath = Main.GetAppPath(Main.AppsDict[Main.AppsList[i]]);
                    string appNameHash = SilDev.Crypt.MD5.Encrypt(Main.AppsList[i]);
                    string img16Path = Path.Combine(ImageCacheDir, appNameHash);
                    if (!File.Exists(img16Path))
                        img16Path = Path.Combine(Path.GetDirectoryName(appPath), "appicon.png");
                    if (!File.Exists(img16Path))
                        img16Path = Path.Combine(Path.GetDirectoryName(appPath), "App\\AppInfo\\appicon_16.png");
                    if (!File.Exists(img16Path))
                    {
                        Icon ico = GetSmallIcon(appPath);
                        if (ico != null)
                        {
                            Image img = ImageHighQualityResize(ico.ToBitmap(), 16, 16);
                            if (!Directory.Exists(ImageCacheDir))
                                Directory.CreateDirectory(ImageCacheDir);
                            img.Save(Path.Combine(ImageCacheDir, appNameHash));
                            imgList.Images.Add(ImageHighQualityResize(ico.ToBitmap(), 16, 16));
                        }
                        else
                            throw new Exception();
                    }
                    else
                        imgList.Images.Add(ImageHighQualityResize(Image.FromFile(img16Path), 16, 16));
                }
                catch
                {
                    imgList.Images.Add(DefaultExeIcon);
                }
            }
            appsListView.SmallImageList = imgList;
            int width = 400;
            int height = 400;
            bool scrollable = false;
            if (appsListView.Items.Count > 20)
            {
                scrollable = true;
                if (!appsListView.Scrollable)
                    appsListView.Scrollable = true;
                int multiplier = 0;
                for (int i = 20; i < appsListView.Items.Count; i++)
                {
                    if (i == 36 || height + 8 >= Screen.PrimaryScreen.WorkingArea.Height - 64)
                        break;
                    height += 20;
                    multiplier = i;
                }
                if (appsListView.Items.Count > multiplier + 20)
                {
                    int columns = 0;
                    int maxLen = 0;
                    for (int i = 0; i < appsListView.Items.Count; i++)
                    {
                        if (i % multiplier == 0)
                            columns++;
                        if (maxLen < appsListView.Items[i].Text.Length)
                            maxLen = appsListView.Items[i].Text.Length;
                    }
                    width = ((maxLen * columns) * 8) - 48;
                }
                else
                    scrollable = false;
            }
            if (width > Screen.PrimaryScreen.WorkingArea.Width)
                width = Screen.PrimaryScreen.WorkingArea.Width;
            if (Width != width)
                Width = width;
            if (Height != height)
                Height = height;
            if (appsListView.Scrollable && !scrollable)
                appsListView.Scrollable = false;
            switch (SilDev.WinAPI.GetTaskBarLocation())
            {
                case SilDev.WinAPI.Location.LEFT:
                    Left = Screen.PrimaryScreen.WorkingArea.X;
                    Top = 0;
                    break;
                case SilDev.WinAPI.Location.TOP:
                    Left = 0;
                    Top = Screen.PrimaryScreen.WorkingArea.Y;
                    break;
                case SilDev.WinAPI.Location.RIGHT:
                    Left = Screen.PrimaryScreen.WorkingArea.Width - Width;
                    Top = 0;
                    break;
                default:
                    Left = 0;
                    Top = Screen.PrimaryScreen.WorkingArea.Height - Height;
                    break;
            }
            appsListView.EndUpdate();
            appsCount.Text = string.Format(Lang.GetText(appsCount), appsListView.Items.Count.ToString());
        }

        private void appsListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;
            if (appsListView.SelectedItems.Count > 0)
                Main.StartApp(appsListView.SelectedItems[0].Text, true);
        }

        private void appsListView_ItemMouseHover(object sender, ListViewItemMouseHoverEventArgs e)
        {
            e.Item.Selected = true;
        }

        private void appMenu_Opening(object sender, CancelEventArgs e)
        {
            if (appsListView.SelectedItems.Count == 0)
            {
                e.Cancel = true;
                return;
            }
            CloseAtDeactivateEvent = false;
        }

        private void appMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem i = (ToolStripMenuItem)sender;
            switch (i.Name)
            {
                case "appMenuItem1":
                    Main.StartApp(appsListView.SelectedItems[0].Text, !string.IsNullOrWhiteSpace(Main.CmdLineApp));
                    break;
                case "appMenuItem2":
                    Main.StartApp(appsListView.SelectedItems[0].Text, !string.IsNullOrWhiteSpace(Main.CmdLineApp), true);
                    break;
                case "appMenuItem3":
                    Main.OpenAppLocation(appsListView.SelectedItems[0].Text);
                    break;
                case "appMenuItem4":
                    if (SilDev.Data.CreateShortcut(Main.GetAppPath(Main.AppsDict[appsListView.SelectedItems[0].Text]), Path.Combine("%DesktopDir%", appsListView.SelectedItems[0].Text)))
                        SilDev.MsgBox.Show(this, Lang.GetText("ShortcutCreatedMsg0"), Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    else
                        SilDev.MsgBox.Show(this, Lang.GetText("ShortcutCreatedMsg1"), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case "appMenuItem5":
                    if (appsListView.SelectedItems.Count > 0)
                    {
                        if (!appsListView.LabelEdit)
                            appsListView.LabelEdit = true;
                        appsListView.SelectedItems[0].BeginEdit();
                    }
                    break;
                case "appMenuItem6":
                    if (SilDev.MsgBox.Show(this, string.Format(Lang.GetText("appMenuItem5Msg"), appsListView.SelectedItems[0].Text), Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            string appDir = Path.GetDirectoryName(Main.GetAppPath(Main.AppsDict[appsListView.SelectedItems[0].Text]));
                            if (Directory.Exists(appDir))
                            {
                                Directory.Delete(appDir, true);
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
                        SilDev.MsgBox.Show(this, Lang.GetText("OperationCanceledMsg"), Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    break;
            }
            CloseAtDeactivateEvent = true;
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
                    string appIniPath = Path.Combine(Path.GetDirectoryName(appPath), string.Format("{0}.ini", Path.GetFileName(Path.GetDirectoryName(appPath))));
                    if (!File.Exists(appIniPath))
                        File.Create(appIniPath).Close();
                    SilDev.Initialization.WriteValue("AppInfo", "Name", e.Label, appIniPath);
                }
                catch (Exception ex)
                {
                    SilDev.Log.Debug(ex);
                }
                MenuViewForm_Update();
            }
        }

        private void aboutBtn_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAtDeactivateEvent = false;
                using (Form dialog = new AboutForm())
                {
                    Point point = GetWindowStartPos(new Point(dialog.Width, dialog.Height));
                    if (point != new Point(0, 0))
                    {
                        dialog.StartPosition = FormStartPosition.Manual;
                        dialog.Left = point.X;
                        dialog.Top = point.Y;
                    }
                    dialog.TopMost = TopMost;
                    dialog.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                SilDev.Log.Debug(ex);
            }
        }

        private void downloadBtn_Click(object sender, EventArgs e)
        {
#if x86
            SilDev.Run.App(new ProcessStartInfo() { FileName = Path.Combine(Application.StartupPath, "Binaries\\AppsDownloader.exe") });
#else
            SilDev.Run.App(new ProcessStartInfo() { FileName = Path.Combine(Application.StartupPath, "Binaries\\AppsDownloader64.exe") });
#endif
            Close();
        }

        private void settingsBtn_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAtDeactivateEvent = false;
                using (Form dialog = new SettingsForm(appsListView.SelectedItems.Count > 0 ? appsListView.SelectedItems[0].Text : string.Empty))
                {
                    Point point = GetWindowStartPos(new Point(dialog.Width, dialog.Height));
                    if (point != new Point(0, 0))
                    {
                        dialog.StartPosition = FormStartPosition.Manual;
                        dialog.Left = point.X;
                        dialog.Top = point.Y;
                    }
                    dialog.TopMost = TopMost;
                    dialog.ShowDialog();
                    Lang.SetControlLang(this);
                    for (int i = 0; i < appMenu.Items.Count; i++)
                        appMenu.Items[i].Text = Lang.GetText(appMenu.Items[i].Name);
                    string text = Lang.GetText(searchBox).Replace(" ", string.Empty).ToLower();
                    searchBox.Text = string.Format("{0}{1}", text.Substring(0, 1).ToUpper(), text.Substring(1));
                    Main.SetAppDirs();
                    MenuViewForm_Update();
                }
            }
            catch (Exception ex)
            {
                SilDev.Log.Debug(ex);
            }
        }

        private Point GetWindowStartPos(Point _point)
        {
            Point point = new Point();
            switch (SilDev.WinAPI.GetTaskBarLocation())
            {
                case SilDev.WinAPI.Location.LEFT:
                    point.X = Cursor.Position.X - (_point.X / 2);
                    point.Y = Cursor.Position.Y;
                    break;
                case SilDev.WinAPI.Location.TOP:
                    point.X = Cursor.Position.X - (_point.X / 2);
                    point.Y = Cursor.Position.Y;
                    break;
                case SilDev.WinAPI.Location.RIGHT:
                    point.X = Screen.PrimaryScreen.WorkingArea.Width - _point.X;
                    point.Y = Cursor.Position.Y;
                    break;
                default:
                    point.X = Cursor.Position.X - (_point.X / 2);
                    point.Y = Cursor.Position.Y - _point.Y;
                    break;
            }
            if (point.X + _point.X > Screen.PrimaryScreen.WorkingArea.Width)
                point.Y = Screen.PrimaryScreen.WorkingArea.Width - _point.X;
            if (point.Y + _point.Y > Screen.PrimaryScreen.WorkingArea.Height)
                point.Y = Screen.PrimaryScreen.WorkingArea.Height - _point.Y;
            return point;
        }

        private void searchBox_Enter(object sender, EventArgs e)
        {
            searchBox.Font = new Font("Segoe UI", 8.25F);
            searchBox.ForeColor = SystemColors.WindowText;
            searchBox.Text = string.Empty;
        }

        private void searchBox_Leave(object sender, EventArgs e)
        {
            searchBox.Font = new Font("Comic Sans MS", 8.25F, FontStyle.Italic);
            searchBox.ForeColor = SystemColors.GrayText;
            string text = Lang.GetText(searchBox).Replace(" ", string.Empty).ToLower();
            searchBox.Text = string.Format("{0}{1}", text.Substring(0, 1).ToUpper(), text.Substring(1));
        }

        private void searchBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (appsListView.SelectedItems.Count > 0)
                    Main.StartApp(appsListView.SelectedItems[0].Text, true);
                return;
            }
            searchBox.Refresh();
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchBox.Text))
                return;
            List<string> itemList = new List<string>();
            foreach (ListViewItem item in appsListView.Items)
            {
                item.ForeColor = SystemColors.ControlText;
                item.BackColor = SystemColors.Control;
                itemList.Add(item.Text);
            }
            foreach (ListViewItem item in appsListView.Items)
            {
                if (item.Text == Main.SearchMatchItem(searchBox.Text, itemList))
                {
                    item.ForeColor = SystemColors.Control;
                    item.BackColor = SystemColors.HotTrack;
                    item.Selected = true;
                    break;
                }
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutBtn_MouseEnter(object sender, EventArgs e)
        {
            ((PictureBox)sender).Image = Properties.Resources.help_16;
        }

        private void aboutBtn_MouseLeave(object sender, EventArgs e)
        {
            ((PictureBox)sender).Image = Properties.Resources.help_gray_16;
        }

        private static Bitmap ImageHighQualityResize(Image image, int width, int heigth)
        {
            Bitmap bmp = new Bitmap(width, heigth);
            bmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                gr.CompositingMode = CompositingMode.SourceCopy;
                gr.CompositingQuality = CompositingQuality.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.SmoothingMode = SmoothingMode.HighQuality;
                using (ImageAttributes imgAttrib = new ImageAttributes())
                {
                    imgAttrib.SetWrapMode(WrapMode.TileFlipXY);
                    gr.DrawImage(image, new Rectangle(0, 0, width, heigth), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imgAttrib);
                }
            }
            return bmp;
        }

        private static Icon GetSmallIcon(string _file)
        {
            try
            {
                IntPtr[] _icons = new IntPtr[1];
                IconResourceBox.ExtractIconEx(_file, 0, new IntPtr[1], _icons, 1);
                return Icon.FromHandle(_icons[0]);
            }
            catch
            {
                return null;
            }
        }
    }
}
