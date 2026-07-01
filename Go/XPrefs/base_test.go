// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XPrefs

import (
	"encoding/json"
	"fmt"
	"testing"

	"github.com/stretchr/testify/assert"
)

var sharedPrefs = &Base{}
var ignoreSetup = false

func TestSet(t *testing.T) {
	if ignoreSetup {
		return
	}
	sharedPrefs = &Base{}

	t.Run("Basic", func(t *testing.T) {
		assert.Equal(t, sharedPrefs, sharedPrefs.Set("int", 42))
		assert.Equal(t, 42, sharedPrefs.pairs["int"])

		sharedPrefs.Set("int8", int8(8))
		assert.Equal(t, int8(8), sharedPrefs.pairs["int8"])

		sharedPrefs.Set("int16", int16(16))
		assert.Equal(t, int16(16), sharedPrefs.pairs["int16"])

		sharedPrefs.Set("int32", int32(32))
		assert.Equal(t, int32(32), sharedPrefs.pairs["int32"])

		sharedPrefs.Set("int64", int64(64))
		assert.Equal(t, int64(64), sharedPrefs.pairs["int64"])

		sharedPrefs.Set("float32", float32(3.14))
		assert.Equal(t, float32(3.14), sharedPrefs.pairs["float32"])

		sharedPrefs.Set("float64", float64(3.14159))
		assert.Equal(t, float64(3.14159), sharedPrefs.pairs["float64"])

		sharedPrefs.Set("string", "1000")
		assert.Equal(t, "1000", sharedPrefs.pairs["string"])

		sharedPrefs.Set("bool", true)
		assert.Equal(t, true, sharedPrefs.pairs["bool"])

		nested := &Base{}
		nested.Set("id", 1)
		nested.Set("name", "foo")
		sharedPrefs.Set("nested", nested)
		assert.Equal(t, nested, sharedPrefs.pairs["nested"])
	})

	t.Run("Array", func(t *testing.T) {
		sharedPrefs.Set("ints", []int{1, 2, 3})
		assert.Equal(t, []int{1, 2, 3}, sharedPrefs.pairs["ints"])

		sharedPrefs.Set("int8s", []int8{1, 2, 3})
		assert.Equal(t, []int8{1, 2, 3}, sharedPrefs.pairs["int8s"])

		sharedPrefs.Set("int16s", []int16{1, 2, 3})
		assert.Equal(t, []int16{1, 2, 3}, sharedPrefs.pairs["int16s"])

		sharedPrefs.Set("int32s", []int32{1, 2, 3})
		assert.Equal(t, []int32{1, 2, 3}, sharedPrefs.pairs["int32s"])

		sharedPrefs.Set("int64s", []int64{1, 2, 3})
		assert.Equal(t, []int64{1, 2, 3}, sharedPrefs.pairs["int64s"])

		sharedPrefs.Set("float32s", []float32{1.1, 2.2, 3.3})
		assert.Equal(t, []float32{1.1, 2.2, 3.3}, sharedPrefs.pairs["float32s"])

		sharedPrefs.Set("float64s", []float64{1.1, 2.2, 3.3})
		assert.Equal(t, []float64{1.1, 2.2, 3.3}, sharedPrefs.pairs["float64s"])

		sharedPrefs.Set("strings", []string{"1001", "1002", "1003"})
		assert.Equal(t, []string{"1001", "1002", "1003"}, sharedPrefs.pairs["strings"])

		sharedPrefs.Set("bools", []bool{true, false, true})
		assert.Equal(t, []bool{true, false, true}, sharedPrefs.pairs["bools"])

		nesteds := make([]IBase, 3)
		for i := range 3 {
			nested := &Base{}
			nested.Set("id", i)
			nested.Set("name", fmt.Sprintf("foo%d", i))
			nesteds[i] = nested
		}
		sharedPrefs.Set("nesteds", nesteds)
		assert.Equal(t, nesteds, sharedPrefs.pairs["nesteds"])
	})
}

func TestDelete(t *testing.T) {
	TestSet(t)

	assert.Equal(t, sharedPrefs, sharedPrefs.Delete("string"))
	_, exist := sharedPrefs.pairs["string"]
	assert.False(t, exist)

	sharedPrefs.Delete("nonexistent")
	_, exist = sharedPrefs.pairs["nonexistent"]
	assert.False(t, exist)
}

func TestExists(t *testing.T) {
	TestSet(t)

	assert.True(t, sharedPrefs.Exists("string"))
	assert.False(t, sharedPrefs.Exists("nonexistent"))
}

func TestRange(t *testing.T) {
	TestSet(t)

	t.Run("All", func(t *testing.T) {
		sharedPrefs.Range(func(key string, value any) bool {
			assert.Equal(t, sharedPrefs.pairs[key], value)
			return true
		})
	})

	t.Run("Break", func(t *testing.T) {
		var count = 0
		sharedPrefs.Range(func(key string, value any) bool {
			count++
			return count < 2
		})
		assert.Equal(t, 2, count)
	})
}

