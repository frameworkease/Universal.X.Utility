// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System.Collections.Generic;
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXFile
{
    [Test]
    public void ManifestCompare()
    {
        // Arrange
        var manifest1 = new XFile.Manifest();
        manifest1.Files.Add(new XFile.FileInfo { Name = "file1.txt", Hash = "md5_1", Size = 100 });
        manifest1.Files.Add(new XFile.FileInfo { Name = "file2.txt", Hash = "md5_2", Size = 200 });

        var manifest2 = new XFile.Manifest();
        manifest2.Files.Add(new XFile.FileInfo { Name = "file1.txt", Hash = "md5_1", Size = 100 });
        manifest2.Files.Add(new XFile.FileInfo { Name = "file3.txt", Hash = "md5_3", Size = 300 });

        // Act
        var diff = manifest1.Compare(manifest2);

        // Assert
        Assert.That(diff.Deleted.Count, Is.EqualTo(1), "应检测到一个被删除的文件（file2.txt）。");
        Assert.That(diff.Added.Count, Is.EqualTo(1), "应检测到一个新增的文件（file3.txt）。");
        Assert.That(diff.Modified.Count, Is.EqualTo(0), "不应有被修改的文件。");
    }

    [Test]
    public void ManifestStringify()
    {
        // Arrange
        var manifest = new XFile.Manifest();
        manifest.Files.Add(new XFile.FileInfo { Name = "file1.txt", Hash = "md5_1", Size = 100 });
        manifest.Files.Add(new XFile.FileInfo { Name = "file2.txt", Hash = "md5_2", Size = 200 });

        // Act
        var result = manifest.Stringify(true);

        // Assert
        Assert.That(result, Is.Not.Empty, "清单文本不应为空。");
        var parsedFiles = JSON.Parse<List<XFile.FileInfo>>(result);
        Assert.That(parsedFiles, Is.Not.Null, "清单文本应能正确解析。");
        Assert.That(parsedFiles.Count, Is.EqualTo(2), "清单应包含两个文件记录。");
        Assert.That(parsedFiles[0].Name, Is.EqualTo("file1.txt"), "第一个文件名称应正确。");
        Assert.That(parsedFiles[0].Hash, Is.EqualTo("md5_1"), "第一个文件 MD5 应正确。");
        Assert.That(parsedFiles[0].Size, Is.EqualTo(100), "第一个文件大小应正确。");
        Assert.That(parsedFiles[1].Name, Is.EqualTo("file2.txt"), "第二个文件名称应正确。");
        Assert.That(parsedFiles[1].Hash, Is.EqualTo("md5_2"), "第二个文件 MD5 应正确。");
        Assert.That(parsedFiles[1].Size, Is.EqualTo(200), "第二个文件大小应正确。");
    }

    [Test]
    public void ManifestParse()
    {
        // Arrange
        var manifest = new XFile.Manifest();
        manifest.Files.AddRange(new List<XFile.FileInfo>
        {
            new() { Name = "file1.txt", Hash = "d41d8cd98f00b204e9800998ecf8427e", Size = 0 },
            new() { Name = "file2.txt", Hash = "d41d8cd98f00b204e9800998ecf8427e", Size = 123 }
        });
        var data = manifest.Stringify(false);

        // Act
        var result = manifest.Parse(data);

        // Assert
        Assert.That(result, Is.Empty, "解析过程不应产生错误信息。");
        Assert.That(manifest.Files.Count, Is.EqualTo(2), "清单应包含两个文件记录。");
        Assert.That(manifest.Files[0].Name, Is.EqualTo("file1.txt"), "第一个文件名称应正确解析。");
        Assert.That(manifest.Files[0].Size, Is.EqualTo(0), "第一个文件大小应为 0。");
        Assert.That(manifest.Files[1].Name, Is.EqualTo("file2.txt"), "第二个文件名称应正确解析。");
        Assert.That(manifest.Files[1].Size, Is.EqualTo(123), "第二个文件大小应为 123。");
    }
}
