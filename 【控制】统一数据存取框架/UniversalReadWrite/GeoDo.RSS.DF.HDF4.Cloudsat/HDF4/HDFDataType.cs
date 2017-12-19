using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.HDF4;

namespace GeoDo.RSS.DF.HDF4.Cloudsat
{
    public class HDFDataType
    {
        public static int GetSize(HDF4Helper.DataTypeDefinitions dataType)
        {
            switch (dataType)
            {
                case HDF4Helper.DataTypeDefinitions.DFNT_CHAR8:
                //case HDF4Helper.DataTypeDefinitions.DFNT_CHAR:
                case HDF4Helper.DataTypeDefinitions.DFNT_UCHAR8:
                //case HDF4Helper.DataTypeDefinitions.DFNT_UCHAR:
                case HDF4Helper.DataTypeDefinitions.DFNT_INT8:
                case HDF4Helper.DataTypeDefinitions.DFNT_UINT8:
                case HDF4Helper.DataTypeDefinitions.DFNT_NINT8:
                case HDF4Helper.DataTypeDefinitions.DFNT_NUINT8:
                    return 8;
                case HDF4Helper.DataTypeDefinitions.DFNT_INT16:
                case HDF4Helper.DataTypeDefinitions.DFNT_UINT16:
                case HDF4Helper.DataTypeDefinitions.DFNT_NINT16:
                case HDF4Helper.DataTypeDefinitions.DFNT_NUINT16:
                    return 16;
                case HDF4Helper.DataTypeDefinitions.DFNT_INT32:
                case HDF4Helper.DataTypeDefinitions.DFNT_UINT32:
                case HDF4Helper.DataTypeDefinitions.DFNT_FLOAT32:
                case HDF4Helper.DataTypeDefinitions.DFNT_NINT32:
                case HDF4Helper.DataTypeDefinitions.DFNT_NUINT32:
                case HDF4Helper.DataTypeDefinitions.DFNT_NFLOAT32:
                    return 32;
                case HDF4Helper.DataTypeDefinitions.DFNT_FLOAT64:
                case HDF4Helper.DataTypeDefinitions.DFNT_NFLOAT64:
                    return 64;
                default:
                    return 0;
            }
        }
    }
}
