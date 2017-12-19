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

namespace GeoDo.RSS.RasterTools
{
    public partial class UCScatterGraph : UserControl, IScatterPlotTool
    {
        private string _fileName;
        private int _marginLeft = 80;
        private int _marginRight = 30;
        private int _marginTop = 30;
        private int _marginBottom = 60;
        private int _scaleCount = 5;
        private int _minScaleLen = 8;
        private int _maxScaleLen = 16;
        private int _minorScaleCount = 5;
        private Color _fillColor = Color.Black;
        private Color _axisColor = Color.White;
        private Pen _axisPen = new Pen(Color.White);
        private SolidBrush _axisBrush = new SolidBrush(Color.White);
        //
        private IRasterDataProvider _dataProvider;
        private int _xBandNo, _yBandNo;
        private string _xBandName;
        private string _yBandName;
        private int[] _aoi;
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
        private IScatterPixelVisitor _visitor;
        private bool _isFirst = true;
        private Form _owner;
        private LinearFitObject _fitObj;
        //
        public event EventHandler LinearFitFinished;

        public UCScatterGraph()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Paint += new PaintEventHandler(UCScatterGraph_Paint);
            SizeChanged += new EventHandler(UCScatterGraph_SizeChanged);
            int radius = 1;
            _pointBitmap = new Bitmap(radius, radius, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(_pointBitmap))
            {
                g.FillRectangle(_axisBrush, 0, 0, radius, radius);
            }
            Load += new EventHandler(UCScatterGraph_Load);
        }

        void UCScatterGraph_Load(object sender, EventArgs e)
        {
            _owner = this.FindForm();
        }

        void UCScatterGraph_SizeChanged(object sender, EventArgs e)
        {
            _isFirst = true;
            TryAbortVisiting();
            if (_yBandMatrix != null)
                _yBandMatrix.Dispose();
            _yBandMatrix = null;
            _buffer = new byte[Width * Height];
            if (_bufferBitmap != null)
                _bufferBitmap.Dispose();
            _bufferBitmap = new Bitmap(this.Width, this.Height, PixelFormat.Format24bppRgb);
            Rerender();
        }

        private void TryAbortVisiting()
        {
            if (_visitor != null)
                _visitor.Abort();
        }

        public void AbortRendering()
        {
            TryAbortVisiting();
        }

        public void Reset(IRasterDataProvider dataProvider, int xBandNo, int yBandNo,int[] aoi, LinearFitObject fitObj, Action<int, string> progressTracker)
        {
            _fitObj = fitObj;
            _dataProvider = dataProvider;
            _xBandNo = xBandNo;
            _yBandNo = yBandNo;
            _fileName = _dataProvider.fileName;
            _xBandName = Path.GetFileName(_fileName) + " (Band " + _xBandNo.ToString() + ")";
            _yBandName = Path.GetFileName(_fileName) + " (Band " + _yBandNo.ToString() + ")";
            _aoi = aoi;
            //
            IMaxMinValueComputer c = MaxMinValueComputerFactory.GetMaxMinValueComputer(dataProvider.DataType);
            double[] minValues = new double[2];
            double[] maxValues = new double[2];
            double[] meanValues = new double[2];
            c.Compute(new IRasterBand[] { dataProvider.GetRasterBand(xBandNo), dataProvider.GetRasterBand(yBandNo) },aoi, out minValues, out maxValues, out meanValues, progressTracker);
            _minValueXBand = minValues[0];
            _maxValueXBand = maxValues[0];
            _minValueYBand = minValues[1];
            _maxValueYBand = maxValues[1];
            _oMinValueX = _minValueXBand;
            _oMaxValueX = _maxValueXBand;
            //
            _visitor = ScatterPixelVisitorFactory.GetVisitor(_dataProvider.DataType);
            _visitor.Init(_dataProvider.GetRasterBand(xBandNo), _dataProvider.GetRasterBand(yBandNo));
        }

