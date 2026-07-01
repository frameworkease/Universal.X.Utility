// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XApp

import (
	"github.com/frameworkease/Universal.X.Utility/Go/XEvent"
)

type Events uint8

const (
	EventOnAwake Events = iota
	EventOnStart
	EventOnStop
)

var Event = &XEvent.Manager{}
