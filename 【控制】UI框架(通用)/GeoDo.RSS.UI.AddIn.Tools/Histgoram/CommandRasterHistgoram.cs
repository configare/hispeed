using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.RasterTools;
using GeoDo.RSS.Core.RasterDrawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandRasterHistgoram : Command
    {
        public CommandRasterHistgoram()
            : base()
        {
            _name = "CommandRasterHistgoram";
            _text = _toolTip = "直方图统计";
            _id = 7101;
        }

        public override void Execute(string argument)
        {
            base.Execute(argument);
        }

        public override void Execute()
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            if (drawing == null)
                return;
            int[] aoi = GetAOI();
            using (frmDataProviderSelector frm = new frmDataProviderSelector(aoi != null))
            {
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.SetDataProvider(drawing.DataProviderCopy);
                frm.SetSelectedBands(drawing.SelectedBandNos);
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    IRasterDataProvider dataProvider = null;
                    bool isNew = false;
                    try
                    {
                        dataProvider = frm.DataProvider;
                        int[] bandNos = frm.BandNos;
                        isNew = frm.IsNewDataProvider;
                        bool isApplyAoi = frm.ApplyAoi;
                        if (isApplyAoi && aoi != null)
                            DoStat(dataProvider, bandNos, aoi);
                        else
                            DoStat(dataProvider, bandNos, null);
                    }
                    finally
                    {
                        if (isNew && dataProvider != null)
                            dataProvider.Dispose();
                    }
                }
            }
        }

        private void DoStat(IRasterDataProvider dataProvider, int[] bandNos,int[] aoi)
        {
            Dictionary<int, RasterQuickStatResult> results;
            IProgressMonitor progress = _smartSession.ProgressMonitorManager.DefaultProgressMonitor;
            try
            {
                progress.Reset("正在执行直方图统计...", 100);
                progress.Start(false);
                IRasterQuickStatTool stat = new RasterQuickStatTool();
                results = stat.Compute(dataProvider, aoi, bandNos,
                    (idx, tip) =>
                    {
                        progress.Boost(idx, "正在执行直方图统计...");
                    });
            }
            finally 
            {
                progress.Finish();
            }
            /*using (*/
            frmRasterQuickStat frm = new frmRasterQuickStat();//)
            {
                frm.Owner = _smartSession.SmartWindowManager.MainForm as Form;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Apply(dataProvider.fileName, results);
                frm.Show();
            }
        }

        private int[] GetAOI()
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return null;
            return viewer.AOIProvider.GetIndexes();
        }
    }
}