        public void Reset(IRasterDataProvider dataProvider, int xBandNo, int yBandNo,double[] xBandInvalidValue,double[] yBandInvalidValue, int[] aoi, LinearFitObject fitObj, Action<int, string> progressTracker)
        {
            _fitObj = fitObj;
            _dataProvider = dataProvider;
            _xBandNo = xBandNo;
            _yBandNo = yBandNo;
            _fileName = _dataProvider.fileName;
            string[] filenames=_fileName.Split(new char[]{';'},StringSplitOptions.RemoveEmptyEntries);
            if (filenames.Length == 2)
            {
                _xBandName = Path.GetFileName(filenames[0]) + " (Band " + _xBandNo.ToString() + ")";
                _yBandName = Path.GetFileName(filenames[1]) + " (Band " + _yBandNo.ToString() + ")";
                _fileName = "";
            }
            _aoi = aoi;
            //
            IMaxMinValueComputer c = MaxMinValueComputerFactory.GetMaxMinValueComputer(dataProvider.DataType);
            double[] minValues = new double[2];
            double[] maxValues = new double[2];
            double[] meanValues = new double[2];
            c.Compute(new IRasterBand[] { dataProvider.GetRasterBand(xBandNo), dataProvider.GetRasterBand(yBandNo) },aoi,new double[][]{xBandInvalidValue,yBandInvalidValue},out minValues, out maxValues, out meanValues, progressTracker);
            _minValueXBand = minValues[0];
            _maxValueXBand = maxValues[0];
            _minValueYBand = minValues[1];
            _maxValueYBand = maxValues[1];
            _oMinValueX = _minValueXBand;
            _oMaxValueX = _maxValueXBand;
            //
            _visitor = ScatterPixelVisitorFactory.GetVisitor(_dataProvider.DataType);
            _visitor.Init(_dataProvider.GetRasterBand(xBandNo), _dataProvider.GetRasterBand(yBandNo));
        }

        public void Reset(IRasterDataProvider dataProvider, int xBandNo, int yBandNo,int[] aoi,XYAxisEndpointValue endpointValues, LinearFitObject fitObj, Action<int, string> progressTracker)
        {
            _fitObj = fitObj;
            _dataProvider = dataProvider;
            _xBandNo = xBandNo;
            _yBandNo = yBandNo;
            _fileName = _dataProvider.fileName;
            _xBandName = Path.GetFileName(_fileName) + " (Band " + _xBandNo.ToString() + ")";
            _yBandName = Path.GetFileName(_fileName) + " (Band " + _yBandNo.ToString() + ")";
            _aoi = aoi;
            //
            IMaxMinValueComputer c = MaxMinValueComputerFactory.GetMaxMinValueComputer(dataProvider.DataType);
            double[] minValues = new double[2];
            double[] maxValues = new double[2];
            double[] meanValues = new double[2];
            c.Compute(new IRasterBand[] { dataProvider.GetRasterBand(xBandNo), dataProvider.GetRasterBand(yBandNo) },aoi, out minValues, out maxValues, out meanValues, progressTracker);
            _minValueXBand = endpointValues.MinX;
            _maxValueXBand = endpointValues.MaxX;
            _minValueYBand = endpointValues.MinY;
            _maxValueYBand = endpointValues.MaxY;
            _oMinValueX = minValues[0];
            _oMaxValueX = maxValues[0];
            //
            _visitor = ScatterPixelVisitorFactory.GetVisitor(_dataProvider.DataType);
            _visitor.Init(_dataProvider.GetRasterBand(xBandNo), _dataProvider.GetRasterBand(yBandNo));
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
            if (_visitor == null)
                return;
            if (_owner != null)
                _owner.Text = "波段散点图...";
            Stopwatch sw = new Stopwatch();
            sw.Start();
            g.Clear(_fillColor);
            if (_isFirst)
            {
                DrawAxisAndBox(g);
                this.Refresh();
                _isFirst = false;
            }
            DrawPoints(g, _marginLeft, Width - _marginRight, Height - _marginBottom, _marginTop);
            DrawAxisAndBox(g);
            Console.WriteLine("Lost Time : " + sw.ElapsedMilliseconds.ToString());
        }

