// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;

namespace FrameworkEase.Universal.X.Utility
{
    public partial class XLog
    {
        internal static void ResolvePrint(object[] args, out Tag tag, out object[] nargs)
        {
            if (args is { Length: > 0 } && args[0] is Tag atag)
            {
                tag = atag;
                nargs = args[1..];
            }
            else
            {
                tag = default;
                nargs = args;
            }
        }

#if UNITY_5_3_OR_NEWER
        [UnityEngine.HideInCallstack]
#endif
        public static void Print(Levels level, Tag tag, object content, params object[] args) { Adapter.Write(new Data { Level = level, Content = content, Tag = tag, Args = args, Time = DateTime.Now }); }

#if UNITY_5_3_OR_NEWER
        [UnityEngine.HideInCallstack]
#endif
#if FRAMEWORKEASE_XLOG_CONDITIONAL_PRINT
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_EMERGENCY")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_ALERT")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_CRITICAL")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_ERROR")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_WARN")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_NOTICE")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_INFO")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_DEBUG")]
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
#endif
        public static void Emergency(object content, params object[] args)
        {
            ResolvePrint(args, out var tag, out var nargs);
            Print(Levels.Emergency, tag, content, nargs);
        }

#if UNITY_5_3_OR_NEWER
        [UnityEngine.HideInCallstack]
#endif
#if FRAMEWORKEASE_XLOG_CONDITIONAL_PRINT
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_ALERT")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_CRITICAL")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_ERROR")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_WARN")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_NOTICE")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_INFO")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_DEBUG")]
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
#endif
        public static void Alert(object content, params object[] args)
        {
            ResolvePrint(args, out var tag, out var nargs);
            Print(Levels.Alert, tag, content, nargs);
        }

#if UNITY_5_3_OR_NEWER
        [UnityEngine.HideInCallstack]
#endif
#if FRAMEWORKEASE_XLOG_CONDITIONAL_PRINT
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_CRITICAL")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_ERROR")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_WARN")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_NOTICE")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_INFO")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_DEBUG")]
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
#endif
        public static void Critical(object content, params object[] args)
        {
            ResolvePrint(args, out var tag, out var nargs);
            Print(Levels.Critical, tag, content, nargs);
        }

#if UNITY_5_3_OR_NEWER
        [UnityEngine.HideInCallstack]
#endif
#if FRAMEWORKEASE_XLOG_CONDITIONAL_PRINT
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_ERROR")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_WARN")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_NOTICE")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_INFO")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_DEBUG")]
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
#endif
        public static void Error(object content, params object[] args)
        {
            ResolvePrint(args, out var tag, out var nargs);
            Print(Levels.Error, tag, content, nargs);
        }

#if UNITY_5_3_OR_NEWER
        [UnityEngine.HideInCallstack]
#endif
#if FRAMEWORKEASE_XLOG_CONDITIONAL_PRINT
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_WARN")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_NOTICE")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_INFO")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_DEBUG")]
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
#endif
        public static void Warn(object content, params object[] args)
        {
            ResolvePrint(args, out var tag, out var nargs);
            Print(Levels.Warn, tag, content, nargs);
        }

#if UNITY_5_3_OR_NEWER
        [UnityEngine.HideInCallstack]
#endif
#if FRAMEWORKEASE_XLOG_CONDITIONAL_PRINT
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_NOTICE")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_INFO")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_DEBUG")]
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
#endif
        public static void Notice(object content, params object[] args)
        {
            ResolvePrint(args, out var tag, out var nargs);
            Print(Levels.Notice, tag, content, nargs);
        }

#if UNITY_5_3_OR_NEWER
        [UnityEngine.HideInCallstack]
#endif
#if FRAMEWORKEASE_XLOG_CONDITIONAL_PRINT
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_INFO")]
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_DEBUG")]
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
#endif
        public static void Info(object content, params object[] args)
        {
            ResolvePrint(args, out var tag, out var nargs);
            Print(Levels.Info, tag, content, nargs);
        }

#if UNITY_5_3_OR_NEWER
        [UnityEngine.HideInCallstack]
#endif
#if FRAMEWORKEASE_XLOG_CONDITIONAL_PRINT
        [System.Diagnostics.Conditional("FRAMEWORKEASE_XLOG_CONDITIONAL_DEBUG")]
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
#endif
        public static void Debug(object content, params object[] args)
        {
            ResolvePrint(args, out var tag, out var nargs);
            Print(Levels.Debug, tag, content, nargs);
        }
    }
}
