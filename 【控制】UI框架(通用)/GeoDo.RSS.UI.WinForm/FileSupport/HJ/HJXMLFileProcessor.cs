using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.UI.AddIn.CanvasViewer;
using System.Text.RegularExpressions;
using System.IO;

namespace GeoDo.RSS.UI.WinForm
{
    [Export(typeof(IOpenFileProcessor)), ExportMetadata("VERSION", "1")]
    internal class HJXMLFileProcessor : OpenFileProcessor, IRasterFileProcessor
    {
        public HJXMLFileProcessor()
            : base()
        {
            _extNames.Add(".XML");
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
            CreateCanvasViewer(fname);
            return true;
        }

        private void CreateCanvasViewer(string fname)
        {
            CanvasViewer cv = new CanvasViewer(OpenFileFactory.GetTextByFileName(fname), _session);
            _session.SmartWindowManager.DisplayWindow(cv);
            RasterLayerBuilder.CreateAndLoadRasterLayerForHJ(_session, cv.Canvas, fname);
        }
    }
}
