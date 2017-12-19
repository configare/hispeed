using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.Drawing;
using System.Drawing.Drawing2D;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    /// <summary>
    /// 像素信息，用于绘制及统计值
    /// </summary>
    public struct PixelInfo
    {
        //public double GeoX;
        //public double GeoY;
        public double PrjX;
        public double PrjY;
        public int RasterX;
        public int RasterY;
        public int ScreenX;
        public int ScreenY;

        public static PixelInfo Empty
        {
            get { return new PixelInfo(); }
        }

        public bool IsEmpty
        {
            get
            {
                return //GeoX == 0 && GeoY == 0 && 
                    PrjX == 0 && PrjY == 0 && RasterX == 0 && RasterY == 0 && ScreenX == 0 && ScreenY == 0;
            }
        }
    }

    /// <summary>
    /// 剖面图，图层
    /// </summary>
    public class ProfileLayer : Layer, IFlyLayer
    {
        private bool _visible = true;
        /// <summary>
        /// 两个端点的栅格坐标
        /// </summary>
        private PixelInfo _beginPoint = PixelInfo.Empty;
        private PixelInfo _endPoint = PixelInfo.Empty;
        private PixelInfo _mousePoint = PixelInfo.Empty;
        /// <summary>
        /// 是否已经确定起始端点。
        /// </summary>
        //private bool _isRending = false;
        /// <summary>
        /// 当前剖面所选线段的两个顶点坐标(屏幕坐标)
        /// </summary>
        private Point[] _vertices = null;
        private bool _isLabeling = true;
        private Font _labelFont = new Font("微软雅黑", 9);
        private Pen _axixPen;                 //坐标轴颜色
        private Pen _pofileLinePen;                 //坐标轴颜色
        private Pen _linePen = new Pen(Color.Yellow, 1);        //单通道曲线颜色
        private Pen[] _rgbLinePen = null;                       //三通道合成曲线颜色
        private List<RasterProfileData> _profileData = new List<RasterProfileData>();//长度为1代表灰度通道，长度为3代表RGB三通道
        private bool _isNewProfile = true;
        private bool _isGettingData = false;
        private Brush _profileRectangleBrush;   //剖面图外框
        private Brush _profileFillBrush;        //剖面图填充
        private Brush _labelBrush;              //标签笔刷
        private Bitmap _bufferBmp = null;
        private int offsetToTopAndLeft = 20;        //整个背景的顶部和右侧间距
        private int backgroundWidth = 300;          //整个背景的宽和高
        private int backgroundHeight = 200;

        public ProfileLayer()
            : base()
        {
            _name = "ProfileLayer";
            _alias = "剖面图";
            _linePen.LineJoin = LineJoin.Bevel;
            _axixPen = new Pen(Color.Yellow);
            _pofileLinePen = new Pen(Color.Yellow);           //坐标轴颜色
            _pofileLinePen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            _pofileLinePen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            _vertices = new Point[] { Point.Empty, Point.Empty };
            _rgbLinePen = new Pen[] { Pens.Red, Pens.Green, Pens.Blue };
            _profileRectangleBrush = new SolidBrush(Color.Red); //剖面图外框
            _profileFillBrush = new SolidBrush(Color.FromArgb(250, 64, 64, 64));
            _labelBrush = new SolidBrush(Color.Yellow);
        }

        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
            }
        }

        public void NewProfile()
        {
            _visible = true;
            _enabled = true;
            _isNewProfile = true;
        }

        public void Render(object sender, IDrawArgs drawArgs)
        {
            if (!_visible || _beginPoint.IsEmpty)
                return;
            if (_endPoint.IsEmpty && _mousePoint.IsEmpty)
                return;
            ICanvas canvas = sender as ICanvas;
            ICoordinateTransform coordTran = canvas.CoordTransform;
            //初始化剖面图缓存图像。
            //...暂未实现

            //绘制剖面线段
            DrawProfileLine(canvas, drawArgs);
            // 绘制剖面图
            if (!_isGettingData)
            {
                DrawProfile(canvas, drawArgs);
            }
        }

        private bool _iserror = false;

        /// <summary>
        /// 图像上绘制剖面图对应的直线
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="drawArgs"></param>
        private void DrawProfileLine(ICanvas canvas, IDrawArgs drawArgs)
        {
            if (_iserror)
                return;
            Graphics g = drawArgs.Graphics as Graphics;
            SmoothingMode smode = g.SmoothingMode;
            try
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                int x, y;
                ICoordinateTransform coordTran = canvas.CoordTransform;
                coordTran.Prj2Screen(_beginPoint.PrjX, _beginPoint.PrjY, out x, out y);
                _vertices[0].X = x;
                _vertices[0].Y = y;
                if (!_endPoint.IsEmpty)
                {
                    coordTran.Prj2Screen(_endPoint.PrjX, _endPoint.PrjY, out x, out y);
                    _vertices[1].X = x;
                    _vertices[1].Y = y;
                }
                else if (!_mousePoint.IsEmpty)
                {
                    coordTran.Prj2Screen(_mousePoint.PrjX, _mousePoint.PrjY, out x, out y);
                    if (x == _vertices[0].X && y == _vertices[0].Y)
                        return;
                    if (MathHelp.DistanceLessThan(_vertices[0].X, _vertices[0].Y, x, y, 4))
                        return;
                    _vertices[1].X = x;
                    _vertices[1].Y = y;
                }
                g.DrawLines(_pofileLinePen, _vertices);
                g.FillEllipse(_profileRectangleBrush, new RectangleF(_vertices[0].X - 2, _vertices[0].Y - 2, 4, 4));    //端点
            }
            catch (Exception ex)
            {
                _iserror = true;
                SizeF fontSize = g.MeasureString(ex.Message, _labelFont);
                RectangleF fontRect = new RectangleF(canvas.Container.Width - fontSize.Width - offsetToTopAndLeft, offsetToTopAndLeft, fontSize.Width, fontSize.Height);
                g.DrawString(ex.Message, _labelFont, _profileRectangleBrush, fontRect);
            }
            finally
            {
                g.SmoothingMode = smode;
            }
        }

        /// <summary>
        /// 绘制剖面图
        /// 绘制位置在画布的左上角
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="drawArgs"></param>
        private void DrawProfile(ICanvas canvas, IDrawArgs drawArgs)
        {
            if (_profileData == null || _profileData.Count == 0)
                return;
            //数据准备
            int canvasWidth = canvas.Container.Width;
            int gridWidth = 300;    //整个背景的宽和高
            int gridHeight = backgroundHeight - 20;
            double minValue = 0;
            double maxValue = 0;
            GetProfileMinMaxValue(_profileData, out minValue, out maxValue);
            double yStep = (maxValue - minValue) / 10f;
            //绘制剖面图背景（在画布的右上侧）
            Graphics g = drawArgs.Graphics as Graphics;
            //绘制背景
            int backgroundOptX = canvasWidth - backgroundWidth - offsetToTopAndLeft - 50;//外框
            int backgroundOptY = offsetToTopAndLeft - 10;
            Rectangle banckGroundRect = new Rectangle(backgroundOptX, backgroundOptY, backgroundWidth + 55, backgroundHeight + 10);
            g.FillRectangle(_profileFillBrush, banckGroundRect);
            g.DrawRectangle(Pens.Gray, banckGroundRect);
            //绘制x,y轴
            int gridOptX = canvasWidth - gridWidth - offsetToTopAndLeft;
            int gridOptY = offsetToTopAndLeft + gridHeight;
            PointF gridOpt = new PointF(gridOptX, offsetToTopAndLeft + gridHeight);//xy坐标原点(左下角)
            g.DrawLine(_axixPen, gridOptX, gridOptY, gridOptX, offsetToTopAndLeft);
            g.DrawLine(_axixPen, gridOptX, gridOptY, gridOptX + gridWidth, gridOptY);
            //g.FillEllipse(Brushes.Red, new RectangleF(gridOptX - 2, gridOptY - 2, 4, 4));
            //绘制格网
            double preLabelV = minValue;
            double labelV;
            int span = gridHeight / 10;
            int yGrid = gridOptY;
            for (int iSclae = 0; iSclae < 11; iSclae++)
            {
                if (iSclae != 0)
                    g.DrawLine(Pens.YellowGreen, gridOptX, yGrid, gridOptX + gridWidth, yGrid);
                if (_isLabeling)
                {
                    labelV = preLabelV;
                    string str = labelV.ToString("0.##");
                    SizeF size = g.MeasureString(str, _labelFont);
                    g.DrawString(str, _labelFont, _labelBrush, (int)(gridOptX - size.Width), (int)(yGrid - size.Height / 2));
                    preLabelV += yStep;
                }
                yGrid = yGrid - span;
            }
            //绘制折线
            int dataCount = _profileData[0].DataValues.Length;
            int profileCount = _profileData.Count;
            float perUnitPixelsX = gridWidth;
            if (dataCount > 1)
                perUnitPixelsX = (gridWidth * 1.0f / (dataCount - 1));
            float[] locationXs = new float[dataCount];
            float xLocation = 0;
            for (int i = 0; i < dataCount; i++)//计算所有点的x坐标
            {
                locationXs[i] = gridOpt.X + xLocation;
                xLocation += perUnitPixelsX;
            }
            for (int p = 0; p < profileCount; p++)//计算各通道值的y坐标
            {
                float perUnitPixelsY = gridHeight;
                if (maxValue != minValue)
                    perUnitPixelsY = (float)(gridHeight / (maxValue - minValue));
                List<PointF> axisXPts = new List<PointF>();
                PointF pt = new PointF();
                for (int i = 0; i < dataCount; i++)
                {
                    pt.X = locationXs[i];
                    pt.Y = gridOpt.Y - (float)(_profileData[p].DataValues[i] - minValue) * perUnitPixelsY;
                    axisXPts.Add(pt);
                }
                //g.DrawLines(_linePen, axisXPts.ToArray());
                GraphicsPath gp = new GraphicsPath();
                gp.AddLines(axisXPts.ToArray());
                if (profileCount == 1)
                    g.DrawPath(_linePen, gp);
                else if (profileCount == 3)
                    g.DrawPath(_rgbLinePen[p], gp);
            }
            //绘制axisX轴label
            int axisXCount = dataCount;
            float xLabelStep = axisXCount * 1f / 2;
            SizeF fSzie = Size.Empty;
            string text = "";
            int textLocationX = 0;
            int index = 0;
            List<int> labelXIndexs = new List<int>();
            if (axisXCount <= 4)
            {
                for (int i = 0; i < axisXCount; i++)
                {
                    labelXIndexs.Add(i);
                }
            }
            else
            {
                labelXIndexs.Add(0);
                labelXIndexs.Add((int)(axisXCount * 1f / 2) - 1);
                labelXIndexs.Add(axisXCount - 1);
            }
            for (int i = 0; i < labelXIndexs.Count; i++)
            {
                index = labelXIndexs[i];
                text = string.Format("({0},{1})", _profileData[0].Locations[index].X, _profileData[0].Locations[index].Y);
                fSzie = g.MeasureString(text, _labelFont);
                if (locationXs[index] + fSzie.Width / 2 > banckGroundRect.Right)//绘制超过右侧背景边缘
                    textLocationX = (int)(banckGroundRect.Right - fSzie.Width);
                else if (locationXs[index] - fSzie.Width / 2 < banckGroundRect.Left)
                    textLocationX = banckGroundRect.Left;
                else
                    textLocationX = (int)(locationXs[index] - fSzie.Width / 2);
                g.DrawString(text, _labelFont, _labelBrush, textLocationX, gridOptY + 2);
                if (index > 0)
                    g.DrawLine(Pens.YellowGreen, locationXs[index], gridOptY, locationXs[index], gridOptY - 5);
            }
        }

        private void GetProfileMinMaxValue(List<RasterProfileData> profileData, out double minValue, out double maxValue)
        {
            minValue = profileData[0].MinValue;
            maxValue = profileData[0].MaxValue;
            for (int i = 0; i < profileData.Count; i++)
            {
                if (minValue > profileData[i].MinValue)
                    minValue = profileData[i].MinValue;
                if (maxValue < profileData[i].MaxValue)
                    maxValue = profileData[i].MaxValue;
            }
        }

        /// <summary>
        /// 获取两点和水平轴的夹角，
        /// If you want the the angle between the line defined by these two points and the horizontal axis:
        /// angle = Math.Atan2(yDiff, xDiff) * (180 / Math.PI);
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        private double GetAngle(double x1, double y1, double x2, double y2, out double width)
        {
            double xDiff = x2 - x1;
            double yDiff = y2 - y1;
            double LineLength = (float)Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
            //double angle = (float)(Math.Asin(yDiff / LineLength) * 180 / Math.PI);
            //if (xDiff < 0 && yDiff > 0)
            //    angle = 180 - angle;
            //else if (xDiff < 0 && yDiff < 0)
            //    angle = 180 + Math.Abs(angle);
            width = LineLength;
            //If you want the the angle between the line defined by these two points and the horizontal axis:
            double angle = Math.Atan2(yDiff, xDiff) * (180 / Math.PI);
            return angle;
        }

        public void Event(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
            if (!_enabled)
                return;
            ICanvas canvas = sender as ICanvas;
            IRasterDrawing drawing = canvas.PrimaryDrawObject as IRasterDrawing;
            if (drawing == null || drawing.DataProviderCopy == null)
                return;
            ICoordinateTransform coordTran = canvas.CoordTransform;
            switch (eventType)
            {
                case enumCanvasEventType.MouseDown:
                    {
                        try
                        {
                            _isGettingData = true;
                            PixelInfo pInfo = GetPixelInfo(canvas, e.ScreenX, e.ScreenY);
                            if (_beginPoint.IsEmpty || _beginPoint.IsEmpty || _isNewProfile)
                            {//开始一条新的
                                _beginPoint = pInfo;
                                _endPoint = PixelInfo.Empty;
                                _vertices = new Point[] { Point.Empty, Point.Empty };
                                _isNewProfile = false;
                            }
                            else
                            {//完成一条线，完成本次剖面线选择
                                _endPoint = pInfo;
                                _enabled = false;
                                GetData(canvas);
                            }
                        }
                        finally
                        {
                            _isGettingData = false;
                        }
                    }
                    break;
                case enumCanvasEventType.MouseMove:
                    if (_endPoint.IsEmpty && !_isNewProfile)
                    {
                        PixelInfo pInfo = GetPixelInfo(canvas, e.ScreenX, e.ScreenY);
                        _mousePoint = pInfo;
                    }
                    break;
                case enumCanvasEventType.MouseUp:
                    break;
                case enumCanvasEventType.MouseWheel:
                    break;
                case enumCanvasEventType.Paning:
                    break;
                case enumCanvasEventType.Zooming:
                    break;
                case enumCanvasEventType.BeginPan:
                    break;
                case enumCanvasEventType.BeginZoom:
                    break;
                case enumCanvasEventType.DoubleClick:
                    break;
                case enumCanvasEventType.EndPaning:
                    break;
                case enumCanvasEventType.EndZoom:
                    break;
                case enumCanvasEventType.KeyDown:
                    break;
                case enumCanvasEventType.KeyPress:
                    break;
                case enumCanvasEventType.KeyUp:
                    break;
                default:
                    break;
            }
        }

        private PixelInfo GetPixelInfo(ICanvas canvas, int screenX, int screenY)
        {
            ICoordinateTransform coordTran = canvas.CoordTransform;
            PixelInfo pInfo = new PixelInfo();
            pInfo.ScreenX = screenX;
            pInfo.ScreenY = screenY;
            if (canvas.IsReverseDirection)
            {
                IReversedCoordinateTransform tran = canvas as IReversedCoordinateTransform;
                tran.Screen2Prj(screenX, screenY, out pInfo.PrjX, out pInfo.PrjY);
                tran.Screen2Raster((float)screenX, (float)screenY, out pInfo.RasterY, out pInfo.RasterX);
            }
            else
            {
                coordTran.Screen2Prj(screenX, screenY, out pInfo.PrjX, out pInfo.PrjY);
                coordTran.Screen2Raster((float)screenX, (float)screenY, out pInfo.RasterY, out pInfo.RasterX);
            }
            //coordTran.Prj2Geo(pInfo.PrjX, pInfo.PrjY, out pInfo.GeoX, out pInfo.GeoY);
            return pInfo;
        }

        private void GetData(ICanvas canvas)
        {
            //ICoordinateTransform coordTran = canvas.CoordTransform;
            Point[] linePts = new Point[2] { new Point(_beginPoint.RasterX, _beginPoint.RasterY), new Point(_endPoint.RasterX, _endPoint.RasterY) };
            //获取采样点的值
            double width;
            double angle = GetAngle(linePts[0].X, linePts[0].Y, linePts[1].X, linePts[1].Y, out width);
            Point[] dataPts = GetPointsAtLine(new PointF(linePts[0].X, linePts[0].Y), new PointF(linePts[1].X, linePts[1].Y), width, 1);
            if (dataPts.Length == 0)
                return;
            IRasterDrawing drawing = canvas.PrimaryDrawObject as IRasterDrawing;
            IRasterDataProvider raster = drawing.DataProviderCopy;
            List<Point> dataPtList = new List<Point>();
            for (int i = 0; i < dataPts.Length; i++)
            {
                if (dataPts[i].X < 0 || dataPts[i].Y < 0 || dataPts[i].X >= raster.Width || dataPts[i].Y >= raster.Height)
                    continue;
                dataPtList.Add(dataPts[i]);
            }
            if (dataPtList.Count == 0)
                return;
            dataPts = dataPtList.ToArray();
            int[] bandNos = drawing.SelectedBandNos;
            _profileData.Clear();
            if (bandNos.Length == 1)
            {
                IRasterBand band = raster.GetRasterBand(bandNos[0]);
                RasterProfileData data = GetDataAtPoints(dataPts, band);
                _profileData.Add(data);
            }
            else if (bandNos.Length == 3)
            {
                for (int i = 0; i < bandNos.Length; i++)
                {
                    IRasterBand band = raster.GetRasterBand(bandNos[i]);
                    RasterProfileData data = GetDataAtPoints(dataPts, band);
                    _profileData.Add(data);
                }
            }
        }

        /// <summary>
        /// 获取两点之间的点集合
        /// </summary>
        /// <param name="bPoint"></param>
        /// <param name="ePoint"></param>
        /// <param name="width"></param>
        /// <param name="sampleStep"></param>
        /// <returns></returns>
        private Point[] GetPointsAtLine(PointF bPoint, PointF ePoint, double width, int sampleStep)
        {
            float dx = ePoint.X - bPoint.X;
            float dy = ePoint.Y - bPoint.Y;
            float x = 0, y = 0;
            float cosa = dx / (float)width;
            float sina = dy / (float)width;
            List<Point> pts = new List<Point>();
            Point pt = new Point();
            for (int L = 0; L <= width; L += sampleStep)
            {
                x = bPoint.X + L * cosa;
                y = bPoint.Y + L * sina;
                pt.X = (int)x;
                pt.Y = (int)y;
                pts.Add(pt);
            }
            return pts.ToArray();
        }

        /// <summary>
        /// 获取栅格数据指定位置的值
        /// </summary>
        /// <param name="pts"></param>
        /// <param name="band"></param>
        /// <returns></returns>
        private RasterProfileData GetDataAtPoints(Point[] pts, IRasterBand band)
        {
            double[] dataValues = Read(band, pts);
            return new RasterProfileData(pts, dataValues);
        }

        public double[] Read(IRasterBand band, Point[] ptsAtLine)
        {
            double[] buffer = new double[ptsAtLine.Length];
            switch (band.DataType)
            {
                case enumDataType.Byte:
                    {
                        byte[] valuesByte = Read<byte>(band, ptsAtLine);
                        for (int i = 0; i < ptsAtLine.Length; i++)
                            buffer[i] = valuesByte[i];
                    }
                    break;
                case enumDataType.Double:
                    {
                        Double[] valuesByte = Read<Double>(band, ptsAtLine);
                        for (int i = 0; i < ptsAtLine.Length; i++)
                            buffer[i] = valuesByte[i];
                    }
                    break;
                case enumDataType.Float:
                    {
                        Single[] valuesByte = Read<Single>(band, ptsAtLine);
                        for (int i = 0; i < ptsAtLine.Length; i++)
                            buffer[i] = valuesByte[i];
                    }
                    break;
                case enumDataType.Int16:
                    {
                        Int16[] valuesByte = Read<Int16>(band, ptsAtLine);
                        for (int i = 0; i < ptsAtLine.Length; i++)
                            buffer[i] = valuesByte[i];
                    }
                    break;
                case enumDataType.Int32:
                    {
                        Int32[] valuesByte = Read<Int32>(band, ptsAtLine);
                        for (int i = 0; i < ptsAtLine.Length; i++)
                            buffer[i] = valuesByte[i];
                    }
                    break;
                case enumDataType.Int64:
                    {
                        Int64[] valuesByte = Read<Int64>(band, ptsAtLine);
                        for (int i = 0; i < ptsAtLine.Length; i++)
                            buffer[i] = valuesByte[i];
                    }
                    break;
                case enumDataType.UInt16:
                    {
                        UInt16[] valuesByte = Read<UInt16>(band, ptsAtLine);
                        for (int i = 0; i < ptsAtLine.Length; i++)
                            buffer[i] = valuesByte[i];
                    }
                    break;
                case enumDataType.UInt32:
                    {
                        UInt32[] valuesByte = Read<UInt32>(band, ptsAtLine);
                        for (int i = 0; i < ptsAtLine.Length; i++)
                            buffer[i] = valuesByte[i];
                    }
                    break;
                case enumDataType.UInt64:
                    {
                        UInt64[] valuesByte = Read<UInt64>(band, ptsAtLine);
                        for (int i = 0; i < ptsAtLine.Length; i++)
                            buffer[i] = valuesByte[i];
                    }
                    break;
                case enumDataType.Unknow:
                    break;
                default:
                    break;
            }
            return buffer;
        }

        public Ttype[] Read<Ttype>(IRasterBand band, Point[] ptsAtLine)
        {
            //int typeSize = (band as RasterBand).DataTypeSize;
            Ttype[] dataBuffer = new Ttype[1];
            Ttype[] dataValues = new Ttype[ptsAtLine.Length];
            GCHandle handles = GCHandle.Alloc(dataBuffer, GCHandleType.Pinned);
            try
            {
                for (int i = 0; i < ptsAtLine.Length; i++)
                {
                    int x = (int)ptsAtLine[i].X;
                    int y = (int)ptsAtLine[i].Y;
                    band.Read(x, y, 1, 1, handles.AddrOfPinnedObject(), band.DataType, 1, 1);
                    dataValues[i] = dataBuffer[0];
                }
                return dataValues;
            }
            finally
            {
                handles.Free();
            }
        }

        public override void Dispose()
        {
            _linePen.Dispose();
            _labelFont.Dispose();
            _pofileLinePen.Dispose();
            _profileFillBrush.Dispose();
            _profileRectangleBrush.Dispose();
            _rgbLinePen = null;
            base.Dispose();
        }
    }

    public class MathHelp
    {
        /// <summary>
        /// 计算两点之间的像元距离是否在指定距离<paramref name="d"/>内
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool DistanceLessThan(double x1, double x2, double y1, double y2, double d)
        {
            return ((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)) < d * d;
        }

        public static double Distance(double x1, double x2, double y1, double y2, double d)
        {
            return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }

        /// <summary>
        /// 计算两坐标之间的角度
        /// +-180
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double Angle(Point p, Point p2)    //Tuple<double, double>
        {
            double angleOfLine = Math.Atan2((p2.Y - p.Y), (p2.X - p.X)) * 180 / Math.PI;
            return angleOfLine;
        }
    }
}
