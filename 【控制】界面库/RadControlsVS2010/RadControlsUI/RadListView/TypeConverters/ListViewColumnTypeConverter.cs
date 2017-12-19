using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace Telerik.WinControls.UI
{
    public class ListViewColumnTypeConverter : TypeConverter
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
            if ((destinationType == typeof(InstanceDescriptor)) && (value is ListViewDetailColumn))
            {
                ConstructorInfo constructor;
                ListViewDetailColumn column = (ListViewDetailColumn)value;

                constructor = typeof(ListViewDetailColumn).GetConstructor(new Type[] { typeof(string), typeof(string) });
                if (constructor != null)
                {
                    return new InstanceDescriptor(constructor, new object[] { column.Name, column.HeaderText }, false);
                }

            }
            if ((destinationType == typeof(string)) && (value is ListViewDataItem))
            {
                return ((ListViewDetailColumn)value).Name;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
