// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XLog

import (
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestDataStringify(t *testing.T) {
	type MyStruct struct {
		Name string
		Age  int
	}

	tests := []struct {
		level   Levels
		content any
		args    []any
		tag     Tag
		want    string
	}{
		{LevelInfo, nil, nil, Tag{}, "[01/01 00:00:00.000] [I] <nil>"},
		{LevelInfo, "%v %s", []any{"test", "format"}, Tag{}, "[01/01 00:00:00.000] [I] test format"},
		{LevelInfo, struct {
			Arg0 string
			Arg1 string
		}{Arg0: "test", Arg1: "struct"}, nil, Tag{}, "[01/01 00:00:00.000] [I] {test struct}"},
		{LevelInfo, "test tag", nil, Tag{pairs: []string{"key1", "value1", "key2", "value2"}}, "[01/01 00:00:00.000] [I] [key1=value1, key2=value2] test tag"},
		{LevelEmergency, "test emergency", nil, Tag{}, "[01/01 00:00:00.000] [M] test emergency"},
		{LevelAlert, "test alert", nil, Tag{}, "[01/01 00:00:00.000] [A] test alert"},
		{LevelCritical, "test critical", nil, Tag{}, "[01/01 00:00:00.000] [C] test critical"},
		{LevelError, "test error", nil, Tag{}, "[01/01 00:00:00.000] [E] test error"},
		{LevelWarn, "test warn", nil, Tag{}, "[01/01 00:00:00.000] [W] test warn"},
		{LevelNotice, "test notice", nil, Tag{}, "[01/01 00:00:00.000] [N] test notice"},
		{LevelInfo, "test info", nil, Tag{}, "[01/01 00:00:00.000] [I] test info"},
		{LevelDebug, "test debug", nil, Tag{}, "[01/01 00:00:00.000] [D] test debug"},
	}

	for _, tt := range tests {
		data := Data{
			Level:   tt.level,
			Content: tt.content,
			Args:    tt.args,
			Tag:     tt.tag,
		}
		result := data.Stringify()
		assert.Equal(t, tt.want, result)
	}
}
