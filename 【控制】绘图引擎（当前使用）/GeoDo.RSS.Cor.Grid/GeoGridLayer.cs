using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.ComponentModel;
using System.Xml.Linq;
using System.Drawing;
using GeoDo.Project;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using GeoDo.RSS.Core.RasterDrawing;
using System.Runtime.InteropServices;
using GeoDo.Core;

namespace GeoDo.RSS.Core.Grid
{
    public class GeoGridLayer : Layer, IRenderLayer, IGridLayer
    {
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void MemoryCopy(IntPtr pdest, IntPtr psrc, int length);

        struct Range
        {
            public float MinValue;
            public float MaxValue;

            public Range(float minValue, float maxValue)
            {
                MinValue = minValue;
                MaxValue = maxValue;
            }
        }

        struct GridLine
        {
            public int BeginIndex;
            public int SegmentCount;

            public GridLine(int beginIndex, int segmentCount)
            {
                BeginIndex = beginIndex;
                SegmentCount = segmentCount;
            }
        }

        public enum EnumLabelFormat
        {
            NSEW,
            OnlyNumber,
            Number
        }

        private Color _gridColor = Color.Yellow;
        private Pen _gridPen = new Pen(Color.Yellow, 1);
        private Color _labelColor = Color.Yellow;
        private SolidBrush _labelBrush = new SolidBrush(Color.Yellow);
        private SolidBrush _maskBrush = new SolidBrush(Color.Blue);
        private Color _maskColor = Color.Empty;
        private Font _labelFont = new Font("微软雅黑", 9f);
        private bool _visible = true;
        private bool _enableLabling = true;
        private bool _enableMaskColor = true;
        //
        private ICanvas _canvas;
        private ISpatialReference _spatialRef;
        private ICoordinateTransform _coordTransfrom;
        private int _beginLon = -180;
        private int _beginLat = -90;
        private int _endLon = 180;
        private int _endLat = 90;
        private double _gridSpan = 10d;
        private int _lonLabelStep = 1;
        private int _latLabelStep = 1;
        //
        private List<Range> _validLonRanges = new List<Range>();
        private Range _validLatRange = new Range();
        private List<GridLine> _gridLines = null;
        /*
        * 按照最小间隔1度创建缓存区
        */
        PointF[] _allPrjPoints;        //经纬网格所有点(经线点+纬线点)的投影坐标(只转换一次)
        PointF[] _allPixelPoints;      //经纬网格所有点(经线点+纬线点)的屏幕坐标(每次转换,但为了提高性能开辟了缓存区)
        private int _latLines;         //纬线条数 
        private int _lonLines;         //经线条数 
        private SizeF _fontSize;
        private EnumLabelFormat _labelFormat = EnumLabelFormat.NSEW;

        public GeoGridLayer()
        {
            _gridPen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
            _gridPen.DashPattern = new float[] { 4, 4 };
            _name = _alias = "经纬网格";
            UpdateFontSize();
        }

        private void UpdateFontSize()
        {
            using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
            {
                _fontSize = g.MeasureString("180", _labelFont);
            }
        }

        [DisplayName("是否显示"), Category("状态")]
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        [DisplayName("经纬网间隔"), Category("网格设置")]
        public double GridSpan
        {
            get { return _gridSpan; }
            set
            {
                if (value != _gridSpan)
                {
                    value = Math.Max(0.1d, value);
                    value = Math.Min(90d, value);
                    _gridSpan = value;
                    //recompute geogrid lines
                    _gridLines = null;
                }
            }
        }

        [DisplayName("起始经度"), Category("网格设置")]
        public int BeginLon
        {
            get { return _beginLon; }
            set
            {
                if (_beginLon != value && value < _endLon)
                {
                    if (value < -180)
                        value = -180;
                    else if (value > 180)
                        value = 180;
                    _beginLon = value;
                    //recompute geogrid lines
                    _gridLines = null;
                }
            }
        }

        [DisplayName("起始纬度"), Category("网格设置")]
        public int BeginLat
        {
            get { return _beginLat; }
            set
            {
                if (_beginLat != value && value < _endLat)
                {
                    if (value < -90)
                        value = -90;
                    else if (value > 90)
                        value = 90;
                    _beginLat = value;
                    //recompute geogrid lines
                    _gridLines = null;
                }
            }
        }

