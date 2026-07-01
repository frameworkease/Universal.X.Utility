// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XPrefs

import (
	"encoding/json"
	"fmt"
	"maps"
	"reflect"
	"regexp"
	"sort"
	"strconv"
	"strings"
	"sync"
)

type IBase interface {
	Set(key string, value any) IBase
	Delete(key string) IBase
	Exists(key string) bool
	Range(callback func(key string, value any) bool)
	Get(key string, defval ...any) any
	Gets(key string, defval ...[]any) []any
	GetInt(key string, defval ...int) int
	GetInts(key string, defval ...[]int) []int
	GetFloat(key string, defval ...float32) float32
	GetFloats(key string, defval ...[]float32) []float32
	GetString(key string, defval ...string) string
	GetStrings(key string, defval ...[]string) []string
	GetBool(key string, defval ...bool) bool
	GetBools(key string, defval ...[]bool) []bool
	Stringify(pretty ...bool) string
	Parse(content []byte) bool
	Evaluate(expr string) string
}

type Base struct {
	mutex sync.RWMutex
	pairs map[string]any
}

func (prefs *Base) Set(key string, value any) IBase {
	if key == "" {
		fmt.Println("XPrefs.Base.Set: key is nil.")
		return prefs
	}
	if value == nil {
		fmt.Println("XPrefs.Base.Set: value is nil.")
		return prefs
	}

	prefs.mutex.Lock()
	defer prefs.mutex.Unlock()

	if prefs.pairs == nil {
		prefs.pairs = make(map[string]any)
	}
	prefs.pairs[key] = value

	return prefs
}

func (prefs *Base) Delete(key string) IBase {
	if key == "" {
		fmt.Println("XPrefs.Base.Delete: key is nil.")
		return prefs
	}

	prefs.mutex.Lock()
	defer prefs.mutex.Unlock()

	delete(prefs.pairs, key)

	return prefs
}

func (prefs *Base) Exists(key string) bool {
	if key == "" {
		fmt.Println("XPrefs.Base.Exists: key is nil.")
		return false
	}

	prefs.mutex.RLock()
	defer prefs.mutex.RUnlock()

	_, exists := prefs.pairs[key]
	return exists
}

func (prefs *Base) Range(callback func(key string, value any) bool) {
	if callback == nil {
		fmt.Println("XPrefs.Base.Range: callback is nil.")
		return
	}

	prefs.mutex.RLock()
	if len(prefs.pairs) == 0 {
		prefs.mutex.RUnlock()
		return
	}

	pairs := make(map[string]any, len(prefs.pairs))
	maps.Copy(pairs, prefs.pairs)
	prefs.mutex.RUnlock()

	for key, value := range pairs {
		if !callback(key, value) {
			break
		}
	}
}

func (prefs *Base) Get(key string, defval ...any) any {
	if key == "" {
		fmt.Println("XPrefs.Base.Get: key is nil.")
		if len(defval) > 0 {
			return defval[0]
		}
		return nil
	}

	prefs.mutex.RLock()
	defer prefs.mutex.RUnlock()

	if val, exists := prefs.pairs[key]; exists {
		switch val := val.(type) {
		case map[string]any:
			nv := &Base{}
			maps.Copy(nv.pairs, val)
			return nv
		default:
			return val
		}
	}
	if len(defval) > 0 {
		return defval[0]
	}
	return nil
}

func (prefs *Base) Gets(key string, defval ...[]any) []any {
	if key == "" {
		fmt.Println("XPrefs.Base.Gets: key is nil.")
		if len(defval) > 0 {
			return defval[0]
		}
		return nil
	}

	prefs.mutex.RLock()
	defer prefs.mutex.RUnlock()

	if val, exists := prefs.pairs[key]; exists {
		switch v := val.(type) {
		case []map[string]any:
			var nv []any
			for _, vv := range v {
				p := &Base{}
				maps.Copy(p.pairs, vv)
				nv = append(nv, p)
			}
			return nv
		case []any:
			return v
		default:
			rv := reflect.ValueOf(val)
			if rv.Kind() == reflect.Slice {
				nv := make([]any, rv.Len())
				for i := 0; i < rv.Len(); i++ {
					vv := rv.Index(i).Interface()
					if mv, ok := vv.(map[string]any); ok {
						p := &Base{}
						maps.Copy(p.pairs, mv)
						nv[i] = p
					} else {
						nv[i] = vv
					}
				}
				return nv
			}
		}
	}

	if len(defval) > 0 {
		return defval[0]
	}

	return nil
}

