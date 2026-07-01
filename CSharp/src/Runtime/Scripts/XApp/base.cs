// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System.Threading;

namespace FrameworkEase.Universal.X.Utility
{
    public static partial class XApp
    {
        public interface IBase
        {
            bool Awake();
            void Start();
            void Stop(CountdownEvent counter);
        }
    }
}
