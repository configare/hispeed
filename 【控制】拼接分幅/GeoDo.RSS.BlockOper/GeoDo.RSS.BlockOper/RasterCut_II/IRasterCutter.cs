using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.BlockOper
{
    public interface IRasterCutter
    {
        void Cut(IRasterDataProvider srcRaster, int[] aoi, int[] outBandNos,
                 string outDriver,string outFileName, Action<int, string> processTracker);
        void Cut(IRasterDataProvider srcRaster, CoordEnvelope outRect, int[] outBandNos,
                 string outDriver,string outFileName, Action<int, string> processTracker);
    }
}
