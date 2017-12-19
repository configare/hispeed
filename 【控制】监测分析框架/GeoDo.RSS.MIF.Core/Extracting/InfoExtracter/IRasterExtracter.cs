using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public interface IRasterExtracter<TDataType,TFeature>:IFeaturePixelExtracter<TDataType,TFeature>
    {
        void Reset(IArgumentProvider argProvider, int[] visitBandNos, string express);
    }
}
