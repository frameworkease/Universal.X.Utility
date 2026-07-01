// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXEvent
{
    [Test]
    public void Register()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        var multipleManager = new XEvent.Manager { Singleton = false };
        var singletonManager = new XEvent.Manager { Singleton = true };
        XEvent.Callback callback = (args) => { };
        Action callback0 = () => { };
        Action<int> callback1 = (arg1) => { };
        Action<int, string> callback2 = (arg1, arg2) => { };
        Action<int, string, double> callback3 = (arg1, arg2, arg3) => { };

        // 注册事件
        {
            Assert.That(multipleManager.Register(10086, callback, true), Is.True, "多重监听管理器的事件 10086 的回调函数 callback 应当注册成功。");
            Assert.That(multipleManager.Register(10086, callback, true), Is.False, "多重监听管理器的事件 10086 的回调函数 callback 不应重复注册。");
            Assert.That(multipleManager.Handlers[10086][0].Once, Is.True, "多重监听管理器的事件 10086 的回调函数 callback 应当为单次回调。");
            Assert.That(object.ReferenceEquals(multipleManager.Handlers[10086][0].Origin, callback), Is.True, "多重监听管理器的事件 10086 的回调函数应当为 callback。");
            Assert.That(singletonManager.Register(10086, callback), Is.True, "单一监听管理器的事件 10086 的回调函数 callback 应当注册成功。");
            Assert.That(singletonManager.Register(0, (XEvent.Callback)null), Is.False, "注册空的回调函数应返回失败。");

            Assert.That(multipleManager.Register(10086, (Action)callback0, true), Is.True, "多重监听管理器的事件 10086 的回调函数 callback0 应当注册成功。");
            Assert.That(multipleManager.Register(10086, (Action)callback0, true), Is.False, "多重监听管理器的事件 10086 的回调函数 callback0 不应重复注册。");
            Assert.That(multipleManager.Handlers[10086][1].Once, Is.True, "多重监听管理器的事件 10086 的回调函数 callback0 应当为单次回调。");
            Assert.That(object.ReferenceEquals(multipleManager.Handlers[10086][1].Origin, callback0), Is.True, "多重监听管理器的事件 10086 的回调函数应当为 callback0。");
            Assert.That(singletonManager.Register(10086, (Action)callback0), Is.False, "单一监听管理器的事件 10086 的回调函数 callback0 不支持多重监听模式。");
            Assert.That(singletonManager.Register(0, (Action)null), Is.False, "注册空的回调函数应返回失败。");

            Assert.That(multipleManager.Register(10010, callback1, true), Is.True, "多重监听管理器的事件 10010 的回调函数 callback1 应当注册成功。");
            Assert.That(multipleManager.Register(10010, callback1, true), Is.False, "多重监听管理器的事件 10010 的回调函数 callback1 不应重复注册。");
            Assert.That(multipleManager.Handlers[10010][0].Once, Is.True, "多重监听管理器的事件 10010 的回调函数 callback1 应当为单次回调。");
            Assert.That(object.ReferenceEquals(multipleManager.Handlers[10010][0].Origin, callback1), Is.True, "多重监听管理器的事件 10010 的回调函数应当为 callback1。");
            Assert.That(singletonManager.Register(10086, callback1), Is.False, "单一监听管理器的事件 10086 的回调函数 callback1 不支持多重监听模式。");
            Assert.That(singletonManager.Register<int>(0, null), Is.False, "注册空的回调函数应返回失败。");

            Assert.That(multipleManager.Register(10010, callback2, true), Is.True, "多重监听管理器的事件 10010 的回调函数 callback2 应当注册成功。");
            Assert.That(multipleManager.Register(10010, callback2, true), Is.False, "多重监听管理器的事件 10010 的回调函数 callback2 不应重复注册。");
            Assert.That(multipleManager.Handlers[10010][1].Once, Is.True, "多重监听管理器的事件 10010 的回调函数 callback2 应当为单次回调。");
            Assert.That(object.ReferenceEquals(multipleManager.Handlers[10010][1].Origin, callback2), Is.True, "多重监听管理器的事件 10010 的回调函数应当为 callback2。");
            Assert.That(singletonManager.Register(10086, callback2), Is.False, "单一监听管理器的事件 10086 的回调函数 callback2 不支持多重监听模式。");
            Assert.That(singletonManager.Register<int, string>(0, null), Is.False, "注册空的回调函数应返回失败。");

            Assert.That(multipleManager.Register(10000, callback3, true), Is.True, "多重监听管理器的事件 10000 的回调函数 callback3 应当注册成功。");
            Assert.That(multipleManager.Register(10000, callback3, true), Is.False, "多重监听管理器的事件 10000 的回调函数 callback3 不应重复注册。");
            Assert.That(multipleManager.Handlers[10000][0].Once, Is.True, "多重监听管理器的事件 10000 的回调函数 callback3 应当为单次回调。");
            Assert.That(object.ReferenceEquals(multipleManager.Handlers[10000][0].Origin, callback3), Is.True, "多重监听管理器的事件 10000 的回调函数应当为 callback3。");
            Assert.That(singletonManager.Register(10086, callback3), Is.False, "单一监听管理器的事件 10086 的回调函数 callback3 不支持多重监听模式。");
            Assert.That(singletonManager.Register<int, string, double>(0, null), Is.False, "注册空的回调函数应返回失败。");
        }

        // 注销事件
        {
            Assert.That(singletonManager.Unregister(10086), Is.True, "单一监听管理器的事件 10086 的所有回调函数应当注销成功。");

            Assert.That(multipleManager.Unregister(10086, callback), Is.True, "多重监听管理器的事件 10086 的回调函数 callback 应当注销成功。");
            Assert.That(multipleManager.Handlers[10086].Count, Is.EqualTo(1), "注销多重监听管理器的事件 10086 的回调函数 callback 后，事件句柄列表应当不为空。");
            Assert.That(multipleManager.Unregister(10086, callback), Is.False, "多重监听管理器的事件 10086 的回调函数 callback 应当注销失败。");

            Assert.That(multipleManager.Unregister(10086, callback0), Is.True, "多重监听管理器的事件 10086 的回调函数 callback0 应当注销成功。");
            Assert.That(multipleManager.Handlers.ContainsKey(10086), Is.False, "注销多重监听管理器的事件 10086 的回调函数 callback0 后，事件句柄列表应当为空。");
            Assert.That(multipleManager.Unregister(10086, callback0), Is.False, "多重监听管理器的事件 10086 的回调函数 callback0 应当注销失败。");

            Assert.That(multipleManager.Unregister(10010, callback1), Is.True, "多重监听管理器的事件 10010 的回调函数 callback1 应当注销成功。");
            Assert.That(multipleManager.Handlers[10010].Count, Is.EqualTo(1), "注销多重监听管理器的事件 10010 的回调函数 callback1 后，事件句柄列表应当不为空。");
            Assert.That(multipleManager.Unregister(10010, callback1), Is.False, "多重监听管理器的事件 10010 的回调函数 callback1 应当注销失败。");

            Assert.That(multipleManager.Unregister(10010, callback2), Is.True, "多重监听管理器的事件 10010 的回调函数 callback2 应当注销成功。");
            Assert.That(multipleManager.Handlers.ContainsKey(10010), Is.False, "注销多重监听管理器的事件 10010 的回调函数 callback2 后，事件句柄列表应当为空。");
            Assert.That(multipleManager.Unregister(10010, callback2), Is.False, "多重监听管理器的事件 10010 的回调函数 callback2 应当注销失败。");

            Assert.That(multipleManager.Unregister(10000, callback3), Is.True, "多重监听管理器的事件 10000 的回调函数 callback3 应当注销成功。");
            Assert.That(multipleManager.Handlers.ContainsKey(10000), Is.False, "注销多重监听管理器的事件 10000 的回调函数 callback3 后，事件句柄列表应当为空。");
            Assert.That(multipleManager.Unregister(10000, callback3), Is.False, "多重监听管理器的事件 10000 的回调函数 callback3 应当注销失败。");

            Assert.That(multipleManager.Unregister(99999, callback0), Is.False, "注销不存在的事件标识应返回失败。");
            Assert.That(multipleManager.Unregister(10010, callback1), Is.False, "注销已注销的回调函数应返回失败。");
            Assert.That(multipleManager.Unregister(10010, callback2), Is.False, "注销已注销的回调函数应返回失败。");
            Assert.That(multipleManager.Unregister(10000, callback3), Is.False, "注销已注销的回调函数应返回失败。");

            Action ncallback0 = () => { };
            Action<int> ncallback1 = (arg1) => { };
            Action<int, string> ncallback2 = (arg1, arg2) => { };
            Action<int, string, double> ncallback3 = (arg1, arg2, arg3) => { };
            Assert.That(multipleManager.Unregister(10086, ncallback0), Is.False, "注销不存在的回调函数应返回失败。");
            Assert.That(multipleManager.Unregister(10010, ncallback1), Is.False, "注销不存在的回调函数应返回失败。");
            Assert.That(multipleManager.Unregister(10010, ncallback2), Is.False, "注销不存在的回调函数应返回失败。");
            Assert.That(multipleManager.Unregister(10000, ncallback3), Is.False, "注销不存在的回调函数应返回失败。");

            Assert.That(multipleManager.Unregister(0, (Action)null), Is.False, "注销空的回调函数应返回失败。");
            Assert.That(multipleManager.Unregister<int>(0, null), Is.False, "注销空的回调函数应返回失败。");
            Assert.That(multipleManager.Unregister<int, string>(0, null), Is.False, "注销空的回调函数应返回失败。");
            Assert.That(multipleManager.Unregister<int, string, double>(0, null), Is.False, "注销空的回调函数应返回失败。");

            Assert.That(multipleManager.Handlers.Count, Is.EqualTo(0), "多重监听管理器的事件句柄列表应当为空。");
            Assert.That(singletonManager.Handlers.Count, Is.EqualTo(0), "单一监听管理器的事件句柄列表应当为空。");
        }
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void Clear()
    {
        var manager = new XEvent.Manager();
        manager.Register(1, (args) => { });
        manager.Clear();
        Assert.That(manager.Handlers.Count, Is.EqualTo(0), "清除事件后，事件句柄列表应当为空。");
    }

    [Test]
    public void Notify()
    {
        var manager = new XEvent.Manager();

        // 基本通知
        {
            object[] vars = null;
            bool var0 = false;
            int val1 = 0;
            string val2 = null;
            double val3 = 0;
            manager.Register(10001, (args) => { vars = args; });
            manager.Register(10010, () => { var0 = true; });
            manager.Register<int>(10011, (v) => { val1 = v; });
            manager.Register<int, string>(10012, (v1, v2) => { val1 = v1; val2 = v2; });
            manager.Register<int, string, double>(10013, (v1, v2, v3) => { val1 = v1; val2 = v2; val3 = v3; });

            Assert.That(manager.Notify(10001, 42), Is.True, "事件 10001 应当通知成功。");
            Assert.That(vars[0], Is.EqualTo(42), "事件 10001 的回调函数应当接收到参数 42。");
            Assert.That(manager.Notify(10010), Is.True, "事件 10010 应当通知成功。");
            Assert.That(var0, Is.True, "事件 10010 的 RegisterT0 注册的回调函数应当被执行。");
            Assert.That(manager.Notify(10011, 123), Is.True, "事件 10011 应当通知成功。");
            Assert.That(val1, Is.EqualTo(123), "事件 10011 的 RegisterT1 注册的回调函数应当接收到参数 123。");
            Assert.That(manager.Notify(10012, 456, "world"), Is.True, "事件 10012 应当通知成功。");
            Assert.That(val1, Is.EqualTo(456), "事件 10012 的 RegisterT2 注册的回调函数应当接收到第一个参数 456。");
            Assert.That(val2, Is.EqualTo("world"), "事件 10012 的 RegisterT2 注册的回调函数应当接收到第二个参数 world。");
            Assert.That(manager.Notify(10013, 789, "test", 2.71), Is.True, "事件 10013 应当通知成功。");
            Assert.That(val1, Is.EqualTo(789), "事件 10013 的 RegisterT3 注册的回调函数应当接收到第一个参数 789。");
            Assert.That(val2, Is.EqualTo("test"), "事件 10013 的 RegisterT3 注册的回调函数应当接收到第二个参数 test。");
            Assert.That(val3, Is.EqualTo(2.71), "事件 10013 的 RegisterT3 注册的回调函数应当接收到第三个参数 2.71。");
            Assert.That(manager.Notify(99999), Is.False, "通知不存在的事件标识应返回失败。");
        }

        // 多重监听
        {
            int count = 0, count0 = 0, count1 = 0, count2 = 0, count3 = 0;
            manager.Register(10020, (args) => { count++; });
            manager.Register(10020, () => { count0++; });
            manager.Register<int>(10020, (v) => { count1++; });
            manager.Register<int, string>(10020, (v1, v2) => { count2++; });
            manager.Register<int, string, double>(10020, (v1, v2, v3) => { count3++; });

            Assert.That(manager.Notify(10020), Is.True, "事件 10020 应当通知成功。");
            Assert.That(manager.Notify(10020), Is.True, "事件 10020 应当通知成功。");
            Assert.That(count, Is.EqualTo(2), "Register 注册的事件 10020 的回调函数应当被执行两次。");
            Assert.That(count0, Is.EqualTo(2), "RegisterT0 注册的事件 10020 的回调函数应当被执行两次。");
            Assert.That(count1, Is.EqualTo(2), "RegisterT1 注册的事件 10020 的回调函数应当被执行两次。");
            Assert.That(count2, Is.EqualTo(2), "RegisterT2 注册的事件 10020 的回调函数应当被执行两次。");
            Assert.That(count3, Is.EqualTo(2), "RegisterT3 注册的事件 10020 的回调函数应当被执行两次。");
        }

        // 单次回调
        {
            int count = 0, count0 = 0, count1 = 0, count2 = 0, count3 = 0;
            manager.Register(10030, (args) => { count++; }, true);
            manager.Register(10031, () => { count0++; }, true);
            manager.Register<int>(10032, (v) => { count1++; }, true);
            manager.Register<int, string>(10033, (v1, v2) => { count2++; }, true);
            manager.Register<int, string, double>(10034, (v1, v2, v3) => { count3++; }, true);

            Assert.That(manager.Notify(10030), Is.True, "事件 10030 的首次通知应返回成功。");
            Assert.That(count, Is.EqualTo(1), "Register 注册的事件 10030 的回调函数应当被执行一次。");
            Assert.That(manager.Notify(10030), Is.False, "事件 10030 的单次回调执行后，再次通知应返回失败。");
            Assert.That(count, Is.EqualTo(1), "Register 注册的事件 10030 的回调函数不应再次执行。");

            Assert.That(manager.Notify(10031), Is.True, "事件 10031 的首次通知应返回成功。");
            Assert.That(count0, Is.EqualTo(1), "RegisterT0 注册的事件 10031 的回调函数应当被执行一次。");
            Assert.That(manager.Notify(10031), Is.False, "事件 10031 的单次回调执行后，再次通知应返回失败。");
            Assert.That(count0, Is.EqualTo(1), "RegisterT0 注册的事件 10031 的回调函数不应再次执行。");

            Assert.That(manager.Notify(10032), Is.True, "事件 10032 的首次通知应返回成功。");
            Assert.That(count1, Is.EqualTo(1), "RegisterT1 注册的事件 10032 的回调函数应当被执行一次。");
            Assert.That(manager.Notify(10032), Is.False, "事件 10032 的单次回调执行后，再次通知应返回失败。");
            Assert.That(count1, Is.EqualTo(1), "RegisterT1 注册的事件 10032 的回调函数不应再次执行。");

            Assert.That(manager.Notify(10033), Is.True, "事件 10033 的首次通知应返回成功。");
            Assert.That(count2, Is.EqualTo(1), "RegisterT2 注册的事件 10033 的回调函数应当被执行一次。");
            Assert.That(manager.Notify(10033), Is.False, "事件 10033 的单次回调执行后，再次通知应返回失败。");
            Assert.That(count2, Is.EqualTo(1), "RegisterT2 注册的事件 10033 的回调函数不应再次执行。");

            Assert.That(manager.Notify(10034), Is.True, "事件 10034 的首次通知应返回成功。");
            Assert.That(count3, Is.EqualTo(1), "RegisterT3 注册的事件 10034 的回调函数应当被执行一次。");
            Assert.That(manager.Notify(10034), Is.False, "事件 10034 的单次回调执行后，再次通知应返回失败。");
            Assert.That(count3, Is.EqualTo(1), "RegisterT3 注册的事件 10034 的回调函数不应再次执行。");
        }
    }
}
