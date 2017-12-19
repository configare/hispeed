using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public delegate void MonitoringProductLoadedHandler(object sender,IMonitoringProduct product);
    public delegate void MonitoringSubProductLoadedHandler(object sender,IMonitoringProduct product,IMonitoringSubProduct subProduct);

    public interface IMonitoringSessionEvents
    {
        MonitoringProductLoadedHandler OnMonitoringProductLoaded { get; set; }
        MonitoringSubProductLoadedHandler OnMonitoringSubProductLoaded { get; set; }
    }
}
