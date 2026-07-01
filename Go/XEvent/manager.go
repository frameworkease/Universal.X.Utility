// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XEvent

import (
	"reflect"
	"slices"
	"sync"

	"github.com/frameworkease/Universal.X.Utility/Go/XLog"
)

type Callback func(args ...any)

type Handler struct {
	Func   Callback
	Origin uintptr
	Once   bool
}

type Manager struct {
	Mutex    sync.RWMutex
	Singleton bool
	Handlers map[int][]*Handler
}

func (manager *Manager) Register(id int, callback Callback, once ...bool) bool {
	if nil == callback {
		XLog.Error("XEvent.Manager.Register: nil callback, id=%v", id)
		return false
	}
	manager.Mutex.Lock()
	defer manager.Mutex.Unlock()

	if manager.Handlers == nil {
		manager.Handlers = make(map[int][]*Handler)
	}

	handlers, ok := manager.Handlers[id]
	if !ok {
		handlers = make([]*Handler, 0)
	}

	if manager.Singleton && len(handlers) > 0 {
		XLog.Error("XEvent.Manager.Register: singleton mode doesn't allow multiple registrations, id=%v", id)
		return false
	}

	cptr := uintptr(reflect.ValueOf(callback).Pointer())
	for i := 0; i < len(handlers); i++ {
		if handlers[i].Origin == cptr {
			return false
		}
	}

	handler := new(Handler)
	handler.Func = callback
	handler.Origin = cptr
	handler.Once = len(once) > 0 && once[0]
	handlers = append(handlers, handler)
	manager.Handlers[id] = handlers

	return true
}

func (manager *Manager) Unregister(id int, callback ...Callback) bool {
	manager.Mutex.Lock()
	defer manager.Mutex.Unlock()

	if manager.Handlers == nil {
		return false
	}

	handlers, ok := manager.Handlers[id]
	if !ok {
		return false
	}

	sig := false
	var hptr uintptr = 0
	if len(callback) > 0 && callback[0] != nil {
		hptr = uintptr(reflect.ValueOf(callback[0]).Pointer())
	}
	if hptr != 0 {
		nhandlers := slices.DeleteFunc(handlers, func(ele *Handler) bool {
			ok := ele.Origin == hptr
			return ok
		})
		sig = len(nhandlers) != len(handlers)
		if len(nhandlers) == 0 {
			delete(manager.Handlers, id)
		} else {
			manager.Handlers[id] = nhandlers
		}
	} else {
		sig = len(handlers) > 0
		if sig {
			delete(manager.Handlers, id)
		}
	}
	return sig
}

func (manager *Manager) Clear() {
	manager.Mutex.Lock()
	defer manager.Mutex.Unlock()

	if manager.Handlers == nil {
		return
	} else {
		manager.Handlers = make(map[int][]*Handler)
	}
}

func (manager *Manager) Notify(id int, args ...any) bool {
	manager.Mutex.RLock()
	if manager.Handlers == nil {
		manager.Mutex.RUnlock()
		return false
	} else {
		handlers, ok := manager.Handlers[id]
		manager.Mutex.RUnlock()
		if !ok || len(handlers) == 0 {
			return false
		}

		once := false
		for _, handler := range handlers {
			if handler != nil && handler.Func != nil {
				if handler.Once {
					once = true
				}
				handler.Func(args...)
			}
		}

		if once {
			manager.Mutex.Lock()
			defer manager.Mutex.Unlock()

			nhandlers, ok := manager.Handlers[id]
			if ok {
				onces := make(map[uintptr]bool)
				for _, handler := range handlers {
					if handler.Once {
						onces[handler.Origin] = true
					}
				}
				nhandlers = slices.DeleteFunc(nhandlers, func(ele *Handler) bool { return onces[ele.Origin] })
				if len(nhandlers) == 0 {
					delete(manager.Handlers, id)
				} else {
					manager.Handlers[id] = nhandlers
				}
			}
		}

		return true
	}
}
