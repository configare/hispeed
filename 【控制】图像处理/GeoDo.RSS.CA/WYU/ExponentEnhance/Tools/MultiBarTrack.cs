using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace GeoDo.RSS.CA
{
    public partial class MultiBarTrack : UserControl
    {
        public delegate void BarValueChangedHandler(object sender, int barIndex, double value, Point location);
        public delegate void BarValueChangedFinishedHandler(object sender, int barIndex, double value);

        private const int cstMarginAtLeftAndRight = 10;
        private const int cstTrackLineHeight = 6;
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
        private Color _trackLineColor = Color.Gray;
        private Brush _trackLineBrush = null;

        public MultiBarTrack()
        {
            InitializeComponent();
            SetStaticVasOfTrackBarItem();
            Load += new EventHandler(MultiBarTrack_Load);
            _trackLineBrush = new SolidBrush(_trackLineColor);
        }

        void MultiBarTrack_Load(object sender, EventArgs e)
        {
            float span = (Width - 2 * cstMarginAtLeftAndRight) / (_barItemCount + 1);
            for (int i = 0; i < _barItemCount; i++)
                _items.Add(new TrackBarItem((i + 1) * span,this));
            if (_items != null && _items.Count == 2)
            {
                _items[0].OnSelectedHandler += new EventHandler(MultiBarTrack_OnSelectedHandler);
                _items[1].OnSelectedHandler += new EventHandler(MultiBarTrack_OnSelectedHandler);
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
            TrackBarItem.TrackLineHeight = cstTrackLineHeight;
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
                if (value < 2 || value>5)
                    return;
                _barItemCount = value;
            }
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
                _trackLineBrush = new SolidBrush(_trackLineColor);
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

        public double GetValueAt(int barIndex)
        {
            if (_items == null || _items.Count==0 || barIndex<0 || barIndex>_items.Count -1)
                return 0;
            float left = _items[barIndex].Left - cstMarginAtLeftAndRight;
            float totalLen = Width - 2 * cstMarginAtLeftAndRight;
            float p = left / totalLen;
            return  _minEndPointValue + (_maxEndPointValue - _minEndPointValue) * p;
        }

        public double SetValueAt(int barIndex, double value)
        {
            if (_items == null || _items.Count == 0 || barIndex < 0 || barIndex > _items.Count - 1)
                goto EndLine;
            float totalLen = Width - 2 * cstMarginAtLeftAndRight;
            double totalValue = _maxEndPointValue - _minEndPointValue;
            double valuePerPixel = totalValue / totalLen;
            float pixelCount =(float) ((value - _minEndPointValue) / valuePerPixel);
            float newLeft = cstMarginAtLeftAndRight + pixelCount;
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
            _items[barIndex].Left = newLeft;
            Refresh();
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
            if (Width < cstMarginAtLeftAndRight * 3 || Height < cstTrackLineHeight * 3)
                return;
            DrawTrackLine(e.Graphics);
            if (_items != null)
                foreach (TrackBarItem it in _items)
                    it.Draw(e.Graphics);
        }

        private void DrawTrackLine(Graphics graphics)
        {
            lock (_trackLineBrush)
            {
                _trackLineRect.Location = new PointF(cstMarginAtLeftAndRight, Height / 2f - cstTrackLineHeight / 2f);
                _trackLineRect.Width = Width - cstMarginAtLeftAndRight * 2f;
                _trackLineRect.Height = cstTrackLineHeight;
                graphics.FillRectangle(_trackLineBrush, _trackLineRect.X, _trackLineRect.Y, _trackLineRect.Width, _trackLineRect.Height);
                graphics.DrawRectangle(Pens.Black, _trackLineRect.X, _trackLineRect.Y, _trackLineRect.Width, _trackLineRect.Height);
                graphics.DrawString(_minEndPointValue.ToString(), Font, Brushes.Black, cstMarginAtLeftAndRight, Height / 2 + cstTrackLineHeight);
                SizeF size = graphics.MeasureString(_maxEndPointValue.ToString(), Font);
                graphics.DrawString(_maxEndPointValue.ToString(), Font, Brushes.Black, Width - (int)size.Width, Height / 2 + cstTrackLineHeight);
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
                            if (e.X - _minSpan < it.Left && _items.IndexOf(it) <_items.IndexOf(_currentTrackBarItem))
                                return;
                        }
                    }
                }
                _currentTrackBarItem.Left = e.X;
                if(BarValueChanged != null)
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
            if (_currentTrackBarItem != null)
            {
                int idx = _items.IndexOf(_currentTrackBarItem);
                if (BarValueChangedFinished != null)
                    BarValueChangedFinished(this, idx, GetValueAt(idx));

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
        private const int cstTrackBarItemWidth = 16;
        private const int cstTrackBarItemHeight = 8;
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

        public TrackBarItem(float left, Control container):this(container)
        {    _left = left;
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

        public void Draw(Graphics g)
        {
            if (float.IsInfinity(_leftPercent))
                return;
            float leftP = Container.Width * _leftPercent;
            g.SmoothingMode = SmoothingMode.HighQuality;
            float top = Container.Height / 2f + TrackLineHeight / 2f;
            PointF[] pts = new PointF[3];
            float x = 0, y = 0;
            x = leftP - cstTrackBarItemWidth / 2f;
            y = top + cstTrackBarItemHeight;
            pts[0].X = x; pts[0].Y = y;
            //
            x = leftP;
            y = top;
            pts[1].X = x; pts[1].Y = y;
            //
            x = leftP + cstTrackBarItemWidth / 2f;
            y = top + cstTrackBarItemHeight;
            pts[2].X = x; pts[2].Y = y;
            //
            _bounds = new GraphicsPath();
            for (int i = 0; i < pts.Length - 1; i++)
                _bounds.AddLine(pts[i], pts[i+1]);
            _bounds.AddLine(pts[pts.Length - 1], pts[0]);
            //
            if (_selected)
                 g.FillPath(Brushes.White, _bounds);
            else
                g.FillPath(Brushes.Gray, _bounds);
            g.DrawPath(Pens.Black, _bounds);
        }

        public bool Contains(Point location)
        {
            if (_bounds == null)
                return false;
            return _bounds.IsVisible(location);
        }
    }
}
