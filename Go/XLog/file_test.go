// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XLog

import (
	"fmt"
	"os"
	"path/filepath"
	"regexp"
	"slices"
	"strings"
	"sync"
	"testing"
	"time"

	"github.com/frameworkease/Universal.X.Utility/Go/XFile"
	"github.com/frameworkease/Universal.X.Utility/Go/XString"
	"github.com/stretchr/testify/assert"
)

func TestFileName(t *testing.T) {
	apt := &File{}
	assert.Equal(t, "File", apt.Name())
}

func TestFileSetup(t *testing.T) {
	t.Run("Setup", func(t *testing.T) {
		defer ResetTestAdapter(t)()

		apt := &File{Level: LevelError, Count: 100, Line: 1000000, Size: 1 << 27, Day: 7, Path: defaultFilePath}
		Adapter.Setup(apt)
		assert.True(t, slices.ContainsFunc(Adapter.adapters, func(ele IAdapter) bool {
			return ele == apt
		}))
		assert.Equal(t, 200000, cap(apt.cdata))
		assert.Equal(t, int32(1), apt.iclose)
		assert.Equal(t, 1, cap(apt.sclose))
		assert.NotNil(t, apt.wclose)
		assert.Equal(t, 1, cap(apt.wflush))
		apt.Reset()
	})

	t.Run("Loop", func(t *testing.T) {
		for _, cfg := range []struct{ line, size int }{
			{100, 1 << 27},
			{1000000, 4000},
		} {
			defer ResetTestAdapter(t)()

			dir := t.TempDir()
			fbase := "Loop"
			fext := ".log"
			pat := regexp.MustCompile(fmt.Sprintf(`^%s\.\d{4}-\d{2}-\d{2}\.\d{3}%s$`, fbase, fext))
			file := filepath.Join(dir, fbase+fext)

			wclean := make(chan *sync.WaitGroup, 1)
			apt := &File{Level: LevelInfo, Count: 5, Line: cfg.line, Size: cfg.size, Day: 3, Path: file, wclean: wclean}
			Adapter.Setup(apt)
			for i := range 1000 {
				apt.Write(Data{Level: LevelNotice, Content: fmt.Sprintf("line num: #%03d", i), Time: time.Now()})
			}
			apt.Flush()
			defer apt.Reset()

			files, err := os.ReadDir(dir)
			assert.NoError(t, err)
			count := 0
			before := time.Now().Add(-4 * 24 * time.Hour)
			for _, file := range files {
				if pat.MatchString(file.Name()) {
					if count < 3 {
						err = os.Chtimes(filepath.Join(dir, file.Name()), before, before)
						assert.NoError(t, err)
					}
					count++
				}
			}
			assert.Equal(t, 5, count)
			assert.Equal(t, 6, len(files))
			assert.FileExists(t, file)
			data, err := os.ReadFile(file)
			assert.NoError(t, err)
			assert.Empty(t, data)

			wg := &sync.WaitGroup{}
			wg.Add(1)
			wclean <- wg
			wg.Wait()
			files, err = os.ReadDir(dir)
			assert.NoError(t, err)
			count = 0
			for _, file := range files {
				if pat.MatchString(file.Name()) {
					count++
				}
			}
			assert.Equal(t, 2, count)
		}
	})
}

func TestFileWrite(t *testing.T) {
	defer ResetTestAdapter(t)()

	file := XString.Evaluate(defaultFilePath, XFile.Directory.Evaluator)
	if _, err := os.Stat(file); err == nil {
		os.Remove(file)
	}
	defer func() {
		if _, err := os.Stat(file); err == nil {
			os.Remove(file)
		}
	}()
	os.WriteFile(file, fmt.Appendf(nil, "init log\n"), 0644)

	apt := &File{Level: LevelInfo, Count: 100, Line: 1000000, Size: 1 << 27, Day: 7, Path: filepath.Join(t.TempDir(), ".invalid")}
	Adapter.Setup(apt)
	for i := range 100000 {
		data := Data{Content: fmt.Sprintf("test log %d", i), Time: time.Now()}
		if i%2 == 0 {
			data.Level = LevelNotice
		} else {
			data.Level = LevelDebug
		}
		apt.Write(data)
	}

	apt.Reset()
	for i := range 1000 {
		apt.Write(Data{Level: LevelInfo, Content: fmt.Sprintf("test log %d", i), Time: time.Now()})
	}

	assert.FileExists(t, file)
	data, err := os.ReadFile(file)
	assert.NoError(t, err)
	content := string(data)
	assert.Equal(t, 1, strings.Count(content, "init log"))
	assert.Equal(t, 50000, strings.Count(content, "test log"))
}

func TestFileFlush(t *testing.T) {
	defer ResetTestAdapter(t)()

	file := filepath.Join(t.TempDir(), "Flush.log")
	apt := &File{Level: LevelInfo, Count: 100, Line: 1000000, Size: 1 << 27, Day: 7, Path: file}
	Adapter.Setup(apt)
	for i := range 100000 {
		apt.Write(Data{Level: LevelNotice, Content: fmt.Sprintf("test log %d", i), Time: time.Now()})
	}

	wg := sync.WaitGroup{}
	for range 10 {
		wg.Add(1)
		go func() {
			defer wg.Done()
			apt.Flush()
		}()
	}
	wg.Wait()

	apt.Reset()
	apt.Flush()

	assert.FileExists(t, file)
	data, err := os.ReadFile(file)
	assert.NoError(t, err)
	content := string(data)
	assert.Equal(t, 100000, strings.Count(content, "test log"))
}

func TestFileReset(t *testing.T) {
	defer ResetTestAdapter(t)()

	file := filepath.Join(t.TempDir(), "Reset.log")
	apt := &File{Level: LevelInfo, Count: 100, Line: 1000000, Size: 1 << 27, Day: 7, Path: file}
	Adapter.Setup(apt)
	for i := range 100000 {
		apt.Write(Data{Level: LevelNotice, Content: fmt.Sprintf("test log %d", i), Time: time.Now()})
	}

	wg := sync.WaitGroup{}
	for range 10 {
		wg.Add(1)
		go func() {
			defer wg.Done()
			apt.Reset()
		}()
	}
	wg.Wait()

	assert.FileExists(t, file)
	data, err := os.ReadFile(file)
	assert.NoError(t, err)
	content := string(data)
	assert.Equal(t, 100000, strings.Count(content, "test log"))
}
