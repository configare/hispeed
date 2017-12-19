using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public interface IExtractResultDisplayer
    {
        void Display(IMonitoringSubProduct subProduct, IExtractResult result);
    }
}
