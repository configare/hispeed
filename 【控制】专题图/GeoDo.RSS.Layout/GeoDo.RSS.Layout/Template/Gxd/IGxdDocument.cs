using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Layout
{
    public interface IGxdDocument:IGxdItem
    {
        IGxdTemplateHost GxdTemplateHost { get; }
        List<IGxdDataFrame> DataFrames { get; }
        void SaveAs(string fname);
        string FullPath { get; set; }
    }
}
