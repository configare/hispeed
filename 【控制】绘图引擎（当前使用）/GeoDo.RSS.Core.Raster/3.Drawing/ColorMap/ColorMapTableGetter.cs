using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.Core.RasterDrawing
{
    internal static class ColorMapTableGetterFactory
    {
        public static IColorMapTableGetter GetColorTableGetter(enumDataType dataType,ColorMapTable<double> oColorTable)
        {
            switch (dataType)
            {
                case enumDataType.Byte:
                    return new ColorMapTableSetter<byte>(oColorTable);
                case enumDataType.Int16:
                    return new ColorMapTableSetter<Int16>(oColorTable);
                case enumDataType.UInt16:
                    return new ColorMapTableSetter<UInt16>(oColorTable);
                case enumDataType.Int32:
                    return new ColorMapTableSetter<Int32>(oColorTable);
                case enumDataType.UInt32:
                    return new ColorMapTableSetter<UInt32>(oColorTable);
                case enumDataType.Int64:
                    return new ColorMapTableSetter<Int64>(oColorTable);
                case enumDataType.Float:
                    return new ColorMapTableSetter<float>(oColorTable);
                case enumDataType.Double:
                    return new ColorMapTableSetter<double>(oColorTable);
                default:
                    return null;
            }
        }
    }

    internal class ColorMapTableSetter<T> : IColorMapTableGetter
    {
        private object _stretcher = null;
        private ColorMapTable<int> _colorTable = null;

        public ColorMapTableSetter(ColorMapTable<double> oColorMapTable)
        {
            enumDataType dataType = DataTypeHelper.DataType2Enum(typeof(T));
            switch (dataType)
            {
                case enumDataType.Byte:
                    Func<byte, byte> stByte = (v) =>
                    {
                        for (byte i = 0; i < oColorMapTable.Items.Count; i++)
                            if (v >= (byte)oColorMapTable.Items[i].MinValue && v < (byte)oColorMapTable.Items[i].MaxValue)
                                return (byte)(i + 1);
                        return 255;
                    };
                    _stretcher = stByte;
                    break;
                case enumDataType.Float:
                    Func<float, byte> stFloat = (v) =>
                    {
                        for (byte i = 0; i < oColorMapTable.Items.Count; i++)
                            if (v >= (float)oColorMapTable.Items[i].MinValue && v < (float)oColorMapTable.Items[i].MaxValue)
                                return (byte)(i + 1);
                        return 255;
                    };
                    _stretcher = stFloat;
                    break;
                case enumDataType.Double:
                    Func<double, byte> stDouble = (v) =>
                    {
                        for (byte i = 0; i < oColorMapTable.Items.Count; i++)
                            if (v >= (double)oColorMapTable.Items[i].MinValue && v < (double)oColorMapTable.Items[i].MaxValue)
                                return (byte)(i + 1);
                        return 255;
                    };
                    _stretcher = stDouble;
                    break;
                case enumDataType.UInt16:
                    Func<UInt16, byte> stUInt16 = (v) =>
                    {
                        for (byte i = 0; i < oColorMapTable.Items.Count; i++)
                            if (v >= (UInt16)oColorMapTable.Items[i].MinValue && v < (UInt16)oColorMapTable.Items[i].MaxValue)
                                return (byte)(i + 1);
                        return 255;
                    };
                    _stretcher = stUInt16;
                    break;
                case enumDataType.Int16:
                    Func<Int16, byte> stInt16 = (v) =>
                    {
                        for (byte i = 0; i < oColorMapTable.Items.Count; i++)
                            if (v >= (Int16)oColorMapTable.Items[i].MinValue && v < (Int16)oColorMapTable.Items[i].MaxValue)
                                return (byte)(i + 1);
                        return 255;
                    };
                    _stretcher = stInt16;
                    break;
                case enumDataType.Int32:
                    Func<Int32, byte> stInt32 = (v) =>
                    {
                        for (byte i = 0; i < oColorMapTable.Items.Count; i++)
                            if (v >= (Int32)oColorMapTable.Items[i].MinValue && v < (Int32)oColorMapTable.Items[i].MaxValue)
                                return (byte)(i + 1);
                        return 255;
                    };
                    _stretcher = stInt32;
                    break;
                case enumDataType.UInt32:
                    Func<UInt32, byte> stUInt32 = (v) =>
                    {
                        for (byte i = 0; i < oColorMapTable.Items.Count; i++)
                            if (v >= (UInt32)oColorMapTable.Items[i].MinValue && v < (UInt32)oColorMapTable.Items[i].MaxValue)
                                return (byte)(i + 1);
                        return 255;
                    };
                    _stretcher = stUInt32;
                    break;
                case enumDataType.Int64:
                    Func<Int64, byte> stInt64 = (v) =>
                    {
                        for (byte i = 0; i < oColorMapTable.Items.Count; i++)
                            if (v >= (Int64)oColorMapTable.Items[i].MinValue && v < (Int64)oColorMapTable.Items[i].MaxValue)
                                return (byte)(i + 1);
                        return 255;
                    };
                    _stretcher = stInt64;
                    break;
                case enumDataType.UInt64:
                    Func<UInt64, byte> stUInt64 = (v) =>
                    {
                        for (byte i = 0; i < oColorMapTable.Items.Count; i++)
                            if (v >= (UInt64)oColorMapTable.Items[i].MinValue && v < (UInt64)oColorMapTable.Items[i].MaxValue)
                                return (byte)(i + 1);
                        return 255;
                    };
                    _stretcher = stUInt64;
                    break;
            }
            _colorTable = new ColorMapTable<int>();
            for (byte i = 1; i <= oColorMapTable.Items.Count; i++)  //如果Count>=255则出错。因为byte类型的i=255时候，i++后，i=0了
                _colorTable.Items.Add(new ColorMapItem<int>(i, i + 1, oColorMapTable.Items[i - 1].Color));
        }

        public object Stretcher
        {
            get { return _stretcher as Func<T, byte>; }
        }

        public ColorMapTable<int> ColorTable
        {
            get { return _colorTable; }
        }
    }
}
