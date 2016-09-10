using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AppsLauncher
{
    public partial class IconBrowserForm : Form
    {
        public IconBrowserForm()
        {
            InitializeComponent();

            Icon = Properties.Resources.PortableApps_blue;
            Text = "Icon Resource Browser";

            BackColor = Color.FromArgb(255, (int)(Main.Colors.Layout.R * .5f), (int)(Main.Colors.Layout.G * .5f), (int)(Main.Colors.Layout.B * .5f));
            ForeColor = Main.Colors.ControlText;

            IconPanel.BackColor = Main.Colors.Control;
            IconPanel.ForeColor = Main.Colors.ControlText;

            ResourceFilePath.BackColor = Main.Colors.Control;
            ResourceFilePath.ForeColor = Main.Colors.ControlText;

            ResourceFileBrowserBtn.BackgroundImage = SilDev.Resource.SystemIconAsImage(SilDev.Resource.SystemIconKey.DIRECTORY, false, Main.SysIcoResPath);
            ResourceFileBrowserBtn.BackColor = Main.Colors.Button;
            ResourceFileBrowserBtn.ForeColor = Main.Colors.ButtonText;
            ResourceFileBrowserBtn.FlatAppearance.MouseOverBackColor = Main.Colors.ButtonHover;

            // How ever the layout is wrong in Windows 7
            if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1)
                Padding = new Padding(0, 0, 12, 0);
        }

        private void IconBrowserForm_Load(object sender, EventArgs e)
        {
            ResourceFilePath.Text = Main.SysIcoResPath;
            if (File.Exists(ResourceFilePath.Text))
                ShowIconResources(ResourceFilePath.Text);
        }

        private void IconPanel_Scroll(object sender, ScrollEventArgs e) =>
            ((Panel)sender).Update();

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
            string path = SilDev.Run.EnvVarFilter(ResourceFilePath.Text);
            if ((path.EndsWith(".ico", StringComparison.OrdinalIgnoreCase) || 
                 path.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || 
                 path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)) && File.Exists(path))
                ShowIconResources(path);
        }

        private void ShowIconResources(string path)
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
                        boxes[i] = new IconResourceBox(path, i);
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
