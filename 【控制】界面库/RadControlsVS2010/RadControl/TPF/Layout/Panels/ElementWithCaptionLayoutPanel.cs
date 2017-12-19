using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Primitives;
using System.Windows.Forms;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.Layouts
{
    /// <summary>ElementWithCaptionLayoutPanel is a container for elements with a caption.</summary>
	public class ElementWithCaptionLayoutPanel : LayoutPanel
	{
		public static RadProperty CaptionElementProperty = RadProperty.Register(
			"CaptionElement", typeof(bool), typeof(ElementWithCaptionLayoutPanel), new RadElementPropertyMetadata(
				false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty CaptionOnTopProperty = RadProperty.Register(
			"CaptionOnTop", typeof(bool), typeof(ElementWithCaptionLayoutPanel), new RadElementPropertyMetadata(
				false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty ShowCaptionProperty = RadProperty.Register("ShowCaption", typeof(bool), typeof(ElementWithCaptionLayoutPanel),
			new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		RadElement captionElement = null;
		RadElement bodyElement = null;

        /// <summary>
        /// Gets or sets a boolean value indicating whether there is a caption on the
        /// top.
        /// </summary>
		public bool CaptionOnTop
		{
			get
			{
				return (bool) this.GetValue(CaptionOnTopProperty);
			}
			set
			{
				this.SetValue(CaptionOnTopProperty, value);
			}
		}

		[RadPropertyDefaultValue("ShowCaption", typeof(ElementWithCaptionLayoutPanel))]
		public bool ShowCaption
		{
			get
			{				
				return (bool)this.GetValue(ShowCaptionProperty);
			}
			set
			{
				InitializeElements();
				this.SetValue(ShowCaptionProperty, value);
			}
		}


        /// <summary>
        /// Performs layout changes based on the element given as a paramater. 
        /// Sizes and places are determined by the concrete layout panel that is used. 
        /// Since all layout panels update their layout automatically through events, 
        /// this function is rarely used directly.
        /// </summary>
		public override void PerformLayoutCore(RadElement affectedElement)
		{
			Rectangle bounds = this.Parent.Bounds;

			Size fixedSize = this.ParentFixedSize;

			InitializeElements();
			Size captionSize = Size.Empty;
			Size bodySize = Size.Empty;

            int captionHeight = 8;

            if (captionElement != null)
			{
				captionSize = captionElement.GetPreferredSize(captionSize);
				if (captionElement.Visibility != ElementVisibility.Collapsed)
					captionSize.Height = Math.Max(captionHeight, captionSize.Height);
			}

			if (bodyElement != null)
			{
				if (this.Parent.AutoSize && (this.MinSize != Size.Empty))
                    bodyElement.MinSize = new Size(this.MinSize.Width, this.MinSize.Height - captionHeight);

				bodySize = new Size(bounds.Width, bounds.Height - captionSize.Height);
				bodySize = bodyElement.GetPreferredSize(bodySize);
				
				if(bodyElement.Visibility == ElementVisibility.Collapsed)
				{
					bodySize.Height = 0;
				}
			}

			int maxWidth = Math.Max(captionSize.Width, bodySize.Width);
			int maxBodyHeight = bodySize.Height;
			if (fixedSize != LayoutUtils.InvalidSize)
			{
				maxWidth = fixedSize.Width;
				maxBodyHeight = fixedSize.Height - captionSize.Height;
			}

			if (captionElement != null)
			{
				Size size = new Size(maxWidth, captionSize.Height);
				Point captionLocation = this.CaptionOnTop ? Point.Empty : new Point(0, maxBodyHeight);
				captionElement.Bounds = new Rectangle(captionLocation, size);
			}
			if (bodyElement != null)
			{
				Size size = new Size(maxWidth, maxBodyHeight);
				Point bodyLocation = this.CaptionOnTop ? new Point(0, captionSize.Height) : Point.Empty;
				bodyElement.Bounds = new Rectangle(bodyLocation, size);
			}
		}

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF bounds =  this.Parent.Bounds;

            SizeF fixedSize = finalSize;

            InitializeElements();
            SizeF captionSize = SizeF.Empty;
            SizeF bodySize = SizeF.Empty;

            int captionHeight = 8;

            if (captionElement != null)
            {
                captionSize = captionElement.DesiredSize;
                if (captionElement.Visibility != ElementVisibility.Collapsed)
                    captionSize.Height = Math.Max(captionHeight, captionSize.Height);
            }

            if (bodyElement != null)
            {
                bodySize = bodyElement.DesiredSize;

                if (bodyElement.Visibility == ElementVisibility.Collapsed)
                {
                    bodySize.Height = 0;
                }
            }

            float maxWidth = Math.Max(captionSize.Width, bodySize.Width);
            float maxBodyHeight = bodySize.Height;
            maxWidth = fixedSize.Width;
            maxBodyHeight = fixedSize.Height - captionSize.Height;

            if (captionElement != null)
            {
                SizeF size = new SizeF(maxWidth, captionSize.Height);
                PointF captionLocation = this.CaptionOnTop ? PointF.Empty : new PointF(0, maxBodyHeight);
                captionElement.Arrange( new RectangleF(captionLocation, size));
            }
            if (bodyElement != null)
            {
                SizeF size = new SizeF(maxWidth, maxBodyHeight);
                PointF bodyLocation = this.CaptionOnTop ? new PointF(0, captionSize.Height) : PointF.Empty;
                bodyElement.Arrange( new RectangleF(bodyLocation, size));
            }
            return finalSize;
        }

		private void InitializeElements()
		{
			foreach (RadElement element in this.Children)
			{
				if ((bool) element.GetValue(CaptionElementProperty))
					captionElement = element;
				else
					bodyElement = element;
			}
		}

        /// <summary>
        /// Retrieves the preferred size of the layout panel. If the proposed size is 
        /// smaller than the minimal one, the minimal one is retrieved. Since all layout 
        /// panels update their layout automatically through events, this function is 
        /// rarely used directly.
        /// </summary>
		public override Size GetPreferredSizeCore(Size proposedSize)
		{
			InitializeElements();

			int maxWidth = 0;
			int height = 0;
			if (captionElement.Visibility != ElementVisibility.Collapsed)
			{
				maxWidth = Math.Max(captionElement.FullSize.Width, bodyElement.FullSize.Width);
				height = captionElement.FullSize.Height + bodyElement.FullSize.Height;
			}
			else
			{
				maxWidth = bodyElement.FullSize.Width;
				height = bodyElement.FullSize.Height;
			}
			return new Size(maxWidth, height);
		}

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            this.InitializeElements();
            base.MeasureOverride(availableSize);

            return new SizeF( Math.Max( captionElement.DesiredSize.Width, bodyElement.DesiredSize.Width ),captionElement.DesiredSize.Height+ bodyElement.DesiredSize.Height);
        }
       
		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
			if (e.Property == ElementWithCaptionLayoutPanel.ShowCaptionProperty)
			{
				this.captionElement.Visibility = (bool)e.NewValue ? ElementVisibility.Visible : ElementVisibility.Collapsed;
			}
			base.OnPropertyChanged(e);
		}
	}
}