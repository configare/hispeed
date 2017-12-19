using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class ExcelRangeColor
    {
        public int bRow;
        public int bCol;
        public int eRow;
        public int eCol;
        public byte red;
        public byte green;
        public byte blue;

        public ExcelRangeColor(int bRow, int bCol, int eRow, int eCol, byte red, byte green, byte blue)
        {
            // TODO: Complete member initialization
            this.bRow = bRow;
            this.bCol = bCol;
            this.eRow = eRow;
            this.eCol = eCol;
            this.red = red;
            this.green = green;
            this.blue = blue;
        }
    }
}
