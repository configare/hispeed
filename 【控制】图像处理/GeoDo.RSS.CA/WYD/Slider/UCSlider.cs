using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace GeoDo.RSS.CA
{
    public partial class UCSlider : UserControl
    {
        public delegate void BarValueChangedHandler(object sender);

        private int _width = 0;
        private int _height = 0;
        private int _wide = 0;
        private int _high = 0;
        private int _quantity = 1;
        private int[] _minValue;
        private int[] _maxValue;
        private float[] _value;
        private int _index = 0;
        private PointF _pointA;
        private PointF _pointB;
        private PointF _pointC;
        private float _gap;
        private int _l = 8;
        private int _h = 15;
        private List<TriControlPoint> _trianglePoints = new List<TriControlPoint>();
        private TriControlPoint _trianglePoint = new TriControlPoint();
        private PointF[] _pntArr = new PointF[3];
        private bool _isFirst = true;
        private PointF _pntMove;
        private bool _isFristDown = true;
        private GraphicsPath _path;
        private Pen pen;
        private Brush brush;
        private Rectangle sliderRectangle;
        private LinearGradientBrush sliderBrush;
        public event BarValueChangedHandler BarValueChanged = null;

        public UCSlider()
        {
            InitializeComponent();
            DoubleBuffered = true;
            pen = GetPenColor();
            brush = GetBrushColor();
        }

        [Category("自定义属性")]
        public float[] Value
        {
            get { return _value; }
            set { _value = value; }
        }

        [Category("自定义属性")]
        public int[] MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        [Category("自定义属性")]
        public int[] MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        private void UCSlider_Load_1(object sender, EventArgs e)
        {
            InitalizeVariables();
        }

        public void InitalizeVariables()
        {
            _width = this.Size.Width;
            _height = this.Size.Height;
            _wide = _width - 2 * _l;
            _high = _height - _h;
            sliderRectangle = new Rectangle(_l, 0, _wide, _high);
            sliderBrush = new LinearGradientBrush(sliderRectangle, Color.Black, Color.White, LinearGradientMode.Horizontal);
        }

        public void Display(int quantity)
        {
            _quantity = quantity;
            _value = new float[_quantity];
        }

        public void Display(int quantity, int[] minValue, int[] maxValue)
        {
            _quantity = quantity;
            _minValue = minValue;
            _maxValue = maxValue;
            _value = new float[_quantity];
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawSlider(e.Graphics);
        }

        private void DrawSlider(Graphics g)
        {
            if (_isFirst)
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                if (_quantity > 2)
                {
                    _gap = (float)_wide / (_quantity - 1);
                }
                if (_quantity == 2)
                {
                    _gap = (float)_wide;
                }
                for (int i = 0; i < _quantity; i++)
                {
                    _trianglePoint = new TriControlPoint();
                    _trianglePoint.Location = new PointF((_l + _gap * i), _high);
                    _trianglePoints.Add(_trianglePoint);
                }
                _pointB.Y = _h + _high;
                _pointC.Y = _high + _h;
                _trianglePoint = null;
                _isFirst = false;
            }
            g.DrawRectangle(pen, sliderRectangle);
            g.FillRectangle(sliderBrush, sliderRectangle);
            for (int i = 0; i < _quantity; i++)
            {
                _trianglePoint = _trianglePoints[i];
                _pointA = _trianglePoint.Location;
                _pointB.X = _pointA.X - _l;
                _pointC.X = _pointA.X + _l;
                _pntArr[0] = _pointA;
                _pntArr[1] = _pointB;
                _pntArr[2] = _pointC;
                _path = new GraphicsPath();
                _path.AddPolygon(_pntArr);
                _trianglePoint.Path = _path;
                g.FillPath(brush, _path);
            }
            _trianglePoint = null;
        }

        private Pen GetPenColor()
        {
            Pen pen = new Pen(Color.Black);
            return pen;
        }

        private Brush GetBrushColor()
        {
            Brush brush = Brushes.Black;
            return brush;
        }

        private void FirstSetValue()
        {
            for (int i = 0; i < _quantity; i++)
            {
                SetValue(i);
            }
        }

        private void SetValue(int index)
        {
            _value[index] = (_trianglePoints[index].Location.X - _l) * (_maxValue[index] - _minValue[index]) / (_wide) + _minValue[index];
            if (BarValueChanged != null)
                BarValueChanged(this);
        }

        private void SetValue(float svalue, int index)
        {
            _value[index] = svalue;
            //if (BarValueChanged != null)
            //    BarValueChanged(this);
        }

        public float GetValue(int index)
        {
            return _value[index];
        }

        public void SetSliderValue(float value, int index)
        {
            _trianglePoints[index].Location.X = ((value - _minValue[index]) / (_maxValue[index] - _minValue[index]) * _wide) + _l;
            _trianglePoints[index].Location.X = ControlSliderOver(index, _trianglePoints[index].Location);
            SetValue(value, index);
            Invalidate();
        }

        private float ControlSliderOver(int index, PointF p)
        {
            if (index < (_quantity - 1) && p.X >= _trianglePoints[index + 1].Location.X)
            {
                return (_trianglePoints[index + 1].Location.X - 1);
            }
            else if (index > 0 && p.X <= _trianglePoints[index - 1].Location.X)
            {
                return (_trianglePoints[index - 1].Location.X + 1);
            }
            else
            {
                return p.X;
            }
        }

        private void UCSlider_MouseDown(object sender, MouseEventArgs e)
        {
            if (_isFristDown)
            {
                FirstSetValue();
                _isFristDown = false;
            }
            _pntMove = Point.Empty;
            for (int i = 0; i < _quantity; i++)
            {
                if (_trianglePoints[i].Path.IsVisible(e.X, e.Y))
                {
                    _pntMove = _trianglePoints[i].Location;
                    _index = i;
                    return;
                }
            }
        }

        private void UCSlider_MouseUp(object sender, MouseEventArgs e)
        {
            if (_pntMove != Point.Empty)
                _pntMove = Point.Empty;
        }

        private void UCSlider_MouseMove(object sender, MouseEventArgs e)
        {
            if (_pntMove != Point.Empty)
            {
                _pntMove.X = e.X;
                if (e.X < _l)
                {
                    _pntMove.X = _l;
                }
                else if (e.X > (_width - _l))
                {
                    _pntMove.X = _width - _l;
                }
                _trianglePoints[_index].Location.X = ControlSliderOver(_index, _pntMove);
                SetValue(_index);
                Invalidate();
                return;
            }
        }
    }

    public class TriControlPoint
    {
        public PointF Location;
        public GraphicsPath Path;
    }
}
