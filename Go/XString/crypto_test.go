// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XString

import (
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestCrypto(t *testing.T) {
	t.Run("AES", func(t *testing.T) {
		inputStr := "Hello, World!"
		assert.Equal(t, inputStr, Decrypt(Encrypt(inputStr)), "AES 加密解密后的文本应与原文相同。")
		assert.Equal(t, inputStr, Decrypt(Encrypt(inputStr, CryptoOptions{Algorithm: SymmetricAlgorithmAES, Key: "12345678abcdefgh", IV: "abcdefgh12345678"}), CryptoOptions{Algorithm: SymmetricAlgorithmAES, Key: "12345678abcdefgh", IV: "abcdefgh12345678"}), "使用自定义密钥的 AES 加密解密后的文本应与原文相同。")

		inputBytes := []byte(inputStr)
		assert.Equal(t, inputBytes, Decrypt(Encrypt(inputBytes)), "AES 加密解密后的字节数组应与原文相同。")
		assert.Equal(t, inputBytes, Decrypt(Encrypt(inputBytes, CryptoOptions{Algorithm: SymmetricAlgorithmAES, Key: "12345678abcdefgh", IV: "abcdefgh12345678"}), CryptoOptions{Algorithm: SymmetricAlgorithmAES, Key: "12345678abcdefgh", IV: "abcdefgh12345678"}), "使用自定义密钥的 AES 加密解密后的字节数组应与原文相同。")

		assert.Equal(t, "", Encrypt(""), "空字符串加密应返回空字符串。")
		assert.Equal(t, "", Decrypt(""), "空字符串解密应返回空字符串。")
		assert.Equal(t, "", Decrypt("InvalidBase64!@#"), "无效 Base64 字符串解密应返回空字符串。")
	})

	t.Run("DES", func(t *testing.T) {
		inputStr := "Hello, World!"
		assert.Equal(t, inputStr, Decrypt(Encrypt(inputStr, CryptoOptions{Algorithm: SymmetricAlgorithmDES}), CryptoOptions{Algorithm: SymmetricAlgorithmDES}), "DES 加密解密后的文本应与原文相同。")
		assert.Equal(t, inputStr, Decrypt(Encrypt(inputStr, CryptoOptions{Algorithm: SymmetricAlgorithmDES, Key: "12345678", IV: "abcdefgh"}), CryptoOptions{Algorithm: SymmetricAlgorithmDES, Key: "12345678", IV: "abcdefgh"}), "使用自定义密钥的 DES 加密解密后的文本应与原文相同。")

		inputBytes := []byte(inputStr)
		assert.Equal(t, inputBytes, Decrypt(Encrypt(inputBytes, CryptoOptions{Algorithm: SymmetricAlgorithmDES}), CryptoOptions{Algorithm: SymmetricAlgorithmDES}), "DES 加密解密后的字节数组应与原文相同。")
		assert.Equal(t, inputBytes, Decrypt(Encrypt(inputBytes, CryptoOptions{Algorithm: SymmetricAlgorithmDES, Key: "12345678", IV: "abcdefgh"}), CryptoOptions{Algorithm: SymmetricAlgorithmDES, Key: "12345678", IV: "abcdefgh"}), "使用自定义密钥的 DES 加密解密后的字节数组应与原文相同。")

		assert.Equal(t, "", Encrypt("", CryptoOptions{Algorithm: SymmetricAlgorithmDES}), "空字符串加密应返回空字符串。")
		assert.Equal(t, "", Decrypt("", CryptoOptions{Algorithm: SymmetricAlgorithmDES}), "空字符串解密应返回空字符串。")
		assert.Equal(t, "", Decrypt("InvalidBase64!@#", CryptoOptions{Algorithm: SymmetricAlgorithmDES}), "无效 Base64 字符串解密应返回空字符串。")
	})
}
