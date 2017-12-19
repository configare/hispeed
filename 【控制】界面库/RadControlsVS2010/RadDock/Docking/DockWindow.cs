using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System;
using System.Diagnostics;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Defines the common properties for a RadDock-related object such as DockWindow, FloatingWindow, AutoHidePopup, etc.
    /// </summary>
    public interface IDockWindow
    {
        /// <summary>
        /// Gets the unique name of this instance.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets or sets the text associated with this instance.
        /// </summary>
        string Text { get; set; }
        /// <summary>
        /// Gets the RadDock instance this object is associated with.
        /// </summary>
        RadDock DockManager { get; }
    }

    /// <summary>
    /// Base class for all tool and document windows available per <see cref="RadDock">RadDock</see> instance.
    /// Implements the IDockWindow interface.
    /// </summary>
    [Designer("Telerik.WinControls.UI.Design.DockWindowDesigner, Telerik.WinControls.UI.Design, Version=" + VersionNumber.Number + ", Culture=neutral, PublicKeyToken=5bb2a467cbec794e")]
    [ToolboxItem(false)]
    public abstract class DockWindow : TabPanel, IDockWindow
    {
        #region Fields

        private AllowedDockState allowDockStates;
        private DockWindowCloseAction closeAction;
        private RadDock dockManager;
        private DockState dockState;
        private DockState desiredDockState;
        private DockState previousDockState;
        private Size autoHideSize;
        private TabStripItem autoHideTab;
        private AutoHideGroup autoHideGroup;
        private ToolStripCaptionButtons toolCaptionButtons;
        private DocumentStripButtons documentButtons;
        private Size defaultFloatingSize;
        private Color localBackColor;

        #endregion

        /// <summary>
        /// Initializes a new <see cref="DockWindow">DockWindow</see> instance.
        /// </summary>
        public DockWindow()
        {
            this.dockState = this.DefaultDockState;
            this.previousDockState = this.dockState;
            this.desiredDockState = this.dockState;
            this.autoHideSize = Size.Empty;
            this.allowDockStates = AllowedDockState.All;
            this.closeAction = this.DefaultCloseAction;

            this.toolCaptionButtons = ToolStripCaptionButtons.All;
            this.documentButtons = DocumentStripButtons.All;
            this.defaultFloatingSize = FloatingWindow.DefaultFloatingSize;
        }

        /// <summary>
        /// Retrieves the default DockState for this instance. Will be <see cref="Telerik.WinControls.UI.Docking.DockState.Docked">Docked</see> for <see cref="ToolWindow">ToolWindow</see> instances
        /// and <see cref="Telerik.WinControls.UI.Docking.DockState.TabbedDocument">TabbedDocument</see> for <see cref="DocumentWindow">DocumentWindow</see> instances.
        /// </summary>
        protected internal virtual DockState DefaultDockState
        {
            get
            {
                return DockState.Docked;
            }
        }

        /// <summary>
        /// Retrieves the default <see cref="DockWindowCloseAction">CloseAction</see> for this instance. Will be <see cref="DockWindowCloseAction.Hide">Hide</see> for <see cref="ToolWindow">ToolWindow</see> instances
        /// and <see cref="DockWindowCloseAction.CloseAndDispose">CloseAndDispose</see> for <see cref="DocumentWindow">DocumentWindow</see> instances.
        /// </summary>
        protected internal virtual DockWindowCloseAction DefaultCloseAction
        {
            get
            {
                return DockWindowCloseAction.Hide;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);

            if (this.dockManager != null && this.dockManager.ShouldProcessNotification())
            {
                this.dockManager.ActiveWindow = this;
            }
        }

        /// <summary>
        /// Provides special handling for the WM_SETFOCUS notification.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (this.dockManager == null)
            {
                return;
            }

            switch (m.Msg)
            {
                case NativeMethods.WM_SETFOCUS:
                    this.dockManager.ActiveWindow = this;
                    break;
                case NativeMethods.WM_PARENTNOTIFY:
                    int message = NativeMethods.Util.LowOrder(m.WParam.ToInt32());
                    if (message == NativeMethods.WM_LBUTTONDOWN ||
                        message == NativeMethods.WM_RBUTTONDOWN ||
                        message == NativeMethods.WM_XBUTTONDOWN)
                    {
                        Control hit = ControlHelper.GetControlUnderMouse();
                        if (hit == null)
                        {
                            break;
                        }

                        if (hit == this || !ControlHelper.GetControlStyle(hit, ControlStyles.Selectable))
                        {
                            this.dockManager.ActiveWindow = this;                            
                        }
                    }
                    break;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (this.dockManager != null)
            {
                this.dockManager.ActiveWindow = this;
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the associated command buttons when the window resides in a ToolTabStrip instance.
        /// </summary>
        [DefaultValue(ToolStripCaptionButtons.All)]
        [Description("Gets or sets the visibility of the associated command buttons when the window resides in a ToolTabStrip instance.")]
        public ToolStripCaptionButtons ToolCaptionButtons
        {
            get
            {
                return this.toolCaptionButtons;
            }
            set
            {
                if (this.toolCaptionButtons == value)
                {
                    return;
                }

                this.toolCaptionButtons = value;
                DockTabStrip strip = this.DockTabStrip;
                if (strip != null)
                {
                    strip.UpdateButtons();
                }
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the associated command buttons when the window resides in a DocumentTabStrip instance.
        /// </summary>
        [DefaultValue(DocumentStripButtons.All)]
        [Description("Gets or sets the visibility of the associated command buttons when the window resides in a DocumentTabStrip instance.")]
        public DocumentStripButtons DocumentButtons
        {
            get
            {
                return this.documentButtons;
            }
            set
            {
                if (this.documentButtons == value)
                {
                    return;
                }

                this.documentButtons = value;
                DockTabStrip strip = this.DockTabStrip;
                if (strip != null)
                {
                    strip.UpdateButtons();
                }
            }
        }

        /// <summary>
        /// Gets the DockTabStrip which hosts this window.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockTabStrip DockTabStrip
        {
            get
            {
                return this.Parent as DockTabStrip;
            }
        }

        /// <summary>
        /// Gets or sets the size to be used when the window is floated for the first time and does not have previous floating state saved.
        /// </summary>
        [Description("Gets or sets the size to be used when the window is floated for the first time and does not have previous floating state saved.")]
        public Size DefaultFloatingSize
        {
            get
            {
                return this.defaultFloatingSize;
            }
            set
            {
                if (value.Width < 0)
                {
                    value.Width = 0;
                }
                if (value.Height < 0)
                {
                    value.Height = 0;
                }

                this.defaultFloatingSize = value;
            }
        }

        public override Color BackColor
        {
            get
            {
                if (this.localBackColor == Color.Empty)
                {
                    DockTabStrip owner = this.DockTabStrip;
                    if (owner != null)
                    {
                        return owner.DefaultDockWindowBackColor;
                    }
                }
                return base.BackColor;
            }
            set
            {
                this.localBackColor = value;
                base.BackColor = value;
            }
        }

        bool ShouldSerializeBackColor()
        {
            return this.localBackColor != Color.Empty;
        }

        bool ShouldSerializeDefaultFloatingSize()
        {
            return this.defaultFloatingSize != FloatingWindow.DefaultFloatingSize;
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Size AutoHideSize
        {
            get
            {
                return autoHideSize;
            }
            set
            {
                autoHideSize = value;
                if (this.DockManager != null)
                {
                    this.DockManager.OnDockWindowAutoHideSizeChanged(this);
                }
            }
        }

        /// <summary>
        /// Asks the current <see cref="DockWindow.DockManager">DockManager</see> instance (if any) to close the window.
        /// </summary>
        public void Close()
        {
            if (this.dockManager != null)
            {
                this.dockManager.CloseWindow(this);
            }
        }

        /// <summary>
        /// Ensures that the window is currently visible on its hosting <see cref="DockTabStrip">DockTabStrip</see>.
        /// </summary>
        public void EnsureVisible()
        {
            DockTabStrip strip = (DockTabStrip)this.TabStrip;
            if (strip == null)
            {
                return;
            }

            strip.SuspendStripNotifications(false, true);
            //strip.SelectTabItem(this.TabStripItem);
            strip.ActiveWindow = this;
            strip.ResumeStripNotifications(false, true);
            strip.UpdateTabSelection(true);
        }

        /// <summary>
        /// Get or set allowed dock states for DockWindow instance
        /// </summary>
        [DefaultValue(AllowedDockState.All)]
        [Browsable(false)]
        public AllowedDockState AllowedDockState
        {
            get
            {
                return allowDockStates;
            }
            set
            {
                allowDockStates = value;
            }
        }

        /// <summary>
        /// Gets or sets the action to be used when a Close request occurs for this window.
        /// </summary>
        [Description("Gets or sets the action to be used when a Close request occurs for this window.")]
        public virtual DockWindowCloseAction CloseAction
        {
            get
            {
                return this.closeAction;
            }
            set
            {
                this.closeAction = value;
            }
        }

        bool ShouldSerializeCloseAction()
        {
            return this.closeAction != this.DefaultCloseAction;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TabStripItem AutoHideTab
        {
            get
            {
                return this.autoHideTab;
            }
            internal set
            {
                this.autoHideTab = value;
            }
        }


        /// <summary>
        /// Asks the current <see cref="DockWindow.DockManager">DockManager</see> instance (if any) to dock the window to the specified target, using the desired position.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="position"></param>
        public void DockTo(DockWindow target, DockPosition position)
        {
            if (target != null && target.DockManager!= null)
            {
                target.DockManager.DockWindow(this, target, position);
            }
        }

        internal void UpdateActiveState(bool active)
        {
            DockTabStrip strip = this.DockTabStrip;
            if (strip != null)
            {
                strip.UpdateActiveWindow(this, active);
            }
        }

        internal AutoHideGroup AutoHideGroup
        {
            get
            {
                return this.autoHideGroup;
            }
            set
            {
                this.autoHideGroup = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DockState PreviousDockState
        {
            get
            {
                return this.previousDockState;
            }
            set
            {
                if (value == DockState.AutoHide)
                {
                    value = DockState.Docked;
                }

                this.previousDockState = value;
            }
        }

        /// <summary>
        /// Gets or sets the DockState which is desired for the window
        /// and will be applied when the RadDock instance this window is attached to is completely initialized.
        /// This property is used internally by the framework and is not intended to be used directly by code.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DockState DesiredDockState
        {
            get
            {
                return this.desiredDockState;
            }
            set
            {
                if (this.desiredDockState == value)
                {
                    return;
                }

                this.desiredDockState = value;
            }
        }

        /// <summary>
        /// Get the parent floating window when the window is in floating mode
        /// </summary>
		[Browsable(false)]
        public FloatingWindow FloatingParent
        {
            get
            {
                return this.FindForm() as FloatingWindow;
            }
        }

        private bool ShouldSerializeDesiredDockState()
        {
            return this.desiredDockState == DockState.Hidden;
        }

        /// <summary>
        /// Gets the current dock state of the window.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockState DockState
        {
            get
            {
                return this.dockState;
            }
            set
            {
                if (this.dockState == value)
                {
                    return;
                }

                if (this.dockManager != null)
                {
                    this.dockManager.SetWindowState(this, value);
                }
                else
                {
                    this.desiredDockState = value;
                }
            }
        }

        internal void ResetDesiredDockState()
        {
            this.desiredDockState = this.DefaultDockState;
        }
        
        internal DockState InnerDockState
        {
            get
            {
                return this.dockState;
            }
            set
            {
                this.dockState = value;
                if (value == DockState.Hidden)
                {
                    this.desiredDockState = DockState.Hidden;
                }
                else
                {
                    this.ResetDesiredDockState();
                }
            }
        }

        /// <summary>
        /// Hide the ToolWindow from RadDock manager
        /// </summary>
        public new void Hide()
        {
            if (this.dockManager != null)
            {
                this.DockState = DockState.Hidden;
            }
            else
            {
                base.Hide();
            }
        }

        /// <summary>
        /// Display the ToolWindow if was previously hidden.
        /// </summary>
        public new void Show()
        {
            if (this.dockManager != null)
            {
                this.dockManager.DisplayWindow(this);
            }
            else
            {
                base.Show();
            }
        }

        /// <summary>
        /// Get the floating status of window
        /// </summary>
        [Browsable(false)]
        public bool IsInFloatingMode
        {
            get
            {
                return this.FloatingParent != null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override ISite Site
        {
            get
            {
                return base.Site;
            }
            set
            {
                //set the name of the site as local, since it is shadowed at design-time
                if (value != null)
                {
                    this.Name = value.Name;
                    this.Text = value.Name;
                }
                base.Site = value;
            }
        }

        /// <summary>
        /// Gets or sets value representing unique Name of the DockWindow
        /// </summary>
        [Browsable(false)]
        public new string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                if (base.Name == value)
                {
                    return;
                }

                string oldValue = base.Name;
                base.Name = value;
                this.OnNameChanged(oldValue);
            }
        }

        /// <summary>
        /// Notifies the owning <see cref="RadDock">RadDock</see> instance for a change in the Name value.
        /// </summary>
        /// <param name="oldName"></param>
        protected virtual void OnNameChanged(string oldName)
        {
            if (this.dockManager != null)
            {
                this.dockManager.OnDockWindowNameChanged(this, oldName);
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            this.UpdateOnTextChanged();
        }

        /// <summary>
        /// Updates all associated UI - such as TabItems, Captions, etc. upon TextChanged event.
        /// </summary>
        protected virtual void UpdateOnTextChanged()
        {
            if (this.TabStripItem != null)
            {
                this.TabStripItem.Text = this.Text;
            }

            if (this.autoHideTab != null)
            {
                this.autoHideTab.Text = this.Text;
            }

            ToolTabStrip parentStrip = this.TabStrip as ToolTabStrip;
            if (parentStrip != null && parentStrip.ActiveWindow == this)
            {
                parentStrip.CaptionText = this.Text;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetType().FullName + "; Text: " + this.Text;
        }

        protected override void OnImageChanged(EventArgs e)
        {
            base.OnImageChanged(e);

            if (this.autoHideTab != null)
            {
                this.autoHideTab.Image = this.Image;
            }
        }

        protected override void OnToolTipTextChanged(EventArgs e)
        {
            base.OnToolTipTextChanged(e);

            if (this.autoHideTab != null)
            {
                this.autoHideTab.ToolTipText = this.ToolTipText;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this.dockManager = null;

            base.Dispose(disposing);
        }

        /// <summary>
        /// Called by the owning RadDock instance when this window is about to close.
        /// Allows specific DockWindow implementations to optionally cancel the operation and/or perform additional actions.
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnClosing(DockWindowCancelEventArgs e)
        {
        }

        /// <summary>
        /// Called by the owning RadDock instance when the window has been successfully closed.
        /// Depending on the current CloseAction, the window will be either hidden, removed or both plus disposed when entering this method.
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnClosed(DockWindowEventArgs e)
        {
        }

        #region IDockWindow Members

        /// <summary>
        /// Gets the current <see cref="RadDock">RadDock</see> instance this window is associated with.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual RadDock DockManager
        {
            get
            {
                return this.dockManager;
            }
            internal set
            {
                this.dockManager = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new Padding Padding
        {
            get
            {
                return base.Padding;
            }
            set
            {
                base.Padding = value;
            }
        }


        /// <summary>
        /// Gets the <see cref="DockType">DockType</see> of this instance.
        /// </summary>
        [Browsable(false)]
        public virtual DockType DockType
        {
            get
            {
                return DockType.ToolWindow;
            }
        }

        #endregion
    }
}
