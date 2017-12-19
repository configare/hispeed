using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.AgileMap.Core
{
    public interface ILocatingFocusLayer:ILightLayer
    {
        bool Enabled { get; set; }
        void Focus(ShapePoint locationPrj, int times, int interval);
        void Focus(ShapePoint locationPrj, int times, int interval,string labelString);
        bool IsShowBubble { get; set; }
        Image BubbleImage { get; set; }
        float BubbleOffsetX { get; set; }
        float BubbleOffsetY { get; set; }
        RectangleF BubbleRect { get; }
    }
}
