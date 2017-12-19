using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
	/// <summary>
	///		Represents a generic menu item which could have any descendant of RadElement placed inside.
	///		Such element could be placed in the menu by setting the ContentElement property.
	/// </summary>
	[RadToolboxItemAttribute(false)]
	public class RadMenuContentItem : RadMenuItemBase
	{
		// Fields
		private ImagePrimitive imagePrimitive;
		private FillPrimitive fillPrimitive;
		private BorderPrimitive borderPrimitive;
		private RadElement contentElement;

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.Class = "RadMenuContentItem";
        }

        #region Properties

        public static RadProperty ShowImageOffsetProperty = RadProperty.Register(
			"ShowImageOffset", typeof(bool), typeof(RadMenuContentItem), new RadElementPropertyMetadata(
				true, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		/// <summary>
		///	Gets or sets if the image column offset is shown along with content element or not.
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[Description("Gets or sets if the image is shown along with content element or not.")]
		[RadPropertyDefaultValue("ShowImageOffset", typeof(RadMenuContentItem))]
		public virtual bool ShowImageOffset
		{
			get 
			{
				return (bool)base.GetValue(RadMenuContentItem.ShowImageOffsetProperty);
			}
			set 
			{
				base.SetValue(RadMenuContentItem.ShowImageOffsetProperty, value);
			}
		}
						
		/// <summary>
		///		Provides a reference to the content element in the menu item.
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[Description("Provides a reference to the content element in the menu item.")]
		public RadElement ContentElement
		{
			get
			{
				return this.contentElement;
			}
			set
			{
				if (this.contentElement != value)
				{
					if (this.contentElement != null)
					{
						this.Children.Remove(this.contentElement);
					}
					this.contentElement = value;
					this.Children.Add(this.contentElement);
					this.contentElement.NotifyParentOnMouseInput = true;
				}
			}
        }

        #endregion

        protected override void CreateChildElements()
		{
            // fill
			this.fillPrimitive = new FillPrimitive();
            this.fillPrimitive.BackColor = Color.Empty;
            this.fillPrimitive.GradientStyle = GradientStyles.Solid;
            this.fillPrimitive.Class = "RadMenuItemFillPrimitive";
            this.fillPrimitive.Name = "MenuContentItemFill";
            this.Children.Add(this.fillPrimitive);

            // border
			this.borderPrimitive = new BorderPrimitive();
			this.borderPrimitive.Class = "RadMenuItemBorderPrimitive";
            this.borderPrimitive.Name = "MenuContentItemBorder";
            this.Children.Add(this.borderPrimitive);

            // image
			this.imagePrimitive = new ImagePrimitive();
			this.imagePrimitive.SetValue(ImageAndTextLayoutPanel.IsImagePrimitiveProperty, true);
			this.imagePrimitive.BindProperty(ImagePrimitive.ImageIndexProperty, this, RadButtonItem.ImageIndexProperty, PropertyBindingOptions.TwoWay);
			this.imagePrimitive.BindProperty(ImagePrimitive.ImageProperty, this, RadButtonItem.ImageProperty, PropertyBindingOptions.TwoWay);
			this.imagePrimitive.BindProperty(ImagePrimitive.ImageKeyProperty, this, RadButtonItem.ImageKeyProperty, PropertyBindingOptions.TwoWay);
			this.imagePrimitive.Class = "RadMenuItemImagePrimitive";
			this.Children.Add(this.imagePrimitive);
		}

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF clientRect = GetClientRectangle(finalSize);
            RadDropDownMenuLayout menuLayout = this.Parent as RadDropDownMenuLayout;
            int leftColumnWidth = 0;
            if (menuLayout != null)
            {
                leftColumnWidth = (int)menuLayout.LeftColumnWidth;
            }

            foreach (RadElement element in this.Children)
            {
                if (element == this.imagePrimitive)
                {
                    RectangleF finalRect = new RectangleF(clientRect.Left, clientRect.Top, leftColumnWidth, clientRect.Height);
                    if (this.RightToLeft)
                    {
                        finalRect = LayoutUtils.RTLTranslateNonRelative(finalRect, clientRect);
                    }

                    element.Arrange(finalRect);
                }
                else if (element == this.contentElement)
                {
                    int leftColumnPadding = 0;
                    int rightColumnWidth = 0;
                    
                    if (menuLayout != null)
                    {
                        leftColumnPadding = (int)menuLayout.LeftColumnMaxPadding;
                        rightColumnWidth = (int)menuLayout.RightColumnWidth;
                    }

                    RectangleF finalRect = new RectangleF(
                        clientRect.Left + leftColumnPadding + leftColumnWidth,
                        clientRect.Top,
                        clientRect.Width - (leftColumnPadding + rightColumnWidth),
                        clientRect.Height);

                    if (this.RightToLeft)
                    {
                        finalRect = LayoutUtils.RTLTranslateNonRelative(finalRect, clientRect);
                    }

                    element.Arrange(finalRect);
                }
                else if (element.FitToSizeMode == RadFitToSizeMode.FitToParentBounds)
                {
                    element.Arrange(new RectangleF(Point.Empty, finalSize));
                }
                else if (element.FitToSizeMode == RadFitToSizeMode.FitToParentPadding)
                {
                    element.Arrange(new RectangleF(this.BorderThickness.Left, this.BorderThickness.Top, finalSize.Width - this.BorderThickness.Horizontal, finalSize.Height - this.BorderThickness.Vertical));
                }
                else
                {
                    element.Arrange(clientRect);
                }
            }
            
            return finalSize;
        }

        protected internal override bool? ShouldSerializeProperty(PropertyDescriptor property)
        {
            if (property.Name == "Class")
            {
                return this.Class != "RadMenuContentItem";
            }

            return base.ShouldSerializeProperty(property);
        }
    }
}
