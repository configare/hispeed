using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Telerik.WinControls.Interfaces;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Base class for all generic RadDock objects - such as Services, Settings, etc.
    /// </summary>
    [Serializable]
    public class RadDockObject : IDisposable, INotifyPropertyChanged, INotifyPropertyChangingEx
    {
        #region Constructor

        static RadDockObject()
        {
            EventDisposed = new object();
            EventPropertyChanging = new object();
            EventPropertyChanged = new object();
        }

        #endregion

        #region Interfaces

        #region IDisposable

        /// <summary>
        /// Notifies that the object is disposed.
        /// </summary>
        public event EventHandler Disposed
        {
            add
            {
                this.Events.AddHandler(EventDisposed, value);
            }
            remove
            {
                this.Events.RemoveHandler(EventDisposed, value);
            }
        }

        /// <summary>
        /// Forces object clean-up and resource release.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs the actual dispose logic.
        /// </summary>
        /// <param name="managed">True to notify that managed resources should also be disposed.</param>
        protected void Dispose(bool managed)
        {
            if (this.isDisposed || this.disposing)
            {
                return;
            }

            this.disposing = true;

            if (managed)
            {
                DisposeManagedResources();
            }
            DisposeUnmanagedResources();

            this.disposing = false;
            this.isDisposed = true;
        }

        /// <summary>
        /// Disposes any managed resources associated with this object.
        /// </summary>
        protected virtual void DisposeManagedResources()
        {
            if (this.events != null)
            {
                EventHandler eh = this.events[EventDisposed] as EventHandler;
                this.events.Dispose();

                if (eh != null)
                {
                    eh(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Disposes any unmanaged resources associated with this instance.
        /// </summary>
        protected virtual void DisposeUnmanagedResources()
        {
        }

        #endregion

        #region INotifyPropertyChanged

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangingEventHandlerEx PropertyChanging
        {
            add
            {
                this.Events.AddHandler(EventPropertyChanging, value);
            }
            remove
            {
                this.Events.RemoveHandler(EventPropertyChanging, value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                this.Events.AddHandler(EventPropertyChanged, value);
            }
            remove
            {
                this.Events.RemoveHandler(EventPropertyChanged, value);
            }
        }

        /// <summary>
        /// Raises the PropertyChanging notification.
        /// </summary>
        /// <param name="propName"></param>
        /// <returns>True to indicate that the change is accepted, false otherwise.</returns>
        protected virtual bool OnPropertyChanging(string propName)
        {
            PropertyChangingEventHandlerEx eh = this.Events[EventPropertyChanging] as PropertyChangingEventHandlerEx;
            if (eh != null)
            {
                eh(this, new PropertyChangingEventArgsEx(propName));
            }

            return true;
        }

        /// <summary>
        /// Raises the <see cref="RadDockObject.PropertyChanged">PropertyChanged</see> event.
        /// </summary>
        /// <param name="propName"></param>
        protected virtual void OnPropertyChanged(string propName)
        {
            PropertyChangedEventHandler eh = this.Events[EventPropertyChanged] as PropertyChangedEventHandler;
            if (eh != null)
            {
                eh(this, new PropertyChangedEventArgs(propName));
            }
        }

        /// <summary>
        /// Determines whether the property with the specified name needs serialization.
        /// </summary>
        /// <param name="propName"></param>
        /// <returns></returns>
        protected virtual bool ShouldSerializeProperty(string propName)
        {
            return false;
        }

        #endregion

        #endregion

        #region Properties

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

        [Browsable(false)]
        public bool Disposing
        {
            get
            {
                return this.disposing;
            }
        }

        [Browsable(false)]
        public bool IsDisposed
        {
            get
            {
                return this.isDisposed;
            }
        }

        #endregion

        #region Fields

        [NonSerialized]
        private bool disposing;
        [NonSerialized]
        private bool isDisposed;
        [NonSerialized]
        private EventHandlerList events;

        #endregion

        #region Static

        private static object EventDisposed;
        private static object EventPropertyChanging;
        private static object EventPropertyChanged;

        #endregion
    }
}
