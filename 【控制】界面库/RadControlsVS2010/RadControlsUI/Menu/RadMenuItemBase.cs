using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Design;


namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a base class for the RadMenuItem class.
    /// </summary>
    //[RadToolboxItem(false)]
    public abstract class RadMenuItemBase : RadButtonItem, IHierarchicalItem, ISiteProvider, ITooltipOwner
    {
        #region BitState Keys
        internal const ulong IsMdiListItemStateKey = CaptureOnMouseDownStateKey << 1;
        internal const ulong IsParticipatingInMergeStateKey = IsMdiListItemStateKey << 1;
        internal const ulong MdiListStateKey = IsParticipatingInMergeStateKey << 1;
        internal const ulong SelectedByMouseStateKey = MdiListStateKey << 1;
        internal const ulong IsMainMenuItemStateKey = SelectedByMouseStateKey << 1;
        internal const ulong HandleClickStateKey = IsMainMenuItemStateKey << 1;
        internal const ulong PreparedForDesignTimeStateKey = HandleClickStateKey << 1;
        internal const ulong HandlesKeyboardStateKey = PreparedForDesignTimeStateKey << 1;

        #endregion

        private MenuMerge mergeType = MenuMerge.Add;
        private int mergeOrder = 0;
        private int positionToBeRestoredAfterMerge = -1;

        #region OldMenu

        internal virtual int OffsetWidth
        {
            get
            {
                return 0;
            }
        }

        internal virtual RadElement ElementToOffset
        {
            get
            {
                return null;
            }
        }

        internal virtual RadElement ElementToAlignRight
        {
            get
            {
                return null;
            }
        }

        internal virtual int AlignRightOffsetWidth
        {
            get
            {
                return 0;
            }
        }

        internal virtual void SetElementOffsets(Padding padding)
        {
        }

        #endregion

        // Fields
        private object owner;
        private IHierarchicalItem hierarchyParent;
        private RadDropDownMenu dropDown;
        private Control ownerControl;
        private RadMenuItemAccessibleObject accessibleObject;

        protected MouseButtons pressedButton = MouseButtons.None;

        /// <summary>
        /// Initializes a new instance of the RadMenuItemBase class.
        /// </summary>
        public RadMenuItemBase()
        {
        }

        static RadMenuItemBase()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RadMenuItemBaseStateManagerFactory(), typeof(RadMenuItemBase));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.BitState[CaptureOnMouseDownStateKey] = false;
            this.BitState[HandleClickStateKey] = true;
            this.Class = "RadMenuItem";
            this.ClickMode = ClickMode.Release;
        }

        protected internal override bool ProcessMnemonic(char charCode)
        {
            if (Control.IsMnemonic(charCode, this.Text))
            {
                this.Select();
            }
            return base.ProcessMnemonic(charCode);
        }

        protected override void DisposeManagedResources()
        {
            if (this.dropDown != null)
            {
                //fix for 371350
                this.UnWireDropDownEvents();
                this.dropDown.Dispose();
            }

            if (this.accessibleObject != null)
            {
                this.accessibleObject.Dispose();
                this.accessibleObject = null;
            }

            base.DisposeManagedResources();
        }

        /// <summary>
        ///		Calls the ShowPopup method and displays the child items in a popup window.
        /// </summary>
        public virtual void ShowChildItems()
        {
            Debug.Assert(this.ElementState == ElementState.Loaded, "Displaying child items without being loaded.");
            if (!this.IsInValidState(true))
            {
                return;
            }

            EnsureDropDownCreated();

            if ((this.Visibility != ElementVisibility.Visible) || (!this.HasChildren && GetSite() == null))
            {
                return;
            }

            if (this.IsPopupShown)
            {
                return;
            }

            this.AdjustDropDownAnimations();

            if (this.GetValueSource(RadMenuItemBase.PopupDirectionProperty) == ValueSource.DefaultValue
                || this.GetValueSource(RadMenuItemBase.PopupDirectionProperty) == ValueSource.DefaultValueOverride)
            {
                this.AdjustDropDownAlignmentForOrientation();
            }
            else
            {
                this.AdjustDropDownAlignmentForPopupDirection();
            }

            this.dropDown.Show(this, 0, this.PopupDirection);
        }

        protected virtual void AdjustDropDownAlignmentForPopupDirection()
        {
            if (this.GetSite() != null)
            {
                return;
            }

            switch (this.PopupDirection)
            {
                case RadDirection.Down:
                    {
                        this.dropDown.HorizontalPopupAlignment = HorizontalPopupAlignment.LeftToLeft;
                        this.dropDown.VerticalPopupAlignment = VerticalPopupAlignment.TopToBottom;
                        break;
                    }
                case RadDirection.Left:
                    {
                        this.dropDown.HorizontalPopupAlignment = HorizontalPopupAlignment.RightToLeft;
                        this.dropDown.VerticalPopupAlignment = VerticalPopupAlignment.TopToTop;
                        break;
                    }
                case RadDirection.Right:
                    {
                        this.dropDown.HorizontalPopupAlignment = HorizontalPopupAlignment.LeftToRight;
                        this.dropDown.VerticalPopupAlignment = VerticalPopupAlignment.TopToTop;
                        break;
                    }
                case RadDirection.Up:
                    {
                        this.dropDown.HorizontalPopupAlignment = HorizontalPopupAlignment.LeftToLeft;
                        this.dropDown.VerticalPopupAlignment = VerticalPopupAlignment.BottomToTop;
                        break;
                    }
            }
        }

        protected virtual void AdjustDropDownAlignmentForOrientation()
        {
            if (this.HierarchyParent != null || !this.IsMainMenuItem)
            {
                if (!this.RightToLeft)
                {
                    this.dropDown.HorizontalPopupAlignment = HorizontalPopupAlignment.LeftToRight;
                }
                else
                {
                    this.dropDown.HorizontalPopupAlignment = HorizontalPopupAlignment.RightToLeft;
                }

                this.dropDown.VerticalPopupAlignment = VerticalPopupAlignment.TopToTop;
                this.dropDown.HorizontalAlignmentCorrectionMode = AlignmentCorrectionMode.SnapToOuterEdges;
            }
            else
            {
                this.dropDown.HorizontalAlignmentCorrectionMode = AlignmentCorrectionMode.Smooth;
                this.dropDown.VerticalAlignmentCorrectionMode = AlignmentCorrectionMode.SnapToOuterEdges;

                if (this.IsInValidState(true) && this.ElementTree.Control is RadMenu)
                {
                    if ((this.ElementTree.Control as RadMenu).Orientation == Orientation.Vertical)
                    {
                        this.dropDown.HorizontalPopupAlignment = HorizontalPopupAlignment.LeftToRight;
                        this.dropDown.VerticalPopupAlignment = VerticalPopupAlignment.TopToTop;
                    }
                    else
                    {
                        this.dropDown.HorizontalPopupAlignment = (this.RightToLeft) ? HorizontalPopupAlignment.RightToRight : HorizontalPopupAlignment.LeftToLeft;
                        this.dropDown.VerticalPopupAlignment = VerticalPopupAlignment.TopToBottom;
                    }
                }
            }
        }

        protected virtual void AdjustDropDownAnimations()
        {
            if (this.IsInValidState(true))
            {
                if (this.ElementTree.Control is RadMenu)
                {
                    this.dropDown.AnimationEnabled = ((RadMenu)this.ElementTree.Control).DropDownAnimationEnabled;
                    this.dropDown.EasingType = ((RadMenu)this.ElementTree.Control).DropDownAnimationEasing;
                    this.dropDown.AnimationFrames = ((RadMenu)this.ElementTree.Control).DropDownAnimationFrames;
                    this.dropDown.DropDownAnimationDirection = this.PopupDirection;
                }
                else if (this.ElementTree.Control is RadDropDownMenu)
                {
                    this.dropDown.AnimationEnabled = ((RadDropDownMenu)this.ElementTree.Control).AnimationEnabled;
                    this.dropDown.EasingType = ((RadDropDownMenu)this.ElementTree.Control).EasingType;
                    this.dropDown.AnimationFrames = ((RadDropDownMenu)this.ElementTree.Control).AnimationFrames;
                }
            }
        }

        /// <summary>
        ///	Closes the RadMenuItemBase popup.
        /// </summary>
        public virtual void HideChildItems()
        {
            if (dropDown != null)
            {
                dropDown.ClosePopup(RadPopupCloseReason.CloseCalled);
            }
        }

        #region Events

        /// <summary>
        ///		Occurs after the menu item dropdown opens.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Occurs after the menu item dropdown opens.")]
        public event EventHandler DropDownOpened;

        /// <summary>
        ///		Occurs before the menu item dropdown opens.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Occurs before the menu item dropdown opens.")]
        public event CancelEventHandler DropDownOpening;

        /// <summary>
        ///		Occurs after the menu item dropdown closes.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Occurs after the menu item dropdown closes.")]
        public event RadPopupClosedEventHandler DropDownClosed;

        /// <summary>
        /// Occurs before the popup is closed.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Occurs before the popup is closed.")]
        public event RadPopupClosingEventHandler DropDownClosing;

        #endregion

        #region Properties

        #region Internal

        internal int PositionToBeRestoredAfterMerge
        {
            get
            {
                return this.positionToBeRestoredAfterMerge;
            }
            set
            {
                this.positionToBeRestoredAfterMerge = value;
            }
        }

        internal protected override bool DesignTimeAllowDrop
        {
            get
            {
                return false;
            }
        }

        internal RadMenuItemAccessibleObject AccessibleObject
        {
            get
            {
                if (this.accessibleObject == null)
                {
                    this.accessibleObject = new RadMenuItemAccessibleObject(this);
                }

                return this.accessibleObject;
            }
        }

        #endregion

        [DefaultValue(false)]
        public bool MdiList
        {
            get
            {
                return this.GetBitState(MdiListStateKey);
            }
            set
            {
                this.SetBitState(MdiListStateKey, value);
            }
        }

        [DefaultValue(MenuMerge.Add)]
        public MenuMerge MergeType
        {
            get
            {
                return mergeType;
            }
            set
            {
                mergeType = value;
            }
        }

        [DefaultValue(0)]
        public int MergeOrder
        {
            get
            {
                return mergeOrder;
            }
            set
            {
                mergeOrder = value;
            }
        }

        public static readonly RadProperty SelectedProperty = RadProperty.Register(
            "Selected", typeof(bool), typeof(RadMenuItemBase), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.CanInheritValue));

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [RadPropertyDefaultValue("Selected", typeof(RadMenuItemBase))]
        public bool Selected
        {
            get
            {
                return (bool)this.GetValue(SelectedProperty);
            }
            set
            {
                this.SetValue(SelectedProperty, value);
            }
        }

        public static readonly RadProperty IsPopupShownProperty = RadProperty.Register(
            "IsPopupShown", typeof(bool), typeof(RadMenuItemBase), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.None));

        /// <summary>
        /// Gets a value indiciating that the popup containing this menu item's children is shown.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [RadPropertyDefaultValue("IsPopupShown", typeof(RadMenuItemBase))]
        [Description("Indicates whether the popup containing this menu item's children is shown.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPopupShown
        {
            get
            {
                return (bool)this.GetValue(IsPopupShownProperty);
            }
        }

        public static readonly RadProperty PopupDirectionProperty = RadProperty.Register(
            "PopupDirection", typeof(RadDirection), typeof(RadMenuItemBase), new RadElementPropertyMetadata(
                RadDirection.Right, ElementPropertyOptions.None));

        /// <summary>
        ///		Gets or sets the direction of the popup which is opened by this menu item.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets the direction of the popup which is opened by this menu item.")]
        public RadDirection PopupDirection
        {
            get
            {
                return (RadDirection)this.GetValue(PopupDirectionProperty);
            }
            set
            {
                this.SetValue(PopupDirectionProperty, value);
            }
        }

        /// <summary>
        ///		Gets a collection of the child items.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadItemOwnerCollection Items
        {
            get
            {
                this.EnsureDropDownCreated();
                return this.dropDown.Items;
            }
        }

        /// <summary>
        /// Gets or sets menu header column text
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue("")]
        public string DropDownHeaderText
        {
            get
            {
                if (this.dropDown != null)
                {
                    return this.dropDown.HeaderText;
                }
                return "";
            }
            set
            {
                EnsureDropDownCreated();
                this.dropDown.HeaderText = value;
            }
        }

        /// <summary>
        /// Gets or sets menu header column image
        /// </summary>
        [DefaultValue(null)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image DropDownHeaderImage
        {
            get
            {
                if (this.dropDown != null)
                {
                    return this.dropDown.HeaderImage;
                }
                return null;
            }
            set
            {
                EnsureDropDownCreated();
                this.dropDown.HeaderImage = value;
            }
        }

        /// <summary>
        ///		Indicates whether the DropDown of the item should have two columns or one column.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Indicates whether the DropDown of the item should have two columns or one column.")]
        [DefaultValue(false)]
        [Obsolete]
        public bool HasTwoColumnDropDown
        {
            get
            {
                EnsureDropDownCreated();
                return dropDown.IsTwoColumnMenu;
            }
            set
            {
                EnsureDropDownCreated();
                dropDown.IsTwoColumnMenu = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMainMenuItem
        {
            get
            {
                return this.GetBitState(IsMainMenuItemStateKey);
            }
            set
            {
                this.SetBitState(IsMainMenuItemStateKey, value);
            }
        }

        /// <summary>
        ///		Returns the instance of the root MenuElement which hosts all menu items.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Obsolete("Use the Owner property instead. This property will be removed in Q1 2010.")]
        public RadElement MenuElement
        {
            get
            {
                return this.Owner as RadElement;
            }
            set
            {
                this.Owner = value;
            }
        }

        /// <summary>
        /// Returns the control that owns this item. This can be a RadMenu or RadDropDownMenu.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Control OwnerControl
        {
            get
            {
                if (ownerControl != null)
                {
                    return ownerControl;
                }
                if ((this.Owner is RadMenuItemBase) && (this.Owner as RadMenuItemBase).ElementTree != null)
                {
                    return (this.Owner as RadMenuItemBase).ElementTree.Control;
                }

                if (this.ElementTree != null)
                {
                    return this.ElementTree.Control;
                }

                return null;
            }
            internal set
            {
                this.ownerControl = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsOnDropDown
        {
            get
            {
                return this.Parent is RadDropDownMenuLayout;
            }
        }

        /// <summary>
        ///		Gets a values indicating whether this item has child items to show.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool HasChildItemsToShow
        {
            get
            {
                if (this.dropDown != null)
                {
                    return this.LayoutableChildrenCount > 0;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the drop down menu associated with this menu item
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadDropDownMenu DropDown
        {
            get
            {
                return this.dropDown;
            }
        }

        protected override IComponentTreeHandler ShortcutsHandler
        {
            get
            {
                if (this.Owner is RadMenuItemBase)
                {
                    return (this.Owner as RadMenuItemBase).ElementTree.ComponentTreeHandler;
                }
                return null;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
        public virtual bool HandlesKeyboard
        {
            get
            {
                return this.GetBitState(HandlesKeyboardStateKey);
            }
            set
            {
                this.SetBitState(HandlesKeyboardStateKey, value);
            }
        }

        #endregion

        private IHierarchicalItem FindRootHierarchyItem()
        {
            IHierarchicalItem current = this.hierarchyParent;
            while (current != null)
            {
                if (current.ParentItem == null)
                {
                    return current;
                }
                current = current.ParentItem;
            }
            return null;
        }

        #region IHierarchicalItem Members
        public IHierarchicalItem RootItem
        {
            get
            {
                return FindRootHierarchyItem();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Owner
        {
            get
            {
                if (this.HierarchyParent != null)
                {
                    return this.HierarchyParent.Owner;
                }
                return this.owner;
            }
            set
            {
                if (this.HierarchyParent == null)
                {
                    this.owner = value;
                }
            }
        }

        /// <summary>
        ///		Gets a value indicating whether this item has child items.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasChildren
        {
            get
            {
                return (this.dropDown != null && this.dropDown.Items.Count > 0);
            }
        }

        /// <summary>
        ///		Gets a value indicating whether this item is in the root items collection.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsRootItem
        {
            get
            {
                return this.HierarchyParent == null;
            }
        }

        /// <summary>
        ///     Gets or sets the parent menu item.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Obsolete("This property is obsolete and will be removed in the next release. Use the HierarchyParent property instead.")]
        public new IHierarchicalItem ParentItem
        {
            get
            {
                return this.hierarchyParent;
            }
            set
            {
                this.hierarchyParent = value;
            }
        }

        /// <summary>
        ///	    Gets or sets the parent menu item.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IHierarchicalItem HierarchyParent
        {
            get
            {
                return this.hierarchyParent;
            }
            set
            {
                this.hierarchyParent = value;
            }
        }

        /// <summary>
        ///		Gets the next child item in the parent item's Items collection.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadItem Next
        {
            get
            {
                if (this.HierarchyParent != null)
                {
                    int count = this.HierarchyParent.Items.Count;
                    int offset = this.HierarchyParent.Items.IndexOf(this);
                    offset++;
                    if (count > offset)
                    {
                        return this.HierarchyParent.Items[offset] as RadItem;
                    }
                }
                return null;
            }
        }

        /// <summary>
        ///		Gets the previous child item in the parent item's Items collection.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadItem Previous
        {
            get
            {
                if (this.HierarchyParent != null)
                {
                    int offset = this.HierarchyParent.Items.IndexOf(this);
                    offset--;
                    if (offset >= 0)
                    {
                        return this.HierarchyParent.Items[offset] as RadItem;
                    }
                }
                return null;
            }
        }

        #endregion

        #region ISiteProvider

        public ISite GetSite()
        {
            ISite site = base.Site;
            if (site == null)
            {
                if (this.dropDown != null && this.dropDown.Items.Count > 0)
                {
                    site = this.dropDown.Items[0].Site;
                }
            }

            return site;
        }

        #endregion

        #region Events handlers

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (this.GetSite() != null && this.ElementTree.Control is IItemsControl)
            {
                IItemsControl itemsControl = this.ElementTree.Control as IItemsControl;

                foreach (RadMenuItemBase item in itemsControl.Items)
                {
                    if (!object.ReferenceEquals(this, item) && item.IsPopupShown)
                    {
                        item.HideChildItems();
                    }
                }
            }

            if (!this.IsPopupShown)
            {
                this.ShowChildItems();
            }
            else if (!this.IsOnDropDown)
            {
                this.HideChildItems();
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            this.Select();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.IsMouseDown = false;
            if (this.IsPopupShown)
            {
                return;
            }

            IItemsControl itemsControl = this.ElementTree.Control as IItemsControl;

            if (itemsControl == null)
            {
                return;
            }

            if (object.ReferenceEquals(itemsControl.GetSelectedItem(), this))
            {
                this.Deselect();
            }
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            if (GetSite() != null && !(this.ElementTree.Control is RadMenu))
            {
                PopupManager.Default.CloseAll(RadPopupCloseReason.Mouse);

                IDesignerHost host = (IDesignerHost)this.Site.GetService(typeof(IDesignerHost));
                //RadItemDesigner designer = (RadItemDesigner)host.GetDesigner(this);
                //designer.DoDefaultAction();
            }
        }

        protected override void OnBubbleEvent(RadElement sender, RoutedEventArgs args)
        {
            base.OnBubbleEvent(sender, args);

            if (args.RoutedEvent == RadItem.MouseClickedEvent)
            {
                if (GetSite() != null && !(this.ElementTree.Control is RadMenu))
                {
                    ISelectionService service = this.Site.GetService(typeof(ISelectionService)) as ISelectionService;

                    if (service != null && !service.GetComponentSelected(this))
                    {
                        service.SetSelectedComponents(new IComponent[] { this });
                    }
                }
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == RadElement.RightToLeftProperty && this.dropDown != null)
            {
                this.dropDown.RightToLeft = (this.RightToLeft) ? System.Windows.Forms.RightToLeft.Yes : System.Windows.Forms.RightToLeft.No;
            }

        }

        /// <summary>
        ///		Raises the <see cref="DropDownClosed"/> event.
        /// </summary>
        /// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnDropDownClosed(RadPopupClosedEventArgs args)
        {
            if (this.DropDownClosed != null)
            {
                this.DropDownClosed(this, args);
            }

            this.SetValue(RadMenuItemBase.IsPopupShownProperty, false);
        }

        /// <summary>
        ///		Raises the <see cref="DropDownOpening"/> event.
        /// </summary>
        /// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnDropDownOpening(CancelEventArgs args)
        {
            if (this.DropDownOpening != null)
            {
                this.DropDownOpening(this, args);
            }
            //PrepareMenuForDesignTime();
        }

        /// <summary>
        /// Raises the <see cref="DropDownClosing"/> event.
        /// </summary>
        /// <param name="args">An instance of the <see cref="RadPopupClosingEventArgs"/> class 
        /// that contains information about the event.</param>
        protected virtual void OnDropDownClosing(RadPopupClosingEventArgs args)
        {
            if (this.DropDownClosing != null)
            {
                this.DropDownClosing(this, args);
            }
        }

        /// <summary>
        ///		Raises the <see cref="DropDownOpened"/> event.
        /// </summary>
        /// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnDropDownOpened(EventArgs args)
        {
            if (this.DropDownOpened != null)
            {
                this.DropDownOpened(this, args);
            }

            this.SetValue(RadMenuItemBase.IsPopupShownProperty, true);
        }

        #endregion

        protected virtual RadDropDownMenu CreateDropDownMenu()
        {
            return new RadDropDownMenu(this);
        }

        protected virtual void EnsureDropDownCreated()
        {
            if (dropDown != null)
                return;

            this.dropDown = this.CreateDropDownMenu();
            this.dropDown.RightToLeft = (this.RightToLeft) ? System.Windows.Forms.RightToLeft.Yes : System.Windows.Forms.RightToLeft.No;
            this.WireDropDownEvents();
            this.OnDropDownCreated();
            #region Obsolete

            //this.dropDown.DropDownOpened += delegate(object sender, EventArgs args)
            //{
            //    this.dropDown.Site = this.ElementTree.Control.Site;
            //    this.SetValue(IsPopupShownProperty, true);
            //    this.OnDropDownOpened(EventArgs.Empty);
            //};


            //this.dropDown.PopupOpening += delegate(object sender, CancelEventArgs args)
            //{
            //    if (!this.RightToLeft ||  !(this.ElementTree.Control is RadMenu) )
            //    {
            //        return;
            //    }

            //    RadPopupOpeningEventArgs e = args as RadPopupOpeningEventArgs;
            //    if (e != null)
            //    {
            //        e.CustomLocation = new Point(e.CustomLocation.X - (this.dropDown.Size.Width - this.Size.Width), e.CustomLocation.Y);
            //    }
            //};

            //this.dropDown.DropDownOpening += delegate(object sender, CancelEventArgs args)
            //{
            //    this.OnDropDownOpening(args);
            //};


            //this.dropDown.DropDownClosed += delegate(object sender, RadPopupClosedEventArgs args)
            //{
            //    this.SetValue(IsPopupShownProperty, false);
            //    this.OnDropDownClosed(args);
            //};

            //this.dropDown.PopupClosing += delegate(object sender, RadPopupClosingEventArgs args)
            //{
            //    this.OnDropDownClosing(args);
            //};
            #endregion

        }

        private void WireDropDownEvents()
        {
            this.dropDown.DropDownOpened += dropDown_DropDownOpened;
            this.dropDown.DropDownOpening += dropDown_DropDownOpening;
            this.dropDown.DropDownClosed += dropDown_DropDownClosed;
            this.dropDown.PopupClosing += dropDown_PopupClosing;
        }

        private void UnWireDropDownEvents()
        {
            this.dropDown.DropDownOpened -= dropDown_DropDownOpened;
            this.dropDown.DropDownOpening -= dropDown_DropDownOpening;
            this.dropDown.DropDownClosed -= dropDown_DropDownClosed;
            this.dropDown.PopupClosing -= dropDown_PopupClosing;
        }


        void dropDown_PopupClosing(object sender, RadPopupClosingEventArgs args)
        {
            this.OnDropDownClosing(args);
        }

        void dropDown_DropDownClosed(object sender, RadPopupClosedEventArgs args)
        {
            this.SetValue(IsPopupShownProperty, false);
            this.OnDropDownClosed(args);
        }

        void dropDown_DropDownOpening(object sender, CancelEventArgs e)
        {
            this.OnDropDownOpening(e);
        }

        void dropDown_DropDownOpened(object sender, EventArgs e)
        {
            this.dropDown.Site = this.ElementTree.Control.Site;
            this.SetValue(IsPopupShownProperty, true);
            this.OnDropDownOpened(EventArgs.Empty);
        }

        protected virtual void OnDropDownCreated()
        {
        }

        protected virtual RectangleF GetClientRectangle(SizeF finalSize)
        {
            float left = this.Padding.Left + this.BorderThickness.Left;
            float top = this.Padding.Top + this.BorderThickness.Top;
            float width = Math.Max(0, finalSize.Width - (this.Padding.Horizontal + this.BorderThickness.Horizontal));
            float height = Math.Max(0, finalSize.Height - (this.Padding.Vertical + this.BorderThickness.Vertical));
            return new RectangleF(left, top, width, height);
        }

        protected internal override bool? ShouldSerializeProperty(PropertyDescriptor property)
        {
            if (property.Name == "Class")
            {
                return this.Class != "RadMenuItem";
            }

            return base.ShouldSerializeProperty(property);
        }
        //private void PrepareMenuForDesignTime()
        //{
        //    ISite site = this.GetSite();
        //    if (site != null)
        //    {
        //        if (!this.GetBitState(PreparedForDesignTimeStateKey))
        //        {
        //            this.BitState[PreparedForDesignTimeStateKey] = true;
        //            this.Items.Add(new RadNewMenuItem("Add new", this, this.Items, this, "Items", null));
        //        }
        //        for (int i = this.Items.Count - 1; i >= 0; i--)
        //            if (this.Items[i].Site == null && !(this.Items[i] is RadNewMenuItem))
        //            {
        //                this.Items.RemoveAt(i);
        //            }
        //        for (int i = 0; i < this.Items.Count-1; i++)
        //        {
        //            if (this.Items[i] is RadNewMenuItem)
        //            {
        //                RadNewMenuItem item = (RadNewMenuItem)this.Items[i];
        //                this.Items.RemoveAt(i);
        //                this.Items.Add(item);
        //                break;
        //            }
        //        }
        //    }
        //}
    }
}
