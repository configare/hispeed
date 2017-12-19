using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.ComponentModel;
using GeoDo.RSS.RasterTools;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Xml.Linq;
using System.IO;
using GeoDo.Core;

namespace GeoDo.RSS.Core.VectorDrawing
{
    [DisplayName(), TypeConverter(typeof(ExpandableObjectConverter))]
    public class ContourClass : IDisposable
    {
        private Pen _pen;
        private double _contourValue;
        public bool IsProjected = false;
        internal Bitmap _labelBuffer;
        private bool _isDisplay = true;

        public ContourClass()
        { 
        }

        public ContourClass(double contourValue, Color color, float lineWidth)
        {
            _contourValue = contourValue;
            _pen = new Pen(new SolidBrush(color), lineWidth);
        }

        [DisplayName("等值")]
        public double ContourValue
        {
            get { return _contourValue; }
            set { _contourValue = value; }
        }

        [DisplayName("颜色"),XmlPersist(typeof(ColorPropertyConverter))]
        public Color ContourColor
        {
            get { return _pen != null ? Pen.Color : Color.Empty; }
            set
            {
                float width = 1;
                if (_pen != null)
                {
                    width = _pen.Width;
                    _pen.Dispose();
                }
                _pen = new Pen(new SolidBrush(value), width);
                if (_labelBuffer != null)
                {
                    _labelBuffer.Dispose();
                    _labelBuffer = null;
                }
            }
        }

        [DisplayName("线宽")]
        public float LineWidth
        {
            get { return _pen != null ? Pen.Width : 1f; }
            set
            {
                Color c = Color.Red;
                if (_pen != null)
                {
                    c = _pen.Color;
                    _pen.Dispose();
                }
                _pen = new Pen(new SolidBrush(c), value);
                if (_labelBuffer != null)
                {
                    _labelBuffer.Dispose();
                    _labelBuffer = null;
                }
            }
        }

        [DisplayName("是否显示")]
        public bool IsDisplay
        {
            get { return _isDisplay; }
            set { _isDisplay = value; }
        }

        [Browsable(false),XmlPersist(false)]
        public Pen Pen
        {
            get { return _pen; }
        }

        [Browsable(false),XmlPersist(false)]
        public Bitmap LabelBuffer
        {
            get { return _labelBuffer; }
        }

        public void UpdateLabelBuffer(Graphics g, Font font, Brush labelBrush, Brush maskBrush, string labelFormat)
        {
            SizeF sizef = g.MeasureString(_contourValue.ToString(labelFormat), font);
            Bitmap bitmap = new Bitmap((int)Math.Ceiling(sizef.Width), (int)Math.Ceiling(sizef.Height), PixelFormat.Format32bppArgb);
            using (Graphics bg = Graphics.FromImage(bitmap))
            {
                bg.Clear(Color.Transparent);
                if (maskBrush != null)
                    ContourLayer.DrawStringWithBorder(_contourValue.ToString(labelFormat), bg, new PointF(0, 0), font, labelBrush, maskBrush);
                else
                    bg.DrawString(_contourValue.ToString(labelFormat), font, labelBrush, new PointF(0, 0));
            }
            _labelBuffer = bitmap;
        }

        public override string ToString()
        {
            return _contourValue.ToString() + "," + this.IsDisplay.ToString();
        }

        public void Dispose()
        {
            if (_pen != null)
            {
                _pen.Dispose();
                _pen = null;
            }
            if (_labelBuffer != null)
            {
                _labelBuffer.Dispose();
                _labelBuffer = null;
            }
        }
    }

    public class ContourLayer : Layer, IVectorLayer, IContourLayer, IActionAtContructAfter
    {
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void MemoryCopy(IntPtr pdest, IntPtr psrc, int length);
        protected bool _visible = true;
        protected bool _isFillColor = false;
        protected bool _isLabel = false;
        protected ICanvas _canvas;
        protected ContourLine.ContourEnvelope _crtCanvasEnvelope;
        protected int MIN_WIDTH = 10;
        protected int MIN_HEIGHT = 10;
        protected EventHandler _canvasSizeChanged;
        protected ContourClass[] _contourClassItems;
        protected ContourLine[] _cntLines;
        protected GraphicsPath _grahpicPath = new GraphicsPath();
        protected int _pointfSize = (int)Math.Log(Marshal.SizeOf(typeof(PointF)), 2);//2^3 = 8;
        protected Font _font = new Font("宋体", 9f);
        protected Brush _labelBrush = new SolidBrush(Color.Black);
        protected bool _isMaskLabel = true;
        protected Brush _maskBrush = new SolidBrush(Color.FromArgb(192, 192, 255));
        protected bool _isProjected = false;
        protected bool _isSmoothMode = false;
        protected bool _isUseCurveRender = false;
        protected float _tension = 0.4f;
        protected string _labelFormat = "0.##";
        protected bool _isCheckConflicting = true;
        protected IConflictor _conflictor;
        protected bool _isCanDocumentable = true;
        protected string _fileName;

