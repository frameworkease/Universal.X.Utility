// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XLoom

import (
	"sync"
	"sync/atomic"
	"testing"
	"time"

	"github.com/stretchr/testify/assert"
)

func TestRunAsync(t *testing.T) {
	t.Run("Basic", func(t *testing.T) {
		done := make(chan struct{})
		RunAsync(func() { close(done) })
		select {
		case <-done:
		case <-time.After(time.Second):
			t.Fatal("异步函数未在时间内完成。")
		}
	})

	t.Run("Nil", func(t *testing.T) { RunAsync(nil) })

	t.Run("Restart", func(t *testing.T) {
		done := make(chan struct{})
		var count int32
		RunAsync(func() {
			if atomic.AddInt32(&count, 1) == 1 {
				panic("test panic")
			}
			close(done)
		}, true)
		select {
		case <-done:
			assert.Equal(t, int32(2), atomic.LoadInt32(&count), "函数应当执行两次，因为发生了异常重试。")
		case <-time.After(time.Second):
			t.Fatal("异常重试未在时间内完成。")
		}
	})
}

func TestRunAsyncT1(t *testing.T) {
	t.Run("Basic", func(t *testing.T) {
		result := make(chan int, 1)
		RunAsyncT1(func(x int) { result <- x * 2 }, 21)
		select {
		case val := <-result:
			assert.Equal(t, 42, val)
		case <-time.After(time.Second):
			t.Fatal("异步函数未在时间内完成。")
		}
	})

	t.Run("Nil", func(t *testing.T) { RunAsyncT1(nil, 42) })

	t.Run("Restart", func(t *testing.T) {
		wg := sync.WaitGroup{}
		wg.Add(2)
		count := 0
		RunAsyncT1(func(x int) {
			wg.Done()
			count++
			if count == 1 {
				panic("test panic")
			}
		}, 42, true)
		wg.Wait()
		assert.Equal(t, 2, count, "函数应当执行两次，因为发生了异常重试。")
	})
}

func TestRunAsyncT2(t *testing.T) {
	t.Run("Basic", func(t *testing.T) {
		result := make(chan string, 1)
		RunAsyncT2(func(x int, y string) { result <- y + ":" + string(rune(x)) }, 65, "A")
		select {
		case val := <-result:
			assert.Equal(t, "A:A", val)
		case <-time.After(time.Second):
			t.Fatal("异步函数未在时间内完成。")
		}
	})

	t.Run("Nil", func(t *testing.T) { RunAsyncT2(nil, 42, "test") })

	t.Run("Restart", func(t *testing.T) {
		wg := sync.WaitGroup{}
		wg.Add(2)
		count := 0
		RunAsyncT2(func(x int, y string) {
			wg.Done()
			count++
			if count == 1 {
				panic("test panic")
			}
		}, 42, "test", true)
		wg.Wait()
		assert.Equal(t, 2, count, "函数应当执行两次，因为发生了异常重试。")
	})
}

func TestRunAsyncT3(t *testing.T) {
	t.Run("Basic", func(t *testing.T) {
		result := make(chan string, 1)
		RunAsyncT3(func(x int, y string, z bool) {
			if z {
				result <- y + ":" + string(rune(x))
			}
		}, 65, "A", true)
		select {
		case val := <-result:
			assert.Equal(t, "A:A", val)
		case <-time.After(time.Second):
			t.Fatal("异步函数未在时间内完成。")
		}
	})

	t.Run("Nil", func(t *testing.T) { RunAsyncT3(nil, 42, "test", true) })

	t.Run("Restart", func(t *testing.T) {
		wg := sync.WaitGroup{}
		wg.Add(2)
		count := 0
		RunAsyncT3(func(x int, y string, z bool) {
			wg.Done()
			count++
			if count == 1 {
				panic("test panic")
			}
		}, 42, "test", true, true)
		wg.Wait()
		assert.Equal(t, 2, count, "函数应当执行两次，因为发生了异常重试。")
	})
}
