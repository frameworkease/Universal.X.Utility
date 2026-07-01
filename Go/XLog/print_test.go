// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XLog

import (
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestPrintAll(t *testing.T) {
	prints := []struct {
		level Levels
		print func(content any, args ...any)
	}{
		{LevelEmergency, Emergency},
		{LevelAlert, Alert},
		{LevelCritical, Critical},
		{LevelError, Error},
		{LevelWarn, Warn},
		{LevelNotice, Notice},
		{LevelInfo, Info},
		{LevelDebug, Debug},
	}

	defer ResetTestAdapter(t)()

	Adapter.Setup(&MyAdapter{name: "adapter1"})
	Adapter.Setup(&MyAdapter{name: "adapter2"})

	tag := Tag{}
	for _, print := range prints {
		print.print("test log", tag)
	}

	assert.Len(t, Adapter.adapters[0].(*MyAdapter).datas, 8)
	assert.Len(t, Adapter.adapters[1].(*MyAdapter).datas, 8)
}

func TestPrintTag(t *testing.T) {
	defer ResetTestAdapter(t)()

	Adapter.Setup(&MyAdapter{name: "adapter1"})

	var tag Tag
	tag.Set("key", "value")
	Info("test log", tag)

	data := Adapter.adapters[0].(*MyAdapter).datas[0]
	assert.Equal(t, "value", data.Tag.Get("key"))
}
