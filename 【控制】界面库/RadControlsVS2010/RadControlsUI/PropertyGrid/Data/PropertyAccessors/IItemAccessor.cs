using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;

namespace Telerik.WinControls.UI.PropertyGridData
{
    /// <summary>
    /// Defines an interface used to acces property information in RadPropertyGrid.
    /// </summary>
    public interface IItemAccessor
    {
        /// <summary>
        /// Gets the property name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Gets the property description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the property category.
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Gets the property type.
        /// </summary>
        Type PropertyType { get; }

        /// <summary>
        /// Gets the <see cref="PropertyDescriptor"/> associated with this property.
        /// </summary>
        PropertyDescriptor PropertyDescriptor { get; }

        /// <summary>
        /// Gets the <see cref="UITypeEditor"/> associated with this property.
        /// </summary>
        UITypeEditor UITypeEditor { get; }

        /// <summary>
        /// Gets the <see cref="TypeConverter"/> associated with this property.
        /// </summary>
        TypeConverter TypeConverter { get; }
    }
}
