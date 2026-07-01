// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XLog

import (
	"fmt"
	"os"
	"slices"
	"sync"
)

type IAdapter interface {
	Name() string
	Setup()
	Write(data Data)
	Flush()
	Reset()
}

type adapterScope struct {
	mutex    sync.RWMutex
	adapters []IAdapter
}

var Adapter = &adapterScope{}

func (as *adapterScope) Setup(apt ...IAdapter) bool {
	if len(apt) == 0 {
		return false
	}
	as.mutex.Lock()
	defer as.mutex.Unlock()

	ok := true
	for _, ele := range apt {
		if ele == nil {
			ok = false
			continue
		}
		if slices.Contains(as.adapters, ele) {
			fmt.Fprintf(os.Stderr, "XLog.Adapter.Setup: duplicated adapter: %v.\n", ele.Name())
			ok = false
			continue
		}
		as.adapters = append(as.adapters, ele)
		ele.Setup()
		fmt.Printf("XLog.Adapter.Setup: registered adapter: %s.\n", ele.Name())
	}
	return ok
}

func (as *adapterScope) Reset(apt ...IAdapter) bool {
	as.mutex.Lock()
	defer as.mutex.Unlock()

	if len(apt) == 0 {
		cnt := len(as.adapters)
		for _, ele := range as.adapters {
			ele.Reset()
		}
		as.adapters = nil
		fmt.Printf("XLog.Adapter.Reset: unregistered %d adapter(s).\n", cnt)
	} else {
		ele := apt[0]
		i := slices.Index(as.adapters, ele)
		if i < 0 {
			return false
		}
		as.adapters = append(as.adapters[:i], as.adapters[i+1:]...)
		ele.Reset()
		fmt.Printf("XLog.Adapter.Reset: unregistered adapter: %v.\n", ele.Name())
	}
	return true
}

func (as *adapterScope) Range(callback func(apt IAdapter) bool) {
	if callback == nil {
		return
	}
	for _, apt := range as.adapters {
		if !callback(apt) {
			break
		}
	}
}

func (as *adapterScope) Write(data Data) {
	if len(as.adapters) == 0 {
		fmt.Println(data.Stringify())
	} else {
		for _, apt := range as.adapters {
			apt.Write(data)
		}
	}
}

func (as *adapterScope) Flush() {
	fmt.Printf("XLog.Adapter.Flush: performing flush with %d adapter(s).\n", len(as.adapters))
	as.mutex.RLock()
	defer as.mutex.RUnlock()

	for _, apt := range as.adapters {
		apt.Flush()
	}
}
