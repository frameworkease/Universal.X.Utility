// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XFile

import (
	"path/filepath"
	"testing"

	"github.com/frameworkease/Universal.X.Utility/Go/XString"
	"github.com/stretchr/testify/assert"
	"github.com/stretchr/testify/require"
)

func TestFileExists(t *testing.T) {
	testDir := filepath.Join(t.TempDir(), "FileExists")

	// 不存在的文件
	assert.False(t, Exists(filepath.Join(testDir, "nonexistent.txt")), "不存在的文件应返回 false。")

	// 创建文件
	testFile := filepath.Join(testDir, "test.txt")
	require.True(t, Save(testFile, "test content"), "应能创建测试文件。")
	assert.True(t, Exists(testFile), "已存在的文件应返回 true。")

	// 目录路径应返回 false
	assert.False(t, Exists(testDir), "目录路径应返回 false。")
}

func TestFileOpen(t *testing.T) {
	testDir := filepath.Join(t.TempDir(), "FileOpen")
	testFile := filepath.Join(testDir, "test.txt")
	content := "test content"

	// 保存文件
	require.True(t, Save(testFile, content), "应能保存文件。")

	// 测试 Open[string]
	{
		result := Open[string](testFile)
		assert.Equal(t, content, result, "应能读取文件内容为 string。")
	}

	// 测试 Open[[]byte]
	{
		result := Open[[]byte](testFile)
		assert.Equal(t, []byte(content), result, "应能读取文件内容为 []byte。")
	}

	// 不存在的文件
	{
		nonexistent := filepath.Join(testDir, "nonexistent.txt")
		resultStr := Open[string](nonexistent)
		assert.Equal(t, "", resultStr, "不存在的文件应返回空 string。")

		resultBytes := Open[[]byte](nonexistent)
		assert.Nil(t, resultBytes, "不存在的文件应返回 nil []byte。")
	}
}

func TestFileSave(t *testing.T) {
	testDir := filepath.Join(t.TempDir(), "FileSave")

	tests := []struct {
		name     string
		path     string
		data     any
		expected bool
	}{
		{"保存 string", filepath.Join(testDir, "string.txt"), "test content", true},
		{"保存 []byte", filepath.Join(testDir, "bytes.txt"), []byte("test content"), true},
		{"保存到新目录", filepath.Join(testDir, "subdir", "file.txt"), "content", true},
		{"保存空内容", filepath.Join(testDir, "empty.txt"), "", true},
		{"保存空 []byte", filepath.Join(testDir, "empty_bytes.txt"), []byte{}, true},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			var result bool
			switch v := tt.data.(type) {
			case string:
				result = Save(tt.path, v)
			case []byte:
				result = Save(tt.path, v)
			}
			assert.Equal(t, tt.expected, result, "保存文件结果不正确。")
			if tt.expected {
				assert.True(t, Exists(tt.path), "文件应被成功创建。")
			}
		})
	}
}

func TestFileSize(t *testing.T) {
	testDir := filepath.Join(t.TempDir(), "FileSize")

	// 不存在的文件
	{
		result := Size(filepath.Join(testDir, "nonexistent.txt"))
		assert.Equal(t, int64(-1), result, "不存在的文件应返回 -1。")
	}

	// 空文件
	{
		emptyFile := filepath.Join(testDir, "empty.txt")
		require.True(t, Save(emptyFile, ""), "应能创建空文件。")
		result := Size(emptyFile)
		assert.Equal(t, int64(0), result, "空文件应返回 0。")
	}

	// 有内容的文件
	{
		content := "test content"
		contentFile := filepath.Join(testDir, "content.txt")
		require.True(t, Save(contentFile, content), "应能创建文件。")
		result := Size(contentFile)
		assert.Equal(t, int64(len(content)), result, "文件大小应等于内容长度。")
	}
}

func TestFileDelete(t *testing.T) {
	testDir := filepath.Join(t.TempDir(), "FileDelete")

	// 删除不存在的文件
	{
		result := Delete(filepath.Join(testDir, "nonexistent.txt"))
		assert.False(t, result, "删除不存在的文件应返回 false。")
	}

	// 删除存在的文件
	{
		testFile := filepath.Join(testDir, "test.txt")
		require.True(t, Save(testFile, "content"), "应能创建文件。")
		assert.True(t, Exists(testFile), "文件应存在。")

		result := Delete(testFile)
		assert.True(t, result, "应能删除文件。")
		assert.False(t, Exists(testFile), "文件应被删除。")
	}
}

