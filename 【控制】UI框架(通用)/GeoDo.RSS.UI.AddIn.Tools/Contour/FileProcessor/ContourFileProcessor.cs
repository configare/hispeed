using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using System.IO;
using GeoDo.RSS.RasterTools;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.VectorDrawing;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    [Export(typeof(IOpenFileProcessor)), ExportMetadata("VERSION", "1")]
    internal class ContourFileProcessor : OpenFileProcessor,IRasterFileProcessor
    {
        public ContourFileProcessor()
            : base()
        {
            _extNames.Add(".XML");
        }

        protected override bool FileHeaderIsOK(string fname, string extName)
        {
            fname += ".contour";
            if (!File.Exists(fname))
                throw new FileNotFoundException("附属文件\""+Path.GetFileName(fname)+"\"丢失!");
            using (FileStream fs = new FileStream(fname, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs, Encoding.Unicode))
                {
                    char[] fileIdentify = br.ReadChars(ContourPersist.FILE_IDENTIFY.Length);
                    return ContourPersist.FileIdentifyIsOK(fileIdentify);
                }
            }
        }

        public override bool Open(string fname, out bool memoryIsNotEnough)
        {
            memoryIsNotEnough = true;
            if (_session.SmartWindowManager.ActiveCanvasViewer == null)
                return false;
            ICanvas canvas = _session.SmartWindowManager.ActiveCanvasViewer.Canvas;
            OpenContourFile(fname, canvas);
            return true;
        }

        private void OpenContourFile(string fname, ICanvas canvas)
        {
            IContourLayer lyr = ContourLayer.FromXml(fname,true);
            if (lyr != null)
            {
                canvas.LayerContainer.Layers.Add(lyr);
                canvas.Refresh(Core.DrawEngine.enumRefreshType.All);
            }
        }
    }
}
