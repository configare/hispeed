using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a visual element, which contains set of common buttons for a <see cref="RadPageViewItem"/> instance.
    /// </summary>
    public class RadPageViewButtonsPanel : RadPageViewElementBase
    {
        #region Rad Properties

        public static RadProperty ButtonsSizeProperty = RadProperty.Register(
            "ButtonsSize",
            typeof(Size),
            typeof(RadPageViewButtonsPanel),
            new RadElementPropertyMetadata(new Size(16, 16),
                ElementPropertyOptions.AffectsMeasure |
                ElementPropertyOptions.AffectsLayout));

        public static RadProperty ButtonsSpacingProperty = RadProperty.Register(
           "ButtonsSpacing",
           typeof(int),
           typeof(RadPageViewButtonsPanel),
           new RadElementPropertyMetadata(1,
               ElementPropertyOptions.AffectsMeasure |
               ElementPropertyOptions.AffectsDisplay));

        #endregion

        #region CLR Properties

        /// <summary>
        /// Gets or sets the size to be applied to each of the embedded buttons.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the size to be applied to each of the embedded buttons.")]
        public Size ButtonsSize
        {
            get
            {
                return (Size)this.GetValue(ButtonsSizeProperty);
            }
            set
            {
                this.SetValue(ButtonsSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the spacing between each two buttons.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the spacing between each two buttons.")]
        public int ButtonsSpacing
        {
            get
            {
                return (int)this.GetValue(ButtonsSpacingProperty);
            }
            set
            {
                this.SetValue(ButtonsSpacingProperty, value);
            }
        }

        #endregion

        #region Layout

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ButtonsSizeProperty)
            {
                Size size = (Size)e.NewValue;
                foreach (RadElement child in this.Children)
                {
                    child.SetDefaultValueOverride(MinSizeProperty, size);
                }
            }
        }

        protected override void OnChildrenChanged(RadElement child, ItemsChangeOperation changeOperation)
        {
            base.OnChildrenChanged(child, changeOperation);

            if (changeOperation == ItemsChangeOperation.Inserted ||
                changeOperation == ItemsChangeOperation.Set)
            {
                child.SetDefaultValueOverride(MinSizeProperty, this.ButtonsSize);
            }
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF measured = SizeF.Empty;
            int visibleCount = 0;
            Size buttonsSize = this.ButtonsSize;

            foreach (RadElement button in this.Children)
            {
                if (button.Visibility == ElementVisibility.Collapsed)
                {
                    continue;
                }

                button.Measure(buttonsSize);

                SizeF desired = button.DesiredSize;
                Padding margin = button.Margin;

                switch (this.ContentOrientation)
                {
                    case PageViewContentOrientation.Horizontal:
                    case PageViewContentOrientation.Horizontal180:
                        measured.Width += desired.Width + margin.Horizontal;
                        measured.Height = Math.Max(measured.Height, desired.Height + margin.Vertical);
                        break;
                    case PageViewContentOrientation.Vertical90:
                    case PageViewContentOrientation.Vertical270:
                        measured.Width = Math.Max(measured.Width, desired.Width + margin.Horizontal);
                        measured.Height += desired.Height + margin.Vertical;
                        break;
                }

                visibleCount++;
            }

            if (visibleCount == 0)
            {
                return SizeF.Empty;
            }

            //add buttons spacing
            if (visibleCount > 1)
            {
                int spacing = this.ButtonsSpacing * (visibleCount - 1);
                switch (this.ContentOrientation)
                {
                    case PageViewContentOrientation.Horizontal:
                    case PageViewContentOrientation.Horizontal180:
                        measured.Width += spacing;
                        break;
                    case PageViewContentOrientation.Vertical90:
                    case PageViewContentOrientation.Vertical270:
                        measured.Height += spacing;
                        break;
                }
            }

            measured = this.ApplyClientOffset(measured);
            measured = this.ApplyMinMaxSize(measured);

            return measured;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF client = this.GetClientRectangle(finalSize);
            this.ArrangeButtons(client);

            return finalSize;
        }

        private void ArrangeButtons(RectangleF client)
        {
            float left = client.X;
            float top = client.Y;
            PointF location = PointF.Empty;
            int spacing = this.ButtonsSpacing;

            foreach (RadElement button in this.Children)
            {
                if (button.Visibility == ElementVisibility.Collapsed)
                {
                    continue;
                }

                SizeF desired = button.DesiredSize;
                Padding margin = button.Margin;

                switch (this.ContentOrientation)
                {
                    case PageViewContentOrientation.Horizontal:
                    case PageViewContentOrientation.Horizontal180:
                        location = new PointF(left + margin.Left, client.Y + (client.Height - desired.Height) / 2);
                        left += desired.Width + margin.Horizontal + spacing;
                        break;
                    case PageViewContentOrientation.Vertical90:
                    case PageViewContentOrientation.Vertical270:
                        location = new PointF(client.X + (client.Width - desired.Width) / 2, top + margin.Top);
                        top += desired.Height + margin.Vertical + spacing;
                        break;
                }

                button.Arrange(new RectangleF(location, desired));
            }
        }

        #endregion
    }
}
