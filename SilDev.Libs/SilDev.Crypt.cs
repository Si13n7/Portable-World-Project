﻿
// Copyright(c) 2016 Si13n7 'Roy Schroedel' Developments(r)
// This file is licensed under the MIT License

#region Si13n7 Dev. ® created code

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SilDev
{
    public static class Crypt
    {
        #region BaseExtrem

        public static class BaseExtrem
        {
            public static string[] EncryptToArray(string _inputText, int _cryptStrenght)
            {
                string crypt = Encrypt(_inputText, _cryptStrenght);
                StringBuilder output = new StringBuilder();
                int length = crypt.Length;
                while (length > 16)
                    length /= 2;
                if (length % 2 != 0)
                    length++;
                int seperator = 0;
                while (seperator * seperator < length)
                    seperator++;
                for (int i = 1; i < crypt.Length + 1; i++)
                {
                    output.Append(crypt[i - 1]);
                    if (i % seperator == 0)
                        output.Append('\n');
                }
                return output.ToString().Split('\n').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            }

            public static string DecryptArray(string[] _inputHashArray, int _cryptStrenght) =>
                Decrypt(string.Concat(_inputHashArray), _cryptStrenght);

            public static string Encrypt(string _inputText, int _cryptStrenght)
            {
                StringBuilder output = new StringBuilder(Base64.Encrypt(_inputText));
                for (int i = 0; i < _cryptStrenght; i++)
                {
                    output = output.Replace(output.ToString(), Misc.ReverseString(output.ToString()));
                    output = output.Replace(output.ToString(), Base64.Encrypt(output.ToString()));
                }
                return output.ToString();
            }

            public static string Encrypt(string _inputText) =>
                Encrypt(_inputText, 16);

            public static string Decrypt(string _inputHash, int _cryptStrenght)
            {
                string output = Base64.Decrypt(_inputHash);
                for (int i = 0; i < _cryptStrenght; i++)
                {
                    output = Misc.ReverseString(output);
                    output = Base64.Decrypt(output);
                }
                return output;
            }

            public static string Decrypt(string _inputHash) =>
                Decrypt(_inputHash, 16);
        }

        #endregion

        #region Base64

        public static class Base64
        {
            public static string Encrypt(string _inputText)
            {
                try
                {
                    byte[] output = Encoding.UTF8.GetBytes(_inputText);
                    return Convert.ToBase64String(output);
                }
                catch (Exception ex)
                {
                    Log.Debug(ex);
                    return string.Empty;
                }
            }

            public static string Decrypt(string _inputHash)
            {
                try
                {
                    byte[] output = Convert.FromBase64String(_inputHash);
                    return Encoding.UTF8.GetString(output);
                }
                catch (Exception ex)
                {
                    Log.Debug(ex);
                    return string.Empty;
                }
            }

            public static string EncryptFile(string _inputText)
            {
                string output = string.Empty;
                if (File.Exists(_inputText))
                {
                    try
                    {
                        byte[] input = File.ReadAllBytes(_inputText);
                        output = Convert.ToBase64String(input);
                    }
                    catch (Exception ex)
                    {
                        Log.Debug(ex);
                    }
                }
                return output;
            }

            public static byte[] DecryptFile(string _inputHash)
            {
                try
                {
                    byte[] output = Convert.FromBase64String(_inputHash);
                    return Convert.FromBase64String(_inputHash);
                }
                catch (Exception ex)
                {
                    Log.Debug(ex);
                    return null;
                }
            }

            public static string EncryptImage(System.Drawing.Image _inputImage, System.Drawing.Imaging.ImageFormat _inputFormat)
            {
                string output = string.Empty;
                try
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        _inputImage.Save(memoryStream, _inputFormat);
                        output = Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug(ex);
                }
                return output;
            }

            public static System.Drawing.Image DecryptImage(string _inputHash)
            {
                try
                {
                    System.Drawing.Image output;
                    using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(_inputHash)))
                        output = System.Drawing.Image.FromStream(memoryStream);
                    return output;
                }
                catch (Exception ex)
                {
                    Log.Debug(ex);
                    return null;
                }
            }
        }

        #endregion

        #region Message-Digest Algorithm 5

        public static class MD5
        {
            public static bool Compare(string _checksumA, string _checksumB) =>
                _checksumA.Length == 32 && _checksumB.Length == 32 ? _checksumA.ToLower() == _checksumB.ToLower() : false;

            public static string Encrypt(string _inputText)
            {
                string output = string.Empty;
                try
                {
                    using (System.Security.Cryptography.MD5 create = System.Security.Cryptography.MD5.Create())
                    {
                        byte[] hashedDataBytes = create.ComputeHash(Encoding.UTF8.GetBytes(_inputText));
                        output = Misc.ByteArrayToString(hashedDataBytes);
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug(ex);
                }
                return output;
            }

            public static string EncryptFile(string _inputFilePath)
            {
                string output = string.Empty;
                if (File.Exists(_inputFilePath))
                {
                    try
                    {
                        using (FileStream stream = File.OpenRead(_inputFilePath))
                        {
                            System.Security.Cryptography.MD5 crypt = new System.Security.Cryptography.MD5CryptoServiceProvider();
                            byte[] encode = crypt.ComputeHash(stream);
                            output = BitConverter.ToString(encode);
                            output = output.Replace("-", string.Empty).ToLower();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Debug(ex);
                    }
                }
                return output;
            }

            public static string EncryptFile(FileInfo _inputFileInfo) =>
                EncryptFile(_inputFileInfo.FullName);
        }

        #endregion

        #region Secure Hash Algorithm

        public static class SHA
        {
            public enum CryptKind : int
            {
                SHA1 = 128,
                SHA256 = 256,
                SHA384 = 384,
                SHA512 = 512,
            }

            public static string Encrypt(string _inputText, CryptKind _inputBits)
            {
                string output = string.Empty;
                try
                {
                    switch ((int)_inputBits)
                    {
                        case 256:
                            using (System.Security.Cryptography.SHA256 create = System.Security.Cryptography.SHA256.Create())
                            {
                                var hashedDataBytes = create.ComputeHash(Encoding.UTF8.GetBytes(_inputText));
                                output = Misc.ByteArrayToString(hashedDataBytes);
                            }
                            break;
                        case 384:
                            using (System.Security.Cryptography.SHA384 create = System.Security.Cryptography.SHA384.Create())
                            {
                                var hashedDataBytes = create.ComputeHash(Encoding.UTF8.GetBytes(_inputText));
                                output = Misc.ByteArrayToString(hashedDataBytes);
                            }
                            break;
                        case 512:
                            using (System.Security.Cryptography.SHA512 create = System.Security.Cryptography.SHA512.Create())
                            {
                                var hashedDataBytes = create.ComputeHash(Encoding.UTF8.GetBytes(_inputText));
                                output = Misc.ByteArrayToString(hashedDataBytes);
                            }
                            break;
                        default:
                            using (System.Security.Cryptography.SHA1 create = System.Security.Cryptography.SHA1.Create())
                            {
                                var hashedDataBytes = create.ComputeHash(Encoding.UTF8.GetBytes(_inputText));
                                output = Misc.ByteArrayToString(hashedDataBytes);
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug(ex);
                }
                return output;
            }

            public static string Encrypt(string _inputText, int _inputBits)
            {
                string output = string.Empty;
                switch ((int)_inputBits)
                {
                    case 256:
                        output = Encrypt(_inputText, CryptKind.SHA256);
                        break;
                    case 384:
                        output = Encrypt(_inputText, CryptKind.SHA384);
                        break;
                    case 512:
                        output = Encrypt(_inputText, CryptKind.SHA512);
                        break;
                    default:
                        output = Encrypt(_inputText, CryptKind.SHA1);
                        break;
                }
                return output;
            }

            public static string Encrypt(string _inputText) =>
                Encrypt(_inputText, CryptKind.SHA1);

            public static string EncryptFile(string _inputFilePath, CryptKind _inputBits)
            {
                string output = string.Empty;
                if (File.Exists(_inputFilePath))
                {
                    try
                    {
                        using (FileStream stream = File.OpenRead(_inputFilePath))
                        {
                            byte[] encode = null;
                            switch ((int)_inputBits)
                            {
                                case 256:
                                    encode = new System.Security.Cryptography.SHA256CryptoServiceProvider().ComputeHash(stream);
                                    break;
                                case 384:
                                    encode = new System.Security.Cryptography.SHA384CryptoServiceProvider().ComputeHash(stream);
                                    break;
                                case 512:
                                    encode = new System.Security.Cryptography.SHA512CryptoServiceProvider().ComputeHash(stream);
                                    break;
                                default:
                                    encode = new System.Security.Cryptography.SHA1CryptoServiceProvider().ComputeHash(stream);
                                    break;
                            }
                            output = BitConverter.ToString(encode);
                            output = output.Replace("-", string.Empty).ToLower();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Debug(ex);
                    }
                }
                return output;
            }

            public static string EncryptFile(FileInfo _inputFileInfo) =>
                EncryptFile(_inputFileInfo.FullName, CryptKind.SHA1);
        }

        #endregion

        #region Miscellaneous

        public static class Misc
        {
            public static string ByteArrayToString(byte[] _inputArray)
            {
                StringBuilder output = new StringBuilder(string.Empty);
                try
                {
                    foreach (var o in _inputArray)
                        output.Append(o.ToString("x2"));
                }
                catch (Exception ex)
                {
                    Log.Debug(ex);
                }
                return output.ToString();
            }

            public static string ByteArrayToString(object _inputArray)
            {
                StringBuilder output = new StringBuilder(string.Empty);
                try
                {
                    foreach (var o in (byte[])_inputArray)
                        output.Append(o.ToString("x2"));
                }
                catch (Exception ex)
                {
                    Log.Debug(ex);
                }
                return output.ToString();
            }

            private static int FindBytes(byte[] src, byte[] find)
            {
                int index = -1;
                int matchIndex = 0;
                for (int i = 0; i < src.Length; i++)
                {
                    if (src[i] == find[matchIndex])
                    {
                        if (matchIndex == (find.Length - 1))
                        {
                            index = i - matchIndex;
                            break;
                        }
                        matchIndex++;
                    }
                    else
                        matchIndex = 0;
                }
                return index;
            }

            public static byte[] ReplaceBytes(byte[] _source, byte[] _search, byte[] _replacement)
            {
                byte[] dst = null;
                try
                {
                    int index = FindBytes(_source, _search);
                    if (index >= 0)
                    {
                        dst = new byte[_source.Length - _search.Length + _replacement.Length];
                        Buffer.BlockCopy(_source, 0, dst, 0, index);
                        Buffer.BlockCopy(_replacement, 0, dst, index, _replacement.Length);
                        Buffer.BlockCopy(_source, index + _search.Length, dst, index + _replacement.Length, _source.Length - (index + _search.Length));
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug(ex);
                }
                return dst;
            }

            public static string ConvertToHex(string _input)
            {
                UTF8Encoding encode = new UTF8Encoding();
                string convert = ByteArrayToString(encode.GetBytes(_input));
                StringBuilder output = new StringBuilder();
                try
                {
                    for (int i = 0; i < convert.Length; i++)
                    {
                        if (i > 0 && i % 2 == 0)
                            output.Append(' ');
                        output.Append(convert[i]);
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug(ex);
                }
                return output.ToString();
            }

            public static string ReconvertFromHex(string _input)
            {
                string filter = _input.Replace(" ", string.Empty);
                byte[] raw = new byte[filter.Length / 2];
                for (int i = 0; i < raw.Length; i++)
                    raw[i] = Convert.ToByte(filter.Substring(i * 2, 2), 16);

                return Encoding.UTF8.GetString(raw);
            }

            public static string ReverseString(string _input)
            {
                StringBuilder output = new StringBuilder(string.Empty);
                for (int i = (_input.Length - 1); i >= 0; i--)
                    output.Append(_input[i]);
                return output.ToString();
            }
        }

        #endregion
    }
}

#endregion
