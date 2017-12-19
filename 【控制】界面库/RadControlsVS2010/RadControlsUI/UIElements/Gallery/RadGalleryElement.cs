using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using System.Drawing;
using System.ComponentModel;
using Telerik.WinControls.Elements;
using Telerik.WinControls.Primitives;
using System.Windows.Forms;
using Telerik.WinControls.Design;
using System.Drawing.Design;
using System.Diagnostics;
using Telerik.WinControls.Layout;

namespace Telerik.WinControls.UI
{

    [Designer(DesignerConsts.RadGalleryElementDesignerString)]
    public class RadGalleryElement :  RadItem, IDropDownMenuOwner, IItemsOwner
    {
        #region BitState Keys

        internal const ulong FilterEnabledStateKey = RadItemLastStateKey << 1;
        internal const ulong ItemSelectionStateKey = FilterEnabledStateKey << 1;
        internal const ulong ZoomItemsStateKey = ItemSelectionStateKey << 1;
        internal const ulong DropDownInheritThemeClassNameStateKey = ZoomItemsStateKey << 1;

        #endregion

        #region Consts
        private const int defaultMenuHeight = 21;
        private const int minOffset = 5;
        private readonly Size buttonsPanelMaxSize = new Size(0, 54);//54 is come from RadOverFlowPrimitive - only with this size Arrow looks ok
        private readonly Size arrowButtonsMinSize = new Size(15,20);// only with this size arrow primitive look ok 
        #endregion 

        #region Fields

        private RadItemOwnerCollection items;
        private RadItemOwnerCollection groups;
        private RadItemOwnerCollection filters;
        private RadItemOwnerCollection tools;
        private RadCanvasViewport popupGroupsViewport = new RadCanvasViewport();

        private IntegralScrollWrapPanel inribbonItemsLayoutPanel;
        private int currentLine = 0; 
        private RadImageButtonElement upButton;
        private RadImageButtonElement downButton;
        private RadImageButtonElement popupButton;
        private BorderPrimitive inribbonPanelBorder;
        private FillPrimitive inribbonFillPrimitive;
        private DropDownEditorLayoutPanel inribbonPanel;
        private RadGalleryButtonsLayoutPanel buttonsPanel;

        private StackLayoutPanel dropDownPanel;
        //private RadDropDownMenu menu;
        private RadScrollViewer popupScrollViewer;
        private RadGalleryMenuLayoutPanel subMenuLayoutPanel;
        private RadDropDownButtonElement filterDropDown;
        private RadGalleryItem selectedItem;
        
        private RadDirection dropDownDirection = RadDirection.Down;
        private GalleryMouseOverBehavior zoomBehavior;
        private RadGalleryItem lastClickedItem;

        private RadGalleryPopupElement galleryPopupElement;
        private RadGalleryDropDown downMenu;

        private static readonly object GalleryItemHoverEventKey;
        private static readonly object DropDownOpeningEventKey;
        private static readonly object DropDownOpenedEventKey;
        private static readonly object DropDownClosingEventKey;
        private static readonly object DropDownClosedEventKey;

        public static RadProperty IsSelectedProperty = RadProperty.Register(
            "IsSelected", typeof(bool), typeof(RadGalleryElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.None));

        public static RadProperty MaxRowsProperty = RadProperty.Register("MaxRows", typeof(int), typeof(RadGalleryElement),
            new RadElementPropertyMetadata(2, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsParentMeasure));

