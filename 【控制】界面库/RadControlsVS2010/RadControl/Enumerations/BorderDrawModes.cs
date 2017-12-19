namespace Telerik.WinControls
{
    /// <summary>
    /// Defines drawing border corners.
    /// </summary>
    public enum BorderDrawModes
    {
        HorizontalOverVertical = 0,
        LeftOverTop = 1,
        RightOverTop = 2,
        LeftOverBottom = 4,
        RightOverBottom = 8, 
        TopLeading = RightOverTop | LeftOverBottom,
        LeftLeading = LeftOverTop | RightOverBottom,
        VerticalOverHorizontal = RightOverTop | LeftOverBottom | LeftOverTop | RightOverBottom,
        TopOver = LeftOverBottom | RightOverBottom, 
        BottomOver = RightOverTop | LeftOverTop,
        LeftOver = LeftOverBottom | LeftOverTop,
        RightOver = RightOverTop | RightOverBottom
    }
}
