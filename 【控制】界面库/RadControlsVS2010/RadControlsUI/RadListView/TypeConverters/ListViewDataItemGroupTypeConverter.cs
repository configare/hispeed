using System; 
using System.ComponentModel; 
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Collections;

namespace Telerik.WinControls.UI
{
    class ListViewDataItemGroupTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return ((((sourceType == typeof(string)) && (context != null)) && (context.Instance is ListViewDataItem)) || base.CanConvertFrom(context, sourceType));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return ((destinationType == typeof(InstanceDescriptor)) || ((((destinationType == typeof(string)) && (context != null)) && (context.Instance is ListViewDataItem)) || base.CanConvertTo(context, destinationType)));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string str = ((string)value).Trim();
                if ((context != null) && (context.Instance != null))
                {
                    ListViewDataItem instance = context.Instance as ListViewDataItem;
                    if ((instance != null) && (instance.Owner != null))
                    {
                        foreach (ListViewDataItemGroup group in instance.Owner.Groups)
                        {
                            if (group.Text == str)
                            {
                                return group;
                            }
                        }
                    }
                }
            }
            if ((value != null) && !value.Equals(String.Empty))
            {
                return base.ConvertFrom(context, culture, value);
            }
            return null;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if ((destinationType == typeof(InstanceDescriptor)) && (value is ListViewDataItemGroup))
            {
                ListViewDataItemGroup group = (ListViewDataItemGroup)value;
                ConstructorInfo constructor = typeof(ListViewDataItemGroup).GetConstructor(new Type[] { typeof(string) });
                if (constructor != null)
                {
                    return new InstanceDescriptor(constructor, new object[] { group.Text }, false);
                }
            }
            if ((destinationType == typeof(string)))
            {
                if (value == null)
                {
                    return String.Empty;
                }
                else if (value is ListViewDataItemGroup)
                {
                    ListViewDataItemGroup group = (ListViewDataItemGroup)value;
                    return group.Text;
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if ((context != null) && (context.Instance != null))
            {
                ListViewDataItem instance = context.Instance as ListViewDataItem;
                if ((instance != null) && (instance.Owner != null))
                {
                    ArrayList values = new ArrayList();
                    foreach (ListViewDataItemGroup group in instance.Owner.Groups)
                    {
                        values.Add(group.Text);
                    }
                    values.Add(null);
                    return new TypeConverter.StandardValuesCollection(values);
                }
            }
            return null;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}