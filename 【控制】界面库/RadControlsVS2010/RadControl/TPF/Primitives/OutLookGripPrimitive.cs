using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Paint;
using System.Drawing;

namespace Telerik.WinControls.Primitives
{
	public class OutLookGripPrimitive : FillPrimitive
	{
		protected override void CreateChildElements()
		{
			base.CreateChildElements();
			this.ZIndex = 10000;
		}

		public override void PaintPrimitive(IGraphics g, float angle, SizeF scale)
		{
			base.PaintPrimitive(g, angle, scale);
			Rectangle rect = new Rectangle(Point.Empty, this.Size);

            //int dotsOffset = 2;

			RectangleF chunkbar = new RectangleF(this.Size.Width / 2 - 18, 4, 2F, 2F);

			for (int i = 0; i < 9; i++)
			{
				RectangleF shadowRect = new RectangleF(chunkbar.X + 0.1F, chunkbar.Y + 0.1F, 2F, 2F);

				g.FillRectangle(shadowRect, Color.White);
				g.FillRectangle(chunkbar, Color.Black);
				chunkbar.X += 4;
			}
		}
	}
}