        private void DrawAxisAndBox(Graphics g)
        {
            SizeF fontSize = new SizeF();
            //
            float leftBandNameMargin = 22;
            //
            fontSize = g.MeasureString(_maxValueYBand.ToString("0.####"), Font);
            _marginLeft = (int)Math.Ceiling(Math.Max(fontSize.Width + leftBandNameMargin + _maxScaleLen, _marginLeft));
            //draw fileName
            float x;
            if (!string.IsNullOrEmpty(_fileName))
            {
                fontSize = g.MeasureString(_fileName, Font);
                x = _marginLeft + (Width - _marginLeft - _marginRight - fontSize.Width) / 2f;
                g.DrawString("文件 : " + _fileName, Font, _axisBrush, new PointF(x, 4));
            }
            //draw yBandName
            fontSize = g.MeasureString(_yBandName, Font);
            x = leftBandNameMargin;
            float y = _marginTop + ((Height - _marginTop - _marginBottom) - fontSize.Width) / 2f;
            if (_yBandMatrix == null)
            {
                _yBandMatrix = new Matrix();
                _yBandMatrix.RotateAt(90, new PointF(x, y));
            }
            Matrix oldMatrix = g.Transform;
            g.Transform = _yBandMatrix;
            g.DrawString(_yBandName, Font, _axisBrush, x, y);
            g.Transform = oldMatrix;
            //draw xBandName
            fontSize = g.MeasureString(_xBandName, Font);
            x = _marginLeft + (Width - _marginLeft - _marginRight - fontSize.Width) / 2f;
            g.DrawString(_xBandName, Font, _axisBrush, x, Height - fontSize.Height - 4);
            //draw axis
            g.DrawRectangle(_axisPen, _marginLeft, _marginTop, Width - _marginLeft - _marginRight, Height - _marginTop - _marginBottom);
            //draw y scales
            fontSize = g.MeasureString(_maxValueYBand.ToString("0.####"), Font);
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
                g.DrawLine(_axisPen, x, y, _marginLeft, y);
                stringV = (valueY.ToString("0.####") + " ").PadLeft(nCountChars);
                fontSize = g.MeasureString(stringV, Font);
                g.DrawString(stringV, Font, _axisBrush, x - fontSize.Width, y - fontSize.Height / 2f);
                if (i == _scaleCount)
                    break;
                y1 = y;
                for (int j = 0; j < _minorScaleCount; j++)
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
                fontSize = g.MeasureString(stringV, Font);
                g.DrawLine(_axisPen, x, y, x, y - _maxScaleLen);
                g.DrawString(stringV, Font, _axisBrush, x - fontSize.Width / 2f, Height - _marginBottom + _maxScaleLen + 2);
                if (i == _scaleCount)
                    break;
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

        private unsafe void DrawPoints(Graphics g, int x1, int x2, int y1, int y2)
        {
            int count = 0;
            int refreshCount = 10000;
            float xSpan = (x2 - x1) / (float)(_maxValueXBand - _minValueXBand);
            float ySpan = (y1 - y2) / (float)(_maxValueYBand - _minValueYBand);
            int x, y;
            int idx = 0;
            int[] rowIndexes = new int[this.Height];
            for (int i = 0; i < this.Height; i++)
                rowIndexes[i] = i * this.Width;
            //fit
            double Exy = 0, Ex = 0, Ey = 0, Ex2 = 0, Ey2 = 0;
            //int n = _dataProvider.Width * _dataProvider.Height;
            int n = 0;
            //
            _visitor.Visit(_aoi,
                            (xValue, yValue) =>
                            {
                                x = x1 + (int)((xValue - _minValueXBand) * xSpan);
                                y = y1 - (int)((yValue - _minValueYBand) * ySpan);
                                if (y < 0 || x < 0 || y >= this.Height || x >= this.Width)
                                    return;
                                //idx = y * Width + x;
                                idx = rowIndexes[y] + x;
                                if (_buffer[idx] == 0)
                                {
                                    g.DrawImageUnscaled(_pointBitmap, x, y);
                                    _buffer[idx] = 1;
                                }
                                n++;
                                //fit(最小平方法线性拟合)
                                if (_fitObj != null)
                                {
                                    Exy += (xValue * yValue);
                                    Ex += xValue;
                                    Ey += yValue;
                                    Ex2 += (xValue * xValue);
                                    Ey2 += (yValue * yValue);
                                }
                                //
                                count++;
                                if (count == refreshCount)
                                {
                                    count = 0;
                                    this.Refresh();
                                }
                            },
                            (percent, tip) =>
                            {
                                if (_owner != null)
                                    _owner.Text = "波段散点图...," + percent.ToString() + "%";
                            }
                );
            //fit
            if (_fitObj != null)
            {
                //y = a + bx
                _fitObj.b = (n * Exy - Ex * Ey) / (n * Ex2 - Ex * Ex);
                _fitObj.a = Ey / n - _fitObj.b * Ex / n;
                _fitObj.r2 = (Math.Pow((n * Exy - Ex * Ey), 2)) / ((n * Ex2 - Ex * Ex) * (n * Ey2 - Ey * Ey));
                DrawFitedLine(g, x1, y1, xSpan, ySpan);
                //
                if (LinearFitFinished != null)
                    LinearFitFinished(_fitObj, null);
            }
            //
            this.Invalidate();
        }


        private void DrawFitedLine(Graphics g, int x1, int y1, float xSpan, float ySpan)
        {
            double xValue1 = _oMinValueX;
            double xValue2 = _oMaxValueX;
            double newMinValueYBand = _fitObj.a + _fitObj.b * xValue1;
            double newMaxValueYBand = _fitObj.a + _fitObj.b * xValue2;
            int bx = 0, by = 0, ex = 0, ey = 0;
            bx = x1 + (int)((xValue1 - _minValueXBand) * xSpan);
            by = y1 - (int)((newMinValueYBand - _minValueYBand) * ySpan);
            ex = x1 + (int)((xValue2 - _minValueXBand) * xSpan);
            ey = y1 - (int)((newMaxValueYBand - _minValueYBand) * ySpan);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.DrawLine(Pens.Red, bx, by, ex, ey);
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
