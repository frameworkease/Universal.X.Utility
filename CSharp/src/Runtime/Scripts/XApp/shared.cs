// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace FrameworkEase.Universal.X.Utility
{
    public static partial class XApp
    {
        internal static IBase instance;

        internal static int runOnce = 0;

        internal static int quitOnce = 0;

        internal static readonly CancellationTokenSource quitSource = new();

        internal static readonly object quitLock = new();

        public static T Instance<T>() where T : class, IBase => instance as T;

        public static async Task Run(IBase application)
        {
            if (Interlocked.CompareExchange(ref runOnce, 1, 0) != 0) return;

            instance = application ?? throw new Exception("XApp.Run: application is null.");
            if (!instance.Awake()) throw new Exception("XApp.Run: application awake failed.");

            Event.Notify(Events.OnAwake);
            XLog.Notice("XApp.Run: application has been awaked.");

            application.Start();
            Event.Notify(Events.OnStart);
            XLog.Notice("XApp.Run: application has been started.");

            try
            {
                var cancelSource = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true;
                    cancelSource.Cancel();
                };

                try
                {
                    var quitTask = Task.Run(() =>
                    {
                        try
                        {
                            quitSource.Token.WaitHandle.WaitOne();
                            XLog.Notice("XApp.Run: receive signal of quit.");
                        }
                        catch (ObjectDisposedException) { }
                    });

                    var cancelTask = Task.Run(() =>
                    {
                        try
                        {
                            cancelSource.Token.WaitHandle.WaitOne();
                            XLog.Notice("XApp.Run: receive signal of cancel.");
                        }
                        catch (ObjectDisposedException) { }
                    });

                    await Task.WhenAny(quitTask, cancelTask);
                }
                finally { cancelSource.Dispose(); }
            }
            finally
            {
                var counter = new CountdownEvent(1);
                try
                {
                    application.Stop(counter);
                    Event.Notify(Events.OnStop, counter);
                    counter.Signal();
                    await Task.Run(() => counter.Wait());
                }
                catch (Exception e) { XLog.Error(e); }
                finally { counter.Dispose(); }
                XLog.Notice("XApp.Run: application has been stopped.");
#if UNITY_5_3_OR_NEWER
                UnityEngine.Application.Quit();
#endif
            }
        }

        public static void Quit()
        {
            if (Interlocked.CompareExchange(ref quitOnce, 1, 0) != 0) return;

            lock (quitLock)
            {
                try { quitSource.Cancel(); }
                catch { }
            }
        }
    }
}
