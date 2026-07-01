// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXPrefs
{
    private static XPrefs.Base sharedPrefs = new();
    private static bool ignoreSetup = false;

    [Test]
    public void Set()
    {
        if (ignoreSetup) return;
        sharedPrefs = new XPrefs.Base();

        #region Basic
        {
            Assert.That(sharedPrefs.Set("int", 42), Is.EqualTo(sharedPrefs));
            Assert.That(sharedPrefs.pairs["int"], Is.EqualTo(42));

            sharedPrefs.Set("int8", (sbyte)8);
            Assert.That(sharedPrefs.pairs["int8"], Is.EqualTo((sbyte)8));

            sharedPrefs.Set("int16", (short)16);
            Assert.That(sharedPrefs.pairs["int16"], Is.EqualTo((short)16));

            sharedPrefs.Set("int32", 32);
            Assert.That(sharedPrefs.pairs["int32"], Is.EqualTo(32));

            sharedPrefs.Set("int64", 64L);
            Assert.That(sharedPrefs.pairs["int64"], Is.EqualTo(64L));

            sharedPrefs.Set("float32", 3.14f);
            Assert.That(sharedPrefs.pairs["float32"], Is.EqualTo(3.14f));

            sharedPrefs.Set("float64", 3.14159);
            Assert.That(sharedPrefs.pairs["float64"], Is.EqualTo(3.14159));

            sharedPrefs.Set("string", "1000");
            Assert.That(sharedPrefs.pairs["string"], Is.EqualTo("1000"));

            sharedPrefs.Set("bool", true);
            Assert.That(sharedPrefs.pairs["bool"], Is.EqualTo(true));

            var nested = new XPrefs.Base();
            nested.Set("id", 1);
            nested.Set("name", "foo");
            sharedPrefs.Set("nested", nested);
            Assert.That(sharedPrefs.pairs["nested"], Is.EqualTo(nested));
        }
        #endregion

        #region Array
        {
            sharedPrefs.Set("ints", new int[] { 1, 2, 3 });
            Assert.That(sharedPrefs.pairs["ints"], Is.EqualTo(new int[] { 1, 2, 3 }));

            sharedPrefs.Set("int8s", new sbyte[] { 1, 2, 3 });
            Assert.That(sharedPrefs.pairs["int8s"], Is.EqualTo(new sbyte[] { 1, 2, 3 }));

            sharedPrefs.Set("int16s", new short[] { 1, 2, 3 });
            Assert.That(sharedPrefs.pairs["int16s"], Is.EqualTo(new short[] { 1, 2, 3 }));

            sharedPrefs.Set("int32s", new int[] { 1, 2, 3 });
            Assert.That(sharedPrefs.pairs["int32s"], Is.EqualTo(new int[] { 1, 2, 3 }));

            sharedPrefs.Set("int64s", new long[] { 1, 2, 3 });
            Assert.That(sharedPrefs.pairs["int64s"], Is.EqualTo(new long[] { 1, 2, 3 }));

            sharedPrefs.Set("float32s", new float[] { 1.1f, 2.2f, 3.3f });
            Assert.That(sharedPrefs.pairs["float32s"], Is.EqualTo(new float[] { 1.1f, 2.2f, 3.3f }));

            sharedPrefs.Set("float64s", new double[] { 1.1, 2.2, 3.3 });
            Assert.That(sharedPrefs.pairs["float64s"], Is.EqualTo(new double[] { 1.1, 2.2, 3.3 }));

            sharedPrefs.Set("strings", new string[] { "1001", "1002", "1003" });
            Assert.That(sharedPrefs.pairs["strings"], Is.EqualTo(new string[] { "1001", "1002", "1003" }));

            sharedPrefs.Set("bools", new bool[] { true, false, true });
            Assert.That(sharedPrefs.pairs["bools"], Is.EqualTo(new bool[] { true, false, true }));

            var nesteds = new XPrefs.IBase[3];
            for (int i = 0; i < 3; i++)
            {
                var nestedItem = new XPrefs.Base();
                nestedItem.Set("id", i);
                nestedItem.Set("name", $"foo{i}");
                nesteds[i] = nestedItem;
            }
            sharedPrefs.Set("nesteds", nesteds);
            Assert.That(sharedPrefs.pairs["nesteds"], Is.EqualTo(nesteds));
        }
        #endregion
    }

    [Test]
    public void Delete()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        Set();

        Assert.That(sharedPrefs.Delete("string"), Is.EqualTo(sharedPrefs));
        Assert.That(sharedPrefs.pairs.ContainsKey("string"), Is.False);

        sharedPrefs.Delete("nonexistent");
        Assert.That(sharedPrefs.pairs.ContainsKey("nonexistent"), Is.False);
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void Exists()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        Set();

        Assert.That(sharedPrefs.Exists("string"), Is.True);
        Assert.That(sharedPrefs.Exists("nonexistent"), Is.False);
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void Range()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        Set();

        #region All
        {
            sharedPrefs.Range((key, value) =>
            {
                Assert.That(sharedPrefs.pairs[key], Is.EqualTo(value));
                return true;
            });
        }
        #endregion

        #region Break
        {
            int count = 0;
            sharedPrefs.Range((key, value) =>
            {
                count++;
                return count < 2;
            });
            Assert.That(count, Is.EqualTo(2));
        }
        #endregion
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void Get()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        Set();

        Assert.That(sharedPrefs.Get("int"), Is.EqualTo(42));
        Assert.That(sharedPrefs.Get("int8"), Is.EqualTo((sbyte)8));
        Assert.That(sharedPrefs.Get("int16"), Is.EqualTo((short)16));
        Assert.That(sharedPrefs.Get("int32"), Is.EqualTo(32));
        Assert.That(sharedPrefs.Get("int64"), Is.EqualTo(64L));
        Assert.That(sharedPrefs.Get("float32"), Is.EqualTo(3.14f));
        Assert.That(sharedPrefs.Get("float64"), Is.EqualTo(3.14159));
        Assert.That(sharedPrefs.Get("string"), Is.EqualTo("1000"));
        Assert.That(sharedPrefs.Get("bool"), Is.EqualTo(true));

        var nested = sharedPrefs.Get("nested") as XPrefs.Base;
        Assert.That(nested, Is.Not.Null);
        Assert.That(nested.Get("id"), Is.EqualTo(1));
        Assert.That(nested.Get("name"), Is.EqualTo("foo"));

        Assert.That(sharedPrefs.Get("nonexistent", "defval"), Is.EqualTo("defval"));
        Assert.That(sharedPrefs.Get("", "defval"), Is.EqualTo("defval"));
        Assert.That(sharedPrefs.Get(""), Is.Null);
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void Gets()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        Set();

        var ints = sharedPrefs.Gets("ints");
        Assert.That(ints[0], Is.EqualTo(1));
        Assert.That(ints[1], Is.EqualTo(2));
        Assert.That(ints[2], Is.EqualTo(3));

        var int8s = sharedPrefs.Gets("int8s");
        Assert.That(int8s[0], Is.EqualTo((sbyte)1));
        Assert.That(int8s[1], Is.EqualTo((sbyte)2));
        Assert.That(int8s[2], Is.EqualTo((sbyte)3));

        var int16s = sharedPrefs.Gets("int16s");
        Assert.That(int16s[0], Is.EqualTo((short)1));
        Assert.That(int16s[1], Is.EqualTo((short)2));
        Assert.That(int16s[2], Is.EqualTo((short)3));

        var int32s = sharedPrefs.Gets("int32s");
        Assert.That(int32s[0], Is.EqualTo(1));
        Assert.That(int32s[1], Is.EqualTo(2));
        Assert.That(int32s[2], Is.EqualTo(3));

        var int64s = sharedPrefs.Gets("int64s");
        Assert.That(int64s[0], Is.EqualTo(1L));
        Assert.That(int64s[1], Is.EqualTo(2L));
        Assert.That(int64s[2], Is.EqualTo(3L));

        var float32s = sharedPrefs.Gets("float32s");
        Assert.That(float32s[0], Is.EqualTo(1.1f));
        Assert.That(float32s[1], Is.EqualTo(2.2f));
        Assert.That(float32s[2], Is.EqualTo(3.3f));

        var float64s = sharedPrefs.Gets("float64s");
        Assert.That(float64s[0], Is.EqualTo(1.1));
        Assert.That(float64s[1], Is.EqualTo(2.2));
        Assert.That(float64s[2], Is.EqualTo(3.3));

        var strings = sharedPrefs.Gets("strings");
        Assert.That(strings[0], Is.EqualTo("1001"));
        Assert.That(strings[1], Is.EqualTo("1002"));
        Assert.That(strings[2], Is.EqualTo("1003"));

        var bools = sharedPrefs.Gets("bools");
        Assert.That(bools[0], Is.EqualTo(true));
        Assert.That(bools[1], Is.EqualTo(false));
        Assert.That(bools[2], Is.EqualTo(true));

        var nesteds = sharedPrefs.Get("nesteds") as XPrefs.IBase[];
        Assert.That(nesteds, Is.Not.Null);
        Assert.That(nesteds[0].Get("id"), Is.EqualTo(0));
        Assert.That(nesteds[0].Get("name"), Is.EqualTo("foo0"));
        Assert.That(nesteds[1].Get("id"), Is.EqualTo(1));
        Assert.That(nesteds[1].Get("name"), Is.EqualTo("foo1"));
        Assert.That(nesteds[2].Get("id"), Is.EqualTo(2));
        Assert.That(nesteds[2].Get("name"), Is.EqualTo("foo2"));

        Assert.That(sharedPrefs.Gets("nonexistent", new object[] { 99, 100 }), Is.EqualTo(new object[] { 99, 100 }));
        Assert.That(sharedPrefs.Gets("", new object[] { 99, 100 }), Is.EqualTo(new object[] { 99, 100 }));
        Assert.That(sharedPrefs.Gets(""), Is.Null);
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void GetInt()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        Set();

        Assert.That(sharedPrefs.GetInt("int"), Is.EqualTo(42));
        Assert.That(sharedPrefs.GetInt("int8"), Is.EqualTo(8));
        Assert.That(sharedPrefs.GetInt("int16"), Is.EqualTo(16));
        Assert.That(sharedPrefs.GetInt("int32"), Is.EqualTo(32));
        Assert.That(sharedPrefs.GetInt("int64"), Is.EqualTo(64));
        Assert.That(sharedPrefs.GetInt("float32"), Is.EqualTo(3));
        Assert.That(sharedPrefs.GetInt("float64"), Is.EqualTo(3));
        Assert.That(sharedPrefs.GetInt("string"), Is.EqualTo(1000));
        Assert.That(sharedPrefs.GetInt("bool"), Is.EqualTo(1));
        Assert.That(sharedPrefs.GetInt("nonexistent", 99), Is.EqualTo(99));
        Assert.That(sharedPrefs.GetInt("", 99), Is.EqualTo(99));
        Assert.That(sharedPrefs.GetInt(""), Is.EqualTo(0));
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void GetInts()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        Set();

        Assert.That(sharedPrefs.GetInts("ints"), Is.EqualTo(new int[] { 1, 2, 3 }));
        Assert.That(sharedPrefs.GetInts("int8s"), Is.EqualTo(new int[] { 1, 2, 3 }));
        Assert.That(sharedPrefs.GetInts("int16s"), Is.EqualTo(new int[] { 1, 2, 3 }));
        Assert.That(sharedPrefs.GetInts("int32s"), Is.EqualTo(new int[] { 1, 2, 3 }));
        Assert.That(sharedPrefs.GetInts("int64s"), Is.EqualTo(new int[] { 1, 2, 3 }));
        Assert.That(sharedPrefs.GetInts("float32s"), Is.EqualTo(new int[] { 1, 2, 3 }));
        Assert.That(sharedPrefs.GetInts("float64s"), Is.EqualTo(new int[] { 1, 2, 3 }));
        Assert.That(sharedPrefs.GetInts("strings"), Is.EqualTo(new int[] { 1001, 1002, 1003 }));
        Assert.That(sharedPrefs.GetInts("bools"), Is.EqualTo(new int[] { 1, 0, 1 }));
        Assert.That(sharedPrefs.GetInts("nonexistent", new int[] { 99, 100 }), Is.EqualTo(new int[] { 99, 100 }));
        Assert.That(sharedPrefs.GetInts("", new int[] { 99, 100 }), Is.EqualTo(new int[] { 99, 100 }));
        Assert.That(sharedPrefs.GetInts(""), Is.Null);
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void GetFloat()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        Set();

        Assert.That(sharedPrefs.GetFloat("int"), Is.EqualTo(42f));
        Assert.That(sharedPrefs.GetFloat("int8"), Is.EqualTo(8f));
        Assert.That(sharedPrefs.GetFloat("int16"), Is.EqualTo(16f));
        Assert.That(sharedPrefs.GetFloat("int32"), Is.EqualTo(32f));
        Assert.That(sharedPrefs.GetFloat("int64"), Is.EqualTo(64f));
        Assert.That(sharedPrefs.GetFloat("float32"), Is.EqualTo(3.14f));
        Assert.That(sharedPrefs.GetFloat("float64"), Is.EqualTo(3.14159f));
        Assert.That(sharedPrefs.GetFloat("string"), Is.EqualTo(1000f));
        Assert.That(sharedPrefs.GetFloat("bool"), Is.EqualTo(1f));
        Assert.That(sharedPrefs.GetFloat("nonexistent", 99f), Is.EqualTo(99f));
        Assert.That(sharedPrefs.GetFloat("", 99f), Is.EqualTo(99f));
        Assert.That(sharedPrefs.GetFloat(""), Is.EqualTo(0f));
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void GetFloats()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        Set();

        Assert.That(sharedPrefs.GetFloats("int8s"), Is.EqualTo(new float[] { 1, 2, 3 }));
        Assert.That(sharedPrefs.GetFloats("int16s"), Is.EqualTo(new float[] { 1, 2, 3 }));
        Assert.That(sharedPrefs.GetFloats("int32s"), Is.EqualTo(new float[] { 1, 2, 3 }));
        Assert.That(sharedPrefs.GetFloats("int64s"), Is.EqualTo(new float[] { 1, 2, 3 }));
        Assert.That(sharedPrefs.GetFloats("float32s"), Is.EqualTo(new float[] { 1.1f, 2.2f, 3.3f }));
        Assert.That(sharedPrefs.GetFloats("float64s"), Is.EqualTo(new float[] { 1.1f, 2.2f, 3.3f }));
        Assert.That(sharedPrefs.GetFloats("strings"), Is.EqualTo(new float[] { 1001, 1002, 1003 }));
        Assert.That(sharedPrefs.GetFloats("bools"), Is.EqualTo(new float[] { 1, 0, 1 }));
        Assert.That(sharedPrefs.GetFloats("nonexistent", new float[] { 99, 100 }), Is.EqualTo(new float[] { 99, 100 }));
        Assert.That(sharedPrefs.GetFloats("", new float[] { 99, 100 }), Is.EqualTo(new float[] { 99, 100 }));
        Assert.That(sharedPrefs.GetFloats(""), Is.Null);
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void GetString()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        Set();

        Assert.That(sharedPrefs.GetString("int"), Is.EqualTo("42"));
        Assert.That(sharedPrefs.GetString("int8"), Is.EqualTo("8"));
        Assert.That(sharedPrefs.GetString("int16"), Is.EqualTo("16"));
        Assert.That(sharedPrefs.GetString("int32"), Is.EqualTo("32"));
        Assert.That(sharedPrefs.GetString("int64"), Is.EqualTo("64"));
        Assert.That(sharedPrefs.GetString("float32"), Is.EqualTo("3.14"));
        Assert.That(sharedPrefs.GetString("float64"), Is.EqualTo("3.14159"));
        Assert.That(sharedPrefs.GetString("string"), Is.EqualTo("1000"));
        Assert.That(sharedPrefs.GetString("bool"), Is.EqualTo("True"));
        Assert.That(sharedPrefs.GetString("nonexistent", "99"), Is.EqualTo("99"));
        Assert.That(sharedPrefs.GetString("", "defval"), Is.EqualTo("defval"));
        Assert.That(sharedPrefs.GetString(""), Is.EqualTo(""));
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void GetStrings()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        Set();

        Assert.That(sharedPrefs.GetStrings("ints"), Is.EqualTo(new string[] { "1", "2", "3" }));
        Assert.That(sharedPrefs.GetStrings("int8s"), Is.EqualTo(new string[] { "1", "2", "3" }));
        Assert.That(sharedPrefs.GetStrings("int16s"), Is.EqualTo(new string[] { "1", "2", "3" }));
        Assert.That(sharedPrefs.GetStrings("int32s"), Is.EqualTo(new string[] { "1", "2", "3" }));
        Assert.That(sharedPrefs.GetStrings("int64s"), Is.EqualTo(new string[] { "1", "2", "3" }));
        Assert.That(sharedPrefs.GetStrings("float32s"), Is.EqualTo(new string[] { "1.1", "2.2", "3.3" }));
        Assert.That(sharedPrefs.GetStrings("float64s"), Is.EqualTo(new string[] { "1.1", "2.2", "3.3" }));
        Assert.That(sharedPrefs.GetStrings("strings"), Is.EqualTo(new string[] { "1001", "1002", "1003" }));
        Assert.That(sharedPrefs.GetStrings("bools"), Is.EqualTo(new string[] { "True", "False", "True" }));
        Assert.That(sharedPrefs.GetStrings("nonexistent", new string[] { "99", "100" }), Is.EqualTo(new string[] { "99", "100" }));
        Assert.That(sharedPrefs.GetStrings("", new string[] { "99", "100" }), Is.EqualTo(new string[] { "99", "100" }));
        Assert.That(sharedPrefs.GetStrings(""), Is.Null);
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void GetBool()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        Set();

        Assert.That(sharedPrefs.GetBool("bool"), Is.True);
        Assert.That(sharedPrefs.GetBool("nonexistent", true), Is.True);
        Assert.That(sharedPrefs.GetBool("", true), Is.True);
        Assert.That(sharedPrefs.GetBool(""), Is.False);
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void GetBools()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        Set();

        Assert.That(sharedPrefs.GetBools("bools"), Is.EqualTo(new bool[] { true, false, true }));
        Assert.That(sharedPrefs.GetBools("nonexistent", new bool[] { false, false, true }), Is.EqualTo(new bool[] { false, false, true }));
        Assert.That(sharedPrefs.GetBools("", new bool[] { true, true, true }), Is.EqualTo(new bool[] { true, true, true }));
        Assert.That(sharedPrefs.GetBools(""), Is.Null);
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void Stringify()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        Set();

        #region Compact
        {
            var result = sharedPrefs.Stringify();
            Assert.That(result, Does.Not.Contain("\n"));
        }
        #endregion

        #region Pretty
        {
            var result = sharedPrefs.Stringify(true);
            Assert.That(result, Does.Contain("\n"));
        }
        #endregion
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void Parse()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        Set();

        ignoreSetup = true;
        try
        {
            var content = sharedPrefs.Stringify();
            Assert.That(sharedPrefs.Parse(content), Is.True);

            GetInt();
            GetInts();
            GetFloat();
            GetFloats();
            GetString();
            GetStrings();
            GetBool();
            GetBools();
        }
        finally { ignoreSetup = false; }
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void Evaluate()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        Set();

        Assert.That(sharedPrefs.Evaluate("${XPrefs.string} ${XPrefs.int} ${XPrefs.nested.name} ${XPrefs.nonexistent}"), Is.EqualTo("1000 42 foo ${Unknown.XPrefs.nonexistent}"));

        sharedPrefs.Set("empty", "");
        Assert.That(sharedPrefs.Evaluate("${XPrefs.empty}"), Is.EqualTo("${Unknown.XPrefs.empty}"));
        Assert.That(sharedPrefs.Evaluate("${XPrefs.nonexistent}"), Is.EqualTo("${Unknown.XPrefs.nonexistent}"));
        Assert.That(sharedPrefs.Evaluate("${XPrefs.outer${XPrefs.inner}}"), Is.EqualTo("${Nested.${XPrefs.outer${XPrefs.inner}}}"));

        sharedPrefs.Set("recursive1", "${XPrefs.recursive2}");
        sharedPrefs.Set("recursive2", "${XPrefs.recursive1}");
        Assert.That(sharedPrefs.Evaluate("${XPrefs.recursive1}"), Is.EqualTo("${Recursive.XPrefs.recursive1}"));
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }
}
