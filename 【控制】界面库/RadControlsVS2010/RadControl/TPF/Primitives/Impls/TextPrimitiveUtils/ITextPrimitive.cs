using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Paint;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.TextPrimitiveUtils;

namespace Telerik.WinControls.Primitives
{
    public interface ITextPrimitive
    {
        void PaintPrimitive(IGraphics graphics, float angle, SizeF scale, TextParams textParams);
        void PaintPrimitive(IGraphics graphics, TextParams textParams);
        SizeF MeasureOverride(SizeF availableSize, TextParams textParams);
        void OnMouseMove(object sender, MouseEventArgs e);
        SizeF GetTextSize(SizeF proposedSize, TextParams textParams);
        SizeF GetTextSize( TextParams textParams);
        //StringFormat CreateStringFormat( TextParams textParams);
    }   
}
