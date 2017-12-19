using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Layout;
using System.Windows.Forms;
using Telerik.WinControls.Primitives;
using System.Drawing;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public enum DropDownPosition
    {
        Unknown,
        LeftContent,
        RightContent,
        RightPopupContent,
        Popup
    }

    public class RadDropDownMenuElement : RadElement
    {
        private FillPrimitive fill;
        private FillPrimitive leftColumnFill;
        private BorderPrimitive leftColumnBorder;
        private BorderPrimitive border;
        private RadDropDownMenuLayout layoutPanel;
        private RadScrollViewer scrollViewer;
        private RadElement leftColumnElement;
        private ImageAndTextLayoutPanel headerColumnElement;
        private FillPrimitive headerColumnFill;
        private BorderPrimitive headerColumnBorder;
        private TextPrimitive headerColumnText;
        private ImagePrimitive headerColumnImage;
        private RadElement headerElement;

        internal const ulong UseScrollingStateKey = RadElementLastStateKey << 1;
        

        protected override void InitializeFields()
        {
            SetBitState(UseScrollingStateKey, true);
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
            this.Class = "DropDownMenuElement";
        }

        #region Properties

        public static readonly RadProperty DropDownPositionProperty = RadProperty.Register(
            "DropDownPosition", typeof(DropDownPosition), typeof(RadDropDownMenuElement), new RadElementPropertyMetadata(
                DropDownPosition.Unknown, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public RadElement LayoutPanel
        {
            get
            {
                return this.layoutPanel;
            }
        }

        public FillPrimitive Fill
        {
            get
            {
                return this.fill;
            }
        }

        public BorderPrimitive Border
        {
            get
            {
                return this.border;
            }
        }

        public RadDropDownMenuLayout Layout
        {
            get 
            {
                return this.layoutPanel;
            }
        }

        public RadElement LeftColumnElement
        {
            get
            {
                return this.leftColumnElement;
            }
        }

        public FillPrimitive LeftColumnFill
        {
            get
            {
                return this.leftColumnFill;
            }
        }

        public BorderPrimitive LeftColumnBorder
        {
            get 
            {
                return this.leftColumnBorder;
            }
        }

        public ImageAndTextLayoutPanel HeaderColumn
        {
            get
            {
                return this.headerColumnElement;
            }
        }

        public FillPrimitive HeaderColumnFill
        {
            get
            {
                return this.headerColumnFill;
            }
        }

        public BorderPrimitive HeaderColumnBorder
        {
            get
            {
                return this.headerColumnBorder;
            }
        }

        public TextPrimitive HeaderColumnText
        {
            get
            {
                return this.headerColumnText;
            }
        }

        public ImagePrimitive HeaderColumnImage
        {
            get
            {
                return this.headerColumnImage;
            }
        }

        /// <summary>
        /// Gets or sets the header text of the drop-down menu.
        /// </summary>
        public string HeaderText
        {
            get
            {
                return this.headerColumnText.Text;
            }
            set
            {
                this.headerColumnText.Text = value;
                SetHeaderVisibility();
            }
        }

        /// <summary>
        /// Gets or sets the header image of the drop-down menu.
        /// </summary>
        public Image HeaderImage
        {
            get
            {
                return this.headerColumnImage.Image;
            }
            set
            {
                this.headerColumnImage.Image = value;
                SetHeaderVisibility();
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="RadScrollViewer"/>
        /// class that represents layout panel that provides scrolling functionality.
        /// </summary>
        public RadScrollViewer ScrollPanel
        {
            get
            {
                return this.scrollViewer;
            }
        }

        protected internal override bool CanHaveOwnStyle
        {
            get
            {
                return true;
            }
        }

        #endregion

        protected override void CreateChildElements()
        {
            this.fill = new FillPrimitive();
            this.fill.Class = "RadSubMenuPanelBackFillPrimitive";
            this.Children.Add(this.fill);

            this.border = new BorderPrimitive();
            this.border.Class = "RadSubMenuPanelBorderPrimitive";
            this.Children.Add(this.border);

            leftColumnElement = new RadElement();
            leftColumnElement.Class = "RadSubMenuPanelLeftElement";
            this.Children.Add(leftColumnElement);
            
            leftColumnFill = new FillPrimitive();
            leftColumnFill.Class = "RadSubMenuPanelFillPrimitive";
            leftColumnElement.Children.Add(leftColumnFill);

            leftColumnBorder = new BorderPrimitive();
            leftColumnBorder.Class = "RadSubMenuPanelLeftBorderPrimitive";
            leftColumnElement.Children.Add(leftColumnBorder);

            StackLayoutPanel panel = new StackLayoutPanel();
            panel.Orientation = Orientation.Horizontal;
            panel.ZIndex = 1;
            this.Children.Add(panel);

            panel.Children.Add(CreateHeaderColumnElement());

            this.layoutPanel = CreateMenuLayout();

            if (GetBitState(UseScrollingStateKey))
            {
                this.scrollViewer = new RadScrollViewer();

                this.scrollViewer.ShowBorder = false;
                this.scrollViewer.ShowFill = false;
                this.scrollViewer.Viewport = this.layoutPanel;
                panel.Children.Add(this.scrollViewer);
            }
            else
            {
                panel.Children.Add(this.layoutPanel);
            }
        }

        protected virtual RadDropDownMenuLayout CreateMenuLayout()
        {
            return new RadDropDownMenuLayout();
        }    
            
        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            base.MeasureOverride(availableSize);
            this.layoutPanel.Measure(availableSize);
            SizeF resultSize = layoutPanel.DesiredSize;
            int leftColumnWidth = (int)Math.Ceiling(this.layoutPanel.LeftColumnWidth + this.layoutPanel.LeftColumnMaxPadding);
            resultSize.Width += leftColumnWidth;
            resultSize.Width += this.Padding.Horizontal;
            resultSize.Height += this.Padding.Vertical;
            resultSize.Height += this.BorderThickness.Vertical;
            resultSize.Width  += this.BorderThickness.Horizontal;
            
            return this.ApplySizeConstraints(resultSize);
        }

        protected virtual SizeF ApplySizeConstraints(SizeF desiredSize)
        {
            RadPopupControlBase ownerPopup = this.ElementTree.Control as RadPopupControlBase;

            if (ownerPopup == null)
                return desiredSize;
            return ownerPopup.ApplySizingConstraints(desiredSize.ToSize(), ownerPopup.GetCurrentScreen());
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            base.ArrangeOverride(finalSize);

            float width = this.layoutPanel.LeftColumnWidth + this.layoutPanel.LeftColumnMaxPadding;         
            RectangleF leftRect = new RectangleF(this.headerElement.DesiredSize.Width +
                this.BorderThickness.Left + this.Padding.Left + this.layoutPanel.BoundingRectangle.Left, 
                this.BorderThickness.Top + this.Padding.Top, 
                width, 
                finalSize.Height - (this.BorderThickness.Vertical + this.Padding.Vertical));

            if (this.RightToLeft)
            {
                leftRect.X = finalSize.Width -( this.headerElement.DesiredSize.Width +
                this.BorderThickness.Right + this.Padding.Right + leftRect.Width);
            }

            leftColumnElement.Arrange(leftRect);
            SizeF scrollViewerArrangeSize = new SizeF(finalSize);
            scrollViewerArrangeSize.Width -= (this.BorderThickness.Horizontal + this.Padding.Horizontal);
            scrollViewerArrangeSize.Height -= (this.BorderThickness.Vertical + this.Padding.Vertical);
            RectangleF scrollViewerRect = new RectangleF(new PointF(this.headerElement.DesiredSize.Width, 0), scrollViewerArrangeSize);

            if (this.RightToLeft)
            {
                scrollViewerRect.X = finalSize.Width - this.Padding.Horizontal - this.BorderThickness.Vertical - scrollViewerRect.X - scrollViewerRect.Width;
            }

            if (GetBitState(UseScrollingStateKey) && this.scrollViewer != null)
            {
                this.scrollViewer.Arrange(scrollViewerRect);
            }
            else
            {
                this.layoutPanel.Arrange(scrollViewerRect);
            }

            return finalSize;
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == DropDownPositionProperty)
            {
                foreach (RadElement element in this.ChildrenHierarchy)
                {
                    element.SetValue(RadDropDownMenuElement.DropDownPositionProperty, e.NewValue);
                }
            }            
        }

        private RadElement CreateHeaderColumnElement()
        {
            headerElement = new RadElement();
            headerElement.Visibility = ElementVisibility.Collapsed;

            // fill
            headerColumnFill = new FillPrimitive();
            headerColumnFill.Class = "RadSubMenuPanelHeaderFill";
            headerElement.Children.Add(headerColumnFill);

            // border
            headerColumnBorder = new BorderPrimitive();
            headerColumnBorder.Class = "RadSubMenuPanelHeaderBorder";
            headerElement.Children.Add(headerColumnBorder);

            headerColumnElement = new ImageAndTextLayoutPanel();
            headerColumnElement.Class = "RadSubMenuPanelHeaderColumn";
            headerColumnElement.ZIndex = 1;
            headerColumnElement.AngleTransform = 270;
            headerElement.Children.Add(headerColumnElement);

            // text
            headerColumnText = new TextPrimitive();
            headerColumnText.SetValue(ImageAndTextLayoutPanel.IsTextPrimitiveProperty, true);
            headerColumnText.Class = "RadMenuItemTextPrimitive";
            headerColumnElement.Children.Add(headerColumnText);

            // image
            headerColumnImage = new ImagePrimitive();
            headerColumnImage.SetValue(ImageAndTextLayoutPanel.IsImagePrimitiveProperty, true);
            headerColumnImage.Class = "RadMenuItemImagePrimitive";
            headerColumnElement.Children.Add(this.headerColumnImage);

            return headerElement;
        }

        private void SetHeaderVisibility()
        {
            if (headerElement != null)
            {
                if (!string.IsNullOrEmpty(this.headerColumnText.Text) ||
                    this.headerColumnImage.Image != null)
                {
                    this.headerElement.Visibility = ElementVisibility.Visible;
                }
                else
                {
                    this.headerElement.Visibility = ElementVisibility.Hidden;
                }
            }
        }
    }
}
