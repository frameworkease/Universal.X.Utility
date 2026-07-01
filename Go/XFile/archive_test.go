// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XFile

import (
	"path/filepath"
	"testing"

	"github.com/stretchr/testify/assert"
	"github.com/stretchr/testify/require"
)

func TestArchiveZip(t *testing.T) {
	testDir := filepath.Join(t.TempDir(), "ArchiveZip")

	// 文件压缩
	{
		srcFile := filepath.Join(testDir, "test.txt")
		dstZip := filepath.Join(testDir, "test.zip")
		extractedDir := filepath.Join(testDir, "extracted")
		content := "Hello World!"
		require.True(t, Save(srcFile, content), "应能创建源文件。")

		// 压缩
		{
			Archive.Compress(srcFile, dstZip, ArchiveOptions{Format: ArchiveZip}).Wait()
			assert.True(t, Exists(dstZip), "压缩文件应被创建。")
		}

		// 解压
		{
			Archive.Extract(dstZip, extractedDir, ArchiveOptions{Format: ArchiveZip}).Wait()
			extractedFile := filepath.Join(extractedDir, "test.txt")
			assert.True(t, Exists(extractedFile), "解压后的文件应存在。")
			actualContent := Open[string](extractedFile)
			assert.Equal(t, content, actualContent, "解压后的文件内容应一致。")
		}
	}

	// 目录压缩
	{
		srcDir := filepath.Join(testDir, "src_dir")
		dstZip := filepath.Join(testDir, "dir.zip")
		extractedDir := filepath.Join(testDir, "extracted_dir")

		Directory.Create(srcDir)
		Save(filepath.Join(srcDir, "file1.txt"), "content1")
		Save(filepath.Join(srcDir, "file2.txt"), "content2")
		Save(filepath.Join(srcDir, "exclude.txt"), "exclude")
		Directory.Create(filepath.Join(srcDir, "subdir"))
		Save(filepath.Join(srcDir, "subdir", "file3.txt"), "content3")

		// 压缩
		{
			var onCompleteCalled bool
			var onProgressValues []float64

			Archive.Compress(srcDir, dstZip, ArchiveOptions{
				OnProgress: func(progress float64) { onProgressValues = append(onProgressValues, progress) },
				OnComplete: func() { onCompleteCalled = true },
			}).Wait()

			assert.True(t, Exists(dstZip), "压缩文件应被创建。")
			assert.True(t, onCompleteCalled, "压缩 OnComplete 回调应被调用。")
			assert.Equal(t, 4, len(onProgressValues), "压缩 OnProgress 回调应被调用 4 次。")
			assert.GreaterOrEqual(t, onProgressValues[len(onProgressValues)-1], 100.0, "压缩最终进度应达到 100%。")
		}

		// 解压
		{
			var onCompleteCalled bool
			var onProgressValues []float64

			Archive.Extract(dstZip, extractedDir, ArchiveOptions{
				OnProgress: func(progress float64) { onProgressValues = append(onProgressValues, progress) },
				OnComplete: func() { onCompleteCalled = true },
			}).Wait()

			assert.True(t, onCompleteCalled, "解压 OnComplete 回调应被调用。")
			assert.Equal(t, 4, len(onProgressValues), "解压 OnProgress 回调应被调用 4 次。")
			assert.GreaterOrEqual(t, onProgressValues[len(onProgressValues)-1], 100.0, "解压最终进度应达到 100%。")
			assert.True(t, Exists(filepath.Join(extractedDir, "file1.txt")), "解压后的文件应存在。")
			assert.True(t, Exists(filepath.Join(extractedDir, "file2.txt")), "解压后的文件应存在。")
			assert.True(t, Exists(filepath.Join(extractedDir, "subdir", "file3.txt")), "解压后的子目录文件应存在。")
			assert.True(t, Exists(filepath.Join(extractedDir, "exclude.txt")), "解压后的文件应存在。")
			assert.Equal(t, "content1", Open[string](filepath.Join(extractedDir, "file1.txt")), "解压后的文件内容应一致。")
			assert.Equal(t, "content2", Open[string](filepath.Join(extractedDir, "file2.txt")), "解压后的文件内容应一致。")
			assert.Equal(t, "content3", Open[string](filepath.Join(extractedDir, "subdir", "file3.txt")), "解压后的子目录文件内容应一致。")
			assert.Equal(t, "exclude", Open[string](filepath.Join(extractedDir, "exclude.txt")), "解压后的文件内容应一致。")
		}
	}

	// 边界测试
	{
		// 源文件不存在
		{
			var errorOccurred string
			Archive.Compress(filepath.Join(testDir, "nonexistent.txt"), filepath.Join(testDir, "error1.zip"), ArchiveOptions{
				OnError: func(err string) { errorOccurred = err },
			}).Wait()
			assert.NotEmpty(t, errorOccurred, "源文件不存在时应触发 OnError 回调。")
		}

		// 压缩文件不存在
		{
			var errorOccurred string
			Archive.Extract(filepath.Join(testDir, "nonexistent.zip"), filepath.Join(testDir, "error_extracted"), ArchiveOptions{
				OnError: func(err string) { errorOccurred = err },
			}).Wait()
			assert.NotEmpty(t, errorOccurred, "压缩文件不存在时应触发 OnError 回调。")
		}

		// 压缩文件无效
		{
			invalidZip := filepath.Join(testDir, "invalid.zip")
			Save(invalidZip, "not a zip file")

			extractedDir := filepath.Join(testDir, "invalid_extracted")
			var errorOccurred string
			Archive.Extract(invalidZip, extractedDir, ArchiveOptions{
				OnError: func(err string) { errorOccurred = err },
			}).Wait()
			assert.NotEmpty(t, errorOccurred, "解压无效压缩文件时应触发 OnError 回调。")
		}
	}
}
