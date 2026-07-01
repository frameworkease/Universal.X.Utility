// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XString

import (
	"testing"

	"github.com/stretchr/testify/assert"
)

// TestHash 测试哈希功能
func TestHash(t *testing.T) {
	t.Run("MD5", func(t *testing.T) {
		assert.Equal(t, "8b1a9953c4611296a827abf8c47804d7", Hash("Hello"), "MD5 哈希值应正确。")
		assert.Equal(t, "b10a8db164e0754105b7a99be72e3fe5", Hash("Hello World"), "Hello World 的 MD5 哈希值应正确。")
		assert.Equal(t, "", Hash(""), "空字符串应返回空字符串。")
		assert.Equal(t, "", Hash([]byte(nil)), "nil 输入应返回空字符串。")
	})

	t.Run("SHA1", func(t *testing.T) {
		assert.Equal(t, "f7ff9e8b7bb2e09b70935a5d785e0cc5d9d0abf0", Hash("Hello", HashAlgorithmSHA1), "SHA1 哈希值应正确。")
		assert.Equal(t, "0a4d55a8d778e5022fab701977c5d840bbc486d0", Hash("Hello World", HashAlgorithmSHA1), "Hello World 的 SHA1 哈希值应正确。")
		assert.Equal(t, "", Hash("", HashAlgorithmSHA1), "空字符串应返回空字符串。")
		assert.Equal(t, "", Hash([]byte(nil), HashAlgorithmSHA1), "nil 输入应返回空字符串。")
	})

	t.Run("SHA256", func(t *testing.T) {
		assert.Equal(t, "185f8db32271fe25f561a6fc938b2e264306ec304eda518007d1764826381969", Hash("Hello", HashAlgorithmSHA256), "SHA256 哈希值应正确。")
		assert.Equal(t, "a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e", Hash("Hello World", HashAlgorithmSHA256), "Hello World 的 SHA256 哈希值应正确。")
		assert.Equal(t, "", Hash("", HashAlgorithmSHA256), "空字符串应返回空字符串。")
		assert.Equal(t, "", Hash([]byte(nil), HashAlgorithmSHA256), "nil 输入应返回空字符串。")
	})
}
