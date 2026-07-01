// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XLoom

import (
	"sync"
	"sync/atomic"

	"github.com/frameworkease/Universal.X.Utility/Go/XLog"
	"github.com/frameworkease/Universal.X.Utility/Go/XTime"
)

var (
	timerIncrement   atomic.Int64
	allTimers        [][]*timer
	newTimers        [][]*timer
	newTimerLocks    []sync.Mutex
	removeTimers     [][]int
	removeTimerLocks []sync.Mutex
)

type timer struct {
	id       int
	callback func()
	initial  int
	period   int
	trigger  int
	repeat   int
	panic    bool
}

func setupTimer(num int) {
	allTimers = make([][]*timer, num)
	newTimers = make([][]*timer, num)
	newTimerLocks = make([]sync.Mutex, num)
	removeTimers = make([][]int, num)
	removeTimerLocks = make([]sync.Mutex, num)
}

func updateTimer(loomID int) {
	if len(newTimers[loomID]) > 0 {
		newTimerLocks[loomID].Lock()
		allTimers[loomID] = append(allTimers[loomID], newTimers[loomID]...)
		newTimers[loomID] = newTimers[loomID][:0]
		newTimerLocks[loomID].Unlock()
	}
	if len(removeTimers[loomID]) > 0 {
		removeTimerLocks[loomID].Lock()
		for _, id := range removeTimers[loomID] {
			for idx, timer := range allTimers[loomID] {
				if id == timer.id {
					allTimers[loomID] = append(allTimers[loomID][:idx], allTimers[loomID][idx+1:]...)
					break
				}
			}
		}
		removeTimers[loomID] = removeTimers[loomID][:0]
		removeTimerLocks[loomID].Unlock()
	}
	if allTimers[loomID] != nil {
		nowTime := XTime.Milliseconds()
		for _, timer := range allTimers[loomID] {
			if timer.panic {
				if timer.repeat > 0 { // interval 发生 panic 不取消定时器
					timer.panic = false
					timer.repeat++
					timer.trigger = timer.initial + timer.period*timer.repeat
				} else { // timeout 发生 panic 则直接移除
					removeTimerLocks[loomID].Lock()
					removeTimers[loomID] = append(removeTimers[loomID], timer.id)
					removeTimerLocks[loomID].Unlock()
					continue
				}
			}
			if timer.trigger <= nowTime { // 因存在固定刷新间歇，可能会导致间歇调用的周期越来越长
				if timer.callback != nil {
					timer.panic = true
					timer.callback()
					timer.panic = false
				}
				if timer.repeat == 0 {
					removeTimerLocks[loomID].Lock()
					removeTimers[loomID] = append(removeTimers[loomID], timer.id)
					removeTimerLocks[loomID].Unlock()
				} else {
					timer.repeat++
					timer.trigger = timer.initial + timer.period*timer.repeat
				}
			}
		}
	}
}

func SetTimeout(callback func(), timeout int, loomID ...int) int {
	if callback == nil {
		XLog.Critical("XLoom.SetTimeout: callback can not be nil.")
		return -1
	}
	if timeout < 0 {
		XLog.Critical("XLoom.SetTimeout: timeout of %v can not be zero or negative.", timeout)
		return -1
	}
	lid := -1
	if len(loomID) == 1 {
		lid = loomID[0]
	} else {
		lid = ID()
	}
	if lid < 0 {
		XLog.Critical("XLoom.SetTimeout: loom id of %v can not be zero or negative.", lid)
		return -1
	}
	if lid >= loomCount {
		XLog.Critical("XLoom.SetTimeout: loom id of %v can not equals or greater than %v.", lid, Count())
		return -1
	}

	timer := &timer{
		id:       int(timerIncrement.Add(1)),
		callback: callback,
		initial:  XTime.Milliseconds(),
		period:   timeout,
		repeat:   0,
		trigger:  0,
	}
	timer.trigger = timer.initial + timer.period

	newTimerLocks[lid].Lock()
	newTimers[lid] = append(newTimers[lid], timer)
	newTimerLocks[lid].Unlock()
	return timer.id
}

func ClearTimeout(id int, loomID ...int) {
	lid := -1
	if len(loomID) == 1 {
		lid = loomID[0]
	} else {
		lid = ID()
	}
	if lid < 0 {
		XLog.Critical("XLoom.ClearTimeout: loom id of %v can not be zero or negative.", lid)
		return
	}
	if lid >= loomCount {
		XLog.Critical("XLoom.ClearTimeout: loom id of %v can not equals or greater than %v.", lid, Count())
		return
	}

	removeTimerLocks[lid].Lock()
	removeTimers[lid] = append(removeTimers[lid], id)
	removeTimerLocks[lid].Unlock()
}

func SetInterval(callback func(), interval int, loomID ...int) int {
	if callback == nil {
		XLog.Critical("XLoom.SetInterval: callback can not be nil.")
		return -1
	}
	if interval < 0 {
		XLog.Critical("XLoom.SetInterval: interval of %v can not be zero or negative.", interval)
		return -1
	}
	lid := -1
	if len(loomID) == 1 {
		lid = loomID[0]
	} else {
		lid = ID()
	}
	if lid < 0 {
		XLog.Critical("XLoom.SetInterval: loom id of %v can not be zero or negative.", lid)
		return -1
	}
	if lid >= loomCount {
		XLog.Critical("XLoom.SetInterval: loom id of %v can not equals or greater than %v.", lid, Count())
		return -1
	}

	timer := &timer{
		id:       int(timerIncrement.Add(1)),
		callback: callback,
		initial:  XTime.Milliseconds(),
		period:   interval,
		repeat:   1,
		trigger:  0,
	}
	timer.trigger = timer.initial + timer.period

	newTimerLocks[lid].Lock()
	newTimers[lid] = append(newTimers[lid], timer)
	newTimerLocks[lid].Unlock()
	return timer.id
}

func ClearInterval(id int, loomID ...int) { ClearTimeout(id, loomID...) }
