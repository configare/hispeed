using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System.ComponentModel;
using System.Drawing.Design;
using Telerik.WinControls.Design;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a Ribbon Bar group. The Group can contain telerik controls. You may
    /// group related controls in groups; this gives the application intuitive interface.
    /// </summary>
    [ToolboxItem(false), ComVisible(false)]
    [Designer(DesignerConsts.RadRibbonBarGroupDesignerString)]
    public class RadRibbonBarGroup : CollapsibleElement, IItemsElement
    {
        private readonly static Padding defaultMargin = new Padding(2);
        private readonly static Size defaultMaxSize = new Size(0, 100);
        private readonly static Size defaultMinSize = new Size(20, 86);

        static RadRibbonBarGroup()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ItemStateManagerFactory(), typeof(RadRibbonBarGroup));
        }

        public static RadProperty OldImageProperty = RadProperty.Register(
            "OldImage", typeof(Image), typeof(RadRibbonBarGroup), new RadElementPropertyMetadata(
                null, ElementPropertyOptions.None));

        public static RadProperty OldImageIndexProperty = RadProperty.Register(
            "OldImageIndex", typeof(int), typeof(RadRibbonBarGroup), new RadElementPropertyMetadata(
                -1, ElementPropertyOptions.None));

        private RadItemOwnerCollection items;
        private StackLayoutPanel stackLayoutPanel;
        private RadDropDownButtonElement dropDownElement;
        private RadButtonElement dialogButton;
        private TextPrimitive textPrimitive;
        private BorderPrimitive borderPrimitive;
        private FillPrimitive captionElementFill;
        private FillPrimitive groupFill;
        private int collapsingPriority = 0;        

        /// <summary>
        /// Occurs when Dialog Button is clicked
        /// </summary>
        [Description("Occurs when Dialog Button is clicked")]
        [Category("Action")]
        public event EventHandler DialogButtonClick;

        protected override Type ThemeEffectiveType
        {
            get
            {
                return typeof(RadRibbonBarGroup);
            }
        }

        #region Elements

        /// <summary>
        /// Gets an instance of the <see cref="BorderPrimitive"/>class
        /// that represents the group's outer border.
        /// </summary>
        public BorderPrimitive GroupBorder
        {
            get
            {
                return this.borderPrimitive;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="FillPrimitive"/>class
        /// that represents the group's fill;
        /// </summary>
        public FillPrimitive GroupFill
        {
            get
            {
                return this.groupFill;
            }
        }

        #endregion

        /// <summary>
        /// Get or sets value indicating whether Dialog button is visible or hidden. 
        /// </summary>
        [DefaultValue(false)]
        [Category(RadDesignCategory.AppearanceCategory), Description("Show or hide the dialog button")]
        public bool ShowDialogButton
        {
            get
            {
                return this.dialogButton.Visibility == ElementVisibility.Visible;
            }

            set
            {
                if (value)
                {
                    this.dialogButton.Visibility = ElementVisibility.Visible;
                    this.textPrimitive.Padding = new Padding(this.textPrimitive.Padding.Left,
                                                              this.textPrimitive.Padding.Top,
                                                              this.textPrimitive.Padding.Right + 10,
                                                              this.textPrimitive.Padding.Bottom);
                }
                else
                {
                    this.dialogButton.Visibility = ElementVisibility.Collapsed;
                    this.textPrimitive.Padding = new Padding(this.textPrimitive.Padding.Left,
                                                              this.textPrimitive.Padding.Top,
                                                              this.textPrimitive.Padding.Right - 10,
                                                              this.textPrimitive.Padding.Bottom);
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)]
        public RadButtonElement DialogButton
        {
            get
            {
                return this.dialogButton;
            }
        }     

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public RadDropDownButtonElement DropDownElement
        {
            get
            {
                return this.dropDownElement;
            }
        }

        private ElementWithCaptionLayoutPanel elementWithCaptionLayoutPanel;       

        //This is a dummy attribute. It serves to enable the Add an item..
        //option in the context menu of the group when in design mode
        
        /// <summary>Gets a collection of nested items.</summary>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [Description("Gets a collection of the items placed in the chunk.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor))]
        [RadNewItem("", false, false, false)]
        public RadItemOwnerCollection Items
        {
            get
            {
                return this.items;
            }
        }

        /// <summary><para>Gets or sets the orientation of the items inside the chunk. Possible values are: Horizontal and 
        /// Vertical.</para></summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets the orientation of the items inside the group. Possible values are: Horizontal and Vertical.")]
        [DefaultValue(typeof(Orientation), "Horizontal")]
        public Orientation Orientation
        {
            get
            {
                return this.stackLayoutPanel.Orientation;
            }

            set
            {
                this.stackLayoutPanel.Orientation = value;
            }
        }

        private Image collapsedImage = null;

        /// <summary>
        /// Gets or sets the image that is displayed when the chunk is collapsed.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
        [DefaultValue(null)]
        public Image CollapsedImage
        {
            get
            {
                return this.collapsedImage;
            }

            set
            {
                this.collapsedImage = value;
                this.dropDownElement.Image = value;
            }
        }

        /// <summary>
        /// Get or Set collapsing order weight - biger mean to start collapsing from this RadRibbonbarGroup
        /// </summary>
        [DefaultValue(0)]
        [Description("Get or Set collapsing order weight - biger mean to start collapsing from this RadRibbonbarGroup")]
        public int CollapsingPriority
        {
            get { return collapsingPriority; }
            set { collapsingPriority = value; }
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
            this.items = new RadItemOwnerCollection();
            this.items.ItemTypes = new Type[] 
            {
                typeof(RadButtonElement),
                typeof(RadCheckBoxElement),
                typeof(RadComboBoxElement),
                typeof(RadDropDownButtonElement),
                typeof(RadDropDownListElement),
                typeof(RadGalleryElement),
                typeof(RadImageButtonElement),
                typeof(RadRadioButtonElement),
                typeof(RadRepeatButtonElement),
                typeof(RadRibbonBarButtonGroup),
                typeof(RadSplitButtonElement),
                typeof(RadTextBoxElement),
                typeof(RadToggleButtonElement)
            };
            this.items.ExcludedTypes = new Type[] { typeof(RadTextBoxElement) };
            this.AutoSize = true;
            this.MinSize = defaultMinSize;
            this.MaxSize = defaultMaxSize;
            this.Margin = defaultMargin;
            this.Shape = new RoundRectShape(3);
        }

        protected override void CreateChildElements()
        {
            this.borderPrimitive = new BorderPrimitive();
            this.borderPrimitive.Class = "RibbonBarChunkBorder";
            this.borderPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

            this.groupFill = new FillPrimitive();
            this.groupFill.Class = "RibbonBarGroupMainFill";
            this.groupFill.ZIndex = -1;
            this.groupFill.SetDefaultValueOverride(FillPrimitive.BackColorProperty, Color.Transparent);
            this.groupFill.SetDefaultValueOverride(FillPrimitive.GradientStyleProperty, GradientStyles.Solid);

            this.textPrimitive = new TextPrimitive();
            this.textPrimitive.RadPropertyChanged += new RadPropertyChangedEventHandler(this.textPrimitive_RadPropertyChanged);
            this.textPrimitive.BindProperty(TextPrimitive.TextProperty, this, RadRibbonBarGroup.TextProperty, PropertyBindingOptions.TwoWay);
            this.textPrimitive.Class = "RibbonBarChunkCaption";
            this.textPrimitive.Alignment = ContentAlignment.MiddleCenter;
            this.textPrimitive.Padding = new Padding(0);
            this.elementWithCaptionLayoutPanel = new ElementWithCaptionLayoutPanel();

            DockLayoutPanel dockLayout = new DockLayoutPanel();

            this.captionElementFill = new FillPrimitive();
            this.captionElementFill.AutoSizeMode = RadAutoSizeMode.Auto;
            this.captionElementFill.SetValue(ElementWithCaptionLayoutPanel.CaptionElementProperty, true);
            this.captionElementFill.Class = "ChunkCaptionFill";

            this.captionElementFill.Children.Add(dockLayout);

            //add DialogButton
            //PP 14/09/2007
            this.dialogButton = new RadButtonElement();
            this.dialogButton.Padding = new Padding(0, 3, 0, 0);

            this.dialogButton.SetDefaultValueOverride(RadButtonItem.ImageProperty,
                ResourceHelper.ImageFromResource(typeof(RadRibbonBarGroup), "Telerik.WinControls.UI.Resources.RibbonDialogButton.png"));
            this.dialogButton.Alignment = ContentAlignment.BottomRight;
            this.dialogButton.Visibility = ElementVisibility.Collapsed;
            this.dialogButton.Class = "DialogButtonClass";
            this.dialogButton.SetValue(DockLayoutPanel.DockProperty, Dock.Right);
            dockLayout.Children.Add(this.dialogButton);
            
            //end add            
            dockLayout.Children.Add(this.textPrimitive);
            dockLayout.LastChildFill = true;
            elementWithCaptionLayoutPanel.Children.Add(this.captionElementFill);
            FillPrimitive bodyElementFill = new FillPrimitive();
            bodyElementFill.AutoSizeMode = RadAutoSizeMode.Auto;
            bodyElementFill.Class = "ChunkBodyFill";
            bodyElementFill.Padding = new Padding(2, 2, 2, 0);
            BorderPrimitive bodyBorder = new BorderPrimitive();
            bodyBorder.Class = "BodyBorder";
            bodyBorder.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            bodyBorder.GradientStyle = GradientStyles.Linear;
            bodyBorder.ForeColor = Color.Transparent;
            bodyBorder.ForeColor2 = Color.White;
            bodyBorder.ForeColor3 = Color.White;
            bodyBorder.ForeColor4 = Color.White;
            bodyElementFill.Children.Add(bodyBorder);
            this.stackLayoutPanel = new StackLayoutPanel();
            this.stackLayoutPanel.Class = "Test";//remove this - only for tests
            this.elementWithCaptionLayoutPanel.Children.Add(bodyElementFill);
            bodyElementFill.Children.Add(this.stackLayoutPanel);
            this.dropDownElement = new RadRibbonBarGroupDropDownButtonElement();
            this.dropDownElement.DropDownInheritsThemeClassName = true;
            this.dropDownElement.Visibility = ElementVisibility.Collapsed;
            this.dropDownElement.ActionButton.Shape = new RoundRectShape(4);
            //this.dropDownElement.DropDownMenu.RootElement.ApplyShapeToControl = true;
            //this.dropDownElement.DropDownMenu.RootElement.Shape = new RoundRectShape(4);
            this.dropDownElement.BorderElement.Class = "GroupDropDownButtonBorder";
            this.dropDownElement.BorderElement.Visibility = ElementVisibility.Collapsed;

            //As of Q1 2010, theme support for the group's popup border and fill
            RadDropDownMenuElement element = this.dropDownElement.DropDownMenu.PopupElement as RadDropDownMenuElement;
            element.Fill.Class = "RibbonBarGroupDropDownFill";
            element.Border.Class = "RibbonBarGroupDropDownBorder";
            element.Class = "RibbonBarGroupDropDownElement";
            this.dropDownElement.DropDownMenu.RootElement.Class = "RibbonBarGroupDropDownRoot";
            //

            FillPrimitive dropDownFill = new FillPrimitive();
            dropDownFill.Visibility = ElementVisibility.Collapsed;
            dropDownFill.Class = "ChunkBodyFill";
            dropDownElement.DropDownMenu.RootElement.Children.Add(dropDownFill);

            this.dropDownElement.Image = ResourceHelper.ImageFromResource(typeof(RadRibbonBarGroup), "Telerik.WinControls.UI.Resources.dropDown.png");
            this.dropDownElement.DisplayStyle = DisplayStyle.ImageAndText;
            this.dropDownElement.TextImageRelation = TextImageRelation.ImageAboveText;
            this.dropDownElement.ShowArrow = false;
            this.dropDownElement.Margin = new Padding(4, 4, 4, 4);
            this.dropDownElement.ActionButton.BorderElement.Visibility = ElementVisibility.Hidden;
            this.dropDownElement.ActionButton.BorderElement.Class = "GroupDropDownButtonInnerBorder";
            this.dropDownElement.ActionButton.Padding = new Padding(4, 10, 4, 28);
            this.dropDownElement.BindProperty(RadDropDownButtonElement.TextProperty, this, RadRibbonBarGroup.TextProperty, PropertyBindingOptions.OneWay);
            this.Children.Add(this.dropDownElement);
            this.dropDownElement.ImageAlignment = ContentAlignment.MiddleCenter;
            this.dropDownElement.ThemeRole = "RibbonGroupDropDownButton";
            this.Children.Add(this.elementWithCaptionLayoutPanel);
            this.Children.Add(this.borderPrimitive);
            this.Children.Add(this.groupFill);
            this.items.Owner = this.stackLayoutPanel;
            this.items.ItemsChanged += this.ItemChanged;
        }

        private void textPrimitive_RadPropertyChanged(object sender, RadPropertyChangedEventArgs e)
        {
            if (e.Property == RadElement.PaddingProperty)
            {
               this.textPrimitive.Padding = new Padding(0);
            }
        }

        protected override void OnBubbleEvent(RadElement sender, RoutedEventArgs args)
        {
            base.OnBubbleEvent(sender, args);

            if (args.RoutedEvent == RadElement.MouseClickedEvent &&
                sender == this.dialogButton)
            {
                this.OnDialogButtonClick(sender, args.OriginalEventArgs);
            }
        }

        public void ItemClicked(RadItem item)
        {
        }

        protected virtual void OnDialogButtonClick(object sender, EventArgs e)
        {
            if (this.DialogButtonClick != null)
            {
                this.DialogButtonClick(sender, e);
            }
        }

        private void ItemChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            this.invalidateCollapsableChildrenCollection = true;

            if (operation != ItemsChangeOperation.Inserted &&
                operation != ItemsChangeOperation.Set)
            {
                return;
            }

            if  (target.GetType() == typeof(RadButtonElement))
            {
                RadButtonElement buttonElement = target as RadButtonElement;

                if (string.IsNullOrEmpty(buttonElement.Class))
                {
                    buttonElement.Class = "RibbonBarButtonElement";
                }

                buttonElement.ButtonFillElement.Class = "ButtonInRibbonFill";
                buttonElement.BorderElement.Class = "ButtonInRibbonBorder";
            }
            else if (target is RadRibbonBarButtonGroup)
            {
                RadRibbonBarButtonGroup buttonGroup = (RadRibbonBarButtonGroup)target;
                buttonGroup.MinSize = new Size(22, 22);
            }
            else if (target is RadComboBoxElement || target is RadDropDownListElement)
            {
                target.MinSize = new Size(140, 0);
                target.StretchVertically = false;
                target.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
            }
            else if (target.GetType() == typeof(RadRadioButtonElement))
            {
                target.MinSize = new Size(20, 0);
                target.StretchVertically = false;
                target.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
            }
            else if (target is RadTextBoxElement)
            {
                target.MinSize = new Size(140, 0);
                target.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;

                // The border size is more correct in without horizontal padding
                Padding temp = target.Padding;
                temp.Left = 0;
                temp.Right = 0;
                target.Padding = temp;
            }
            else if (target is RadGalleryElement)
            {
                target.Padding = new Padding(2, 2, 0, 0);
            }
            else if (target.GetType() == typeof(RadCheckBoxElement))
            {
                target.StretchVertically = false;
            }
            //Georgi: TODO: Why SplitButton needs special attention and DropDownButton no???
            //else if (target is RadSplitButtonElement)
            //{
            //    target.StretchHorizontally = false;
            //    target.StretchVertically = false;
            //    target.Children[0].Visibility = ElementVisibility.Hidden;
            //}

            target.NotifyParentOnMouseInput = true;
        }

        protected internal override bool? ShouldSerializeProperty(PropertyDescriptor property)
        {
            if (property.Name == "Margin")
            {
                return this.Margin != defaultMargin;
            }

            if (property.Name == "MinSize")
            {
                return this.MinSize != defaultMinSize;
            }

            if (property.Name == "MaxSize")
            {
                return this.MaxSize != defaultMaxSize;
            }

            return base.ShouldSerializeProperty(property);
        }

        /// <summary>
        /// Overrides object ToString() method. Returns the value of the Text property
        /// prefixed with the "chunk:" string.
        /// </summary>
        public override string ToString()
        {
            return "RibbonBarGroup: " + this.Text;
        }

        #region ICollapsibleElement Members

        internal ChunkVisibilityState VisibilityState
        {
            get
            {
                return (ChunkVisibilityState)this.CollapseStep;
            }
        }

        #region ImageRelated

        private bool ChangeImages(RadButtonElement button, bool display)
        {
            if ((button.SmallImage == null) &&
                (button.SmallImageIndex == -1) &&
                (button.SmallImageKey == string.Empty))
            {
                return false;
            }

            if (!display)
            {
                button.UseSmallImageList = true;
                this.PreserveOldImage(button);

                if (button.SmallImage != null)
                {
                    button.Image = button.SmallImage;
                }
                else if (button.SmallImageIndex != -1)
                {
                    button.ImageIndex = button.SmallImageIndex;
                }
                else if (button.SmallImageKey != string.Empty)
                {
                    button.ImageKey = button.SmallImageKey;
                }
            }
            else
            {
                button.UseSmallImageList = false;
                this.RestoreOldImage(button);
            }

            return true;
        }

        private void PreserveOldImage(RadButtonElement button)
        {
            if (button.Image != null)
            {
                button.SetValue(RadButtonElement.LargeImageProperty, button.Image);
            }
            else if (button.ImageIndex != -1)
            {
                button.SetValue(RadButtonElement.LargeImageIndexProperty, button.ImageIndex);
            }
            else if (button.ImageKey == string.Empty)
            {
                button.SetValue(RadButtonElement.LargeImageKeyProperty, button.ImageKey);
            }
        }

        private void RestoreOldImage(RadButtonElement button)
        {
            if (button.LargeImage != null)
            {
                button.Image = button.LargeImage;
                button.SetValue(RadButtonElement.LargeImageProperty, null);
            }
            else if (button.LargeImageIndex != -1)
            {
                button.ImageIndex = button.LargeImageIndex;
                button.SetValue(RadButtonElement.LargeImageIndexProperty, -1);
            }
            else if (button.LargeImageKey == string.Empty)
            {
                button.ImageKey = button.LargeImageKey;
                button.SetValue(RadButtonElement.LargeImageKeyProperty, string.Empty);
            }
        }

        #endregion

        /// <summary>Expands the chunk.</summary>
        override public bool ExpandElementToStep(int collapseStep)
        {
            bool result = false;
            if (!this.CanCollapseOrExpandElement(ChunkVisibilityState.Expanded))
            {
                return result;
            }

            this.InvalidateIfNeeded();

            if (this.CollapseStep == (int)ChunkVisibilityState.Collapsed)
            {
                this.ExpandChunkFromDropDown();
                --this.CollapseStep;
                result = true;
                this.CollapseCollection((int)ChunkVisibilityState.NoText);
            }
            else
            {
                result = this.ExpandCollection(collapseStep);
            }

            return result;
        }

        private bool CanCollapseOrExpandElement(ChunkVisibilityState state)
        {
            if (this.IsDesignMode)
            {
                return false;
            }

            return true;
        }

        /// <summary>Collapses the chunk.</summary>
        public override bool CollapseElementToStep(int nextStep)
        {
            if (!this.CanCollapseOrExpandElement(ChunkVisibilityState.Collapsed))
            {
                return false;
            }

            this.InvalidateIfNeeded();

            bool result = false;

            if (nextStep == (int)ChunkVisibilityState.Collapsed)
            {
                this.CollapseChunkToDropDown();
                this.CollapseStep = nextStep;
                result = true;
            }
            else
            {
                result = this.CollapseCollection(nextStep);
            }

            return result;
        }

        private void CollapseChunkToDropDown()
        {
            this.ExpandElementToStep((int)ChunkVisibilityState.Expanded);

            ExpandableStackLayout.InvalidateAll(this);

            SizeF thisSize = this.DesiredSize;

            this.elementWithCaptionLayoutPanel.SuspendThemeRefresh();
            this.Children.Remove(this.elementWithCaptionLayoutPanel);
            this.elementWithCaptionLayoutPanel.ResumeThemeRefresh();

            RadItem item = new RadItem();
            item.UseNewLayoutSystem = true;
            item.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
            item.Children.Add(this.elementWithCaptionLayoutPanel);
            item.MaxSize = new Size(0, thisSize.ToSize().Height);
            item.MinSize = thisSize.ToSize();
            RadDropDownMenu menu = this.dropDownElement.DropDownMenu;            
            //remove left column from Menu
            RadDropDownMenuElement menuPopup = (RadDropDownMenuElement)menu.PopupElement;
            StackLayoutPanel menuStack = menuPopup.LayoutPanel as StackLayoutPanel;
            Debug.Assert( menuStack!=null, "Chunk menu stack is null");
            menuStack.Orientation = this.Orientation;
            menuPopup.Layout.LeftColumnMinWidth = 0;
            this.dropDownElement.Items.Add(item);           
            //fix for Missing theme if chunk is not expanded
            this.dropDownElement.Visibility = ElementVisibility.Visible;
            this.CollapseStep = (int)ChunkVisibilityState.Collapsed;
            menu.SetTheme();            
            menu.MouseUp += new MouseEventHandler(this.menu_MouseUp);
        }

        private void menu_MouseUp(object sender, MouseEventArgs e)
        {
            RadDropDownMenu menu = (RadDropDownMenu)sender;

            RadElement clickedButton = menu.ElementTree.GetElementAtPoint(e.Location);
            if (clickedButton == null)
            {
                return;
            }

            if (clickedButton is RadComboBoxElement ||
                clickedButton is RadSplitButtonElement ||
                clickedButton is RadDropDownButtonElement ||
                clickedButton is RadArrowButtonElement ||
                clickedButton.Class == "GalleryPopupButtonButton"||
                clickedButton.Class == "GalleryDownButton"||
                clickedButton.Class == "GalleryUpButton" ||
                clickedButton is RadGalleryElement)
            {
                return;
            }

            menu.ClosePopup(RadPopupCloseReason.Mouse);
        }

        private void ExpandChunkFromDropDown()
        {
            this.dropDownElement.Visibility = ElementVisibility.Collapsed;
            this.dropDownElement.Items.Clear();
            this.elementWithCaptionLayoutPanel.UseNewLayoutSystem = true;
            this.Children.Add(this.elementWithCaptionLayoutPanel);
        }

        public override bool CanExpandToStep(int nextStep)
        {
            if (nextStep >= this.CollapseStep)
            {
                return false;
            }

            if (nextStep == (int)ChunkVisibilityState.Collapsed)
            {
                return false;
            }

            if (this.CollapseStep == (int)ChunkVisibilityState.Collapsed)
            {
                return true;
            }

            this.InvalidateIfNeeded();
            for (int i = 0; i < this.collapsableChildren.Count; ++i)
            {
                bool canExpand = this.collapsableChildren[i].CanExpandToStep(nextStep);
                if (canExpand)
                {
                    return true;
                }
            }

            return false;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool CanCollapseToStep(int nextStep)
        {
            if (!this.AllowCollapsed)
            {
                return false;
            }

            if (nextStep <= this.CollapseStep)
            {
                return false;
            }

            if (nextStep == (int)ChunkVisibilityState.Collapsed)
            {
                return true;
            }

            this.InvalidateIfNeeded();

            for (int i = 0; i < this.collapsableChildren.Count; ++i)
            {
                bool canCollapse = this.collapsableChildren[i].CanCollapseToStep(nextStep);
                if (canCollapse)
                {
                    return true;
                }
            }

            return false;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override int CollapseMaxSteps
        {
            get
            {
                return 4;
            }
        }

        #endregion
    }
}

