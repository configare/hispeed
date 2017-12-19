using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using CodeCell.Bricks.Runtime;

namespace GeoDo.RSS.Layout.GDIPlus
{
    public class SelectedEditBox : ISelectedEditBox, IRenderable, IDisposable
    {
        public static Pen SELECTED_BOX_PEN = new Pen(Color.FromArgb(21, 209, 209));
        public static SolidBrush SELECT_BOX_BRUSH = new SolidBrush(Color.FromArgb(21, 209, 209));
        public const int ANCHOR_POINT_HALF_WIDTH = 5;
        protected RectangleF[] _anchorPointRects = new RectangleF[8];
        protected RectangleF _anchorRect;
        protected GraphicsPath _pinBounds;
        protected PointF _pinPoint;
        protected PointF _pinEndPoint;
        protected const int PIN_RADIUS = 5;
        protected const int PIN_POLE_EXT_LENGTH = 80;
        protected ISizableElement _element;
        private float _scale = 1f;

        public SelectedEditBox(ISizableElement element)
        {
            _element = element;
            SetPen();
            UpdateAnchors();
        }

        private void SetPen()
        {
            SELECTED_BOX_PEN.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            SELECTED_BOX_PEN.DashPattern = new float[] { 5f, 2f };
        }

        public int IndexOfAnchor(float screenX, float screenY)
        {
            for (int i = 0; i < 8; i++)
                if (_anchorPointRects[i].Contains(screenX, screenY))
                    return i;
            if (Math.Abs(_element.Angle) > float.Epsilon)
            {
                using (Matrix m = new Matrix())
                {
                    PointF cpt = new PointF((_anchorRect.Left + _anchorRect.Right) / 2f, (_anchorRect.Top + _anchorRect.Bottom) / 2f);
                    m.RotateAt(_element.Angle, cpt);
                    m.Invert();
                    //
                    PointF pt = new PointF(screenX, screenY);
                    PointF[] pts = new PointF[] { pt };
                    m.TransformPoints(pts);
                    pt = pts[0];
                    //
                    if (_pinBounds.IsVisible(pt.X, pt.Y))
                        return 8;//rotate pin
                }
            }
            else
            {
                if (_pinBounds.IsVisible(screenX, screenY))
                    return 8;//rotate pin
            }
            return -1;
        }

        public void Apply(ILayoutRuntime runtime, int anchorIndex, PointF bPoint, PointF ePoint)
        {
            float offsetX = ePoint.X - bPoint.X;
            float offsetY = ePoint.Y - bPoint.Y;
            //约束长宽比
            //Console.WriteLine(anchorIndex.ToString());
            //Console.WriteLine("OffsetY:" + offsetY.ToString());
            if (Control.ModifierKeys == Keys.Shift)
            {
                if (anchorIndex == 0 || anchorIndex == 2 || anchorIndex == 4 || anchorIndex == 6)
                {
                    float factor = Math.Abs(offsetX / _element.Size.Width);
                    if (offsetX < 0 && anchorIndex == 4)
                        factor *= -1f;
                    else if (offsetX > 0 && anchorIndex == 6)
                        factor *= -1f;
                    else if (offsetX < 0 && anchorIndex == 0)
                        factor *= -1f;
                    else if (offsetX > 0 && anchorIndex == 2)
                        factor *= -1f;
                    offsetY = factor * _element.Size.Height;
                }
            }
            switch (anchorIndex)
            {
                case 0://up-left
                    _element.ApplyLocation(offsetX, offsetY);
                    _element.ApplySize(-offsetX, -offsetY);
                    break;
                case 4://right-down
                    _element.ApplySize(offsetX, offsetY);
                    break;
                case 1://up
                    _element.ApplyLocation(0, offsetY);
                    _element.ApplySize(0, -offsetY);
                    break;
                case 5://down
                    _element.ApplySize(0, offsetY);
                    break;
                case 3://right
                    _element.ApplySize(offsetX, 0);
                    break;
                case 7://left
                    _element.ApplyLocation(offsetX, 0);
                    _element.ApplySize(-offsetX, 0);
                    break;
                case 2://right-up
                    _element.ApplyLocation(0, offsetY);
                    _element.ApplySize(offsetX, -offsetY);
                    break;
                case 6://left-down
                    _element.ApplyLocation(offsetX, 0);
                    _element.ApplySize(-offsetX, offsetY);
                    break;
                case 8://rotate 
                    if (offsetY != 0)
                    {
                        PointF cpt = new PointF((_anchorRect.Left + _anchorRect.Right) / 2f, (_anchorRect.Top + _anchorRect.Bottom) / 2f);
                        float x = ePoint.X;
                        float y = ePoint.Y;
                        runtime.Layout2Screen(ref x, ref y);
                        float angle = MathHelper.GetAngle(cpt, new PointF(x, y));
                        //Console.WriteLine(angle.ToString());
                        _element.ApplyRotate(angle);
                    }
                    break;
            }
        }

