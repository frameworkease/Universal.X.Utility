// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;

namespace FrameworkEase.Universal.X.Utility
{
    public partial class XLog
    {
        public interface IAdapter
        {
            string Name { get; }
            void Setup();
            void Write(Data data);
            void Flush();
            void Reset();
        }

        public static class Adapter
        {
            internal static readonly object mutex = new();
            internal static readonly List<IAdapter> adapters = new();

            public static void Range(Func<IAdapter, bool> callback)
            {
                if (callback == null) return;
                foreach (var apt in adapters) if (!callback(apt)) break;
            }

            public static bool Setup(params IAdapter[] apt)
            {
                if (apt == null || apt.Length == 0) return false;
                lock (mutex)
                {
                    var ok = true;
                    foreach (var ele in apt)
                    {
                        if (ele == null)
                        {
                            ok = false;
                            continue;
                        }
                        if (adapters.Contains(ele))
                        {
#if UNITY_5_3_OR_NEWER
                            UnityHandler.origin.LogFormat(UnityEngine.LogType.Error, null, $"XLog.Adapter.Setup: duplicated adapter: {ele.Name}.");
#else
                            Console.Error.WriteLine($"XLog.Adapter.Setup: duplicated adapter: {ele.Name}.");
#endif
                            ok = false;
                            continue;
                        }
                        adapters.Add(ele);
                        ele.Setup();
#if UNITY_5_3_OR_NEWER
                        UnityHandler.origin.LogFormat(UnityEngine.LogType.Log, null, $"XLog.Adapter.Setup: registered adapter: {ele.Name}.");
#else
                        Console.WriteLine($"XLog.Adapter.Setup: registered adapter: {ele.Name}.");
#endif
                    }
                    return ok;
                }
            }

#if UNITY_5_3_OR_NEWER
            [UnityEngine.HideInCallstack]
#endif
            public static void Write(Data data)
            {
                if (adapters.Count == 0)
                {
#if UNITY_5_3_OR_NEWER
                    UnityHandler.origin.LogFormat(UnityEngine.LogType.Log, null, "{0}", data.Stringify());
#else
                    Console.WriteLine(data.Stringify());
#endif
                }
                else foreach (var apt in adapters) apt.Write(data);
            }

            public static void Flush()
            {
                lock (mutex)
                {
#if UNITY_5_3_OR_NEWER
                    UnityHandler.origin.LogFormat(UnityEngine.LogType.Log, null, $"XLog.Adapter.Flush: performing flush with {adapters.Count} adapter(s).");
#else
                    Console.WriteLine($"XLog.Adapter.Flush: performing flush with {adapters.Count} adapter(s).");
#endif
                    foreach (var apt in adapters) apt.Flush();
                }
            }

            public static bool Reset(IAdapter apt = null)
            {
                lock (mutex)
                {
                    if (apt == null)
                    {
                        var cnt = adapters.Count;
                        foreach (var ele in adapters) ele.Reset();
                        adapters.Clear();
#if UNITY_5_3_OR_NEWER
                        UnityHandler.origin.LogFormat(UnityEngine.LogType.Log, null, $"XLog.Adapter.Reset: unregistered {cnt} adapter(s).");
#else
                        Console.WriteLine($"XLog.Adapter.Reset: unregistered {cnt} adapter(s).");
#endif
                    }
                    else
                    {
                        var idx = adapters.IndexOf(apt);
                        if (idx < 0) return false;
                        adapters.RemoveAt(idx);
                        apt.Reset();
#if UNITY_5_3_OR_NEWER
                        UnityHandler.origin.LogFormat(UnityEngine.LogType.Log, null, $"XLog.Adapter.Reset: unregistered adapter: {apt.Name}.");
#else
                        Console.WriteLine($"XLog.Adapter.Reset: unregistered adapter: {apt.Name}.");
#endif
                    }
                    return true;
                }
            }
        }
    }
}