func TestGet(t *testing.T) {
	TestSet(t)

	assert.Equal(t, 42, sharedPrefs.Get("int"))
	assert.Equal(t, int8(8), sharedPrefs.Get("int8"))
	assert.Equal(t, int16(16), sharedPrefs.Get("int16"))
	assert.Equal(t, int32(32), sharedPrefs.Get("int32"))
	assert.Equal(t, int64(64), sharedPrefs.Get("int64"))
	assert.Equal(t, float32(3.14), sharedPrefs.Get("float32"))
	assert.Equal(t, float64(3.14159), sharedPrefs.Get("float64"))
	assert.Equal(t, "1000", sharedPrefs.Get("string"))
	assert.Equal(t, true, sharedPrefs.Get("bool"))

	nested := sharedPrefs.Get("nested").(*Base)
	assert.Equal(t, 1, nested.Get("id"))
	assert.Equal(t, "foo", nested.Get("name"))

	assert.Equal(t, "defval", sharedPrefs.Get("nonexistent", "defval"))
	assert.Equal(t, "defval", sharedPrefs.Get("", "defval"))
	assert.Equal(t, nil, sharedPrefs.Get(""))
}

func TestGets(t *testing.T) {
	TestSet(t)

	ints := sharedPrefs.Gets("ints")
	assert.Equal(t, 1, ints[0])
	assert.Equal(t, 2, ints[1])
	assert.Equal(t, 3, ints[2])

	int8s := sharedPrefs.Gets("int8s")
	assert.Equal(t, int8(1), int8s[0])
	assert.Equal(t, int8(2), int8s[1])
	assert.Equal(t, int8(3), int8s[2])

	int16s := sharedPrefs.Gets("int16s")
	assert.Equal(t, int16(1), int16s[0])
	assert.Equal(t, int16(2), int16s[1])
	assert.Equal(t, int16(3), int16s[2])

	int32s := sharedPrefs.Gets("int32s")
	assert.Equal(t, int32(1), int32s[0])
	assert.Equal(t, int32(2), int32s[1])
	assert.Equal(t, int32(3), int32s[2])

	int64s := sharedPrefs.Gets("int64s")
	assert.Equal(t, int64(1), int64s[0])
	assert.Equal(t, int64(2), int64s[1])
	assert.Equal(t, int64(3), int64s[2])

	float32s := sharedPrefs.Gets("float32s")
	assert.Equal(t, float32(1.1), float32s[0])
	assert.Equal(t, float32(2.2), float32s[1])
	assert.Equal(t, float32(3.3), float32s[2])

	float64s := sharedPrefs.Gets("float64s")
	assert.Equal(t, float64(1.1), float64s[0])
	assert.Equal(t, float64(2.2), float64s[1])
	assert.Equal(t, float64(3.3), float64s[2])

	strings := sharedPrefs.Gets("strings")
	assert.Equal(t, "1001", strings[0])
	assert.Equal(t, "1002", strings[1])
	assert.Equal(t, "1003", strings[2])

	bools := sharedPrefs.Gets("bools")
	assert.Equal(t, true, bools[0])
	assert.Equal(t, false, bools[1])
	assert.Equal(t, true, bools[2])

	nesteds := sharedPrefs.Get("nesteds").([]IBase)
	assert.Equal(t, nesteds[0].Get("id"), 0)
	assert.Equal(t, nesteds[0].Get("name"), "foo0")
	assert.Equal(t, nesteds[1].Get("id"), 1)
	assert.Equal(t, nesteds[1].Get("name"), "foo1")
	assert.Equal(t, nesteds[2].Get("id"), 2)
	assert.Equal(t, nesteds[2].Get("name"), "foo2")

	assert.Equal(t, []any{99, 100}, sharedPrefs.Gets("nonexistent", []any{99, 100}))
	assert.Equal(t, []any{99, 100}, sharedPrefs.Gets("", []any{99, 100}))
	assert.Nil(t, sharedPrefs.Gets(""))
}

func TestGetInt(t *testing.T) {
	TestSet(t)

	assert.Equal(t, 42, sharedPrefs.GetInt("int"))
	assert.Equal(t, 8, sharedPrefs.GetInt("int8"))
	assert.Equal(t, 16, sharedPrefs.GetInt("int16"))
	assert.Equal(t, 32, sharedPrefs.GetInt("int32"))
	assert.Equal(t, 64, sharedPrefs.GetInt("int64"))
	assert.Equal(t, 3, sharedPrefs.GetInt("float32"))
	assert.Equal(t, 3, sharedPrefs.GetInt("float64"))
	assert.Equal(t, 1000, sharedPrefs.GetInt("string"))
	assert.Equal(t, 1, sharedPrefs.GetInt("bool"))
	assert.Equal(t, 99, sharedPrefs.GetInt("nonexistent", 99))
	assert.Equal(t, 99, sharedPrefs.GetInt("", 99))
	assert.Equal(t, 0, sharedPrefs.GetInt(""))
}

