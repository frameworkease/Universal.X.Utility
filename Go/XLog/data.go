// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XLog

import (
	"fmt"
	"time"
)

type Data struct {
	Level   Levels
	Content any
	Args    []any
	Tag     Tag
	Time    time.Time
}

func (data *Data) Stringify() string {
	var str string
	switch val := data.Content.(type) {
	case string:
		if len(data.Args) > 0 {
			str = fmt.Sprintf(val, data.Args...)
		} else {
			str = val
		}
	default:
		if data.Content == nil {
			str = "<nil>"
		} else {
			str = fmt.Sprint(val)
		}
	}
	tag := data.Tag.Stringify()
	if tag != "" {
		return data.Time.Format("[01/02 15:04:05.000] ") + Labels[data.Level] + " " + tag + " " + str
	}
	return data.Time.Format("[01/02 15:04:05.000] ") + Labels[data.Level] + " " + str
}
