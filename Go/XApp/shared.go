// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XApp

import (
	"os"
	"os/signal"
	"sync"
	"syscall"

	"github.com/frameworkease/Universal.X.Utility/Go/XLog"
	"github.com/illumitacit/gostd/quit"
)

var (
	instance IBase
	runOnce  sync.Once
	quitOnce sync.Once
)

func Instance[T IBase]() T { return instance.(T) }

func Run(application IBase) {
	runOnce.Do(func() {
		if application == nil {
			panic("XApp.Run: application is nil.")
		}
		instance = application

		if !application.Awake() {
			panic("XApp.Run: application awake failed.")
		}
		Event.Notify(int(EventOnAwake))
		XLog.Notice("XApp.Run: application has been awaked.")

		application.Start()
		Event.Notify(int(EventOnStart))
		XLog.Notice("XApp.Run: application has been started.")

		defer func() {
			wg := &sync.WaitGroup{}
			application.Stop(wg)
			Event.Notify(int(EventOnStop), wg)
			wg.Wait()
			XLog.Notice("XApp.Run: application has been stopped.")
		}()

		for {
			defer func() {
				quit.GetWaiter().Wait()
			}()
			ch := make(chan os.Signal, 1)
			signal.Notify(ch, syscall.SIGTERM, syscall.SIGINT)
			for {
				select {
				case sig, ok := <-ch:
					if ok {
						XLog.Notice("XApp.Run: receive signal of %v.", sig.String())
					} else {
						XLog.Notice("XApp.Run: channel of signal is closed.")
					}
					return
				case <-quit.GetQuitChannel():
					XLog.Notice("XApp.Run: receive signal of quit.")
					return
				}
			}
		}
	})
}

func Quit() { quitOnce.Do(func() { quit.BroadcastShutdown() }) }
