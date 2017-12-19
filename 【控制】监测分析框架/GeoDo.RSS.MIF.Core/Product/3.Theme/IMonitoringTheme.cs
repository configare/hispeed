using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public interface IMonitoringTheme:IDisposable
    {
        string Name { get; }
        string Identify { get; }
        List<IMonitoringProduct> Products { get; }
        IMonitoringProduct GetProductByIdentify(string identify);
    }
}
