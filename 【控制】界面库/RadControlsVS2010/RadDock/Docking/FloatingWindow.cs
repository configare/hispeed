using System.ComponentModel;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;

namespace Telerik.WinControls.UI.Docking
{ 
    /// <summary>
    /// Represents a popup form that is used to host floating <see cref="DockWindow">DockWindow</see> instances.
    /// </summary>
    public class FloatingWindow : DockPopupForm
    {
        #region Fields

        private RadSplitContainer dockContainer;
        private bool dragged;
        private bool captionCapture;
        private bool captionDoubleClick;
        private Point captionDragStart;
        private int zIndex;

        /// <summary>
        /// Defines the default size for floating windows.
        /// </summary>
        public static readonly Size DefaultFloatingSize = new Size(800, 600);

        #endregion

        #region Constructor/Public Properties

        static FloatingWindow()
        {
            //ThemeResolutionService.RegisterThemeFromStorage(ThemeStorageType.Resource, "Telerik.WinControls.UI.Docking.Resources.ControlDefault.FloatingWindow.xml");
            new Telerik.WinControls.Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_Docking_FloatingWindow().DeserializeTheme();
        }

        /// <summary>
        /// This method supports internal RadDock infrastructure and should not be called directly from your code
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public FloatingWindow()
            : this(null)
        {
        }

        /// <summary>
        /// Creates a new FloatingWindow instance passing an owner RadDock instance as a parameter
        /// </summary>
        public FloatingWindow(RadDock dockManager)
            :base(dockManager)
        {
            base.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.ShowInTaskbar = false;
            base.SizeGripStyle = SizeGripStyle.Hide;
            base.StartPosition = FormStartPosition.Manual;

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.DoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);

            this.dockContainer = new RadSplitContainer();
            this.dockContainer.Dock = DockStyle.Fill;
            this.dockContainer.ThemeClassName = RadDock.DockSplitContainerThemeClassName;
            this.dockContainer.ControlAdded += OnDockContainer_ControlAdded;
            this.dockContainer.ControlRemoved += OnDockContainer_ControlRemoved;
            this.dockContainer.PanelCollapsedChanged += OnDockContainer_PanelCollapsedChanged;
            this.dockContainer.ControlTreeChanged += OnDockContainer_ControlTreeChanged;

            this.Controls.Add(this.dockContainer);

            if (dockManager != null)
            {
                this.dockContainer.ThemeName = dockManager.ThemeName;
                this.RightToLeft = dockManager.RightToLeft;
            }
            this.StartPosition = FormStartPosition.Manual; 
        }


        /// <summary>
        /// Gets the SplitContiner instance that is the root of the hierarchy of DockWindows inside this FloatingWindow
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadSplitContainer DockContainer
        {
            get
            {
                return this.dockContainer;
            }
        }

        #endregion

        #region Event Handlers/Notifications

        private void OnDockContainer_ControlTreeChanged(object sender, ControlTreeChangedEventArgs args)
        {
            if (this.dockManager != null)
            {
                this.dockManager.OnControlTreeChanged(args);
            }
        }

        private void OnDockContainer_PanelCollapsedChanged(object sender, SplitPanelEventArgs e)
        {
            this.UpdateCaptions(true);
        }

        private void OnDockContainer_ControlRemoved(object sender, ControlEventArgs e)
        {
            this.UpdateCaptions(true);
        }

        private void OnDockContainer_ControlAdded(object sender, ControlEventArgs e)
        {
            this.UpdateCaptions(true);
        }

        internal void OnSelectedTabChanged(DockTabStrip strip)
        {
            this.UpdateCaptions(true);
        }

        internal void SetDragged(bool drag)
        {
            if (this.dragged == drag)
            {
                return;
            }

            this.captionCapture = false;
            this.dragged = drag;
            if (drag)
            {
                this.Invalidate(true);
                this.Update();
            }
        }

        #endregion

        #region Properties

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal int ZIndex
        {
            get
            {
                return zIndex;
            }            
            set
            {
                zIndex = value;
            }
        }

        #endregion

        #region Overrides

        //Override the ControlDefinesThemeForElement method to enable
        //the separated styling of the title bar of a floating window.
        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            if (element is RadFormTitleBarElement)
            {
                return true;
            }

