using System;
using System.ComponentModel;
using System.Drawing;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Paint;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;
using Telerik.WinControls.Design;
using System.Drawing.Design;

namespace Telerik.WinControls.Primitives
{
    /// <summary><para>Represents a border that is drawn on the screen.</para></summary>
    public class BorderPrimitive : BasePrimitive, IBorderElement, IPrimitiveElement
    {
        #region Fields

        private BorderPrimitiveImpl borderPrimitiveImpl;
        private BorderPrimitive childPrimitive;
        private Padding ownBorderThickness = Padding.Empty;

        #endregion

        #region Constructor

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.borderPrimitiveImpl = new BorderPrimitiveImpl(this, this);
        }

        #endregion

        #region IPrimitiveElement Members

        float IPrimitiveElement.BorderThickness
        {
            get
            {
                return this.Width;
            }
        }

        RectangleF IPrimitiveElement.GetPaintRectangle(float left, float angle, SizeF scale)
        {
            return this.GetPaintRectangle(left, angle, scale);
        }

        RectangleF IPrimitiveElement.GetExactPaintingRectangle(float angle, SizeF scale)
        {
            return base.GetPatchedRect(new RectangleF(0, 0, this.Size.Width - 1, this.Size.Height - 1), angle, scale);
        }

        bool IPrimitiveElement.IsDesignMode
        {
            get { return this.IsDesignMode; }
        }

        protected virtual bool ShouldUsePaintBuffer()
        {
            return true;
        }

        ElementShape IPrimitiveElement.GetCurrentShape()
        {
            return this.GetCurrentShape();
        }

        bool IPrimitiveElement.ShouldUsePaintBuffer()
        {
            return this.ShouldUsePaintBuffer();
        }

        #endregion

        #region Overrides

        public override Filter GetStylablePropertiesFilter()
        {
            return new OrFilter(Telerik.WinControls.PropertyFilter.BorderPrimitiveFilter,
                Telerik.WinControls.PropertyFilter.AppearanceFilter,
                Telerik.WinControls.PropertyFilter.BehaviorFilter);
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF res = base.MeasureOverride(availableSize);

            if (this.childPrimitive != null)
            {
                this.childPrimitive.Measure(
                    new SizeF(availableSize.Width - this.ownBorderThickness.Horizontal, availableSize.Height - this.ownBorderThickness.Vertical));
            }

            return res;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            SizeF res = base.ArrangeOverride(finalSize);

            if (this.childPrimitive != null)
            {
                this.childPrimitive.Arrange(new RectangleF(new Point(this.ownBorderThickness.Left, this.ownBorderThickness.Top),
                    new SizeF(finalSize.Width - this.ownBorderThickness.Horizontal, finalSize.Height - this.ownBorderThickness.Vertical)));
            }

            return res;
        }

        protected internal override object GetDefaultValue(RadPropertyValue propVal, object baseDefaultValue)
        {
            RadProperty property = propVal.Property;

            if (property == ForeColorProperty)
            {
                return SystemColors.ControlDarkDark;
            }

            if (property == AutoSizeModeProperty)
            {
                return RadAutoSizeMode.Auto;
            }

            if (property == FitToSizeModeProperty)
            {
                return RadFitToSizeMode.FitToParentBounds;
            }

            if (property == SmoothingModeProperty)
            {
                return SmoothingMode.AntiAlias;
            }

            return base.GetDefaultValue(propVal, baseDefaultValue);
        }

        protected internal override bool? ShouldSerializeProperty(PropertyDescriptor property)
        {
            if (property.Name == VisualElement.SmoothingModeProperty.Name)
            {
                return this.SmoothingMode != SmoothingMode.AntiAlias;
            }

            if (property.Name == RadElement.FitToSizeModeProperty.Name)
            {
                return this.FitToSizeMode != RadFitToSizeMode.FitToParentBounds;
            }

            return base.ShouldSerializeProperty(property);
        }

        /// <summary>
        /// Virtual method that paints the primitive on the screen. It may be overriden by
        /// the derived types.
        /// </summary>
        public override void PaintPrimitive(IGraphics graphics, float angle, SizeF scale)
        {
            this.borderPrimitiveImpl.PaintBorder(graphics, angle, scale);
        }

        protected override bool ShouldPaintUsingParentShape
        {
            get
            {
                return this.PaintUsingParentShape;
            }
        }

