// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XLog

import (
	"time"
)

func resolvePrint(args []any) (Tag, []any) {
	if len(args) > 0 {
		if arg0, ok := args[0].(Tag); ok {
			return arg0, args[1:]
		}
	}
	return Tag{}, args
}

func Print(level Levels, tag Tag, content any, args ...any) {
	Adapter.Write(Data{Level: level, Content: content, Tag: tag, Args: args, Time: time.Now()})
}

func Emergency(content any, args ...any) {
	tag, args := resolvePrint(args)
	Print(LevelEmergency, tag, content, args...)
}

func Alert(content any, args ...any) {
	tag, args := resolvePrint(args)
	Print(LevelAlert, tag, content, args...)
}

func Critical(content any, args ...any) {
	tag, args := resolvePrint(args)
	Print(LevelCritical, tag, content, args...)
}

func Error(content any, args ...any) {
	tag, args := resolvePrint(args)
	Print(LevelError, tag, content, args...)
}

func Warn(content any, args ...any) {
	tag, args := resolvePrint(args)
	Print(LevelWarn, tag, content, args...)
}

func Notice(content any, args ...any) {
	tag, args := resolvePrint(args)
	Print(LevelNotice, tag, content, args...)
}

func Info(content any, args ...any) {
	tag, args := resolvePrint(args)
	Print(LevelInfo, tag, content, args...)
}

func Debug(content any, args ...any) {
	tag, args := resolvePrint(args)
	Print(LevelDebug, tag, content, args...)
}
