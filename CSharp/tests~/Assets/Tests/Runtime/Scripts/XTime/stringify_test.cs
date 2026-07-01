// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXTime
{
    [Test]
    public void Stringify()
    {
        // 准备测试数据
        var testDateTime = new DateTime(2024, 3, 21, 14, 30, 0, 123);
        var testSeconds = (int)(testDateTime - XTime.Initial).TotalSeconds;
        var testMilliseconds = (long)(testDateTime - XTime.Initial).TotalMilliseconds;
        var expectedDateTimeStr = "2024-03-21 14:30:00";
        var expectedDateStr = "2024-03-21";
        var expectedMillisStr = "2024-03-21 14:30:00.123";
        var expectedChineseStr = "2024年03月21日";

        // 验证秒级时间戳格式化
        Assert.That(XTime.Stringify(testSeconds), Is.EqualTo(expectedDateTimeStr), "默认格式的秒级时间戳格式化应正确。");
        Assert.That(XTime.Stringify(testSeconds, "yyyy-MM-dd"), Is.EqualTo(expectedDateStr), "自定义格式的秒级时间戳格式化应正确。");

        // 验证毫秒级时间戳格式化
        Assert.That(XTime.Stringify(testMilliseconds), Is.EqualTo(expectedMillisStr), "默认格式的毫秒级时间戳格式化应正确。");
        Assert.That(XTime.Stringify(testMilliseconds, "yyyy-MM-dd HH:mm"), Is.EqualTo("2024-03-21 14:30"), "自定义格式的毫秒级时间戳格式化应正确。");

        // 验证DateTime格式化
        Assert.That(XTime.Stringify(testDateTime), Is.EqualTo(expectedMillisStr), "默认格式的DateTime格式化应正确。");
        Assert.That(XTime.Stringify(testDateTime, "yyyy年MM月dd日"), Is.EqualTo(expectedChineseStr), "中文格式的DateTime格式化应正确。");
    }
}
