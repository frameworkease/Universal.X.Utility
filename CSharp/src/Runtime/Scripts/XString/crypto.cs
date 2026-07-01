// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FrameworkEase.Universal.X.Utility
{
    public static partial class XString
    {
        public enum SymmetricAlgorithm : byte
        {
            AES,
            DES
        }

        public static string Encrypt(this string input, SymmetricAlgorithm algorithm = SymmetricAlgorithm.AES, string key = "", string iv = "") { return Encrypt<string, string>(input, algorithm, key, iv); }

        public static byte[] Encrypt(this byte[] input, SymmetricAlgorithm algorithm = SymmetricAlgorithm.AES, string key = "", string iv = "") { return Encrypt<byte[], byte[]>(input, algorithm, key, iv); }

        internal static TOutput Encrypt<TInput, TOutput>(TInput input, SymmetricAlgorithm algorithm = SymmetricAlgorithm.AES, string key = "", string iv = "")
        {
            if (input == null || (input is string s && string.IsNullOrEmpty(s))) return default;
            try
            {
                byte[] data = input switch
                {
                    string str => Encoding.UTF8.GetBytes(str),
                    byte[] bytes => bytes,
                    _ => throw new ArgumentException($"Unsupported input type: {typeof(TInput)}"),
                };
                byte[] encrypted = algorithm switch
                {
                    SymmetricAlgorithm.AES => EncryptAES(data, key, iv),
                    SymmetricAlgorithm.DES => EncryptDES(data, key, iv),
                    _ => throw new ArgumentException($"Unsupported crypto type: {algorithm}"),
                };
                if (typeof(TOutput) == typeof(string)) return (TOutput)(object)System.Convert.ToBase64String(encrypted);
                else if (typeof(TOutput) == typeof(byte[])) return (TOutput)(object)encrypted;
                else throw new ArgumentException($"Unsupported output type: {typeof(TOutput)}");
            }
            catch (Exception e)
            {
                XLog.Error(e);
                return default;
            }
        }

        internal static byte[] EncryptAES(byte[] data, string key, string iv)
        {
            byte[] aesKeyBytes = string.IsNullOrEmpty(key) ? AESKey : Encoding.UTF8.GetBytes(key);
            byte[] aesIVBytes = string.IsNullOrEmpty(iv) ? AESIV : Encoding.UTF8.GetBytes(iv);
            using var aes = Aes.Create();
            using var ms = new MemoryStream();
            aes.Key = aesKeyBytes;
            aes.IV = aesIVBytes;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();
            }
            return ms.ToArray();
        }

        internal static byte[] EncryptDES(byte[] data, string key, string iv)
        {
            var desKey = string.IsNullOrEmpty(key) ? DESKey : Encoding.UTF8.GetBytes(key);
            var desIV = string.IsNullOrEmpty(iv) ? DESIV : Encoding.UTF8.GetBytes(iv);
            using var dcsp = DES.Create();
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, dcsp.CreateEncryptor(desKey, desIV), CryptoStreamMode.Write))
            {
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();
            }
            return ms.ToArray();
        }

        public static string Decrypt(this string input, SymmetricAlgorithm algorithm = SymmetricAlgorithm.AES, string key = "", string iv = "") { return Decrypt<string, string>(input, algorithm, key, iv); }

        public static byte[] Decrypt(this byte[] input, SymmetricAlgorithm algorithm = SymmetricAlgorithm.AES, string key = "", string iv = "") { return Decrypt<byte[], byte[]>(input, algorithm, key, iv); }

        internal static TOutput Decrypt<TInput, TOutput>(TInput input, SymmetricAlgorithm algorithm = SymmetricAlgorithm.AES, string key = "", string iv = "")
        {
            if (input == null || (input is string s && string.IsNullOrEmpty(s))) return default;
            try
            {
                byte[] data = input switch
                {
                    string str => Convert.FromBase64String(str),
                    byte[] bytes => bytes,
                    _ => throw new ArgumentException($"Unsupported input type: {typeof(TInput)}"),
                };
                byte[] decrypted = algorithm switch
                {
                    SymmetricAlgorithm.AES => DecryptAES(data, key, iv),
                    SymmetricAlgorithm.DES => DecryptDES(data, key, iv),
                    _ => throw new ArgumentException($"Unsupported crypto type: {algorithm}"),
                };
                if (typeof(TOutput) == typeof(string)) return (TOutput)(object)Encoding.UTF8.GetString(decrypted);
                else if (typeof(TOutput) == typeof(byte[])) return (TOutput)(object)decrypted;
                else throw new ArgumentException($"Unsupported output type: {typeof(TOutput)}");
            }
            catch (Exception e)
            {
                XLog.Error(e);
                return default;
            }
        }

        internal static byte[] DecryptAES(byte[] data, string key, string iv)
        {
            byte[] aesKeyBytes = string.IsNullOrEmpty(key) ? AESKey : Encoding.UTF8.GetBytes(key);
            byte[] aesIVBytes = string.IsNullOrEmpty(iv) ? AESIV : Encoding.UTF8.GetBytes(iv);
            using var aes = Aes.Create();
            using var ms = new MemoryStream();
            aes.Key = aesKeyBytes;
            aes.IV = aesIVBytes;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();
            }
            return ms.ToArray();
        }

        internal static byte[] DecryptDES(byte[] data, string key, string iv)
        {
            var desKey = string.IsNullOrEmpty(key) ? DESKey : Encoding.UTF8.GetBytes(key);
            var desIV = string.IsNullOrEmpty(iv) ? DESIV : Encoding.UTF8.GetBytes(iv);
            using var dcsp = DES.Create();
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, dcsp.CreateDecryptor(desKey, desIV), CryptoStreamMode.Write))
            {
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();
            }
            return ms.ToArray();
        }

