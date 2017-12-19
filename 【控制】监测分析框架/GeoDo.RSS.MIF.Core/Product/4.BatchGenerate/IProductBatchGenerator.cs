using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public interface IProductBatchGenerator
    {
        void Batch(IMonitoringProduct product,Action<IMonitoringSubProduct,string> statusPrinter,Action<int,string> processTracker);
    }
}
