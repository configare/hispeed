using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.TextPrimitiveUtils;
using Telerik.WinControls.Layouts;
using System.Windows.Forms;
using System.Drawing.Text;

namespace Telerik.WinControls.Primitives
{
    public class TextPrimitiveHtmlImpl : ITextPrimitive
    {
        #region const

        readonly SizeF scalingSize = new SizeF(1f, 1f);

        #endregion

        #region fields

        protected bool isDirty;
        private SizeF previousAvailableSize = SizeF.Empty;
        private bool useHTMLRendering = false;
        private bool isMouseAlreadyPress = false;
        private FormattedTextBlock textBlock = new FormattedTextBlock();

        #endregion

        public TextPrimitiveHtmlImpl()
        {
            this.isDirty = true;            
        }
        
        #region ITextPrimitive Members        

        public void PaintPrimitive(Telerik.WinControls.Paint.IGraphics graphics, float angle, System.Drawing.SizeF scale, TextParams textParams)
        {
            if (this.isDirty)
            {
                this.isDirty = false;
                this.TextBlock = TinyHTMLParsers.Parse(textParams);
            }

            RectangleF paintingRectangle = textParams.paintingRectangle;
            StringFormat format = textParams.CreateStringFormat();           
            this.TextBlock.PaintFormatTextBlock(graphics, paintingRectangle, true, format, TextFormatFlags.Default, textParams.textWrap, textParams.ClipText);
            format.Dispose();
        }

        public void PaintPrimitive(Telerik.WinControls.Paint.IGraphics graphics, TextParams textParams)
        {
            this.PaintPrimitive(graphics, 0f, scalingSize, textParams);
        }
        
        public System.Drawing.SizeF MeasureOverride(System.Drawing.SizeF availableSize, TextParams textParams)
        {
            if (previousAvailableSize != availableSize)
            {
                this.isDirty = true;
                previousAvailableSize = availableSize;
            }

            if (this.isDirty)
            {
                this.isDirty = false;
                this.TextBlock = TinyHTMLParsers.Parse(textParams);
            }

            Size textSize = Size.Ceiling( this.TextBlock.GetTextSize(availableSize, textParams));
            Size res = LayoutUtils.FlipSizeIf(textParams.textOrientation == Orientation.Vertical, textSize);
            Size minSize = LayoutUtils.FlipSizeIf(textParams.textOrientation == Orientation.Vertical, textSize);
            res.Width = Math.Max(res.Width, minSize.Width);
            res.Height = Math.Max(res.Height, minSize.Height);
            return res;
        }

        #endregion

        #region html helpers

        public bool AllowHTMLRendering()
        {
            return /*!DisableHTMLRendering &&*/ this.useHTMLRendering;
        }      

        public FormattedTextBlock TextBlock
        {
            get
            {
                return textBlock;
            }
            set
            {
                textBlock = value;
            }
        }

        #region Link Support
       
        public void OnMouseMove(object sender, MouseEventArgs e)
        {  
            this.textBlock.MouseMove(sender, e);
          
            if ((Control.MouseButtons & MouseButtons.Left) != 0)
            {
                if (!this.isMouseAlreadyPress)
                {
                    this.isMouseAlreadyPress = true;
                    this.textBlock.MouseUp(sender, e);
                }
            }
            else
            { 
                this.isMouseAlreadyPress = false;               
            }
        }

        #endregion

        #endregion

        public SizeF GetTextSize(TextParams textParams)
        {
            if (this.isDirty)
            {
                this.isDirty = false;
                this.TextBlock = TinyHTMLParsers.Parse(textParams);
            }

            SizeF proposedSize = new SizeF(float.MaxValue, float.MaxValue);           
            SizeF textSize = this.TextBlock.GetTextSize(proposedSize, textParams);			
            return textSize;
        }

        public SizeF GetTextSize(SizeF proposedSize, TextParams textParams)
        {
            if (this.isDirty)
            {
                this.isDirty = false;
                this.TextBlock = TinyHTMLParsers.Parse(textParams);
            }

            SizeF res = this.TextBlock.GetTextSize(proposedSize, textParams);            
            return res;
        }    
    }
}
