﻿namespace AppsDownloader.Libraries
{
    using System.IO;
    using SilDev;

    internal static class CorePaths
    {
        private static string[] _appDirs;
        private static string _appsDir, _appImages, _appsLauncher, _appsSuiteUpdater, _archiver, _homeDir, _redirectUrl, _repositoryPath, _tempDir;

        internal static string AppsDir
        {
            get
            {
                if (_appsDir == default(string))
                    _appsDir = Path.Combine(HomeDir, "Apps");
                return _appsDir;
            }
        }

        internal static string[] AppDirs
        {
            get
            {
                if (_appDirs != default(string[]))
                    return _appDirs;
                _appDirs = new[]
                {
                    AppsDir,
                    Path.Combine(AppsDir, ".free"),
                    Path.Combine(AppsDir, ".repack"),
                    Path.Combine(AppsDir, ".share")
                };
                return _appDirs;
            }
        }

        internal static string AppImages
        {
            get
            {
                if (_appImages == default(string))
                    _appImages = Path.Combine(HomeDir, "Assets\\AppImages.dat");
                return _appImages;
            }
        }

        internal static string AppsLauncher
        {
            get
            {
                if (_appsLauncher == default(string))
#if x86
                    _appsLauncher = Path.Combine(HomeDir, "AppsLauncher.exe");
#else
                    _appsLauncher = Path.Combine(HomeDir, "AppsLauncher64.exe");
#endif
                return _appsLauncher;
            }
        }

        internal static string AppsSuiteUpdater
        {
            get
            {
                if (_appsSuiteUpdater == default(string))
                    _appsSuiteUpdater = PathEx.Combine(PathEx.LocalDir, "Updater.exe");
                return _appsSuiteUpdater;
            }
        }

        internal static string FileArchiver
        {
            get
            {
                if (_archiver != default(string))
                    return _archiver;
#if x86
                _archiver = PathEx.Combine(PathEx.LocalDir, "Helper\\7z\\7zG.exe");
#else
                _archiver = PathEx.Combine(PathEx.LocalDir, "Helper\\7z\\x64\\7zG.exe");
#endif
                Compaction.Zip7Helper.ExePath = _archiver;
                return _archiver;
            }
        }

        internal static string HomeDir
        {
            get
            {
                if (_homeDir == default(string))
                    _homeDir = PathEx.Combine(PathEx.LocalDir, "..");
                return _homeDir;
            }
        }

        internal static string RepositoryUrl
        {
            get
            {
                if (_repositoryPath == default(string))
                    _repositoryPath = "https://github.com/Si13n7/PortableAppsSuite/raw/master";
                return _repositoryPath;
            }
        }

        internal static string RedirectUrl
        {
            get
            {
                if (_redirectUrl == default(string))
                    _redirectUrl = "https://temp.si13n7.com/transfer.php?base=";
                return _redirectUrl;
            }
        }

        internal static string TempDir
        {
            get
            {
                if (_tempDir != default(string))
                    return _tempDir;
                _tempDir = Path.Combine(HomeDir, "Documents\\.cache");
                if (Directory.Exists(_tempDir))
                    return _tempDir;
                if (DirectoryEx.Create(_tempDir))
                {
                    using (var process = ProcessEx.Start(AppsLauncher, ActionGuid.RepairDirs, false, false))
                        if (process?.HasExited == false)
                            process.WaitForExit();
                    return _tempDir;
                }
                _tempDir = EnvironmentEx.GetVariableValue("TEMP");
                return _tempDir;
            }
        }
    }
}
