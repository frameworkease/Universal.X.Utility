// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XFile

import (
	"fmt"
	"os"
	"os/exec"
	"path/filepath"
	"regexp"
	"strings"
	"sync"

	"github.com/frameworkease/Universal.X.Utility/Go/XString"
)

type directoryScope struct {
	projectValue  string
	projectInit   sync.Once
	assetValue    string
	assetInit     sync.Once
	assetTestArgs []string
	localValue    string
	localInit     sync.Once
	localTestArgs []string
	Evaluator     XString.IEvaluator
}

type directoryEvaluator struct {
	pattern *regexp.Regexp
}

var Directory = &directoryScope{
	Evaluator: &directoryEvaluator{
		pattern: regexp.MustCompile(`\$\{XFile\.Directory\.([^}]+?)\}`),
	},
}

func (ds *directoryScope) Exists(path string) bool {
	s, e := os.Stat(path)
	if e != nil {
		return false
	}
	return s.IsDir()
}

func (ds *directoryScope) Create(path string, overwrite ...bool) bool {
	_overwrite := false
	if len(overwrite) == 1 {
		_overwrite = overwrite[0]
	}
	if ds.Exists(path) {
		if !_overwrite {
			return true
		}
		if !ds.Delete(path, true) {
			return false
		}
	}
	err := os.MkdirAll(path, os.ModePerm)
	if err != nil {
		fmt.Printf("XFile.Dir.Create: %v\n", err)
		return false
	}
	return true
}

func (ds *directoryScope) Delete(path string, recursive ...bool) bool {
	_recursive := true
	if len(recursive) == 1 {
		_recursive = recursive[0]
	}
	if !ds.Exists(path) {
		return false
	}
	if _recursive {
		if e := os.RemoveAll(path); e != nil {
			fmt.Printf("XFile.Dir.Delete: %v\n", e)
			return false
		}
	} else {
		if e := os.Remove(path); e != nil {
			fmt.Printf("XFile.Dir.Delete: %v\n", e)
			return false
		}
	}
	return true
}

func (ds *directoryScope) Copy(src, dst string, exclude ...string) bool {
	if !ds.Exists(src) {
		fmt.Printf("XFile.Directory.Copy: source directory does not exist: %s\n", src)
		return false
	}
	if !ds.Create(dst) {
		fmt.Printf("XFile.Directory.Copy: failed to create target directory: %s\n", dst)
		return false
	}
	err := filepath.Walk(src, func(path string, info os.FileInfo, err error) error {
		if err != nil {
			return err
		}
		relPath, e := filepath.Rel(src, path)
		if e != nil {
			return e
		}
		if relPath == "." {
			return nil
		}
		skip := false
		if len(exclude) > 0 {
			for _, pattern := range exclude {
				matched, e := regexp.MatchString(pattern, path)
				if e == nil && matched {
					skip = true
					break
				}
			}
		}
		if skip {
			if info.IsDir() {
				return filepath.SkipDir
			}
			return nil
		}
		dstPath := filepath.Join(dst, relPath)
		if info.IsDir() {
			if !ds.Create(dstPath) {
				return fmt.Errorf("failed to create directory: %s", dstPath)
			}
		} else {
			if !Copy(path, dstPath, true) {
				return fmt.Errorf("failed to copy file: %s", path)
			}
		}
		return nil
	})
	if err != nil {
		fmt.Printf("XFile.Directory.Copy: %v\n", err)
		return false
	}
	return true
}

func (ds *directoryScope) Move(src, dst string, overwrite ...bool) bool {
	_overwrite := true
	if len(overwrite) == 1 {
		_overwrite = overwrite[0]
	}
	if !ds.Exists(src) {
		fmt.Printf("XFile.Directory.Move: source directory does not exist: %s\n", src)
		return false
	}
	if ds.Exists(dst) && !_overwrite {
		fmt.Printf("XFile.Directory.Move: target directory already exists: %s\n", dst)
		return false
	}
	if ds.Exists(dst) && _overwrite {
		if !ds.Delete(dst, true) {
			return false
		}
	}
	parent := filepath.Dir(dst)
	if !ds.Exists(parent) {
		if !ds.Create(parent) {
			fmt.Printf("XFile.Directory.Move: failed to create parent directory: %s\n", parent)
			return false
		}
	}
	if e := os.Rename(src, dst); e != nil {
		fmt.Printf("XFile.Directory.Move: %v\n", e)
		return false
	}
	return true
}