        public ContourLayer()
            : base()
        {
            _name = "等值线";
        }

        public ContourLayer(string name)
            : base()
        {
            _name = name;
        }

        [DisplayName("是否显示"), Category("显示")]
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        [DisplayName("是否填色"), Category("显示")]
        public bool IsFillColor
        {
            get { return _isFillColor; }
            set { _isFillColor = value; }
        }

        [DisplayName("标注格式"), Category("显示")]
        public string LabelFormat
        {
            get { return _labelFormat; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                _labelFormat = value;
                FreeLabelBuffers();
            }
        }

        [DisplayName("是否标注"), Category("标注")]
        public bool IsLabel
        {
            get { return _isLabel; }
            set { _isLabel = value; }
        }

        [DisplayName("冲突检测"), Category("标注")]
        public bool IsCheckConflicting
        {
            get { return _isCheckConflicting; }
            set { _isCheckConflicting = value; }
        }

        [DisplayName("等值线"), Category("显示"),
        TypeConverter(typeof(ExtenderProvidedPropertyAttribute)),
        ReadOnly(false), XmlPersist("ContourClass", typeof(ContourClass))]
        public ContourClass[] Items
        {
            get { return _contourClassItems; }
            set { _contourClassItems = value; }
        }

        [Browsable(false),XmlPersist(false)]
        public ContourLine[] ContourLines
        {
            get { return _cntLines; }
        }

        [DisplayName("平滑绘制"), Category("显示")]
        public bool IsSmoothMode
        {
            get { return _isSmoothMode; }
            set { _isSmoothMode = value; }
        }

        [DisplayName("基数样条"), Category("显示")]
        public bool IsUseCurveRender
        {
            get { return _isUseCurveRender; }
            set { _isUseCurveRender = value; }
        }

        [DisplayName("张力系数"), Category("显示")]
        public float Tension
        {
            get { return _tension; }
            set
            {
                _tension = value;
                if (_tension < float.Epsilon)
                    _tension = 0.1f;
            }
        }

        [DisplayName("最小宽度(像素)"), Category("显示过滤"), Description("在当前比例尺下等值线最小外包矩形宽度小于该值时不显示等值线。")]
        public int Min_Width_Filter
        {
            get { return MIN_WIDTH; }
            set { MIN_WIDTH = value; }
        }

        [DisplayName("最小高度(像素)"), Category("显示过滤"), Description("在当前比例尺下等值线最小外包矩形高度小于该值时不显示等值线。")]
        public int Min_Height_Filter
        {
            get { return MIN_HEIGHT; }
            set { MIN_HEIGHT = value; }
        }

        [DisplayName("标注字体"), Category("标注"), XmlPersist(typeof(FontPropertyConverter))]
        public Font LabelFont
        {
            get { return _font; }
            set
            {
                if (_font != null)
                    _font.Dispose();
                _font = value;
                FreeLabelBuffers();
            }
        }

        [DisplayName("颜色"), Category("标注"), XmlPersist(typeof(ColorPropertyConverter))]
        public Color LabelColor
        {
            get
            {
                return _labelBrush != null ? (_labelBrush as SolidBrush).Color : Color.Empty;
            }
            set
            {
                if (_labelBrush != null)
                    _labelBrush.Dispose();
                _labelBrush = new SolidBrush(value);
                FreeLabelBuffers();
            }
        }

        [DisplayName("边框颜色"), Category("标注"), XmlPersist(typeof(ColorPropertyConverter))]
        public Color MaskColor
        {
            get
            {
                return _maskBrush != null ? (_maskBrush as SolidBrush).Color : Color.Empty;
            }
            set
            {
                if (_maskBrush != null)
                    _maskBrush.Dispose();
                _maskBrush = new SolidBrush(value);
                FreeLabelBuffers();
            }
        }

        [DisplayName("显示边框"), Category("标注")]
        public bool IsMaskLabel
        {
            get { return _isMaskLabel; }
            set
            {
                _isMaskLabel = value;
                FreeLabelBuffers();
            }
        }

        [DisplayName("文件名"), Category("文档化")]
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        private void FreeLabelBuffers()
        {
            if (_contourClassItems == null || _contourClassItems.Length == 0)
                return;
            foreach (ContourClass item in _contourClassItems)
            {
                if (item == null)
                    continue;
                if (item.LabelBuffer != null)
                {
                    item.LabelBuffer.Dispose();
                    item._labelBuffer = null;
                }
            }
        }

