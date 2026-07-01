// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XString

import (
	"testing"

	"github.com/stretchr/testify/assert"
)

// MyEvaluator 是用于测试的求值器实现。
type MyEvaluator struct{ Vars map[string]string }

func (e *MyEvaluator) Evaluate(input string) string {
	if input == "" {
		return ""
	}
	if e.Vars != nil {
		input = Evaluate(input, e.Vars)
	}
	return input
}

// TestEvaluate 测试 Evaluate 函数
func TestEvaluate(t *testing.T) {
	t.Run("Evaluator", func(t *testing.T) {
		evaluator1 := &MyEvaluator{
			Vars: map[string]string{
				"name": "World",
			},
		}
		evaluator2 := &MyEvaluator{
			Vars: map[string]string{
				"greeting": "Hello",
			},
		}
		assert.Equal(t, "Hello World", Evaluate("Hello ${name}", evaluator1))
		assert.Equal(t, "World World", Evaluate("${name} ${name}", evaluator1))
		assert.Equal(t, "Hello World and ${other}", Evaluate("${greeting} ${name} and ${other}", evaluator1, evaluator2))
		assert.Equal(t, "", Evaluate("", evaluator1))
		assert.Equal(t, "Hello ${name}", Evaluate("Hello ${name}", (IEvaluator)(nil)))
		assert.Equal(t, "Hello ${name}", Evaluate("Hello ${name}", &MyEvaluator{}))
	})

	t.Run("Variable", func(t *testing.T) {
		variable1 := map[string]string{
			"name": "World",
		}
		variable2 := map[string]string{
			"greeting": "Hello",
		}
		assert.Equal(t, "Hello World", Evaluate("Hello ${name}", variable1))
		assert.Equal(t, "World World", Evaluate("${name} ${name}", variable1))
		assert.Equal(t, "Hello World and ${other}", Evaluate("${greeting} ${name} and ${other}", variable1, variable2))
		assert.Equal(t, "", Evaluate("", variable1))
		assert.Equal(t, "Hello ${name}", Evaluate("Hello ${name}", (map[string]string)(nil)))
		assert.Equal(t, "Hello ${name}", Evaluate("Hello ${name}", map[string]string{}))
	})
}
