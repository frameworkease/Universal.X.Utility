// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XFile

import (
	"encoding/json"
	"fmt"
)

type FileInfo struct {
	Name string
	Hash string
	Size int64
}

type DiffInfo struct {
	Added    []*FileInfo
	Modified []*FileInfo
	Deleted  []*FileInfo
}

type Manifest struct {
	Files []*FileInfo
}

func (m *Manifest) Compare(other *Manifest) *DiffInfo {
	diffInfo := &DiffInfo{
		Added:    make([]*FileInfo, 0),
		Modified: make([]*FileInfo, 0),
		Deleted:  make([]*FileInfo, 0),
	}

	selfFiles := m.Files
	otherFiles := other.Files
	visited := make(map[int]bool)

	// 检查当前清单中的文件
	for i := range selfFiles {
		sf := selfFiles[i]
		found := false
		for j := range otherFiles {
			of := otherFiles[j]
			if of.Name == sf.Name {
				if sf.Hash != of.Hash {
					diffInfo.Modified = append(diffInfo.Modified, of)
				}
				found = true
				visited[j] = true
				break
			}
		}
		if !found {
			diffInfo.Deleted = append(diffInfo.Deleted, sf)
		}
	}

	// 检查目标清单中的新文件
	for i := range otherFiles {
		if !visited[i] {
			diffInfo.Added = append(diffInfo.Added, otherFiles[i])
		}
	}

	return diffInfo
}

func (m *Manifest) Stringify(pretty ...bool) string {
	if len(pretty) > 0 && pretty[0] {
		json, err := json.MarshalIndent(m.Files, "", "  ")
		if err != nil {
			fmt.Printf("XFile.Manifest.Stringify: marshal error: %v\n", err)
		}
		return string(json)
	} else {
		json, err := json.Marshal(m.Files)
		if err != nil {
			fmt.Printf("XFile.Manifest.Stringify: marshal error: %v\n", err)
		}
		return string(json)
	}
}

func (m *Manifest) Parse(content []byte) error {
	if len(content) == 0 {
		return fmt.Errorf("null content for parsing manifest")
	}
	var files []*FileInfo
	if err := json.Unmarshal(content, &files); err != nil {
		return fmt.Errorf("invalid JSON format for parsing manifest: %w", err)
	}
	m.Files = files
	return nil
}
