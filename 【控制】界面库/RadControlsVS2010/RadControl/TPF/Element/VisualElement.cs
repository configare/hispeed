using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Paint;
using Telerik.WinControls.Design;
using System.Drawing.Design;
using System.Diagnostics;

namespace Telerik.WinControls
{
    /// <summary>Extends RadElement and adds visual properties common to all elements.</summary>
	[DefaultProperty("BackColor")]
	public class VisualElement : RadElement
	{
        #region Constructors

        static VisualElement()
        {
            EventFontChanged = new object();
        }

        #endregion

        #region Events

        /// <summary>Fires when the font is changed.</summary>
        public event EventHandler FontChanged
        {
            add
            {
                this.Events.AddHandler(EventFontChanged, value);
            }
            remove
            {
                this.Events.RemoveHandler(EventFontChanged, value);
            }
        }

        #endregion

        #region Overrides

        protected virtual void OnFontChanged(EventArgs e)
        {
            EventHandler eh = this.Events[EventFontChanged] as EventHandler;
            if (eh != null)
            {
                eh(this, e);
            }

        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (e.Property == FontProperty)
            {
                this.OnFontChanged(EventArgs.Empty);
            }
            else if (e.Property == StyleProperty)
            {
                //Logic moved to StyleManager 
                ////unapply previous style
                //if (e.OldValue != null)
                //{
                //    ((StyleSheet)e.OldValue).Unapply(this);
                //}

                //if (e.NewValue != null)
                //{
                //    //apply new one, forbidding animations temporary
                //    AnimatedPropertySetting.AnimationsEnabled = false;

                //    StyleSheet style = (StyleSheet)e.NewValue;
                //    style.ProcessStyle(this);
                //    style.ApplyStyle(this);

                //    AnimatedPropertySetting.AnimationsEnabled = true;
                //}

                this.OnStyleChanged(e);
            }

            base.OnPropertyChanged(e);
        }

        protected override void PaintElement(IGraphics graphics, float angle, SizeF scale)
        {
            Rectangle rect = new Rectangle(Point.Empty, this.Size);
            graphics.FillRectangle(rect, this.BackColor);

            base.PaintElement(graphics, angle, scale);
        }

        protected override void PaintChildren(IGraphics graphics, Rectangle clipRectange, float angle, SizeF scale, bool useRelativeTransformation)
        {
            if (this.Opacity < 1)
            {
                this.saveGraphicsOpacity = graphics.Opacity;
                graphics.ChangeOpacity(this.Opacity * graphics.Opacity);
            }

            base.PaintChildren(graphics, clipRectange, angle, scale, useRelativeTransformation);

            if (saveGraphicsOpacity != null && saveGraphicsOpacity.HasValue)
            {
                graphics.ChangeOpacity(this.saveGraphicsOpacity.Value);
            }
        }

        protected override void PrePaintElement(IGraphics graphics)
        {
            base.PrePaintElement(graphics);

            Graphics rawGraphics = (Graphics)graphics.UnderlayGraphics;

            graphicsCurrentSmoothingMode = rawGraphics.SmoothingMode;
            graphics.ChangeSmoothingMode(this.SmoothingMode);

            graphicsCurrentTextRenderingHint = rawGraphics.TextRenderingHint;
            if (this.Enabled)
                rawGraphics.TextRenderingHint = this.TextRenderingHint;
            else
                rawGraphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            if (this.Opacity < 1)
            {
                this.saveGraphicsOpacity = graphics.Opacity;
                graphics.ChangeOpacity(this.Opacity * graphics.Opacity);
            }
        }

        protected override void PostPaintElement(IGraphics graphics)
        {
            base.PostPaintElement(graphics);

            Graphics rawGraphics = (Graphics)graphics.UnderlayGraphics;

            graphics.ChangeSmoothingMode(this.graphicsCurrentSmoothingMode);
            rawGraphics.TextRenderingHint = this.graphicsCurrentTextRenderingHint;

            if (saveGraphicsOpacity != null && saveGraphicsOpacity.HasValue)
            {
                graphics.ChangeOpacity(this.saveGraphicsOpacity.Value);
            }
        }

        #endregion

        #region Properties

        public static RadProperty DefaultSizeProperty =
            RadProperty.RegisterAttached("DefaultSize", typeof(Size), typeof(VisualElement),
                new RadElementPropertyMetadata(Size.Empty,
                ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsParentArrange | ElementPropertyOptions.AffectsMeasure));

        /// <summary>Gets or sets the DefaultSize.</summary>
        [Description("DefaultSize of an element. The property is inheritable through the element tree.")]
        [RadPropertyDefaultValue("DefaultSize", typeof(VisualElement))]
        [Category(RadDesignCategory.AppearanceCategory)]                
        public virtual Size DefaultSize
        {
            get
            {
                 return (Size)this.GetValue(DefaultSizeProperty);                   
            }
            set
            {
                this.SetValue(DefaultSizeProperty, value);
            }
        }


