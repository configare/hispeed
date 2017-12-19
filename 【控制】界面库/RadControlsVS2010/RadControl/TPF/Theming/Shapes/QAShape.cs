using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Collections;

namespace Telerik.WinControls.Tests
{
	[ToolboxItem(false), ComVisible(false)]
	public class QAShape : ElementShape
	{
		/// <summary>
		/// Creates donut-like path. Overrides the method defined in its base class -
		/// ElementShape.
		/// </summary>
		public override GraphicsPath  CreatePath(Rectangle bounds)
		{
			bool isBorderPath = false;
			Rectangle rect = new Rectangle(bounds.X + 10, bounds.Y, bounds.Width, bounds.Height);

			int num1 = 12;
			int num2 = 0x13;
			int num3 = 0x19;
			int num4 = (int)Math.Pow((double)num3, 2);
			Point point1 = new Point(rect.Left - num3, rect.Top + num2);
			Point[] pointArray1 = new Point[num1];
			byte[] buffer1 = new byte[num1];
			int num5 = (1 + rect.Left) - num1;
			for (int num6 = 0; num6 < pointArray1.Length; num6++)
			{
				int num7 = (num6 == 0) ? num5 : (1 + pointArray1[num6 - 1].X);
				int num8 = point1.Y - ((int)Math.Sqrt(num4 - Math.Pow((double)(num7 - point1.X), 2)));
				if (num8 < rect.Top)
				{
					num8 = rect.Top;
				}
				pointArray1[num6] = new Point(num7, num8);
				buffer1[num6] = 1;
			}
			if (!isBorderPath)
			{
				ArrayList list1 = new ArrayList();
				for (int num9 = 0; num9 < pointArray1.Length; num9++)
				{
					list1.Add(pointArray1[num9]);
					if (num9 != (pointArray1.Length - 1))
					{
						int num10 = pointArray1[num9 + 1].Y - pointArray1[num9].Y;
						if (num10 > 1)
						{
							int num11 = (num10 > 4) ? ((int)Math.Round((double)(num10 / 2))) : 2;
							for (int num12 = 1; num12 < num11; num12++)
							{
								list1.Add(new Point(pointArray1[num9].X, pointArray1[num9].Y + num12));
							}
						}
					}
				}
				byte[] buffer2 = new byte[list1.Count];
				for (int num13 = 0; num13 < buffer2.Length; num13++)
				{
					buffer2[num13] = 1;
				}
				pointArray1 = (Point[])list1.ToArray(typeof(Point));
				buffer1 = buffer2;
			}
			Point[] pointArray2 = new Point[] { new Point(rect.Left, rect.Top), new Point(rect.Left - num1, rect.Top) };
			GraphicsPath path1 = new GraphicsPath();
			Point[] pointArray3 = new Point[pointArray1.Length];
			pointArray1[0] = pointArray1[1];
			for (int i = 0; i < pointArray1.Length; i++)
			{
				pointArray3[i] = pointArray1[pointArray1.Length - i - 1];
			}

            //windows 7 fix GDI+
            //p.p. 16/01/09
            GraphicsPath tempPath = new GraphicsPath();
            tempPath.AddLines(pointArray3);
            //end fix
            path1.AddPath(tempPath, true);

			path1.AddLine(bounds.Left + 10, bounds.Top, bounds.Right - 20, bounds.Top);
			if (rect.Height >= 15)
			{
				path1.AddArc(rect.Right - 20, rect.Top, 10, 20, -90, 90);
			//	path1.AddLine(rect.Right, rect.Top + 20, rect.Right, rect.Bottom - 20);

				path1.AddArc(rect.Right - 20, rect.Bottom - 20, 10, 20, 0, 90);
			}
			else
			{
				int height = rect.Height;
				if (height <= 0) height = 1;
				path1.AddArc(rect.Right - 10, rect.Top, 10, height, 270, 180);
			}

			path1.AddLine(rect.Right - 20, rect.Bottom, rect.Left , rect.Bottom);
			path1.CloseAllFigures();
			return path1;
		}

		
	}
}
