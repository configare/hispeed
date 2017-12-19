using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class ColorMapTable<T>
    {
        private List<ColorMapItem<T>> _items = new List<ColorMapItem<T>>();
        public Color DefaultColor = Color.Transparent;

        public ColorMapTable()
        { 
        }

        public List<ColorMapItem<T>> Items
        {
            get { return _items; }
        }
    }
}
