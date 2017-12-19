using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class ProductBatchGenerator:IProductBatchGenerator
    {
        protected IArgumentProviderSetter _setter;
        protected IBatchResultCollecter _collecter;

        public ProductBatchGenerator(IArgumentProviderSetter setter,IBatchResultCollecter collecter)
        {
            _setter = setter;
            _collecter = collecter;
        }

        public void Batch(IMonitoringProduct product, Action<IMonitoringSubProduct, string> statusPrinter, Action<int, string> processTracker)
        {
            if (product == null || product.SubProducts.Count == 0)
                return;
            int count = product.SubProducts.Count;
            int steps = 100 / count;
            for (int i = 0; i < count; i++)
            {
                IMonitoringSubProduct sub = product.SubProducts[i];
                if (processTracker != null)
                    processTracker(steps, "正在生成\"" + sub.Name + "\"...");
                try
                {
                    _setter.Fill(product, sub);
                    if (!sub.CanDo)
                        continue;
                    IExtractResult result = sub.Make(null);
                    _collecter.Collect(sub,result);
                }
                catch (Exception ex)
                {
                    if (processTracker != null)
                        processTracker(steps, "生成\"" + sub.Name + "\"时发生错误:"+ex.Message);
                }
            }
        }
    }
}
