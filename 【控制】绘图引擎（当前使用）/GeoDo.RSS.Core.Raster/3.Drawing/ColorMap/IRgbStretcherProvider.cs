using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public interface IRgbStretcherProvider
    {
        object GetStretcher(string fname, string colorTableName, out ColorMapTable<int> colorMapTable);
        object GetStretcher(string fname, enumDataType inDataType, string colorTableName, out ColorMapTable<int> colorMapTable);
    }
}
