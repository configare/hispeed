using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace GeoDo.RSS.RasterTools
{
    public partial class UCHistogramGrapCanvas : UserControl
    {
        private Dictionary<int, RasterQuickStatResult> _results;
        private Dictionary<int, Pen> _curvePens;
        private Dictionary<int, Brush> _legendBurshes;
        private int[] _selectedBandNos;
        private string _fileName;
        private int _marginLeft = 80;
        private int _marginRight = 100;
        private int _marginTop = 30;
        private int _marginBottom = 36;
        private int _vScaleCount = 6;//直方图纵坐标轴频率分段数
        private int _hScaleCount = 10;//直方图横坐标轴分段数
        private int _minScaleLen = 6;
        private int _maxScaleLen = 12;
        private int _minorScaleCount = 5;
        private int _LegendWidth = 12;//图例
        private Color _fillColor = Color.Black;
        private Color _axisColor = Color.White;
        private Pen _axisPen = new Pen(Color.White);
        private SolidBrush _axisBrush = new SolidBrush(Color.White);
        private double _minValue, _maxValue;
        private double _minCount, _maxCount;

        public UCHistogramGrapCanvas()
        {
            InitializeComponent();
            DoubleBuffered = true;
            SizeChanged += new EventHandler(UCHistogramGrapCanvas_SizeChanged);
            Paint += new PaintEventHandler(UCHistogramGrapCanvas_Paint);
        }

        public double MinValue
        {
            get { return _minValue; }
        }

        public double MaxValue
        {
            get { return _maxValue; }
        }

        public double MinCount
        {
            get { return _minCount; }
        }

        public double MaxCount
        {
            get { return _maxCount; }
        }

        public void SetValues(double minValue, double maxValue, double minCount, double maxCount)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _minCount = minCount;
            _maxCount = maxCount;
        }

        private void SetAxisPen()
        {
            if (_axisPen != null)
            {
                _axisPen.Dispose();
                _axisBrush.Dispose();
            }
            _axisPen = new Pen(_axisColor);
            _axisBrush = new SolidBrush(_axisColor);
        }

        void UCHistogramGrapCanvas_SizeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        public void ChangeBand(int bandNo)
        {
            if (bandNo == -1)
            {
                _selectedBandNos = new int[_results.Count];
                int i = 0;
                foreach (int b in _results.Keys)
                {
                    _selectedBandNos[i++] = b;
                }
            }
            else
            {
                _selectedBandNos = new int[] { bandNo };
            }
            Invalidate();
        }

        public void Apply(string fileName, Dictionary<int, RasterQuickStatResult> results)
        {
            _curvePens = new Dictionary<int, Pen>();
            _legendBurshes = new Dictionary<int, Brush>();
            _fileName = fileName;
            _results = results;
            _minValue = double.MaxValue;
            _maxValue = double.MinValue;
            double minCount1, maxCount1;
            _minCount = double.MaxValue;
            _maxCount = double.MinValue;
            foreach (int bandNo in results.Keys)
            {
                RasterQuickStatResult restul = results[bandNo];
                if (restul.MinValue < _minValue)
                    _minValue = restul.MinValue;
                if (restul.MaxValue > _maxValue)
                    _maxValue = restul.MaxValue;
                HistogramResult histResult = restul.HistogramResult;
                minCount1 = histResult.Items.Min();
                maxCount1 = histResult.Items.Max();
                if (minCount1 < _minCount)
                    _minCount = minCount1;
                if (maxCount1 > _maxCount)
                    _maxCount = maxCount1;
                //
                _curvePens.Add(bandNo, new Pen(GetRandomColor()));
                _legendBurshes.Add(bandNo, new SolidBrush(_curvePens[bandNo].Color));
            }
            //
            ChangeBand(-1);
        }

        public void SaveAsFile(string fName)
        {
            using (Bitmap bm = new Bitmap(Width, Height, PixelFormat.Format24bppRgb))
            {
                using (Graphics g = Graphics.FromImage(bm))
                {
                    UCHistogramGrapCanvas_Paint(null, new PaintEventArgs(g, new Rectangle(0, 0, Width, Height)));
                }
                bm.Save(fName, ImageFormat.Bmp);
            }
        }

        void UCHistogramGrapCanvas_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(_fillColor);
            //
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //
            SizeF fontSize = e.Graphics.MeasureString(_maxCount.ToString(), Font);
            int nCountChars = _maxCount.ToString().Length;
            _marginLeft = (int)Math.Ceiling(fontSize.Width) + 2;
            //draw fileName
            e.Graphics.DrawString("文件 : " + _fileName, Font, _axisBrush, new PointF(_marginLeft + 2, 4));
            //draw axises
            e.Graphics.DrawRectangle(_axisPen, _marginLeft, _marginTop, this.Width - _marginLeft - _marginRight, this.Height - _marginTop - _marginBottom);
            //draw v axis
            double diffValue = (MaxCount - _minCount) / (float)_vScaleCount;//直方图坐标轴频率分段间隔
            double v;
            float y = Height - _marginBottom;
            float y1;
            float dltY = fontSize.Height / 2f;
            float ySpan = (Height - _marginTop - _marginBottom) / (float)_vScaleCount;//隔ySpan绘制频率值
            float y1Span = ySpan / _minorScaleCount;//隔y1Span绘制格线
            float x = 0;
            for (int i = 0; i <= _vScaleCount; i++)
            {
                v = (int)(_minCount + diffValue * i);
                e.Graphics.DrawString(v.ToString().PadLeft(nCountChars), Font, _axisBrush, new PointF(x, y - dltY));
                e.Graphics.DrawLine(_axisPen, _marginLeft, y, _marginLeft + _maxScaleLen, y);
                if (i == _vScaleCount)
                    break;
                y1 = y;
                for (int j = 0; j < _minorScaleCount; j++)
                {
                    e.Graphics.DrawLine(_axisPen, _marginLeft, y1, _marginLeft + _minScaleLen, y1);
                    y1 -= y1Span;
                }
                y -= ySpan;
            }
            //draw h axis
            diffValue = (MaxValue - MinValue) / (float)_hScaleCount;
            x = _marginLeft;
            float x1 = x;
            y = Height - _marginBottom;
            float xSpan = (Width - _marginLeft - _marginRight) / (float)_hScaleCount;
            float x1Span = xSpan / _minorScaleCount;
            string vString;
            for (int i = 0; i <= _hScaleCount; i++)
            {
                v = (_minValue + diffValue * i);
                vString = v.ToString("0.####");
                e.Graphics.DrawLine(_axisPen, x, y, x, y - _maxScaleLen);
                fontSize = e.Graphics.MeasureString(vString, Font);
                e.Graphics.DrawString(vString, Font, _axisBrush, x - fontSize.Width / 2f, y + 2);
                if (i == _hScaleCount)
                    break;
                x1 = x;
                for (int j = 0; j < _minorScaleCount; j++)
                {
                    e.Graphics.DrawLine(_axisPen, x1, y, x1, y - _minScaleLen);
                    x1 += x1Span;
                }
                x += xSpan;
            }
            //draw curve，绘制曲线
            if (_selectedBandNos == null || _selectedBandNos.Length == 0)
                return;            
            double bValue;
            diffValue = (MaxValue - MinValue) / (Width - _marginLeft - _marginRight);//直方图水平方向的分辨率倒数（间隔多点一个单位）
            double diffCount = (MaxCount - MinCount) / (Height - _marginTop - _marginBottom);//直方图垂直方向的分辨率倒数
            if (diffValue == 0d || diffCount == 0d)
            { }
            else
            {
                foreach (int bandNo in _selectedBandNos)
                {
                    RasterQuickStatResult result = _results[bandNo];
                    HistogramResult hist = result.HistogramResult;
                    int vCount = hist.ActualBuckets;
                    //zyb，20131119新加
                    if (vCount <=1 )
                        continue;
                    PointF[] pts = new PointF[vCount];
                    for (int i = 0; i < vCount; i++)
                    {
                        bValue = _minValue + i * hist.Bin;
                        x = (float)(_marginLeft + (bValue - _minValue) / diffValue);
                        y = (float)(Height - _marginBottom - hist.Items[i] / diffCount);
                        pts[i] = new PointF(x, y);
                    }
                    e.Graphics.DrawCurve(_curvePens[bandNo], pts, 0f);
                }
            }
            //draw lengends,绘制图例
            x = Width - _marginRight + 20;
            y = _marginTop;
            foreach (int bandNo in _selectedBandNos)
            {
                e.Graphics.FillRectangle(_legendBurshes[bandNo], x, y, _LegendWidth, _LegendWidth);
                e.Graphics.DrawString("Band " + bandNo.ToString(), Font, _legendBurshes[bandNo], x + _LegendWidth + 2, y);
                y += (fontSize.Height + 4);
            }
        }

        Random _random = new Random();
        private Color GetRandomColor()
        {
            return Color.FromArgb((byte)_random.Next(0, 255), (byte)_random.Next(0, 255), (byte)_random.Next(0, 255));
        }
    }
}
