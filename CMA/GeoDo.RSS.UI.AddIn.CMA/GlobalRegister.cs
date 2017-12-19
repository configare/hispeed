using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.UI.AddIn.Theme;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.UI.AddIn.CMA
{
    public static class GlobalRegister
    {
        internal static ISmartSession Session;
        private static IMonitoringProduct _product;

        public static void RegisteAll()
        {
            Session = SmartApp.SmartSession;
            AttachEvents();
        }

        private static void AttachEvents()
        {
            ISmartSessionEvents evts = Session as ISmartSessionEvents;
            evts.OnSmartSessionLoaded += new SmartSessionLoadedHandler(SmartSessionIsLoaded);
            evts.OnFileOpended += new FileOpenedHandler(FileOpened);
            ISmartWindowManager mgr = Session.SmartWindowManager;
            mgr.OnActiveWindowChanged += new OnActiveWindowChangedHandler(ActiveViewerChanged);
            IMonitoringSessionEvents msevts = Session.MonitoringSession as IMonitoringSessionEvents;
            msevts.OnMonitoringProductLoaded += new MonitoringProductLoadedHandler(ProductLoaded);
            msevts.OnMonitoringSubProductLoaded += new MonitoringSubProductLoadedHandler(SubProductLoaded);
        }

        static void SmartSessionIsLoaded(object sender)
        {
        }

        static void FileOpened(object sender, string fname)
        {
            ICanvasViewer viewer = Session.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            if (drawing == null)
                return;
            if (drawing.FileName == fname)
            {
                ApplyImageEnhancor();
            }
        }

        static void ProductLoaded(object sender, IMonitoringProduct prd)
        {
            if (prd == null)
                return;
            //if (_product == null || _product != prd)//产品切换
            _product = prd;
            ApplyImageEnhancor();
        }

        static void ActiveViewerChanged(object sender, ISmartWindow oldWindow, ISmartWindow newWindow)
        {
            if (newWindow == null || !(newWindow is ICanvasViewer))
                return;
            if (newWindow == oldWindow)
                return;
            ICanvasViewer viewer = newWindow as ICanvasViewer;
            if (viewer.ActiveObject == null)    //{GeoDo.RSS.Core.RasterDrawing.RasterDrawing}
                return;
            IRasterDrawing draw = viewer.ActiveObject as IRasterDrawing;
            if (draw == null)
                return;
            if (draw.RgbProcessorStack != null && draw.RgbProcessorStack.Count > 0)     //已经应用过的不再加载方案
                return;
            ApplyImageEnhancor();
        }

        static void ApplyImageEnhancor()
        {
            string enhanceFile = ImageEnhanceFactory.GetEnhanceNameCurDrawing(Session);
            if (!string.IsNullOrWhiteSpace(enhanceFile))
            {
                ImageEnhancor imageEn = new ImageEnhancor(Session);
                imageEn.ApplyImageEnhance(enhanceFile);
            }
        }

        static void SubProductLoaded(object sender, IMonitoringProduct prd, IMonitoringSubProduct subPrd)
        {
        }
    }
}
