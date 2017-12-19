using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;

namespace testFir
{
    public interface IMonitoringEnvironent
    {
        IMonitoringProduct ActiveProduct { get; }
        IExtractPanel ActiveExtractPane { get; }
    }
}
