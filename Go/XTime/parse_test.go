// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XTime

import (
	"testing"
	"time"

	"github.com/stretchr/testify/assert"
)

func TestParse(t *testing.T) {
	// 准备测试数据
	testDateTime := time.Date(2024, 3, 21, 14, 30, 0, 123000000, time.Local)
	testSeconds := int(testDateTime.Sub(Initial()).Seconds())
	testMilliseconds := int64(testDateTime.Sub(Initial()).Milliseconds())

	// 秒级时间戳转 time.Time
	timeFromSeconds := Parse(testSeconds)

	// 验证转换后的时间各个部分
	assert.Equal(t, testDateTime.Year(), timeFromSeconds.Year(), "年份应匹配。")
	assert.Equal(t, testDateTime.Month(), timeFromSeconds.Month(), "月份应匹配。")
	assert.Equal(t, testDateTime.Day(), timeFromSeconds.Day(), "日期应匹配。")
	assert.Equal(t, testDateTime.Hour(), timeFromSeconds.Hour(), "小时应匹配。")
	assert.Equal(t, testDateTime.Minute(), timeFromSeconds.Minute(), "分钟应匹配。")
	assert.Equal(t, testDateTime.Second(), timeFromSeconds.Second(), "秒数应匹配。")

	// 毫秒级时间戳转 time.Time
	timeFromMilliseconds := Parse(testMilliseconds)

	// 验证转换后的时间各个部分
	assert.Equal(t, testDateTime.Year(), timeFromMilliseconds.Year(), "年份应匹配。")
	assert.Equal(t, testDateTime.Month(), timeFromMilliseconds.Month(), "月份应匹配。")
	assert.Equal(t, testDateTime.Day(), timeFromMilliseconds.Day(), "日期应匹配。")
	assert.Equal(t, testDateTime.Hour(), timeFromMilliseconds.Hour(), "小时应匹配。")
	assert.Equal(t, testDateTime.Minute(), timeFromMilliseconds.Minute(), "分钟应匹配。")
	assert.Equal(t, testDateTime.Second(), timeFromMilliseconds.Second(), "秒数应匹配。")
	assert.Equal(t, testDateTime.Nanosecond()/1000000, timeFromMilliseconds.Nanosecond()/1000000, "毫秒数应匹配。")
}
