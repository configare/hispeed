using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Themes.ControlDefault;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents the main element of the
    /// <see cref="Telerik.WinControls.UI.RadScrollablePanel"/> control. This element contains
    /// a <see href="see Telerik.WinControls.Primitives.FillPrimitive"/>
    /// and a <see href="see Telerik.WinControls.Primitives.BorderPrimitive"/>.
    /// </summary>
    public class RadScrollablePanelElement : RadItem
    {
        #region Fields

        private BorderPrimitive panelBorderPrimitive;
        private FillPrimitive panelFillPrimitive;
        private TextPrimitive panelTextPrimitive;

        #endregion

        #region Ctor

        /// <summary>
        /// Creates an instance of the RadPanelElement class.
        /// </summary>
        public RadScrollablePanelElement()
        {
            this.UseNewLayoutSystem = true;
        }

        static RadScrollablePanelElement()
        {
            new ScrollablePanel().DeserializeTheme();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets an instance of the <see cref="Telerik.WinControls.Primitives.TextPrimitive"/>
        /// class which represents the text of the panel.
        /// </summary>
        [Browsable(false)]
        public TextPrimitive TextPrimitive
        {
            get
            {
                return this.panelTextPrimitive;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="Telerik.WinControls.Primitives.BorderPrimitive"/> class
        /// which represents the border of the panel.
        /// </summary>
        [Browsable(false)]
        public BorderPrimitive Border
        {
            get
            {
                return this.panelBorderPrimitive;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="Telerik.WinControls.Primitives.FillPrimitive"/>
        /// class which represents the fill of the panel.
        /// </summary>
        [Browsable(false)]
        public FillPrimitive Fill
        {
            get
            {
                return this.panelFillPrimitive;
            }
        }

        #endregion

        #region Methods

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.panelFillPrimitive = new FillPrimitive();
            this.panelFillPrimitive.Class = "RadScrollablePanelFill";
            this.Children.Add(this.panelFillPrimitive);

            this.panelBorderPrimitive = new BorderPrimitive();
            this.panelBorderPrimitive.Class = "RadScrollablePanelBorder";
            this.Children.Add(this.panelBorderPrimitive);

            this.panelTextPrimitive = new TextPrimitive();
            this.panelTextPrimitive.Text = "RadScrollablePanelText";
            this.panelTextPrimitive.BindProperty(TextPrimitive.TextProperty, this, RadLabelElement.TextProperty, PropertyBindingOptions.TwoWay);
            this.Children.Add(this.panelTextPrimitive);
        }

        #endregion
    }
}
