// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXLog
{
    internal class MyAdapter : XLog.IAdapter
    {
        public string name;
        public List<XLog.Data> datas = new();
        public bool bsetup;
        public bool breset;
        public bool bflush;

        public string Name => name;
        public void Setup() => bsetup = true;
        public void Reset() => breset = true;
        public void Write(XLog.Data data) => datas.Add(data);
        public void Flush() => bflush = true;
    }

    internal static Action ResetTestAdapter()
    {
        var originalAdapters = new List<XLog.IAdapter>(XLog.Adapter.adapters);

        XLog.Adapter.Reset();
        Assert.That(XLog.Adapter.adapters.Count, Is.EqualTo(0));

        return () =>
        {
            lock (XLog.Adapter.mutex)
            {
                XLog.Adapter.adapters.Clear();
                XLog.Adapter.adapters.AddRange(originalAdapters);
            }
        };
    }

    [Test]
    public void AdapterRange()
    {
        var defer = ResetTestAdapter();
        try
        {
            XLog.Adapter.Setup(new MyAdapter { name = "adapter1" }, new MyAdapter { name = "adapter2" }, new MyAdapter { name = "adapter3" });

            var names = new List<string>();
            XLog.Adapter.Range(adapter =>
            {
                names.Add(adapter.Name);
                return true;
            });
            Assert.That(names.Count, Is.EqualTo(3));
            Assert.That(names.Contains("adapter1"), Is.True);
            Assert.That(names.Contains("adapter2"), Is.True);
            Assert.That(names.Contains("adapter3"), Is.True);

            int count = 0;
            XLog.Adapter.Range(adapter =>
            {
                count++;
                return count < 2;
            });
            Assert.That(count, Is.EqualTo(2));
        }
        finally { defer(); }
    }

    [Test]
    public void AdapterManage()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        var defer = ResetTestAdapter();
        try
        {
            var apt1 = new MyAdapter { name = "adapter1" };
            var apt2 = new MyAdapter { name = "adapter2" };

            #region Setup
            {
                Assert.That(XLog.Adapter.Setup(apt1), Is.True);
                Assert.That(XLog.Adapter.Setup(apt1), Is.False);
                Assert.That(apt1.bsetup, Is.True);
                Assert.That(XLog.Adapter.adapters.Count, Is.EqualTo(1));
                Assert.That(XLog.Adapter.adapters.Contains(apt1), Is.True);

                Assert.That(XLog.Adapter.Setup(apt2), Is.True);
                Assert.That(apt2.bsetup, Is.True);
                Assert.That(XLog.Adapter.adapters.Count, Is.EqualTo(2));
                Assert.That(XLog.Adapter.adapters.Contains(apt2), Is.True);
            }
            #endregion

            #region Reset
            {
                Assert.That(XLog.Adapter.Reset(apt2), Is.True);
                Assert.That(XLog.Adapter.Reset(apt2), Is.False);
                Assert.That(apt2.breset, Is.True);
                Assert.That(XLog.Adapter.adapters.Count, Is.EqualTo(1));

                Assert.That(XLog.Adapter.Reset(null), Is.True);
                Assert.That(apt1.breset, Is.True);
                Assert.That(XLog.Adapter.adapters.Count, Is.EqualTo(0));
            }
            #endregion
        }
        finally { defer(); }
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void AdapterWrite()
    {
        var defer = ResetTestAdapter();
        try
        {
            XLog.Adapter.Write(new XLog.Data { Level = XLog.Levels.Info, Content = "test log" });

            var apt1 = new MyAdapter { name = "adapter1" };
            var apt2 = new MyAdapter { name = "adapter2" };
            XLog.Adapter.Setup(apt1);
            XLog.Adapter.Setup(apt2);

            for (int i = 0; i < 10; i++) XLog.Adapter.Write(new XLog.Data { Level = XLog.Levels.Info, Content = $"test log {i}" });
            Assert.That(apt1.datas.Count, Is.EqualTo(10));
            Assert.That(apt2.datas.Count, Is.EqualTo(10));
        }
        finally { defer(); }
    }

    [Test]
    public void AdapterFlush()
    {
        var defer = ResetTestAdapter();
        try
        {
            var apt1 = new MyAdapter { name = "adapter1" };
            var apt2 = new MyAdapter { name = "adapter2" };
            XLog.Adapter.Setup(apt1);
            XLog.Adapter.Setup(apt2);

            XLog.Adapter.Flush();
            Assert.That(apt1.bflush, Is.True);
            Assert.That(apt2.bflush, Is.True);
        }
        finally { defer(); }
    }
}
