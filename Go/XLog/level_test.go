// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XLog

import (
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestTypes(t *testing.T) {
	tests := []struct {
		name  string
		level Levels
		want  int8
	}{
		{"Unknown", LevelUnknown, 0},
		{"Emergency", LevelEmergency, 1},
		{"Alert", LevelAlert, 2},
		{"Critical", LevelCritical, 3},
		{"Error", LevelError, 4},
		{"Warn", LevelWarn, 5},
		{"Notice", LevelNotice, 6},
		{"Info", LevelInfo, 7},
		{"Debug", LevelDebug, 8},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) { assert.Equal(t, tt.want, int8(tt.level)) })
	}
}

func TestLabels(t *testing.T) {
	expected := []string{"[U]", "[M]", "[A]", "[C]", "[E]", "[W]", "[N]", "[I]", "[D]"}

	assert.Equal(t, len(expected), len(Labels))

	for i, want := range expected {
		assert.Equal(t, want, Labels[i])
	}

	levels := []Levels{LevelUnknown, LevelEmergency, LevelAlert, LevelCritical, LevelError, LevelWarn, LevelNotice, LevelInfo, LevelDebug}
	for i, level := range levels {
		assert.Equal(t, expected[i], Labels[level])
	}
}
