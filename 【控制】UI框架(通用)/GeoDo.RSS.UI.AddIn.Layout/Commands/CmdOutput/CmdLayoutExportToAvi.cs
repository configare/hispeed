using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel.Composition;
using System.IO;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.VideoMark;
using GeoDo.RSS.Layout.GDIPlus;
using GeoDo.RSS.Layout;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdLayoutExportToAvi : Command
    {
        private IAVIHelper _avi = null;
        private int _interval = 0;
        private string _fname = null;

        public CmdLayoutExportToAvi()
            : base()
        {
            _id = 6038;
            _name = "ExportToAvi";
            _text = _toolTip = "导出动画";
        }

        public override void Execute()
        {
            ILayoutViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            IAVILayer aviLyr = GetAviLayer(viewer);
            if (aviLyr == null)
                return;
            _interval = aviLyr.Interval;
            ILayoutHost host = viewer.LayoutHost;
            if (host == null)
                return;
            ExportBitmaps(host);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argument">导出的动画的文件名</param>
        public override void Execute(string argument)
        {
            _fname = argument;
            Execute();
        }

        private void ExportBitmaps(ILayoutHost host)
        {
            if (host == null)
                return;
            _avi = new AVIHelper(host);
            _avi.OnTimerStopped += new EventHandler(GetBitmaps);
        }

        private void GetBitmaps(object sender, EventArgs e)
        {
            Bitmap[] bitmaps = _avi.Bitmaps;
            if (bitmaps == null || bitmaps.Length == 0)
                return;
            Size size = bitmaps[0].Size;
            if (_fname == null)
                _fname = SelectFilename();
            string outputType = GetTypeFromFilename(_fname);
            if (string.IsNullOrEmpty(outputType))
                return;
            ViedoMarkFactory fac = new ViedoMarkFactory();
            bool isOk = fac.Mark(_fname, outputType, size, _interval, bitmaps, null);
            DisposeAvi();
        }

        private string GetTypeFromFilename(string fname)
        {
            string extension = Path.GetExtension(fname);
            switch (extension.ToLower())
            {
                case ".avi":
                    return "avi";
                case ".gif":
                    return "gif";
                default:
                    return null;
            }
        }

        private string SelectFilename()
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "GIF(*.gif)|*.gif|AVI(*.avi)|*.avi";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    return dlg.FileName;
            }
            return null;
        }

        private IAVILayer GetAviLayer(ILayoutViewer viewer)
        {
            ILayoutHost host = viewer.LayoutHost;
            if (host == null)
                return null;
            IDataFrame df = host.ActiveDataFrame;
            if (df == null)
                return null;
            if (df.Provider == null)
                return null;
            ICanvas canvas = (df.Provider as IDataFrameDataProvider).Canvas;
            if (canvas == null)
                return null;
            if (canvas.LayerContainer == null)
                return null;
            List<ILayer> lyrs = canvas.LayerContainer.Layers;
            if (lyrs == null || lyrs.Count == 0)
                return null;
            foreach (ILayer lyr in lyrs)
                if (lyr is IAVILayer)
                    return lyr as IAVILayer;
            return null;
        }

        private void DisposeAvi()
        {
            _avi.OnTimerStopped -= new EventHandler(GetBitmaps);
            _avi.Dispose();
        }
    }
}
