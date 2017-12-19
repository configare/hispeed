using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Layouts;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using Telerik.WinControls.Paint;
using System.Drawing.Drawing2D;

namespace Telerik.WinControls.UI
{
    public abstract class RadPageViewItem : RadPageViewElementBase
    {
        #region Fields

        private SizeF forcedLayoutSize;
        private SizeF currentSize;
        private Point dragStart;
        private RadPageViewItemButtonsPanel buttonsPanel;
        private bool isSystemItem;

        //references
        private RadPageViewPage page;
        private RadPageViewElement owner;
        private RadElement content;

        #endregion

        #region Constructor/Initialization

        static RadPageViewItem()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RadPageViewItemStateManager(), typeof(RadPageViewItem));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.AllowDrag = true;

            this.MinSize = new Size(8, 8);
            this.Padding = new Padding(4);
            this.ImageAlignment = ContentAlignment.MiddleLeft;
            this.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.TextAlignment = ContentAlignment.MiddleLeft;
            this.AutoEllipsis = true;
            this.ClipText = true;
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.buttonsPanel = new RadPageViewItemButtonsPanel(this);
            this.Children.Add(this.buttonsPanel);
        }

        protected internal override bool? ShouldSerializeProperty(PropertyDescriptor property)
        {
            if (property.Name == "AutoEllipsis")
            {
                return !this.AutoEllipsis;
            }

            return base.ShouldSerializeProperty(property);
        }

        protected override void DisposeManagedResources()
        {
            if (this.page != null)
            {
                this.Detach();
            }
            this.owner = null;

            base.DisposeManagedResources();
        }

        public virtual void Attach(RadPageViewPage page)
        {
            //cache references
            this.page = page;

            if (this.page != null)
            {
                this.page.Item = this;

                //update visual settings from page
                this.Text = this.page.Text;
                this.Image = this.page.Image;
                this.Title = this.page.Title;
                this.Description = this.page.Description;
                this.ImageAlignment = this.page.ImageAlignment;
                this.TextAlignment = this.page.TextAlignment;
                this.TextImageRelation = this.page.TextImageRelation;
                this.ToolTipText = this.page.ToolTipText;
                this.Enabled = this.page.Enabled;
                this.PageLength = this.page.PageLength;
                this.IsContentVisible = this.page.IsContentVisible;
            }
        }

        public virtual void Detach()
        {
            if (this.page != null)
            {
                this.page.Item = null;
                this.page = null;
            }
        }

        public override string ToString()
        {
            return this.GetType().Name + "; Text: " + this.Text;
        }

        #endregion

        #region Rad Properties

        public static RadProperty ButtonsAlignmentProperty = RadProperty.Register(
            "ButtonsAlignment",
            typeof(PageViewItemButtonsAlignment),
            typeof(RadPageViewItem),
            new RadElementPropertyMetadata(PageViewItemButtonsAlignment.ContentBeforeButtons,
                ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsLayout));

        public static RadProperty AutoFlipMarginProperty = RadProperty.Register(
            "AutoFlipMargin",
            typeof(bool),
            typeof(RadPageViewItem),
            new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsMeasure));

        public static RadProperty IsSelectedProperty = RadProperty.Register(
           "IsSelected",
           typeof(bool),
           typeof(RadPageViewItem),
           new RadElementPropertyMetadata(false, ElementPropertyOptions.None));

        public static RadProperty TitleProperty = RadProperty.Register(
           "Title",
           typeof(string),
           typeof(RadPageViewItem),
           new RadElementPropertyMetadata(string.Empty, ElementPropertyOptions.None));

        public static RadProperty DescriptionProperty = RadProperty.Register(
           "Description",
           typeof(string),
           typeof(RadPageViewItem),
           new RadElementPropertyMetadata(string.Empty, ElementPropertyOptions.None));


        #endregion

        #region CLR Properties

        /// <summary>
        /// Gets or sets the length of the <see cref="RadPageViewPage"/> associated
        /// with this <see cref="RadPageViewItem"/>. By default, this property returns -1;
        /// </summary>
        internal virtual int PageLength
        {
            get
            {
                return -1;
            }
            set
            {
            }
        }

        /// <summary>
        /// Determines whether the content of the current item is visible. This property is equivalent
        /// to the IsSelected property, however its semantics can be changed in scenarios where multiple
        /// content areas can be visible as in the <see cref="RadPageViewExplorerBarElement"/>.
        /// </summary>
        internal virtual bool IsContentVisible
        {
            get
            {
                return this.IsSelected;
            }
            set
            {
            }
        }

        /// <summary>
        /// Determines whether the current instance is internally created by the ViewElement and represents some built-in functionality.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSystemItem
        {
            get
            {
                return this.isSystemItem;
            }
            internal set
            {
                this.isSystemItem = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="RadPageViewItemButtonsPanel">RadPageViewItemButtonsPanel</see> that contains all the buttons, associated with the item.
        /// </summary>
        [Browsable(false)]
        public RadPageViewItemButtonsPanel ButtonsPanel
        {
            get
            {
                return this.buttonsPanel;
            }
        }

        /// <summary>
        /// Gets or sets the alignment of item's associated buttons.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the alignment of item's associated buttons.")]
        public PageViewItemButtonsAlignment ButtonsAlignment
        {
            get
            {
                return (PageViewItemButtonsAlignment)this.GetValue(ButtonsAlignmentProperty);
            }
            set
            {
                this.SetValue(ButtonsAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that determines whether the item margin will be automatically
        /// flipped according to the orientation of the items in the <see cref="RadPageView"/> control.
        /// </summary>
        [Description("Gets or sets a boolean value that determines whether the item margin will be automatically"
            + " flipped according to the orientation of the items in the control.")]
        [DefaultValue(true)]
        public bool AutoFlipMargin
        {
            get
            {
                return (bool)this.GetValue(AutoFlipMarginProperty);
            }
            set
            {
                this.SetValue(AutoFlipMarginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the title of the item. Title is visualized in the Header area of the owning view element.
        /// </summary>
        [Description("Gets or sets the title of the item. Title is visualized in the Header area of the owning view element.")]
        public string Title
        {
            get
            {
                string title = (string)this.GetValue(TitleProperty);
                if (string.IsNullOrEmpty(title))
                {
                    return this.Text;
                }

                return title;
            }
            set
            {
                this.SetValue(TitleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the description of the item. Description is visualized in the Footer area of the owning view element.
        /// </summary>
        [Description("Gets or sets the description of the item. Description is visualized in the Footer area of the owning view element.")]
        public string Description
        {
            get
            {
                string description = (string)this.GetValue(DescriptionProperty);
                if (string.IsNullOrEmpty(description))
                {
                    return this.Text;
                }

                return description;
            }
            set
            {
                this.SetValue(DescriptionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the RadElement instance that represents the content of this item.
        /// The content is used when item is not bound to a RadPageViewPage instance.
        /// </summary>
        [DefaultValue(null)]
        [Description("Gets or sets the RadElement instance that represents the content of this item. The content is used when item is not bound to a RadPageViewPage instance.")]
        public RadElement Content
        {
            get
            {
                return this.content;
            }
            set
            {
                if (this.content == value)
                {
                    return;
                }

                if (!this.OnContentChanging(value))
                {
                    return;
                }

                this.SetContentCore(value);
            }
        }

        /// <summary>
        /// Gets the size that is forced by the layout element for this item. It may differ from the DesiredSize one.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SizeF ForcedLayoutSize
        {
            get
            {
                if (this.forcedLayoutSize == SizeF.Empty)
                {
                    return this.DesiredSize;
                }

                return this.forcedLayoutSize;
            }
            internal set
            {
                this.forcedLayoutSize = value;
            }
        }

        /// <summary>
        /// Gets the current size of the item. This may differ from Bounds.Size as it reflects internal changes within the item itself.
        /// </summary>
        [Browsable(false)]
        public SizeF CurrentSize
        {
            get
            {
                return this.currentSize;
            }
        }

        /// <summary>
        /// Determines whether the item is currently selected (associated with the SelectedPage of the owning RadPageView).
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool IsSelected
        {
            get
            {
                return (bool)this.GetValue(IsSelectedProperty);
            }
            set
            {
                this.SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets the RadPageViewPage instance associated with this item.
        /// </summary>
        [Browsable(false)]
        public RadPageViewPage Page
        {
            get
            {
                return this.page;
            }
        }

        /// <summary>
        /// Gets the RadPageViewElement that owns this item.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadPageViewElement Owner
        {
            get
            {
                return this.owner;
            }
            internal set
            {
                this.owner = value;
            }
        }

        #endregion

        #region Input

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Debug.Assert(this.owner != null, "Must have an owning UI at this point.");
            this.owner.OnItemClick(this, e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            this.dragStart = e.Location;

            Debug.Assert(this.owner != null, "Must have an owning UI at this point.");
            this.owner.OnItemMouseDown(this, e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            Debug.Assert(this.owner != null, "Must have an owning UI at this point.");
            this.owner.OnItemMouseUp(this, e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Debug.Assert(this.owner != null, "Must have an owning UI at this point.");

            if (this.Capture && e.Button == this.owner.ActionMouseButton &&
                this.ControlBoundingRectangle.Contains(e.Location) &&
                RadDragDropService.ShouldBeginDrag(e.Location, this.dragStart))
            {
                this.owner.OnItemDrag(this, e);
            }
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF result = base.MeasureOverride(availableSize);

            if (availableSize == LayoutUtils.InfinitySize)
            {
                this.forcedLayoutSize = SizeF.Empty;
            }
            else
            {
                this.forcedLayoutSize = availableSize;
            }

            return result;
        }

        protected override SizeF CalculateMeasuredSize(SizeF contentSize, SizeF childSize)
        {
            if (this.buttonsPanel.DesiredSize == SizeF.Empty)
            {
                return base.CalculateMeasuredSize(contentSize, childSize);
            }

            Padding margins = this.buttonsPanel.Margin;
            SizeF measured = SizeF.Empty;

            switch (this.ButtonsAlignment)
            {
                case PageViewItemButtonsAlignment.ButtonsAboveContent:
                case PageViewItemButtonsAlignment.ContentAboveButtons:
                    measured.Width = Math.Max(contentSize.Width, childSize.Width + margins.Horizontal);
                    measured.Height = contentSize.Height + childSize.Height + margins.Vertical;
                    break;
                case PageViewItemButtonsAlignment.ButtonsBeforeContent:
                case PageViewItemButtonsAlignment.ContentBeforeButtons:
                    measured.Width = contentSize.Width + childSize.Width + margins.Horizontal;
                    measured.Height = Math.Max(contentSize.Height, childSize.Height + margins.Vertical);
                    break;
                default:
                    measured = base.CalculateMeasuredSize(contentSize, childSize);
                    break;
            }

            return measured;
        }

        protected override void ArrangeChildren(SizeF available)
        {
            SizeF desired = this.buttonsPanel.DesiredSize;
            if (desired == SizeF.Empty)
            {
                return;
            }

            RectangleF client = this.GetButtonsClientRect(available);
            float x = client.X;
            float y = client.Y;

            Padding margin = this.RotatePadding(this.buttonsPanel.Margin);
            PageViewItemButtonsAlignment alignment = this.RotateButtonsAlignment(this.ButtonsAlignment);
            if (this.RightToLeft)
            {
                alignment = this.RTLTransformButtonsAlignment(alignment);
            }

            switch (alignment)
            {
                case PageViewItemButtonsAlignment.ButtonsAboveContent:
                    x = client.X + (client.Width - desired.Width) / 2 + margin.Left;
                    y = client.Y + margin.Top;
                    break;
                case PageViewItemButtonsAlignment.ContentAboveButtons:
                    x = client.X + (client.Width - desired.Width) / 2 + margin.Left;
                    y = client.Bottom - margin.Bottom - desired.Height;
                    break;
                case PageViewItemButtonsAlignment.ButtonsBeforeContent:
                    x = client.X + margin.Left;
                    y = client.Y + margin.Top + (client.Height - desired.Height) / 2;
                    break;
                case PageViewItemButtonsAlignment.ContentBeforeButtons:
                    x = client.Right - margin.Right - desired.Width;
                    y = client.Y + margin.Top + (client.Height - desired.Height) / 2;
                    break;
            }

            this.buttonsPanel.Arrange(new RectangleF(x, y, desired.Width, desired.Height));
        }

        private RectangleF GetButtonsClientRect(SizeF available)
        {
            Padding padding = this.RotatePadding(this.Padding);
            Padding border = this.RotatePadding(this.GetBorderThickness(true));

            return new RectangleF(
                padding.Left + border.Left,
                padding.Top + border.Top,
                available.Width - padding.Horizontal - border.Horizontal,
                available.Height - padding.Vertical - border.Vertical);
        }

        #endregion

        #region Content

        protected virtual Padding RotatePadding(Padding margin)
        {
            if (this.ContentOrientation == PageViewContentOrientation.Horizontal)
            {
                return margin;
            }

            switch (this.ContentOrientation)
            {
                case PageViewContentOrientation.Horizontal180:
                    margin = LayoutUtils.RotateMargin(margin, 180);
                    break;
                case PageViewContentOrientation.Vertical90:
                    margin = LayoutUtils.RotateMargin(margin, 90);
                    break;
                case PageViewContentOrientation.Vertical270:
                    margin = LayoutUtils.RotateMargin(margin, 270);
                    break;
            }

            return margin;
        }

        protected virtual PageViewItemButtonsAlignment RTLTransformButtonsAlignment(PageViewItemButtonsAlignment alignment)
        {
            switch (alignment)
            {
                case PageViewItemButtonsAlignment.ButtonsBeforeContent:
                    alignment = PageViewItemButtonsAlignment.ContentBeforeButtons;
                    break;

                case PageViewItemButtonsAlignment.ContentBeforeButtons:
                    alignment = PageViewItemButtonsAlignment.ButtonsBeforeContent;
                    break;

                case PageViewItemButtonsAlignment.ButtonsAboveContent:
                    alignment = PageViewItemButtonsAlignment.ContentAboveButtons;
                    break;

                case PageViewItemButtonsAlignment.ContentAboveButtons:
                    alignment = PageViewItemButtonsAlignment.ButtonsAboveContent;
                    break;
            }

            return alignment;
        }

        protected virtual PageViewItemButtonsAlignment RotateButtonsAlignment(PageViewItemButtonsAlignment alignment)
        {
            if (alignment == PageViewItemButtonsAlignment.Overlay)
            {
                return alignment;
            }

            if (this.ContentOrientation == PageViewContentOrientation.Horizontal)
            {
                return alignment;
            }

            switch (this.ContentOrientation)
            {
                case PageViewContentOrientation.Horizontal180:
                    switch (alignment)
                    {
                        case PageViewItemButtonsAlignment.ButtonsAboveContent:
                            alignment = PageViewItemButtonsAlignment.ContentAboveButtons;
                            break;
                        case PageViewItemButtonsAlignment.ContentAboveButtons:
                            alignment = PageViewItemButtonsAlignment.ButtonsAboveContent;
                            break;
                        case PageViewItemButtonsAlignment.ButtonsBeforeContent:
                            alignment = PageViewItemButtonsAlignment.ContentBeforeButtons;
                            break;
                        case PageViewItemButtonsAlignment.ContentBeforeButtons:
                            alignment = PageViewItemButtonsAlignment.ButtonsBeforeContent;
                            break;
                    }
                    break;
                case PageViewContentOrientation.Vertical90:
                    switch (alignment)
                    {
                        case PageViewItemButtonsAlignment.ButtonsAboveContent:
                            alignment = PageViewItemButtonsAlignment.ContentBeforeButtons;
                            break;
                        case PageViewItemButtonsAlignment.ButtonsBeforeContent:
                            alignment = PageViewItemButtonsAlignment.ButtonsAboveContent;
                            break;
                        case PageViewItemButtonsAlignment.ContentAboveButtons:
                            alignment = PageViewItemButtonsAlignment.ButtonsBeforeContent;
                            break;
                        case PageViewItemButtonsAlignment.ContentBeforeButtons:
                            alignment = PageViewItemButtonsAlignment.ContentAboveButtons;
                            break;
                    }
                    break;
                case PageViewContentOrientation.Vertical270:
                    switch (alignment)
                    {
                        case PageViewItemButtonsAlignment.ButtonsAboveContent:
                            alignment = PageViewItemButtonsAlignment.ButtonsBeforeContent;
                            break;
                        case PageViewItemButtonsAlignment.ButtonsBeforeContent:
                            alignment = PageViewItemButtonsAlignment.ContentAboveButtons;
                            break;
                        case PageViewItemButtonsAlignment.ContentAboveButtons:
                            alignment = PageViewItemButtonsAlignment.ContentBeforeButtons;
                            break;
                        case PageViewItemButtonsAlignment.ContentBeforeButtons:
                            alignment = PageViewItemButtonsAlignment.ButtonsAboveContent;
                            break;
                    }
                    break;
            }

            return alignment;
        }

        protected internal override void SetContentOrientation(PageViewContentOrientation orientation, bool recursive)
        {
            this.currentSize = SizeF.Empty;
            this.buttonsPanel.SetContentOrientation(orientation, false);

            base.SetContentOrientation(orientation, recursive);
        }

        protected virtual void SetContentCore(RadElement value)
        {
            this.content = value;
            if (this.owner != null)
            {
                this.owner.OnItemContentChanged(this);
            }
        }

        protected virtual bool OnContentChanging(RadElement value)
        {
            if (this.owner != null)
            {
                return this.owner.OnItemContentChanging(this, value);
            }

            return true;
        }

        #endregion

        #region Overrides

        protected override void OnUnloaded(ComponentThemableElementTree oldTree)
        {
            base.OnUnloaded(oldTree);

            this.currentSize = SizeF.Empty;
            this.forcedLayoutSize = SizeF.Empty;
        }

        protected override void SetBoundsCore(Rectangle bounds)
        {
            base.SetBoundsCore(bounds);

            this.currentSize = bounds.Size;
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (page != null)
            {
                this.UpdatePage(e);
            }

            if (this.owner != null)
            {
                this.owner.OnItemPropertyChanged(this, e);
            }
        }

        #endregion

        #region Private Implementation

        private void UpdatePage(RadPropertyChangedEventArgs e)
        {
            if (e.Property == TextProperty)
            {
                this.page.Text = (string)e.NewValue;
            }
            else if (e.Property == ImageProperty)
            {
                this.page.Image = (Image)e.NewValue;
            }
            else if (e.Property == TitleProperty)
            {
                this.page.Title = (string)e.NewValue;
            }
            else if (e.Property == DescriptionProperty)
            {
                this.page.Description = (string)e.NewValue;
            }
            else if (e.Property == ImageAlignmentProperty)
            {
                this.page.ImageAlignment = (ContentAlignment)e.NewValue;
            }
            else if (e.Property == TextAlignmentProperty)
            {
                this.page.TextAlignment = (ContentAlignment)e.NewValue;
            }
            else if (e.Property == TextImageRelationProperty)
            {
                this.page.TextImageRelation = (TextImageRelation)e.NewValue;
            }
            else if (e.Property == EnabledProperty)
            {
                bool enabled = (bool)e.NewValue;
                if (page.Enabled != enabled)
                {
                    this.page.Enabled = enabled;
                }
            }
        }

        #endregion
    }
}