func (prefs *Base) GetInt(key string, defval ...int) int {
	if key == "" {
		fmt.Println("XPrefs.Base.GetInt: key is nil.")
		if len(defval) > 0 {
			return defval[0]
		}
		return 0
	}

	prefs.mutex.RLock()
	defer prefs.mutex.RUnlock()

	if val, exists := prefs.pairs[key]; exists {
		switch v := val.(type) {
		case int:
			return v
		case int8:
			return int(v)
		case int16:
			return int(v)
		case int32:
			return int(v)
		case int64:
			return int(v)
		case float32:
			return int(v)
		case float64:
			return int(v)
		case string:
			if iv, err := strconv.Atoi(v); err == nil {
				return iv
			}
		case bool:
			if v {
				return 1
			}
			return 0
		}
	}
	if len(defval) > 0 {
		return defval[0]
	}
	return 0
}

func (prefs *Base) GetInts(key string, defval ...[]int) []int {
	if key == "" {
		fmt.Println("XPrefs.Base.GetInts: key is nil.")
		if len(defval) > 0 {
			return defval[0]
		}
		return nil
	}

	prefs.mutex.RLock()
	defer prefs.mutex.RUnlock()

	if val, exists := prefs.pairs[key]; exists {
		switch v := val.(type) {
		case []int:
			return v
		default:
			rv := reflect.ValueOf(val)
			if rv.Kind() == reflect.Slice {
				arr := make([]int, rv.Len())
				for i := 0; i < rv.Len(); i++ {
					vv := rv.Index(i).Interface()
					switch ve := vv.(type) {
					case int:
						arr[i] = ve
					case int8:
						arr[i] = int(ve)
					case int16:
						arr[i] = int(ve)
					case int32:
						arr[i] = int(ve)
					case int64:
						arr[i] = int(ve)
					case float32:
						arr[i] = int(ve)
					case float64:
						arr[i] = int(ve)
					case string:
						if iv, err := strconv.Atoi(ve); err == nil {
							arr[i] = iv
						}
					case bool:
						if ve {
							arr[i] = 1
						} else {
							arr[i] = 0
						}
					}
				}
				return arr
			}
		}
	}
	if len(defval) > 0 {
		return defval[0]
	}
	return nil
}

func (prefs *Base) GetFloat(key string, defval ...float32) float32 {
	if key == "" {
		fmt.Println("XPrefs.Base.GetFloat: key is nil.")
		if len(defval) > 0 {
			return defval[0]
		}
		return 0
	}

	prefs.mutex.RLock()
	defer prefs.mutex.RUnlock()

	if val, exists := prefs.pairs[key]; exists {
		switch v := val.(type) {
		case int:
			return float32(v)
		case int8:
			return float32(v)
		case int16:
			return float32(v)
		case int32:
			return float32(v)
		case int64:
			return float32(v)
		case float32:
			return float32(v)
		case float64:
			return float32(v)
		case string:
			if fv, err := strconv.ParseFloat(v, 32); err == nil {
				return float32(fv)
			}
		case bool:
			if v {
				return 1
			}
			return 0
		}
	}
	if len(defval) > 0 {
		return defval[0]
	}
	return 0
}

func (prefs *Base) GetFloats(key string, defval ...[]float32) []float32 {
	if key == "" {
		fmt.Println("XPrefs.Base.GetFloats: key is nil.")
		if len(defval) > 0 {
			return defval[0]
		}
		return nil
	}

	prefs.mutex.RLock()
	defer prefs.mutex.RUnlock()

	if val, exists := prefs.pairs[key]; exists {
		switch v := val.(type) {
		case []float32:
			return v
		default:
			rv := reflect.ValueOf(val)
			if rv.Kind() == reflect.Slice {
				arr := make([]float32, rv.Len())
				for i := 0; i < rv.Len(); i++ {
					vv := rv.Index(i).Interface()
					switch ve := vv.(type) {
					case int:
						arr[i] = float32(ve)
					case int8:
						arr[i] = float32(ve)
					case int16:
						arr[i] = float32(ve)
					case int32:
						arr[i] = float32(ve)
					case int64:
						arr[i] = float32(ve)
					case float32:
						arr[i] = ve
					case float64:
						arr[i] = float32(ve)
					case string:
						if fv, err := strconv.ParseFloat(ve, 32); err == nil {
							arr[i] = float32(fv)
						}
					case bool:
						if ve {
							arr[i] = 1
						} else {
							arr[i] = 0
						}
					}
				}
				return arr
			}
		}
	}
	if len(defval) > 0 {
		return defval[0]
	}
	return nil
}

