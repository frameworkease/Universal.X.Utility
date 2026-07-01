// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System.IO;
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXFile
{
    [Test]
    public void FileExists()
    {
        var testDir = Path.Join(Path.GetTempPath(), "FileExists");
        // 不存在的文件
        Assert.That(XFile.Exists(Path.Join(testDir, "nonexistent.txt")), Is.False, "不存在的文件应返回 false。");

        // 创建文件
        var testFile = Path.Join(testDir, "test.txt");
        Assert.That(XFile.Save(testFile, "test content"), Is.True, "应能创建测试文件。");
        Assert.That(XFile.Exists(testFile), Is.True, "已存在的文件应返回 true。");

        // 目录路径应返回 false
        Assert.That(XFile.Exists(testDir), Is.False, "目录路径应返回 false。");
    }

    [Test]
    public void FileOpen()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        var testDir = Path.Join(Path.GetTempPath(), "FileOpen");
        var testFile = Path.Join(testDir, "test.txt");
        var content = "test content";

        // 保存文件
        Assert.That(XFile.Save(testFile, content), Is.True, "应能保存文件。");

        // 测试 Open<string>
        {
            var result = XFile.Open<string>(testFile);
            Assert.That(result, Is.EqualTo(content), "应能读取文件内容为 string。");
        }

        // 测试 Open<byte[]>
        {
            var result = XFile.Open<byte[]>(testFile);
            Assert.That(result, Is.EqualTo(System.Text.Encoding.UTF8.GetBytes(content)), "应能读取文件内容为 byte[]。");
        }

        // 不存在的文件
        {
            var nonexistent = Path.Join(testDir, "nonexistent.txt");
            var resultStr = XFile.Open<string>(nonexistent);
            Assert.That(resultStr, Is.Null, "不存在的文件应返回空 null string。");

            var resultBytes = XFile.Open<byte[]>(nonexistent);
            Assert.That(resultBytes, Is.Null, "不存在的文件应返回 null byte[]。");
        }
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void FileSave()
    {
        var testDir = Path.Join(Path.GetTempPath(), "FileSave");
        var tests = new[]
        {
            new { name = "保存 string", path = Path.Join(testDir, "string.txt"), data = (object)"test content", expected = true },
            new { name = "保存 byte[]", path = Path.Join(testDir, "bytes.txt"), data = (object)System.Text.Encoding.UTF8.GetBytes("test content"), expected = true },
            new { name = "保存到新目录", path = Path.Join(testDir, "subdir", "file.txt"), data = (object)"content", expected = true },
            new { name = "保存空内容", path = Path.Join(testDir, "empty.txt"), data = (object)"", expected = true },
            new { name = "保存空 byte[]", path = Path.Join(testDir, "empty_bytes.txt"), data = (object)new byte[0], expected = true },
        };

        foreach (var tt in tests)
        {
            bool result;
            if (tt.data is string str) result = XFile.Save(tt.path, str);
            else if (tt.data is byte[] bytes) result = XFile.Save(tt.path, bytes);
            else result = false;
            Assert.That(result, Is.EqualTo(tt.expected), $"保存文件结果不正确: {tt.name}。");
            if (tt.expected) Assert.That(XFile.Exists(tt.path), Is.True, $"文件应被成功创建: {tt.name}。");
        }
    }

    [Test]
    public void FileSize()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        var testDir = Path.Join(Path.GetTempPath(), "FileSize");

        // 不存在的文件
        {
            var result = XFile.Size(Path.Join(testDir, "nonexistent.txt"));
            Assert.That(result, Is.EqualTo(-1), "不存在的文件应返回 -1。");
        }

        // 空文件
        {
            var emptyFile = Path.Join(testDir, "empty.txt");
            Assert.That(XFile.Save(emptyFile, ""), Is.True, "应能创建空文件。");
            var result = XFile.Size(emptyFile);
            Assert.That(result, Is.EqualTo(0), "空文件应返回 0。");
        }

        // 有内容的文件
        {
            var content = "test content";
            var contentFile = Path.Join(testDir, "content.txt");
            Assert.That(XFile.Save(contentFile, content), Is.True, "应能创建文件。");
            var result = XFile.Size(contentFile);
            Assert.That(result, Is.EqualTo(content.Length), "文件大小应等于内容长度。");
        }
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void FileDelete()
    {
        var testDir = Path.Join(Path.GetTempPath(), "FileDelete");

        // 删除不存在的文件
        {
            var result = XFile.Delete(Path.Join(testDir, "nonexistent.txt"));
            Assert.That(result, Is.False, "删除不存在的文件应返回 false。");
        }

        // 删除存在的文件
        {
            var testFile = Path.Join(testDir, "test.txt");
            Assert.That(XFile.Save(testFile, "content"), Is.True, "应能创建文件。");
            Assert.That(XFile.Exists(testFile), Is.True, "文件应存在。");

            var result = XFile.Delete(testFile);
            Assert.That(result, Is.True, "应能删除文件。");
            Assert.That(XFile.Exists(testFile), Is.False, "文件应被删除。");
        }
    }

    [Test]
    public void FileCopy()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        var testDir = Path.Join(Path.GetTempPath(), "FileCopy");
        // 源文件不存在
        {
            var result = XFile.Copy(Path.Join(testDir, "nonexistent.txt"), Path.Join(testDir, "dst.txt"));
            Assert.That(result, Is.False, "源文件不存在时应返回 false。");
        }

        // 正常复制
        {
            var srcFile = Path.Join(testDir, "src.txt");
            var dstFile = Path.Join(testDir, "dst.txt");
            var content = "test content";
            Assert.That(XFile.Save(srcFile, content), Is.True, "应能创建源文件。");

            var result = XFile.Copy(srcFile, dstFile);
            Assert.That(result, Is.True, "应能复制文件。");
            Assert.That(XFile.Exists(dstFile), Is.True, "目标文件应存在。");

            var copiedContent = XFile.Open<string>(dstFile);
            Assert.That(copiedContent, Is.EqualTo(content), "复制的文件内容应一致。");
        }

        // 复制到新目录
        {
            var srcFile = Path.Join(testDir, "src2.txt");
            var dstFile = Path.Join(testDir, "subdir", "dst2.txt");
            var content = "test content 2";
            Assert.That(XFile.Save(srcFile, content), Is.True, "应能创建源文件。");

            var result = XFile.Copy(srcFile, dstFile);
            Assert.That(result, Is.True, "应能复制文件到新目录。");
            Assert.That(XFile.Exists(dstFile), Is.True, "目标文件应存在。");
        }

        // 目标文件已存在，不允许覆盖
        {
            var srcFile = Path.Join(testDir, "src3.txt");
            var dstFile = Path.Join(testDir, "dst3.txt");
            Assert.That(XFile.Save(srcFile, "source"), Is.True, "应能创建源文件。");
            Assert.That(XFile.Save(dstFile, "destination"), Is.True, "应能创建目标文件。");

            var result = XFile.Copy(srcFile, dstFile, false);
            Assert.That(result, Is.False, "不允许覆盖时应返回 false。");

            // 验证目标文件内容未改变
            var copiedContent = XFile.Open<string>(dstFile);
            Assert.That(copiedContent, Is.EqualTo("destination"), "目标文件内容不应改变。");
        }

        // 目标文件已存在，允许覆盖
        {
            var srcFile = Path.Join(testDir, "src4.txt");
            var dstFile = Path.Join(testDir, "dst4.txt");
            Assert.That(XFile.Save(srcFile, "source"), Is.True, "应能创建源文件。");
            Assert.That(XFile.Save(dstFile, "destination"), Is.True, "应能创建目标文件。");

            var result = XFile.Copy(srcFile, dstFile, true);
            Assert.That(result, Is.True, "允许覆盖时应返回 true。");

            // 验证目标文件内容已改变
            var copiedContent = XFile.Open<string>(dstFile);
            Assert.That(copiedContent, Is.EqualTo("source"), "目标文件内容应被覆盖。");
        }
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void FileMove()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        var testDir = Path.Join(Path.GetTempPath(), "FileMove");
        // 源文件不存在
        {
            var result = XFile.Move(Path.Join(testDir, "nonexistent.txt"), Path.Join(testDir, "dst.txt"));
            Assert.That(result, Is.False, "源文件不存在时应返回 false。");
        }

        // 正常移动
        {
            var srcFile = Path.Join(testDir, "src.txt");
            var dstFile = Path.Join(testDir, "dst.txt");
            var content = "test content";
            Assert.That(XFile.Save(srcFile, content), Is.True, "应能创建源文件。");

            var result = XFile.Move(srcFile, dstFile);
            Assert.That(result, Is.True, "应能移动文件。");
            Assert.That(XFile.Exists(srcFile), Is.False, "源文件应被移动。");
            Assert.That(XFile.Exists(dstFile), Is.True, "目标文件应存在。");

            var movedContent = XFile.Open<string>(dstFile);
            Assert.That(movedContent, Is.EqualTo(content), "移动的文件内容应一致。");
        }

        // 移动到新目录
        {
            var srcFile = Path.Join(testDir, "src2.txt");
            var dstFile = Path.Join(testDir, "subdir", "dst2.txt");
            var content = "test content 2";
            Assert.That(XFile.Save(srcFile, content), Is.True, "应能创建源文件。");

            var result = XFile.Move(srcFile, dstFile);
            Assert.That(result, Is.True, "应能移动文件到新目录。");
            Assert.That(XFile.Exists(srcFile), Is.False, "源文件应被移动。");
            Assert.That(XFile.Exists(dstFile), Is.True, "目标文件应存在。");
        }

        // 目标文件已存在，不允许覆盖
        {
            var srcFile = Path.Join(testDir, "src3.txt");
            var dstFile = Path.Join(testDir, "dst3.txt");
            Assert.That(XFile.Save(srcFile, "source"), Is.True, "应能创建源文件。");
            Assert.That(XFile.Save(dstFile, "destination"), Is.True, "应能创建目标文件。");

            var result = XFile.Move(srcFile, dstFile, false);
            Assert.That(result, Is.False, "不允许覆盖时应返回 false。");
            Assert.That(XFile.Exists(srcFile), Is.True, "源文件应仍然存在。");
            Assert.That(XFile.Exists(dstFile), Is.True, "目标文件应仍然存在。");
        }

        // 目标文件已存在，允许覆盖
        {
            var srcFile = Path.Join(testDir, "src4.txt");
            var dstFile = Path.Join(testDir, "dst4.txt");
            Assert.That(XFile.Save(srcFile, "source"), Is.True, "应能创建源文件。");
            Assert.That(XFile.Save(dstFile, "destination"), Is.True, "应能创建目标文件。");

            var result = XFile.Move(srcFile, dstFile, true);
            Assert.That(result, Is.True, "允许覆盖时应返回 true。");
            Assert.That(XFile.Exists(srcFile), Is.False, "源文件应被移动。");
            Assert.That(XFile.Exists(dstFile), Is.True, "目标文件应存在。");

            var movedContent = XFile.Open<string>(dstFile);
            Assert.That(movedContent, Is.EqualTo("source"), "目标文件内容应被覆盖。");
        }
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void FileHash()
    {
        var testDir = Path.Join(Path.GetTempPath(), "FileHash");
        var testContent = "Hello World!";
        var testFile = Path.Join(testDir, "Greet.txt");
        Assert.That(XFile.Save(testFile, testContent), Is.True, "应能创建文件。");

        // 全量哈希
        {
            // MD5
            var result = XFile.Hash(testFile, new XFile.HashOptions { SegmentCount = 0, Algorithm = XString.HashAlgorithm.MD5 });
            Assert.That(result, Is.EqualTo("ed076287532e86365e841e92bfc50d8c"), "全量 MD5 哈希值应正确。");

            // SHA1
            result = XFile.Hash(testFile, new XFile.HashOptions { SegmentCount = 0, Algorithm = XString.HashAlgorithm.SHA1 });
            Assert.That(result, Is.EqualTo("2ef7bde608ce5404e97d5f042f95f89f1c232871"), "全量 SHA1 哈希值应正确。");

            // SHA256
            result = XFile.Hash(testFile, new XFile.HashOptions { SegmentCount = 0, Algorithm = XString.HashAlgorithm.SHA256 });
            Assert.That(result, Is.EqualTo("7f83b1657ff1fc53b92dc18148a1d65dfc2d4b1fa3d677284addd200126d9069"), "全量 SHA256 哈希值应正确。");
        }

        // 分段哈希
        {
            // MD5
            var result = XFile.Hash(testFile, new XFile.HashOptions { SegmentCount = 4, SegmentSize = 2, Algorithm = XString.HashAlgorithm.MD5 });
            Assert.That(result, Is.EqualTo("8d46918ac7890c6d82f98c974ef54b93"), "分段 MD5 哈希值应正确。");

            // SHA1
            result = XFile.Hash(testFile, new XFile.HashOptions { SegmentCount = 4, SegmentSize = 2, Algorithm = XString.HashAlgorithm.SHA1 });
            Assert.That(result, Is.EqualTo("2563bc97dbe03949d2d38844747268cff96e97b0"), "分段 SHA1 哈希值应正确。");

            // SHA256
            result = XFile.Hash(testFile, new XFile.HashOptions { SegmentCount = 4, SegmentSize = 2, Algorithm = XString.HashAlgorithm.SHA256 });
            Assert.That(result, Is.EqualTo("456b637b98bc2373c0e94b38b89691912c57dba942ac2579e4f6f712862e0cc5"), "分段 SHA256 哈希值应正确。");
        }

        // 边界测试
        {
            var result = XFile.Hash(Path.Join(testDir, "nonexistent.txt"));
            Assert.That(result, Is.EqualTo(""), "不存在的文件应返回空字符串。");
            var emptyFile = Path.Join(testDir, "Empty.txt");
            Assert.That(XFile.Save(emptyFile, ""), Is.True, "应能创建空文件。");
            result = XFile.Hash(emptyFile);
            Assert.That(result, Is.EqualTo(""), "空文件应返回空字符串。");
        }
    }
}
