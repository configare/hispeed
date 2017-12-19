using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Core
{
    public interface ICandidatePixelFilterPipe
    {
        IList<ICandidatePixelFilter> Filters { get; }
        void Reset();
        int[] Filter(IRasterDataProvider dataProvider, int[] aoi,Action<string> contextMessage);
    }
}
