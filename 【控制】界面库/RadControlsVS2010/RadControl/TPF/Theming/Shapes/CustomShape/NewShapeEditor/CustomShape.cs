using System;
using System.Collections.Generic;
using Telerik.WinControls;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Telerik.WinControls
{
    [Designer(DesignerConsts.CustomShapeDesignerString)]
    [DesignTimeVisible(false), ToolboxItem(false)]
    public class CustomShape : ElementShape
    {
        #region Private Data

        private ShapeLinesCollection shape;

        private Rectangle dimension;

        #endregion

        #region Constructors

        public CustomShape()
        {
            shape = new ShapeLinesCollection();
        }

        public CustomShape(Rectangle rect)
        {
            shape = new ShapeLinesCollection();

            dimension = rect;

            AddLine(rect.Location, new PointF(rect.Right, rect.Top));
            AppendLine(new PointF(rect.Right, rect.Bottom));
            AppendLine(new PointF(rect.Left, rect.Bottom));
            CloseFigureUsingLine();

        }

        public CustomShape(IContainer container)
        {
            shape = new ShapeLinesCollection();
            container.Add(this);
        }

        #endregion

        #region Accessors

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle Dimension
        {
            get { return dimension; }
            set { dimension = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ShapeLinesCollection Shape
        {
            get { return shape; }
            set
            {
                if (value != null) shape = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] 
        public string AsString
        {
            get { return SerializeProperties(); }
            set { DeserializeProperties(value); }
        }

        #endregion

        #region Copy Methods

        public CustomShape Clone()
        {
            CustomShape cs = new CustomShape();

            cs.shape.CopyFrom(shape);
            cs.dimension = new Rectangle(dimension.Location, dimension.Size);

            return cs;
        }

        public void CopyFrom(CustomShape cs)
        {
            if (cs == null || cs.Dimension == null) return;

            shape.CopyFrom(cs.shape);
            dimension = new Rectangle(cs.Dimension.Location, cs.Dimension.Size);
        }

        #endregion

        #region Serialization / Deserialization

        /// <summary>Serializes properties. Required for telerik serialization mechanism.</summary>
        public override string SerializeProperties()
        {
            string res = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3}:", dimension.X, dimension.Y, dimension.Width, dimension.Height);

            res += shape.SerializeProperties();
            
            return res;
        }
        /// <summary>Deserializes properties. Required for telerik deserialization mechanism.</summary>
        public override void DeserializeProperties(string propertiesString)
        {
            shape.Reset();

            string[] tokens = propertiesString.Split(':');

            string[] strdim = tokens[0].Split(',');
            if (strdim.Length < 4)
                return;

            dimension = new Rectangle(
                (int)Math.Round(float.Parse(strdim[0], CultureInfo.InvariantCulture)),
                (int)Math.Round(float.Parse(strdim[1], CultureInfo.InvariantCulture)),
                (int)Math.Round(float.Parse(strdim[2], CultureInfo.InvariantCulture)),
                (int)Math.Round(float.Parse(strdim[3], CultureInfo.InvariantCulture))
                );

            shape.DeserializeProperties(propertiesString);
        }

        #endregion

        #region Shape Path & Dimension Methods

        public override GraphicsPath CreatePath(Rectangle bounds)
        {
            return shape.CreatePath(dimension, bounds);
        }

        public RectangleF GetBoundingRectangle()
        {
            return shape.GetBoundingRect();
        }

        public bool DoFixDimension(bool forceFix)
        {
            if ((!dimension.IsEmpty) && (!forceFix)) return true;

            RectangleF brect = shape.GetBoundingRect();

            if (brect.IsEmpty) return false;

            dimension = new Rectangle(
                (int) Math.Round(brect.X),
                (int) Math.Round(brect.Y),
                (int) Math.Round(brect.Width),
                (int) Math.Round(brect.Height)
                );

            return true;
        }

        public bool DoFixDimension()
        {
            return DoFixDimension(false);
        }

        #endregion

        #region Shape Modification Methods

        public void AddLine(PointF from, PointF to)
        {
            ShapeLine line = new ShapeLine(new ShapePoint(from), new ShapePoint(to));

            shape.Add(line);
        }

        public void AddBezier(PointF from, PointF ctrl1, PointF ctrl2, PointF to)
        {
            ShapeBezier curve = new ShapeBezier(
                new ShapePoint(from),
                new ShapePoint(ctrl1),
                new ShapePoint(ctrl2),
                new ShapePoint(to)
                );

            shape.Add(curve);
        }

        public bool AppendLine(PointF to)
        {
            ShapePoint pt = shape.GetLastPoint();

            if (pt == null) return false;

            ShapeLine line = new ShapeLine(pt, new ShapePoint(to));

            shape.Add(line);

            return true;
        }

        public bool AppendBezier(PointF ctrl1, PointF ctrl2, PointF to)
        {
            ShapePoint pt = shape.GetLastPoint();

            if (pt == null) return false;

            ShapeBezier curve = new ShapeBezier(
                pt, 
                new ShapePoint(ctrl1), 
                new ShapePoint(ctrl2), 
                new ShapePoint(to)
                );

            shape.Add(curve);

            return true;
          
        }

        public bool CloseFigureUsingLine()
        {
            ShapePoint first = shape.GetFirstPoint();
            ShapePoint last = shape.GetLastPoint();

            if ( (first == null) || (last == null) )                 
                return false;

            ShapeLine line = new ShapeLine(last, first);

            shape.Add(line);

            return true;
        }

        public bool CloseFigureUsingBezier(PointF ctrl1, PointF ctrl2)
        {
            ShapePoint first = shape.GetFirstPoint();
            ShapePoint last = shape.GetLastPoint();

            if ((first == null) || (last == null))
                return false;

            ShapeBezier curve = new ShapeBezier(
                last,
                new ShapePoint(ctrl1),
                new ShapePoint(ctrl2), 
                first
                );

            shape.Add(curve);

            return true;
        }

        #region Create Shape Methods 

        // Closed Shape
        protected bool CreateClosedShape(ShapePoint[] pts)
        {
            int count = pts.Length;

            if (count < 2) return false;

            shape.Reset();

            for (int i = 0; i < count; i++)
                shape.Add(new ShapeLine(pts[i], pts[(i + 1) % count]));

            return DoFixDimension(true);
        }

        public bool CreateClosedShape(PointF[] points)
        {
            if (points == null) return false;
            if (points.Length < 2) return false;

            ShapePoint[] pts = new ShapePoint[points.Length];

            for (int i = 0; i < pts.Length; i++)
                pts[i] = new ShapePoint(points[i]);

            return CreateClosedShape(pts);
        }

        public bool CreateClosedShape(List<PointF> points)
        {
            if (points == null) return false;
            if (points.Count < 2) return false;

            ShapePoint[] pts = new ShapePoint[points.Count];

            for (int i = 0; i < pts.Length; i++)
                pts[i] = new ShapePoint(points[i]);

            return CreateClosedShape(pts);
        }

        //Rectangle Shape
        public void CreateRectangleShape(PointF from, PointF to)
        {
            ShapePoint[] pt = new ShapePoint[4]
                {
                    new ShapePoint(from), new ShapePoint(to.X, from.Y),
                    new ShapePoint(to), new ShapePoint(from.X, to.Y)
                };

            CreateClosedShape(pt);
        }

        public void CreateRectangleShape(float x, float y, float width, float height)
        {
            CreateRectangleShape(new PointF(x, y), new PointF(x + width, y + height));
        }

        public void CreateRectangleShape(PointF pos, SizeF size)
        {
            CreateRectangleShape(
                new PointF(pos.X, pos.Y), 
                new PointF(pos.X + size.Width, pos.Y + size.Height)
                );
        }

        public void CreateRectangleShape(Rectangle rect)
        {
            CreateRectangleShape(rect.Location, new PointF(rect.Right, rect.Bottom));
        }

        #endregion

        #endregion
    }
}
