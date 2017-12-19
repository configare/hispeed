using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Telerik.WinControls
{
    public class ColorAnimationStepConverter: TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string strValue = (string)value;
            if (string.IsNullOrEmpty(strValue))
            {
                new ColorAnimationStep(0, 0, 0, 0);
            }

            CultureInfo currCulture = AnimationValueCalculatorFactory.SerializationCulture;

            string[] resArray = strValue.Split(currCulture.TextInfo.ListSeparator[0]);

            if (resArray.Length == 3)
            {
                TypeConverter intConverter = TypeDescriptor.GetConverter(typeof(int));
                return new
                    ColorAnimationStep(0,
                                       (int)intConverter.ConvertFrom(context, currCulture, resArray[0]),
                                       (int)intConverter.ConvertFrom(context, currCulture, resArray[1]),
                                       (int)intConverter.ConvertFrom(context, currCulture, resArray[2]));
            }
            else

                if (resArray.Length == 4)
                {
                    TypeConverter intConverter = TypeDescriptor.GetConverter(typeof(int));
                    return new
                        ColorAnimationStep(
                        (int)intConverter.ConvertFrom(context, currCulture, resArray[0]),
                        (int)intConverter.ConvertFrom(context, currCulture, resArray[1]),
                        (int)intConverter.ConvertFrom(context, currCulture, resArray[2]),
                        (int)intConverter.ConvertFrom(context, currCulture, resArray[3]));
                }
                else
                {
                    throw base.GetConvertFromException(value);
                }
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return false;
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            ColorAnimationStep step = (ColorAnimationStep)value;

            CultureInfo currCulture = AnimationValueCalculatorFactory.SerializationCulture;

            if (step == null)
            {
                return (string)null;
            }

            TypeConverter intConverter = TypeDescriptor.GetConverter(typeof(int));

            string[] resArray = new string[] 
                {
                    (string)intConverter.ConvertTo( context, currCulture, step.A, destinationType),
                    " " + (string)intConverter.ConvertTo( context, currCulture, step.R, destinationType),
                    " " + (string)intConverter.ConvertTo( context, currCulture, step.G, destinationType),
                    " " + (string)intConverter.ConvertTo( context, currCulture, step.B, destinationType)
                };

            return string.Join(currCulture.TextInfo.ListSeparator, resArray);
        }
    }
}
