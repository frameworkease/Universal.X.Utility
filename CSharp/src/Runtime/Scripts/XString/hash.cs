// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Security.Cryptography;
using System.Text;

namespace FrameworkEase.Universal.X.Utility
{
    public static partial class XString
    {
        public enum HashAlgorithm : byte
        {
            MD5,
            SHA1,
            SHA256
        }

        public static string Hash(this string input, HashAlgorithm type = HashAlgorithm.MD5)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return Encoding.UTF8.GetBytes(input).Hash(type);
        }

        public static string Hash(this byte[] input, HashAlgorithm type = HashAlgorithm.MD5)
        {
            if (input == null) return string.Empty;
            try
            {
                byte[] hash;
                switch (type)
                {
                    case HashAlgorithm.MD5:
                        {
                            using var md5 = MD5.Create();
                            hash = md5.ComputeHash(input);
                            break;
                        }
                    case HashAlgorithm.SHA1:
                        {
                            using var sha1 = SHA1.Create();
                            hash = sha1.ComputeHash(input);
                            break;
                        }
                    case HashAlgorithm.SHA256:
                        {
                            using var sha256 = SHA256.Create();
                            hash = sha256.ComputeHash(input);
                            break;
                        }
                    default: throw new ArgumentException($"Unsupported hash type: {type}");
                }
                var sb = new StringBuilder(hash.Length * 2);
                foreach (var b in hash) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
            catch (Exception e)
            {
                XLog.Error(e);
                return string.Empty;
            }
        }
    }
}
