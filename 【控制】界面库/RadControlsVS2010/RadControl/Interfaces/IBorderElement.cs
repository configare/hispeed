using System.Drawing;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.Primitives
{
    public interface IBorderElement : IBoxStyle, IBoxElement
    {
        Color ForeColor { get; }
        Color ForeColor2 { get; }
        Color ForeColor3 { get; }
        Color ForeColor4 { get; }

        Color InnerColor { get; }
        Color InnerColor2 { get; }
        Color InnerColor3 { get; }
        Color InnerColor4 { get; }
        BorderBoxStyle BoxStyle { get; }
        GradientStyles GradientStyle { get; }
        float GradientAngle { get; }
        BorderDrawModes BorderDrawMode { get; }
    }
}