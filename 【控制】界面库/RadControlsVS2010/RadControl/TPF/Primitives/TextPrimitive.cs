using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Text;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Paint;
using System.Drawing;
using System.Windows.Forms;
using System;
using Telerik.WinControls.Design;
using System.Text;
using Telerik.WinControls.TextPrimitiveUtils;
using System.Drawing.Design;

namespace Telerik.WinControls.Primitives
{
    /// <summary>Represents text that is drawn on the screen.
    /// <para>
    ///  Extends %BasePrimitive:Telerik.WinControls.Primitives.BasePrimitive%.
    ///</para></summary>
    public class TextPrimitive : BasePrimitive, ITextProvider, ITextPrimitive
    {
        private string textWithoutMnemonics = string.Empty; 
        internal ITextPrimitive textPrimitiveImpl;

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.StretchHorizontally = false;
            this.StretchVertically = false;
            this.textPrimitiveImpl = new TextPrimitiveImpl();
        }

        public static RadProperty TextProperty = RadProperty.Register(
            "Text", typeof(string), typeof(TextPrimitive), new RadElementPropertyMetadata(
                string.Empty, 
                ElementPropertyOptions.AffectsDisplay
                | ElementPropertyOptions.AffectsLayout
                | ElementPropertyOptions.InvalidatesLayout
                | ElementPropertyOptions.AffectsMeasure
                | ElementPropertyOptions.AffectsParentMeasure
                | ElementPropertyOptions.AffectsParentArrange));

        /// <summary>Gets or sets the displayed text.</summary>
        [RadPropertyDefaultValue("Text", typeof(TextPrimitive)), Category(RadDesignCategory.AppearanceCategory), Localizable(true)]
        public string Text
        {
            get
            {
                string text = (string)this.GetValue(TextProperty);
                if (text == null)
                {
                    text = string.Empty;
                }
                return text;
            }
            set
            {
                this.SetValue(TextProperty, value);
                this.ToggleHTML(this.Text);
            }
        }

