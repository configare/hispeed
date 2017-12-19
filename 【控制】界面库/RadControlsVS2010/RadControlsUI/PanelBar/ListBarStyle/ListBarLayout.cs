using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using System.Drawing;

namespace Telerik.WinControls.UI
{
	public class ListBarLayout : PanelBarBaseLayout
	{
		protected override System.Drawing.SizeF ArrangeOverride(System.Drawing.SizeF finalSize)
		{
			float y = 0;	
			float groupsHeight = 0;

			for (int i = 0; i < this.Children.Count; i++)
			{
				RadPanelBarGroupElement group = (RadPanelBarGroupElement)this.Children[i];
				groupsHeight += group.CaptionHeight - 1;
			}

			for (int i = 0; i < this.Children.Count; i++)
			{
			RadPanelBarGroupElement group = (RadPanelBarGroupElement)this.Children[i];
				PointF groupLocation = new PointF(1, y);
				SizeF groupSize = new SizeF(
					finalSize.Width - 2,
					group.CaptionHeight-1);

               

				if (group.Expanded)
				{
					float preferredHeight = 0;
					for (int j = 0; j < group.Items.Count; j++)
					{
						preferredHeight += group.Items[j].DesiredSize.Height;
					}

					groupSize = new SizeF(
						finalSize.Width - 2,
						finalSize.Height - groupsHeight + group.CaptionHeight - 3);
				}

				RectangleF groupBounds = new RectangleF(groupLocation, groupSize);
				group.Arrange(groupBounds);
				y += groupSize.Height;
			}

			this.OnArrangeModified(EventArgs.Empty);
			return finalSize;
		}

	}
}
