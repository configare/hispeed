using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Layout;
using GeoDo.RSS.UI.AddIn.CanvasViewer;
using GeoDo.RSS.Layout.GDIPlus;
using System.IO;
using GeoDo.RSS.DF.GDAL.HDF5Universal;

namespace GeoDo.RSS.UI.WinForm
{
    /// <summary>
    /// 定量产品显示
    /// 定量产品填色颜色表由RgbStretcherProvider类提供
    /// </summary>
    [Export(typeof(IOpenFileProcessor)), ExportMetadata("VERSION", "1")]
    internal class RasterProductFileProcessor : OpenFileProcessor
    {
        public RasterProductFileProcessor()
            : base()
        {
            _extNames.Add(".DAT");
            _extNames.Add(".MVG");
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
            else if (viewer is ILayoutViewer)
                AddDataToLayoutViewer(viewer as ILayoutViewer, fname);
            return true;
        }

        private void AddLayer2CanvasViewer(ICanvasViewer viewer, string fname)
        {
            CreateCanvasViewer(fname);
        }

        private void CreateCanvasViewer(string fname)
        {
            CanvasViewer cv = new CanvasViewer(OpenFileFactory.GetTextByFileName(fname), _session);
            _session.SmartWindowManager.DisplayWindow(cv);//如果不加载，则cv.Canvas为空，但此时上没有加载新的Layer
            RasterLayerBuilder.CreateAndLoadRasterLayer(_session, cv.Canvas, fname, new RgbStretcherProvider());
            _session.SmartWindowManager.DisplayWindow(cv);//
        }

        public void AddDataToLayoutViewer(ILayoutViewer viewer, string fname, params object[] options)
        {
            if (viewer == null)
                return;
            IDataFrame df = viewer.LayoutHost.ActiveDataFrame;
            if (df == null)
                return;
            IDataFrameDataProvider provider = df.Provider as IDataFrameDataProvider;
            if (provider == null)
                return;
            ICanvas canvas = provider.Canvas;
            if (canvas == null)
                return;
            RasterLayerBuilder.CreateAndLoadRasterLayer(_session, canvas, fname, new RgbStretcherProvider());
            viewer.LayoutHost.Render();
        }

    }
}
