// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;

namespace FrameworkEase.Universal.X.Utility
{
    public static partial class XTime
    {
        public static string Stringify(int seconds, string format = "yyyy-MM-dd HH:mm:ss") { return Initial.AddSeconds(seconds).ToString(format); }
        public static string Stringify(long milliseconds, string format = "yyyy-MM-dd HH:mm:ss.fff") { return Initial.AddMilliseconds(milliseconds).ToString(format); }
        public static string Stringify(DateTime time, string format = "yyyy-MM-dd HH:mm:ss.fff") { return time.ToString(format); }
    }
}
