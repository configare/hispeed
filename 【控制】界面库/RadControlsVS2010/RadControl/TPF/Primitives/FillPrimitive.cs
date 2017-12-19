using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using Telerik.WinControls.Paint;
using System.Windows.Forms.VisualStyles;
using Telerik.WinControls.Design;
using System.Drawing.Design;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.Primitives
{
    /// <summary>Represents a filling that is drawn on the screen.</summary>
	[Editor(typeof(RadFillEditor), typeof(UITypeEditor))]
    public class FillPrimitive : BasePrimitive, IFillElement, IPrimitiveElement
    {
        #region Constructors/Initializers

        protected override void InitializeFields()
        {
            this.fillPrimitiveImpl = new FillPrimitiveImpl(this, this);
            base.InitializeFields();
        }

        #endregion

        #region IPrimitiveElement Members

        float IPrimitiveElement.BorderThickness
        {
            get
            {
                if (this.Parent != null)
                {
                    return this.Parent.BorderThickness.Left;
                }

                return 0;
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
            return new OrFilter(Telerik.WinControls.PropertyFilter.FillPrimitiveFilter,
                Telerik.WinControls.PropertyFilter.AppearanceFilter,
                Telerik.WinControls.PropertyFilter.BehaviorFilter);
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

        protected internal override object GetDefaultValue(RadPropertyValue propVal, object baseDefaultValue)
        {
            RadProperty property = propVal.Property;

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

        protected override void OnBoundsChanged(RadPropertyChangedEventArgs e)
        {
            this.fillPrimitiveImpl.OnBoundsChanged((Rectangle)e.OldValue);

            base.OnBoundsChanged(e);
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            this.fillPrimitiveImpl.InvalidateFillCache(e.Property);

            base.OnPropertyChanged(e);
        }

        /// <summary>Draws the primitive on the screen.</summary>
        public override void PaintPrimitive(IGraphics g, float angle, SizeF scale)
        {
            this.fillPrimitiveImpl.PaintFill(g, angle, scale);
        }

        protected override bool ShouldPaintUsingParentShape
        {
            get
            {
                return this.PaintUsingParentShape;
            }
        }

        #endregion

        #region Properties

        [DefaultValue(RadFitToSizeMode.FitToParentPadding)]
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

		public static RadProperty BackColor2Property = RadProperty.Register(
			"BackColor2", typeof(Color), typeof(FillPrimitive), 
            new RadElementPropertyMetadata(SystemColors.Control, ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        /// Gets or sets background color. This property is applicable to radial, glass,
        /// office glass, gel, and vista gradients.
        /// </summary>
		[Description("Second color component when gradient style is other than solid")]
        [RadPropertyDefaultValue("BackColor2", typeof(FillPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
		[Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(RadColorEditorConverter))]
		public virtual Color BackColor2
		{
			get
			{
                return (Color)this.GetValue(BackColor2Property);
			}
			set
			{
				this.SetValue(BackColor2Property, value);
			}
        }

        [Description("The third color component when when gradient style is other than solid and number of colors property is greater than 2")]
        public static RadProperty BackColor3Property = RadProperty.Register(
            "BackColor3", typeof(Color), typeof(FillPrimitive),
            new RadElementPropertyMetadata(SystemColors.ControlDark, ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        /// Gets or sets background color. This property is applicable to radial, glass,
        /// office glass, and vista gradients.
        /// </summary>
        [RadPropertyDefaultValue("BackColor3", typeof(FillPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public virtual Color BackColor3
        {
            get
            {
                return (Color)this.GetValue(BackColor3Property);
            }
            set
            {
                this.SetValue(BackColor3Property, value);
            }
        }

        [Description("The fourth color component when when gradient style is other than solid and number of colors property is greater than 3")]
        public static RadProperty BackColor4Property = RadProperty.Register(
            "BackColor4", typeof(Color), typeof(FillPrimitive),
            new RadElementPropertyMetadata(SystemColors.ControlLightLight, ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        /// Gets or sets background color. This property is applicable to radial, glass,
        /// office glass, and vista gradients.
        /// </summary>
        [RadPropertyDefaultValue("BackColor4", typeof(FillPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
        [Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(RadColorEditorConverter))]
        public virtual Color BackColor4
        {
            get
            {
                return (Color)this.GetValue(BackColor4Property);
            }
            set
            {
                this.SetValue(BackColor4Property, value);
            }
        }

        public static RadProperty NumberOfColorsProperty = RadProperty.Register(
            "NumberOfColors", typeof(int), typeof(FillPrimitive), new RadElementPropertyMetadata(
                2, ElementPropertyOptions.AffectsDisplay));

        /// <summary>Gets or sets the number of used colors in the gradient effect.</summary>
        [Description("Maximum number of colors to be used in any of the gradient styles (other than solid). Some styles like \"Glass\" always require using 4 colors, ignoring this property value")]
        [RadPropertyDefaultValue("NumberOfColors", typeof(FillPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
        public virtual int NumberOfColors
        {
            get
            {
                return (int)this.GetValue(NumberOfColorsProperty);
            }
            set
            {
                this.SetValue(NumberOfColorsProperty, value);
            }
        }

        public static RadProperty GradientStyleProperty = RadProperty.Register(
            "GradientStyle", typeof(GradientStyles), typeof(FillPrimitive), new RadElementPropertyMetadata(
                GradientStyles.Linear, ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        /// Gets and sets the gradient style. The possible values are defined in the gradient
        /// style enumeration: solid, linear, radial, glass, office glass, gel, and vista.
        /// </summary>
        [Description("Style of fill to be used")]
        [RadPropertyDefaultValue("GradientStyle", typeof(FillPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
        public virtual GradientStyles GradientStyle
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

        public static RadProperty GradientAngleProperty = RadProperty.Register(
            "GradientAngle", typeof(float), typeof(FillPrimitive), new RadElementPropertyMetadata(
                90f, ElementPropertyOptions.AffectsDisplay));

        /// <summary><para>Gets or sets gradient angle for linear gradient.</para></summary>
        [Description("Gradient angle to be applied with linear style of fill.")]
        [RadPropertyDefaultValue("GradientAngle", typeof(FillPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
        public virtual float GradientAngle
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

        public static RadProperty GradientPercentageProperty = RadProperty.Register(
            "GradientPercentage", typeof(float), typeof(FillPrimitive), new RadElementPropertyMetadata(
                0.5f, ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        /// Gets or sets GradientPercentage for linear, glass, office glass, gel, vista, and
        /// radial gradients.
        /// </summary>
        [Description("For liner gradient style with more than 2 colors, indicates the position of the gradient stop between the first and the second color components. Custom settings for other gradient styles.")]
        [RadPropertyDefaultValue("GradientPercentage", typeof(FillPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
        public virtual float GradientPercentage
        {
            get
            {
                return (float)GetValue(GradientPercentageProperty);
            }
            set
            {
                this.SetValue(GradientPercentageProperty, value);
            }
        }

        [Description("For liner gradient style with more than 3 colors, indicates the position of the gradient stop between the second andthe third color components. Custom settings for other gradient styles.")]
        public static RadProperty GradientPercentage2Property = RadProperty.Register(
            "GradientPercentage2", typeof(float), typeof(FillPrimitive), new RadElementPropertyMetadata(
                0.666f, ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        /// Gets or sets GradientPercentage for office glass, vista, and radial
        /// gradients.
        /// </summary>
        [RadPropertyDefaultValue("GradientPercentage2", typeof(FillPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
        public virtual float GradientPercentage2
        {
            get
            {
                return (float)GetValue(GradientPercentage2Property);
            }
            set
            {
                this.SetValue(GradientPercentage2Property, value);
            }
        }

        public static readonly RadProperty PaintUsingParentShapeProperty = RadProperty.Register(
            "PaintUsingParentShape", typeof(bool), typeof(FillPrimitive), new RadElementPropertyMetadata(
                true, ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        /// Specifies whether the FillPrimitive should fill the GraphicsPath defined by its Parent.Shape. If false, it will fill its bounding rectangle.
        /// </summary>
        [Description("Specifies whether the FillPrimitive should fill the GraphicsPath defined by its Parent.Shape. If false, it will fill its bounding rectangle.")]
        [RadPropertyDefaultValue("PaintUsingParentShape", typeof(FillPrimitive))]
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

        #endregion

        #region Fields

        private FillPrimitiveImpl fillPrimitiveImpl;

        #endregion
    }	
}
