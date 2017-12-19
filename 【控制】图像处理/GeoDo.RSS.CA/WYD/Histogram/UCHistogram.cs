using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.CA
{
    /*
     * 直方图控件 ——by魏亚东
     * 直方图控件使用方法：
     * 1、将控件拖入窗口，通过Size属性调节大小
     * 2、调用直方图控件的Display方法修改其属性
    */


    /// <summary>
    /// 直方图控件
    /// </summary>
    public partial class UCHistogram : UserControl
    {
        private int _length = 0;//控件高   @@
        private int _width = 0; //控件宽   @@
        private bool _isShowInfo = true;//是否显示辅助信息   @@
        private float _pointX = 0;//矩形起始点横坐标
        private float _pointY = 0;//矩形起始点纵坐标
        private float _high = 120;//直方图高度
        private float _wide = 200;//直方图宽度
        private float _oneHigh = 0;//矩形高度
        private float _oneWide = 0;//矩形宽度
        private int _toplen = 10;//直方图到控件上方的距离
        private int _bottomlen = 15;//直方图到控件底部的距离
        private int _leftlen = 10;//直方图到控件左边的距离
        private int _rightlen = 10;//直方图到控件右边的距离
        private int _maxValue = 0;//最大值
        private int _count = 0;//直方图矩形数量   
        private int _step = 1;//对数组统计的步长   @@
        private int[] _value = new int[0];//传入数组 @@
        private Color _color = Color.Black;//颜色
        private int _beginValue = 0; //起始值   @@  
        private int _endValue = 255; //结束值   @@
        private float _highMaxValue = 0; //高度最大值   @@
        private int _highValue = 0;//纵坐标的值   @@
        private int _wideValue = 0;//横坐标的值   @@
        private int _mouseX = 0;//鼠标位置
        private int _mouseY = 0;//鼠标位置
        private int _keduchang = 5;//每个刻度线的长度   @@

        public UCHistogram()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        [Category("自定义属性")]
        public int Step
        {
            get { return _step; }
            set { _step = value; }
        }

        [Category("自定义属性")]
        public bool IsShowInfo
        {
            get { return _isShowInfo; }
            set { _isShowInfo = value; }
        }

        [Category("自定义属性")]
        public int Keduchang
        {
            get { return _keduchang; }
            set { _keduchang = value; }
        }

        [Category("自定义属性")]
        public int BeginValue
        {
            get { return _beginValue; }
            set { _beginValue = value; }
        }

        [Category("自定义属性")]
        public int EndValue
        {
            get { return _endValue; }
            set { _endValue = value; }
        }

        public float HighMaxValue
        {
            get { return _highMaxValue; }
        }

        public int HighValue
        {
            get { return _highValue; }
        }

        public int WideValue
        {
            get { return _wideValue; }
        }

        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public int histWidth
        {
            get { return _width; }
            set { _width = value; }
        }

        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        public int[] Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private void UCHistogram_Load(object sender, EventArgs e)
        {
            Init(); 
        }

        private void Init()
        {
            _width = this.Size.Width;
            _length = this.Size.Height;
            _wide = _width - _leftlen - _rightlen;
            _high = _length - _toplen - _bottomlen;
        }

        /// <summary>
        /// 直方图显示方法
        /// </summary>
        /// <param name="histogram"></param>
        public void Display(Int32[] histogram)
        {
            Display(histogram, Color.Black);
        }

        /// <summary>
        /// 直方图显示方法
        /// </summary>
        /// <param name="histogram"></param>
        /// <param name="ench"></param>
        public void Display(Int32[] histogram, Color color)
        {
            _value = histogram;
            _count = histogram.Count();
            _color = color;
            GetMaxValue();
            _highMaxValue = _maxValue;
            _endValue = histogram.Count() - 1;
            _beginValue = 0;
            _wide = _width - _leftlen - _rightlen;
            _high = _length - _toplen - _bottomlen;
            Invalidate();
        }

        /// <summary>
        /// 直方图显示方法
        /// </summary>
        /// <param name="histogram"></param>
        /// <param name="beginValue"></param>
        /// <param name="endValue"></param>
        public void Display(Int32[] histogram, int beginValue, int endValue)
        {
            Display(histogram, beginValue, endValue, Color.Black);
        }

        public void Display(Int32[] histogram, int step, int beginValue, int endValue)
        {
            _step = step;
            if (_step == 0)
            {
                _step = 1;
            }
            Display(histogram, beginValue, endValue, Color.Black);
        }

        /// <summary>
        /// 直方图显示方法
        /// </summary>
        /// <param name="histogram">数组</param>
        /// <param name="beginValue">最小值</param>
        /// <param name="endValue">最大值</param>
        /// <param name="ench">通道</param>
        public void Display(Int32[] histogram, int beginValue, int endValue, Color color)
        {
            _value = histogram;
            if (_step == 0)
            {
                _step = 1;
            }
            _count = (endValue - beginValue + 1) / _step;
            _color = color;
            GetMaxValue();
            _highMaxValue = _maxValue;
            _endValue = endValue;
            _beginValue = beginValue;
            _wide = _width - _leftlen - _rightlen;
            _high = _length - _toplen - _bottomlen;
            Invalidate();
        }

        /// <summary>
        /// 可以选取步长的直方图显示方法
        /// </summary>
        /// <param name="histogram"></param>
        /// <param name="step"></param>
        /// <param name="beginValue"></param>
        /// <param name="endValue"></param>
        /// <param name="enumchannel"></param>
        public void Display(Int32[] histogram, int step, int beginValue, int endValue, Color color)
        {
            _value = histogram;
            _step = step;
            if (_step == 0)
            {
                _step = 1;
            }
            _count = (endValue - beginValue + 1) / step;
            _color = color;
            GetMaxValue();
            _highMaxValue = _maxValue;
            _endValue = endValue;
            _beginValue = beginValue;
            _wide = _width - _leftlen - _rightlen;
            _high = _length - _toplen - _bottomlen;
            Invalidate();
        }

        /// <summary>
        /// 获取数组中的最大值
        /// </summary>
        private int GetMaxValue()
        {
            for (int i = 0; i < _count; i++)
            {
                if (i > 0)
                {
                    _maxValue = Math.Max(_maxValue, _value[i * _step + _beginValue]);
                }
                else
                {
                    _maxValue = _value[0];
                }
            }
            return _maxValue;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            _highMaxValue = GetMaxValue();
            DrawHistogram(e.Graphics);
            DrawCalibration(e.Graphics);
            DrawString(e.Graphics);
        }

        /// <summary>
        /// 画直方图
        /// </summary>
        /// <param name="g"></param>
        private void DrawHistogram(Graphics g)
        {
            if (_step < 1)
            {
                _step = 1;
            }
            else if (_step > _count)
            {
                _step = _count;
            }
            _highMaxValue = GetMaxValue();
            if ((_highMaxValue != 0) && (_count != 0))
            {
                _oneWide = (_wide) / (float)_count;
                Pen pen = GetPenColor(_color);
                for (int i = 0; i < _count; i++)
                {
                    int valuei = _value[i * _step + _beginValue];
                    _pointX = _oneWide * i + _leftlen;
                    _oneHigh = (((valuei * 100) / _highMaxValue) * _high) / 100;
                    _pointY = _high - _oneHigh + _toplen;
                    DrawRectangle(g, pen, _pointX, _pointY, _oneWide, _oneHigh);
                }
            }
        }

        /// <summary>
        /// 填充颜色
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        private Pen GetPenColor(Color color)
        {
            Pen pen = null;
            Brush brush = new SolidBrush(color);
            pen = new Pen(brush, _oneWide);
            return pen;
        }

        private Pen GetPenColor()
        {
            Pen pen = new Pen(Color.Black);
            return pen;
        }

        /// <summary>
        /// 画矩形
        /// </summary>
        /// <param name="g"></param>
        /// <param name="pen"></param>
        /// <param name="pointX"></param>
        /// <param name="pointY"></param>
        /// <param name="width"></param>
        /// <param name="hight"></param>
        private void DrawRectangle(Graphics g, Pen pen, float pointX, float pointY, float width, float hight)
        {
            g.DrawRectangle(pen, pointX, pointY, width, hight);
        }

        /// <summary>
        /// 画刻度
        /// </summary>
        /// <param name="g"></param>
        private void DrawCalibration(Graphics g)
        {
            Pen pen = GetPenColor();
            for (int i = 0; i <= 10; i++)
            {
                float xwide = _wide / 10;
                g.DrawLine(pen, xwide * i + _leftlen, _high + _toplen, xwide * i + _leftlen, _high + _toplen + _keduchang);
            }
            for (int i = 0; i <= 5; i++)
            {
                float xhigt = _high / 5;
                g.DrawLine(pen, _leftlen, xhigt * i + _toplen, _leftlen - _keduchang, xhigt * i + _toplen);
            }
            g.DrawRectangle(pen, _leftlen, _toplen, _wide, _high);
            Pen p = new Pen(Color.Yellow);
            if (_isShowInfo && (_mouseX >= _leftlen) && (_mouseX <= _leftlen + _wide) && _mouseY >= _toplen && _mouseY <= _toplen + _high)
            {
                g.DrawLine(p, _mouseX, _toplen, _mouseX, _toplen + _high);
                g.DrawLine(p, _leftlen, _mouseY, _leftlen + _wide, _mouseY);
            }
        }

        /// <summary>
        /// 画字符串
        /// </summary>
        /// <param name="g"></param>
        private void DrawString(Graphics g)
        {
            if (_isShowInfo)
            {
                Font font = new System.Drawing.Font(DefaultFont, FontStyle.Regular);
                g.DrawString(_highMaxValue.ToString(), font, new SolidBrush(Color.Black), _leftlen, 0);
                g.DrawString(_beginValue.ToString(), font, new SolidBrush(Color.Black), _leftlen, _toplen + _high + _keduchang);
                g.DrawString(_endValue.ToString(), font, new SolidBrush(Color.Black), _wide, _toplen + _high + _keduchang);
                g.DrawString("(" + _wideValue.ToString() + ":" + _highValue.ToString() + ")", font, new SolidBrush(Color.Black), _leftlen + _wide / 2, 0);
            }
        }

        /// <summary>
        /// 获取鼠标信息
        /// </summary>
        /// <param name="wideValue"></param>
        public void GetMouseInfo(int wideValue, int highValue)
        {
            if ((wideValue >= _leftlen) && (wideValue <= _leftlen + _wide) && highValue >= _toplen && highValue <= _toplen + _high)
            {
                _wideValue = Math.Max(_beginValue, Math.Min(_endValue, (int)((float)((wideValue - _leftlen) / _oneWide) * _step + _beginValue)));
                _highValue = _value[_wideValue];
                _mouseX = wideValue;
                _mouseY = highValue;
                Invalidate();
            }
        }

        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UCHistogram_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isShowInfo)
            {
                GetMouseInfo(e.X, e.Y);
            }
        }
    }
}
