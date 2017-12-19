using System;
using System.Drawing;
using Telerik.WinControls;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.UI;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents the groupbox footer.
    /// </summary>
    public class GroupBoxFooter : GroupBoxVisualElement
    {
        private TextPrimitive textPrimitive;
        private ImagePrimitive imagePrimitive;
        private ImageAndTextLayoutPanel imageAndTextLayout;

        public TextPrimitive TextPrimitive
        {
            get { return textPrimitive; }
            set { textPrimitive = value; }
        }

        public ImagePrimitive ImagePrimitive
        {
            get { return imagePrimitive; }
            set { imagePrimitive = value; }
        }

        public ImageAndTextLayoutPanel ImageAndTextLayout
        {
            get { return imageAndTextLayout; }
            set { imageAndTextLayout = value; }
        }

        /// <summary>
        /// Creates child elements. Please refer to TPF documentation for more information.
        /// </summary>
        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.textPrimitive = new TextPrimitive();
            this.imagePrimitive = new ImagePrimitive();
            this.imageAndTextLayout = new ImageAndTextLayoutPanel();
            this.imageAndTextLayout.UseNewLayoutSystem = true;
            this.textPrimitive.SetValue(ImageAndTextLayoutPanel.IsTextPrimitiveProperty, true);
            this.imagePrimitive.SetValue(ImageAndTextLayoutPanel.IsImagePrimitiveProperty, true);

            this.imageAndTextLayout.Children.Add(imagePrimitive);
            this.imageAndTextLayout.Children.Add(textPrimitive);
            this.Children.Add(imageAndTextLayout);

            this.Class = "GroupBoxFooter";

            this.Visibility = ElementVisibility.Collapsed;
        }

        /// <summary>
        /// Performs layout measure. Please refer to TPF documentation for more information.
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns>desired size</returns>
        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF desiredSize = SizeF.Empty;

            imageAndTextLayout.Measure(availableSize);
            desiredSize.Width = Math.Max(imageAndTextLayout.DesiredSize.Width + this.BorderThickness.Vertical, desiredSize.Width);
            desiredSize.Height = Math.Max(imageAndTextLayout.DesiredSize.Height + this.BorderThickness.Horizontal, desiredSize.Height);

            return desiredSize;
        }

        /// <summary>
        /// Returns class name.
        /// </summary>
        /// <returns>class name</returns>
        public override string ToString()
        {
            return "GroupBoxFooter";
        }
    }
}