        private void TryUpdateLabelBuffers(IDrawArgs arg)
        {
            if (!_isLabel || _contourClassItems == null || _contourClassItems.Length == 0)
                return;
            Graphics g = arg.Graphics as Graphics;
            foreach (ContourClass item in _contourClassItems)
            {
                if (item.LabelBuffer == null)
                    item.UpdateLabelBuffer(g, _font, _labelBrush, _maskBrush, _labelFormat);
            }
        }

        public void Apply(ContourLine[] contourLines, ContourClass[] items, bool isLabel, bool isFillColor)
        {
            _isLabel = isLabel;
            _isFillColor = isFillColor;
            _cntLines = contourLines;
            _contourClassItems = items;
        }

        public void Apply(ContourLine[] contourLines, bool isProjected)
        {
            _cntLines = contourLines;
            _isProjected = isProjected;
        }

        public void Render(object sender, IDrawArgs drawArgs)
        {
            if (_cntLines == null || _cntLines.Length == 0 || _contourClassItems == null || _contourClassItems.Length == 0)
                return;
            if (_canvas == null)
            {
                _canvas = sender as ICanvas;
                _canvasSizeChanged = new EventHandler(Container_SizeChanged);
                _canvas.OnEnvelopeChanged += _canvasSizeChanged;
                _conflictor = new PixelConflictor(_canvas.Container.Size);
            }
            CoordEnvelope evp = _canvas.CurrentEnvelope;
            _crtCanvasEnvelope = new ContourLine.ContourEnvelope((float)evp.MinX, (float)evp.MaxX, (float)evp.MinY, (float)evp.MaxY);
            TryUpdateLabelBuffers(drawArgs);
            TryProject();
            TrySetSmoothMode(drawArgs);
            TryResetConflictor();
            float minWidth = MIN_WIDTH * _canvas.ResolutionX;
            float minHeight = MIN_WIDTH * _canvas.ResolutionY;
            ContourLine.ContourEnvelope thisEvp = null;
            int classIdx = 0;
            ContourClass cntClass = null;
            foreach (ContourLine cntLine in _cntLines)
            {
                if (cntLine == null)
                    continue;
                classIdx = cntLine.ClassIndex;
                cntClass = _contourClassItems[classIdx];
                thisEvp = cntLine.Envelope;
                //如果不在可视区域则绘制
                if (!_crtCanvasEnvelope.IsInteractived(thisEvp))
                    continue;
                //如果在当前比例尺下太小则不显示
                if (thisEvp.Width > minWidth || thisEvp.Height > minHeight)
                    DrawContour(drawArgs, cntLine, cntClass, _canvas);
            }
        }

        private void TryResetConflictor()
        {
            if (_isCheckConflicting)
                _conflictor.Reset();
        }

        private void TrySetSmoothMode(IDrawArgs arg)
        {
            if (_isSmoothMode)
            {
                Graphics g = arg.Graphics as Graphics;
                g.SmoothingMode = _isSmoothMode ? SmoothingMode.HighQuality : SmoothingMode.Default;
            }
        }

        private void TryProject()
        {
            if (_isProjected)
                return;
            foreach (ContourLine cntLine in _cntLines)
            {
                if (cntLine == null)
                    continue;
                Project(cntLine, _canvas);
            }
            _isProjected = true;
        }

        private unsafe void DrawContour(IDrawArgs drawArgs, ContourLine cntLine, ContourClass cntClass, ICanvas canvas)
        {
            if (!cntClass.IsDisplay)
                return;
            ICoordinateTransform tran = canvas.CoordTransform;
            int nCount = cntLine.Count;
            PointF[] pts = new PointF[nCount];
            GCHandle dstHandle = GCHandle.Alloc(pts, GCHandleType.Pinned);
            GCHandle srcHandle = GCHandle.Alloc(cntLine.Points, GCHandleType.Pinned);
            try
            {
                MemoryCopy(dstHandle.AddrOfPinnedObject(), srcHandle.AddrOfPinnedObject(), nCount << _pointfSize); // nCount * sizeof(PointF)
                QuickTransform quickTran = drawArgs.QuickTransformArgs;
                fixed (PointF* ptr0 = pts)
                {
                    PointF* ptr = ptr0;
                    for (int i = 0; i < nCount; i++, ptr++)
                        quickTran.Transform(ptr);
                }
                Graphics g = drawArgs.Graphics as Graphics;
                //
                if (_isUseCurveRender)
                {
                    g.DrawCurve(cntClass.Pen, pts, _tension);
                }
                else
                {
                    _grahpicPath.Reset();
                    _grahpicPath.AddLines(pts);
                    g.DrawPath(cntClass.Pen, _grahpicPath);
                }
                //
                if (_isLabel)
                {
                    if (_isCheckConflicting)
                    {
                        if (_conflictor.IsConflicted(pts[0], cntClass.LabelBuffer.Size))
                            return;
                        _conflictor.HoldPosition(pts[0], cntClass.LabelBuffer.Size);
                    }
                    g.DrawImageUnscaled(cntClass.LabelBuffer, (int)pts[0].X, (int)pts[0].Y);
                }
            }
            finally
            {
                dstHandle.Free();
                srcHandle.Free();
            }
        }

