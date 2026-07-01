// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.Text;

namespace FrameworkEase.Universal.X.Utility
{
    public partial class XLog
    {
        public struct Tag
        {
            internal List<string> pairs;

            public readonly int Count => pairs != null ? pairs.Count / 2 : 0;

            public void Set(string key, string value)
            {
                pairs ??= new List<string>();
                for (var i = 0; i < pairs.Count; i += 2)
                {
                    if (pairs[i] != key) continue;
                    pairs[i + 1] = value;
                    return;
                }
                pairs.Add(key);
                pairs.Add(value);
            }

            public readonly string Get(string key)
            {
                if (pairs == null) return string.Empty;
                for (var i = 0; i < pairs.Count; i += 2)
                {
                    if (pairs[i] == key) return pairs[i + 1];
                }
                return string.Empty;
            }

            public readonly void Range(Func<string, string, bool> callback)
            {
                if (callback == null || pairs == null) return;
                for (var i = 0; i < pairs.Count; i += 2)
                {
                    if (!callback(pairs[i], pairs[i + 1])) break;
                }
            }

            public readonly string Stringify()
            {
                if (pairs == null || pairs.Count == 0) return string.Empty;
                var builder = new StringBuilder();
                builder.Append("[");
                var first = true;
                for (var i = 0; i < pairs.Count; i += 2)
                {
                    if (!first) builder.Append(", ");
                    else first = false;
                    builder.Append(pairs[i]);
                    builder.Append("=");
                    builder.Append(pairs[i + 1]);
                }
                builder.Append("]");
                return builder.ToString();
            }
        }
    }
}