        public void Render(object sender, IDrawArgs drawArgs)
        {
            if (_anchorRect.IsEmpty || _element == null)
                return;
            Graphics g = drawArgs.Graphics as Graphics;
            SmoothingMode sm = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.HighQuality;
            try
            {
                ILayoutRuntime runtime = (sender as ILayoutHost).LayoutRuntime;
                float x1 = _anchorRect.Left, y1 = _anchorRect.Top;
                float x2 = _anchorRect.Right, y2 = _anchorRect.Bottom;
                runtime.Layout2Screen(ref x1, ref y1);
                runtime.Layout2Screen(ref x2, ref y2);
                _anchorRect.Location = new PointF(x1, y1);
                _anchorRect.Width = Math.Abs(x2 - x1);
                _anchorRect.Height = Math.Abs(y2 - y1);
                g.DrawRectangle(SELECTED_BOX_PEN, _anchorRect.X, _anchorRect.Y, _anchorRect.Width, _anchorRect.Height);
                //
                UpdateAnchorPoints(_anchorRect, runtime.Scale);
                //
                for (int i = 0; i < 8; i++)
                {
                    g.FillRectangle(SELECT_BOX_BRUSH, _anchorPointRects[i].X, _anchorPointRects[i].Y, _anchorPointRects[i].Width, _anchorPointRects[i].Height);
                    g.DrawRectangle(Pens.Black, _anchorPointRects[i].X, _anchorPointRects[i].Y, _anchorPointRects[i].Width, _anchorPointRects[i].Height);
                }
                //draw pin
                if (!(_element is IDataFrame))
                {
                    using (Matrix m = g.Transform.Clone())
                    {
                        PointF cpt = new PointF((_anchorRect.Left + _anchorRect.Right) / 2f, (_anchorRect.Top + _anchorRect.Bottom) / 2f);
                        m.RotateAt(_element.Angle, cpt);
                        Matrix oldM = g.Transform;
                        g.Transform = m;
                        g.FillPath(Brushes.Yellow, _pinBounds);
                        g.DrawPath(Pens.Blue, _pinBounds);
                        g.DrawLine(SELECTED_BOX_PEN,
                            _anchorRect.Left + _anchorRect.Width / 2f,
                             _pinPoint.Y + PIN_RADIUS,
                            _pinPoint.X,
                            _pinPoint.Y + PIN_RADIUS);
                        g.Transform = oldM;
                    }
                }
            }
            finally
            {
                g.SmoothingMode = sm;
            }
        }

        private void UpdateAnchorPoints(RectangleF rect, float scale)
        {
            int boxWidth = (int)(ANCHOR_POINT_HALF_WIDTH * scale);
            PointF middlePoint = new PointF();
            if (_anchorPointRects == null)
                _anchorPointRects = new RectangleF[8];
            _anchorPointRects[0] = new RectangleF(rect.X - boxWidth, rect.Y - ANCHOR_POINT_HALF_WIDTH, 2 * boxWidth, 2 * boxWidth);
            middlePoint.X = rect.Left + rect.Width / 2;
            middlePoint.Y = rect.Top;
            _anchorPointRects[1] = new RectangleF(middlePoint.X - boxWidth, middlePoint.Y - boxWidth, 2 * boxWidth, 2 * boxWidth);
            _anchorPointRects[2] = new RectangleF(rect.Right - boxWidth, rect.Top - boxWidth, 2 * boxWidth, 2 * boxWidth);
            middlePoint.X = rect.Right;
            middlePoint.Y = rect.Top + rect.Height / 2;
            _anchorPointRects[3] = new RectangleF(middlePoint.X - boxWidth, middlePoint.Y - boxWidth, 2 * boxWidth, 2 * boxWidth);
            _anchorPointRects[4] = new RectangleF(rect.Right - boxWidth, rect.Bottom - boxWidth, 2 * boxWidth, 2 * boxWidth);
            middlePoint.X = rect.Left + rect.Width / 2;
            middlePoint.Y = rect.Bottom;
            _anchorPointRects[5] = new RectangleF(middlePoint.X - boxWidth, middlePoint.Y - boxWidth, 2 * boxWidth, 2 * boxWidth);
            _anchorPointRects[6] = new RectangleF(rect.Left - boxWidth, rect.Bottom - boxWidth, 2 * boxWidth, 2 * boxWidth);
            middlePoint.X = rect.Left;
            middlePoint.Y = rect.Top + rect.Height / 2;
            _anchorPointRects[7] = new RectangleF(middlePoint.X - boxWidth, middlePoint.Y - boxWidth, 2 * boxWidth, 2 * boxWidth);
            //
            if (_pinBounds != null)
                _pinBounds.Reset();
            else
                _pinBounds = new GraphicsPath();
            _pinPoint = new PointF(rect.Right + PIN_POLE_EXT_LENGTH * scale,
                (rect.Top + rect.Bottom) / 2f - PIN_RADIUS * scale);
            _pinBounds.AddEllipse(new RectangleF(_pinPoint.X, _pinPoint.Y,
                PIN_RADIUS * 2 * scale, PIN_RADIUS * 2 * scale));
        }

        private void UpdateAnchors()
        {
            _anchorRect.Location = _element.Location;
            _anchorRect.Size = _element.Size;
        }

        public void Dispose()
        {
        }
    }
}
