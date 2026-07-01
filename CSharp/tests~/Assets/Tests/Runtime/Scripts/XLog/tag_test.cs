// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System.Collections.Generic;
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXLog
{
    [Test]
    public void TagCount()
    {
        var tag = new XLog.Tag();
        Assert.That(tag.Count, Is.EqualTo(0));

        tag.Set("key1", "value1");
        Assert.That(tag.Count, Is.EqualTo(1));
    }

    [Test]
    public void TagSet()
    {
        var tag = new XLog.Tag();
        tag.Set("key1", "value1");
        Assert.That(tag.Get("key1"), Is.EqualTo("value1"));

        tag.Set("key2", "value2");
        Assert.That(tag.Get("key2"), Is.EqualTo("value2"));

        tag.Set("key1", "value1_updated");
        Assert.That(tag.Get("key1"), Is.EqualTo("value1_updated"));
    }

    [Test]
    public void TagGet()
    {
        var tag = new XLog.Tag();
        Assert.That(tag.Get("nonexistent"), Is.EqualTo(""));

        tag.Set("key1", "value1");
        Assert.That(tag.Get("key1"), Is.EqualTo("value1"));
    }

    [Test]
    public void TagRange()
    {
        var tag = new XLog.Tag();
        tag.Set("key1", "value1");
        tag.Set("key2", "value2");
        tag.Set("key3", "value3");
        var pairs = new List<string>();
        tag.Range((key, value) =>
        {
            pairs.Add(key);
            pairs.Add(value);
            if (key == "key2") return false;
            else return true;
        });
        Assert.That(pairs, Is.EqualTo(new[] { "key1", "value1", "key2", "value2" }));
    }

    [Test]
    public void TagStringify()
    {
        var tag = new XLog.Tag();
        Assert.That(tag.Stringify(), Is.EqualTo(""));

        tag.Set("key1", "value1");
        Assert.That(tag.Stringify(), Is.EqualTo("[key1=value1]"));

        tag.Set("key2", "value2");
        Assert.That(tag.Stringify(), Is.EqualTo("[key1=value1, key2=value2]"));
    }
}