        /// <summary>
        /// Allow StretchHorizontally
        /// </summary>
        [DefaultValue(false)]
        public override bool StretchHorizontally
        {
            get { return base.StretchHorizontally; }
            set { base.StretchHorizontally = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]        
        public Color CachedForeColor
		{
			set
			{
				cachedForeColor = value;
			}

			get
			{
				return cachedForeColor;
			}
		}

        /// <summary>
        /// Allow StretchVertically
        /// </summary>
        [DefaultValue(false)]
        public override bool StretchVertically
        {
            get { return base.StretchVertically; }
            set { base.StretchVertically = value; }
        }


        /// <summary>
        /// Gets the text without the mnemonic symbols.<br/>
        /// For example the text "a&amp;bc" will be returned as "abc".
        /// </summary>
#if !DEBUG
        [Browsable(false)]
#endif
        //public string TextWithoutMnemonics
        //{
        //    get
        //    {
        //        return this.textWithoutMnemonics;
        //    }
        //}

        public static RadProperty AutoEllipsisProperty =
        RadProperty.Register(
            "AutoEllipsis", typeof(bool), typeof(TextPrimitive), new RadElementPropertyMetadata(
                true, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

        //true if the additional label text is to be indicated by an ellipsis; 
        //otherwise, false. The default is true
        [RadPropertyDefaultValue("AutoEllipsis", typeof(TextPrimitive)), Category(RadDesignCategory.AppearanceCategory), Localizable(true)]
        public bool AutoEllipsis
        {
            get
            {
                return (bool)this.GetValue(AutoEllipsisProperty);
            }
            set
            {
                this.SetValue(AutoEllipsisProperty, value);
            }
        }

        public static RadProperty MeasureTrailingSpacesProperty =
        RadProperty.Register(
            "MeasureTrailingSpaces", typeof(bool), typeof(TextPrimitive), new RadElementPropertyMetadata(
                true, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

        /// <summary>
        /// Includes the trailing space at the end of each line. By default the boundary rectangle returned by the Overload:System.Drawing.Graphics.MeasureString method excludes the space at the end of each line. Set this flag to include that space in measurement.
        /// </summary>
        [DefaultValue(true),
        Description("Includes the trailing space at the end of each line. By default the boundary rectangle returned by the Overload:System.Drawing.Graphics.MeasureString method excludes the space at the end of each line. Set this flag to include that space in measurement."),
        Category(RadDesignCategory.AppearanceCategory), Localizable(true)]
        public bool MeasureTrailingSpaces
        {
            get
            {
                return (bool)this.GetValue(MeasureTrailingSpacesProperty);
            }
            set
            {
                this.SetValue(MeasureTrailingSpacesProperty, value);
            }
        }

        public static RadProperty TextWrapProperty =
     RadProperty.Register(
         "TextWrap", typeof(bool), typeof(TextPrimitive), new RadElementPropertyMetadata(
             false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

        //true if the text should wrap to the available layout rectangle
        //otherwise, false. The default is true
        [RadPropertyDefaultValue("TextWrap", typeof(TextPrimitive)), Category(RadDesignCategory.AppearanceCategory), Localizable(true)]
        public bool TextWrap
        {
            get
            {
                return (bool)this.GetValue(TextWrapProperty);
            }
            set
            {
                this.SetValue(TextWrapProperty, value);
                this.ToggleHTML(this.Text);
            }
        }


        public static RadProperty UseMnemonicProperty =
        RadProperty.Register(
            "UseMnemonic", typeof(bool), typeof(TextPrimitive), new RadElementPropertyMetadata(
                true, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

        //true if the label doesn't display the ampersand character and underlines 
        //  the character after the ampersand in its displayed text and treats the
        //  underlined character as an access key; otherwise, false if the 
        //  ampersand character is displayed in the text of the control. 
        //  The default is true.

        /// <summary>
        /// Gets or sets a value indicating whether the control interprets an ampersand character (&#38;)
        /// in the control's Text property to be an access key prefix character.
        /// </summary>
        /// <returns>
        /// true if the label doesn't display the ampersand character and underlines the character
        /// after the ampersand in its displayed text and treats the underlined character as an access key;
        /// otherwise, false if the ampersand character is displayed in the text of the control.
        /// The default is true.
        /// </returns>
        [RadPropertyDefaultValue("UseMnemonic", typeof(TextPrimitive))]
        [Category(RadDesignCategory.AppearanceCategory), Localizable(true)]
        [Description("If true, each character immediately after an ampersand (&&) will be painted with underscore and the ampersand will be omitted")]
        public bool UseMnemonic
        {
            get
            {
                return (bool)this.GetValue(UseMnemonicProperty);
            }
            set
            {
                this.SetValue(UseMnemonicProperty, value);
            }
        }


        public static RadProperty ShowKeyboardCuesProperty =
      RadProperty.Register(
            "ShowKeyboardCues", typeof(bool), typeof(TextPrimitive), new RadElementPropertyMetadata(
                true, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

        //true if the keyboard accelerators are visible; otherwise, false.
        [RadPropertyDefaultValue("ShowKeyboardCues", typeof(TextPrimitive)), Category(RadDesignCategory.AppearanceCategory), Localizable(true)]
        public bool ShowKeyboardCues
        {
            get
            {
                return (bool)this.GetValue(ShowKeyboardCuesProperty);
            }
            set
            {
                this.SetValue(ShowKeyboardCuesProperty, value);
            }
        }

        public static RadProperty TextOrientationProperty =
            RadItem.TextOrientationProperty.AddOwner(typeof(TextPrimitive), new RadElementPropertyMetadata(
                Orientation.Horizontal, ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.CanInheritValue | ElementPropertyOptions.AffectsParentArrange | ElementPropertyOptions.AffectsParentMeasure));

        /// <summary>Gets or sets the text orientation. Possible values are horizontal 
        /// and vertical.</summary>
        [RadPropertyDefaultValue("TextOrientation", typeof(TextPrimitive)), Category(RadDesignCategory.AppearanceCategory), Localizable(true)]
        public Orientation TextOrientation
        {
            get
            {
                return (Orientation)this.GetValue(TextOrientationProperty);
            }
            set
            {
                this.SetValue(TextOrientationProperty, value);
            }
        }

        public static RadProperty FlipTextProperty =
            RadItem.FlipTextProperty.AddOwner(typeof(TextPrimitive), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.CanInheritValue));

        /// <summary>Gets or sets whether the text will be flipped.</summary>
        [RadPropertyDefaultValue("TextOrientation", typeof(TextPrimitive)), Category(RadDesignCategory.AppearanceCategory), Localizable(true)]
        public bool FlipText
        {
            get
            {
                return (bool)this.GetValue(FlipTextProperty);
            }
            set
            {
                this.SetValue(FlipTextProperty, value);
            }
        }

        public static RadProperty TextFormatFlagsProperty = RadProperty.Register(
            "TextFormatFlags", typeof(TextFormatFlags), typeof(TextPrimitive), new RadElementPropertyMetadata(
                TextFormatFlags.Default, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        /// <summary>
        /// Gets or sets the TextFormatFlags. The TextFormatEumeration specifies the 
        /// display and layout information for the text strings.
        /// </summary>
        [RadPropertyDefaultValue("TextFormatFlags", typeof(TextPrimitive)), Category(RadDesignCategory.AppearanceCategory), Localizable(true)]
        [Obsolete("This member is not used anymore and will be removed in the next version")]
        public TextFormatFlags TextFormatFlags
        {
            get
            {
                return (TextFormatFlags)GetValue(TextFormatFlagsProperty);
            }
            set
            {
                this.SetValue(TextFormatFlagsProperty, value);
            }
        }

        public static RadProperty TextAlignmentProperty = RadProperty.Register(
            "TextAlignment", typeof(ContentAlignment), typeof(TextPrimitive), new RadElementPropertyMetadata(
                ContentAlignment.MiddleLeft, ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        /// Gets or sets the text alignment. Possible values are included in 
        /// the ContentAlignment enumeration.
        /// </summary>
        [RadPropertyDefaultValue("TextAlignment", typeof(TextPrimitive)), Category(RadDesignCategory.AppearanceCategory), Localizable(true)]
        public ContentAlignment TextAlignment
        {
            get
            {
                return (ContentAlignment)this.GetValue(TextAlignmentProperty);
            }
            set
            {
                this.SetValue(TextAlignmentProperty, value);
            }
        }   
     
        private ContentAlignment GetTextAlignment()
        {
            if (this.RightToLeft)
            {
                switch (this.TextAlignment)
                {
                    case ContentAlignment.BottomLeft:
                        return ContentAlignment.BottomRight;
                    case ContentAlignment.BottomRight:
                        return ContentAlignment.BottomLeft;
                    case ContentAlignment.MiddleLeft:
                        return ContentAlignment.MiddleRight;
                    case ContentAlignment.MiddleRight:
                        return ContentAlignment.MiddleLeft;
                    case ContentAlignment.TopLeft:
                        return ContentAlignment.TopRight;
                    case ContentAlignment.TopRight:
                        return ContentAlignment.TopLeft;
                }
            }
            return this.TextAlignment;
        }

        public SizeF GetTextSize()
        {
            TextParams textParams = this.CreateTextParams();
            return this.textPrimitiveImpl.GetTextSize(textParams);
        }
        public SizeF GetTextSize(SizeF proposedSize)
        {
            TextParams textParams = this.CreateTextParams();
            return this.textPrimitiveImpl.GetTextSize(proposedSize,textParams);
        }

        
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static RadProperty StringAlignmentProperty =
            RadItem.StringAlignmentProperty.AddOwner(typeof(TextPrimitive), new RadElementPropertyMetadata(
                StringAlignment.Near, ElementPropertyOptions.CanInheritValue | ElementPropertyOptions.AffectsDisplay));

        [Browsable(false)]
        [Obsolete("Property is irrelevant. Use TextAlignment property instead.")]
        public StringAlignment StringAlignment
        {
            get
            {
                return StringAlignment.Near;
            }
            set
            {
            }
        }

        /// <summary><para>Gets a value indicating whether the primitive has content.</para></summary>
        public override bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(this.Text);
            }
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            // Ensure that Measure() is called for all children
            base.MeasureOverride(availableSize);
            TextParams textParams = this.CreateTextParams();
            return this.textPrimitiveImpl.MeasureOverride(availableSize,textParams);
        }
   
        public override Size GetPreferredSizeCore(Size proposedSize)
        {
            TextParams textParams = this.CreateTextParams();
            Size res;
            if (this.AutoSize)
            {
                if (this.AutoSizeMode == RadAutoSizeMode.Auto ||
                    this.AutoSizeMode == RadAutoSizeMode.WrapAroundChildren)
                {
                    Size textSize = Size.Ceiling( this.textPrimitiveImpl.GetTextSize(proposedSize, textParams));
                    /*textSize.Height = textSize.Height + 10;
                    textSize.Width = textSize.Width + 10;*/
                    res = LayoutUtils.FlipSizeIf(TextOrientation == Orientation.Vertical, textSize);

                    Size minSize = LayoutUtils.FlipSizeIf(TextOrientation == Orientation.Vertical, Size.Ceiling(this.textPrimitiveImpl.GetTextSize(textParams)));
                    res.Width = Math.Max(res.Width, minSize.Width);
                    res.Height = Math.Max(res.Height, minSize.Height);
                    //this.MinSize = LayoutUtils.FlipSizeIf(TextOrientation == Orientation.Vertical, GetTextSize());
                }
                else
                {
                    Size textSize = Size.Ceiling(this.textPrimitiveImpl.GetTextSize(textParams));
                    res = base.GetPreferredSizeCore(proposedSize);
                    if (TextOrientation == Orientation.Vertical)
                    {
                        res.Width = Math.Max(res.Width, textSize.Width);
                        //this.MinSize = new Size(textSize.Width, 0);
                    }
                    else
                    {
                        res.Height = Math.Max(res.Height, textSize.Height);
                        //this.MinSize = new Size(0, textSize.Height);
                    }
                }
            }
            else
            {
                //this.MinSize = Size.Empty;
                res = base.GetPreferredSizeCore(proposedSize);
            }

            return res;
        }

        // helper method which changes the whitespaces with a dummy character


        public StringFormat CreateStringFormat()
        {
            TextParams textParams = this.CreateTextParams();
            return textParams.CreateStringFormat();
        }
     

        public static RadProperty ShadowProperty = RadProperty.Register(
            "Shadow", typeof(ShadowSettings), typeof(TextPrimitive), new RadElementPropertyMetadata(
                null, ElementPropertyOptions.AffectsDisplay));

        /// <summary>Gets or sets the shadow settings.</summary>
        [RadPropertyDefaultValue("Shadow", typeof(TextPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
        [TypeConverter( DesignerConsts.ShadowSettingsConverterString)]
        [Editor(DesignerConsts.ShadowSettingsEditorString, typeof(UITypeEditor))]
        public ShadowSettings Shadow
        {
            get
            {
                return (ShadowSettings)this.GetValue(ShadowProperty);
            }
            set
            {
                this.SetValue(ShadowProperty, value);
            }
        }

        public RectangleF GetFaceRectangle()
        {
            Size size = this.Size;
            Padding padding = this.Padding;

            return new Rectangle(padding.Left, padding.Top, size.Width - padding.Horizontal, size.Height - padding.Vertical);
        }

        /// <summary>Draws the primitive on the screen.</summary>
        public override void PaintPrimitive(IGraphics graphics, float angle, SizeF scale)
        {
            TextParams textParams = this.CreateTextParams();
            this.textPrimitiveImpl.PaintPrimitive(graphics, angle, scale, textParams);            
        }


        /// <summary>Returns the text as a string.</summary>
        public override string ToString()
        {
            return "Text: " + this.Text;
        }


        //TODO: Check whether everything is working
        //protected override void OnFontChanged(EventArgs e)
        //{
        //    base.OnFontChanged(e);
        //    this.ToggleHTML(this.Text);
        //}

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (e.Property == TextPrimitive.TextProperty)
            {
                this.textWithoutMnemonics = TelerikHelper.TextWithoutMnemonics((string)e.NewValue);

                this.ToggleHTML(this.textWithoutMnemonics);
            }
            else if (e.Property == AlignmentProperty || e.Property == FontProperty || e.Property == ForeColorProperty)
            {
                this.ToggleHTML(this.textWithoutMnemonics);
            }
            else if (e.Property == RadElement.AutoSizeModeProperty)
            {
                RadAutoSizeMode newAutoSizeMode = (RadAutoSizeMode)e.NewValue;
                if (newAutoSizeMode == RadAutoSizeMode.WrapAroundChildren)
                {
                    this.TextAlignment = ContentAlignment.MiddleLeft;
                }
                else if (newAutoSizeMode == RadAutoSizeMode.FitToAvailableSize)
                {
                    this.TextAlignment = ContentAlignment.TopLeft;
                }
            }

            base.OnPropertyChanged(e);
        }

        public override Filter GetStylablePropertiesFilter()
        {
            return new OrFilter(Telerik.WinControls.PropertyFilter.TextPrimitiveFilter,
                Telerik.WinControls.PropertyFilter.AppearanceFilter,
                Telerik.WinControls.PropertyFilter.BehaviorFilter);
        }


        private bool useHTMLRendering = false;
        private bool disableHTMLRendering = false;
        private Color cachedForeColor = Color.Black;

        public bool DisableHTMLRendering
        {
            get { return this.disableHTMLRendering; }
            set
            {
                this.disableHTMLRendering = value;                
                this.useHTMLRendering = TinyHTMLParsers.IsHTMLMode(this.Text);
                this.textPrimitiveImpl = TextPrimitiveFactory.CreateTextPrimitiveImp(this.AllowHTMLRendering());
             }
        }

        public bool AllowHTMLRendering()
        {
            return !DisableHTMLRendering && this.useHTMLRendering;
        }

        public void ToggleHTML(string text)
        {
            this.useHTMLRendering = TinyHTMLParsers.IsHTMLMode(text);
            if (!this.DisableHTMLRendering)
            {
                this.textPrimitiveImpl = TextPrimitiveFactory.CreateTextPrimitiveImp(TinyHTMLParsers.IsHTMLMode(text));              
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            this.textPrimitiveImpl.OnMouseMove(this, e);
        }

        protected internal virtual TextParams CreateTextParams()
        {
            TextParams textParams = new TextParams();
            textParams.text = this.Text;
            textParams.alignment = GetTextAlignment();
            textParams.autoEllipsis = this.AutoEllipsis;
            //if (this.ElementTree!=null)
            //{
            //    textParams.autoSize = this.ElementTree.Control.AutoSize;
            //    textParams.maxSize = this.ElementTree.Control.MaximumSize;
            //}

            textParams.flipText = this.FlipText;
            textParams.font = this.Font;
            textParams.foreColor = this.ForeColor;
            textParams.measureTrailingSpaces = this.MeasureTrailingSpaces;
            textParams.paintingRectangle = new RectangleF( PointF.Empty, this.Size );
            textParams.rightToLeft = this.RightToLeft;
            textParams.shadow = this.Shadow;
            textParams.showKeyboardCues = this.ShowKeyboardCues;
            textParams.textOrientation = this.TextOrientation;
            textParams.textRenderingHint = this.TextRenderingHint;
            textParams.textWrap = this.TextWrap;
            textParams.useCompatibleTextRendering = this.UseCompatibleTextRendering;
            textParams.useMnemonic = this.UseMnemonic;
            textParams.stretchHorizontally = this.StretchHorizontally;
            return textParams;
        }

        #region ITextPrimitive Members

        public void PaintPrimitive(IGraphics graphics, float angle, SizeF scale, TextParams textParams)
        {
            this.textPrimitiveImpl.PaintPrimitive(graphics, angle, scale, textParams);
        }

        public void PaintPrimitive(IGraphics graphics, TextParams textParams)
        {
            this.textPrimitiveImpl.PaintPrimitive(graphics, textParams);
        }

        public SizeF MeasureOverride(SizeF availableSize, TextParams textParams)
        {
            return this.textPrimitiveImpl.MeasureOverride(availableSize, textParams);
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            this.textPrimitiveImpl.OnMouseMove(sender, e);
        }

        public SizeF GetTextSize(SizeF proposedSize, TextParams textParams)
        {
            return this.textPrimitiveImpl.GetTextSize(proposedSize, textParams);
        }

        public SizeF GetTextSize(TextParams textParams)
        {
            return this.textPrimitiveImpl.GetTextSize(textParams);
        }

        #endregion
    }
}
