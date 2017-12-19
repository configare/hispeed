using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Collections;

namespace Telerik.WinControls.UI.Carousel
{
    public sealed class Point3DConverter : TypeConverter
    {
        // Methods
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return (destinationType == typeof(InstanceDescriptor)) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = value as string;
            if (str == null)
            {
                return base.ConvertFrom(context, culture, value);
            }
            string str2 = str.Trim();
            if (str2.Length == 0)
            {
                return null;
            }
            if (culture == null)
            {
                culture = CultureInfo.CurrentCulture;
            }
            char ch = culture.TextInfo.ListSeparator[0];
            string[] strArray = str2.Split(new char[] { ch });
            double[] numArray = new double[strArray.Length];
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(double));
            for (int i = 0; i < numArray.Length; i++)
            {
                numArray[i] = (double)converter.ConvertFromString(context, culture, strArray[i]);
            }
            if (numArray.Length != 3)
            {
                //throw new ArgumentException(SR.GetString("TextParseFailedFormat", new object[] { str2, "x, y" }));
                throw new ArgumentException(string.Format("Specified format {0} is not valid for values {1}", new object[] { str2, "x, y, z" }));
            }
            return new Point3D(numArray[0], numArray[1], numArray[2]);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if (value is Point3D)
            {
                if (destinationType == typeof(string))
                {
                    Point3D point = (Point3D)value;
                    if (culture == null)
                    {
                        culture = CultureInfo.CurrentCulture;
                    }
                    string separator = culture.TextInfo.ListSeparator + " ";
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(double));
                    string[] strArray = new string[3];
                    int num = 0;
                    strArray[num++] = converter.ConvertToString(context, culture, point.X);
                    strArray[num++] = converter.ConvertToString(context, culture, point.Y);
                    strArray[num++] = converter.ConvertToString(context, culture, point.Z);
                    return string.Join(separator, strArray);
                }
                if (destinationType == typeof(InstanceDescriptor))
                {
                    Point3D point2 = (Point3D)value;
                    ConstructorInfo constructor = typeof(Point3D).GetConstructor(new Type[] { typeof(double), typeof(double), typeof(double) });
                    if (constructor != null)
                    {
                        return new InstanceDescriptor(constructor, new object[] { point2.X, point2.Y, point2.Z });
                    }
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            if (propertyValues == null)
            {
                throw new ArgumentNullException("propertyValues");
            }
            object obj2 = propertyValues["X"];
            object obj3 = propertyValues["Y"];
            object obj4 = propertyValues["Z"];
            if (((obj2 == null) || (obj3 == null)) ||
               (!(obj2 is double) || !(obj3 is double)))
            {
                throw new ArgumentException("Invalid property value entry");
            }
            if ((obj4 == null) || !(obj4 is double))
            {
                return new Point3D((double)obj2, (double)obj3);
            }
            else
            {
                return new Point3D((double)obj2, (double)obj3, (double)obj4);
            }
        }

        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(Point3D), attributes).Sort(new string[] { "X", "Y", "Z" });
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

    }

 

}
