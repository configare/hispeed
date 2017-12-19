using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.RasterTools
{
    public interface IRasterCut
    {
        void Cut(IRasterDataProvider dataProvider,RasterCut.CutArgument args,Action<int,string> progressTracker);
    }
}
