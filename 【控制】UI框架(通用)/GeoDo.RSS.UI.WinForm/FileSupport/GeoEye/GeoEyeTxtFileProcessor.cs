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
    internal class GeoEyeTxtFileProcessor : OpenFileProcessor, IRasterFileProcessor
    {
        public GeoEyeTxtFileProcessor()
            : base()
        {
            _extNames.Add(".TXT");
        }

        protected override bool FileHeaderIsOK(string fname, string extName)
        {
            string f = Path.GetFileName(fname).ToLower();
            if (!Regex.Match(f, @"^po_\d*_metadata\.txt").Success)
                return false;
            return true;
        }

        public override bool Open(string fname, out bool memoryIsNotEnough)
        {
            memoryIsNotEnough = true;
            CreateCanvasViewer(fname);
            return true;
        }

        private void CreateCanvasViewer(string fname)
        {
            using (frmComponentFileSelect frm = new frmComponentFileSelect())
            {
                frm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                frm.Apply(fname);
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string cid = "ComponentID=" + frm.ComponentID;
                    if (cid == null)
                        return;
                    CanvasViewer cv = new CanvasViewer(OpenFileFactory.GetTextByFileName(fname), _session);
                    _session.SmartWindowManager.DisplayWindow(cv);
                    RasterLayerBuilder.CreateAndLoadRasterLayerForGeoEye(_session, cv.Canvas, fname, cid);
                }
            }
        }
    }
}
