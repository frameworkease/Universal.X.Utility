// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System.Collections.Generic;
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;
#if UNITY_5_3_OR_NEWER
using System.Text.RegularExpressions;
#else
using System;
using System.IO;
#endif

public partial class TestXLog
{
    [Test]
    public void StdName()
    {
        var apt = new XLog.Std { Level = XLog.Levels.Info, Color = false };
        Assert.That(apt.Name, Is.EqualTo("Std"));
    }

    [Test]
    public void StdWrite()
    {
        #region Able
        {
#if UNITY_5_3_OR_NEWER
            {
                var apt = new XLog.Std { Level = XLog.Levels.Notice };
                UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Log, new Regex(@"\[01/01 00:00:00\.000\] \[N\] test able"));
                apt.Write(new XLog.Data { Level = XLog.Levels.Notice, Content = "test able" });
            }
#else
            var origin = Console.Out;
            try
            {
                using (var writer = new StringWriter())
                {
                    Console.SetOut(writer);
                    var apt = new XLog.Std { Level = XLog.Levels.Error };
                    apt.Write(new XLog.Data { Level = XLog.Levels.Notice, Content = "test able" });
                    Assert.That(writer.ToString(), Is.Empty, "日志级别低于期望时，不写入日志。");
                }

                using (var writer = new StringWriter())
                {
                    Console.SetOut(writer);
                    var apt = new XLog.Std { Level = XLog.Levels.Notice };
                    apt.Write(new XLog.Data { Level = XLog.Levels.Notice, Content = "test able" });
                    Assert.That(writer.ToString(), Is.Not.Empty, "日志级别符合期望时，应写入日志。");
                }
            }
            finally { Console.SetOut(origin); }
#endif
        }
        #endregion

        #region Format
        {
#if UNITY_5_3_OR_NEWER
            var apt = new XLog.Std { Level = XLog.Levels.Info };
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Log, new Regex(@"\[01/01 00:00:00\.000\] \[I\] <nil>"));
            apt.Write(new XLog.Data { Level = XLog.Levels.Info });
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Log, new Regex(@"\[01/01 00:00:00\.000\] \[I\] test format"));
            apt.Write(new XLog.Data { Level = XLog.Levels.Info, Content = "{0} {1}", Args = new object[] { "test", "format" } });
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Log, new Regex(@"\[01/01 00:00:00\.000\] \[I\] \{ Arg0 = test, Arg1 = struct \}"));
            apt.Write(new XLog.Data { Level = XLog.Levels.Info, Content = new { Arg0 = "test", Arg1 = "struct" } });
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Log, new Regex(@"\[01/01 00:00:00\.000\] \[I\] \[key1=value1, key2=value2\] test tag"));
            apt.Write(new XLog.Data
            {
                Level = XLog.Levels.Info,
                Content = "test tag",
                Tag = new XLog.Tag { pairs = new List<string> { "key1", "value1", "key2", "value2" } }
            });
#else
            var origin = Console.Out;
            try
            {
                using var writer = new StringWriter();
                Console.SetOut(writer);
                var apt = new XLog.Std { Level = XLog.Levels.Info };
                apt.Write(new XLog.Data { Level = XLog.Levels.Info });
                apt.Write(new XLog.Data { Level = XLog.Levels.Info, Content = "{0} {1}", Args = new object[] { "test", "format" } });
                apt.Write(new XLog.Data { Level = XLog.Levels.Info, Content = new { Arg0 = "test", Arg1 = "struct" } });
                apt.Write(new XLog.Data { Level = XLog.Levels.Info, Content = "test tag", Tag = new XLog.Tag { pairs = new List<string> { "key1", "value1", "key2", "value2" } } });

                var ctt = writer.ToString();
                Assert.That(ctt, Does.Contain("[01/01 00:00:00.000] [I] <nil>"));
                Assert.That(ctt, Does.Contain("[01/01 00:00:00.000] [I] test format"));
                Assert.That(ctt, Does.Contain("[01/01 00:00:00.000] [I] { Arg0 = test, Arg1 = struct }"));
                Assert.That(ctt, Does.Contain("[01/01 00:00:00.000] [I] [key1=value1, key2=value2] test tag"));
            }
            finally { Console.SetOut(origin); }
