using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.RasterTools;
using System.Windows.Forms;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.MathAlg;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandAOI2ShpFile : Command
    {
        public CommandAOI2ShpFile()
            : base()
        {
            _name = "CommandAOI2ShpFile";
            _text = _toolTip = "AOI导出为Shp文件";
            _id = 7103;
        }

        public override void Execute(string argument)
        {
            base.Execute();
        }

        public override void Execute()
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "ESRI Shape Files(*.shp)|*.shp";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    AOI2ShapeFile  aoi2ShpFile = new AOI2ShapeFile();
                    aoi2ShpFile.Export(GetEnvelope(drawing.DataProviderCopy),
                        new System.Drawing.Size(drawing.DataProviderCopy.Width, drawing.DataProviderCopy.Height),
                        GetAOI(), dlg.FileName);
                }
            }
        }

        private CodeCell.AgileMap.Core.Envelope GetEnvelope(IRasterDataProvider dataProvider)
        {
            return new CodeCell.AgileMap.Core.Envelope(dataProvider.CoordEnvelope.MinX,
                dataProvider.CoordEnvelope.MinY,
                dataProvider.CoordEnvelope.MaxX,
                dataProvider.CoordEnvelope.MaxY);
        }

        private int[] GetAOI()
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return null;
            return viewer.AOIProvider.GetIndexes();
        }

        private IRasterDataProvider GetCurrentDataProvider()
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return null;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            if (drawing != null)
                return drawing.DataProviderCopy;
            return null;
        }
    }
}
