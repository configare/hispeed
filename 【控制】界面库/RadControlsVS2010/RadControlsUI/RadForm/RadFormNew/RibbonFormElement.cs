using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;


namespace Telerik.WinControls.UI
{
        /// <summary>
    /// This is the class that represents the element hierarchy which
    /// is painted in the non-client area of a RadForm.
    /// </summary>
    public class RibbonFormElement : RadItem
    {
        #region RadProperties

        public static RadProperty IsFormActiveProperty = RadProperty.Register(
            "IsFormActive",
            typeof(bool),
            typeof(RibbonFormElement),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue)
            );

        #endregion

        #region Fields

        private RibbonFormBorderPrimitive borderPrimitive;

        #endregion

        #region Constructor

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
        }

        static RibbonFormElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RibbonFormElementStateManager(), typeof(RibbonFormElement));
            new Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_RadRibbonForm().DeserializeTheme();
        }

        #endregion

        #region Properties


        /// <summary>
        /// Gets the BorderPrimitive of the RadFormElement.
        /// </summary>
        public RibbonFormBorderPrimitive Border
        {
            get
            {
                return this.borderPrimitive;
            }
        }

        #endregion

        #region Methods

        #region Overriden

        protected override void CreateChildElements()
        {
            base.CreateChildElements();


            this.borderPrimitive = new RibbonFormBorderPrimitive();
            this.borderPrimitive.Class = "RibbonFormBorder";
            this.borderPrimitive.StretchVertically = true;
            this.borderPrimitive.StretchHorizontally = true;


            this.Children.Add(this.borderPrimitive);
        }

        #endregion

        #endregion
    }
}
