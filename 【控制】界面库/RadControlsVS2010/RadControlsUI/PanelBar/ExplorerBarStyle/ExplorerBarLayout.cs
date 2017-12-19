using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls;

namespace Telerik.WinControls.UI
{
	public class ExplorerBarLayout : PanelBarBaseLayout
	{


		protected override SizeF MeasureOverride(SizeF availableSize)
		{
			float finalHeight = 0;

			RadPanelBarElement panelBar = (RadPanelBarElement)this.Parent;
			if (panelBar == null)
				return base.MeasureOverride(availableSize);

            base.MeasureOverride(availableSize);

            float y = panelBar.TopOffset;
            float x = 1 + panelBar.LeftOffset;
            int counter = 0;
          
            for (int i = 0; i < this.Children.Count; i++)
            {
                RadPanelBarGroupElement group = (RadPanelBarGroupElement)this.Children[i];

                PointF groupLocation = new PointF(x, y);


                SizeF groupSize = new SizeF(
                (availableSize.Width - 2 - panelBar.LeftOffset - panelBar.RightOffset - ( (panelBar.NumberOfColumns - 1) * panelBar.SpacingBetweenColumns ) ) / panelBar.NumberOfColumns,
                availableSize.Height);


                //	group.Measure(availableSize);

                if (counter >= panelBar.NumberOfColumns)
                {
                    x = 1 + panelBar.LeftOffset;
                    counter = 0;

                    float maxHeight = 0;
                    for (int j = i - panelBar.NumberOfColumns; j < i; j++)
                    {
                        RadPanelBarGroupElement searchingGroup = (RadPanelBarGroupElement)this.Children[j];

                        maxHeight = Math.Max(maxHeight, searchingGroup.DesiredSize.Height);
                    }


                    y += maxHeight + panelBar.SpacingBetweenGroups;
                    groupLocation = new PointF(x, y);
                 }
                else
                {
                    x += groupSize.Width + panelBar.SpacingBetweenColumns;
                }

                int captionSize = (int)group.GetCaptionElement().DesiredSize.Height;

                RectangleF groupBounds = new RectangleF(groupLocation, groupSize);
                  group.Measure(groupBounds.Size);

                  if (group.ContentPanel.Height + captionSize > groupBounds.Height)
                  {
                      double calculatedHeight = captionSize + group.ContentPanel.Height;

                      groupBounds = new RectangleF(groupLocation, new SizeF(groupSize.Width, (float)calculatedHeight));
                      group.Measure(groupBounds.Size);
                  }

             
                finalHeight += group.DesiredSize.Height;
                counter++;
            }

			this.OnMeasureModified(EventArgs.Empty);

            
			return new SizeF(availableSize.Width, finalHeight);
		}

		protected override System.Drawing.SizeF ArrangeOverride(System.Drawing.SizeF finalSize)
		{
			//base.ArrangeOverride(finalSize);
			float finalHeight = 0;

			RadPanelBarElement panelBar = (RadPanelBarElement)this.Parent;

			if (panelBar == null)
				return base.ArrangeOverride(finalSize);

			float y = panelBar.TopOffset;
			float x = 1 + panelBar.LeftOffset;
			int counter = 0;
      
			for (int i = 0; i < this.Children.Count; i++)
			{
				RadPanelBarGroupElement group = (RadPanelBarGroupElement)this.Children[i];

				PointF groupLocation = new PointF(x, y);

				SizeF groupSize = new SizeF(
				(finalSize.Width - 2 - panelBar.LeftOffset - panelBar.RightOffset - ( (panelBar.NumberOfColumns - 1) * panelBar.SpacingBetweenColumns ) ) / panelBar.NumberOfColumns,
				group.DesiredSize.Height);


				if (counter >= panelBar.NumberOfColumns)
				{
					x = 1 + panelBar.LeftOffset;
					counter = 0;

					float maxHeight = 0;
					for (int j = i - panelBar.NumberOfColumns; j < i; j++)
					{
						RadPanelBarGroupElement searchingGroup = (RadPanelBarGroupElement)this.Children[j];

						maxHeight = Math.Max(maxHeight, searchingGroup.DesiredSize.Height - 1);
					}

					y += maxHeight + panelBar.SpacingBetweenGroups;
					groupLocation = new PointF(x, y);
					x += groupSize.Width + panelBar.SpacingBetweenColumns;

				}
				else
				{
					x += groupSize.Width + panelBar.SpacingBetweenColumns;
				}

				RectangleF groupBounds = new RectangleF(groupLocation, groupSize);

				group.Arrange(groupBounds);

				finalHeight += y;
				counter++;
			}

			this.OnArrangeModified(EventArgs.Empty);
			return finalSize;// new SizeF(finalSize.Width, Math.Max(finalSize.Height, finalHeight));
		}
	}
}
