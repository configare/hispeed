using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.RasterDrawing;

namespace GeoDo.RSS.Core.View
{
    public interface IRasterLayer
    {
        IRasterDrawing CurrentRasterDrawing { get; set; }
    }
}