        protected override void OnParentChanged(RadElement previousParent)
        {
            base.OnParentChanged(previousParent);

            this.SynchronizeWithParentBorderThickness();
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (e.Property == BorderPrimitive.BorderBoxStyleProperty ||
                e.Property == BorderPrimitive.BorderDrawModeProperty ||
                e.Property == BorderPrimitive.WidthProperty ||
                e.Property == BorderPrimitive.LeftWidthProperty ||
                e.Property == BorderPrimitive.TopWidthProperty ||
                e.Property == BorderPrimitive.RightWidthProperty ||
                e.Property == BorderPrimitive.BottomWidthProperty ||
                e.Property == RadElement.VisibilityProperty)
            {
                this.ownBorderThickness = this.GetOwnBorderThickness();
                this.SynchronizeWithParentBorderThickness();
            }

            base.OnPropertyChanged(e);
        }

        protected override void OnChildrenChanged(RadElement child, ItemsChangeOperation changeOperation)
        {
            base.OnChildrenChanged(child, changeOperation);

            if (child is BorderPrimitive)
            {
                this.SynchronizeWithParentBorderThickness();

                if (this.childPrimitive == null)
                {
                    this.childPrimitive = child as BorderPrimitive;
                }
                else if (child is BorderPrimitive && this.childPrimitive == child
                    && changeOperation == ItemsChangeOperation.Removed)
                {
                    this.childPrimitive = null;

                    foreach (RadElement item in this.Children)
                    {
                        if (item is BorderPrimitive)
                        {
                            this.childPrimitive = item as BorderPrimitive;
                            this.SynchronizeWithParentBorderThickness();
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region Private Implementation

        private void SynchronizeWithParentBorderThickness()
        {
            RadElement parent = this.Parent;

            if (!(parent is BorderPrimitive))
            {
                if (parent == null)
                    return;
                if (this.Visibility == ElementVisibility.Collapsed)
                {
                    parent.BorderThickness = new Padding(0);
                    return;
                }

                parent.BorderThickness = this.GetBorderThickness();
            }
        }

        private Padding GetOwnBorderThickness()
        {
            Padding result = new Padding();
            switch (this.BoxStyle)
            {
                case BorderBoxStyle.SingleBorder:
                    result.All = (int)Math.Round(this.Width);
                    break;
                case BorderBoxStyle.FourBorders:
                    result.Left = (int)Math.Round(this.LeftWidth);
                    result.Top = (int)Math.Round(this.TopWidth);
                    result.Right = (int)Math.Round(this.RightWidth);
                    result.Bottom = (int)Math.Round(this.BottomWidth);
                    break;
                case BorderBoxStyle.OuterInnerBorders:
                    {
                        int width = (int)Math.Round(this.Width);
                        //seems like the following is correct but some elements like
                        //- DropDown, Split buttons, text box in the combo get clipped at the bottom this way
                        //if (width == 1)
                        //{
                        //    width = 2;
                        //}
                        result.All = width;
                    }
                    break;
            }

            return result;
        }

        internal Padding GetBorderThickness()
        {
            Padding result = this.ownBorderThickness = this.GetOwnBorderThickness();

            for (int i = 0; i < this.Children.Count; i++)
            {
                if (this.Children[i] is BorderPrimitive)
                {
                    BorderPrimitive childBorder = this.Children[i] as BorderPrimitive;
                    result = Padding.Add(result, childBorder.GetBorderThickness());
                }
            }
            this.BorderThickness = result;
            return result;
        }

        #endregion

        #region Rad Properties

        public static readonly RadProperty BorderBoxStyleProperty = RadProperty.Register(
            "BoxStyle", typeof(BorderBoxStyle), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                BorderBoxStyle.SingleBorder, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

        public static readonly RadProperty BorderDrawModeProperty = RadProperty.Register(
            "BorderDrawMode", typeof(BorderDrawModes), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                BorderDrawModes.RightOverTop, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty WidthProperty = RadProperty.Register(
            "Width", typeof(float), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                1f, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

        public static readonly RadProperty LeftWidthProperty = RadProperty.Register(
            "LeftWidth", typeof(float), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                1f, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

        public static readonly RadProperty TopWidthProperty = RadProperty.Register(
            "TopWidth", typeof(float), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                1f, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

        public static readonly RadProperty RightWidthProperty = RadProperty.Register(
            "RightWidth", typeof(float), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                1f, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

        public static readonly RadProperty BottomWidthProperty = RadProperty.Register(
            "BottomWidth", typeof(float), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                1f, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

        public static readonly RadProperty LeftColorProperty = RadProperty.Register(
            "LeftColor", typeof(Color), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                SystemColors.ControlDark, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty TopColorProperty = RadProperty.Register(
            "TopColor", typeof(Color), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                SystemColors.ControlDark, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty RightColorProperty = RadProperty.Register(
            "RightColor", typeof(Color), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                SystemColors.ControlDark, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty BottomColorProperty = RadProperty.Register(
            "BottomColor", typeof(Color), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                SystemColors.ControlDark, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty LeftShadowColorProperty = RadProperty.Register(
            "LeftShadowColor", typeof(Color), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                Color.Empty, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty TopShadowColorProperty = RadProperty.Register(
            "TopShadowColor", typeof(Color), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                Color.Empty, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty RightShadowColorProperty = RadProperty.Register(
            "RightShadowColor", typeof(Color), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                Color.Empty, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty BottomShadowColorProperty =
            RadProperty.Register("BottomShadowColor", typeof(Color), typeof(BorderPrimitive),
                new RadElementPropertyMetadata(Color.Empty, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty GradientAngleProperty = RadProperty.Register(
            "GradientAngle", typeof(float), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                270f, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty GradientStyleProperty = RadProperty.Register(
            "GradientStyle", typeof(GradientStyles), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                GradientStyles.Solid, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ForeColor2Property = RadProperty.Register(
            "ForeColor2", typeof(Color), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                SystemColors.ControlDark, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty ForeColor3Property = RadProperty.Register(
            "ForeColor3", typeof(Color), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                SystemColors.ControlDark, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty ForeColor4Property = RadProperty.Register(
            "ForeColor4", typeof(Color), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                SystemColors.ControlDark, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty InnerColorProperty = RadProperty.Register(
            "InnerColor", typeof(Color), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                SystemColors.ControlLightLight, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty InnerColor2Property = RadProperty.Register(
            "InnerColor2", typeof(Color), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                SystemColors.Control, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty InnerColor3Property = RadProperty.Register(
            "InnerColor3", typeof(Color), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                SystemColors.ControlDark, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty InnerColor4Property = RadProperty.Register(
            "InnerColor4", typeof(Color), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                SystemColors.ControlDarkDark, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static readonly RadProperty PaintUsingParentShapeProperty = RadProperty.Register(
            "PaintUsingParentShape", typeof(bool), typeof(BorderPrimitive), new RadElementPropertyMetadata(
                true, ElementPropertyOptions.AffectsDisplay));

        #endregion

        #region CLR Properties

        [DefaultValue(RadFitToSizeMode.FitToParentBounds)]
        public override RadFitToSizeMode FitToSizeMode
        {
            get
            {
                return base.FitToSizeMode;
            }
            set
            {
                base.FitToSizeMode = value;
            }
        }

        /// <summary>
        /// 	<para class="MsoNormal" style="MARGIN: 0in 0in 0pt">
        /// 		<span style="FONT-SIZE: 8pt; COLOR: black; FONT-FAMILY: Verdana">Gets or sets the
        ///     Border style. The two possible values are SingleBorder and FourBorder. In the
        ///     single border case, all four sides share the same appearance although the entire
        ///     border may have gradient. In four border case, each of the four sides may differ in
        ///     appearance. For example, the left border may have different color, shadowcolor, and
        ///     width from the rest. When SingleBorder is chosen, you should use the general
        ///     properties such as width and color, and respectively, when the FourBorder style is
        ///     chosen you should use properties prefixed with the corresponding side, for example,
        ///     LeftColor, LeftWidth for the left side.</span></para>
        /// </summary>
        [RadPropertyDefaultValue("BoxStyle", typeof(BorderPrimitive))]
        [Category(BasePrimitive.BoxCategory)]
        [Description("Determine the sizing style of the border's sides")]
        public BorderBoxStyle BoxStyle
        {
            get
            {
                return (BorderBoxStyle)this.GetValue(BorderBoxStyleProperty);
            }
            set
            {
                this.SetValue(BorderBoxStyleProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [RadPropertyDefaultValue("BorderDrawModes", typeof(BorderPrimitive))]
        [Category(BasePrimitive.BoxCategory)]
        //[Description("Determine the sizing style of the border's sides")]
        public BorderDrawModes BorderDrawMode
        {
            get
            {
                return (BorderDrawModes)this.GetValue(BorderDrawModeProperty);
            }
            set
            {
                this.SetValue(BorderDrawModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets float value indicating the width of the border
        /// measured in pixels. It is only used when <em>SingleBorder</em> style is chosen for the
        /// <em>BoxStyle</em> property which effectively means that all four borders share the same
        /// width.
        /// </summary>
        [RadPropertyDefaultValue("Width", typeof(BorderPrimitive))]
        [Category(BasePrimitive.BoxCategory)]
        [Description("Gets or sets the thickness of the border (if its BoxStyle is SingleBorder)")]
        public float Width
        {
            get
            {
                return (float)this.GetValue(WidthProperty);
            }
            set
            {
                this.SetValue(WidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a float value width of the left border. This property
        /// has effect only if <em>FourBorders</em> style is used in <em>BoxStyle</em> property and
        /// affects only the width of the left border.
        /// </summary>
        [RadPropertyDefaultValue("LeftWidth", typeof(BorderPrimitive)), Category(BasePrimitive.BoxCategory)]
        public float LeftWidth
        {
            get
            {
                return (float)this.GetValue(LeftWidthProperty);
            }
            set
            {
                this.SetValue(LeftWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a float value width of the top border . This property
        /// has effect only if <em>FourBorders</em> style is used in <em>BoxStyle</em> property,
        /// and affects only the top border.
        /// </summary>
        [RadPropertyDefaultValue("TopWidth", typeof(BorderPrimitive)), Category(BasePrimitive.BoxCategory)]
        public float TopWidth
        {
            get
            {
                return (float)this.GetValue(TopWidthProperty);
            }
            set
            {
                this.SetValue(TopWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a float value width of the right border. This
        /// property has effect only if <em>FourBorders</em> style is used in <em>BoxStyle</em>
        /// property, and affects only the right border.
        /// </summary>
        [RadPropertyDefaultValue("RightWidth", typeof(BorderPrimitive)), Category(BasePrimitive.BoxCategory)]
        public float RightWidth
        {
            get
            {
                return (float)this.GetValue(RightWidthProperty);
            }
            set
            {
                this.SetValue(RightWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a float value width. This property has effect only if
        /// <em>FourBorders</em> style is used in <em>BoxStyle</em> property, and affects only the
        /// bottom border.
        /// </summary>
        [RadPropertyDefaultValue("BottomWidth", typeof(BorderPrimitive)), Category(BasePrimitive.BoxCategory)]
        public float BottomWidth
        {
            get
            {
                return (float)this.GetValue(BottomWidthProperty);
            }
            set
            {
                this.SetValue(BottomWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets and sets the left border color. This applies only if FourBorders is chosen
        /// for BoxStyle property, and affects only the left border.
        /// </summary>
        [RadPropertyDefaultValue("LeftColor", typeof(BorderPrimitive)), Category(BasePrimitive.BoxCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public Color LeftColor
        {
            get
            {
                return (Color)this.GetValue(LeftColorProperty);
            }
            set
            {
                this.SetValue(LeftColorProperty, value);
            }
        }

        /// <summary>
        /// Gets and sets the top border color. This applies only if FourBorders is chosen
        /// for BoxStyle property, and affects only the top border.
        /// </summary>
        [RadPropertyDefaultValue("TopColor", typeof(BorderPrimitive)), Category(BasePrimitive.BoxCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public Color TopColor
        {
            get
            {
                return (Color)this.GetValue(TopColorProperty);
            }
            set
            {
                this.SetValue(TopColorProperty, value);
            }
        }

        /// <summary>
        /// Gets and sets the right border color. This applies only if FourBorders is chosen
        /// for BoxStyle property, and affects only the right border.
        /// </summary>
        [RadPropertyDefaultValue("RightColor", typeof(BorderPrimitive)), Category(BasePrimitive.BoxCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public Color RightColor
        {
            get
            {
                return (Color)this.GetValue(RightColorProperty);
            }
            set
            {
                this.SetValue(RightColorProperty, value);
            }
        }

        /// <summary>
        /// Gets and sets the bottom border color. This applies only if FourBorders is chosen
        /// for BoxStyle property, and affects only the bottom border.
        /// </summary>
        [RadPropertyDefaultValue("BottomColor", typeof(BorderPrimitive)), Category(BasePrimitive.BoxCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public Color BottomColor
        {
            get
            {
                return (Color)this.GetValue(BottomColorProperty);
            }
            set
            {
                this.SetValue(BottomColorProperty, value);
            }
        }

        /// <summary>
        /// Gets and sets the left shadow color. This option applies only if
        /// fourBorders is chosen, and affects only the left border.
        /// </summary>
        [RadPropertyDefaultValue("LeftShadowColor", typeof(BorderPrimitive)), Category(BasePrimitive.BoxCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public Color LeftShadowColor
        {
            get
            {
                return (Color)this.GetValue(LeftShadowColorProperty);
            }
            set
            {
                this.SetValue(LeftShadowColorProperty, value);
            }
        }

        /// <summary>
        /// Gets and sets the top shadow color. This option applies only if
        /// fourBorders is chosen, and affects only the top border.
        /// </summary>
        [RadPropertyDefaultValue("TopShadowColor", typeof(BorderPrimitive)), Category(BasePrimitive.BoxCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public Color TopShadowColor
        {
            get
            {
                return (Color)this.GetValue(TopShadowColorProperty);
            }
            set
            {
                this.SetValue(TopShadowColorProperty, value);
            }
        }

        /// <summary>
        /// Gets and sets the right shadow color. This option applies only if
        /// fourBorders is chosen, and affects only the right border.
        /// </summary>
        [RadPropertyDefaultValue("RightShadowColor", typeof(BorderPrimitive)), Category(BasePrimitive.BoxCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public Color RightShadowColor
        {
            get
            {
                return (Color)this.GetValue(RightShadowColorProperty);
            }
            set
            {
                this.SetValue(RightShadowColorProperty, value);
            }
        }

        /// <summary>
        /// Gets and sets the bottom shadow color. This option applies only if
        /// fourBorders is chosen, and affects only the bottom border.
        /// </summary>
        [RadPropertyDefaultValue("BottomShadowColor", typeof(BorderPrimitive)), Category(BasePrimitive.BoxCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public Color BottomShadowColor
        {
            get
            {
                return (Color)this.GetValue(BottomShadowColorProperty);
            }
            set
            {
                this.SetValue(BottomShadowColorProperty, value);
            }
        }

        /// <summary>
        /// Specifies whether the BorderPrimitive should draw the GraphicsPath defined by its Parent.Shape. If false, it will draw its bounding rectangle.
        /// </summary>
        [Description("Specifies whether the BorderPrimitive should draw the GraphicsPath defined by its Parent.Shape. If false, it will draw its bounding rectangle.")]
        [RadPropertyDefaultValue("PaintUsingParentShape", typeof(BorderPrimitive))]
        public bool PaintUsingParentShape
        {
            get
            {
                return (bool)this.GetValue(PaintUsingParentShapeProperty);
            }
            set
            {
                this.SetValue(PaintUsingParentShapeProperty, value);
            }
        }

        /// <summary>
        /// Gets the border offset of the primitive. It effectively retrieves the upper-left
        /// corner inside the primitive border. It takes into consideration the BoxStyle property
        /// and possible different widths of the left and the upper side.
        /// </summary>
        [Browsable(false)]
        public SizeF Offset
        {
            get
            {
                if (this.BoxStyle == BorderBoxStyle.SingleBorder)
                {
                    float width = this.Width;
                    return new SizeF(width, width);
                }
                else
                {
                    return new SizeF(this.LeftWidth, this.TopWidth);
                }
            }
        }

        /// <summary>Retrieves size of the combined bottom, right, upper, and left border.</summary>
        [Browsable(false)]
        public SizeF BorderSize
        {
            get
            {
                return new SizeF(this.HorizontalWidth, this.VerticalWidth);
            }
        }

        /// <summary>Gets the horizontal width of the combined left and right border.</summary>
        [Browsable(false)]
        public float HorizontalWidth
        {
            get
            {
                if (this.BoxStyle == BorderBoxStyle.SingleBorder)
                {
                    return 2 * this.Width;
                }
                else
                {
                    return this.LeftWidth + this.RightWidth;
                }
            }
        }

        /// <summary>Gets the vertical width of the combined bottom and upper border.</summary>
        [Browsable(false)]
        public float VerticalWidth
        {
            get
            {
                if (this.BoxStyle == BorderBoxStyle.SingleBorder)
                {
                    return 2 * this.Width;
                }
                else
                {
                    return this.TopWidth + this.BottomWidth;
                }
            }
        }

        /// <summary>Gets or sets gradient angle for linear gradient measured in degrees.</summary>
        [RadPropertyDefaultValue("GradientAngle", typeof(BorderPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
        public float GradientAngle
        {
            get
            {
                return (float)this.GetValue(GradientAngleProperty);
            }
            set
            {
                this.SetValue(GradientAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets gradient style. Possible styles are solid, linear, radial, glass,
        /// office glass, gel, and vista.
        /// </summary>
        [RadPropertyDefaultValue("GradientStyle", typeof(BorderPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
        public GradientStyles GradientStyle
        {
            get
            {
                return (GradientStyles)this.GetValue(GradientStyleProperty);
            }
            set
            {
                this.SetValue(GradientStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets color used by radial, glass, office glass, gel, and vista gradients.
        /// This is one of the colors that are used in the gradient effect.
        /// </summary>
        [RadPropertyDefaultValue("ForeColor2", typeof(BorderPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public virtual Color ForeColor2
        {
            get
            {
                return (Color)this.GetValue(ForeColor2Property);
            }
            set
            {
                this.SetValue(ForeColor2Property, value);
            }
        }

        /// <summary>
        /// Gets or sets color used by radial, glass, office glass, and vista gradients. This
        /// is one of the colors that are used in the gradient effect.
        /// </summary>
        [RadPropertyDefaultValue("ForeColor3", typeof(BorderPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public virtual Color ForeColor3
        {
            get
            {
                return (Color)this.GetValue(ForeColor3Property);
            }
            set
            {
                this.SetValue(ForeColor3Property, value);
            }
        }

        /// <summary>
        /// Gets or sets color used by radial, glass, office glass, and vista gradients. This
        /// is one of the colors that are used in the gradient effect.
        /// </summary>
        [RadPropertyDefaultValue("ForeColor4", typeof(BorderPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public virtual Color ForeColor4
        {
            get
            {
                return (Color)this.GetValue(ForeColor4Property);
            }
            set
            {
                this.SetValue(ForeColor4Property, value);
            }
        }

        /// <summary>
        /// Gets or sets color used by radial, glass, office glass, gel, and vista gradients.
        /// This is one of the colors that are used in the gradient effect.
        /// </summary>
        [RadPropertyDefaultValue("InnerColor", typeof(BorderPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public virtual Color InnerColor
        {
            get
            {
                return (Color)this.GetValue(InnerColorProperty);
            }
            set
            {
                this.SetValue(InnerColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets color used by radial, glass, office glass, gel, and vista gradients.
        /// This is one of the colors that are used in the gradient effect.
        /// </summary>
        [RadPropertyDefaultValue("InnerColor2", typeof(BorderPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public virtual Color InnerColor2
        {
            get
            {
                return (Color)this.GetValue(InnerColor2Property);
            }
            set
            {
                this.SetValue(InnerColor2Property, value);
            }
        }

        /// <summary>
        /// Gets or sets color used by radial, glass, office glass, gel, and vista gradients.
        /// This is one of the colors that are used in the gradient effect.
        /// </summary>
        [RadPropertyDefaultValue("InnerColor3", typeof(BorderPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public virtual Color InnerColor3
        {
            get
            {
                return (Color)this.GetValue(InnerColor3Property);
            }
            set
            {
                this.SetValue(InnerColor3Property, value);
            }
        }

        /// <summary>
        /// Gets or sets color used by radial, glass, office glass, gel, and vista gradients.
        /// This is one of the colors that are used in the gradient effect.
        /// </summary>
        [RadPropertyDefaultValue("InnerColor4", typeof(BorderPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public virtual Color InnerColor4
        {
            get
            {
                return (Color)this.GetValue(InnerColor4Property);
            }
            set
            {
                this.SetValue(InnerColor4Property, value);
            }
        }

        #endregion

       
    }
}