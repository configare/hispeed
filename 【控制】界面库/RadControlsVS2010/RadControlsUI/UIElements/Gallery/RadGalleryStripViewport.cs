using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
	public class RadGalleryStripViewport : RadStripViewport
	{
		private Size galleryItemsSize;

		public Size GalleryItemsSize
		{
			get { return this.galleryItemsSize; }
			set { this.galleryItemsSize = value; }
		}

		#region IScrollViewport implementation		
		
		public override Point ResetValue(Point currentValue, Size viewportSize, Size canvasSize)
		{
			Point res = Point.Empty;
			res.X = RadCanvasViewport.ValidatePosition(currentValue.X,
				canvasSize.Width - viewportSize.Width);
			res.Y = RadCanvasViewport.ValidatePosition(currentValue.Y,
				canvasSize.Height - viewportSize.Height);			
			return res;			
		}
				
		public override void DoScroll(Point oldValue, Point newValue)
		{
			this.PositionOffset = new SizeF(-newValue.X, -newValue.Y);			
		}

		public override Size ScrollOffsetForChildVisible(RadElement childElement, Point currentScrollValue)
		{
			Rectangle clientRect = new Rectangle(Point.Empty, this.Size);
			Rectangle childRect = childElement.Bounds;
			childRect.Offset((int)Math.Round(this.PositionOffset.Width), (int)Math.Round(this.PositionOffset.Height));
			Size childOffset = RadCanvasViewport.CalcMinOffset(childRect, clientRect);
			Size viewportOffset = new Size(-childOffset.Width, -childOffset.Height);
			return viewportOffset;			
		}

		public override ScrollPanelParameters GetScrollParams(Size viewportSize, Size canvasSize)
		{
			return new ScrollPanelParameters(
				0, Math.Max(1, canvasSize.Width), this.galleryItemsSize.Width, Math.Max(1, viewportSize.Width),
				0, Math.Max(1, canvasSize.Height), this.galleryItemsSize.Height, Math.Max(1, viewportSize.Height)
			);			
		}
		#endregion		

        //public override void PerformLayoutCore(RadElement affectedElement)
        //{			
        //    foreach (ForceSizeLayoutPanel item in this.Children)
        //    {
        //        ((RadGalleryGroupItem)item.Children[0]).ItemsLayoutPanel.ChildrenForcedSize = this.galleryItemsSize;
        //    }
        //    base.PerformLayoutCore(affectedElement);
        //}
	}
}
