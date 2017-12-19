using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using Telerik.WinControls.Themes.ControlDefault;
using System.Collections;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    public class RadPageViewStripElement : RadPageViewElement
    {
        #region Fields

        private StripViewItemContainer itemContainer;
        private RadPageViewStripNewItem newItem;

        #endregion

        #region Constructor/Initializers

        static RadPageViewStripElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new StripViewElementStateManager(), typeof(RadPageViewStripElement));
            new ControlDefault_RadPageView_Telerik_WinControls_UI_RadPageViewStripElement().DeserializeTheme();
        }

        public RadPageViewStripElement()
        {
        }

        protected override void CreateChildElements()
        {
            this.itemContainer = new StripViewItemContainer();
            this.Children.Add(this.itemContainer);

            this.newItem = new RadPageViewStripNewItem();
            this.newItem.Owner = this;

            base.CreateChildElements();
        }

        #endregion

        #region RadProperties

        public static RadProperty NewItemVisibilityProperty = RadProperty.Register(
            "NewItemVisibility",
            typeof(StripViewNewItemVisibility),
            typeof(RadPageViewStripElement),
            new RadElementPropertyMetadata(StripViewNewItemVisibility.Hidden, 
                ElementPropertyOptions.AffectsMeasure | 
                ElementPropertyOptions.AffectsDisplay));

        public static RadProperty AnimatedStripScrollingProperty = RadProperty.Register(
            "AnimatedStripScrolling",
            typeof(bool),
            typeof(RadPageViewStripElement),
            new RadElementPropertyMetadata(true, ElementPropertyOptions.None));

        public static RadProperty StripScrollingAnimationProperty = RadProperty.Register(
            "StripScrollingAnimation",
            typeof(RadEasingType),
            typeof(RadPageViewStripElement),
            new RadElementPropertyMetadata(RadEasingType.InOutQuad, ElementPropertyOptions.None));

        public static RadProperty StripButtonsProperty = RadProperty.Register(
            "StripButtons",
            typeof(StripViewButtons),
            typeof(RadPageViewStripElement),
            new RadElementPropertyMetadata(StripViewButtons.Auto,
                ElementPropertyOptions.AffectsMeasure |
                ElementPropertyOptions.AffectsDisplay |
                ElementPropertyOptions.CanInheritValue));

        public static RadProperty StripAlignmentProperty = RadProperty.Register(
            "StripAlignment",
            typeof(StripViewAlignment),
            typeof(RadPageViewStripElement),
            new RadElementPropertyMetadata(StripViewAlignment.Top,
                ElementPropertyOptions.AffectsMeasure |
                ElementPropertyOptions.AffectsDisplay |
                ElementPropertyOptions.CanInheritValue));

        public static RadProperty ItemFitModeProperty = RadProperty.Register(
           "ItemFitMode",
           typeof(StripViewItemFitMode),
           typeof(RadPageViewStripElement),
           new RadElementPropertyMetadata(StripViewItemFitMode.None,
               ElementPropertyOptions.AffectsMeasure |
               ElementPropertyOptions.AffectsDisplay |
               ElementPropertyOptions.CanInheritValue));

        public static RadProperty ItemAlignmentProperty = RadProperty.Register(
           "ItemAlignment",
           typeof(StripViewItemAlignment),
           typeof(RadPageViewStripElement),
           new RadElementPropertyMetadata(StripViewItemAlignment.Near,
               ElementPropertyOptions.AffectsMeasure |
               ElementPropertyOptions.AffectsDisplay |
               ElementPropertyOptions.CanInheritValue));

        #endregion

        #region CLR Properties

        internal override PageViewLayoutInfo ItemLayoutInfo
        {
            get
            {
                return this.itemContainer.ItemLayout.LayoutInfo;
            }
        }

        protected override RadElement ItemsParent
        {
            get
            {
                return this.itemContainer.ItemLayout;
            }
        }

        /// <summary>
        /// Gets the <see cref="RadPageViewStripItem">RadPageViewStripItem</see> that represents the NewItem functionality.
        /// </summary>
        [Browsable(false)]
        public RadPageViewStripItem NewItem
        {
            get
            {
                return this.newItem;
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the internal NewItem.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets the visibility of the internal NewItem.")]
        public StripViewNewItemVisibility NewItemVisibility
        {
            get
            {
                return (StripViewNewItemVisibility)this.GetValue(NewItemVisibilityProperty);
            }
            set
            {
                this.SetValue(NewItemVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Determines whether strip scrolling will be animated.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Determines whether strip scrolling will be animated.")]
        [RadPropertyDefaultValue("AnimatedStripScrolling", typeof(RadPageViewStripElement))]
        public bool AnimatedStripScrolling
        {
            get
            {
                return (bool)this.GetValue(AnimatedStripScrollingProperty);
            }
            set
            {
                this.SetValue(AnimatedStripScrollingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the easing type of the strip scroll animation.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets the easing type of the strip scroll animation.")]
        [RadPropertyDefaultValue("StripScrollingAnimation", typeof(RadPageViewStripElement))]
        public RadEasingType StripScrollingAnimation
        {
            get
            {
                return (RadEasingType)this.GetValue(StripScrollingAnimationProperty);
            }
            set
            {
                this.SetValue(StripScrollingAnimationProperty, value);
            }
        }

        /// <summary>
        /// Gets the container that holds item layout and strip buttons panel.
        /// </summary>
        [Browsable(false)]
        public StripViewItemContainer ItemContainer
        {
            get
            {
                return this.itemContainer;
            }
        }

        /// <summary>
        /// Determines scroll buttons' visibility.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Determines scroll buttons' visibility.")]
        [RadPropertyDefaultValue("StripButtons", typeof(RadPageViewStripElement))]
        public StripViewButtons StripButtons
        {
            get
            {
                return (StripViewButtons)this.GetValue(StripButtonsProperty);
            }
            set
            {
                this.SetValue(StripButtonsProperty, value);
            }
        }

        /// <summary>
        /// Determines the alignment of items within the strip layout.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Determines the alignment of items within the strip layout.")]
        [RadPropertyDefaultValue("ItemAlignment", typeof(RadPageViewStripElement))]
        public StripViewItemAlignment ItemAlignment
        {
            get
            {
                return (StripViewItemAlignment)this.GetValue(ItemAlignmentProperty);
            }
            set
            {
                this.SetValue(ItemAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Determines the fit mode to be applied when measuring child items.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Determines the fit mode to be applied when measuring child items.")]
        [RadPropertyDefaultValue("ItemFitMode", typeof(RadPageViewStripElement))]
        public StripViewItemFitMode ItemFitMode
        {
            get
            {
                return (StripViewItemFitMode)this.GetValue(ItemFitModeProperty);
            }
            set
            {
                this.SetValue(ItemFitModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the alignment of item strip within the view.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the alignment of item strip within the view.")]
        [RadPropertyDefaultValue("StripAlignment", typeof(RadPageViewStripElement))]
        public StripViewAlignment StripAlignment
        {
            get
            {
                return (StripViewAlignment)this.GetValue(StripAlignmentProperty);
            }
            set
            {
                this.SetValue(StripAlignmentProperty, value);
            }
        }

        #endregion

        #region Overrides

        public override RectangleF GetItemsRect()
        {
            return this.itemContainer.ItemLayout.ControlBoundingRectangle;
        }

        protected override RadPageViewItem CreateItem()
        {
            return new RadPageViewStripItem();
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == StripAlignmentProperty)
            {
                this.UpdateItemOrientation(this.Items);
                this.UpdateItemContainer((StripViewAlignment)e.NewValue);

                if (this.SelectedItem != null && this.EnsureSelectedItemVisible)
                {
                    this.EnsureItemVisible(this.SelectedItem);
                }
            }
            else if (e.Property == StripScrollingAnimationProperty)
            {
                this.itemContainer.ItemLayout.SetScrollAnimation((RadEasingType)e.NewValue);
            }
            else if (e.Property == AnimatedStripScrollingProperty)
            {
                this.itemContainer.ItemLayout.EnableScrolling((bool)e.NewValue);
            }
            else if (e.Property == StripButtonsProperty)
            {
                StripViewButtons buttons = (StripViewButtons)e.NewValue;
                if(buttons == StripViewButtons.None)
                {
                    this.itemContainer.ButtonsPanel.Visibility = ElementVisibility.Collapsed;
                }
                else
                {
                    this.itemContainer.ButtonsPanel.Visibility = ElementVisibility.Visible;
                }
            }
            else if(e.Property == NewItemVisibilityProperty)
            {
                this.UpdateNewItem();
            }

            if (PropertyInvalidatesScrollOffset(e.Property) && 
                this.EnsureSelectedItemVisible &&
                this.SelectedItem != null)
            {
                this.itemContainer.ItemLayout.EnsureVisible(this.SelectedItem);
            }
        }

        protected internal override PageViewContentOrientation GetAutomaticItemOrientation(bool content)
        {
            switch (this.StripAlignment)
            {
                case StripViewAlignment.Left:
                    return PageViewContentOrientation.Vertical270;
                case StripViewAlignment.Right:
                    return PageViewContentOrientation.Vertical90;
                case StripViewAlignment.Bottom:
                    return content ? PageViewContentOrientation.Horizontal : PageViewContentOrientation.Horizontal180;
                default:
                    return PageViewContentOrientation.Horizontal;
            }
        }

        protected override bool EnsureItemVisibleCore(RadPageViewItem item)
        {
            return this.itemContainer.ItemLayout.EnsureVisible(item);
        }

        protected internal override bool IsNextKey(Keys key)
        {
            StripViewAlignment align = this.StripAlignment;
            switch(align)
            {
                case StripViewAlignment.Left:
                case StripViewAlignment.Right:
                    return key == Keys.Down;
            }

            return base.IsNextKey(key);
        }

        protected internal override bool IsPreviousKey(Keys key)
        {
            StripViewAlignment align = this.StripAlignment;
            switch (align)
            {
                case StripViewAlignment.Left:
                case StripViewAlignment.Right:
                    return key == Keys.Up;
            }

            return base.IsPreviousKey(key);
        }

        protected internal override void StartItemDrag(RadPageViewItem item)
        {
            base.StartItemDrag(item);

            this.itemContainer.ItemLayout.EnableScrolling(false);
        }

        protected internal override void EndItemDrag(RadPageViewItem item)
        {
            base.EndItemDrag(item);

            this.itemContainer.ItemLayout.EnableScrolling(this.AnimatedStripScrolling);
        }

        protected override void UpdateItemOrientation(IEnumerable items)
        {
            base.UpdateItemOrientation(items);
            base.UpdateItemOrientation(new RadPageViewItem[] { this.newItem });
        }

        protected override void AddItemCore(RadPageViewItem item)
        {
            base.AddItemCore(item);

            this.ItemsParent.SuspendLayout();

            if (this.newItem.Parent == this.ItemsParent)
            {
                this.ItemsParent.Children.Remove(this.newItem);
            }

            switch(this.NewItemVisibility)
            {
                case StripViewNewItemVisibility.End:
                    //send the NewItem to the end
                    this.ItemsParent.Children.Add(this.newItem);
                    break;
                case StripViewNewItemVisibility.Front:
                    //bring the NewItem to front
                    this.ItemsParent.Children.Insert(0, this.newItem);
                    break;
            }

            this.ItemsParent.ResumeLayout(false);
        }

        protected internal override void CloseItem(RadPageViewItem item)
        {
            if (item == this.newItem)
            {
                return;
            }

            base.CloseItem(item);
        }

        protected override bool CanDropOverItem(RadPageViewItem dragItem, RadPageViewItem hitItem)
        {
            if (hitItem == this.newItem)
            {
                return false;
            }

            return base.CanDropOverItem(dragItem, hitItem);
        }

        protected override void DisposeManagedResources()
        {
            this.newItem.Dispose();

            base.DisposeManagedResources();
        }

        #endregion

        #region NewItem

        private void UpdateNewItem()
        {
            if (this.newItem.Parent == this.ItemsParent)
            {
                this.ItemsParent.Children.Remove(this.newItem);
            }

            switch(this.NewItemVisibility)
            {
                case StripViewNewItemVisibility.End:
                    this.ItemsParent.Children.Add(this.newItem);
                    break;
                case StripViewNewItemVisibility.Front:
                    this.ItemsParent.Children.Insert(0, this.newItem);
                    break;
            }
        }

        protected virtual void OnNewItemRequested()
        {
            if (this.Owner != null)
            {
                this.Owner.OnNewPageRequested(EventArgs.Empty);
            }
        }

        protected internal override void OnItemMouseDown(RadPageViewItem sender, MouseEventArgs e)
        {
            if (sender == this.newItem)
            {
                return;
            }

            base.OnItemMouseDown(sender, e);
        }

        protected internal override void OnItemClick(RadPageViewItem sender, EventArgs e)
        {
            base.OnItemClick(sender, e);

            if (sender == this.newItem)
            {
                this.OnNewItemRequested();
            }
        }

        #endregion

        #region Layout

        public StripViewButtons HitTestButtons(Point controlClient)
        {
            foreach (RadPageViewButtonElement button in this.itemContainer.ButtonsPanel.Children)
            {
                if (button.ControlBoundingRectangle.Contains(controlClient))
                {
                    return (StripViewButtons)button.Tag;
                }
            }

            return StripViewButtons.Auto;
        }

        protected override SizeF MeasureItems(SizeF availableSize)
        {
            SizeF elementAvailableSize = availableSize;
            SizeF desiredSize = SizeF.Empty; 

            this.itemContainer.Measure(elementAvailableSize);

            if (this.StripAlignment == StripViewAlignment.Top || this.StripAlignment == StripViewAlignment.Bottom)
            {
                desiredSize.Height += this.itemContainer.DesiredSize.Height;
                elementAvailableSize.Height -= this.itemContainer.DesiredSize.Height;

                if (this.ContentArea.Visibility != ElementVisibility.Collapsed)
                {
                    this.ContentArea.Measure(elementAvailableSize);
                }
                desiredSize.Height += this.ContentArea.DesiredSize.Height;
                desiredSize.Width += this.ContentArea.DesiredSize.Width;
            }
            else
            {
                desiredSize.Width += this.itemContainer.DesiredSize.Width;
                elementAvailableSize.Width -= this.itemContainer.DesiredSize.Width;

                if (this.ContentArea.Visibility != ElementVisibility.Collapsed)
                {
                    this.ContentArea.Measure(elementAvailableSize);
                }
                desiredSize.Width += this.ContentArea.DesiredSize.Width;
                desiredSize.Height += this.ContentArea.DesiredSize.Height;
            }

            if (StretchHorizontally)
            {
                desiredSize.Width = availableSize.Width;
            }
            if (StretchVertically)
            {
                desiredSize.Height = availableSize.Height;
            }

            desiredSize.Width = Math.Min(desiredSize.Width, availableSize.Width);
            desiredSize.Height = Math.Min(desiredSize.Height, availableSize.Height);

            return desiredSize;
        }

        protected override RectangleF ArrangeItems(RectangleF itemsRect)
        {
            RectangleF contentRect = itemsRect;

            if (this.itemContainer.Visibility != ElementVisibility.Collapsed)
            {
                SizeF stripSize = this.itemContainer.DesiredSize;
                switch (this.StripAlignment)
                {
                    case StripViewAlignment.Left:
                        contentRect = this.ArrangeLeft(itemsRect, stripSize);
                        break;
                    case StripViewAlignment.Right:
                        contentRect = this.ArrangeRight(itemsRect, stripSize);
                        break;
                    case StripViewAlignment.Bottom:
                        contentRect = this.ArrangeBottom(itemsRect, stripSize);
                        break;
                    default:
                        contentRect = this.ArrangeTop(itemsRect, stripSize);
                        break;
                }
            }

            return contentRect;
        }

        private RectangleF ArrangeLeft(RectangleF client, SizeF stripSize)
        {
            Padding stripMargin = this.itemContainer.Margin;
            RectangleF stripRect = new RectangleF(
                client.X + stripMargin.Left,
                client.Y + stripMargin.Top,
                stripSize.Width,
                stripSize.Height - stripMargin.Vertical);
            this.itemContainer.Arrange(stripRect);

            return new RectangleF(
                stripRect.Right + stripMargin.Right,
                client.Y,
                client.Width - stripRect.Width - stripMargin.Horizontal,
                client.Height);
        }

        private RectangleF ArrangeTop(RectangleF client, SizeF stripSize)
        {
            Padding stripMargin = this.itemContainer.Margin;
            RectangleF stripRect = new RectangleF(
                client.X + stripMargin.Left, 
                client.Y + stripMargin.Top, 
                stripSize.Width - stripMargin.Horizontal, 
                stripSize.Height);
            this.itemContainer.Arrange(stripRect);

            return new RectangleF(
                client.X, 
                stripRect.Bottom + stripMargin.Vertical,
                client.Width, 
                client.Height - stripRect.Height - stripMargin.Vertical);
        }

        private RectangleF ArrangeRight(RectangleF client, SizeF stripSize)
        {
            Padding stripMargin = this.itemContainer.Margin;
            RectangleF stripRect = new RectangleF(
                client.Right - stripMargin.Right - stripSize.Width,
                client.Y + stripMargin.Top,
                stripSize.Width,
                stripSize.Height - stripMargin.Vertical);
            this.itemContainer.Arrange(stripRect);

            return new RectangleF(
                client.X,
                client.Y,
                client.Width - stripRect.Width - stripMargin.Horizontal,
                client.Height);
        }

        private RectangleF ArrangeBottom(RectangleF client, SizeF stripSize)
        {
            Padding stripMargin = this.itemContainer.Margin;
            RectangleF stripRect = new RectangleF(
                client.X + stripMargin.Left,
                client.Bottom - stripMargin.Bottom - stripSize.Height,
                stripSize.Width - stripMargin.Horizontal,
                stripSize.Height);
            this.itemContainer.Arrange(stripRect);

            return new RectangleF(
                client.X,
                client.Y,
                client.Width,
                client.Height - stripRect.Height - stripMargin.Vertical);
        }

        private void UpdateItemContainer(StripViewAlignment align)
        {
            switch(align)
            {
                case StripViewAlignment.Left:
                case StripViewAlignment.Right:
                    this.itemContainer.SetDefaultValueOverride(StretchHorizontallyProperty, false);
                    this.itemContainer.SetDefaultValueOverride(StretchVerticallyProperty, true);
                    break;
                case StripViewAlignment.Top:
                case StripViewAlignment.Bottom:
                    this.itemContainer.SetDefaultValueOverride(StretchVerticallyProperty, false);
                    this.itemContainer.SetDefaultValueOverride(StretchHorizontallyProperty, true);
                    break;
            }

            this.itemContainer.ButtonsPanel.SetContentOrientation(this.GetAutomaticItemOrientation(true), true);
        }

        internal static bool PropertyInvalidatesScrollOffset(RadProperty property)
        {
            return (property == RadPageViewStripElement.StripAlignmentProperty ||
                property == RadPageViewStripElement.ItemFitModeProperty ||
                property == RadPageViewStripElement.ItemAlignmentProperty ||
                property == RadPageViewElement.ItemSizeModeProperty ||
                property == RadPageViewElement.ItemContentOrientationProperty);
        }

        #endregion
    }
}
