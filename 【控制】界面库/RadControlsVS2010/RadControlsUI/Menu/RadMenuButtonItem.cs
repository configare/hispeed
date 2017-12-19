using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.ComponentModel;
using Telerik.WinControls.Primitives;
using System.Drawing;
using Telerik.WinControls.Layouts;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class RadMenuButtonItem : RadMenuItemBase
    {
        private FillPrimitive fillPrimitive;
        private BorderPrimitive borderPrimitive; 
        private RadButtonElement buttonElement;

        #region Constructors 
     
        public RadMenuButtonItem()
            : this("")
        {
        }

        public RadMenuButtonItem(string text)
        {
            this.Text = text;
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.Class = "RadMenuButtonItem";
        }

        #endregion

        #region Properties

        /// <summary>
        ///		Provides a reference to the ButtonElement element in the menu item.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Provides a reference to the ButtonElement in the menu item.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadButtonElement ButtonElement
        {
            get 
            {
                return buttonElement; 
            }
        }


        /// <summary>
        ///    Gets or sets the index value of the image that is displayed on the item. 
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the index value of the image that is displayed on the item.")]
        [RelatedImageList("MenuElement.ElementTree.Control.ImageList")]
        public override int ImageIndex
        {
            get
            {
                return base.ImageIndex;
            }
            set
            {
                base.ImageIndex = value;
            }
        }

        /// <summary>
        ///    Gets or sets the key accessor for the image in the ImageList.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the key accessor for the image in the ImageList.")]
        [RelatedImageList("MenuElement.ElementTree.Control.ImageList")]
        public override string ImageKey
        {
            get
            {
                return base.ImageKey;
            }
            set
            {
                base.ImageKey = value;
            }
        }

        #endregion

        protected override void CreateChildElements()
        {
            // fill
            this.fillPrimitive = new FillPrimitive();
            this.fillPrimitive.Class = "RadMenuItemFillPrimitive";
            this.fillPrimitive.Name = "MenuButtonItemFill";
            this.fillPrimitive.BackColor = Color.Empty;
            this.fillPrimitive.GradientStyle = GradientStyles.Solid;
            this.Children.Add(this.fillPrimitive);

            // border
            this.borderPrimitive = new BorderPrimitive();
            this.borderPrimitive.Class = "RadMenuItemBorderPrimitive";
            this.borderPrimitive.Name = "MenuButtonItemBorder";
            this.Children.Add(this.borderPrimitive);

            this.buttonElement = new RadButtonElement();
            this.buttonElement.CanFocus = false;
            this.buttonElement.Click += buttonElement_Click;
            this.buttonElement.ImagePrimitive.BindProperty(ImagePrimitive.ImageIndexProperty, this, RadButtonItem.ImageIndexProperty, PropertyBindingOptions.TwoWay);
            this.buttonElement.ImagePrimitive.BindProperty(ImagePrimitive.ImageProperty, this, RadButtonItem.ImageProperty, PropertyBindingOptions.TwoWay);
            this.buttonElement.ImagePrimitive.BindProperty(ImagePrimitive.ImageKeyProperty, this, RadButtonItem.ImageKeyProperty, PropertyBindingOptions.TwoWay);
            this.buttonElement.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.Children.Add(this.buttonElement);

            this.buttonElement.BindProperty(RadButtonElement.TextProperty, this, RadButtonItem.TextProperty, PropertyBindingOptions.TwoWay);
        }


        protected override void DisposeManagedResources()
        {
            this.buttonElement.Click -= buttonElement_Click;
            base.DisposeManagedResources();
        }


        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF buttonSize = new SizeF(availableSize);
            RadDropDownMenuLayout menuLayout = this.Parent as RadDropDownMenuLayout;
            if (menuLayout != null)
            {
                buttonSize.Width = availableSize.Width - menuLayout.LeftColumnMaxPadding - menuLayout.LeftColumnWidth;
            }
            this.buttonElement.Measure(buttonSize);

            return base.MeasureOverride(availableSize);
        }
        
        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF clientRect = GetClientRectangle(finalSize);
            RadDropDownMenuLayout menuLayout = this.Parent as RadDropDownMenuLayout;
            float leftColumn = 0;
            float rightColumn = 0;
            if (menuLayout != null)
            {
                leftColumn = menuLayout.LeftColumnMaxPadding + menuLayout.LeftColumnWidth;
                rightColumn = menuLayout.LeftColumnMaxPadding + menuLayout.RightColumnWidth;
            }

            foreach (RadElement element in this.Children)
            {
                if (element == this.buttonElement)
                {
                    RectangleF rect = new RectangleF(clientRect.Left + leftColumn, clientRect.Top,
                        clientRect.Width - leftColumn - rightColumn, element.DesiredSize.Height);
                    if (this.RightToLeft)
                    { 
                        rect = LayoutUtils.RTLTranslateNonRelative(rect ,clientRect);
                    }

                    element.Arrange(rect);
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
                return this.Class != "RadMenuButtonItem";
            }

            return base.ShouldSerializeProperty(property);
        }

        private void buttonElement_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
            if (this.ElementTree != null &&
               !(this.ElementTree.Control is RadMenu))
            {
                if (this.ElementTree.Control is IPopupControl)
                {
                    (this.ElementTree.Control as IPopupControl).ClosePopup(RadPopupCloseReason.CloseCalled);
                }
            }
        }

        protected override void OnClick(EventArgs e)
        {
            if (GetSite() != null && !(this.ElementTree.Control is RadMenu))
            {
                ISelectionService service = this.Site.GetService(typeof(ISelectionService)) as ISelectionService;

                if (service != null && !service.GetComponentSelected(this))
                {
                    service.SetSelectedComponents(new IComponent[] { this });
                }
            }
            else
            {
                base.OnClick(e);
            }
        }
    }
}
