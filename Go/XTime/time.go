// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XTime

import (
	"time"
)

var initial = time.Date(1970, 1, 1, 0, 0, 0, 0, time.Local)

func Initial() time.Time { return initial }

func Current() time.Time { return time.Now() }

func Seconds() int {
	now := time.Now()
	return int(now.Sub(Initial()).Seconds())
}

func Milliseconds() int {
	now := time.Now()
	return int(now.Sub(Initial()).Milliseconds())
}

func Microseconds() int {
	now := time.Now()
	return int(now.Sub(Initial()).Microseconds())
}
