using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Paint;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.TextPrimitiveUtils;

namespace Telerik.WinControls.Primitives
{
    public interface ITextProvider
    {
        string Text { get; set; }
        ContentAlignment TextAlignment { get; set; }
        ShadowSettings Shadow { get; set; }
        Orientation TextOrientation { get; set; }
        bool FlipText { get; set; }
        bool AutoEllipsis { get; set; }
        bool UseMnemonic { get; set; }
        RectangleF GetFaceRectangle();
        bool TextWrap { get; set; }
        Font Font { set; get; }
        bool ShowKeyboardCues { get; set; }
        bool MeasureTrailingSpaces { set; get; }
    }
}
