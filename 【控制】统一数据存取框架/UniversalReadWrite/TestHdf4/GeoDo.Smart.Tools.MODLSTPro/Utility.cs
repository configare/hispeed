using System;
using GeoDo.HDF4;
using HDF5DotNet;

namespace GeoDo.Smart.Tools.MODLSTPro
{
    public static class Utility
    {
        public static object EnumParse(Type type, string value)
        {
            var obj = Enum.Parse(type, value);
            if (!Enum.IsDefined(type, obj))
                throw new Exception(String.Format("{0} 中不存在 {1},无法进行转换！", type, value));
            return obj;
        }

        public static T EnumParse<T>(string enumString, T defaultType)
        {
            var obj = EnumParse(typeof(T), enumString);
            var co = obj is T ? (T)obj : defaultType;
            return co;
        }

        public static H5T.H5Type GetH5Type(HDF4Helper.DataTypeDefinitions dataType)
        {
            switch (dataType)
            {
                case HDF4Helper.DataTypeDefinitions.DFNT_CHAR8:
                //case HDF4Helper.DataTypeDefinitions.DFNT_CHAR:
                case HDF4Helper.DataTypeDefinitions.DFNT_INT8:
                case HDF4Helper.DataTypeDefinitions.DFNT_NINT8:
                    return H5T.H5Type.NATIVE_SCHAR;
                case HDF4Helper.DataTypeDefinitions.DFNT_UCHAR8:
                //case HDF4Helper.DataTypeDefinitions.DFNT_UCHAR:
                case HDF4Helper.DataTypeDefinitions.DFNT_UINT8:
                case HDF4Helper.DataTypeDefinitions.DFNT_NUINT8:
                    return H5T.H5Type.NATIVE_UCHAR;
                case HDF4Helper.DataTypeDefinitions.DFNT_INT16:
                case HDF4Helper.DataTypeDefinitions.DFNT_NINT16:
                    return H5T.H5Type.NATIVE_SHORT;
                case HDF4Helper.DataTypeDefinitions.DFNT_UINT16:
                case HDF4Helper.DataTypeDefinitions.DFNT_NUINT16:
                    return H5T.H5Type.NATIVE_USHORT;
                case HDF4Helper.DataTypeDefinitions.DFNT_INT32:
                case HDF4Helper.DataTypeDefinitions.DFNT_NINT32:
                    return H5T.H5Type.NATIVE_INT;
                case HDF4Helper.DataTypeDefinitions.DFNT_UINT32:
                case HDF4Helper.DataTypeDefinitions.DFNT_NUINT32:
                    return H5T.H5Type.NATIVE_UINT;
                case HDF4Helper.DataTypeDefinitions.DFNT_FLOAT32:
                case HDF4Helper.DataTypeDefinitions.DFNT_NFLOAT32:
                    return H5T.H5Type.NATIVE_FLOAT;
                case HDF4Helper.DataTypeDefinitions.DFNT_FLOAT64:
                case HDF4Helper.DataTypeDefinitions.DFNT_NFLOAT64:
                    return H5T.H5Type.NATIVE_DOUBLE;
                default:
                    return 0;
            }
        }

        public static Type GetBaseType(HDF4Helper.DataTypeDefinitions dataType)
        {
            switch (dataType)
            {
                case HDF4Helper.DataTypeDefinitions.DFNT_INT8:
                case HDF4Helper.DataTypeDefinitions.DFNT_NINT8:
                //case HDF4Helper.DataTypeDefinitions.DFNT_UCHAR:
                case HDF4Helper.DataTypeDefinitions.DFNT_UINT8:
                case HDF4Helper.DataTypeDefinitions.DFNT_NUINT8:
                    return typeof(byte);
                case HDF4Helper.DataTypeDefinitions.DFNT_CHAR8:
                case HDF4Helper.DataTypeDefinitions.DFNT_UCHAR8:
                    return typeof(string);
                case HDF4Helper.DataTypeDefinitions.DFNT_INT16:
                case HDF4Helper.DataTypeDefinitions.DFNT_NINT16:
                    return typeof(short);
                case HDF4Helper.DataTypeDefinitions.DFNT_UINT16:
                case HDF4Helper.DataTypeDefinitions.DFNT_NUINT16:
                    return typeof(ushort);
                case HDF4Helper.DataTypeDefinitions.DFNT_INT32:
                case HDF4Helper.DataTypeDefinitions.DFNT_NINT32:
                    return typeof(int);
                case HDF4Helper.DataTypeDefinitions.DFNT_UINT32:
                case HDF4Helper.DataTypeDefinitions.DFNT_NUINT32:
                    return typeof(uint);
                case HDF4Helper.DataTypeDefinitions.DFNT_FLOAT32:
                case HDF4Helper.DataTypeDefinitions.DFNT_NFLOAT32:
                    return typeof(float);
                case HDF4Helper.DataTypeDefinitions.DFNT_FLOAT64:
                case HDF4Helper.DataTypeDefinitions.DFNT_NFLOAT64:
                    return typeof(double);
                default:
                    return typeof(int);
            }
        }
    }
}