        [DisplayName("结束经度"), Category("网格设置")]
        public int EndLon
        {
            get { return _endLon; }
            set
            {
                if (_endLon != value && value > _beginLon)
                {
                    if (value < -180)
                        value = -180;
                    else if (value > 180)
                        value = 180;
                    _endLon = value;
                    //recompute geogrid lines
                    _gridLines = null;
                }
            }
        }

        [DisplayName("结束纬度"), Category("网格设置")]
        public int EndLat
        {
            get { return _endLat; }
            set
            {
                if (_endLat != value && value > _beginLat)
                {
                    if (value < -90)
                        value = -90;
                    else if (value > 90)
                        value = 90;
                    _endLat = value;
                    //recompute geogrid lines
                    _gridLines = null;
                }
            }
        }

        [DisplayName("是否标注"), Category("标注设置")]
        public bool EnableLabling
        {
            get { return _enableLabling; }
            set { _enableLabling = value; }
        }

        [DisplayName("经度步长"), Category("标注设置")]
        public int LonLabelStep
        {
            get { return _lonLabelStep; }
            set
            {
                if (_lonLabelStep != value)
                {
                    _lonLabelStep = value;
                    if (_lonLabelStep < 1)
                        _lonLabelStep = 1;
                }
            }
        }

        [DisplayName("纬度步长"), Category("标注设置")]
        public int LatLabelStep
        {
            get { return _latLabelStep; }
            set
            {
                if (_latLabelStep != value)
                {
                    _latLabelStep = value;
                    if (_latLabelStep < 1)
                        _latLabelStep = 1;
                }
            }
        }

        [DisplayName("标注字体"), Category("标注设置"), XmlPersist(typeof(FontPropertyConverter))]
        public Font LabelFont
        {
            get { return _labelFont; }
            set
            {
                if (value != null)
                {
                    if (_labelFont != null)
                        _labelFont.Dispose();
                    _labelFont = value;
                    UpdateFontSize();
                }
            }
        }

        [DisplayName("标注格式"), Category("标注设置"), XmlPersist(typeof(EnumConverter))]
        public EnumLabelFormat LabelFormat
        {
            get { return _labelFormat; }
            set { _labelFormat = value; }
        }

        [DisplayName("字体光晕"), Category("标注设置"), XmlPersist(typeof(ColorPropertyConverter))]
        public Color FontMaskColor
        {
            get { return _fontMaskColor; }
            set
            {
                if (_maskColor != value)
                {
                    _maskColor = value;
                    if (_maskBrush != null)
                        _maskBrush.Dispose();
                    _maskBrush = new SolidBrush(_maskColor);
                }
            }
        }

        [DisplayName("使用字体光晕"), Category("标注设置")]
        public bool EnableMaskColor
        {
            get { return _enableMaskColor; }
            set
            {
                _enableMaskColor = value;
            }
        }

        [DisplayName("标注颜色"), Category("标注设置"), XmlPersist(typeof(ColorPropertyConverter))]
        public Color LabelColor
        {
            get { return _labelColor; }
            set
            {
                if (_labelColor != value)
                {
                    _labelColor = value;
                    if (_labelBrush != null)
                        _labelBrush.Dispose();
                    _labelBrush = new SolidBrush(_labelColor);
                }
            }
        }

        [DisplayName("网格颜色"), Category("网格设置"), XmlPersist(typeof(ColorPropertyConverter))]
        public Color GridColor
        {
            get { return _gridColor; }
            set
            {
                _gridColor = value;
                float[] dashPattern = null;
                DashStyle dashStyle = DashStyle.Solid;
                float width = _gridPen.Width;
                if (_gridPen != null)
                {
                    dashPattern = _gridPen.DashPattern;
                    dashStyle = _gridPen.DashStyle;
                    _gridPen.Dispose();
                }
                _gridPen = new Pen(value, width);
                _gridPen.DashStyle = dashStyle;
                _gridPen.DashPattern = dashPattern;
            }
        }

        [DisplayName("线型"), Category("网格设置"), XmlPersist(false)]
        public DashStyle LineStyle
        {
            get { return _gridPen.DashStyle; }
            set { _gridPen.DashStyle = value; }
        }

        [DisplayName("线型(虚线)"), Category("网格设置"), XmlPersist(typeof(ArrayConverter))]
        public float[] DashPattern
        {
            get { return _gridPen.DashPattern; }
            set { _gridPen.DashPattern = value; }
        }

        [DisplayName("线宽"), Category("网格设置"), XmlPersist(false)]
        public float LineWidth
        {
            get { return _gridPen.Width; }
            set { _gridPen.Width = value; }
        }