#if FRAMEWORKEASE_XSTRING_NATIVE_CRYPTO && !UNITY_WEBGL
        [System.Runtime.InteropServices.DllImport("__Internal")]
        internal static extern IntPtr GetAESKey();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        internal static extern int GetAESKeyLength();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        internal static extern IntPtr GetAESIV();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        internal static extern int GetAESIVLength();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        internal static extern IntPtr GetDESKey();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        internal static extern int GetDESKeyLength();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        internal static extern IntPtr GetDESIV();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        internal static extern int GetDESIVLength();

        internal static byte[] AESKey
        {
            get
            {
                var length = GetAESKeyLength();
                var result = new byte[length];
                var ptr = GetAESKey();
                System.Runtime.InteropServices.Marshal.Copy(ptr, result, 0, length);
                return result;
            }
        }

        internal static byte[] AESIV
        {
            get
            {
                var length = GetAESIVLength();
                var result = new byte[length];
                var ptr = GetAESIV();
                System.Runtime.InteropServices.Marshal.Copy(ptr, result, 0, length);
                return result;
            }
        }

        internal static byte[] DESKey
        {
            get
            {
                var length = GetDESKeyLength();
                var result = new byte[length];
                var ptr = GetDESKey();
                System.Runtime.InteropServices.Marshal.Copy(ptr, result, 0, length);
                return result;
            }
        }

        internal static byte[] DESIV
        {
            get
            {
                var length = GetDESIVLength();
                var result = new byte[length];
                var ptr = GetDESIV();
                System.Runtime.InteropServices.Marshal.Copy(ptr, result, 0, length);
                return result;
            }
        }
#else
        internal static readonly byte[] AESKey =
        {
            0xA1, 0xB2, 0xC3, 0xD4, 0x55, 0x66, 0x77, 0x88,
            0x09, 0x1A, 0x2B, 0x3C, 0x4D, 0x5E, 0x6F, 0x10
        };

        internal static readonly byte[] AESIV =
        {
            0x2F, 0x8E, 0x4D, 0x9A, 0x6B, 0x1C, 0x3D, 0x7E,
            0x5A, 0x8B, 0x2C, 0x9D, 0x4E, 0x1F, 0x6A, 0x3B
        };

        internal static readonly byte[] DESKey = { 0x7B, 0x4A, 0xF3, 0x91, 0xE5, 0xD2, 0x8C, 0x6F };

        internal static readonly byte[] DESIV = { 0x2A, 0x9F, 0x5C, 0x8E, 0x1B, 0x4D, 0x7A, 0x3C };
#endif
    }
}
