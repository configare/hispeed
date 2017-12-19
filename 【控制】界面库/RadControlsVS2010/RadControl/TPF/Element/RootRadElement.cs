using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Design;
using Telerik.WinControls.Paint;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls
{
    public class RootRadElement : RadItem //VisualElement
    {
        #region BitState Keys

        internal const ulong DefaultStretchHorizontallyStateKey = RadItemLastStateKey << 1;
        internal const ulong DefaultStretchVerticallyStateKey = DefaultStretchHorizontallyStateKey << 1;
        internal const ulong ControlInitiatedPropertyChangeStateKey = DefaultStretchVerticallyStateKey << 1;
        internal const ulong RootElementInitiatedPropertyChangeStateKey = ControlInitiatedPropertyChangeStateKey << 1;
        internal const ulong DisableControlSizeSetStateKey = RootElementInitiatedPropertyChangeStateKey << 1;
        internal const ulong ForcingLocationStateKey = DisableControlSizeSetStateKey << 1;

        #endregion

        public static readonly RoutedEvent OnRoutedImageListChanged = RootRadElement.RegisterRoutedEvent("OnRoutedImageListChanged", typeof(RootRadElement));

        /// <summary>
        /// Tunnels when the AutoSize property of RadControl changes in order to notify any children that should take special actions.
        /// </summary>
        public static readonly RoutedEvent AutoSizeChangedEvent = RadElement.RegisterRoutedEvent("AutoSizeChangedEvent", typeof(RootRadElement));

        /// <summary>
        /// Tunnels when some of the stretch properties (horizontal or vertical) has changed in order to notify any children that should take special actions.
        /// </summary>
        public static readonly RoutedEvent StretchChangedEvent = RadElement.RegisterRoutedEvent("StretchChangedEvent", typeof(RootRadElement));

        /// <summary>
        /// Tunnels when the layout has been suspended in order to notify any children that should take special actions in this case - like RadHostItem.
        /// </summary>
        public static RoutedEvent RootLayoutSuspendedEvent = RadElement.RegisterRoutedEvent("RootLayoutSuspendedEvent", typeof(RootRadElement));

        /// <summary>
        /// Tunnels when the layout has been resumed in order to notify any children that should take special actions in this case - like RadHostItem.
        /// </summary>
        public static RoutedEvent RootLayoutResumedEvent = RadElement.RegisterRoutedEvent("RootLayoutResumedEvent", typeof(RootRadElement));

        [DefaultValue(RadAutoSizeMode.FitToAvailableSize)]
        public override RadAutoSizeMode AutoSizeMode
        {
            get
            {
                return base.AutoSizeMode;
            }
            set
            {
                base.AutoSizeMode = value;
            }
        }

        public override bool OverridesDefaultLayout
        {
            get
            {
                return true;
            }
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.ShouldHandleMouseInput = false;
        }

        public static RadProperty UsePaintCacheProperty = RadProperty.Register(
            "UsePaintCache", typeof(bool), typeof(RootRadElement), new RadElementPropertyMetadata(
                true, ElementPropertyOptions.AffectsDisplay));

        [RadPropertyDefaultValue("UsePaintCache", typeof(RootRadElement))]
        public bool UsePaintCache
        {
            get
            {
                return (bool)this.GetValue(UsePaintCacheProperty);
            }
            set
            {
                this.SetValue(UsePaintCacheProperty, value);
            }
        }

        public static RadProperty ApplyShapeToControlProperty = RadProperty.Register(
            "ApplyShapeToControl", typeof(bool), typeof(RootRadElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay));
        /// <summary>
        /// Gets or sets value indicating whether the shape set to the root element would be applied as a region to 
        /// the RadControl that contains the element.
        /// </summary>
        [Description("Gets or sets value indicating whether the shape set to the root element" +
                    "would be applied as a region to the RadControl that contains the element.")]
        [RadPropertyDefaultValue("ApplyShapeToControl", typeof(RootRadElement))]
        public bool ApplyShapeToControl
        {
            get
            {
                return (bool)this.GetValue(ApplyShapeToControlProperty);
            }
            set
            {
                this.SetValue(ApplyShapeToControlProperty, value);
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool RightToLeft
        {
            get
            {
                return base.RightToLeft;
            }
            set
            {
                base.RightToLeft = value;
            }
        }

        /// <summary>
        /// Determines whether to use compatible text rendering engine (GDI+) or not (GDI). 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool UseCompatibleTextRendering
        {
            get
            {
                return base.UseCompatibleTextRendering;
            }
            set
            {
                base.UseCompatibleTextRendering = value;
            }
        }

        //[Localizable(true), Description("Gets or sets the tooltip text associated with this item.")]
        //[DefaultValue(""), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[Browsable(false)]
        //public override string ToolTipText
        //{ 
        //    get
        //    {
        //        return base.ToolTipText;
        //    }
        //    set
        //    {
        //        base.ToolTipText = value;
        //    }
        //}

        //[DefaultValue(false),
        //Category(RadDesignCategory.BehaviorCategory),
        //Description("ToolStripItemAutoToolTipDescr"),
        //DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        //Browsable(false)]
        //public override bool AutoToolTip
        //{
        //    get
        //    {
        //        return base.AutoToolTip;
        //    }
        //    set
        //    {
        //        base.AutoToolTip = value;
        //    }
        //}

        protected override bool PerformLayoutTransformation(ref RadMatrix matrix)
        {
            // No layout transformation needed for the root element as it represents the control itself.
            return false;
        }

        protected override void OnDisplayPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (!this.IsInValidState(true))
                return;

            this.ElementTree.ComponentTreeHandler.OnDisplayPropertyChanged(e);

            base.OnDisplayPropertyChanged(e);
        }

        public static RadProperty ControlBoundsProperty =
            RadProperty.Register("ControlBounds", typeof(Rectangle), typeof(RadElement),
                new RadElementPropertyMetadata(Rectangle.Empty,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        /// <summary>
        ///     Gets or sets a value corresponding to the bounding rectangle of the owning Control.
        /// </summary>
        [Description("Represents the owning control bounding rectangle")]
        [RadPropertyDefaultValue("ControlBounds", typeof(RootRadElement)), Category(RadDesignCategory.LayoutCategory)]
        public virtual Rectangle ControlBounds
        {
            get
            {
                return (Rectangle)this.GetValue(RootRadElement.ControlBoundsProperty);
            }
            set
            {
                this.SetValue(RootRadElement.ControlBoundsProperty, value);
            }
        }

        internal void SaveCurrentStretchModeAsDefault()
        {
            this.BitState[DefaultStretchHorizontallyStateKey] = this.StretchHorizontally;
            this.BitState[DefaultStretchVerticallyStateKey] = this.StretchVertically;
        }

        internal void ForceLocationTo(Point newLocation)
        {
            Rectangle bounds = this.Bounds;
            if (bounds.Location != newLocation)
            {
                this.BitState[ForcingLocationStateKey] = true;
                SetBoundsCore(new Rectangle(newLocation, bounds.Size));
                this.BitState[ForcingLocationStateKey] = false;
            }
        }

        protected internal override object CoerceValue(RadPropertyValue propVal, object baseValue)
        {
            if (propVal.Property == RootRadElement.ControlBoundsProperty)
            {
                return this.ElementTree.Control.Bounds;
            }

            return base.CoerceValue(propVal, baseValue);
        }

        protected override void OnPropertyChanging(RadPropertyChangingEventArgs args)
        {
            if (this.GetBitState(ForcingLocationStateKey) && args.Property == RadElement.BoundsProperty)
                return;
            base.OnPropertyChanging(args);
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (this.ElementTree == null)
            {
                return;
            }

            if (this.GetBitState(ForcingLocationStateKey) && e.Property == RadElement.BoundsProperty)
            {
                return;
            }

            base.OnPropertyChanged(e);

            if (!this.GetBitState(ControlInitiatedPropertyChangeStateKey))
            {
                this.BitState[RootElementInitiatedPropertyChangeStateKey] = true;
                if (e.Property == RootRadElement.ControlBoundsProperty)
                {
                    this.ElementTree.Control.Bounds = (Rectangle)e.NewValue;
                }
                else if (e.Property == RootRadElement.StretchHorizontallyProperty)
                {
                    RoutedEventArgs args = new RoutedEventArgs(new StretchEventArgs(true, (bool)e.NewValue), StretchChangedEvent);
                    this.RaiseTunnelEvent(this, args);
                }
                else if (e.Property == RootRadElement.StretchVerticallyProperty)
                {
                    RoutedEventArgs args = new RoutedEventArgs(new StretchEventArgs(false, (bool)e.NewValue), StretchChangedEvent);
                    this.RaiseTunnelEvent(this, args);
                }
                else if (e.Property == RadElement.BoundsProperty)
                {
                    if (!this.UseNewLayoutSystem)
                    {
                        if (this.AutoSize && this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize)
                        {
                            //this.Size = this.ElementTree.Control.Size;
                        }
                        else
                        {
                            this.ElementTree.LockControlLayout();
                            this.ElementTree.Control.Size = ((Rectangle)e.NewValue).Size;
                            this.ElementTree.UnlockControlLayout();
                        }
                    }
                    if ((this.Shape != null) && (this.ElementTree != null) &&
                        this.ApplyShapeToControl)
                    {
                        Rectangle oldBounds = (Rectangle)e.OldValue;
                        Rectangle newBounds = (Rectangle)e.NewValue;
                        //change region only of the Size has changed
                        if (oldBounds.Size != newBounds.Size)
                        {
                            Rectangle boundsRect = new Rectangle(Point.Empty, new Size(this.Size.Width, this.Size.Height));
                            this.ElementTree.Control.Region = this.Shape.CreateRegion(boundsRect);
                        }
                    }
                }
                else if ((e.Property == ShapeProperty) && this.ApplyShapeToControl)
                {
                    ElementShape shape = e.NewValue as ElementShape;
                    if ((shape != null) && (this.ElementTree != null))
                    {
                        this.ElementTree.Control.Region = new Region(shape.GetElementShape(this));
                    }
                }
                else if (e.Property == ApplyShapeToControlProperty)
                {
                    if ((bool)e.NewValue && this.Shape != null)
                    {
                        this.ElementTree.Control.Region = new Region(this.Shape.GetElementShape(this));
                    }
                    else
                    {
                        this.ElementTree.Control.Region = null;
                    }
                }

                //ask the component handler to sync its ambient properties - e.g. BackColor, ForeColor, etc.
                this.ElementTree.ComponentTreeHandler.OnAmbientPropertyChanged(e.Property);
                //Note ! the following lines should be the last in the routine in order to avoid infinite loop when the corresponding control's property changes
                this.BitState[RootElementInitiatedPropertyChangeStateKey] = false;
            }
            this.BitState[ControlInitiatedPropertyChangeStateKey] = false;
        }

        protected override void OnLayoutPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (this.UseNewLayoutSystem)
            {
                if (e.Property == RadElement.StretchHorizontallyProperty || e.Property == RadElement.StretchVerticallyProperty)
                {
                    IComponentTreeHandler ctl = this.ElementTree.ComponentTreeHandler;
                    if (ctl != null)
                    {
                        ctl.ElementTree.ResetSize();
                        return;
                    }
                }
            }
            base.OnLayoutPropertyChanged(e);
        }

        protected override void OnBoundsChanged(RadPropertyChangedEventArgs e)
        {
            base.OnBoundsChanged(e);

            if (this.UseNewLayoutSystem)
            {
                if (!this.LayoutManager.IsUpdating)
                {
                    Control ctl = this.ElementTree.Control;
                    if (ctl != null && ctl.AutoSize)
                    {
                        // Set only Size here - OnLocationChanged() will set Location
                        ctl.Size = this.Size;
                    }
                }
            }
        }

        protected override void OnLocationChanged(RadPropertyChangedEventArgs e)
        {
            base.OnLocationChanged(e);

            if (this.UseNewLayoutSystem)
            {
                if (!this.LayoutManager.IsUpdating)
                {
                    Control ctl = this.ElementTree.Control;
                    if (ctl != null)
                    {
                        //old code
                        //ctl.Location = this.Location;

                        //FIXED: the location relationship between the rootradelement and the radcontrol when set through bounds
                        //WINF-10440
                        ctl.Location = ((Rectangle)e.NewValue).Location;
                    }
                }
            }
        }

        protected override SizeF MeasureCore(SizeF availableSize)
        {
            SizeF ownAvailableSize = new SizeF(Math.Max(availableSize.Width, 0), Math.Max(availableSize.Height, 0));
            MinMax max1 = new MinMax(this);

            ownAvailableSize.Width = Math.Max(max1.minWidth, Math.Min(ownAvailableSize.Width, max1.maxWidth));
            ownAvailableSize.Height = Math.Max(max1.minHeight, Math.Min(ownAvailableSize.Height, max1.maxHeight));

            if (!this.StretchHorizontally)
                ownAvailableSize.Width = float.PositiveInfinity;
            if (!this.StretchVertically)
                ownAvailableSize.Height = float.PositiveInfinity;

            if (max1.maxWidth > 0 && ownAvailableSize.Width > max1.maxWidth  )
            {
                ownAvailableSize.Width = max1.maxWidth;
            }

            if (max1.maxHeight > 0 && ownAvailableSize.Height > max1.maxHeight )
            {
                ownAvailableSize.Height = max1.maxHeight;
            }

            SizeF measuredSize = this.MeasureOverride(ownAvailableSize);

            measuredSize = new SizeF(Math.Max(measuredSize.Width, max1.minWidth), Math.Max(measuredSize.Height, max1.minHeight));

            if (measuredSize.Width > max1.maxWidth && max1.maxWidth > 0)
            {
                measuredSize.Width = max1.maxWidth;
            }
            if (measuredSize.Height > max1.maxHeight && max1.maxHeight > 0)
            {
                measuredSize.Height = max1.maxHeight;
            }

            float fullWidth = measuredSize.Width;
            float fullHeight = measuredSize.Height;

            return new SizeF(Math.Max(0, fullWidth), Math.Max(0, fullHeight));
        }

        protected override void ArrangeCore(RectangleF finalRect)
        {
            Control ctl = this.ElementTree.Control;

            SizeF availableSize = finalRect.Size;
            availableSize.Width = Math.Max(0, availableSize.Width);
            availableSize.Height = Math.Max(0, availableSize.Height);
            SizeF desiredOwnSize = this.DesiredSize;

            if (!this.StretchHorizontally)
            {
                availableSize.Width = desiredOwnSize.Width;
            }
            if (!this.StretchVertically)
            {
                availableSize.Height = desiredOwnSize.Height;
            }

            MinMax max1 = new MinMax(this);

            SizeF ownAvailableSize = new SizeF(Math.Max(availableSize.Width, 0), Math.Max(availableSize.Height, 0));
            ownAvailableSize.Width = Math.Max(max1.minWidth, Math.Min(ownAvailableSize.Width, max1.maxWidth));
            ownAvailableSize.Height = Math.Max(max1.minHeight, Math.Min(ownAvailableSize.Height, max1.maxHeight));

            SizeF size7 = this.ArrangeOverride(ownAvailableSize);
            SizeF size8 = size7;

            if (max1.maxWidth > 0)
                size8.Width = Math.Min(size7.Width, max1.maxWidth);
            if (max1.maxHeight > 0)
                size8.Height = Math.Min(size7.Height, max1.maxHeight);

            Rectangle alignedRect = new Rectangle(Point.Ceiling(finalRect.Location), Size.Ceiling(size8));
            this.Bounds = alignedRect;
            if (ctl != null && ctl.AutoSize && !this.GetBitState(DisableControlSizeSetStateKey))
                ctl.Size = alignedRect.Size;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            for (int i = 0; i < this.Children.Count; i++)
            {
                RadElement child = this.Children[i];
                child.Arrange(new RectangleF(PointF.Empty, finalSize));
            }
            return finalSize;
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF totalSize = SizeF.Empty;
            for (int i = 0; i < this.Children.Count; i++)
            {
                RadElement child = this.Children[i];
                child.Measure(availableSize);

                SizeF s = child.DesiredSize;
                if (totalSize.Width < s.Width)
                    totalSize.Width = s.Width;
                if (totalSize.Height < s.Height)
                    totalSize.Height = s.Height;
            }
            return totalSize;
        }

        protected internal override RectangleF GetArrangeRect(RectangleF proposed)
        {
            if (proposed.Width > 0 && proposed.Height > 0)
            {
                return proposed;
            }

            RectangleF result = this.ElementTree.Control.Bounds;
            if (float.IsPositiveInfinity(proposed.Width))
            {
                result.Width = this.DesiredSize.Width;
            }
            if (float.IsPositiveInfinity(proposed.Height))
            {
                result.Height = this.DesiredSize.Height;
            }

            return result;
        }

        /// <summary>
        /// Paints the RootElement and its element tree. Intended for use by RadControl inheritors.
        /// </summary>
        /// <param name="graphics">IGrpahics object to be used to paint elements</param>
        /// <param name="clipRectangle">Clipping rectangle to be painted. Only those elements from the tree which intersect with this rectangle will be painted.</param>
        public void Paint(IGraphics graphics, Rectangle clipRectangle)
        {
            this.PaintOverride(graphics, clipRectangle, 0f, new SizeF(1, 1), false);
        }

        /// <summary>
        /// Forces theme re-applying to all elements in the element tree.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ForceApplyTheme()
        {
            this.ApplyThemeRecursive();
        }

        internal void ControlThemeChanged()
        {
            if (this.ElementTree == null)
            {
                return;
            }

            this.ElementTree.Control.SuspendLayout();

            //this.ApplyThemeRecursive();
            this.ElementTree.StyleManager.AttachStylesToElementTree();

            this.ElementTree.Control.ResumeLayout(true);
        }

        internal void NotifyControlImageListChanged()
        {
            RoutedEventArgs args = new RoutedEventArgs(EventArgs.Empty, OnRoutedImageListChanged);
            this.RaiseTunnelEvent(this, args);
        }

        internal void OnControlAutoSizeChanged(bool autoSize)
        {
            if (!this.UseNewLayoutSystem)
            {
                if (autoSize)
                    this.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
                else
                    this.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            }

            RoutedEventArgs args = new RoutedEventArgs(new AutoSizeEventArgs(autoSize), AutoSizeChangedEvent);
            this.RaiseTunnelEvent(this, args);
        }

        static readonly ArrayList excludedRootElementProps = new ArrayList(new string[] 
			{
                "Visibility",
                "Font",
                "BackColor",
				"FitToAvailableSize",
				"Bounds",
				"Name",
				"ShouldHandleMouseInput",
				"UsePaintCache"
			});

        protected internal override bool? ShouldSerializeProperty(PropertyDescriptor property)
        {
            if (property.Name == "StretchHorizontally")
            {
                bool value = (bool)property.GetValue(this);
                return value != this.GetBitState(DefaultStretchHorizontallyStateKey);
            }
            else if (property.Name == "StretchVertically")
            {
                bool value = (bool)property.GetValue(this);
                return value != this.GetBitState(DefaultStretchVerticallyStateKey);
            }
            else if (property.Name == "ToolTipText")
            {
                string value = (string)property.GetValue(this);
                return !string.IsNullOrEmpty(value);
            }

            if (excludedRootElementProps.Contains(property.Name))
            {
                return false;
            }

            return null;//Default should serialize
        }

        protected internal override object GetInheritedValue(RadProperty property)
        {
            if (this.ElementTree == null)
            {
                return base.GetInheritedValue(property);
            }

            return this.ElementTree.ComponentTreeHandler.GetAmbientPropertyValue(property);
        }

        public override bool IsElementVisible
        {
            get
            {
                if (this.ElementTree != null)
                {
                    return this.Visibility == ElementVisibility.Visible && this.ElementTree.Control.Visible;
                }

                return base.IsElementVisible;
            }
        }
    }

    public class AutoSizeEventArgs : EventArgs
    {
        public readonly bool AutoSize;

        public AutoSizeEventArgs(bool autoSize)
        {
            this.AutoSize = autoSize;
        }
    }

    public class StretchEventArgs : EventArgs
    {
        public readonly bool IsStretchHorizontal;
        public readonly bool StretchValue;

        public StretchEventArgs(bool isStretchHorizontal, bool stretchValue)
        {
            this.IsStretchHorizontal = isStretchHorizontal;
            this.StretchValue = stretchValue;
        }
    }
}