func TestFileCopy(t *testing.T) {
	testDir := filepath.Join(t.TempDir(), "FileCopy")

	// 源文件不存在
	{
		result := Copy(filepath.Join(testDir, "nonexistent.txt"), filepath.Join(testDir, "dst.txt"))
		assert.False(t, result, "源文件不存在时应返回 false。")
	}

	// 正常复制
	{
		srcFile := filepath.Join(testDir, "src.txt")
		dstFile := filepath.Join(testDir, "dst.txt")
		content := "test content"
		require.True(t, Save(srcFile, content), "应能创建源文件。")

		result := Copy(srcFile, dstFile)
		assert.True(t, result, "应能复制文件。")
		assert.True(t, Exists(dstFile), "目标文件应存在。")

		copiedContent := Open[string](dstFile)
		assert.Equal(t, content, copiedContent, "复制的文件内容应一致。")
	}

	// 复制到新目录
	{
		srcFile := filepath.Join(testDir, "src2.txt")
		dstFile := filepath.Join(testDir, "subdir", "dst2.txt")
		content := "test content 2"
		require.True(t, Save(srcFile, content), "应能创建源文件。")

		result := Copy(srcFile, dstFile)
		assert.True(t, result, "应能复制文件到新目录。")
		assert.True(t, Exists(dstFile), "目标文件应存在。")
	}

	// 目标文件已存在，不允许覆盖
	{
		srcFile := filepath.Join(testDir, "src3.txt")
		dstFile := filepath.Join(testDir, "dst3.txt")
		require.True(t, Save(srcFile, "source"), "应能创建源文件。")
		require.True(t, Save(dstFile, "destination"), "应能创建目标文件。")

		result := Copy(srcFile, dstFile, false)
		assert.False(t, result, "不允许覆盖时应返回 false。")

		// 验证目标文件内容未改变
		copiedContent := Open[string](dstFile)
		assert.Equal(t, "destination", copiedContent, "目标文件内容不应改变。")
	}

	// 目标文件已存在，允许覆盖
	{
		srcFile := filepath.Join(testDir, "src4.txt")
		dstFile := filepath.Join(testDir, "dst4.txt")
		require.True(t, Save(srcFile, "source"), "应能创建源文件。")
		require.True(t, Save(dstFile, "destination"), "应能创建目标文件。")

		result := Copy(srcFile, dstFile, true)
		assert.True(t, result, "允许覆盖时应返回 true。")

		// 验证目标文件内容已改变
		copiedContent := Open[string](dstFile)
		assert.Equal(t, "source", copiedContent, "目标文件内容应被覆盖。")
	}
}

