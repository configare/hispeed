using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Globalization;

namespace Telerik.WinControls.Data
{
    public class FilterValueStringConverter: StringConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            object resultValue = null;
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                resultValue = base.ConvertTo(context, culture, value, destinationType);
            }
            finally 
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
            return resultValue;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            object resultValue = null;
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                resultValue = base.ConvertFrom(context, culture, value);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
            return resultValue;
        }
    }
}
