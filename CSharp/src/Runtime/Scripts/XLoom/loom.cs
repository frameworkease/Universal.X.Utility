// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace FrameworkEase.Universal.X.Utility
{
    public partial class XLoom
#if UNITY_5_3_OR_NEWER
        : UnityEngine.MonoBehaviour
#endif
    {
        #region 业务线程
        internal class Loom : SynchronizationContext
        {
            internal readonly ConcurrentQueue<(SendOrPostCallback, object)> Queue = new();
            public override void Post(SendOrPostCallback callback, object state) { Queue.Enqueue((callback, state)); }
        }
        #endregion

        #region 静态变量
        internal static readonly Mutex initMutex = new();
        internal static int loomCount = 0;
        internal static int loomQueue = 50000;
        internal static Loom[] looms = Array.Empty<Loom>();
        internal static readonly ConcurrentDictionary<Thread, int> loomThreads = new();
        internal static bool[] loomPauses = Array.Empty<bool>();
        #endregion

        #region 初始化
#if UNITY_5_3_OR_NEWER
        internal static XLoom instance;
#endif
        public static void Setup(int count, int step, int queue)
        {
            if (count <= 0 || step <= 0 || queue <= 0) throw new Exception($"XLoom.Setup: invalid parameters, count: {count}, step: {step}, queue: {queue}.");

            lock (initMutex)
            {
#if UNITY_5_3_OR_NEWER
                if (instance == null)
                {
                    var go = new UnityEngine.GameObject("[Loom Updater]");
                    go.AddComponent<XLoom>();
                    DontDestroyOnLoad(go);
                }
#endif

                foreach (var thread in loomThreads.Keys)
                {
                    if (thread == Thread.CurrentThread) continue;
                    try { thread.Interrupt(); }
                    catch { }
                }
                loomThreads.Clear();

                loomCount = count;
                loomQueue = queue;
                looms = new Loom[count];
                loomPauses = new bool[count];

                for (var i = 0; i < count; i++)
                {
                    looms[i] = new Loom();
                    loomPauses[i] = false;
                }

                allTimers = new List<Timer>[count];
                newTimers = new List<Timer>[count];
                removeTimers = new List<int>[count];

                for (var i = 0; i < count; i++)
                {
                    allTimers[i] = new List<Timer>();
                    newTimers[i] = new List<Timer>();
                    removeTimers[i] = new List<int>();
                }

                for (var i = 0; i < count; i++)
                {
                    var loomID = i;
                    var loom = looms[loomID];
#if UNITY_5_3_OR_NEWER
                    if (loomID == 0) loomThreads[Thread.CurrentThread] = loomID;
                    else
#endif
                    {
                        var thread = new Thread(() =>
                        {
                            // 设置业务线程的同步上下文
                            // 通过队列确保异步任务执行的线程一致性
                            SynchronizationContext.SetSynchronizationContext(loom);
                            Loop(loomID, step);
                        });
                        loomThreads[thread] = loomID;
                        thread.Start();
                    }
                }

                XLog.Notice($"XLoom.Setup: allocated {count} loom(s).");
            }
        }

        public static void Reset()
        {
#if UNITY_5_3_OR_NEWER
            if (instance != null)
            {
                UnityEngine.Object.DestroyImmediate(instance.gameObject);
                instance = null;
            }
#else
            lock (initMutex)
            {
                foreach (var thread in loomThreads.Keys)
                {
                    if (thread == Thread.CurrentThread) continue;
                    try { thread.Interrupt(); }
                    catch { }
                }
            }
#endif
            loomThreads.Clear();
            loomCount = 0;
            loomQueue = 0;
            looms = Array.Empty<Loom>();
            loomPauses = Array.Empty<bool>();
            allTimers = null;
            newTimers = null;
            removeTimers = null;
        }
        #endregion

        #region 自循环
#if UNITY_5_3_OR_NEWER
        internal void Awake() { instance = this; }

        internal void Update() { Loop(0, 0); }

        internal void OnDestroy()
        {
            lock (initMutex)
            {
                foreach (var thread in loomThreads.Keys)
                {
                    if (thread == Thread.CurrentThread) continue;
                    try { thread.Interrupt(); }
                    catch { }
                }
            }
            instance = null;
        }
#endif

        internal static void Loop(int loomID, int step)
        {
            try
            {
                while (true)
                {
                    if (!loomPauses[loomID])
                    {
                        while (looms[loomID].Queue.TryDequeue(out var task))
                        {
                            try { task.Item1(task.Item2); }
                            catch (Exception e) { XLog.Error(new Exception($"XLoom.Loop({loomID}): execute runin failed: ", e)); }
                        }
                        try { Tick(loomID); }
                        catch (Exception e) { XLog.Error(new Exception($"XLoom.Loop({loomID}): tick timers failed: ", e)); }
                    }
                    if (step > 0) Thread.Sleep(step);
                    else break;
                }
            }
            catch (Exception e) when (e is OperationCanceledException || e is ThreadInterruptedException || e is ThreadAbortException) { XLog.Notice($"XLoom.Loop({loomID}): receive signal of interrupt."); }
            catch (Exception e) { XLog.Error(new Exception($"XLoom.Loop({loomID}): unexpected exception: ", e)); }
        }
        #endregion

        #region 公共接口
        public static void Pause(int? loomID = null)
        {
            if (loomID != null)
            {
                if (loomID.Value < 0) { XLog.Error($"XLoom.Pause: loom id of {loomID.Value} can not be zero or negative."); return; }
                if (loomID.Value >= loomCount) { XLog.Error($"XLoom.Pause: loom id of {loomID.Value} can not equals or greater than {loomCount}."); return; }
                loomPauses[loomID.Value] = true;
            }
            else for (var id = 0; id < loomPauses.Length; id++) loomPauses[id] = true;
        }

        public static void Resume(int? loomID = null)
        {
            if (loomID != null)
            {
                if (loomID.Value < 0) { XLog.Error($"XLoom.Resume: loom id of {loomID.Value} can not be zero or negative."); return; }
                if (loomID.Value >= loomCount) { XLog.Error($"XLoom.Resume: loom id of {loomID.Value} can not equals or greater than {loomCount}."); return; }
                loomPauses[loomID.Value] = false;
            }
            else for (var id = 0; id < loomPauses.Length; id++) loomPauses[id] = false;
        }

        public static bool RunIn(Action callback, int? loomID = null) { return RunIn(_ => callback(), loomID); }

        public static bool RunIn(SendOrPostCallback callback, int? loomID = null, object state = null)
        {
            if (callback == null) { XLog.Error("XLoom.RunIn: callback can not be nil."); return false; }
            loomID ??= 0;
            if (loomID.Value < 0) { XLog.Error($"XLoom.RunIn: loom id of {loomID.Value} can not be zero or negative."); return false; }
            if (loomID.Value >= loomCount) { XLog.Error($"XLoom.RunIn: loom id of {loomID.Value} can not equals or greater than {loomCount}."); return false; }
            if (looms[loomID.Value].Queue.Count >= loomQueue) { XLog.Error($"XLoom.RunIn: too many runins of {loomID.Value} (queue limit: {loomQueue})."); return false; }
            looms[loomID.Value].Queue.Enqueue((callback, state));
            return true;
        }

        public static int Count => loomCount;

        public static int ID(int? threadID = null)
        {
            if (threadID != null)
            {
                foreach (var thread in loomThreads.Keys)
                {
                    if (thread.ManagedThreadId == threadID) return loomThreads[thread];
                }
                return -1;
            }
            return loomThreads.TryGetValue(Thread.CurrentThread, out var loomID) ? loomID : -1;
        }
        #endregion
    }
}
