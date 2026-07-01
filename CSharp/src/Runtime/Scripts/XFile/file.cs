// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.IO;

namespace FrameworkEase.Universal.X.Utility
{
    public partial class XFile
    {
        public struct HashOptions
        {
            public XString.HashAlgorithm Algorithm;
            public int SegmentCount;
            public int SegmentSize;
        }

#if UNITY_5_3_OR_NEWER
        internal static readonly UnityEngine.AndroidJavaClass AndroidProxy = new("com.frameworkease.universal.x.utility.XFile");
#endif

        public static bool Exists(string path)
        {
            try
            {
#if UNITY_5_3_OR_NEWER
                if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android)
                {
                    if (path.StartsWith("jar:file://")) return AndroidProxy.CallStatic<bool>("HasAsset", path);
                }
#endif
                if (string.IsNullOrEmpty(path)) return false;
                return File.Exists(path);
            }
            catch { }
            return false;
        }

        public static T Open<T>(string path)
        {
            try
            {
#if UNITY_5_3_OR_NEWER
                if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android)
                {
                    if (path.StartsWith("jar:file://"))
                    {
                        var bytes = AndroidProxy.CallStatic<byte[]>("OpenAsset", path);
                        if (bytes != null)
                        {
                            if (typeof(T) == typeof(string)) return (T)(object)System.Text.Encoding.UTF8.GetString(bytes);
                            else if (typeof(T) == typeof(byte[])) return (T)(object)bytes;
                        }
                        else return default;
                    }
                }
#endif
                if (typeof(T) == typeof(string)) return (T)(object)File.ReadAllText(path);
                else if (typeof(T) == typeof(byte[])) return (T)(object)File.ReadAllBytes(path);
            }
            catch (Exception e) { XLog.Error(new Exception(path, e)); }
            return default;
        }

        public static bool Save(string path, string data)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || data == null) return false;
                var dir = System.IO.Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    if (!Directory.Create(dir))
                    {
                        XLog.Error($"XFile.Save: failed to create directory: {dir}");
                        return false;
                    }
                }
                File.WriteAllText(path, data);
                return true;
            }
            catch (Exception e) { XLog.Error(new Exception(path, e)); }
            return false;
        }

        public static bool Save(string path, byte[] data)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || data == null) return false;
                var dir = System.IO.Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    if (!Directory.Create(dir))
                    {
                        XLog.Error($"XFile.Save: failed to create directory: {dir}");
                        return false;
                    }
                }
                File.WriteAllBytes(path, data);
                return true;
            }
            catch (Exception e) { XLog.Error(new Exception(path, e)); }
            return false;
        }

        public static long Size(string path)
        {
            try
            {
#if UNITY_5_3_OR_NEWER
                if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android)
                {
                    if (path.StartsWith("jar:file://")) return AndroidProxy.CallStatic<long>("AssetSize", path);
                }
#endif
                var info = new System.IO.FileInfo(path);
                return info.Length;
            }
            catch (Exception e) { XLog.Error(new Exception(path, e)); }
            return -1;
        }

        public static bool Delete(string path)
        {
            try
            {
                if (!Exists(path)) return false;
                File.Delete(path);
                return true;
            }
            catch (Exception e) { XLog.Error(new Exception(path, e)); }
            return false;
        }

        public static bool Copy(string src, string dst, bool overwrite = true)
        {
            try
            {
                if (!Exists(src))
                {
                    XLog.Error($"XFile.Copy: source file does not exist: {src}");
                    return false;
                }
                if (Exists(dst) && !overwrite)
                {
                    XLog.Error($"XFile.Copy: target file already exists: {dst}");
                    return false;
                }
                var dir = System.IO.Path.GetDirectoryName(dst);
                if (!Directory.Exists(dir))
                {
                    if (!Directory.Create(dir))
                    {
                        XLog.Error($"XFile.Copy: failed to create directory: {dir}");
                        return false;
                    }
                }
                var data = Open<byte[]>(src);
                if (data == null) return false;
                return Save(dst, data);
            }
            catch (Exception e) { XLog.Error(new Exception($"{src} -> {dst}", e)); }
            return false;
        }

        public static bool Move(string src, string dst, bool overwrite = true)
        {
            try
            {
                if (!Exists(src))
                {
                    XLog.Error($"XFile.Move: source file does not exist: {src}");
                    return false;
                }
                if (Exists(dst) && !overwrite)
                {
                    XLog.Error($"XFile.Move: target file already exists: {dst}");
                    return false;
                }
                var dir = System.IO.Path.GetDirectoryName(dst);
                if (!Directory.Exists(dir))
                {
                    if (!Directory.Create(dir))
                    {
                        XLog.Error($"XFile.Move: failed to create directory: {dir}");
                        return false;
                    }
                }
                if (Exists(dst) && overwrite) File.Delete(dst);
                File.Move(src, dst);
                return true;
            }
            catch (Exception e) { XLog.Error(new Exception($"{src} -> {dst}", e)); }
            return false;
        }

        public static string Hash(string path, HashOptions? options = null)
        {
            if (!Exists(path)) return string.Empty;

            var fileSize = Size(path);
            if (fileSize <= 0) return string.Empty;

            var algorithm = XString.HashAlgorithm.MD5;
            var segmentCount = 8;
            var segmentSize = 64 * 1024;

            if (options.HasValue)
            {
                var opt = options.Value;
                if (opt.Algorithm != 0) algorithm = opt.Algorithm;
                if (opt.SegmentCount >= 0) segmentCount = opt.SegmentCount;
                if (opt.SegmentSize > 0) segmentSize = opt.SegmentSize;
            }

            if (segmentCount <= 0)
            {
                var data = Open<byte[]>(path);
                if (data == null) return string.Empty;
                return data.Hash(algorithm);
            }
            else
            {
                try
                {
                    using var ms = new MemoryStream();
                    ms.Write(BitConverter.GetBytes(fileSize));

                    using var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                    var readBuffer = new byte[segmentSize];

                    for (int i = 0; i < segmentCount; i++)
                    {
                        var offset = fileSize * i / segmentCount;
                        var remaining = fileSize - offset;
                        if (remaining <= 0) break;

                        var readSize = (int)Math.Min(Math.Min(remaining, segmentSize), readBuffer.Length);
                        if (readSize <= 0) break;

                        file.Seek(offset, SeekOrigin.Begin);

                        var totalRead = 0;
                        while (totalRead < readSize)
                        {
                            var n = file.Read(readBuffer, totalRead, readSize - totalRead);
                            if (n == 0) break;
                            totalRead += n;
                        }

                        if (totalRead > 0) ms.Write(readBuffer, 0, totalRead);
                    }

                    ms.Position = 0;
                    var buffer = ms.ToArray();
                    return buffer.Hash(algorithm);
                }
                catch (Exception e) { XLog.Error(new Exception(path, e)); }
                return string.Empty;
            }
        }
    }
}
