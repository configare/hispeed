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
    public partial class PointsControl : UserControl
    {
        Point lastMouseXY = new Point(int.MinValue, int.MinValue); 
        private int lastKey = -1;
        private int lastValue = -1;
        private bool tracking = false;

        public PointsControl()
            : this(3, 256)
        {
        }

        protected internal PointsControl(int channels, int entries)
        {
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            this.channels = channels;
            this.entries = entries;

            InitializeComponent();
        }

        private Point[] _controlPoints;
        public Point[] ControlPoints
        {
            get { return _controlPoints; }
            set
            {
                _controlPoints = value;
                Invalidate();
            }
        }

        protected int channels;
        public int Channels
        {
            get
            {
                return this.channels;
            }
        }

        protected int entries;
        public int Entries
        {
            get
            {
                return entries;
            }
        }

        protected Color[] visualColors;
        public Color GetVisualColor(int channel)
        {
            return visualColors[channel];
        }

        protected string[] channelNames;
        public string GetChannelName(int channel)
        {
            return channelNames[channel];
        }

        protected bool[] mask;
        public void SetSelected(int channel, bool val)
        {
            mask[channel] = val;
            Invalidate();
        }

        public bool GetSelected(int channel)
        {
            return mask[channel];
        }

        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler<Point> CoordinatesChanged;
        protected virtual void OnCoordinatesChanged()
        {
            if (CoordinatesChanged != null)
            {
                CoordinatesChanged(this, new EventArgs<Point>(new Point(lastKey, lastValue)));
            }
        }
        
        public void ResetControlPoints()
        {
            _controlPoints = new Point[Channels];

            for (int i = 0; i < Channels; ++i)
            {
                _controlPoints[i] = new Point(0, 0);
            }

            Invalidate();
            OnValueChanged();
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            DrawToGraphics(e.Graphics);
            base.OnPaint(e);
        }

        private void DrawToGraphics(Graphics g)
        {
            ColorBgra colorSolid = ColorBgra.FromColor(this.ForeColor);
            ColorBgra colorGuide = ColorBgra.FromColor(this.ForeColor);
            ColorBgra colorGrid = ColorBgra.FromColor(this.ForeColor);

            colorGrid.A = 128;
            colorGuide.A = 96;

            Pen penSolid = new Pen(colorSolid.ToColor(), 1);
            Pen penGrid = new Pen(colorGrid.ToColor(), 1);
            Pen penGuide = new Pen(colorGuide.ToColor(), 1);

            penGrid.DashStyle = DashStyle.Dash;

            g.Clear(this.BackColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            ////绘制底图
            //GrawActiveDrawing(g);

            Rectangle ourRect = ClientRectangle;
            ourRect.Inflate(-1, -1);
            //辅助线_格网线
            for (float f = 0.25f; f <= 0.75f; f += 0.25f)
            {
                float x = Utility.Lerp(ourRect.Left, ourRect.Right, f);
                float y = Utility.Lerp(ourRect.Top, ourRect.Bottom, f);

                g.DrawLine(penGrid,
                    Point.Round(new PointF(x, ourRect.Top)),
                    Point.Round(new PointF(x, ourRect.Bottom)));

                g.DrawLine(penGrid,
                    Point.Round(new PointF(ourRect.Left, y)),
                    Point.Round(new PointF(ourRect.Right, y)));
            }
            //辅助线_对角线
            g.DrawLine(penGrid, ourRect.Left, ourRect.Bottom, ourRect.Right, ourRect.Top);

            float width = this.ClientRectangle.Width;
            float height = this.ClientRectangle.Height;
            //绘制控制点
            for (int c = 0; c < channels; ++c)
            {
                Point pt = _controlPoints[c];
                Color color = GetVisualColor(c);
                //Color colorSelected = ColorBgra.Blend(ColorBgra.FromColor(color), ColorBgra.White, 128).ToColor();

                const float penWidthNonSelected = 1;
                const float penWidthSelected = 2;
                float penWidth = mask[c] ? penWidthSelected : penWidthNonSelected;
                Pen penSelected = new Pen(color, penWidth);

                //color.A = 128;

                Pen pen = new Pen(color, penWidth);
                Brush brush = new SolidBrush(color);
                SolidBrush brushSelected = new SolidBrush(Color.White);


                int k = pt.X;
                float x = k * (width - 1) / (entries - 1);
                float y = (entries - 1 - pt.Y) * (height - 1) / (entries - 1);

                const float radiusSelected = 4;
                const float radiusNotSelected = 3;
                const float radiusUnMasked = 2;

                bool selected = mask[c];//当前控制点

                float size = selected ? radiusSelected : (mask[c] ? radiusNotSelected : radiusUnMasked);
                RectangleF rect = Utility.RectangleFromCenter(new PointF(x, y), size);

                g.FillEllipse(selected ? brushSelected : brush, rect.X, rect.Y, rect.Width, rect.Height);
                g.DrawEllipse(selected ? penSelected : pen, rect.X, rect.Y, rect.Width, rect.Height);

                pen.Dispose();
            }
            penSolid.Dispose();
            penGrid.Dispose();
            penGuide.Dispose();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            float width = this.ClientRectangle.Width;
            float height = this.ClientRectangle.Height;
            int mx = (int)Utility.Clamp(0.5f + e.X * (entries - 1) / (width - 1), 0, Entries - 1);
            int my = (int)Utility.Clamp(0.5f + Entries - 1 - e.Y * (entries - 1) / (height - 1), 0, Entries - 1);

            if (0 != e.Button)
            {
                tracking = (e.Button == MouseButtons.Left);
                lastKey = mx;

                for (int c = 0; c < channels; ++c)
                {
                    Point channelControlPoints = _controlPoints[c];
                    if (mask[c])
                    {
                        OnValueChanged();
                    }
                }
            }

            OnMouseMove(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            float width = this.ClientRectangle.Width;
            float height = this.ClientRectangle.Height;
            int mx = (int)Utility.Clamp(0.5f + e.X * (entries - 1) / (width - 1), 0, Entries - 1);
            int my = (int)Utility.Clamp(0.5f + Entries - 1 - e.Y * (entries - 1) / (height - 1), 0, Entries - 1);

            Invalidate();

            if (tracking && e.Button == MouseButtons.None)
            {
                tracking = false;
            }
            if (tracking)
            {
                bool changed = false;
                for (int c = 0; c < channels; ++c)
                {
                    Point channelControlPoints = _controlPoints[c];
                    if (mask[c])
                    {
                        if (mx >= 0 && mx < Entries)
                        {
                            int newValue = Utility.Clamp(my, 0, Entries - 1);

                            if (_controlPoints[c].Y != newValue || _controlPoints[c].X != mx)
                            {
                                _controlPoints[c].X = mx;
                                _controlPoints[c].Y = newValue;
                                changed = true;
                            }
                        }
                    }
                }
                if (changed)
                {
                    Update();
                    OnValueChanged();
                }
            }
            else
            {
                Update();
            }

            lastKey = mx;
            lastValue = my;
            OnCoordinatesChanged();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (0 != (e.Button & MouseButtons.Left) && tracking)
            {
                tracking = false;
                lastKey = -1;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            lastKey = -1;
            lastValue = -1;
            lastMouseXY = new Point(int.MinValue, int.MinValue);
            Invalidate();
            OnCoordinatesChanged();

            base.OnMouseLeave(e);
        }

        #region 底图
        private Bitmap _activeDrawing;

        internal void UpdateActiveDrawing(Bitmap ActiveDrawing)
        {
            _activeDrawing = ActiveDrawing;
            Invalidate();
        }

        private void GrawActiveDrawing(Graphics g)
        {
            if (_activeDrawing != null)
            {
                float width = this.ClientRectangle.Width;
                float height = this.ClientRectangle.Height;
                g.DrawImage(_activeDrawing, 1, 1, width, height);
            }
        }
        #endregion
    }
}
