using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class ColorMapItem<T>
    {
        public T MinValue;
        public T MaxValue;
        public Color Color;

        public ColorMapItem(T minValue, T maxValue, Color color)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Color = color;
        }
    }
}
