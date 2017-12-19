using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    internal static class VisibilityBoxes
    {
        // Methods
        static VisibilityBoxes()
        {
            VisibilityBoxes.VisibleBox = ElementVisibility.Visible;
            VisibilityBoxes.HiddenBox = ElementVisibility.Hidden;
            VisibilityBoxes.CollapsedBox = ElementVisibility.Collapsed;
        }

        internal static object Box(ElementVisibility value)
        {
            if (value == ElementVisibility.Visible)
            {
                return VisibilityBoxes.VisibleBox;
            }
            if (value == ElementVisibility.Hidden)
            {
                return VisibilityBoxes.HiddenBox;
            }
            return VisibilityBoxes.CollapsedBox;
        }

        // Fields
        internal static object CollapsedBox;
        internal static object HiddenBox;
        internal static object VisibleBox;
    }
}
