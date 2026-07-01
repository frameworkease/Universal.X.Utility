// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XApp

import (
	"sync"
	"testing"
	"time"

	"github.com/frameworkease/Universal.X.Utility/Go/XLog"
	"github.com/stretchr/testify/assert"
)

type MyApplication struct {
	started bool
	stopped bool
	mu      sync.Mutex
}

func (application *MyApplication) Awake() bool {
	return true
}

func (application *MyApplication) Start() {
	application.mu.Lock()
	defer application.mu.Unlock()
	application.started = true
	XLog.Notice("MyApplication started.")
}

func (application *MyApplication) Stop(wg *sync.WaitGroup) {
	application.mu.Lock()
	defer application.mu.Unlock()
	wg.Add(1)       // 先增加计数
	defer wg.Done() // 确保在函数返回时减少计数
	application.stopped = true
	XLog.Notice("MyApplication stopped.")
}

func TestLifecycle(t *testing.T) {
	var onAwakeEventInvoked bool
	var onStartEventInvoked bool
	var onStopEventInvoked bool
	Event.Register(int(EventOnAwake), func(args ...any) {
		onAwakeEventInvoked = true
	})
	Event.Register(int(EventOnStart), func(args ...any) {
		onStartEventInvoked = true
	})
	Event.Register(int(EventOnStop), func(args ...any) {
		onStopEventInvoked = true
		if len(args) > 0 {
			if wg, ok := args[0].(*sync.WaitGroup); ok {
				wg.Add(1)
				go func() {
					time.Sleep(100 * time.Millisecond)
					wg.Done()
				}()
			}
		}
	})

	application := &MyApplication{}
	done := make(chan struct{})

	go func() {
		Run(application)
		close(done)
	}()

	// 等待应用程序启动
	time.Sleep(500 * time.Millisecond)

	application.mu.Lock()
	assert.True(t, application.started, "应用程序应当启动。")
	application.mu.Unlock()
	assert.True(t, onAwakeEventInvoked, "应用程序初始化事件应当被调用。")
	assert.True(t, onStartEventInvoked, "应用程序启动事件应当被调用。")

	// 触发退出
	Quit()

	// 等待应用程序完全停止
	select {
	case <-done:
		// 应用程序已经停止
	case <-time.After(time.Second):
		t.Fatal("等待应用程序停止超时。")
	}

	application.mu.Lock()
	assert.True(t, application.stopped, "应用程序应当停止。")
	application.mu.Unlock()
	assert.True(t, onStopEventInvoked, "应用程序退出事件应当被调用。")
}
