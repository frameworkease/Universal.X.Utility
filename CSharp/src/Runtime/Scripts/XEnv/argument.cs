// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace FrameworkEase.Universal.X.Utility
{
    public partial class XEnv
    {
        public static class Argument
        {
            internal static readonly List<KeyValuePair<string, string>> cache = new();

            internal static readonly Mutex mutex = new();

            internal static volatile bool setup;

            internal static readonly ArgumentEvaluator evaluator = new();

            public static XString.IEvaluator Evaluator => evaluator;

            internal static void EnsureSetup() { if (!setup) Setup(); }

            public static void Setup(params string[] extras)
            {
                lock (mutex)
                {
                    cache.Clear();

                    var raws = new List<string>();
                    if (extras is { Length: > 0 }) raws.AddRange(extras);
                    var cargs = Environment.GetCommandLineArgs();
                    if (cargs is { Length: > 1 }) raws.AddRange(cargs[1..]);

                    for (var i = 0; i < raws.Count; i++)
                    {
                        var arg = raws[i];
                        if (string.IsNullOrEmpty(arg) || !arg.StartsWith('-')) continue;

                        var key = arg.StartsWith("--") ? arg[2..] : arg[1..];
                        var idx = key.IndexOf('=');
                        if (idx != -1)
                        {
                            var value = key[(idx + 1)..];
                            key = key[..idx];
                            cache.Add(new KeyValuePair<string, string>(key, value));
                            continue;
                        }

                        if (i + 1 >= raws.Count || raws[i + 1].StartsWith('-'))
                        {
                            cache.Add(new KeyValuePair<string, string>(key, string.Empty));
                            continue;
                        }

                        cache.Add(new KeyValuePair<string, string>(key, raws[i + 1]));
                        i++;
                    }

                    setup = true;
                }
            }

            public static string Get(string key)
            {
                EnsureSetup();
                lock (mutex)
                {
                    foreach (var pair in cache)
                    {
                        if (pair.Key == key) return pair.Value;
                    }
                    return string.Empty;
                }
            }

            public static bool Exists(string key)
            {
                EnsureSetup();
                lock (mutex)
                {
                    foreach (var pair in cache)
                    {
                        if (pair.Key == key) return true;
                    }
                    return false;
                }
            }

            public static void Range(Func<string, string, bool> callback)
            {
                EnsureSetup();
                if (callback == null) throw new Exception("XEnv.Argument.Range: callback is null.");
                lock (mutex)
                {
                    foreach (var pair in cache)
                    {
                        if (!callback(pair.Key, pair.Value)) break;
                    }
                }
            }
        }

        internal sealed class ArgumentEvaluator : XString.IEvaluator
        {
            internal static readonly Regex Pattern = new(@"\$\{XEnv\.Argument\.([^}]+?)\}", RegexOptions.Compiled);

            public string Evaluate(string expr)
            {
                if (string.IsNullOrEmpty(expr)) return expr;
                var visited = new HashSet<string>();

                string repl(Match m)
                {
                    var keyName = m.Groups[1].Value;
                    var key = "Argument." + keyName;

                    if (keyName.Contains("${")) return $"${{Nested.{m.Value}}}";

                    if (!visited.Add(key)) return $"${{Recursive.XEnv.{key}}}";
                    try
                    {
                        var value = Argument.Get(keyName);
                        if (!string.IsNullOrEmpty(value)) return Pattern.Replace(value, repl);
                        return $"${{Unknown.XEnv.Argument.{keyName}}}";
                    }
                    finally { visited.Remove(key); }
                }

                return Pattern.Replace(expr, repl);
            }
        }
    }
}
