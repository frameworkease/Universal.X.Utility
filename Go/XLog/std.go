// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XLog

import (
	"fmt"
	"io"
	"os"

	"github.com/shiena/ansicolor"
)

func stdBrush(color string) func(string) string {
	pre := "\033["
	reset := "\033[0m"
	return func(text string) string { return pre + color + "m" + text + reset }
}

var stdBrushes = []func(string) string{
	stdBrush("1;39"), // Emergency          black
	stdBrush("1;36"), // Alert              cyan
	stdBrush("1;35"), // Critical           magenta
	stdBrush("1;31"), // Error              red
	stdBrush("1;33"), // Warn               yellow
	stdBrush("1;32"), // Notice             green
	stdBrush("1;30"), // Info               grey
	stdBrush("1;34"), // Debug              blue
}

type Std struct {
	Level Levels
	Color bool

	writer io.Writer
}

func (apt *Std) Name() string { return "Std" }

func (apt *Std) Setup() { apt.writer = ansicolor.NewAnsiColorWriter(os.Stdout) }

func (apt *Std) Write(data Data) {
	if data.Level <= apt.Level {
		tm := data.Time.Format("[01/02 15:04:05.000]")
		lv := Labels[data.Level]
		if apt.Color && data.Level != LevelUnknown {
			lv = stdBrushes[data.Level-1](lv)
		}
		tag := data.Tag.Stringify()
		if tag == "" {
			fmt.Fprint(apt.writer, tm, " ", lv, " ")
		} else {
			fmt.Fprint(apt.writer, tm, " ", lv, " ", tag, " ")
		}
		switch val := data.Content.(type) {
		case string:
			if len(data.Args) > 0 {
				fmt.Fprintf(apt.writer, val, data.Args...)
			} else {
				fmt.Fprint(apt.writer, val)
			}
		default:
			if data.Content == nil {
				fmt.Fprint(apt.writer, "<nil>")
			} else {
				fmt.Fprint(apt.writer, val)
			}
		}
		fmt.Fprintln(apt.writer)
	}
}

func (apt *Std) Flush() { fmt.Println("XLog.Std.Flush: performed.") }

func (apt *Std) Reset() { fmt.Println("XLog.Std.Reset: performed.") }
