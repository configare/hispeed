using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using OSGeo.GDAL;

namespace GeoDo.RSS.DF.GDAL
{
    public static class GDALHelper
    {
        public static enumDataType GDALDataType2DataType(OSGeo.GDAL.DataType dataType)
        {
            switch (dataType)
            {
                case OSGeo.GDAL.DataType.GDT_Byte:
                    return enumDataType.Byte;
                case OSGeo.GDAL.DataType.GDT_Float32:
                    return enumDataType.Float;
                case OSGeo.GDAL.DataType.GDT_Float64:
                    return enumDataType.Double;
                case OSGeo.GDAL.DataType.GDT_Int16:
                    return enumDataType.Int16;
                case OSGeo.GDAL.DataType.GDT_Int32:
                    return enumDataType.Int32;
                case OSGeo.GDAL.DataType.GDT_UInt16:
                    return enumDataType.UInt16;
                case OSGeo.GDAL.DataType.GDT_UInt32:
                    return enumDataType.UInt32;
                default:
                    throw new DataTypeIsNotSupportException(dataType.ToString());
            }
        }

        public static DataType DataType2GDALDataType(enumDataType dataType)
        {
            switch (dataType)
            { 
                case enumDataType.Byte:
                    return DataType.GDT_Byte;
                case enumDataType.Double:
                    return DataType.GDT_Float64;
                case enumDataType.Float:
                    return DataType.GDT_Float32;
                case enumDataType.Int16:
                    return DataType.GDT_Int16;
                case enumDataType.Int32:
                    return DataType.GDT_Int32;
                case enumDataType.UInt16:
                    return DataType.GDT_UInt16;
                case enumDataType.UInt32:
                    return DataType.GDT_UInt32;
                case enumDataType.Atypism:
                case enumDataType.Bits:
                default:
                    return DataType.GDT_Unknown;
            }
        }

        public static void GetBandAttributes(Band band, AttributeManager attributes)
        {
            GetAttributes(domain => band.GetMetadata(domain), attributes);
        }

        public static void GetDatasetAttributes(Dataset dataset, AttributeManager attributes)
        {
            GetAttributes(domain => dataset.GetMetadata(domain), attributes);
        }

        private static void GetAttributes(Func<string, string[]> getAttributeHandler, AttributeManager attributes)
        {
            string[] domains = new string[] { "SUBDATASETS", "IMAGE_STRUCTURE", "RFC" };
            string[] atts = null;
            foreach (string domain in domains)
            {
                Dictionary<string, string> dic = attributes.CreateAttributeDomain(domain);
                atts = getAttributeHandler(domain);
                foreach (string att in atts)
                {
                    string[] parts = att.Split('=');
                    if (parts.Length == 1)
                        dic.Add(parts[0], parts[0]);
                    else if (parts.Length == 2)
                    {
                        dic.Add(parts[0], parts[1]);
                    }
                }
            }
        }
    }
}
