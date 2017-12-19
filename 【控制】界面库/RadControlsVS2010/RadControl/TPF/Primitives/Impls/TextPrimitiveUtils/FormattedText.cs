using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Paint;

namespace Telerik.WinControls.TextPrimitiveUtils
{
    public class FormattedText
    {
        public enum HTMLLikeListType : byte
        {
            None,
            OrderedList,
            List
        }       

        const float DefaultOffsetSize = 30f;
        const char BulletSymbol = ((char)183);        

        //members
        protected string fontName;
        internal string text = string.Empty;
        protected Color fontColor;
        protected FontStyle fontStyle;
        protected ContentAlignment contentAlignment;
        protected SizeF blockSize;
        protected float fontSize;
        protected string htmlTag;
        protected Color? bgColor;
        protected Image image;
        protected string link;
        protected int offset;
        protected float offsetSize;
        protected int number;//if this is -1 means to use non ordered lists
        protected HTMLLikeListType prevListType;
        protected HTMLLikeListType listType;
        protected bool shouldDisplayBullet;
        private bool isDirtySize;
        protected bool startNewLine;
        protected SizeF bulletSize;
        private bool isClosingTag;
        private RectangleF drawingRectangle;
        private FontStyle bulletFontStyle;		
        private float bulletFontSize;
        private string bulletFontName;
        private float baseLine;		
        [ThreadStatic]
        private static Dictionary<int, float> baseLines;// = new Dictionary<int, float>(4);
        //constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public FormattedText()
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="prototypeFormattedText"></param>
        public FormattedText(FormattedText prototypeFormattedText)
        {
            //copy all setting from originalFormattedText except Text and BlockSize
            this.FontName = prototypeFormattedText.fontName;
            this.fontColor = prototypeFormattedText.fontColor;
            this.fontStyle = prototypeFormattedText.fontStyle;
            this.FontSize = prototypeFormattedText.FontSize;
            this.contentAlignment = prototypeFormattedText.contentAlignment;
            this.htmlTag = prototypeFormattedText.htmlTag;
            this.bgColor = prototypeFormattedText.bgColor;
            this.link = prototypeFormattedText.link;
            this.offset = prototypeFormattedText.offset;
            this.offsetSize = prototypeFormattedText.offsetSize;
            this.number = prototypeFormattedText.number;
            this.listType = prototypeFormattedText.listType;
            this.bulletFontName = prototypeFormattedText.bulletFontName;
            this.bulletFontSize = prototypeFormattedText.bulletFontSize;
            this.bulletFontStyle = prototypeFormattedText.bulletFontStyle;

            if (string.IsNullOrEmpty(prototypeFormattedText.text))
            {
                this.shouldDisplayBullet = prototypeFormattedText.shouldDisplayBullet;
            }
        }      

        //properties
        /// <summary>
        /// Get or sets HTML tag of the current text block
        /// </summary>
        public string HtmlTag
        {
            get
            {
                return htmlTag;
            }
            set
            {
                htmlTag = value;
            }
        }

