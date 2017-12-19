using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.TextPrimitiveUtils;
using Telerik.WinControls.Layouts;
using System.Windows.Forms;
using System.Drawing.Text;
using Telerik.WinControls.Paint;

namespace Telerik.WinControls.Primitives
{
    public class TextPrimitiveImpl : ITextPrimitive
    {
        #region Fields
        private SizeF measuredSize;

        #endregion

        #region Consts

        private LayoutUtils.MeasureTextCache textMeasurementCache = null;
        private const ContentAlignment anyRight = ContentAlignment.BottomRight | ContentAlignment.MiddleRight | ContentAlignment.TopRight;
        private const ContentAlignment anyBottom = ContentAlignment.BottomRight | ContentAlignment.BottomCenter | ContentAlignment.BottomLeft;
        private const ContentAlignment anyCenter = ContentAlignment.BottomCenter | ContentAlignment.MiddleCenter | ContentAlignment.TopCenter;
        private const ContentAlignment anyMiddle = ContentAlignment.MiddleRight | ContentAlignment.MiddleCenter | ContentAlignment.MiddleLeft;
        private const ContentAlignment anyTop = ContentAlignment.TopRight | ContentAlignment.TopCenter | ContentAlignment.TopLeft;

        #endregion       

        #region ITextPrimitive Members

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
        }

        public void PaintPrimitive(IGraphics graphics, float angle, SizeF scale, TextParams textParams)
        {
            if (textParams.useCompatibleTextRendering)
            {
                graphics.DrawString(textParams, this.measuredSize);
            }
            else
            {
                TextFormatFlags flags = this.CreateTextFormatFlags(textParams);
                Graphics underlayGraphics = (Graphics)graphics.UnderlayGraphics;
                System.Drawing.Drawing2D.GraphicsState state = underlayGraphics.Save();
                underlayGraphics.ResetTransform();
                TextRenderer.DrawText(underlayGraphics,
                                      textParams.text,
                                      textParams.font,
                                      Rectangle.Ceiling(textParams.paintingRectangle),
                                      textParams.foreColor,
                                      flags);
                underlayGraphics.Restore(state);       
            }
        }

        public void PaintPrimitive(Telerik.WinControls.Paint.IGraphics graphics, TextParams textParams)
        {
            this.PaintPrimitive(graphics, 0f, new SizeF(1f, 1f), textParams);
        }

        public SizeF MeasureOverride(SizeF availableSize, TextParams textParams)
        {           
            SizeF textSize = this.GetTextSize(availableSize, textParams);
            SizeF res = LayoutUtils.FlipSizeIf(textParams.textOrientation == Orientation.Vertical, textSize);
            SizeF minSize = LayoutUtils.FlipSizeIf(textParams.textOrientation == Orientation.Vertical, textSize);

            res.Width = Math.Max(res.Width, minSize.Width);
            res.Height = Math.Max(res.Height, minSize.Height);

            return res;
        }

        #endregion

        #region Helper function

        public SizeF GetTextSize(SizeF proposedSize, TextParams textParams)
        {
            SizeF res = SizeF.Empty;
            if (string.IsNullOrEmpty(textParams.text) || proposedSize.Width == 0)
            {
                return SizeF.Empty;
            }

            StringFormat sf = textParams.CreateStringFormat();
            int avaibleWidth = (int)(float.IsInfinity(proposedSize.Width) ? int.MaxValue : Math.Floor(proposedSize.Width));                               
            string textToMeasure = textParams.text;

            if (textParams.useCompatibleTextRendering)
            {              
                //p.p. 29.07.09
                //fix very strange bug with Hebrew and FitToGrid Renedering hint
                lock (MeasurementGraphics.SyncObject)
                {
                    Graphics g = RadGdiGraphics.MeasurementGraphics;
                    g.TextRenderingHint = textParams.textRenderingHint;
                    res = g.MeasureString(textToMeasure, textParams.font, avaibleWidth, sf);

                    res.Height = (float)Math.Round(res.Height);
                    res.Width = (float)Math.Ceiling(res.Width);

                    //g.Dispose();
                }
            }
            else if (!string.IsNullOrEmpty(textToMeasure))
            {
                TextFormatFlags flags = this.CreateTextFormatFlags(textParams);
                res = TextRenderer.MeasureText(textToMeasure, textParams.font, new Size(avaibleWidth, int.MaxValue), flags);
            }
            
            sf.Dispose();

            this.measuredSize = res;
            return res;
        }

        //used only in old layouts
        public SizeF GetTextSize(SizeF proposedSize, bool autoFitInProposedSize, TextParams textParams)
        {
            SizeF res = SizeF.Empty;
            StringFormat sf = textParams.CreateStringFormat();
            string textToMeasure = textParams.text;

            //p.p. 29.07.09
            //fix very strange bug with Hebrew and FitToGrid Renedering hint
            lock (MeasurementGraphics.SyncObject)
            {
                Graphics g = RadGdiGraphics.MeasurementGraphics;

                g.TextRenderingHint = textParams.textRenderingHint;
                //StringFormat sf = this.CreateStringFormat();
                string measureableString = GetHelperString(textToMeasure);

                if (autoFitInProposedSize)
                {
                    res = g.MeasureString(measureableString, textParams.font, proposedSize, sf);
                }
                else
                {
                    res = g.MeasureString(measureableString, textParams.font, 0, sf);
                }

                //g.Dispose();
            }

            sf.Dispose();

            this.measuredSize = res;
            return res;
        }

