using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Layout
{
    public abstract class RasterLegendElement : SizableElement, IPersitable
    {
        public static Func<string, LegendItem[]> RasterLegendItemsGetter;

        public RasterLegendElement()
            : base()
        { 
        }
    }
}
