// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

#if UNITY_5_3_OR_NEWER && !FRAMEWORKEASE_UNITY_XLOG_REDIRECT_DISABLED
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXLog
{
    [Test]
    public void UnityOnLoad()
    {
        Assert.That(XLog.UnityHandler.origin, Is.Not.Null);
        Assert.That(UnityEngine.Debug.unityLogger.logHandler, Is.InstanceOf<XLog.UnityHandler>());
    }
}
#endif
