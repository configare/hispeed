using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Paint;
using System.Drawing;
using Telerik.WinControls.Design;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Telerik.WinControls.Primitives
{
	public class FocusPrimitive : BasePrimitive
	{
		private BorderPrimitive border;
		
		public FocusPrimitive(BorderPrimitive border)
		{
			this.border = border;
		}

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.AutoSizeMode = RadAutoSizeMode.Auto;
            this.FitToSizeMode = RadFitToSizeMode.FitToParentBounds;
        }

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

        
        public static RadProperty ForeColor2Property = RadProperty.Register(
			"ForeColor2", typeof(Color), typeof(FocusPrimitive), new RadElementPropertyMetadata(
				Color.FromKnownColor(KnownColor.ControlDark), ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

		/// <summary>
		/// Gets or sets color used by radial, glass, office glass, gel, and vista gradients.
		/// This is one of the colors that are used in the gradient effect.
		/// </summary>
		[RadPropertyDefaultValue("ForeColor2", typeof(FocusPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
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

		public static RadProperty ForeColor3Property = RadProperty.Register(
			"ForeColor3", typeof(Color), typeof(FocusPrimitive), new RadElementPropertyMetadata(
				Color.FromKnownColor(KnownColor.ControlDark), ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

		/// <summary>
		/// Gets or sets color used by radial, glass, office glass, and vista gradients. This
		/// is one of the colors that are used in the gradient effect.
		/// </summary>
		[RadPropertyDefaultValue("ForeColor3", typeof(FocusPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
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

		public static RadProperty ForeColor4Property = RadProperty.Register(
			"ForeColor4", typeof(Color), typeof(FocusPrimitive), new RadElementPropertyMetadata(
				Color.FromKnownColor(KnownColor.ControlDark), ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

		/// <summary>
		/// Gets or sets color used by radial, glass, office glass, and vista gradients. This
		/// is one of the colors that are used in the gradient effect.
		/// </summary>
		[RadPropertyDefaultValue("ForeColor4", typeof(FocusPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
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

		public static RadProperty InnerColorProperty = RadProperty.Register(
			"InnerColor", typeof(Color), typeof(FocusPrimitive), new RadElementPropertyMetadata(
				Color.FromKnownColor(KnownColor.ControlLightLight), ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

		/// <summary>
		/// Gets or sets color used by radial, glass, office glass, gel, and vista gradients.
		/// This is one of the colors that are used in the gradient effect.
		/// </summary>
		[RadPropertyDefaultValue("InnerColor", typeof(FocusPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
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

		public static RadProperty InnerColor2Property = RadProperty.Register(
			"InnerColor2", typeof(Color), typeof(FocusPrimitive), new RadElementPropertyMetadata(
				Color.FromKnownColor(KnownColor.Control), ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

		/// <summary>
		/// Gets or sets color used by radial, glass, office glass, gel, and vista gradients.
		/// This is one of the colors that are used in the gradient effect.
		/// </summary>
		[RadPropertyDefaultValue("InnerColor2", typeof(FocusPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
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

		public static RadProperty InnerColor3Property = RadProperty.Register(
			"InnerColor3", typeof(Color), typeof(FocusPrimitive), new RadElementPropertyMetadata(
				Color.FromKnownColor(KnownColor.ControlDark), ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

		/// <summary>
		/// Gets or sets color used by radial, glass, office glass, gel, and vista gradients.
		/// This is one of the colors that are used in the gradient effect.
		/// </summary>
		[RadPropertyDefaultValue("InnerColor3", typeof(FocusPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
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

		public static RadProperty InnerColor4Property = RadProperty.Register(
			"InnerColor4", typeof(Color), typeof(FocusPrimitive), new RadElementPropertyMetadata(
				Color.FromKnownColor(KnownColor.ControlDarkDark), ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

		/// <summary>
		/// Gets or sets color used by radial, glass, office glass, gel, and vista gradients.
		/// This is one of the colors that are used in the gradient effect.
		/// </summary>
		[RadPropertyDefaultValue("InnerColor4", typeof(FocusPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
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

		public static readonly RadProperty BorderBoxStyleProperty = RadProperty.Register(
		   "BoxStyle", typeof(BorderBoxStyle), typeof(FocusPrimitive), new RadElementPropertyMetadata(
			   BorderBoxStyle.SingleBorder, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

		[RadPropertyDefaultValue("BoxStyle", typeof(FocusPrimitive)), Description(BasePrimitive.BoxCategory)]
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

		public static RadProperty GradientStyleProperty = RadProperty.Register(
			"GradientStyle", typeof(GradientStyles), typeof(FocusPrimitive), new RadElementPropertyMetadata(
				GradientStyles.Linear, ElementPropertyOptions.AffectsDisplay));

		/// <summary>
		/// Gets or sets gradient style. Possible styles are solid, linear, radial, glass,
		/// office glass, gel, and vista.
		/// </summary>
		[RadPropertyDefaultValue("GradientStyle", typeof(FocusPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
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

		public static RadProperty GradientAngleProperty = RadProperty.Register(
			"GradientAngle", typeof(float), typeof(FocusPrimitive), new RadElementPropertyMetadata(
				90f, ElementPropertyOptions.AffectsDisplay));

		/// <summary>Gets or sets gradient angle for linear gradient measured in degrees.</summary>
		[RadPropertyDefaultValue("GradientAngle", typeof(FocusPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
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

		public override void PaintPrimitive(IGraphics graphics, float angle, SizeF scale)
		{
			if ((this.Size.Width <= 0) || (this.Size.Height <= 0))
				return;

			Color[] gradientColors = new Color[] { this.ForeColor, this.ForeColor2, this.ForeColor3, this.ForeColor4 };
			
			int parentBorderWidth = 1;
            if(this.border != null)
			{
				if(this.border.BoxStyle == BorderBoxStyle.OuterInnerBorders)
				{
					parentBorderWidth = 2;
				}
			}

            Rectangle rectangle = new Rectangle(parentBorderWidth - 1, parentBorderWidth - 1,
                Size.Width - parentBorderWidth - 1, Size.Height - parentBorderWidth - 1);

			DrawRectangle(graphics, rectangle, gradientColors, 1);


			Color[] innerColors = new Color[] { this.InnerColor, this.InnerColor2, this.InnerColor3, this.InnerColor4 };

			Rectangle newRectangle = Rectangle.Inflate(rectangle, -1, -1);
			DrawRectangle(graphics, newRectangle, innerColors, 1);
		}

		private void DrawRectangle(IGraphics graphics, Rectangle rectangle, Color[] gradientColors, float width)
		{
			if (this.BoxStyle == BorderBoxStyle.FourBorders)
			{
				graphics.DrawRectangle(rectangle, this.ForeColor, PenAlignment.Inset, width);
				return;
			}

			if (this.GradientStyle == GradientStyles.Solid)
			{
				graphics.DrawRectangle(rectangle, gradientColors[0], PenAlignment.Inset, width);
			}
			else if (this.GradientStyle == GradientStyles.Linear)
			{
				graphics.DrawLinearGradientRectangle(rectangle, gradientColors, PenAlignment.Center, width, this.GradientAngle);
			}
		}
	}
}
