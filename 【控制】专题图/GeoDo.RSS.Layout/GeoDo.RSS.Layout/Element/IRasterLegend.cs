using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Layout
{
    public interface IRasterLegend
    {
        string ColorTableName { get; set; }
        void Update(LegendItem[] items);
    }
}
