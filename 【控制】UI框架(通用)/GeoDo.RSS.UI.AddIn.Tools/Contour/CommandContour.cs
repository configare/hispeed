using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.RasterTools;
using System.Windows.Forms;
using GeoDo.RSS.Core.RasterDrawing;
using CodeCell.AgileMap.Core;
using System.Drawing;
using GeoDo.RSS.Core.VectorDrawing;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandContour : Command
    {
        public CommandContour()
            : base()
        {
            _name = "CommandContour";
            _text = _toolTip = "等值线";
            _id = 7105;
        }

        public override void Execute()
        {
            Execute(null);
        }

        public override void Execute(string argument)
        {
            IRasterDataProvider dataProvider = null;
            bool isNew = false;
            int[] aoi = GetAOI();
            frmContourSettings.ContourItem[] contourItems = null;
            bool isNeedDisplay = false;
            bool isNeedFillColor = false;
            bool isNeedLabel = false;
            string shpFile = null;
            int bandNo = 1;
            int sample = 1;
            using (frmContourSettings frm = new frmContourSettings())
            {
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Apply(GetCurrentDataProvider(out bandNo), bandNo, aoi);
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    dataProvider = frm.DataProvider;
                    isNew = frm.IsNewDataProvider;
                    contourItems = frm.ContourValues;
                    isNeedDisplay = frm.IsNeedDisplay;
                    isNeedFillColor = frm.IsNeedFillColor;
                    isNeedLabel = frm.IsNeedLabel;
                    bandNo = frm.BandNo;
                    sample = frm.Sample;
                    shpFile = string.IsNullOrWhiteSpace(frm.ShpFileName) ? null : frm.ShpFileName;
                    aoi = frm.AOI;
                }
            }
            if (dataProvider == null || contourItems == null)
                return;
            DoGenerateContourLines(dataProvider, isNew, bandNo, aoi, contourItems, isNeedDisplay, isNeedLabel, isNeedFillColor, sample, shpFile);
        }

        private void DoGenerateContourLines(IRasterDataProvider dataProvider, bool isNew, int bandNo, int[] aoi, frmContourSettings.ContourItem[] contourItems, bool isNeedDisplay, bool isNeedLabel, bool isNeedFillColor, int sample, string shpFileName)
        {
            try
            {
                IProgressMonitor progress = _smartSession.ProgressMonitorManager.DefaultProgressMonitor;
                try
                {
                    progress.Reset("正在生成等值线...", 100);
                    progress.Start(false);
                    IContourGenerateTool tool = new ContourGenerateTool();
                    tool.Sample = sample;
                    double[] cvs = ToContourValues(contourItems);
                    ContourLine[] cntLines = tool.Generate(dataProvider.GetRasterBand(bandNo),
                        cvs, aoi,
                        (idx, tip) => { progress.Boost(idx, tip); });
                    if (cntLines == null || cntLines.Length == 0)
                    {
                        MsgBox.ShowInfo("不存在符合指定条件的等值线！");
                        return;
                    }
                    if (shpFileName != null)
                        TryExport2ShapeFile(dataProvider, cntLines, shpFileName, progress,isNeedDisplay);
                    if (isNeedDisplay)
                        TryDisplay(cntLines, contourItems, isNeedLabel, isNeedFillColor, isNew, dataProvider);
                }
                finally
                {
                    progress.Finish();
                }
            }
            finally
            {
                if (isNew && !isNeedDisplay)
                {
                    dataProvider.Dispose();
                }
            }
        }

        private unsafe void TryExport2ShapeFile(IRasterDataProvider dataProvider, ContourLine[] cntLines, string shpFileName, IProgressMonitor progress,bool isNeedDisplay)
        {
            int cntCount = cntLines.Length;
            string tip = "正在将等值线导出为矢量文件({0}/{1})...";
            progress.Reset("正在将等值线导出为矢量文件...", cntLines.Length);
            IEsriShapeFilesWriter writer = new EsriShapeFilesWriterII(shpFileName, enumShapeType.Polyline);
            writer.BeginWrite();
            try
            {
                double resX = dataProvider.CoordEnvelope.Width / dataProvider.Width;
                double resY = dataProvider.CoordEnvelope.Height / dataProvider.Height;
                double minX = dataProvider.CoordEnvelope.MinX;
                double maxY = dataProvider.CoordEnvelope.MaxY;
                Feature[] buffer = new Feature[1];
                for (int i = 0; i < cntCount; i++)
                {
                    if (cntLines[i] == null)
                        continue;
                    Feature fet = GetFeature(cntLines[i], resX, resY, minX, maxY, i);
                    if (fet != null)
                    {
                        buffer[0] = fet;
                        writer.Write(buffer);
                    }
                    progress.Boost(i, string.Format(tip, i + 1, cntCount));
                }
            }
            finally
            {
                writer.EndWriter();
                progress.Boost(cntCount);
                //没有直接显示则打开矢量文件
                if (!isNeedDisplay)
                {
                    if (MsgBox.ShowQuestionYesNo("是否打开矢量文件\"" + shpFileName + "\"?") == DialogResult.Yes)
                    {
                        OpenFileFactory.Open(shpFileName);
                    }
                }
            }
        }

        private unsafe Feature GetFeature(ContourLine cntLine, double resX, double resY,
            double minX, double maxY, int OID)
        {
            int ptCount = cntLine.Count;
            ShapePoint[] pts = new ShapePoint[ptCount];
            fixed (PointF* ptr0 = cntLine.Points)
            {
                PointF* ptr = ptr0;
                for (int i = 0; i < ptCount; i++, ptr++)
                {
                    ptr->X = (float)(ptr->X * resX + minX);
                    ptr->Y = (float)(maxY - ptr->Y * resY);
                    pts[i] = new ShapePoint(ptr->X, ptr->Y);
                }
            }
            ShapeLineString ring = new ShapeLineString(pts);
            ShapePolyline ply = new ShapePolyline(new ShapeLineString[] { ring });
            Feature fet = new Feature(OID, ply, new string[] { "Contour" }, new string[] { cntLine.ContourValue.ToString() }, null);
            return fet;
        }

        private double[] ToContourValues(frmContourSettings.ContourItem[] contourItems)
        {
            double[] vs = new double[contourItems.Length];
            int idx = 0;
            foreach (frmContourSettings.ContourItem it in contourItems)
                vs[idx++] = it.ContourValue;
            return vs;
        }

        private void TryDisplay(ContourLine[] cntLines, frmContourSettings.ContourItem[] items, bool isLabel, bool isFillColor, bool isNew, IRasterDataProvider dataProvider)
        {
            if (cntLines == null || cntLines.Length == 0)
                return;
            var v = cntLines.Where((cntLine) => { return cntLine != null; });
            if (v == null || v.Count() == 0)
                return;
            ContourClass[] dstItems = ToContourClass(items);
            foreach (ContourLine cntLine in v)
            {
                for (int i = 0; i < dstItems.Length; i++)
                {
                    if (Math.Abs(dstItems[i].ContourValue - cntLine.ContourValue) < double.Epsilon)
                    {
                        cntLine.ClassIndex = i;
                        break;
                    }
                }
            }
            if (isNew)
            {
                string fname = dataProvider.fileName;
                dataProvider.Dispose();
                OpenFileFactory.Open(fname);

            }
            ICanvasViewer cv = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (cv == null)
                return;
            IContourLayer lyr = new ContourLayer("等值线");
            lyr.Apply(v.ToArray(), dstItems, isLabel, isFillColor);
            cv.Canvas.LayerContainer.Layers.Add(lyr as GeoDo.RSS.Core.DrawEngine.ILayer);
            cv.Canvas.Refresh(Core.DrawEngine.enumRefreshType.All);
        }

        private ContourClass[] ToContourClass(frmContourSettings.ContourItem[] items)
        {
            ContourClass[] its = new ContourClass[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                its[i] = new ContourClass(items[i].ContourValue, items[i].Color, 1f);
            }
            return its;
        }

        private int[] GetAOI()
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return null;
            return viewer.AOIProvider.GetIndexes();
        }

        private IRasterDataProvider GetCurrentDataProvider(out int bandNo)
        {
            bandNo = 0;
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return null;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            if (drawing != null)
            {
                bandNo = drawing.SelectedBandNos[0];
                return drawing.DataProviderCopy;
            }
            return null;
        }
    }
}