            return base.ControlDefinesThemeForElement(element);
        }

        //Override the themeclassname property to enable
        //separate themes for floating windows and radforms
        public override string ThemeClassName
        {
            get
            {
                return typeof(FloatingWindow).FullName;
            }
            set
            {
                base.ThemeClassName = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dockContainer.ControlAdded -= OnDockContainer_ControlAdded;
                this.dockContainer.ControlRemoved -= OnDockContainer_ControlRemoved;
                this.dockContainer.PanelCollapsedChanged -= OnDockContainer_PanelCollapsedChanged;
                this.dockContainer.ControlTreeChanged -= OnDockContainer_ControlTreeChanged;
            }

            base.Dispose(disposing);
        }

        protected override void OnLoad(EventArgs e)
        {
            this.UpdateCaptions(false);

            base.OnLoad(e);
        }

        /// <summary>
        /// Delegate the event to all nested ToolTabStrip instances.
        /// </summary>
        protected override void OnThemeChanged()
        {
            base.OnThemeChanged();

            if (this.dockContainer == null)
            {
                return;
            }

            this.dockContainer.ThemeName = this.ThemeName;

            foreach (DockTabStrip strip in ControlHelper.GetChildControls<DockTabStrip>(this.DockContainer, true))
            {
                if (strip.DockManager == this.dockManager)
                {
                    strip.ThemeName = this.ThemeName;
                }
            }
        }

        /// <summary>
        /// Provides support for z-ordering of all popup-forms of the owning Form.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (this.dockManager == null)
            {
                return;
            }

            Form dockForm = this.dockManager.FindForm();
            if (dockForm != null)
            {
                //remove and add ourselves again to update the z-order
                dockForm.RemoveOwnedForm(this);
                dockForm.AddOwnedForm(this);
            }

            DockWindow activeWindow = this.dockManager.ActiveWindow;
            if (activeWindow != null && activeWindow.FindForm() == this)
            {
                return;
            }

            foreach (DockTabStrip strip in ControlHelper.GetChildControls<DockTabStrip>(this.dockContainer, true))
            {
                if (strip.ActiveWindow != null && strip.DockManager == this.dockManager)
                {
                    this.dockManager.ActiveWindow = strip.ActiveWindow;
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_NCLBUTTONUP:
                    this.captionCapture = false;
                    if (this.captionDoubleClick)
                    {
                        RestoreAllDockWindows();
                        this.captionDoubleClick = false;
                        return;
                    }
                    break;
                case NativeMethods.WM_NCLBUTTONDBLCLK:
                    if (m.WParam.ToInt64() == NativeMethods.HTCAPTION)
                    {
                        this.captionDoubleClick = true;
                    }
                    break;
                case NativeMethods.WM_NCLBUTTONDOWN:
                    //Handle NC_LBUTTONDOWN to prevent WM_ENTERSIZEMOVE modal loop
                    if (m.WParam.ToInt64() == NativeMethods.HTCAPTION && this.IsInDragArea())
                    {
                        this.captionDoubleClick = false;
                        this.captionCapture = true;
                        captionDragStart = Control.MousePosition;
                        ControlHelper.BringToFront(m.HWnd, true);
                        return;
                    }
                    break;
                case NativeMethods.WM_NCMOUSEMOVE:
                    if (captionCapture)
                    {
                        if (Control.MouseButtons != MouseButtons.Left)
                        {
                            captionCapture = false;
                        }
                        else if (DockHelper.ShouldBeginDrag(Control.MousePosition, this.captionDragStart))
                        {
                            this.dockManager.BeginDrag(this);
                        }
                        return;
                    }
                    break;
                case NativeMethods.WM_NCHITTEST:
                    if (this.dragged)
                    {
                        //we are not target of mouse input while being dragged
                        m.Result = new IntPtr(NativeMethods.HTTRANSPARENT);
                        return;
                    }
                    break;
                case NativeMethods.WM_NCRBUTTONDOWN:
                    //prevent right-button down within the non-client area
                    if (this.dockManager != null)
                    {
                        return;
                    }
                    break;
                case NativeMethods.WM_NCRBUTTONUP:
                    if (this.dockManager == null)
                    {
                        break;
                    }

                    DockWindow activeWindow = this.dockManager.ActiveWindow;
                    if(activeWindow == null || activeWindow.FloatingParent != this)
                    {
                        break;
                    }

                    ContextMenuService service = this.DockManager.GetService<ContextMenuService>(ServiceConstants.ContextMenu);
                    if (service != null)
                    {
                        service.DisplayContextMenu(this.dockManager.ActiveWindow, Control.MousePosition);
                    }
                    return;
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            base.OnClosing(e);

            this.dockManager.CloseWindows(DockHelper.GetDockWindows(this.dockContainer, true, this.dockManager));
        }

        #endregion

        #region Helper Methods

        private void RestoreAllDockWindows()
        {
            if (!this.dockManager.ShouldProcessNotification())
            {
                return;
            }

            RedockService service = this.dockManager.GetService<RedockService>(ServiceConstants.Redock);
            if (service != null && service.CanOperate())
            {
                service.Redock(DockHelper.GetDockWindows(this.dockContainer, true, this.dockManager), RedockTransactionReason.FloatingWindowCaptionDoubleClick, true);
            }
        }

        private bool IsInDragArea()
        {
            Point client = this.PointToClient(Control.MousePosition);
            client.X += this.FormBehavior.ClientMargin.Left;
            client.Y += this.FormBehavior.ClientMargin.Top;

            RadElement el = this.ElementTree.GetElementAtPoint(client);
            if (el != null && el is RadButtonItem)
            {
                return false;
            }

            return true;
        }

        private void UpdateCaptions(bool checkDockManager)
        {
            List<ToolTabStrip> visibleStrips = new List<ToolTabStrip>();
            foreach (Control child in ControlHelper.EnumChildControls(this.dockContainer, true))
            {
                ToolTabStrip strip = child as ToolTabStrip;
                if (strip == null || strip.Collapsed)
                {
                    continue;
                }
                if (checkDockManager && strip.DockManager != this.dockManager)
                {
                    continue;
                }

                visibleStrips.Add(strip);
            }

            bool hasCaption = visibleStrips.Count > 1;

            foreach (ToolTabStrip strip in visibleStrips)
            {
                strip.CanDisplayCaption = hasCaption;
                strip.PerformLayout();
            }

            if (hasCaption)
            {
                this.Text = string.Empty;
            }
            else if (visibleStrips.Count == 1)
            {
                DockWindow window = ((DockTabStrip)visibleStrips[0]).ActiveWindow;
                if (window != null)
                {
                    this.Text = window.Text;
                }
            }

            this.dockContainer.PerformLayout();
        }

        internal void UpdateVisibility()
        {
            if (this.IsDisposed || this.Disposing)
            {
                return;
            }

            bool hasRedockTargets = false;
            bool hasVisibleStrips = false;

            foreach (DockTabStrip strip in ControlHelper.GetChildControls<DockTabStrip>(this.dockContainer, true))
            {
                if (strip.Disposing || strip.IsDisposed)
                {
                    continue;
                }

                if (strip.IsRedockTarget)
                {
                    hasRedockTargets = true;
                }
                if (strip.TabPanels.Count > 0)
                {
                    hasVisibleStrips = true;
                    break;
                }
            }

            if (hasVisibleStrips)
            {
                if (!this.Visible)
                {
                    this.Show();
                }
                return;
            }


            if (hasRedockTargets)
            {
                this.Hide();
            }
            else
            {
                this.Dispose();
            }
        }

        internal void RemovePlaceHolders()
        {
            foreach (DockWindowPlaceholder holder in ControlHelper.GetChildControls<DockWindowPlaceholder>(this.dockContainer, true))
            {
                DockTabStrip parent = holder.DockTabStrip;
                Debug.Assert(parent != null, "Invalid parent for a DockWindowPlaceHolder");
                if(parent == null)
                {
                    continue;
                }

                int index = parent.TabPanels.IndexOf(holder);

                ToolWindow actualWindow = new ToolWindow(holder.DockWindowText);
                actualWindow.Name = holder.DockWindowName;

                holder.Dispose();

                parent.TabPanels.Insert(index, actualWindow);
            }
        }

        #endregion
    }
}
