using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.EC_GRIB
{
    public static class GribHelper
    {
        public static string GetOringinatingCenter(byte value)
        {
            switch (value)
            {
                case 7:
                    return "US Weather Service - National Met. Center";
                case 8:
                    return "US Weather Service - NWS Telecomms Gateway";
                case 9:
                    return "US Weather Service - Field Stations";
                case 34:
                    return "Japanese Meteorological Agency - Tokyo";
                case 52:
                    return "National Hurricane Center, Miami";
                case 54:
                    return "Canadian Meteorological Service - Montreal";
                case 57:
                    return "U.S. Air Force - Global Weather Center";
                case 58:
                    return "US Navy - Fleet Numerical Oceanography Center";
                case 59:
                    return "NOAA Forecast Systems Lab, Boulder CO";
                case 74:
                    return "U.K. Met Office - Bracknell";
                case 85:
                    return "French Weather Service - Toulouse";
                case 97:
                    return "European Space Agency (ESA)";
                case 98:
                    return "European Center for Medium-Range Weather Forecasts - Reading";
                case 99:
                    return "DeBilt, Netherlands";
                default:
                    return "NON";
            }
        }

        public static enumThreeSection GetThreeSection(byte value)
        {
            if (value == 128)
                return enumThreeSection.GDS;
            else
                return enumThreeSection.BMS;
        }

        public static object[] GetParameterAndUnits(byte value)
        {
            if (value >= 128 && value <= 254)
                return null;
            //
            //
            //
            return null;
        }

        public static object[] GetLevelOrLayer(byte type, int value)
        {
            switch (type)
            {
                case 1:
                    return new object[] { "surface of earth including sea surface" };
                case 100:
                    return new object[] { "isobaric level", value };
                default:
                    return null;
            }
        }

        public static DateTime GetDateTime(byte[] timeValue)
        {
            if (timeValue == null || timeValue.Length != 5)
                return DateTime.MinValue;
            return new DateTime(2000 + timeValue[0], timeValue[1], (int)timeValue[2], timeValue[3], timeValue[4], 0);
        }

        public static float ComputeActualValue(float xdata, int dvalue, float evalue, float rvalue)
        {
            rvalue = (float)(Math.Pow(2, (-24)) * 1218018 * Math.Pow(16, (68 - 64)));
            return (float)((rvalue + (xdata * Math.Pow(2, evalue))) / Math.Pow(10, dvalue));
        }
    }
}
