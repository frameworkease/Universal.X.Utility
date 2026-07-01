// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XFile

import (
	"os"
	"path/filepath"
	"sync"
	"testing"

	"github.com/frameworkease/Universal.X.Utility/Go/XString"
	"github.com/stretchr/testify/assert"
	"github.com/stretchr/testify/require"
)

func TestDirectoryExists(t *testing.T) {
	// 测试准备
	testDir := filepath.Join(t.TempDir(), "DirectoryExists")
	testFile := filepath.Join(testDir, "test.txt")
	assert.True(t, Save(testFile, "test content"), "创建文件应当成功。")

	// 常规测试
	assert.True(t, Directory.Create(testDir), "创建目录应当成功。")
	assert.True(t, Directory.Exists(testDir), "新建目录应当存在。")

	// 边界测试
	assert.False(t, Directory.Exists(testFile), "文件路径应当不存在。")
	assert.False(t, Directory.Exists("NonExistingDirectory"), "不存在的目录应当不存在。")
}

func TestDirectoryCreate(t *testing.T) {
	// 测试准备
	testDir := filepath.Join(t.TempDir(), "DirectoryCreate")

	// 常规测试
	assert.True(t, Directory.Create(testDir), "创建目录应当成功。")
	assert.True(t, Directory.Exists(testDir), "新建目录应当存在。")
	assert.True(t, Directory.Create(testDir), "再次创建应当成功。")
	assert.True(t, Directory.Create(testDir, true), "覆盖重建应当成功。")
}

func TestDirectoryDelete(t *testing.T) {
	// 测试准备
	testDir := filepath.Join(t.TempDir(), "DirectoryDelete")
	testFile := filepath.Join(testDir, "test.txt")
	nestedDir := filepath.Join(testDir, "nested")
	nestedDirFile := filepath.Join(nestedDir, "test.txt")
	emptyDir := filepath.Join(testDir, "empty")
	assert.True(t, Save(testFile, "test content"), "创建文件应当成功。")
	assert.True(t, Directory.Create(nestedDir), "创建目录应当成功。")
	assert.True(t, Save(nestedDirFile, "nested content"), "创建文件应当成功。")
	assert.True(t, Directory.Create(emptyDir), "创建空目录应当成功。")

	// 常规测试
	assert.False(t, Directory.Delete(nestedDir, false), "非空目录应当删除失败。")
	assert.True(t, Directory.Delete(emptyDir, false), "空目录应当删除成功。")
	assert.True(t, Directory.Delete(testDir), "目录应当删除成功。")
	assert.False(t, Directory.Exists(testDir), "目录应当不存在。")
	assert.False(t, Directory.Exists(emptyDir), "目录应当不存在。")
	assert.False(t, Directory.Exists(nestedDir), "目录应当不存在。")
	assert.False(t, Exists(nestedDirFile), "文件应当不存在。")
	assert.False(t, Exists(testFile), "文件应当不存在。")

	// 边界测试
	assert.False(t, Directory.Delete("NonExistingDirectory"), "删除不存在的目录应当失败。")
}

func TestDirectoryCopy(t *testing.T) {
	// 测试准备
	testDir := filepath.Join(t.TempDir(), "DirectoryCopy")
	srcDir := filepath.Join(testDir, "src")
	dstDir := filepath.Join(testDir, "dst")
	subDir := filepath.Join(srcDir, "subdir")
	excludeFile := filepath.Join(srcDir, "exclude.tmp")
	normalFile := filepath.Join(srcDir, "normal.txt")
	subFile := filepath.Join(subDir, "subfile.txt")
	assert.True(t, Directory.Create(srcDir), "创建源目录应当成功。")
	assert.True(t, Directory.Create(subDir), "创建子目录应当成功。")
	assert.True(t, Save(excludeFile, "exclude content"), "创建排除文件应当成功。")
	assert.True(t, Save(normalFile, "normal content"), "创建普通文件应当成功。")
	assert.True(t, Save(subFile, "sub content"), "创建子目录文件应当成功。")

	// 常规测试
	assert.True(t, Directory.Copy(srcDir, dstDir, `.*\.tmp$`), "复制操作应成功。")
	assert.True(t, Directory.Exists(dstDir), "目标目录应被成功创建。")
	assert.False(t, Exists(filepath.Join(dstDir, "exclude.tmp")), "被排除的文件不应被复制。")
	assert.True(t, Exists(filepath.Join(dstDir, "normal.txt")), "普通文件应被成功复制。")
	assert.Equal(t, "normal content", Open[string](filepath.Join(dstDir, "normal.txt")), "复制的文件内容应保持一致。")
	assert.True(t, Exists(filepath.Join(dstDir, "subdir", "subfile.txt")), "子目录文件应被成功复制。")
	assert.Equal(t, "sub content", Open[string](filepath.Join(dstDir, "subdir", "subfile.txt")), "复制的文件内容应保持一致。")

	// 边界测试
	assert.False(t, Directory.Copy("NonExistingSource", filepath.Join(testDir, "dst")), "源目录不存在时应失败。")
}

