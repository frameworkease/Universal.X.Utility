// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System.Collections.Generic;
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXLog
{
    [Test]
    public void DataStringify()
    {
        var tests = new[]
        {
            new { level = XLog.Levels.Info, data = (object)null, args = (object[])null, tag = new XLog.Tag(), want = "[01/01 00:00:00.000] [I] <nil>" },
            new { level = XLog.Levels.Info, data = (object)"{0} {1}", args = new object[] { "test", "format" }, tag = new XLog.Tag(), want = "[01/01 00:00:00.000] [I] test format" },
            new { level = XLog.Levels.Info, data = (object)new { Arg0 = "test", Arg1 = "struct" }, args = (object[])null, tag = new XLog.Tag(), want = "[01/01 00:00:00.000] [I] { Arg0 = test, Arg1 = struct }" },
            new { level = XLog.Levels.Info, data = (object)"test tag", args = (object[])null, tag = new XLog.Tag { pairs = new List<string> { "key1", "value1", "key2", "value2" } }, want = "[01/01 00:00:00.000] [I] [key1=value1, key2=value2] test tag" },
            new { level = XLog.Levels.Emergency, data = (object)"test emergency", args = (object[])null, tag = new XLog.Tag(), want = "[01/01 00:00:00.000] [M] test emergency" },
            new { level = XLog.Levels.Alert, data = (object)"test alert", args = (object[])null, tag = new XLog.Tag(), want = "[01/01 00:00:00.000] [A] test alert" },
            new { level = XLog.Levels.Critical, data = (object)"test critical", args = (object[])null, tag = new XLog.Tag(), want = "[01/01 00:00:00.000] [C] test critical" },
            new { level = XLog.Levels.Error, data = (object)"test error", args = (object[])null, tag = new XLog.Tag(), want = "[01/01 00:00:00.000] [E] test error" },
            new { level = XLog.Levels.Warn, data = (object)"test warn", args = (object[])null, tag = new XLog.Tag(), want = "[01/01 00:00:00.000] [W] test warn" },
            new { level = XLog.Levels.Notice, data = (object)"test notice", args = (object[])null, tag = new XLog.Tag(), want = "[01/01 00:00:00.000] [N] test notice" },
            new { level = XLog.Levels.Info, data = (object)"test info", args = (object[])null, tag = new XLog.Tag(), want = "[01/01 00:00:00.000] [I] test info" },
            new { level = XLog.Levels.Debug, data = (object)"test debug", args = (object[])null, tag = new XLog.Tag(), want = "[01/01 00:00:00.000] [D] test debug" },
        };

        foreach (var tt in tests)
        {
            var data = new XLog.Data
            {
                Level = tt.level,
                Content = tt.data,
                Args = tt.args,
                Tag = tt.tag,
            };
            var result = data.Stringify();
            Assert.That(result, Is.EqualTo(tt.want));
        }
    }
}
