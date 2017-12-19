using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.RasterTools
{
    public interface IBandMathTool:IRasterTool
    {
        void Compute(IRasterDataProvider dataProvider,string expression, string outDriver, string outFile,Action<int,string> progressTracker);
        void Compute(IRasterDataProvider dataProvider, string expression, IRasterBand dstRasterBand,Action<int,string> progressTracker);
    }
}
