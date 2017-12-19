using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.UI.AddIn.CanvasViewer;
using System.IO;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Layout.GDIPlus;

namespace GeoDo.RSS.UI.WinForm
{
    [Export(typeof(IOpenFileProcessor)), ExportMetadata("VERSION", "1")]
    internal class RasterFileProcessor : OpenFileProcessor,IRasterFileProcessor
    {
        public RasterFileProcessor()
            : base()
        {
            _extNames.Add(".LDF");
            _extNames.Add(".LD2");
            _extNames.Add(".LD3");
            _extNames.Add(".LDFF");
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
            //if (viewer == null)
                CreateCanvasViewer(fname);
            //else if (viewer is ICanvasViewer)
            //    AddLayer2CanvasViewer(viewer as ICanvasViewer, fname);
            //else if (viewer is ILayoutViewer)
            //    AddDataToLayoutViewer(viewer as ILayoutViewer, fname);
            return true;
        }

        private void AddLayer2CanvasViewer(ICanvasViewer viewer, string fname)
        {
            CreateCanvasViewer(fname);
        }

        private void CreateCanvasViewer(string fname)
        {
            CanvasViewer cv = null;
            try
            {
                cv = new CanvasViewer(OpenFileFactory.GetTextByFileName(fname), _session);
                _session.SmartWindowManager.AddDocument(cv);
                RasterLayerBuilder.CreateAndLoadRasterLayer(_session, cv.Canvas, fname, GetRgbStretcherProvider(fname));
                _session.SmartWindowManager.ActivateWindow(cv);
            }
            catch
            {
                if (cv != null)
                    cv.Close();
                throw;
            }
        }

        private RgbStretcherProvider GetRgbStretcherProvider(string fname)
        {
            IRasterDataProvider prd = null;
            try
            {
                prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
                if (prd != null && prd.BandCount == 1)
                    return new RgbStretcherProvider();
            }
            finally
            {
                if (prd != null)
                    prd.Dispose();
            }
            return null;
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
            RasterLayerBuilder.CreateAndLoadRasterLayer(_session, canvas, fname, options);
            viewer.LayoutHost.Render();
        }
    }
}
