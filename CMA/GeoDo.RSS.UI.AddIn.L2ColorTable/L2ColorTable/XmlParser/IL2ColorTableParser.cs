using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.UI.AddIn.L2ColorTable
{
    public interface IL2ColorTableParser:IDisposable
    {
        string[] GetBandValueRanges();
        string[] GetColorTables();
        string GetBandValueRangeInnerText(string name);
        string GetColorTableInnerText(string name);
        BandValueColorPair[] GetBandValueColorPair(string datasetName);
    }
}
