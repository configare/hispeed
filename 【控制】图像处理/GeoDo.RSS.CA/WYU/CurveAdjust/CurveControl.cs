using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using GeoDo.RSS.Core.CA;
using System.Diagnostics;

namespace GeoDo.RSS.CA
{
    public partial class CurveControl : UserControl
    {
        const int OFFSET = 20;
        const int GRAYLINE_WIDTH = 10;
        const int BLANK = 1;
        const int POINT_RADIUS = 5;
        private IInterpolator _interpolator = null;
        private Rectangle _workRect = Rectangle.Empty;//网格区矩形
        private Color _colorSel = Color.Black;//所选的通道颜色
        private Pen _penGrid = null;
        private Pen _penSolid = null;
        private Pen _penLine = null;
        [NonSerialized]
        private List<PixelControlPoint> _pixelControlPoints = new List<PixelControlPoint>();
        [NonSerialized]
        private PixelControlPoint _currentPixelPoint = null;
        public event EventHandler ControlPointChanged = null;

        public CurveControl()
        {
            InitializeComponent();
            Load += new EventHandler(CurveControl_Load);
            DoubleBuffered = true;
            InitPenReg();
        }

        public PixelControlPoint CurrentPixelPoint
        {
            get { return _currentPixelPoint; }
        }

        public Color ColorSel
        {
            get { return _colorSel; }
            set { _colorSel = value; }
        }

        public List<PixelControlPoint> PixelControlPoints
        {
            get { return _pixelControlPoints; }
            set { _pixelControlPoints = value; }
        }

        void CurveControl_Load(object sender, EventArgs e)
        {
            InitControlPointList();
        }

        private void InitPenReg()
        {
            _penGrid = new Pen(Color.Gray, 1);//绘制网格
            _penSolid = new Pen(ForeColor, 1);//绘制矩形中实线
            _penGrid.DashStyle = DashStyle.Dash;
            _penGrid.Width = 0.5f;
            _penSolid.DashStyle = DashStyle.Solid;
            _penSolid.Width = 1.5f;
        }

        private void InitControlPointList()
        {
            PixelControlPoint beginPoint = new PixelControlPoint();
            beginPoint.Location = new Point(OFFSET + BLANK, Height - OFFSET - BLANK);
            PixelControlPoint endPoint = new PixelControlPoint();
            endPoint.Location = new Point(ClientRectangle.Right - BLANK, BLANK);
            _pixelControlPoints.Add(beginPoint);
            _pixelControlPoints.Add(endPoint);
        }

        /// <summary>
        /// 根据插值器类型绘制图像
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawAxisAndGrid(e.Graphics);
            if (_interpolator == null)
                return;
            if (_interpolator is LineInterpolator || _interpolator is SplineInterpolator)
                DrawLineCurveInterpolator(e.Graphics);            
        }

        /// <summary>
        /// 绘制工作区网格以及坐标条
        /// </summary>
        /// <param name="curveGraphic"></param>
        private void DrawAxisAndGrid(Graphics curveGraphic)
        {
            if (_penGrid == null || _penSolid == null)
                InitPenReg();

            _penLine = new Pen(_colorSel);         
            curveGraphic.Clear(this.BackColor);
            curveGraphic.SmoothingMode = SmoothingMode.AntiAlias;

            _workRect = ClientRectangle;
            _workRect.X = _workRect.X + OFFSET;
            _workRect.Width = _workRect.Width - OFFSET;
            _workRect.Height = _workRect.Height - OFFSET;

            Rectangle buttomRect = new Rectangle(_workRect.Left, _workRect.Bottom + 7, _workRect.Width - 1, GRAYLINE_WIDTH);//底端渐变条矩形
            curveGraphic.DrawRectangle(_penSolid, buttomRect);
            LinearGradientBrush buttomBrush = new LinearGradientBrush(buttomRect, Color.Black, Color.White, LinearGradientMode.Horizontal);//渐变画刷
            curveGraphic.FillRectangle(buttomBrush, buttomRect);//绘制最底端矩形条

            Rectangle leftRect = new Rectangle(_workRect.Left - OFFSET, _workRect.Top, GRAYLINE_WIDTH, _workRect.Height);//左端渐变条
            curveGraphic.DrawRectangle(_penSolid, leftRect);
            LinearGradientBrush leftBrush = new LinearGradientBrush(leftRect, Color.White, Color.Black, LinearGradientMode.Vertical);
            curveGraphic.FillRectangle(leftBrush, leftRect);//绘制左端矩形条

            _workRect.Inflate(-BLANK, -BLANK);
            Pen temptPen =(Pen) _penGrid.Clone();
            float a = _workRect.Right - _workRect.Left;
            float b = _workRect.Bottom - _workRect.Top;

            for (float f = 0.0f; f <= 1.0f; f += 0.25f)
            {
                float x = _workRect.Left + f * a;
                float y = _workRect.Top + f * b;
                if (f == 0.0f || f == 1.0f)
                    _penGrid = _penSolid;
                curveGraphic.DrawLine(_penGrid, Point.Round(new PointF(x, _workRect.Top)), Point.Round(new PointF(x, _workRect.Bottom)));
                curveGraphic.DrawLine(_penGrid, Point.Round(new PointF(_workRect.Left, y)), Point.Round(new PointF(_workRect.Right, y)));
                _penGrid =temptPen;
            }
        }

