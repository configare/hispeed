using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public class DataTypeHelper
    {
        public static int SizeOf(enumDataType dataType)
        {
            switch (dataType)
            {
                case enumDataType.Byte:
                    return 1;
                case enumDataType.UInt16:
                case enumDataType.Int16:
                    return 2;
                case enumDataType.Int32:
                case enumDataType.UInt32:
                case enumDataType.Float:
                    return 4;
                case enumDataType.Int64:
                case enumDataType.UInt64:
                case enumDataType.Double:
                    return 8;
                default:
                    return 0;
            }
        }

        public static Type Enum2DataType(enumDataType dataType)
        {
            switch (dataType) 
            {
                case enumDataType.Byte:
                    return typeof(byte);
                case enumDataType.Double:
                    return typeof(double);
                case enumDataType.Float:
                    return typeof(float);
                case enumDataType.Int16:
                    return typeof(Int16);
                case enumDataType.Int32:
                    return typeof(Int32);
                case enumDataType.Int64:
                    return typeof(Int64);
                case enumDataType.UInt16:
                    return typeof(UInt16);
                case enumDataType.UInt32:
                    return typeof(UInt32);
                case enumDataType.UInt64:
                    return typeof(UInt64);
                default:
                    return null;
            }
        }

        public static enumDataType DataType2Enum(Type type)
        { 
           if(type.Equals(typeof(byte)))
               return enumDataType.Byte ;
           else if (type.Equals(typeof(Int16)))
               return enumDataType.Int16;
           else if (type.Equals(typeof(UInt16)))
               return enumDataType.UInt16;
           else if (type.Equals(typeof(Int32)))
               return enumDataType.Int32;
           else if (type.Equals(typeof(UInt32)))
               return enumDataType.UInt32;
           else if (type.Equals(typeof(Int64)))
               return enumDataType.Int64;
           else if (type.Equals(typeof(UInt64)))
               return enumDataType.UInt64;
           else if (type.Equals(typeof(float)))
               return enumDataType.Float;
           else if (type.Equals(typeof(double)))
               return enumDataType.Double;
           return enumDataType.Atypism;
        }

        public static long MinValue(enumDataType dataType)
        {
            switch (dataType)
            {
                case enumDataType.Byte:
                    return byte.MinValue;
                case enumDataType.Double:
                    return 0;
                case enumDataType.Float:
                    return 0;
                case enumDataType.Int16:
                    return Int16.MinValue;
                case enumDataType.Int32:
                    return Int32.MinValue;
                case enumDataType.Int64:
                    return Int64.MinValue;
                case enumDataType.UInt16:
                    return UInt16.MinValue;
                case enumDataType.UInt32:
                    return UInt32.MinValue;
                case enumDataType.UInt64:
                    return (long)UInt64.MinValue;
                default:
                    return 0;
            }
        }

        public static long MaxValue(enumDataType dataType)
        {
            switch (dataType)
            {
                case enumDataType.Byte:
                    return byte.MaxValue;
                case enumDataType.Double:
                    return 0;
                case enumDataType.Float:
                    return 0;
                case enumDataType.Int16:
                    return Int16.MaxValue;
                case enumDataType.Int32:
                    return Int32.MaxValue;
                case enumDataType.Int64:
                    return Int64.MaxValue;
                case enumDataType.UInt16:
                    return UInt16.MaxValue;
                case enumDataType.UInt32:
                    return UInt32.MaxValue;
                case enumDataType.UInt64:
                    return (long)Int64.MaxValue;
                default:
                    return 0;
            }
        }
    }
}
