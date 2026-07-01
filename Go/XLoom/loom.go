// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XLoom

import (
	"fmt"
	"os"
	"os/signal"
	"sync"
	"syscall"
	"time"

	"github.com/frameworkease/Universal.X.Utility/Go/XLog"
	"github.com/illumitacit/gostd/quit"
	"github.com/petermattis/goid"
)

var (
	loomInitMutex sync.Mutex
	loomPause     []bool
	loomPauseSig  []chan bool
	loomInitSig   []chan os.Signal
	loomCloseSig  []chan bool
	loomCloseWait sync.WaitGroup
	loomIDMap     = make(map[int64]int)
	loomIDMu      sync.Mutex
	loomCount     int
	loomTask      []chan func()
)

func Setup(count, step, queue int) {
	loomInitMutex.Lock()
	defer loomInitMutex.Unlock()

	if count <= 0 || step <= 0 || queue <= 0 {
		panic(fmt.Sprintf("XLoom.Setup: invalid parameters, count: %v, step: %v, queue: %v.", count, step, queue))
	}

	// 关闭所有线程。
	if len(loomCloseSig) > 0 {
		for _, ch := range loomCloseSig {
			ch <- true
		}
		loomCloseWait.Wait()
	}
	loomCloseWait = sync.WaitGroup{}

	loomCount = count

	loomTask = make([]chan func(), count)
	loomInitSig = make([]chan os.Signal, count)
	loomCloseSig = make([]chan bool, count)
	loomPause = make([]bool, count)
	loomPauseSig = make([]chan bool, count)

	for i := range count {
		loomTask[i] = make(chan func(), queue)
		loomInitSig[i] = make(chan os.Signal, 1)
		loomCloseSig[i] = make(chan bool, 1)
		loomPauseSig[i] = make(chan bool, 1)
	}

	setupTimer(count)

	wg := sync.WaitGroup{}
	for i := range count {
		wg.Add(1)

		doneOnce := sync.Once{}
		RunAsyncT1(func(loomID int) {
			initSig := loomInitSig[i]
			signal.Notify(initSig, syscall.SIGTERM, syscall.SIGINT)
			pauseSig := loomPauseSig[i]
			closeSig := loomCloseSig[i]

			loomCloseWait.Add(1)
			quit.GetWaiter().Add(1)
			defer func() {
				quit.GetWaiter().Done()
				loomCloseWait.Done()
			}()

			loomIDMu.Lock()
			loomIDMap[goid.Get()] = loomID
			loomIDMu.Unlock()

			updateTicker := time.NewTicker(time.Millisecond * time.Duration(step))
			defer updateTicker.Stop()

			doneOnce.Do(func() { // 确保只调用一次，否则recover后会重复调用
				wg.Done() // 确保线程启动完成
			})

			for {
				if loomPause[loomID] {
					select {
					case <-updateTicker.C:
					case val := <-pauseSig:
						XLog.Notice("XLoom.Loop(%v): receive signal of pause(%v).", loomID, val)
					case <-closeSig:
						XLog.Notice("XLoom.Loop(%v): receive signal of close.", loomID)
						return
					case sig, ok := <-initSig:
						if ok {
							XLog.Notice("XLoom.Loop(%v): receive signal of %v.", i, sig.String())
						} else {
							XLog.Notice("XLoom.Loop(%v): channel of signal is closed.", i)
						}
						return
					case <-quit.GetQuitChannel():
						XLog.Notice("XLoom.Loop(%v): receive signal of quit.", loomID)
						return
					}
				} else {
					select {
					case runIn, ok := <-loomTask[loomID]:
						if ok {
							runIn()
						} else {
							XLog.Error("XLoom.Loop(%v): get runin with ret false.", loomID)
						}
					case <-updateTicker.C:
						updateTimer(loomID)
					case val := <-pauseSig:
						XLog.Notice("XLoom.Loop(%v): receive signal of pause(%v).", loomID, val)
					case <-closeSig:
						XLog.Notice("XLoom.Loop(%v): receive signal of close.", loomID)
						return
					case sig, ok := <-initSig:
						if ok {
							XLog.Notice("XLoom.Loop(%v): receive signal of %v.", i, sig.String())
						} else {
							XLog.Notice("XLoom.Loop(%v): channel of signal is closed.", i)
						}
						return
					case <-quit.GetQuitChannel():
						XLog.Notice("XLoom.Loop(%v): receive signal of quit.", loomID)
						return
					}
				}
			}
		}, i, true)
	}

	XLog.Notice("XLoom.Setup: allocated %v loom(s).", count)
	loomCount = count
	wg.Wait()
}

func Reset() {
	loomInitMutex.Lock()
	defer loomInitMutex.Unlock()

	if len(loomCloseSig) > 0 {
		for _, ch := range loomCloseSig {
			ch <- true
		}
		loomCloseWait.Wait()
	}
	loomCloseWait = sync.WaitGroup{}

	loomCount = 0
	loomTask = nil
	loomInitSig = nil
	loomCloseSig = nil
	loomPause = nil
	loomPauseSig = nil
	loomIDMap = make(map[int64]int)
	loomIDMu = sync.Mutex{}
}

func Pause(loomID ...int) {
	if len(loomID) == 1 {
		lid := loomID[0]
		if lid < 0 {
			XLog.Error("XLoom.Pause: loom id of %v can not be zero or negative.", lid)
			return
		}
		if lid >= loomCount {
			XLog.Error("XLoom.Pause: loom id of %v can not equals or greater than %v.", lid, Count())
			return
		}
		loomPause[lid] = true
		loomPauseSig[lid] <- true
	} else {
		for lid := range loomPause {
			loomPause[lid] = true
			loomPauseSig[lid] <- true
		}
	}
}

func Resume(loomID ...int) {
	if len(loomID) == 1 {
		lid := loomID[0]
		if lid < 0 {
			XLog.Error("XLoom.Resume: loom id of %v can not be zero or negative.", lid)
			return
		}
		if lid >= loomCount {
			XLog.Error("XLoom.Resume: loom id of %v can not equals or greater than %v.", lid, Count())
			return
		}
		loomPause[lid] = false
		loomPauseSig[lid] <- false
	} else {
		for lid := range loomPause {
			loomPause[lid] = false
			loomPauseSig[lid] <- false
		}
	}
}

func RunIn(callback func(), loomID ...int) bool {
	if callback == nil {
		XLog.Error("XLoom.RunIn: callback can not be nil.")
		return false
	}
	lid := -1
	if len(loomID) == 1 {
		lid = loomID[0]
	} else {
		lid = 0
	}
	if lid < 0 {
		XLog.Error("XLoom.RunIn: loom id of %v can not be zero or negative.", lid)
		return false
	}
	if lid >= loomCount {
		XLog.Error("XLoom.RunIn: loom id of %v can not equals or greater than %v.", lid, Count())
		return false
	}
	ch := loomTask[lid]
	select {
	case ch <- callback:
		return true
	default:
		XLog.Error("XLoom.RunIn: too many runins of %v.", lid)
		return false
	}
}

func Count() int { return loomCount }

func ID(goroutineID ...int64) int {
	var gid int64
	if len(goroutineID) == 1 {
		gid = goroutineID[0]
	} else {
		gid = goid.Get()
	}
	if loomID, ok := loomIDMap[gid]; ok {
		return loomID
	}
	return -1
}
