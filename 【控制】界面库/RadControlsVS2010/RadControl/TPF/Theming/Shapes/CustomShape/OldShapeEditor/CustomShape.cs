using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;

namespace Telerik.WinControls.OldShapeEditor
{
    /// <summary>Represents custom shape of an element.</summary>
    //[Designer(typeof(CustomShapeDesigner))]
    //[DesignTimeVisible(false)]
    [ToolboxItem(false), DesignTimeVisible(false)]
    public class CustomShape : ElementShape
    {
        /// <summary>Initializes a new instance of the CustomShape class.</summary>
        public CustomShape()
        {
			points = new List<ShapePoint>();
        }

        /*/// <summary>Initializes a new instance of the CustomShape class using a container.</summary>
        public CustomShape(IContainer container)
        {
            container.Add(this);
        }*/
        
        List<ShapePoint> points = new List<ShapePoint>();
        /// <summary>Gets a List of Shape points.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<ShapePoint> Points
        {
            get { return points; }
        }

        Rectangle dimension;
        /// <summary>Gets or sets a Rectangle indicating the dimension of the shape.</summary>
        public Rectangle Dimension
        {
            get { return dimension; }
            set { dimension = value; }
        }

        /// <summary>Creates a path using a ractangle for bounds.</summary>
        public override GraphicsPath CreatePath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();
            
            for (int i = 0; i < points.Count; i++)
            {
                ShapePoint p1 = points[i];
                ShapePoint p2 = i < points.Count - 1 ? points[i + 1] : points[0];

                Point rp1 = p1.GetPoint(dimension, bounds);
                Point rp2 = p2.GetPoint(dimension, bounds);

                if (p1.Bezier)
                    path.AddBezier(rp1, p1.ControlPoint1.GetPoint(dimension, bounds),
                        p1.ControlPoint2.GetPoint(dimension, bounds), rp2);
                else
                    path.AddLine(rp1, rp2);
            }
            path.CloseAllFigures();
        
            return path;
        }

        /// <summary>Serializes properties. Required for telerik serialization mechanism.</summary>
        public override string SerializeProperties()
        {
            string s = string.Format("{0},{1},{2},{3}:", dimension.X, dimension.Y, dimension.Width, dimension.Height);
            foreach(ShapePoint point in points)
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
        /// <summary>Deserializes properties. Required for telerik deserialization mechanism.</summary>
        public override void DeserializeProperties(string propertiesString)
        {
            string[] tokens = propertiesString.Split(':');
            
            string[] strdim = tokens[0].Split(',');
            dimension = new Rectangle(int.Parse(strdim[0]), int.Parse(strdim[1]), 
                int.Parse(strdim[2]), int.Parse(strdim[3]));

            for (int i = 1; i < tokens.Length; i++)
            {
                string[] strpt = tokens[i].Split(',');
                if (strpt.Length > 2)
                {
                    ShapePoint point = new ShapePoint(int.Parse(strpt[0]), int.Parse(strpt[1]));
                    point.Bezier = bool.Parse(strpt[2]);
                    point.ControlPoint1.X = int.Parse(strpt[3]);
                    point.ControlPoint1.Y = int.Parse(strpt[4]);
                    point.ControlPoint2.X = int.Parse(strpt[5]);
                    point.ControlPoint2.Y = int.Parse(strpt[6]);
                    point.Anchor = (AnchorStyles)int.Parse(strpt[7]);
                    points.Add(point);
                }
            }
        }
    }
}
