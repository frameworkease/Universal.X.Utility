// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XLoom

import (
	"fmt"
	"os"
	"runtime/debug"
)

func RunAsync(callback func(), restart ...bool) {
	if callback == nil {
		return
	}
	go func() {
		defer func() {
			if err := recover(); err != nil {
				fmt.Fprintf(os.Stderr, "XLoom.RunAsync: %v\n%s\n", err, debug.Stack())
				if len(restart) == 1 && restart[0] {
					RunAsync(callback, restart...)
				}
			}
		}()
		callback()
	}()
}

func RunAsyncT1[T1 any](callback func(T1), arg1 T1, restart ...bool) {
	if callback == nil {
		return
	}
	go func() {
		defer func() {
			if err := recover(); err != nil {
				fmt.Fprintf(os.Stderr, "XLoom.RunAsyncT1: %v\n%s\n", err, debug.Stack())
				if len(restart) == 1 && restart[0] {
					RunAsyncT1(callback, arg1, restart...)
				}
			}
		}()
		callback(arg1)
	}()
}

func RunAsyncT2[T1, T2 any](callback func(T1, T2), arg1 T1, arg2 T2, restart ...bool) {
	if callback == nil {
		return
	}
	go func() {
		defer func() {
			if err := recover(); err != nil {
				fmt.Fprintf(os.Stderr, "XLoom.RunAsyncT2: %v\n%s\n", err, debug.Stack())
				if len(restart) == 1 && restart[0] {
					RunAsyncT2(callback, arg1, arg2, restart...)
				}
			}
		}()
		callback(arg1, arg2)
	}()
}

func RunAsyncT3[T1, T2, T3 any](callback func(T1, T2, T3), arg1 T1, arg2 T2, arg3 T3, restart ...bool) {
	if callback == nil {
		return
	}
	go func() {
		defer func() {
			if err := recover(); err != nil {
				fmt.Fprintf(os.Stderr, "XLoom.RunAsyncT3: %v\n%s\n", err, debug.Stack())
				if len(restart) == 1 && restart[0] {
					RunAsyncT3(callback, arg1, arg2, arg3, restart...)
				}
			}
		}()
		callback(arg1, arg2, arg3)
	}()
}
