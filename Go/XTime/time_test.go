// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XTime

import (
	"testing"
	"time"

	"github.com/stretchr/testify/assert"
)

func TestCurrent(t *testing.T) {
	assert.WithinDuration(t, Current(), time.Now(), 100*time.Millisecond, "获取的当前时间应在100毫秒误差范围内。")
}

func TestSeconds(t *testing.T) {
	now := time.Now()
	expected := int(now.Sub(Initial()).Seconds())
	seconds := Seconds()
	delta := seconds - expected
	assert.LessOrEqual(t, delta, 1, "获取的秒级时间戳应在1秒误差范围内。")
}

func TestMilliseconds(t *testing.T) {
	now := time.Now()
	expected := int(now.Sub(Initial()).Milliseconds())
	milliseconds := Milliseconds()
	delta := milliseconds - expected
	assert.LessOrEqual(t, delta, 100, "获取的毫秒级时间戳应在100毫秒误差范围内。")
}

func TestMicroseconds(t *testing.T) {
	now := time.Now()
	expected := int(now.Sub(Initial()).Microseconds())
	microseconds := Microseconds()
	delta := microseconds - expected
	assert.LessOrEqual(t, delta, 10000, "获取的微秒级时间戳应在10000微秒误差范围内。")
}
