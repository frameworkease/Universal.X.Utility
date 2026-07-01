// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XFile

import (
	"archive/zip"
	"fmt"
	"io"
	"os"
	"path/filepath"
	"sync"
)

type ArchiveFormat uint8

const (
	ArchiveZip ArchiveFormat = iota
	ArchiveTarGz
	ArchiveRar
	ArchiveSevenZ
)

type ArchiveOptions struct {
	Format     ArchiveFormat
	OnProgress func(progress float64)
	OnComplete func()
	OnError    func(string)
}

type archiveScope struct{}

var Archive = &archiveScope{}

func (as *archiveScope) Compress(src string, dst string, opt ArchiveOptions) *sync.WaitGroup {
	wg := &sync.WaitGroup{}
	wg.Add(1)
	switch opt.Format {
	case ArchiveZip:
		go as.compressZip(wg, src, dst, opt)
	default:
		go func() {
			defer wg.Done()
			if opt.OnError != nil {
				opt.OnError(fmt.Sprintf("unsupported format: %d", opt.Format))
			}
		}()
	}
	return wg
}

func (as *archiveScope) Extract(src string, dst string, opt ArchiveOptions) *sync.WaitGroup {
	wg := &sync.WaitGroup{}
	wg.Add(1)
	switch opt.Format {
	case ArchiveZip:
		go as.extractZip(wg, src, dst, opt)
	default:
		go func() {
			defer wg.Done()
			if opt.OnError != nil {
				opt.OnError(fmt.Sprintf("unsupported format: %d", opt.Format))
			}
		}()
	}
	return wg
}

func (as *archiveScope) compressZip(wg *sync.WaitGroup, src string, dst string, opt ArchiveOptions) {
	defer wg.Done()

	srcInfo, e := os.Stat(src)
	if e != nil {
		if opt.OnError != nil {
			opt.OnError(fmt.Sprintf("source does not exist: %s", src))
		}
		return
	}

	zipDir := filepath.Dir(dst)
	if !Directory.Exists(zipDir) {
		if !Directory.Create(zipDir) {
			if opt.OnError != nil {
				opt.OnError(fmt.Sprintf("failed to create zip directory: %s", zipDir))
			}
			return
		}
	}

	zipFile, e := os.Create(dst)
	if e != nil {
		if opt.OnError != nil {
			opt.OnError(fmt.Sprintf("failed to create zip file: %v", e))
		}
		return
	}
	defer zipFile.Close()

	zipWriter := zip.NewWriter(zipFile)
	defer zipWriter.Close()

	var totalSize int64
	var processedSize int64
	var fileCount int
	var totalFiles int

	if !srcInfo.IsDir() { // 文件压缩
		totalFiles = 1
		totalSize = srcInfo.Size()

		srcFile, e := os.Open(src)
		if e != nil {
			if opt.OnError != nil {
				opt.OnError(e.Error())
			}
			return
		}
		defer srcFile.Close()

		relPath := filepath.Base(src)
		zipEntry, e := zipWriter.Create(relPath)
		if e != nil {
			if opt.OnError != nil {
				opt.OnError(e.Error())
			}
			return
		}

		written, e := io.Copy(zipEntry, srcFile)
		if e != nil {
			if opt.OnError != nil {
				opt.OnError(e.Error())
			}
			return
		}

		processedSize += written
		fileCount++

		if opt.OnProgress != nil && totalSize > 0 {
			progress := float64(processedSize) / float64(totalSize) * 100.0
			opt.OnProgress(progress)
		}
	} else { // 目录压缩
		dir, e := filepath.Abs(src)
		if e != nil {
			if opt.OnError != nil {
				opt.OnError(e.Error())
			}
			return
		}
		dir = filepath.Clean(dir)

		// 统计文件
		type fileInfo struct {
			path    string
			size    int64
			relPath string
		}
		var zipFiles []fileInfo
		filepath.Walk(dir, func(path string, info os.FileInfo, err error) error {
			if err != nil {
				return err
			}
			if path == dir {
				return nil
			}
			if !info.IsDir() {
				relPath, e := filepath.Rel(dir, path)
				if e != nil {
					return e
				}
				relPath = filepath.ToSlash(relPath)
				zipFiles = append(zipFiles, fileInfo{
					path:    path,
					size:    info.Size(),
					relPath: relPath,
				})
				totalFiles++
				totalSize += info.Size()
			}
			return nil
		})

		// 压缩文件
		for _, file := range zipFiles {
			srcFile, e := os.Open(file.path)
			if e != nil {
				if opt.OnError != nil {
					opt.OnError(e.Error())
				}
				return
			}
			zipEntry, e := zipWriter.Create(file.relPath)
			if e != nil {
				srcFile.Close()
				if opt.OnError != nil {
					opt.OnError(e.Error())
				}
				return
			}
			written, e := io.Copy(zipEntry, srcFile)
			srcFile.Close()
			if e != nil {
				if opt.OnError != nil {
					opt.OnError(e.Error())
				}
				return
			}

			processedSize += written
			fileCount++
			if opt.OnProgress != nil && totalSize > 0 {
				progress := float64(processedSize) / float64(totalSize) * 100.0
				opt.OnProgress(progress)
			}
		}
	}

	if opt.OnComplete != nil {
		opt.OnComplete()
	}
}

