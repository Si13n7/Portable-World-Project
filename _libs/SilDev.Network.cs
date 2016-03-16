﻿
// Copyright(c) 2016 Si13n7 'Roy Schroedel' Developments(r)
// This file is licensed under the MIT License

#region Si13n7 Dev. ® created code

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

namespace SilDev
{
    public static class Network
    {
        #region DOWNLOAD

        private static WebClient client;

        private static Stopwatch watch = new Stopwatch();

        public static Dictionary<string, ASYNCDOWNLOADINFODATA> AsyncDownloadInfo { get; set; } = new Dictionary<string, ASYNCDOWNLOADINFODATA>();

        private static string LatestAsyncDownloadInfoKey = string.Empty;
        public static ASYNCDOWNLOADINFODATA LatestAsyncDownloadInfo
        {
            get
            {
                try
                {
                    return AsyncDownloadInfo[LatestAsyncDownloadInfoKey];
                }
                catch
                {
                    return new ASYNCDOWNLOADINFODATA();
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class ASYNCDOWNLOADINFODATA
        {
            public Uri FileUrl;
            public string FilePath;
            public string DataReceived = string.Empty;
            public long CurrentSize = 0;
            public long TotalSize = 0;
            public int ProgressPercentage = 0;
            public string TransferSpeed = string.Empty;
            public TimeSpan TimeElapsed = TimeSpan.MinValue;
            public int StatusCode = 0;
            public string StatusMessage = string.Empty;
        }

        public static void DownloadFileAsync(string _infoKey, string _srcUrl, string _destPath, string _user, string _password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_infoKey))
                    throw new Exception("Info key is empty.");
                if (AsyncDownloadIsBusy())
                    throw new Exception("Async file download is already busy, multiple calls are not allowed.");
                if (File.Exists(_destPath))
                    File.Delete(_destPath);
                using (client = new WebClient())
                {
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileAsync_Completed);
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadFileAsync_ProgressChanged);
                    if (!string.IsNullOrWhiteSpace(_user) && !string.IsNullOrWhiteSpace(_password))
                        client.Credentials = new NetworkCredential(_user, _password);
                    ASYNCDOWNLOADINFODATA state = new ASYNCDOWNLOADINFODATA();
                    state.FileUrl = _srcUrl.ToUri();
                    state.FilePath = _destPath;
                    bool exists = OnlineFileExists(state.FileUrl, _user, _password);
                    if (!exists)
                    {
                        state.StatusCode = 3;
                        state.StatusMessage = "Download failed!";
                    }
                    if (!AsyncDownloadInfo.ContainsKey(_infoKey))
                        AsyncDownloadInfo.Add(_infoKey, state);
                    else
                        AsyncDownloadInfo[_infoKey] = state;
                    LatestAsyncDownloadInfoKey = _infoKey;
                    if (exists)
                    {
                        client.DownloadFileAsync(state.FileUrl, state.FilePath);
                        watch.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                watch.Reset();
                Log.Debug(ex.Message, _srcUrl.ToString().Replace(" ", "%20"));
            }
        }

        public static void DownloadFileAsync(string _srcUrl, string _destPath, string _user, string _password) =>
            DownloadFileAsync(AsyncDownloadInfo.Keys.Count.ToString(), _srcUrl, _destPath, _user, _password);

        public static void DownloadFileAsync(string _infoKey, string _srcUrl, string _destPath) =>
            DownloadFileAsync(_infoKey, _srcUrl, _destPath, null, null);

        public static void DownloadFileAsync(string _srcUrl, string _destPath) =>
            DownloadFileAsync(AsyncDownloadInfo.Keys.Count.ToString(), _srcUrl, _destPath, null, null);

