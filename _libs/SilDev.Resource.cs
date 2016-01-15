﻿
#region SILENT DEVELOPMENTS generated code

using System;
using System.Linq;
using System.IO;

namespace SilDev
{
    public static class Resource
    {
        public static void ExtractConvert(byte[] _res, string _file, bool _convert)
        {
            try
            {
                using (var ms = new MemoryStream(_res))
                {
                    byte[] data = ms.ToArray();
                    if (_convert)
                        data = data.Reverse().ToArray();
                    FileStream fs = new FileStream(_file, FileMode.CreateNew, FileAccess.Write);
                    fs.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }
        }

        public static void ExtractConvert(byte[] _res, string _file)
        {
            ExtractConvert(_res, _file, true);
        }

        public static void Extract(byte[] _res, string _file)
        {
            ExtractConvert(_res, _file, false);
        }

        public static void PlayWave(Stream _res)
        {
            try
            {
                using (Stream audio = _res)
                {
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(audio);
                    player.Play();
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }
        }
    }
}

#endregion
