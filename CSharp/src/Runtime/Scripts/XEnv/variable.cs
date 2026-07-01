// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FrameworkEase.Universal.X.Utility
{
    public partial class XEnv
    {
        public static class Variable
        {
            internal static volatile bool setup;

            internal static readonly VariableEvaluator evaluator = new();

            public static XString.IEvaluator Evaluator => evaluator;

            internal static void EnsureSetup() { if (!setup) Setup(); }

            public static void Setup(params string[] extras)
            {
                #region 解析文件
                try
                {
                    var envFile = Path.Join(XFile.Directory.Project, ".env");
                    if (XFile.Exists(envFile))
                    {
                        var lines = File.ReadAllLines(envFile);
                        foreach (var rawLine in lines)
                        {
                            var line = rawLine.Trim();
                            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#')) continue;

                            var idx = line.IndexOf('=');
                            if (idx < 0) continue;

                            var key = line[..idx].Trim();
                            var value = line[(idx + 1)..].Trim();
                            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value)) continue;

                            Environment.SetEnvironmentVariable(key, value);
                        }
                    }
                }
                catch (Exception e) { XLog.Error(e); }
                #endregion

                #region 解析参数
                if (extras is { Length: > 0 })
                {
                    foreach (var extra in extras)
                    {
                        if (string.IsNullOrEmpty(extra)) continue;

                        var idx = extra.IndexOf('=');
                        if (idx < 0) continue;

                        var key = extra[..idx].Trim();
                        var value = extra[(idx + 1)..].Trim();
                        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value)) continue;

                        Environment.SetEnvironmentVariable(key, value);
                    }
                }
                #endregion

                setup = true;
            }

            public static string Get(string key)
            {
                EnsureSetup();
                return Environment.GetEnvironmentVariable(key);
            }

            public static bool Exists(string key)
            {
                EnsureSetup();
                return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key));
            }

            public static void Range(Func<string, string, bool> callback)
            {
                EnsureSetup();
                if (callback == null) throw new Exception("XEnv.Variable.Range: callback is null.");
                foreach (System.Collections.DictionaryEntry entry in Environment.GetEnvironmentVariables())
                {
                    var key = entry.Key.ToString();
                    var value = entry.Value?.ToString() ?? string.Empty;
                    if (!callback(key, value)) break;
                }
            }
        }

        internal sealed class VariableEvaluator : XString.IEvaluator
        {
            internal static readonly Regex Pattern = new(@"\$\{(?:XEnv\.Variable\.([^}]+?)|([A-Za-z_][A-Za-z0-9_]*))\}", RegexOptions.Compiled);

            public string Evaluate(string expr)
            {
                if (string.IsNullOrEmpty(expr)) return expr;
                var visited = new HashSet<string>();

                string repl(Match m)
                {
                    var g1 = m.Groups[1].Value;
                    var g2 = m.Groups[2].Value;

                    string key;
                    string value = null;

                    if (!string.IsNullOrEmpty(g2))
                    {
                        key = "Variable." + g2;
                        value = Variable.Get(g2);
                    }
                    else
                    {
                        key = "Variable." + g1;
                        if (g1.Contains("${")) return $"${{Nested.{m.Value}}}";
                        value = Variable.Get(g1);
                    }

                    if (!visited.Add(key)) return $"${{Recursive.XEnv.{key}}}";
                    try
                    {
                        if (!string.IsNullOrEmpty(value)) return Pattern.Replace(value, repl);
                        if (!string.IsNullOrEmpty(g2)) return $"${{Unknown.XEnv.Variable.{g2}}}";
                        return $"${{Unknown.XEnv.Variable.{g1}}}";
                    }
                    finally { visited.Remove(key); }
                }

                return Pattern.Replace(expr, repl);
            }
        }
    }
}
