﻿
#region SILENT DEVELOPMENTS generated code

using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;

namespace SilDev
{
    public static class Log
    {
        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();

        public readonly static string ConsoleTitle = string.Format("Debug Console ('{0}')", Path.GetFileName(Application.ExecutablePath));
        public readonly static string DebugFile = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), string.Format("debug-{0}.log", Crypt.MD5.Encrypt(Application.ExecutablePath)));
        private static int DebugMode = 0;

        public static void ActivateDebug(int _enabled)
        {
            if (File.Exists(DebugFile))
                File.Delete(DebugFile);
            DebugMode = _enabled;
        }

        public static void ActivateDebug()
        {
            ActivateDebug(2);
        }

        public static void AllowDebug()
        {
            if (new Regex("/debug [0-2]|/debug \"[0-2]\"").IsMatch(Environment.CommandLine))
            {
                int option = 0;
                if (int.TryParse(new Regex("/debug ([0-2]?)").Match(Environment.CommandLine.Replace("\"", string.Empty)).Groups[1].ToString(), out option))
                    ActivateDebug(option);
            }
        }

        public static void Debug(string _msg, string _trace)
        {
            if (DebugMode < 1)
                return;
            string logo = @"   _________.__  ____ ________        _________ {0}" +
                          @"  /   _____/|__|/_   |\_____  \   ____\______  \{0}" +
                          @"  \_____  \ |  | |   |  _(__  <  /    \   /    /{0}" +
                          @"  /        \|  | |   | /       \|   |  \ /    / {0}" +
                          @" /_______  /|__| |___|/______  /|___|  //____/  {0}" +
                          @"         \/                  \/      \/         {0}";
            logo = string.Format(logo, Environment.NewLine);
            string date = DateTime.Now.ToString(CultureInfo.CreateSpecificCulture("en-US"));
            string trace = string.Format("{0}{1}", _trace[0].ToString().ToUpper(), _trace.Substring(1));
            string msg = string.Format("Time:{0}{2}{1}Msg:{0}{3}{1}Trace:{0}{4}{1}", (char)27, (char)29, date, _msg, trace);
            msg = msg.Replace(Environment.NewLine, ";");
            if (!File.Exists(DebugFile))
                File.Create(DebugFile).Close();
            if (File.Exists(DebugFile))
            {
                try
                {
                    using (StreamWriter sw = File.AppendText(DebugFile))
                        sw.WriteLine(msg.Replace(((char)27).ToString(), " ").Replace(((char)29).ToString(), Environment.NewLine));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if (DebugMode > 1)
            {
                try
                {
                    AllocConsole();
                    IntPtr stdHandle = GetStdHandle(-11);
                    SafeFileHandle sfh = new SafeFileHandle(stdHandle, true);
                    FileStream fs = new FileStream(sfh, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding(437));
                    sw.AutoFlush = true;
                    Console.SetOut(sw);
                    if (Console.Title != ConsoleTitle)
                    {
                        Console.Title = ConsoleTitle;
                        Console.BufferHeight = 8000;
                        Console.BufferWidth = 8000;
                        Console.SetWindowSize(Math.Min(100, Console.LargestWindowWidth), Math.Min(40, Console.LargestWindowHeight));
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine(logo);
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("               DEBUG CONSOLE v1.3");
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.ResetColor();
                    }
                    foreach (string line in msg.Split((char)29))
                    {
                        string[] results = line.Split((char)27);
                        if (results.Length == 2)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(results[0]);
                            for (int i = 0; i < (7 - results[0].Length); i++)
                                Console.Write(" ");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.Write(results[1]);
                        }
                        Console.Write(Environment.NewLine);
                    }
                }
                catch (Exception ex)
                {
                    DebugMode = 1;
                    Debug(ex);
                }
            }
        }

        public static void Debug(string _msg)
        {
            if (DebugMode < 1)
                return;
            Debug(_msg, "None");
        }

        public static void Debug(Exception _ex)
        {
            if (DebugMode < 1)
                return;
            string msg = _ex.Message;
            string trace = _ex.StackTrace.TrimStart();
            Debug(msg, trace);
        }
    }
}

#endregion
