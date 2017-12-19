using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public interface IMonitoringProduct:IDisposable
    {
        string Name { get; }
        string Identify { get; }
        ProductDef Definition { get; }
        List<IMonitoringSubProduct> SubProducts { get; }
        IMonitoringSubProduct GetSubProductByIdentify(string identify);
        WorkspaceDef GetWorkspaceDef();
    }
}
