// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;

namespace FrameworkEase.Universal.X.Utility
{
    public static partial class XFile
    {
        public class FileInfo
        {
            public string Name;

            public string Hash;

            public long Size;
        }

        public class DiffInfo
        {
            public readonly List<FileInfo> Added = new();

            public readonly List<FileInfo> Modified = new();

            public readonly List<FileInfo> Deleted = new();
        }

        public class Manifest
        {
            public readonly List<FileInfo> Files = new();

            public virtual DiffInfo Compare(Manifest other)
            {
                var diffInfo = new DiffInfo();
                var selfFiles = Files;
                var otherFiles = other.Files;
                var visited = new List<FileInfo>();
                for (var i = 0; i < selfFiles.Count; i++)
                {
                    var sf = selfFiles[i];
                    var sig = false;
                    for (var j = 0; j < otherFiles.Count; j++)
                    {
                        var of = otherFiles[j];
                        if (of.Name == sf.Name)
                        {
                            if (sf.Hash != of.Hash)
                            {
                                diffInfo.Modified.Add(of);
                            }
                            sig = true;
                            visited.Add(of);
                            break;
                        }
                    }
                    if (!sig) diffInfo.Deleted.Add(sf);
                }
                for (var i = 0; i < otherFiles.Count; i++)
                {
                    var fi = otherFiles[i];
                    if (!visited.Contains(fi)) diffInfo.Added.Add(fi);
                }
                return diffInfo;
            }

            public string Stringify(bool pretty = false) { return JSON.Stringify(Files, pretty); }

            public virtual string Parse(string content)
            {
                if (string.IsNullOrEmpty(content)) return "null content for parsing mainfest";
                else
                {
                    try
                    {
                        var files = JSON.Parse<List<FileInfo>>(content);
                        if (files == null) return "invalid JSON format for parsing manifest";
                        Files.Clear();
                        Files.AddRange(files);
                    }
                    catch (Exception e)
                    {
                        XLog.Error(e);
                        return e.Message;
                    }
                }
                return string.Empty;
            }
        }
    }
}
