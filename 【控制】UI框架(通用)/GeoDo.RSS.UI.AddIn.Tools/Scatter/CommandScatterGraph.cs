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
    public class CommandScatterGraph : Command
    {
        public CommandScatterGraph()
            : base()
        {
            _name = "CommandScatterGraph";
            _text = _toolTip = "波段散点图";
            _id = 7102;
        }

        public override void Execute(string argument)
        {
            base.Execute();
        }

        public override void Execute()
        {
            //Test();
            //return;
            IRasterDataProvider dataProvider = null;
            bool isNew = false;
            int[] bandNos = null;
            int[] aoi = GetAOI();
            using (frmScatterVarSelector frm = new frmScatterVarSelector())
            {
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Apply(GetCurrentDataProvider(), aoi);
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    dataProvider = frm.DataProvider;
                    bandNos = new int[] { frm.XBandNo, frm.YBandNo };
                    isNew = frm.IsNewDataProvider;
                    aoi = frm.AOI;
                }
            }
            if (bandNos == null || dataProvider == null)
                return;
            //
            IProgressMonitor progress = _smartSession.ProgressMonitorManager.DefaultProgressMonitor;
            try
            {
                progress.Reset("正在准备生成散点图...", 100);
                progress.Start(false);
                frmScatterGraph frm1 = new frmScatterGraph();
                frm1.Owner = _smartSession.SmartWindowManager.MainForm as Form;
                frm1.StartPosition = FormStartPosition.CenterScreen;
                LinearFitObject fitObj = new LinearFitObject();
                frm1.Reset(dataProvider, bandNos[0], bandNos[1],aoi,
                           fitObj,
                            (idx, tip) => { progress.Boost(idx, "正在准备生成散点图..."); }
                           );
                progress.Finish();
                frm1.Show();
                frm1.Rerender();
                frm1.FormClosed+=new FormClosedEventHandler((obj,e)=>
                {
                    if (isNew)
                        dataProvider.Dispose();
                });
            }
            finally
            {
                progress.Finish();
            }
        }

        ExportPixelsByFeatures.PixelFeatures result;
        UInt16[][] bandValues;
        private void Test()
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    IExportPixelsByFeatures exp = new ExportPixelsByFeatures();
                    result = exp.Export(GetEnvelope(drawing.DataProviderCopy),
                        new System.Drawing.Size(drawing.DataProviderCopy.Width, drawing.DataProviderCopy.Height),
                        dlg.FileName, "length", null);
                    //
                    IAOIRasterFetcher<UInt16> ft = new AOIRasterFetcher<UInt16>();                    
                    ft.Fetch(drawing.DataProviderCopy, result.RasterIndexes, new int[] { 1, 2, 3 }, out bandValues);
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