        private static void DownloadFileAsync_ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ASYNCDOWNLOADINFODATA state = AsyncDownloadInfo[LatestAsyncDownloadInfoKey];
            try
            {
                state.CurrentSize = e.BytesReceived;
                state.TotalSize = e.TotalBytesToReceive;
                state.TimeElapsed = watch.Elapsed;
                string received = $"{(state.CurrentSize / 1024f / 1024f).ToString("0.00")} MB / {(state.TotalSize / 1024f / 1024f).ToString("0.00")} MB";
                string speed = (e.BytesReceived / 1024 / state.TimeElapsed.TotalSeconds).ToString("0");
                if (state.ProgressPercentage != e.ProgressPercentage)
                {
                    state.DataReceived = received;
                    state.ProgressPercentage = e.ProgressPercentage;
                    state.TransferSpeed = $"{speed} kb/s";
                }
            }
            catch (Exception ex)
            {
                state.TimeElapsed = watch.Elapsed;
                state.StatusCode = 3;
                state.StatusMessage = $"Error after {state.TimeElapsed}s.{Environment.NewLine}{ex.Message}";
                watch.Reset();
                Log.Debug(ex);
            }
            AsyncDownloadInfo[LatestAsyncDownloadInfoKey] = state;
        }

        private static void DownloadFileAsync_Completed(object sender, AsyncCompletedEventArgs e)
        {
            watch.Reset();
            ASYNCDOWNLOADINFODATA state = AsyncDownloadInfo[LatestAsyncDownloadInfoKey];
            if (e.Cancelled)
            {
                client.Dispose();
                try
                {
                    if (File.Exists(state.FilePath))
                        File.Delete(state.FilePath);
                }
                catch (Exception ex)
                {
                    Log.Debug(ex);
                }
                state.StatusCode = 2;
                state.StatusMessage = "Download canceled!";
            }
            else
            {
                if (File.Exists(state.FilePath))
                    state.CurrentSize = new FileInfo(state.FilePath).Length;
                if (File.Exists(state.FilePath) && state.CurrentSize == state.TotalSize)
                {
                    state.StatusCode = 1;
                    state.StatusMessage = "Download completed!";
                }
                else
                {
                    state.StatusCode = 3;
                    state.StatusMessage = "Download failed!";
                }
            }
            AsyncDownloadInfo[LatestAsyncDownloadInfoKey] = state;
        }

        public static bool AsyncDownloadIsBusy()
        {
            try
            {
                return client.IsBusy;
            }
            catch
            {
                return false;
            }
        }

        public static void CancelAsyncDownload()
        {
            if (AsyncDownloadIsBusy())
                client.CancelAsync();
        }

        public static bool DownloadFile(Uri _srcUrl, string _destPath, string _user, string _password)
        {
            try
            {
                if (File.Exists(_destPath))
                    File.Delete(_destPath);
                if (!OnlineFileExists(_srcUrl, _user, _password))
                    throw new FileNotFoundException();
                using (WebClient tmp = new WebClient())
                {
                    if (!string.IsNullOrWhiteSpace(_user) && !string.IsNullOrWhiteSpace(_password))
                        tmp.Credentials = new NetworkCredential(_user, _password);
                    tmp.DownloadFile(_srcUrl, _destPath);
                }
                return File.Exists(_destPath);
            }
            catch (Exception ex)
            {
                Log.Debug(ex.Message, _srcUrl.ToString().Replace(" ", "%20"));
                return false;
            }
        }

        public static bool DownloadFile(string _srcUrl, string _destPath, string _user, string _password) =>
            DownloadFile(_srcUrl.ToUri(), _destPath, _user, _password);

        public static bool DownloadFile(Uri _srcUrl, string _destPath) =>
            DownloadFile(_srcUrl, _destPath, null, null);

        public static bool DownloadFile(string _srcUrl, string _destPath) =>
            DownloadFile(_srcUrl.ToUri(), _destPath, null, null);

        public static byte[] DownloadData(Uri _url, string _user, string _password)
        {
            byte[] data = null;
            try
            {
                using (WebClient tmp = new WebClient())
                {
                    if (!string.IsNullOrWhiteSpace(_user) && !string.IsNullOrWhiteSpace(_password))
                        tmp.Credentials = new NetworkCredential(_user, _password);
                    data = tmp.DownloadData(_url);
                }
                if (data == null)
                    throw new Exception("No downloadable data found.");
            }
            catch (Exception ex)
            {
                Log.Debug(ex.Message, _url.ToString().Replace(" ", "%20"));
            }
            return data;
        }

