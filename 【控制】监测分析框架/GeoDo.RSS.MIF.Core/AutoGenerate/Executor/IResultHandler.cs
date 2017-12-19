using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public interface IResultHandler
    {
        void HandleResult(IContextEnvironment contextEnvironment, IMonitoringProduct product, IMonitoringSubProduct subProduct, IExtractResult result);
    }
}
