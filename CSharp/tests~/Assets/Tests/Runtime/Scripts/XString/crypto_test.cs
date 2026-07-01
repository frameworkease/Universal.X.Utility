// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System.Text;
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXString
{
    [Test]
    public void Crypto()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        #region AES
        {
            var inputStr = "Hello, World!";
            Assert.That(inputStr.Encrypt().Decrypt(), Is.EqualTo(inputStr), "AES 加密解密后的文本应与原文相同。");
            Assert.That(inputStr.Encrypt(key: "12345678abcdefgh", iv: "abcdefgh12345678").Decrypt(key: "12345678abcdefgh", iv: "abcdefgh12345678"), Is.EqualTo(inputStr), "使用自定义密钥的 AES 加密解密后的文本应与原文相同。");

            var inputBytes = Encoding.UTF8.GetBytes(inputStr);
            Assert.That(inputBytes.Encrypt().Decrypt(), Is.EqualTo(inputBytes), "AES 加密解密后的字节数组应与原文相同。");
            Assert.That(inputBytes.Encrypt(key: "12345678abcdefgh", iv: "abcdefgh12345678").Decrypt(key: "12345678abcdefgh", iv: "abcdefgh12345678"), Is.EqualTo(inputBytes), "使用自定义密钥的 AES 加密解密后的字节数组应与原文相同。");

            Assert.That(inputStr.Encrypt(key: "wrongkey", iv: "wrongiv"), Is.Null, "错误的密钥和向量加密应返回空字符串。");
            Assert.That(inputStr.Decrypt(key: "wrongkey", iv: "wrongiv"), Is.Null, "错误的密钥和向量解密应返回空字符串。");
            Assert.That("".Encrypt(), Is.Null, "空字符串加密应返回空字符串。");
            Assert.That("".Decrypt(), Is.Null, "空字符串解密应返回空字符串。");
            Assert.That(((string)null).Encrypt(), Is.Null, "null 输入加密应返回空字符串。");
            Assert.That(((string)null).Decrypt(), Is.Null, "null 输入解密应返回空字符串。");
            Assert.That("InvalidBase64!@#".Decrypt(), Is.Null, "无效 Base64 字符串解密应返回空字符串。");
        }
        #endregion

        #region DES
        {
            var inputStr = "Hello, World!";
            Assert.That(inputStr.Encrypt(XString.SymmetricAlgorithm.DES).Decrypt(XString.SymmetricAlgorithm.DES), Is.EqualTo(inputStr), "DES 加密解密后的文本应与原文相同。");
            Assert.That(inputStr.Encrypt(XString.SymmetricAlgorithm.DES, key: "12345678", iv: "abcdefgh").Decrypt(XString.SymmetricAlgorithm.DES, key: "12345678", iv: "abcdefgh"), Is.EqualTo(inputStr), "使用自定义密钥的 DES 加密解密后的文本应与原文相同。");

            var inputBytes = Encoding.UTF8.GetBytes(inputStr);
            Assert.That(inputBytes.Encrypt(XString.SymmetricAlgorithm.DES).Decrypt(XString.SymmetricAlgorithm.DES), Is.EqualTo(inputBytes), "DES 加密解密后的字节数组应与原文相同。");
            Assert.That(inputBytes.Encrypt(XString.SymmetricAlgorithm.DES, key: "12345678", iv: "abcdefgh").Decrypt(XString.SymmetricAlgorithm.DES, key: "12345678", iv: "abcdefgh"), Is.EqualTo(inputBytes), "使用自定义密钥的 DES 加密解密后的字节数组应与原文相同。");

            Assert.That(inputStr.Encrypt(XString.SymmetricAlgorithm.DES, key: "wrongkey", iv: "wrongiv"), Is.Null, "错误的密钥和向量加密应返回空字符串。");
            Assert.That(inputStr.Decrypt(XString.SymmetricAlgorithm.DES, key: "wrongkey", iv: "wrongiv"), Is.Null, "错误的密钥和向量解密应返回空字符串。");
            Assert.That("".Encrypt(XString.SymmetricAlgorithm.AES), Is.Null, "空字符串加密应返回空字符串。");
            Assert.That("".Decrypt(XString.SymmetricAlgorithm.AES), Is.Null, "空字符串解密应返回空字符串。");
            Assert.That(((string)null).Encrypt(XString.SymmetricAlgorithm.AES), Is.Null, "null 输入加密应返回空字符串。");
            Assert.That(((string)null).Decrypt(XString.SymmetricAlgorithm.AES), Is.Null, "null 输入解密应返回空字符串。");
            Assert.That("InvalidBase64!@#".Decrypt(XString.SymmetricAlgorithm.AES), Is.Null, "无效 Base64 字符串解密应返回空字符串。");
        }
        #endregion

#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }
}
