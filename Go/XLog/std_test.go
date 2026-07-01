// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XLog

import (
	"bytes"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestStdName(t *testing.T) {
	apt := &Std{Level: LevelInfo, Color: false}
	assert.Equal(t, "Std", apt.Name())
}

func TestStdWrite(t *testing.T) {
	t.Run("Able", func(t *testing.T) {
		{
			writer := &bytes.Buffer{}
			apt := &Std{Level: LevelError, writer: writer}
			apt.Write(Data{Level: LevelNotice, Content: "test able"})
			assert.Equal(t, 0, writer.Len(), "日志级别低于期望时，不写入日志。")
		}

		{
			writer := &bytes.Buffer{}
			apt := &Std{Level: LevelNotice, writer: writer}
			apt.Write(Data{Level: LevelNotice, Content: "test able"})
			assert.Greater(t, writer.Len(), 0, "日志级别符合期望时，应写入日志。")
		}
	})

	t.Run("Format", func(t *testing.T) {
		writer := &bytes.Buffer{}
		apt := &Std{Level: LevelInfo, writer: writer}
		apt.Write(Data{Level: LevelInfo})
		apt.Write(Data{Level: LevelInfo, Content: "%v %s", Args: []any{"test", "format"}})
		apt.Write(Data{Level: LevelInfo, Content: struct {
			Arg0 string
			Arg1 string
		}{Arg0: "test", Arg1: "struct"}, Args: nil})
		apt.Write(Data{Level: LevelInfo, Content: "test tag", Tag: Tag{pairs: []string{"key1", "value1", "key2", "value2"}}})

		ctt := writer.String()
		assert.Contains(t, ctt, "[01/01 00:00:00.000] [I] <nil>")
		assert.Contains(t, ctt, "[01/01 00:00:00.000] [I] test format")
		assert.Contains(t, ctt, "[01/01 00:00:00.000] [I] {test struct}")
		assert.Contains(t, ctt, "[01/01 00:00:00.000] [I] [key1=value1, key2=value2] test tag")
	})

	t.Run("Color", func(t *testing.T) {
		writer := &bytes.Buffer{}
		apt := &Std{Level: LevelDebug, Color: true, writer: writer}
		apt.Write(Data{Level: LevelEmergency, Content: "test emergency"})
		apt.Write(Data{Level: LevelAlert, Content: "test alert"})
		apt.Write(Data{Level: LevelCritical, Content: "test critical"})
		apt.Write(Data{Level: LevelError, Content: "test error"})
		apt.Write(Data{Level: LevelWarn, Content: "test warn"})
		apt.Write(Data{Level: LevelNotice, Content: "test notice"})
		apt.Write(Data{Level: LevelInfo, Content: "test info"})
		apt.Write(Data{Level: LevelDebug, Content: "test debug"})

		ctt := writer.String()
		assert.Contains(t, ctt, "[01/01 00:00:00.000] \033[1;39m[M]\033[0m test emergency", "Emergency 级别应使用黑色 (1;39)")
		assert.Contains(t, ctt, "[01/01 00:00:00.000] \033[1;36m[A]\033[0m test alert", "Alert 级别应使用青色 (1;36)")
		assert.Contains(t, ctt, "[01/01 00:00:00.000] \033[1;35m[C]\033[0m test critical", "Critical 级别应使用品红色 (1;35)")
		assert.Contains(t, ctt, "[01/01 00:00:00.000] \033[1;31m[E]\033[0m test error", "Error 级别应使用红色 (1;31)")
		assert.Contains(t, ctt, "[01/01 00:00:00.000] \033[1;33m[W]\033[0m test warn", "Warn 级别应使用黄色 (1;33)")
		assert.Contains(t, ctt, "[01/01 00:00:00.000] \033[1;32m[N]\033[0m test notice", "Notice 级别应使用绿色 (1;32)")
		assert.Contains(t, ctt, "[01/01 00:00:00.000] \033[1;30m[I]\033[0m test info", "Info 级别应使用灰色 (1;30)")
		assert.Contains(t, ctt, "[01/01 00:00:00.000] \033[1;34m[D]\033[0m test debug", "Debug 级别应使用蓝色 (1;34)")
	})
}

func TestStdFlush(t *testing.T) {
	apt := &Std{Level: LevelInfo, Color: false}
	apt.Flush()
}

func TestStdReset(t *testing.T) {
	apt := &Std{Level: LevelInfo, Color: false}
	apt.Reset()
}