        public static byte[] DownloadData(string _url, string _user, string _password) =>
            DownloadData(_url.ToUri(), _user, _password);

        public static byte[] DownloadData(Uri _url) =>
            DownloadData(_url, null, null);

        public static byte[] DownloadData(string _url) =>
            DownloadData(_url.ToUri(), null, null);

        public static string DownloadString(Uri _url, string _user, string _password)
        {
            string str = string.Empty;
            try
            {
                using (WebClient tmp = new WebClient())
                {
                    if (!string.IsNullOrWhiteSpace(_user) && !string.IsNullOrWhiteSpace(_password))
                        tmp.Credentials = new NetworkCredential(_user, _password);
                    str = tmp.DownloadString(_url);
                }
                if (string.IsNullOrWhiteSpace(str))
                    throw new Exception("No downloadable string found.");
            }
            catch (Exception ex)
            {
                Log.Debug(ex.Message, _url.ToString().Replace(" ", "%20"));
            }
            return str;
        }

        public static string DownloadString(string _url, string _user, string _password) =>
            DownloadString(_url.ToUri(), _user, _password);

        public static string DownloadString(Uri _url) =>
            DownloadString(_url, null, null);

        public static string DownloadString(string _url) =>
            DownloadString(_url.ToUri(), null, null);

        #endregion

        #region MISC

        private enum PublicDnsProvider
        {
            censurfridns_dk,
            Comodo_Secure_DNS,
            DNS_Advantage,
            DNS_WATCH,
            Dyn,
            FreeDNS,
            Google,
            GreenTeamDNS,
            Hurricane_Electric,
            Level3,
            Norton_ConnectSafe,
            OpenDNS_Home,
            OpenNIC,
            puntCAT,
            SafeDNS,
            SmartViper
        }

        private readonly static Dictionary<PublicDnsProvider, List<string>> PublicDnsList = new Dictionary<PublicDnsProvider, List<string>>()
        {
            { PublicDnsProvider.censurfridns_dk, new List<string> { "89.233.43.71", "91.239.100.100" } },
            { PublicDnsProvider.Comodo_Secure_DNS, new List<string> { "8.26.56.26", "8.20.247.20" } },
            { PublicDnsProvider.DNS_Advantage, new List<string> { "156.154.70.1", "156.154.71.1" } },
            { PublicDnsProvider.DNS_WATCH, new List<string> { "84.200.69.80", "84.200.70.40" } },
            { PublicDnsProvider.Dyn, new List<string> { "216.146.35.35", "216.146.36.36" } },
            { PublicDnsProvider.FreeDNS, new List<string> { "37.235.1.174", "37.235.1.177" } },
            { PublicDnsProvider.Google, new List<string> { "8.8.8.8", "8.8.4.4" } },
            { PublicDnsProvider.GreenTeamDNS, new List<string> { "81.218.119.11", "209.88.198.133" } },
            { PublicDnsProvider.Hurricane_Electric, new List<string> { "74.82.42.42" } },
            { PublicDnsProvider.Level3, new List<string> { "209.244.0.3", "209.244.0.4" } },
            { PublicDnsProvider.Norton_ConnectSafe, new List<string> { "199.85.126.10", "199.85.127.10" } },
            { PublicDnsProvider.OpenDNS_Home, new List<string> { "208.67.222.222", "208.67.220.220" } },
            { PublicDnsProvider.OpenNIC, new List<string> { "107.150.40.234", "50.116.23.211" } },
            { PublicDnsProvider.puntCAT, new List<string> { "109.69.8.51" } },
            { PublicDnsProvider.SafeDNS, new List<string> { "195.46.39.39", "195.46.39.40" } },
            { PublicDnsProvider.SmartViper, new List<string> { "208.76.50.50", "208.76.51.51" } }
        };

