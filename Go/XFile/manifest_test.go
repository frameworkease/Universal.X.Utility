// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XFile

import (
	"encoding/json"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestManifestCompare(t *testing.T) {
	t.Run("Added&Deleted", func(t *testing.T) {
		// Arrange
		manifest1 := new(Manifest)
		manifest1.Files = []*FileInfo{
			{Name: "file1.txt", Hash: "md5_1", Size: 100},
			{Name: "file2.txt", Hash: "md5_2", Size: 200},
		}

		manifest2 := new(Manifest)
		manifest2.Files = []*FileInfo{
			{Name: "file1.txt", Hash: "md5_1", Size: 100},
			{Name: "file3.txt", Hash: "md5_3", Size: 300},
		}

		// Act
		diff := manifest1.Compare(manifest2)

		// Assert
		assert.Equal(t, 1, len(diff.Deleted), "应检测到一个被删除的文件（file2.txt）。")
		assert.Equal(t, "file2.txt", diff.Deleted[0].Name, "被删除的文件应为 file2.txt。")
		assert.Equal(t, 1, len(diff.Added), "应检测到一个新增的文件（file3.txt）。")
		assert.Equal(t, "file3.txt", diff.Added[0].Name, "新增的文件应为 file3.txt。")
		assert.Equal(t, 0, len(diff.Modified), "不应有被修改的文件。")
	})

	t.Run("Modified", func(t *testing.T) {
		// Arrange
		manifest1 := new(Manifest)
		manifest1.Files = []*FileInfo{
			{Name: "file1.txt", Hash: "md5_1", Size: 100},
			{Name: "file2.txt", Hash: "md5_2", Size: 200},
		}

		manifest2 := new(Manifest)
		manifest2.Files = []*FileInfo{
			{Name: "file1.txt", Hash: "md5_1_modified", Size: 150},
			{Name: "file2.txt", Hash: "md5_2", Size: 200},
		}

		// Act
		diff := manifest1.Compare(manifest2)

		// Assert
		assert.Equal(t, 0, len(diff.Deleted), "不应有被删除的文件。")
		assert.Equal(t, 0, len(diff.Added), "不应有新增的文件。")
		assert.Equal(t, 1, len(diff.Modified), "应检测到一个被修改的文件（file1.txt）。")
		assert.Equal(t, "file1.txt", diff.Modified[0].Name, "被修改的文件应为 file1.txt。")
		assert.Equal(t, "md5_1_modified", diff.Modified[0].Hash, "修改后的 MD5 应正确。")
	})
}

func TestManifestStringify(t *testing.T) {
	// Arrange
	manifest := new(Manifest)
	manifest.Files = []*FileInfo{
		{Name: "file1.txt", Hash: "md5_1", Size: 100},
		{Name: "file2.txt", Hash: "md5_2", Size: 200},
	}

	// Act
	result := manifest.Stringify(true)

	// Assert
	assert.NotEmpty(t, result, "清单文本不应为空。")
	var parsedFiles []*FileInfo
	err := json.Unmarshal([]byte(result), &parsedFiles)
	assert.NoError(t, err, "清单文本应能正确解析。")
	assert.NotNil(t, parsedFiles, "清单文本应能正确解析。")
	assert.Equal(t, 2, len(parsedFiles), "清单应包含两个文件记录。")
	assert.Equal(t, "file1.txt", parsedFiles[0].Name, "第一个文件名称应正确。")
	assert.Equal(t, "md5_1", parsedFiles[0].Hash, "第一个文件 MD5 应正确。")
	assert.Equal(t, int64(100), parsedFiles[0].Size, "第一个文件大小应正确。")
	assert.Equal(t, "file2.txt", parsedFiles[1].Name, "第二个文件名称应正确。")
	assert.Equal(t, "md5_2", parsedFiles[1].Hash, "第二个文件 MD5 应正确。")
	assert.Equal(t, int64(200), parsedFiles[1].Size, "第二个文件大小应正确。")
}

func TestManifestParse(t *testing.T) {
	// Arrange
	manifest := new(Manifest)
	manifest.Files = []*FileInfo{
		{Name: "file1.txt", Hash: "d41d8cd98f00b204e9800998ecf8427e", Size: 0},
		{Name: "file2.txt", Hash: "d41d8cd98f00b204e9800998ecf8427e", Size: 123},
	}
	data := manifest.Stringify(false)

	// Act
	err := manifest.Parse([]byte(data))

	// Assert
	assert.NoError(t, err, "清单解析应该成功完成。")
	assert.Equal(t, 2, len(manifest.Files), "清单应包含两个文件记录。")
	assert.Equal(t, "file1.txt", manifest.Files[0].Name, "第一个文件名称应正确解析。")
	assert.Equal(t, int64(0), manifest.Files[0].Size, "第一个文件大小应为 0。")
	assert.Equal(t, "file2.txt", manifest.Files[1].Name, "第二个文件名称应正确解析。")
	assert.Equal(t, int64(123), manifest.Files[1].Size, "第二个文件大小应为 123。")
}