#endif
        }
        #endregion

        #region Color
        {
#if UNITY_5_3_OR_NEWER
            var apt = new XLog.Std { Level = XLog.Levels.Debug, Color = true };
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Exception, new Regex(@"\[01/01 00:00:00\.000\] <color=black><b>\[M\]</b></color> test emergency"));
            apt.Write(new XLog.Data { Level = XLog.Levels.Emergency, Content = "test emergency" });
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Error, new Regex(@"\[01/01 00:00:00\.000\] <color=cyan><b>\[A\]</b></color> test alert"));
            apt.Write(new XLog.Data { Level = XLog.Levels.Alert, Content = "test alert" });
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Error, new Regex(@"\[01/01 00:00:00\.000\] <color=magenta><b>\[C\]</b></color> test critical"));
            apt.Write(new XLog.Data { Level = XLog.Levels.Critical, Content = "test critical" });
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Error, new Regex(@"\[01/01 00:00:00\.000\] <color=red><b>\[E\]</b></color> test error"));
            apt.Write(new XLog.Data { Level = XLog.Levels.Error, Content = "test error" });
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Log, new Regex(@"\[01/01 00:00:00\.000\] <color=yellow><b>\[W\]</b></color> test warn"));
            apt.Write(new XLog.Data { Level = XLog.Levels.Warn, Content = "test warn" });
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Log, new Regex(@"\[01/01 00:00:00\.000\] <color=green><b>\[N\]</b></color> test notice"));
            apt.Write(new XLog.Data { Level = XLog.Levels.Notice, Content = "test notice" });
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Log, new Regex(@"\[01/01 00:00:00\.000\] <color=grey><b>\[I\]</b></color> test info"));
            apt.Write(new XLog.Data { Level = XLog.Levels.Info, Content = "test info" });
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Log, new Regex(@"\[01/01 00:00:00\.000\] <color=blue><b>\[D\]</b></color> test debug"));
            apt.Write(new XLog.Data { Level = XLog.Levels.Debug, Content = "test debug" });
#else
            var origin = Console.Out;
            try
            {
                using var writer = new StringWriter();
                Console.SetOut(writer);
                var apt = new XLog.Std { Level = XLog.Levels.Debug, Color = true };
                apt.Write(new XLog.Data { Level = XLog.Levels.Emergency, Content = "test emergency" });
                apt.Write(new XLog.Data { Level = XLog.Levels.Alert, Content = "test alert" });
                apt.Write(new XLog.Data { Level = XLog.Levels.Critical, Content = "test critical" });
                apt.Write(new XLog.Data { Level = XLog.Levels.Error, Content = "test error" });
                apt.Write(new XLog.Data { Level = XLog.Levels.Warn, Content = "test warn" });
                apt.Write(new XLog.Data { Level = XLog.Levels.Notice, Content = "test notice" });
                apt.Write(new XLog.Data { Level = XLog.Levels.Info, Content = "test info" });
                apt.Write(new XLog.Data { Level = XLog.Levels.Debug, Content = "test debug" });

                var ctt = writer.ToString();
                Assert.That(ctt, Does.Contain("[01/01 00:00:00.000] \u001b[1;39m[M]\u001b[0m test emergency"), "Emergency 级别应使用黑色 (1;39)");
                Assert.That(ctt, Does.Contain("[01/01 00:00:00.000] \u001b[1;36m[A]\u001b[0m test alert"), "Alert 级别应使用青色 (1;36)");
                Assert.That(ctt, Does.Contain("[01/01 00:00:00.000] \u001b[1;35m[C]\u001b[0m test critical"), "Critical 级别应使用品红色 (1;35)");
                Assert.That(ctt, Does.Contain("[01/01 00:00:00.000] \u001b[1;31m[E]\u001b[0m test error"), "Error 级别应使用红色 (1;31)");
                Assert.That(ctt, Does.Contain("[01/01 00:00:00.000] \u001b[1;33m[W]\u001b[0m test warn"), "Warn 级别应使用黄色 (1;33)");
                Assert.That(ctt, Does.Contain("[01/01 00:00:00.000] \u001b[1;32m[N]\u001b[0m test notice"), "Notice 级别应使用绿色 (1;32)");
                Assert.That(ctt, Does.Contain("[01/01 00:00:00.000] \u001b[1;30m[I]\u001b[0m test info"), "Info 级别应使用灰色 (1;30)");
                Assert.That(ctt, Does.Contain("[01/01 00:00:00.000] \u001b[1;34m[D]\u001b[0m test debug"), "Debug 级别应使用蓝色 (1;34)");
            }
            finally { Console.SetOut(origin); }
#endif
        }
        #endregion
    }

    [Test]
    public void StdFlush()
    {
        var apt = new XLog.Std { Level = XLog.Levels.Info, Color = false };
        apt.Flush();
    }

    [Test]
    public void StdClose()
    {
        var apt = new XLog.Std { Level = XLog.Levels.Info, Color = false };
        apt.Reset();
    }
}
