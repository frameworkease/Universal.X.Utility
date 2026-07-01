// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XEnv

import (
	"os"
	"path/filepath"
	"testing"

	"github.com/frameworkease/Universal.X.Utility/Go/XFile"
	"github.com/frameworkease/Universal.X.Utility/Go/XString"
	"github.com/stretchr/testify/assert"
)

func TestVariable(t *testing.T) {
	defer Variable.Setup()

	testKeys := []string{"TEST_KEY1", "TEST_KEY2", "TEST_KEY3", "TEST_KEY4", "TEST_EMPTY"}
	defer func() {
		for _, key := range testKeys {
			os.Unsetenv(key)
		}
	}()

	os.Setenv("TEST_KEY1", "value1")
	os.Setenv("TEST_KEY2", "value2")
	os.Setenv("TEST_KEY3", "value3")
	os.Setenv("TEST_KEY4", "value4")
	os.Setenv("TEST_EMPTY", "")

	envFile := filepath.Join(XFile.Directory.Project(), ".env")
	envContent := "TEST_KEY1=fileValue1\n# This is a comment\nTEST_KEY2=fileValue2\n"
	assert.NoError(t, os.WriteFile(envFile, []byte(envContent), 0644))
	defer os.Remove(envFile)

	Variable.Setup("TEST_KEY3=extraValue3", "TEST_KEY4=extraValue4")

	t.Run("Get", func(t *testing.T) {
		assert.Equal(t, "fileValue1", Variable.Get("TEST_KEY1"), "Get 环境变量 TEST_KEY1 的值应当和预期相符。")
		assert.Equal(t, "fileValue2", Variable.Get("TEST_KEY2"), "Get 环境变量 TEST_KEY2 的值应当和预期相符。")
		assert.Equal(t, "extraValue3", Variable.Get("TEST_KEY3"), "Get 环境变量 TEST_KEY3 的值应当和预期相符。")
		assert.Equal(t, "extraValue4", Variable.Get("TEST_KEY4"), "Get 环境变量 TEST_KEY4 的值应当和预期相符。")
		assert.Equal(t, "", Variable.Get("TEST_EMPTY"), "Get 环境变量 TEST_EMPTY 的值应当返回空字符串。")
		assert.Equal(t, "", Variable.Get("TEST_NONEXISTENT"), "Get 不存在的环境变量应当返回空字符串。")
	})

	t.Run("Exists", func(t *testing.T) {
		assert.True(t, Variable.Exists("TEST_KEY1"), "Exists 环境变量 TEST_KEY1 应当存在。")
		assert.True(t, Variable.Exists("TEST_KEY2"), "Exists 环境变量 TEST_KEY2 应当存在。")
		assert.True(t, Variable.Exists("TEST_KEY3"), "Exists 环境变量 TEST_KEY3 应当存在。")
		assert.True(t, Variable.Exists("TEST_KEY4"), "Exists 环境变量 TEST_KEY4 应当存在。")
		assert.False(t, Variable.Exists("TEST_EMPTY"), "Exists 环境变量 TEST_EMPTY 应当返回 false。")
		assert.False(t, Variable.Exists("TEST_NONEXISTENT"), "Exists 不存在的环境变量应当返回 false。")
	})

	t.Run("Range", func(t *testing.T) {
		Variable.Range(func(key, value string) bool {
			switch key {
			case "TEST_KEY1":
				assert.Equal(t, "fileValue1", value, "Range 环境变量 TEST_KEY1 的值应当和预期相符。")
			case "TEST_KEY2":
				assert.Equal(t, "fileValue2", value, "Range 环境变量 TEST_KEY2 的值应当和预期相符。")
			case "TEST_KEY3":
				assert.Equal(t, "extraValue3", value, "Range 环境变量 TEST_KEY3 的值应当和预期相符。")
			case "TEST_KEY4":
				assert.Equal(t, "extraValue4", value, "Range 环境变量 TEST_KEY4 的值应当和预期相符。")
			}
			return true
		})

		var count = 0
		Variable.Range(func(key, value string) bool { count++; return count < 3 })
		assert.Equal(t, 3, count, "Range 应当在 callback 返回 false 时停止遍历。")
		assert.Panics(t, func() { Variable.Range(nil) }, "Range 应当在 callback 为 nil 时 panic。")
	})

	t.Run("Evaluate", func(t *testing.T) {
		os.Setenv("TEST_VAR", "test_var")
		defer os.Unsetenv("TEST_VAR")

		result := XString.Evaluate("${XEnv.Variable.TEST_VAR}", Variable.Evaluator)
		assert.Equal(t, "test_var", result)

		result = XString.Evaluate("${TEST_VAR}", Variable.Evaluator)
		assert.Equal(t, "test_var", result)

		result = XString.Evaluate("${TEST_UNKNOWN}", Variable.Evaluator)
		assert.Equal(t, "${Unknown.XEnv.Variable.TEST_UNKNOWN}", result)
	})
}
