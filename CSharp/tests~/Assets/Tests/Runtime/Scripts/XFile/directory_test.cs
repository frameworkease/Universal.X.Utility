// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System.IO;
using FrameworkEase.Universal.X.Utility;
using NUnit.Framework;

public partial class TestXFile
{
    [Test]
    public void DirectoryExists()
    {
        var testDir = Path.Join(Path.GetTempPath(), "DirectoryExists");
        var testFile = Path.Join(testDir, "test.txt");
        Assert.That(XFile.Save(testFile, "test content"), Is.True, "创建文件应当成功。");

        Assert.That(XFile.Directory.Create(testDir), Is.True, "创建目录应当成功。");
        Assert.That(XFile.Directory.Exists(testDir), Is.True, "新建目录应当存在。");

        Assert.That(XFile.Directory.Exists(testFile), Is.False, "文件路径应当不存在。");
        Assert.That(XFile.Directory.Exists("NonExistingDirectory"), Is.False, "不存在的目录应当不存在。");
    }

    [Test]
    public void DirectoryCreate()
    {
        var testDir = Path.Join(Path.GetTempPath(), "DirectoryCreate");
        var nestedDir = Path.Join(testDir, "nested");

        Assert.That(XFile.Directory.Create(nestedDir), Is.True, "创建目录应当成功。");
        Assert.That(XFile.Directory.Exists(nestedDir), Is.True, "新建目录应当存在。");
        Assert.That(XFile.Directory.Create(nestedDir), Is.True, "再次创建应当成功。");
        Assert.That(XFile.Directory.Create(nestedDir, true), Is.True, "覆盖重建应当成功。");
    }

    [Test]
    public void DirectoryDelete()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        var testDir = Path.Join(Path.GetTempPath(), "DirectoryDelete");
        var testFile = Path.Join(testDir, "test.txt");
        var nestedDir = Path.Join(testDir, "nested");
        var nestedDirFile = Path.Join(nestedDir, "test.txt");
        var emptyDir = Path.Join(testDir, "empty");
        Assert.That(XFile.Save(testFile, "test content"), Is.True, "创建文件应当成功。");
        Assert.That(XFile.Directory.Create(nestedDir), Is.True, "创建目录应当成功。");
        Assert.That(XFile.Save(nestedDirFile, "nested content"), Is.True, "创建文件应当成功。");
        Assert.That(XFile.Directory.Create(emptyDir), Is.True, "创建空目录应当成功。");

        Assert.That(XFile.Directory.Delete(nestedDir, false), Is.False, "非空目录应当删除失败。");
        Assert.That(XFile.Directory.Delete(emptyDir, false), Is.True, "空目录应当删除成功。");
        Assert.That(XFile.Directory.Delete(testDir), Is.True, "目录应当删除成功。");
        Assert.That(XFile.Directory.Exists(testDir), Is.False, "目录应当不存在。");
        Assert.That(XFile.Directory.Exists(emptyDir), Is.False, "目录应当不存在。");
        Assert.That(XFile.Directory.Exists(nestedDir), Is.False, "目录应当不存在。");
        Assert.That(XFile.Exists(nestedDirFile), Is.False, "文件应当不存在。");
        Assert.That(XFile.Exists(testFile), Is.False, "文件应当不存在。");

        Assert.That(XFile.Directory.Delete("NonExistingDirectory"), Is.False, "删除不存在的目录应当失败。");
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void DirectoryCopy()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        var testDir = Path.Join(Path.GetTempPath(), "DirectoryCopy");
        var srcDir = Path.Join(testDir, "src");
        var dstDir = Path.Join(testDir, "dst");
        var subDir = Path.Join(srcDir, "subdir");
        var excludeFile = Path.Join(srcDir, "exclude.tmp");
        var normalFile = Path.Join(srcDir, "normal.txt");
        var subFile = Path.Join(subDir, "subfile.txt");
        Assert.That(XFile.Directory.Create(srcDir), Is.True, "创建源目录应当成功。");
        Assert.That(XFile.Directory.Create(subDir), Is.True, "创建子目录应当成功。");
        Assert.That(XFile.Save(excludeFile, "exclude content"), Is.True, "创建排除文件应当成功。");
        Assert.That(XFile.Save(normalFile, "normal content"), Is.True, "创建普通文件应当成功。");
        Assert.That(XFile.Save(subFile, "sub content"), Is.True, "创建子目录文件应当成功。");

