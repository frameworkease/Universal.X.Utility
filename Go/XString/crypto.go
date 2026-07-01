// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XString

import (
	"bytes"
	"crypto/aes"
	"crypto/cipher"
	"crypto/des"
	"encoding/base64"
)

type SymmetricAlgorithm uint8

const (
	SymmetricAlgorithmAES SymmetricAlgorithm = iota
	SymmetricAlgorithmDES
)

type CryptoOptions struct {
	Algorithm SymmetricAlgorithm
	Key       string
	IV        string
}

var defaultAESKey = []byte{
	0xA1, 0xB2, 0xC3, 0xD4, 0x55, 0x66, 0x77, 0x88,
	0x09, 0x1A, 0x2B, 0x3C, 0x4D, 0x5E, 0x6F, 0x10,
}

var defaultAESIV = []byte{
	0x2F, 0x8E, 0x4D, 0x9A, 0x6B, 0x1C, 0x3D, 0x7E,
	0x5A, 0x8B, 0x2C, 0x9D, 0x4E, 0x1F, 0x6A, 0x3B,
}

var defaultDESKey = []byte{0x7B, 0x4A, 0xF3, 0x91, 0xE5, 0xD2, 0x8C, 0x6F}

var defaultDESIV = []byte{0x2A, 0x9F, 0x5C, 0x8E, 0x1B, 0x4D, 0x7A, 0x3C}

func encryptAES(data []byte, key string, iv string) []byte {
	aesKeyBytes := defaultAESKey
	if key != "" {
		keyBytes := []byte(key)
		if len(keyBytes) > 32 {
			keyBytes = keyBytes[:32]
		} else if len(keyBytes) > 24 {
			keyBytes = append(keyBytes, make([]byte, 32-len(keyBytes))...)
		} else if len(keyBytes) > 16 {
			keyBytes = append(keyBytes, make([]byte, 24-len(keyBytes))...)
		} else {
			keyBytes = append(keyBytes, make([]byte, 16-len(keyBytes))...)
		}
		aesKeyBytes = keyBytes
	}

	aesIVBytes := defaultAESIV
	if iv != "" {
		ivBytes := []byte(iv)
		if len(ivBytes) > 16 {
			ivBytes = ivBytes[:16]
		} else {
			ivBytes = append(ivBytes, make([]byte, 16-len(ivBytes))...)
		}
		aesIVBytes = ivBytes
	}

	block, err := aes.NewCipher(aesKeyBytes)
	if err != nil {
		return nil
	}

	data = pkcs7Padding(data, block.BlockSize())
	crypted := make([]byte, len(data))
	mode := cipher.NewCBCEncrypter(block, aesIVBytes)
	mode.CryptBlocks(crypted, data)

	return crypted
}

func encryptDES(data []byte, key string, iv string) []byte {
	desKey := defaultDESKey
	if key != "" {
		desKey = padKey([]byte(key))
	}

	desIV := defaultDESIV
	if iv != "" {
		ivBytes := []byte(iv)
		if len(ivBytes) > 8 {
			ivBytes = ivBytes[:8]
		} else {
			ivBytes = append(ivBytes, make([]byte, 8-len(ivBytes))...)
		}
		desIV = ivBytes
	}

	block, err := des.NewCipher(desKey)
	if err != nil {
		return nil
	}

	data = pkcs7Padding(data, block.BlockSize())
	crypted := make([]byte, len(data))
	mode := cipher.NewCBCEncrypter(block, desIV)
	mode.CryptBlocks(crypted, data)

	return crypted
}

func decryptAES(data []byte, key string, iv string) []byte {
	aesKeyBytes := defaultAESKey
	if key != "" {
		keyBytes := []byte(key)
		if len(keyBytes) > 32 {
			keyBytes = keyBytes[:32]
		} else if len(keyBytes) > 24 {
			keyBytes = append(keyBytes, make([]byte, 32-len(keyBytes))...)
		} else if len(keyBytes) > 16 {
			keyBytes = append(keyBytes, make([]byte, 24-len(keyBytes))...)
		} else {
			keyBytes = append(keyBytes, make([]byte, 16-len(keyBytes))...)
		}
		aesKeyBytes = keyBytes
	}

	aesIVBytes := defaultAESIV
	if iv != "" {
		ivBytes := []byte(iv)
		if len(ivBytes) > 16 {
			ivBytes = ivBytes[:16]
		} else {
			ivBytes = append(ivBytes, make([]byte, 16-len(ivBytes))...)
		}
		aesIVBytes = ivBytes
	}

	block, err := aes.NewCipher(aesKeyBytes)
	if err != nil {
		return nil
	}

	if len(data) < aes.BlockSize {
		return nil
	}

	decrypted := make([]byte, len(data))
	mode := cipher.NewCBCDecrypter(block, aesIVBytes)
	mode.CryptBlocks(decrypted, data)

	return pkcs7UnPadding(decrypted)
}

