// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

#if !UNITY_5_3_OR_NEWER || (UNITY_5_3_OR_NEWER && !FRAMEWORKEASE_UNITY_XFILE_ARCHIVE_DISABLED)
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXFile
{
    [Test]
    public async Task ArchiveZip()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        var testDir = System.IO.Path.Join(System.IO.Path.GetTempPath(), "ArchiveZip");
        // 文件压缩
        {
            var srcFile = System.IO.Path.Join(testDir, "test.txt");
            var dstZip = System.IO.Path.Join(testDir, "test.zip");
            var extractedDir = System.IO.Path.Join(testDir, "extracted");
            var content = "Hello World!";
            Assert.That(XFile.Save(srcFile, content), Is.True, "应能创建源文件。");

            // 压缩
            {
                await XFile.Archive.Compress(srcFile, dstZip, new XFile.ArchiveOptions { Format = XFile.ArchiveFormat.Zip });
                Assert.That(XFile.Exists(dstZip), Is.True, "压缩文件应被创建。");
            }

            // 解压
            {
                await XFile.Archive.Extract(dstZip, extractedDir, new XFile.ArchiveOptions { Format = XFile.ArchiveFormat.Zip });
                var extractedFile = System.IO.Path.Join(extractedDir, "test.txt");
                Assert.That(XFile.Exists(extractedFile), Is.True, "解压后的文件应存在。");
                var actualContent = XFile.Open<string>(extractedFile);
                Assert.That(actualContent, Is.EqualTo(content), "解压后的文件内容应一致。");
            }
        }

        // 目录压缩
        {
            var srcDir = System.IO.Path.Join(testDir, "src_dir");
            var dstZip = System.IO.Path.Join(testDir, "dir.zip");
            var extractedDir = System.IO.Path.Join(testDir, "extracted_dir");

            XFile.Directory.Create(srcDir);
            XFile.Save(System.IO.Path.Join(srcDir, "file1.txt"), "content1");
            XFile.Save(System.IO.Path.Join(srcDir, "file2.txt"), "content2");
            XFile.Save(System.IO.Path.Join(srcDir, "exclude.txt"), "exclude");
            XFile.Directory.Create(System.IO.Path.Join(srcDir, "subdir"));
            XFile.Save(System.IO.Path.Join(srcDir, "subdir", "file3.txt"), "content3");

            // 压缩
            {
                bool onCompleteCalled = false;
                var onProgressValues = new List<double>();

                var options = new XFile.ArchiveOptions
                {
                    OnProgress = (progress) => { onProgressValues.Add(progress); },
                    OnComplete = () => { onCompleteCalled = true; },
                };

                await XFile.Archive.Compress(srcDir, dstZip, options);

                Assert.That(XFile.Exists(dstZip), Is.True, "压缩文件应被创建。");
                Assert.That(onCompleteCalled, Is.True, "压缩 OnComplete 回调应被调用。");
                Assert.That(onProgressValues.Count, Is.EqualTo(4), "压缩 OnProgress 回调应被调用 4 次。");
                Assert.That(onProgressValues.Last(), Is.GreaterThanOrEqualTo(100.0), "压缩最终进度应达到 100%。");
            }

            // 解压
            {
                bool onCompleteCalled = false;
                var onProgressValues = new List<double>();

                var options = new XFile.ArchiveOptions
                {
                    OnProgress = (progress) => { onProgressValues.Add(progress); },
                    OnComplete = () => { onCompleteCalled = true; },
                };

                await XFile.Archive.Extract(dstZip, extractedDir, options);

                Assert.That(onCompleteCalled, Is.True, "解压 OnComplete 回调应被调用。");
                Assert.That(onProgressValues.Count, Is.EqualTo(4), "解压 OnProgress 回调应被调用 4 次。");
                Assert.That(onProgressValues.Last(), Is.GreaterThanOrEqualTo(100.0), "解压最终进度应达到 100%。");
                Assert.That(XFile.Exists(System.IO.Path.Join(extractedDir, "file1.txt")), Is.True, "解压后的文件应存在。");
                Assert.That(XFile.Exists(System.IO.Path.Join(extractedDir, "file2.txt")), Is.True, "解压后的文件应存在。");
                Assert.That(XFile.Exists(System.IO.Path.Join(extractedDir, "subdir", "file3.txt")), Is.True, "解压后的子目录文件应存在。");
                Assert.That(XFile.Exists(System.IO.Path.Join(extractedDir, "exclude.txt")), Is.True, "解压后的文件应存在。");
                Assert.That(XFile.Open<string>(System.IO.Path.Join(extractedDir, "file1.txt")), Is.EqualTo("content1"), "解压后的文件内容应一致。");
                Assert.That(XFile.Open<string>(System.IO.Path.Join(extractedDir, "file2.txt")), Is.EqualTo("content2"), "解压后的文件内容应一致。");
                Assert.That(XFile.Open<string>(System.IO.Path.Join(extractedDir, "subdir", "file3.txt")), Is.EqualTo("content3"), "解压后的子目录文件内容应一致。");
                Assert.That(XFile.Open<string>(System.IO.Path.Join(extractedDir, "exclude.txt")), Is.EqualTo("exclude"), "解压后的文件内容应一致。");
            }
        }

        // 边界测试
        {
            // 源文件不存在
            {
                string errorOccurred = string.Empty;
                var options = new XFile.ArchiveOptions
                {
                    OnError = (err) => { errorOccurred = err; },
                };
                await XFile.Archive.Compress(System.IO.Path.Join(testDir, "nonexistent.txt"), System.IO.Path.Join(testDir, "error1.zip"), options);
                Assert.That(errorOccurred, Is.Not.Null.And.Not.Empty, "源文件不存在时应触发 OnError 回调。");
            }

            // 压缩文件不存在
            {
                string errorOccurred = string.Empty;
                var options = new XFile.ArchiveOptions
                {
                    OnError = (err) => { errorOccurred = err; },
                };
                await XFile.Archive.Extract(System.IO.Path.Join(testDir, "nonexistent.zip"), System.IO.Path.Join(testDir, "error_extracted"), options);
                Assert.That(errorOccurred, Is.Not.Empty, "压缩文件不存在时应触发 OnError 回调。");
            }

            // 压缩文件无效
            {
                var invalidZip = System.IO.Path.Join(testDir, "invalid.zip");
                XFile.Save(invalidZip, "not a zip file");

                var extractedDir = System.IO.Path.Join(testDir, "invalid_extracted");
                string errorOccurred = string.Empty;
                var options = new XFile.ArchiveOptions
                {
                    OnError = (err) => { errorOccurred = err; },
                };
                await XFile.Archive.Extract(invalidZip, extractedDir, options);
                Assert.That(errorOccurred, Is.Not.Empty, "解压无效压缩文件时应触发 OnError 回调。");
            }
        }
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }
}
#endif
