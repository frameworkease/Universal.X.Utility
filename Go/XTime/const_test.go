// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XTime

import (
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestConstants(t *testing.T) {
	// 秒级
	assert.Equal(t, 1, Second1)
	assert.Equal(t, 2, Second2)
	assert.Equal(t, 3, Second3)
	assert.Equal(t, 4, Second4)
	assert.Equal(t, 5, Second5)
	assert.Equal(t, 6, Second6)
	assert.Equal(t, 7, Second7)
	assert.Equal(t, 8, Second8)
	assert.Equal(t, 9, Second9)
	assert.Equal(t, 10, Second10)
	assert.Equal(t, 15, Second15)
	assert.Equal(t, 20, Second20)
	assert.Equal(t, 25, Second25)
	assert.Equal(t, 30, Second30)
	assert.Equal(t, 35, Second35)
	assert.Equal(t, 40, Second40)
	assert.Equal(t, 45, Second45)
	assert.Equal(t, 50, Second50)
	assert.Equal(t, 55, Second55)

	// 分钟级
	assert.Equal(t, 60, Minute1)
	assert.Equal(t, 120, Minute2)
	assert.Equal(t, 180, Minute3)
	assert.Equal(t, 240, Minute4)
	assert.Equal(t, 300, Minute5)
	assert.Equal(t, 360, Minute6)
	assert.Equal(t, 420, Minute7)
	assert.Equal(t, 480, Minute8)
	assert.Equal(t, 540, Minute9)
	assert.Equal(t, 600, Minute10)
	assert.Equal(t, 720, Minute12)
	assert.Equal(t, 900, Minute15)
	assert.Equal(t, 1200, Minute20)
	assert.Equal(t, 1500, Minute25)
	assert.Equal(t, 1800, Minute30)
	assert.Equal(t, 2100, Minute35)
	assert.Equal(t, 2400, Minute40)
	assert.Equal(t, 2700, Minute45)
	assert.Equal(t, 3000, Minute50)
	assert.Equal(t, 3300, Minute55)

	// 小时级
	assert.Equal(t, 3600, Hour1)
	assert.Equal(t, 7200, Hour2)
	assert.Equal(t, 10800, Hour3)
	assert.Equal(t, 14400, Hour4)
	assert.Equal(t, 18000, Hour5)
	assert.Equal(t, 21600, Hour6)
	assert.Equal(t, 25200, Hour7)
	assert.Equal(t, 28800, Hour8)
	assert.Equal(t, 32400, Hour9)
	assert.Equal(t, 36000, Hour10)
	assert.Equal(t, 39600, Hour11)
	assert.Equal(t, 43200, Hour12)
	assert.Equal(t, 46800, Hour13)
	assert.Equal(t, 50400, Hour14)
	assert.Equal(t, 54000, Hour15)
	assert.Equal(t, 57600, Hour16)
	assert.Equal(t, 61200, Hour17)
	assert.Equal(t, 64800, Hour18)
	assert.Equal(t, 68400, Hour19)
	assert.Equal(t, 72000, Hour20)
	assert.Equal(t, 75600, Hour21)
	assert.Equal(t, 79200, Hour22)
	assert.Equal(t, 82800, Hour23)

	// 天级
	assert.Equal(t, 86400, Day1)
	assert.Equal(t, 172800, Day2)
	assert.Equal(t, 259200, Day3)
	assert.Equal(t, 345600, Day4)
	assert.Equal(t, 432000, Day5)
	assert.Equal(t, 518400, Day6)
	assert.Equal(t, 604800, Day7)
	assert.Equal(t, 691200, Day8)
	assert.Equal(t, 777600, Day9)
	assert.Equal(t, 864000, Day10)
	assert.Equal(t, 1296000, Day15)
	assert.Equal(t, 1728000, Day20)
	assert.Equal(t, 2592000, Day30)
}
