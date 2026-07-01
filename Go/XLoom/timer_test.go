// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XLoom

import (
	"testing"
	"time"

	"github.com/frameworkease/Universal.X.Utility/Go/XTime"
	"github.com/stretchr/testify/assert"
)

func TestTimer(t *testing.T) {
	defer func() { Reset() }()
	Setup(2, 10, 1000)

	t.Run("Timeout", func(t *testing.T) {
		done := make(chan struct{})
		tm := XTime.Milliseconds()
		dt := 0
		tm1 := SetTimeout(func() {
			dt = XTime.Milliseconds() - tm
			close(done)
		}, 500, 0)

		clear := true
		tm2 := SetTimeout(func() { clear = false }, 500, 0)
		ClearTimeout(tm2, 0)

		select {
		case <-done:
		case <-time.After(time.Second):
			t.Fatal("定时器回调超时")
		}

		assert.Greater(t, tm1, 0, "返回的定时器 ID 应当为正数。")
		assert.GreaterOrEqual(t, dt, 500, "等待时间应当大于等于 500 毫秒。")
		assert.Equal(t, true, clear, "清除的定时器不应当被回调。")

		assert.Equal(t, -1, SetTimeout(nil, 100, 0), "传入空的回调函数应当返回 -1。")
		assert.Equal(t, -1, SetTimeout(func() {}, -1, 0), "传入小于零的超时时长应当返回 -1。")
		assert.Equal(t, -1, SetTimeout(func() {}, 100, -1), "传入非法的 loomID 应当返回 -1。")
		assert.Equal(t, -1, SetTimeout(func() {}, 100, 999), "传入越界的 loomID 应当返回 -1。")
	})

	t.Run("Interval", func(t *testing.T) {
		count := 0
		done := make(chan struct{})

		tm := XTime.Milliseconds()
		dt := 0
		tm1 := 0
		tm1 = SetInterval(func() {
			count++
			if count >= 3 {
				dt = XTime.Milliseconds() - tm
				ClearInterval(tm1, 1)
				close(done)
			}
			panic("test interval panic") // 触发 panic，下一个周期的定时器应当继续执行
		}, 200, 1)

		clear := true
		tm2 := SetInterval(func() { clear = false }, 200, 1)
		ClearInterval(tm2, 1)

		select {
		case <-done:
		case <-time.After(time.Second):
			t.Fatal("定时器回调超时")
		}

		assert.Greater(t, tm1, 0, "返回的定时器 ID 应当为正数。")
		assert.GreaterOrEqual(t, dt, 600, "等待时间应当大于等于 600 毫秒。")
		assert.Equal(t, true, clear, "清除的定时器不应当被回调。")

		assert.Equal(t, -1, SetInterval(nil, 100, 0), "传入空的回调函数应当返回 -1。")
		assert.Equal(t, -1, SetInterval(func() {}, -1, 0), "传入小于零的超时时长应当返回 -1。")
		assert.Equal(t, -1, SetInterval(func() {}, 100, -1), "传入非法的 loomID 应当返回 -1。")
		assert.Equal(t, -1, SetInterval(func() {}, 100, 999), "传入越界的 loomID 应当返回 -1。")
	})
}