        /// <summary>
        /// 按照点数组绘制折线和曲线
        /// </summary>
        /// <param name="g"></param>
        private void DrawLineCurveInterpolator(Graphics g)
        {
            g.IntersectClip(_workRect);
            PixelControlPoint ppt;
            int count = _pixelControlPoints.Count;
            if (_interpolator is LineInterpolator)
            {
                for (int i = 0; i < _pixelControlPoints.Count - 1; i++)
                    g.DrawLine(_penLine, _pixelControlPoints[i].Location, _pixelControlPoints[i + 1].Location);
            }
            if (_interpolator is SplineInterpolator)
            {
                List<Point> pointsList = new List<Point>();
                for (int i = 0; i < count; i++)
                {
                    pointsList.Add(_pixelControlPoints[i].Location);
                }
                g.DrawCurve(_penLine, pointsList.ToArray(), 0.75f);
            }
            for (int i = 0; i < _pixelControlPoints.Count; i++)
            {
                GraphicsPath path = new GraphicsPath();
                ppt = _pixelControlPoints[i];
                path.AddEllipse(ppt.Location.X - POINT_RADIUS, ppt.Location.Y - POINT_RADIUS, POINT_RADIUS << 1, POINT_RADIUS << 1);
                if (ppt == _currentPixelPoint)
                {
                    g.FillEllipse(Brushes.White, ppt.Location.X - POINT_RADIUS, ppt.Location.Y - POINT_RADIUS, POINT_RADIUS << 1, POINT_RADIUS << 1);
                }
                else
                {
                    g.FillEllipse(Brushes.Gray, ppt.Location.X - POINT_RADIUS, ppt.Location.Y - POINT_RADIUS, POINT_RADIUS << 1, POINT_RADIUS << 1);
                }
                if (ppt.Path != null)
                    ppt.Path.Dispose();
                ppt.Path = path;
                g.DrawPath(_penLine, path);
            }
        }

