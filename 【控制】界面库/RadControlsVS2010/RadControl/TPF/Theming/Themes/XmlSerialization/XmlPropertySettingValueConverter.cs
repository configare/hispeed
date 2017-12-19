using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.XmlSerialization;

namespace Telerik.WinControls.Themes.XmlSerialization
{
    /// <summary>
    /// 
    /// </summary>
    public class XmlPropertySettingValueConverter : SerializationConverter
    {
        public override string ConvertToString(object propertyOwner, PropertyDescriptor property, object value)
        {
            if (propertyOwner is XmlPropertySetting)
            {
                string propertyFullName = (propertyOwner as XmlPropertySetting).Property;
                if (string.IsNullOrEmpty(propertyFullName))
                {
                    return null;
                }

                RadProperty radProperty = XmlPropertySetting.DeserializeProperty(propertyFullName);
                if (radProperty != null)
                {
                    return XmlPropertySetting.SerializeValue(radProperty, value);
                }
            }

            throw new NotSupportedException("Only XmlPropertySetting instances' value can be converted.");
        }

        public override object ConvertFromString(object propertyOwner, PropertyDescriptor property, string value)
        {
            if (propertyOwner is XmlPropertySetting)
            {
                string propertyFullName = (propertyOwner as XmlPropertySetting).Property;
                if (string.IsNullOrEmpty(propertyFullName))
                {
                    return null;
                }

                RadProperty radProperty = XmlPropertySetting.DeserializeProperty(propertyFullName);                
                if (radProperty != null)
                {
                    return XmlPropertySetting.DeserializeValue(radProperty, value);                    
                }
            }

            throw new NotSupportedException("Only XmlPropertySetting instances' value can be converted.");
        }

        public override Type GetActualPropertyType(object propertyOwner, PropertyDescriptor property)
        {
            if (propertyOwner is XmlPropertySetting)
            {
                string propertyFullName = (propertyOwner as XmlPropertySetting).Property;
                if (string.IsNullOrEmpty(propertyFullName))
                {
                    return null;
                }

                RadProperty radProperty = XmlPropertySetting.DeserializeProperty(propertyFullName);
                if (radProperty != null)
                {
                    if (radProperty.PropertyType != null)
                    {
                        return radProperty.PropertyType;
                    }
                    else
                    {
                    }
                }
            }

            throw new NotSupportedException("Only XmlPropertySetting instances' value can be converted.");
        }

        public override RadProperty GetRadProperty(object propertyOwner, PropertyDescriptor property)
        {
            if (propertyOwner is XmlPropertySetting)
            {
                string propertyFullName = (propertyOwner as XmlPropertySetting).Property;
                if (string.IsNullOrEmpty(propertyFullName))
                {
                    return null;
                }

                RadProperty radProperty = XmlPropertySetting.DeserializeProperty(propertyFullName);
                if (radProperty != null)
                {
                    return radProperty;
                }
            }

            throw new NotSupportedException("Only XmlPropertySetting instances' value can be converted.");
        }
    }
}
