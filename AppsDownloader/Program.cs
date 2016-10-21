namespace AppsDownloader
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Management;
    using System.Threading;
    using System.Windows.Forms;
    using SilDev;
    using UI;

    internal static class Program
    {
        private static readonly string HomePath = PathEx.Combine(PathEx.LocalDir, "..");

        [STAThread]
        private static void Main()
        {
            Log.FileDir = PathEx.Combine(PathEx.LocalDir, "..\\Documents\\.cache\\logs");
            Ini.File(HomePath, "Settings.ini");
            Log.AllowLogging(Ini.File(), "Settings", "Debug");
#if x86
            string appsDownloader64;
            if (Environment.Is64BitOperatingSystem && File.Exists(appsDownloader64 = PathEx.Combine(PathEx.LocalDir, $"{Process.GetCurrentProcess().ProcessName}64.exe")))
            {
                ProcessEx.Start(appsDownloader64, EnvironmentEx.CommandLine());
                return;
            }
#endif
            if (!RequirementsAvailable())
            {
                var updPath = PathEx.Combine(PathEx.LocalDir, "Updater.exe");
                if (File.Exists(updPath))
                    ProcessEx.Start(updPath);
                else
                {
                    Lang.ResourcesNamespace = typeof(Program).Namespace;
                    if (MessageBox.Show(Lang.GetText("RequirementsErrorMsg"), @"Portable Apps Suite", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                        Process.Start("https://github.com/Si13n7/PortableAppsSuite/releases");
                }
                return;
            }
            try
            {
                var current = Process.GetCurrentProcess();
                bool newInstance;
                using (new Mutex(true, current.ProcessName, out newInstance))
                {
                    var allowInstance = newInstance;
                    if (!allowInstance)
                    {
                        var count = 0;
                        foreach (var p in Process.GetProcessesByName(current.ProcessName))
                        {
                            string query = $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {p.Id}";
                            using (var mObj = new ManagementObjectSearcher(query))
                                if (mObj.Get().Cast<ManagementBaseObject>().Any(obj => obj["CommandLine"].ToString().ContainsEx("{F92DAD88-DA45-405A-B0EB-10A1E9B2ADDD}")))
                                    count++;
                        }
                        allowInstance = count == 1;
                    }
                    if (!allowInstance)
                        return;
                    Lang.ResourcesNamespace = typeof(Program).Namespace;
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainForm().AddLoadingTimeStopwatch());
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        private static bool RequirementsAvailable()
        {
            if (!Elevation.WritableLocation())
                Elevation.RestartAsAdministrator(EnvironmentEx.CommandLine());
            string[] sArray =
            {
                "..\\Apps\\.free\\",
                "..\\Apps\\.repack\\",
                "..\\Apps\\.share\\",
                "..\\Assets\\icon.db",
                "Updater.exe",
#if x86
                "Helper\\7z\\7z.dll",
                "Helper\\7z\\7zG.exe",
                "..\\AppsLauncher.exe",
#else
                "Helper\\7z\\x64\\7z.dll",
                "Helper\\7z\\x64\\7zG.exe",
                "..\\AppsLauncher64.exe"
#endif
            };
            foreach (var s in sArray)
            {
                var path = PathEx.Combine(PathEx.LocalDir, s);
                if (s.EndsWith("\\"))
                {
                    try
                    {
                        if (!Directory.Exists(path))
                            ProcessEx.Start(new ProcessStartInfo
                                     {
                                         Arguments = "{48FDE635-60E6-41B5-8F9D-674E9F535AC7} {9AB50CEB-3D99-404E-BD31-4E635C09AF0F}",
#if x86
                                         FileName = "%CurDir%\\..\\AppsLauncher.exe"
#else
                                         FileName = "%CurDir%\\..\\AppsLauncher64.exe"
#endif
                                     })?.WaitForExit();
                        if (!Directory.Exists(path))
                            throw new DirectoryNotFoundException();
                    }
                    catch
                    {
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
    }
}
