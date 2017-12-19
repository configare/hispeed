using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;

namespace Telerik.WinControls
{
    [ToolboxItem(false)]
    public class CustomShapeConvertor
    {
        List<OldShapePoint> points = new List<OldShapePoint>();

        public CustomShapeConvertor()
        {
			points = new List<OldShapePoint>();
        }
        

        public List<OldShapePoint> Points
        {
            get { return points; }
        }

        Rectangle dimension;

        public Rectangle Dimension
        {
            get { return dimension; }
            set { dimension = value; }
        }

        public string SerializeProperties()
        {
            string s = string.Format("{0},{1},{2},{3}:", dimension.X, dimension.Y, dimension.Width, dimension.Height);
            foreach(OldShapePoint point in points)
            {
                s += string.Format("{0},{1},{2},{3},{4},{5},{6},{7}:",
                    (int)point.X, (int)point.Y,
                    point.Bezier,
                    (int)point.ControlPoint1.X, (int)point.ControlPoint1.Y,
                    (int)point.ControlPoint2.X, (int)point.ControlPoint2.Y,
                    (int)point.Anchor);
            }
            return s;
        }

        public CustomShape GetShape()
        {
            CustomShape newShape = new CustomShape();

            newShape.DeserializeProperties(this.SerializeProperties());

            return newShape;
        }

    }

    [ToolboxItem(false)]
    public class OldShapePoint : OldShapePointBase
    {

        OldShapePointBase controlPoint1 = new OldShapePointBase();

        public OldShapePointBase ControlPoint1
        {
            get { return controlPoint1; }
            set { controlPoint1 = value; }
        }

        OldShapePointBase controlPoint2 = new OldShapePointBase();

        public OldShapePointBase ControlPoint2
        {
            get { return controlPoint2; }
            set { controlPoint2 = value; }
        }

        bool bezier = false;

        public bool Bezier
        {
            get { return bezier; }
            set { bezier = value; }
        }

        public OldShapePoint()
        {
        }

        public OldShapePoint(int x, int y)
            : base(x, y)
        {
        }

        public OldShapePoint(Point point)
            : base(point)
        {
        }

        public OldShapePoint(OldShapePoint point)
            : base(point)
        {
            this.ControlPoint1 = new OldShapePointBase(point.ControlPoint1);
            this.ControlPoint2 = new OldShapePointBase(point.ControlPoint2);
            this.Bezier = point.bezier;
        }
    }

    [ToolboxItem(false)]
    public class OldShapePointBase
    {
        protected float x = 0;

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        protected float y = 0;

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        AnchorStyles anchor = AnchorStyles.None;

        public AnchorStyles Anchor
        {
            get { return anchor; }
            set { anchor = value; }
        }

        bool locked = false;

        public bool Locked
        {
            get { return locked; }
            set { locked = value; }
        }

        public OldShapePointBase()
        {
        }

        public OldShapePointBase(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public OldShapePointBase(Point point)
        {
            this.x = point.X;
            this.y = point.Y;
        }

        public OldShapePointBase(OldShapePointBase point)
        {
            this.x = point.X;
            this.y = point.Y;
            this.anchor = point.Anchor;
            this.locked = point.Locked;
        }

        public void Set(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public void Set(Point point)
        {
            Set(point.X, point.Y);
        }

        public Point GetPoint()
        {
            return new Point((int)x, (int)y);
        }

        public Point GetPoint(Rectangle bounds)
        {
            return new Point(bounds.X + (int)Math.Round(x), bounds.Y + (int)Math.Round(y));
        }
        public Point GetPoint(Rectangle src, Rectangle dst)
        {
            float x = this.x - src.X;
            float y = this.y - src.Y;

            float dx = x / src.Width;
            float dy = y / src.Height;

            float dstx = 0;
            float dsty = 0;

            if ((this.anchor & AnchorStyles.Left) != 0) dstx = dst.X + x;
            else if ((this.anchor & AnchorStyles.Right) != 0) dstx = dst.Right - (src.Width - x);
            else dstx = dst.X + dx * dst.Width;

            if ((this.anchor & AnchorStyles.Top) != 0) dsty = dst.Y + y;
            else if ((this.anchor & AnchorStyles.Bottom) != 0) dsty = dst.Bottom - (src.Height - y);
            else dsty = dst.Y + dy * dst.Height;

            return new Point((int)Math.Round(dstx), (int)Math.Round(dsty));
        }

        public Rectangle GetBounds(int weight)
        {
            return new Rectangle((int)Math.Round(x - weight / 4), (int)Math.Round(y - weight / 4), weight, weight);
        }
        public bool IsVisible(int x, int y, int width)
        {
            return GetBounds(width).Contains(x, y);
        }

        public override string ToString()
        {
            return string.Format("Point: {0},{1}", x.ToString(), y.ToString());
        }
    }

}
