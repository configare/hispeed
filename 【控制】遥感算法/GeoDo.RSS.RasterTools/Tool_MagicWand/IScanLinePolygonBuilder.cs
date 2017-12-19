using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.RasterTools
{
    public interface IScanLinePolygonBuilder
    {
        void AddScanLine(int row, int bCol, int eCol);
        ScanLinePolygon Build();
    }
}