        Assert.That(XFile.Directory.Copy(srcDir, dstDir, @".*\.tmp$"), Is.True, "复制操作应成功。");
        Assert.That(XFile.Directory.Exists(dstDir), Is.True, "目标目录应被成功创建。");
        Assert.That(XFile.Exists(Path.Join(dstDir, "exclude.tmp")), Is.False, "被排除的文件不应被复制。");
        Assert.That(XFile.Exists(Path.Join(dstDir, "normal.txt")), Is.True, "普通文件应被成功复制。");
        Assert.That(XFile.Open<string>(Path.Join(dstDir, "normal.txt")), Is.EqualTo("normal content"), "复制的文件内容应保持一致。");
        Assert.That(XFile.Exists(Path.Join(dstDir, "subdir", "subfile.txt")), Is.True, "子目录文件应被成功复制。");
        Assert.That(XFile.Open<string>(Path.Join(dstDir, "subdir", "subfile.txt")), Is.EqualTo("sub content"), "复制的文件内容应保持一致。");

        Assert.That(XFile.Directory.Copy("NonExistingSource", Path.Join(testDir, "dst")), Is.False, "源目录不存在时应失败。");
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void DirectoryMove()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        var testDir = Path.Join(Path.GetTempPath(), "DirectoryMove");
        var srcDir = Path.Join(testDir, "src");
        var dstDir = Path.Join(testDir, "dst");
        var srcFile = Path.Join(srcDir, "nested", "file.txt");
        Assert.That(XFile.Directory.Create(srcDir), Is.True, "创建源目录应当成功。");
        Assert.That(XFile.Save(srcFile, "content"), Is.True, "创建文件应当成功。");

        for (int i = 0; i < 3; i++)
        {
            string src, dst;
            if (i % 2 == 0)
            {
                src = srcDir;
                dst = dstDir;
            }
            else
            {
                src = dstDir;
                dst = srcDir;
            }
            Assert.That(XFile.Directory.Create(dst), Is.True, "创建目标目录应当成功。");
            var dstFile = Path.Join(dst, "nested", "file.txt");
            Assert.That(XFile.Directory.Move(src, dst, true), Is.True, "移动操作应成功。");
            Assert.That(XFile.Directory.Exists(src), Is.False, "源目录应被移动。");
            Assert.That(XFile.Directory.Exists(dst), Is.True, "目标目录应存在。");
            Assert.That(XFile.Exists(dstFile), Is.True, "文件应被移动。");
            Assert.That(XFile.Open<string>(dstFile), Is.EqualTo("content"), "文件内容应正确。");
        }

        Assert.That(XFile.Directory.Move(srcDir, srcDir, false), Is.False, "目标目录已存在且不允许覆盖时应失败。");
        Assert.That(XFile.Directory.Move("NonExistingSource", Path.Join(testDir, "dst")), Is.False, "源目录不存在时应失败。");
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void DirectoryWalk()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
#endif
        var testDir = Path.Join(Path.GetTempPath(), "DirectoryWalk");
        XFile.Directory.Create(Path.Join(testDir, "dir1"));
        XFile.Directory.Create(Path.Join(testDir, "dir2"));
        XFile.Save(Path.Join(testDir, "file1.txt"), "content1");
        XFile.Save(Path.Join(testDir, "file2.txt"), "content2");
        XFile.Save(Path.Join(testDir, "dir1", "file3.txt"), "content3");

        var walkedPaths = new System.Collections.Generic.List<string>();
        XFile.Directory.Walk(testDir, (path, info) =>
        {
            walkedPaths.Add(path);
            return true;
        });
        Assert.That(walkedPaths.Count, Is.GreaterThan(0), "应遍历到文件或目录。");
        Assert.That(walkedPaths, Contains.Item(testDir), "应包含根目录。");
        Assert.That(walkedPaths, Contains.Item(Path.Join(testDir, "file1.txt")), "应包含文件。");
        Assert.That(walkedPaths, Contains.Item(Path.Join(testDir, "dir1")), "应包含子目录。");