func decryptDES(data []byte, key string, iv string) []byte {
	desKey := defaultDESKey
	if key != "" {
		desKey = padKey([]byte(key))
	}

	desIV := defaultDESIV
	if iv != "" {
		ivBytes := []byte(iv)
		if len(ivBytes) > 8 {
			ivBytes = ivBytes[:8]
		} else {
			ivBytes = append(ivBytes, make([]byte, 8-len(ivBytes))...)
		}
		desIV = ivBytes
	}

	block, err := des.NewCipher(desKey)
	if err != nil {
		return nil
	}

	if len(data) < des.BlockSize {
		return nil
	}

	decrypted := make([]byte, len(data))
	mode := cipher.NewCBCDecrypter(block, desIV)
	mode.CryptBlocks(decrypted, data)

	return pkcs7UnPadding(decrypted)
}

func padKey(key []byte) []byte {
	if len(key) > 8 {
		return key[:8]
	}
	return append(key, bytes.Repeat([]byte{0}, 8-len(key))...)
}

func pkcs7Padding(data []byte, blockSize int) []byte {
	padding := blockSize - len(data)%blockSize
	padtext := bytes.Repeat([]byte{byte(padding)}, padding)
	return append(data, padtext...)
}

func pkcs7UnPadding(data []byte) []byte {
	length := len(data)
	if length == 0 {
		return data
	}
	unpadding := int(data[length-1])
	if unpadding > length {
		return data
	}
	return data[:(length - unpadding)]
}

func Encrypt[T string | []byte](input T, options ...CryptoOptions) T {
	var zero T

	// 处理选项，设置默认值
	algorithm := SymmetricAlgorithmAES
	key := ""
	iv := ""
	if len(options) > 0 {
		opt := options[0]
		algorithm = opt.Algorithm
		key = opt.Key
		iv = opt.IV
	}

	// 检查输入
	var data []byte
	switch v := any(input).(type) {
	case string:
		if v == "" {
			return zero
		}
		data = []byte(v)
	case []byte:
		if len(v) == 0 {
			return zero
		}
		data = v
	default:
		return zero
	}

	// 加密
	var encrypted []byte
	switch algorithm {
	case SymmetricAlgorithmAES:
		encrypted = encryptAES(data, key, iv)
	case SymmetricAlgorithmDES:
		encrypted = encryptDES(data, key, iv)
	default:
		return zero
	}

	if encrypted == nil {
		return zero
	}

	// 根据输入类型返回相同类型
	switch any(zero).(type) {
	case string:
		return any(base64.StdEncoding.EncodeToString(encrypted)).(T)
	case []byte:
		return any(encrypted).(T)
	default:
		return zero
	}
}

func Decrypt[T string | []byte](input T, options ...CryptoOptions) T {
	var zero T

	algorithm := SymmetricAlgorithmAES
	key := ""
	iv := ""
	if len(options) > 0 {
		opt := options[0]
		algorithm = opt.Algorithm
		key = opt.Key
		iv = opt.IV
	}

	// 检查输入
	var data []byte
	switch v := any(input).(type) {
	case string:
		if v == "" {
			return zero
		}
		decoded, err := base64.StdEncoding.DecodeString(v)
		if err != nil {
			return zero
		}
		data = decoded
	case []byte:
		if len(v) == 0 {
			return zero
		}
		data = v
	default:
		return zero
	}

	// 解密
	var decrypted []byte
	switch algorithm {
	case SymmetricAlgorithmAES:
		decrypted = decryptAES(data, key, iv)
	case SymmetricAlgorithmDES:
		decrypted = decryptDES(data, key, iv)
	default:
		return zero
	}

	if decrypted == nil {
		return zero
	}

	// 根据输入类型返回相同类型
	switch any(zero).(type) {
	case string:
		return any(string(decrypted)).(T)
	case []byte:
		return any(decrypted).(T)
	default:
		return zero
	}
}
