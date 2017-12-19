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
using CanvasView =GeoDo.RSS.UI.AddIn.CanvasViewer.CanvasViewer;
using GeoDo.RSS.DF.GDAL.HDF5GEO;

namespace GeoDo.RSS.DF.AddIn.HDF5GEO
{
    [Export(typeof(IOpenFileProcessor)), ExportMetadata("VERSION", "1")]
    public class FY3BIceConFileProcessor : OpenFileProcessor, IRasterFileProcessor
    {

        public FY3BIceConFileProcessor()
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
            return IceConDriver.IsCompatible(fname, null);
        }

        private void CreateCanvasViewer(string fname)
        {
            using (frmIceConDataSelect frm = new frmIceConDataSelect())
            {
                string[] slec = new string[] { "icecon_north", "icecon_south" };
                frm.Apply(slec);
                frm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string [] cid = new string []{ frm.ComponentID} ;
                    if (cid == null || cid[0] == null)
                        return;
                    CanvasView cv = new CanvasView(OpenFileFactory.GetTextByFileName(fname), _session);
                    _session.SmartWindowManager.DisplayWindow(cv);
                    object[] args = new object[] {cid };
                    RasterLayerBuilder.CreateAndLoadRasterLayer(_session, cv.Canvas, fname, args);
                }
            }
        }



    }
}
