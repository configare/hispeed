using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI.Docking
{
    public interface IDockingGuidesTemplate: IDisposable
    {
        int DockingHintBorderWidth { get; }
        Color DockingHintBackColor { get; }
        Color DockingHintBorderColor { get; }

        IDockingGuideImage LeftImage { get; }
        IDockingGuideImage RightImage { get; }
        IDockingGuideImage TopImage { get; }
        IDockingGuideImage BottomImage { get; }
        IDockingGuideImage FillImage { get; }
        IDockingGuideImage CenterBackgroundImage { get; }
    }
}
