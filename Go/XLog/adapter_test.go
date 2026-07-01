// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XLog

import (
	"fmt"
	"testing"

	"github.com/stretchr/testify/assert"
)

type MyAdapter struct {
	name   string
	datas  []Data
	bsetup bool
	breset bool
	bflush bool
}

func (m *MyAdapter) Name() string    { return m.name }
func (m *MyAdapter) Setup()          { m.bsetup = true }
func (m *MyAdapter) Reset()          { m.breset = true }
func (m *MyAdapter) Write(data Data) { m.datas = append(m.datas, data) }
func (m *MyAdapter) Flush()          { m.bflush = true }

func ResetTestAdapter(t *testing.T) func() {
	originalAdapters := Adapter.adapters

	Adapter.Reset()
	assert.Len(t, Adapter.adapters, 0)

	return func() {
		Adapter.mutex.Lock()
		defer Adapter.mutex.Unlock()

		Adapter.adapters = originalAdapters
	}
}

func TestAdapterRange(t *testing.T) {
	defer ResetTestAdapter(t)()

	Adapter.Setup(&MyAdapter{name: "adapter1"}, &MyAdapter{name: "adapter2"}, &MyAdapter{name: "adapter3"})

	t.Run("Full", func(t *testing.T) {
		names := make([]string, 0)
		Adapter.Range(func(adapter IAdapter) bool {
			names = append(names, adapter.Name())
			return true
		})
		assert.Len(t, names, 3)
		assert.Contains(t, names, "adapter1")
		assert.Contains(t, names, "adapter2")
		assert.Contains(t, names, "adapter3")
	})

	t.Run("Break", func(t *testing.T) {
		count := 0
		Adapter.Range(func(adapter IAdapter) bool {
			count++
			return count < 2
		})
		assert.Equal(t, 2, count)
	})
}

func TestAdapterManage(t *testing.T) {
	defer ResetTestAdapter(t)()

	apt1 := &MyAdapter{name: "adapter1"}
	apt2 := &MyAdapter{name: "adapter2"}

	{
		assert.True(t, Adapter.Setup(apt1))
		assert.False(t, Adapter.Setup(apt1))
		assert.True(t, apt1.bsetup)
		assert.Len(t, Adapter.adapters, 1)
		assert.Contains(t, Adapter.adapters, apt1)

		assert.True(t, Adapter.Setup(apt2))
		assert.True(t, apt2.bsetup)
		assert.Len(t, Adapter.adapters, 2)
		assert.Contains(t, Adapter.adapters, apt2)
	}

	{
		assert.True(t, Adapter.Reset(apt2))
		assert.False(t, Adapter.Reset(apt2))
		assert.True(t, apt2.breset)
		assert.Len(t, Adapter.adapters, 1)

		assert.True(t, Adapter.Reset())
		assert.True(t, apt1.breset)
		assert.Len(t, Adapter.adapters, 0)
	}
}

func TestAdapterWrite(t *testing.T) {
	defer ResetTestAdapter(t)()

	Adapter.Write(Data{Level: LevelInfo, Content: "test log"})

	apt1 := &MyAdapter{name: "adapter1"}
	apt2 := &MyAdapter{name: "adapter2"}
	Adapter.Setup(apt1)
	Adapter.Setup(apt2)

	for i := range 10 {
		Adapter.Write(Data{Level: LevelInfo, Content: fmt.Sprintf("test log %d", i)})
	}
	assert.Len(t, apt1.datas, 10)
	assert.Len(t, apt2.datas, 10)
}

func TestAdapterFlush(t *testing.T) {
	defer ResetTestAdapter(t)()

	apt1 := &MyAdapter{name: "adapter1"}
	apt2 := &MyAdapter{name: "adapter2"}
	Adapter.Setup(apt1)
	Adapter.Setup(apt2)

	Adapter.Flush()
	assert.True(t, apt1.bflush)
	assert.True(t, apt2.bflush)
}
