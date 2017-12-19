using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.Layouts
{
	public class ForceSizeLayoutPanel : LayoutPanel
	{
        public readonly static RadProperty ForceChildWidthToParentProperty =
			RadProperty.Register("ForceChildWidthToParent", typeof(bool), typeof(ForceSizeLayoutPanel),
				new RadElementPropertyMetadata(BooleanBoxes.FalseBox,
				ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public readonly static RadProperty ForceChildHeightToParentProperty =
			RadProperty.Register("ForceChildHeightToParent", typeof(bool), typeof(ForceSizeLayoutPanel),
				new RadElementPropertyMetadata(BooleanBoxes.FalseBox,
				ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		[Description("")]
		[RadPropertyDefaultValue("ForceChildWidthToParent", typeof(ForceSizeLayoutPanel))]
		[Category(RadDesignCategory.LayoutCategory)]
		public bool ForceChildWidthToParent
		{
			get { return (bool)this.GetValue(ForceChildWidthToParentProperty); }
			set { this.SetValue(ForceChildWidthToParentProperty, value); }
		}

		[Description("")]
		[RadPropertyDefaultValue("ForceChildHeightToParent", typeof(ForceSizeLayoutPanel))]
		[Category(RadDesignCategory.LayoutCategory)]
		public bool ForceChildHeightToParent
		{
			get { return (bool)this.GetValue(ForceChildHeightToParentProperty); }
			set { this.SetValue(ForceChildHeightToParentProperty, value); }
		}

		protected override void OnChildrenChanged(RadElement child, ItemsChangeOperation changeOperation)
		{
			base.OnChildrenChanged(child, changeOperation);
			if (this.Children.Count > 1)
				throw new InvalidOperationException("ForceSizeLayoutPanel must have maximum one child");
		}

		public override void PerformLayoutCore(RadElement affectedElement)
		{
			if (this.ForceChildWidthToParent || this.ForceChildHeightToParent)
			{
				Size proposedSize = this.AvailableSize;

				foreach (RadElement child in this.Children)
				{
					Size prefSize = child.GetPreferredSize(proposedSize);

					if (this.ForceChildWidthToParent)
						child.Size = new Size(this.Parent.FieldSize.Width, prefSize.Height);
					if (this.ForceChildHeightToParent)
						child.Size = new Size(prefSize.Width, this.Parent.FieldSize.Height);
				}
			}
			else
			{
				base.PerformLayout(affectedElement);
			}
		}

		public override Size GetPreferredSizeCore(Size proposedSize)
		{
			Size res = base.GetPreferredSizeCore(proposedSize);

			if (this.ForceChildWidthToParent)
				res.Width = this.Parent.Size.Width;
			if (this.ForceChildHeightToParent)
				res.Height = this.Parent.Size.Height;

			return res;
		}
	}
}
