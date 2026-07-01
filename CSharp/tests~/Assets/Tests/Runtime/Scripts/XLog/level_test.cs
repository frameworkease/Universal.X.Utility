// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXLog
{
    [Test]
    public void Levels()
    {
        var tests = new[]
        {
            new { name = "Unknown", level = XLog.Levels.Unknown, want = (sbyte)0 },
            new { name = "Emergency", level = XLog.Levels.Emergency, want = (sbyte)1 },
            new { name = "Alert", level = XLog.Levels.Alert, want = (sbyte)2 },
            new { name = "Critical", level = XLog.Levels.Critical, want = (sbyte)3 },
            new { name = "Error", level = XLog.Levels.Error, want = (sbyte)4 },
            new { name = "Warn", level = XLog.Levels.Warn, want = (sbyte)5 },
            new { name = "Notice", level = XLog.Levels.Notice, want = (sbyte)6 },
            new { name = "Info", level = XLog.Levels.Info, want = (sbyte)7 },
            new { name = "Debug", level = XLog.Levels.Debug, want = (sbyte)8 },
        };

        foreach (var tt in tests) Assert.That((sbyte)tt.level, Is.EqualTo(tt.want));
    }

    [Test]
    public void Labels()
    {
        var expected = new[] { "[U]", "[M]", "[A]", "[C]", "[E]", "[W]", "[N]", "[I]", "[D]" };

        Assert.That(XLog.Labels.Length, Is.EqualTo(expected.Length));
        for (var i = 0; i < expected.Length; i++) Assert.That(XLog.Labels[i], Is.EqualTo(expected[i]));

        var levels = new[]
        {
            XLog.Levels.Unknown,
            XLog.Levels.Emergency,
            XLog.Levels.Alert,
            XLog.Levels.Critical,
            XLog.Levels.Error,
            XLog.Levels.Warn,
            XLog.Levels.Notice,
            XLog.Levels.Info,
            XLog.Levels.Debug
        };
        for (var i = 0; i < levels.Length; i++) Assert.That(XLog.Labels[(int)levels[i]], Is.EqualTo(expected[i]));
    }
}
