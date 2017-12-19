using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public static class ProductColorTableFactory
    {
        private static ProductColorTable[] ColotTables = null;

        public static object[] GetStretcher<T>(ProductColorTable colorTable)
        {
            Type type = typeof(T);
            if (type.Equals(typeof(byte)))
                return (new ColorTable2RgbStretcherByte()).GetStretcher(colorTable);
            else if (type.Equals(typeof(Int16)))
                return (new ColorTable2RgbStretcherInt16()).GetStretcher(colorTable);
            else if (type.Equals(typeof(UInt16)))
                return (new ColorTable2RgbStretcherUInt16()).GetStretcher(colorTable);
            else if (type.Equals(typeof(Int32)))
                return (new ColorTable2RgbStretcherInt32()).GetStretcher(colorTable);
            else if (type.Equals(typeof(UInt32)))
                return (new ColorTable2RgbStretcherUInt32()).GetStretcher(colorTable);
            else if (type.Equals(typeof(Int64)))
                return (new ColorTable2RgbStretcherInt64()).GetStretcher(colorTable);
            else if (type.Equals(typeof(UInt64)))
                return (new ColorTable2RgbStretcherUInt64()).GetStretcher(colorTable);
            else if (type.Equals(typeof(float)))
                return (new ColorTable2RgbStretcherFloat()).GetStretcher(colorTable);
            else if (type.Equals(typeof(double)))
                return (new ColorTable2RgbStretcherDouble()).GetStretcher(colorTable);
            return null;
        }

        public static ProductColorTable[] GetAllColorTables()
        {
            if (ColotTables == null || ColotTables.Length == 0)
            {
                ProductColorTableParser parser = new ProductColorTableParser();
                ColotTables = parser.Load();
            }
            if (ColotTables == null || ColotTables.Length == 0)
                return null;
            return ColotTables;
        }

        public static void ReLoadAllColorTables()
        {
            ProductColorTableParser parser = new ProductColorTableParser();
            ColotTables = parser.Load();
        }

        public static ProductColorTable GetColorTable(string colorTableName)
        {
            if (string.IsNullOrEmpty(colorTableName))
                return null;
            ProductColorTable[] tables = GetAllColorTables();
            foreach (ProductColorTable colorTable in tables)
            {
                if (colorTable.ColorTableName != null && colorTable.ColorTableName == colorTableName)
                    return colorTable;
            }
            return null;
        }

        public static ProductColorTable GetColorTable(string productIdentify, string subProductIdentify)
        {
            ProductColorTable[] tables = GetAllColorTables();
            foreach (ProductColorTable colorTable in tables)
            {
                if (colorTable.Identify.ToUpper() == productIdentify.ToUpper()
                    && colorTable.SubIdentify == subProductIdentify.ToUpper())
                    return colorTable;
            }
            return null;
        }
    }
}
