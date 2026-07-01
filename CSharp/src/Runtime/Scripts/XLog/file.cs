// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FrameworkEase.Universal.X.Utility
{
    public partial class XLog
    {
        public class File : IAdapter
        {
            internal const string DefaultPath = "${XFile.Directory.Local}/Log/App.log";
            internal ConcurrentQueue<Data> cdata;
            internal int iclose;
            internal AutoResetEvent rwrite;
            internal AutoResetEvent rflush;
            internal ConcurrentQueue<CountdownEvent> wflush;
            internal AutoResetEvent rclose;
            internal AutoResetEvent wclose;
            internal AutoResetEvent rclean;
            internal CountdownEvent wclean;

            public virtual string Name { get; protected set; } = "File";
            public Levels Level;
            public int Count;
            public int Line;
            public int Size;
            public int Day;
            public string Path;

            public virtual void Setup()
            {
                if (Volatile.Read(ref iclose) != 0) return;

                var pcount = Count;
                var pline = Line;
                var psize = Size;
                var pday = Day;
                var ppath = Path.Evaluate(XFile.Directory.Evaluator, XEnv.Argument.Evaluator, XEnv.Variable.Evaluator);

                var pdir = System.IO.Path.GetDirectoryName(ppath);
                var pbase = System.IO.Path.GetFileName(ppath);
                var pext = System.IO.Path.GetExtension(ppath);

                if (string.IsNullOrEmpty(pdir) || string.IsNullOrEmpty(pbase) || string.IsNullOrEmpty(pext) || pbase == pext)
                {
                    var dpath = DefaultPath.Evaluate(XFile.Directory.Evaluator);
#if UNITY_5_3_OR_NEWER
                    UnityHandler.origin.LogFormat(UnityEngine.LogType.Error, null, $"XLog.File.Setup: invalid path format: {ppath}, use default: {dpath}");
#else
                    Console.Error.WriteLine($"XLog.File.Setup: invalid path format: {ppath}, use default: {dpath}");
#endif
                    ppath = dpath;
                    pdir = System.IO.Path.GetDirectoryName(ppath);
                    pext = System.IO.Path.GetExtension(ppath);
                    pbase = System.IO.Path.GetFileName(ppath);
                }
                pbase = pbase.Replace(pext, "");

                cdata = new ConcurrentQueue<Data>();
                rwrite = new AutoResetEvent(false);
                rflush = new AutoResetEvent(false);
                wflush = new ConcurrentQueue<CountdownEvent>();
                rclose = new AutoResetEvent(false);
                wclose = new AutoResetEvent(false);

                var winit = new CountdownEvent(1);
                Task.Run(() =>
                {
                    try
                    {
                        StreamWriter fwriter = null;
                        int fline = 0;
                        long fsize = 0;

                        void create()
                        {
                            if (fwriter != null)
                            {
                                try { fwriter.Close(); }
                                catch { }
                                fwriter = null;
                            }
                            try
                            {
                                if (!XFile.Directory.Exists(pdir)) XFile.Directory.Create(pdir);
                            }
                            catch (Exception e)
                            {
#if UNITY_5_3_OR_NEWER
                                UnityHandler.origin.LogFormat(UnityEngine.LogType.Error, null, $"XLog.File.Setup.Create: failed to create dir: {e}");
#else
                                Console.Error.WriteLine($"XLog.File.Setup.Create: failed to create dir: {e}");
#endif
                                return;
                            }

                            fsize = 0;
                            fline = 0;
                            try
                            {
                                if (System.IO.File.Exists(ppath))
                                {
                                    var finfo = new FileInfo(ppath);
                                    fsize = finfo.Length;
                                    if (finfo.Length > 0 && pline > 0)
                                    {
                                        using var reader = new StreamReader(ppath, new UTF8Encoding(false));
                                        while (reader.ReadLine() != null) fline++;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
#if UNITY_5_3_OR_NEWER
                                UnityHandler.origin.LogFormat(UnityEngine.LogType.Error, null, $"XLog.File.Setup.Create: failed to get stat: {e}");
#else
                                Console.Error.WriteLine($"XLog.File.Setup.Create: failed to get stat: {e}");
#endif
                                return;
                            }

                            try
                            {
                                var fs = new FileStream(ppath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                                fwriter = new StreamWriter(fs, new UTF8Encoding(false)) { AutoFlush = true };
                            }
                            catch (Exception e)
                            {
#if UNITY_5_3_OR_NEWER
                                UnityHandler.origin.LogFormat(UnityEngine.LogType.Error, null, $"XLog.File.Setup.Create: failed to open file: {e}");
#else
                                Console.Error.WriteLine($"XLog.File.Setup.Create: failed to open file: {e}");
#endif
                            }
                        }

                        void cleanup()
                        {
                            try
                            {
                                var pat = new Regex($"^{Regex.Escape(pbase)}\\.\\d{{4}}-\\d{{2}}-\\d{{2}}\\.\\d{{3}}{Regex.Escape(pext)}$", RegexOptions.Compiled);
                                var files = new List<FileInfo>();
                                if (Directory.Exists(pdir))
                                {
                                    foreach (var entry in Directory.GetFiles(pdir))
                                    {
                                        var name = System.IO.Path.GetFileName(entry);
                                        if (pat.IsMatch(name))
                                        {
                                            try { files.Add(new FileInfo(entry)); }
                                            catch (Exception e)
                                            {
#if UNITY_5_3_OR_NEWER
                                                UnityHandler.origin.LogFormat(UnityEngine.LogType.Error, null, $"XLog.File.Setup.Cleanup: failed to get info: {e}");
#else
                                                Console.Error.WriteLine($"XLog.File.Setup.Cleanup: failed to get info: {e}");
#endif
                                            }
                                        }
                                    }
                                }
                                files.Sort((a, b) => a.LastWriteTime.CompareTo(b.LastWriteTime));
                                var before = DateTime.Now.AddDays(-pday);
                                for (int i = 0; i < files.Count; i++)
                                {
                                    var file = files[i];
                                    if ((pcount > 0 && files.Count - i > pcount) || (pday > 0 && file.LastWriteTime < before))
                                    {
                                        try { System.IO.File.Delete(file.FullName); }
                                        catch (Exception e)
                                        {
#if UNITY_5_3_OR_NEWER
                                            UnityHandler.origin.LogFormat(UnityEngine.LogType.Error, null, $"XLog.File.Setup.Cleanup: failed to remove file {file.Name}: {e}");
#else
                                            Console.Error.WriteLine($"XLog.File.Setup.Cleanup: failed to remove file {file.Name}: {e}");
#endif
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
#if UNITY_5_3_OR_NEWER
                                UnityHandler.origin.LogFormat(UnityEngine.LogType.Error, null, $"XLog.File.Setup.Cleanup: failed to read dir: {e}");
#else
                                Console.Error.WriteLine($"XLog.File.Setup.Cleanup: failed to read dir: {e}");
#endif
                            }
                        }

                        void rotate()
                        {
                            if (fwriter != null)
                            {
                                try { fwriter.Close(); }
                                catch { }
                                fwriter = null;
                            }

                            var num = 1;
                            var sdate = DateTime.Now.ToString("yyyy-MM-dd");
                            try
                            {
                                if (Directory.Exists(pdir))
                                {
                                    var pat = new Regex($"^{Regex.Escape(pbase)}\\.{Regex.Escape(sdate)}\\.(\\d{{3}}){Regex.Escape(pext)}$", RegexOptions.Compiled);
                                    foreach (var file in Directory.GetFiles(pdir))
                                    {
                                        var name = System.IO.Path.GetFileName(file);
                                        var match = pat.Match(name);
                                        if (match.Success && int.TryParse(match.Groups[1].Value, out var val) && val >= num) num = val + 1;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
#if UNITY_5_3_OR_NEWER
                                UnityHandler.origin.LogFormat(UnityEngine.LogType.Error, null, $"XLog.File.Setup.Rotate: failed to read dir: {e}");
#else
                                Console.Error.WriteLine($"XLog.File.Setup.Rotate: failed to read dir: {e}");
#endif
                            }

                            var tfile = System.IO.Path.Join(pdir, $"{pbase}.{sdate}.{num:D3}{pext}");
                            try
                            {
                                if (System.IO.File.Exists(ppath)) System.IO.File.Move(ppath, tfile);
                            }
                            catch (Exception e)
                            {
#if UNITY_5_3_OR_NEWER
                                UnityHandler.origin.LogFormat(UnityEngine.LogType.Error, null, $"XLog.File.Setup.Rotate: failed to rotate file: {e}");
#else
                                Console.Error.WriteLine($"XLog.File.Setup.Rotate: failed to rotate file: {e}");
#endif
                            }
                        }

                        void write(Data data)
                        {
                            if (fwriter != null)
                            {
                                try
                                {
                                    var str = data.Stringify();
                                    fwriter.WriteLine(str);
                                    fline++;
                                    fsize += str.Length + 1;
                                }
                                catch (Exception e)
                                {
#if UNITY_5_3_OR_NEWER
                                    UnityHandler.origin.LogFormat(UnityEngine.LogType.Error, null, $"XLog.File.Setup.Write: failed to write: {e}");
#else
                                    Console.Error.WriteLine($"XLog.File.Setup.Write: failed to write: {e}");
#endif
                                }
                            }

                            if ((pline > 0 && fline >= pline) || (psize > 0 && fsize >= psize))
                            {
                                rotate();
                                cleanup();
                                create();
                                if (fwriter == null)
                                {
#if UNITY_5_3_OR_NEWER
                                    UnityHandler.origin.LogFormat(UnityEngine.LogType.Error, null, "XLog.File.Setup.Write: failed to new writer.");
#else
                                    Console.Error.WriteLine("XLog.File.Setup.Write: failed to new writer.");
#endif
                                }
                            }
                        }

                        try
                        {
#if UNITY_5_3_OR_NEWER
                            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += _ => Flush();
                            UnityEngine.Application.quitting += Reset;
#if UNITY_EDITOR
                            UnityEditor.EditorApplication.quitting += Reset;
                            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += Reset;
#endif
#else
                            Console.CancelKeyPress += (_, e) =>
                            {
                                e.Cancel = true;
                                Reset();
                            };
                            AppDomain.CurrentDomain.ProcessExit += (_, _) => Reset();
#endif

                            var signals = rclean != null ? new WaitHandle[] { rwrite, rflush, rclose, rclean } : new WaitHandle[] { rwrite, rflush, rclose };
                            using var _ = new Timer(_ => cleanup(), null, TimeSpan.FromHours(1), TimeSpan.FromHours(1));

                            create();
                            Interlocked.Exchange(ref iclose, 1);
                            winit.Signal();

                            while (true)
                            {
                                var signal = WaitHandle.WaitAny(signals);
                                if (signal >= 0)
                                {
                                    while (cdata.TryDequeue(out var data)) write(data);
                                    if (signal == 1)
                                    {
                                        while (wflush.TryDequeue(out var wg))
                                        {
                                            fwriter?.Flush();
                                            wg.Signal();
                                        }
                                    }
                                    else if (signal == 2)
                                    {
                                        break;
                                    }
                                    else if (signal == 3)
                                    {
                                        cleanup();
                                        wclean?.Signal();
                                    }
                                }
                            }
                        }
                        finally
                        {
                            Interlocked.Exchange(ref iclose, 2);
                            while (cdata.TryDequeue(out var data)) write(data);
                            if (fwriter != null)
                            {
                                fwriter.Flush();
                                fwriter.Close();
                            }
                            wclose.Set();
                        }
                    }
                    catch (Exception e)
                    {
#if UNITY_5_3_OR_NEWER
                        UnityHandler.origin.LogFormat(UnityEngine.LogType.Error, null, $"XLog.File.Setup.Loop: unhandled exception: {e}");
#else
                        Console.Error.WriteLine($"XLog.File.Setup.Loop: unhandled exception: {e}");
#endif
                    }
                });

                winit.Wait();
            }

            public virtual void Write(Data data)
            {
                if (data.Level <= Level && Volatile.Read(ref iclose) == 1)
                {
                    cdata.Enqueue(data);
                    rwrite.Set();
                }
            }

            public virtual void Flush()
            {
                if (Volatile.Read(ref iclose) != 1) return;
                var wg = new CountdownEvent(1);
                wflush.Enqueue(wg);
                rflush.Set();
                wg.Wait();
#if UNITY_5_3_OR_NEWER
                UnityHandler.origin.LogFormat(UnityEngine.LogType.Log, null, "XLog.File.Flush: performed.");
#else
                Console.WriteLine("XLog.File.Flush: performed.");
#endif
            }

            public virtual void Reset()
            {
                if (Interlocked.CompareExchange(ref iclose, 2, 1) != 1) return;
                wclose.Reset();
                rclose.Set();
                wclose.WaitOne();
#if UNITY_5_3_OR_NEWER
                UnityHandler.origin.LogFormat(UnityEngine.LogType.Log, null, "XLog.File.Reset: performed.");
#else
                Console.WriteLine("XLog.File.Reset: performed.");
#endif
            }
        }
    }
}
