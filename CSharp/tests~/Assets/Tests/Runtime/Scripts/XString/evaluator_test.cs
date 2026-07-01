// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System.Collections.Generic;
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXString
{
    /// <summary>
    /// MyEvaluator 是用于测试的求值器实现。
    /// </summary>
    private class MyEvaluator : XString.IEvaluator
    {
        public Dictionary<string, string> Vars;

        public string Evaluate(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            if (Vars != null) input = input.Evaluate(Vars);
            return input;
        }
    }

    [Test]
    public void Evaluate()
    {
        #region Evaluator
        {
            var evaluator1 = new MyEvaluator { Vars = new Dictionary<string, string> { { "name", "World" } } };
            var evaluator2 = new MyEvaluator { Vars = new Dictionary<string, string> { { "greeting", "Hello" } } };
            Assert.That("Hello ${name}".Evaluate(evaluator1), Is.EqualTo("Hello World"));
            Assert.That("${name} ${name}".Evaluate(evaluator1), Is.EqualTo("World World"));
            Assert.That("${greeting} ${name} and ${other}".Evaluate(evaluator1, evaluator2), Is.EqualTo("Hello World and ${other}"));
            Assert.That("".Evaluate(evaluator1), Is.EqualTo(string.Empty));
            Assert.That(((string)null).Evaluate(evaluator1), Is.EqualTo(string.Empty));
            Assert.That("Hello ${name}".Evaluate((XString.IEvaluator)null), Is.EqualTo("Hello ${name}"));
            Assert.That("Hello ${name}".Evaluate(new MyEvaluator()), Is.EqualTo("Hello ${name}"));
        }
        #endregion

        #region Variable
        {
            var variable1 = new Dictionary<string, string> { { "name", "World" } };
            var variable2 = new Dictionary<string, string> { { "greeting", "Hello" } };
            Assert.That("Hello ${name}".Evaluate(variable1), Is.EqualTo("Hello World"));
            Assert.That("${name} ${name}".Evaluate(variable1), Is.EqualTo("World World"));
            Assert.That("${greeting} ${name} and ${other}".Evaluate(variable1, variable2), Is.EqualTo("Hello World and ${other}"));
            Assert.That("".Evaluate(variable1), Is.EqualTo(string.Empty));
            Assert.That(((string)null).Evaluate(variable1), Is.EqualTo(string.Empty));
            Assert.That("Hello ${name}".Evaluate((IReadOnlyDictionary<string, string>)null), Is.EqualTo("Hello ${name}"));
            Assert.That("Hello ${name}".Evaluate(new Dictionary<string, string>()), Is.EqualTo("Hello ${name}"));
        }
        #endregion
    }
}
