// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.Threading;

namespace FrameworkEase.Universal.X.Utility
{
    public partial class XLoom
    {
        internal static long timerIncrement = 0;
        internal static List<Timer>[] allTimers = Array.Empty<List<Timer>>();
        internal static List<Timer>[] newTimers = Array.Empty<List<Timer>>();
        internal static List<int>[] removeTimers = Array.Empty<List<int>>();

        internal class Timer
        {
            internal int ID { get; set; }
            internal Action Callback { get; set; }
            internal long Initial { get; set; }
            internal long Period { get; set; }
            internal long Trigger { get; set; }
            internal long Repeat { get; set; }
            internal bool Panic { get; set; }
        }

        internal static void Tick(int loomID)
        {
            // 添加新定时器
            if (newTimers[loomID].Count > 0)
            {
                lock (newTimers[loomID])
                {
                    allTimers[loomID].AddRange(newTimers[loomID]);
                    newTimers[loomID].Clear();
                }
            }

            // 删除定时器
            if (removeTimers[loomID].Count > 0)
            {
                lock (removeTimers[loomID])
                {
                    foreach (var id in removeTimers[loomID])
                    {
                        for (int i = allTimers[loomID].Count - 1; i >= 0; i--)
                        {
                            if (allTimers[loomID][i].ID == id)
                            {
                                allTimers[loomID].RemoveAt(i);
                                break;
                            }
                        }
                    }
                    removeTimers[loomID].Clear();
                }
            }

            // 更新定时器
            if (allTimers[loomID] != null)
            {
                var nowTime = XTime.Milliseconds;
                for (int i = allTimers[loomID].Count - 1; i >= 0; i--)
                {
                    var timer = allTimers[loomID][i];
                    if (timer.Panic)
                    {
                        if (timer.Repeat > 0) // interval 发生 panic 不取消定时器
                        {
                            timer.Panic = false;
                            timer.Trigger = timer.Initial + timer.Period * (++timer.Repeat);
                        }
                        else // timeout 发生 panic 则直接移除
                        {
                            lock (removeTimers[loomID]) removeTimers[loomID].Add(timer.ID);
                            continue;
                        }
                    }
                    if (timer.Trigger <= nowTime) // 因存在固定刷新间歇，可能会导致间歇调用的周期越来越长
                    {
                        if (timer.Callback != null)
                        {
                            timer.Panic = true;
                            try { timer.Callback(); }
                            catch (Exception e) { XLog.Error(new Exception($"XLoom.Tick({loomID}): execute timer callback failed: ", e)); }
                            timer.Panic = false;
                        }
                        if (timer.Repeat == 0)
                        {
                            lock (removeTimers[loomID]) removeTimers[loomID].Add(timer.ID);
                        }
                        else timer.Trigger = timer.Initial + timer.Period * (++timer.Repeat);
                    }
                }
            }
        }

        public static int SetTimeout(Action callback, long timeout, int? loomID = null)
        {
            if (callback == null)
            {
                XLog.Critical("XLoom.SetTimeout: callback can not be nil.");
                return -1;
            }
            if (timeout < 0)
            {
                XLog.Critical($"XLoom.SetTimeout: timeout of {timeout} can not be zero or negative.");
                return -1;
            }

            loomID ??= ID();
            if (loomID.Value < 0)
            {
                XLog.Critical($"XLoom.SetTimeout: loom id of {loomID.Value} can not be zero or negative.");
                return -1;
            }
            if (loomID.Value >= loomCount)
            {
                XLog.Critical($"XLoom.SetTimeout: loom id of {loomID.Value} can not equals or greater than {loomCount}.");
                return -1;
            }

            var now = XTime.Milliseconds;
            var timer = new Timer
            {
                ID = (int)Interlocked.Increment(ref timerIncrement),
                Callback = callback,
                Initial = now,
                Period = timeout,
                Repeat = 0,
                Trigger = now + timeout,
            };
            lock (newTimers[loomID.Value]) newTimers[loomID.Value].Add(timer);

            return timer.ID;
        }

        public static void ClearTimeout(int id, int? loomID = null)
        {
            loomID ??= ID();
            if (loomID.Value < 0)
            {
                XLog.Critical($"XLoom.ClearTimeout: loom id of {loomID.Value} can not be zero or negative.");
                return;
            }
            if (loomID.Value >= loomCount)
            {
                XLog.Critical($"XLoom.ClearTimeout: loom id of {loomID.Value} can not equals or greater than {loomCount}.");
                return;
            }
            lock (removeTimers[loomID.Value]) removeTimers[loomID.Value].Add(id);
        }

        public static int SetInterval(Action callback, long interval, int? loomID = null)
        {
            if (callback == null)
            {
                XLog.Critical("XLoom.SetInterval: callback can not be nil.");
                return -1;
            }
            if (interval < 0)
            {
                XLog.Critical($"XLoom.SetInterval: interval of {interval} can not be zero or negative.");
                return -1;
            }

            loomID ??= ID();
            if (loomID.Value < 0)
            {
                XLog.Critical($"XLoom.SetInterval: loom id of {loomID.Value} can not be zero or negative.");
                return -1;
            }
            if (loomID.Value >= loomCount)
            {
                XLog.Critical($"XLoom.SetInterval: loom id of {loomID.Value} can not equals or greater than {loomCount}.");
                return -1;
            }

            var now = XTime.Milliseconds;
            var timer = new Timer
            {
                ID = (int)Interlocked.Increment(ref timerIncrement),
                Callback = callback,
                Initial = now,
                Period = interval,
                Repeat = 1,
                Trigger = now + interval,
            };
            lock (newTimers[loomID.Value]) newTimers[loomID.Value].Add(timer);

            return timer.ID;
        }

        public static void ClearInterval(int id, int? loomID = null) => ClearTimeout(id, loomID);
    }
}
