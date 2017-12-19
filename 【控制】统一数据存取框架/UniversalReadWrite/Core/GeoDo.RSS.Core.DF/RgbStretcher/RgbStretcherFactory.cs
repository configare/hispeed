using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    /// <summary>
    /// GetDefaultBands新增支持区分白天、夜晚
    /// </summary>
    public static class RgbStretcherFactory
    {
        private static Dictionary<StretcherConfigItem, object> _stretchers = null;
        private static List<string> _autoStrechFileNameRulers;

        static RgbStretcherFactory()
        {
            LoadStretchers();
        }

        public static bool IsUseAutoStretcher(string fname)
        {
            if (_autoStrechFileNameRulers != null && _autoStrechFileNameRulers.Count > 0)
            {
                foreach (string ruler in _autoStrechFileNameRulers)
                    if (fname.Contains(ruler))
                        return true;
            }
            return false;
        }

        public static object GetStretcher(string productIdentify, string subProductIdentify)
        {
            if (productIdentify == null || subProductIdentify == null)
                return null;
            foreach (StretcherConfigItem item in _stretchers.Keys)
            {
                if (item.ProductIdentify == null || item.SubProductidentify == null)
                    continue;
                if (item.IsProduct && item.ProductIdentify == productIdentify && item.SubProductidentify == subProductIdentify)
                {
                    return _stretchers[item];
                }
            }
            return null;
        }

        public static object GetStretcher(string stretcherName)
        {
            if (string.IsNullOrWhiteSpace(stretcherName))
                return null;
            foreach (StretcherConfigItem item in _stretchers.Keys)
            {
                if (string.IsNullOrWhiteSpace(item.Name))
                    continue;
                if (item.Name == stretcherName)
                {
                    return _stretchers[item];
                }
            }
            return null;
        }

        public static object GetStretcher(string satellite, string sensor, bool isOrbit, int bandNo)
        {
            foreach (StretcherConfigItem item in _stretchers.Keys)
            {
                if (item.Satellites.Contains(satellite) && item.Sensors.Contains(sensor) &&
                    item.BandNo.Contains(bandNo) && item.IsOribit == isOrbit)
                {
                    return _stretchers[item];
                }
            }
            return null;
        }

        private static void LoadStretchers()
        {
            _stretchers = new Dictionary<StretcherConfigItem, object>();
            string ruler;
            StretcherConfigItem[] items = (new StretcherConfigParser()).Parser(out ruler);
            if (ruler != null)
            {
                string[] parts = ruler.Split(',');
                _autoStrechFileNameRulers = new List<string>(parts);
            }
            if (items != null)
            {
                foreach (StretcherConfigItem it in items)
                {
                    object obj = it.CreateStretcher();
                    if (obj != null)
                        _stretchers.Add(it, obj);
                }
            }
        }

        public static object CreateStretcher(enumDataType dataType, double minValue, double maxValue)
        {
            switch (dataType)
            {
                case enumDataType.Byte:
                    return new LinearRgbStretcherByte((byte)minValue, (byte)maxValue, 0, 255, false);
                case enumDataType.Int16:
                    return new LinearRgbStretcherInt16((Int16)minValue, (Int16)maxValue, 0, 255, false);
                case enumDataType.UInt16:
                    return new LinearRgbStretcherUInt16((UInt16)minValue, (UInt16)maxValue, 0, 255, false);
                case enumDataType.Int32:
                    return new LinearRgbStretcherInt32((Int32)minValue, (Int32)maxValue, 0, 255, false);
                case enumDataType.UInt32:
                    return new LinearRgbStretcherUInt32((UInt32)minValue, (UInt32)maxValue, 0, 255, false);
                case enumDataType.Int64:
                    return new LinearRgbStretcherInt64((Int64)minValue, (Int64)maxValue, 0, 255, false);
                case enumDataType.Float:
                    return new LinearRgbStretcherFloat((float)minValue, (float)maxValue, 0, 255, false);
                case enumDataType.UInt64:
                    return new LinearRgbStretcherUInt64((UInt64)minValue, (UInt64)maxValue, 0, 255, false);
                case enumDataType.Double:
                    return new LinearRgbStretcherDouble((double)minValue, (double)maxValue, 0, 255, false);
                default:
                    return null;
            }
        }

        public static int[] GetDefaultBands(string satellite, string sensor, bool isOrbit)
        {
            foreach (StretcherConfigItem item in _stretchers.Keys)
            {
                if (item.Satellites.Contains(satellite) &&
                    item.Sensors.Contains(sensor) &&
                    item.IsOribit == isOrbit
                    )
                {
                    int[] bands = item.DefaultBands;
                    if (bands != null)
                        return bands.Clone() as int[];
                    return null;
                }
            }
            return null;
        }

        public static int[] GetDefaultBands(string satellite, string sensor, bool isOrbit, bool isDay)
        {
            foreach (StretcherConfigItem item in _stretchers.Keys)
            {
                if (item.Satellites.Contains(satellite) &&
                    item.Sensors.Contains(sensor) &&
                    item.IsOribit == isOrbit
                    )
                {
                    if (!isDay && item.DefaultBandsExt != null)
                        return item.DefaultBandsExt.Clone() as int[];
                    else
                    {
                        int[] bands = item.DefaultBands;
                        if (bands != null)
                            return bands.Clone() as int[];
                    }
                    return null;
                }
            }
            return null;
        }
    }
}
