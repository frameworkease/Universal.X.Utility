// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Threading;
using System.Threading.Tasks;
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXLoom
{
    #region 定时器
    [Test]
    public async Task Timeout()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        var tcs = new TaskCompletionSource<bool>();
        var startTime = XTime.Milliseconds;
        var deltaTime = 0;

        var timer1 = XLoom.SetTimeout(() =>
        {
            deltaTime = (int)(XTime.Milliseconds - startTime);
            tcs.SetResult(true);
        }, 500, 0);

        var clearFlag = true;
        var timer2 = XLoom.SetTimeout(() => clearFlag = false, 500, 0);
        XLoom.ClearTimeout(timer2, 0);

        var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(1)));
        Assert.That(completedTask == tcs.Task, Is.True, "定时器回调应当在 1 秒内完成。");

        Assert.That(timer1, Is.GreaterThan(0), "返回的定时器应当大于 0。");
        Assert.That(deltaTime, Is.GreaterThanOrEqualTo(500), "等待时间应当大于或等于 500 毫秒。");
        Assert.That(clearFlag, Is.True, "清除的定时器应当不会被调用。");

        // 测试无效参数
        Assert.That(XLoom.SetTimeout(null, 100, 0), Is.EqualTo(-1), "空的回调函数应当返回 -1。");
        Assert.That(XLoom.SetTimeout(() => { }, -1, 0), Is.EqualTo(-1), "负的超时时间应当返回 -1。");
        Assert.That(XLoom.SetTimeout(() => { }, 100, -1), Is.EqualTo(-1), "无效的业务线程标识应当返回 -1。");
        Assert.That(XLoom.SetTimeout(() => { }, 100, 999), Is.EqualTo(-1), "超出范围的业务线程标识应当返回 -1。");
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public async Task Interval()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        var count = 0;
        var tcs = new TaskCompletionSource<bool>();
        var startTime = XTime.Milliseconds;
        var deltaTime = 0;

        var interval1 = 0;
        interval1 = XLoom.SetInterval(() =>
        {
            Interlocked.Increment(ref count);
            if (count >= 3)
            {
                deltaTime = (int)(XTime.Milliseconds - startTime);
                XLoom.ClearInterval(interval1, 1);
                tcs.SetResult(true);
            }
            throw new Exception("test interval panic"); // 触发 panic，下一个周期的定时器应当继续执行
        }, 200, 1);

        var clearFlag = true;
        var interval2 = XLoom.SetInterval(() => clearFlag = false, 200, 1);
        XLoom.ClearInterval(interval2, 1);

        var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(1)));
        Assert.That(completedTask == tcs.Task, Is.True, "定时器回调应当在 1 秒内完成。");

        Assert.That(interval1, Is.GreaterThan(0), "返回的定时器应当大于 0。");
        Assert.That(deltaTime, Is.GreaterThanOrEqualTo(600), "等待时间应当大于或等于 600 毫秒。");
        Assert.That(clearFlag, Is.True, "清除的定时器应当不会被调用。");

        // 测试无效参数
        Assert.That(XLoom.SetInterval(null, 100, 0), Is.EqualTo(-1), "空的回调函数应当返回 -1。");
        Assert.That(XLoom.SetInterval(() => { }, -1, 0), Is.EqualTo(-1), "负的间隔时间应当返回 -1。");
        Assert.That(XLoom.SetInterval(() => { }, 100, -1), Is.EqualTo(-1), "无效的业务线程标识应当返回 -1。");
        Assert.That(XLoom.SetInterval(() => { }, 100, 999), Is.EqualTo(-1), "超出范围的业务线程标识应当返回 -1。");
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }
    #endregion
}