func TestGetInts(t *testing.T) {
	TestSet(t)

	assert.Equal(t, []int{1, 2, 3}, sharedPrefs.GetInts("ints"))
	assert.Equal(t, []int{1, 2, 3}, sharedPrefs.GetInts("int8s"))
	assert.Equal(t, []int{1, 2, 3}, sharedPrefs.GetInts("int16s"))
	assert.Equal(t, []int{1, 2, 3}, sharedPrefs.GetInts("int32s"))
	assert.Equal(t, []int{1, 2, 3}, sharedPrefs.GetInts("int64s"))
	assert.Equal(t, []int{1, 2, 3}, sharedPrefs.GetInts("float32s"))
	assert.Equal(t, []int{1, 2, 3}, sharedPrefs.GetInts("float64s"))
	assert.Equal(t, []int{1001, 1002, 1003}, sharedPrefs.GetInts("strings"))
	assert.Equal(t, []int{1, 0, 1}, sharedPrefs.GetInts("bools"))
	assert.Equal(t, []int{99, 100}, sharedPrefs.GetInts("nonexistent", []int{99, 100}))
	assert.Equal(t, []int{99, 100}, sharedPrefs.GetInts("", []int{99, 100}))
	assert.Nil(t, sharedPrefs.GetInts(""))
}

func TestGetFloat(t *testing.T) {
	TestSet(t)

	assert.Equal(t, float32(42), sharedPrefs.GetFloat("int"))
	assert.Equal(t, float32(8), sharedPrefs.GetFloat("int8"))
	assert.Equal(t, float32(16), sharedPrefs.GetFloat("int16"))
	assert.Equal(t, float32(32), sharedPrefs.GetFloat("int32"))
	assert.Equal(t, float32(64), sharedPrefs.GetFloat("int64"))
	assert.Equal(t, float32(3.14), sharedPrefs.GetFloat("float32"))
	assert.Equal(t, float32(3.14159), sharedPrefs.GetFloat("float64"))
	assert.Equal(t, float32(1000), sharedPrefs.GetFloat("string"))
	assert.Equal(t, float32(1), sharedPrefs.GetFloat("bool"))
	assert.Equal(t, float32(99), sharedPrefs.GetFloat("nonexistent", float32(99)))
	assert.Equal(t, float32(99), sharedPrefs.GetFloat("", float32(99)))
	assert.Equal(t, float32(0), sharedPrefs.GetFloat(""))
}

func TestGetFloats(t *testing.T) {
	TestSet(t)

	assert.Equal(t, []int{1, 2, 3}, sharedPrefs.GetInts("ints"))
	assert.Equal(t, []float32{1, 2, 3}, sharedPrefs.GetFloats("int8s"))
	assert.Equal(t, []float32{1, 2, 3}, sharedPrefs.GetFloats("int16s"))
	assert.Equal(t, []float32{1, 2, 3}, sharedPrefs.GetFloats("int32s"))
	assert.Equal(t, []float32{1, 2, 3}, sharedPrefs.GetFloats("int64s"))
	assert.Equal(t, []float32{1.1, 2.2, 3.3}, sharedPrefs.GetFloats("float32s"))
	assert.Equal(t, []float32{1.1, 2.2, 3.3}, sharedPrefs.GetFloats("float64s"))
	assert.Equal(t, []float32{1001, 1002, 1003}, sharedPrefs.GetFloats("strings"))
	assert.Equal(t, []float32{1, 0, 1}, sharedPrefs.GetFloats("bools"))
	assert.Equal(t, []float32{99, 100}, sharedPrefs.GetFloats("nonexistent", []float32{99, 100}))
	assert.Equal(t, []float32{99, 100}, sharedPrefs.GetFloats("", []float32{99, 100}))
	assert.Nil(t, sharedPrefs.GetFloats(""))
}

func TestGetString(t *testing.T) {
	TestSet(t)

	assert.Equal(t, "42", sharedPrefs.GetString("int"))
	assert.Equal(t, "8", sharedPrefs.GetString("int8"))
	assert.Equal(t, "16", sharedPrefs.GetString("int16"))
	assert.Equal(t, "32", sharedPrefs.GetString("int32"))
	assert.Equal(t, "64", sharedPrefs.GetString("int64"))
	assert.Equal(t, "3.14", sharedPrefs.GetString("float32"))
	assert.Equal(t, "3.14159", sharedPrefs.GetString("float64"))
	assert.Equal(t, "1000", sharedPrefs.GetString("string"))
	assert.Equal(t, "true", sharedPrefs.GetString("bool"))
	assert.Equal(t, "99", sharedPrefs.GetString("nonexistent", "99"))
	assert.Equal(t, "defval", sharedPrefs.GetString("", "defval"))
	assert.Equal(t, "", sharedPrefs.GetString(""))
}

