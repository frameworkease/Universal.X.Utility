// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XLog

import (
	"fmt"
	"strings"
)

type Tag struct {
	pairs []string
}

func (tag *Tag) Count() int { return len(tag.pairs) / 2 }

func (tag *Tag) Set(key string, value string) {
	for i := 0; i < len(tag.pairs); i += 2 {
		if tag.pairs[i] == key {
			tag.pairs[i+1] = value
			return
		}
	}
	tag.pairs = append(tag.pairs, key, value)
}

func (tag *Tag) Get(key string) string {
	for i := 0; i < len(tag.pairs); i += 2 {
		if tag.pairs[i] == key {
			return tag.pairs[i+1]
		}
	}
	return ""
}

func (tag *Tag) Range(callback func(key string, value string) bool) {
	if callback == nil || len(tag.pairs) == 0 {
		return
	}
	for i := 0; i < len(tag.pairs); i += 2 {
		if !callback(tag.pairs[i], tag.pairs[i+1]) {
			break
		}
	}
}

func (tag *Tag) Stringify() string {
	if len(tag.pairs) == 0 {
		return ""
	}
	var builder strings.Builder
	builder.WriteString("[")
	first := true
	for i := 0; i < len(tag.pairs); i += 2 {
		if !first {
			builder.WriteString(", ")
		} else {
			first = false
		}
		fmt.Fprintf(&builder, "%s=%s", tag.pairs[i], tag.pairs[i+1])
	}
	builder.WriteString("]")
	return builder.String()
}
