using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Telerik.WinControls.Themes
{
    public class HslColorConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return ((destinationType == typeof(InstanceDescriptor)) || base.CanConvertTo(context, destinationType));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = value as string;
            if (str == null)
            {
                return base.ConvertFrom(context, culture, value);
            }
            object resHslColor = null;

            string strValue = str.Trim();

            if (strValue.Length == 0)
            {
                return HslColor.Empty;
            }

            if (culture == null)
            {
                culture = CultureInfo.CurrentCulture;
            }
            char listSeparatorChar = culture.TextInfo.ListSeparator[0];

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(double));

            string[] strArray = strValue.Split(new char[] { listSeparatorChar });
            double[] numArray = new double[strArray.Length];
            for (int i = 0; i < numArray.Length; i++)
            {
                numArray[i] = (double)converter.ConvertFromString(context, culture, strArray[i]);
            }
            switch (numArray.Length)
            {
                case 1:
                    resHslColor = HslColor.FromAhsl((int)numArray[0]);
                    break;

                case 3:
                    resHslColor = HslColor.FromAhsl(numArray[0], numArray[1], numArray[2]);
                    break;

                case 4:
                    resHslColor = HslColor.FromAhsl((int)numArray[0], numArray[1], numArray[2], numArray[3]);
                    break;
            }

            if (resHslColor == null)
            {
                throw new ArgumentException("Invalid HSL color string representation.");
            }
            return resHslColor;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if (value is HslColor)
            {
                if (destinationType == typeof(string))
                {
                    string[] strArray;
                    HslColor HslColor = (HslColor)value;
                    if (HslColor == HslColor.Empty)
                    {
                        return string.Empty;
                    }
                    if (culture == null)
                    {
                        culture = CultureInfo.CurrentCulture;
                    }
                    string separator = culture.TextInfo.ListSeparator + " ";
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(double));
                    int num = 0;
                    if (HslColor.A < 0xff)
                    {
                        strArray = new string[4];
                        strArray[num++] = converter.ConvertToString(context, culture, HslColor.A);
                    }
                    else
                    {
                        strArray = new string[3];
                    }
                    strArray[num++] = converter.ConvertToString(context, culture, HslColor.H);
                    strArray[num++] = converter.ConvertToString(context, culture, HslColor.S);
                    strArray[num++] = converter.ConvertToString(context, culture, HslColor.L);
                    return string.Join(separator, strArray);
                }
                if (destinationType == typeof(InstanceDescriptor))
                {
                    MemberInfo member = null;
                    object[] arguments = null;
                    HslColor HslColor2 = (HslColor)value;
                    if (HslColor2.IsEmpty)
                    {
                        member = typeof(HslColor).GetField("Empty");
                    }
                    else if (HslColor2.A != 0xff)
                    {
                        member = typeof(HslColor).GetMethod("FromAhsl", new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) });
                        arguments = new object[] { HslColor2.A, HslColor2.H, HslColor2.S, HslColor2.L };
                    }
                    else
                    {
                        member = typeof(HslColor).GetMethod("FromAhsl", new Type[] { typeof(int), typeof(int), typeof(int) });
                        arguments = new object[] { HslColor2.H, HslColor2.S, HslColor2.L };
                    }

                    return new InstanceDescriptor(member, arguments);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
