using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public interface IBatchResultCollecter
    {
        void Collect(IMonitoringSubProduct subProduct, IExtractResult result);
    }
}