		public static RadProperty ForeColorProperty =
            RadProperty.Register("ForeColor", typeof(Color), typeof(VisualElement),
				new RadElementPropertyMetadata(Color.FromKnownColor(KnownColor.ControlText),
				ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        /// <summary>Gets or sets the forecolor. Color type represents an ARGB color.</summary>
        [Description("Foreground color - ex. of the text and borders of an element. The property is inheritable through the element tree.")]
        [RadPropertyDefaultValue("ForeColor", typeof(VisualElement)), Category(RadDesignCategory.AppearanceCategory)]
		[Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(RadColorEditorConverter))]
		public virtual Color ForeColor
		{
			get
			{
                return (Color)this.GetValue(ForeColorProperty);
			}
			set
			{
				this.SetValue(ForeColorProperty, value);
			}
		}

		public static RadProperty BackColorProperty =
            RadProperty.RegisterAttached("BackColor", typeof(Color), typeof(VisualElement),
			new RadElementPropertyMetadata(Color.FromKnownColor(KnownColor.ControlLightLight),
			ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        /// <summary>Gets or sets the backcolor. Color type represents an ARGB color.</summary>
		[Description("Background color - ex. of the fill of an element. The property is inheritable through the element tree.")]
        [RadPropertyDefaultValue("BackColor", typeof(VisualElement)), Category(RadDesignCategory.AppearanceCategory)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(RadColorEditorConverter))]
		public virtual Color BackColor
		{
			get
			{
                return (Color)this.GetValue(BackColorProperty);
			}
			set
			{
				this.SetValue(BackColorProperty, value);
			}
		}

        public static RadProperty FontProperty =
			RadProperty.Register("Font", typeof(Font), typeof(VisualElement),
			new RadElementPropertyMetadata(Control.DefaultFont,
            ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsParentMeasure |
            ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsParentArrange |
			ElementPropertyOptions.CanInheritValue));

        /// <summary>
        /// Gets or sets the font. Font type defines a particular format for text, including
        /// font face, size, and style attributes.
        /// </summary>
		[Description("Font - ex. of the text of an element. The property is inheritable through the element tree.")]
        [RadPropertyDefaultValue("Font", typeof(VisualElement)), Category(RadDesignCategory.AppearanceCategory)]
		public virtual Font Font
		{
			get
			{
                return (Font)this.GetValue(FontProperty);
			}
			set
			{
                this.SetValue(FontProperty, value);
			}
		}

        public static RadProperty SmoothingModeProperty =
                    RadProperty.Register("SmoothingMode", typeof(SmoothingMode), typeof(VisualElement),
                    new RadElementPropertyMetadata(SmoothingMode.Default, ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        /// Gets or sets the smoothing mode of an element. Smoothing mode enumeration defines
        /// possible values.
        /// </summary>
		[Description("Graphincs smoothing mode to be used for painting the element and its children.")]
        [RadPropertyDefaultValue("SmoothingMode", typeof(VisualElement)), Category(RadDesignCategory.AppearanceCategory)]
        public virtual SmoothingMode SmoothingMode
        {
            get
            {
                return (SmoothingMode)this.GetValue(SmoothingModeProperty);
            }
            set
            {
                this.SetValue(SmoothingModeProperty, value);
            }
        }

		public static RadProperty OpacityProperty =
					RadProperty.Register("Opacity", typeof(double), typeof(VisualElement),
					new RadElementPropertyMetadata(1.0d, ElementPropertyOptions.AffectsDisplay));

		/// <summary>
		/// Gets or sets the opacity of an element. Value 0.0f indicates that the element is completely transparent,
		/// 1.0f means the element is not transparent (the default value).
		/// </summary>
		[Description("Graphincs opacity mode to be used for painting the element and its children.")]
		[RadPropertyDefaultValue("Opacity", typeof(VisualElement)), Category(RadDesignCategory.AppearanceCategory)]
		public virtual double Opacity
		{
			get
			{
				return (double)this.GetValue(OpacityProperty);
			}
			set
			{
				this.SetValue(OpacityProperty, value);
			}
		}

        public static RadProperty TextRenderingHintProperty =
                    RadProperty.Register("TextRenderingHint", typeof(TextRenderingHint), typeof(VisualElement),
                    new RadElementPropertyMetadata(TextRenderingHint.SystemDefault, 
                        ElementPropertyOptions.AffectsDisplay |
                        ElementPropertyOptions.AffectsMeasure |//p.p. 29.07.09
                                                               //fix very strange bug with Hebrew and FitToGrid Renedering hint should AffectsMeasure
                        ElementPropertyOptions.AffectsParentMeasure |
                        ElementPropertyOptions.InvalidatesLayout |
                        ElementPropertyOptions.AffectsLayout | 
                        ElementPropertyOptions.AffectsParentArrange |
                        ElementPropertyOptions.CanInheritValue));

        /// <summary>Gets or sets the text rendering hint.</summary>
		[Description("Graphincs text-rendering mode to be used for painting text of the element")]
        [RadPropertyDefaultValue("TextRenderingHint", typeof(VisualElement)), Category(RadDesignCategory.AppearanceCategory)]
        public virtual TextRenderingHint TextRenderingHint
        {
            get
            {
                return (TextRenderingHint)this.GetValue(TextRenderingHintProperty);
            }
            set
            {
                this.SetValue(TextRenderingHintProperty, value);
            }
        }

		#endregion

        #region Fields

        private SmoothingMode graphicsCurrentSmoothingMode;
        private TextRenderingHint graphicsCurrentTextRenderingHint;
        private double? saveGraphicsOpacity = null;

        private static object EventFontChanged;

        //keep consistency in bit state keys declaration
        internal const ulong VisualElementLastStateKey = RadElementLastStateKey;

        #endregion
	}
}
