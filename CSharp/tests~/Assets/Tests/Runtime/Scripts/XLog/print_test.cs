// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXLog
{
    [Test]
    public void PrintAll()
    {
        var prints = new[]
        {
            new { level = XLog.Levels.Emergency, print = new Action<object, object[]>((data, args) => XLog.Emergency(data, args)) },
            new { level = XLog.Levels.Alert, print = new Action<object, object[]>((data, args) => XLog.Alert(data, args)) },
            new { level = XLog.Levels.Critical, print = new Action<object, object[]>((data, args) => XLog.Critical(data, args)) },
            new { level = XLog.Levels.Error, print = new Action<object, object[]>((data, args) => XLog.Error(data, args)) },
            new { level = XLog.Levels.Warn, print = new Action<object, object[]>((data, args) => XLog.Warn(data, args)) },
            new { level = XLog.Levels.Notice, print = new Action<object, object[]>((data, args) => XLog.Notice(data, args)) },
            new { level = XLog.Levels.Info, print = new Action<object, object[]>((data, args) => XLog.Info(data, args)) },
            new { level = XLog.Levels.Debug, print = new Action<object, object[]>((data, args) => XLog.Debug(data, args)) },
        };

        var defer = ResetTestAdapter();
        try
        {
            XLog.Adapter.Setup(new MyAdapter { name = "adapter1" });
            XLog.Adapter.Setup(new MyAdapter { name = "adapter2" });

            var tag = new XLog.Tag();
            foreach (var print in prints) print.print("test log", new object[] { tag });

            Assert.That(((MyAdapter)XLog.Adapter.adapters[0]).datas.Count, Is.EqualTo(8));
            Assert.That(((MyAdapter)XLog.Adapter.adapters[1]).datas.Count, Is.EqualTo(8));
        }
        finally { defer(); }
    }

    [Test]
    public void PrintTag()
    {
        var defer = ResetTestAdapter();
        try
        {
            XLog.Adapter.Setup(new MyAdapter { name = "adapter1" });

            var tag = new XLog.Tag { pairs = new List<string> { "key", "value" } };
            XLog.Info("test log", tag);

            var data = ((MyAdapter)XLog.Adapter.adapters[0]).datas[0];
            Assert.That(data.Tag.Get("key"), Is.EqualTo("value"));
        }
        finally { defer(); }
    }
}
