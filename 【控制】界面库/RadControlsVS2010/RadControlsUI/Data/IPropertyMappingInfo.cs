using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI.Data
{
    /// <summary>
    /// Associates a source properties collection with the corresponding properties collection exposed by the scheduler events. 
    /// It is used in common by all RadScheduler data providers.
    /// Contains a collection of SchedulerMapping objects, and is implemented by the 
    /// </summary>
    public interface IPropertyMappingInfo : IEnumerable<PropertyMapping>
    {
        /// <summary>
        /// Searches for a SchedulerMapping instance that binds a property of an item from the data store to 
        /// a property of an item from RadScheduler. The RadScheduler items are events, resources, etc.
        /// </summary>
        /// <param name="logicalProperty">Property name of an item in RadScheduler.</param>
        /// <returns>The first element that matches the property name, if found.</returns>
        PropertyMapping FindByLogicalItemProperty(string logicalProperty);

        /// <summary>
        /// Searches for a SchedulerMapping instance that binds a property of an item from the data store to 
        /// a property of an item from RadScheduler. The RadScheduler items are events, resources, etc.
        /// </summary>
        /// <param name="dataSourceProperty">Property name of an item in the data store.</param>
        /// <returns>The first element that matches the property name, if found.</returns>
        PropertyMapping FindByDataSourceProperty(string dataSourceProperty);
    }

    /// <summary>
    /// Represents the method that will handle the type conversion between the values of corresponding properties.
    /// </summary>
    /// <param name="item">The value to be converted.</param>
    /// <param name="converter">The converter applied.</param>
    /// <returns>The converted value.</returns>
    public delegate object ConvertCallback(object item, TypeConverter converter);
}
