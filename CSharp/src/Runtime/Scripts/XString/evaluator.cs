// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System.Collections.Generic;

namespace FrameworkEase.Universal.X.Utility
{
    public static partial class XString
    {
        public interface IEvaluator
        {
            string Evaluate(string input);
        }

        public static string Evaluate(this string input, params IEvaluator[] evaluators)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            foreach (var source in evaluators)
            {
                if (source != null) input = source.Evaluate(input);
            }
            return input;
        }

        public static string Evaluate(this string input, params IReadOnlyDictionary<string, string>[] variables)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            foreach (var source in variables)
            {
                if (source != null)
                {
                    foreach (var item in source)
                    {
                        input = input.Replace($"${{{item.Key}}}", item.Value);
                    }
                }
            }
            return input;
        }
    }
}
