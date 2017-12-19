using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using Telerik.WinControls;

namespace Telerik.WinControls
{
    [DesignTimeVisible(false)]
    [ToolboxItem(false)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShapePoint : Component
    {
        #region Data
        private PointF point;
        private bool isModified;
        private bool isLocked;

        private AnchorStyles anchor = AnchorStyles.None;

        #endregion

        #region Constructors

        public ShapePoint()
        {
            point = new PointF(0, 0);
            isLocked = false;
        }

        public ShapePoint(Point pt)
        {
            point = new PointF(pt.X, pt.Y);
            isLocked = false;
        }

        public ShapePoint(PointF pt)
        {
            point = new PointF(pt.X, pt.Y);
            isLocked = false;
        }

        public ShapePoint(int x, int y)
        {
            point = new PointF(x, y);
            isLocked = false;
        }

        public ShapePoint(float x, float y)
        {
            point = new PointF(x, y);
            isLocked = false;
        }

        public ShapePoint(ShapePoint pt)
        {
            point = pt.point;
            isLocked = pt.isLocked;
        }

        #endregion

        #region Accessors

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public PointF Location
        {
            get { return point; }
            set
            {
                if (isLocked) return;

                point = value;
            }
        }

        [DefaultValue(0)]
        [Category(RadDesignCategory.LayoutCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The X coordinate of the point")]
        [NotifyParentProperty(true)]
        public float X
        {
            get { return point.X; }
            set { if (!isLocked) point.X = value; }
        }

        [DefaultValue(0)]
        [Category(RadDesignCategory.LayoutCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The Y coordinate of the point")]
        [NotifyParentProperty(true)]
        public float Y
        {
            get { return point.Y; }
            set { if (!isLocked) point.Y = value; }
        }

        [Browsable(false)]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsModified
        {
            get { return isModified; }
            set { if (!isLocked) isModified = value; }
        }

        [DefaultValue(false)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Determines if the current point could be moved or not.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool IsLocked
        {
            get { return isLocked; }
            set 
            {
                if (value) isModified = false;
                isLocked = value; 
            }
        }

        #endregion

        public static float DistSquared(PointF a, PointF b)
        {
            PointF d = new PointF(a.X - b.X, a.Y - b.Y);

            return d.X * d.X + d.Y * d.Y;
        }

        public void Set(Point pt)
        {
            X = pt.X;
            Y = pt.Y;
        }

        public void Set(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Set(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Point Get()
        {
            return new Point((int)Math.Round(X), (int)Math.Round(Y));
        }

        public Point GetPoint(Rectangle bounds)
        {
            return new Point(bounds.X + (int)Math.Round(X), bounds.Y + (int)Math.Round(Y));
        }

        public Point GetPoint(Rectangle src, Rectangle dst)
        {
            float x = X - src.X;
            float y = Y - src.Y;

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
        #region Type casts

        public static implicit operator Point (ShapePoint pt)
        {
            return new Point((int)pt.X, (int)pt.Y);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Point: {0},{1}", X.ToString(), Y.ToString());
        }

    }
}
