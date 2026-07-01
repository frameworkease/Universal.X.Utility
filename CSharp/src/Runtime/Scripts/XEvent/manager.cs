// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FrameworkEase.Universal.X.Utility
{
    public partial class XEvent
    {
        public delegate void Callback(params object[] args);

        public class Handler
        {
            public Callback Func;

            public Delegate Origin;

            public bool Once;
        }

        public partial class Manager : IDisposable
        {
            public bool Singleton { get; set; }

            public Dictionary<int, List<Handler>> Handlers { get; protected set; } = new();

            protected readonly ReaderWriterLockSlim Mutex = new();

            protected readonly ThreadLocal<List<Handler>> Batches = new(() => new List<Handler>(64));

            public void Dispose() { Mutex?.Dispose(); Batches?.Dispose(); }

            public virtual bool Register(Enum id, Callback callback, bool once = false) { return Register(id.GetHashCode(), callback, once); }

            public virtual bool Register(int id, Callback callback, bool once = false)
            {
                if (callback == null)
                {
                    XLog.Error("XEvent.Manager.Register: nil callback, id={0}.", id);
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
                        XLog.Error("XEvent.Manager.Register: singleton mode doesn't allow multiple registrations, id={0}.", id);
                        return false;
                    }
                    for (var i = 0; i < handlers.Count; i++)
                    {
                        if (Equals(handlers[i].Origin, callback)) return false;
                    }
                    var handler = new Handler { Func = callback, Origin = callback, Once = once };
                    handlers.Add(handler);
                    return true;
                }
                finally { Mutex.ExitWriteLock(); }
            }

            public virtual bool Unregister(Enum id, Callback callback = null) { return Unregister(id.GetHashCode(), callback); }

            public virtual bool Unregister(int id, Callback callback = null)
            {
                Mutex.EnterWriteLock();
                try
                {
                    var ret = false;
                    if (Handlers.TryGetValue(id, out var handlers))
                    {
                        if (callback != null)
                        {
                            if (handlers.Count > 0)
                            {
                                ret = handlers.RemoveAll(h => Equals(h.Origin, callback)) > 0;
                                if (handlers.Count == 0) Handlers.Remove(id);
                            }
                        }
                        else
                        {
                            ret = handlers.Count > 0;
                            Handlers.Remove(id);
                        }
                    }
                    return ret;
                }
                finally { Mutex.ExitWriteLock(); }
            }

            public virtual void Clear()
            {
                Mutex.EnterWriteLock();
                try { Handlers.Clear(); }
                finally { Mutex.ExitWriteLock(); }
            }

            public virtual bool Notify(Enum id, params object[] args) { return Notify(id.GetHashCode(), args); }

            public virtual bool Notify(int id, params object[] args)
            {
                var batches = Batches.Value;
                Mutex.EnterReadLock();
                try
                {
                    if (batches.Count > 0) batches.Clear();
                    if (Handlers.TryGetValue(id, out var handlers))
                    {
                        if (handlers != null && handlers.Count > 0)
                        {
                            for (int i = 0; i < handlers.Count; i++)
                            {
                                var handler = handlers[i];
                                if (handler != null && handler.Func != null)
                                {
                                    batches.Add(handler);
                                }
                            }
                        }
                    }
                }
                finally { Mutex.ExitReadLock(); }

                if (batches.Count > 0)
                {
                    var once = false;
                    for (int i = 0; i < batches.Count; i++)
                    {
                        var handler = batches[i];
                        if (handler.Once) once = true;
                        handler?.Func?.Invoke(args);
                    }
                    if (once)
                    {
                        Mutex.EnterWriteLock();
                        try
                        {
                            if (Handlers.TryGetValue(id, out var nhandlers))
                            {
                                nhandlers.RemoveAll(h => batches.Any(o => o.Once && object.ReferenceEquals(o.Origin, h.Origin)));
                                if (nhandlers.Count == 0) Handlers.Remove(id);
                            }
                        }
                        finally { Mutex.ExitWriteLock(); }
                    }
                    batches.Clear();
                    return true;
                }
                return false;
            }
        }
    }
}
