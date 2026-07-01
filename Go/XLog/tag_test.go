// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XLog

import (
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestTagCount(t *testing.T) {
	tag := Tag{}
	assert.Equal(t, 0, tag.Count())

	tag.Set("key1", "value1")
	assert.Equal(t, 1, tag.Count())
}

func TestTagSet(t *testing.T) {
	tag := Tag{}
	tag.Set("key1", "value1")
	assert.Equal(t, "value1", tag.Get("key1"))

	tag.Set("key2", "value2")
	assert.Equal(t, "value2", tag.Get("key2"))

	tag.Set("key1", "value1_updated")
	assert.Equal(t, "value1_updated", tag.Get("key1"))
}

func TestTagGet(t *testing.T) {
	tag := Tag{}
	assert.Equal(t, "", tag.Get("nonexistent"))

	tag.Set("key1", "value1")
	assert.Equal(t, "value1", tag.Get("key1"))
}

func TestTagRange(t *testing.T) {
	tag := Tag{}
	tag.Set("key1", "value1")
	tag.Set("key2", "value2")
	tag.Set("key3", "value3")
	var pairs []string
	tag.Range(func(key string, value string) bool {
		pairs = append(pairs, key, value)
		if key == "key2" {
			return false
		}
		return true
	})
	assert.Equal(t, []string{"key1", "value1", "key2", "value2"}, pairs)
}

func TestTagStringify(t *testing.T) {
	tag := Tag{}
	assert.Equal(t, "", tag.Stringify())

	tag.Set("key1", "value1")
	assert.Equal(t, "[key1=value1]", tag.Stringify())

	tag.Set("key2", "value2")
	assert.Equal(t, "[key1=value1, key2=value2]", tag.Stringify())
}
