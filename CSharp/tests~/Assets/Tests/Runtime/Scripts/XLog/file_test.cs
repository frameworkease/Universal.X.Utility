// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXLog
{
    [Test]
    public void FileName()
    {
        var apt = new XLog.File();
        Assert.That(apt.Name, Is.EqualTo("File"));
    }

    [Test]
    public void FileSetup()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        #region Setup
        {
            var defer = ResetTestAdapter();
            try
            {
                var apt = new XLog.File
                {
                    Level = XLog.Levels.Error,
                    Count = 100,
                    Line = 1000000,
                    Size = 1 << 27,
                    Day = 7,
                    Path = XLog.File.DefaultPath
                };
                XLog.Adapter.Setup(apt);
                Assert.That(XLog.Adapter.adapters.Contains(apt), Is.True);
                Assert.That(apt.cdata, Is.Not.Null);
                Assert.That(Volatile.Read(ref apt.iclose), Is.EqualTo(1));
                Assert.That(apt.rwrite, Is.Not.Null);
                Assert.That(apt.rflush, Is.Not.Null);
                Assert.That(apt.wflush, Is.Not.Null);
                Assert.That(apt.rclose, Is.Not.Null);
                Assert.That(apt.wclose, Is.Not.Null);
                apt.Reset();
            }
            finally { defer(); }
        }
        #endregion

        #region Loop
        {
            foreach (var cfg in new (int line, int size)[]
                     {
                         (100, 1 << 27),
                         (1000000, 4000)
                     })
            {
                var defer = ResetTestAdapter();
                try
                {
                    var dir = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());
                    XFile.Directory.Create(dir);
                    var fbase = "Loop";
                    var fext = ".log";
                    var pat = new Regex($"^{fbase}\\.\\d{{4}}-\\d{{2}}-\\d{{2}}\\.\\d{{3}}{fext}$", RegexOptions.Compiled);
                    var file = Path.Join(dir, fbase + fext);

                    var rclean = new AutoResetEvent(false);
                    var wclean = new CountdownEvent(1);
                    var apt = new XLog.File
                    {
                        Level = XLog.Levels.Info,
                        Count = 5,
                        Line = cfg.line,
                        Size = cfg.size,
                        Day = 3,
                        Path = file,
                        rclean = rclean,
                        wclean = wclean
                    };
                    XLog.Adapter.Setup(apt);
                    for (var i = 0; i < 1000; i++) apt.Write(new XLog.Data { Level = XLog.Levels.Notice, Content = $"line num: #{i:D3}", Time = DateTime.Now });
                    apt.Flush();

                    var files = Directory.GetFiles(dir);
                    var count = 0;
                    var before = DateTime.Now.AddDays(-4);
                    foreach (var f in files)
                    {
                        var fileName = Path.GetFileName(f);
                        if (pat.IsMatch(fileName))
                        {
                            if (count < 3)
                            {
                                File.SetCreationTime(f, before);
                                File.SetLastWriteTime(f, before);
                            }
                            count++;
                        }
                    }
                    Assert.That(count, Is.EqualTo(5));
                    Assert.That(files.Length, Is.EqualTo(6));
                    Assert.That(File.Exists(file), Is.True);
                    Assert.That(new FileInfo(file).Length, Is.EqualTo(0L));

                    rclean.Set();
                    wclean.Wait();

                    files = Directory.GetFiles(dir);
                    count = 0;
                    foreach (var f in files)
                    {
                        var fileName = Path.GetFileName(f);
                        if (pat.IsMatch(fileName)) count++;
                    }
                    Assert.That(count, Is.EqualTo(2));
                    apt.Reset();
                }
                finally { defer(); }
            }
        }
        #endregion
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void FileWrite()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        var defer = ResetTestAdapter();
        try
        {
            var file = XLog.File.DefaultPath.Evaluate(XFile.Directory.Evaluator);
            if (File.Exists(file)) File.Delete(file);
            try
            {
                File.WriteAllText(file, "init log\n");

                var apt = new XLog.File
                {
                    Level = XLog.Levels.Info,
                    Count = 100,
                    Line = 1000000,
                    Size = 1 << 27,
                    Day = 7,
                    Path = Path.Join(Path.GetTempPath(), ".invalid")
                };
                XLog.Adapter.Setup(apt);
                for (var i = 0; i < 100000; i++)
                {
                    var data = new XLog.Data { Content = $"test log {i}", Time = DateTime.Now };
                    if (i % 2 == 0) data.Level = XLog.Levels.Notice;
                    else data.Level = XLog.Levels.Debug;
                    apt.Write(data);
                }

                apt.Reset();
                for (var i = 0; i < 1000; i++) apt.Write(new XLog.Data { Level = XLog.Levels.Info, Content = $"test log {i}", Time = DateTime.Now });

                Assert.That(File.Exists(file), Is.True);
                var content = File.ReadAllText(file);
                Assert.That(content.Split(new[] { "init log" }, StringSplitOptions.None).Length - 1, Is.EqualTo(1));
                Assert.That(content.Split(new[] { "test log" }, StringSplitOptions.None).Length - 1, Is.EqualTo(50000));
            }
            finally
            {
                if (XFile.Exists(file)) XFile.Delete(file);
            }
        }
        finally { defer(); }
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void FileFlush()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        var defer = ResetTestAdapter();
        try
        {
            var dir = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());
            XFile.Directory.Create(dir);
            var file = Path.Join(dir, "Flush.log");
            var apt = new XLog.File
            {
                Level = XLog.Levels.Info,
                Count = 100,
                Line = 1000000,
                Size = 1 << 27,
                Day = 7,
                Path = file
            };
            XLog.Adapter.Setup(apt);
            for (var i = 0; i < 100000; i++) apt.Write(new XLog.Data { Level = XLog.Levels.Notice, Content = $"test log {i}", Time = DateTime.Now });

            var wg = new CountdownEvent(10);
            for (var i = 0; i < 10; i++)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    apt.Flush();
                    wg.Signal();
                });
            }
            wg.Wait();

            apt.Reset();
            apt.Flush();

            Assert.That(File.Exists(file), Is.True);
            var content = File.ReadAllText(file);
            Assert.That(content.Split(new[] { "test log" }, StringSplitOptions.None).Length - 1, Is.EqualTo(100000));
        }
        finally { defer(); }
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void FileReset()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        var defer = ResetTestAdapter();
        try
        {
            var dir = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());
            XFile.Directory.Create(dir);
            var file = Path.Join(dir, "Reset.log");
            var apt = new XLog.File
            {
                Level = XLog.Levels.Info,
                Count = 100,
                Line = 1000000,
                Size = 1 << 27,
                Day = 7,
                Path = file
            };
            XLog.Adapter.Setup(apt);
            for (var i = 0; i < 100000; i++) apt.Write(new XLog.Data { Level = XLog.Levels.Notice, Content = $"test log {i}", Time = DateTime.Now });

            var wg = new CountdownEvent(10);
            for (var i = 0; i < 10; i++)
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    apt.Reset();
                    wg.Signal();
                });
            wg.Wait();

            Assert.That(File.Exists(file), Is.True);
            var content = File.ReadAllText(file);
            Assert.That(content.Split(new[] { "test log" }, StringSplitOptions.None).Length - 1, Is.EqualTo(100000));
        }
        finally { defer(); }
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }
}
