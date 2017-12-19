using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Telerik.WinControls.XmlSerialization
{
    /// <summary>
    /// Represents a base class for the XML serialization converters. 
    /// </summary>
    public abstract class SerializationConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyOwner"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract string ConvertToString(object propertyOwner, PropertyDescriptor property, object value);

        public abstract object ConvertFromString(object propertyOwner, PropertyDescriptor property, string value);

        public abstract Type GetActualPropertyType(object propertyOwner, PropertyDescriptor property);

        public virtual RadProperty GetRadProperty(object propertyOwner, PropertyDescriptor property)
        {
            return null;
        }
    }
}
