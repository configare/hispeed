using System;
using Telerik.WinControls.Primitives;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>The main element of the <strong>RadPanel</strong> control.</summary>
    public class RadPanelElement : RadItem
    {
        private BorderPrimitive borderPrimitive;
        private FillPrimitive fillPrimitive;
        private TextPrimitive textPrimitive;

        static RadPanelElement()
        {
            new Themes.ControlDefault.Panel().DeserializeTheme();

            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new PanelStateManagerFactory(), typeof(RadPanelElement));
        }

        /// <summary>Create the elements in the hierarchy.</summary>
        protected override void CreateChildElements()
        {
            this.borderPrimitive = new BorderPrimitive();
            borderPrimitive.Class = "RadPanelBorder";

            this.fillPrimitive = new FillPrimitive();
            fillPrimitive.BackColor = Color.Transparent;
            fillPrimitive.BackColor2 = Color.Transparent;
            fillPrimitive.BackColor3 = Color.Transparent;
            fillPrimitive.BackColor4 = Color.Transparent;
            fillPrimitive.Class = "RadPanelFill";

            this.textPrimitive = new TextPrimitive();
            this.textPrimitive.StretchHorizontally = true;
            this.textPrimitive.StretchVertically = true;
            this.textPrimitive.Alignment = ContentAlignment.MiddleLeft;
            textPrimitive.BindProperty(TextPrimitive.TextProperty, this, RadLabelElement.TextProperty, PropertyBindingOptions.TwoWay);

            this.Children.Add(this.fillPrimitive);
            this.Children.Add(this.borderPrimitive);
            this.Children.Add(this.textPrimitive);
        }

        private RectangleF GetClientRectangle(SizeF finalSize)
        {
            Padding padding = this.Padding;
            RectangleF clientRect = new RectangleF(padding.Left, padding.Top,
                finalSize.Width - padding.Horizontal, finalSize.Height - padding.Vertical);

            if (this.borderPrimitive.Visibility != ElementVisibility.Collapsed)
            {
                switch (this.borderPrimitive.BoxStyle)
                {
                    case BorderBoxStyle.FourBorders:
                        clientRect.X += borderPrimitive.LeftWidth;
                        clientRect.Y += borderPrimitive.TopWidth;
                        clientRect.Width -= borderPrimitive.LeftWidth + borderPrimitive.RightWidth;
                        clientRect.Height -= borderPrimitive.TopWidth + borderPrimitive.BottomWidth;
                        break;
                    default:
                        clientRect.X += borderPrimitive.Width;
                        clientRect.Y += borderPrimitive.Width;
                        clientRect.Width -= borderPrimitive.Width + borderPrimitive.Width;
                        clientRect.Height -= borderPrimitive.Width + borderPrimitive.Width;
                        break;
                }
            }

            clientRect.Width = Math.Max(0, clientRect.Width);
            clientRect.Height = Math.Max(0, clientRect.Height);

            return clientRect;
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            base.MeasureOverride(availableSize);

            RectangleF rect = this.GetClientRectangle(availableSize);

            this.textPrimitive.Measure(rect.Size);

            return availableSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            base.ArrangeOverride(finalSize);

            RectangleF rect = this.GetClientRectangle(finalSize);

            this.textPrimitive.Arrange(rect);

            return finalSize;
        }

        #region Properties

        /// <summary>
        /// Gets the <see cref="FillPrimitive"/>of the
        /// panel element.
        /// </summary>
        [Browsable(false)]
        public FillPrimitive PanelFill
        {
            get
            {
                return this.fillPrimitive;
            }
        }

        /// <summary>
        /// Gets the <see cref="BorderPrimitive"/>of the
        /// panel element.
        /// </summary>
        [Browsable(false)]
        public BorderPrimitive PanelBorder
        {
            get
            {
                return this.borderPrimitive;
            }
        }

        /// <summary>
        /// Gets the <see cref="TextPrimitive"/>of the
        /// panel element.
        /// </summary>
        [Browsable(false)]
        public TextPrimitive PanelText
        {
            get
            {
                return this.textPrimitive;
            }
        }

        #endregion

    }
}
