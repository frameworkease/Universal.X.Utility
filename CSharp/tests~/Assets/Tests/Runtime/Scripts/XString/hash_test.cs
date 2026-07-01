// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXString
{
    [Test]
    public void Hash()
    {
        #region MD5
        {
            Assert.That("Hello".Hash(), Is.EqualTo("8b1a9953c4611296a827abf8c47804d7"), "MD5 哈希值应正确。");
            Assert.That("Hello World".Hash(), Is.EqualTo("b10a8db164e0754105b7a99be72e3fe5"), "Hello World 的 MD5 哈希值应正确。");
            Assert.That("".Hash(), Is.EqualTo(string.Empty), "空字符串应返回空字符串。");
            Assert.That(((string)null).Hash(), Is.EqualTo(string.Empty), "null 输入应返回空字符串。");
        }
        #endregion

        #region SHA1
        {
            Assert.That("Hello".Hash(XString.HashAlgorithm.SHA1), Is.EqualTo("f7ff9e8b7bb2e09b70935a5d785e0cc5d9d0abf0"), "SHA1 哈希值应正确。");
            Assert.That("Hello World".Hash(XString.HashAlgorithm.SHA1), Is.EqualTo("0a4d55a8d778e5022fab701977c5d840bbc486d0"), "Hello World 的 SHA1 哈希值应正确。");
            Assert.That("".Hash(XString.HashAlgorithm.SHA1), Is.EqualTo(string.Empty), "空字符串应返回空字符串。");
            Assert.That(((string)null).Hash(XString.HashAlgorithm.SHA1), Is.EqualTo(string.Empty), "null 输入应返回空字符串。");
        }
        #endregion

        #region SHA256
        {
            Assert.That("Hello".Hash(XString.HashAlgorithm.SHA256), Is.EqualTo("185f8db32271fe25f561a6fc938b2e264306ec304eda518007d1764826381969"), "SHA256 哈希值应正确。");
            Assert.That("Hello World".Hash(XString.HashAlgorithm.SHA256), Is.EqualTo("a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e"), "Hello World 的 SHA256 哈希值应正确。");
            Assert.That("".Hash(XString.HashAlgorithm.SHA256), Is.EqualTo(string.Empty), "空字符串应返回空字符串。");
            Assert.That(((string)null).Hash(XString.HashAlgorithm.SHA256), Is.EqualTo(string.Empty), "null 输入应返回空字符串。");
        }
        #endregion
    }
}
