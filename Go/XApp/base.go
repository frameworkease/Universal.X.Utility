// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XApp

import (
	"sync"
)

type IBase interface {
	Awake() bool
	Start()
	Stop(wg *sync.WaitGroup)
}
