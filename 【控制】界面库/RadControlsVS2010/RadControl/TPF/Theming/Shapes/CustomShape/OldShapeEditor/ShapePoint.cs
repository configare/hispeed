using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace Telerik.WinControls.OldShapeEditor
{
	/// <summary>
	/// Represents a shape point.
	/// </summary>
	public class ShapePoint : ShapePointBase
	{
		/// <summary>
		/// Exposes the line direction.
		/// </summary>
		internal enum LineDirections
		{
			South, Nord, East, West, SouthEast, SouthWest, NordEast, NordWest
		}

		/// <summary>
		/// Exposes the line position.
		/// </summary>
		internal enum LinePositions
		{
			/// <summary>
			/// Indicates horizontal position.
			/// </summary>
			Horizontal,

			/// <summary>
			/// Indicates vertical position.
			/// </summary>
			Vertical
		}

		ShapePointBase controlPoint1 = new ShapePointBase();

		/// <summary>
		/// Gets or sets the first control point.
		/// </summary>
		[Category(RadDesignCategory.AppearanceCategory), Description("the bezier curve control point 1")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ShapePointBase ControlPoint1
		{
			get { return controlPoint1; }
			set { controlPoint1 = value; }
		}

		ShapePointBase controlPoint2 = new ShapePointBase();
		/// <summary>
		/// Gets or sets the second control point.
		/// </summary>
		[Category(RadDesignCategory.AppearanceCategory), Description("The bezier curve control point 2")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ShapePointBase ControlPoint2
		{
			get { return controlPoint2; }
			set { controlPoint2 = value; }
		}

		bool bezier = false;
		[DefaultValue(false), Category(RadDesignCategory.AppearanceCategory), Description("Determines if this point marks the begin of a bezier curve")]
		public bool Bezier
		{
			get { return bezier; }
			set { bezier = value; }
		}
		/// <summary>
		/// Initializes a new instance of the ShapePoint class.
		/// </summary>
		public ShapePoint()
		{
		}

		/// <summary>
		/// Initializes a new instance of the ShapePoint class from
		/// the X and Y coordinates of the point.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public ShapePoint(int x, int y)
			: base(x, y)
		{
		}

		/// <summary>
		/// Initializes a new instance of the ShapePoint class from a Point structure.
		/// </summary>
		public ShapePoint(Point point)
			: base(point)
		{
		}

		/// <summary>
		/// Initializes a new instance of the ShapePoint class using a ShapePoint instance.
		/// </summary>
		/// <param name="point"></param>
		public ShapePoint(ShapePoint point)
			: base(point)
		{
			this.ControlPoint1 = new ShapePointBase(point.ControlPoint1);
			this.ControlPoint2 = new ShapePointBase(point.ControlPoint2);
			this.Bezier = point.bezier;
		}

		/// <summary>
		/// Retrieves the line direction of the line that passes through the instance
		/// point and the point given as an argument.
		/// </summary>
		/// <param name="nextPoint"></param>
		/// <returns></returns>
		LineDirections GetLineDirection(ShapePointBase nextPoint)
		{
			if (X == nextPoint.X)
				if (Y < nextPoint.Y) return LineDirections.South;
				else return LineDirections.Nord;
			else if (Y == nextPoint.Y)
				if (X < nextPoint.X) return LineDirections.West;
				else return LineDirections.East;
			else
				if (X < nextPoint.X)
					if (Y < nextPoint.Y) return LineDirections.SouthWest;
					else return LineDirections.NordWest;
				else
					if (Y < nextPoint.Y) return LineDirections.NordEast;
					else return LineDirections.SouthEast;
		}  

		/// <summary>
		/// Creates a Bezier curve between the current point and the point given as a
		/// parameter.
		/// </summary>
		/// <param name="nextPoint"></param>
		public void CreateBezier(ShapePointBase nextPoint)
		{
			LineDirections dir = GetLineDirection(nextPoint);

			ControlPoint1.Set(X + 10, Y);
			ControlPoint2.Set(nextPoint.X - 10, nextPoint.Y);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="nextPoint"></param>
		/// <returns></returns>
		public Point[] GetCurve(ShapePoint nextPoint)
		{
			double x1 = x, x2 = nextPoint.X, cx1 = controlPoint1.X, cx2 = controlPoint2.X;
			double y1 = y, y2 = nextPoint.Y, cy1 = controlPoint1.Y, cy2 = controlPoint2.Y;

			/* calculate the polynomial coefficients */
			double cx = 3.0 * (cx1 - x1);
			double bx = 3.0 * (cx2 - cx1) - cx;
			double ax = x2 - x1 - cx - bx;

			double cy = 3.0 * (cy1 - y1);
			double by = 3.0 * (cy2 - cy1) - cy;
			double ay = y2 - y1 - cy - by;

			/* calculate the curve point at parameter value t */
			Point[] points = new Point[10];
			for (int i = 0; i < 10; i++)
			{
				double t = i / 9.0;
				double tSquared = t * t;
				double tCubed = tSquared * t;

				points[i] = new Point(
					(int)((ax * tCubed) + (bx * tSquared) + (cx * t) + x1),
					(int)((ay * tCubed) + (by * tSquared) + (cy * t) + y1));
			}

			return points;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="nextPoint"></param>
		/// <param name="pt"></param>
		/// <param name="width"></param>
		/// <returns></returns>
		public bool IsVisible(ShapePoint nextPoint, Point pt, int width)
		{
			if (bezier)
				return IsCurveVisible(GetCurve(nextPoint), pt, width);
			else
				return IsLineVisible(GetPoint(), nextPoint.GetPoint(), pt, width);
		}

		bool IsLineVisible(Point pt1, Point pt2, Point pt, double radius)
		{
			double A = pt1.Y - pt2.Y;
			double B = pt2.X - pt1.X;
			double C = pt1.X * pt2.Y - pt2.X * pt1.Y;
			double D = (A * pt.X + B * pt.Y + C) / Math.Sqrt(A * A + B * B);
			if (Math.Abs(D) < radius)
			{
				double _x1 = Math.Min(pt1.X, pt2.X) - radius, _x2 = Math.Max(pt1.X, pt2.X) + radius;
				double _y1 = Math.Min(pt1.Y, pt2.Y) - radius, _y2 = Math.Max(pt1.Y, pt2.Y) + radius;
				if (_x1 <= pt.X && pt.X <= _x2 && _y1 <= pt.Y && pt.Y <= _y2)
					return true;
			}

			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="points"></param>
		/// <param name="pt"></param>
		/// <param name="radius"></param>
		/// <returns></returns>
		bool IsCurveVisible(Point[] points, Point pt, double radius)
		{
			for (int i = 0; i < points.Length - 1; i++)
			{
				Point lpt1 = points[i];
				Point lpt2 = points[i + 1];

				if (IsLineVisible(lpt1, lpt2, pt, radius))
					return true;
			}

			return false;
		}
	}
}
