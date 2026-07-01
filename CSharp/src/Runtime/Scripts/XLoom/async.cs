// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Threading.Tasks;

namespace FrameworkEase.Universal.X.Utility
{
    public partial class XLoom
    {
        #region 异步执行
        public static async Task RunAsync(Action callback, bool restart = false)
        {
            if (callback == null) return;

            try { await Task.Run(callback); }
            catch (Exception e)
            {
                XLog.Error(e);
                if (restart) await RunAsync(callback, restart);
            }
        }

        public static async Task RunAsync<T1>(Action<T1> callback, T1 arg1, bool restart = false)
        {
            if (callback == null) return;

            try { await Task.Run(() => callback(arg1)); }
            catch (Exception e)
            {
                XLog.Error(e);
                if (restart) await RunAsync<T1>(callback, arg1, restart);
            }
        }

        public static async Task RunAsync<T1, T2>(Action<T1, T2> callback, T1 arg1, T2 arg2, bool restart = false)
        {
            if (callback == null) return;

            try { await Task.Run(() => callback(arg1, arg2)); }
            catch (Exception e)
            {
                XLog.Error(e);
                if (restart) await RunAsync(callback, arg1, arg2, restart);
            }
        }

        public static async Task RunAsync<T1, T2, T3>(Action<T1, T2, T3> callback, T1 arg1, T2 arg2, T3 arg3, bool restart = false)
        {
            if (callback == null) return;

            try { await Task.Run(() => callback(arg1, arg2, arg3)); }
            catch (Exception e)
            {
                XLog.Error(e);
                if (restart) await RunAsync(callback, arg1, arg2, arg3, restart);
            }
        }
        #endregion
    }
}
