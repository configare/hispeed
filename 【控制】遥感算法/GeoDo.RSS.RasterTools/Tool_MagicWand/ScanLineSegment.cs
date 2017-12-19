using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.RasterTools
{
    public struct ScanLineSegment
    {
        public int Row;
        public int BeginCol;
        public int EndCol;

        public ScanLineSegment(int row, int bCol, int eCol)
        {
            Row = row;
            BeginCol = bCol;
            EndCol = eCol;
        }
    }
}
