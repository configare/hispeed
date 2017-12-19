using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.RasterTools
{
    public interface IBandMathVisitor
    {
        void Visit<TSrc, TDst>(IRasterBand[] srcRasters, IRasterBand dstRaster, Func<TSrc[], TDst> mathOperator,Action<int, string> progressCallback);
    }
}
