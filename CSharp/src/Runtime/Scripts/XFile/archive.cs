// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

#if !UNITY_5_3_OR_NEWER || (UNITY_5_3_OR_NEWER && !FRAMEWORKEASE_UNITY_XFILE_ARCHIVE_DISABLED)
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;

namespace FrameworkEase.Universal.X.Utility
{
    public partial class XFile
    {
        public enum ArchiveFormat : byte
        {
            Zip = 0,
            TarGz,
            Rar,
            SevenZ
        }

        public class ArchiveOptions
        {
            public ArchiveFormat Format = ArchiveFormat.Zip;
            public Action<double> OnProgress;
            public Action OnComplete;
            public Action<string> OnError;
        }

        public static class Archive
        {
            public static Task Compress(string src, string dst, ArchiveOptions options = null)
            {
                var opt = options ?? new ArchiveOptions();
                return Task.Run(() =>
                {
                    try
                    {
                        switch (opt.Format)
                        {
                            case ArchiveFormat.Zip:
                                CompressZip(src, dst, opt);
                                break;
                            default:
                                opt.OnError?.Invoke($"unsupported format: {opt.Format}");
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        XLog.Error(new Exception($"{src} -> {dst}", e));
                        opt.OnError?.Invoke(e.Message);
                    }
                });
            }

            public static Task Extract(string src, string dst, ArchiveOptions options = null)
            {
                var opt = options ?? new ArchiveOptions();
                return Task.Run(() =>
                {
                    try
                    {
                        switch (opt.Format)
                        {
                            case ArchiveFormat.Zip:
                                ExtractZip(src, dst, opt);
                                break;
                            default:
                                opt.OnError?.Invoke($"unsupported format: {opt.Format}");
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        XLog.Error(new Exception($"{src} -> {dst}", e));
                        opt.OnError?.Invoke(e.Message);
                    }
                });
            }

            private static void CompressZip(string src, string dst, ArchiveOptions opt)
            {
                var srcInfo = new System.IO.FileInfo(src);
                if (!srcInfo.Exists && !System.IO.Directory.Exists(src))
                {
                    opt.OnError?.Invoke($"source does not exist: {src}");
                    return;
                }

                var zipDir = System.IO.Path.GetDirectoryName(dst);
                if (!Directory.Exists(zipDir))
                {
                    if (!Directory.Create(zipDir))
                    {
                        opt.OnError?.Invoke($"failed to create zip directory: {zipDir}");
                        return;
                    }
                }

                long totalSize = 0;
                long processedSize = 0;
                var zipFiles = new List<(string path, long size, string relPath)>();

                if (!System.IO.Directory.Exists(src))
                {
                    totalSize = srcInfo.Length;
                    zipFiles.Add((src, srcInfo.Length, System.IO.Path.GetFileName(src)));
                }
                else
                {
                    var dir = System.IO.Path.GetFullPath(src);
                    if (dir.Length > 1) dir = dir.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);

                    foreach (var file in System.IO.Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                    {
                        var relPath = System.IO.Path.GetRelativePath(dir, file).Replace('\\', '/');
                        var fileInfo = new System.IO.FileInfo(file);
                        zipFiles.Add((file, fileInfo.Length, relPath));
                        totalSize += fileInfo.Length;
                    }
                }

                using (var zipStream = new ZipOutputStream(File.Create(dst)))
                {
                    foreach (var (filePath, fileSize, relPath) in zipFiles)
                    {
                        using var srcFile = File.OpenRead(filePath);
                        var zipEntry = new ZipEntry(relPath)
                        {
                            DateTime = new System.IO.FileInfo(filePath).LastWriteTime,
                            Size = srcFile.Length
                        };
                        zipStream.PutNextEntry(zipEntry);

                        srcFile.CopyTo(zipStream);
                        processedSize += fileSize;

                        if (opt.OnProgress != null && totalSize > 0)
                        {
                            var progress = (double)processedSize / totalSize * 100.0;
                            opt.OnProgress(progress);
                        }
                    }
                    zipStream.Finish();
                }

                opt.OnComplete?.Invoke();
            }

            private static void ExtractZip(string src, string dst, ArchiveOptions opt)
            {
                if (!Exists(src))
                {
                    opt.OnError?.Invoke($"zip file does not exist: {src}");
                    return;
                }

                if (!Directory.Exists(dst))
                {
                    if (!Directory.Create(dst))
                    {
                        opt.OnError?.Invoke($"failed to create target directory: {dst}");
                        return;
                    }
                }

                long totalSize = 0;
                long processedSize = 0;
                var zipFiles = new List<ZipEntry>();

                using (var zipFile = new ZipFile(src))
                {
                    foreach (ZipEntry entry in zipFile)
                    {
                        zipFiles.Add(entry);
                        totalSize += entry.Size;
                    }

                    int fileIndex = 0;
                    foreach (ZipEntry entry in zipFiles)
                    {
                        var filePath = System.IO.Path.Join(dst, entry.Name);
                        if (entry.IsDirectory)
                        {
                            if (!Directory.Create(filePath))
                            {
                                opt.OnError?.Invoke($"failed to create directory: {filePath}");
                                return;
                            }
                            continue;
                        }

                        var parent = System.IO.Path.GetDirectoryName(filePath);
                        if (!Directory.Exists(parent))
                        {
                            if (!Directory.Create(parent))
                            {
                                opt.OnError?.Invoke($"failed to create parent directory: {parent}");
                                return;
                            }
                        }

                        using (var entryStream = zipFile.GetInputStream(entry))
                        using (var dstFile = File.Create(filePath))
                        {
                            entryStream.CopyTo(dstFile);
                            processedSize += entry.Size;

                            if (opt.OnProgress != null && totalSize > 0)
                            {
                                var progress = (double)processedSize / totalSize * 100.0;
                                opt.OnProgress(progress);
                            }
                            else if (opt.OnProgress != null && zipFiles.Count > 0)
                            {
                                var progress = (double)(fileIndex + 1) / zipFiles.Count * 100.0;
                                opt.OnProgress(progress);
                            }
                        }
                        fileIndex++;
                    }
                }

                opt.OnComplete?.Invoke();
            }
        }
    }
}
#endif
