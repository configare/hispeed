using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    internal static class AutoSizeModeBoxes
    {
        // Methods
        static AutoSizeModeBoxes()
        {
            AutoSizeModeBoxes.AutoBox = RadAutoSizeMode.Auto;
            AutoSizeModeBoxes.WrapAroundChildrenBox = RadAutoSizeMode.WrapAroundChildren;
            AutoSizeModeBoxes.FitToAvailableSizeBox = RadAutoSizeMode.FitToAvailableSize;
        }

        internal static object Box(RadAutoSizeMode value)
        {
            if (value == RadAutoSizeMode.Auto)
            {
                return AutoSizeModeBoxes.AutoBox;
            }
            if (value == RadAutoSizeMode.WrapAroundChildren)
            {
                return AutoSizeModeBoxes.WrapAroundChildrenBox;
            }
            return AutoSizeModeBoxes.FitToAvailableSizeBox;
        }

        // Fields
        internal static object AutoBox;
        internal static object WrapAroundChildrenBox;
        internal static object FitToAvailableSizeBox;
    }
}
