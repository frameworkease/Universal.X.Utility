// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXEnv
{
    [Test]
    public void Argument()
    {
        try
        {
            XEnv.Argument.Setup(
                    "--key1=value1",      // 双横杠等号格式
                    "-key2=value2",       // 单横杠等号格式
                    "--key3", "value3",   // 双横杠空格格式
                    "-key4", "value4",    // 单横杠空格格式
                    "--flag1",            // 双横杠无值
                    "-flag2",             // 单横杠无值
                    "--empty=",           // 空值参数
                    "invalid",            // 非法参数
                    "--key1=newValue1",   // 多值参数
                    "--key3=newValue3"    // 多值参数
                );

            #region Get
            {
                Assert.That(XEnv.Argument.Get("key1"), Is.EqualTo("value1"));
                Assert.That(XEnv.Argument.Get("key2"), Is.EqualTo("value2"));
                Assert.That(XEnv.Argument.Get("key3"), Is.EqualTo("value3"));
                Assert.That(XEnv.Argument.Get("key4"), Is.EqualTo("value4"));
                Assert.That(XEnv.Argument.Get("flag1"), Is.EqualTo(""));
                Assert.That(XEnv.Argument.Get("flag2"), Is.EqualTo(""));
                Assert.That(XEnv.Argument.Get("empty"), Is.EqualTo(""));
            }
            #endregion

            #region Exists
            {
                Assert.That(XEnv.Argument.Exists("key1"), Is.True);
                Assert.That(XEnv.Argument.Exists("invalid"), Is.False);
            }
            #endregion

            #region Range
            {
                var expected = new[]
                {
                    "key1=value1", "key2=value2", "key3=value3", "key4=value4", "flag1=", "flag2=", "empty=",
                    "key1=newValue1", "key3=newValue3"
                };
                var seen = new List<string>();
                XEnv.Argument.Range((key, value) =>
                {
                    seen.Add($"{key}={value}");
                    return true;
                });
                Assert.That(seen.Count, Is.GreaterThan(expected.Length));
                Assert.That(seen.GetRange(0, expected.Length), Is.EqualTo(expected));

                var count = 0;
                XEnv.Argument.Range((key, value) => ++count < 3);
                Assert.That(count, Is.EqualTo(3));
                Assert.Throws<Exception>(() => XEnv.Argument.Range(null));
            }
            #endregion

            #region Evaluate
            {
                XEnv.Argument.Setup("-test_arg=test_arg");
                Assert.That("${XEnv.Argument.test_arg}".Evaluate(XEnv.Argument.Evaluator), Is.EqualTo("test_arg"));

                XEnv.Argument.Setup("-test_arg1=${XEnv.Argument.test_arg2}", "-test_arg2=${XEnv.Argument.test_arg1}");
                Assert.That("${XEnv.Argument.test_arg1}".Evaluate(XEnv.Argument.Evaluator), Is.EqualTo("${Recursive.XEnv.Argument.test_arg1}"));
                Assert.That("${XEnv.Argument.test_arg1${XEnv.Argument.test_arg2}}".Evaluate(XEnv.Argument.Evaluator), Is.EqualTo("${Nested.${XEnv.Argument.test_arg1${XEnv.Argument.test_arg2}}}"));
            }
            #endregion
        }
        finally { XEnv.Argument.Setup(); }
    }
}
