﻿namespace AppsLauncher.Libraries
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using LangResources;
    using Properties;
    using SilDev;

    internal static class Recovery
    {
        internal static bool AppsSuiteIsHealthy(bool repair = true)
        {
            if (!Elevation.WritableLocation())
                Elevation.RestartAsAdministrator();
            while (true)
            {
                try
                {
                    if (!File.Exists(Settings.CorePaths.AppsDownloader) || 
                        !File.Exists(Settings.CorePaths.AppsSuiteUpdater) || 
                        !File.Exists(Settings.CorePaths.FileArchiver))
                        throw new FileNotFoundException();
                }
                catch (FileNotFoundException ex)
                {
                    Log.Write(ex);
                    if (!repair)
                        return false;
                    RepairAppsSuite();
                }

                try
                {
                    foreach (var dir in Settings.CorePaths.AppDirs)
                        if (!Directory.Exists(dir))
                            throw new PathNotFoundException(dir);
                }
                catch (PathNotFoundException ex)
                {
                    Log.Write(ex);
                    if (!repair)
                        return false;
                    RepairAppsSuiteDirs();
                }

                try
                {
                    var envDir = EnvironmentEx.GetVariableValue(Settings.EnvironmentVariable);
                    if (!Settings.DeveloperVersion && !string.IsNullOrWhiteSpace(envDir) && !envDir.EqualsEx(PathEx.LocalDir))
                        throw new ArgumentException();
                }
                catch (ArgumentException ex)
                {
                    Log.Write(ex);
                    if (!repair)
                        return false;
                    RepairEnvironmentVariable();
                }

                if (!repair)
                    return true;
                repair = false;
            }
        }

        internal static void RepairAppsSuite()
        {
            if (File.Exists(Settings.CorePaths.AppsSuiteUpdater) && File.Exists(Settings.CorePaths.FileArchiver))
                ProcessEx.Start(Settings.CorePaths.AppsSuiteUpdater);
            else
            {
                Language.ResourcesNamespace = typeof(Program).Namespace;
                if (MessageBox.Show(Language.GetText(nameof(en_US.RequirementsErrorMsg)), Resources.Titel, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                {
                    var path = PathEx.AltCombine(Resources.GitProfileUri, Resources.GitReleasesPath);
                    Process.Start(path);
                }
            }
            Environment.ExitCode = 1;
            Environment.Exit(Environment.ExitCode);
        }

        internal static void RepairAppsSuiteDirs()
        {
            if (!Elevation.WritableLocation())
                Elevation.RestartAsAdministrator(Settings.ActionGuid.RepairDirs);

            foreach (var dirs in new[]
            {
                Settings.CorePaths.AppDirs,
                Settings.CorePaths.UserDirs
            })
                foreach (var dir in dirs)
                    if (!DirectoryEx.Create(dir))
                        Elevation.RestartAsAdministrator(Settings.ActionGuid.RepairDirs);

            var iniMap = new[]
            {
                new[]
                {
                    Settings.CorePaths.AppDirs.First(),
                    "IconResource=..\\Assets\\FolderIcons.dll,3"
                },
                new[]
                {
                    Settings.CorePaths.AppDirs.Second(),
                    "LocalizedResourceName=\"Si13n7.com\" - Freeware",
                    "IconResource=..\\..\\Assets\\FolderIcons.dll,4"
                },
                new[]
                {
                    Settings.CorePaths.AppDirs.Third(),
                    "LocalizedResourceName=\"PortableApps.com\" - Repacks",
                    "IconResource=..\\..\\Assets\\FolderIcons.dll,2"
                },
                new[]
                {
                    Settings.CorePaths.AppDirs.Last(),
                    "LocalizedResourceName=\"Si13n7.com\" - Shareware",
                    "IconResource=..\\..\\Assets\\FolderIcons.dll,1"
                },
                new[]
                {
                    PathEx.Combine(PathEx.LocalDir, "Assets"),
                    "IconResource=FolderIcons.dll,5"
                },
                new[]
                {
                    PathEx.Combine(PathEx.LocalDir, "Binaries"),
                    "IconResource=..\\Assets\\FolderIcons.dll,5"
                },
                new[]
                {
                    Settings.CorePaths.UserDirs.First(),
                    "LocalizedResourceName=Profile",
                    "IconResource=%SystemRoot%\\system32\\imageres.dll,117",
                    "IconFile=%SystemRoot%\\system32\\shell32.dll",
                    "IconIndex=-235"
                },
                new[]
                {
                    PathEx.Combine(PathEx.LocalDir, "Documents", ".cache"),
                    "IconResource=%SystemRoot%\\system32\\imageres.dll,112"
                },
                new[]
                {
                    Settings.CorePaths.UserDirs.Second(),
                    "LocalizedResourceName=@%SystemRoot%\\system32\\shell32.dll,-21770",
                    "IconResource=%SystemRoot%\\system32\\imageres.dll,-112",
                    "IconFile=%SystemRoot%\\system32\\shell32.dll",
                    "IconIndex=-235"
                },
                new[]
                {
                    Settings.CorePaths.UserDirs.Third(),
                    "LocalizedResourceName=@%SystemRoot%\\system32\\shell32.dll,-21790",
                    "IconResource=%SystemRoot%\\system32\\imageres.dll,-108",
                    "IconFile=%SystemRoot%\\system32\\shell32.dll",
                    "IconIndex=-237",
                    "InfoTip=@%SystemRoot%\\system32\\shell32.dll,-12689"
                },
                new[]
                {
                    Settings.CorePaths.UserDirs.Fourth(),
                    "LocalizedResourceName=@%SystemRoot%\\system32\\shell32.dll,-21779",
                    "IconResource=%SystemRoot%\\system32\\imageres.dll,-113",
                    "IconFile=%SystemRoot%\\system32\\shell32.dll",
                    "IconIndex=-236",
                    "InfoTip=@%SystemRoot%\\system32\\shell32.dll,-12688"
                },
                new[]
                {
                    Settings.CorePaths.UserDirs.Last(),
                    "LocalizedResourceName=@%SystemRoot%\\system32\\shell32.dll,-21791",
                    "IconResource=%SystemRoot%\\system32\\imageres.dll,-189",
                    "IconFile=%SystemRoot%\\system32\\shell32.dll",
                    "IconIndex=-238",
                    "InfoTip=@%SystemRoot%\\system32\\shell32.dll,-12690"
                },
                new[]
                {
                    PathEx.Combine(PathEx.LocalDir, "Help"),
                    "IconResource=..\\Assets\\FolderIcons.dll,4"
                },
                new[]
                {
                    PathEx.Combine(PathEx.LocalDir, "Restoration"),
                    "..\\Assets\\FolderIcons.dll,1"
                }
            };
            for (var i = 0; i < iniMap.Length; i++)
            {
                var array = iniMap[i];
                var dir = array.FirstOrDefault();
                if (!PathEx.IsValidPath(dir) || i >= iniMap.Length - 2 && !Directory.Exists(dir))
                    continue;
                if (!Elevation.WritableLocation(dir))
                    Elevation.RestartAsAdministrator(Settings.ActionGuid.RepairDirs);
                var path = PathEx.Combine(dir, "desktop.ini");
                foreach (var str in array.Skip(1))
                {
                    var ent = str?.Split('=');
                    if (ent?.Length != 2)
                        continue;
                    var key = ent.FirstOrDefault();
                    if (string.IsNullOrEmpty(key))
                        continue;
                    var val = ent.LastOrDefault();
                    if (string.IsNullOrEmpty(val))
                        continue;
                    Ini.WriteDirect(".ShellClassInfo", key, val, path);
                }
                FileEx.SetAttributes(path, FileAttributes.System | FileAttributes.Hidden);
                DirectoryEx.SetAttributes(dir, FileAttributes.ReadOnly);
            }
        }

        internal static void RepairEnvironmentVariable()
        {
            if (!Elevation.IsAdministrator)
            {
                using (var p = ProcessEx.Start(PathEx.LocalPath, Settings.ActionGuid.RepairVariable, true, false))
                    if (p?.HasExited == false)
                        p.WaitForExit();
                return;
            }
            var envDir = EnvironmentEx.GetVariableValue(Settings.EnvironmentVariable);
            if (!Settings.DeveloperVersion && !string.IsNullOrWhiteSpace(envDir) && !envDir.EqualsEx(PathEx.LocalDir))
                SystemIntegration.Enable(true, false);
        }
    }
}
