﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace AppsLauncher
{
    public partial class MainForm : Form
    {
        protected bool IsStarted, ValidData;
        
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (int)SilDev.WinAPI.Win32HookAction.WM_COPYDATA:
                    SilDev.WinAPI.CopyDataStruct st = (SilDev.WinAPI.CopyDataStruct)Marshal.PtrToStructure(m.LParam, typeof(SilDev.WinAPI.CopyDataStruct));
                    string strData = Marshal.PtrToStringUni(st.lpData);
                    if (!string.IsNullOrWhiteSpace(strData) && !Main.CmdLine.ToLower().Contains(strData.ToLower()))
                    {
                        Main.CmdLineArray.Add(strData.Replace("\"", string.Empty));
                        showBalloonTip(Text, Lang.GetText("cmdLineUpdated"));
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        public MainForm()
        {
            InitializeComponent();
#if !x86
            Text = string.Format("{0} (64-bit)", Text);
#endif
            Icon = Properties.Resources.PortableApps_blue;
            BackColor = Color.FromArgb(255, Main.LayoutColor.R, Main.LayoutColor.G, Main.LayoutColor.B);
            notifyIcon.Icon = Properties.Resources.world_16;
            startBtn.FlatAppearance.MouseOverBackColor = Main.LayoutColor;
            settingsBtn.FlatAppearance.MouseOverBackColor = Main.LayoutColor;
            if (!searchBox.Focused)
                searchBox.Select();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Lang.SetControlLang(this);
            if (!Directory.Exists(Main.AppsPath))
                Main.RepairAppsLauncher();
            Main.CheckCmdLineApp();
            appsBox_Update();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            SilDev.Initialization.WriteValue("History", "PID", Process.GetCurrentProcess().MainWindowHandle);
            if (!string.IsNullOrWhiteSpace(Main.CmdLineApp))
            {
                RunCmdLine.Enabled = true;
                return;
            }
            Opacity = 1f;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            int StartMenuIntegration = 0;
            int.TryParse(SilDev.Initialization.ReadValue("Settings", "StartMenuIntegration"), out StartMenuIntegration);
            if (StartMenuIntegration > 0)
            {
                List<string> list = new List<string>();
                foreach (string item in appsBox.Items)
                    list.Add(item);
                Main.StartMenuFolderUpdate(list);
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SilDev.Initialization.WriteValue("History", "PID", 0);
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            Array items;
            ValidData = DragFileName(out items, e);
            if (ValidData)
            {
                bool DataAdded = false;
                foreach (object item in items)
                {
                    if (item is string)
                    {
                        Main.CmdLineArray.Add(((string)item).Replace("\"", string.Empty));
                        DataAdded = true;
                    }
                }
                if (DataAdded)
                {
                    showBalloonTip(Text, Lang.GetText("cmdLineUpdated"));
                    Main.CheckCmdLineApp();
                    foreach (var ent in Main.AppsDict)
                    {
                        if (ent.Value == Main.CmdLineApp)
                        {
                            appsBox.SelectedItem = ent.Key;
                            Main.CmdLineApp = string.Empty;
                        }
                    }
                }
                e.Effect = DragDropEffects.Copy;
            }
            else
                e.Effect = DragDropEffects.None;
        }

        protected bool DragFileName(out Array files, DragEventArgs e)
        {
            bool ret = false;
            files = null;
            if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                Array data = e.Data.GetData("FileDrop") as Array;
                if (data != null)
                {
                    if ((data.Length >= 1) && (data.GetValue(0) is string))
                    {
                        files = data;
                        ret = true;
                    }
                }
            }
            return ret;
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            if (!IsStarted)
                IsStarted = true;
            else
                appsBox_Update();
        }

        private void MainForm_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            try
            {
                using (Form dialog = new AboutForm())
                {
                    dialog.TopMost = TopMost;
                    dialog.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                SilDev.Log.Debug(ex);
            }
            e.Cancel = true;
        }

        private void appsBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                Main.StartApp(appsBox.SelectedItem.ToString(), true);
        }

        private void appsBox_Update()
        {
            Main.CheckAvailableApps();
            string selectedItem = string.Empty;
            if (appsBox.SelectedIndex >= 0)
                selectedItem = appsBox.SelectedItem.ToString();
            appsBox.Items.Clear();
            foreach (string ent in Main.AppsList)
                appsBox.Items.Add(ent);
            if (appsBox.SelectedIndex < 0)
            {
                string lastItem = SilDev.Initialization.ReadValue("History", "LastItem");
                if (!string.IsNullOrWhiteSpace(lastItem))
                    if (appsBox.Items.Contains(lastItem))
                        appsBox.SelectedItem = lastItem;
            }
            if (!string.IsNullOrWhiteSpace(selectedItem))
                appsBox.SelectedItem = selectedItem;
            if (appsBox.SelectedIndex < 0)
                appsBox.SelectedIndex = 0;
            appsCount.Text = string.Format(Lang.GetText(appsCount), appsBox.Items.Count, appsBox.Items.Count == 1 ? "App" : "Apps");
            int StartMenuIntegration = 0;
            int.TryParse(SilDev.Initialization.ReadValue("Settings", "StartMenuIntegration"), out StartMenuIntegration);
            if (StartMenuIntegration > 0)
            {
                List<string> list = new List<string>();
                foreach (string item in appsBox.Items)
                    list.Add(item);
                Main.StartMenuFolderUpdate(list);
            }
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
#if x86
            SilDev.Run.App(new ProcessStartInfo() { FileName = Path.Combine(Application.StartupPath, "Binaries\\AppsDownloader.exe") });
#else
            SilDev.Run.App(new ProcessStartInfo() { FileName = Path.Combine(Application.StartupPath, "Binaries\\AppsDownloader64.exe") });
#endif
        }

        private void addBtn_MouseEnter(object sender, EventArgs e)
        {
            toolTip.SetToolTip(sender as Control, Lang.GetText(string.Format("{0}Tip", (sender as Control).Name)));
            ((Button)sender).Image = Properties.Resources.add_b_13;
        }

        private void addBtn_MouseLeave(object sender, EventArgs e)
        {
            ((Button)sender).Image = Properties.Resources.add_a_13;
        }

        private void appMenuItem_Opening(object sender, CancelEventArgs e)
        {
            for (int i = 0; i < appMenu.Items.Count; i++)
            {
                string text = Lang.GetText(appMenu.Items[i].Name);
                appMenu.Items[i].Text = !string.IsNullOrWhiteSpace(text) ? text : appMenu.Items[i].Text;
            }
        }

        private void appMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem i = (ToolStripMenuItem)sender;
            switch (i.Name)
            {
                case "appMenuItem1":
                    Main.StartApp(appsBox.SelectedItem.ToString(), true);
                    break;
                case "appMenuItem2":
                    Main.StartApp(appsBox.SelectedItem.ToString(), true, true);
                    break;
                case "appMenuItem3":
                    Main.OpenAppLocation(appsBox.SelectedItem.ToString());
                    break;
                case "appMenuItem4":
                    if (SilDev.Data.CreateShortcut(Main.GetAppPath(Main.AppsDict[appsBox.SelectedItem.ToString()]), Path.Combine("%DesktopDir%", appsBox.SelectedItem.ToString()), Main.CmdLine))
                        SilDev.MsgBox.Show(this, Lang.GetText("ShortcutCreatedMsg0"), Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    else
                        SilDev.MsgBox.Show(this, Lang.GetText("ShortcutCreatedMsg1"), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case "appMenuItem6":
                    if (SilDev.MsgBox.Show(this, string.Format(Lang.GetText("appMenuItem5Msg"), appsBox.SelectedItem), Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            string appDir = Path.GetDirectoryName(Main.GetAppPath(Main.AppsDict[appsBox.SelectedItem.ToString()]));
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
        }

        private void searchBox_Enter(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Font = new Font("Segoe UI", 8.25F);
            tb.ForeColor = SystemColors.WindowText;
            tb.Text = string.Empty;
        }

        private void searchBox_Leave(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Font = new Font("Comic Sans MS", 8.25F);
            tb.ForeColor = SystemColors.GrayText;
            tb.Text = Lang.GetText(tb);
        }

        private void searchBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                Main.StartApp(appsBox.SelectedItem.ToString(), true);
                return;
            }
            ((TextBox)sender).Refresh();
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(tb.Text))
                return;
            List<string> itemList = new List<string>();
            foreach (var item in appsBox.Items)
                itemList.Add(item.ToString());
            foreach (var item in appsBox.Items)
            {
                if (item.ToString() == Main.SearchMatchItem(tb.Text, itemList))
                {
                    appsBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            if (PointToClient(MousePosition).X >= (b.Width - 6))
                appMenu.Show(b, new Point(0, b.Height), ToolStripDropDownDirection.BelowRight);
            else
                Main.StartApp(appsBox.SelectedItem.ToString(), true);
        }

        private void startBtn_MouseMove(object sender, MouseEventArgs e)
        {
            Button b = (Button)sender;
            if (PointToClient(MousePosition).X >= (b.Width - 6))
            {
                b.Image = Properties.Resources.split_b_20;
                b.BackgroundImage = Properties.Resources.split_135x20;
            }
            else
                startBtn_MouseLeave(sender, EventArgs.Empty);
        }

        private void startBtn_MouseLeave(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            b.Image = Properties.Resources.split_a_20;
            b.BackgroundImage = null;
        }

        private void settingsBtn_Click(object sender, EventArgs e)
        {
            try
            {
                using (Form dialog = new SettingsForm(appsBox.SelectedItem.ToString()))
                {
                    dialog.TopMost = TopMost;
                    dialog.ShowDialog();
                    Lang.SetControlLang(this);
                    Main.SetAppDirs();
                    appsBox_Update();
                }
            }
            catch (Exception ex)
            {
                SilDev.Log.Debug(ex);
            }
        }

        private void RunCmdLine_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
                    return;
                foreach (string app in Main.AppsList)
                    if (Main.AppsDict[app] == Main.CmdLineApp)
                        appsBox.SelectedItem = app;
                if (appsBox.SelectedIndex > 0)
                {
                    bool noConfirm = bool.Parse(SilDev.Initialization.ReadValue(Main.AppsDict[appsBox.SelectedItem.ToString()], "NoConfirm"));
                    if (!Main.CmdLineMultipleApps && noConfirm)
                    {
                        RunCmdLine.Enabled = false;
                        Main.StartApp(appsBox.SelectedItem.ToString(), true);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                SilDev.Log.Debug(ex);
            }
            RunCmdLine.Enabled = false;
            Opacity = 1f;
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            if (notifyIconDisabler.IsBusy)
                notifyIconDisabler.CancelAsync();
            if (notifyIcon.Visible)
                notifyIcon.Visible = false;
        }

        private void notifyIconDisabler_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < 3000; i++)
            {
                if (notifyIconDisabler.CancellationPending)
                { 
                    e.Cancel = true;
                    return;
                }
                Thread.Sleep(1);
            }
        }

        private void notifyIconDisabler_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (notifyIcon.Visible)
                notifyIcon.Visible = false;
        }

        private void showBalloonTip(string _title, string _tip)
        {
            if (!notifyIcon.Visible)
                notifyIcon.Visible = true;
            if (!notifyIconDisabler.IsBusy)
                notifyIconDisabler.RunWorkerAsync();
            notifyIcon.ShowBalloonTip(1800, _title, _tip, ToolTipIcon.Info);
        }
    }
}
