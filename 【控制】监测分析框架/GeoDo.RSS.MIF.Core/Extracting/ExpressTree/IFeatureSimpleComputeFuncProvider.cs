using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public interface IFeatureSimpleComputeFuncProvider<TDataType,TFeature>
    {
        Func<TDataType,TFeature> GetComputeFunc();
    }
}
