// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

namespace FrameworkEase.Universal.X.Utility
{
    public partial class XLog
    {
        public enum Levels : sbyte
        {
            Unknown,
            Emergency,
            Alert,
            Critical,
            Error,
            Warn,
            Notice,
            Info,
            Debug,
        }

        public static readonly string[] Labels =
        {
            "[U]", // Unknown
            "[M]", // Emergency
            "[A]", // Alert
            "[C]", // Critical
            "[E]", // Error
            "[W]", // Warn
            "[N]", // Notice
            "[I]", // Info
            "[D]", // Debug
        };
    }
}
