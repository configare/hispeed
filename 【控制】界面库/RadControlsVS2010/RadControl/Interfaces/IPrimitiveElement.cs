using System.Drawing;
using System.Drawing.Drawing2D;
using Telerik.WinControls.Paint;

namespace Telerik.WinControls.Primitives
{
    public interface IPrimitiveElement
    {
        Size Size { get; }
        bool IsDesignMode { get; }
        //FillElementPaintBuffer FillElementPaintBuffer { get; }
        RadElement Parent { get; }
        ComponentThemableElementTree ElementTree { get; }
        //TODO: border thickness should be calculated for each side of the bounding rectangle
        float BorderThickness { get; }
        RectangleF GetPaintRectangle(float left, float angle, SizeF scale);        
        bool ShouldUsePaintBuffer();
        ElementShape GetCurrentShape();
        RectangleF GetExactPaintingRectangle(float angle, SizeF scale);
        Font Font { get; set; }
    }
}