// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System.Threading;
using System.Threading.Tasks;
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public class TestXApp
{
    internal class MyApplication : XApp.IBase
    {
        internal bool awaked;
        internal bool started;
        internal bool stopped;

        public bool Awake()
        {
            awaked = true;
            return true;
        }

        public void Start() { started = true; }

        public void Stop(CountdownEvent counter)
        {
            stopped = true; // 设置标志
            counter.AddCount(); // 增加计数
            Task.Run(async () => // 异步停止
            {
                await Task.Delay(100);
                counter.Signal(); // 完成计数
            });
        }
    }

    [Test]
    public async Task Lifecycle()
    {
        var onAwakeEventInvoked = false;
        var onStartEventInvoked = false;
        var onStopEventInvoked = false;
        XApp.Event.Register(XApp.Events.OnAwake, () => { onAwakeEventInvoked = true; });
        XApp.Event.Register(XApp.Events.OnStart, () => { onStartEventInvoked = true; });
        XApp.Event.Register<CountdownEvent>(XApp.Events.OnStop, (counter) =>
        {
            onStopEventInvoked = true; // 设置标志
            counter.AddCount(); // 增加计数
            Task.Run(async () => // 异步停止
            {
                await Task.Delay(100);
                counter.Signal(); // 完成计数
            });
        });

        var myApplication = new MyApplication();
        _ = XApp.Run(myApplication); // 启动应用
        _ = XApp.Run(myApplication); // 重复启动
        await Task.Delay(500); // 等待启动

        Assert.That(XApp.Instance<MyApplication>(), Is.EqualTo(myApplication), "应用程序实例应当相等。");
        Assert.That(myApplication.awaked, Is.True, "应用程序应当初始化。");
        Assert.That(onAwakeEventInvoked, Is.True, "应用程序初始化事件应当被调用。");
        Assert.That(myApplication.started, Is.True, "应用程序应当已启动。");
        Assert.That(onStartEventInvoked, Is.True, "应用程序启动事件应当被调用。");

        XApp.Quit(); // 退出应用
        XApp.Quit(); // 重复退出
        await Task.Delay(500); // 等待退出
        Assert.That(myApplication.stopped, Is.True, "应用程序应当已退出。");
        Assert.That(onStopEventInvoked, Is.True, "应用程序退出事件应当被调用。");
    }
}
