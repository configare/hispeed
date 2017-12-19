using System;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System.ComponentModel;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Base class for all Popup Forms that are used with a RadDock instance.
    /// Such Forms are <see cref="FloatingWindow"/> and <see cref="AutoHidePopup"/> classes.
    /// </summary>
    [ToolboxItem(false)]
    public abstract class DockPopupForm : RadForm
    {
        #region Constructor

        /// <summary>
        /// Constructs new DockPopupForm instance, associated with the specified RadDock instance.
        /// </summary>
        /// <param name="dockManager">The RadDock instance this Popup is attached to.</param>
        public DockPopupForm(RadDock dockManager)
        {
            this.dockManager = dockManager;
            this.AllowTheming = this.dockManager.EnableFloatingWindowTheming;

            this.ShowIcon = false;
        }

        internal DockPopupForm(RadDock dockManager, bool preventNCActivate)
            :this(dockManager)
        {
            if (preventNCActivate)
            {
                this.popupHelper = new PopupWindowHelper();
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Releases the resources used by this instance.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (popupHelper != null)
            {
                this.popupHelper.ReleaseHandle();
            }

            base.Dispose(disposing);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            if (this.popupHelper != null)
            {
                this.popupHelper.ReleaseHandle();
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (this.popupHelper != null)
            {
                Form owner = this.dockManager.FindForm();
                if (owner != null)
                {
                    owner.AddOwnedForm(this);
                    this.popupHelper.AssignHandle(owner.Handle);
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.loaded = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            this.loaded = false;
        }

        #endregion

        /// <summary>
        /// Gets the RadDock instance this Popup Form is associated with.
        /// </summary>
        protected internal RadDock DockManager
        {
            get 
            { 
                return this.dockManager; 
            }
        }

        #region Fields

        internal RadDock dockManager;
        internal bool loaded;

        private PopupWindowHelper popupHelper;

        #endregion

        #region Nested Types

        internal class PopupWindowHelper : NativeWindow
        {
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                if (m.Msg == NativeMethods.WM_NCACTIVATE)
                {
                    if (((int)m.WParam) == 0)
                    {
                        NativeMethods.SendMessage(this.Handle, NativeMethods.WM_NCACTIVATE, 1, 0);
                    }
                }
                else if (m.Msg == NativeMethods.WM_ACTIVATEAPP)
                {
                    if ((int)m.WParam == 0)
                    {
                        NativeMethods.PostMessage(this.Handle, NativeMethods.WM_NCACTIVATE, 0, IntPtr.Zero);
                    }
                }
            }
        }

        #endregion
    }
}
