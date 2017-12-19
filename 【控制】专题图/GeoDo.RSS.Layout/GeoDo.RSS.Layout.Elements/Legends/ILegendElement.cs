using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Layout.Elements
{
    public interface ILegendElement: ISizableElement, IDisposable,IRasterLegend
    {
        LegendItem[] LegendItems { get; set; }
        int LegendTextSpan { get; set; }
        Font LegendTextFont { get; set; }
        string Text { get; set; }
        Color Color { get; set; }
        bool IsShowBorder { get; set; }
        Color BorderColor { get; set; }
    }
}