func TestGetStrings(t *testing.T) {
	TestSet(t)

	assert.Equal(t, []string{"1", "2", "3"}, sharedPrefs.GetStrings("ints"))
	assert.Equal(t, []string{"1", "2", "3"}, sharedPrefs.GetStrings("int8s"))
	assert.Equal(t, []string{"1", "2", "3"}, sharedPrefs.GetStrings("int16s"))
	assert.Equal(t, []string{"1", "2", "3"}, sharedPrefs.GetStrings("int32s"))
	assert.Equal(t, []string{"1", "2", "3"}, sharedPrefs.GetStrings("int64s"))
	assert.Equal(t, []string{"1.1", "2.2", "3.3"}, sharedPrefs.GetStrings("float32s"))
	assert.Equal(t, []string{"1.1", "2.2", "3.3"}, sharedPrefs.GetStrings("float64s"))
	assert.Equal(t, []string{"1001", "1002", "1003"}, sharedPrefs.GetStrings("strings"))
	assert.Equal(t, []string{"true", "false", "true"}, sharedPrefs.GetStrings("bools"))
	assert.Equal(t, []string{"99", "100"}, sharedPrefs.GetStrings("nonexistent", []string{"99", "100"}))
	assert.Equal(t, []string{"99", "100"}, sharedPrefs.GetStrings("", []string{"99", "100"}))
	assert.Nil(t, sharedPrefs.GetStrings(""))
}

func TestGetBool(t *testing.T) {
	TestSet(t)

	assert.Equal(t, true, sharedPrefs.GetBool("bool"))
	assert.Equal(t, true, sharedPrefs.GetBool("nonexistent", true))
	assert.Equal(t, true, sharedPrefs.GetBool("", true))
	assert.Equal(t, false, sharedPrefs.GetBool(""))
}

func TestGetBools(t *testing.T) {
	TestSet(t)

	assert.Equal(t, []bool{true, false, true}, sharedPrefs.GetBools("bools"))
	assert.Equal(t, []bool{false, false, true}, sharedPrefs.GetBools("nonexistent", []bool{false, false, true}))
	assert.Equal(t, []bool{true, true, true}, sharedPrefs.GetBools("", []bool{true, true, true}))
	assert.Nil(t, sharedPrefs.GetBools(""))
}

func TestStringify(t *testing.T) {
	TestSet(t)

	t.Run("Compact", func(t *testing.T) {
		result := sharedPrefs.Stringify()
		assert.NotContains(t, result, "\n")
	})

	t.Run("Pretty", func(t *testing.T) {
		result := sharedPrefs.Stringify(true)
		assert.Contains(t, result, "\n")
	})
}

func TestParse(t *testing.T) {
	TestSet(t)

	ignoreSetup = true
	defer func() { ignoreSetup = false }()

	content, err := json.Marshal(sharedPrefs.pairs)
	assert.NoError(t, err)
	assert.True(t, sharedPrefs.Parse(content))

	TestGetInt(t)
	TestGetInts(t)
	TestGetFloat(t)
	TestGetFloats(t)
	TestGetString(t)
	TestGetStrings(t)
	TestGetBool(t)
	TestGetBools(t)
}

func TestEvaluate(t *testing.T) {
	TestSet(t)

	assert.Equal(t, "1000 42 foo ${Unknown.XPrefs.nonexistent}", sharedPrefs.Evaluate("${XPrefs.string} ${XPrefs.int} ${XPrefs.nested.name} ${XPrefs.nonexistent}"))
	sharedPrefs.Set("empty", "")
	assert.Equal(t, "${Unknown.XPrefs.empty}", sharedPrefs.Evaluate("${XPrefs.empty}"))
	assert.Equal(t, "${Unknown.XPrefs.nonexistent}", sharedPrefs.Evaluate("${XPrefs.nonexistent}"))
	assert.Equal(t, "${Nested.${XPrefs.outer${XPrefs.inner}}}", sharedPrefs.Evaluate("${XPrefs.outer${XPrefs.inner}}"))
	sharedPrefs.Set("recursive1", "${XPrefs.recursive2}")
	sharedPrefs.Set("recursive2", "${XPrefs.recursive1}")
	assert.Equal(t, "${Recursive.XPrefs.recursive1}", sharedPrefs.Evaluate("${XPrefs.recursive1}"))
}
