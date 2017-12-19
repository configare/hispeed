using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Base class for a <see cref="TabStripPanel">TabStripPanel</see> instance that resides on a <see cref="RadDock">RadDock</see> scene.
    /// </summary>
    [ToolboxItem(false)]
    [Designer("Telerik.WinControls.UI.Design.DockTabStripDesigner, Telerik.WinControls.UI.Design, Version=" + VersionNumber.Number + ", Culture=neutral, PublicKeyToken=5bb2a467cbec794e")]
    public abstract class DockTabStrip : TabStripPanel
    {
        #region Fields

        private RadDock dockManager;
        private Point initialMousePosition;
        private TabStripItem draggedItem;
        private int redockReferenceCount;
        private bool allowTransparentBackColor;

        #endregion

        #region Constructors/Initializers

        /// <summary>
        /// Initializes a new <see cref="DockTabStrip">DockTabStrip</see> instance and associates it with the specified RadDock instance.
        /// </summary>
        /// <param name="dockManager"></param>
        public DockTabStrip(RadDock dockManager)
        {
            this.dockManager = dockManager;
            this.allowTransparentBackColor = true;
            this.tabStripElement.AllowDrag = true;
        }

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            if (element is RadTabStripElement)
            {
                return true;
            }

            return base.ControlDefinesThemeForElement(element);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determines whether this tabstrip is referenced for the RedockService as a target of Redock operation.
        /// This flag is used to determine whether a RadSplitContainer may be freely cleaned-up or should be reserved.
        /// </summary>
        internal virtual bool IsRedockTarget
        {
            get
            {
                return this.redockReferenceCount > 0;
            }
            set
            {
                if (value)
                {
                    this.redockReferenceCount++;
                }
                else
                {
                    this.redockReferenceCount--;
                    this.redockReferenceCount = Math.Max(0, this.redockReferenceCount);
                }
            }
        }

        protected internal virtual Color DefaultDockWindowBackColor
        {
            get
            {
                if (this.tabStripElement != null)
                {
                    return (Color)this.tabStripElement.ContentArea.GetValue(VisualElement.BackColorProperty);
                }

                return this.BackColor;
            }
        }

        /// <summary>
        /// Gets or sets the BackColor of the strip.
        /// Transparent BackColor is a special case, further controlled by the <see cref="DockTabStrip.AllowTransparentBackColor">AllowTransparentBackColor</see> property.
        /// </summary>
        [Description("Gets or sets the BackColor of the strip. Transparent BackColor is a special case, further controlled by the AllowTransparentBackColor property.")]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                if (value == Color.Transparent && !this.allowTransparentBackColor)
                {
                    return;
                }

                base.BackColor = value;
            }
        }

        /// <summary>
        /// Determines whether the control accepts Color.Transparent as its BackColor.
        /// </summary>
        [DefaultValue(true)]
        [Description("Determines whether the control accepts Color.Transparent as its BackColor.")]
        public bool AllowTransparentBackColor
        {
            get
            {
                return this.allowTransparentBackColor;
            }
            set
            {
                this.allowTransparentBackColor = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="DockType">DockType</see> member of this instance.
        /// </summary>
        [Browsable(false)]
        public abstract DockType DockType
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool Collapsed
        {
            get
            {
                return base.Collapsed;
            }
            set
            {
                base.Collapsed = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new TabPanelCollection TabPanels
        {
            get { return base.TabPanels; }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadDock DockManager
        {
            get
            {
                return this.dockManager;
            }
            internal set
            {

                if (this.dockManager != value)
                {
                    this.dockManager = value;
                    this.OnDockManagerChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the currently active <see cref="DockWindow">DockWindow</see> instance.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockWindow ActiveWindow
        {
            get
            {
                if (this.SelectedTab is DockWindow)
                {
                    return (DockWindow)this.SelectedTab;
                }

                return null;
            }
            set
            {
                if (value != null && value.InnerDockState == DockState.Hidden)
                {
                    return;
                }

                this.SelectedTab = value;
            }
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (!this.ContainsFocus)
            {
                base.Focus();
            }

            base.OnMouseDown(e);

            if (this.dockManager != null)
            {
                this.dockManager.ActiveWindow = this.ActiveWindow;
            }

            if (e.Button == MouseButtons.Left)
            {
                this.initialMousePosition = e.Location;
            }
        }        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.draggedItem == null ||
                this.tabStripElement.ItemContainer.ControlBoundingRectangle.Contains(e.Location))
            {
                return;
            }
            if (this.draggedItem == null)
            {
                return;
            }

            if (this.dockManager != null)
            {
                this.dockManager.BeginDrag(this.ActiveWindow);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            base.OnMouseCaptureChanged(e);

            this.draggedItem = null;
        }

        /// <summary>
        /// Overrides the method to provide support for instanciating a DragDropService operation.
        /// </summary>
        /// <param name="mouse"></param>
        protected override void OnDragInitialized(Point mouse)
        {
            if (this.DockManager == null)
            {
                return;
            }

            DockWindow activeWindow = this.ActiveWindow;
            if (activeWindow == null)
            {
                return;
            }

            draggedItem = this.elementTree.GetElementAtPoint(mouse) as TabStripItem;
            object dragged = null;

            if (draggedItem != null)
            {
                if (!this.tabStripElement.ItemContainer.ControlBoundingRectangle.Contains(mouse))
                {
                    dragged = activeWindow;
                }

                if (this.TabPanels.Count == 1 && this.ActiveWindow.DockState == DockState.Floating)
                {
                    dragged = null;
                }
            }
            else
            {
                if (activeWindow.DockState != DockState.AutoHide && this.IsDragAllowed(this.DragStart))
                {
                    dragged = this;
                }
            }

            if (dragged != null)
            {
                this.dockManager.BeginDrag(dragged);
            }
        }

        /// <summary>
        /// Allows inheritors to provide additional functionality upon owning RadDock instance change.
        /// </summary>
        protected virtual void OnDockManagerChanged()
        {
        }

        protected internal virtual void OnLocalizationProviderChanged()
        {
        }

        /// <summary>
        /// Closes the corresponding <see cref="DockWindow">DockWindow</see> instance.
        /// </summary>
        /// <param name="item"></param>
        protected internal override void OnTabCloseButtonClicked(TabStripItem item)
        {
            DockWindow window = item.TabPanel as DockWindow;
            if (window != null)
            {
                window.Close();
            }
        }

        #endregion

        #region Implementation

        #region Internal methods

        /// <summary>
        /// Provides routine which allows the strip to decide whether it should be collapsed or disposed.
        /// Internally used by the docking framework to defragment the dock tree.
        /// </summary>
        protected internal virtual void CheckCollapseOrDispose()
        {
            if (this.dockManager == null || this.dockManager.IsInTransactionBlock)
            {
                return;
            }

            bool collapse = this.GetCollapsed();
            if (!collapse)
            {
                this.Collapsed = false;
            }
            else
            {
                if (this.redockReferenceCount > 0)
                {
                    this.Collapsed = true;
                }
                else
                {
                    this.Dispose();
                }
            }
        }

        /// <summary>
        /// Updates the additional buttons, associated with the strip.
        /// E.g. a ToolTabStrip will have caption buttons, while a DocumentTabStrip will have strip buttons.
        /// </summary>
        protected internal virtual void UpdateButtons()
        {
        }

        /// <summary>
        /// Allows an affected strip to perform additional update after a transaction completion.
        /// </summary>
        protected internal virtual void UpdateAfterTransaction()
        {
            this.UpdateButtons();

            this.tabStripElement.InvalidateMeasure();
            this.tabStripElement.InvalidateArrange();
        }

        /// <summary>
        /// Allows a DockTabStrip to perform some additional operations upon activation of an owned DockWindow.
        /// For example a ToolTabStrip will update its Caption with Active or Inactive state correspondingly.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="active">True if the window is currently active, false otherwise.</param>
        protected internal virtual void UpdateActiveWindow(DockWindow window, bool active)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateFocus"></param>
        protected internal override void UpdateTabSelection(bool updateFocus)
        {
            base.UpdateTabSelection(updateFocus);

            DockWindow active = this.ActiveWindow;
            if (active == null)
            {
                return;
            }

            this.UpdateButtons();

            if (!updateFocus || this.dockManager == null)
            {
                return;
            }

            if (this.dockManager.IsLoaded && this.dockManager.ShouldProcessNotification())
            {
                this.dockManager.ActiveWindow = active;
            }
        }

        protected internal override void UpdateAfterControlRemoved(Control value)
        {
            base.UpdateAfterControlRemoved(value);

            DockWindow active = this.ActiveWindow;
            if (active != null)
            {
                this.UpdateActiveWindow(active, false);
            }
        }

        #endregion

        /// <summary>
        /// Copies the settings of the current strip to the target one. Currently the SizeInfo member is copied.
        /// </summary>
        /// <param name="clone"></param>
        internal virtual void CopyTo(DockTabStrip clone)
        {
            clone.SizeInfo.Copy(this.SizeInfo);
        }
       
        /// <summary>
        /// Determines whether the strip should be collapsed. E.g. it may not have child panels but should not be disposed as it may be a redock target.
        /// </summary>
        /// <returns></returns>
        protected virtual bool GetCollapsed()
        {
            return this.TabPanels.Count == 0;
        }

        /// <summary>
        /// Determines whether a drag operation is currently allowed.
        /// </summary>
        /// <param name="location">The location to examine, in client coordinates.</param>
        /// <returns></returns>
        protected virtual bool IsDragAllowed(Point location)
        {
            return false;
        }

        #endregion

        
    }
}