        public static bool InternetIsAvailable(object _publicDnsProvider)
        {
            bool IsAvailable = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces().Any(x => x.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up);
            if (!IsAvailable)
                return IsAvailable;
            IsAvailable = false;
            if (_publicDnsProvider is bool && (bool)_publicDnsProvider)
            {
                foreach (var entry in PublicDnsList)
                {
                    foreach (string ip in entry.Value)
                    {
                        IsAvailable = Ping(ip) < 999;
                        if (IsAvailable)
                            return IsAvailable;
                    }
                }
            }
            else
            {
                foreach (var ip in PublicDnsList[_publicDnsProvider is PublicDnsProvider ? (PublicDnsProvider)_publicDnsProvider : PublicDnsProvider.Google])
                {
                    IsAvailable = Ping(ip) < 999;
                    if (IsAvailable)
                        return IsAvailable;
                }
            }
            return IsAvailable;
        }

        public static bool InternetIsAvailable() =>
            InternetIsAvailable(PublicDnsProvider.Google);

        public static bool UrlIsValid(Uri _url)
        {
            int StatusCode = 500;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url);
                request.Timeout = 3000;
                request.Method = "HEAD";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    StatusCode = (int)response.StatusCode;
                if (StatusCode >= 500 && StatusCode <= 510)
                    throw new HttpListenerException();
            }
            catch (Exception ex)
            {
                Log.Debug(ex.Message, _url.ToString().Replace(" ", "%20"));
            }
            return StatusCode >= 100 && StatusCode < 400;
        }

        public static bool UrlIsValid(string _url) =>
            UrlIsValid(_url.ToUri());

        public static System.Net.NetworkInformation.PingReply PingReply { get; private set; }

        public static long Ping(string _url)
        {
            long RoundtripTime = short.MaxValue;
            try
            {
                using (System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping())
                {
                    PingReply = ping.Send(_url.ToUri().Host, 3000);
                    if (PingReply.Status == System.Net.NetworkInformation.IPStatus.Success)
                    {
                        Log.Debug($"Reply from '{PingReply.Address}': bytes={32} time<1ms TTL='{PingReply.RoundtripTime}'");
                        RoundtripTime = PingReply.RoundtripTime;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.Message, _url.Replace(" ", "%20"));
            }
            return RoundtripTime;
        }

        private static Uri ToUri<T>(this T _url)
        {
            try
            {
                string url = _url as string;
                if (!url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) && !url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                    url = $"http://{url}";
                return new Uri(url);
            }
            catch
            {
                return null;
            }
        }

        public static bool OnlineFileExists(Uri _url, string _user, string _password)
        {
            long ContentLength = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url);
                request.Timeout = 3000;
                if (!string.IsNullOrWhiteSpace(_user) && !string.IsNullOrWhiteSpace(_password))
                    request.Credentials = new NetworkCredential(_user, _password);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    ContentLength = response.ContentLength;
            }
            catch (Exception ex)
            {
                Log.Debug(ex.Message, _url.ToString().Replace(" ", "%20"));
            }
            return ContentLength > 0;
        }

        public static bool OnlineFileExists(string _url, string _user, string _password) =>
            OnlineFileExists(_url.ToUri(), _user, _password);

        public static bool OnlineFileExists(Uri _url) =>
            OnlineFileExists(_url, null, null);

        public static bool OnlineFileExists(string _url) =>
            OnlineFileExists(_url.ToUri(), null, null);

        public static DateTime GetOnlineFileDate(Uri _url, string _user, string _password)
        {
            DateTime LastModified = DateTime.Now;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url);
                request.Timeout = 3000;
                if (!string.IsNullOrWhiteSpace(_user) && !string.IsNullOrWhiteSpace(_password))
                    request.Credentials = new NetworkCredential(_user, _password);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    LastModified = response.LastModified;
            }
            catch (Exception ex)
            {
                Log.Debug(ex.Message, _url.ToString().Replace(" ", "%20"));
            }
            return LastModified;
        }

        public static DateTime GetOnlineFileDate(string _url, string _user, string _password) =>
            GetOnlineFileDate(_url.ToUri(), _user, _password);

        public static DateTime GetOnlineFileDate(Uri _url) =>
            GetOnlineFileDate(_url, null, null);

        public static DateTime GetOnlineFileDate(string _url) =>
            GetOnlineFileDate(_url.ToUri(), null, null);

        public static string GetOnlineFileName(Uri _url)
        {
            string name = string.Empty;
            try
            {
                using (WebClient client = new WebClient())
                {
                    using (Stream stream = client.OpenRead(_url))
                    {
                        string cd = client.ResponseHeaders["content-disposition"];
                        if (!string.IsNullOrWhiteSpace(cd))
                        {
                            int i = cd.IndexOf("filename=", StringComparison.CurrentCultureIgnoreCase);
                            if (i >= 0)
                                name = cd.Substring(i + 10);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.Message, _url.ToString().Replace(" ", "%20"));
            }
            return name;
        }

        public static string GetOnlineFileName(string _url) =>
            GetOnlineFileName(_url.ToUri());

        public static List<string> GetAvailableServers(string _iniUrl, bool _internetIsAvailable)
        {
            List<string> servers = new List<string>();
            try
            {
                if (!_internetIsAvailable)
                    throw new Exception("Currently is no connection available.");
                string OnlineIniFileContent = string.Empty;
                for (int i = 0; i < 3; i++)
                {
                    OnlineIniFileContent = DownloadString(_iniUrl);
                    if (!string.IsNullOrWhiteSpace(OnlineIniFileContent))
                        break;
                    Thread.Sleep(100);
                }
                if (string.IsNullOrWhiteSpace(OnlineIniFileContent))
                    throw new Exception("Currently is no connection available.");
                Dictionary<string, long> sortHelper = new Dictionary<string, long>();
                foreach (string section in Ini.GetSections(OnlineIniFileContent))
                {
                    if (section == "root")
                        continue;
                    string address = Ini.Read(section, "address", OnlineIniFileContent);
                    if (string.IsNullOrWhiteSpace(address))
                        continue;
                    Ping(address);
                    sortHelper.Add(address, PingReply.RoundtripTime);
                }
                servers = sortHelper.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value).Keys.ToList();
            }
            catch (Exception ex)
            {
                Log.Debug(ex.Message, _iniUrl.Replace(" ", "%20"));
            }
            return servers;
        }

        public static List<string> GetAvailableServers(string _iniUrl) =>
            GetAvailableServers(_iniUrl, InternetIsAvailable());

        public static string GetTheBestServer(string _iniUrl, bool _internetIsAvailable)
        {
            try
            {
                if (!_internetIsAvailable)
                    throw new Exception("Currently is no connection available.");
                string OnlineIniFileContent = string.Empty;
                for (int i = 0; i < 3; i++)
                {
                    OnlineIniFileContent = DownloadString(_iniUrl);
                    if (!string.IsNullOrWhiteSpace(OnlineIniFileContent))
                        break;
                    Thread.Sleep(100);
                }
                if (string.IsNullOrWhiteSpace(OnlineIniFileContent))
                    throw new Exception("Currently is no connection available.");
                Dictionary<string, long> connections = new Dictionary<string, long>();
                foreach (string section in Ini.GetSections(OnlineIniFileContent))
                {
                    if (section == "root")
                        continue;
                    string address = Ini.Read(section, "address", OnlineIniFileContent);
                    if (string.IsNullOrWhiteSpace(address))
                        continue;
                    Ping(address);
                    connections.Add(address, PingReply.RoundtripTime);
                }
                if (connections.Count > 1)
                {
                    var sortedConnections = from entry in connections orderby entry.Value ascending select entry;
                    connections = sortedConnections.ToDictionary(pair => pair.Key, pair => pair.Value);
                }
                foreach (var ent in connections)
                {
                    if (ent.Value < long.MaxValue)
                    {
                        Log.Debug($"The best connection has been selected: '{Dns.GetHostAddresses(ent.Key)[0]}'");
                        return ent.Key;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.Message, _iniUrl.Replace(" ", "%20"));
            }
            return null;
        }

        public static string GetTheBestServer(string _iniUrl) =>
            GetTheBestServer(_iniUrl, InternetIsAvailable());

        #endregion
    }
}

#endregion
