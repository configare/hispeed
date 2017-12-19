using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.UI.WinForm
{
    /// <summary>
    /// 定量产品色标
    /// </summary>
    public class RgbStretcherProvider : IRgbStretcherProvider
    {
        public object GetStretcher(string fname, enumDataType inDataType, string colorTableName, out ColorMapTable<int> colorMapTable)
        {
            colorMapTable = null;
            ProductColorTable pct = null;
            if (colorTableName == null)
            {
                string[] parts = Path.GetFileName(fname).Split('_');
                //by chennan 20120819 单通道ldf产品付色
                //if (parts.Length < 3 && !fname.ToUpper().EndsWith(".DAT"))
                if (parts.Length < 3)
                    return null;
                string productIdentify = parts[0];
                string subProductIdentify = parts[1];
                pct = ProductColorTableFactory.GetColorTable(productIdentify, subProductIdentify);
            }
            else
            {
                pct = ProductColorTableFactory.GetColorTable(colorTableName);
            }
            if (pct == null)
                return null;
            //enumDataType dataType = enumDataType.Atypism;
            if (inDataType == enumDataType.Atypism)
                using (IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider)
                {
                    inDataType = prd.DataType;
                }
            return GetStretcher(inDataType, pct, out colorMapTable);
        }

        /// <summary>
        /// 根据文件名或颜色表查找分段填色表
        /// 从GeoDo.RSS.Core.RasterDrawing.DataProviderReader<T>中调用
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="colorTableName"></param>
        /// <param name="colorMapTable"></param>
        /// <returns></returns>
        public object GetStretcher(string fname, string colorTableName, out ColorMapTable<int> colorMapTable)
        {
            colorMapTable = null;
            return GetStretcher(fname, enumDataType.Atypism, colorTableName, out colorMapTable);
        }

        public object GetStretcher(enumDataType dataType, ProductColorTable oColorMapTable, out ColorMapTable<int> colorMapTable)
        {
            object stretcher = null;
            switch (dataType)
            {
                case enumDataType.Byte:
                    Func<byte, byte> stByte = (v) =>
                    {
                        for (byte i = 0; i < oColorMapTable.ProductColors.Length; i++)
                            if (v >= oColorMapTable.ProductColors[i].MinValue && v < oColorMapTable.ProductColors[i].MaxValue)
                                return (byte)(i + 1);
                        return 255;
                    };
                    stretcher = stByte;
                    break;
                case enumDataType.Float:
                    Func<float, byte> stFloat = (v) =>
                    {
                        for (byte i = 0; i < oColorMapTable.ProductColors.Length; i++)
                            if (v >= oColorMapTable.ProductColors[i].MinValue && v < oColorMapTable.ProductColors[i].MaxValue)
                                return (byte)(i + 1);
                        return 255;
                    };
                    stretcher = stFloat;
                    break;
                case enumDataType.Double:
                    Func<double, byte> stDouble = (v) =>
                    {
                        for (byte i = 0; i < oColorMapTable.ProductColors.Length; i++)
                            if (v >= oColorMapTable.ProductColors[i].MinValue && v < oColorMapTable.ProductColors[i].MaxValue)
                                return (byte)(i + 1);
                        return 255;
                    };
                    stretcher = stDouble;
                    break;
                case enumDataType.UInt16:
                    Func<UInt16, byte> stUInt16 = (v) =>
                    {
                        for (byte i = 0; i < oColorMapTable.ProductColors.Length; i++)
                            if (v >= oColorMapTable.ProductColors[i].MinValue && v < oColorMapTable.ProductColors[i].MaxValue)
                                return (byte)(i + 1);
                        return 255;
                    };
                    stretcher = stUInt16;
                    break;
                case enumDataType.Int16:
                    Func<Int16, byte> stInt16 = (v) =>
                    {
                        for (byte i = 0; i < oColorMapTable.ProductColors.Length; i++)
                            if (v >= oColorMapTable.ProductColors[i].MinValue && v < oColorMapTable.ProductColors[i].MaxValue)
                                return (byte)(i + 1);
                        return 255;
                    };
                    stretcher = stInt16;
                    break;
                case enumDataType.Int32:
                    Func<Int32, byte> stInt32 = (v) =>
                    {
                        for (byte i = 0; i < oColorMapTable.ProductColors.Length; i++)
                            if (v >= oColorMapTable.ProductColors[i].MinValue && v < oColorMapTable.ProductColors[i].MaxValue)
                                return (byte)(i + 1);
                        return 255;
                    };
                    stretcher = stInt32;
                    break;
                case enumDataType.UInt32:
                    Func<UInt32, byte> stUInt32 = (v) =>
                    {
                        for (byte i = 0; i < oColorMapTable.ProductColors.Length; i++)
                            if (v >= oColorMapTable.ProductColors[i].MinValue && v < oColorMapTable.ProductColors[i].MaxValue)
                                return (byte)(i + 1);
                        return 255;
                    };
                    stretcher = stUInt32;
                    break;
                case enumDataType.Int64:
                    Func<Int64, byte> stInt64 = (v) =>
                    {
                        for (byte i = 0; i < oColorMapTable.ProductColors.Length; i++)
                            if (v >= oColorMapTable.ProductColors[i].MinValue && v < oColorMapTable.ProductColors[i].MaxValue)
                                return (byte)(i + 1);
                        return 255;
                    };
                    stretcher = stInt64;
                    break;
                case enumDataType.UInt64:
                    Func<UInt64, byte> stUInt64 = (v) =>
                    {
                        for (byte i = 0; i < oColorMapTable.ProductColors.Length; i++)
                            if (v >= oColorMapTable.ProductColors[i].MinValue && v < oColorMapTable.ProductColors[i].MaxValue)
                                return (byte)(i + 1);
                        return 255;
                    };
                    stretcher = stUInt64;
                    break;
            }
            colorMapTable = new ColorMapTable<int>();
            for (byte i = 1; i <= oColorMapTable.ProductColors.Length; i++)
                colorMapTable.Items.Add(new ColorMapItem<int>(i, i + 1, oColorMapTable.ProductColors[i - 1].Color));
            return stretcher;

        }
    }
}
