using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.UI.AddIn.CanvasViewer;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.DrawEngine.GDIPlus;

namespace GeoDo.RSS.UI.WinForm
{
    [Export(typeof(IOpenFileProcessor)), ExportMetadata("VERSION", "1")]
    internal class OrbitFileProcessor : OpenFileProcessor, IRasterFileProcessor
    {
        public OrbitFileProcessor()
            : base()
        {
            _extNames.Add(".HDF");
            _extNames.Add(".1BD");
            _extNames.Add(".L1B");
            _extNames.Add(".1B");
            _extNames.Add(".1A5");
        }

        protected override bool FileHeaderIsOK(string fname, string extName)
        {
            return true;
        }

        public override bool Open(string fname, out bool memoryIsNotEnough)
        {
            memoryIsNotEnough = false;
            if (!MemoryIsEnoughChecker.MemoryIsEnouggWithMsgBoxForRaster(fname))
            {
                memoryIsNotEnough = true;
                return false;
            }
            ISmartViewer viewer = _session.SmartWindowManager.ActiveViewer;
            if (viewer == null)
                CreateCanvasViewer(fname);
            else if (viewer is ICanvasViewer)
                AddLayer2CanvasViewer(viewer as ICanvasViewer, fname);
            return true;
        }

        private void AddLayer2CanvasViewer(ICanvasViewer viewer, string fname)
        {
            CreateCanvasViewer(fname);
        }

        private void CreateCanvasViewer(string fname)
        {
            CanvasViewer cv = new CanvasViewer(OpenFileFactory.GetTextByFileName(fname), _session);
            cv.Tag = fname;
            _session.SmartWindowManager.DisplayWindow(cv);
            RasterLayerBuilder.CreateAndLoadRasterLayer(_session, cv.Canvas, fname);
            //自动翻转升轨数据
            RasterDrawing drawing = cv.ActiveObject as RasterDrawing;
            if (drawing != null && drawing.DataProvider != null)
            {
                DataIdentify idt = drawing.DataProvider.DataIdentify;
                if (idt != null && idt.IsOrbit && idt.IsAscOrbitDirection)
                {
                    cv.Canvas.IsReverseDirection = true;
                    cv.Canvas.Refresh(Core.DrawEngine.enumRefreshType.RasterLayer);
                }
            }
            //轨道数据自动全屏显示
            (cv.Canvas as Canvas).SetToFitWidth();
        }
    }
}
