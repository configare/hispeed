using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.RasterTools;
using System.Windows.Forms;
using GeoDo.RSS.Core.VectorDrawing;

namespace GeoDo.RSS.UI.AddIn.SpatialInterpolation
{
    [Export(typeof(ICommand))]
    class CommandPointContour : Command
    {
        public CommandPointContour()
        {
            _id = 50002;
            _name = "PointContour";
            _text = "散点图->等值线";
            _toolTip = "散点图生成等值线";
        }

        public override void Execute()
        {
            Execute(null);
        }

        public override void Execute(string argument)
        {
            //判断当前视图中显示的数据，如果没有点状矢量数据，则返回
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return;
            ICanvas canvas = viewer.Canvas;
            if (canvas == null)
                return;
            IVectorHostLayer vectorHost = canvas.LayerContainer.VectorHost;
            if (vectorHost == null)
                return;
            Map map = vectorHost.Map as Map;
            if (map == null)
                return;
            CodeCell.AgileMap.Core.ILayer[] layers = map.LayerContainer.Layers;
            if (layers == null || layers.Length == 0)
                return;

            CodeCell.AgileMap.Core.FeatureLayer fetL = null;
            CodeCell.AgileMap.Core.FeatureClass fetc = null;
            int nCount = 0;
            for (nCount = 0; nCount < layers.Length; nCount++)
            {
                fetL = map.LayerContainer.Layers[nCount] as CodeCell.AgileMap.Core.FeatureLayer;
                fetc = fetL.Class as CodeCell.AgileMap.Core.FeatureClass;
                if (fetc.ShapeType == enumShapeType.Point)
                {
                    break;
                }
            }
            if (nCount == layers.Length)
            {
                MessageBox.Show("视图中未显示点状矢量数据！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Feature[] features = fetc.GetVectorFeatures();
            if (features == null || features.Length == 0)
                return;
            string fileName = null;
            IDataSource ds = fetc.DataSource as FileDataSource;
            if (ds != null)
                fileName = (ds as FileDataSource).FileUrl;
            if (ds == null)
            {
                ds = fetc.DataSource as MemoryDataSource;
                if (ds != null)
                    fileName = (ds as MemoryDataSource).Name;
            }
            int featureCount = features.Count();
            int fieldCount = fetc.FieldNames.Count();

            //取出矢量数据中的字段值
            Dictionary<string, ArrayList> dicFieldValues = new Dictionary<string, ArrayList>();
            for (int i = 0; i < fieldCount; i++)
            {
                ArrayList fieldValues = new ArrayList();
                for (int j = 0; j < featureCount; j++)
                {
                    fieldValues.Add(features[j].FieldValues[i]);
                }
                dicFieldValues.Add(fetc.FieldNames[i], fieldValues);
            }

            for (int i = 0; i < fieldCount; i++)
            {
                double temp;
                if (double.TryParse(features[0].FieldValues[i], out temp))
                {
                }
                else
                {
                    dicFieldValues.Remove(features[0].FieldNames[i]);
                }
            }

            //先插值（插值后的点数由计算的defResX值确定）。再根据插值点生成等值线
            double defResX = (fetc.FullEnvelope.MaxX - fetc.FullEnvelope.MinX) / 500;

            using (frmPointContour frm = new frmPointContour())
            {
                frmPointContour.ContourItem[] contourItems = null;
                frm.DicFieldValues = dicFieldValues;
                frm.SetShpFile(fileName);
                bool isNeedDisplay = false;
                bool isNeedLabel = false;

                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //获取坐标值和字段值
                    contourItems = frm.ContourValues;
                    isNeedDisplay = frm.IsNeedDisplay;
                    isNeedLabel = frm.IsNeedLabel;
                    string selFieldName = frm.GetSelFieldName();
                    string shpOutFile = string.IsNullOrWhiteSpace(frm.ShpFileName) ? null : frm.ShpFileName;

                    List<double> coordListX = new List<double>();
                    List<double> coordListY = new List<double>();
                    List<double> valueList = new List<double>();
                    for (int i = 0; i < featureCount; i++)
                    {
                        ShapePoint point = features[i].Geometry as ShapePoint;
                        if (point == null)
                            continue;

                        coordListX.Add(point.X);
                        coordListY.Add(point.Y);
                        valueList.Add(Convert.ToDouble(dicFieldValues[selFieldName][i]));
                    }

                    //执行生成等值线的操作
                    IDW_Interpolation interpolation = new IDW_Interpolation();
                    interpolation.CoordPointXArr = coordListX.ToArray();
                    interpolation.CoordPointYArr = coordListY.ToArray();
                    interpolation.PointValueArr = valueList.ToArray();

                    DoGenerateContourLines(canvas, defResX, defResX, contourItems, isNeedDisplay, isNeedLabel, interpolation, shpOutFile);
                }
            }
        }

        private void DoGenerateContourLines(ICanvas canvas, double resX, double resY, frmPointContour.ContourItem[] contourItems, bool isNeedDisplay, bool isNeedLabel, IDW_Interpolation interpolate, string shpFileName)
        {
            IProgressMonitor progress = _smartSession.ProgressMonitorManager.DefaultProgressMonitor;
            try
            {
                progress.Reset("正在生成等值线...", 100);
                progress.Start(false);
                ContourGenerateTool tool = new ContourGenerateTool();
                double[] cvs = ToContourValues(contourItems);

                ContourLine[] cntLines = tool.Generate(resX, resY, enumDataType.Float, cvs, interpolate, (idx, tip) => { progress.Boost(idx, tip); });
                if (cntLines == null || cntLines.Length == 0)
                {
                    MsgBox.ShowInfo("不存在符合指定条件的等值线！");
                    return;
                }

                double dMinX = interpolate.CoordPointXArr.Min();
                double dMaxY = interpolate.CoordPointYArr.Max();
                for (int i = 0; i < cntLines.Count(); i++ )
                {
                    ContourLine cntLine = cntLines[i];
                    ContourLine newCntLine = new ContourLine(cntLine.ContourValue);
                    PointF[] pts = cntLine.Points;
                    for (int j = 0; j < pts.Count(); j++)
                    {
                        pts[j].X = Convert.ToSingle(dMinX + resX * pts[j].X);
                        pts[j].Y = Convert.ToSingle(dMaxY - resY * pts[j].Y);
                    }
                    newCntLine.AddPoints(pts);
                    newCntLine.UpdateEnvelope();
                    cntLines[i] = newCntLine;
                }
                if (shpFileName != null)
                    TryExport2ShapeFile(canvas, cntLines, shpFileName, progress, isNeedDisplay);
                if (isNeedDisplay)
                    TryDisplay(cntLines, contourItems, isNeedLabel);
            }
            finally
            {
                progress.Finish();
            }
        }

        private unsafe void TryExport2ShapeFile(ICanvas canvas, ContourLine[] cntLines, string shpFileName, IProgressMonitor progress, bool isNeedDisplay)
        {
            int cntCount = cntLines.Length;
            string tip = "正在将等值线导出为矢量文件({0}/{1})...";
            progress.Reset("正在将等值线导出为矢量文件...", cntLines.Length);
            IEsriShapeFilesWriter writer = new EsriShapeFilesWriterII(shpFileName, enumShapeType.Polyline);
            writer.BeginWrite();
            try
            {
                Feature[] buffer = new Feature[1];
                for (int i = 0; i < cntCount; i++)
                {
                    if (cntLines[i] == null)
                        continue;
                    Feature fet = GetFeature(cntLines[i], canvas, i);
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

        private unsafe Feature GetFeature(ContourLine cntLine, ICanvas canvas, int OID)
        {
            int ptCount = cntLine.Count;
            ShapePoint[] pts = new ShapePoint[ptCount];
            fixed (PointF* ptr0 = cntLine.Points)
            {
                PointF* ptr = ptr0;
                for (int i = 0; i < ptCount; i++, ptr++)
                {
                    double geoX, geoY;
                    canvas.CoordTransform.Prj2Geo(ptr->X, ptr->Y, out geoX, out geoY);
                    pts[i] = new ShapePoint(geoX, geoY);
                }
            }
            ShapeLineString ring = new ShapeLineString(pts);
            ShapePolyline ply = new ShapePolyline(new ShapeLineString[] { ring });
            Feature fet = new Feature(OID, ply, new string[] { "Contour" }, new string[] { cntLine.ContourValue.ToString() }, null);
            return fet;
        }

        private double[] ToContourValues(frmPointContour.ContourItem[] contourItems)
        {
            double[] vs = new double[contourItems.Length];
            int idx = 0;
            foreach (frmPointContour.ContourItem it in contourItems)
                vs[idx++] = it.ContourValue;
            return vs;
        }

        private void TryDisplay(ContourLine[] cntLines, frmPointContour.ContourItem[] items, bool isLabel)
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
            ICanvasViewer cv = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (cv == null)
                return;
            IContourLayer lyr = new ContourLayer("等值线");
            lyr.Apply(v.ToArray(), dstItems, isLabel, false);
            cv.Canvas.LayerContainer.Layers.Add(lyr as GeoDo.RSS.Core.DrawEngine.ILayer);
            cv.Canvas.Refresh(Core.DrawEngine.enumRefreshType.All);
        }

        private ContourClass[] ToContourClass(frmPointContour.ContourItem[] items)
        {
            ContourClass[] its = new ContourClass[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                its[i] = new ContourClass(items[i].ContourValue, items[i].Color, 1f);
            }
            return its;
        }
    }
}
