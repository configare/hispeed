using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using CodeCell.Bricks.Runtime;

namespace CodeCell.Bricks.ModelFabric
{
    internal class ActionElementLink:IRenderable,IDisposable
    {
        private ActionElement _preActionElement = null;
        private ActionElement _nextActionElement = null;
        internal static Pen PenLink = new Pen(Color.FromArgb(94, 146, 231));
        internal static Brush FillArrow = new SolidBrush(PenLink.Color);

        static ActionElementLink()
        {
            PenLink.Width = 1;
        }

        public ActionElementLink(ActionElement preAction, ActionElement nextAction)
        {
            _preActionElement = preAction;
            _nextActionElement = nextAction;
        }

        public ActionElement PreAction
        {
            get { return _preActionElement; }
        }

        public ActionElement NextAction
        {
            get { return _nextActionElement; }
        }

        #region IRenderable 成员

        public void Render(RenderArg arg)
        {
            PointF fPt = PointF.Empty, tPt = PointF.Empty;
            GeometryHelper.GetLinkLine(_preActionElement, _nextActionElement, out fPt, out tPt);
            arg.Graphics.DrawLine(PenLink, fPt, tPt);
            arg.Graphics.FillPolygon(FillArrow, GetArrowPoints(fPt, tPt));
        }

        public bool IsHited(Point point)
        {
            if (_preActionElement == null || _nextActionElement == null)
                return false;
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddLine(_preActionElement.Location, _nextActionElement.Location);
                using (Region r = new Region(path))
                {
                    return r.IsVisible(point);
                }
            }
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
        }

        #endregion

        #region IDisposable 成员

        void IDisposable.Dispose()
        {
            
        }

        #endregion

        private PointF[] GetArrowPoints(PointF fpt, PointF tpt)
        {
            int w = 8;
            int h = 10;
            PointF[] pts = new PointF[] { new PointF(0, h), new PointF(w, 0), new PointF(-w, 0) };
            int lineLength = 0;
            float angle = MathHelper.GetAngle(tpt, fpt, out lineLength);
            PointF cpt = MathHelper.GetOnePointAtLine(tpt, fpt, lineLength, h);
            using (Matrix m = new Matrix())
            {
                m.Translate(cpt.X, cpt.Y);
                if (Math.Abs(angle - 0) < float.Epsilon)
                {
                    if (tpt.X > fpt.X)
                        m.Rotate(-90);
                    else
                        m.Rotate(90);
                }
                else
                    m.Rotate(angle + 90);
                m.TransformPoints(pts);
            }
            return pts;
        }
    }
}
