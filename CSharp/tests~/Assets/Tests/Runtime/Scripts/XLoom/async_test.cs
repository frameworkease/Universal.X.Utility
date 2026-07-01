// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Threading.Tasks;
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXLoom
{
    #region 异步任务
    [Test]
    public async Task Async()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        var executed = false;
        await XLoom.RunAsync(() => executed = true);
        Assert.That(executed, Is.True, "任务应当被执行。");

        var executeCount = 0;
        await XLoom.RunAsync(() =>
        {
            executeCount++;
            if (executeCount == 1) throw new Exception("test panic");
        }, true);
        Assert.That(executeCount, Is.EqualTo(2), "任务应当被执行 2 次。");
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public async Task AsyncT1()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        var executed = false;
        await XLoom.RunAsync(_ => executed = true, 1);
        Assert.That(executed, Is.True, "任务应当被执行。");

        var executeCount = 0;
        await XLoom.RunAsync(_ =>
        {
            executeCount++;
            if (executeCount == 1) throw new Exception("test panic");
        }, 1, true);
        Assert.That(executeCount, Is.EqualTo(2), "任务应当被执行 2 次。");
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public async Task AsyncT2()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        var executed = false;
        await XLoom.RunAsync((_, _) => executed = true, 1, 2);
        Assert.That(executed, Is.True, "任务应当被执行。");

        var executeCount = 0;
        await XLoom.RunAsync((_, _) =>
        {
            executeCount++;
            if (executeCount == 1) throw new Exception("test panic");
        }, 1, 2, true);
        Assert.That(executeCount, Is.EqualTo(2), "任务应当被执行 2 次。");
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public async Task AsyncT3()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        var executed = false;
        await XLoom.RunAsync((_, _, _) => executed = true, 1, 2, 3);
        Assert.That(executed, Is.True, "任务应当被执行。");

        var executeCount = 0;
        await XLoom.RunAsync((_, _, _) =>
        {
            executeCount++;
            if (executeCount == 1) throw new Exception("test panic");
        }, 1, 2, 3, true);
        Assert.That(executeCount, Is.EqualTo(2), "任务应当被执行 2 次。");
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }
    #endregion
}
