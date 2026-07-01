// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XEnv

import (
	"testing"

	"github.com/frameworkease/Universal.X.Utility/Go/XString"
	"github.com/stretchr/testify/assert"
)

func TestArgument(t *testing.T) {
	defer Argument.Setup()

	Argument.Setup(
		"--key1=value1",    // 双横杠等号格式
		"-key2=value2",     // 单横杠等号格式
		"--key3", "value3", // 双横杠空格格式
		"-key4", "value4", // 单横杠空格格式
		"--flag1",          // 双横杠无值
		"-flag2",           // 单横杠无值
		"--empty=",         // 空值参数
		"invalid",          // 非法参数
		"--key1=newValue1", // 多值参数
		"--key3=newValue3", // 多值参数
	)

	t.Run("Get", func(t *testing.T) {
		assert.Equal(t, "value1", Argument.Get("key1"))
		assert.Equal(t, "value2", Argument.Get("key2"))
		assert.Equal(t, "value3", Argument.Get("key3"))
		assert.Equal(t, "value4", Argument.Get("key4"))
		assert.Equal(t, "", Argument.Get("flag1"))
		assert.Equal(t, "", Argument.Get("flag2"))
		assert.Equal(t, "", Argument.Get("empty"))
	})

	t.Run("Exists", func(t *testing.T) {
		assert.True(t, Argument.Exists("key1"))
		assert.False(t, Argument.Exists("invalid"))
	})

	t.Run("Range", func(t *testing.T) {
		expected := []string{"key1=value1", "key2=value2", "key3=value3", "key4=value4", "flag1=", "flag2=", "empty=", "key1=newValue1", "key3=newValue3"}
		var seen []string
		Argument.Range(func(key, value string) bool {
			seen = append(seen, key+"="+value)
			return true
		})
		assert.Greater(t, len(seen), len(expected))
		assert.Equal(t, expected, seen[:len(expected)])

		var count int
		Argument.Range(func(key, value string) bool { count++; return count < 3 })
		assert.Equal(t, 3, count)
		assert.Panics(t, func() { Argument.Range(nil) })
	})

	t.Run("Evaluate", func(t *testing.T) {
		Argument.Setup("-test_arg=test_arg")
		defer Argument.Setup()

		result := XString.Evaluate("${XEnv.Argument.test_arg}", Argument.Evaluator)
		assert.Equal(t, "test_arg", result)

		Argument.Setup("-test_arg1=${XEnv.Argument.test_arg2}", "-test_arg2=${XEnv.Argument.test_arg1}")
		defer Argument.Setup()

		result = XString.Evaluate("${XEnv.Argument.test_arg1}", Argument.Evaluator)
		assert.Equal(t, "${Recursive.XEnv.Argument.test_arg1}", result)

		result = XString.Evaluate("${XEnv.Argument.test_arg1${XEnv.Argument.test_arg2}}", Argument.Evaluator)
		assert.Equal(t, "${Nested.${XEnv.Argument.test_arg1${XEnv.Argument.test_arg2}}}", result)
	})
}
