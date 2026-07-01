// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XEvent

import (
	"reflect"

	"github.com/frameworkease/Universal.X.Utility/Go/XLog"
)

func register(manager *Manager, id int, proxy Callback, origin any, once bool, source string) bool {
	manager.Mutex.Lock()
	defer manager.Mutex.Unlock()

	handlers := manager.Handlers[id]

	if manager.Singleton && len(handlers) > 0 {
		XLog.Error("XEvent.%s: singleton mode doesn't allow multiple registrations, id=%v", source, id)
		return false
	}

	cptr := uintptr(reflect.ValueOf(origin).Pointer())
	for i := 0; i < len(handlers); i++ {
		if handlers[i].Origin == cptr {
			return false
		}
	}

	handler := new(Handler)
	handler.Func = proxy
	handler.Origin = cptr
	handler.Once = once
	handlers = append(handlers, handler)
	manager.Handlers[id] = handlers

	return true
}

func unregister(manager *Manager, id int, callback any, source string) bool {
	if callback == nil {
		XLog.Error("XEvent.%s: nil callback, id=%v", source, id)
		return false
	}

	cptr := uintptr(reflect.ValueOf(callback).Pointer())
	manager.Mutex.Lock()
	defer manager.Mutex.Unlock()

	handlers, ok := manager.Handlers[id]
	if !ok {
		return false
	}

	nhandlers := make([]*Handler, 0, len(handlers))
	for i := range handlers {
		if handlers[i].Origin != cptr {
			nhandlers = append(nhandlers, handlers[i])
		}
	}

	sig := len(nhandlers) != len(handlers)
	if sig {
		if len(nhandlers) == 0 {
			delete(manager.Handlers, id)
		} else {
			manager.Handlers[id] = nhandlers
		}
	}

	return sig
}

func RegisterT0(manager *Manager, id int, callback func(), once ...bool) bool {
	if callback == nil {
		XLog.Error("XEvent.RegisterT0: nil callback, id=%v", id)
		return false
	}
	proxy := func(args ...any) { callback() }
	return register(manager, id, proxy, callback, len(once) > 0 && once[0], "RegisterT0")
}

func RegisterT1[T1 any](manager *Manager, id int, callback func(T1), once ...bool) bool {
	if callback == nil {
		XLog.Error("XEvent.RegisterT1: nil callback, id=%v", id)
		return false
	}

	proxy := func(args ...any) {
		var arg1 T1
		if len(args) > 0 && args[0] != nil {
			if v, ok := args[0].(T1); ok {
				arg1 = v
			}
		}
		callback(arg1)
	}

	return register(manager, id, proxy, callback, len(once) > 0 && once[0], "RegisterT1")
}

func RegisterT2[T1, T2 any](manager *Manager, id int, callback func(T1, T2), once ...bool) bool {
	if callback == nil {
		XLog.Error("XEvent.RegisterT2: nil callback, id=%v", id)
		return false
	}

	proxy := func(args ...any) {
		var arg1 T1
		var arg2 T2
		if len(args) > 0 && args[0] != nil {
			if v, ok := args[0].(T1); ok {
				arg1 = v
			}
		}
		if len(args) > 1 && args[1] != nil {
			if v, ok := args[1].(T2); ok {
				arg2 = v
			}
		}
		callback(arg1, arg2)
	}

	return register(manager, id, proxy, callback, len(once) > 0 && once[0], "RegisterT2")
}

func RegisterT3[T1, T2, T3 any](manager *Manager, id int, callback func(T1, T2, T3), once ...bool) bool {
	if callback == nil {
		XLog.Error("XEvent.RegisterT3: nil callback, id=%v", id)
		return false
	}

	proxy := func(args ...any) {
		var arg1 T1
		var arg2 T2
		var arg3 T3
		if len(args) > 0 && args[0] != nil {
			if v, ok := args[0].(T1); ok {
				arg1 = v
			}
		}
		if len(args) > 1 && args[1] != nil {
			if v, ok := args[1].(T2); ok {
				arg2 = v
			}
		}
		if len(args) > 2 && args[2] != nil {
			if v, ok := args[2].(T3); ok {
				arg3 = v
			}
		}
		callback(arg1, arg2, arg3)
	}

	return register(manager, id, proxy, callback, len(once) > 0 && once[0], "RegisterT3")
}

func UnregisterT0(manager *Manager, id int, callback func()) bool {
	return unregister(manager, id, callback, "UnregisterT0")
}

func UnregisterT1[T1 any](manager *Manager, id int, callback func(T1)) bool {
	return unregister(manager, id, callback, "UnregisterT1")
}

func UnregisterT2[T1, T2 any](manager *Manager, id int, callback func(T1, T2)) bool {
	return unregister(manager, id, callback, "UnregisterT2")
}

func UnregisterT3[T1, T2, T3 any](manager *Manager, id int, callback func(T1, T2, T3)) bool {
	return unregister(manager, id, callback, "UnregisterT3")
}
