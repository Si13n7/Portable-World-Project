﻿namespace AppsDownloader.Libraries
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using SilDev;
    using SilDev.QuickWmi;

    internal static class Shareware
    {
        private static string _computerId;
        private static Dictionary<byte[], Tuple<byte[], byte[]>> _data;

        private static string ComputerId
        {
            get
            {
                if (_computerId == default(string))
                    _computerId = Win32_OperatingSystem.SerialNumber?.EncryptToSha256();
                return _computerId;
            }
        }

        internal static Dictionary<byte[], Tuple<byte[], byte[]>> Data
        {
            get
            {
                if (_data != default(Dictionary<byte[], Tuple<byte[], byte[]>>))
                    return _data;

                if (File.Exists(CachePaths.SwData))
                    _data = FileEx.ReadAllBytes(CachePaths.SwData)?.DeserializeObject<Dictionary<byte[], Tuple<byte[], byte[]>>>();

                if (_data?.Any() != true)
                    _data = new Dictionary<byte[], Tuple<byte[], byte[]>>();

                var key = Settings.GetConfigKey(nameof(Shareware), nameof(Data), "Count");
                var count = Ini.Read(Settings.Section, key, 0);
                Ini.RemoveKey(Settings.Section, key);
                Ini.WriteDirect(Settings.Section, key, null);

                var write = false;
                for (var i = 0; i < count; i++)
                {
                    key = Settings.GetConfigKey(nameof(Shareware), nameof(Data), i.ToString(), "Server");
                    var srv = Encrypt(Ini.Read(Settings.Section, key));
                    Ini.RemoveKey(Settings.Section, key);
                    Ini.WriteDirect(Settings.Section, key, null);

                    key = Settings.GetConfigKey(nameof(Shareware), nameof(Data), i.ToString(), "User");
                    var usr = Encrypt(Ini.Read(Settings.Section, key));
                    Ini.RemoveKey(Settings.Section, key);
                    Ini.WriteDirect(Settings.Section, key, null);

                    key = Settings.GetConfigKey(nameof(Shareware), nameof(Data), i.ToString(), "Password");
                    var pwd = Encrypt(Ini.Read(Settings.Section, key));
                    Ini.RemoveKey(Settings.Section, key);
                    Ini.WriteDirect(Settings.Section, key, null);

                    CacheData.UpdateSettingsMerges(Settings.Section);

                    if (srv == default(byte[]) || usr == default(byte[]) || pwd == default(byte[]))
                        continue;

                    _data[srv] = Tuple.Create(usr, pwd);
                    if (!write)
                        write = true;
                }

                if (write && _data.Any())
                {
                    FileEx.WriteAllBytes(CachePaths.SwData, _data.SerializeObject());
                    return _data;
                }

                if (File.Exists(CachePaths.SwData) && !_data.Any())
                    FileEx.TryDelete(CachePaths.SwData);
                return _data;
            }
        }

        internal static bool Enabled =>
            Data?.Any() == true;

        internal static string Decrypt(byte[] data) =>
            data?.DecryptFromAes256(ComputerId).ToStringEx();

        internal static byte[] Encrypt(string data) =>
            data?.EncryptToAes256(ComputerId);

        internal static byte[] FindAddressKey(string address) =>
            Data.Keys.FirstOrDefault(key => address.EqualsEx(Decrypt(key)));

        internal static IEnumerable<string> GetAddresses() =>
            Data.Keys.Select(Decrypt);

        internal static string GetUser(string address) =>
            Data.TryGetValue(FindAddressKey(address), out var value) ? Decrypt(value.Item1) : default(string);

        internal static string GetPassword(string address) =>
            Data.TryGetValue(FindAddressKey(address), out var value) ? Decrypt(value.Item2) : default(string);
    }
}
