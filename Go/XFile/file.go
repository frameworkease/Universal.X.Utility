// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XFile

import (
	"bytes"
	"encoding/binary"
	"fmt"
	"io"
	"os"
	"path/filepath"

	"github.com/frameworkease/Universal.X.Utility/Go/XString"
)

type HashOptions struct {
	Algorithm    XString.HashAlgorithm
	SegmentCount int
	SegmentSize  int
}

func Exists(path string) bool {
	s, e := os.Stat(path)
	if e != nil {
		return false
	}
	return s.Mode().IsRegular()
}

func Open[T string | []byte](path string) T {
	b, e := os.ReadFile(path)
	if e != nil {
		fmt.Printf("XFile.Open: %v\n", e)
		var zero T
		return zero
	}
	var result T
	switch any(result).(type) {
	case string:
		return any(string(b)).(T)
	case []byte:
		return any(b).(T)
	default:
		var zero T
		return zero
	}
}

func Save[T string | []byte](path string, data T) bool {
	var bytes []byte
	switch v := any(data).(type) {
	case string:
		bytes = []byte(v)
	case []byte:
		bytes = v
	default:
		fmt.Printf("XFile.Save: unsupported data type: %T.\n", v)
		return false
	}
	dir := filepath.Dir(path)
	if !Directory.Exists(dir) {
		if !Directory.Create(dir) {
			fmt.Printf("XFile.Save: failed to create directory: %s.\n", dir)
			return false
		}
	}
	e := os.WriteFile(path, bytes, 0644)
	if e != nil {
		fmt.Printf("XFile.Save: %v\n", e)
		return false
	}
	return true
}

func Size(path string) int64 {
	info, e := os.Stat(path)
	if e != nil {
		return -1
	}
	return info.Size()
}

func Delete(path string) bool {
	if e := os.Remove(path); e != nil {
		fmt.Printf("XFile.Delete: %v\n", e)
		return false
	}
	return true
}

func Copy(src, dst string, overwrite ...bool) bool {
	_overwrite := true
	if len(overwrite) == 1 {
		_overwrite = overwrite[0]
	}
	if !Exists(src) {
		fmt.Printf("XFile.Copy: source file does not exist: %s\n", src)
		return false
	}
	if Exists(dst) && !_overwrite {
		fmt.Printf("XFile.Copy: target file already exists: %s\n", dst)
		return false
	}
	dir := filepath.Dir(dst)
	if !Directory.Exists(dir) {
		if !Directory.Create(dir) {
			fmt.Printf("XFile.Copy: failed to create directory: %s\n", dir)
			return false
		}
	}
	data := Open[[]byte](src)
	if data == nil {
		return false
	}
	return Save(dst, data)
}

func Move(src, dst string, overwrite ...bool) bool {
	_overwrite := true
	if len(overwrite) == 1 {
		_overwrite = overwrite[0]
	}
	if !Exists(src) {
		fmt.Printf("XFile.Move: source file does not exist: %s\n", src)
		return false
	}
	if Exists(dst) && !_overwrite {
		fmt.Printf("XFile.Move: target file already exists: %s\n", dst)
		return false
	}
	dir := filepath.Dir(dst)
	if !Directory.Exists(dir) {
		if !Directory.Create(dir) {
			fmt.Printf("XFile.Move: failed to create directory: %s\n", dir)
			return false
		}
	}
	if e := os.Rename(src, dst); e != nil {
		fmt.Printf("XFile.Move: %v\n", e)
		return false
	}
	return true
}

func Hash(path string, options ...HashOptions) string {
	if !Exists(path) {
		return ""
	}
	fileSize := Size(path)
	if fileSize <= 0 {
		return ""
	}
	_algorithm := XString.HashAlgorithmMD5 // 默认 MD5 算法
	_segmentCount := 8                     // 默认 8 段样本
	_segmentSize := 64 * 1024              // 默认 64KB 每段
	if len(options) > 0 {
		opt := options[0]
		if opt.Algorithm != 0 {
			_algorithm = opt.Algorithm
		}
		if opt.SegmentCount >= 0 {
			_segmentCount = opt.SegmentCount
		}
		if opt.SegmentSize > 0 {
			_segmentSize = opt.SegmentSize
		}
	}

	if _segmentCount <= 0 { // 全量哈希
		data := Open[[]byte](path)
		if data == nil {
			return ""
		}
		return XString.Hash(data, _algorithm)
	} else { // 分段哈希
		var buffer bytes.Buffer
		binary.Write(&buffer, binary.LittleEndian, fileSize)

		file, e := os.Open(path)
		if e != nil {
			fmt.Printf("XFile.Hash: %v\n", e)
			return ""
		}
		defer file.Close()

		readBuffer := make([]byte, _segmentSize)
		for i := 0; i < _segmentCount; i++ {
			// 计算采样偏移
			offset := fileSize * int64(i) / int64(_segmentCount)
			remaining := fileSize - offset
			if remaining <= 0 {
				break
			}

			// 计算读取大小
			readSize := min(min(remaining, int64(_segmentSize)), int64(len(readBuffer)))
			if readSize <= 0 {
				break
			}

			// 定位采样位置
			_, e = file.Seek(offset, io.SeekStart)
			if e != nil {
				fmt.Printf("XFile.Hash: %v\n", e)
				return ""
			}

			// 读取采样数据
			totalRead := 0
			for totalRead < int(readSize) {
				n, e := file.Read(readBuffer[totalRead:])
				if e != nil && e != io.EOF {
					fmt.Printf("XFile.Hash: %v\n", e)
					return ""
				}
				if n == 0 {
					break
				}
				totalRead += n
			}

			// 写入采样数据
			if totalRead > 0 {
				buffer.Write(readBuffer[:totalRead])
			}
		}

		return XString.Hash(buffer.Bytes(), _algorithm)
	}
}