        private unsafe void Project(ContourLine cntLine, ICanvas canvas)
        {
            ICoordinateTransform tran = canvas.CoordTransform;
            if (tran == null)
                return;
            int nCount = cntLine.Count;
            fixed (PointF* ptr0 = cntLine.Points)
            {
                PointF* ptr = ptr0;
                for (int i = 0; i < nCount; i++, ptr++)
                    tran.Raster2Prj(ptr);
            }
            cntLine.UpdateEnvelope();
        }

        internal static void DrawStringWithBorder(string text, Graphics g, PointF pointF, Font font, Brush fontBrush, Brush maskBrush)
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

        void Container_SizeChanged(object sender, EventArgs e)
        {
            if (_isCheckConflicting)
            {
                _conflictor.Reset(_canvas.Container.Size);
            }
        }

        public void Save(string fname, bool isWithDataFile)
        {
            if (string.IsNullOrEmpty(fname))
                return;
            if (!fname.ToLower().EndsWith(".xml"))
                fname += ".xml";
            _fileName = fname;
            //保存视图（等值线绘制及标绘的设置）
            (new Object2Xml()).ToXmlFile(this, fname);
            //保存等值线数据（采用投影坐标）
            if (isWithDataFile)
            {
                ContourDataStorager st = new ContourDataStorager();
                ContourLine.ContourEnvelope envelope = null;
                string spatialRef = null;
                GetSpatialRefEnvelope(out envelope, out spatialRef);
                string dataFileName = fname + ".contour";
                st.Save(_cntLines, envelope, spatialRef, dataFileName);
            }
        }

        private void GetSpatialRefEnvelope(out ContourLine.ContourEnvelope envelope, out string spatialRef)
        {
            envelope = null;
            spatialRef = null;
            if (_canvas == null || _canvas.PrimaryDrawObject == null)
                return;
            IPrimaryDrawObject primaryObj = _canvas.PrimaryDrawObject;
            envelope = new ContourLine.ContourEnvelope(
                (float)primaryObj.OriginalEnvelope.MinX,
                (float)primaryObj.OriginalEnvelope.MaxX,
                (float)primaryObj.OriginalEnvelope.MinY,
                (float)primaryObj.OriginalEnvelope.MaxY);
            spatialRef = primaryObj.SpatialRef;
        }

        public void Load()
        {
            if (string.IsNullOrEmpty(_fileName))
                return;
            TryLoadContourValues(_fileName, this);
        }

        public XElement ToXml()
        {
            return (new Object2Xml()).ToXml(this);
        }

        public static IContourLayer FromXml(XElement element)
        {
            return (new Object2Xml()).FromXml(element) as IContourLayer;
        }

        public static IContourLayer FromXml(string fname, bool isWithData)
        {
            IContourLayer lyr = (new Object2Xml()).FromXml(fname) as IContourLayer;
            if (lyr == null)
                return lyr;
            if (isWithData)
                TryLoadContourValues(fname, lyr);
            if (lyr is ContourLayer)
                (lyr as ContourLayer).FileName = fname;
            return lyr;
        }

        private static void TryLoadContourValues(string fname, IContourLayer layer)
        {
            string fnameContour = fname + ".contour";
            if (!File.Exists(fnameContour))
                return;
            IContourPersisit persist = new ContourPersist();
            GeoDo.RSS.RasterTools.ContourPersist.enumCoordType coordType;
            string spatialRef;
            ContourLine.ContourEnvelope evp;
            ContourLine[] cntLines = persist.Read(fnameContour, out coordType, out evp, out spatialRef);
            layer.Apply(cntLines, true);
        }

        public override void Dispose()
        {
            if (_contourClassItems != null)
            {
                foreach (ContourClass it in _contourClassItems)
                    it.Dispose();
                _contourClassItems = null;
            }
            if (_grahpicPath != null)
            {
                _grahpicPath.Dispose();
                _grahpicPath = null;
            }
            if (_font != null)
            {
                _font.Dispose();
                _font = null;
            }
            if (_labelBrush != null)
            {
                _labelBrush.Dispose();
                _labelBrush = null;
            }
            if (_maskBrush != null)
            {
                _maskBrush.Dispose();
                _maskBrush = null;
            }
            if (_conflictor != null)
            {
                _conflictor.Dispose();
                _conflictor = null;
            }
            FreeLabelBuffers();
            if (_canvas != null)
            {
                _canvas.OnEnvelopeChanged -= _canvasSizeChanged;
                _canvasSizeChanged = null;
            }
            base.Dispose();
        }
    }
}
