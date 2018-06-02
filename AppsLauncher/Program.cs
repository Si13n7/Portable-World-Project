namespace AppsLauncher
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
    using Windows;
    using Libraries;
    using SilDev;
    using SilDev.Forms;

    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Settings.Initialize();

            var instanceKey = PathEx.LocalPath.GetHashCode().ToString();
            using (new Mutex(true, instanceKey, out var newInstance))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Language.ResourcesNamespace = typeof(Program).Namespace;
                MessageBoxEx.TopMost = true;

                if (newInstance && Arguments.ValidPaths.Any() && !ActionGuid.IsDisallowInterface)
                {
                    Application.Run(new OpenWithForm().Plus());
                    return;
                }

                if (newInstance || ActionGuid.IsAllowNewInstance)
                {
                    Application.Run(new MenuViewForm().Plus());
                    return;
                }

                if (!EnvironmentEx.CommandLineArgs(false).Any())
                    return;

                switch (EnvironmentEx.CommandLineArgs(false).Count)
                {
                    case 1:
                    {
                        var first = EnvironmentEx.CommandLineArgs(false).First();
                        switch (first)
                        {
                            case ActionGuid.RepairAppsSuite:
                                Recovery.RepairAppsSuite();
                                return;
                            case ActionGuid.RepairDirs:
                                Recovery.RepairAppsSuiteDirs();
                                return;
                            case ActionGuid.RepairVariable:
                                Recovery.RepairEnvironmentVariable();
                                return;
                        }
                        break;
                    }
                    case 2:
                    {
                        var first = EnvironmentEx.CommandLineArgs(false).First();
                        switch (first)
                        {
                            case ActionGuid.FileTypeAssociation:
                                FileTypeAssoc.Associate(EnvironmentEx.CommandLineArgs(false).SecondOrDefault());
                                return;
                            case ActionGuid.RestoreFileTypes:
                                FileTypeAssoc.Restore(EnvironmentEx.CommandLineArgs(false).SecondOrDefault());
                                return;
                            case ActionGuid.SystemIntegration:
                                SystemIntegration.Enable(EnvironmentEx.CommandLineArgs(false).SecondOrDefault().ToBoolean());
                                return;
                        }
                        break;
                    }
                }

                if (!Arguments.ValidPaths.Any())
                    return;
                IntPtr hWnd;
                do
                {
                    hWnd = Reg.Read(Settings.RegistryPath, "Handle", IntPtr.Zero);
                }
                while (hWnd == IntPtr.Zero);
                WinApi.NativeHelper.SendArgs(hWnd, Arguments.ValidPathsStr);
            }
        }
    }
}
