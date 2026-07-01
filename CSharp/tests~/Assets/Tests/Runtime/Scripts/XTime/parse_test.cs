// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXTime
{
    [Test]
    public void Parse()
    {
        // 准备测试数据
        var testDateTime = new DateTime(2024, 3, 21, 14, 30, 0, 123);
        var testSeconds = (int)(testDateTime - XTime.Initial).TotalSeconds;
        var testMilliseconds = (long)(testDateTime - XTime.Initial).TotalMilliseconds;
        var expectedTime = testDateTime;

        // 秒级时间戳转DateTime
        var timeFromSeconds = XTime.Parse(testSeconds);

        // 验证转换后的时间各个部分
        Assert.That(timeFromSeconds.Year, Is.EqualTo(expectedTime.Year), "年份应匹配。");
        Assert.That(timeFromSeconds.Month, Is.EqualTo(expectedTime.Month), "月份应匹配。");
        Assert.That(timeFromSeconds.Day, Is.EqualTo(expectedTime.Day), "日期应匹配。");
        Assert.That(timeFromSeconds.Hour, Is.EqualTo(expectedTime.Hour), "小时应匹配。");
        Assert.That(timeFromSeconds.Minute, Is.EqualTo(expectedTime.Minute), "分钟应匹配。");
        Assert.That(timeFromSeconds.Second, Is.EqualTo(expectedTime.Second), "秒数应匹配。");

        // 毫秒级时间戳转DateTime
        var timeFromMilliseconds = XTime.Parse(testMilliseconds);

        // 验证转换后的时间各个部分
        Assert.That(timeFromMilliseconds.Year, Is.EqualTo(expectedTime.Year), "年份应匹配。");
        Assert.That(timeFromMilliseconds.Month, Is.EqualTo(expectedTime.Month), "月份应匹配。");
        Assert.That(timeFromMilliseconds.Day, Is.EqualTo(expectedTime.Day), "日期应匹配。");
        Assert.That(timeFromMilliseconds.Hour, Is.EqualTo(expectedTime.Hour), "小时应匹配。");
        Assert.That(timeFromMilliseconds.Minute, Is.EqualTo(expectedTime.Minute), "分钟应匹配。");
        Assert.That(timeFromMilliseconds.Second, Is.EqualTo(expectedTime.Second), "秒数应匹配。");
        Assert.That(timeFromMilliseconds.Millisecond, Is.EqualTo(expectedTime.Millisecond), "毫秒数应匹配。");
    }
}
