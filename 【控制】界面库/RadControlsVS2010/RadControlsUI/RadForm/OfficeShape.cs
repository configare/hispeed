using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI
{
    /// <summary>Represents the shape of the MS Office forms.</summary>
	[ToolboxItem(false), ComVisible(false)]
	public class OfficeShape : ElementShape
	{
        private bool roundedBottom;

        public OfficeShape()
        {
        }

        public OfficeShape(bool roundedBottom)
        {
            this.roundedBottom = roundedBottom;
        }

        /// <summary><para>Gets or sets whether the bottom edges of the form should be rounded.</para></summary>
        [Description("Gets or sets whether the bottom edges of the form should be rounded.")]
        [DefaultValue(false)]
        [Category("Appearance")]
        public bool RoundedBottom
        {
            get { return this.roundedBottom; }
            set { this.roundedBottom = value; }
        }

        public GraphicsPath GetContourPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();

            if (bounds.Height <= 0 || bounds.Width <= 0)
                return path;
            if (bounds.Height < 10 || bounds.Width < 10)
            {
                RoundRectShape rrs = new RoundRectShape(5);
                path = rrs.CreatePath(bounds);
                return path;
            }


            GraphicsPath res = new GraphicsPath();

            Point[] points = null;
            
            if (this.RoundedBottom)
            {
                points = GetRoundedBottomContour(bounds);
            }
            else
            {
                points = GetCutBottomContour(bounds);
            }
            
            res.AddLines(points);

            return res;
        }

        /// <summary>Greates the path.</summary>
		public override GraphicsPath CreatePath(Rectangle bounds)
		{
			GraphicsPath path = new GraphicsPath();

			if (bounds.Height <= 0 || bounds.Width <= 0)
				return path;
            if (bounds.Height < 10 || bounds.Width < 10)
            {
                RoundRectShape rrs = new RoundRectShape(5);
                path = rrs.CreatePath(bounds);
                return path;
            }

            Rectangle[] rects = GetRectangles(bounds);
            path.AddRectangles(rects);

            //Point[] points = GetRoundedBottomContour(bounds);
            //path.AddLines(points);

			//path.CloseFigure();

			return path;
		}

        protected override Rectangle GetBounds(RadElement element)
        {
            return new Rectangle(Point.Empty, element.Size);
        }

        /// <summary>Serializes properties. Required for telerik serialization mechanism.</summary>
        public override string SerializeProperties()
        {
            string res = this.RoundedBottom.ToString();

            return res;
        }

        /// <summary>Deserializes properties. Required for telerik deserialization mechanism.</summary>
        public override void DeserializeProperties(string propertiesString)
        {
            if (string.IsNullOrEmpty(propertiesString))
                return;

            this.RoundedBottom = bool.Parse(propertiesString);
        }

        private Rectangle[] GetRectangles(Rectangle bounds)
        {
            Rectangle[] rects = null;

            if (this.RoundedBottom)
            {
                rects = new Rectangle[] {
                    new Rectangle(bounds.X + 4, bounds.Y + 0,       bounds.Width - 8, 1),
                    new Rectangle(bounds.X + 2, bounds.Y + 1,       bounds.Width - 4, 1),
                    new Rectangle(bounds.X + 1, bounds.Y + 2,       bounds.Width - 2, 2),

                    new Rectangle(bounds.X + 0, bounds.Y + 4,       bounds.Width, bounds.Height - 8),
                    
                    new Rectangle(bounds.X + 1, bounds.Bottom - 4,  bounds.Width - 2, 2),
                    new Rectangle(bounds.X + 2, bounds.Bottom - 2,  bounds.Width - 4, 1),
                    new Rectangle(bounds.X + 4, bounds.Bottom - 1,  bounds.Width - 8, 1)
                };
            }
            else
            {
                rects = new Rectangle[] {
                    new Rectangle(bounds.X + 4, bounds.Y + 0, bounds.Width - 8, 1),
                    new Rectangle(bounds.X + 2, bounds.Y + 1, bounds.Width - 4, 1),
                    new Rectangle(bounds.X + 1, bounds.Y + 2, bounds.Width - 2, 2),

                    new Rectangle(bounds.X + 0, bounds.Y + 4, bounds.Width, bounds.Height - 4)
                };
            }

            return rects;
        }

        private Point[] GetRoundedBottomContour(Rectangle bounds)
        {
            //Rectangle[] rects = GetRectangles(bounds);

            //List<Point> points = new List<Point>();
            //int middleRectIndex = rects.Length / 2;

            /*for (int i = 0; i <= middleRectIndex; i++)
            {
                points.Add(rects[i].Location);
            }
            points.Add(new Point(rects[middleRectIndex].X, rects[middleRectIndex].Bottom));
            for (int i = middleRectIndex + 1; i < rects.Length; i++)
            {
                points.Add(new Point(rects[i].X, rects[i].Bottom));
            }
            points.Add(new Point(rects[rects.Length - 1].Right, rects[rects.Length - 1].Bottom));
            for (int i = rects.Length - 1; i > middleRectIndex; i--)
            {
                points.Add(new Point(rects[i].Right, rects[i].Bottom));
            }
            points.Add(new Point(rects[middleRectIndex].Right, rects[middleRectIndex].Bottom));
            for (int i = middleRectIndex; i >= 0; i--)
            {
                points.Add(new Point(rects[i].Right, rects[i].Y));
            }
            points.Add(rects[0].Location);*/



            /*points.Add(rects[0].Location);
            for (int i = 1; i <= middleRectIndex; i++)
            {
                points.Add(new Point(rects[i - 1].X, rects[i].Y));
                points.Add(rects[i].Location);
            }
            points.Add(new Point(rects[middleRectIndex].X, rects[middleRectIndex].Bottom));
            for (int i = middleRectIndex + 1; i < rects.Length; i++)
            {
                points.Add(new Point(rects[i].X, rects[i-1].Bottom));
                points.Add(new Point(rects[i].X, rects[i].Bottom));
            }
            points.Add(new Point(rects[rects.Length - 1].Right, rects[rects.Length - 1].Bottom));
            for (int i = rects.Length - 1; i > middleRectIndex; i--)
            {
                points.Add(new Point(rects[i].Right, rects[i - 1].Bottom));
                points.Add(new Point(rects[i - 1].Right, rects[i - 1].Bottom));
            }
            points.Add(new Point(rects[middleRectIndex].Right, rects[middleRectIndex].Y));
            for (int i = middleRectIndex; i > 0; i--)
            {
                points.Add(new Point(rects[i - 1].Right, rects[i].Y));
                points.Add(new Point(rects[i - 1].Right, rects[i - 1].Y));
            }
            points.Add(rects[0].Location);*/

            //return points.ToArray();

            Point[] res = new Point[] {
                new Point(bounds.X + 4, bounds.Y),
                new Point(bounds.X + 3, bounds.Y + 1),
                new Point(bounds.X + 2, bounds.Y + 1),
                new Point(bounds.X + 1, bounds.Y + 2),
                new Point(bounds.X + 1, bounds.Y + 3),
                new Point(bounds.X, bounds.Y + 4),
                new Point(bounds.X, bounds.Bottom - 4),
                new Point(bounds.X + 1, bounds.Bottom - 3),
                new Point(bounds.X + 1, bounds.Bottom - 2),
                new Point(bounds.X + 2, bounds.Bottom - 1),
                new Point(bounds.X + 3, bounds.Bottom - 1),
                new Point(bounds.X + 4, bounds.Bottom),
                new Point(bounds.Right - 4, bounds.Bottom),
                new Point(bounds.Right - 3, bounds.Bottom - 1),
                new Point(bounds.Right - 2, bounds.Bottom - 1),
                new Point(bounds.Right - 1, bounds.Bottom - 2),
                new Point(bounds.Right - 1, bounds.Bottom - 3),
                new Point(bounds.Right, bounds.Bottom - 4),
                new Point(bounds.Right, bounds.Y + 4),
                new Point(bounds.Right - 1, bounds.Y + 3),
                new Point(bounds.Right - 1, bounds.Y + 2),
                new Point(bounds.Right - 2, bounds.Y + 1),
                new Point(bounds.Right - 3, bounds.Y + 1),
                new Point(bounds.Right - 4, bounds.Y),
                new Point(bounds.X + 4, bounds.Y)
            };

            return res;
        }

        private Point[] GetCutBottomContour(Rectangle bounds)
        {
            /*Rectangle[] rects = GetRectangles(bounds);

            List<Point> points = new List<Point>();
            for (int i = 0; i < rects.Length; i++)
            {
                points.Add(rects[i].Location);
            }
            points.Add(new Point(rects[rects.Length - 1].X, rects[rects.Length - 1].Bottom));
            points.Add(new Point(rects[rects.Length - 1].Right, rects[rects.Length - 1].Bottom));
            points.Add(new Point(rects[rects.Length - 1].Right, rects[rects.Length - 1].Y));

            for (int i = rects.Length - 1; i >= 0; i--)
            {
                points.Add(new Point(rects[i].Right, rects[i].Y));
            }
            points.Add(rects[0].Location);

            return points.ToArray();*/

            Point[] res = new Point[] {
                new Point(bounds.X + 4, bounds.Y),
                new Point(bounds.X + 3, bounds.Y + 1),
                new Point(bounds.X + 2, bounds.Y + 1),
                new Point(bounds.X + 1, bounds.Y + 2),
                new Point(bounds.X + 1, bounds.Y + 3),
                new Point(bounds.X, bounds.Y + 4),
                new Point(bounds.X, bounds.Bottom),
                new Point(bounds.Right, bounds.Bottom),
                new Point(bounds.Right, bounds.Y + 4),
                new Point(bounds.Right - 1, bounds.Y + 3),
                new Point(bounds.Right - 1, bounds.Y + 2),
                new Point(bounds.Right - 2, bounds.Y + 1),
                new Point(bounds.Right - 3, bounds.Y + 1),
                new Point(bounds.Right - 4, bounds.Y),
                new Point(bounds.X + 4, bounds.Y)
            };

            return res;
        }
	}	
}