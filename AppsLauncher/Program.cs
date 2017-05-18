using static AppsLauncher.Main;

namespace AppsLauncher
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
    using LangResources;
    using Properties;
    using SilDev;
    using SilDev.Forms;
    using UI;

    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Log.FileDir = Path.Combine(TmpDir, "logs");
            Ini.SetFile(PathEx.LocalDir, "Settings.ini");
            Log.AllowLogging(Ini.FilePath, Resources.LogConfigPattern, "Debug");
#if x86
            string appsLauncher64;
            if (Environment.Is64BitOperatingSystem && File.Exists(appsLauncher64 = PathEx.Combine(PathEx.LocalDir, $"{ProcessEx.CurrentName}64.exe")))
            {
                ProcessEx.Start(appsLauncher64, EnvironmentEx.CommandLine(false));
                return;
            }
#endif
            if (!RequirementsAvailable())
            {
                var updPath = PathEx.Combine(PathEx.LocalDir, "Binaries\\Updater.exe");
                if (File.Exists(updPath))
                    ProcessEx.Start(updPath);
                else
                {
                    Lang.ResourcesNamespace = typeof(Program).Namespace;
                    if (MessageBox.Show(Lang.GetText(nameof(en_US.RequirementsErrorMsg)), Resources.Titel, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                        Process.Start(PathEx.AltCombine(Resources.GitProfileUri, Resources.GitReleasesPath));
                }
                return;
            }
            if (Log.DebugMode < 2)
                CheckEnvironmentVariable();
            var instanceKey = PathEx.LocalPath.GetHashCode().ToString();
            bool newInstance;
            using (new Mutex(true, instanceKey, out newInstance))
            {
                MessageBoxEx.TopMost = true;
                Lang.ResourcesNamespace = typeof(Program).Namespace;
                if (newInstance && ReceivedPathsArray.Count == 0 || ActionGuid.IsAllowNewInstance)
                {
                    SetInterfaceSettings();
                    Application.Run(new MenuViewForm().Plus());
                    return;
                }
                if (EnvironmentEx.CommandLineArgs(false).Count == 0)
                    return;
                if ((newInstance || ActionGuid.IsAllowNewInstance) && !ActionGuid.IsDisallowInterface)
                {
                    SetInterfaceSettings();
                    Application.Run(new OpenWithForm().Plus());
                    return;
                }
                if (ActionGuid.IsRepairDirs)
                    return;
                if (EnvironmentEx.CommandLineArgs(false).Count == 2)
                {
                    var first = EnvironmentEx.CommandLineArgs(false).First();
                    switch (first)
                    {
                        case ActionGuid.FileTypeAssociation:
                            SetInterfaceSettings();
                            AssociateFileTypesHandler(EnvironmentEx.CommandLineArgs(false).Skip(1).First());
                            return;
                        case ActionGuid.RestoreFileTypes:
                            SetInterfaceSettings();
                            RestoreFileTypesHandler(EnvironmentEx.CommandLineArgs(false).Skip(1).First());
                            return;
                        case ActionGuid.SystemIntegration:
                            SetInterfaceSettings();
                            SystemIntegrationHandler(EnvironmentEx.CommandLineArgs(false).Skip(1).First().ToBoolean());
                            return;
                    }
                }
                if (ReceivedPathsArray.Count == 0)
                    return;
                long hWnd;
                do
                {
                    hWnd = Reg.Read<long>(RegPath, "Handle");
                }
                while (hWnd <= 0);
                WinApi.SendArgs((IntPtr)hWnd, ReceivedPathsStr);
            }
        }

        private static bool RequirementsAvailable()
        {
            if (!Elevation.WritableLocation())
                Elevation.RestartAsAdministrator(EnvironmentEx.CommandLine());
            string[] sArray =
            {
                "Apps\\.free\\",
                "Apps\\.repack\\",
                "Apps\\.share\\",
                "Assets\\images.zip",
#if x86
                "Binaries\\AppsDownloader.exe",
#else
                "Binaries\\AppsDownloader64.exe",
#endif
                "Binaries\\Updater.exe"
            };
            foreach (var s in sArray)
            {
                var path = PathEx.Combine(PathEx.LocalDir, s);
                if (s.EndsWith("\\"))
                {
                    try
                    {
                        if (!Directory.Exists(path))
                            RepairAppsSuiteHandler();
                        if (!Directory.Exists(path))
                            throw new PathNotFoundException(path);
                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex);
                        return false;
                    }
                }
                else
                {
                    if (!File.Exists(path))
                        return false;
                }
            }
            return true;
        }

        private static void SetInterfaceSettings()
        {
            SetAppDirs();
            var color = WinApi.GetSystemThemeColor();
            Colors.System = color;
            color = Ini.Read("Settings", "Window.Colors.Base").FromHtmlToColor(Colors.System);
            Colors.Base = color;
            color = Color.FromArgb(byte.MaxValue, (byte)(color.R * .5f), (byte)(color.G * .5f), (byte)(color.B * .5f));
            Colors.BaseDark = color;
            color = Ini.Read("Settings", "Window.Colors.Control").FromHtmlToColor(SystemColors.Window);
            Colors.Control = color;
            color = Ini.Read("Settings", "Window.Colors.ControlText").FromHtmlToColor(SystemColors.WindowText);
            Colors.ControlText = color;
            color = Ini.Read("Settings", "Window.Colors.Button").FromHtmlToColor(SystemColors.ButtonFace);
            Colors.Button = color;
            color = Ini.Read("Settings", "Window.Colors.ButtonHover").FromHtmlToColor(ProfessionalColors.ButtonSelectedHighlight);
            Colors.ButtonHover = color;
            color = Ini.Read("Settings", "Window.Colors.ButtonText").FromHtmlToColor(SystemColors.ControlText);
            Colors.ButtonText = color;
            color = Ini.Read("Settings", "Window.Colors.Highlight").FromHtmlToColor(SystemColors.Highlight);
            Colors.Highlight = color;
            color = Ini.Read("Settings", "Window.Colors.HighlightText").FromHtmlToColor(SystemColors.HighlightText);
            Colors.HighlightText = color;
            AppsLauncher.Main.FontFamily = Ini.Read("Settings", "Window.FontFamily");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }
    }
}
