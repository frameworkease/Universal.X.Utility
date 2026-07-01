// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XEnv

import (
	"fmt"
	"os"
	"regexp"
	"strings"
	"sync"
	"sync/atomic"

	"github.com/frameworkease/Universal.X.Utility/Go/XString"
)

type argumentScope struct {
	cache     [][2]string
	mutex     sync.RWMutex
	setup     atomic.Bool
	Evaluator XString.IEvaluator
}

type argumentEvaluator struct {
	pattern *regexp.Regexp
}

var Argument = &argumentScope{
	Evaluator: &argumentEvaluator{
		pattern: regexp.MustCompile(`\$\{XEnv\.Argument\.([^}]+?)\}`),
	},
}

func (as *argumentScope) ensureSetup() {
	if !as.setup.Load() {
		as.Setup()
	}
}

func (as *argumentScope) Setup(extras ...string) {
	as.mutex.Lock()
	defer as.mutex.Unlock()

	as.cache = nil
	raws := make([]string, 0, len(extras)+len(os.Args))
	raws = append(raws, extras...)
	raws = append(raws, os.Args[1:]...)

	for i := 0; i < len(raws); i++ {
		arg := raws[i]
		if arg == "" || !strings.HasPrefix(arg, "-") {
			continue
		}
		var key string
		if after, ok := strings.CutPrefix(arg, "--"); ok {
			key = after
		} else {
			key = strings.TrimPrefix(arg, "-")
		}
		if before, after, ok := strings.Cut(key, "="); ok {
			as.cache = append(as.cache, [2]string{before, after})
			continue
		}
		if i+1 >= len(raws) || strings.HasPrefix(raws[i+1], "-") {
			as.cache = append(as.cache, [2]string{key, ""})
			continue
		}
		as.cache = append(as.cache, [2]string{key, raws[i+1]})
		i++
	}
	as.setup.Store(true)
}

func (as *argumentScope) Get(key string) string {
	as.ensureSetup()

	as.mutex.RLock()
	defer as.mutex.RUnlock()

	for _, p := range as.cache {
		if p[0] == key {
			return p[1]
		}
	}
	return ""
}

func (as *argumentScope) Exists(key string) bool {
	as.ensureSetup()

	as.mutex.RLock()
	defer as.mutex.RUnlock()

	for _, p := range as.cache {
		if p[0] == key {
			return true
		}
	}
	return false
}

func (as *argumentScope) Range(callback func(key, value string) bool) {
	as.ensureSetup()

	if callback == nil {
		panic("XEnv.Argument.Range: callback is nil.")
	}
	as.mutex.RLock()
	defer as.mutex.RUnlock()

	for _, p := range as.cache {
		if !callback(p[0], p[1]) {
			break
		}
	}
}

func (ae *argumentEvaluator) Evaluate(expr string) string {
	if expr == "" {
		return expr
	}

	visited := make(map[string]bool)

	var repl func(string) string
	repl = func(m string) string {
		sm := ae.pattern.FindStringSubmatch(m)
		keyName := sm[1]
		key := "Argument." + keyName

		if strings.Contains(keyName, "${") {
			return fmt.Sprintf("${Nested.%v}", m)
		}

		if visited[key] {
			return fmt.Sprintf("${Recursive.XEnv.%v}", key)
		}
		visited[key] = true
		defer delete(visited, key)

		value := Argument.Get(keyName)
		if value != "" {
			return ae.pattern.ReplaceAllStringFunc(value, repl)
		}
		return fmt.Sprintf("${Unknown.XEnv.Argument.%v}", keyName)
	}

	return ae.pattern.ReplaceAllStringFunc(expr, repl)
}
