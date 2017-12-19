    
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.UI;

namespace Telerik.WinControls.UI
{
	public class RadSpinArrowElement : RadItem
	{
		private ArrowPrimitive arrow;

		public RadSpinArrowElement( ArrowDirection direction )			
		{
			this.Arrow.Visibility = ElementVisibility.Visible;
            this.Arrow.Direction = direction;
		}

		public ArrowPrimitive Arrow
		{
			get
			{
				return arrow;
			}
		}

		protected override void CreateChildElements()
		{
			arrow = new ArrowPrimitive();
			arrow.Visibility = ElementVisibility.Hidden;
			this.Children.Add(arrow);
		}

        protected override SizeF ArrangeOverride(SizeF finalSize)
		{
			arrow.Arrange(new RectangleF(finalSize.Width - 12, (finalSize.Height - 6)/2, 6, 6));
			return finalSize;
		}
	}
}