func (prefs *Base) GetString(key string, defval ...string) string {
	if key == "" {
		fmt.Println("XPrefs.Base.GetString: key is nil.")
		if len(defval) > 0 {
			return defval[0]
		}
		return ""
	}

	prefs.mutex.RLock()
	defer prefs.mutex.RUnlock()

	if val, exists := prefs.pairs[key]; exists {
		if strVal, ok := val.(string); ok {
			return strVal
		} else {
			return fmt.Sprintf("%v", val)
		}
	}
	if len(defval) > 0 {
		return defval[0]
	}
	return ""
}

func (prefs *Base) GetStrings(key string, defval ...[]string) []string {
	if key == "" {
		fmt.Println("XPrefs.Base.GetStrings: key is nil.")
		if len(defval) > 0 {
			return defval[0]
		}
		return nil
	}

	prefs.mutex.RLock()
	defer prefs.mutex.RUnlock()

	if val, exists := prefs.pairs[key]; exists {
		switch v := val.(type) {
		case []string:
			return v
		default:
			rv := reflect.ValueOf(val)
			if rv.Kind() == reflect.Slice {
				arr := make([]string, rv.Len())
				for i := 0; i < rv.Len(); i++ {
					vv := rv.Index(i).Interface()
					if sv, ok := vv.(string); ok {
						arr[i] = sv
					} else {
						arr[i] = fmt.Sprintf("%v", vv)
					}
				}
				return arr
			}
		}
	}
	if len(defval) > 0 {
		return defval[0]
	}
	return nil
}

func (prefs *Base) GetBool(key string, defval ...bool) bool {
	if key == "" {
		fmt.Println("XPrefs.Base.GetBool: key is nil.")
		if len(defval) > 0 {
			return defval[0]
		}
		return false
	}

	prefs.mutex.RLock()
	defer prefs.mutex.RUnlock()

	if val, exists := prefs.pairs[key]; exists {
		if boolVal, ok := val.(bool); ok {
			return boolVal
		}
	}
	if len(defval) > 0 {
		return defval[0]
	}
	return false
}

func (prefs *Base) GetBools(key string, defval ...[]bool) []bool {
	if key == "" {
		fmt.Println("XPrefs.Base.GetBools: key is nil.")
		if len(defval) > 0 {
			return defval[0]
		}
		return nil
	}

	prefs.mutex.RLock()
	defer prefs.mutex.RUnlock()

	if val, exists := prefs.pairs[key]; exists {
		switch v := val.(type) {
		case []bool:
			return v
		default:
			rv := reflect.ValueOf(val)
			if rv.Kind() == reflect.Slice {
				arr := make([]bool, rv.Len())
				for i := 0; i < rv.Len(); i++ {
					vv := rv.Index(i).Interface()
					if bv, ok := vv.(bool); ok {
						arr[i] = bv
					}
				}
				return arr
			}
		}
	}
	if len(defval) > 0 {
		return defval[0]
	}
	return nil
}

