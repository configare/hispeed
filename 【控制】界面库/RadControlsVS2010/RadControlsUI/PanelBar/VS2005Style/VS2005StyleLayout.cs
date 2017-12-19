using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls;

namespace Telerik.WinControls.UI
{
	public class VS2005StyleLayout : PanelBarBaseLayout
	{
		protected override SizeF MeasureOverride(SizeF availableSize)
		{
			float finalHeight = 0;

			RadPanelBarElement panelBar = (RadPanelBarElement)this.Parent;

			float y = 0;
			float x = 1;

			for (int i = 0; i < this.Children.Count; i++)
			{
				RadPanelBarGroupElement group = (RadPanelBarGroupElement)this.Children[i];

				PointF groupLocation = new PointF(x, y);

				SizeF groupSize = new SizeF(
				availableSize.Width - 2,
				availableSize.Height);
	
				RectangleF groupBounds = new RectangleF(groupLocation, groupSize);

				group.Measure(groupBounds.Size);
		
				finalHeight += y;
			}

			this.OnMeasureModified(EventArgs.Empty);

			return availableSize;
		}

		protected override System.Drawing.SizeF ArrangeOverride(System.Drawing.SizeF finalSize)
		{
			//base.ArrangeOverride(finalSize);
			float finalHeight = 0;

			RadPanelBarElement panelBar = (RadPanelBarElement)this.Parent;

			float y = 0;
			float x = 1;

			for (int i = 0; i < this.Children.Count; i++)
			{
				RadPanelBarGroupElement group = (RadPanelBarGroupElement)this.Children[i];
				
				PointF groupLocation = new PointF(x, y);

					SizeF groupSize = new SizeF(
					finalSize.Width - 2,
					group.DesiredSize.Height);

					y +=group.DesiredSize.Height - 1;
	
				RectangleF groupBounds = new RectangleF(groupLocation, groupSize);
	
				group.Arrange(groupBounds);
	
				finalHeight += y;
			}

			this.OnArrangeModified(EventArgs.Empty);
			return finalSize;
		}	
	}
}
