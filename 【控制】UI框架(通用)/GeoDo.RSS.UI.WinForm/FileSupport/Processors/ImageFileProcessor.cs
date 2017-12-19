using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.UI.AddIn.CanvasViewer;

namespace GeoDo.RSS.UI.WinForm
{
    [Export(typeof(IOpenFileProcessor)), ExportMetadata("VERSION", "1")]
    internal class ImageFileProcessor : OpenFileProcessor, IRasterFileProcessor
    {
        public ImageFileProcessor()
            : base()
        {
            _extNames.Add(".TIF");
            _extNames.Add(".TIFF");
            _extNames.Add(".IMG");
            _extNames.Add(".JPG");
            _extNames.Add(".JPEG");
            _extNames.Add(".BMP");
            _extNames.Add(".PNG");
            _extNames.Add(".GIF");
            _extNames.Add(".TIL");
            _extNames.Add(".GPC");
            _extNames.Add(".AWX");
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
            _session.SmartWindowManager.DisplayWindow(cv);
            //_session.SmartWindowManager.DisplayWindow(cv, new WindowPosition(DockStyle.Right, false));
            RasterLayerBuilder.CreateAndLoadRasterLayer(_session, cv.Canvas, fname);
        }
    }
}
