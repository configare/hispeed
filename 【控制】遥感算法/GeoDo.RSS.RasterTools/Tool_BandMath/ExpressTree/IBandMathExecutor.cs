using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.RasterTools
{
    public interface IBandMathExecutor
    {
        void Compute(IRasterDataProvider dataProvider, string expression, IRasterBand dstRasterBand, Action<int, string> progressTracker);
    }
}
