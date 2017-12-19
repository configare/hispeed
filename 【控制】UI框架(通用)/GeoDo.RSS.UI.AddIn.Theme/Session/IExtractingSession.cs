using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public interface IExtractingSession
    {
        IMonitoringProduct CurrentProduct { get; }
        IMonitoringSubProduct CurrentSubProduct { get; }
        bool IsActive { get; }
        bool IsNeedSave();
        void Saved();
        bool Start(ICanvasViewer viewer,IMonitoringProduct product, IMonitoringSubProduct subProduct);
        void Stop();
        void ChangeSubProduct(IMonitoringSubProduct subProduct);
        void ApplyResult(IPixelIndexMapper result);
        void ApplyErase(int[] aoi);
        void ApplyAdsorb(int[] aoi);
        string AddToWorkspace(IWorkspace wks);
        IPixelIndexMapper GetBinaryValuesMapper(string productIdentify, string subProductIdentify);
    }
}
