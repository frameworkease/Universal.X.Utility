// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XTime

import (
	"testing"
	"time"

	"github.com/stretchr/testify/assert"
)

func TestStringify(t *testing.T) {
	// 准备测试数据
	testDateTime := time.Date(2024, 3, 21, 14, 30, 0, 123000000, time.Local)
	testSeconds := int(testDateTime.Sub(Initial()).Seconds())
	testMilliseconds := int64(testDateTime.Sub(Initial()).Milliseconds())

	expectedDateTimeStr := "2024-03-21 14:30:00"
	expectedDateStr := "2024-03-21"
	expectedMillisStr := "2024-03-21 14:30:00.123"
	expectedChineseStr := "2024年03月21日"

	// 验证秒级时间戳格式化
	assert.Equal(t, expectedDateTimeStr, Stringify(testSeconds), "默认格式的秒级时间戳格式化应正确。")
	assert.Equal(t, expectedDateStr, Stringify(testSeconds, "2006-01-02"), "自定义格式的秒级时间戳格式化应正确。")

	// 验证毫秒级时间戳格式化
	assert.Equal(t, expectedMillisStr, Stringify(testMilliseconds), "默认格式的毫秒级时间戳格式化应正确。")
	assert.Equal(t, "2024-03-21 14:30", Stringify(testMilliseconds, "2006-01-02 15:04"), "自定义格式的毫秒级时间戳格式化应正确。")

	// 验证 time.Time 格式化
	assert.Equal(t, expectedMillisStr, Stringify(testDateTime), "默认格式的 time.Time 格式化应正确。")
	assert.Equal(t, expectedChineseStr, Stringify(testDateTime, "2006年01月02日"), "中文格式的 time.Time 格式化应正确。")
}
