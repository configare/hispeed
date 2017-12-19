using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layout;
using Telerik.WinControls.Primitives;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using System.ComponentModel;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// RadScrollLayoutPanel is the layout panel that arranges viewport, horizontal and vertical scrollbars
    /// and a spot that appears when both scrollbars are shown.
    /// </summary>
    /// <remarks>
    ///     For more information about scrolling see the help for
    ///     <see cref="RadScrollViewer">RadScrollViewer class</see> and for
    ///     <see cref="IRadScrollViewport">IRadScrollViewport interace</see>.
    /// </remarks>
    public class RadScrollLayoutPanel : LayoutPanel
    {
        #region BitState Keys

        internal const ulong UseScrollCallbackStateKey = LayoutPanelLastStateKey << 1;
        internal const ulong IsScrollingStateKey = UseScrollCallbackStateKey << 1;
        internal const ulong IsHorizScrollNeededStateKey = IsScrollingStateKey << 1;
        internal const ulong IsVertScrollNeededStateKey = IsHorizScrollNeededStateKey << 1;
        internal const ulong MeasureWithAvaibleSizeStateKey = IsVertScrollNeededStateKey << 1;
        internal const ulong UsePhysicalScrollingStateKey = MeasureWithAvaibleSizeStateKey << 1;

        internal const ulong RadScrollLayoutPanelLastStateKey = UsePhysicalScrollingStateKey;

        #endregion

        #region Fields

        //public const int InitialScrollThickness = SystemInformation.VerticalScrollBarWidth;
        private const int InitialPixelsPerLineScroll = 16;

        private RadScrollBarElement horizontalScrollBar;
        private RadScrollBarElement verticalScrollBar;
        /// <summary>
        /// The spot between the ScrollBars when both are shown
        /// </summary>
        protected FillPrimitive blankSpot;

        private Size clientSize;
        private Size viewportSize;
        private Size extentSize;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when horizontal or vertical scrolling is performed
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Occurs when the user scrolls the content of the viewport.")]
        public event RadScrollPanelHandler Scroll;

        protected virtual void OnScroll(Point oldValue, Point newValue)
        {
            if (Scroll != null)
            {
                Scroll(this, new ScrollPanelEventArgs(oldValue, newValue));
            }
        }

        /// <summary>
        /// Occurs when the need for horizontal or vertical scrollbar has changed.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Occurs when the need for horizontal or vertical scrollbar has changed.")]
        public event ScrollNeedsHandler ScrollNeedsChanged;

        protected virtual void OnScrollNeedsChanged(ScrollNeedsEventArgs args)
        {
            if (ScrollNeedsChanged != null)
            {
                ScrollNeedsChanged(this, args);
            }
        }

        /// <summary>
        /// Occurs when property that affects the scrolling functionality is changed.
        /// </summary>
        [Category(RadDesignCategory.PropertyChangedCategory)]
        [Description("Occurs when property that affects the scrolling functionality is changed.")]
        public event RadPanelScrollParametersHandler ScrollParametersChanged;

        protected virtual void OnScrollParametersChanged(RadScrollBarElement scrollBar)
        {
            if (scrollBar != null && ScrollParametersChanged != null)
            {
                ScrollParametersChanged(this, new RadPanelScrollParametersEventArgs(
                    scrollBar.ScrollType == ScrollType.Horizontal, scrollBar.GetParameters()));
            }
        }

        /// <summary>
        /// Occurs when the Viewport is changed
        /// </summary>
        [Browsable(false)]
        public event ScrollViewportSetHandler ScrollViewportSet;
        protected virtual void OnNewViewportSet(RadElement oldViewport, RadElement newViewport)
        {
            if (this.ScrollViewportSet != null)
                this.ScrollViewportSet(this, new ScrollViewportSet(oldViewport, newViewport));
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the horizontal scrollbar
        /// </summary>
        public RadScrollBarElement HorizontalScrollBar
        {
            get
            {
                return this.horizontalScrollBar;
            }
        }

        /// <summary>
        /// Gets the vertical scrollbar
        /// </summary>
        public RadScrollBarElement VerticalScrollBar
        {
            get
            {
                return this.verticalScrollBar;
            }
        }

        /// <summary>
        /// Gets the retcangle that is between the two scrollbars when they both are shown.
        /// </summary>
        public FillPrimitive BlankSpot
        {
            get
            {
                return this.blankSpot;
            }
        }

        internal int SmallHorizontalChange
        {
            get
            {
                return this.horizontalScrollBar.SmallChange;
            }
        }

        internal int SmallVerticalChange
        {
            get
            {
                return this.verticalScrollBar.SmallChange;
            }
        }

        internal int LargeHorizontalChange
        {
            get
            {
                return this.horizontalScrollBar.LargeChange;
            }
        }

        internal int LargeVerticalChange
        {
            get
            {
                return this.verticalScrollBar.LargeChange;
            }
        }

        /// <summary>
        /// Gets a value indicating whether can be performed horizontal scrolling operation
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanHorizontalScroll
        {
            get
            {
                return this.GetBitState(IsHorizScrollNeededStateKey);
            }
        }

        /// <summary>
        /// Gets a value indicating whether can be performed vertical scrolling operation
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanVerticalScroll
        {
            get
            {
                return this.GetBitState(IsVertScrollNeededStateKey);
            }
        }

        private ScrollState horizontalScrollState;
        /// <summary>Gets or sets the scroll state of the horizontal scroll bar.</summary>
        /// <value>State of type <see cref="ScrollState"/>. Default value is AutoHide.</value>
        [DefaultValue(ScrollState.AutoHide)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Determine when the horizontal scroll bar should be shown.")]
        public ScrollState HorizontalScrollState
        {
            get { return horizontalScrollState; }

            set
            {
                horizontalScrollState = value;
                ResetLayout();
            }
        }

        private ScrollState verticalScrollState;
        /// <summary>Gets or sets the scroll state of the vertical scroll bar.</summary>
        /// <value>State of type <see cref="ScrollState"/>. Default value is AutoHide.</value>
        [DefaultValue(ScrollState.AutoHide)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Determine when the vertical scroll bar should be shown.")]
        public ScrollState VerticalScrollState
        {
            get { return verticalScrollState; }

            set
            {
                verticalScrollState = value;
                ResetLayout();
            }
        }

        // This property performs layout but in different way than using Metadata.AffectsLayout -
        // ResetLayout() is used.
        // Be careful with the caching mechanism - ResetLayout must be called when that property is changed.
        public static readonly RadProperty ScrollThicknessProperty = RadProperty.Register(
            "ScrollThickness",
            typeof(int),
            typeof(RadScrollLayoutPanel),
            new RadElementPropertyMetadata(SystemInformation.VerticalScrollBarWidth, ElementPropertyOptions.None));

        private int scrollThicknessCache = SystemInformation.VerticalScrollBarWidth;
        /// <summary>
        /// Gets or sets the thickness of the scrollbar.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(16)]
        [Description("Gets or sets the thickness of the scrollbar.")]
        public int ScrollThickness
        {
            get
            {
                // this.scrollThicknessCache is set by OnPropertyChnaged()
                return this.scrollThicknessCache;
            }
            set
            {
                this.SetValue(ScrollThicknessProperty, value);
            }
        }

        private RadElement viewport;
        /// <summary>
        ///     Gets or sets the element which content will be scrolled if the scroll viewer has
        ///     not enough space for it. Very often the viewport is a layout panel that implements
        ///     <see cref="IRadScrollViewport"/>.
        /// </summary>
        /// <value>
        /// Object of type RadElement which represents the content that could be scrolled if
        /// necessary. Default value is null.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public RadElement Viewport
        {
            get { return this.viewport; }
            set
            {
                if (value != this.viewport)
                {
                    RadElement oldViewport = this.viewport;
                    if (this.viewport != null)
                        this.Children.Remove(this.viewport);
                    this.viewport = value;
                    if (this.viewport != null)
                        this.Children.Add(this.viewport);
                    SetViewportAutoSizeMode();

                    OnNewViewportSet(oldViewport, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether physical or logical scrolling will be
        /// used.
        /// </summary>
        /// <value>Boolean value: when it is false logical scrolling will be used.</value>
        /// <remarks>
        /// 	<para>
        ///         This property cannot be set to false if <see cref="Viewport"/> does not
        ///         implement <see cref="IRadScrollViewport"/>.
        ///     </para>
        /// 	<para>
        ///         Default value is true for ordinary viewports and false for viewports that
        ///         implement <see cref="IRadScrollViewport"/>.
        ///     </para>
        /// </remarks>
        [DefaultValue(false)]
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Switch Physical / Logical scrolling (if possible).")]
        public bool UsePhysicalScrolling
        {
            get
            {
                if (!(this.viewport is IRadScrollViewport))
                    return true;

                IVirtualViewport virtualViewport = this.viewport as IVirtualViewport;
                if (virtualViewport != null && virtualViewport.Virtualized)
                    return false;

                return this.GetBitState(UsePhysicalScrollingStateKey);
            }
            set
            {
                if (value != this.GetBitState(UsePhysicalScrollingStateKey))
                {
                    this.BitState[UsePhysicalScrollingStateKey] = value;
                    this.SetViewportAutoSizeMode();
                }
            }
        }

        private Point pixelsPerLineScroll = new Point(InitialPixelsPerLineScroll, InitialPixelsPerLineScroll);
        /// <summary>
        /// 	<para>Gets or sets the number of pixels to use when performing Line
        ///     Up/Down/Left/Right scrolling operation.</para>
        /// 	<para>Still the scrolling position can be set with one pixel accuracy if the scroll
        ///     bar thumb is dragged.</para>
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Specifies the number of pixels for Line scroll (Small change) when Physical scrolling is used.")]
        public Point PixelsPerLineScroll
        {
            get
            {
                return pixelsPerLineScroll;
            }

            set
            {
                pixelsPerLineScroll = value;
                this.ResetLayout();
            }
        }

        /// <summary>Gets the minimum possible scrolling position.</summary>
        /// <value>
        /// Point which contains minimum values for scrolling in horizontal and vertical
        /// direction.
        /// </value>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Specifies the minimum scrolling value (in both horizontal and vertical directions)")]
        public Point MinValue
        {
            get
            {
                return new Point(
                  this.horizontalScrollBar.Minimum,
                  this.verticalScrollBar.Minimum);
            }
            set
            {
                this.horizontalScrollBar.Minimum = value.X;
                this.verticalScrollBar.Minimum = value.Y;
                ResetLayout();
            }
        }

        /// <summary>Gets the maximum opssible scrolling position.</summary>
        /// <value>
        /// Point which contains maximum values for scrolling in horizontal and vertical
        /// direction.
        /// </value>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Specifies the maximum scrolling value (in both horizontal and vertical directions)")]
        public Point MaxValue
        {
            get
            {
                return new Point(
                  this.horizontalScrollBar.Maximum,
                  this.verticalScrollBar.Maximum);
            }
            set
            {
                this.horizontalScrollBar.Maximum = value.X;
                this.verticalScrollBar.Maximum = value.Y;
                this.OnNotifyPropertyChanged("MaxValue");
                ResetLayout();
            }
        }

        /// <summary>
        ///     Gets or sets the scrolling position. The value is between
        ///     <see cref="MinValue"/> and <see cref="MaxValue"/>.
        /// </summary>
        /// <value>
        /// Point which contains the current scrolling position in horizontal and vertical
        /// direction.
        /// </value>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Specifies the current scrolling value (in both horizontal and vertical directions)")]
        public Point Value
        {
            get
            {
                return new Point(
                  this.horizontalScrollBar.Value,
                  this.verticalScrollBar.Value);
            }
            set
            {
                ScrollTo(value.X, value.Y);
            }
        }

        // TODO: Remove ForceViewportWidth and ForceViewportHeight properties
        public static readonly RadProperty ForceViewportWidthProperty = RadProperty.Register(
            "ForceViewportWidth",
            typeof(bool),
            typeof(RadScrollLayoutPanel),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ForceViewportWidth
        {
            get { return (bool)GetValue(ForceViewportWidthProperty); }
            set { SetValue(ForceViewportWidthProperty, value); }
        }

        public static readonly RadProperty ForceViewportHeightProperty = RadProperty.Register(
            "ForceViewportHeight",
            typeof(bool),
            typeof(RadScrollLayoutPanel),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ForceViewportHeight
        {
            get { return (bool)GetValue(ForceViewportHeightProperty); }
            set { SetValue(ForceViewportHeightProperty, value); }
        }

        public bool MeasureWithAvaibleSize
        {
            get
            {
                return this.GetBitState(MeasureWithAvaibleSizeStateKey);
            }
            set
            {
                this.SetBitState(MeasureWithAvaibleSizeStateKey, value);
            }
        }

        #endregion

        #region Constructors

        public RadScrollLayoutPanel()
        {
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.horizontalScrollState = ScrollState.AutoHide;
            this.verticalScrollState = ScrollState.AutoHide;

            this.BitState[UseScrollCallbackStateKey] = true;
            this.ClipDrawing = true;
        }

        public RadScrollLayoutPanel(RadElement viewport)
        {
            this.Viewport = viewport;
        }

        public RadScrollLayoutPanel(RadElement viewport, int initialScrollThickness)
            : this(viewport)
        {
            this.ScrollThickness = initialScrollThickness;
        }

        #endregion

        #region Overrides
        protected override void CreateChildElements()
        {
            this.horizontalScrollBar = new RadScrollBarElement();
            this.horizontalScrollBar.ScrollType = ScrollType.Horizontal;
            this.horizontalScrollBar.Visibility = ElementVisibility.Collapsed;
            this.horizontalScrollBar.Minimum = 0;
            this.horizontalScrollBar.ZIndex = 1000;
            this.horizontalScrollBar.Class = "ScrollPanelHorizontalScrollBar";
            this.horizontalScrollBar.ThemeRole = "RadScrollBarElement";
            this.Children.Add(this.horizontalScrollBar);

            this.verticalScrollBar = new RadScrollBarElement();
            this.verticalScrollBar.ScrollType = ScrollType.Vertical;
            this.verticalScrollBar.Visibility = ElementVisibility.Collapsed;
            this.verticalScrollBar.Minimum = 0;
            this.verticalScrollBar.ZIndex = 1000;
            this.verticalScrollBar.Class = "ScrollPanelVerticalScrollBar";
            this.verticalScrollBar.ThemeRole = "RadScrollBarElement";
            this.Children.Add(this.verticalScrollBar);

            this.blankSpot = new FillPrimitive();
            this.blankSpot.Class = "ScrollPanelBlankSpotFill";
            this.blankSpot.Visibility = ElementVisibility.Collapsed;
            this.blankSpot.ZIndex = 1000;
            this.blankSpot.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            this.Children.Add(this.blankSpot);

            this.horizontalScrollBar.Scroll += OnHScroll;
            this.verticalScrollBar.Scroll += OnVScroll;
            this.horizontalScrollBar.ScrollParameterChanged += new EventHandler(OnScrollBarParameterChanged);
            this.verticalScrollBar.ScrollParameterChanged += new EventHandler(OnScrollBarParameterChanged);
        }

        public override void PerformLayoutCore(RadElement affectedElement)
        {
            if (this.GetBitState(IsScrollingStateKey))
                return;

            this.ResetLayout();
        }

        public override Size GetPreferredSizeCore(Size proposedSize)
        {
            if (this.GetBitState(IsScrollingStateKey))
                return this.Size;

            if (this.viewport == null)
                return base.GetPreferredSizeCore(proposedSize);

            if (this.AutoSize && this.AutoSizeMode == RadAutoSizeMode.WrapAroundChildren)
            {
                Size viewportSize = this.viewport.GetPreferredSize(proposedSize);
                Size extentSize = this.UsePhysicalScrolling ? viewportSize :
                    ((IRadScrollViewport)this.viewport).GetExtentSize();
                ScrollFlags sf = GetScrollingNeeds(extentSize, this.MaxSize);
                Size scollBarsSize = GetScrollBarsSize(sf);
                return Size.Add(viewportSize, scollBarsSize);
            }
            else
            {
                return base.GetPreferredSizeCore(proposedSize);
            }
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            for (int i = 0; i < this.Children.Count; i++)
            {
                RadElement child = this.Children[i];
                if (Object.ReferenceEquals(child, this.viewport) && !this.MeasureWithAvaibleSize)
                {
                    child.Measure(new SizeF(float.PositiveInfinity, float.PositiveInfinity));
                }
                else
                {
                    child.Measure(availableSize);
                }
            }

            return SizeF.Empty;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            IRadScrollViewport scrollViewport = this.viewport as IRadScrollViewport;
            if (scrollViewport != null)
                scrollViewport.InvalidateViewport();

            if (this.viewport == null)
            {
                base.ArrangeOverride(finalSize);

                this.horizontalScrollBar.Visibility = ElementVisibility.Collapsed;
                this.verticalScrollBar.Visibility = ElementVisibility.Collapsed;

                return finalSize;
            }

            // Init size members (clientSize, viewportSize, extentSize)
            this.clientSize = Size.Round(finalSize);
            //TODO:
            //changed this.viewport.DesiziredSize to this.viewport.Size
            //MUST be tested 
            Size preferredViewportSize = Size.Round(this.viewport.DesiredSize);
            this.extentSize = this.UsePhysicalScrolling ? preferredViewportSize :
                    ((IRadScrollViewport)this.viewport).GetExtentSize();
            ScrollFlags sf = GetScrollingNeeds(this.extentSize, this.clientSize);
            Size scrollBarsSize = GetScrollBarsSize(sf);
            this.viewportSize = Size.Subtract(this.clientSize, scrollBarsSize);

            // Sets Visibility and Enabled properties of the scrollbars and the blankspot
            ResetScrollState(sf);

            // !!! blankSpotSize is non-empty ONLY when both scrollbars are visible
            Size blankSpotSize = Size.Empty;
            if (this.BlankSpot.Visibility == ElementVisibility.Visible)
            {
                blankSpotSize = scrollBarsSize;
            }

            float viewportLeftPos = this.RightToLeft ? scrollBarsSize.Width : 0;
            float horizontalScrollLeftPos = this.RightToLeft ? scrollBarsSize.Width : 0;
            float verticalScrollLeftPos = this.RightToLeft ? 0 : finalSize.Width - scrollBarsSize.Width;
            float blankSpotLeftPos = this.RightToLeft ? 0 : finalSize.Width - blankSpotSize.Width;

            RectangleF viewportRect = new RectangleF(
                viewportLeftPos, 0,
                this.viewportSize.Width, this.viewportSize.Height);
            RectangleF vertScrollRect = new RectangleF(
                verticalScrollLeftPos, 0,
                scrollBarsSize.Width, Math.Max(0, finalSize.Height - scrollBarsSize.Height));
            RectangleF horizScrollRect = new RectangleF(
                horizontalScrollLeftPos, finalSize.Height - scrollBarsSize.Height,
                Math.Max(0, finalSize.Width - scrollBarsSize.Width), scrollBarsSize.Height);
            RectangleF blankSpotRectRect = new RectangleF(
                blankSpotLeftPos, finalSize.Height - blankSpotSize.Height,
                blankSpotSize.Width, blankSpotSize.Height);

            this.viewport.Arrange(viewportRect);
            this.VerticalScrollBar.Arrange(vertScrollRect);
            this.HorizontalScrollBar.Arrange(horizScrollRect);
            this.BlankSpot.Arrange(blankSpotRectRect);

            ResetScrollPos();

            return finalSize;
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == RadElement.AutoSizeModeProperty)
            {
                SetViewportAutoSizeMode();
            }
            else if (e.Property == ScrollThicknessProperty)
            {
                this.scrollThicknessCache = (int)e.NewValue;
                ResetLayout();
            }
        }
        #endregion

        #region APIs
        public void ScrollTo(int xpos, int ypos)
        {
            Point oldValue = this.Value;

            if (xpos < this.MinValue.X)
                xpos = this.MinValue.X;
            int maxVisibleValue = Math.Min(this.MaxValue.X - this.horizontalScrollBar.LargeChange + 1, this.MaxValue.X);
            if (xpos > maxVisibleValue)
                xpos = maxVisibleValue;

            if (ypos < this.MinValue.Y)
                ypos = this.MinValue.Y;
            maxVisibleValue = Math.Min(this.MaxValue.Y - this.verticalScrollBar.LargeChange + 1, this.MaxValue.Y);
            if (ypos > maxVisibleValue)
                ypos = maxVisibleValue;

            ScrollWithInternal(xpos - oldValue.X, ypos - oldValue.Y, true);
        }

        public void ScrollWith(int xoffs, int yoffs)
        {
            Point newValue = Point.Add(this.Value, new Size(xoffs, yoffs));

            if (newValue.X < this.MinValue.X)
                xoffs = this.MinValue.X - this.Value.X;
            int maxVisibleValue = Math.Min(this.MaxValue.X - this.horizontalScrollBar.LargeChange + 1, this.MaxValue.X);
            if (newValue.X > maxVisibleValue)
                xoffs = maxVisibleValue - this.Value.X;

            if (newValue.Y < this.MinValue.Y)
                yoffs = this.MinValue.Y - this.Value.Y;
            maxVisibleValue = Math.Min(this.MaxValue.Y - this.verticalScrollBar.LargeChange + 1, this.MaxValue.Y);
            if (newValue.Y > maxVisibleValue)
                yoffs = maxVisibleValue - this.Value.Y;

            ScrollWithInternal(xoffs, yoffs, true);
        }

        public void ScrollElementIntoView(RadElement childElement)
        {
            if (childElement == null)
                return;

            if (this.UseNewLayoutSystem)
            {
                this.UpdateLayout();
            }

            if (this.UsePhysicalScrolling)
            {
                Rectangle viewportRect = new Rectangle(Point.Empty, this.viewportSize);
                Rectangle childRect = childElement.FullBoundingRectangle;
                childRect.Offset((int)Math.Round(this.viewport.PositionOffset.Width),
                    (int)Math.Round(this.viewport.PositionOffset.Height));
                Size childOffset = RadCanvasViewport.CalcMinOffset(childRect, viewportRect);
                ScrollWith(-childOffset.Width, -childOffset.Height);
            }
            else
            {
                if (this.viewport != null)
                {
                    Size offset = ((IRadScrollViewport)this.viewport).ScrollOffsetForChildVisible(childElement, this.Value);
                    ScrollWith(offset.Width, offset.Height);
                }
            }
        }
        #endregion

        private void ScrollWithInternal(int xoffs, int yoffs, bool resetAll)
        {
            if (xoffs != 0 || yoffs != 0)
            {
                Point oldValue = this.Value;
                Size offs = new Size(xoffs, yoffs);
                Point newValue = GetValidValue(Point.Add(oldValue, offs));
                if (resetAll)
                {
                    SetScrollValueInternal(newValue);
                    ResetLayout();
                }
                DoScroll(oldValue, newValue);
            }
        }

        private void OnScrollBarParameterChanged(object sender, EventArgs e)
        {
            OnScrollParametersChanged((RadScrollBarElement)sender);
        }

        private void OnHScroll(Object sender, ScrollEventArgs e)
        {
            if (this.GetBitState(UseScrollCallbackStateKey))
            {
                this.ScrollWithInternal(e.NewValue - e.OldValue, 0, false);
            }
        }

        private void OnVScroll(Object sender, ScrollEventArgs e)
        {
            if (this.GetBitState(UseScrollCallbackStateKey))
            {
                this.ScrollWithInternal(0, e.NewValue - e.OldValue, false);
            }
        }

        #region ResetLayout
        public void ResetLayout()
        {
            if (this.UseNewLayoutSystem)
            {
                this.InvalidateMeasure();
                this.InvalidateArrange();
            }
            else
            {
                IRadScrollViewport scrollViewport = this.viewport as IRadScrollViewport;
                if (scrollViewport != null)
                    scrollViewport.InvalidateViewport();

                if (this.viewport == null)
                {
                    this.horizontalScrollBar.Visibility = ElementVisibility.Collapsed;
                    this.verticalScrollBar.Visibility = ElementVisibility.Collapsed;
                    return;
                }

                // Init size members (clientSize, viewportSize, extentSize)
                this.clientSize = this.Parent.FieldSize;
                Size preferredViewportSize = this.viewport.GetPreferredSize(this.clientSize);
                this.extentSize = this.UsePhysicalScrolling ? preferredViewportSize :
                        ((IRadScrollViewport)this.viewport).GetExtentSize();
                ScrollFlags sf = GetScrollingNeeds(this.extentSize, this.clientSize);
                Size scrollBarsSize = GetScrollBarsSize(sf);
                this.viewportSize = Size.Subtract(this.clientSize, scrollBarsSize);

                // Viewport
                Point startPoint = new Point(base.RightToLeft ? scrollBarsSize.Width : 0, 0);
                Rectangle viewportRect = new Rectangle(startPoint,
                    this.UsePhysicalScrolling ? this.extentSize : this.viewportSize);
                this.viewport.SetBounds(viewportRect);

                ResetScrollState(sf);
                ResetScrollPos();

                OnSizeHScroll(this.clientSize);
                OnSizeVScroll(this.clientSize);
                OnSizeBlankSpot(this.clientSize);
            }
        }

        private ScrollFlags GetScrollingNeeds(Size extentSize, Size clientSize)
        {
            // Check where is (Bottom, Right) point of extentSize in clientSize
            // There are 9 posible positions - labeled with areas from A to I:
            //
            //                            Vertical ScrollBar
            //                                  ^
            //                        (0,0)     |
            //                          +-----+---+
            //    Viewport Rectangle <- |     |   |
            //                          |     |   |
            //                          |   A | B | C
            //                          +-----+---+
            //  Horizontal ScrollBar <- |   D | E | F
            //                          +-----+---+
            //                              G   H   I

            bool oldHorizScrollNeeded = this.GetBitState(IsHorizScrollNeededStateKey);
            bool oldVerticalScrollNeeded = this.GetBitState(IsVertScrollNeededStateKey);

            this.BitState[IsHorizScrollNeededStateKey] = false;
            this.BitState[IsVertScrollNeededStateKey] = false;

            // Check cases when (Bottom, Right) of the Extent is on different areas
            // according Client rectangle
            if (extentSize.Width > clientSize.Width)
            {// Point C, F and I
                this.BitState[IsHorizScrollNeededStateKey] = true;
                if (extentSize.Height > clientSize.Height - scrollThicknessCache)
                {// Points F and I
                    this.BitState[IsVertScrollNeededStateKey] = true;
                }
            }
            else if (extentSize.Width > clientSize.Width - scrollThicknessCache)
            {// Points B, E and H
                if (extentSize.Height > clientSize.Height)
                {// Point H
                    this.BitState[IsHorizScrollNeededStateKey] = true;
                    this.BitState[IsVertScrollNeededStateKey] = true;
                }
                else if (extentSize.Height > clientSize.Height - scrollThicknessCache)
                {// Point E
                    if (
                            this.verticalScrollState != ScrollState.AlwaysHide &&
                            this.horizontalScrollState != ScrollState.AlwaysHide &&
                            (this.horizontalScrollState != ScrollState.AutoHide ||
                            this.verticalScrollState != ScrollState.AutoHide) &&
                            (
                                (
                                 this.verticalScrollBar.Visibility != ElementVisibility.Collapsed &&
                                 this.verticalScrollState == ScrollState.AlwaysShow
                                ) 
                                ||
                                (
                                 this.horizontalScrollBar.Visibility != ElementVisibility.Collapsed &&                                 
                                 this.horizontalScrollState == ScrollState.AlwaysShow
                                )
                            )
                        )
                    {
                        this.BitState[IsHorizScrollNeededStateKey] = true;
                        this.BitState[IsVertScrollNeededStateKey] = true;
                    }
                }
                else
                {// Point B
                    if (this.verticalScrollState == ScrollState.AlwaysShow)
                    {
                        this.BitState[IsHorizScrollNeededStateKey] = true;
                    }
                }
            }
            else
            {// Point A, D and G
                if (extentSize.Height > clientSize.Height)
                {// Point G
                    this.BitState[IsVertScrollNeededStateKey] = true;
                }
                else if (extentSize.Height > clientSize.Height - scrollThicknessCache)
                {// Point D
                    if (this.horizontalScrollState == ScrollState.AlwaysShow)
                    {
                        this.BitState[IsVertScrollNeededStateKey] = true;
                    }
                }
            }

            bool horizNeedChanged = oldHorizScrollNeeded != this.GetBitState(IsHorizScrollNeededStateKey);
            bool vertNeedChanged = oldVerticalScrollNeeded != this.GetBitState(IsVertScrollNeededStateKey);
            if (horizNeedChanged || vertNeedChanged)
            {
                OnScrollNeedsChanged(new ScrollNeedsEventArgs(oldHorizScrollNeeded, this.GetBitState(IsHorizScrollNeededStateKey),
                    oldVerticalScrollNeeded, this.GetBitState(IsVertScrollNeededStateKey)));
            }

            ScrollFlags res = 0;
            if (this.GetBitState(IsHorizScrollNeededStateKey))
                res |= ScrollFlags.Horizontal;
            if (this.GetBitState(IsVertScrollNeededStateKey))
                res |= ScrollFlags.Vertical;

            return res;
        }

        /// <summary>
        /// Set visible and enabled state of the ScrollBars.
        /// </summary>
        private void ResetScrollState(ScrollFlags flags)
        {
            bool isHorizScrollNeeded = (flags & ScrollFlags.Horizontal) == ScrollFlags.Horizontal;
            bool isVertScrollNeeded = (flags & ScrollFlags.Vertical) == ScrollFlags.Vertical;

            switch (this.horizontalScrollState)
            {
                case ScrollState.AlwaysShow:
                    this.horizontalScrollBar.Enabled = isHorizScrollNeeded;
                    this.horizontalScrollBar.Visibility = ElementVisibility.Visible;
                    break;
                case ScrollState.AlwaysHide:
                    this.horizontalScrollBar.Enabled = isHorizScrollNeeded;
                    this.horizontalScrollBar.Visibility = ElementVisibility.Collapsed;
                    break;
                case ScrollState.AutoHide:
                    if (isHorizScrollNeeded)
                        this.horizontalScrollBar.Enabled = true;
                    this.horizontalScrollBar.Visibility = isHorizScrollNeeded ?
                        ElementVisibility.Visible : ElementVisibility.Collapsed;
                    break;
            }

            switch (this.verticalScrollState)
            {
                case ScrollState.AlwaysShow:
                    this.verticalScrollBar.Enabled = isVertScrollNeeded;
                    this.verticalScrollBar.Visibility = ElementVisibility.Visible;
                    break;
                case ScrollState.AlwaysHide:
                    this.verticalScrollBar.Enabled = isVertScrollNeeded;
                    this.verticalScrollBar.Visibility = ElementVisibility.Collapsed;
                    break;
                case ScrollState.AutoHide:
                    if (isVertScrollNeeded)
                        this.verticalScrollBar.Enabled = true;
                    this.verticalScrollBar.Visibility = isVertScrollNeeded ?
                        ElementVisibility.Visible : ElementVisibility.Collapsed;
                    break;
            }

            if ((horizontalScrollBar.Visibility == ElementVisibility.Collapsed) ||
                (verticalScrollBar.Visibility == ElementVisibility.Collapsed))
                this.blankSpot.Visibility = ElementVisibility.Collapsed;
            else
                this.blankSpot.Visibility = ElementVisibility.Visible;
        }

        internal void ResetScrollParamsInternal()
        {
            ScrollPanelParameters scrollParams = GetScrollParams();
            this.horizontalScrollBar.SetParameters(scrollParams.HorizontalScrollParameters);
            this.verticalScrollBar.SetParameters(scrollParams.VerticalScrollParameters);
        }

        /// <summary>
        /// Make viewportOffset to be with correct value.
        /// Set Value of ScrollBars using viewportOffset
        /// </summary>
        private void ResetScrollPos()
        {
            this.ResetScrollParamsInternal();

            Point oldValue = this.Value;
            Point newValue = oldValue;
            if (this.UsePhysicalScrolling)
            {
                newValue.X = RadCanvasViewport.ValidatePosition(newValue.X,
                    this.extentSize.Width - this.viewportSize.Width);
                newValue.Y = RadCanvasViewport.ValidatePosition(newValue.Y,
                    this.extentSize.Height - this.viewportSize.Height);
            }
            else
            {
                if (this.viewport != null)
                {
                    newValue = ((IRadScrollViewport)this.viewport).ResetValue(this.Value, this.viewportSize, this.extentSize);
                }
            }

            newValue = GetValidValue(newValue);

            SetScrollValueInternal(newValue);

            if (oldValue != newValue || newValue == this.MinValue)
                DoScroll(oldValue, newValue);
        }

        private void OnSizeHScroll(Size clientSize)
        {
            if (this.horizontalScrollBar.Visibility != ElementVisibility.Collapsed)
            {
                Size proposedSize, SBPreferedSize;

                int width = (this.blankSpot.Visibility == ElementVisibility.Collapsed) ?
                    clientSize.Width : clientSize.Width - scrollThicknessCache;
                proposedSize = new Size(width, scrollThicknessCache);
                SBPreferedSize = this.horizontalScrollBar.GetPreferredSize(proposedSize);

                this.horizontalScrollBar.SetBounds(
                    this.IsRtl(0, scrollThicknessCache),
                    clientSize.Height - scrollThicknessCache,
                    proposedSize.Width,
                    proposedSize.Height);
            }
        }

        private void OnSizeVScroll(Size clientSize)
        {
            if (this.verticalScrollBar.Visibility != ElementVisibility.Collapsed)
            {
                Size proposedSize, SBPreferedSize;

                int height = (this.blankSpot.Visibility == ElementVisibility.Collapsed) ?
                    clientSize.Height : clientSize.Height - scrollThicknessCache;
                proposedSize = new Size(scrollThicknessCache, height);
                SBPreferedSize = this.verticalScrollBar.GetPreferredSize(proposedSize);

                this.verticalScrollBar.SetBounds(
                   this.IsRtl(clientSize.Width - scrollThicknessCache, 0),
                    0,
                    proposedSize.Width,
                    proposedSize.Height);
            }
        }

        private void OnSizeBlankSpot(Size clientSize)
        {
            if (this.blankSpot.Visibility != ElementVisibility.Collapsed)
            {
                Size proposedSize = new Size(scrollThicknessCache, scrollThicknessCache);
                this.blankSpot.GetPreferredSize(proposedSize);
                this.blankSpot.SetBounds(
                    this.IsRtl(clientSize.Width - scrollThicknessCache, 0),
                    clientSize.Height - scrollThicknessCache,
                    proposedSize.Width,
                    proposedSize.Height);
            }
        }

        #endregion

        #region Operations
        private void SetViewportAutoSizeMode()
        {
            if (this.viewport == null)
                return;

            this.viewport.BypassLayoutPolicies = true;

            RadAutoSizeMode oldViewportAutoSizeMode = this.viewport.AutoSizeMode;
            if (this.UsePhysicalScrolling || this.AutoSizeMode == RadAutoSizeMode.WrapAroundChildren)
            {
                this.viewport.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
            }
            if (oldViewportAutoSizeMode == this.viewport.AutoSizeMode)
            {
                this.IsLayoutInvalidated = true;
                PerformLayout(this);
            }
        }

        private Size GetScrollBarsSize(ScrollFlags flags)
        {
            int vertical = 0;
            if ((this.horizontalScrollState == ScrollState.AlwaysShow) ||
                (this.horizontalScrollState == ScrollState.AutoHide && (flags & ScrollFlags.Horizontal) == ScrollFlags.Horizontal))
            {
                vertical = this.scrollThicknessCache;
            }

            int horizontal = 0;
            if ((this.verticalScrollState == ScrollState.AlwaysShow) ||
                 (this.verticalScrollState == ScrollState.AutoHide && (flags & ScrollFlags.Vertical) == ScrollFlags.Vertical))
            {
                horizontal = this.scrollThicknessCache;
            }

            return new Size(horizontal, vertical);
        }

        private int ValidateRange(int value, int minimum, int maximum)
        {
            if (maximum >= minimum)
            {
                if (value < minimum)
                    value = minimum;
                else if (value > maximum)
                    value = maximum;
            }
            return value;
        }

        private Point GetValidValue(Point value)
        {
            Point maxValue = this.MaxValue;
            Point minValue = this.MinValue;
            return new Point(
                ValidateRange(value.X, minValue.X, maxValue.X),
                ValidateRange(value.Y, minValue.Y, maxValue.Y));
        }

        private ScrollPanelParameters GetScrollParams()
        {
            if (this.UsePhysicalScrolling)
            {
                return new ScrollPanelParameters(
                    0, Math.Max(1, this.extentSize.Width), this.pixelsPerLineScroll.X, Math.Max(1, this.viewportSize.Width),
                    0, Math.Max(1, this.extentSize.Height), this.pixelsPerLineScroll.Y, Math.Max(1, this.viewportSize.Height)
                );
            }
            // This function should never be called when this.viewport is null...
            return ((IRadScrollViewport)this.viewport).GetScrollParams(this.viewportSize, this.extentSize);
        }

        internal void SetScrollValueInternal(Point newValue)
        {
            this.BitState[UseScrollCallbackStateKey] = false;
            this.horizontalScrollBar.Value = newValue.X;
            this.verticalScrollBar.Value = newValue.Y;
            this.BitState[UseScrollCallbackStateKey] = true;
        }

        // Scrolls the content only - the scroll bar thumbs must be moved explicitly
        private void DoScroll(Point oldValue, Point newValue)
        {
            if (this.viewport == null)
                return;

            this.BitState[IsScrollingStateKey] = true;
            if (this.UsePhysicalScrolling)
            {
                this.viewport.PositionOffset = new SizeF(-newValue.X, -newValue.Y);
            }
            else
            {
                ((IRadScrollViewport)this.viewport).DoScroll(oldValue, newValue);
            }
            this.BitState[IsScrollingStateKey] = false;
            OnScroll(oldValue, newValue);
        }

        private int IsRtl(int nonRTLvalue, int RTLvalue)
        {
            return base.RightToLeft ? RTLvalue : nonRTLvalue;
        }

        #endregion


        #region Nested types
        [Flags()]
        private enum ScrollFlags
        {
            Horizontal = 0x01,
            Vertical = 0x02
        }
        #endregion
    }
}
