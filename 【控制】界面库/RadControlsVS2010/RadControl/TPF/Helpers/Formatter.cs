using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    
    public class Formatter
    {
        // Methods
        static Formatter()
        {
            Formatter.stringType = typeof(string);
            Formatter.booleanType = typeof(bool);
            Formatter.checkStateType = typeof(CheckState);
            Formatter.parseMethodNotFound = new object();
            Formatter.defaultDataSourceNullValue = DBNull.Value;
        }
        /// <summary>
        /// Initializes a new instance of the Formatter class.
        /// </summary>
        public Formatter()
        {
        }

        private static object ChangeType(object value, System.Type type, IFormatProvider formatInfo)
        {
            object obj1;
            try
            {
                if (formatInfo == null)
                {
                    formatInfo = CultureInfo.CurrentCulture;
                }
                obj1 = Convert.ChangeType(value, type, formatInfo);
            }
            catch (InvalidCastException exception1)
            {
                throw new FormatException(exception1.Message, exception1);
            }
            return obj1;
        }

        private static bool EqualsFormattedNullValue(object value, object formattedNullValue, IFormatProvider formatInfo)
        {
            if ((formattedNullValue is string) && (value is string))
            {
                return (string.Compare((string)value, (string)formattedNullValue, true, Formatter.GetFormatterCulture(formatInfo)) == 0);
            }
            return object.Equals(value, formattedNullValue);
        }

        public static object FormatObject(object value, System.Type targetType, TypeConverter sourceConverter, TypeConverter targetConverter, string formatString, IFormatProvider formatInfo, object formattedNullValue, object dataSourceNullValue)
        {
            if (Formatter.IsNullData(value, dataSourceNullValue))
            {
                value = DBNull.Value;
            }
            System.Type type1 = targetType;
            targetType = Formatter.NullableUnwrap(targetType);
            sourceConverter = Formatter.NullableUnwrap(sourceConverter);
            targetConverter = Formatter.NullableUnwrap(targetConverter);
            bool flag1 = targetType != type1;
            object obj1 = Formatter.FormatObjectInternal(value, targetType, sourceConverter, targetConverter, formatString, formatInfo, formattedNullValue);
            if ((type1.IsValueType && (obj1 == null)) && !flag1)
            {
                throw new FormatException(Formatter.GetCantConvertMessage(value, targetType));
            }
            return obj1;
        }

        private static object FormatObjectInternal(object value, System.Type targetType, TypeConverter sourceConverter, TypeConverter targetConverter, string formatString, IFormatProvider formatInfo, object formattedNullValue)
        {
            if ((value == DBNull.Value) || (value == null))
            {
                if (formattedNullValue != null)
                {
                    return formattedNullValue;
                }
                if (targetType == Formatter.stringType)
                {
                    return string.Empty;
                }
                if (targetType == Formatter.checkStateType)
                {
                    return CheckState.Indeterminate;
                }
                return null;
            }
            if (((targetType == Formatter.stringType) && (value is IFormattable)) && !string.IsNullOrEmpty(formatString))
            {
                return (value as IFormattable).ToString(formatString, formatInfo);
            }
            System.Type type1 = value.GetType();
            TypeConverter converter1 = TypeDescriptor.GetConverter(type1);
            if (((sourceConverter != null) && (sourceConverter != converter1)) && sourceConverter.CanConvertTo(targetType))
            {
                return sourceConverter.ConvertTo(null, Formatter.GetFormatterCulture(formatInfo), value, targetType);
            }
            TypeConverter converter2 = TypeDescriptor.GetConverter(targetType);
            if (((targetConverter != null) && (targetConverter != converter2)) && targetConverter.CanConvertFrom(type1))
            {
                return targetConverter.ConvertFrom(null, Formatter.GetFormatterCulture(formatInfo), value);
            }
            if (targetType == Formatter.checkStateType)
            {
                if (type1 == Formatter.booleanType)
                {
                    return (((bool)value) ? CheckState.Checked : CheckState.Unchecked);
                }
                if (sourceConverter == null)
                {
                    sourceConverter = converter1;
                }
                if ((sourceConverter != null) && sourceConverter.CanConvertTo(Formatter.booleanType))
                {
                    return (((bool)sourceConverter.ConvertTo(null, Formatter.GetFormatterCulture(formatInfo), value, Formatter.booleanType)) ? CheckState.Checked : CheckState.Unchecked);
                }
            }
            if (targetType.IsAssignableFrom(type1))
            {
                return value;
            }
            if (sourceConverter == null)
            {
                sourceConverter = converter1;
            }
            if (targetConverter == null)
            {
                targetConverter = converter2;
            }
            if ((sourceConverter != null) && sourceConverter.CanConvertTo(targetType))
            {
                return sourceConverter.ConvertTo(null, Formatter.GetFormatterCulture(formatInfo), value, targetType);
            }
            if ((targetConverter != null) && targetConverter.CanConvertFrom(type1))
            {
                return targetConverter.ConvertFrom(null, Formatter.GetFormatterCulture(formatInfo), value);
            }
            if (!(value is IConvertible))
            {
                throw new FormatException(Formatter.GetCantConvertMessage(value, targetType));
            }
            return Formatter.ChangeType(value, targetType, formatInfo);
        }

        private static string GetCantConvertMessage(object value, System.Type targetType)
        {
            string text1 = (value == null) ? "Formatter Cant Convert Null" : "Formatter Cant Convert";
            object[] objArray1 = new object[] { value, targetType.Name };
            return string.Format(CultureInfo.CurrentCulture, text1, objArray1);
        }

        public static object GetDefaultDataSourceNullValue(System.Type type)
        {
            if ((type != null) && !type.IsValueType)
            {
                return null;
            }
            return Formatter.defaultDataSourceNullValue;
        }

        private static CultureInfo GetFormatterCulture(IFormatProvider formatInfo)
        {
            if (formatInfo is CultureInfo)
            {
                return (formatInfo as CultureInfo);
            }
            return CultureInfo.CurrentCulture;
        }

        public static object InvokeStringParseMethod(object value, System.Type targetType, IFormatProvider formatInfo)
        {
            object obj1;
            try
            {
                System.Type[] typeArray1 = new System.Type[] { Formatter.stringType, typeof(NumberStyles), typeof(IFormatProvider) };
                MethodInfo info1 = targetType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, typeArray1, null);
                if (info1 != null)
                {
                    object[] objArray1 = new object[] { (string)value, NumberStyles.Any, formatInfo };
                    return info1.Invoke(null, objArray1);
                }
                System.Type[] typeArray2 = new System.Type[] { Formatter.stringType, typeof(IFormatProvider) };
                info1 = targetType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, typeArray2, null);
                if (info1 != null)
                {
                    object[] objArray2 = new object[] { (string)value, formatInfo };
                    return info1.Invoke(null, objArray2);
                }
                System.Type[] typeArray3 = new System.Type[] { Formatter.stringType };
                info1 = targetType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, typeArray3, null);
                if (info1 != null)
                {
                    object[] objArray3 = new object[] { (string)value };
                    return info1.Invoke(null, objArray3);
                }
                obj1 = Formatter.parseMethodNotFound;
            }
            catch (TargetInvocationException exception1)
            {
                throw new FormatException(exception1.InnerException.Message, exception1.InnerException);
            }
            return obj1;
        }

        public static bool IsNullData(object value, object dataSourceNullValue)
        {
            if ((value != null) && (value != DBNull.Value))
            {
                return object.Equals(value, Formatter.NullData(value.GetType(), dataSourceNullValue));
            }
            return true;
        }

        private static TypeConverter NullableUnwrap(TypeConverter typeConverter)
        {
            NullableConverter converter1 = typeConverter as NullableConverter;
            if (converter1 == null)
            {
                return typeConverter;
            }
            return converter1.UnderlyingTypeConverter;
        }

        private static System.Type NullableUnwrap(System.Type type)
        {
            if (type == Formatter.stringType)
            {
                return Formatter.stringType;
            }
            System.Type type1 = Nullable.GetUnderlyingType(type);
            return (type1 ?? type);
        }

        public static object NullData(System.Type type, object dataSourceNullValue)
        {
            if (!type.IsGenericType || (type.GetGenericTypeDefinition() != typeof(Nullable<>)))
            {
                return dataSourceNullValue;
            }
            if ((dataSourceNullValue != null) && (dataSourceNullValue != DBNull.Value))
            {
                return dataSourceNullValue;
            }
            return null;
        }

        public static object ParseObject(object value, System.Type targetType, System.Type sourceType, TypeConverter targetConverter, TypeConverter sourceConverter, IFormatProvider formatInfo, object formattedNullValue, object dataSourceNullValue)
        {
            System.Type type1 = targetType;
            sourceType = Formatter.NullableUnwrap(sourceType);
            targetType = Formatter.NullableUnwrap(targetType);
            sourceConverter = Formatter.NullableUnwrap(sourceConverter);
            targetConverter = Formatter.NullableUnwrap(targetConverter);
            object obj1 = Formatter.ParseObjectInternal(value, targetType, sourceType, targetConverter, sourceConverter, formatInfo, formattedNullValue);
            if (obj1 == DBNull.Value)
            {
                return Formatter.NullData(type1, dataSourceNullValue);
            }
            return obj1;
        }

        private static object ParseObjectInternal(object value, System.Type targetType, System.Type sourceType, TypeConverter targetConverter, TypeConverter sourceConverter, IFormatProvider formatInfo, object formattedNullValue)
        {
            if (Formatter.EqualsFormattedNullValue(value, formattedNullValue, formatInfo) || (value == DBNull.Value))
            {
                return DBNull.Value;
            }
            TypeConverter converter1 = TypeDescriptor.GetConverter(targetType);
            if (((targetConverter != null) && (converter1 != targetConverter)) && targetConverter.CanConvertFrom(sourceType))
            {
                return targetConverter.ConvertFrom(null, Formatter.GetFormatterCulture(formatInfo), value);
            }
            TypeConverter converter2 = TypeDescriptor.GetConverter(sourceType);
            if (((sourceConverter != null) && (converter2 != sourceConverter)) && sourceConverter.CanConvertTo(targetType))
            {
                return sourceConverter.ConvertTo(null, Formatter.GetFormatterCulture(formatInfo), value, targetType);
            }
            if (value is string)
            {
                object obj1 = Formatter.InvokeStringParseMethod(value, targetType, formatInfo);
                if (obj1 != Formatter.parseMethodNotFound)
                {
                    return obj1;
                }
            }
            else if (value is CheckState)
            {
                CheckState state1 = (CheckState)value;
                if (state1 == CheckState.Indeterminate)
                {
                    return DBNull.Value;
                }
                if (targetType == Formatter.booleanType)
                {
                    return (state1 == CheckState.Checked);
                }
                if (targetConverter == null)
                {
                    targetConverter = converter1;
                }
                if ((targetConverter != null) && targetConverter.CanConvertFrom(Formatter.booleanType))
                {
                    return targetConverter.ConvertFrom(null, Formatter.GetFormatterCulture(formatInfo), state1 == CheckState.Checked);
                }
            }
            else if ((value != null) && targetType.IsAssignableFrom(value.GetType()))
            {
                return value;
            }
            if (targetConverter == null)
            {
                targetConverter = converter1;
            }
            if (sourceConverter == null)
            {
                sourceConverter = converter2;
            }
            if ((targetConverter != null) && targetConverter.CanConvertFrom(sourceType))
            {
                return targetConverter.ConvertFrom(null, Formatter.GetFormatterCulture(formatInfo), value);
            }
            if ((sourceConverter != null) && sourceConverter.CanConvertTo(targetType))
            {
                return sourceConverter.ConvertTo(null, Formatter.GetFormatterCulture(formatInfo), value, targetType);
            }
            if (!(value is IConvertible))
            {
                throw new FormatException(Formatter.GetCantConvertMessage(value, targetType));
            }
            return Formatter.ChangeType(value, targetType, formatInfo);
        }


        // Fields
        private static System.Type booleanType;
        private static System.Type checkStateType;
        private static object defaultDataSourceNullValue;
        private static object parseMethodNotFound;
        private static System.Type stringType;
    }
}
