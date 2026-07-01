// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

#if UNITY_5_3_OR_NEWER
using System;

namespace FrameworkEase.Universal.X.Utility
{
    public partial class XLog
    {
        internal partial class UnityHandler : UnityEngine.ILogHandler
        {
            internal static readonly UnityEngine.ILogHandler origin = UnityEngine.Debug.unityLogger.logHandler;

            [UnityEngine.HideInCallstack]
            public void LogFormat(UnityEngine.LogType type, UnityEngine.Object context, string format, params object[] args)
            {
                if (Adapter.adapters.Count == 0) origin.LogFormat(type, context, format, args);
                else
                {
                    var data = new Data
                    {
                        Level = type switch
                        {
                            UnityEngine.LogType.Error => Levels.Error,
                            UnityEngine.LogType.Warning => Levels.Warn,
                            _ => Levels.Info
                        },
                        Content = format,
                        Args = args,
                        Time = DateTime.Now
                    };
                    foreach (var apt in Adapter.adapters) apt.Write(data);
                }
            }

            [UnityEngine.HideInCallstack]
            public void LogException(Exception exception, UnityEngine.Object context)
            {
                if (Adapter.adapters.Count == 0) origin.LogException(exception, context);
                else
                {
                    var data = new Data
                    {
                        Level = Levels.Emergency,
                        Content = exception,
                        Time = DateTime.Now
                    };
                    foreach (var apt in Adapter.adapters) apt.Write(data);
                }
            }
        }

#if !FRAMEWORKEASE_UNITY_XLOG_REDIRECT_DISABLED
        internal partial class UnityHandler
        {
            [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.AfterAssembliesLoaded)]
            internal static void OnLoad()
            {
                UnityEngine.Debug.unityLogger.logHandler = new UnityHandler();
                origin.LogFormat(UnityEngine.LogType.Log, null, "XLog.Unity.OnLoad: UnityEngine.Debug.unityLogger.logHandler has been redirected.");
#if UNITY_EDITOR
                UnityEditor.EditorApplication.playModeStateChanged += state =>
                {
                    if (origin == null || state != UnityEditor.PlayModeStateChange.ExitingPlayMode) return;
                    UnityEngine.Debug.unityLogger.logHandler = origin;
                    origin.LogFormat(UnityEngine.LogType.Log, null, "XLog.Unity.OnLoad: UnityEngine.Debug.unityLogger.logHandler has been restored.");
                };
#endif
            }
        }
#endif
    }
}
#endif