        int walkedCount = 0;
        XFile.Directory.Walk(testDir, (path, info) =>
        {
            walkedCount++;
            if (walkedCount >= 2) return false;
            return true;
        });
        Assert.That(walkedCount, Is.EqualTo(2), "应在遍历 2 个项后停止。");

        XFile.Directory.Walk(testDir, null);
        bool walked = false;
        XFile.Directory.Walk("NonExistingPath", (path, info) => { walked = true; return true; });
        Assert.That(walked, Is.False, "不存在的路径不应被遍历。");
#if UNITY_5_3_OR_NEWER
        UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
#endif
    }

    [Test]
    public void DirectoryProject()
    {
        var projectPath = XFile.Directory.Project;
        Assert.That(projectPath, Is.Not.Empty, "Project 路径不应为空。");
        Assert.That(XFile.Directory.Project, Is.EqualTo(projectPath), "多次访问 Project 应返回相同值。");
    }

    [Test]
    public void DirectoryAsset()
    {
        try
        {
            {
                XFile.Directory.assetInit = false;
                XFile.Directory.assetValue = null;
                XFile.Directory.assetTestArgs.Clear();

                var assetPath = XFile.Directory.Asset;
                Assert.That(assetPath, Is.Not.Empty, "Asset 路径不应为空。");
                Assert.That(XFile.Directory.Asset, Is.EqualTo(assetPath), "多次访问 Asset 应返回相同值。");
            }

#if !UNITY_5_3_OR_NEWER
            {
                XFile.Directory.assetInit = false;
                XFile.Directory.assetValue = null;
                XFile.Directory.assetTestArgs.Clear();
                XFile.Directory.assetTestArgs.Add("-XFile.Directory.Asset=/custom/path");
                Assert.That(XFile.Directory.Asset, Is.EqualTo("/custom/path"), "应能通过 -XFile.Directory.Asset=<path> 参数格式设置 Asset 路径。");
            }

            {
                XFile.Directory.assetInit = false;
                XFile.Directory.assetValue = null;
                XFile.Directory.assetTestArgs.Clear();
                XFile.Directory.assetTestArgs.Add("-XFile.Directory.Asset");
                XFile.Directory.assetTestArgs.Add("/custom/path");
                Assert.That(XFile.Directory.Asset, Is.EqualTo("/custom/path"), "应能通过 -XFile.Directory.Asset <path> 参数格式设置 Asset 路径。");
            }

            {
                XFile.Directory.assetInit = false;
                XFile.Directory.assetValue = null;
                XFile.Directory.assetTestArgs.Clear();
                XFile.Directory.assetTestArgs.Add("--XFile.Directory.Asset=/custom/path");
                Assert.That(XFile.Directory.Asset, Is.EqualTo("/custom/path"), "应能通过 --XFile.Directory.Asset=<path> 参数格式设置 Asset 路径。");
            }

            {
                XFile.Directory.assetInit = false;
                XFile.Directory.assetValue = null;
                XFile.Directory.assetTestArgs.Clear();
                XFile.Directory.assetTestArgs.Add("--XFile.Directory.Asset");
                XFile.Directory.assetTestArgs.Add("/custom/path");
                Assert.That(XFile.Directory.Asset, Is.EqualTo("/custom/path"), "应能通过 --XFile.Directory.Asset <path> 参数格式设置 Asset 路径。");
            }
#endif
        }
        finally
        {
            XFile.Directory.assetTestArgs.Clear();
            XFile.Directory.assetInit = false;
            XFile.Directory.assetValue = null;
        }
    }

    [Test]
    public void DirectoryLocal()
    {
        try
        {
            {
                XFile.Directory.localInit = false;
                XFile.Directory.localValue = null;
                XFile.Directory.localTestArgs.Clear();

                var localPath = XFile.Directory.Local;
                Assert.That(localPath, Is.Not.Empty, "Local 路径不应为空。");
                Assert.That(XFile.Directory.Exists(localPath), Is.True, "Local 目录应已自动创建。");
                Assert.That(XFile.Directory.Local, Is.EqualTo(localPath), "多次访问 Local 应返回相同值。");
            }

            {
                var testDir = Path.Combine(Path.GetTempPath(), "custom/path/1");
                XFile.Directory.localInit = false;
                XFile.Directory.localValue = null;
                XFile.Directory.localTestArgs.Clear();
                XFile.Directory.localTestArgs.Add("-XFile.Directory.Local=" + testDir);
                Assert.That(XFile.Directory.Local, Is.EqualTo(testDir), "应能通过 -XFile.Directory.Local=<path> 参数格式设置 Local 路径。");
                Assert.That(XFile.Directory.Exists(XFile.Directory.Local), Is.True, "通过参数设置的 Local 目录应已自动创建。");
            }

            {
                var testDir = Path.Combine(Path.GetTempPath(), "custom/path/2");
                XFile.Directory.localInit = false;
                XFile.Directory.localValue = null;
                XFile.Directory.localTestArgs.Clear();
                XFile.Directory.localTestArgs.Add("-XFile.Directory.Local");
                XFile.Directory.localTestArgs.Add(testDir);
                Assert.That(XFile.Directory.Local, Is.EqualTo(testDir), "应能通过 -XFile.Directory.Local <path> 参数格式设置 Local 路径。");
                Assert.That(XFile.Directory.Exists(XFile.Directory.Local), Is.True, "通过参数设置的 Local 目录应已自动创建。");
            }

            {
                var testDir = Path.Combine(Path.GetTempPath(), "custom/path/3");
                XFile.Directory.localInit = false;
                XFile.Directory.localValue = null;
                XFile.Directory.localTestArgs.Clear();
                XFile.Directory.localTestArgs.Add("--XFile.Directory.Local=" + testDir);
                Assert.That(XFile.Directory.Local, Is.EqualTo(testDir), "应能通过 --XFile.Directory.Local=<path> 参数格式设置 Local 路径。");
                Assert.That(XFile.Directory.Exists(XFile.Directory.Local), Is.True, "通过参数设置的 Local 目录应已自动创建。");
            }

            {
                var testDir = Path.Combine(Path.GetTempPath(), "custom/path/4");
                XFile.Directory.localInit = false;
                XFile.Directory.localValue = null;
                XFile.Directory.localTestArgs.Clear();
                XFile.Directory.localTestArgs.Add("--XFile.Directory.Local");
                XFile.Directory.localTestArgs.Add(testDir);
                Assert.That(XFile.Directory.Local, Is.EqualTo(testDir), "应能通过 --XFile.Directory.Local <path> 参数格式设置 Local 路径。");
                Assert.That(XFile.Directory.Exists(XFile.Directory.Local), Is.True, "通过参数设置的 Local 目录应已自动创建。");
            }
        }
        finally
        {
            XFile.Directory.localTestArgs.Clear();
            XFile.Directory.localInit = false;
            XFile.Directory.localValue = null;
        }
    }

    [Test]
    public void DirectoryEvaluator()
    {
        Assert.That("${XFile.Directory.Project}".Evaluate(XFile.Directory.Evaluator), Is.EqualTo(XFile.Directory.Project), "Directory.Project 应当正确求值。");
        Assert.That("${XFile.Directory.Local}".Evaluate(XFile.Directory.Evaluator), Is.EqualTo(XFile.Directory.Local), "Directory.Local 应当正确求值。");
        Assert.That("${XFile.Directory.Asset}".Evaluate(XFile.Directory.Evaluator), Is.EqualTo(XFile.Directory.Asset), "Directory.Asset 应当正确求值。");
        Assert.That("${XFile.Directory.Missing}".Evaluate(XFile.Directory.Evaluator), Is.EqualTo("${Unknown.XFile.Directory.Missing}"), "Directory.Missing 应当返回 Unknown。");
    }
}
