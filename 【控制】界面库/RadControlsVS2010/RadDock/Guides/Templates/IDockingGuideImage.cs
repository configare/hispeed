using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI.Docking
{
    public interface IDockingGuideImage
    {
        Point LocationOnCenterGuide { get; set; }
        Image Image { get; set; }
        Image HotImage { get; set; }
        Size PreferredSize { get; }
    }
}
