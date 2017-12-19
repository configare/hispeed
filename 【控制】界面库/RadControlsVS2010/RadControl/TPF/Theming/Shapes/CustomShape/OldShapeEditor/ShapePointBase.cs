using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace Telerik.WinControls.OldShapeEditor
{
    /// <summary>
    /// Represents a base class of the ShapePoint class.
    /// </summary>
    [DesignTimeVisible(false)]
    [ToolboxItem(false)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShapePointBase: Component
    {
        protected float x = 0;
        
        /// <summary>
        /// Gets or sets a float value indicating the X coordinate of the shape point.
        /// </summary>
        [DefaultValue(0), Category(RadDesignCategory.AppearanceCategory), Description("The X coordinate of the point")]
        public float X
        {
            get { return x; }
            set { x = value; }
        }

        protected float y = 0;

        /// <summary>
        /// Gets or sets a float value indicating the Y coordinate of the shape point.
        /// </summary>
        [DefaultValue(0), Category(RadDesignCategory.AppearanceCategory), Description("The Y coordinate of the point")]
        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        AnchorStyles anchor = AnchorStyles.None;

        /// <summary>
        /// Gets or sets a value indicating the anchor style.
        /// </summary>
        [DefaultValue(AnchorStyles.None), Category(RadDesignCategory.AppearanceCategory), Description("The anchor styles of this point. Possible values are left or right and top or bottom")]
        public AnchorStyles Anchor
        {
            get { return anchor; }
            set { anchor = value; }
        }

        bool locked = false;

        /// <summary>
        /// Gets or sets a boolean value indicating whether the shape point is locked.
        /// </summary>
        public bool Locked
        {
            get { return locked; }
            set { locked = value; }
        }

        internal bool Selected = false;

        protected bool modified = false;

        [Browsable(false)]
        public bool IsModified
        {
            get { return modified; }
            set { modified = value; }
        }

        [Browsable(false)]
        public PointF Point
        {
            get { return new PointF(x, y); }
            set
            {
                x = value.X;
                y = value.Y;
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the ShapePointbase class.
        /// </summary>
        public ShapePointBase()
        {
        }
        /// <summary>
        /// Initializes a new instance of the ShapePoint class using X and Y
        /// coordinates.
        /// </summary>
        public ShapePointBase(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Initializes a new instance of the ShapePoint class using a Point structure.
        /// </summary>
        /// <param name="point"></param>
        public ShapePointBase(Point point)
        {
            this.x = point.X;
            this.y = point.Y;
        }

        /// <summary>
        /// Initializes a new instance of the ShapePoint class using an instance of the
        /// ShapePointBase class.
        /// </summary>
        /// <param name="point"></param>
		public ShapePointBase(ShapePointBase point)
		{
			this.x = point.X;
			this.y = point.Y;
			this.anchor = point.Anchor;
			this.locked = point.Locked;
		}

        /// <summary>
        /// Sets the X and Y coordinates of the shape point.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Set(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Sets the point position from a Point structure.
        /// </summary>
        /// <param name="point"></param>
        public void Set(Point point)
        {
            Set(point.X, point.Y);
        }

        /// <summary>
        /// Retrieves a Point structure corresponding to the point position.
        /// </summary>
        /// <returns></returns>
        public Point GetPoint()
        {
            return new Point((int)x, (int)y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
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

            //if ((this.anchor & AnchorStyles.Left) != 0) dstx = dst.X + x;
            //else if ((this.anchor & AnchorStyles.Right) != 0) dstx = dst.Width - (src.Width - x);
            //else dstx = dst.X + dx * dst.Width;

            if ((this.anchor & AnchorStyles.Top) != 0) dsty = dst.Y + y;
            else if ((this.anchor & AnchorStyles.Bottom) != 0) dsty = dst.Bottom - (src.Height - y);
            else dsty = dst.Y + dy * dst.Height;

            //if ((this.anchor & AnchorStyles.Top) != 0) dsty = dst.Y + x;
            //else if ((this.anchor & AnchorStyles.Bottom) != 0) dsty = dst.Height - (src.Height - y);
            //else dsty = dst.Y + dy * dst.Height;

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

        /// <summary>
        /// Retrieves a string representation of the ShapePointBase class.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Point: {0},{1}", x.ToString(), y.ToString());
        }
    }
}
