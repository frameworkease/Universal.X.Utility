// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XLoom

import (
	"sync"
	"testing"
	"time"

	"github.com/stretchr/testify/assert"
)

func TestLoom(t *testing.T) {
	defer func() { Reset() }()

	t.Run("Count", func(t *testing.T) {
		Setup(2, 10, 1000)
		assert.Equal(t, 2, Count(), "应当有 2 个业务线程。")
	})

	t.Run("ID", func(t *testing.T) {
		Setup(2, 10, 1000)

		var wg sync.WaitGroup
		wg.Add(1)

		RunIn(func() {
			lid := ID()
			assert.Equal(t, 0, lid, "应当运行在业务线程 0 中。")
			wg.Done()
		}, 0)

		wg.Wait()
	})

	t.Run("RunIn", func(t *testing.T) {
		Setup(2, 10, 1000)

		var wg sync.WaitGroup
		executed := false
		wg.Add(1)

		RunIn(func() {
			executed = true
			wg.Done()
		}, 0)

		wg.Wait()
		assert.True(t, executed, "任务应当被执行。")
	})

	t.Run("Manage", func(t *testing.T) {
		Setup(2, 10, 1000)

		var wg sync.WaitGroup

		Pause(0)
		wg.Add(1)
		RunIn(func() {
			wg.Done()
		}, 0)
		Resume(0)

		done := make(chan struct{})
		go func() {
			wg.Wait()
			close(done)
		}()

		select {
		case <-done:
		case <-time.After(time.Second):
			t.Fatal("任务未在时间内执行。")
		}
	})
}
