// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XLog

import (
	"bytes"
	"fmt"
	"io"
	"os"
	"os/signal"
	"path/filepath"
	"regexp"
	"sort"
	"strconv"
	"strings"
	"sync"
	"sync/atomic"
	"syscall"
	"time"

	"github.com/frameworkease/Universal.X.Utility/Go/XEnv"
	"github.com/frameworkease/Universal.X.Utility/Go/XFile"
	"github.com/frameworkease/Universal.X.Utility/Go/XString"
	"github.com/illumitacit/gostd/quit"
)

type File struct {
	Level Levels
	Count int
	Line  int
	Size  int
	Day   int
	Path  string

	wclean chan *sync.WaitGroup
	cdata  chan Data
	iclose int32
	sclose chan os.Signal
	wclose *sync.WaitGroup
	wflush chan *sync.WaitGroup
}

const defaultFilePath = "${XFile.Directory.Local}/Log/App.log"

func (apt *File) Setup() {
	if atomic.LoadInt32(&apt.iclose) != 0 {
		return
	}

	pcount := apt.Count
	pline := apt.Line
	psize := apt.Size
	pday := apt.Day
	ppath := filepath.Clean(XString.Evaluate(apt.Path, XFile.Directory.Evaluator, XEnv.Argument.Evaluator, XEnv.Variable.Evaluator))
	pdir := filepath.Dir(ppath)
	pbase := filepath.Base(ppath)
	pext := filepath.Ext(ppath)
	if pdir == "" || pbase == "" || pext == "" || pbase == pext {
		dpath := filepath.Clean(XString.Evaluate(defaultFilePath, XFile.Directory.Evaluator))
		fmt.Fprintf(os.Stderr, "XLog.File.Setup: invalid path format: %s, use default: %s\n", ppath, dpath)
		ppath = dpath
		pdir = filepath.Dir(ppath)
		pext = filepath.Ext(ppath)
		pbase = filepath.Base(ppath)
	}
	pbase = strings.ReplaceAll(pbase, pext, "")

	apt.cdata = make(chan Data, 200000)
	apt.sclose = make(chan os.Signal, 1)
	signal.Notify(apt.sclose, syscall.SIGTERM, syscall.SIGINT)
	apt.wclose = &sync.WaitGroup{}
	apt.wclose.Add(1)
	apt.wflush = make(chan *sync.WaitGroup, 1)

	winit := &sync.WaitGroup{}
	winit.Add(1)
	go func() {
		defer func() {
			if r := recover(); r != nil {
				fmt.Fprintf(os.Stderr, "XLog.File.Setup.Loop: unhandled panic: %v\n", r)
			}
		}()

		var fwriter *os.File
		var fline, fsize int
		create := func() {
			if fwriter != nil {
				fwriter.Close()
				fwriter = nil
			}
			if err := os.MkdirAll(pdir, 0755); err != nil {
				fmt.Fprintf(os.Stderr, "XLog.File.Setup.Create: failed to create dir: %v\n", err)
				return
			}
			fs, err := os.OpenFile(ppath, os.O_WRONLY|os.O_APPEND|os.O_CREATE, 0644)
			if err != nil {
				fmt.Fprintf(os.Stderr, "XLog.File.Setup.Create: failed to open file: %v\n", err)
				return
			}
			if err = os.Chmod(ppath, 0644); err != nil {
				fs.Close()
				fmt.Fprintf(os.Stderr, "XLog.File.Setup.Create: failed to chmod: %v\n", err)
				return
			}
			stat, err := fs.Stat()
			if err != nil {
				fs.Close()
				fmt.Fprintf(os.Stderr, "XLog.File.Setup.Create: failed to get stat: %v\n", err)
				return
			}
			fsize = int(stat.Size())
			fline = 0
			if stat.Size() > 0 && pline > 0 {
				cnt := 0
				buf := make([]byte, 32768)
				sep := []byte{'\n'}
				for {
					c, err := fs.Read(buf)
					if err != nil && err != io.EOF {
						fmt.Fprintf(os.Stderr, "XLog.File.Setup.Create: failed to read line: %v\n", err)
						break
					}
					cnt += bytes.Count(buf[:c], sep)
					if err == io.EOF {
						break
					}
				}
				fline = cnt
			}
			fwriter = fs
		}

		cleanup := func() {
			pat := regexp.MustCompile(fmt.Sprintf(`^%s\.\d{4}-\d{2}-\d{2}\.\d{3}%s$`, pbase, pext))
			var files []os.FileInfo
			if entries, err := os.ReadDir(pdir); err != nil {
				fmt.Fprintf(os.Stderr, "XLog.File.Setup.Cleanup: failed to read dir: %v\n", err)
			} else {
				for _, entry := range entries {
					if pat.MatchString(entry.Name()) {
						info, err := entry.Info()
						if err != nil {
							fmt.Fprintf(os.Stderr, "XLog.File.Setup.Cleanup: failed to get info: %v\n", err)
							continue
						}
						files = append(files, info)
					}
				}
			}
			sort.Slice(files, func(i, j int) bool { return files[i].ModTime().Before(files[j].ModTime()) })
			before := time.Now().Add(-24 * time.Hour * time.Duration(pday))
			for i, file := range files {
				if (pcount > 0 && len(files)-i > pcount) || (pday > 0 && file.ModTime().Before(before)) {
					if err := os.Remove(filepath.Join(pdir, file.Name())); err != nil {
						fmt.Fprintf(os.Stderr, "XLog.File.Setup.Cleanup: failed to remove file %s: %v\n", file.Name(), err)
					}
				}
			}
		}

		rotate := func() {
			if fwriter != nil {
				fwriter.Close()
				fwriter = nil
			}

			num := 1
			sdate := time.Now().Format("2006-01-02")
			if files, err := os.ReadDir(pdir); err != nil {
				fmt.Fprintf(os.Stderr, "XLog.File.Setup.Rotate: failed to read dir: %v\n", err)
			} else {
				pat := regexp.MustCompile(fmt.Sprintf(`^%s\.%s\.(\d{3})%s$`, pbase, sdate, pext))
				for _, file := range files {
					if !file.IsDir() {
						matches := pat.FindStringSubmatch(file.Name())
						if len(matches) == 2 {
							if val, err := strconv.Atoi(matches[1]); err == nil && val >= num {
								num = val + 1
							}
						}
					}
				}
			}

			tfile := filepath.Join(pdir, fmt.Sprintf("%s.%s.%03d%s", pbase, sdate, num, pext))
			if err := os.Rename(ppath, tfile); err != nil {
				fmt.Fprintf(os.Stderr, "XLog.File.Setup.Rotate: failed to rotate file: %v\n", err)
			}
		}

		write := func(data Data) {
			if fwriter != nil {
				str := data.Stringify()
				if _, err := fmt.Fprintln(fwriter, str); err != nil {
					fmt.Fprintf(os.Stderr, "XLog.File.Setup.Write: %s\n", err)
				} else {
					fline++
					fsize += len(str) + 1
				}
			}

			if (pline > 0 && fline >= pline) || (psize > 0 && fsize >= psize) {
				rotate()
				cleanup()
				create()
				if fwriter == nil {
					fmt.Fprintf(os.Stderr, "XLog.File.Setup.Write: failed to new writer.\n")
				}
			}
		}

		defer func() {
			atomic.StoreInt32(&apt.iclose, 2)
			for len(apt.cdata) > 0 {
				data := <-apt.cdata
				write(data)
			}
			if fwriter != nil {
				fwriter.Sync()
				fwriter.Close()
			}
			apt.wclose.Done()
			quit.GetWaiter().Done()
		}()

		create()
		atomic.StoreInt32(&apt.iclose, 1)

		ticker := time.NewTicker(1 * time.Hour)
		defer ticker.Stop()

		squit := quit.GetQuitChannel()
		quit.GetWaiter().Add(1)

		winit.Done()

		for {
			select {
			case data := <-apt.cdata:
				write(data)
			case wg := <-apt.wflush:
				for len(apt.cdata) > 0 {
					data := <-apt.cdata
					write(data)
				}
				if fwriter != nil {
					fwriter.Sync()
				}
				wg.Done()
			case <-ticker.C:
				cleanup()
			case wg := <-apt.wclean:
				cleanup()
				wg.Done()
			case <-apt.sclose:
				fmt.Println("XLog.File.Setup.Loop: receive signal of close.")
				return
			case <-squit:
				fmt.Println("XLog.File.Setup.Loop: receive signal of quit.")
				return
			}
		}
	}()
	winit.Wait()
}

func (apt *File) Name() string { return "File" }

func (apt *File) Write(data Data) {
	if data.Level <= apt.Level && atomic.LoadInt32(&apt.iclose) == 1 {
		select {
		case apt.cdata <- data:
		default:
		}
	}
}

func (apt *File) Flush() {
	if atomic.LoadInt32(&apt.iclose) != 1 {
		return
	}
	wg := &sync.WaitGroup{}
	wg.Add(1)
	apt.wflush <- wg
	wg.Wait()
	fmt.Println("XLog.File.Flush: performed.")
}

func (apt *File) Reset() {
	if !atomic.CompareAndSwapInt32(&apt.iclose, 1, 2) {
		return
	}
	signal.Stop(apt.sclose)
	close(apt.sclose)
	apt.wclose.Wait()
	fmt.Println("XLog.File.Reset: performed.")
}
