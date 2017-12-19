using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI;
using System.Diagnostics;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Implements the Caption element in a <see cref="ToolTabStrip">ToolTabStrip</see> control.
    /// </summary>
    public class ToolWindowCaptionElement : RadItem //Inherits RadItem to enable theming and Spy
    {
        #region Constructor

        static ToolWindowCaptionElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ToolWindowCaptionElementStateManager(), typeof(ToolWindowCaptionElement));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.ShouldHandleMouseInput = false;
            this.BypassLayoutPolicies = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// A property to determine whether the element is currently active (associated with the <see cref="RadDock.ActiveWindow">ActiveWindow</see> instance within a RadDock).
        /// </summary>
        public static RadProperty IsActiveProperty = RadProperty.Register(
            "IsActive",
            typeof(bool),
            typeof(ToolWindowCaptionElement),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        #endregion
    }
}
