using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.UI;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.RasterDrawing;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public class AutoGenerateResultHandler:IResultHandler
    {
        private ISmartSession _session;

        public AutoGenerateResultHandler(ISmartSession session)
        {
            _session = session;
        }

        public void HandleResult(IContextEnvironment contextEnvironment, IMonitoringProduct product, IMonitoringSubProduct subProduct, IExtractResult result)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct(subProduct.Identify);
            if (subProduct.Identify == "DBLV")//只有判识结果在叠加在影像上
            {
                (_session.MonitoringSession as IMonitoringSession).ExtractingSession.Start((_session.SmartWindowManager.ActiveCanvasViewer), product, subProduct);
                DisplayResultClass.DisplayResult(_session, subProduct, result, true);
            }
            else 
            {
                DisplayResultClass.DisplayResult(_session, subProduct, result, true);
            }
            string fname;
            if (result is IPixelIndexMapper)
            {
                fname = (_session.MonitoringSession as IMonitoringSession).ExtractingSession.AddToWorkspace((_session.MonitoringSession as IMonitoringSession).Workspace);
                DisplayResultClass._contextEnvironment.PutContextVar(subProduct.Identify, fname);
            }
            else if (result is IExtractResultArray)
            {
                IExtractResultArray extResultMapper = result as ExtractResultArray;
                IExtractResultBase[] mappers = extResultMapper.PixelMappers;
                if (mappers == null || mappers.Length == 0)
                    return;
                foreach (IExtractResultBase mapper in mappers)
                {
                    if (mapper is IPixelIndexMapper)
                    {
                        fname = (_session.MonitoringSession as IMonitoringSession).ExtractingSession.AddToWorkspace((_session.MonitoringSession as IMonitoringSession).Workspace);
                        DisplayResultClass._contextEnvironment.PutContextVar(subProduct.Identify, fname);
                    }
                }
            }
            else if (result is IFileExtractResult && subProduct.Identify == "DBLV")
            {
                DisplayResultClass._contextEnvironment.PutContextVar(subProduct.Identify, (result as IFileExtractResult).FileName);
            }

        }
    }
}
