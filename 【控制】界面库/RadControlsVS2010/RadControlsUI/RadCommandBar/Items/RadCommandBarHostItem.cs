using Telerik.WinControls;
using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a host for <see cref="RadElement"/> elements in <see cref="CommandBarStripElement"/>.
    /// </summary>
    public class CommandBarHostItem : RadCommandBarBaseItem
    {
        #region Static Members

        static SizeF defaultSize = new SizeF(15, 15);

        static CommandBarHostItem()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ItemStateManagerFactory(), typeof(CommandBarHostItem));
        }

        #endregion

        #region Fields

        private RadElement hostedItem;
        private Control hostedControl;

        #endregion

        #region Constructors

        public CommandBarHostItem()
        {
        }

        public CommandBarHostItem(RadElement hostedItem)
        {
            this.HostedItem = hostedItem;
        }

        public CommandBarHostItem(Control hostedControl)
        {
            RadHostItem item = new RadHostItem(hostedControl);
            this.HostedItem = item;
            this.hostedControl = hostedControl;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the hosted <see cref="RadElement"/>.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public RadElement HostedItem
        {
            get
            {
                return hostedItem;
            }
            set
            {   
                if (hostedItem!=value)
                {
                    if (hostedItem != null)
                    {
                        this.Children.Remove(hostedItem);
                    }
                   
                    this.hostedItem = value;
                    this.hostedControl = null;

                    if (hostedItem != null)
                    {
                        this.Children.Add(hostedItem);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the hosted <see cref="Control"/>.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Control HostedControl
        {
            get
            {
                return this.hostedControl;
            }
            set
            {
                this.HostedItem = new RadHostItem(value);
                this.hostedControl = value;
            }
        }

        /// <summary>
        ///		Show or hide item from the strip
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory), DefaultValue(true)]
        [Description("Indicates whether the item should be drawn in the strip.")]
        public override bool VisibleInStrip
        {
            get
            {
                return base.VisibleInStrip;
            }
            set
            {
                base.VisibleInStrip = value;
                if (this.hostedItem != null)
                {
                    this.hostedItem.SetValue(RadElement.VisibilityProperty, (value) ? ElementVisibility.Visible : ElementVisibility.Collapsed);
                }
            }
        }

        #endregion

        #region Overrides

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.NotifyParentOnMouseInput = false;
            this.CanFocus = false;
            this.SetDefaultValueOverride(RadElement.StretchHorizontallyProperty, false);
            this.SetDefaultValueOverride(RadElement.StretchVerticallyProperty, false);
            this.Alignment = ContentAlignment.MiddleLeft;
        }
        protected override System.Drawing.SizeF MeasureOverride(System.Drawing.SizeF availableSize)
        {
            if (hostedItem == null)
            {
                return defaultSize;
            }
            availableSize = GetClientRectangle(availableSize).Size;
            hostedItem.Measure(availableSize);
            SizeF measuredSize = new SizeF( Math.Min(availableSize.Width, hostedItem.DesiredSize.Width),Math.Min( availableSize.Height, hostedItem.DesiredSize.Height ) );
            Padding border = GetBorderThickness(this.DrawBorder);
            measuredSize.Width += border.Left + border.Right + this.Padding.Left + this.Padding.Right;
            measuredSize.Height += border.Top + border.Bottom + this.Padding.Top + this.Padding.Bottom;
            return measuredSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF clientRect = GetClientRectangle(finalSize);
            if (hostedItem == null)
            {
                return finalSize;
            }

            hostedItem.Arrange(clientRect);
            return finalSize;
        }

        #endregion
    }
}