        /// <summary>Retrieves the text size.</summary>
        public SizeF GetTextSize(TextParams textParams)
        {
            return this.GetTextSize(LayoutUtils.MaxSizeF, textParams);
        }
        
        public ContentAlignment RtlContentAlignment(TextParams textParams)
        {
            if (!textParams.rightToLeft)
            {
                return textParams.alignment;
            }

            if ((textParams.alignment & WindowsFormsUtils.AnyTopAlign) != ((ContentAlignment)0))
            {
                switch (textParams.alignment)
                {
                    case ContentAlignment.TopLeft:
                        return ContentAlignment.TopRight;

                    case ContentAlignment.TopRight:
                        return ContentAlignment.TopLeft;
                }
            }
            if ((textParams.alignment & WindowsFormsUtils.AnyMiddleAlign) != ((ContentAlignment)0))
            {
                switch (textParams.alignment)
                {
                    case ContentAlignment.MiddleLeft:
                        return ContentAlignment.MiddleRight;

                    case ContentAlignment.MiddleRight:
                        return ContentAlignment.MiddleLeft;
                }
            }
            if ((textParams.alignment & WindowsFormsUtils.AnyBottomAlign) == ((ContentAlignment)0))
            {
                return textParams.alignment;
            }
            ContentAlignment alignment3 = textParams.alignment;
            if (alignment3 != ContentAlignment.BottomLeft)
            {
                if (alignment3 == ContentAlignment.BottomRight)
                {
                    return ContentAlignment.BottomLeft;
                }
                return textParams.alignment;
            }
            return ContentAlignment.BottomRight;
        }

        public TextFormatFlags CreateTextFormatFlags(TextParams textParams)
        {
            ContentAlignment textAlign = this.RtlContentAlignment(textParams);
            TextFormatFlags flags = TranslateAlignmentForGDI(textAlign);//|(TextFormatFlags.TextBoxControl| TextFormatFlags.WordBreak);
            if (textParams.autoEllipsis)
            {
                flags |= TextFormatFlags.EndEllipsis;
            }

            if (textParams.rightToLeft)
            {
                flags |= TextFormatFlags.RightToLeft;
            }

            if (!textParams.useMnemonic)
            {
                flags |= TextFormatFlags.NoPrefix;
            }

            if (!textParams.showKeyboardCues)
            {
                flags |= TextFormatFlags.HidePrefix;
            }

            if (!textParams.textWrap)
            {
                flags |= TextFormatFlags.SingleLine;
            }

            flags |= TextFormatFlags.PreserveGraphicsTranslateTransform;// | TextFormatFlags.NoPadding;
            return flags;
        }

        internal TextFormatFlags TextFormatFlagsForAlignmentGDI(ContentAlignment align)
        {
            TextFormatFlags result = TranslateAlignmentForGDI(align);
            return result;
        }

        internal TextFormatFlags TranslateLineAlignmentForGDI(ContentAlignment align)
        {
            if ((align & anyRight) != ((ContentAlignment)0))
            {
                return TextFormatFlags.Right;
            }
            if ((align & anyCenter) != ((ContentAlignment)0))
            {
                return TextFormatFlags.HorizontalCenter;
            }
            return TextFormatFlags.GlyphOverhangPadding;
        }

        internal TextFormatFlags TranslateAlignmentForGDI(ContentAlignment align)
        {
            TextFormatFlags result = TextFormatFlags.Default;
            switch (align)
            {
                case ContentAlignment.BottomCenter:
                    result = TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom;
                    break;
                case ContentAlignment.BottomLeft:
                    result = TextFormatFlags.Left | TextFormatFlags.Bottom;
                    break;
                case ContentAlignment.BottomRight:
                    result = TextFormatFlags.Right | TextFormatFlags.Bottom;
                    break;
                case ContentAlignment.MiddleCenter:
                    result = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.MiddleLeft:
                    result = TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
                    break;
                case ContentAlignment.MiddleRight:
                    result = TextFormatFlags.VerticalCenter | TextFormatFlags.Right;
                    break;
                case ContentAlignment.TopCenter:
                    result = TextFormatFlags.HorizontalCenter | TextFormatFlags.Top;
                    break;
                case ContentAlignment.TopLeft:
                    result = TextFormatFlags.Left | TextFormatFlags.Top;
                    break;
                case ContentAlignment.TopRight:
                    result = TextFormatFlags.Right | TextFormatFlags.Top;
                    break;
                default:
                    break;
            }

            return result;
        }

        private string GetHelperString(string s)
        {
            StringBuilder sb = new StringBuilder();

            int i = 0;
            while (i < s.Length)
            {
                if (s[i].Equals(' '))
                {
                    sb.Append("a");
                }
                else
                {
                    sb.Append(s[i]);
                }
                i++;
            }

            return sb.ToString();
        }

        internal LayoutUtils.MeasureTextCache MeasureTextCache
        {
            get
            {
                if (this.textMeasurementCache == null)
                {
                    this.textMeasurementCache = new LayoutUtils.MeasureTextCache();
                }
                return this.textMeasurementCache;
            }
        }

        internal virtual TextFormatFlags CreateTextFormatFlags(Size constrainingSize, TextParams textParams)
        {
            TextFormatFlags flags = CreateTextFormatFlags(textParams);

            if (!this.MeasureTextCache.TextRequiresWordBreak(textParams.text, textParams.font, constrainingSize, flags))
            {
                flags &= ~(TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak);
            }
            return flags;
        }
        #endregion
    }
}