        public unsafe void Render(object sender, IDrawArgs drawArgs)
        {
            if (_canvas == null)
            {
                _canvas = sender as ICanvas;
                _spatialRef = GetSpatialRef();
                _coordTransfrom = _canvas.CoordTransform;
            }
            Graphics g = drawArgs.Graphics as Graphics;
            if (_gridLines == null)
            {
                bool isOK = ComputeValidGeoRange();
                if (!isOK)
                    return;
                ComputeGridLines(_coordTransfrom);
            }
            if (_gridLines != null)
            {
                GCHandle prjHandle = GCHandle.Alloc(_allPrjPoints, GCHandleType.Pinned);
                GCHandle screenHandle = GCHandle.Alloc(_allPixelPoints, GCHandleType.Pinned);
                try
                {
                    QuickTransform qickTran = _coordTransfrom.QuickTransform;
                    int ptCount = _allPixelPoints.Length;
                    MemoryCopy(screenHandle.AddrOfPinnedObject(), prjHandle.AddrOfPinnedObject(), ptCount * Marshal.SizeOf(typeof(PointF)));
                    fixed (PointF* ptr0 = _allPixelPoints)
                    {
                        PointF* pixPtr = ptr0;
                        for (int i = 0; i < ptCount; i++, pixPtr++)
                        {
                            qickTran.Transform(pixPtr);
                        }
                    }
                    //draw lon lines
                    DrawLines(g, 0, _lonLines);
                    //draw lat lines
                    int end = Math.Min(_lonLines + _latLines, _gridLines.Count);
                    DrawLines(g, _lonLines, end);
                    //label
                    DrawLabel(g, qickTran);
                }
                finally
                {
                    prjHandle.Free();
                    screenHandle.Free();
                }
            }
        }

