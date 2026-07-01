// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace FrameworkEase.Universal.X.Utility
{
    public static partial class XPrefs
    {
        public interface IBase : XString.IEvaluator
        {
            IBase Set(string key, object value);

            IBase Delete(string key);

            bool Exists(string key);

            void Range(Func<string, object, bool> callback);

            object Get(string key, object defval = null);

            object[] Gets(string key, object[] defval = null);

            int GetInt(string key, int defval = 0);

            int[] GetInts(string key, int[] defval = null);

            float GetFloat(string key, float defval = 0f);

            float[] GetFloats(string key, float[] defval = null);

            string GetString(string key, string defval = "");

            string[] GetStrings(string key, string[] defval = null);

            bool GetBool(string key, bool defval = false);

            bool[] GetBools(string key, bool[] defval = null);

            string Stringify(bool pretty = false);

            bool Parse(byte[] content);

            bool Parse(string content);
        }

        public class Base : IBase
        {
            internal ReaderWriterLockSlim mutex = new();
            internal Dictionary<string, object> pairs = new();

            public virtual IBase Set(string key, object value)
            {
                if (string.IsNullOrEmpty(key)) { XLog.Error("XPrefs.Base.Set: key is nil."); return this; }
                if (value == null) { XLog.Error("XPrefs.Base.Set: value is nil."); return this; }

                mutex.EnterWriteLock();
                try { pairs[key] = value; }
                finally { mutex.ExitWriteLock(); }

                return this;
            }

            public virtual IBase Delete(string key)
            {
                if (string.IsNullOrEmpty(key)) { XLog.Error("XPrefs.Base.Delete: key is nil."); return this; }

                mutex.EnterWriteLock();
                try { pairs.Remove(key); }
                finally { mutex.ExitWriteLock(); }

                return this;
            }

            public virtual bool Exists(string key)
            {
                if (string.IsNullOrEmpty(key)) { XLog.Error("XPrefs.Base.Exists: key is nil."); return false; }

                mutex.EnterReadLock();
                try { return pairs.ContainsKey(key); }
                finally { mutex.ExitReadLock(); }
            }

            public virtual void Range(Func<string, object, bool> callback)
            {
                if (callback == null) { XLog.Error("XPrefs.Base.Range: callback is nil."); return; }

                Dictionary<string, object> pairs;
                mutex.EnterReadLock();
                try
                {
                    if (this.pairs.Count == 0) return;
                    pairs = new Dictionary<string, object>(this.pairs);
                }
                finally { mutex.ExitReadLock(); }

                foreach (var kvp in pairs)
                {
                    if (!callback(kvp.Key, kvp.Value)) break;
                }
            }

            public virtual object Get(string key, object defval = null)
            {
                if (string.IsNullOrEmpty(key)) { XLog.Error("XPrefs.Base.Get: key is nil."); return defval; }

                mutex.EnterReadLock();
                try
                {
                    if (pairs.TryGetValue(key, out var val))
                    {
                        if (val is Dictionary<string, object> dict) return new Base { pairs = new Dictionary<string, object>(dict) };
                        else return val;
                    }
                }
                finally { mutex.ExitReadLock(); }

                return defval;
            }

            public virtual object[] Gets(string key, object[] defval = null)
            {
                if (string.IsNullOrEmpty(key)) { XLog.Error("XPrefs.Base.Gets: key is nil."); return defval; }

                mutex.EnterReadLock();
                try
                {
                    if (pairs.TryGetValue(key, out var val))
                    {
                        if (val is Dictionary<string, object>[] darr)
                        {
                            var nv = new object[darr.Length];
                            for (var i = 0; i < darr.Length; i++) nv[i] = new Base { pairs = new Dictionary<string, object>(darr[i]) };
                            return nv;
                        }
                        if (val is object[] oarr) return oarr;

                        var type = val.GetType();
                        if (type.IsArray)
                        {
                            var arr = (Array)val;
                            var nv = new object[arr.Length];
                            for (var i = 0; i < arr.Length; i++)
                            {
                                var ele = arr.GetValue(i);
                                if (ele is Dictionary<string, object> mv) nv[i] = new Base { pairs = new Dictionary<string, object>(mv) };
                                else nv[i] = ele;
                            }
                            return nv;
                        }
                    }
                }
                finally { mutex.ExitReadLock(); }

                return defval;
            }

            public virtual int GetInt(string key, int defval = 0)
            {
                if (string.IsNullOrEmpty(key)) { XLog.Error("XPrefs.Base.GetInt: key is nil."); return defval; }

                mutex.EnterReadLock();
                try
                {
                    if (pairs.TryGetValue(key, out var val))
                    {
                        switch (val)
                        {
                            case int i: return i;
                            case sbyte sb: return sb;
                            case short s: return s;
                            case long l: return (int)l;
                            case float f: return (int)f;
                            case double d: return (int)d;
                            case string s:
                                if (int.TryParse(s, out var iv)) return iv;
                                break;
                            case bool b: return b ? 1 : 0;
                        }
                    }
                }
                finally { mutex.ExitReadLock(); }

                return defval;
            }

            public virtual int[] GetInts(string key, int[] defval = null)
            {
                if (string.IsNullOrEmpty(key)) { XLog.Error("XPrefs.Base.GetInts: key is nil."); return defval; }

                mutex.EnterReadLock();
                try
                {
                    if (pairs.TryGetValue(key, out var val))
                    {
                        if (val is int[] ival) return ival;

                        var type = val.GetType();
                        if (type.IsArray)
                        {
                            var arr = (Array)val;
                            var rarr = new int[arr.Length];
                            for (var i = 0; i < arr.Length; i++)
                            {
                                var item = arr.GetValue(i);
                                switch (item)
                                {
                                    case int iv: rarr[i] = iv; break;
                                    case sbyte sb: rarr[i] = sb; break;
                                    case short s: rarr[i] = s; break;
                                    case long l: rarr[i] = (int)l; break;
                                    case float f: rarr[i] = (int)f; break;
                                    case double d: rarr[i] = (int)d; break;
                                    case string str:
                                        if (int.TryParse(str, out var parsedInt)) rarr[i] = parsedInt;
                                        break;
                                    case bool b: rarr[i] = b ? 1 : 0; break;
                                }
                            }
                            return rarr;
                        }
                    }
                }
                finally { mutex.ExitReadLock(); }

                return defval;
            }

            public virtual float GetFloat(string key, float defval = 0f)
            {
                if (string.IsNullOrEmpty(key)) { XLog.Error("XPrefs.Base.GetFloat: key is nil."); return defval; }

                mutex.EnterReadLock();
                try
                {
                    if (pairs.TryGetValue(key, out var val))
                    {
                        switch (val)
                        {
                            case int i: return i;
                            case sbyte sb: return sb;
                            case short s: return s;
                            case long l: return l;
                            case float f: return f;
                            case double d: return (float)d;
                            case string s:
                                if (float.TryParse(s, out var fv)) return fv;
                                break;
                            case bool b: return b ? 1 : 0;
                        }
                    }
                }
                finally { mutex.ExitReadLock(); }

                return defval;
            }

            public virtual float[] GetFloats(string key, float[] defval = null)
            {
                if (string.IsNullOrEmpty(key)) { XLog.Error("XPrefs.Base.GetFloats: key is nil."); return defval; }

                mutex.EnterReadLock();
                try
                {
                    if (pairs.TryGetValue(key, out var val))
                    {
                        if (val is float[] fval) return fval;

                        var type = val.GetType();
                        if (type.IsArray)
                        {
                            var arr = (Array)val;
                            var rarr = new float[arr.Length];
                            for (var i = 0; i < arr.Length; i++)
                            {
                                var item = arr.GetValue(i);
                                switch (item)
                                {
                                    case int iv: rarr[i] = iv; break;
                                    case sbyte sb: rarr[i] = sb; break;
                                    case short s: rarr[i] = s; break;
                                    case long l: rarr[i] = l; break;
                                    case float f: rarr[i] = f; break;
                                    case double d: rarr[i] = (float)d; break;
                                    case string str:
                                        if (float.TryParse(str, out var fv)) rarr[i] = fv;
                                        break;
                                    case bool b: rarr[i] = b ? 1 : 0; break;
                                }
                            }
                            return rarr;
                        }
                    }
                }
                finally { mutex.ExitReadLock(); }

                return defval;
            }

            public virtual string GetString(string key, string defval = "")
            {
                if (string.IsNullOrEmpty(key)) { XLog.Error("XPrefs.Base.GetString: key is nil."); return defval; }

                mutex.EnterReadLock();
                try
                {
                    if (pairs.TryGetValue(key, out var val))
                    {
                        if (val is string sval) return sval;
                        else return val?.ToString() ?? string.Empty;
                    }
                }
                finally { mutex.ExitReadLock(); }

                return defval;
            }

            public virtual string[] GetStrings(string key, string[] defval = null)
            {
                if (string.IsNullOrEmpty(key)) { XLog.Error("XPrefs.Base.GetStrings: key is nil."); return defval; }

                mutex.EnterReadLock();
                try
                {
                    if (pairs.TryGetValue(key, out var val))
                    {
                        if (val is string[] sarr) return sarr;

                        var type = val.GetType();
                        if (type.IsArray)
                        {
                            var arr = (Array)val;
                            var rarr = new string[arr.Length];
                            for (var i = 0; i < arr.Length; i++)
                            {
                                var item = arr.GetValue(i);
                                if (item is string s) rarr[i] = s;
                                else rarr[i] = item?.ToString() ?? string.Empty;
                            }
                            return rarr;
                        }
                    }
                }
                finally { mutex.ExitReadLock(); }

                return defval;
            }

            public virtual bool GetBool(string key, bool defval = false)
            {
                if (string.IsNullOrEmpty(key)) { XLog.Error("XPrefs.Base.GetBool: key is nil."); return defval; }

                mutex.EnterReadLock();
                try
                {
                    if (pairs.TryGetValue(key, out var val))
                    {
                        if (val is bool bval) return bval;
                    }
                }
                finally { mutex.ExitReadLock(); }

                return defval;
            }

            public virtual bool[] GetBools(string key, bool[] defval = null)
            {
                if (string.IsNullOrEmpty(key)) { XLog.Error("XPrefs.Base.GetBools: key is nil."); return defval; }

                mutex.EnterReadLock();
                try
                {
                    if (pairs.TryGetValue(key, out var val))
                    {
                        if (val is bool[] barr) return barr;

                        var type = val.GetType();
                        if (type.IsArray)
                        {
                            var arr = (Array)val;
                            var rarr = new bool[arr.Length];
                            for (var i = 0; i < arr.Length; i++)
                            {
                                var item = arr.GetValue(i);
                                if (item is bool b) rarr[i] = b;
                            }
                            return rarr;
                        }
                    }
                }
                finally { mutex.ExitReadLock(); }

                return defval;
            }

            public virtual string Stringify(bool pretty = false)
            {
                var visited = new HashSet<IBase>();
                Dictionary<string, object> visit(Dictionary<string, object> origin)
                {
                    var npairs = new Dictionary<string, object>();
                    foreach (var kvp in origin)
                    {
                        if (kvp.Value is IBase prefs1)
                        {
                            if (visited.Contains(prefs1)) npairs[kvp.Key] = "<Recursive>";
                            else
                            {
                                visited.Add(prefs1);
                                var sorigin = new Dictionary<string, object>();
                                prefs1.Range((k, v) => { sorigin[k] = v; return true; });
                                npairs[kvp.Key] = visit(sorigin);
                            }
                        }
                        else if (kvp.Value is Dictionary<string, object> dict1) npairs[kvp.Key] = visit(dict1);
                        else if (kvp.Value is IList list)
                        {
                            var nlist = new List<object>();
                            for (var i = 0; i < list.Count; i++)
                            {
                                var ele = list[i];
                                if (ele is IBase prefs2)
                                {
                                    if (visited.Contains(prefs2)) nlist.Add("<Recursive>");
                                    else
                                    {
                                        visited.Add(prefs2);
                                        var sorigin = new Dictionary<string, object>();
                                        prefs2.Range((k, v) => { sorigin[k] = v; return true; });
                                        nlist.Add(visit(sorigin));
                                    }
                                }
                                else if (ele is Dictionary<string, object> dict2) nlist.Add(visit(dict2));
                                else nlist.Add(ele);
                            }
                            npairs[kvp.Key] = nlist;
                        }
                        else npairs[kvp.Key] = kvp.Value;
                    }
                    return npairs;
                }

                mutex.EnterReadLock();
                var npairs = visit(pairs);
                mutex.ExitReadLock();

                var keys = new List<string>();
                foreach (var kvp in npairs) keys.Add(kvp.Key);
                keys.Sort();
                var sorted = new Dictionary<string, object>();
                foreach (var key in keys) sorted[key] = npairs[key];
                try { return JSON.Stringify(sorted, pretty); }
                catch (Exception e) { XLog.Error($"XPrefs.Base.Stringify: marshal error: {e}"); return ""; }
            }

            public virtual bool Parse(byte[] content)
            {
                if (content == null || content.Length == 0)
                {
                    XLog.Error("XPrefs.Base.Parse: nil content.");
                    return false;
                }
                return Parse(Encoding.UTF8.GetString(content));
            }

            public virtual bool Parse(string content)
            {
                if (string.IsNullOrEmpty(content))
                {
                    XLog.Error("XPrefs.Base.Parse: nil content.");
                    return false;
                }

                try
                {
                    if (JSON.Parse(content, typeof(Dictionary<string, object>)) is not Dictionary<string, object> pairs)
                    {
                        XLog.Error("XPrefs.Base.Parse: unmarshal error: nil instance.");
                        return false;
                    }

                    mutex.EnterWriteLock();
                    try { foreach (var kvp in pairs) this.pairs[kvp.Key] = kvp.Value; }
                    finally { mutex.ExitWriteLock(); }

                    return true;
                }
                catch (Exception e)
                {
                    XLog.Error($"XPrefs.Base.Parse: unmarshal error: {e.Message}");
                    return false;
                }
            }

            public virtual string Evaluate(string expr)
            {
                if (string.IsNullOrEmpty(expr)) return expr;

                var pattern = new Regex(@"\$\{XPrefs\.([^}]+?)\}");
                var visited = new HashSet<string>();

                string repl(Match match)
                {
                    var matched = match.Groups[1].Value;
                    if (matched.Contains("${")) return $"${{Nested.{match.Value}}}";
                    if (visited.Contains(matched)) return $"${{Recursive.XPrefs.{matched}}}";
                    visited.Add(matched);
                    try
                    {
                        string value;
                        if (matched.Contains("."))
                        {
                            var parts = matched.Split('.');
                            IBase current = this;
                            for (var i = 0; i < parts.Length - 1; i++)
                            {
                                if (!current.Exists(parts[i])) return $"${{Unknown.XPrefs.{matched}}}";
                                var next = current.Get(parts[i]);
                                if (next == null) return $"${{Unknown.XPrefs.{matched}}}";
                                if (next is IBase baseObj) current = baseObj;
                                else return $"${{Unknown.XPrefs.{matched}}}";
                            }
                            value = current.GetString(parts[^1]);
                        }
                        else
                        {
                            if (!Exists(matched)) return $"${{Unknown.XPrefs.{matched}}}";
                            value = GetString(matched);
                        }
                        if (string.IsNullOrEmpty(value)) return $"${{Unknown.XPrefs.{matched}}}";
                        return pattern.Replace(value, repl);
                    }
                    finally { visited.Remove(matched); }
                }

                return pattern.Replace(expr, repl);
            }
        }
    }
}
