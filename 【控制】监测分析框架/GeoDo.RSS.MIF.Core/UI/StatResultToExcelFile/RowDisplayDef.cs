using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.MIF.Core
{
    public class RowDisplayDef
    {
        public List<int> DisplayRow = new List<int>();
        public string PageName;
        public bool DataLabel = false;
        public bool calcAverage = false;
        public bool calcMaxValue = false;

        public RowDisplayDef()
        { }

        public RowDisplayDef(List<int> displayRow, string pageName)
        {
            DisplayRow = displayRow;
            PageName = pageName;
        }
    }
}
