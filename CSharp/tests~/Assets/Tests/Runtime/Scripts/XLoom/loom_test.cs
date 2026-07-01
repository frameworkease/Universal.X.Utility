// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXLoom
{
    [OneTimeSetUp]
    public void Setup() { XLoom.Setup(10, 10, 1000); }

    [OneTimeTearDown]
    public void Reset() { XLoom.Reset(); }

    [Test]
    public void Count() { Assert.That(XLoom.Count, Is.EqualTo(10), "应当有 10 个业务线程。"); }

    [Test]
    public async Task ID()
    {
        #region 当前线程
        {
            var tcs = new TaskCompletionSource<int>();
            XLoom.RunIn(() => tcs.SetResult(XLoom.ID()), 0);
            await Task.WhenAny(tcs.Task, Task.Delay(1000));
            Assert.That(tcs.Task.Result, Is.EqualTo(0), "业务线程标识应当为 0。");
        }
        #endregion

        #region 特定线程
        {
            var tcs = new TaskCompletionSource<int>();
            XLoom.RunIn(() => tcs.SetResult(XLoom.ID(Environment.CurrentManagedThreadId)), 1);
            await Task.WhenAny(tcs.Task, Task.Delay(1000));
            Assert.That(tcs.Task.Result, Is.EqualTo(1), "业务线程标识应当为 1。");
        }
        #endregion
    }

    [Test]
    public async Task RunIn()
    {
        var tcss = new List<TaskCompletionSource<bool>>();
        var rets = new List<int[]>();
        for (var i = 0; i < XLoom.Count; i++)
        {
            var loomID = i;
            tcss.Add(new TaskCompletionSource<bool>());
            rets.Add(new int[2]);
            XLoom.RunIn(async () =>
            {
                rets[loomID][0] = Environment.CurrentManagedThreadId;
                await Task.Delay(200);
                rets[loomID][1] = Environment.CurrentManagedThreadId;
                tcss[loomID].SetResult(true);
            }, loomID);
        }
        await Task.WhenAll(tcss.Select(tcs => tcs.Task));
        for (var i = 0; i < XLoom.Count; i++) Assert.That(rets[i][0], Is.EqualTo(rets[i][1]), "异步任务应当在同一个线程中执行。");
    }

    [Test]
    public async Task Manage()
    {
        var tcs = new TaskCompletionSource<bool>();
        XLoom.Pause(0);
        XLoom.RunIn(() => tcs.SetResult(true), 0);
        XLoom.Resume(0);
        await Task.WhenAny(tcs.Task, Task.Delay(1000));
        Assert.That(tcs.Task.Result, Is.True, "任务应当在 1 秒内完成。");
    }
}
