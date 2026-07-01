// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XString

import (
	"crypto/md5"
	"crypto/sha1"
	"crypto/sha256"
	"encoding/hex"
)

type HashAlgorithm uint8

const (
	HashAlgorithmMD5 HashAlgorithm = iota
	HashAlgorithmSHA1
	HashAlgorithmSHA256
)

func Hash[T string | []byte](input T, algorithm ...HashAlgorithm) string {
	var data []byte
	switch v := any(input).(type) {
	case string:
		if v == "" {
			return ""
		}
		data = []byte(v)
	case []byte:
		if v == nil {
			return ""
		}
		data = v
	default:
		return ""
	}

	alg := HashAlgorithmMD5
	if len(algorithm) > 0 {
		alg = algorithm[0]
	}

	var hash []byte
	switch alg {
	case HashAlgorithmMD5:
		h := md5.Sum(data)
		hash = h[:]
	case HashAlgorithmSHA1:
		h := sha1.Sum(data)
		hash = h[:]
	case HashAlgorithmSHA256:
		h := sha256.Sum256(data)
		hash = h[:]
	default:
		return ""
	}

	return hex.EncodeToString(hash)
}