func (ds *directoryScope) Walk(path string, walker func(path string, info os.FileInfo) bool) {
	if walker == nil {
		fmt.Printf("XFile.Directory.Walk: walker is nil.\n")
		return
	}
	err := filepath.Walk(path, func(p string, info os.FileInfo, err error) error {
		if err != nil {
			return err
		}
		if !walker(p, info) {
			return fmt.Errorf("stop walk")
		}
		return nil
	})
	if err != nil && err.Error() != "stop walk" {
		fmt.Printf("XFile.Directory.Walk: %v\n", err)
	}
}

func (ds *directoryScope) Project() string {
	ds.projectInit.Do(func() {
		cmd := exec.Command("go", "list", "-m", "-f", "{{.Dir}}")
		if output, err := cmd.Output(); err == nil {
			ds.projectValue = strings.TrimSpace(string(output))
		} else {
			cwd, _ := os.Getwd()
			ds.projectValue = cwd
		}
	})
	return ds.projectValue
}

func (ds *directoryScope) Asset() string {
	ds.assetInit.Do(func() {
		args := make([]string, 0)
		args = append(args, os.Args[1:]...)
		args = append(args, ds.assetTestArgs...)
		for i := 0; i < len(args); i++ {
			arg := args[i]
			if arg == "" || !(strings.HasPrefix(arg, "-XFile.Directory.Asset") || strings.HasPrefix(arg, "--XFile.Directory.Asset")) {
				continue
			}
			_, after, ok := strings.Cut(arg, "=")
			if ok {
				ds.assetValue = strings.TrimSpace(after)
			} else if i < len(args)-1 {
				ds.assetValue = strings.TrimSpace(args[i+1])
			}
			if ds.assetValue != "" {
				break
			}
		}
		if ds.assetValue == "" {
			cwd, _ := os.Getwd()
			ds.assetValue = filepath.Join(cwd, "Assets")
		}
	})
	return ds.assetValue
}

func (ds *directoryScope) Local() string {
	ds.localInit.Do(func() {
		args := make([]string, 0)
		args = append(args, os.Args[1:]...)
		args = append(args, ds.localTestArgs...)
		for i := 0; i < len(args); i++ {
			arg := args[i]
			if arg == "" || !(strings.HasPrefix(arg, "-XFile.Directory.Local") || strings.HasPrefix(arg, "--XFile.Directory.Local")) {
				continue
			}
			_, after, ok := strings.Cut(arg, "=")
			if ok {
				ds.localValue = strings.TrimSpace(after)
			} else if i < len(args)-1 {
				ds.localValue = strings.TrimSpace(args[i+1])
			}
			if ds.localValue != "" {
				break
			}
		}
		if ds.localValue == "" {
			cwd, _ := os.Getwd()
			ds.localValue = filepath.Join(cwd, "Local")
		}
		if err := os.MkdirAll(ds.localValue, 0755); err != nil {
			panic(fmt.Sprintf("Failed to create local directory: %v", err))
		}
	})
	return ds.localValue
}

func (de *directoryEvaluator) Evaluate(expr string) string {
	if expr == "" {
		return expr
	}

	visited := make(map[string]bool)

	var repl func(string) string
	repl = func(m string) string {
		sm := de.pattern.FindStringSubmatch(m)
		keyName := sm[1]
		key := "Directory." + keyName

		if strings.Contains(keyName, "${") {
			return fmt.Sprintf("${Nested.%v}", m)
		}

		if visited[key] {
			return fmt.Sprintf("${Recursive.XFile.%v}", key)
		}
		visited[key] = true
		defer delete(visited, key)

		var value string
		switch keyName {
		case "Project":
			value = Directory.Project()
		case "Local":
			value = Directory.Local()
		case "Asset":
			value = Directory.Asset()
		default:
			return fmt.Sprintf("${Unknown.XFile.Directory.%v}", keyName)
		}

		if value != "" {
			return de.pattern.ReplaceAllStringFunc(value, repl)
		}

		return fmt.Sprintf("${Unknown.XFile.Directory.%v}", keyName)
	}

	return de.pattern.ReplaceAllStringFunc(expr, repl)
}
