using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace GeoDo.RSS.MIF.UI
{
    public partial class MultiBarTrack : UserControl
    {
        public delegate void BarValueChangedHandler(object sender, int barIndex, double value, Point location);
        public delegate void BarValueChangedFinishedHandler(object sender, int barIndex, double value);
        public enum enumValidPartion
        {
            LeftSegment,
            RightSegment,
            MiddleSegment,
            SinglePoint
        }
        private enumValidPartion _validPartion = enumValidPartion.MiddleSegment;
        private const int cstMarginAtLeftAndRight = 10;
        private int _trackLineHeight = 8;
        private int _minSpan = 6;
        private List<TrackBarItem> _items = new List<TrackBarItem>();
        private RectangleF _trackLineRect = new RectangleF();
        private int _barItemCount = 2;
        private bool _isDraging = false;
        private TrackBarItem _currentTrackBarItem = null;
        private Point _startPoint = Point.Empty;
        private double _minEndPointValue = 0;
        private double _maxEndPointValue = 100;
        public event BarValueChangedHandler BarValueChanged = null;
        public event BarValueChangedFinishedHandler BarValueChangedFinished = null;
        private Pen _borderPen = new Pen(Color.Black);
        private Color _trackLineColor = Color.FromArgb(153, 180, 209);
        private Color _trackLineBorderColor = Color.Black;
        private Brush _trackLineBrush = new SolidBrush(Color.FromArgb(153, 180, 209));
        private Brush _trackBarBrush = new SolidBrush(Color.FromArgb(128, 153, 180, 209));
        private Brush _selectedTrackBarBrush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));
        private Color _validSegmentColor = Color.FromArgb(255, 192, 192);
        private Brush _validSegmentBrush = new SolidBrush(Color.FromArgb(255, 192, 192));
        private int _topPading = 2;

        public MultiBarTrack()
        {
            InitializeComponent();
            SetStaticVasOfTrackBarItem();

            Load += new EventHandler(MultiBarTrack_Load);
            _trackLineBrush = new SolidBrush(_trackLineColor);
            _trackBarBrush = new SolidBrush(Color.FromArgb(128, 153, 180, 209));
        }

        void MultiBarTrack_Load(object sender, EventArgs e)
        {
        }

        private void ReloadItem()
        {
            if (_items.Count != 0)
            {
                foreach (TrackBarItem it in _items)
                {
                    it.OnSelectedHandler -= new EventHandler(MultiBarTrack_OnSelectedHandler);
                }
                _items.Clear();
            }
            float span = (Width - 2 * cstMarginAtLeftAndRight) / (_barItemCount + 1);
            for (int i = 0; i < _barItemCount; i++)
                _items.Add(new TrackBarItem((i + 1) * span, this));
            if (_items != null)
            {
                foreach (TrackBarItem it in _items)
                {
                    it.OnSelectedHandler += new EventHandler(MultiBarTrack_OnSelectedHandler);
                }
            }
        }

        private bool _isOverLeftBar = false;
        private bool _isOverRightBar = false;
        void MultiBarTrack_OnSelectedHandler(object sender, EventArgs e)
        {
            _isOverLeftBar = false;
            _isOverRightBar = false;
            if (_items[0].Equals(sender))
                _isOverLeftBar = true;
            else if (_items[1].Equals(sender))
                _isOverRightBar = true;
        }

        public bool IsOverLeftBar
        {
            get { return _isOverLeftBar; }
        }

        public bool IsOverRightBar
        {
            get { return _isOverRightBar; }
        }

        private void SetStaticVasOfTrackBarItem()
        {
            TrackBarItem.MarginAtLeftAndRight = cstMarginAtLeftAndRight;
            TrackBarItem.TrackLineHeight = _trackLineHeight;
        }

        protected override void OnResize(EventArgs e)
        {
            Invalidate();
        }

        [Category("自定义属性")]
        public int BarItemCount
        {
            get { return _barItemCount; }
            set
            {
                if (value < 1)
                    value = 1;
                if (value > 2)
                    value = 2;
                _barItemCount = value;
                if (_barItemCount == 2)
                    _validPartion = enumValidPartion.MiddleSegment;
                else
                    _validPartion = enumValidPartion.RightSegment;
            }
        }

        [Category("自定义属性")]
        public int TrackLineHeight
        {
            get { return _trackLineHeight; }
            set { _trackLineHeight = value; }
        }

        [Category("自定义属性")]
        public double MinEndPointValue
        {
            get { return _minEndPointValue; }
            set
            {
                _minEndPointValue = value;

            }
        }

        [Category("自定义属性")]
        public double MaxEndPointValue
        {
            get { return _maxEndPointValue; }
            set
            {
                _maxEndPointValue = value;
            }
        }

        [Category("自定义属性")]
        public Color TrackLineColor
        {
            get { return _trackLineColor; }
            set
            {
                _trackLineColor = value;
                if (_trackLineBrush != null)
                    _trackLineBrush.Dispose();
                _trackLineBrush = new SolidBrush(_trackLineColor);
                if (_trackBarBrush != null)
                    _trackBarBrush.Dispose();
                _trackBarBrush = new SolidBrush(Color.FromArgb(128, value.R, value.G, value.B));
            }
        }

        [Category("自定义属性")]
        public Color TrackLineBorderColor
        {
            get { return _trackLineBorderColor; }
            set
            {
                if (_borderPen != null)
                    _borderPen.Dispose();
                _trackLineBorderColor = value;
                _borderPen = new Pen(_trackLineBorderColor);
            }
        }

        [Category("自定义属性")]
        public Color ValidSegmentColor
        {
            get { return _validSegmentColor; }
            set
            {
                _validSegmentColor = value;
                if (_validSegmentBrush != null)
                    _validSegmentBrush.Dispose();
                _validSegmentBrush = new SolidBrush(_validSegmentColor);
            }
        }

        [Category("自定义属性")]
        public int MinSpan
        {
            get { return _minSpan; }
            set
            {
                if (value < 0 && value > 10)
                    return;
                _minSpan = value;
            }
        }

        [Category("自定义属性")]
        public enumValidPartion ValidPartion
        {
            get { return _validPartion; }
            set { _validPartion = value; }
        }

        public double GetValueAt(int barIndex)
        {
            if (_items == null || _items.Count == 0 || barIndex < 0 || barIndex > _items.Count - 1)
                return 0;
            float left = _items[barIndex].Left - cstMarginAtLeftAndRight;
            float totalLen = Width - 2 * cstMarginAtLeftAndRight;
            float p = left / totalLen;
            return _minEndPointValue + (_maxEndPointValue - _minEndPointValue) * p;
        }

        public void SetValues(double[] values)
        {
            ReloadItem();
            if (_items.Count == 0 || values == null || _items.Count != values.Length)
                return;
            float totalLen = Width - 2 * cstMarginAtLeftAndRight;
            double totalValue = _maxEndPointValue - _minEndPointValue;
            double valuePerPixel = totalValue / totalLen;
            for (int i = 0; i < values.Length; i++)
            {
                if (_items[i] == null)
                    continue;
                float pixelCount = (float)((values[i] - _minEndPointValue) / valuePerPixel);
                _items[i].Left = cstMarginAtLeftAndRight + pixelCount;
            }
            Invalidate();
        }

        public double SetValueAt(int barIndex, double value)
        {
            if (_items == null || _items.Count == 0 || barIndex < 0)
                goto EndLine;
            float totalLen = Width - 2 * cstMarginAtLeftAndRight;
            double totalValue = _maxEndPointValue - _minEndPointValue;
            double valuePerPixel = totalValue / totalLen;
            float pixelCount = (float)((value - _minEndPointValue) / valuePerPixel);
            float newLeft = cstMarginAtLeftAndRight + pixelCount;
            if (_items.Count == 1)
            {
                if (newLeft > Width - cstMarginAtLeftAndRight)
                    newLeft = Width - cstMarginAtLeftAndRight;
                else if (newLeft < cstMarginAtLeftAndRight)
                    newLeft = cstMarginAtLeftAndRight;
            }
            if (_items.Count >= 2)
            {
                if (barIndex == _items.Count - 1)
                {
                    if (newLeft > Width - cstMarginAtLeftAndRight)
                        newLeft = Width - cstMarginAtLeftAndRight;
                    else if (newLeft < _items[barIndex - 1].Left + _minSpan)
                        newLeft = _items[barIndex - 1].Left + _minSpan;
                }
                else if (barIndex == 0)
                {
                    if (newLeft < cstMarginAtLeftAndRight)
                        newLeft = cstMarginAtLeftAndRight;
                    else if (newLeft > _items[barIndex + 1].Left - _minSpan)
                        newLeft = _items[barIndex + 1].Left - _minSpan;
                }
                else
                {
                    if (newLeft > _items[barIndex + 1].Left)
                        newLeft = _items[barIndex + 1].Left - _minSpan;
                    else if (newLeft < _items[barIndex - 1].Left)
                        newLeft = _items[barIndex - 1].Left + _minSpan;
                }
            }
            _items[barIndex].Left = newLeft;
            Invalidate();
        EndLine:
            double v = GetValueAt(barIndex);
            if (BarValueChanged != null)
                BarValueChanged(this, barIndex, v, Point.Empty);
            if (BarValueChangedFinished != null)
                BarValueChangedFinished(this, barIndex, v);
            return v;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < cstMarginAtLeftAndRight * 3 || Height < _trackLineHeight * 3)
                return;
            DrawTrackLine(e.Graphics);
            if (_items != null)
                foreach (TrackBarItem it in _items)
                    it.Draw(e.Graphics, _trackBarBrush, _selectedTrackBarBrush, _borderPen);
        }

        private void DrawValidSegment(Graphics graphics)
        {
            float x1 = 0;
            //float y = Height / 2f - _trackLineHeight / 2f;
            float y = (20 - _trackLineHeight) / 2f + _topPading;
            float x2 = 0;
            switch (_validPartion)
            {
                case enumValidPartion.LeftSegment:
                    if (_items.Count != 1)
                        return;
                    x1 = cstMarginAtLeftAndRight;
                    x2 = _items[0].Left - cstMarginAtLeftAndRight;
                    break;
                case enumValidPartion.RightSegment:
                    if (_items.Count != 1)
                        return;
                    x1 = _items[0].Left;
                    x2 = Width - cstMarginAtLeftAndRight - x1;
                    break;
                case enumValidPartion.MiddleSegment:
                    if (_items.Count != 2)
                        return;
                    x1 = _items[0].Left;
                    x2 = _items[1].Left - x1;
                    break;
                case enumValidPartion.SinglePoint:
                    x1 = _items[0].Left;
                    x2 = _items[0].Left + 4;
                    return;
                    break;
            }
            if (float.IsNaN(_items[0].Left) || float.IsInfinity(_items[0].Left))
                return;
            graphics.FillRectangle(_validSegmentBrush, x1, y, x2, _trackLineHeight);
        }

        private void DrawTrackLine(Graphics graphics)
        {
            lock (_trackLineBrush)
            {
                //_trackLineRect.Location = new PointF(cstMarginAtLeftAndRight, Height / 2f - _trackLineHeight / 2f);
                _trackLineRect.Location = new PointF(cstMarginAtLeftAndRight, (20 - _trackLineHeight) / 2f + _topPading);
                _trackLineRect.Width = Width - cstMarginAtLeftAndRight * 2f;
                _trackLineRect.Height = _trackLineHeight;
                graphics.FillRectangle(_trackLineBrush, _trackLineRect.X, _trackLineRect.Y, _trackLineRect.Width, _trackLineRect.Height);
                DrawValidSegment(graphics);
                graphics.DrawRectangle(_borderPen, _trackLineRect.X, _trackLineRect.Y, _trackLineRect.Width, _trackLineRect.Height);
                //graphics.DrawString(_minEndPointValue.ToString(), Font, Brushes.Black, cstMarginAtLeftAndRight, Height/ 2f + _trackLineHeight);
                graphics.DrawString(_minEndPointValue.ToString(), Font, Brushes.Black, cstMarginAtLeftAndRight, _trackLineHeight * 2 + _topPading);
                SizeF size = graphics.MeasureString(_maxEndPointValue.ToString(), Font);
                //graphics.DrawString(_maxEndPointValue.ToString(), Font, Brushes.Black, Width - (int)size.Width, Height / 2f + _trackLineHeight);
                graphics.DrawString(_maxEndPointValue.ToString(), Font, Brushes.Black, Width - (int)size.Width, _trackLineHeight * 2 + _topPading);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!_isDraging)
            {
                foreach (TrackBarItem it in _items)
                {
                    if (it.Contains(e.Location))
                    {
                        _currentTrackBarItem = it;
                        _currentTrackBarItem.Selected = true;
                        _isDraging = true;
                        _startPoint = e.Location;
                        Invalidate();
                        break;
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_isDraging && _currentTrackBarItem != null)
            {
                if (e.X < _trackLineRect.X || e.X > _trackLineRect.X + _trackLineRect.Width)
                    return;
                foreach (TrackBarItem it in _items)
                {
                    if (!it.Equals(_currentTrackBarItem))
                    {
                        if (_startPoint.X < e.X)
                        {
                            if (e.X + _minSpan > it.Left && _items.IndexOf(it) > _items.IndexOf(_currentTrackBarItem))
                                return;
                        }
                        else
                        {
                            if (e.X - _minSpan < it.Left && _items.IndexOf(it) < _items.IndexOf(_currentTrackBarItem))
                                return;
                        }
                    }
                }
                _currentTrackBarItem.Left = e.X;
                if (BarValueChanged != null)
                {
                    int idx = _items.IndexOf(_currentTrackBarItem);
                    BarValueChanged(this, idx, GetValueAt(idx), e.Location);
                }
                Invalidate();
            }
            else
            {
                foreach (TrackBarItem it in _items)
                    it.Selected = false;
                foreach (TrackBarItem it in _items)
                {
                    if (it.Contains(e.Location))
                    {
                        it.Selected = true;
                        break;
                    }
                }
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_isDraging && _currentTrackBarItem != null)
            {
                int idx = _items.IndexOf(_currentTrackBarItem);
                if (BarValueChangedFinished != null)
                {
                    _isDraging = false;
                    BarValueChangedFinished(this, idx, GetValueAt(idx));
                }
                if (_currentTrackBarItem != null)
                {
                    _currentTrackBarItem.Selected = false;
                }
                _currentTrackBarItem = null;
            }
            _isDraging = false;
            _startPoint = Point.Empty;
            Invalidate();
        }
    }

    [Serializable]
    internal class TrackBarItem
    {
        private Control Container = null;
        internal static int TrackLineHeight = 6;
        internal static int MarginAtLeftAndRight = 4;
        private const int cstTrackBarItemWidth = 10;
        private const int cstTrackBarItemHeight = 20;
        private float _left = 0;
        private float _leftPercent = 0;
        [NonSerialized]
        private GraphicsPath _bounds = null;
        private bool _selected = false;
        public event EventHandler OnSelectedHandler;

        public TrackBarItem(Control container)
        {
            Container = container;
        }

        public TrackBarItem(float left, Control container)
            : this(container)
        {
            _left = left;
            _leftPercent = _left / Container.Width;
        }

        internal bool Selected
        {
            set
            {
                _selected = value;
                if (value)
                    if (OnSelectedHandler != null)
                        OnSelectedHandler(this, null);
            }
        }

        public float Left
        {
            get
            {
                //return _left; 
                return _leftPercent * Container.Width;
            }
            set
            {
                _left = value;
                _leftPercent = _left / Container.Width;
            }
        }

        public void Draw(Graphics g, Brush brush, Brush selectedBrush, Pen borderPen)
        {
            if (float.IsInfinity(_leftPercent))
                return;
            float leftP = Container.Width * _leftPercent;
            g.SmoothingMode = SmoothingMode.HighQuality;
            //
            float x = leftP - cstTrackBarItemWidth / 2f;
            float y = 2; //Container.Height / 2f - cstTrackBarItemHeight / 2f; //由居中对齐改为Top对齐
            RectangleF rect = new RectangleF(x, y, cstTrackBarItemWidth, cstTrackBarItemHeight);
            if (_bounds == null)
                _bounds = new GraphicsPath();
            else
                _bounds.Reset();
            _bounds.AddRectangle(rect);
            if (_selected)
                g.FillPath(selectedBrush, _bounds);
            else
                g.FillPath(brush, _bounds);
            g.DrawPath(borderPen, _bounds);
        }

        public bool Contains(Point location)
        {
            if (_bounds == null)
                return false;
            return _bounds.IsVisible(location);
        }
    }
}
