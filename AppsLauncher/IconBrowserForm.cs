﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace AppsLauncher
{
    public partial class IconBrowserForm : Form
    {
        private static string _fileName;

        private static string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public IconBrowserForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.PortableApps_blue;
            Text = "Icon Resource Browser";
        }

        private void IconBrowserForm_Load(object sender, EventArgs e)
        {
            ResourceFilePath.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "imageres.dll");
            if (File.Exists(ResourceFilePath.Text))
                ShowIconResources(ResourceFilePath.Text);
        }

        private void ResourceFileBrowserBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog() { Multiselect = false, InitialDirectory = Application.StartupPath, RestoreDirectory = false })
            {
                dialog.ShowDialog(new Form() { ShowIcon = false, TopMost = true });
                if (!string.IsNullOrWhiteSpace(dialog.FileName))
                    ResourceFilePath.Text = dialog.FileName;
            }
        }

        private void ResourceFilePath_TextChanged(object sender, EventArgs e)
        {
            string path = SilDev.Run.EnvironmentVariableFilter(ResourceFilePath.Text);
            if ((path.EndsWith(".ico", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)) && File.Exists(path))
                ShowIconResources(path);
        }

        private void ShowIconResources(string _file)
        {
            try
            {
                IconResourceBox[] boxes = new IconResourceBox[short.MaxValue];
                if (IconPanel.Controls.Count > 0)
                    IconPanel.Controls.Clear();
                for (int i = 0; i < short.MaxValue; i++)
                {
                    try
                    {
                        boxes[i] = new IconResourceBox(_file, i);
                        IconPanel.Controls.Add(boxes[i]);
                    }
                    catch
                    {
                        break;
                    }
                }
                if (boxes[0] == null)
                    return;
                int max = IconPanel.Width / boxes[0].Width;
                int line = 0;
                int column = 0;
                for (int i = 0; i < boxes.Length; i++)
                {
                    if (boxes[i] == null)
                        continue;
                    line = i / max;
                    column = i - line * max;
                    boxes[i].Location = new Point(column * boxes[i].Width, line * boxes[i].Height);
                }
            }
            catch (Exception ex)
            {
                SilDev.Log.Debug(ex);
            }
        }
    }
}
