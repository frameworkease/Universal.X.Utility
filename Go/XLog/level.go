// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XLog

type Levels int8

const (
	LevelUnknown Levels = iota
	LevelEmergency
	LevelAlert
	LevelCritical
	LevelError
	LevelWarn
	LevelNotice
	LevelInfo
	LevelDebug
)

var Labels = [LevelDebug + 1]string{
	"[U]", // Unknown
	"[M]", // Emergency
	"[A]", // Alert
	"[C]", // Critical
	"[E]", // Error
	"[W]", // Warn
	"[N]", // Notice
	"[I]", // Info
	"[D]", // Debug
}
