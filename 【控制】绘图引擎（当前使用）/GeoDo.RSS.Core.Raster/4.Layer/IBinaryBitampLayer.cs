using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public interface IBinaryBitampLayer:ILayer,IRasterLayer
    {
        string Name { get; }
        Bitmap Bitmap { get; set; }
    }
}
