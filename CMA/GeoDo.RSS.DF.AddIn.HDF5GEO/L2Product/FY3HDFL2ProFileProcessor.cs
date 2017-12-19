using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using System.IO;
using System.Text.RegularExpressions;
using GeoDo.HDF5;
using GeoDo.RSS.UI.AddIn.CanvasViewer;
using CanvasView = GeoDo.RSS.UI.AddIn.CanvasViewer.CanvasViewer;
using GeoDo.RSS.DF.GDAL.HDF5GEO;

namespace GeoDo.RSS.DF.AddIn.HDF5GEO
{
    [Export(typeof(IOpenFileProcessor)), ExportMetadata("VERSION", "1")]
    public class FY3HDFL2ProFileProcessor : OpenFileProcessor, IRasterFileProcessor
    {

        public FY3HDFL2ProFileProcessor()
            : base()
        {
            _extNames.Add(".HDF");
        }

        public override bool Open(string fname, out bool memoryIsNotEnough)
        {
            memoryIsNotEnough = true;
            CreateCanvasViewer(fname);
            return true;
        }

        public override bool IsSupport(string fname, string extName)
        {
            return FileHeaderIsOK(fname, extName);
        }

        protected override bool FileHeaderIsOK(string fname, string extName)
        {
            return FY3HDFL2ProductDriver.IsCompatible(fname, null);
        }

        private void CreateCanvasViewer(string fname)
        {
            L2ProductDefind[] l2Pros = L2ProductDefindParser.GetL2ProductDefs(fname);
            if (l2Pros == null || l2Pros.Length == 0)
                return;
            if (l2Pros.Length == 1)
            {
                CanvasView cv = new CanvasView(OpenFileFactory.GetTextByFileName(fname), _session);
                _session.SmartWindowManager.DisplayWindow(cv);
                RasterLayerBuilder.CreateAndLoadRasterLayer(_session, cv.Canvas, fname, null);
                return;
            }
            using (frmFY3L2ProDataSelect frm = new frmFY3L2ProDataSelect(l2Pros))
            {
                frm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string proDesc = frm.ProDesc;
                    if (string.IsNullOrEmpty(proDesc))
                        return;
                    CanvasView cv = new CanvasView(OpenFileFactory.GetTextByFileName(fname), _session);
                    _session.SmartWindowManager.DisplayWindow(cv);
                    object[] args = new object[] { new string[] { proDesc } };
                    RasterLayerBuilder.CreateAndLoadRasterLayer(_session, cv.Canvas, fname, args);
                }
            }
        }



    }
}
