using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents a basic object which implements IDisposable interface.
    /// </summary>
    [Serializable]
    public class DisposableObject : IDisposable
    {
        #region Constructor

        static DisposableObject()
        {
            DisposedEventKey = new object();
            DisposingEventKey = new object();
        }

        public DisposableObject()
        {
            this.bitState = new RadBitVector64(0);
        }

        #endregion

        #region BitState

        /// <summary>
        /// Gets the current bit state for the object, defined by the provided key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected internal bool GetBitState(ulong key)
        {
            return this.bitState[key];
        }

        /// <summary>
        /// Applies the specified boolean value to the BitVector of the object.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected internal virtual void SetBitState(ulong key, bool value)
        {
            if (this.bitState[key] == value)
            {
                return;
            }

            bool oldValue = this.bitState[key];
            this.bitState[key] = value;
            this.OnBitStateChanged(key, oldValue, value);
        }

        /// <summary>
        /// Notifies the object for a change in its bit state.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        /// <param name="oldValue"></param>
        protected virtual void OnBitStateChanged(ulong key, bool oldValue, bool newValue)
        {
        }

        /// <summary>
        /// Gets the RadBitVector64 structure that holds all the bit states of the object.
        /// </summary>
        protected internal RadBitVector64 BitState
        {
            get
            {
                return this.bitState;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Provides a simple list of delegates.
        /// </summary>
        protected EventHandlerList Events
        {
            get
            {
                if (this.events == null)
                {
                    this.events = new EventHandlerList();
                }
                return this.events;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler Disposed
        {
            add
            {
                this.Events.AddHandler(DisposedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(DisposedEventKey, value);
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler Disposing
        {
            add
            {
                this.Events.AddHandler(DisposingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(DisposingEventKey, value);
            }
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Determines whether the object is in a process of being disposed of.
        /// </summary>
        [Browsable(false)]
        public bool IsDisposing
        {
            get
            {
                return this.bitState[DisposingStateKey];
            }
        }

        /// <summary>
        /// Determines whether the object is already disposed.
        /// </summary>
        [Browsable(false)]
        public bool IsDisposed
        {
            get
            {
                return this.bitState[DisposedStateKey];
            }
        }

        /// <summary>
        /// Releases all resources associated with this object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs the actual Dispose logic.
        /// </summary>
        /// <param name="disposing"></param>
        protected void Dispose(bool disposing)
        {
            //we are already disposed of
            if (this.IsDisposing || this.IsDisposed)
            {
                return;
            }

            this.SetBitState(DisposingStateKey, true);

            if (this.events != null)
            {
                EventHandler eh = this.events[DisposingEventKey] as EventHandler;
                if (eh != null)
                {
                    eh(this, EventArgs.Empty);
                }
            }

            this.PerformDispose(disposing);

            this.SetBitState(DisposingStateKey, false);
            this.SetBitState(DisposedStateKey, true);
        }

        /// <summary>
        /// Performs the core resources release logic.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void PerformDispose(bool disposing)
        {
            lock (syncRoot)
            {
                if (disposing)
                {
                    this.DisposeManagedResources();
                }

                this.DisposeUnmanagedResources();
            }
        }

        /// <summary>
        /// Disposes all MANAGED resources - such as Bitmaps, GDI+ objects, etc.
        /// </summary>
        protected virtual void DisposeManagedResources()
        {
            if (this.events != null)
            {
                EventHandler eh = this.events[DisposedEventKey] as EventHandler;
                this.events.Dispose();

                if (eh != null)
                {
                    eh(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Releases any UNMANAGED resources used by this object.
        /// NOTE: If you declare some unmanaged resources in your class,
        /// you should override its finalizer and put disposing logic there also.
        /// </summary>
        protected virtual void DisposeUnmanagedResources()
        {
        }

        #endregion

        #region Fields

        [NonSerialized]
        private RadBitVector64 bitState;
        [NonSerialized]
        private EventHandlerList events;

        #endregion

        #region Static

        private static readonly object DisposingEventKey;
        private static readonly object DisposedEventKey;
        private static object syncRoot = new object();

        internal const ulong DisposingStateKey = 1;
        internal const ulong DisposedStateKey = DisposingStateKey << 1;
        internal const ulong DisposableObjectLastStateKey = DisposedStateKey;

        #endregion
    }
}
