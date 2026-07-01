// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.IO;
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXEnv
{
    [Test]
    public void Variable()
    {
        var testKeys = new[] { "TEST_KEY1", "TEST_KEY2", "TEST_KEY3", "TEST_KEY4", "TEST_EMPTY" };

        Environment.SetEnvironmentVariable("TEST_KEY1", "value1");
        Environment.SetEnvironmentVariable("TEST_KEY2", "value2");
        Environment.SetEnvironmentVariable("TEST_KEY3", "value3");
        Environment.SetEnvironmentVariable("TEST_KEY4", "value4");
        Environment.SetEnvironmentVariable("TEST_EMPTY", "");

        var envFile = Path.Join(XFile.Directory.Project, ".env");
        var envContent = "TEST_KEY1=fileValue1\n# This is a comment\nTEST_KEY2=fileValue2\n";
        File.WriteAllText(envFile, envContent);

        XEnv.Variable.Setup("TEST_KEY3=extraValue3", "TEST_KEY4=extraValue4");

        try
        {
            #region Get
            {
                Assert.That(XEnv.Variable.Get("TEST_KEY1"), Is.EqualTo("fileValue1"), "Get 环境变量 TEST_KEY1 的值应当和预期相符。");
                Assert.That(XEnv.Variable.Get("TEST_KEY2"), Is.EqualTo("fileValue2"), "Get 环境变量 TEST_KEY2 的值应当和预期相符。");
                Assert.That(XEnv.Variable.Get("TEST_KEY3"), Is.EqualTo("extraValue3"), "Get 环境变量 TEST_KEY3 的值应当和预期相符。");
                Assert.That(XEnv.Variable.Get("TEST_KEY4"), Is.EqualTo("extraValue4"), "Get 环境变量 TEST_KEY4 的值应当和预期相符。");
                Assert.That(string.IsNullOrEmpty(XEnv.Variable.Get("TEST_EMPTY")), Is.True, "Get 环境变量 TEST_EMPTY 的值应当返回空。");
                Assert.That(string.IsNullOrEmpty(XEnv.Variable.Get("TEST_NONEXISTENT")), Is.True, "Get 不存在的环境变量应当返回空。");
            }
            #endregion

            #region Exists
            {
                Assert.That(XEnv.Variable.Exists("TEST_KEY1"), Is.True, "Exists 环境变量 TEST_KEY1 应当存在。");
                Assert.That(XEnv.Variable.Exists("TEST_KEY2"), Is.True, "Exists 环境变量 TEST_KEY2 应当存在。");
                Assert.That(XEnv.Variable.Exists("TEST_KEY3"), Is.True, "Exists 环境变量 TEST_KEY3 应当存在。");
                Assert.That(XEnv.Variable.Exists("TEST_KEY4"), Is.True, "Exists 环境变量 TEST_KEY4 应当存在。");
                Assert.That(XEnv.Variable.Exists("TEST_EMPTY"), Is.False, "Exists 环境变量 TEST_EMPTY 应当返回 false。");
                Assert.That(XEnv.Variable.Exists("TEST_NONEXISTENT"), Is.False, "Exists 不存在的环境变量应当返回 false。");
            }
            #endregion

            #region Range
            {
                XEnv.Variable.Range((key, value) =>
                {
                    switch (key)
                    {
                        case "TEST_KEY1":
                            Assert.That(value, Is.EqualTo("fileValue1"), "Range 环境变量 TEST_KEY1 的值应当和预期相符。");
                            break;
                        case "TEST_KEY2":
                            Assert.That(value, Is.EqualTo("fileValue2"), "Range 环境变量 TEST_KEY2 的值应当和预期相符。");
                            break;
                        case "TEST_KEY3":
                            Assert.That(value, Is.EqualTo("extraValue3"), "Range 环境变量 TEST_KEY3 的值应当和预期相符。");
                            break;
                        case "TEST_KEY4":
                            Assert.That(value, Is.EqualTo("extraValue4"), "Range 环境变量 TEST_KEY4 的值应当和预期相符。");
                            break;
                    }
                    return true;
                });

                var count = 0;
                XEnv.Variable.Range((key, value) => ++count < 3);
                Assert.That(count, Is.EqualTo(3), "Range 应当在 callback 返回 false 时停止遍历。");
                Assert.Throws<Exception>(() => XEnv.Variable.Range(null), "Range 应当在 callback 为 null 时抛出异常。");
            }
            #endregion

            #region Evaluate
            {
                Environment.SetEnvironmentVariable("TEST_VAR", "test_var");
                try
                {
                    Assert.That("${XEnv.Variable.TEST_VAR}".Evaluate(XEnv.Variable.Evaluator), Is.EqualTo("test_var"));
                    Assert.That("${TEST_VAR}".Evaluate(XEnv.Variable.Evaluator), Is.EqualTo("test_var"));
                    Assert.That("${TEST_UNKNOWN}".Evaluate(XEnv.Variable.Evaluator), Is.EqualTo("${Unknown.XEnv.Variable.TEST_UNKNOWN}"));
                }
                finally { Environment.SetEnvironmentVariable("TEST_VAR", null); }
            }
            #endregion
        }
        finally
        {
            foreach (var key in testKeys) Environment.SetEnvironmentVariable(key, null);
            if (File.Exists(envFile)) File.Delete(envFile);
        }
    }
}