func (prefs *Base) Stringify(pretty ...bool) string {
	visited := make(map[IBase]bool)
	var visit func(map[string]any) map[string]any
	visit = func(origin map[string]any) map[string]any {
		npairs := make(map[string]any, len(origin))
		for key, value := range origin {
			if prefs1, ok := value.(IBase); ok {
				if visited[prefs1] {
					npairs[key] = "<Recursive>"
				} else {
					visited[prefs1] = true
					sorigin := make(map[string]any)
					prefs1.Range(func(k string, v any) bool {
						sorigin[k] = v
						return true
					})
					npairs[key] = visit(sorigin)
				}
			} else if dict1, ok := value.(map[string]any); ok {
				npairs[key] = visit(dict1)
			} else if rv := reflect.ValueOf(value); rv.Kind() == reflect.Slice {
				nlist := make([]any, 0, rv.Len())
				for i := 0; i < rv.Len(); i++ {
					ele := rv.Index(i).Interface()
					if prefs2, ok := ele.(IBase); ok {
						if visited[prefs2] {
							nlist = append(nlist, "<Recursive>")
						} else {
							visited[prefs2] = true
							sorigin := make(map[string]any)
							prefs2.Range(func(k string, v any) bool {
								sorigin[k] = v
								return true
							})
							nlist = append(nlist, visit(sorigin))
						}
					} else if dict2, ok := ele.(map[string]any); ok {
						nlist = append(nlist, visit(dict2))
					} else {
						nlist = append(nlist, ele)
					}
				}
				npairs[key] = nlist
			} else {
				npairs[key] = value
			}
		}
		return npairs
	}

	prefs.mutex.RLock()
	npairs := visit(prefs.pairs)
	prefs.mutex.RUnlock()

	keys := make([]string, 0, len(npairs))
	for k := range npairs {
		keys = append(keys, k)
	}
	sort.Strings(keys)
	sorted := make(map[string]any, len(keys))
	for _, key := range keys {
		sorted[key] = npairs[key]
	}

	if len(pretty) > 0 && pretty[0] {
		ctt, err := json.MarshalIndent(sorted, "", "  ")
		if err != nil {
			fmt.Printf("XPrefs.Base.Stringify: marshal error: %v\n", err)
		}
		return string(ctt)
	} else {
		ctt, err := json.Marshal(sorted)
		if err != nil {
			fmt.Printf("XPrefs.Base.Stringify: marshal error: %v\n", err)
		}
		return string(ctt)
	}
}

func (prefs *Base) Parse(content []byte) bool {
	if len(content) == 0 {
		fmt.Printf("XPrefs.Base.Parse: nil content.\n")
		return false
	}
	var pairs map[string]any
	err := json.Unmarshal(content, &pairs)
	if err != nil {
		fmt.Printf("XPrefs.Base.Parse: unmarshal error: %v\n", err)
		return false
	}

	prefs.mutex.Lock()
	defer prefs.mutex.Unlock()

	if prefs.pairs == nil {
		prefs.pairs = make(map[string]any)
	}
	maps.Copy(prefs.pairs, pairs)
	return true
}

func (prefs *Base) Evaluate(expr string) string {
	pattern := regexp.MustCompile(`\$\{XPrefs\.([^}]+?)\}`)
	visited := make(map[string]bool)

	var repl func(string) string
	repl = func(match string) string {
		matched := pattern.FindStringSubmatch(match)[1]
		if strings.Contains(matched, "${") {
			return fmt.Sprintf("${Nested.%v}", match)
		}
		if visited[matched] {
			return fmt.Sprintf("${Recursive.XPrefs.%v}", matched)
		}
		visited[matched] = true
		defer delete(visited, matched)

		var value string
		if strings.Contains(matched, ".") {
			parts := strings.Split(matched, ".")
			current := (any)(prefs).(IBase)
			for i := 0; i < len(parts)-1; i++ {
				if !current.Exists(parts[i]) {
					return fmt.Sprintf("${Unknown.XPrefs.%v}", matched)
				}
				next := current.Get(parts[i])
				if next == nil {
					return fmt.Sprintf("${Unknown.XPrefs.%v}", matched)
				}
				if base, ok := next.(IBase); ok {
					current = base
				} else {
					return fmt.Sprintf("${Unknown.XPrefs.%v}", matched)
				}
			}
			value = current.GetString(parts[len(parts)-1])
		} else {
			if !prefs.Exists(matched) {
				return fmt.Sprintf("${Unknown.XPrefs.%v}", matched)
			}
			value = prefs.GetString(matched)
		}

		if value == "" {
			return fmt.Sprintf("${Unknown.XPrefs.%v}", matched)
		}

		return pattern.ReplaceAllStringFunc(value, repl)
	}

	return pattern.ReplaceAllStringFunc(expr, repl)
}