        /// <summary>
        /// 获取鼠标点击时的坐标(界面—>点),并排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurveControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X < _workRect.Left || e.X > _workRect.Right || e.Y > _workRect.Bottom || e.Y < _workRect.Top)
                return;
            //左键按下时新增控制点
            if (e.Button == MouseButtons.Left)
            {
                //判断路径是否包含鼠标点，包含则将选中点赋给当前点，不包含则重新出创建当前点
                foreach (PixelControlPoint pt in _pixelControlPoints)
                {
                    if (pt.Path.IsVisible(e.X, e.Y))
                    {
                        _currentPixelPoint = pt;
                        return;
                    }
                }
                AddControlPoint(e.X, e.Y);
            }
            //右键按下时删除控制点
            if (e.Button == MouseButtons.Right)
            {
                DeleteControlPoint(e.X, e.Y);
            }
        }

        /// <summary>
        /// 鼠标拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurveControl_MouseMove(object sender, MouseEventArgs e)
        {
            //鼠标没有按下时，当鼠标放在已有控制点上方时，将鼠标位置传给当前点；
            if (e.Button == System.Windows.Forms.MouseButtons.None)
            {
                foreach (PixelControlPoint pt in _pixelControlPoints)
                {
                    if (pt.Path == null)
                        continue;
                    if (pt.Path.IsVisible(e.X, e.Y))
                    {
                        _currentPixelPoint = pt;
                        Invalidate();
                        return;
                    }
                }
                _currentPixelPoint = null;
                Invalidate();
                return;
            }
            //当鼠标已经按下时，设置鼠标拖动的效果
            if (_currentPixelPoint != null)
            {
                //判断按下时选中的点是否为两个端点，是则x值不变，y改变（只能上下拖动）；否则重新赋值给当前值
                if (_pixelControlPoints[0] == _currentPixelPoint || _pixelControlPoints[_pixelControlPoints.Count - 1] == _currentPixelPoint)
                {
                    if (e.Y < _workRect.Bottom && e.Y > _workRect.Top)
                        _currentPixelPoint.Location.Y = e.Y;
                }
                else
                {
                    if (e.X > _workRect.Left && e.X < _workRect.Right && e.Y < _workRect.Bottom && e.Y > _workRect.Top)
                    {
                        _currentPixelPoint.Location.X = e.X;
                        _currentPixelPoint.Location.Y = e.Y;
                    }
                }
                _pixelControlPoints.Sort(new PixelControlPointXComparer());
                Invalidate();
                TryApply();
            }
        }

        /// <summary>
        /// 鼠标弹起时设置当前值为空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurveControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (_currentPixelPoint != null)
                _currentPixelPoint = null;
            Invalidate(); 
        }

        private void TryApply()
        {
            ApplyControlPoints();
            if (ControlPointChanged != null)
                ControlPointChanged(this, null);
        }

        internal void Apply(IInterpolator interpolator)
        {
            _interpolator = interpolator;
            ApplyControlPoints();
        }

        private void ApplyControlPoints()
        {
            List<Point> pointList = new List<Point>();
            int count = _pixelControlPoints.Count;
            for (int i = 0; i < count; i++)
            {
                pointList.Add(ControlToPoint(_pixelControlPoints[i].Location));
            }
            (_interpolator as ControlPointInterpolator).UpdateControlPoints(pointList.ToArray());
        }

        /// <summary>
        /// 增加控制点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        internal void AddControlPoint(int x, int y)
        {
            CreatCurrentPoint(x, y);

            if (Math.Abs(_currentPixelPoint.Location.X - _pixelControlPoints[0].Location.X) < POINT_RADIUS ||
                Math.Abs(_currentPixelPoint.Location.X - _pixelControlPoints[_pixelControlPoints.Count - 1].Location.X) < POINT_RADIUS)
                return;

            foreach (PixelControlPoint pt in _pixelControlPoints)
            {
                if (Math.Abs(_currentPixelPoint.Location.X - pt.Location.X) < POINT_RADIUS)
                {
                    _pixelControlPoints.Remove(pt);
                    break;
                }
            }
            _pixelControlPoints.Add(_currentPixelPoint);
            _pixelControlPoints.Sort(new PixelControlPointXComparer());
            Invalidate();//start
            TryApply();
            //stop
        }

        /// <summary>
        /// 删除控制点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        internal void DeleteControlPoint(int x, int y)
        {
            foreach (PixelControlPoint pt in _pixelControlPoints)
            {
                if (pt.Path.IsVisible(x, y))
                {
                    if (_pixelControlPoints[0] == pt || _pixelControlPoints[_pixelControlPoints.Count - 1] == pt)
                        return;
                    _pixelControlPoints.Remove(pt);
                    _currentPixelPoint = pt;
                    break;
                }
            }
            CreatCurrentPoint(x, y);
            _pixelControlPoints.Sort(new PixelControlPointXComparer());
            Invalidate();
            TryApply();
        }

        /// <summary>
        /// 创建当前控制点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void CreatCurrentPoint(int x, int y)
        {
            _currentPixelPoint = new PixelControlPoint();
            _currentPixelPoint.Location = new Point(x, y);
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(_currentPixelPoint.Location.X - POINT_RADIUS, _currentPixelPoint.Location.Y - POINT_RADIUS, POINT_RADIUS << 1, POINT_RADIUS << 1);
            _currentPixelPoint.Path = path;
        }

        /// <summary>
        /// 重置点 
        /// </summary>
        internal void Reset()
        {
            _pixelControlPoints.Clear();
            InitControlPointList();
            Invalidate();
        }

        /// <summary>
        /// 点坐标—>界面坐标
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        internal Point PointToControl(Point pt)
        {
            double scaleY = (_workRect.Height) / 255f;
            double scaleX = (_workRect.Width) / 255f;
            pt.X = (int)(pt.X * scaleX) + _workRect.Left + BLANK;
            pt.Y = _workRect.Bottom - (int)(pt.Y * scaleY) - BLANK;
            return pt;
        }

        /// <summary>
        /// 界面坐标—>点坐标
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        internal Point ControlToPoint(Point pt)
        {
            pt.X = (int)((pt.X - _workRect.Left)*255f / _workRect.Width);
            pt.Y = (int)((_workRect.Bottom - pt.Y) * 255f / _workRect.Height);
            return pt;
        }

        private void CurveControl_RegionChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }
    }
}
