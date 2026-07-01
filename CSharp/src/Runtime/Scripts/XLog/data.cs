// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;

namespace FrameworkEase.Universal.X.Utility
{
    public partial class XLog
    {
        public struct Data
        {
            public Levels Level;
            public object Content;
            public object[] Args;
            public Tag Tag;
            public DateTime Time;

            public readonly string Stringify()
            {
                var fmt = Content is string str ? str : null;
                var ctt = fmt != null ? (Args != null && Args.Length > 0 ? string.Format(fmt, Args) : fmt) : (Content?.ToString() ?? "<nil>");
                var tag = Tag.Stringify();
                if (string.IsNullOrEmpty(tag)) return Time.ToString("[MM/dd HH:mm:ss.fff] ") + Labels[(int)Level] + " " + ctt;
                else return Time.ToString("[MM/dd HH:mm:ss.fff] ") + Labels[(int)Level] + " " + tag + " " + ctt;
            }
        }
    }
}
