using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Telerik.WinControls
{
    /// <summary>
    /// Exposes methods and properties for a concrete property setttings used in StyleSheets and Themes.
	/// PropertySetting can customize the current value of any RadPropertry of any RadElement instance.
    /// </summary>
    public interface IPropertySetting : ICloneable
    {
        /// <summary>
        /// Gets or sets the property itself.
        /// </summary>
        RadProperty Property
        {
            get;
            set;
        }
        /// <summary>
        /// Retrieves the current value of the property.
        /// </summary>
        /// <param name="forObject">
        ///     
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        object GetCurrentValue(RadObject forObject);
        /// <summary>
        /// Applies the value to the element given as a parameter.
        /// </summary>
        /// <param name="element">
        /// the element that the property value is applied to.
        /// </param>
        void ApplyValue(RadElement element);
        /// <summary>
        /// Unapply the property to the element given as a parameter.
        /// </summary>
        /// <param name="element">
        /// the element that the property value is unapplied to.
        /// </param>
        void UnapplyValue(RadElement element);
        /// <summary>
        /// Serializes the property setting.
        /// </summary>
        /// <returns></returns>
        XmlPropertySetting Serialize();
        /// <summary>
        /// Unregisters the value.
        /// </summary>
        /// <param name="selectedElement"></param>
        void UnregisterValue(RadElement selectedElement);

		/// <summary>
		/// Called when the property setting is not valid any more, since the target element property has been set to
		/// another value.
		/// </summary>
		void PropertySettingRemoving(RadObject targetRadObject);
    }
}