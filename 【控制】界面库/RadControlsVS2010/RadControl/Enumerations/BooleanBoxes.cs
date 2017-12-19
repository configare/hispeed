using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    internal static class BooleanBoxes
    {
        // Methods
        static BooleanBoxes()
        {
            BooleanBoxes.TrueBox = true;
            BooleanBoxes.FalseBox = false;
        }

        internal static object Box(bool value)
        {
            if (value)
            {
                return BooleanBoxes.TrueBox;
            }
            return BooleanBoxes.FalseBox;
        }

        // Fields
        internal static object FalseBox;
        internal static object TrueBox;
    }
}
