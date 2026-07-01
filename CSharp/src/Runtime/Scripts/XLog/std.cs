// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;

namespace FrameworkEase.Universal.X.Utility
{
    public partial class XLog
    {
        public class Std : IAdapter
        {
            internal static Func<string, string> Brush(string color)
            {
#if UNITY_5_3_OR_NEWER
                return (text) => $"<color={color}><b>{text}</b></color>";
#else
                var pre = "\u001b[";
                var reset = "\u001b[0m";
                return (text) => pre + color + "m" + text + reset;
#endif
            }

            internal static readonly Func<string, string>[] Brushes =
            {
#if UNITY_5_3_OR_NEWER
                Brush("black"),   // Emergency
                Brush("cyan"),    // Alert
                Brush("magenta"), // Critical
                Brush("red"),     // Error
                Brush("yellow"),  // Warn
                Brush("green"),   // Notice
                Brush("grey"),    // Info
                Brush("blue"),    // Debug
#else
                Brush("1;39"), // Emergency
                Brush("1;36"), // Alert
                Brush("1;35"), // Critical
                Brush("1;31"), // Error
                Brush("1;33"), // Warn
                Brush("1;32"), // Notice
                Brush("1;30"), // Info
                Brush("1;34"), // Debug
#endif
            };

            public virtual string Name { get; protected set; } = "Std";

            public Levels Level;

            public bool Color;

            public virtual void Setup() { }

#if UNITY_5_3_OR_NEWER
            [UnityEngine.HideInCallstack]
#endif
            public virtual void Write(Data data)
            {
                if (data.Level <= Level)
                {
                    var tm = data.Time.ToString("[MM/dd HH:mm:ss.fff] ");
                    var lv = Labels[(int)data.Level];
                    if (Color && data.Level != Levels.Unknown) lv = Brushes[(int)data.Level - 1](lv);
                    var tag = data.Tag.Stringify();
                    var fmt = data.Content is string str ? str : null;
                    var ctt = fmt != null ? (data.Args != null && data.Args.Length > 0 ? string.Format(fmt, data.Args) : fmt) : (data.Content?.ToString() ?? "<nil>");
                    ctt = string.IsNullOrEmpty(tag) ? tm + lv + " " + ctt : tm + lv + " " + tag + " " + ctt;
#if UNITY_5_3_OR_NEWER
                    switch (data.Level)
                    {
                        case Levels.Emergency:
                            UnityHandler.origin.LogFormat(UnityEngine.LogType.Exception, null, "{0}", ctt);
                            break;
                        case <= Levels.Error:
                            UnityHandler.origin.LogFormat(UnityEngine.LogType.Error, null, "{0}", ctt);
                            break;
                        default:
                            UnityHandler.origin.LogFormat(UnityEngine.LogType.Log, null, "{0}", ctt);
                            break;
                    }
#else
                    Console.Out.WriteLine(ctt);
#endif
                }
            }

            public virtual void Flush()
            {
#if UNITY_5_3_OR_NEWER
                UnityHandler.origin.LogFormat(UnityEngine.LogType.Log, null, "XLog.Std.Flush: performed.");
#else
                Console.WriteLine("XLog.Std.Flush: performed.");
#endif
            }

            public virtual void Reset()
            {
#if UNITY_5_3_OR_NEWER
                UnityHandler.origin.LogFormat(UnityEngine.LogType.Log, null, "XLog.Std.Reset: performed.");
#else
                Console.WriteLine("XLog.Std.Reset: performed.");
#endif
            }
        }
    }
}
