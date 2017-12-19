using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{

    /// <summary>
    /// Represents a drop down menu used in radComboBox and radMenu.
    /// </summary>
    [ToolboxItem(false)]
    [RadToolboxItem(false)]
    [DefaultProperty("Items")]

    public class RadDropDownMenu : RadItemsPopupControl, ITooltipOwner
    {
        #region Nested

        internal delegate void PerformClickInvoker(RadMenuItemBase item);

        #endregion
        //private RadItemOwnerCollection items;
        private RadElement ownerElement;
        private RadElement popupElement;
        private RadItem clickedItem;

        private Timer childDropDownTimeout;
        private bool showing;

        static RadDropDownMenu()
        {
            new Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_RadDropDownMenu().DeserializeTheme();

        }

        /// <summary>
        /// Initializes a new instane of the RadDropDownMenu class
        /// </summary>
        public RadDropDownMenu()
            : this(null)
        {
        }

        /// <summary>
        /// Creates an instance of the RadDropDownMenu class.
        /// </summary>
        /// <param name="ownerElement">An instance of the RadElement class 
        /// that represents the owner of this drop down menu</param>
        public RadDropDownMenu(RadElement ownerElement)
            : this(ownerElement, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RadDropDownMenu class using menu owner and
        /// the parent panel.
        /// </summary>
        /// <param name="ownerElement">
        /// Initializes the owner element.
        /// </param>
        /// <param name="parentPanel">
        /// Initializes the parent panel.
        /// </param>
        //[Obsolete("Do not use this constructor. It will be removed in Q1 2010.", false)]
        public RadDropDownMenu(RadElement ownerElement, RadDropDownMenu parentPanel)
            : base(ownerElement)
        {
            this.UseNewLayoutSystem = true;
            this.AutoSize = true;
            this.Items.ItemsChanged += new ItemChangedDelegate(OnItemsChanged);
            this.Items.ItemTypes = new Type[] { typeof(RadMenuItemBase) };
            this.ownerElement = ownerElement;
            this.CausesValidation = false;
            this.Visible = false;
            this.InitializeChildren();
            this.HorizontalAlignmentCorrectionMode = AlignmentCorrectionMode.SnapToOuterEdges;
            this.VerticalAlignmentCorrectionMode = AlignmentCorrectionMode.SnapToOuterEdges;
            this.FadeAnimationType = FadeAnimationType.FadeIn;
            this.DropShadow = true;
            this.childDropDownTimeout = new Timer();
            this.childDropDownTimeout.Interval = SystemInformation.MenuShowDelay == 0 ? 40 : SystemInformation.MenuShowDelay;
            this.childDropDownTimeout.Tick += new EventHandler(OnChildrenDropDown_TimeOut);
        }

        private void OnChildrenDropDown_TimeOut(object sender, EventArgs e)
        {
            RadMenuItemBase selectedItem = this.GetSelectedItem() as RadMenuItemBase;

            if (selectedItem != null)
            {
                selectedItem.ShowChildItems();
            }

            this.childDropDownTimeout.Stop();
        }

        protected virtual void InitializeChildren()
        {
            RadElement root = this.RootElement;

            root.StretchVertically = false;
            root.StretchHorizontally = false;

            if (this.popupElement == null)
            {
                this.popupElement = CreatePopupElement();
                root.Children.Add(this.popupElement);
            }
        }

        protected override bool ProcessDialogChar(char charCode)
        {
            return true;
        }

        protected override void OnLoad(Size desiredSize)
        {
            if (this.ownerElement != null && this.ownerElement.IsInValidState(true))
            {
                this.ImageList = this.ownerElement.ElementTree.ComponentTreeHandler.ImageList;
                this.BindingContext = this.ownerElement.ElementTree.Control.BindingContext;
            }

            base.OnLoad(desiredSize);
        }

        #region Properties

        /// <summary>
        /// Gets the item that has been clicked. This property is valid when the drop-down is closed by an item click.
        /// </summary>
        public RadItem ClickedItem
        {
            get
            {
                return this.clickedItem;
            }
        }

        public IComponentTreeHandler RootTreeHandler
        {
            get
            {
                bool finished = false;
                IPopupControl parent = this.OwnerPopup;
                RadElement ownerElement = this.OwnerElement;
                while (!finished)
                {
                    if (parent != null)
                    {
                        ownerElement = parent.OwnerElement;
                        parent = parent.OwnerPopup;
                    }
                    else
                    {
                        finished = true;

                        if (ownerElement != null && ownerElement.IsInValidState(true))
                        {
                            return ownerElement.ElementTree.ComponentTreeHandler;
                        }
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets the popup element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadElement PopupElement
        {
            get
            {
                return this.popupElement;
            }
            set
            {
                if (this.popupElement != value && value != null)
                {
                    if (this.popupElement != null &&
                        !(this.popupElement.IsDisposing || this.popupElement.IsDisposed))
                    {
                        this.RootElement.Children.Remove(this.popupElement);
                        this.popupElement.Disposed -= this.OnPopupElement_Disposed;
                    }

                    popupElement = value;

                    if (!(this.popupElement.IsDisposing || this.popupElement.IsDisposed))
                    {
                        this.popupElement.Disposed += this.OnPopupElement_Disposed;
                    }

                    this.Items.Owner = popupElement;
                    this.RootElement.Children.Add(this.popupElement);
                }
            }
        }

        private void OnPopupElement_Disposed(object sender, EventArgs e)
        {
            this.popupElement = null;
        }

        /// <summary>
        ///	Indicates whether the DropDown contains one or two cloumns of items.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool IsTwoColumnMenu
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        [DefaultValue(false), Browsable(false)]
        public new bool CausesValidation
        {
            get
            {
                return base.CausesValidation;
            }
            set
            {
                base.CausesValidation = value;
            }
        }

        internal override int ShowParams
        {
            get
            {
                return 4;
            }
        }

        /// <summary>
        /// Gets or sets menu header column text
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue("")]
        public string HeaderText
        {
            get
            {
                RadDropDownMenuElement element = popupElement as RadDropDownMenuElement;
                if (element != null)
                {
                    return element.HeaderText;
                }
                return null;
            }
            set
            {
                RadDropDownMenuElement element = popupElement as RadDropDownMenuElement;
                if (element != null)
                {
                    element.HeaderText = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets menu header column image
        /// </summary>
        [DefaultValue(null)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image HeaderImage
        {
            get
            {
                RadDropDownMenuElement element = popupElement as RadDropDownMenuElement;
                if (element != null)
                {
                    return element.HeaderImage;
                }
                return null;
            }
            set
            {
                RadDropDownMenuElement element = popupElement as RadDropDownMenuElement;
                if (element != null)
                {
                    element.HeaderImage = value;
                }
            }
        }

        #endregion

        #region Public functions

        /// <summary>
        /// Displays the RadDropDownMenu in its default position.
        /// </summary>
        public new void Show()
        {
            if (showing)
                base.Show();
            else
                ShowCore(this.Location, 0, RadDirection.Down);
        }

        /// <summary>
        /// Displays the RadDropDownMenu relative to the specified screen location.
        /// </summary>
        /// <param name="x">The horizontal screen coordinate, in pixels.</param>
        /// <param name="y">The vertical screen coordinate, in pixels.</param>
        public void Show(int x, int y)
        {
            ShowCore(new Point(x, y), 0, RadDirection.Right);
        }

        /// <summary>
        /// Displays the RadDropDownMenu relative to the specified screen location.
        /// </summary>
        /// <param name="point">The horizontal and vertical location of the screen's upper-left corner, in pixels.</param>
        public new void Show(Point point)
        {
            ShowCore(point, 0, RadDirection.Right);
        }

        /// <summary>
        /// Positions the ToolStripDropDown relative to the specified screen location and with the specified direction.
        /// </summary>
        /// <param name="point">The horizontal and vertical location of the screen's upper-left corner, in pixels.</param>
        /// <param name="popupDirection">One of the RadDirection values.</param>
        public void Show(Point point, RadDirection popupDirection)
        {
            ShowCore(point, 0, popupDirection);
        }

        /// <summary>
        /// Positions the RadDropDownMenu relative to the specified control location.
        /// </summary>
        /// <param name="control">The control that is the reference point for the RadDropDownMenu position.</param>
        /// <param name="x">The horizontal coordinate relative to the control, in pixels.</param>
        /// <param name="y">The vertical coordinate relative to the control, in pixels.</param>
        public void Show(Control control, int x, int y)
        {
            ShowCore(control.PointToScreen(new Point(x, y)), 0, RadDirection.Right);
        }

        /// <summary>
        /// Positions the RadDropDownMenu relative to the specified control location.
        /// </summary>
        /// <param name="control">The control that is the reference point for the RadDropDownMenu position.</param>
        /// <param name="point">The horizontal and vertical location of the reference control's upper-left corner, in pixels.</param>
        public void Show(Control control, Point point)
        {
            ShowCore(control.PointToScreen(point), 0, RadDirection.Right);
        }

        /// <summary>
        /// Positions the RadDropDownMenu relative to the specified control location and with the specified direction.
        /// </summary>
        /// <param name="control">The control that is the reference point for the RadDropDownMenu position.</param>
        /// <param name="point">The horizontal and vertical location of the reference control's upper-left corner, in pixels.</param>
        /// <param name="popupDirection">One of the RadDirection values.</param>
        public void Show(Control control, Point point, RadDirection popupDirection)
        {
            ShowCore(control.PointToScreen(point), 0, popupDirection);
        }

        /// <summary>
        /// Positions the RadDropDownMenu relative to the specified RadItem location.
        /// </summary>
        /// <param name="item">The RadItem that is the reference point for the RadDropDownMenu position.</param>
        /// <param name="x">The horizontal coordinate relative to the control, in pixels.</param>
        /// <param name="y">The vertical coordinate relative to the control, in pixels.</param>
        public void Show(RadItem item, int x, int y)
        {
            ShowCore(item.PointToScreen(new Point(x, y)), 0, RadDirection.Right);
        }

        /// <summary>
        /// Positions the RadDropDownMenu relative to the specified RadItem location.
        /// </summary>
        /// <param name="item">The RadItem that is the reference point for the RadDropDownMenu position.</param>
        /// <param name="point">The horizontal and vertical location of the RadItem's upper-left corner, in pixels.</param>
        public void Show(RadItem item, Point point)
        {
            ShowCore(item.PointToScreen(point), 0, RadDirection.Right);
        }

        /// <summary>
        /// Positions the RadDropDownMenu relative to the specified RadItem location and with the specified direction.
        /// </summary>
        /// <param name="item">The RadItem that is the reference point for the RadDropDownMenu position.</param>
        /// <param name="point">The horizontal and vertical location of the RadItem's upper-left corner, in pixels.</param>
        /// <param name="popupDirection">One of the RadDirection values.</param>
        public void Show(RadItem item, Point point, RadDirection popupDirection)
        {
            ShowCore(item.PointToScreen(point), 0, popupDirection);
        }

        /// <summary>
        /// Positions the RadDropDownMenu relative to the specified RadItem location and 
        /// with specified direction and offset according to the owner.
        /// </summary>
        /// <param name="item">The RadItem that is the reference point for the RadDropDownMenu position.</param>
        /// <param name="ownerOffset">Specifies the offset from the owner in pixels.</param>
        /// <param name="popupDirection">One of the RadDirection values.</param>
        public void Show(RadItem item, int ownerOffset, RadDirection popupDirection)
        {
            ShowCore(Point.Empty, ownerOffset, popupDirection);
        }

        #endregion

        #region Theme support

        public override string ThemeClassName
        {
            get
            {
                //resolution of class name for the RadDropDowneMenu control:
                //1. Check if theme class name inherits from owner
                //2. Check if set locally
                //3. Check if owner control can provide theme
                //4. Return the RadDropDowMenu class name...

                if (this.ownerElement != null && this.ownerElement.IsInValidState(true))
                {
                    IComponentTreeHandler ownerControl = this.OwnerElement.ElementTree.ComponentTreeHandler;
                    if (ownerControl != null)
                    {
                        if (this.OwnerElement is IDropDownMenuOwner)
                        {
                            if ((this.OwnerElement as IDropDownMenuOwner).DropDownInheritsThemeClassName)
                            {
                                return ownerControl.ThemeClassName;
                            }
                        }
                        else if (ownerControl is RadDropDownMenu)
                        {
                            return (ownerControl as IComponentTreeHandler).ThemeClassName;
                        }
                    }
                }

                return base.ThemeClassName;
            }
            set
            {
                base.ThemeClassName = value;
            }
        }

        #endregion

        protected virtual void ShowCore(Point point, int ownerOffset, RadDirection popupDirection)
        {

            if (this.Visible || this.IsDisposed)
            {
                return;
            }

            showing = true;
            bool loaded = this.IsLoaded;
            this.SetTheme();

            if (!loaded)
            {
                this.LoadElementTree();
                loaded = this.IsLoaded;
            }

            SetTheme();
            Point screenLocation = Point.Empty;

            if (this.ownerElement != null && this.ownerElement.IsDesignMode)
            {
                IntPtr activeHwnd = NativeMethods.GetActiveWindow();
                NativeMethods.POINT pt = new NativeMethods.POINT(0, 0);
                NativeMethods.ClientToScreen(new HandleRef(this, activeHwnd), pt);
                point.X -= pt.x;
                point.Y -= pt.y;

                if (ownerElement.IsInValidState(true))
                {
                    screenLocation = ownerElement.ElementTree.Control.PointToScreen(ownerElement.ControlBoundingRectangle.Location);
                }
                else
                {
                    screenLocation = this.OnDropDownLocationNeeded(point);
                }
            }
            else
            {
                if (this.ownerElement != null)
                {
                    if (ownerElement.IsInValidState(true))
                    {
                        screenLocation = ownerElement.ElementTree.Control.PointToScreen(ownerElement.ControlBoundingRectangle.Location);
                    }
                    else
                    {
                        screenLocation = this.OnDropDownLocationNeeded(point);
                    }
                }
                else
                {
                    screenLocation = this.OnDropDownLocationNeeded(point);
                }
            }

            screenLocation.Offset(ownerOffset, ownerOffset);

            Size alignmentRectSize = this.ownerElement != null ? this.ownerElement.ControlBoundingRectangle.Size : Size.Empty;

            Rectangle alignmentRectangle = new Rectangle(screenLocation, alignmentRectSize);

            showing = false;
            // FIX: the drop down animation does not work correctly when the popupDirection is set to left or up.
            //this.DropDownAnimationDirection = popupDirection;
            base.ShowPopup(alignmentRectangle);

            if (loaded)
            {
                this.RootElement.InvalidateMeasure(true);
                this.RootElement.UpdateLayout();

                this.Size = this.RootElement.DesiredSize.ToSize();
            }
        }

        protected virtual Point OnDropDownLocationNeeded(Point point)
        {
            return point;
        }

        protected virtual bool CanProcessItem(RadMenuItemBase menuItem)
        {
            return menuItem != null && menuItem.Enabled;
        }

        protected virtual void OnItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            RadMenuItemBase menuItem = target as RadMenuItemBase;

            if (menuItem != null)
            {
                if (operation == ItemsChangeOperation.Inserted || operation == ItemsChangeOperation.Set)
                {
                    menuItem.HierarchyParent = this.ownerElement as IHierarchicalItem;

                    menuItem.Owner = this.ownerElement;
                }

                else if (operation == ItemsChangeOperation.Removed)
                {
                    menuItem.HierarchyParent = null;
                }
            }

            if (operation == ItemsChangeOperation.Inserted)
            {
                if (menuItem is RadMenuItem && menuItem.Site == null)
                {
                    (menuItem as RadMenuItem).ShowKeyboardCue = false;
                }
            }
        }

        public override bool OnMouseWheel(Control target, int delta)
        {
            RadDropDownMenuElement element = this.popupElement as RadDropDownMenuElement;

            if (element == null)
                return false;

            if (element.ScrollPanel.VerticalScrollBar.Visibility != ElementVisibility.Visible)
            {
                return false;
            }

            if (delta > 0)
            {
                element.ScrollPanel.LineUp();
            }
            else
            {
                element.ScrollPanel.LineDown();
            }

            return true;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            RadMenuItemBase menuItem = this.elementTree.GetElementAtPoint(e.Location) as RadMenuItemBase;
            if (!this.CanProcessItem(menuItem))
            {
                return;
            }

            if (menuItem.ShouldHandleMouseInput
                && menuItem.DropDown != null
                && !menuItem.DropDown.IsVisible)
            {
                this.childDropDownTimeout.Stop();
                menuItem.ShowChildItems();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            RadMenuItemBase menuItem = this.elementTree.GetElementAtPoint(e.Location) as RadMenuItemBase;
            RadMenuItemBase currentSelectedItem = this.GetSelectedItem() as RadMenuItemBase;

            if (!this.CanProcessItem(menuItem))
            {
                base.OnMouseMove(e);
                return;
            }

            if (currentSelectedItem != null &&
                !object.ReferenceEquals(currentSelectedItem, menuItem))
            {
                currentSelectedItem.Deselect();
                currentSelectedItem.HideChildItems();
            }

            this.childDropDownTimeout.Start();

            base.OnMouseMove(e);
        }

        Stopwatch dblClickStopWatch = null;



        private RadElement GetRootOwnerElement()
        {
            IPopupControl currentOwner = this.OwnerPopup;

            if (currentOwner != null)
            {
                while (currentOwner != null)
                {
                    currentOwner = currentOwner.OwnerPopup;

                    if (currentOwner == null)
                    {
                        return this.OwnerElement;
                    }
                }
            }
            else
            {
                return this.OwnerElement;
            }

            return null;
        }
        private RadMenuItemBase elementUnderMouse = null;

        private void DoDesignTimeDoubleClick(MouseEventArgs e)
        {
            RadMenuItemBase oldElementUnderMouse = null;

            oldElementUnderMouse = elementUnderMouse;
            elementUnderMouse = this.ElementTree.GetElementAtPoint(e.Location) as RadMenuItemBase;

            if (dblClickStopWatch == null)
            {
                dblClickStopWatch = Stopwatch.StartNew();
                return;
            }

            if (elementUnderMouse != oldElementUnderMouse)
            {
                if (dblClickStopWatch != null)
                {
                    dblClickStopWatch.Reset();
                    dblClickStopWatch.Start();
                }
                return;
            }

            if (dblClickStopWatch.ElapsedMilliseconds < SystemInformation.DoubleClickTime)
            {
                if (elementUnderMouse == null)
                {
                    return;
                }
                ISite site = elementUnderMouse.GetSite();

                if (site == null)
                {
                    return;
                }

                IDesignerHost host = (IDesignerHost)site.GetService(typeof(IDesignerHost));
                if (host == null)
                {
                    return;

                }

                this.ClosePopup(RadPopupCloseReason.Mouse);

                if (this.OwnerPopup != null)
                {
                    PopupManager.Default.CloseAllToRoot(RadPopupCloseReason.Mouse, this.OwnerPopup);
                }

                dblClickStopWatch.Reset();
                elementUnderMouse = null;
            }
            else
            {
                dblClickStopWatch.Reset();
                elementUnderMouse = null;
            }
        }

        private void DoOnItemClicked(RadMenuItemBase menuItem, MouseEventArgs e)
        {
            if (!menuItem.HasChildren && menuItem.GetSite() == null)
            {
                menuItem.Select();
                PopupManager.Default.CloseAllToRoot(RadPopupCloseReason.Mouse, this);

                if (menuItem.RootItem == null
                    || !(menuItem.RootItem.Owner is RadMenuElement))
                {
                    return;
                }

                if (menuItem.Owner is RadMenuElement)
                {
                    if ((menuItem.Owner as RadMenuElement).ElementTree.Control is RadMenu)
                    {
                        ((menuItem.Owner as RadMenuElement).ElementTree.Control as RadMenu).SetMenuState(RadMenu.RadMenuState.NotActive);
                    }
                }
            }
            else
            {
                if (!(menuItem.GetSite() != null && e.Button == MouseButtons.Right))
                {
                    menuItem.ShowChildItems();
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            RadMenuItemBase menuItem = this.ElementTree.GetElementAtPoint(e.Location) as RadMenuItemBase;

            base.OnMouseClick(e);

            RadElement ownerElement = this.GetRootOwnerElement();
            if (ownerElement != null && e.Button == MouseButtons.Left
                && ownerElement.IsDesignMode)
            {
                this.DoDesignTimeDoubleClick(e);
            }
            else
            {
                if (menuItem == null || !menuItem.Enabled)
                {
                    return;
                }

                this.clickedItem = menuItem;
                this.DoOnItemClicked(menuItem, e);

                if (menuItem.GetSite() == null)
                {
                    if (!menuItem.IsMouseOver)
                    {
                        menuItem.PerformClick();
                    }
                }

                this.clickedItem = null;
            }
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(value);

            if (value == true)
            {
                if (this.ownerElement != null && this.ownerElement.ElementTree != null)
                {
                    Control ownerControl = this.ownerElement.ElementTree.Control;

                    if (ownerControl != null)
                    {
                        ownerControl.Capture = false;
                    }
                }

                if (this.popupElement is RadDropDownMenuElement)
                {
                    this.popupElement.SetValue(RadDropDownMenuElement.DropDownPositionProperty, DropDownPosition.Popup);
                }
            }
            else
            {
                RadMenuItemBase currentItem = this.GetSelectedItem() as RadMenuItemBase;
                if (currentItem != null)
                {
                    currentItem.Deselect();
                }
            }

        }

        protected override void OnItemSelected(ItemSelectedEventArgs args)
        {
            base.OnItemSelected(args);

            if (args.Item is RadMenuItemBase)
            {
                RadMenuItemBase item = args.Item as RadMenuItemBase;
                item.Selected = true;
            }
        }

        protected override void OnItemDeselected(ItemSelectedEventArgs args)
        {
            base.OnItemDeselected(args);

            if (args.Item is RadMenuItemBase)
            {
                RadMenuItemBase item = args.Item as RadMenuItemBase;
                item.Selected = false;
                item.HideChildItems();
            }
        }

        protected override void OnDropDownClosed(RadPopupClosedEventArgs args)
        {
            base.OnDropDownClosed(args);

            RadMenuItemBase currentItem = this.GetSelectedItem() as RadMenuItemBase;
            if (currentItem != null)
            {
                currentItem.Deselect();
            }

            if (!(this.OwnerElement is RadMenuItemBase))
            {
                return;
            }

            RadMenuItemBase ownerMenuItem = this.OwnerElement as RadMenuItemBase;

            if (!(this.lastPressedKey == Keys.Escape)
                && !ownerMenuItem.IsOnDropDown
                && ownerMenuItem.ElementTree.Control is IItemsControl)
            {
                IItemsControl ownerItemsControl = ownerMenuItem.ElementTree.Control as IItemsControl;
                RadMenuItemBase menuItemBase = ownerItemsControl.GetSelectedItem() as RadMenuItemBase;

                if (!object.ReferenceEquals(menuItemBase, ownerMenuItem))
                {
                    return;
                }

                ownerMenuItem.Selected = false;
            }
        }

        public override bool CanClosePopup(RadPopupCloseReason reason)
        {
            if (reason == RadPopupCloseReason.Mouse)
            {
                if (this.OwnerPopup == null && this.OwnerElement != null && this.RootTreeHandler != null)
                {
                    RadMenuItemBase menuItem = this.OwnerElement as RadMenuItemBase;
                    Control rootControl = this.RootTreeHandler as Control;

                    Point currentMousePoint = rootControl.PointToClient(MousePosition);

                    RadElement itemUnderMouse = this.RootTreeHandler.ElementTree.GetElementAtPoint(currentMousePoint);

                    if ((menuItem != null && itemUnderMouse != null &&
                        object.ReferenceEquals(itemUnderMouse, menuItem)
                        && rootControl.Focused) || (menuItem != null && menuItem.IsMouseDown))
                    {
                        return false;
                    }
                }

                if (this.OwnerElement != null)
                {
                    this.OwnerElement.UpdateContainsMouse();
                    if(this.OwnerElement.ContainsMouse || this.OwnerElement.IsMouseDown)
                    {
                        return false;
                    }
                }
            }

            return base.CanClosePopup(reason);
        }

        protected void PerformItemClick(RadMenuItemBase menuItem)
        {
            if (menuItem == null || !menuItem.Enabled)
                return;

            this.clickedItem = menuItem;

            if (menuItem.HasChildren)
            {
                menuItem.ShowChildItems();
                menuItem.DropDown.SelectFirstVisibleItem();
            }
            else
            {
                PopupManager.Default.CloseAllToRoot(RadPopupCloseReason.Keyboard, this);
                if (menuItem.Owner is RadMenuElement)
                {
                    if (((menuItem.Owner as RadMenuElement).ElementTree.Control is RadMenu))
                    {
                        ((menuItem.Owner as RadMenuElement).ElementTree.Control as RadMenu).SetMenuState(RadMenu.RadMenuState.NotActive);
                    }
                }
            }


            menuItem.PerformClick();
            this.clickedItem = null;
        }

        public override bool CanNavigate(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Down:
                case Keys.Up:
                case Keys.Right:
                case Keys.Left:
                    {
                        return this.CheckCanNavigate(keyData);
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        public override bool CanProcessMnemonic(char keyData)
        {
            return this.CheckCanProcessMnemonic(this, keyData);
        }

        protected virtual bool CheckCanProcessMnemonic(IItemsControl itemsControl, char keyData)
        {
            bool result = false;

            foreach (RadItem item in itemsControl.Items)
            {
                RadMenuItemBase menuItem = item as RadMenuItemBase;

                if (menuItem != null && menuItem.IsPopupShown)
                {
                    return this.CheckCanProcessMnemonic(menuItem.DropDown, keyData);
                }

                if (IsMnemonic(keyData, item.Text))
                {
                    if (menuItem != null && menuItem.HasChildren)
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
            }

            return result;
        }



        protected virtual bool CheckCanNavigate(Keys keyData)
        {
            RadMenuItemBase selectedItem = this.GetSelectedItem() as RadMenuItemBase;

            if (selectedItem != null)
            {
                if (keyData == Keys.Right)
                {
                    if (selectedItem.Enabled && selectedItem.HasChildren)
                    {
                        return true;
                    }

                    return false;
                }
                else if (keyData == Keys.Left)
                {
                    if (selectedItem.HierarchyParent != null && !selectedItem.HierarchyParent.IsRootItem)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        private Keys lastPressedKey;

        public override bool OnKeyDown(Keys keyData)
        {
            this.lastPressedKey = keyData;
            switch (keyData)
            {
                case Keys.Up:
                case Keys.Down:
                    {
                        return this.ProcessUpDownNavigationKey(keyData == Keys.Up);
                    }
                case Keys.Left:
                case Keys.Right:
                    {

                        return this.ProcessLeftRightNavigationKey(keyData == Keys.Left);
                    }
                case Keys.Enter:
                    {
                        RadMenuItemBase menuItem = this.GetSelectedItem() as RadMenuItemBase;

                        if (menuItem != null)
                        {
                            this.BeginInvoke(new PerformClickInvoker(this.PerformItemClick), menuItem);
                            return true;
                        }

                        return false;
                    }
                case Keys.Back:
                    {
                        return false;
                    }
                default:
                    {
                        if (this.ProcessMnemonic(keyData))
                        {
                            return true;
                        }

                        return base.OnKeyDown(keyData);
                    }
            }
        }

        protected virtual bool ProcessMnemonic(System.Windows.Forms.Keys keyData)
        {
            uint keyCode = NativeMethods.MapVirtualKey((uint)keyData, 2);

            if (keyCode < Char.MinValue || keyCode > Char.MaxValue)
            {
                return false;
            }

            char charKey = Convert.ToChar(keyCode);

            foreach (RadItem menuItem in this.Items)
            {
                if (IsMnemonic(charKey, menuItem.Text))
                {
                    menuItem.Select();
                    this.BeginInvoke(new PerformClickInvoker(this.PerformItemClick), menuItem);
                    return true;
                }
            }

            return false;
        }

        protected virtual bool ProcessLeftRightNavigationKey(bool isLeft)
        {
            RadMenuItemBase selectedItem = this.GetSelectedItem() as RadMenuItemBase;

            if (selectedItem != null)
            {
                if (!isLeft)
                {
                    if (selectedItem.HasChildren)
                    {
                        selectedItem.ShowChildItems();
                        selectedItem.DropDown.SelectFirstVisibleItem();
                        return true;
                    }

                    return false;
                }
                else
                {
                    if (selectedItem.HierarchyParent != null && (!selectedItem.HierarchyParent.IsRootItem || this.OwnerPopup != null))
                    {
                        this.ClosePopup(RadPopupCloseReason.Keyboard);
                        return true;
                    }

                    return false;
                }
            }

            return false;
        }

        protected virtual bool ProcessUpDownNavigationKey(bool isUp)
        {
            RadMenuItemBase selectedItem = this.GetSelectedItem() as RadMenuItemBase;

            if (selectedItem != null)
            {
                this.SelectNextItem(selectedItem, !isUp);
            }
            else
            {
                if (!isUp)
                {
                    this.SelectFirstVisibleItem();
                }
                else
                {
                    this.SelectLastVisibleItem();
                }
            }

            return true;
        }

        protected virtual RadElement CreatePopupElement()
        {
            RadDropDownMenuElement menuElement = new RadDropDownMenuElement();
            this.Items.Owner = menuElement.LayoutPanel;
            return menuElement;
        }

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            if (this.OwnerElement != null && this.ownerElement.ElementTree != null)
            {
                RadControl ownerControl = this.OwnerElement.ElementTree.Control as RadControl;
                if (ownerControl != null)
                {
                    if (this.OwnerElement is IDropDownMenuOwner)
                    {
                        if (((IDropDownMenuOwner)this.OwnerElement).DropDownInheritsThemeClassName)
                        {
                            return ownerControl.ElementTree.ControlDefinesThemeForElement(element);
                        }
                        else
                        {
                            return ((IDropDownMenuOwner)this.OwnerElement).ControlDefinesThemeForElement(element);
                        }
                    }
                }
            }

            Type elementType = element.GetType();

            if (elementType.Equals(typeof(RadTextBoxElement)))
            {
                if (element.FindAncestorByThemeEffectiveType(typeof(RadComboBoxElement)) != null)
                {
                    return true;
                }
            }
            else if (elementType.Equals(typeof(RadMaskedEditBoxElement)))
            {
                if (element.FindAncestor<RadDateTimePickerElement>() != null)
                {
                    return true;
                }
            }

            return base.ControlDefinesThemeForElement(element);
        }

        internal void SetTheme()
        {
            bool themeNameSet = false;
            if (this.ownerElement != null && this.ownerElement.ElementTree != null)
            {
                Control ownerControl = this.ownerElement.ElementTree.Control;

                IComponentTreeHandler handlerControl = this.ownerElement.ElementTree.ComponentTreeHandler;

                if (ownerControl != null && !string.IsNullOrEmpty(handlerControl.ThemeName))
                {
                    if (this.ThemeName != handlerControl.ThemeName)
                    {
                        this.ThemeName = handlerControl.ThemeName;
                    }
                    themeNameSet = true;
                }
            }

            if (!themeNameSet && string.IsNullOrEmpty(this.ThemeName))
            {
                this.ThemeName = "ControlDefault";
            }
        }

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

                this.childDropDownTimeout.Dispose();
                this.ownerElement = null;
                this.Items.ItemsChanged -= new ItemChangedDelegate(OnItemsChanged);
                this.ImageList = null;
                this.BindingContext = null;
            }

            base.Dispose(disposing);
        }

        #endregion

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new RadDropDownMenuAccessibleObject(this);
        }

        #region ITooltipOwner
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public object Owner
        {
            get
            {
                return this.GetRootOwnerElement();
            }
            set
            {
                
            }
        }
        #endregion
    }
}


