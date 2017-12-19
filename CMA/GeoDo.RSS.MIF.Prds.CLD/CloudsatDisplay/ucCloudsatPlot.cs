using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.DF;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Diagnostics;
using GeoDo.MathAlg;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public partial class ucCloudsatPlot : UserControl
    {
        private string _fileName;
        private int _marginLeft = 80;
        private int _marginRight = 30;
        private int _marginTop = 30;
        private int _marginBottom = 60;
        private int _scaleCount = 10;
        private int _minScaleLen = 8;
        private int _maxScaleLen = 16;
        private int _minorScaleCount = 5;
        private Color _fillColor = Color.White;
        private Color _axisColor = Color.Black;
        private Pen _axisPen = new Pen(Color.Black);
        private SolidBrush _axisBrush = new SolidBrush(Color.Black);
        private Font _font = new Font("微软雅黑", 12);
        private Font _fontLabel = new Font("微软雅黑", 10);
        //
        //private IRasterDataProvider _dataProvider;
        //private int _xBandNo, _yBandNo;
        private string _xBandName = "";
        private string _yBandName = "";
        //private int[] _aoi;
        //
        private Matrix _yBandMatrix = null;
        //
        internal double _minValueXBand;
        internal double _maxValueXBand;
        internal double _minValueYBand;
        internal double _maxValueYBand;
        private double _oMinValueX;
        private double _oMaxValueX;
        //
        private Bitmap _pointBitmap;
        private byte[] _buffer;
        private Bitmap _bufferBitmap;
        //
        private bool _rendering = false;
        private bool _isFirst = true;
        private Form _owner;

        public ucCloudsatPlot()
        {
            InitializeComponent(); 
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲

            Paint += new PaintEventHandler(UCScatterGraph_Paint);
            SizeChanged += new EventHandler(UCScatterGraph_SizeChanged);
            int radius = 1;
            _pointBitmap = new Bitmap(radius, radius, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(_pointBitmap))
            {
                g.FillRectangle(_axisBrush, 0, 0, radius, radius);
            }
            Load += new EventHandler(UCScatterGraph_Load);
            Disposed += new EventHandler(ucCloudsatPlot_Disposed);
        }

        void ucCloudsatPlot_Disposed(object sender, EventArgs e)
        {
            if (_font != null)
                _font.Dispose();
            if (_fontLabel != null)
                _fontLabel.Dispose();
        }

        void UCScatterGraph_Load(object sender, EventArgs e)
        {
            _owner = this.FindForm();
        }

        void UCScatterGraph_SizeChanged(object sender, EventArgs e)
        {
            RefreshAll();
        }

        private void RefreshAll()
        {
            _isFirst = true;
            if (_yBandMatrix != null)
                _yBandMatrix.Dispose();
            _yBandMatrix = null;
            _buffer = new byte[Width * Height];
            //if (_bufferBitmap != null)
            //    _bufferBitmap.Dispose();
            _bufferBitmap = new Bitmap(this.Width, this.Height, PixelFormat.Format24bppRgb);
            Rerender();
        }

        private Bitmap _img = null;
        //private float[] _lats = null;
        //private float[] _longs = null;
        private string[] _datetimes = null;

        public void Reset(string filename, Bitmap img, double minx, double maxx, double miny, double maxy, Action<int, string> progressTracker)
        {
            _img = img;
            _fileName = Path.GetFileNameWithoutExtension(filename);
            _xBandName = "";
            _yBandName = "海波高度(km)";//Altitude
            //
            _minValueXBand = minx;
            _maxValueXBand = maxx;
            _minValueYBand = miny;
            _maxValueYBand = maxy;
            _oMinValueX = minx;
            _oMaxValueX = maxx;
            RefreshAll();
        }

        public void AddYAxis(string[] yaxis)
        {
            //_lats = lats;
            //_longs = longs;
            _datetimes = yaxis;
        }

        public void SetEndPointValues(double minValueX, double maxValueX, double minValueY, double maxValueY)
        {
            _minValueXBand = minValueX;
            _maxValueXBand = maxValueX;
            _minValueYBand = minValueY;
            _maxValueYBand = maxValueY;
            Rerender();
        }

        public void ExportBitmap(string fName)
        {
            if (_bufferBitmap == null)
                return;
            _bufferBitmap.Save(fName, ImageFormat.Bmp);
        }

        void UCScatterGraph_Paint(object sender, PaintEventArgs e)
        {
            if (_bufferBitmap == null)
                return;
            e.Graphics.DrawImage(_bufferBitmap, 0, 0);
        }

        public void Rerender()
        {
            if (_rendering)
                return;
            _rendering = true;
            using (Graphics g = Graphics.FromImage(_bufferBitmap))
            {
                Render(g);
            }
            _rendering = false;
        }

        private void Render(Graphics g)
        {
            if (_owner != null)
                _owner.Text = "CloudSat profile plots";
            Stopwatch sw = new Stopwatch();
            sw.Start();
            g.Clear(_fillColor);
            if (_isFirst)
            {
                DrawAxisAndBox(g);
                //this.Refresh();
                _isFirst = false;
            }
            DrawImg(g, _marginLeft, _marginTop, Width - _marginLeft - _marginRight, Height - _marginTop - _marginBottom);
            DrawAxisAndBox(g);
            Console.WriteLine("Lost Time : " + sw.ElapsedMilliseconds.ToString());
        }

        private void DrawAxisAndBox(Graphics g)
        {
            SizeF fontSize = new SizeF();
            SizeF lableSize;
            //
            float leftBandNameMargin = 12;
            float rightBandNameMargin = 10;
            //
            fontSize = g.MeasureString(_maxValueYBand.ToString("0.####"), _fontLabel);
            lableSize = g.MeasureString(_yBandName, _fontLabel);
            _marginLeft = (int)Math.Ceiling(Math.Max(fontSize.Width + leftBandNameMargin + rightBandNameMargin + _maxScaleLen + lableSize.Height, _marginLeft));
            //draw fileName
            fontSize = g.MeasureString(_fileName, _font);
            float x = _marginLeft + (Width - _marginLeft - _marginRight - fontSize.Width) / 2f;
            g.DrawString(_fileName, _font, _axisBrush, new PointF(x, 4));
            //draw yBandName
            x = leftBandNameMargin;
            float y = _marginTop + ((Height - _marginTop - _marginBottom) + lableSize.Width) / 2f;
            if (_yBandMatrix == null)
            {
                _yBandMatrix = new Matrix();
                _yBandMatrix.RotateAt(-90, new PointF(x, y));
            }
            Matrix oldMatrix = g.Transform;
            g.Transform = _yBandMatrix;
            g.DrawString(_yBandName, _fontLabel, _axisBrush, x, y);
            g.Transform = oldMatrix;
            //draw xBandName
            fontSize = g.MeasureString(_xBandName, _fontLabel);
            x = _marginLeft + (Width - _marginLeft - _marginRight - fontSize.Width) / 2f;
            g.DrawString(_xBandName, _fontLabel, _axisBrush, x, Height - fontSize.Height - 4);
            //draw axis
            g.DrawRectangle(_axisPen, _marginLeft, _marginTop, Width - _marginLeft - _marginRight, Height - _marginTop - _marginBottom);
            //draw y scales
            fontSize = g.MeasureString(_maxValueYBand.ToString("0.####"), _fontLabel);
            double diffY = (_maxValueYBand - _minValueYBand) / _scaleCount;
            float ySpan = (Height - _marginTop - _marginBottom) / (float)_scaleCount;
            float y1Span = ySpan / _minorScaleCount;
            y = Height - _marginBottom;
            float y1 = 0;

            double valueY = _minValueYBand;
            int nCountChars = GetNCountChars(valueY, diffY, _scaleCount) + 1;
            string stringV;
            for (int i = 0; i <= _scaleCount; i++)
            {
                x = _marginLeft - _maxScaleLen;
                g.DrawLine(_axisPen, x, y, _marginLeft, y);//长线
                stringV = (valueY.ToString("0.####") + " ").PadLeft(nCountChars);
                fontSize = g.MeasureString(stringV, _fontLabel);
                g.DrawString(stringV, _fontLabel, _axisBrush, x - fontSize.Width, y - fontSize.Height / 2f);
                if (i == _scaleCount)
                    break;
                y1 = y;
                for (int j = 0; j < _minorScaleCount; j++)//短线
                {
                    g.DrawLine(_axisPen, _marginLeft - _minScaleLen, y1, _marginLeft, y1);
                    y1 -= y1Span;
                }
                y -= ySpan;
                valueY += diffY;
            }
            //draw x scales
            double diffX = (_maxValueXBand - _minValueXBand) / _scaleCount;
            float xSpan = (Width - _marginLeft - _marginRight) / (float)_scaleCount;
            float x1Span = xSpan / _minorScaleCount;
            x = _marginLeft;
            float x1 = 0;
            double valueX = _minValueXBand;
            y = Height - _marginBottom + _maxScaleLen;
            y1 = Height - _marginBottom + _minScaleLen;
            for (int i = 0; i <= _scaleCount; i++)
            {
                stringV = valueX.ToString("0.####");
                fontSize = g.MeasureString(stringV, _fontLabel);
                g.DrawLine(_axisPen, x, y, x, y - _maxScaleLen);
                g.DrawString(stringV, _fontLabel, _axisBrush, x - fontSize.Width / 2f, Height - _marginBottom + _maxScaleLen + 2);
                if (i == _scaleCount)
                    break;
                if (_datetimes != null)//绘制Y轴
                {
                    stringV = _datetimes[i];
                    fontSize = g.MeasureString(stringV, _fontLabel);
                    g.DrawString(stringV, _fontLabel, _axisBrush, x - fontSize.Width / 2f, Height - _marginBottom + _maxScaleLen + 2 + fontSize.Height);
                }
                x1 = x;
                for (int j = 0; j < _minorScaleCount; j++)
                {
                    g.DrawLine(_axisPen, x1, y1, x1, y1 - _minScaleLen);
                    x1 += x1Span;
                }
                x += xSpan;
                valueX += diffX;
            }
        }

        private void DrawImg(Graphics g, int x, int y, int w, int h)
        {
            if (_img != null)
            {
                g.DrawImage(_img, new Rectangle(x, y, w, h));
                this.Invalidate();
            }
        }

        private int GetNCountChars(double valueY, double diffY, int scaleCount)
        {
            int n = 0;
            for (int i = 0; i <= _scaleCount; i++)
            {
                if (valueY.ToString().Length > n)
                    n = (valueY.ToString() + " ").Length;
                valueY += diffY;
            }
            return n;
        }
    }
}
