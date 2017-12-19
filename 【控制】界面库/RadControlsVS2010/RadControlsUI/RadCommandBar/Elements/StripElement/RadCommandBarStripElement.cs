using Telerik.WinControls;
using Telerik.WinControls.Elements;
using System.ComponentModel;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represent a single strip with controls inside 
    /// </summary>
    [Designer(DesignerConsts.CommandBarStripElementDesignerString)]
    public class CommandBarStripElement : RadCommandBarVisualElement
    {
        #region RadProperties
        public static RadProperty DesiredLocationProperty = RadProperty.Register(
            "DesiredLocation", typeof(PointF), typeof(CommandBarStripElement), new RadElementPropertyMetadata(
                new PointF(-1, -1), ElementPropertyOptions.AffectsParentMeasure | ElementPropertyOptions.AffectsParentArrange));

        public static RadProperty VisibleInCommandBarProperty = RadProperty.Register(
                    "VisibleInCommandBar", typeof(bool), typeof(CommandBarStripElement), new RadElementPropertyMetadata(
                        true, ElementPropertyOptions.AffectsParentMeasure | ElementPropertyOptions.AffectsParentArrange));

        #endregion

        #region Consts
        private const float minMeasureSize = 25;
        private Size localMinSize;

        #endregion

        #region Events

        /// <summary>
        /// Occurs before dragging is started.
        /// </summary>
        public event CancelEventHandler BeginDrag;

        /// <summary>
        /// Occurs when item is being dragged.
        /// </summary>
        public event MouseEventHandler Drag;

        /// <summary>
        /// Occurs when item is released and dragging is stopped.
        /// </summary>
        public event EventHandler EndDrag;

        /// <summary>
        /// Occurs when Items collection is changed.
        /// </summary>
        public event RadCommandBarBaseItemCollectionItemChangedDelegate ItemsChanged;

        /// <summary>
        /// Occurs when item is clicked.
        /// </summary>
        public event EventHandler ItemClicked;

        /// <summary>
        /// Occurs when item is moved to the overflow panel.
        /// </summary>
        public event EventHandler ItemOverflowed;

        /// <summary>
        /// Occurs when item is moved out from the overflow panel.
        /// </summary>
        public event EventHandler ItemOutOfOverflow;

        /// <summary>
        /// Occurs before oferflow menu is opened.
        /// </summary>
        public event CancelEventHandler OverflowMenuOpening;

        /// <summary>
        /// Occurs when overflow menu is opened.
        /// </summary>
        public event EventHandler OverflowMenuOpened;

        /// <summary>
        /// Occurs before oferflow menu is opened.
        /// </summary>
        public event CancelEventHandler OverflowMenuClosing;

        /// <summary>
        /// Occurs when overflow menu is opened.
        /// </summary>
        public event EventHandler OverflowMenuClosed;

        /// <summary>
        /// Occurs before VisibleInCommandBar property is changed.
        /// </summary>
        public event CancelEventHandler VisibleInCommandBarChanging;

        /// <summary>
        /// Occurs when VisibleInCommandBar property is changed.
        /// </summary>
        public event EventHandler VisibleInCommandBarChanged;

        /// <summary>
        /// Occurs before item is moved in or out of the UncheckedItems collection.
        /// </summary>
        public event CancelEventHandler ItemVisibleInStripChanging;

        /// <summary>
        /// Occurs when item is moved in or out of the UncheckedItems collection.
        /// </summary>
        public event EventHandler ItemVisibleInStripChanged;

        /// <summary>
        /// Occurs before VisibleInCommandBar property is changed.
        /// </summary>
        public event CancelEventHandler LineChanging;

        /// <summary>
        /// Occurs when VisibleInCommandBar property is changed.
        /// </summary>
        public event EventHandler LineChanged;

        /// <summary>
        /// Occurs when Orientation property is changed.
        /// </summary>
        public event EventHandler OrientationChanged;

        /// <summary>
        /// Occurs before Orientation property is changed.
        /// </summary>
        public event CancelEventHandler OrientationChanging;

        #endregion

        #region Fields

        protected RadCommandBarItemsPanel itemsLayout;
        protected RadCommandBarBaseItemCollection items;
        protected RadCommandBarGrip grip;
        protected RadCommandBarOverflowButton overflowButton;
        internal protected PointF cachedDesiredLocation = new PointF(-1, -1);
        protected bool visibleInCommandBar = true;
        protected bool enableDragging = true;
        protected bool enableFloating = false;
        protected string addOrRemoveButtonsString = "Add or Remove Buttons";
        protected Size overflowMenuMinSize = new System.Drawing.Size(50, 25);
        protected Size overflowMenuMaxSize = new System.Drawing.Size(270, 0);
        protected CommandBarFloatingForm floatingForm;
         
        #endregion

        #region Properties

        /// <summary>
        /// Gets the form in which the items are placed where the strip is floating.
        /// </summary>
        [Browsable(false)]
        [Description("Gets the form in which the items are placed where the strip is floating.")]
        public CommandBarFloatingForm FloatingForm
        {
            get
            {
                return floatingForm;
            }
            set
            {
                floatingForm = value;
            }
        }

        /// <summary>
        /// Gets the layout panel in which the items are arranged.
        /// </summary>
        [Browsable(false)]
        [Description("Gets the layout panel in which the items are arranged.")]
        public RadCommandBarItemsPanel ItemsLayout
        {
            get
            {
                return itemsLayout;
            }
        }


        /// <summary>
        /// Gets or sets Overflow menu single strip minimum size.
        /// </summary>
        [DefaultValue(typeof(Size), "50, 25")]
        [Browsable(false)]
        [Description("Gets or sets Overflow menu single strip minimum size.")]
        public Size OverflowMenuMinSize
        {
            get
            {
                return overflowMenuMinSize;
            }
            set
            {
                overflowMenuMinSize = value;
            }
        }

        /// <summary>
        /// Gets or sets Overflow menu single strip maximum size.
        /// </summary>
        [DefaultValue(typeof(Size), "270, 0")]
        [Browsable(false)]
        [Description("Gets or sets Overflow menu single strip maximum size.")]
        public Size OverflowMenuMaxSize
        {
            get
            {
                return overflowMenuMaxSize;
            }
            set
            {
                overflowMenuMaxSize = value;
            }
        }

        /// <summary>
        /// Gets or sets Overflow menu "Add or Remove Buttons" string.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets Overflow menu \"Add or Remove Buttons\" string.")]
        [Browsable(false)]
        [DefaultValue("Add or Remove Buttons")]
        [Localizable(true)]
        [Obsolete("This is property is obsolete. Please use the CommandBarLocalizationProvider instead.")]
        public string AddOrRemoveButtonsString
        {
            get
            {
                return addOrRemoveButtonsString;
            }
            set
            {
                addOrRemoveButtonsString = value;
            }
        }

        /// <summary>
        /// Gets or sets the desired location of the strip element.
        /// </summary>
        /// 
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets the desired location of the strip element.")]
        [Browsable(false)]
        [DefaultValue(typeof(PointF), "-1,-1")]
        public PointF DesiredLocation
        {
            get
            {
                return cachedDesiredLocation;
            }
            set
            {
                if (this.cachedDesiredLocation == value)
                {
                    return;
                }

                this.cachedDesiredLocation = value; // setting value without invoking the layout
                if (this.IsLayoutSuspended)
                {
                    return;
                }

                this.SetValue(DesiredLocationProperty, value);

                CommandBarRowElement parentRow = (this.Parent as CommandBarRowElement);
                if (this.ShouldChangeLines() && parentRow!=null)
                {
                    CancelEventArgs e = new CancelEventArgs();
                    this.OnLineChanging(e);
                    if (!e.Cancel)
                    {
                        parentRow.MoveCommandStripInOtherLine(this);
                        this.OnLineChanged(new EventArgs());
                    }
                }

                if (this.Parent != null)
                {
                    this.Parent.InvalidateMeasure(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets if the strip can be dragged.
        /// </summary>
        /// 
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets if the strip can be dragged.")]
        [Browsable(true)]
        [DefaultValue(true)]
        public bool EnableDragging
        {
            get
            {
                return this.enableDragging;
            }
            set
            {
                this.enableDragging = value;
            }
        }

        /// <summary>
        /// Gets or sets if the strip can be floating.
        /// </summary>
        /// 
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets if the strip can be dragged.")]
        [Browsable(true)]
        [DefaultValue(false)]
        public bool EnableFloating
        {
            get
            {
                return this.enableFloating;
            }
            set
            {
                this.enableFloating = value;
            }
        }

        /// <summary>
        /// Gets the delta of the drag.  
        /// </summary>
        /// 
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets the delta of the drag.")]
        [Browsable(false)]
        public PointF Delta
        {
            get
            {
                return this.grip.Delta;
            }
        }

        /// <summary>
        /// Gets or sets whether the strip is beeing dragged.  
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets whether the strip is beeing dragged.")]
        [Browsable(false)]
        public bool IsDrag
        {
            get
            {
                return this.grip.IsDrag;
            }
        }

        /// <summary>
        /// Gets or sets whether the strip is visible in the command bar. 
        /// This property is changed by the context menu which is opened on right click on the control.
        /// </summary>
        /// 
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets whether the strip is visible in the command bar.")]
        [Browsable(true)]
        [DefaultValue(true)]
        public bool VisibleInCommandBar
        {
            get
            {
                return this.visibleInCommandBar;
            }
            set
            {
                if (this.visibleInCommandBar != value && !this.OnVisibleInCommandBarChanging(new CancelEventArgs()))
                {

                    this.visibleInCommandBar = value;
                    this.SetValue(RadElement.MinSizeProperty, (value) ? this.localMinSize : Size.Empty);
                    this.SetValue(CommandBarStripElement.VisibleInCommandBarProperty, value);
                    if (this.Parent != null)
                    {
                        this.Parent.InvalidateMeasure(true);
                    }

                    this.OnVisibleInCommandBarChanged(new EventArgs());
                }
            }
        }

        /// <summary>
        /// Gets or sets the elements orientation inside the line element. 
        /// Possible values are horizontal and vertical.
        /// </summary>
        [RadPropertyDefaultValue("Orientation", typeof(CommandBarRowElement))]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets the elements orientation inside the line element.")]
        [Browsable(false)]
        public override Orientation Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                if (this.orientation != value && !this.OnOrientationChanging(new CancelEventArgs()))
                {
                    this.SetOrientationCore(value);
                    this.OnOrientationChanged(new EventArgs());
                }
            }
        }

        /// <summary>
        /// Gets whether the strip has items in its overflow panel.
        /// </summary>
        [Browsable(false)]
        public bool HasOverflowedItems
        {
            get
            {
                return this.overflowButton.HasOverflowedItems;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="RadCommandBarGrip"/> element of the strip.
        /// </summary>
        [Browsable(true)]
        public RadCommandBarGrip Grip
        {
            get
            {
                return grip;
            }
            set
            {
                grip = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="RadCommandBarOverflowButton"/> element of the strip.
        /// </summary>
        [Browsable(true)]
        public RadCommandBarOverflowButton OverflowButton
        {
            get
            {
                return overflowButton;
            }
            set
            {
                overflowButton = value;
            }
        }

        #endregion

        #region Overrides
         
        protected override void OnBubbleEvent(RadElement sender, RoutedEventArgs args)
        {
            if (args.RoutedEvent == RadCommandBarGrip.BeginDraggingEvent)
            {
                CancelEventArgs dragArgs = (CancelEventArgs)args.OriginalEventArgs;
                this.OnBeginDragging(sender, dragArgs);
                args.Canceled = dragArgs.Cancel;
            }

            if (args.RoutedEvent == RadCommandBarGrip.EndDraggingEvent)
            {
                EventArgs dragArgs = args.OriginalEventArgs;
                this.OnEndDragging(sender, dragArgs);
            }

            if (args.RoutedEvent == RadCommandBarGrip.DraggingEvent)
            {
                MouseEventArgs dragArgs = (MouseEventArgs)args.OriginalEventArgs;
                this.OnDragging(sender, dragArgs);
            }

            if (args.RoutedEvent == RadCommandBarBaseItem.ClickEvent)
            {
                this.OnItemClicked(sender, args.OriginalEventArgs);
            }

            if (args.RoutedEvent == RadCommandBarBaseItem.VisibleInStripChangedEvent)
            {
                this.OnItemVisibleInStripChanged(sender, args.OriginalEventArgs);
            }

            if (args.RoutedEvent == RadCommandBarBaseItem.VisibleInStripChangingEvent)
            {
                this.OnItemVisibleInStripChanging(sender, args.OriginalEventArgs as CancelEventArgs);
            }
            base.OnBubbleEvent(sender, args);

        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            availableSize = GetClientRectangle(availableSize).Size;
            if ((!this.visibleInCommandBar && this.Site == null) || this.Visibility == ElementVisibility.Collapsed)
            {
                this.grip.Measure(SizeF.Empty);
                this.itemsLayout.Measure(SizeF.Empty);
                this.overflowButton.Measure(SizeF.Empty);
                return SizeF.Empty;
            }
            SizeF result = this.localMinSize;
            if (this.orientation == Orientation.Horizontal)
            {
                float maxHeight = minMeasureSize;
                this.grip.Measure(availableSize);

                float remainingWidth = availableSize.Width - this.grip.DesiredSize.Width;
                this.overflowButton.Measure(new SizeF(remainingWidth, availableSize.Height));

                remainingWidth -= this.overflowButton.DesiredSize.Width;
                this.itemsLayout.Measure(new SizeF(remainingWidth, availableSize.Height));

                maxHeight = Math.Max(this.grip.DesiredSize.Height, this.overflowButton.DesiredSize.Height);
                maxHeight = Math.Max(maxHeight, this.itemsLayout.DesiredSize.Height);

                float totalWidth = Math.Max(this.grip.DesiredSize.Width + this.overflowButton.DesiredSize.Width + this.itemsLayout.DesiredSize.Width,
                                                    minMeasureSize);

                if (!float.IsInfinity(availableSize.Height)) maxHeight = availableSize.Height;
                result = new SizeF(totalWidth, maxHeight);
            }
            else
            {
                float maxWidth = minMeasureSize;
                this.grip.Measure(new SizeF(availableSize.Width, availableSize.Height));

                float remainingHeight = availableSize.Height - this.grip.DesiredSize.Height;
                this.overflowButton.Measure(new SizeF(availableSize.Width, remainingHeight));

                remainingHeight -= this.overflowButton.DesiredSize.Height;
                this.itemsLayout.Measure(new SizeF(availableSize.Width, remainingHeight));

                maxWidth = Math.Max(this.grip.DesiredSize.Width, this.overflowButton.DesiredSize.Width);
                maxWidth = Math.Max(maxWidth, this.itemsLayout.DesiredSize.Width);

                float totalHeight = Math.Max(this.grip.DesiredSize.Height + this.overflowButton.DesiredSize.Height + this.itemsLayout.DesiredSize.Height,
                                                    minMeasureSize);

                if (!float.IsInfinity(availableSize.Width)) maxWidth = availableSize.Width;
                result = new SizeF(maxWidth, totalHeight);
            }
            Padding thickness = this.GetBorderThickness(true);
            result.Width += thickness.Left + thickness.Right + this.Padding.Left + this.Padding.Right + this.Margin.Left + this.Margin.Right;
            result.Height += thickness.Top + thickness.Bottom + this.Padding.Top + this.Padding.Bottom + this.Margin.Top + this.Margin.Bottom;
            return result;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF clientRect = GetClientRectangle(finalSize);

            if (this.orientation == Orientation.Horizontal)
            {
                PointF leftTopCorner = PointF.Empty;

                if (this.RightToLeft)
                {
                    this.overflowButton.Arrange(new RectangleF(leftTopCorner,
                                                           new SizeF(this.overflowButton.DesiredSize.Width, finalSize.Height)));
                    leftTopCorner.X += this.overflowButton.DesiredSize.Width;
                }
                else
                {
                    this.grip.Arrange(new RectangleF(leftTopCorner,
                                                     new SizeF(this.grip.DesiredSize.Width, finalSize.Height)));
                    leftTopCorner.X += this.grip.DesiredSize.Width;
                }

                float itemsLayoutWidth = (this.itemsLayout.StretchHorizontally) ?
                    finalSize.Width - this.grip.DesiredSize.Width - this.overflowButton.DesiredSize.Width :
                    this.itemsLayout.DesiredSize.Width;

                this.itemsLayout.Arrange(new RectangleF(new PointF(leftTopCorner.X, clientRect.Y),
                                                        new SizeF(itemsLayoutWidth, clientRect.Height)));
                leftTopCorner.X += this.itemsLayout.DesiredSize.Width;

                leftTopCorner.X = Math.Max(leftTopCorner.X, finalSize.Width - overflowButton.DesiredSize.Width);

                if (!this.RightToLeft)
                {
                    this.overflowButton.Arrange(new RectangleF(leftTopCorner,
                                                           new SizeF(this.overflowButton.DesiredSize.Width, finalSize.Height)));
                    leftTopCorner.X += this.overflowButton.DesiredSize.Width;
                }
                else
                {
                    this.grip.Arrange(new RectangleF(leftTopCorner,
                                                     new SizeF(this.grip.DesiredSize.Width, finalSize.Height)));
                    leftTopCorner.X += this.grip.DesiredSize.Width;
                }
                return finalSize;
            }
            else
            {
                PointF leftTopCorner = PointF.Empty;
                this.grip.Arrange(new RectangleF(leftTopCorner,
                                                 new SizeF(finalSize.Width, this.grip.DesiredSize.Height)));
                leftTopCorner.Y += this.grip.DesiredSize.Height;

                float itemsLayoutHeight = (this.itemsLayout.StretchVertically) ?
                    finalSize.Height - this.grip.DesiredSize.Height - this.overflowButton.DesiredSize.Height :
                    this.itemsLayout.DesiredSize.Height;

                this.itemsLayout.Arrange(new RectangleF(new PointF(clientRect.X, leftTopCorner.Y),
                                                        new SizeF(clientRect.Width, itemsLayoutHeight)));
                leftTopCorner.Y += this.itemsLayout.DesiredSize.Height;
                leftTopCorner.Y = Math.Max(leftTopCorner.Y, finalSize.Height - overflowButton.DesiredSize.Height);
                this.overflowButton.Arrange(new RectangleF(leftTopCorner,
                                                           new SizeF(finalSize.Width, this.overflowButton.DesiredSize.Height)));
                leftTopCorner.Y += this.overflowButton.DesiredSize.Height;
                return finalSize;
            }
        }

        protected internal override bool? ShouldSerializeProperty(PropertyDescriptor property)
        {
            if (property.Name == "MinSize")
            {
                return false;
            }

            return base.ShouldSerializeProperty(property);
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements(); 
            this.MinSize = new Size(30, 30);
            this.DrawBorder = true;
            this.Text = "";
            this.SetDefaultValueOverride(LightVisualElement.StretchHorizontallyProperty, false);
            this.SetDefaultValueOverride(LightVisualElement.StretchVerticallyProperty, true);
  
            this.items = new RadCommandBarBaseItemCollection();
            this.grip = new RadCommandBarGrip(this);
            this.overflowButton = new RadCommandBarOverflowButton(this);
            this.itemsLayout = new RadCommandBarItemsPanel(this.items, this.overflowButton.ItemsLayout);

            this.items.Owner = this.itemsLayout;
            this.items.ItemTypes = new Type[] {
                                       typeof(CommandBarButton),                 
                                       typeof(CommandBarDropDownButton),
                                       typeof(CommandBarDropDownList),
                                       typeof(CommandBarHostItem),
                                       typeof(CommandBarSeparator),
                                       typeof(CommandBarLabel),
                                       typeof(CommandBarTextBox),
                                       typeof(CommandBarToggleButton),
                                       typeof(CommandBarSplitButton)
                                   };

            this.Children.Add(this.grip);
            this.Children.Add(this.itemsLayout);
            this.Children.Add(this.overflowButton);

            this.WireEvents();

            this.localMinSize = this.MinSize;
        }

        public override string Text
        {
            get
            {
                return "";
            }
            set
            {
                base.Text = value;
            }
        }

        public override Size MinSize
        {
            get
            {
                return base.MinSize;
            }
            set
            {
                this.localMinSize = value;
                if (this.visibleInCommandBar)
                {
                    base.MinSize = value;
                }
            }
        }
        protected override void DisposeManagedResources()
        {
            this.UnwireEvents();

            base.DisposeManagedResources();
        }

        #endregion

        #region IItemsOwner Members

        /// <summary>
        /// Gets the items contained in the strip.
        /// </summary>
        [RadEditItemsAction]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)] 
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)] 
        [RadNewItem("", false, false, false)]
        public RadCommandBarBaseItemCollection Items
        {
            get
            {
                return this.items;
            }
        }

        #endregion

        #region Events management

        /// <summary>
        /// Raises the <see cref="E:CommandBarStripElement.BeginDrag"/> event
        /// </summary>
        /// <param name="sender">The element that is responsible for firing the event.</param>
        /// <param name="args">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnBeginDragging(object sender, CancelEventArgs args)
        {
            if (BeginDrag != null)
            {
                BeginDrag(sender, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CommandBarStripElement.EndDrag"/> event
        /// </summary>
        /// <param name="sender">The element that is responsible for firing the event.</param>
        /// <param name="args">A <see cref="T:System.EventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnEndDragging(object sender, EventArgs args)
        {
            if (EndDrag != null)
            {
                EndDrag(sender, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CommandBarStripElement.Drag"/> event
        /// </summary>
        /// <param name="sender">The element that is responsible for firing the event.</param>
        /// <param name="args">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnDragging(object sender, MouseEventArgs args)
        {
            if (Drag != null)
            {
                Drag(sender, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CommandBarStripElement.OverflowMenuOpening"/> event.
        /// </summary>
        /// <param name="sender">The element that is responsible for firing the event.</param>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnOverflowMenuOpening(object sender, CancelEventArgs e)
        {
            if (this.OverflowMenuOpening != null)
            {
                this.OverflowMenuOpening(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CommandBarStripElement.OverflowMenuOpened"/> event.
        /// </summary>
        /// <param name="sender">The element that is responsible for firing the event.</param>
        /// <param name="e">A <see cref="T:System.EventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnOverflowMenuOpened(object sender, EventArgs e)
        {
            if (this.OverflowMenuOpened != null)
            {
                this.OverflowMenuOpened(this, e);
            }
        }

        /// <summary>
        /// Raises the  <see cref="E:CommandBarStripElement.OverflowMenuClosing"/> event.
        /// </summary>
        /// <param name="sender">The element that is responsible for firing the event.</param>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the
        /// event data.</param> 
        protected virtual void OnOverflowMenuClosing(object sender, CancelEventArgs e)
        {
            if (this.OverflowMenuClosing != null)
            {
                this.OverflowMenuClosing(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CommandBarStripElement.OverflowMenuClosed"/> event.
        /// </summary>
        /// <param name="sender">The element that is responsible for firing the event.</param>
        /// <param name="e">A <see cref="T:System.EventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnOverflowMenuClosed(object sender, EventArgs e)
        {
            if (this.OverflowMenuClosed != null)
            {
                this.OverflowMenuClosed(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CommandBarStripElement.ItemsChanged"/> event.
        /// </summary>
        /// <param name="changed">The collection that is changed.</param>
        /// <param name="target">The targeted element of the collection.</param>
        /// <param name="operation">The type of the operation.</param>
        protected virtual void OnItemsChanged(RadCommandBarBaseItemCollection changed, RadCommandBarBaseItem target, ItemsChangeOperation operation)
        {
            if (this.ItemsChanged != null)
            {
                this.ItemsChanged(changed, target, operation);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CommandBarStripElement.ItemClicked"/> event.
        /// </summary>
        /// <param name="sender">The element that is responsible for firing the event.</param>
        /// <param name="e">A <see cref="T:System.EventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnItemClicked(object sender, EventArgs e)
        {
            if (this.ItemClicked != null)
            {
                this.ItemClicked(sender, e);
            }
        }

        /// <summary>
        /// Raises the  <see cref="E:CommandBarStripElement.VisibleInCommandBarChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the
        /// event data.</param>
        /// <returns>true if the event should be canceled, false otherwise.</returns>
        protected virtual bool OnVisibleInCommandBarChanging(CancelEventArgs e)
        {
            if (this.VisibleInCommandBarChanging != null)
            {
                this.VisibleInCommandBarChanging(this, e);
                return e.Cancel;
            }
            return false;
        }

        /// <summary>
        /// Raises the <see cref="E:CommandBarStripElement.VisibleInCommandBarChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.EventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnVisibleInCommandBarChanged(EventArgs e)
        {
            if (this.VisibleInCommandBarChanged != null)
            {
                this.VisibleInCommandBarChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the  <see cref="E:CommandBarStripElement.LineChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the
        /// event data.</param>
        /// <returns>true if the event should be canceled, false otherwise.</returns>
        protected virtual bool OnLineChanging(CancelEventArgs e)
        {
            if (this.LineChanging != null)
            {
                this.LineChanging(this, e);
                return e.Cancel;
            }
            return false;
        }

        /// <summary>
        /// Raises the <see cref="E:CommandBarStripElement.LineChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.EventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnLineChanged(EventArgs e)
        {
            if (this.LineChanged != null)
            {
                this.LineChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CommandBarStripElement.ItemOverflowed"/> event.
        /// </summary>
        /// <param name="sender">The element that is responsible for firing the event.</param>
        /// <param name="e">A <see cref="T:System.EventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnItemOverflowed(object sender, EventArgs e)
        {
            if (this.ItemOverflowed != null)
            {
                this.ItemOverflowed(sender, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CommandBarStripElement.ItemOutOfOverflow"/> event.
        /// </summary>
        /// <param name="sender">The element that is responsible for firing the event.</param>
        /// <param name="e">A <see cref="T:System.EventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnItemOutOfOverflow(object sender, EventArgs e)
        {
            if (this.ItemOutOfOverflow != null)
            {
                this.ItemOutOfOverflow(sender, e);
            }
        }

        /// <summary>
        /// Raises the  <see cref="E:CommandBarStripElement.ItemVisibleInStripChanging"/> event.
        /// </summary>
        /// <param name="sender">The element that is responsible for firing the event.</param>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the
        /// event data.</param>
        /// <returns>true if the event should be canceled, false otherwise.</returns>
        protected virtual bool OnItemVisibleInStripChanging(object sender, CancelEventArgs e)
        {
            if (this.ItemVisibleInStripChanging != null)
            {
                if (e == null)
                {
                    e = new CancelEventArgs();
                }

                this.ItemVisibleInStripChanging(sender, e);
                return e.Cancel;
            }
            return false;
        }

        /// <summary>
        /// Raises the <see cref="E:CommandBarStripElement.ItemVisibleInStripChanged"/> event.
        /// </summary>
        /// <param name="sender">The element that is responsible for firing the event.</param>
        /// <param name="e">A <see cref="T:System.EventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnItemVisibleInStripChanged(object sender, EventArgs e)
        {
            if (this.ItemVisibleInStripChanged != null)
            {
                this.ItemVisibleInStripChanged(sender, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CommandBarStripElement.OrientationChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.EventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnOrientationChanged(EventArgs e)
        {
            if (this.OrientationChanged != null)
            {
                this.OrientationChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the  <see cref="E:CommandBarStripElement.OrientationChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the
        /// event data.</param>
        /// <returns>true if the event should be canceled, false otherwise.</returns>
        protected virtual bool OnOrientationChanging(CancelEventArgs e)
        {
            if (this.OrientationChanging != null)
            {
                this.OrientationChanging(this, e);
                return e.Cancel;
            }
            return false;
        }

        #endregion

        #region Helpers

        private bool ShouldChangeLines()
        {
            if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                if (this.DesiredLocation.Y < this.ControlBoundingRectangle.Top - SystemInformation.DragSize.Height * 2)
                {
                    return true;
                }
                else if (this.DesiredLocation.Y > this.ControlBoundingRectangle.Bottom + SystemInformation.DragSize.Height * 2)
                {
                    return true;
                }
            }
            if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
            {
                if (this.DesiredLocation.X < this.ControlBoundingRectangle.Left - SystemInformation.DragSize.Height * 2)
                {
                    return true;
                }
                else if (this.DesiredLocation.X > this.ControlBoundingRectangle.Right + SystemInformation.DragSize.Height * 2)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Forces the drag to end.
        /// </summary>
        protected internal void ForceEndDrag()
        {
            this.grip.EndDrag();
        }

        protected internal void ForceBeginDrag()
        {
            this.grip.BeginDrag(new MouseEventArgs(MouseButtons.Left,1,this.Location.X,this.Location.Y,0));
        }

        protected override Image GetDragHintCore()
        {
            return null;
        }

        protected override bool CanDragCore(Point dragStartPoint)
        {
            return base.CanDragCore(dragStartPoint);
        }
         
        /// <summary>
        /// Measures the items with the size given and calculates the expected size of the strip
        /// including the <see cref="RadCommandBarGrip"/> and <see cref="RadCommandBarOverflowButton"/>.
        /// </summary>
        /// <param name="availableSize">The size to measure the items with.</param>
        /// <returns>The calculated size of the strip.</returns>
        public SizeF GetExpectedSize(SizeF availableSize)
        {
            if (!this.visibleInCommandBar || this.Visibility == ElementVisibility.Collapsed)
            {
                return new SizeF(0, 0);
            }

            SizeF result = this.itemsLayout.GetExpectedSize(availableSize);

            this.overflowButton.Measure(availableSize);
            this.grip.Measure(availableSize);

            if (this.orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                result.Width += this.overflowButton.DesiredSize.Width + this.grip.DesiredSize.Width;
                result.Height = Math.Max(this.overflowButton.DesiredSize.Height, result.Height);
                result.Height = Math.Max(this.grip.DesiredSize.Height, result.Height);
            }
            else
            {
                result.Height += this.overflowButton.DesiredSize.Height + this.grip.DesiredSize.Height;
                result.Width = Math.Max(this.overflowButton.DesiredSize.Width, result.Width);
                result.Width = Math.Max(this.grip.DesiredSize.Width, result.Width);
            }
            Padding thickness = this.GetBorderThickness(true);
            result.Width += thickness.Left + thickness.Right + this.Padding.Left + this.Padding.Right + this.Margin.Left + this.Margin.Right;
            result.Height += thickness.Top + thickness.Bottom + this.Padding.Top + this.Padding.Bottom + this.Margin.Top + this.Margin.Bottom;

            return result;
        }

        /// <summary>
        /// Subscribes to the children's events.
        /// </summary>
        protected virtual void WireEvents()
        {
            this.items.ItemsChanged += new RadCommandBarBaseItemCollectionItemChangedDelegate(OnItemsChanged);// += new ItemChangedDelegate(OnItemsChanged);
            this.overflowButton.OverflowMenuOpening += new CancelEventHandler(OnOverflowMenuOpening);
            this.overflowButton.OverflowMenuOpened += new EventHandler(OnOverflowMenuOpened);
            this.overflowButton.OverflowMenuClosing += new CancelEventHandler(OnOverflowMenuClosing);
            this.overflowButton.OverflowMenuClosed += new EventHandler(OnOverflowMenuClosed);
            this.itemsLayout.ItemOverflowed += new EventHandler(OnItemOverflowed);
            this.itemsLayout.ItemOutOfOverflow += new EventHandler(OnItemOutOfOverflow);
        }

        /// <summary>
        /// Unsubscribe from the children's events.
        /// </summary>
        protected virtual void UnwireEvents()
        {
            this.items.ItemsChanged -= new RadCommandBarBaseItemCollectionItemChangedDelegate(OnItemsChanged);
            this.overflowButton.OverflowMenuOpening -= new CancelEventHandler(OnOverflowMenuOpening);
            this.overflowButton.OverflowMenuOpened -= new EventHandler(OnOverflowMenuOpened);
            this.overflowButton.OverflowMenuClosing -= new CancelEventHandler(OnOverflowMenuClosing);
            this.overflowButton.OverflowMenuClosed -= new EventHandler(OnOverflowMenuClosed);
            this.itemsLayout.ItemOverflowed -= new EventHandler(OnItemOverflowed);
            this.itemsLayout.ItemOutOfOverflow -= new EventHandler(OnItemOutOfOverflow);
        }

        /// <summary>
        /// Applies an orientation to the strip and its children.
        /// </summary>
        /// <param name="value">The orientation to apply.</param>
        protected internal void SetOrientationCore(Orientation value)
        {
            this.itemsLayout.Orientation = value;
            this.grip.Orientation = value;
            this.overflowButton.Orientation = value;
            this.cachedDesiredLocation = new PointF(this.DesiredLocation.Y, this.DesiredLocation.X);
            this.orientation = value;

            if (value == System.Windows.Forms.Orientation.Vertical)
            {
                this.GradientAngle += 90;
            }
            else
            {
                this.GradientAngle -= 90;
            }

            bool isStretchedHorizontally = this.StretchHorizontally;
            this.StretchHorizontally = this.StretchVertically;
            this.StretchVertically = isStretchedHorizontally;
        }

        #endregion
    }
}
