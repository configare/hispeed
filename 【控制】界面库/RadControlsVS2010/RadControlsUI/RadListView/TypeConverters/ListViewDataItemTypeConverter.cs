using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Collections;

namespace Telerik.WinControls.UI
{
    public class ListViewDataItemTypeConverter : TypeConverter
    {
        // Methods
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return ((destinationType == typeof(string)) || 
                (destinationType == typeof(InstanceDescriptor)) || 
                base.CanConvertTo(context, destinationType));
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if ((destinationType == typeof(InstanceDescriptor)) && (value is ListViewDataItem))
            {
                ConstructorInfo constructor;
                ListViewDataItem item = (ListViewDataItem)value;
                 
                string[] strArray = new string[item.SubItems.Count];
                for (int j = 0; j < strArray.Length; j++)
                {
                    strArray[j] = Convert.ToString(item.SubItems[j]);
                }
                 
                if (item.SubItems.Count == 0)
                {
                    constructor = typeof(ListViewDataItem).GetConstructor(new Type[] { typeof(string) });
                    if (constructor != null)
                    {
                        return new InstanceDescriptor(constructor, new object[] { item.Text }, false);
                    }
                }

                constructor = typeof(ListViewDataItem).GetConstructor(new Type[] {typeof(string), typeof(string[]) });
                if (constructor != null)
                {
                    return new InstanceDescriptor(constructor, new object[] {item.Text, strArray }, false);
                }
                
            }
            if ((destinationType == typeof(string)) && (value is ListViewDataItem))
            {
                return ((ListViewDataItem)value).Text;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }


}