        public static RadProperty MaxColumnsProperty = RadProperty.Register("MaxColumns", typeof(int), typeof(RadGalleryElement),
            new RadElementPropertyMetadata(5, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty MaxDropDownColumnsProperty = RadProperty.Register("MaxDropDownColumns", typeof(int), typeof(RadGalleryElement),
            new RadElementPropertyMetadata(0, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty MinDropDownColumnsProperty = RadProperty.Register("MinDropDownColumns", typeof(int), typeof(RadGalleryElement),
            new RadElementPropertyMetadata(0, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

  
        #endregion
      
        #region Constructors
        static RadGalleryElement()
        {
            new Themes.ControlDefault.Gallery().DeserializeTheme();
            GalleryItemHoverEventKey = new object();
            DropDownOpeningEventKey = new object();
            DropDownOpenedEventKey = new object();
            DropDownClosingEventKey = new object();
            DropDownClosedEventKey = new object();
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.BitState[FilterEnabledStateKey] = true;

            this.tools = new RadItemOwnerCollection();
            this.tools.ItemTypes = new Type[] { typeof(RadMenuItemBase) };

            this.items = new RadItemOwnerCollection();
            this.items.ItemTypes = new Type[] { typeof(RadGalleryItem) };
            this.items.ItemsChanged += new ItemChangedDelegate(items_ItemsChanged);

            this.groups = new RadItemOwnerCollection();
            this.groups.ItemTypes = new Type[] { typeof(RadGalleryGroupItem) };

            this.filters = new RadItemOwnerCollection();
            this.filters.ItemTypes = new Type[] { typeof(RadGalleryGroupFilter) };

            this.ClipDrawing = true;
            this.zoomBehavior = new GalleryMouseOverBehavior(this);
            this.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
        }

        private void WireMenuDropDownEvents()
        {
            this.downMenu.MouseWheel += new MouseEventHandler(menu_MouseWheel);
            this.downMenu.DropDownOpening += new CancelEventHandler(this.menu_DropDownOpening);
            this.downMenu.DropDownOpened += new EventHandler(this.menu_DropDownOpened);
            this.downMenu.DropDownClosing += new RadPopupClosingEventHandler(this.menu_DropDownClosing);
            this.downMenu.DropDownClosed += new RadPopupClosedEventHandler(this.menu_DropDownClosed);
        }
        private void UnWireMenuDropDownEvents()
        {
            this.downMenu.MouseWheel -= new MouseEventHandler(menu_MouseWheel);
            this.downMenu.DropDownOpening -= new CancelEventHandler(this.menu_DropDownOpening);
            this.downMenu.DropDownOpened -= new EventHandler(this.menu_DropDownOpened);
            this.downMenu.DropDownClosing -= new RadPopupClosingEventHandler(this.menu_DropDownClosing);
            this.downMenu.DropDownClosed -= new RadPopupClosedEventHandler(this.menu_DropDownClosed);
        }

        protected override void DisposeManagedResources()
        {
            this.UnWireMenuDropDownEvents();
            base.DisposeManagedResources();
        }

        private void menu_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                this.DoScrollLineUp();
            else if (e.Delta < 0)
                this.DoScrollLineDown();

            if (e is HandledMouseEventArgs && this.downMenu!=null && this.downMenu.IsVisible)
            {
                ((HandledMouseEventArgs)e).Handled = true;
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets an instance of the <see cref="RadGalleryDropDown"/>class
        /// that represents the popup control which hosts the <see cref="RadGalleryPopupElement"/>
        /// displayed to the user when he/she clicks on the drop-down button of the gallery.
        /// </summary>
        [Browsable(false)]
        public RadGalleryDropDown GalleryDropDown
        {
            get
            {
                return this.downMenu;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="RadGalleryPopupElement"/>class
        /// that represents the main element put in the <see cref="RadGalleryDropDown"/>
        /// when it is shown to the user. This element holds the content of the gallery,
        /// as well as some additional elements like sizing grip etc.
        /// </summary>
        [Browsable(false)]
        public RadGalleryPopupElement GalleryPopupElement
        {
            get
            {
                return this.galleryPopupElement;
            }
        }

        #region Elements

        /// <summary>
        /// Gets the <see cref="FillPrimitive"/>instance
        /// that represents the Gallery Element's fill.
        /// </summary>
        [Browsable(false)]
        public FillPrimitive InRibbonFill
        {
            get
            {
                return this.inribbonFillPrimitive;
            }
        }

        /// <summary>
        /// Gets the <see cref="BorderPrimitive"/>instance
        /// that represents the Gallery Element's border.
        /// </summary>
        [Browsable(false)]
        public BorderPrimitive InRibbonBorder
        {
            get
            {
                return this.inribbonPanelBorder;
            }
        }

        /// <summary>
        /// Gets tne <see cref="RadImageButtonElement"/> that
        /// represents the up button in the gallery element.
        /// </summary>
        [Browsable(false)]
        public RadImageButtonElement UpButton
        {
            get
            {
                return this.upButton;
            }
        }

        /// <summary>
        /// Gets tne <see cref="RadImageButtonElement"/> that
        /// represents the down button in the gallery element.
        /// </summary>
        [Browsable(false)]
        public RadImageButtonElement DownButton
        {
            get
            {
                return this.downButton;
            }
        }

        /// <summary>
        /// Gets tne <see cref="RadImageButtonElement"/> that
        /// represents the show popup button in the gallery element.
        /// </summary>
        [Browsable(false)]
        public RadImageButtonElement PopupButton
        {
            get
            {
                return this.popupButton;
            }
        }

        #endregion

        private SizingMode dropDownSizingMode = SizingMode.UpDownAndRightBottom;
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the drop down sizing mode. The mode can be: horizontal, veritcal or a combination of them.")]
        [DefaultValue(SizingMode.UpDownAndRightBottom)]
        public SizingMode DropDownSizingMode
        {
            get { return this.dropDownSizingMode; }
            set { this.dropDownSizingMode = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether group filtering is enbled when filters are defined.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets a value indicating whether group filtering is enbled when filters are defined."),
        DefaultValue(true)]
        public bool FilterEnabled
        {
            get
            {
                return this.GetBitState(FilterEnabledStateKey);
            }
            set
            {
                this.SetBitState(FilterEnabledStateKey, value);
            }
        }

        /// <summary>
        /// Gets a collection representing the group filters defined in this gallery.
        /// </summary>	
        [Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor)),
        Category(RadDesignCategory.DataCategory),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Description("Gets a collection representing the group filters defined in this gallery.")]
        public RadItemOwnerCollection Filters
        {
            get
            {
                return this.filters;
            }
        }

        /// <summary>
        /// Gets a collection representing the groups contained in this gallery.
        /// </summary>
        [Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor)),
        Category(RadDesignCategory.DataCategory),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Description("Gets a collection representing the groups contained in this gallery.")]
        public RadItemOwnerCollection Groups
        {
            get
            {
                return this.groups;
            }
        }

        /// <summary>
        /// Returns whether the gallery is currently dropped down.
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Returns whether the gallery is currently dropped down.")]
        public bool IsDroppedDown
        {
            get { return this.downMenu!=null && this.downMenu.IsVisible; }
        }

        /// <summary>
        /// Gets a collection representing the items contained in this gallery.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor)),
        Category(RadDesignCategory.DataCategory)]
        [Description("Gets a collection representing the items contained in this gallery.")]
        [RadNewItem("", false)]
        public RadItemOwnerCollection Items
        {
            get
            {
                return this.items;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the selection of the gallery items is enabled or not.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets a value indicating whether the selection of the gallery items is enabled or not."),
        DefaultValue(false)]
        public bool ItemSelection
        {
            get
            {
                return this.GetBitState(ItemSelectionStateKey);
            }
            set
            {
                this.SetBitState(ItemSelectionStateKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of columns to be shown in the in-ribbon portion of the gallery. 
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        RadPropertyDefaultValue("MaxColumns", typeof(RadGalleryElement)),
        Description("Gets or sets the maximum number of columns to be shown in the in-ribbon part of the gallery.")]
        public int MaxColumns
        {
            get
            {
                return (int)this.GetValue(MaxColumnsProperty);
            }
            set
            {
                this.SetValue(MaxColumnsProperty, value);
                this.inribbonItemsLayoutPanel.MaxColumns = value;               
            }
        }
      
        /// <summary>
        /// Gets or sets the maximum number of columns to be shown in the drop-down portion of the gallery. 
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        RadPropertyDefaultValue("MaxDropDownColumns", typeof(RadGalleryElement)),
        Description("Gets or sets the maximum number of columns to be shown in the drop-down portion of the gallery. ")]
        public int MaxDropDownColumns
        {
            get
            {
                return (int)this.GetValue(MaxDropDownColumnsProperty);
            }
            set
            {
                this.SetValue(MaxDropDownColumnsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of rows to be shown in the in-ribbon portion of the gallery. 
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        RadPropertyDefaultValue("MaxRows", typeof(RadGalleryElement)),
        Description("Gets or sets the maximum number of rows to be shown in the in-ribbon part of the gallery.")]
        public int MaxRows
        {
            get
            {
                return (int)this.GetValue(MaxRowsProperty);
            }
            set
            {
                this.SetValue(MaxRowsProperty, value);
                this.inribbonItemsLayoutPanel.MaxRows = value;
            }
        }



        /// <summary>
        /// Gets or sets the minimum number of columns to be shown in the drop-down portion of the gallery. 
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        RadPropertyDefaultValue("MaxDropDownColumns", typeof(RadGalleryElement)),
        Description("Gets or sets the minimum number of columns to be shown in the drop-down portion of the gallery. ")]
        public int MinDropDownColumns
        {
            get
            {
                return (int)this.GetValue(MinDropDownColumnsProperty);
            }
            set
            {
                this.SetValue(MinDropDownColumnsProperty, value);
            }
        }

      

        /// <summary>
        /// Gets or sets the currently selected item.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Gets or sets the currently selected item."),
        Browsable(false)]
        public RadGalleryItem SelectedItem
        {
            get
            {
                return this.selectedItem;
            }
            set
            {
                if (this.ItemSelection && this.items.Contains(value))
                {
                    if (this.selectedItem != null)
                        this.selectedItem.IsSelected = false;
                    this.selectedItem = value;
                    this.selectedItem.IsSelected = true;
                }
            }
        }

        /// <summary>
        /// Gets the Tools menu items collection where you can add and remove items from the
        /// Tools part of the gallery
        /// </summary>
        [RadEditItemsAction]
        [Category(RadDesignCategory.DataCategory),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadItemOwnerCollection Tools
        {
            get
            {
                return this.tools;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a gallery item is zoomed-in when mouse over it.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets a value indicating whether a gallery item is zoomed-in when mouse over it."),
        DefaultValue(false)]
        public bool ZoomItems
        {
            get
            {
                return this.GetBitState(ZoomItemsStateKey);
            }
            set
            {
                foreach (RadElement item in this.items)
                {
                    if (value)
                        item.AddBehavior(this.zoomBehavior);
                    else
                        item.RemoveBehaviors(this.zoomBehavior);
                }
                this.SetBitState(ZoomItemsStateKey, value);
            }
        }

        #endregion

        #region Menu events
        private void menu_DropDownOpening(object sender, CancelEventArgs e)
        {
            this.OnDropDownOpening(e);
        }

        private void menu_DropDownOpened(object sender, EventArgs e)
        {
            this.OnDropDownOpened(e);
        }

        private void menu_DropDownClosing(object sender, RadPopupClosingEventArgs e)
        {
            this.OnDropDownClosing(e);
        }

        private void menu_DropDownClosed(object sender, RadPopupClosedEventArgs e)
        {
            this.OnDropDownClosed(e);

            if (this.Filters.Count > 0 && this.galleryPopupElement != null)
            {
                this.galleryPopupElement.SelectedFilter = (RadGalleryGroupFilter)this.Filters[0];
            }
            this.MinSize = Size.Empty;
            this.HideZoomPopups();
            this.ResetGalleryItemsAndGroups();
            this.ScrollToSelectedItem();
            this.AdjustGalleryAfterPopUpClose();


            if (this.IsInValidState(true))
            {
                this.ElementTree.Control.Invalidate();
            }
        }
        #endregion

        #region Methods

        protected override void OnBitStateChanged(ulong key, bool oldValue, bool newValue)
        {
            base.OnBitStateChanged(key, oldValue, newValue);

            if (key == ItemSelectionStateKey)
            {
                if (!newValue && this.selectedItem != null)
                {
                    this.selectedItem.IsSelected = false;
                }
            }
        }

        protected override void CreateChildElements()
        {
            this.filterDropDown = new RadDropDownButtonElement();
            this.filterDropDown.Alignment = ContentAlignment.TopLeft;
            this.filterDropDown.ExpandArrowButton = true;
            this.filterDropDown.ArrowButton.Arrow.Alignment = ContentAlignment.MiddleLeft;
            this.filterDropDown.Class = "RadGalleryPopupFilterDropDownButton";
            this.filterDropDown.ActionButton.ButtonFillElement.Class = "RadGalleryPopupFilterActionButtonFill";
            this.filterDropDown.ActionButton.BorderElement.Class = "RadGalleryPopupFilterActionButtonBorder";
            this.filterDropDown.ActionButton.Class = "RadGalleryPopupFilterActionButton";
            this.filterDropDown.ArrowButton.Fill.Class = "RadGalleryPopupFilterArrowButtonFill";
            this.filterDropDown.ArrowButton.Border.Class = "RadGalleryPopupFilterArrowButtonBorder";
            this.filterDropDown.ArrowButton.Arrow.Class = "RadGalleryPopupFilterArrowButtonArrow";
            this.filterDropDown.BorderElement.Class = "RadGalleryPopupFilterDropDownButtonBorder";

            this.popupGroupsViewport = new RadCanvasViewport();
        
            this.popupScrollViewer = new RadScrollViewer(this.popupGroupsViewport);
            this.popupScrollViewer.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.popupScrollViewer.HorizontalScrollState = ScrollState.AlwaysHide;
            this.popupScrollViewer.UsePhysicalScrolling = true;
            this.popupScrollViewer.Class = "RadGalleryPopupScrollViewer";
            this.popupScrollViewer.FillElement.Class = "RadGalleryPopupScrollViewerFill";

            this.subMenuLayoutPanel = new RadGalleryMenuLayoutPanel();
            this.subMenuLayoutPanel.Alignment = ContentAlignment.BottomLeft;
            this.subMenuLayoutPanel.Class = "RadGalleryPoupMenuPanel";

            this.dropDownPanel = new StackLayoutPanel();
            this.dropDownPanel.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.dropDownPanel.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.dropDownPanel.EqualChildrenWidth = true;

            this.downMenu = new RadGalleryDropDown(this);
            this.WireMenuDropDownEvents();
            this.downMenu.VerticalPopupAlignment = VerticalPopupAlignment.TopToTop;
            this.downMenu.HorizontalPopupAlignment = HorizontalPopupAlignment.LeftToLeft;
            this.downMenu.AnimationEnabled = false;

            this.galleryPopupElement = new RadGalleryPopupElement(
                                                           this.Items,
                                                           this.Groups,
                                                           this.Filters,
                                                           this.Tools);
            this.downMenu.PopupElement = this.galleryPopupElement;
            this.downMenu.LoadElementTree();
            this.dropDownPanel.Children.Add(this.filterDropDown);
            this.dropDownPanel.Children.Add(this.popupScrollViewer);
            this.dropDownPanel.Children.Add(this.subMenuLayoutPanel);

            //TODO: perhaps we should implement RadStackViewport to support integral scrolling			
            this.inribbonItemsLayoutPanel = new IntegralScrollWrapPanel();
            this.inribbonItemsLayoutPanel.MaxColumns = this.MaxColumns;
            this.inribbonItemsLayoutPanel.MaxRows = this.MaxRows;
            this.Items.Owner = this.inribbonItemsLayoutPanel;

            this.upButton = new RadImageButtonElement();
            this.upButton.Class = "GalleryUpButton";
            this.upButton.Enabled = false;
            this.upButton.MinSize = arrowButtonsMinSize;
            this.upButton.Click += new EventHandler(upButton_Click);
            this.upButton.BorderElement.Class = "GalleryUpButtonBorder";            
            this.upButton.ButtonFillElement.Class = "GalleryArrowButtonFill";

            this.downButton = new RadImageButtonElement();
            this.downButton.Class = "GalleryDownButton";
            this.downButton.MinSize = arrowButtonsMinSize;
            this.downButton.Click += new EventHandler(downButton_Click);
            this.downButton.BorderElement.Class = "GalleryDownButtonBorder";
            this.downButton.ButtonFillElement.Class = "GalleryArrowButtonFill";

            this.popupButton = new RadImageButtonElement();
            this.popupButton.Class = "GalleryPopupButtonButton";
            this.popupButton.MinSize = arrowButtonsMinSize;
         
            this.popupButton.MouseDown += new System.Windows.Forms.MouseEventHandler(popupButton_MouseDown);
            this.popupButton.BorderElement.Class = "GalleryPopupButtonButtonBorder";
            this.popupButton.ButtonFillElement.Class = "GalleryArrowButtonFill";

            this.buttonsPanel = new RadGalleryButtonsLayoutPanel();
            this.buttonsPanel.MaxSize = buttonsPanelMaxSize;
            this.buttonsPanel.SetValue(DropDownEditorLayoutPanel.IsArrowButtonProperty, true);
            this.buttonsPanel.Children.Add(upButton);
            this.buttonsPanel.Children.Add(downButton);
            this.buttonsPanel.Children.Add(popupButton);

            this.inribbonPanel = new DropDownEditorLayoutPanel();
            this.inribbonPanel.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

            this.inribbonFillPrimitive = new FillPrimitive();
            this.inribbonFillPrimitive.Class = "InribbonGalleryFill";
  
            this.inribbonPanel.Children.Add(this.inribbonItemsLayoutPanel);
            this.inribbonPanel.Children.Add(this.buttonsPanel);
            
            this.inribbonPanelBorder = new BorderPrimitive();
            this.inribbonPanelBorder.Visibility = ElementVisibility.Collapsed;
            this.inribbonPanelBorder.Class = "InribbonGalleryBorder";
            this.Children.Add(this.inribbonPanelBorder);
            this.Children.Add(this.inribbonFillPrimitive);
            this.Children.Add(this.inribbonPanel);
        }

        private void items_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            RadGalleryItem item = target as RadGalleryItem;

            if (item != null)
            {
                switch (operation)
                {
                    case ItemsChangeOperation.Inserted:
                        item.Click += new EventHandler(galleryItem_Clicked);
                        item.MouseHover += new EventHandler(item_MouseHover);
                        if (this.GetBitState(ZoomItemsStateKey))
                        {
                            item.AddBehavior(this.zoomBehavior);
                        }
                        item.Owner = this;
                        item.StretchHorizontally = false;
                        item.StretchVertically = false;

                        break;
                    case ItemsChangeOperation.Removed:
                        item.Click -= new EventHandler(galleryItem_Clicked);
                        item.MouseHover -= new EventHandler(item_MouseHover);
                        break;
                }
            }
            this.SetUpDownButtons();
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            SizeF res = base.ArrangeOverride(finalSize);
            this.SetUpDownButtons();
            return res;
        }
      

        private void item_MouseHover(object sender, EventArgs e)
        {
            this.OnGalleryItemHover(new GalleryItemHoverEventArgs(sender as RadGalleryItem));
        }

        private void galleryItem_Clicked(object sender, EventArgs e)
        {
            this.lastClickedItem = sender as RadGalleryItem;
            if (this.lastClickedItem==null)
            {
                return;
            }
            this.SelectedItem = this.lastClickedItem;
            if (!(this.lastClickedItem.ElementTree.Control is ZoomPopup))
            {
                if (this.downMenu.IsVisible)
                {
                    this.downMenu.CallOnMouseLeave(EventArgs.Empty);
                    this.CloseDropDown();
                }
            }
        }

        public void CloseDropDown()
        {
            if (this.downMenu != null)
                this.downMenu.ClosePopup(RadPopupCloseReason.CloseCalled);
        }

        private void popupButton_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.OnMouseLeave(EventArgs.Empty);
            this.ShowDropDown();
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            this.ScrollLineUp();
        }

        public void ScrollLineUp()
        {
            if (this.currentLine > 0)
            {
                --this.currentLine;
            }
            this.inribbonItemsLayoutPanel.ScrollToLine(currentLine);
            this.SetUpDownButtons();
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            this.ScrollLineDown();
        }

        public void ScrollLineDown()
        {
            if (this.currentLine < this.inribbonItemsLayoutPanel.LineCount)
            {
                ++this.currentLine;
            }
            this.inribbonItemsLayoutPanel.ScrollToLine(currentLine);
            this.SetUpDownButtons();
        }

        private void SetUpDownButtons()
        {            
            if (this.inribbonItemsLayoutPanel.LineCount > -1)
            {
                this.currentLine = this.inribbonItemsLayoutPanel.CurrentLine;
                this.upButton.Enabled = this.currentLine != 0;
                this.downButton.Enabled = this.inribbonItemsLayoutPanel.LineCount - 1 != this.currentLine;
            }
            else
            {
                this.upButton.Enabled = false;
                this.downButton.Enabled = false;
            }
        }

        public void ShowDropDown()
        {
            int galleryHeight = (int)this.CalculateGalleryHeight(this.Size.Width);
       
            
            this.downMenu.MaximumSize = new Size(0, (int) (SystemInformation.VirtualScreen.Height*2.0/3.0));
            if (this.ElementTree != null && this.ElementTree.Control is RadControl)
            {
                this.downMenu.ThemeName = ((RadControl) this.ElementTree.Control).ThemeName;
            }
            
            this.MinSize = this.Size;

            if (this.galleryPopupElement != null)
            {
                this.galleryPopupElement.ClearCollections();
                this.galleryPopupElement.Dispose();
                this.galleryPopupElement = null;
            }
            SizeF initialGallerySize = new SizeF(this.Size.Width, Math.Min(galleryHeight, this.downMenu.MaximumSize.Height));
            SizeF minimumGallerySize = new SizeF(this.Size.Width, this.Size.Height+this.CalculateGalleryMinHeight());

            this.galleryPopupElement = new RadGalleryPopupElement(
                                                           this.Items,
                                                           this.Groups,
                                                           this.Filters,
                                                           this.Tools,
                                                             initialGallerySize,
                                                            minimumGallerySize,
                                                            this.DropDownSizingMode);

            this.downMenu.Size = minimumGallerySize.ToSize();
            this.downMenu.PopupElement = this.galleryPopupElement;
            this.downMenu.PopupElement.MinSize = this.downMenu.Size;
            
            this.downMenu.Show(this,0 , this.dropDownDirection);
            this.UpdatePopUpAfterShow();
            this.downMenu.PopupElement.ZIndex = 1000;
            this.downMenu.BringToFront();
            this.downMenu.PopupElement.BringToFront();
        }

        private void UpdatePopUpAfterShow()
        {
            Application.DoEvents();
            foreach (RadElement galleryElement in this.Items)
            {
                galleryElement.Visibility = ElementVisibility.Visible;
            }
            if (this.galleryPopupElement.ElementTree != null)
            {
                this.galleryPopupElement.ElementTree.Control.Refresh();
            }
            Application.DoEvents();
            this.galleryPopupElement.InvalidateMeasure();
            this.galleryPopupElement.InvalidateArrange();
            this.galleryPopupElement.UpdateLayout();
            if(this.galleryPopupElement.ElementTree.Control!=null)
            {
                this.galleryPopupElement.ElementTree.Control.Refresh();
            }
            Application.DoEvents();
        }

       

        private float CalculateGalleryMinHeight()
        {
          
            int size = this.Size.Height * 2;
            if (this.Items.Count == 0)
            {
                return size;
            }

            SizeF sizeItem = this.Items[0].DesiredSize;
            int offset = this.Filters.Count > 0 ? defaultMenuHeight : 0;
            float toolsHeight = this.Tools.Count*defaultMenuHeight;

            return sizeItem.Height + toolsHeight + offset + minOffset;
        }

        private float CalculateGalleryHeight(int width)
        {
            const int minSize = 100;
            if( this.Items.Count == 0)
            {
                return minSize;
            }
            SizeF sizeItem = this.Items[0].DesiredSize;
            int xCount = 1;
            float yCount = 0;
            if (width > 0 && sizeItem.Width > 0)
            {
                xCount = (int)(width / (sizeItem.Width));
                if( xCount == 0)
                {
                    xCount = 1;
                }
            }

            if (this.Groups.Count > 0)
            {
                foreach (RadGalleryGroupItem groupItem in this.Groups)
                {
                    int rowCount = groupItem.Items.Count/xCount;
                    
                    if( (groupItem.Items.Count % xCount) != 0 )
                    {
                        ++rowCount;
                    }

                    yCount += rowCount * sizeItem.Height;
                    yCount += defaultMenuHeight;
                }
            }
            else
            {
                yCount = ( ( this.Items.Count / xCount ) + 1) * sizeItem.Height;
            }
            int filtersHeight = this.Filters.Count > 0 ? defaultMenuHeight : 0;
            float toolsHeight = this.Tools.Count*defaultMenuHeight;

            return yCount + toolsHeight + filtersHeight + minOffset;
        }

        

        private void AdjustGalleryAfterPopUpClose()
        {
            if( this.ElementTree ==null || this.ElementTree.Control==null)
            {
                return;
            }

            RadRibbonBar ribbonBarControl = this.ElementTree.Control as RadRibbonBar;
            Application.DoEvents();

            if (ribbonBarControl != null)
            {
                ribbonBarControl.RibbonBarElement.InvalidateMeasure();
                ribbonBarControl.RibbonBarElement.InvalidateArrange();
                ribbonBarControl.RibbonBarElement.UpdateLayout();
                ribbonBarControl.Invalidate();
            }
            this.InvalidateMeasure();
            this.InvalidateArrange();
            this.PerformLayout();
            this.inribbonItemsLayoutPanel.InvalidateMeasure();
            this.inribbonItemsLayoutPanel.InvalidateArrange();
            this.inribbonItemsLayoutPanel.PerformLayout();
            Application.DoEvents();
        }

        private void ScrollToSelectedItem()
        {
            if (this.lastClickedItem != null)
            {
                this.inribbonItemsLayoutPanel.ScrollToElement(this.lastClickedItem);
                this.SetUpDownButtons();
                this.lastClickedItem = null;
            }
        }

        //TODO: refactor this function - get as is from OLD Gallery
        private void ResetGalleryItemsAndGroups()
        {
            if (this.groups.Count == 0)
            {
                this.galleryPopupElement.GroupHolderStackLayout.Children.Clear();
            }
            else
            {
                foreach (RadGalleryGroupItem groupItem in this.groups)
                {
                    groupItem.Items.Owner = this.inribbonItemsLayoutPanel;
                    if (this.galleryPopupElement.GroupHolderStackLayout.Children.Contains(groupItem))
                    {
                        this.galleryPopupElement.GroupHolderStackLayout.Children.Remove(groupItem);
                    }
                }
            }
           
            this.Items.Owner = this.inribbonItemsLayoutPanel;

            this.inribbonItemsLayoutPanel.InvalidateMeasure();
            this.inribbonItemsLayoutPanel.InvalidateArrange();
            this.inribbonItemsLayoutPanel.UpdateLayout();
        }

        private void HideZoomPopups()
        {
            foreach (RadGalleryItem item in this.Items)
            {
                if(item.IsZoomShown())
                {
                    item.ZoomHide();
                }
            }
        }

        internal void DoScrollLineUp()
        {
            if (this.downMenu.IsVisible)
                this.popupScrollViewer.LineUp();
        }

        internal void DoScrollLineDown()
        {
            if (this.downMenu.IsVisible)
                this.popupScrollViewer.LineDown();
        }

        //TODO: refactor this function - get as is from OLD Gallery
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Delta > 0)
                this.ScrollLineUp();
            else if (e.Delta < 0)
                this.ScrollLineDown();

            if (e is HandledMouseEventArgs && this.downMenu.IsVisible)
            {
                ((HandledMouseEventArgs)e).Handled = true;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the mouse pointer rests on the gallery item.
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.ActionCategory),
        Description("Occurs when the mouse pointer rests on the gallery item.")]
        public event GalleryItemHoverEventHandler GalleryItemHover
        {
            add
            {
                this.Events.AddHandler(RadGalleryElement.GalleryItemHoverEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadGalleryElement.GalleryItemHoverEventKey, value);
            }
        }

        /// <summary>
        /// Raises the GalleryItemHover event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnGalleryItemHover(GalleryItemHoverEventArgs e)
        {
            GalleryItemHoverEventHandler handler1 = (GalleryItemHoverEventHandler)base.Events[RadGalleryElement.GalleryItemHoverEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Occurs when the drop-down is opening.
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.BehaviorCategory),
        Description("Occurs before the drop-down window appears.")]
        public event CancelEventHandler DropDownOpening
        {
            add
            {
                this.Events.AddHandler(RadGalleryElement.DropDownOpeningEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadGalleryElement.DropDownOpeningEventKey, value);
            }
        }

        /// <summary>
        /// Raises the DropDownOpening event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDropDownOpening(CancelEventArgs e)
        {
            CancelEventHandler handler1 = (CancelEventHandler)this.Events[RadGalleryElement.DropDownOpeningEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Occurs when the drop-down has opened.
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.BehaviorCategory),
        Description("Occurs before the drop-down window appears.")]
        public event EventHandler DropDownOpened
        {
            add
            {
                this.Events.AddHandler(RadGalleryElement.DropDownOpenedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadGalleryElement.DropDownOpenedEventKey, value);
            }
        }

        /// <summary>
        /// Raises the DropDownOpened event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDropDownOpened(EventArgs e)
        {
            EventHandler handler1 = (EventHandler)this.Events[RadGalleryElement.DropDownOpenedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Occurs when the drop-down is about to be closed.
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.BehaviorCategory),
        Description("Occurs when the drop-down is about to be closed.")]
        public event RadPopupClosingEventHandler DropDownClosing
        {
            add
            {
                this.Events.AddHandler(RadGalleryElement.DropDownClosingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadGalleryElement.DropDownClosingEventKey, value);
            }
        }

        /// <summary>
        /// Raises the DropDownClosing event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDropDownClosing(RadPopupClosingEventArgs e)
        {
            RadPopupClosingEventHandler handler1 = (RadPopupClosingEventHandler)this.Events[RadGalleryElement.DropDownClosingEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Occurs when the drop-down window has closed.
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.BehaviorCategory),
        Description("Occurs when the drop-down window has closed.")]
        public event RadPopupClosedEventHandler DropDownClosed
        {
            add
            {
                this.Events.AddHandler(RadGalleryElement.DropDownClosedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadGalleryElement.DropDownClosedEventKey, value);
            }
        }

        /// <summary>
        /// Raises the DropDownClosed event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDropDownClosed(RadPopupClosedEventArgs e)
        {
            RadPopupClosedEventHandler handler1 = (RadPopupClosedEventHandler)this.Events[RadGalleryElement.DropDownClosedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }


        #endregion

        #region IDropDownMenuOwner Members

        /// <summary>
        /// Gets or sets value indicating whether DropDownMenu will have the same class name as the owner control or its own.
        /// True means that the same class name will be used as the control that opened the dropdown.
        /// </summary>
        [DefaultValue(false)]
        public bool DropDownInheritsThemeClassName
        {
            get
            {
                return this.GetBitState(DropDownInheritThemeClassNameStateKey);
            }
            set
            {
                this.SetBitState(DropDownInheritThemeClassNameStateKey, value);
            }
        }
        

        bool IDropDownMenuOwner.ControlDefinesThemeForElement(RadElement element)
        {
            if (element.Class == "RadGalleryPopupFilterDropDownButton")
                return true;

            return false;
        }

        #endregion


        protected override void OnTunnelEvent(RadElement sender, RoutedEventArgs args)
        {
            base.OnTunnelEvent(sender, args);

            if (!this.IsInValidState(true))
            {
                return;
            }

            if (args.RoutedEvent == RootRadElement.OnRoutedImageListChanged)
            {
                this.downMenu.ImageList = this.ElementTree.ComponentTreeHandler.ImageList;
            }
        }

        //#region Collapsible members

        //public override bool ExpandElementToStep(int collapseStep)
        //{
        //    if (this.inribbonItemsLayoutPanel.MaxColumns <= this.MaxColumns)
        //    {
        //        ++this.inribbonItemsLayoutPanel.MaxColumns;
        //        return true;
        //    }
        //    return false;
        //}

        //public override bool CollapseElementToStep(int collapseStep)
        //{
        //    if (this.inribbonItemsLayoutPanel.MaxColumns > 1)
        //    {
        //        --this.inribbonItemsLayoutPanel.MaxColumns;
        //        return true;
        //    }
        //    return false;
        //}

        //public override int CollapseMaxSteps
        //{
        //    get { return this.MaxColumns; }
        //}

        //public override bool CanCollapseToStep(int nextStep)
        //{
        //    return this.inribbonItemsLayoutPanel.MaxColumns  > 1;
        //}

        //public override bool CanExpandToStep(int nextStep)
        //{
        //    return this.inribbonItemsLayoutPanel.MaxColumns <= this.MaxColumns;
        //}
        
        //#endregion
    }
   
    public delegate void GalleryItemHoverEventHandler(object sender, GalleryItemHoverEventArgs args);    
}
