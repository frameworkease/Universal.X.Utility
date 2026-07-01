// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;

namespace FrameworkEase.Universal.X.Utility
{
    public static partial class XTime
    {
        public static readonly DateTime Initial = new DateTime(1970, 1, 1) + TimeZoneInfo.Local.BaseUtcOffset;
        public static DateTime Current => DateTime.Now;
        public static int Seconds => (int)((DateTime.Now.Ticks - Initial.Ticks) / 10_000_000);
        public static long Milliseconds => (DateTime.Now.Ticks - Initial.Ticks) / 10_000;
        public static long Microseconds => (DateTime.Now.Ticks - Initial.Ticks) / 10;
    }
}
