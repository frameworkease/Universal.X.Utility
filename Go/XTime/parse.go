// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XTime

import (
	"time"
)

func Parse[T int | int32 | int64](timestamp T) time.Time {
	switch v := any(timestamp).(type) {
	case int:
		return Initial().Add(time.Duration(v) * time.Second)
	case int32:
		return Initial().Add(time.Duration(v) * time.Second)
	case int64:
		return Initial().Add(time.Duration(v) * time.Millisecond)
	default:
		return Initial()
	}
}
