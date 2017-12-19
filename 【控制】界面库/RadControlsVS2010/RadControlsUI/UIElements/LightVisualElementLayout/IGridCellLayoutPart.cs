using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public interface IVisualLayoutPart
    {
        RectangleF Bounds { get; }
        SizeF DesiredSize { get; }
        Padding Margin
        {
            get;
            set;
        }
        Padding Padding
        {
            get;
            set;
        }
        SizeF Measure(SizeF availableSize);
        SizeF Arrange(RectangleF bounds);
    }
}
