using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Drawing;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.RasterTools;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.RasterDrawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.SpatialInterpolation
{
    [Export(typeof(ICommand))]
    public class CommandPointInterpolation : Command
    {
        public CommandPointInterpolation()
        {
            _id = 50001;
            _name = "PointInterpolation";
            _text = "空间插值";
            _toolTip = "空间插值";
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
            string fileName=null;
            IDataSource ds = fetc.DataSource as FileDataSource;
            if(ds!=null)
                fileName=(ds as FileDataSource).FileUrl;
            if (ds == null)
            {
                ds = fetc.DataSource as MemoryDataSource;
                if (ds != null)
                    fileName = (ds as MemoryDataSource).Name;
            }
            int featureCount = features.Count();
            int fieldCount   = fetc.FieldNames.Count();
            //取出矢量数据中的字段值
            Dictionary<string, ArrayList> dicFieldValues = new Dictionary<string, ArrayList>();
            for (int i = 0; i < fieldCount; i++ )
            {
                ArrayList fieldValues = new ArrayList();
                for (int j=0; j<featureCount; j++)
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
            //计算矢量数据坐标范围和插值后默认的分辨率大小
            double coordXmin, coordXmax, coordYmin, coordYmax;
            canvas.CoordTransform.Prj2Geo(fetc.FullEnvelope.MinX, fetc.FullEnvelope.MinY, out coordXmin, out coordYmin);
            canvas.CoordTransform.Prj2Geo(fetc.FullEnvelope.MaxX, fetc.FullEnvelope.MaxY, out coordXmax, out coordYmax);
            coordXmin = Math.Min(coordXmin, coordXmax);
            coordXmax = Math.Max(coordXmin, coordXmax);
            coordYmin = Math.Min(coordYmin, coordYmax);
            coordYmax = Math.Max(coordYmin, coordYmax);
            double defResX = (coordXmax - coordXmin) / 1000;
            //////////////////////////////////////////////////////////////////////////
            //插入对话框，获取selFieldName值
            //////////////////////////////////////////////////////////////////////////
            using (frmPointInterpolation frm = new frmPointInterpolation())
            {
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.FieldNames = dicFieldValues.Keys.ToArray();
                frm.ResX = defResX;
                frm.ResY = defResX;
                frm.ResXmin = (coordXmax - coordXmin) / 10000;
                frm.ResXmax = (coordXmax - coordXmin) / 50;
                frm.ResYmin = (coordYmax - coordYmin) / 10000;
                frm.ResYmax = (coordYmax - coordYmin) / 50;
                frm.OutputImg = GetOutImgName(fileName);

                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //获取坐标值和字段值
                    string selFieldName = frm.GetSelFieldName();
                    double resolutionX = frm.ResX;
                    double resolutionY = frm.ResY;
                    string outImg = frm.OutputImg;

                    List<double> coordListX = new List<double>();
                    List<double> coordListY = new List<double>();
                    List<double> valueList = new List<double>();

                    double coordX, coordY;
                    for (int i = 0; i < featureCount; i++)
                    {
                        ShapePoint point = features[i].Geometry as ShapePoint;
                        if (point == null)
                            continue;

                        canvas.CoordTransform.Prj2Geo(point.X, point.Y, out coordX, out coordY);
                        coordListX.Add(coordX);
                        coordListY.Add(coordY);
                        valueList.Add(Convert.ToDouble(dicFieldValues[selFieldName][i]));
                    }

                    IProgressMonitor progress = _smartSession.ProgressMonitorManager.DefaultProgressMonitor;
                    if (progress != null)
                    {
                        progress.Reset("", 100);
                        progress.Start(false);
                    }

                    //执行插值操作
                    IDW_Interpolation interpolation = new IDW_Interpolation();
                    interpolation.CoordPointXArr = coordListX.ToArray();
                    interpolation.CoordPointYArr = coordListY.ToArray();
                    interpolation.PointValueArr = valueList.ToArray();
                    if (fetc.SpatialReference != null)
                    {
                        interpolation.DoIDWinterpolation(resolutionX, resolutionY, outImg, "LDF", enumDataType.Double, new Action<int, string>((int progerss, string text) =>
                        {
                            if (progress != null)
                                progress.Boost(progerss, text);
                        }), fetc.SpatialReference.ToProj4String());
                    }
                    else
                    {
                        interpolation.DoIDWinterpolation(resolutionX, resolutionY, outImg, "LDF", enumDataType.Double, new Action<int, string>((int progerss, string text) =>
                        {
                            if (progress != null)
                                progress.Boost(progerss, text);
                        }), null);
                    }
                     if (progress != null)
                         progress.Finish();
                    
                     //对插值后影像进行渲染，并显示到当前视图中
                     AddRasterLayer(outImg, canvas, valueList.Min(), valueList.Max());
                     canvas.Refresh(enumRefreshType.All);
                }
            }
        }

        private string GetOutImgName(string shpFileName)
        {
            string fileName;
            if(string.IsNullOrEmpty(shpFileName)||!File.Exists(shpFileName))
               fileName=AppDomain.CurrentDomain.BaseDirectory+@"空间插值.ldf";
            fileName = Path.GetFileNameWithoutExtension(shpFileName);
            string extName = ".ldf";
            fileName = fileName + "_SI" + extName;
            fileName = Path.Combine(Path.GetDirectoryName(shpFileName), fileName);
            return fileName;
        }

        private void AddRasterLayer(string strRasName, ICanvas canvas, double minValue, double maxValue)
        {
            IRasterDrawing drawing = new RasterDrawing(strRasName, canvas, GetRgbStretcherProvider(strRasName));
            drawing.SelectedBandNos = GetDefaultBands(drawing);
            Color[] colors = GetDefColors(200);
            ColorMapTable<double> colorTable = GetColorTable(colors, maxValue, minValue);
            if (colorTable == null)
                return;
            drawing.ApplyColorMapTable(colorTable);
            IRasterLayer rstLayer = new RasterLayer(drawing);
            canvas.LayerContainer.Layers.Add(rstLayer);
            canvas.PrimaryDrawObject = drawing;
            canvas.CurrentEnvelope = drawing.OriginalEnvelope;

            int times = drawing.GetOverviewLoadTimes();
            if (times == -1)
            {
                drawing.StartLoading(null);
                return;
            }
            string tipstring = "正在读取文件\"" + Path.GetFileName(strRasName) + "\"...";
            try
            {
                _smartSession.ProgressMonitorManager.DefaultProgressMonitor.Reset(tipstring, times);
                _smartSession.ProgressMonitorManager.DefaultProgressMonitor.Start(false);
                drawing.StartLoading((t, p) =>
                {
                    _smartSession.ProgressMonitorManager.DefaultProgressMonitor.Boost(p, tipstring);
                });
            }
            finally
            {
                _smartSession.ProgressMonitorManager.DefaultProgressMonitor.Finish();
            }
        }

        private RgbStretcherProvider GetRgbStretcherProvider(string fname)
        {
            IRasterDataProvider prd = null;
            try
            {
                prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
                if (prd != null && prd.BandCount == 1)
                    return new RgbStretcherProvider();
            }
            finally
            {
                if (prd != null)
                    prd.Dispose();
            }
            return null;
        }

        private static int[] GetDefaultBands(IRasterDrawing drawing)
        {
            IRasterDataProvider prd = drawing.DataProvider;
            if (prd == null)
                return null;
            int[] defaultBands = prd.GetDefaultBands();
            if (defaultBands == null)
                defaultBands = new int[] { 1, 2, 3 };
            return defaultBands;
        }

        private Color[] GetDefColors(int colorCount)
        {
            Bitmap bitmap = new Bitmap(colorCount, 5);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, colorCount, 5), Color.White, Color.Black, LinearGradientMode.Horizontal);
                    
                ColorBlend blend = new ColorBlend();
               // blend.Colors = new Color[] { Color.Black, Color.DarkViolet, Color.LightSalmon, Color.LightYellow, Color.White };
                blend.Colors = new Color[]{Color.Black, Color.DarkOliveGreen, Color.DarkSlateBlue, Color.MediumVioletRed, Color.OrangeRed, Color.LightSalmon, Color.LightYellow, Color.White};
               // blend.Positions = new float[] { 0F, 1 / 20F, 2 / 3F, 9 / 10F,1F };
                //blend.Positions = new float[] { 0F, 1 / 7F, 2 / 7F, 3 / 7F, 4 / 7F, 5 / 7F, 6 / 7F, 1F };
                blend.Positions = new float[] { 0F, 1 / 20F, 1 / 7F, 2 / 7F, 3 / 7F, 5 / 7F, 9 / 10F, 1F };
                //blend.Colors = new Color[] { Color.Black, Color.DarkViolet, Color.Red, Color.Yellow, Color.LightYellow, Color.White };
                //blend.Positions = new float[] { 0F, 1 / 20F, 1/2F, 2/3F, 9 / 10F, 1F };
                brush.InterpolationColors = blend;

                g.FillRectangle(brush, 0, 0, colorCount, 5);
            }

            Color[] colors = new Color[colorCount];
            for (int i = 0; i < colorCount; i++)
            {
                colors[i] = bitmap.GetPixel(i, 2);
            }
            return colors;
        }

        private ColorMapTable<double> GetColorTable(Color[] colors, double maxValue, double minValue)
        {
            ColorMapTable<double> ct = new ColorMapTable<double>();
            int count = colors.Length;
            double span = (maxValue - minValue) / count;
            double v = minValue;
            for (int i = 0; i < count; i++, v += span)
            {
                ct.Items.Add(new ColorMapItem<double>(v, v + span, colors[i]));
            }
            return ct;
        }
    }
}
