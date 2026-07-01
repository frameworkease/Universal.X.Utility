// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XTime

import (
	"time"
)

func Stringify[T int | int32 | int64 | time.Time](tm T, format ...string) string {
	var t time.Time
	var f string
	if len(format) > 0 && format[0] != "" {
		f = format[0]
	}
	switch v := any(tm).(type) {
	case int:
		t = Initial().Add(time.Duration(v) * time.Second)
		if f == "" {
			f = "2006-01-02 15:04:05"
		}
	case int32:
		t = Initial().Add(time.Duration(v) * time.Second)
		if f == "" {
			f = "2006-01-02 15:04:05"
		}
	case int64:
		t = Initial().Add(time.Duration(v) * time.Millisecond)
		if f == "" {
			f = "2006-01-02 15:04:05.000"
		}
	case time.Time:
		t = v
		if f == "" {
			f = "2006-01-02 15:04:05.000"
		}
	}
	return t.Format(f)
}
