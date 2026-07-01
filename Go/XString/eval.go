// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package XString

import (
	"strings"
)

type IEvaluator interface {
	Evaluate(input string) string
}

func Evaluate(input string, evaluators ...any) string {
	if input == "" {
		return ""
	}

	result := input
	for _, evaluator := range evaluators {
		if evaluator == nil {
			continue
		}

		switch src := evaluator.(type) {
		case IEvaluator:
			result = src.Evaluate(result)
		case map[string]string:
			for key, value := range src {
				result = strings.ReplaceAll(result, "${"+key+"}", value)
			}
		}
	}

	return result
}
