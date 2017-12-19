using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.ComponentModel;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{   
    ///<summary>
    ///Represents a layout panel used in the RadCheckBoxElement.
    ///</summary>
    public class CheckBoxLayoutPanel : LayoutPanel
	{
		#region Fields
		private RadElement checkElement;
		private RadElement bodyElement;
        private Size checkSize = Size.Empty;
		private ContentAlignment contentAlignment;
		internal RectangleF checkArea;
		internal RectangleF checkBounds;
		#endregion

		#region Dependency properties
		public static RadProperty CheckAlignmentProperty = RadProperty.Register("CheckAlignment", typeof(ContentAlignment), typeof(CheckBoxLayoutPanel),
		    new RadElementPropertyMetadata(
                ContentAlignment.MiddleLeft,
                ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsDisplay));

		public static RadProperty IsBodyProperty = RadProperty.Register("IsBodyElement", typeof(bool), typeof(CheckBoxLayoutPanel),
			new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty IsCheckmarkProperty = RadProperty.Register("IsCheckElement", typeof(bool), typeof(CheckBoxLayoutPanel),
			new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty CheckMarkOffsetProperty = RadProperty.Register("CheckMarkOffset", typeof(int), typeof(CheckBoxLayoutPanel),
			   new RadElementPropertyMetadata(
				(int)1, 
				ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

		#endregion
		
		#region Properties
        /// <summary>Gets or sets the offset between the check and body elements.</summary>
        /// <remarks>
        /// The body can contain image and / or text the same way as all buttons can -
        /// see <see cref="Telerik.WinControls.UI.RadButton"/>
        /// </remarks>
        /// <seealso cref="RadButton.DisplayStyle">DisplayStyle Property for RadCheckBox.</seealso>
        /// <seealso cref="RadButton.TextImageRelation">TextImageRelation Property for RadCheckBox.</seealso>
        /// <seealso cref="RadButton.ImageAlignment">ImageAlignment Property for RadCheckBox.</seealso>
        /// <seealso cref="RadButton.TextAlignment">TextAlignment Property for RadCheckBox.</seealso>
		[RadPropertyDefaultValue("CheckMarkOffset", typeof(CheckBoxLayoutPanel))]
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Offset between the check and body elements")]
        public int CheckMarkOffset
		{
			get 
			{
				return (int)this.GetValue(CheckMarkOffsetProperty);
			}
			set
			{
				this.SetValue(CheckMarkOffsetProperty, value);
			}

		}
        /// <summary>Gets or set a value indicating the check alignment.</summary>
		public ContentAlignment CheckAlignment
		{
			get
			{
				return (ContentAlignment) this.GetValue(CheckAlignmentProperty);
			}
			set
			{
				this.SetValue(CheckAlignmentProperty, value);
			}
		}		
		#endregion

		#region Methods

		private bool EnsureBodyOrCheckElements()
		{
			if (this.bodyElement == null || this.checkElement == null)
			{
				foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
				{
					if ((bool)child.GetValue(IsBodyProperty))
					{
						this.bodyElement = child;
					}
					else if ((bool)child.GetValue(IsCheckmarkProperty))
					{
						this.checkElement = child;
					}
				}
				return (this.bodyElement != null || this.checkElement!= null);
			}
			else
			{
				return true;
			}
		}

        private void LayoutCheckmark(RectangleF fieldRectangle, bool newLayout)
		{
			float checkWidth = newLayout ? this.checkElement.DesiredSize.Width : checkSize.Width;
            if (newLayout)
                checkBounds = new RectangleF(Point.Empty, checkElement.DesiredSize);
            else
                checkBounds = new Rectangle(Point.Empty, checkSize);

			if (this.RightToLeft)
			{
				contentAlignment = TelerikAlignHelper.RtlTranslateContent(CheckAlignment);
			}
			else
			{
				contentAlignment = CheckAlignment;
			}
			if (checkWidth <= 0)
			{
				return;
			}

			RectangleF rectangle1 = fieldRectangle;
			RectangleF field = rectangle1;
			//setting the check at the right, horizontally
			if ((contentAlignment & (ContentAlignment.BottomRight | (ContentAlignment.MiddleRight | ContentAlignment.TopRight))) != ((ContentAlignment) 0))
			{
				checkBounds.X = (rectangle1.X + rectangle1.Width) - checkBounds.Width;
			}
			//setting the check at the center, horizontally
			else if ((contentAlignment & (ContentAlignment.BottomCenter | (ContentAlignment.MiddleCenter | ContentAlignment.TopCenter))) != ((ContentAlignment) 0))
			{
				checkBounds.X = rectangle1.X + ((rectangle1.Width - checkBounds.Width) / 2);
			}
			
			//setting the check at the bottom, vertically
			if ((contentAlignment & (ContentAlignment.BottomRight | (ContentAlignment.BottomCenter | ContentAlignment.BottomLeft))) != ((ContentAlignment) 0))
			{
				checkBounds.Y = (rectangle1.Y + rectangle1.Height) - checkBounds.Height;
			}
			//setting the check in the center, vertically
			else if ((contentAlignment & (ContentAlignment.MiddleRight | (ContentAlignment.MiddleCenter | ContentAlignment.MiddleLeft))) != ((ContentAlignment)0))
			{
				checkBounds.Y = rectangle1.Y + ((rectangle1.Height - checkBounds.Height) / 2);
			}

			ContentAlignment alignment2 = contentAlignment;
			switch (alignment2)
			{
				//check is aligned on the left size
				case ContentAlignment.TopLeft:
				case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
				{
                    field.X += checkWidth + this.CheckMarkOffset;
					field.Width -= checkWidth + this.CheckMarkOffset;
                    break;
				}
				case ContentAlignment.TopCenter:
				{
					field.Y += checkWidth;
					field.Height -= checkWidth;
                    break;
				}
				case ContentAlignment.TopCenter | ContentAlignment.TopLeft:
				{
					break;
				}
				case ContentAlignment.TopRight:
                case ContentAlignment.BottomRight:
                case ContentAlignment.MiddleRight:
				{
					field.Width -= checkWidth + this.CheckMarkOffset;
                    break;
				}
                case ContentAlignment.BottomCenter: 
                {
                    field.Height -= checkWidth;
                    break;
                }
				case ContentAlignment.MiddleCenter:
				{
                    break;
				}
			}

            if (newLayout)
            {
                this.checkElement.Arrange(checkBounds);
                this.bodyElement.Arrange(field);
            }
            else
            {
                this.checkElement.Bounds = Rectangle.Round(checkBounds);
                this.bodyElement.Bounds = Rectangle.Round(field);
            }
		}

        ///<summary>
        /// Performs layout changes based on the element given as a paramater. 
        /// Sizes and places are determined by the concrete layout panel that is used. 
        /// Since all layout panels update their layout automatically through events, 
        /// this function is rarely used directly.
        /// </summary>
        public override void PerformLayoutCore(RadElement affectedElement)
        {
            if (!EnsureBodyOrCheckElements())
                return;

            if ((checkElement != null) && (bodyElement != null))
            {
                if (this.AutoSize && this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize)
                {
                    this.checkSize = checkElement.GetPreferredSize(this.AvailableSize);
                    this.bodyElement.GetPreferredSize(this.AvailableSize);
                    LayoutCheckmark(new Rectangle(Point.Empty, this.AvailableSize), false);
                }
                else
                {
                    this.checkSize = checkElement.GetPreferredSize(this.Size);
                    // Subtract the image size from the whole field size					
                    Size textSizeLeft = LayoutUtils.SubAlignedRegionCore(this.Size, checkSize, LayoutUtils.IsVerticalAlignment(this.CheckAlignment));
                    this.bodyElement.Size = this.bodyElement.GetPreferredSize(textSizeLeft);
                    LayoutCheckmark(new Rectangle(Point.Empty, CombineSizes(this.bodyElement.Size, this.checkSize)), false);
                }

            }
        }
        ///<summary>
        /// Retrieves the preferred size of the layout panel. If the proposed size is 
        /// smaller than the minimal one, the minimal one is retrieved. Since all layout 
        /// panels update their layout automatically through events, this function is 
        /// rarely used directly.
        /// </summary>
        public override Size GetPreferredSizeCore(Size proposedSize)
        {
            Size res = Size.Empty;
            if (EnsureBodyOrCheckElements())
            {
                if (this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize)
                {
                    //return this.AvailableSize;
                    return this.Parent.Size;
                }
                else
                {
                    if (this.bodyElement.Size != Size.Empty)
                    {
                        res = CombineSizes(this.bodyElement.FullSize, this.checkElement.FullSize);
                    }
                    else
                    {
                        res = this.checkElement.FullSize;
                    }
                }
            }
            return res;
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            // Ensure that Measure() is called for all children
            //p.p. 27.07.09 removed base.MeasureOverride
            //base.MeasureOverride(availableSize);
            if (!EnsureBodyOrCheckElements())
            {
                return SizeF.Empty;
            }
            this.checkElement.Measure( availableSize );
            SizeF bodyElementAvailableSize = this.GetBodyElementAvailableSize(availableSize);
            this.bodyElement.Measure( bodyElementAvailableSize);
            return CombineSizes();
        }

        private SizeF GetBodyElementAvailableSize(SizeF availableSize)
        {
            SizeF bodyElementAvailableSize = SizeF.Empty;
            switch (this.CheckAlignment)
            {
                case ContentAlignment.BottomCenter:
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                    bodyElementAvailableSize = new SizeF(availableSize.Width, availableSize.Height  - this.checkElement.DesiredSize.Height - this.CheckMarkOffset);
                    break;
                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomRight:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.TopLeft:
                case ContentAlignment.TopRight:
                    bodyElementAvailableSize = new SizeF(availableSize.Width - this.checkElement.DesiredSize.Width - this.CheckMarkOffset, availableSize.Height);
                    break;
                default:
                    break;
            }
            return bodyElementAvailableSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            if (!EnsureBodyOrCheckElements())
                return base.ArrangeOverride(finalSize);

            if ((checkElement != null) && (bodyElement != null))
            {
                LayoutCheckmark(new RectangleF(PointF.Empty, finalSize), true);
            }
			else
			{
				if (checkElement != null)
					checkElement.Arrange(new RectangleF(new PointF(0,0), checkElement.DesiredSize));
				
				if (bodyElement != null)
					bodyElement.Arrange(new RectangleF(new PointF(0, 0), bodyElement.DesiredSize));
			}

            return finalSize;
        }

		private SizeF CombineSizes()
		{
			if (bodyElement != null && checkElement != null)
			{
				if (bodyElement.DesiredSize == SizeF.Empty && this.checkElement.DesiredSize != SizeF.Empty)
					return this.checkElement.DesiredSize;

				//middle center
				if (this.CheckAlignment == ContentAlignment.MiddleCenter)
				{
					return new SizeF(LayoutUtils.UnionSizes(bodyElement.DesiredSize, checkElement.DesiredSize));
				}
				//vertical alignment
				else if (LayoutUtils.IsVerticalAlignment(this.CheckAlignment))
				{
					return new SizeF(Math.Max(bodyElement.DesiredSize.Width, checkElement.DesiredSize.Width),
						bodyElement.DesiredSize.Height + checkElement.DesiredSize.Height + this.CheckMarkOffset);
				}
				//horizontal alignment
				else
				{
					return new SizeF(bodyElement.DesiredSize.Width + checkElement.DesiredSize.Width + this.CheckMarkOffset,
						Math.Max(bodyElement.DesiredSize.Height, checkElement.DesiredSize.Height));
				}
			}
			else if (bodyElement == null && checkElement != null)
				return checkElement.DesiredSize;
			else if (bodyElement != null && checkElement == null)
				return bodyElement.DesiredSize;

			return SizeF.Empty;
		}

        private Size CombineSizes(Size imageTextSize, Size checkSize)
        {
            if (this.CheckAlignment == ContentAlignment.MiddleCenter)
            {
                return LayoutUtils.UnionSizes(imageTextSize, checkSize);
            }

            if (LayoutUtils.IsVerticalAlignment(this.CheckAlignment))
            {
                return new Size(Math.Max(imageTextSize.Width, checkSize.Width),
                                imageTextSize.Height + checkSize.Height + this.CheckMarkOffset);
            }

            return new Size(imageTextSize.Width + checkSize.Width + this.CheckMarkOffset,
                            Math.Max(imageTextSize.Height, checkSize.Height));
        }

        #endregion
	}
}