func (as *archiveScope) extractZip(wg *sync.WaitGroup, src string, dst string, opt ArchiveOptions) {
	defer wg.Done()

	if !Exists(src) {
		if opt.OnError != nil {
			opt.OnError(fmt.Sprintf("zip file does not exist: %s", src))
		}
		return
	}

	if !Directory.Exists(dst) {
		if !Directory.Create(dst) {
			if opt.OnError != nil {
				opt.OnError(fmt.Sprintf("failed to create target directory: %s", dst))
			}
			return
		}
	}

	zipReader, e := zip.OpenReader(src)
	if e != nil {
		if opt.OnError != nil {
			opt.OnError(fmt.Sprintf("failed to open zip file: %v", e))
		}
		return
	}
	defer zipReader.Close()

	var totalSize int64
	var processedSize int64
	var totalFiles int
	var zipFiles []*zip.File
	for _, file := range zipReader.File {
		totalFiles++
		totalSize += int64(file.UncompressedSize64)
		zipFiles = append(zipFiles, file)
	}
	for i, file := range zipFiles {
		filePath := filepath.Join(dst, file.Name)
		if file.FileInfo().IsDir() {
			if !Directory.Create(filePath) {
				if opt.OnError != nil {
					opt.OnError(fmt.Sprintf("failed to create directory: %s", filePath))
				}
				return
			}
			continue
		}
		parent := filepath.Dir(filePath)
		if !Directory.Exists(parent) {
			if !Directory.Create(parent) {
				if opt.OnError != nil {
					opt.OnError(fmt.Sprintf("failed to create parent directory: %s", parent))
				}
				return
			}
		}
		rc, e := file.Open()
		if e != nil {
			if opt.OnError != nil {
				opt.OnError(fmt.Sprintf("failed to open file in zip: %v", e))
			}
			return
		}
		dstFile, e := os.Create(filePath)
		if e != nil {
			rc.Close()
			if opt.OnError != nil {
				opt.OnError(fmt.Sprintf("failed to create file: %v", e))
			}
			return
		}
		written, e := io.Copy(dstFile, rc)
		dstFile.Close()
		rc.Close()
		if e != nil {
			if opt.OnError != nil {
				opt.OnError(fmt.Sprintf("failed to extract file: %v", e))
			}
			return
		}

		processedSize += written
		if opt.OnProgress != nil && totalSize > 0 {
			progress := float64(processedSize) / float64(totalSize) * 100.0
			opt.OnProgress(progress)
		} else if opt.OnProgress != nil && totalFiles > 0 {
			progress := float64(i+1) / float64(totalFiles) * 100.0
			opt.OnProgress(progress)
		}
	}

	if opt.OnComplete != nil {
		opt.OnComplete()
	}
}
