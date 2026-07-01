// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XEnv

import (
	"fmt"
	"os"
	"path/filepath"
	"regexp"
	"strings"
	"sync/atomic"

	"github.com/frameworkease/Universal.X.Utility/Go/XFile"
	"github.com/frameworkease/Universal.X.Utility/Go/XString"
)

type variableScope struct {
	setup     atomic.Bool
	Evaluator XString.IEvaluator
}

type variableEvaluator struct {
	pattern *regexp.Regexp
}

var Variable = &variableScope{
	Evaluator: &variableEvaluator{
		pattern: regexp.MustCompile(`\$\{(?:XEnv\.Variable\.([^}]+?)|([A-Za-z_][A-Za-z0-9_]*))\}`),
	},
}

func (vs *variableScope) ensureSetup() {
	if !vs.setup.Load() {
		vs.Setup()
	}
}

func (vs *variableScope) Setup(extras ...string) {
	// 解析文件
	{
		envFile := filepath.Join(XFile.Directory.Project(), ".env")
		if _, err := os.Stat(envFile); err == nil {
			if data, err := os.ReadFile(envFile); err == nil {
				content := string(data)
				if content != "" {
					lines := strings.SplitSeq(content, "\n")
					for rawLine := range lines {
						line := strings.TrimSpace(rawLine)

						// 跳过空行和注释行
						if line == "" || strings.HasPrefix(line, "#") {
							continue
						}

						// 查找等号分隔符
						before, after, ok := strings.Cut(line, "=")
						if !ok {
							continue
						}

						// 提取键和值
						key := strings.TrimSpace(before)
						value := strings.TrimSpace(after)

						// 跳过无效的键
						if key == "" || value == "" {
							continue
						}

						// 设置环境变量
						os.Setenv(key, value)
					}
				}
			}
		}
	}

	// 解析参数
	{
		for _, extra := range extras {
			if extra == "" {
				continue
			}

			// 解析 "KEY=VALUE" 格式
			before, after, ok := strings.Cut(extra, "=")
			if !ok {
				continue
			}

			key := strings.TrimSpace(before)
			value := strings.TrimSpace(after)

			// 跳过无效的键或值
			if key == "" || value == "" {
				continue
			}

			// 设置环境变量
			os.Setenv(key, value)
		}
	}
	vs.setup.Store(true)
}

func (vs *variableScope) Get(key string) string {
	vs.ensureSetup()
	return os.Getenv(key)
}

func (vs *variableScope) Exists(key string) bool {
	vs.ensureSetup()
	val, ok := os.LookupEnv(key)
	return val != "" && ok
}

func (vs *variableScope) Range(callback func(key, value string) bool) {
	vs.ensureSetup()

	if callback == nil {
		panic("XEnv.Variable.Range: callback is nil.")
	}
	for _, env := range os.Environ() {
		before, after, ok := strings.Cut(env, "=")
		if ok {
			if !callback(before, after) {
				break
			}
		}
	}
}

func (ve *variableEvaluator) Evaluate(expr string) string {
	if expr == "" {
		return expr
	}

	visited := make(map[string]bool)

	var repl func(string) string
	repl = func(m string) string {
		sm := ve.pattern.FindStringSubmatch(m)
		g1, g2 := sm[1], sm[2]

		var key string
		var value string
		if g2 != "" {
			key = "Variable." + g2
			value = Variable.Get(g2)
		} else {
			key = "Variable." + g1
			if strings.Contains(g1, "${") {
				return fmt.Sprintf("${Nested.%v}", m)
			}
			value = Variable.Get(g1)
		}

		if visited[key] {
			return fmt.Sprintf("${Recursive.XEnv.%v}", key)
		}
		visited[key] = true
		defer delete(visited, key)

		if value != "" {
			return ve.pattern.ReplaceAllStringFunc(value, repl)
		}
		if g2 != "" {
			return fmt.Sprintf("${Unknown.XEnv.Variable.%v}", g2)
		}
		return fmt.Sprintf("${Unknown.XEnv.Variable.%v}", g1)
	}

	return ve.pattern.ReplaceAllStringFunc(expr, repl)
}
