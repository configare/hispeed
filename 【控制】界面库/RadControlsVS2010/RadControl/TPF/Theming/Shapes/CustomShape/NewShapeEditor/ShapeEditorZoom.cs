using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls
{
    public class ShapeEditorZoom
    {
        #region Private Data

        private float zoomFactor;
        private float zoomMin;
        private float zoomMax;

        private RectangleF workingArea; // the area including all points (in real coords)
        private RectangleF visibleArea; // the area visible on screen (in real coords)

        #endregion

        #region Constructors

        public ShapeEditorZoom()
        {
            zoomFactor = 1f;
            zoomMin = 0.5f; // zoom out 2 times
            zoomMax = 2f; // zoom in 2 times
            workingArea = new RectangleF(0, 0, 0, 0);
            visibleArea = new RectangleF(0, 0, 0, 0);
        }

        public ShapeEditorZoom(float minZoom, float maxZoom)
        {
            ZoomOutMax = minZoom;
            ZoomInMax = maxZoom;
            ZoomFactor = 1;

            workingArea = new RectangleF(0, 0, 0, 0);
            visibleArea = new RectangleF(0, 0, 0, 0);
        }

        public ShapeEditorZoom(float minZoom, float maxZoom, float zoom)
        {
            ZoomOutMax = minZoom;
            ZoomInMax = maxZoom;

            ZoomFactor = zoom;

            workingArea = new RectangleF(0, 0, 0, 0);
            visibleArea = new RectangleF(0, 0, 0, 0);
        }

        #endregion

        #region Accessors

        public float ZoomInMax
        {
            get { return zoomMax; }
            set
            {
                if (value < 1) 
                    zoomMax = 1;
                else
                    zoomMax = value;
            }
        }

        public float ZoomOutMax
        {
            get { return 1f / zoomMin; }
            set 
            {
                if (value < 1f) 
                    zoomMin = 1f;
                else 
                    zoomMin = 1f / value;
            }
        }

        public float ZoomFactor
        {
            get { return zoomFactor; }
            set { zoomFactor = Math.Max(Math.Min(value, zoomMax), zoomMin); }
        }

        public RectangleF WorkingArea
        {
            get { return workingArea; }
            set { workingArea = value; }
        }

        public RectangleF VisibleArea
        {
            get { return visibleArea; }
            set { visibleArea = value; }
        }

        public Size VisibleAreaSize
        {
            get { return new Size((int)Math.Floor(visibleArea.Width), (int)Math.Floor(visibleArea.Height)); }
            set {
                visibleArea.Width = value.Width;
                visibleArea.Height = value.Height;
            }
        }

        public Point Location
        {
            get { return new Point((int)Math.Round(visibleArea.Location.X), (int)Math.Round(visibleArea.Location.Y)); }
            set
            {
                visibleArea.X = value.X;
                visibleArea.Y = value.Y;
            }
        }

        public Size FullArea
        {
            get
            {
                return new Size ((int)(workingArea.Width * zoomFactor), (int)(workingArea.Height * zoomFactor));
                /*RectangleF rect = new RectangleF(
                    visibleArea.X * zoomFactor,
                    visibleArea.Y * zoomFactor,
                    visibleArea.Width * zoomFactor,
                    visibleArea.Height * zoomFactor
                    );
                
                if (WorkingArea.Contains(rect)
                rect = RectangleF.Intersect(rect, WorkingArea);

                return new Size((int)Math.Floor(rect.Width), (int)Math.Floor(rect.Height));*/
            }
        }

        #endregion

        #region Methods

        public float ZoomIn(float step)
        {
            ZoomFactor += step;

            return zoomFactor;
        }

        public float ZoomOut(float step)
        {
            ZoomFactor -= step;

            return zoomFactor;
        }

        public bool Zoom(float value)
        {
            float oldValue = zoomFactor;

            ZoomFactor = value;

            return oldValue != zoomFactor;
        }

        public PointF PtToVirtual(PointF pt)
        {
            pt.X *= zoomFactor;
            pt.Y *= zoomFactor;

            return pt;
        }

        public PointF VirtualToPt(PointF pt)
        {
            pt.X /= zoomFactor;
            pt.Y /= zoomFactor;

            return pt;
        }

        public PointF PtToVisible(PointF pt)
        {
            pt.X = pt.X * zoomFactor - visibleArea.X;
            pt.Y = pt.Y * zoomFactor - visibleArea.Y;

            return pt;
        }

        public PointF VisibleToPt(PointF pt)
        {
            pt.X = (pt.X + visibleArea.X) / zoomFactor;
            pt.Y = (pt.Y + visibleArea.Y) / zoomFactor;

            return pt;
        }

        public RectangleF RectToVirtual(RectangleF rect)
        {
            rect.X *= zoomFactor;
            rect.Y *= zoomFactor;
            rect.Width *= zoomFactor;
            rect.Height *= zoomFactor;

            return rect;
        }

        public RectangleF VirtualToRect(RectangleF rect)
        {
            rect.X /= zoomFactor;
            rect.Y /= zoomFactor;
            rect.Width /= zoomFactor;
            rect.Height /= zoomFactor;

            return rect;
        }

        public RectangleF RectToVisible(RectangleF rect)
        {
            rect.X = rect.X*zoomFactor - visibleArea.X;
            rect.Y = rect.Y*zoomFactor - visibleArea.Y;
            rect.Width *= zoomFactor;
            rect.Height *= zoomFactor;

            return rect;
        }

        public RectangleF VisibleToRect(RectangleF rect)
        {
            rect.X = (rect.X + visibleArea.X)/zoomFactor;
            rect.Y = (rect.Y + visibleArea.Y)/zoomFactor;
            rect.Width /= zoomFactor;
            rect.Height /= zoomFactor;

            return rect;
        }


        public void ZoomAtPoint(PointF pt, PointF ptInView)
        {
            PointF zoomedPt = PtToVirtual(pt);

            visibleArea.X = workingArea.X + zoomedPt.X - ptInView.X;
            visibleArea.Y = workingArea.X + zoomedPt.Y - ptInView.Y;
        }

        public void Scroll(SizeF scrollTo)
        {
            visibleArea.X = scrollTo.Width;
            visibleArea.Y = scrollTo.Height;
        }

        public void Scroll(PointF scrollTo)
        {
            visibleArea.X = scrollTo.X;
            visibleArea.Y = scrollTo.Y;
        }

        public void ScrollHorizontal(float to)
        {
            visibleArea.X = to;
        }

        public void ScrollVertical(float to)
        {
            visibleArea.Y = to;
        }

        public void FitToScreen(RectangleF? rect)
        {
            RectangleF? wrect;

            if ((rect == null) || (rect.Value.IsEmpty))
                wrect = workingArea;
            else
                wrect = rect.Value;

            float coef = Math.Min(
                (visibleArea.Width - 20) / wrect.Value.Width,
                (visibleArea.Height - 20) / wrect.Value.Height
                );

            ZoomFactor = coef;

            CenterVisible(wrect.Value);
        }

        public void FitToScreen()
        {
        }

        private void CenterVisible(RectangleF rect)
        {
            PointF centerVisible = new PointF(visibleArea.Width / 2, visibleArea.Height / 2);
            PointF centerWorking = new PointF(rect.Width / 2, rect.Height / 2);

            visibleArea.X = (rect.X + centerWorking.X) * zoomFactor - centerVisible.X;
            visibleArea.Y = (rect.Y + centerWorking.Y) * zoomFactor - centerVisible.Y;

        }

        #endregion

    }
}