        private void DrawLabel(Graphics g, QuickTransform quickTran)
        {
            if (!_enableLabling)
                return;
            try
            {
                int idx = 0;
                int firstLine = -1;
                double geoX, geoY;
                string labelGeoX = string.Empty, labelGeoY = string.Empty;
                PointF pt;
                //label lonlines
                int endLon = Math.Min(_lonLines, _gridLines.Count);
                for (int iLine = 0; iLine < endLon; iLine += _lonLabelStep)
                {
                    idx = ComputeLabelLocationOfLon(_gridLines[iLine]);
                    if (idx == -1)
                        continue;
                    if (firstLine == -1)
                        firstLine = iLine;
                    pt = _allPixelPoints[idx];
                    geoX = _allPrjPoints[idx].X;
                    geoY = _allPrjPoints[idx].Y;
                    _coordTransfrom.Prj2Geo(geoX, geoY, out geoX, out geoY);
                    if (double.IsInfinity(geoX) || double.IsNaN(geoX) || double.IsInfinity(geoY) || double.IsNaN(geoY))
                        continue;
                    float dlt = 0.005f;
                    if (geoX < 0)
                        dlt = -0.005f;
                    geoX += dlt;
                    labelGeoX = LabelFormatLon(geoX);
                    //避免-180和90重叠
                    if (iLine == firstLine)
                        pt.X += _fontSize.Height;
                    pt.Y = (20 - _fontSize.Height) / 2;
                    if (!_enableMaskColor || _maskBrush == null)
                        g.DrawString(labelGeoX, _labelFont, _labelBrush, pt);
                    else
                        DrawStringWithBorder(labelGeoX, g, pt, _labelFont, _labelBrush, _maskBrush);
                }
                //label latlines
                firstLine = -1;
                int begin = Math.Min(_latLines + _lonLines, _gridLines.Count);
                int endLat = Math.Min(_lonLines, _gridLines.Count);
                for (int iLine = begin - 1; iLine > endLat; iLine -= _latLabelStep)
                {
                    idx = ComputeLabelLocationOfLat(_gridLines[iLine]);
                    if (idx == -1)
                        continue;
                    if (firstLine == -1)
                        firstLine = iLine;
                    pt = _allPixelPoints[idx];
                    geoX = _allPrjPoints[idx].X;
                    geoY = _allPrjPoints[idx].Y;
                    _coordTransfrom.Prj2Geo(geoX, geoY, out geoX, out geoY);
                    if (double.IsInfinity(geoX) || double.IsNaN(geoX) || double.IsInfinity(geoY) || double.IsNaN(geoY))
                        continue;
                    float dlt = 0.005f;
                    if (geoY < 0)
                        dlt = -0.005f;
                    geoY += dlt;
                    labelGeoY = LabelFormatLat(geoY);
                    //避免90和-180重叠
                    if (iLine == firstLine)
                        pt.Y += _fontSize.Height;
                    pt.X = (20 - _fontSize.Width) / 2;
                    SizeF stringSize = g.MeasureString(labelGeoY, _labelFont);
                    g.TranslateTransform(stringSize.Width / 2, stringSize.Height / 2); //设置旋转中心为文字中心
                    g.RotateTransform(90f); //旋转
                    if (!_enableMaskColor || _maskBrush == null)
                        g.DrawString(labelGeoY, _labelFont, _labelBrush, pt);
                    else
                        DrawStringWithBorder(labelGeoY, g, pt, _labelFont, _labelBrush, _maskBrush);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private string LabelFormatLat(double geoY)
        {
            switch (_labelFormat)
            {
                case EnumLabelFormat.NSEW:
                    if (geoY < 0)
                        return geoY.ToString("f1") + "°S";
                    else
                        return geoY.ToString("f1") + "°N";
                case EnumLabelFormat.OnlyNumber:
                    if (geoY < 0)
                        return (-geoY).ToString("f1") + "°";
                    else
                        return geoY.ToString("f1") + "°";
                default:
                    if (geoY < 0)
                        return geoY.ToString("f1") + "°S";
                    else
                        return geoY.ToString("f1") + "°N";
            }
        }

        private string LabelFormatLon(double geoX)
        {
            switch (_labelFormat)
            {
                case EnumLabelFormat.NSEW:
                    if (geoX < 0)
                        return geoX.ToString("f1") + "°W";
                    else
                        return geoX.ToString("f1") + "°E";
                case EnumLabelFormat.OnlyNumber:
                    if (geoX < 0)
                        return (-geoX).ToString("f1") + "°";
                    else
                        return geoX.ToString("f1") + "°";
                case EnumLabelFormat.Number:
                    return geoX.ToString("f1") + "°";
                default:
                    if (geoX < 0)
                        return geoX.ToString("f1") + "°W";
                    else
                        return geoX.ToString("f1") + "°E";
            }
        }

        private int ComputeLabelLocationOfLon(GridLine gridLine)
        {
            int idx = gridLine.BeginIndex + gridLine.SegmentCount - 1;
            int eIdx = idx + 1 - gridLine.SegmentCount;
            int retIdx = -1;
            int i = 0;
            int canvasHeight = _canvas.Container.Height;
            int canvasWidth = _canvas.Container.Width;
            for (i = idx; i >= eIdx; i--)
            {
                if (_allPixelPoints[i].Y > 0 && _allPixelPoints[i].Y < canvasHeight && _allPixelPoints[i].X > 0 && _allPixelPoints[i].X < canvasWidth)
                {
                    retIdx = i;
                    break;
                }
            }
            if (retIdx == -1 && i == eIdx)//所有顶点都在屏幕外面
                return -1;
            return retIdx;
        }

        private int ComputeLabelLocationOfLat(GridLine gridLine)
        {
            int idx = gridLine.BeginIndex;
            int count = gridLine.BeginIndex + gridLine.SegmentCount;
            int retIdx = -1;
            int i = 0;
            int canvasHeight = _canvas.Container.Height;
            int canvasWidth = _canvas.Container.Width;
            for (i = idx; i < count; i++)
            {
                if (_allPixelPoints[i].Y > 0 && _allPixelPoints[i].Y < canvasHeight && _allPixelPoints[i].X > 0 && _allPixelPoints[i].X < canvasWidth)
                {
                    retIdx = i;
                    break;
                }
            }
            if (retIdx == -1 && i == count - 1)//所有顶点都在屏幕外面
                return -1;
            return retIdx;
        }

        private unsafe void DrawLines(Graphics g, int beginLine, int endLine)
        {
            try
            {
                fixed (PointF* ptr0 = _allPixelPoints)
                {
                    PointF* allPointPtr = ptr0;
                    //
                    int ptCount = 0;
                    allPointPtr = ptr0;
                    PointF[] vertexBuffer = null;
                    int prePointCount = 0;
                    //for (int iLine = beginLine; iLine < endLine - 1; iLine++)
                    for (int iLine = beginLine; iLine < endLine; iLine++)
                    {
                        ptCount = _gridLines[iLine].SegmentCount;
                        if (prePointCount != ptCount)
                        {
                            vertexBuffer = new PointF[ptCount];
                            prePointCount = ptCount;
                        }
                        fixed (PointF* buffer0 = vertexBuffer)
                        {
                            PointF* buffer = buffer0;
                            allPointPtr = ptr0 + _gridLines[iLine].BeginIndex;
                            for (int iPt = 0; iPt < ptCount; iPt++, buffer++, allPointPtr++)
                            {
                                buffer->X = allPointPtr->X;
                                buffer->Y = allPointPtr->Y;
                            }
                        }
                        //
                        g.DrawCurve(_gridPen, vertexBuffer);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ComputeGridLines(ICoordinateTransform coordTran)
        {
            try
            {
                if (_gridLines != null)
                    _gridLines.Clear();
                else
                    _gridLines = new List<GridLine>();
                double span = _gridSpan;
                int idx = 0;
                double prjX, prjY;
                double maxLon = 0d;
                double maxLat = 0d;
                double x = 0d, y = 0d;
                //sample lon lines
                foreach (Range range in _validLonRanges)
                {
                    x = Math.Max(range.MinValue, _beginLon);
                    y = 0;
                    maxLon = Math.Min(range.MaxValue, _endLon);
                    maxLat = 0;
                    while (x <= maxLon)
                    {
                        y = Math.Max(_validLatRange.MinValue, _beginLat);
                        GridLine gridLine = new GridLine();
                        gridLine.BeginIndex = idx;
                        maxLat = Math.Min(_validLatRange.MaxValue, _endLat);
                        while (y <= maxLat)
                        {
                            coordTran.Geo2Prj(x, y, out prjX, out prjY);
                            _allPrjPoints[idx].X = (float)prjX;
                            _allPrjPoints[idx].Y = (float)prjY;
                            idx++;
                            y += span;
                        }
                        gridLine.SegmentCount = idx - gridLine.BeginIndex;
                        _gridLines.Add(gridLine);
                        x += span;
                    }
                }
                _lonLines = _gridLines.Count;
                foreach (Range range in _validLonRanges)
                {
                    //sample lat lines
                    y = _validLatRange.MinValue;
                    maxLat = Math.Min(_validLatRange.MaxValue, _endLat);
                    while (y <= _validLatRange.MaxValue)
                    {
                        GridLine gridLine = new GridLine();
                        gridLine.BeginIndex = idx;
                        x = range.MinValue;
                        maxLon = Math.Min(range.MaxValue, _endLon);
                        while (x <= maxLon)
                        {
                            coordTran.Geo2Prj(x, y, out prjX, out prjY);
                            _allPrjPoints[idx].X = (float)prjX;
                            _allPrjPoints[idx].Y = (float)prjY;
                            idx++;
                            x += span;
                        }
                        gridLine.SegmentCount = idx - gridLine.BeginIndex;
                        _gridLines.Add(gridLine);
                        y += span;
                    }
                }
                _latLines = _gridLines.Count - _lonLines;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private bool ComputeValidGeoRange()
        {
            float minLon = -180, maxLon = 180;
            _validLonRanges.Clear();
            //Geographic Lon/Lat //GLL
            if (_spatialRef == null || _spatialRef.ProjectionCoordSystem == null)//地理坐标
            {
                Range lonRange = new Range(Math.Max(-180, _beginLon), Math.Min(180, _endLon));
                _validLatRange.MinValue = Math.Max(-90, _beginLat);
                _validLatRange.MaxValue = Math.Min(90, _endLat);
                _validLonRanges.Add(lonRange);
                goto computeBufferSizeLine;
            }
            NameValuePair lat0 = _spatialRef.ProjectionCoordSystem.GetParaByName("lat0");
            NameValuePair lon0 = _spatialRef.ProjectionCoordSystem.GetParaByName("lon0");
            NameValuePair sp1 = _spatialRef.ProjectionCoordSystem.GetParaByName("sp1");
            NameValuePair sp2 = _spatialRef.ProjectionCoordSystem.GetParaByName("sp2");
            string prjName = _spatialRef.ProjectionCoordSystem.Name.EsriName;
            SetLonRangesAndCorrectEndpoint(prjName, (float)lat0.Value, (float)lon0.Value, out minLon, out maxLon);
            SetLatRangesAndCorrectEndpoint(prjName, lat0, sp1, sp2);
        computeBufferSizeLine:
            _latLines = (int)(((_validLatRange.MaxValue - _validLatRange.MinValue) / _gridSpan) + 1d);
            _latLines *= _validLonRanges.Count;
            foreach (Range range in _validLonRanges)
            {
                _lonLines += (int)(((range.MaxValue - range.MinValue) / _gridSpan) + 1d);
            }
            if (_latLines < 2 || _lonLines < 2)
                return false;
            int ptCount = _lonLines * _latLines * 2;
            _allPixelPoints = new PointF[ptCount];
            _allPrjPoints = new PointF[ptCount];
            return true;
        }

        private void SetLatRangesAndCorrectEndpoint(string prjName, NameValuePair lat0, NameValuePair sp1, NameValuePair sp2)
        {
            switch (prjName)
            {
                case "Lambert_Azimuthal_Equal_Area":
                    if (lat0.Value == 0)//赤道
                    {
                        _validLatRange.MinValue = -90f;
                        _validLatRange.MaxValue = 90f;
                    }
                    else if (lat0.Value == 90)  //北极
                    {
                        _validLatRange.MinValue = 0;
                        _validLatRange.MaxValue = 90f;
                    }
                    else if (lat0.Value == -90)//南极
                    {
                        _validLatRange.MinValue = -90;
                        _validLatRange.MaxValue = 0f;
                    }
                    break;
                case "Albers":
                case "Lambert_Conformal_Conic":
                    if (sp1.Value > 0)//北半球
                    {
                        _validLatRange.MinValue = 0f;
                        _validLatRange.MaxValue = 85;
                    }
                    else//南半球
                    {
                        _validLatRange.MinValue = -85f;
                        _validLatRange.MaxValue = 0;
                    }
                    break;
                default:
                    _validLatRange.MinValue = -85f;
                    _validLatRange.MaxValue = 85;
                    break;
            }
            _validLatRange.MinValue = Math.Max(_validLatRange.MinValue, _beginLat);
            _validLatRange.MaxValue = Math.Min(_validLatRange.MaxValue, _endLat);
        }

        private void SetLonRangesAndCorrectEndpoint(string prjName, float lat0, float lon0, out float min, out float max)
        {
            if (prjName == null)
                prjName = string.Empty;
            if (prjName == "Transverse_Mercator")
            {
                _validLonRanges.Add(new Range(lon0 - 60, lon0 + 60));
            }
            else if (prjName == "Lambert_Azimuthal_Equal_Area")
            {
                if (lat0 == 0)
                {
                    _validLonRanges.Add(new Range(lon0 - 90, lon0 + 90));
                }
                else if (lat0 == -90 || lat0 == 90)
                {
                    _validLonRanges.Add(new Range(-180, 180));
                }
            }
            else if (prjName == "Mercator_2SP" ||
                prjName == "Albers" ||
                prjName == "Lambert_Conformal_Conic" ||
                prjName == "Hammer-Aitoff (world)")
            {
                if (lon0 > 0)//比如中心经度为105
                {
                    float l1 = (int)(lon0 - 180d) + 5;      //-75°+5
                    float l1end = 180;
                    float l2 = -180;
                    float l2end = (int)(lon0 - 180d) - 5;
                    _validLonRanges.Add(new Range(l2, l2end));
                    _validLonRanges.Add(new Range(l1, l1end));
                }
                else if (lon0 < 0)
                {
                    float l1 = -180;
                    float l1end = 180 + lon0 - 5;

                    float l2 = l1end + 5;
                    float l2end = 180;
                    _validLonRanges.Add(new Range(l1, l1end));
                    _validLonRanges.Add(new Range(l2, l2end));
                }
                else
                {
                    _validLonRanges.Add(new Range(-180, 180));
                }
            }
            else
            {
                _validLonRanges.Add(new Range(-180, 180));
            }
            //
            min = float.MaxValue;
            max = float.MinValue;
            for (int i = 0; i < _validLonRanges.Count; i++)
            {
                min = Math.Max(_validLonRanges[i].MinValue, _beginLon);
                max = Math.Min(_validLonRanges[i].MaxValue, _endLon);
                _validLonRanges[i] = new Range(min, max);
            }
            min = float.MaxValue;
            max = float.MinValue;
            foreach (Range r in _validLonRanges)
            {
                if (r.MinValue < min)
                    min = r.MinValue;
                if (r.MaxValue > max)
                    max = r.MaxValue;
            }
        }

        private ISpatialReference GetSpatialRef()
        {
            //此处处理不是很严谨
            if (_canvas == null || _canvas.PrimaryDrawObject == null)
                return new SpatialReference(new GeographicCoordSystem());
            IRasterDrawing drawing = _canvas.PrimaryDrawObject as IRasterDrawing;
            if (drawing == null)
                return new SpatialReference(new GeographicCoordSystem());
            return drawing.DataProvider.SpatialRef;
        }

        public static void DrawStringWithBorder(string text, Graphics g, PointF pointF, Font font, Brush fontBrush, Brush maskBrush)
        {
            g.DrawString(text, font, maskBrush, pointF.X, pointF.Y);
            g.DrawString(text, font, maskBrush, pointF.X + 1, pointF.Y - 1);
            g.DrawString(text, font, maskBrush, pointF.X + 1, pointF.Y);
            g.DrawString(text, font, maskBrush, pointF.X - 1, pointF.Y + 1);
            g.DrawString(text, font, maskBrush, pointF.X - 1, pointF.Y - 1);
            g.DrawString(text, font, maskBrush, pointF.X - 1, pointF.Y);
            g.DrawString(text, font, maskBrush, pointF.X, pointF.Y + 1);
            g.DrawString(text, font, maskBrush, pointF.X, pointF.Y - 1);
            //
            g.DrawString(text, font, fontBrush, pointF.X, pointF.Y);
        }
        #region Xml
        public System.Xml.Linq.XElement ToXml()
        {
            XElement xml = new XElement("GeoGridLayer",
                new XElement("Visible", _visible),
                new XElement("GridSpan", _gridSpan),
                new XElement("BeginLon", _beginLon),
                new XElement("BeginLat", _beginLat),
                new XElement("EndLon", _endLon),
                new XElement("EndLat", _endLat),
                new XElement("LabelFont", FontToXml(_labelFont)),
                new XElement("LabelColor", _labelColor.ToArgb()),
                new XElement("GridColor", _gridColor.ToArgb()),

                new XElement("EnableLabling", _enableLabling),
                new XElement("LonLabelStep", _lonLabelStep),
                new XElement("LatLabelStep", _latLabelStep),
                new XElement("FontMaskColor", _maskColor),
                new XElement("EnableMaskColor", _enableMaskColor),

                new XElement("LineWidth", _gridPen.Width),
                new XElement("LineStyle", _gridPen.DashStyle),
                new XElement("DashPattern", IntArrayToXml(_gridPen.DashPattern)),
                new XElement("LabelFormat", _labelFormat)
                );
            return xml;
        }

        private string IntArrayToXml(float[] vals)
        {
            if (vals == null || vals.Length == 0)
                return "";
            string s = "";
            foreach (float val in vals)
            {
                s += val + ",";
            }
            return s.TrimEnd(',');
        }

        public static GeoGridLayer FromXml(XElement xml)
        {
            if (xml == null)
                return null;
            GeoGridLayer geoGridLayer = new GeoGridLayer();

            bool visible;
            if (TryParse(xml, "Visible", out visible))
                geoGridLayer.Visible = visible;
            bool enableLabling;
            if (TryParse(xml, "EnableLabling", out enableLabling))
                geoGridLayer.EnableLabling = enableLabling;
            bool enableMaskColor;
            if (TryParse(xml, "EnableMaskColor", out enableMaskColor))
                geoGridLayer.EnableMaskColor = enableMaskColor;
            //
            double gridSpan;
            if (TryParse(xml, "GridSpan", out gridSpan))
                geoGridLayer.GridSpan = gridSpan;
            int beginLon;
            if (TryParse(xml, "BeginLon", out beginLon))
                geoGridLayer.BeginLon = beginLon;
            int beginLat;
            if (TryParse(xml, "BeginLat", out beginLat))
                geoGridLayer.BeginLat = beginLat;
            int endLon;
            if (TryParse(xml, "EndLon", out endLon))
                geoGridLayer.EndLon = endLon;
            int endLat;
            if (TryParse(xml, "EndLat", out endLat))
                geoGridLayer.EndLat = endLat;
            Font LabelFont;
            if (TryParse(xml, "LabelFont", out LabelFont))
                geoGridLayer.LabelFont = LabelFont;
            int LabelColor;
            if (TryParse(xml, "LabelColor", out LabelColor))
                geoGridLayer.LabelColor = Color.FromArgb(LabelColor);
            if (TryParse(xml, "FontMaskColor", out LabelColor))
                geoGridLayer.FontMaskColor = Color.FromArgb(LabelColor);
            int GridColor;
            if (TryParse(xml, "GridColor", out GridColor))
                geoGridLayer.GridColor = Color.FromArgb(GridColor);

            int LonLabelStep;
            if (TryParse(xml, "LonLabelStep", out LonLabelStep))
                geoGridLayer.LonLabelStep = LonLabelStep;
            int LatLabelStep;
            if (TryParse(xml, "LatLabelStep", out LatLabelStep))
                geoGridLayer.LatLabelStep = LatLabelStep;

            float LineWidth;
            if (TryParse(xml, "LineWidth", out LineWidth))
                geoGridLayer.LineWidth = LineWidth;
            EnumLabelFormat LabelFormat;
            if (TryParse(xml, "LabelFormat", out LabelFormat))
                geoGridLayer.LabelFormat = LabelFormat;
            DashStyle LineStyle;
            if (TryParse(xml, "LineStyle", out LineStyle))
                geoGridLayer.LineStyle = LineStyle;
            float[] DashPattern;
            if (TryParse(xml, "DashPattern", out DashPattern))
                geoGridLayer.DashPattern = DashPattern;

            return geoGridLayer;
        }

        private static bool TryParse(XElement xml, string xname, out bool value)
        {
            value = false;
            string xmlValue = xml.Element(xname) == null ? null : xml.Element(xname).Value;
            if (string.IsNullOrWhiteSpace(xmlValue))
                return false;
            return bool.TryParse(xmlValue, out value);
        }

        private static bool TryParse(XElement xml, string xname, out float[] DashPattern)
        {
            DashPattern = null;
            string xValue = xml.Element(xname) == null ? null : xml.Element(xname).Value;
            if (string.IsNullOrWhiteSpace(xValue))
                return false;
            List<float> retvalues = new List<float>();
            string[] vals = xValue.Split(',');
            float v;
            foreach (string val in vals)
            {
                if (float.TryParse(val, out v))
                    retvalues.Add(v);
            }
            DashPattern = retvalues.ToArray();
            return true;
        }

        private static bool TryParse(XElement xml, string xname, out DashStyle value)
        {
            value = DashStyle.Custom;
            string gridSpan = xml.Element(xname) == null ? null : xml.Element(xname).Value;
            return (!string.IsNullOrWhiteSpace(gridSpan) && DashStyle.TryParse(gridSpan, out value));
        }

        private static bool TryParse(XElement xml, string xname, out EnumLabelFormat value)
        {
            value = EnumLabelFormat.NSEW;
            string gridSpan = xml.Element(xname) == null ? null : xml.Element(xname).Value;
            return (!string.IsNullOrWhiteSpace(gridSpan) && EnumLabelFormat.TryParse(gridSpan, out value));
        }

        private static bool TryParse(XElement xml, string xname, out Font LabelFont)
        {
            LabelFont = new Font("宋体", 9);
            if (xml.Element(xname) == null)
                return false;
            LabelFont = Base64StringToObject(xml.Element(xname).Value) as Font;
            return LabelFont != null;
        }

        private static bool TryParse(XElement xml, string xname, out int value)
        {
            value = -1;
            string gridSpan = xml.Element(xname) == null ? null : xml.Element(xname).Value;
            return (!string.IsNullOrWhiteSpace(gridSpan) && int.TryParse(gridSpan, out value));
        }

        private static bool TryParse(XElement xml, string xname, out float value)
        {
            value = -1;
            string gridSpan = xml.Element(xname) == null ? null : xml.Element(xname).Value;
            return (!string.IsNullOrWhiteSpace(gridSpan) && float.TryParse(gridSpan, out value));
        }

        private static bool TryParse(XElement xml, string xname, out double value)
        {
            value = -1;
            string gridSpan = xml.Element(xname) == null ? null : xml.Element(xname).Value;
            return (!string.IsNullOrWhiteSpace(gridSpan) && double.TryParse(gridSpan, out value));
        }

        private string FontToXml(Font font)
        {
            if (font == null)
                return null;
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = (IFormatter)new BinaryFormatter();
                formatter.Serialize(ms, font);
                ms.Position = 0;
                ms.Flush();
                using (BinaryReader br = new BinaryReader(ms))
                {
                    byte[] cache = br.ReadBytes((int)ms.Length);
                    return Convert.ToBase64String(cache, 0, cache.Length);
                }
            }
        }

        public static object Base64StringToObject(string s)
        {
            if (String.IsNullOrEmpty(s))
                return null;
            byte[] cache = Convert.FromBase64String(s);
            Stream sm = new MemoryStream(cache);
            IFormatter fomatter = (IFormatter)new BinaryFormatter();
            return fomatter.Deserialize(sm);
        }
        #endregion

        public Color _fontMaskColor { get; set; }
    }
}
