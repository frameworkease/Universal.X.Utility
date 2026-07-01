// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FrameworkEase.Universal.X.Utility
{
    public partial class XFile
    {
        public static partial class Directory
        {
            public static bool Exists(string path)
            {
                try
                {
                    if (string.IsNullOrEmpty(path)) return false;
                    return System.IO.Directory.Exists(path);
                }
                catch { }
                return false;
            }

            public static bool Create(string path, bool overwrite = false)
            {
                try
                {
                    if (Exists(path))
                    {
                        if (!overwrite) return true;
                        if (!Delete(path, true)) return false;
                    }
                    System.IO.Directory.CreateDirectory(path);
                    return true;
                }
                catch (Exception e) { XLog.Error(new Exception(path, e)); }
                return false;
            }

            public static bool Delete(string path, bool recursive = true)
            {
                try
                {
                    if (!Exists(path)) return false;
                    if (recursive) System.IO.Directory.Delete(path, true);
                    else System.IO.Directory.Delete(path, false);
                    return true;
                }
                catch (Exception e) { XLog.Error(new Exception(path, e)); }
                return false;
            }

            public static bool Copy(string src, string dst, params string[] exclude)
            {
                try
                {
                    if (!Exists(src))
                    {
                        XLog.Error($"XFile.Directory.Copy: source directory does not exist: {src}");
                        return false;
                    }
                    if (!Create(dst))
                    {
                        XLog.Error($"XFile.Directory.Copy: failed to create target directory: {dst}");
                        return false;
                    }

                    foreach (var file in System.IO.Directory.GetFiles(src, "*", SearchOption.AllDirectories))
                    {
                        var relPath = Path.GetRelativePath(src, file);
                        if (relPath == ".") continue;

                        var skip = false;
                        if (exclude != null && exclude.Length > 0)
                        {
                            foreach (var pattern in exclude)
                            {
                                if (Regex.IsMatch(file, pattern))
                                {
                                    skip = true;
                                    break;
                                }
                            }
                        }
                        if (skip) continue;

                        var dstPath = Path.Join(dst, relPath);
                        var dstDir = Path.GetDirectoryName(dstPath);
                        if (!Exists(dstDir))
                        {
                            if (!Create(dstDir))
                            {
                                XLog.Error($"XFile.Directory.Copy: failed to create directory: {dstDir}");
                                return false;
                            }
                        }
                        if (!XFile.Copy(file, dstPath, true))
                        {
                            XLog.Error($"XFile.Directory.Copy: failed to copy file: {file}");
                            return false;
                        }
                    }

                    foreach (var dir in System.IO.Directory.GetDirectories(src, "*", SearchOption.AllDirectories))
                    {
                        var relPath = Path.GetRelativePath(src, dir);
                        if (relPath == ".") continue;

                        var skip = false;
                        if (exclude != null && exclude.Length > 0)
                        {
                            foreach (var pattern in exclude)
                            {
                                if (Regex.IsMatch(dir, pattern))
                                {
                                    skip = true;
                                    break;
                                }
                            }
                        }
                        if (skip) continue;

                        var dstPath = Path.Join(dst, relPath);
                        if (!Create(dstPath))
                        {
                            XLog.Error($"XFile.Directory.Copy: failed to create directory: {dstPath}");
                            return false;
                        }
                    }

                    return true;
                }
                catch (Exception e) { XLog.Error($"{src} -> {dst}", e); }
                return false;
            }

            public static bool Move(string src, string dst, bool overwrite = true)
            {
                try
                {
                    if (!Exists(src))
                    {
                        XLog.Error($"XFile.Directory.Move: source directory does not exist: {src}");
                        return false;
                    }
                    if (Exists(dst) && !overwrite)
                    {
                        XLog.Error($"XFile.Directory.Move: target directory already exists: {dst}");
                        return false;
                    }
                    if (Exists(dst) && overwrite)
                    {
                        if (!Delete(dst, true))
                        {
                            return false;
                        }
                    }
                    var parent = Path.GetDirectoryName(dst);
                    if (!Exists(parent))
                    {
                        if (!Create(parent))
                        {
                            XLog.Error($"XFile.Directory.Move: failed to create parent directory: {parent}");
                            return false;
                        }
                    }
                    System.IO.Directory.Move(src, dst);
                    return true;
                }
                catch (Exception e) { XLog.Error(new Exception($"{src} -> {dst}", e)); }
                return false;
            }

            public static void Walk(string path, Func<string, FileSystemInfo, bool> walker)
            {
                if (walker == null)
                {
                    XLog.Error("XFile.Directory.Walk: walker is null.");
                    return;
                }

                try { visit(path, walker); }
                catch (Exception e) { XLog.Error(new Exception(path, e)); }

                static bool visit(string path, Func<string, FileSystemInfo, bool> walker)
                {
                    if (!System.IO.Directory.Exists(path)) return true;

                    var root = new System.IO.DirectoryInfo(path);
                    if (!walker(path, root)) return false;

                    foreach (var file in System.IO.Directory.GetFiles(path))
                    {
                        var info = new System.IO.FileInfo(file);
                        if (!walker(file, info)) return false;
                    }
                    foreach (var dir in System.IO.Directory.GetDirectories(path))
                    {
                        if (!visit(dir, walker)) return false;
                    }
                    return true;
                }
            }
        }

        public static partial class Directory
        {
            internal static bool projectInit;
            internal static string projectValue;

            public static string Project
            {
                get
                {
                    if (!projectInit)
                    {
                        projectValue = System.IO.Directory.GetCurrentDirectory();
#if !UNITY_5_3_OR_NEWER
                        var current = new System.IO.DirectoryInfo(Path.GetFullPath(projectValue));
                        while (current != null)
                        {
                            var slnFiles = current.GetFiles("*.sln");
                            if (slnFiles.Length > 0)
                            {
                                projectValue = current.FullName;
                                break;
                            }
                            current = current.Parent;
                        }
#endif
                        projectInit = true;
                    }
                    return projectValue;
                }
            }
        }

        public static partial class Directory
        {
            internal static bool assetInit;
            internal static string assetValue;
            internal static readonly List<string> assetTestArgs = new();

            public static string Asset
            {
                get
                {
                    if (!assetInit)
                    {
#if UNITY_5_3_OR_NEWER
                        assetValue = UnityEngine.Application.streamingAssetsPath;
#else
                        var args = new List<string>(Environment.GetCommandLineArgs());
                        args.AddRange(assetTestArgs);
                        for (var i = 0; i < args.Count; i++)
                        {
                            var arg = args[i];
                            if (string.IsNullOrEmpty(arg) || !(arg.StartsWith("-XFile.Directory.Asset") || arg.StartsWith("--XFile.Directory.Asset"))) continue;
                            var idx = arg.IndexOf('=');
                            if (idx != -1) assetValue = arg[(idx + 1)..].Trim();
                            else if (i < args.Count - 1) assetValue = args[i + 1].Trim();
                            if (!string.IsNullOrEmpty(assetValue)) break;
                        }
                        if (string.IsNullOrEmpty(assetValue)) assetValue = Path.Join(System.IO.Directory.GetCurrentDirectory(), "Assets");
#endif
                        assetInit = true;
                    }
                    return assetValue;
                }
            }
        }

        public static partial class Directory
        {
            internal static bool localInit;
            internal static string localValue;
            internal static readonly List<string> localTestArgs = new();

            public static string Local
            {
                get
                {
                    if (!localInit)
                    {
                        static string parseArg()
                        {
                            var args = new List<string>(Environment.GetCommandLineArgs());
                            args.AddRange(localTestArgs);
                            for (var i = 0; i < args.Count; i++)
                            {
                                var arg = args[i];
                                if (string.IsNullOrEmpty(arg) || !(arg.StartsWith("-XFile.Directory.Local") || arg.StartsWith("--XFile.Directory.Local"))) continue;
                                var idx = arg.IndexOf('=');
                                if (idx != -1) return arg[(idx + 1)..].Trim();
                                else if (i < args.Count - 1) return args[i + 1].Trim();
                            }
                            return string.Empty;
                        }

#if UNITY_5_3_OR_NEWER
                        if (UnityEngine.Application.isEditor || UnityEngine.Application.isBatchMode ||
                            UnityEngine.Application.platform == UnityEngine.RuntimePlatform.WindowsServer ||
                            UnityEngine.Application.platform == UnityEngine.RuntimePlatform.LinuxServer ||
                            UnityEngine.Application.platform == UnityEngine.RuntimePlatform.OSXServer)
                        {
                            localValue = parseArg();
                            if (string.IsNullOrEmpty(localValue)) localValue = Path.Join(UnityEngine.Application.dataPath, "..", "Local");
                        }
                        else localValue = UnityEngine.Application.persistentDataPath;
#else
                        localValue = parseArg();
                        if (string.IsNullOrEmpty(localValue)) localValue = Path.Join(System.IO.Directory.GetCurrentDirectory(), "Local");
#endif
                        if (!Exists(localValue)) Create(localValue);
                        localInit = true;
                    }
                    return localValue;
                }
            }
        }

        public static partial class Directory
        {
            internal static readonly DirectoryEvaluator evaluator = new();

            public static XString.IEvaluator Evaluator => evaluator;
        }

        internal sealed class DirectoryEvaluator : XString.IEvaluator
        {
            internal static readonly Regex Pattern = new(@"\$\{XFile\.Directory\.([^}]+?)\}", RegexOptions.Compiled);

            public string Evaluate(string expr)
            {
                if (string.IsNullOrEmpty(expr)) return expr;
                var visited = new HashSet<string>();

                string repl(Match m)
                {
                    var keyName = m.Groups[1].Value;
                    var key = "Directory." + keyName;

                    if (keyName.Contains("${")) return $"${{Nested.{m.Value}}}";

                    if (!visited.Add(key)) return $"${{Recursive.XFile.{key}}}";
                    try
                    {
                        var value = keyName switch
                        {
                            "Project" => Directory.Project,
                            "Local" => Directory.Local,
                            "Asset" => Directory.Asset,
                            _ => null,
                        };
                        if (!string.IsNullOrEmpty(value)) return Pattern.Replace(value, repl);
                        return $"${{Unknown.XFile.Directory.{keyName}}}";
                    }
                    finally { visited.Remove(key); }
                }

                return Pattern.Replace(expr, repl);
            }
        }
    }
}
