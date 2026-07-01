// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXTime
{
    [Test]
    public void Constants()
    {
        #region 秒级
        {
            Assert.That(XTime.Second1, Is.EqualTo(1));
            Assert.That(XTime.Second2, Is.EqualTo(2));
            Assert.That(XTime.Second3, Is.EqualTo(3));
            Assert.That(XTime.Second4, Is.EqualTo(4));
            Assert.That(XTime.Second5, Is.EqualTo(5));
            Assert.That(XTime.Second6, Is.EqualTo(6));
            Assert.That(XTime.Second7, Is.EqualTo(7));
            Assert.That(XTime.Second8, Is.EqualTo(8));
            Assert.That(XTime.Second9, Is.EqualTo(9));
            Assert.That(XTime.Second10, Is.EqualTo(10));
            Assert.That(XTime.Second15, Is.EqualTo(15));
            Assert.That(XTime.Second20, Is.EqualTo(20));
            Assert.That(XTime.Second25, Is.EqualTo(25));
            Assert.That(XTime.Second30, Is.EqualTo(30));
            Assert.That(XTime.Second35, Is.EqualTo(35));
            Assert.That(XTime.Second40, Is.EqualTo(40));
            Assert.That(XTime.Second45, Is.EqualTo(45));
            Assert.That(XTime.Second50, Is.EqualTo(50));
            Assert.That(XTime.Second55, Is.EqualTo(55));
        }
        #endregion

        #region 分钟级
        {
            Assert.That(XTime.Minute1, Is.EqualTo(60));
            Assert.That(XTime.Minute2, Is.EqualTo(120));
            Assert.That(XTime.Minute3, Is.EqualTo(180));
            Assert.That(XTime.Minute4, Is.EqualTo(240));
            Assert.That(XTime.Minute5, Is.EqualTo(300));
            Assert.That(XTime.Minute6, Is.EqualTo(360));
            Assert.That(XTime.Minute7, Is.EqualTo(420));
            Assert.That(XTime.Minute8, Is.EqualTo(480));
            Assert.That(XTime.Minute9, Is.EqualTo(540));
            Assert.That(XTime.Minute10, Is.EqualTo(600));
            Assert.That(XTime.Minute12, Is.EqualTo(720));
            Assert.That(XTime.Minute15, Is.EqualTo(900));
            Assert.That(XTime.Minute20, Is.EqualTo(1200));
            Assert.That(XTime.Minute25, Is.EqualTo(1500));
            Assert.That(XTime.Minute30, Is.EqualTo(1800));
            Assert.That(XTime.Minute35, Is.EqualTo(2100));
            Assert.That(XTime.Minute40, Is.EqualTo(2400));
            Assert.That(XTime.Minute45, Is.EqualTo(2700));
            Assert.That(XTime.Minute50, Is.EqualTo(3000));
            Assert.That(XTime.Minute55, Is.EqualTo(3300));
        }
        #endregion

        #region 小时级
        {
            Assert.That(XTime.Hour1, Is.EqualTo(3600));
            Assert.That(XTime.Hour2, Is.EqualTo(7200));
            Assert.That(XTime.Hour3, Is.EqualTo(10800));
            Assert.That(XTime.Hour4, Is.EqualTo(14400));
            Assert.That(XTime.Hour5, Is.EqualTo(18000));
            Assert.That(XTime.Hour6, Is.EqualTo(21600));
            Assert.That(XTime.Hour7, Is.EqualTo(25200));
            Assert.That(XTime.Hour8, Is.EqualTo(28800));
            Assert.That(XTime.Hour9, Is.EqualTo(32400));
            Assert.That(XTime.Hour10, Is.EqualTo(36000));
            Assert.That(XTime.Hour11, Is.EqualTo(39600));
            Assert.That(XTime.Hour12, Is.EqualTo(43200));
            Assert.That(XTime.Hour13, Is.EqualTo(46800));
            Assert.That(XTime.Hour14, Is.EqualTo(50400));
            Assert.That(XTime.Hour15, Is.EqualTo(54000));
            Assert.That(XTime.Hour16, Is.EqualTo(57600));
            Assert.That(XTime.Hour17, Is.EqualTo(61200));
            Assert.That(XTime.Hour18, Is.EqualTo(64800));
            Assert.That(XTime.Hour19, Is.EqualTo(68400));
            Assert.That(XTime.Hour20, Is.EqualTo(72000));
            Assert.That(XTime.Hour21, Is.EqualTo(75600));
            Assert.That(XTime.Hour22, Is.EqualTo(79200));
            Assert.That(XTime.Hour23, Is.EqualTo(82800));
        }
        #endregion

        #region 天级
        {
            Assert.That(XTime.Day1, Is.EqualTo(86400));
            Assert.That(XTime.Day2, Is.EqualTo(172800));
            Assert.That(XTime.Day3, Is.EqualTo(259200));
            Assert.That(XTime.Day4, Is.EqualTo(345600));
            Assert.That(XTime.Day5, Is.EqualTo(432000));
            Assert.That(XTime.Day6, Is.EqualTo(518400));
            Assert.That(XTime.Day7, Is.EqualTo(604800));
            Assert.That(XTime.Day8, Is.EqualTo(691200));
            Assert.That(XTime.Day9, Is.EqualTo(777600));
            Assert.That(XTime.Day10, Is.EqualTo(864000));
            Assert.That(XTime.Day15, Is.EqualTo(1296000));
            Assert.That(XTime.Day20, Is.EqualTo(1728000));
            Assert.That(XTime.Day30, Is.EqualTo(2592000));
        }
        #endregion
    }
}
