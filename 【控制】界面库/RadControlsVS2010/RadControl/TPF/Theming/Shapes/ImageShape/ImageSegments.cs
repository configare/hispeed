using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    [Flags]
    public enum ImageSegments
    {
        Left = 1,
        TopLeft = Left << 1,
        Top = TopLeft << 1,
        TopRight = Top << 1,
        Right = TopRight << 1,
        BottomRight = Right << 1,
        Bottom = BottomRight << 1,
        BottomLeft = Bottom << 1,
        Inner = BottomLeft << 1,
        All = Left | TopLeft | Top | TopRight | Right | BottomRight | Bottom | BottomLeft | Inner
    }
}
