using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.RasterTools;
using CodeCell.AgileMap.Core;
using System.Drawing;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public class GenerateContourLines
    {
        private Action<int, string> _progressTracker = null;
        private IContextMessage _contextMessage = null;

        public GenerateContourLines(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _progressTracker = progressTracker;
            _contextMessage = contextMessage;
        }

        public void DoGenerateContourLines(IRasterDataProvider dataProvider, int bandNo, int[] aoi, double interval, double begin, double end, int sample, string shpFileName, bool isOutputUncompleted)
        {
            try
            {
                IContourGenerateTool tool = new ContourGenerateTool();
                tool.IsOutputUncompleted = isOutputUncompleted;
                tool.Sample = sample;
                ContourLine[] cntLines = tool.Generate(dataProvider.GetRasterBand(bandNo),
                    begin, interval, end, aoi, _progressTracker);
                if (cntLines == null || cntLines.Length == 0)
                {
                    PrintInfo("不存在符合指定条件的等值线！");
                    return;
                }
                if (shpFileName != null)
                    TryExport2ShapeFile(dataProvider, cntLines, shpFileName);
            }
            finally
            {
            }
        }

        public void DoGenerateContourLines(IRasterDataProvider dataProvider, int bandNo, int[] aoi, double[] contourValues, int sample, string shpFileName, bool isOutputUncompleted)
        {
            try
            {
                try
                {
                    IContourGenerateTool tool = new ContourGenerateTool();
                    tool.Sample = sample;
                    ContourLine[] cntLines = tool.Generate(dataProvider.GetRasterBand(bandNo), contourValues, aoi, _progressTracker);
                    if (cntLines == null || cntLines.Length == 0)
                    {
                        PrintInfo("不存在符合指定条件的等值线！");
                        return;
                    }
                    if (shpFileName != null)
                        TryExport2ShapeFile(dataProvider, cntLines, shpFileName);
                }
                finally
                {
                }
            }
            finally
            {
            }
        }

        private unsafe void TryExport2ShapeFile(IRasterDataProvider dataProvider, ContourLine[] cntLines, string shpFileName)
        {
            int cntCount = cntLines.Length;
            string tip = "正在将等值线导出为矢量文件({0}/{1})...";
            float interval = 100f / cntCount;
            IEsriShapeFilesWriter writer = new EsriShapeFilesWriterII(shpFileName, enumShapeType.Polyline);
            writer.BeginWrite();
            int index = 0;
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
                    index = (int)(Math.Floor(interval * i));
                    Feature fet = GetFeature(cntLines[i], resX, resY, minX, maxY, i);
                    if (fet != null)
                    {
                        buffer[0] = fet;
                        writer.Write(buffer);
                    }
                    if (_progressTracker != null)
                        _progressTracker(index, string.Format(tip, i, cntCount));
                }
            }
            finally
            {
                writer.EndWriter();
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

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
    }
}
