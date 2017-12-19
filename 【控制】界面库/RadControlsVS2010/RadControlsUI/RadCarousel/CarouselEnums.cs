using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public enum AutoLoopDirections
    {
        Forward,
        Backward,
        //DependOnMousePosition
    }

    [Flags]
    public enum AutoLoopPauseConditions
    {
        None,
        OnMouseOverCarousel = 1,
        OnMouseOverItem = 2
        //MouseCloseToCenter = 4
        //MouseOverNavigationButtons = 8
    }
}
