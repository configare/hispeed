using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public interface IExtractPanelBuilder
    {
        void Build(IWorkspace wks, IMonitoringSubProduct subProduct);
        void SetArg(string argName, object argValue);
        void ResetDefaultValue();
    }
}