func TestDirectoryMove(t *testing.T) {
	// 测试准备
	testDir := filepath.Join(t.TempDir(), "DirectoryMove")
	srcDir := filepath.Join(testDir, "src")
	dstDir := filepath.Join(testDir, "dst")
	srcFile := filepath.Join(srcDir, "nested", "file.txt")
	assert.True(t, Directory.Create(srcDir), "创建源目录应当成功。")
	assert.True(t, Save(srcFile, "content"), "创建文件应当成功。")

	// 常规测试
	for i := range 3 {
		var src, dst string
		if i%2 == 0 {
			src = srcDir
			dst = dstDir
		} else {
			src = dstDir
			dst = srcDir
		}
		assert.True(t, Directory.Create(dst), "创建目标目录应当成功。")
		dstFile := filepath.Join(dst, "nested", "file.txt")
		assert.True(t, Directory.Move(src, dst, true), "移动操作应成功。")
		assert.False(t, Directory.Exists(src), "源目录应被移动。")
		assert.True(t, Directory.Exists(dst), "目标目录应存在。")
		assert.True(t, Exists(dstFile), "文件应被移动。")
		assert.Equal(t, "content", Open[string](dstFile), "文件内容应正确。")
	}

	// 边界测试
	assert.False(t, Directory.Move(srcDir, srcDir, false), "目标目录已存在且不允许覆盖时应失败。")
	assert.False(t, Directory.Move("NonExistingSource", filepath.Join(testDir, "dst")), "源目录不存在时应失败。")
}

func TestDirectoryWalk(t *testing.T) {
	// 测试准备
	testDir := filepath.Join(t.TempDir(), "DirectoryWalk")
	Directory.Create(filepath.Join(testDir, "dir1"))
	Directory.Create(filepath.Join(testDir, "dir2"))
	Save(filepath.Join(testDir, "file1.txt"), "content1")
	Save(filepath.Join(testDir, "file2.txt"), "content2")
	Save(filepath.Join(testDir, "dir1", "file3.txt"), "content3")

	// 全量遍历
	var walkedPaths []string
	Directory.Walk(testDir, func(path string, info os.FileInfo) bool {
		walkedPaths = append(walkedPaths, path)
		return true
	})
	require.Greater(t, len(walkedPaths), 0, "应遍历到文件或目录。")
	assert.Contains(t, walkedPaths, testDir, "应包含根目录。")
	assert.Contains(t, walkedPaths, filepath.Join(testDir, "file1.txt"), "应包含文件。")
	assert.Contains(t, walkedPaths, filepath.Join(testDir, "dir1"), "应包含子目录。")

	// 条件遍历
	walkedCount := 0
	Directory.Walk(testDir, func(path string, info os.FileInfo) bool {
		walkedCount++
		if walkedCount >= 2 {
			return false
		}
		return true
	})
	assert.Equal(t, 2, walkedCount, "应在遍历 2 个项后停止。")

	// 边界测试
	Directory.Walk(testDir, nil) // 应该不会 panic，只打印错误信息
	walked := false
	Directory.Walk("NonExistingPath", func(path string, info os.FileInfo) bool { walked = true; return true })
	assert.False(t, walked, "不存在的路径不应被遍历。")
}

func TestDirectoryProject(t *testing.T) {
	projectPath := Directory.Project()
	assert.NotEmpty(t, projectPath, "Project 路径不应为空。")
	assert.Equal(t, projectPath, Directory.Project(), "多次访问 Project 应返回相同值。")
}

func TestDirectoryAsset(t *testing.T) {
	defer func() {
		Directory.assetTestArgs = Directory.assetTestArgs[:0]
		Directory.assetInit = sync.Once{}
		Directory.assetValue = ""
	}()

	{
		Directory.assetInit = sync.Once{}
		Directory.assetValue = ""
		Directory.assetTestArgs = Directory.assetTestArgs[:0]

		assetPath := Directory.Asset()
		assert.NotEmpty(t, assetPath, "Asset 路径不应为空。")
		assert.Equal(t, assetPath, Directory.Asset(), "多次访问 Asset 应返回相同值。")
	}

	{
		Directory.assetInit = sync.Once{}
		Directory.assetValue = ""
		Directory.assetTestArgs = Directory.assetTestArgs[:0]
		Directory.assetTestArgs = append(Directory.assetTestArgs, "-XFile.Directory.Asset=/custom/path")

		assetPath := Directory.Asset()
		assert.Equal(t, "/custom/path", assetPath, "应能通过 -XFile.Directory.Asset=<path> 参数格式设置 Asset 路径。")
	}

	{
		Directory.assetInit = sync.Once{}
		Directory.assetValue = ""
		Directory.assetTestArgs = Directory.assetTestArgs[:0]
		Directory.assetTestArgs = append(Directory.assetTestArgs, "-XFile.Directory.Asset", "/custom/path")

		assetPath := Directory.Asset()
		assert.Equal(t, "/custom/path", assetPath, "应能通过 -XFile.Directory.Asset <path> 参数格式设置 Asset 路径。")
	}

	{
		Directory.assetInit = sync.Once{}
		Directory.assetValue = ""
		Directory.assetTestArgs = Directory.assetTestArgs[:0]
		Directory.assetTestArgs = append(Directory.assetTestArgs, "--XFile.Directory.Asset=/custom/path")

		assetPath := Directory.Asset()
		assert.Equal(t, "/custom/path", assetPath, "应能通过 --XFile.Directory.Asset=<path> 参数格式设置 Asset 路径。")
	}

	{
		Directory.assetInit = sync.Once{}
		Directory.assetValue = ""
		Directory.assetTestArgs = Directory.assetTestArgs[:0]
		Directory.assetTestArgs = append(Directory.assetTestArgs, "--XFile.Directory.Asset", "/custom/path")

		assetPath := Directory.Asset()
		assert.Equal(t, "/custom/path", assetPath, "应能通过 --XFile.Directory.Asset <path> 参数格式设置 Asset 路径。")
	}
}

