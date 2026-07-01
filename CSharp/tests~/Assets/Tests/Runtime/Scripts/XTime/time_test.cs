// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXTime
{
    [Test]
    public void Current()
    {
        var now = DateTime.Now;
        var time = XTime.Current;
        Assert.That((time - now).TotalMilliseconds, Is.LessThanOrEqualTo(100), "获取的当前时间应在100毫秒误差范围内。");
    }

    [Test]
    public void Seconds()
    {
        var now = DateTime.Now;
        var expected = (int)(now - XTime.Initial).TotalSeconds;
        var seconds = XTime.Seconds;
        Assert.That(Math.Abs(seconds - expected), Is.LessThanOrEqualTo(1), "获取的秒级时间戳应在1秒误差范围内。");
    }

    [Test]
    public void Milliseconds()
    {
        var now = DateTime.Now;
        var expected = (long)(now - XTime.Initial).TotalMilliseconds;
        var milliseconds = XTime.Milliseconds;
        Assert.That(Math.Abs(milliseconds - expected), Is.LessThanOrEqualTo(100), "获取的毫秒级时间戳应在100毫秒误差范围内。");
    }

    [Test]
    public void Microseconds()
    {
        var now = DateTime.Now;
        var expected = (now.Ticks - XTime.Initial.Ticks) / 10;
        var microseconds = XTime.Microseconds;
        Assert.That(Math.Abs(microseconds - expected), Is.LessThanOrEqualTo(10000), "获取的微秒级时间戳应在10000微秒误差范围内。");
    }
}
