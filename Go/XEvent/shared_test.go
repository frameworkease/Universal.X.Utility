// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XEvent

import (
	"reflect"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestRegister(t *testing.T) {
	multipleManager := &Manager{Singleton: false}
	singletonManager := &Manager{Singleton: true}
	callback := func(args ...any) {}
	callback0 := func() {}
	callback1 := func(arg1 int) {}
	callback2 := func(arg1 int, arg2 string) {}
	callback3 := func(arg1 int, arg2 string, arg3 float64) {}

	// 注册事件
	{
		assert.True(t, multipleManager.Register(10086, callback, true), "多重监听管理器的事件 10086 的回调函数 callback 应当注册成功。")
		assert.False(t, multipleManager.Register(10086, callback, true), "多重监听管理器的事件 10086 的回调函数 callback 不应重复注册。")
		assert.True(t, multipleManager.Handlers[10086][0].Once, "多重监听管理器的事件 10086 的回调函数 callback 应当为单次回调。")
		assert.Equal(t, reflect.ValueOf(callback).Pointer(), multipleManager.Handlers[10086][0].Origin, "多重监听管理器的事件 10086 的回调函数应当为 callback。")
		assert.True(t, singletonManager.Register(10086, callback), "单一监听管理器的事件 10086 的回调函数 callback 应当注册成功。")
		assert.False(t, singletonManager.Register(0, nil), "注册空的回调函数应返回失败。")

		assert.True(t, RegisterT0(multipleManager, 10086, callback0, true), "多重监听管理器的事件 10086 的回调函数 callback0 应当注册成功。")
		assert.False(t, RegisterT0(multipleManager, 10086, callback0, true), "多重监听管理器的事件 10086 的回调函数 callback0 不应重复注册。")
		assert.True(t, multipleManager.Handlers[10086][1].Once, "多重监听管理器的事件 10086 的回调函数 callback0 应当为单次回调。")
		assert.Equal(t, reflect.ValueOf(callback0).Pointer(), multipleManager.Handlers[10086][1].Origin, "多重监听管理器的事件 10086 的回调函数应当为 callback0。")
		assert.False(t, RegisterT0(singletonManager, 10086, callback0), "单一监听管理器的事件 10086 的回调函数 callback0 不支持多重监听模式。")
		assert.False(t, RegisterT0(singletonManager, 0, nil), "注册空的回调函数应返回失败。")

		assert.True(t, RegisterT1(multipleManager, 10010, callback1, true), "多重监听管理器的事件 10010 的回调函数 callback1 应当注册成功。")
		assert.False(t, RegisterT1(multipleManager, 10010, callback1, true), "多重监听管理器的事件 10010 的回调函数 callback1 不应重复注册。")
		assert.True(t, multipleManager.Handlers[10010][0].Once, "多重监听管理器的事件 10010 的回调函数 callback1 应当为单次回调。")
		assert.Equal(t, reflect.ValueOf(callback1).Pointer(), multipleManager.Handlers[10010][0].Origin, "多重监听管理器的事件 10010 的回调函数应当为 callback1。")
		assert.False(t, RegisterT1(singletonManager, 10086, callback1), "单一监听管理器的事件 10086 的回调函数 callback1 不支持多重监听模式。")
		assert.False(t, RegisterT1[any](singletonManager, 0, nil), "注册空的回调函数应返回失败。")

		assert.True(t, RegisterT2(multipleManager, 10010, callback2, true), "多重监听管理器的事件 10010 的回调函数 callback2 应当注册成功。")
		assert.False(t, RegisterT2(multipleManager, 10010, callback2, true), "多重监听管理器的事件 10010 的回调函数 callback2 不应重复注册。")
		assert.True(t, multipleManager.Handlers[10010][1].Once, "多重监听管理器的事件 10010 的回调函数 callback2 应当为单次回调。")
		assert.Equal(t, reflect.ValueOf(callback2).Pointer(), multipleManager.Handlers[10010][1].Origin, "多重监听管理器的事件 10010 的回调函数应当为 callback2。")
		assert.False(t, RegisterT2(singletonManager, 10086, callback2), "单一监听管理器的事件 10086 的回调函数 callback2 不支持多重监听模式。")
		assert.False(t, RegisterT2[any, any](singletonManager, 0, nil), "注册空的回调函数应返回失败。")

		assert.True(t, RegisterT3(multipleManager, 10000, callback3, true), "多重监听管理器的事件 10000 的回调函数 callback3 应当注册成功。")
		assert.False(t, RegisterT3(multipleManager, 10000, callback3, true), "多重监听管理器的事件 10000 的回调函数 callback3 不应重复注册。")
		assert.True(t, multipleManager.Handlers[10000][0].Once, "多重监听管理器的事件 10000 的回调函数 callback3 应当为单次回调。")
		assert.Equal(t, reflect.ValueOf(callback3).Pointer(), multipleManager.Handlers[10000][0].Origin, "多重监听管理器的事件 10000 的回调函数应当为 callback3。")
		assert.False(t, RegisterT3(singletonManager, 10086, callback3), "单一监听管理器的事件 10086 的回调函数 callback3 不支持多重监听模式。")
		assert.False(t, RegisterT3[any, any, any](singletonManager, 0, nil), "注册空的回调函数应返回失败。")
	}

	// 注销事件
	{
		assert.True(t, singletonManager.Unregister(10086), "单一监听管理器的事件 10086 的所有回调函数应当注销成功。")

		assert.True(t, multipleManager.Unregister(10086, callback), "多重监听管理器的事件 10086 的回调函数 callback 应当注销成功。")
		assert.Equal(t, 1, len(multipleManager.Handlers[10086]), "注销多重监听管理器的事件 10086 的回调函数 callback 后，事件句柄列表应当不为空。")
		assert.False(t, multipleManager.Unregister(10086, callback), "多重监听管理器的事件 10086 的回调函数 callback 应当注销失败。")

		assert.True(t, UnregisterT0(multipleManager, 10086, callback0), "多重监听管理器的事件 10086 的回调函数 callback0 应当注销成功。")
		assert.Equal(t, 0, len(multipleManager.Handlers[10086]), "注销多重监听管理器的事件 10086 的回调函数 callback0 后，事件句柄列表应当为空。")
		assert.False(t, UnregisterT0(multipleManager, 10086, callback0), "多重监听管理器的事件 10086 的回调函数 callback0 应当注销失败。")

		assert.True(t, UnregisterT1(multipleManager, 10010, callback1), "多重监听管理器的事件 10010 的回调函数 callback1 应当注销成功。")
		assert.Equal(t, 1, len(multipleManager.Handlers[10010]), "注销多重监听管理器的事件 10010 的回调函数 callback1 后，事件句柄列表应当不为空。")
		assert.False(t, UnregisterT1(multipleManager, 10010, callback1), "多重监听管理器的事件 10010 的回调函数 callback1 应当注销失败。")

		assert.True(t, UnregisterT2(multipleManager, 10010, callback2), "多重监听管理器的事件 10010 的回调函数 callback2 应当注销成功。")
		assert.Equal(t, 0, len(multipleManager.Handlers[10010]), "注销多重监听管理器的事件 10010 的回调函数 callback2 后，事件句柄列表应当为空。")
		assert.False(t, UnregisterT2(multipleManager, 10010, callback2), "多重监听管理器的事件 10010 的回调函数 callback2 应当注销失败。")

		assert.True(t, UnregisterT3(multipleManager, 10000, callback3), "多重监听管理器的事件 10000 的回调函数 callback3 应当注销成功。")
		assert.Equal(t, 0, len(multipleManager.Handlers[10000]), "注销多重监听管理器的事件 10000 的回调函数 callback3 后，事件句柄列表应当为空。")
		assert.False(t, UnregisterT3(multipleManager, 10000, callback3), "多重监听管理器的事件 10000 的回调函数 callback3 应当注销失败。")

		assert.False(t, UnregisterT0(multipleManager, 99999, callback0), "注销不存在的事件标识应返回失败。")
		assert.False(t, UnregisterT1(multipleManager, 10010, callback1), "注销已注销的回调函数应返回失败。")
		assert.False(t, UnregisterT2(multipleManager, 10010, callback2), "注销已注销的回调函数应返回失败。")
		assert.False(t, UnregisterT3(multipleManager, 10000, callback3), "注销已注销的回调函数应返回失败。")

		ncallback0 := func() {}
		ncallback1 := func(arg1 int) {}
		ncallback2 := func(arg1 int, arg2 string) {}
		ncallback3 := func(arg1 int, arg2 string, arg3 float64) {}
		assert.False(t, UnregisterT0(multipleManager, 10086, ncallback0), "注销不存在的回调函数应返回失败。")
		assert.False(t, UnregisterT1(multipleManager, 10010, ncallback1), "注销不存在的回调函数应返回失败。")
		assert.False(t, UnregisterT2(multipleManager, 10010, ncallback2), "注销不存在的回调函数应返回失败。")
		assert.False(t, UnregisterT3(multipleManager, 10000, ncallback3), "注销不存在的回调函数应返回失败。")

		assert.False(t, UnregisterT0(multipleManager, 0, nil), "注销空的回调函数应返回失败。")
		assert.False(t, UnregisterT1[int](multipleManager, 0, nil), "注销空的回调函数应返回失败。")
		assert.False(t, UnregisterT2[int, string](multipleManager, 0, nil), "注销空的回调函数应返回失败。")
		assert.False(t, UnregisterT3[int, string, float64](multipleManager, 0, nil), "注销空的回调函数应返回失败。")

		assert.Len(t, multipleManager.Handlers, 0, "多重监听管理器的事件句柄列表应当为空。")
		assert.Len(t, singletonManager.Handlers, 0, "单一监听管理器的事件句柄列表应当为空。")
	}
}

func TestClear(t *testing.T) {
	manager := &Manager{}
	manager.Register(1, func(args ...any) {})
	manager.Clear()
	assert.Equal(t, 0, len(manager.Handlers), "清除事件后，事件句柄列表应当为空。")
}

func TestNotify(t *testing.T) {
	manager := &Manager{}

	// 基本通知
	{
		var vars []any
		var var0 bool
		var val1 int
		var val2 string
		var val3 float64
		manager.Register(10001, func(args ...any) { vars = args })
		RegisterT0(manager, 10010, func() { var0 = true })
		RegisterT1(manager, 10011, func(v int) { val1 = v })
		RegisterT2(manager, 10012, func(v1 int, v2 string) { val1 = v1; val2 = v2 })
		RegisterT3(manager, 10013, func(v1 int, v2 string, v3 float64) { val1 = v1; val2 = v2; val3 = v3 })

		assert.True(t, manager.Notify(10001, 42), "事件 10001 应当通知成功。")
		assert.Equal(t, 42, vars[0], "事件 10001 的回调函数应当接收到参数 42。")
		assert.True(t, manager.Notify(10010), "事件 10010 应当通知成功。")
		assert.True(t, var0, "事件 10010 的 RegisterT0 注册的回调函数应当被执行。")
		assert.True(t, manager.Notify(10011, 123), "事件 10011 应当通知成功。")
		assert.Equal(t, 123, val1, "事件 10011 的 RegisterT1 注册的回调函数应当接收到参数 123。")
		assert.True(t, manager.Notify(10012, 456, "world"), "事件 10012 应当通知成功。")
		assert.Equal(t, 456, val1, "事件 10012 的 RegisterT2 注册的回调函数应当接收到第一个参数 456。")
		assert.Equal(t, "world", val2, "事件 10012 的 RegisterT2 注册的回调函数应当接收到第二个参数 world。")
		assert.True(t, manager.Notify(10013, 789, "test", 2.71), "事件 10013 应当通知成功。")
		assert.Equal(t, 789, val1, "事件 10013 的 RegisterT3 注册的回调函数应当接收到第一个参数 789。")
		assert.Equal(t, "test", val2, "事件 10013 的 RegisterT3 注册的回调函数应当接收到第二个参数 test。")
		assert.Equal(t, 2.71, val3, "事件 10013 的 RegisterT3 注册的回调函数应当接收到第三个参数 2.71。")
		assert.False(t, manager.Notify(99999), "通知不存在的事件标识应返回失败。")
	}

	// 多重监听
	{
		count, count0, count1, count2, count3 := 0, 0, 0, 0, 0
		manager.Register(10020, func(args ...any) { count++ })
		RegisterT0(manager, 10020, func() { count0++ })
		RegisterT1(manager, 10020, func(v int) { count1++ })
		RegisterT2(manager, 10020, func(v1 int, v2 string) { count2++ })
		RegisterT3(manager, 10020, func(v1 int, v2 string, v3 float64) { count3++ })

		assert.True(t, manager.Notify(10020), "事件 10020 应当通知成功。")
		assert.True(t, manager.Notify(10020), "事件 10020 应当通知成功。")
		assert.Equal(t, 2, count, "Register 注册的事件 10020 的回调函数应当被执行两次。")
		assert.Equal(t, 2, count0, "RegisterT0 注册的事件 10020 的回调函数应当被执行两次。")
		assert.Equal(t, 2, count1, "RegisterT1 注册的事件 10020 的回调函数应当被执行两次。")
		assert.Equal(t, 2, count2, "RegisterT2 注册的事件 10020 的回调函数应当被执行两次。")
		assert.Equal(t, 2, count3, "RegisterT3 注册的事件 10020 的回调函数应当被执行两次。")
	}

	// 单次回调
	{
		count, count0, count1, count2, count3 := 0, 0, 0, 0, 0
		manager.Register(10030, func(args ...any) { count++ }, true)
		RegisterT0(manager, 10031, func() { count0++ }, true)
		RegisterT1(manager, 10032, func(v int) { count1++ }, true)
		RegisterT2(manager, 10033, func(v1 int, v2 string) { count2++ }, true)
		RegisterT3(manager, 10034, func(v1 int, v2 string, v3 float64) { count3++ }, true)

		assert.True(t, manager.Notify(10030), "事件 10030 的首次通知应返回成功。")
		assert.Equal(t, 1, count, "Register 注册的事件 10030 的回调函数应当被执行一次。")
		assert.False(t, manager.Notify(10030), "事件 10030 的单次回调执行后，再次通知应返回失败。")
		assert.Equal(t, 1, count, "Register 注册的事件 10030 的回调函数不应再次执行。")

		assert.True(t, manager.Notify(10031), "事件 10031 的首次通知应返回成功。")
		assert.Equal(t, 1, count0, "RegisterT0 注册的事件 10031 的回调函数应当被执行一次。")
		assert.False(t, manager.Notify(10031), "事件 10031 的单次回调执行后，再次通知应返回失败。")
		assert.Equal(t, 1, count0, "RegisterT0 注册的事件 10031 的回调函数不应再次执行。")

		assert.True(t, manager.Notify(10032), "事件 10032 的首次通知应返回成功。")
		assert.Equal(t, 1, count1, "RegisterT1 注册的事件 10032 的回调函数应当被执行一次。")
		assert.False(t, manager.Notify(10032), "事件 10032 的单次回调执行后，再次通知应返回失败。")
		assert.Equal(t, 1, count1, "RegisterT1 注册的事件 10032 的回调函数不应再次执行。")

		assert.True(t, manager.Notify(10033), "事件 10033 的首次通知应返回成功。")
		assert.Equal(t, 1, count2, "RegisterT2 注册的事件 10033 的回调函数应当被执行一次。")
		assert.False(t, manager.Notify(10033), "事件 10033 的单次回调执行后，再次通知应返回失败。")
		assert.Equal(t, 1, count2, "RegisterT2 注册的事件 10033 的回调函数不应再次执行。")

		assert.True(t, manager.Notify(10034), "事件 10034 的首次通知应返回成功。")
		assert.Equal(t, 1, count3, "RegisterT3 注册的事件 10034 的回调函数应当被执行一次。")
		assert.False(t, manager.Notify(10034), "事件 10034 的单次回调执行后，再次通知应返回失败。")
		assert.Equal(t, 1, count3, "RegisterT3 注册的事件 10034 的回调函数不应再次执行。")
	}
}
