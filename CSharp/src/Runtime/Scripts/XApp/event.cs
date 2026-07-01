// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

namespace FrameworkEase.Universal.X.Utility
{
    public static partial class XApp
    {
        public enum Events : short
        {
            OnAwake,
            OnStart,
            OnStop,
        }

        public static readonly XEvent.Manager Event = new();
    }
}