func TestDirectoryLocal(t *testing.T) {
	defer func() {
		Directory.localTestArgs = Directory.localTestArgs[:0]
		Directory.localInit = sync.Once{}
		Directory.localValue = ""
	}()

	{
		Directory.localInit = sync.Once{}
		Directory.localValue = ""
		Directory.localTestArgs = Directory.localTestArgs[:0]

		localPath := Directory.Local()
		assert.NotEmpty(t, localPath, "Local 路径不应为空。")
		assert.True(t, Directory.Exists(localPath), "Local 目录应已自动创建。")
		assert.Equal(t, localPath, Directory.Local(), "多次访问 Local 应返回相同值。")
	}

	{
		testDir := filepath.Join(os.TempDir(), "custom", "path", "1")
		Directory.localInit = sync.Once{}
		Directory.localValue = ""
		Directory.localTestArgs = Directory.localTestArgs[:0]
		Directory.localTestArgs = append(Directory.localTestArgs, "-XFile.Directory.Local="+testDir)

		localPath := Directory.Local()
		assert.Equal(t, testDir, localPath, "应能通过 -XFile.Directory.Local=<path> 参数格式设置 Local 路径。")
		assert.True(t, Directory.Exists(localPath), "通过参数设置的 Local 目录应已自动创建。")
	}

	{
		testDir := filepath.Join(os.TempDir(), "custom", "path", "2")
		Directory.localInit = sync.Once{}
		Directory.localValue = ""
		Directory.localTestArgs = Directory.localTestArgs[:0]
		Directory.localTestArgs = append(Directory.localTestArgs, "-XFile.Directory.Local", testDir)

		localPath := Directory.Local()
		assert.Equal(t, testDir, localPath, "应能通过 -XFile.Directory.Local <path> 参数格式设置 Local 路径。")
		assert.True(t, Directory.Exists(localPath), "通过参数设置的 Local 目录应已自动创建。")
	}

	{
		testDir := filepath.Join(os.TempDir(), "custom", "path", "3")
		Directory.localInit = sync.Once{}
		Directory.localValue = ""
		Directory.localTestArgs = Directory.localTestArgs[:0]
		Directory.localTestArgs = append(Directory.localTestArgs, "--XFile.Directory.Local="+testDir)

		localPath := Directory.Local()
		assert.Equal(t, testDir, localPath, "应能通过 --XFile.Directory.Local=<path> 参数格式设置 Local 路径。")
		assert.True(t, Directory.Exists(localPath), "通过参数设置的 Local 目录应已自动创建。")
	}

	{
		testDir := filepath.Join(os.TempDir(), "custom", "path", "4")
		Directory.localInit = sync.Once{}
		Directory.localValue = ""
		Directory.localTestArgs = Directory.localTestArgs[:0]
		Directory.localTestArgs = append(Directory.localTestArgs, "--XFile.Directory.Local", testDir)

		localPath := Directory.Local()
		assert.Equal(t, testDir, localPath, "应能通过 --XFile.Directory.Local <path> 参数格式设置 Local 路径。")
		assert.True(t, Directory.Exists(localPath), "通过参数设置的 Local 目录应已自动创建。")
	}
}

func TestDirectoryEvaluator(t *testing.T) {
	assert.Equal(t, Directory.Project(), XString.Evaluate("${XFile.Directory.Project}", Directory.Evaluator), "Directory.Project 应当正确求值。")
	assert.Equal(t, Directory.Local(), XString.Evaluate("${XFile.Directory.Local}", Directory.Evaluator), "Directory.Local 应当正确求值。")
	assert.Equal(t, Directory.Asset(), XString.Evaluate("${XFile.Directory.Asset}", Directory.Evaluator), "Directory.Asset 应当正确求值。")
	assert.Equal(t, "${Unknown.XFile.Directory.Missing}", XString.Evaluate("${XFile.Directory.Missing}", Directory.Evaluator), "Directory.Missing 应当返回 Unknown。")
}
