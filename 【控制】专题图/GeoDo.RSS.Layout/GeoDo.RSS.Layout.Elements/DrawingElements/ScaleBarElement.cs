using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GeoDo.RSS.Layout.Elements
{
    /// <summary>
    /// 修改日期：2014年2月11日
    /// 修改日：罗战克
    /// 修改内容：
    /// 修改比例尺计算错误，添加注释说明
    /// 修改比例尺整体放大缩小时候数值计算错误
    /// 修改比例尺整体放大缩小时候，不需要重新生成比例尺Bitmap缓存
    /// </summary>
    [Export(typeof(IElement)), Category("比例尺条")]
    public class ScaleBarElement : SizableElement, IPersitable, IDisposable
    {
        protected static Font _font = new Font("宋体", 9);
        protected string _unitNameString = "KM";
        protected Color _backColor = Color.Black;
        private float _scale = 1000000; //当前比例尺1:_scale
        private int[] _parts = new int[] { 1, 2, 4 };
        private const int BLANK_STRING = 5;  //标注的空白处大小
        private float _maxLength = 4; //centimeter
        private Brush _brush = null;
        private Pen _pen = null;
        private SizeF _preSize = SizeF.Empty;//前次缓存图像的大小（专题图放大倍数为1:1时候的大小）
        private float _perScale = 0;        //前次比例尺，判断是否需要重新生成缓存的Bitmap
        private Bitmap _scaleBarBmpCache = null;
        private int _count = 0;//需要绘制的矩形的个数

        public ScaleBarElement()
            : base()
        {
            Init();
        }

        public ScaleBarElement(PointF location)
            : base()
        {
            _location = location;
            Init();
        }

        public ScaleBarElement(int[] parts)
        {
            _parts = parts;
            Init();
        }

        private void Init()
        {
            _name = "比例尺条";
            _icon = ImageGetter.GetImageByName("IconScaleBar.png");
            _size = new SizeF(190, 40);
            if (_backColor != null)
            {
                _brush = new SolidBrush(_backColor);
                _pen = new Pen(_backColor);
            }
            else
            {
                _brush = new SolidBrush(Color.Black);
                _pen = new Pen(Color.Black);
            }
            if (_parts != null && _parts.Length != 0)
                _count = _parts.Length;
        }

        #region attributes
        [Persist(enumAttType.UnValueType), DisplayName("填充颜色"), Category("外观")]//"背景色"
        public Color Color
        {
            get { return _backColor; }
            set
            {
                if (_backColor != value)
                {
                    _backColor = value;
                    _preSize = SizeF.Empty;
                }
            }
        }

        [Persist(enumAttType.UnValueType), DisplayName("比例尺分段"), Category("数据")]
        public int[] Parts
        {
            get { return _parts; }
            set
            {
                if (_parts != value)
                {
                    SetPartsValue(value);
                    _count = _parts.Length;
                    _preSize = SizeF.Empty;
                }
            }
        }

        private void SetPartsValue(int[] value)
        {
            for (int i = 0; i < value.Length - 1; i++)
            {
                if (value[i + 1] <= value[i])
                    return;
            }
        }

        [Persist(enumAttType.UnValueType), DisplayName("标注文本字体"), Category("外观")]
        public Font Font
        {
            get { return _font; }
            set
            {
                if (_font != value)
                {
                    _font = value;
                    _preSize = SizeF.Empty;
                }
            }
        }
        #endregion

        public override void Render(object sender, IDrawArgs drawArgs)
        {
            Graphics g = drawArgs.Graphics as Graphics;
            if (g == null)
                return;
            if (_parts == null || _parts.Length == 0)
                return;
            ILayoutRuntime runtime = sender as ILayoutRuntime;
            if (runtime == null)
                return;
            float s = GetDataFrameScale(runtime);
            if (s > 0)
                _scale = s;
            if (float.IsNaN(_scale) || _scale < float.Epsilon)
                return;
            BeginRotate(drawArgs);
            CreatBitmap(drawArgs);
            if (_scaleBarBmpCache == null)
                return;
            try
            {
                float x = _location.X, y = _location.Y;
                drawArgs.Runtime.Layout2Screen(ref x, ref y);
                float scale = runtime.Scale;
                int newWidth = (int)(_size.Width * scale + 0.1f);
                int newHeight = (int)(_size.Height * scale + 0.1f);
                if (newWidth < 1 || newHeight < 1 || newWidth > 10000 || newHeight > 10000)
                    return;
                using (Bitmap bm = new Bitmap((int)_size.Width, (int)_size.Height))
                {
                    using (Graphics gsp = Graphics.FromImage(bm))
                    {
                        gsp.DrawImage(_scaleBarBmpCache, new Rectangle(0, 0, bm.Width, bm.Height));
                    }
                    g.DrawImage(bm, x, y, newWidth, newHeight);
                }
            }
            finally
            {
                EndRotate(drawArgs);
            }
        }

        private void CreatBitmap(IDrawArgs drawArgs)
        {
            if (_scale != _perScale || _perScale == 0 || _preSize != _size)
            {
                if (_scaleBarBmpCache != null)
                    _scaleBarBmpCache.Dispose();
                _scaleBarBmpCache = GetScaleBarBitmap(drawArgs, _size.Width, _size.Height);
                _perScale = _scale;
                _preSize = _size;
            }
        }

        float[] _locx = new float[10];  //locations
        float[] _wid = new float[10];   //widths
        int[] _flag = new int[10];   //texts
        private Bitmap GetScaleBarBitmap(IDrawArgs drawArgs, float w, float h)
        {
            if (w <= 9 || h <= 9)
                return null;
            Bitmap bmp = new Bitmap((int)w, (int)h);
            using (Graphics gps = Graphics.FromImage(bmp))
            {
                int s = 100000;
                if (_scale < 100000)
                    s = 1;
                float txtWid = gps.MeasureString(_unitNameString, _font).Width;
                _maxLength = w - txtWid - BLANK_STRING; //drawArgs.Runtime.Pixel2Centimeter((int)(w - txtWid));
                //绘制矩形比例尺条
                _locx[0] = 0;
                float hei = h - BLANK_STRING - _font.Height;
                float barH = hei + BLANK_STRING;
                gps.DrawString("0", _font, Brushes.Black, 0, barH);
                for (int i = 0; i < _count; i++)
                {
                    if (i != 0)
                        _locx[i] = _locx[0] + _wid[i - 1];
                    _wid[i] = _parts[i] * _maxLength / (float)_parts[_count - 1]; //pixel
                    _flag[i] = (int)Math.Round(_scale * (_wid[i] * 2.54 / gps.DpiX) / s);//2014.2.11修改比例尺计算错误
                    if (i == 0)
                    {
                        gps.DrawRectangle(_pen, _locx[i], 0, _wid[i], hei);
                        gps.FillRectangle(_brush, _locx[i], 0, _wid[i], hei);
                        //写标注
                        gps.DrawString(_flag[i].ToString(), _font, _brush, _locx[i] + _wid[i] - BLANK_STRING * 3, barH);
                    }
                    else
                    {
                        gps.DrawRectangle(_pen, _locx[i], 0, _wid[i] - _wid[i - 1], hei);
                        if (i % 2 == 0)
                            gps.FillRectangle(_brush, _locx[i], 0, _wid[i] - _wid[i - 1], hei);
                        //写标注
                        gps.DrawString(_flag[i].ToString(), _font, _brush, _locx[i] + _wid[i] - _wid[i - 1] - BLANK_STRING * 3, barH);
                    }
                }
                //写单位
                PointF unitPoint = new PointF(_locx[0] + _wid[_count - 1] + BLANK_STRING, 0);
                gps.DrawString(_unitNameString, _font, Brushes.Black, unitPoint);
            }
            return bmp;
        }

        private float GetDataFrameScale(ILayoutRuntime runtime)
        {
            ILayoutHost host = runtime.GetHost();
            if (host == null)
                return -1;
            if (host.ActiveDataFrame == null)
                return -1;
            return host.ActiveDataFrame.LayoutScale;
        }

        public override void InitByXml(System.Xml.Linq.XElement xml)
        {
            if (xml.Attribute("color") != null)
                _backColor = (Color)LayoutFromFile.Base64StringToObject(xml.Attribute("color").Value);
            if (xml.Attribute("parts") != null)
                _parts = (int[])LayoutFromFile.Base64StringToObject(xml.Attribute("parts").Value);
            if (xml.Attribute("font") != null)
                _font = (Font)LayoutFromFile.Base64StringToObject(xml.Attribute("font").Value);
            base.InitByXml(xml);
        }

        public override void Dispose()
        {
            if (_pen != null)
            {
                _pen.Dispose();
                _pen = null;
            }
            if (_brush != null)
            {
                _brush.Dispose();
                _brush = null;
            }
            if (_scaleBarBmpCache != null)
            {
                _scaleBarBmpCache.Dispose();
                _scaleBarBmpCache = null;
            }
            base.Dispose();
        }
    }
}
