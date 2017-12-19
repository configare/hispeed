using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Telerik.WinControls
{
    public sealed class RadKeyboardFilter : IMessageListener
    {
        #region Fields

        [ThreadStatic]
        private static RadKeyboardFilter instance;
        private WeakReferenceList<IKeyboardListener> listeners;
        private bool hookInstalled;
        private bool enabled;
        private bool handleInputForUnmanagedControls;

        #endregion

        #region Constructor

        private RadKeyboardFilter()
        {
            this.listeners = new WeakReferenceList<IKeyboardListener>(true, false);
            this.enabled = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the only instance (per UI thread) of the keyboard filter.
        /// </summary>
        public static RadKeyboardFilter Instance
        {
            get
            {
                //Field is ThreadStatic. No need of double-checking.
                if (instance == null)
                {
                    instance = new RadKeyboardFilter();
                }
                return instance;
            }
        }

        /// <summary>
        /// Allows the filter to be temporary enabled/disabled.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                this.enabled = value;
            }
        }

        /// <summary>
        /// Gets the count of all listeners currently registered with this instance.
        /// </summary>
        public int ListenersCount
        {
            get
            {
                return this.listeners.Count;
            }
        }

        /// <summary>
        /// Determines whether keyboard input that targets unmanaged controls (such as system menu) is processed. False by default.
        /// </summary>
        public bool HandleInputForUnmanagedControls
        {
            get
            {
                return this.handleInputForUnmanagedControls;
            }
            set
            {
                this.handleInputForUnmanagedControls = value;
            }
        }

        #endregion

        #region Public Methods

        public void AddListener(IKeyboardListener listener)
        {
            if (!this.ContainsListener(listener))
            {
                this.listeners.Add(listener);
                this.UpdateHook();
            }
        }

        public void RemoveListener(IKeyboardListener listener)
        {
            int index = this.listeners.IndexOf(listener);
            if (index >= 0)
            {
                this.listeners.RemoveAt(index);
                this.UpdateHook();
            }
        }

        public bool ContainsListener(IKeyboardListener listener)
        {
            return this.listeners.IndexOf(listener) >= 0;
        }

        #endregion

        #region Private Implementation

        private void UpdateHook()
        {
            if (this.listeners.Count > 0)
            {
                if (!this.hookInstalled)
                {
                    RadMessageFilter.Instance.AddListener(this);
                    this.hookInstalled = true;
                }
            }
            else
            {
                if (this.hookInstalled)
                {
                    RadMessageFilter.Instance.RemoveListener(this);
                    this.hookInstalled = false;
                }
            }
        }

        #endregion

        #region IMessageListener Members

        InstalledHook IMessageListener.DesiredHook
        {
            get
            {
                return InstalledHook.GetMessage;
            }
        }

        MessagePreviewResult IMessageListener.PreviewMessage(ref Message msg)
        {
            Debug.Assert(this.listeners.Count > 0, "No listeners, hook should be uninstalled.");

            if (!this.enabled)
            {
                return MessagePreviewResult.NotProcessed;
            }
            if (msg.Msg < NativeMethods.WM_KEYFIRST || msg.Msg > NativeMethods.WM_KEYLAST)
            {
                return MessagePreviewResult.NotProcessed;
            }

            Control target = Control.FromChildHandle(msg.HWnd);
            if (target == null && !this.handleInputForUnmanagedControls)
            {
                return MessagePreviewResult.NotProcessed;
            }

            return this.NotifyKeyboardEvent(target, msg);
        }

        private MessagePreviewResult NotifyKeyboardEvent(Control target, Message msg)
        {
            MessagePreviewResult result = MessagePreviewResult.NotProcessed;
            foreach (IKeyboardListener listener in this.listeners)
            {
                MessagePreviewResult singleResult = this.NotifyListener(listener, target, msg);
                if ((singleResult & MessagePreviewResult.Processed) == MessagePreviewResult.Processed)
                {
                    result |= MessagePreviewResult.Processed;
                }
                if ((singleResult & MessagePreviewResult.NoDispatch) == MessagePreviewResult.NoDispatch)
                {
                    result |= MessagePreviewResult.NoDispatch;
                }
                if ((singleResult & MessagePreviewResult.NoContinue) == MessagePreviewResult.NoContinue)
                {
                    result |= MessagePreviewResult.NoContinue;
                    break;
                }
            }

            return result;
        }

        private MessagePreviewResult NotifyListener(IKeyboardListener listener, Control target, Message msg)
        {
            MessagePreviewResult result = MessagePreviewResult.NotProcessed;
            Keys keyData;

            switch(msg.Msg)
            {
                case NativeMethods.WM_KEYDOWN:
                case NativeMethods.WM_SYSKEYDOWN:
                    keyData = ((Keys)((int)((long)msg.WParam))) | Control.ModifierKeys;
                    result = listener.OnPreviewKeyDown(target, new KeyEventArgs(keyData));
                    break;
                case NativeMethods.WM_CHAR:
                case NativeMethods.WM_SYSCHAR:
                    KeyPressEventArgs e = new KeyPressEventArgs((char)((ushort)((long)msg.WParam)));
                    result = listener.OnPreviewKeyPress(target, e);
                    break;
                case NativeMethods.WM_KEYUP:
                case NativeMethods.WM_SYSKEYUP:
                    keyData = ((Keys)((int)((long)msg.WParam))) | Control.ModifierKeys;
                    result = listener.OnPreviewKeyUp(target, new KeyEventArgs(keyData));
                    break;
            }

            return result;
        }

        void IMessageListener.PreviewWndProc(Message msg)
        {
            throw new NotImplementedException();
        }

        void IMessageListener.PreviewSystemMessage(SystemMessage message, Message msg)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
