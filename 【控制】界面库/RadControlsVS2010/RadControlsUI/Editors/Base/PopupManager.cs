using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class manages all opened popups per UI thread.
    /// </summary>
    internal sealed class PopupManager : IMessageListener
    {
        private List<IPopupControl> popups = new List<IPopupControl>();
        private IPopupControl lastActivatedPopup = null;
        // Boolean field for testing purposes.
        private bool hooked = false;

        // Private constructor to prevent users from creating instances.
        private PopupManager()
        {
        }

        // Gets a value indicating whether the popup manager has hooks installed.
        // For testing purposes only.
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Hooked
        {
            get
            {
                return this.hooked;
            }
        }

        /// <summary>
        /// Gets the count of the IPopupControl instances
        /// currently registered in the PopupManager.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int PopupCount
        {
            get
            {
                return this.popups.Count;
            }
        }

        /// <summary>
        /// The popup which was last activated.
        /// </summary>
        public IPopupControl LastActivatedPopup
        {
            get 
            { 
                return lastActivatedPopup; 
            }
            set 
            { 
                lastActivatedPopup = value; 
            }
        }

        #region IMessageFilter Members

        /// <summary>
        /// Adds a popup form to the popups of the PopupManager and 
        /// registers a message hook if the form provided is the first one.
        /// </summary>
        /// <param name="form">The popup to add.</param>
        public void AddPopup(IPopupControl form)
        {
            if (!popups.Contains(form))
            {

                IPopupControl ownerPopup = form.OwnerPopup;

                if (ownerPopup != null)
                {
                    ownerPopup.Children.Add(form);
                }

                popups.Add(form);

                this.lastActivatedPopup = form;
            }

            if (popups.Count > 0 && !this.hooked)
            {
                RadMessageFilter.Instance.AddListener(this);
                this.hooked = true;
            }
        }

        /// <summary>
        /// Removes the provided popup from the popups of the PopupManager and unregisters the message hook
        /// if there are no more popups.
        /// </summary>
        /// <param name="form">The popup to remove.</param>
        public void RemovePopup(IPopupControl form)
        {
            if (popups.Contains(form))
            {
                while (form.Children.Count > 0)
                {
                    IPopupControl child = form.Children[form.Children.Count - 1];

                    child.ClosePopup(RadPopupCloseReason.ParentClosed);
                }

                IPopupControl ownerPopup = form.OwnerPopup;

                if (ownerPopup != null)
                {
                    System.Diagnostics.Debug.Assert(
                        ownerPopup.Children.Contains(form),
                        "Not contained in owner's Children collection!");
                    ownerPopup.Children.Remove(form);
                }

                popups.Remove(form);
            }

            if (popups.Count == 0)
            {
                this.lastActivatedPopup = null;
                RadMessageFilter.Instance.RemoveListener(this);
                this.hooked = false;
            }
            else
            {
                this.lastActivatedPopup = this.popups[this.popups.Count - 1];
            }
        }

        /// <summary>
        /// Attempts to close an <see cref="IPopupControl"/> implementation.
        /// </summary>
        /// <param name="popup">The popup to close.</param>
        public bool ClosePopup(IPopupControl popup)
        {
            if (!this.ContainsPopup(popup))
            {
                return false;
            }

            if (popup.CanClosePopup(RadPopupCloseReason.CloseCalled))
            {
                PopupCloseInfo closeInfo = new PopupCloseInfo(RadPopupCloseReason.CloseCalled, null);
                popup.ClosePopup(closeInfo);
                return closeInfo.Closed;
            }

            return false;
        }

        /// <summary>
        /// Closes all popups managed by the PopupManager.
        /// </summary>
        /// <param name="reason">Clarification why all popups need to be closed.</param>
        public void CloseAll(RadPopupCloseReason reason)
        {
            while (this.popups.Count > 0)
            {
                this.lastActivatedPopup = this.popups[this.popups.Count - 1];
                if (!this.lastActivatedPopup.CanClosePopup(reason))
                {
                    return;
                }
                PopupCloseInfo info = new PopupCloseInfo(reason, null);
                this.lastActivatedPopup.ClosePopup(info);

                if (!info.Closed)
                {
                    return;
                }
            }

            this.lastActivatedPopup = null;
        }

        /// <summary>
        /// Closes all popups from a leaf to the root.
        /// </summary>
        /// <param name="reason">The reason why popups are closed.</param>
        /// <param name="leaf">The leaf popup from which to start closing the hierarchy.</param>
        public void CloseAllToRoot(RadPopupCloseReason reason, IPopupControl leaf)
        {
            if (leaf.Children.Count > 0)
                throw new InvalidOperationException("The provided IPopupControl is not a leaf");

            bool finished = false;


            if (leaf.CanClosePopup(reason))
            {
                PopupCloseInfo info = new PopupCloseInfo(reason, null);
                leaf.ClosePopup(info);

                if (!info.Closed)
                {
                    return;
                }
            }
            else
            {
                return;
            }

            IPopupControl parent = leaf.OwnerPopup;

            while (!finished)
            {
                if (parent != null && parent.CanClosePopup(reason))
                {
                    PopupCloseInfo info = new PopupCloseInfo(reason, null);
                    parent.ClosePopup(info);

                    if (!info.Closed)
                    {
                        finished = true;
                    }

                    parent = parent.OwnerPopup;
                }
                else
                {
                    finished = true;
                }
            }
        }

        /// <summary>
        /// Checks if the PopupManager monitors the provided popup.
        /// </summary>
        /// <param name="form">The popup to check for.</param>
        /// <returns></returns>
        public bool ContainsPopup(IPopupControl form)
        {
            return popups.Contains(form);
        }

        private void OnActivate(IntPtr param)
        {
            if (param == IntPtr.Zero && popups.Count != 0)
            {
                CloseAll(RadPopupCloseReason.AppFocusChange);
            }
        }

        private bool OnKeyDown(ref Message msg)
        {
            return this.lastActivatedPopup.OnKeyDown((Keys)msg.WParam);
        }

        private bool OnMouseWheel(Message m)
        {
            Debug.Assert(this.lastActivatedPopup != null, "Invalid PopupManager state.");
            if (this.lastActivatedPopup == null)
            {
                return false;
            }

            Control target = Control.FromChildHandle(m.HWnd);
            if (target == null)
            {
                return false;
            }

            int delta = NativeMethods.Util.SignedHIWORD(m.WParam);
            return this.lastActivatedPopup.OnMouseWheel(target, delta);
        }

        /// <summary>
        /// This method begins to close all IPopupControl instances
        /// starting from the end of the collection. If a IPopupControl
        /// cannot be closed, the iteration stops and all popups previously added
        /// to the collection will not be closed.
        /// </summary>
        internal void OnMouseDown(Point cursorPos)
        {
            System.Diagnostics.Debug.Assert(this.popups.Count != 0, "Popup count must not be equal to zero when receiving this event!");

            int i = this.popups.Count - 1;
            bool blockOwners = false;

            while (i > -1 && this.popups.Count > 0)
            {
                IPopupControl currentPopup = this.popups[i--];
                bool containsMouse = currentPopup.Bounds.Contains(cursorPos);

                if (!blockOwners
                    && !containsMouse)
                {
                    if (currentPopup.CanClosePopup(RadPopupCloseReason.Mouse))
                    {
                        PopupCloseInfo closeInfo = new PopupCloseInfo(RadPopupCloseReason.Mouse, null);
                        currentPopup.ClosePopup(closeInfo);

                        blockOwners = !closeInfo.Closed;
                    }
                    else
                    {
                        blockOwners = true;
                    }
                }
                else
                {
                    if (currentPopup.OwnerPopup == null)
                    {
                        blockOwners = false;
                    }
                    else if (containsMouse)
                    {
                        blockOwners = true;
                    }
                }
            }
        }

        [ThreadStatic]
        private static PopupManager defaultManager = null;

        /// <summary>
        /// Gets the only instance of the PopupManager class. Other instances can not be created.
        /// </summary>
        public static PopupManager Default
        {
            get
            {
                if (defaultManager == null)
                {
                    defaultManager = new PopupManager();
                }
                return defaultManager;
            }
        }
        #endregion

        #region IMessageListener Members

        InstalledHook IMessageListener.DesiredHook
        {
            get 
            {
                return InstalledHook.CallWndProc | InstalledHook.GetMessage;
            }
        }

        MessagePreviewResult IMessageListener.PreviewMessage(ref Message msg)
        {
            switch (msg.Msg)
            {
                case NativeMethods.WM_LBUTTONDOWN:
                case NativeMethods.WM_RBUTTONDOWN:
                case NativeMethods.WM_MBUTTONDOWN:
                case NativeMethods.WM_NCLBUTTONDOWN:
                case NativeMethods.WM_NCRBUTTONDOWN:
                case NativeMethods.WM_NCMBUTTONDOWN:
                    OnMouseDown(Cursor.Position);
                    break;
                case NativeMethods.WM_MOUSEWHEEL:
                    if (!this.OnMouseWheel(msg))
                    {
                        this.CloseAll(RadPopupCloseReason.Mouse);
                    }
                    else
                    {
                        return MessagePreviewResult.Processed | MessagePreviewResult.NoContinue | MessagePreviewResult.NoDispatch;
                    }
                    break;
                case NativeMethods.WM_KEYDOWN:
                case NativeMethods.WM_SYSKEYDOWN:
                    {
                        bool processed = OnKeyDown(ref msg);
                        if (processed)
                        {
                            return MessagePreviewResult.ProcessedNoDispatch;
                        }
                    }
                    break;
            }

            return MessagePreviewResult.Processed;
        }

        void IMessageListener.PreviewWndProc(Message msg)
        {
            switch (msg.Msg)
            {
                case NativeMethods.WM_ACTIVATEAPP:
                    OnActivate(msg.WParam);
                    break;
            }
        }

        void IMessageListener.PreviewSystemMessage(SystemMessage message, Message msg)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
