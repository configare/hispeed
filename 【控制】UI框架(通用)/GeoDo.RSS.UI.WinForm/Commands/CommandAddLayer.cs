using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.UI.AddIn.CanvasViewer;
using System.IO;
using System.Drawing;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.UI.WinForm
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandAddLayer : Command
    {
        public CommandAddLayer()
            : base()
        {
            _id = 2004;
            _text = "添加图层";
        }

        public override void Execute()
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = SupportedFileFilters.LdfFilter + "|"
                    + SupportedFileFilters.NoaaFilter + "|" + SupportedFileFilters.SrfFilterString + "|"
                    + SupportedFileFilters.ImageFilterString + "|" + SupportedFileFilters.VectorFilterString;
                dlg.Multiselect = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (string fname in dlg.FileNames)
                        Execute(fname);
                }
            }
        }

        public override void Execute(string argument)
        {
            AddData2CanvasViewer(argument);
        }

        private void AddData2CanvasViewer(string fname)
        {
            ICanvasViewer cv = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (cv == null)
            {
                OpenFileFactory.Open(fname);
                return;
            }
            AddData2CanvasViewer(cv, fname);
        }

        private void AddData2CanvasViewer(ICanvasViewer cv, string fname)
        {
            ISmartSession session = _smartSession;
            ICanvas canvas = cv.Canvas;
            RgbStretcherProvider stretcher = null;
            bool isRaster = false;
            TryGetRasterStretcher(fname, out stretcher, out isRaster);
            if (isRaster)
            {
                RasterLayerBuilder.CreateAndLoadRasterLayer(session, canvas, fname, stretcher);
            }
            else
            {
                TryAddAsVector(fname);
            }
        }

        private void TryAddAsVector(string fname)
        {
            OpenFileFactory.Open(fname);
        }

        private void TryGetRasterStretcher(string fname, out RgbStretcherProvider stretcher,out bool isRaster)
        {
            isRaster = false;
            stretcher = null;
            try
            {
                using (IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider)
                {
                    if (prd != null)
                        isRaster = true;
                    if(prd.BandCount == 1)
                        stretcher = new RgbStretcherProvider();
                }
            }
            catch
            {
                isRaster = false;
                stretcher = null;
            }
        }
    }
}