        /// <summary>
        /// Get or sets FontSize  the current text block
        /// Note: recreate the font
        /// </summary>
        public float FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                fontSize = value;
                this.isDirtySize = true;
            }
        }

        /// <summary>
        /// Get or sets Image for the current text block
        /// Current block should be named Image block
        /// </summary>
        public Image Image
        {
            get
            {
                return this.image;
            }
            set
            {
                this.image = value;
                this.isDirtySize = true;
            }
        }

        /// <summary>
        /// Get or sets the Size the current text block
        /// </summary>
        public SizeF BlockSize
        {
            get
            {
                if (isDirtySize || (blockSize == SizeF.Empty && !string.IsNullOrEmpty(this.text)))
                {
                    this.blockSize = this.GetTextBlockSize(true, new TextFormatFlags());
                    this.isDirtySize = false;
                }

                return blockSize;
            }
            //set { blockSize = value; }
        }

        /// <summary>
        /// current block content alignment
        /// </summary>
        public ContentAlignment ContentAlignment
        {
            get
            {
                return contentAlignment;
            }
            set
            {
                contentAlignment = value;
            }
        }

        public FontStyle FontStyle
        {
            get
            {
                return this.fontStyle;
            }
            set
            {               
                this.fontStyle = this.SelectEffectiveStyle(this.fontName, value);
                this.isDirtySize = true;
            }
        }

        public Color FontColor
        {
            get
            {
                return fontColor;
            }
            set
            {
                fontColor = value;
            }
        }

        public string FontName
        {
            get
            {
                return this.fontName;
            }
            set
            {
                this.fontName = value;
                this.fontStyle = this.SelectEffectiveStyle(this.fontName, this.fontStyle); 
                this.isDirtySize = true;
            }
        }

        public Color? BgColor
        {
            get
            {
                return bgColor;
            }
            set
            {
                bgColor = value;
            }
        }

        /// <summary>
        /// Get or set the text
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
                this.isDirtySize = true;
            }
        }

        public string Link
        {
            set
            {
                link = value;
            }

            get
            {
                return link;
            }
        }

        public int Offset
        {
            set
            {
                offset = value;
                offsetSize = value * DefaultOffsetSize;
            }

            get
            {
                return offset;
            }
        }

        public float OffsetSize
        {
            get
            {
                return this.offsetSize;
            }
        }

        public int Number
        {
            set
            {
                this.number = value;
            }

            get
            {
                return this.number;
            }
        }

        public bool ShouldDisplayBullet
        {
            set
            {
                this.shouldDisplayBullet = value;
            }
            get
            {
                return this.shouldDisplayBullet;
            }
        }

        public string BulletFontName
        {
            set
            {
                bulletFontName = value;
            }

            get
            {
                return bulletFontName;
            }
        }

        public FontStyle BulletFontStyle
        {
            set
            {
                bulletFontStyle = value;
            }

            get
            {
                return bulletFontStyle;
            }
        }

        public float BulletFontSize
        {
            set
            {
                bulletFontSize = value;
            }

            get
            {
                return bulletFontSize;
            }
        }
        
        public HTMLLikeListType ListType
        {
            set
            {
                listType = value;
            }

            get
            {
                return listType;
            }
        }

        public bool StartNewLine
        {
            set
            {
                startNewLine = value;
            }

            get
            {
                return startNewLine;
            }
        }

        public SizeF BulletSize
        {
            get
            {
                return this.bulletSize;
            }
        }

        public bool IsClosingTag
        {
            set
            {
                isClosingTag = value;
            }

            get
            {
                return isClosingTag;
            }
        }

        public RectangleF DrawingRectangle
        {
            get
            {
                return this.drawingRectangle;
            }
        }

        /// <summary>
        /// BaseLine Property
        /// </summary>
        public float BaseLine
        {
            get
            {                
                return this.baseLine;
            }
            set
            {
                this.baseLine = value;
            }
        }

        //helper functions
        public SizeF GetTextBlockSize(bool useCompatibleTextRendering, TextFormatFlags textFormatFlags)
        {
            SizeF resultSize = SizeF.Empty;

            if (this.image != null)
            {
                resultSize = this.image.Size;
            }

            if (string.IsNullOrEmpty(this.Text) && !this.startNewLine)
            {
                return resultSize;
            }

            string textToMeasure = this.text;
            if (string.IsNullOrEmpty(this.Text) && this.startNewLine)
            {
                textToMeasure = "\n";
            }

            using (Font font = new Font(this.fontName, this.FontSize, this.FontStyle))
            {
                if (!useCompatibleTextRendering)
                {
                    SizeF textSize = TextRenderer.MeasureText(textToMeasure, font, new Size(int.MaxValue, int.MaxValue), textFormatFlags);
                    resultSize.Width += textSize.Width;
                    resultSize.Height = Math.Max( resultSize.Height, textSize.Height );
                    return resultSize;
                }
                
                using (StringFormat stringFormat = new StringFormat(StringFormat.GenericTypographic))
                {
                    stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.FitBlackBox | StringFormatFlags.MeasureTrailingSpaces;
                    stringFormat.Alignment = StringAlignment.Near;
                    stringFormat.LineAlignment = StringAlignment.Near;
                    stringFormat.Trimming = StringTrimming.None;
                    stringFormat.HotkeyPrefix = HotkeyPrefix.None;

                    SizeF textSize = SizeF.Empty;

                    lock (MeasurementGraphics.SyncObject)
                    {
                        Graphics g = RadGdiGraphics.MeasurementGraphics;

                        this.bulletSize = this.MeasureBullet(g, stringFormat);
                        this.baseLine = GetBaseLineFromFont(font);

                        textSize = g.MeasureString(textToMeasure, font, 0, stringFormat);

                        resultSize.Width += this.bulletSize.Width + textSize.Width;
                        //TODO: This fixes a 1-pixel error in GDI+                    
                        resultSize.Width++;

                        resultSize.Height = Math.Max(resultSize.Height, this.bulletSize.Height);
                        resultSize.Height = Math.Max(resultSize.Height, textSize.Height);

                        //g.Dispose();
                    }
                }
            }
            return resultSize;
        }

        /// <summary>
        /// GetBaseLineFromFont Method
        /// </summary>
        /// <param name="font">A Font</param>
        /// <returns>A float</returns>
        private static float GetBaseLineFromFont(Font font)
        {
           int hashCode = font.GetHashCode();
           if(baseLines == null)
           {
               baseLines = new Dictionary<int, float>(4);
           }

           if(baseLines.ContainsKey(hashCode))
           {
               return baseLines[hashCode];
           }

           FontTextMetrics textMetric = RadGdiGraphics.GetTextMetric(font);
           baseLines.Add(hashCode, textMetric.ascent);
           return textMetric.ascent;
        }

        private SizeF MeasureBullet(Graphics graphics, StringFormat sf)
        {
            if (!this.shouldDisplayBullet)
            {
                return SizeF.Empty;
            }

            Font bulletFont;
            if (this.listType == HTMLLikeListType.List)
            {
                bulletFont = new Font("Symbol", this.bulletFontSize, FontStyle.Regular);
            }
            else
            {
                bulletFont = new Font(this.bulletFontName, this.bulletFontSize, this.bulletFontStyle);
            }
            
            SizeF size = graphics.MeasureString(this.ProduceTextBullets(), bulletFont, 0, sf);
            bulletFont.Dispose();
            return size;
        }

        private string ProduceTextBullets()
        {
            if (!this.shouldDisplayBullet)
            {
                return string.Empty;
            }

            switch (this.listType)
            {
                case HTMLLikeListType.None:
                    return string.Empty;

                case HTMLLikeListType.OrderedList:
                    return this.number.ToString() + ". ";

                case HTMLLikeListType.List:
                    return BulletSymbol + " ";
            }

            return string.Empty;
        }

        private FontStyle SelectEffectiveStyle(string fontName, FontStyle fontStyle)
        {
            FontFamily fontFamily = null;
            try
            {
                fontFamily = new FontFamily(fontName);
            }
            catch
            {
                return FontStyle.Regular;   	
            }

            FontStyle effectiveStyle = fontStyle;
            if (fontFamily.IsStyleAvailable(effectiveStyle))
            {
                return effectiveStyle;
            }

            FontStyle biStyle = FontStyle.Bold | FontStyle.Italic;
            effectiveStyle &= biStyle;
            List<FontStyle> styles = new List<FontStyle>(4);
            if (effectiveStyle != biStyle)
            {
                styles.Add(FontStyle.Regular);
                styles.Add(FontStyle.Italic);
                styles.Add(FontStyle.Bold);
            }
            else
            {
                styles.Add(biStyle);
                styles.Add(FontStyle.Bold);
                styles.Add(FontStyle.Italic);
                styles.Add(FontStyle.Regular);
            }

            styles.Remove(effectiveStyle);
            for (int i = 0; i < styles.Count; i++)
            {
                if (fontFamily.IsStyleAvailable(styles[i]))
                {
                    effectiveStyle = styles[i];
                    break;
                }
            }

            //turn off the first two bits
            fontStyle &= FontStyle.Underline | FontStyle.Strikeout;
            //turn on the selected bits
            return fontStyle | effectiveStyle;
        }

        //static int i = 0;

        public void PaintFormatText(
            IGraphics graphics,
            RectangleF paintingRectangle,
            bool useCompatibleTextRendering,
            TextFormatFlags flags,
            PointF currentLineBeginLocation,
            float lineHeight,
            float lineBaseLine,
            bool clipText)
        {
            if (this.image != null)
            {
                graphics.DrawBitmap(this.image, (int)currentLineBeginLocation.X, (int)currentLineBeginLocation.Y);
                currentLineBeginLocation.X += this.image.Size.Width;//return;
            }

            if (string.IsNullOrEmpty(this.Text) || currentLineBeginLocation.X >= paintingRectangle.Right)
            {
                return;
            }

            using (Font font = new Font(this.FontName, this.FontSize, this.FontStyle))
            {
                if (!useCompatibleTextRendering)
                {
                    TextRenderer.DrawText((Graphics)graphics.UnderlayGraphics, this.ProduceTextBullets() + this.Text, font, new Rectangle((int)Math.Round(currentLineBeginLocation.X), (int)Math.Round(currentLineBeginLocation.Y), (int)Math.Round(this.BlockSize.Width), (int)Math.Round(lineHeight)), this.FontColor, flags);
                    return;
                }

                SizeF textSize = new SizeF(Math.Min(this.BlockSize.Width, paintingRectangle.Right - currentLineBeginLocation.X), Math.Min(this.BlockSize.Height, paintingRectangle.Height));
                using (StringFormat sf = new StringFormat(StringFormat.GenericTypographic))
                {
                    if (!clipText)
                    {
                        sf.FormatFlags |= StringFormatFlags.NoClip;
                    }
                    else
                    {
                        sf.FormatFlags &= ~StringFormatFlags.NoClip;
                    }

                    sf.FormatFlags |= StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces;
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Near;
                    sf.Trimming = StringTrimming.None;
                    sf.HotkeyPrefix = HotkeyPrefix.None;
                    sf.FormatFlags &= ~StringFormatFlags.LineLimit & ~StringFormatFlags.FitBlackBox;
                    if (this.bgColor != null)
                    {
                        this.DrawBgColor(currentLineBeginLocation, textSize, graphics);
                    }

                    RectangleF textRectangle = new RectangleF(currentLineBeginLocation, textSize);
                    SolidBrush brush = new SolidBrush(this.FontColor);
                    Graphics g = ((Graphics)graphics.UnderlayGraphics);
                    this.DrawBullets(g, textRectangle, sf, brush);
                    float drawingBeginPointX = textRectangle.X + this.bulletSize.Width;                    
                    float drawingBeginPointY = textRectangle.Y + (lineBaseLine - this.baseLine);
                    this.drawingRectangle = new RectangleF(drawingBeginPointX, drawingBeginPointY, textRectangle.Width, textRectangle.Height);
                    //TODO: This fixes a 1-pixel error in GDI+
                    if (clipText)
                    {
                        this.drawingRectangle.Width++;
                    }
                    g.DrawString(this.Text, font, brush, this.drawingRectangle, sf);
                    brush.Dispose();
                }
                //this draw debug rectangles around textblocks
                //++i;
                //graphics.DrawRectangle(Rectangle.Ceiling(drawingRectangle), i % 2 == 1 ? Color.Red : Color.Green);                    
            }
        }

        private void DrawBgColor(PointF location, SizeF textSize, IGraphics graphics)
        {
            int width = (int)(location.X + this.bulletSize.Width);
            Point fillRectanglePoint = new Point(width, (int)location.Y);
            Rectangle fillRectangle = new Rectangle(fillRectanglePoint, textSize.ToSize());
            graphics.FillRectangle(fillRectangle, this.bgColor.Value);
        }

        private void DrawBullets(Graphics g, RectangleF textRectangle, StringFormat sf, SolidBrush brush)
        {
            if (!this.shouldDisplayBullet)
            {
                return;
            }

            Font bulletFont;
            if (this.listType == HTMLLikeListType.List)
            {
                bulletFont = new Font("Symbol", this.bulletFontSize, FontStyle.Regular);
            }
            else
            {
                using (FontFamily fontFamily = new FontFamily(this.bulletFontName))
                {
                    if (fontFamily.IsStyleAvailable(FontStyle.Regular))
                    {
                        bulletFont = new Font(this.bulletFontName, this.bulletFontSize, FontStyle.Regular);
                    }
                    else
                    {
                        bulletFont = new Font(this.bulletFontName, this.bulletFontSize, this.SelectEffectiveStyle(this.bulletFontName, bulletFontStyle));
                    }
                }
            }

            g.DrawString(this.ProduceTextBullets(), bulletFont, brush, LayoutUtils.VAlign(this.bulletSize, textRectangle, ContentAlignment.MiddleLeft), sf);

            bulletFont.Dispose();
        }
    }
}