func TestFileMove(t *testing.T) {
	testDir := filepath.Join(t.TempDir(), "FileMove")

	// 源文件不存在
	{
		result := Move(filepath.Join(testDir, "nonexistent.txt"), filepath.Join(testDir, "dst.txt"))
		assert.False(t, result, "源文件不存在时应返回 false。")
	}

	// 正常移动
	{
		srcFile := filepath.Join(testDir, "src.txt")
		dstFile := filepath.Join(testDir, "dst.txt")
		content := "test content"
		require.True(t, Save(srcFile, content), "应能创建源文件。")

		result := Move(srcFile, dstFile)
		assert.True(t, result, "应能移动文件。")
		assert.False(t, Exists(srcFile), "源文件应被移动。")
		assert.True(t, Exists(dstFile), "目标文件应存在。")

		movedContent := Open[string](dstFile)
		assert.Equal(t, content, movedContent, "移动的文件内容应一致。")
	}

	// 移动到新目录
	{
		srcFile := filepath.Join(testDir, "src2.txt")
		dstFile := filepath.Join(testDir, "subdir", "dst2.txt")
		content := "test content 2"
		require.True(t, Save(srcFile, content), "应能创建源文件。")

		result := Move(srcFile, dstFile)
		assert.True(t, result, "应能移动文件到新目录。")
		assert.False(t, Exists(srcFile), "源文件应被移动。")
		assert.True(t, Exists(dstFile), "目标文件应存在。")
	}

	// 目标文件已存在，不允许覆盖
	{
		srcFile := filepath.Join(testDir, "src3.txt")
		dstFile := filepath.Join(testDir, "dst3.txt")
		require.True(t, Save(srcFile, "source"), "应能创建源文件。")
		require.True(t, Save(dstFile, "destination"), "应能创建目标文件。")

		result := Move(srcFile, dstFile, false)
		assert.False(t, result, "不允许覆盖时应返回 false。")
		assert.True(t, Exists(srcFile), "源文件应仍然存在。")
		assert.True(t, Exists(dstFile), "目标文件应仍然存在。")
	}

	// 目标文件已存在，允许覆盖
	{
		srcFile := filepath.Join(testDir, "src4.txt")
		dstFile := filepath.Join(testDir, "dst4.txt")
		require.True(t, Save(srcFile, "source"), "应能创建源文件。")
		require.True(t, Save(dstFile, "destination"), "应能创建目标文件。")

		result := Move(srcFile, dstFile, true)
		assert.True(t, result, "允许覆盖时应返回 true。")
		assert.False(t, Exists(srcFile), "源文件应被移动。")
		assert.True(t, Exists(dstFile), "目标文件应存在。")

		movedContent := Open[string](dstFile)
		assert.Equal(t, "source", movedContent, "目标文件内容应被覆盖。")
	}
}

func TestFileHash(t *testing.T) {
	testDir := filepath.Join(t.TempDir(), "FileHash")
	testContent := "Hello World!"
	testFile := filepath.Join(testDir, "Greet.txt")
	require.True(t, Save(testFile, testContent), "应能创建文件。")

	// 全量哈希
	{
		// MD5
		result := Hash(testFile, HashOptions{SegmentCount: 0, Algorithm: XString.HashAlgorithmMD5})
		assert.Equal(t, "ed076287532e86365e841e92bfc50d8c", result, "全量 MD5 哈希值应正确。")

		// SHA1
		result = Hash(testFile, HashOptions{SegmentCount: 0, Algorithm: XString.HashAlgorithmSHA1})
		assert.Equal(t, "2ef7bde608ce5404e97d5f042f95f89f1c232871", result, "全量 SHA1 哈希值应正确。")

		// SHA256
		result = Hash(testFile, HashOptions{SegmentCount: 0, Algorithm: XString.HashAlgorithmSHA256})
		assert.Equal(t, "7f83b1657ff1fc53b92dc18148a1d65dfc2d4b1fa3d677284addd200126d9069", result, "全量 SHA256 哈希值应正确。")
	}

	// 分段哈希
	{
		// MD5
		result := Hash(testFile, HashOptions{SegmentCount: 4, SegmentSize: 2, Algorithm: XString.HashAlgorithmMD5})
		assert.Equal(t, "8d46918ac7890c6d82f98c974ef54b93", result, "分段 MD5 哈希值应正确。")

		// SHA1
		result = Hash(testFile, HashOptions{SegmentCount: 4, SegmentSize: 2, Algorithm: XString.HashAlgorithmSHA1})
		assert.Equal(t, "2563bc97dbe03949d2d38844747268cff96e97b0", result, "分段 SHA1 哈希值应正确。")

		// SHA256
		result = Hash(testFile, HashOptions{SegmentCount: 4, SegmentSize: 2, Algorithm: XString.HashAlgorithmSHA256})
		assert.Equal(t, "456b637b98bc2373c0e94b38b89691912c57dba942ac2579e4f6f712862e0cc5", result, "分段 SHA256 哈希值应正确。")
	}

	// 边界测试
	{
		result := Hash(filepath.Join(testDir, "nonexistent.txt"))
		assert.Equal(t, "", result, "不存在的文件应返回空字符串。")
		emptyFile := filepath.Join(testDir, "Empty.txt")
		require.True(t, Save(emptyFile, ""), "应能创建空文件。")
		result = Hash(emptyFile)
		assert.Equal(t, "", result, "空文件应返回空字符串。")
	}
}
