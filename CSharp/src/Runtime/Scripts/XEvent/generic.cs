// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;

namespace FrameworkEase.Universal.X.Utility
{
    public partial class XEvent
    {
        public partial class Manager
        {
            internal bool Register(int id, Callback proxy, Delegate origin, bool once, string source)
            {
                if (origin == null)
                {
                    XLog.Error("XEvent.Manager.{0}: nil callback, id={1}.", source, id);
                    return false;
                }
                Mutex.EnterWriteLock();
                try
                {
                    if (Handlers.TryGetValue(id, out List<Handler> handlers) == false)
                    {
                        handlers = new List<Handler>();
                        Handlers.Add(id, handlers);
                    }
                    if (Singleton && handlers.Count > 0)
                    {
                        XLog.Error("XEvent.Manager.{0}: singleton mode doesn't allow multiple registrations, id={1}.", source, id);
                        return false;
                    }
                    for (var i = 0; i < handlers.Count; i++)
                    {
                        if (Equals(handlers[i].Origin, origin)) return false;
                    }
                    var handler = new Handler { Func = proxy, Origin = origin, Once = once };
                    handlers.Add(handler);
                    return true;
                }
                finally { Mutex.ExitWriteLock(); }
            }

            internal bool Unregister(int id, Delegate callback, string source)
            {
                if (callback == null)
                {
                    XLog.Error("XEvent.Manager.{0}: nil callback, id={1}.", source, id);
                    return false;
                }
                Mutex.EnterWriteLock();
                try
                {
                    var ret = false;
                    if (Handlers.TryGetValue(id, out var handlers))
                    {
                        ret = handlers.RemoveAll(h => Equals(h.Origin, callback)) > 0;
                        if (handlers.Count == 0) Handlers.Remove(id);
                    }
                    return ret;
                }
                finally { Mutex.ExitWriteLock(); }
            }

            public virtual bool Register(Enum id, Action callback, bool once = false) { return Register(id.GetHashCode(), callback, once); }

            public virtual bool Register(int id, Action callback, bool once = false)
            {
                if (callback == null)
                {
                    XLog.Error("XEvent.Manager.Register: nil callback, id={0}.", id);
                    return false;
                }
                var proxy = new Callback(args => callback?.Invoke());
                return Register(id, proxy, callback, once, "Register");
            }

            public virtual bool Register<T1>(Enum id, Action<T1> callback, bool once = false) { return Register(id.GetHashCode(), callback, once); }

            public virtual bool Register<T1>(int id, Action<T1> callback, bool once = false)
            {
                if (callback == null)
                {
                    XLog.Error("XEvent.Manager.Register: nil callback, id={0}.", id);
                    return false;
                }
                var proxy = new Callback(args =>
                {
                    var arg1 = args != null && args.Length > 0 ? (T1)args[0] : default;
                    callback?.Invoke(arg1);
                });
                return Register(id, proxy, callback, once, "RegisterT1");
            }

            public virtual bool Register<T1, T2>(Enum id, Action<T1, T2> callback, bool once = false) { return Register(id.GetHashCode(), callback, once); }

            public virtual bool Register<T1, T2>(int id, Action<T1, T2> callback, bool once = false)
            {
                if (callback == null)
                {
                    XLog.Error("XEvent.Manager.Register: nil callback, id={0}.", id);
                    return false;
                }
                var proxy = new Callback(args =>
                {
                    var arg1 = args != null && args.Length > 0 ? (T1)args[0] : default;
                    var arg2 = args != null && args.Length > 1 ? (T2)args[1] : default;
                    callback?.Invoke(arg1, arg2);
                });
                return Register(id, proxy, callback, once, "RegisterT2");
            }

            public virtual bool Register<T1, T2, T3>(Enum id, Action<T1, T2, T3> callback, bool once = false) { return Register(id.GetHashCode(), callback, once); }

            public virtual bool Register<T1, T2, T3>(int id, Action<T1, T2, T3> callback, bool once = false)
            {
                if (callback == null)
                {
                    XLog.Error("XEvent.Manager.Register: nil callback, id={0}.", id);
                    return false;
                }
                var proxy = new Callback(args =>
                {
                    var arg1 = args != null && args.Length > 0 ? (T1)args[0] : default;
                    var arg2 = args != null && args.Length > 1 ? (T2)args[1] : default;
                    var arg3 = args != null && args.Length > 2 ? (T3)args[2] : default;
                    callback?.Invoke(arg1, arg2, arg3);
                });
                return Register(id, proxy, callback, once, "RegisterT3");
            }

            public virtual bool Unregister(Enum id, Action callback) { return Unregister(id.GetHashCode(), callback); }

            public virtual bool Unregister(int id, Action callback) { return Unregister(id, callback, "Unregister"); }

            public virtual bool Unregister<T1>(Enum id, Action<T1> callback) { return Unregister(id.GetHashCode(), callback); }

            public virtual bool Unregister<T1>(int id, Action<T1> callback) { return Unregister(id, callback, "UnregisterT1"); }

            public virtual bool Unregister<T1, T2>(Enum id, Action<T1, T2> callback) { return Unregister(id.GetHashCode(), callback); }

            public virtual bool Unregister<T1, T2>(int id, Action<T1, T2> callback) { return Unregister(id, callback, "UnregisterT2"); }

            public virtual bool Unregister<T1, T2, T3>(Enum id, Action<T1, T2, T3> callback) { return Unregister(id.GetHashCode(), callback); }

            public virtual bool Unregister<T1, T2, T3>(int id, Action<T1, T2, T3> callback) { return Unregister(id, callback, "UnregisterT3"); }
        }
    }
}
