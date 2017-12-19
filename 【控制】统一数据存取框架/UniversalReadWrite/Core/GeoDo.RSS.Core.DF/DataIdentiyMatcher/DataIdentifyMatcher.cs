using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace GeoDo.RSS.Core.DF
{
    public static class DataIdentifyMatcher
    {
        private static Regex[] SATELLITES = new Regex[]
        {
            new Regex(@"(?<ID>NOAA-\d+)_",RegexOptions.Compiled),
            new Regex(@"(?<ID>NOAA\d*)",RegexOptions.Compiled),
            new Regex(@"(?<ID>NA\d+)_",RegexOptions.Compiled),
            new Regex(@"(?<ID>FY_\d*\S?)",RegexOptions.Compiled),
            new Regex(@"(?<ID>FY\d*\S?)",RegexOptions.Compiled),
            new Regex(@"(?<ID>TERRA)",RegexOptions.Compiled),
            new Regex(@"(?<ID>AQUA)",RegexOptions.Compiled),
            new Regex(@"(?<ID>EOST)",RegexOptions.Compiled),
            new Regex(@"(?<ID>EOSA)",RegexOptions.Compiled),
            new Regex(@"(?<ID>EOS)",RegexOptions.Compiled),
            new Regex(@"(?<ID>MYD\d+)",RegexOptions.Compiled),
            new Regex(@"(?<ID>MOD\d+)",RegexOptions.Compiled),
            new Regex(@"(?<ID>YH\d+)",RegexOptions.Compiled),
            new Regex(@"(?<ID>MST\d+)",RegexOptions.Compiled)
        };

        private static Regex[] SENSORS = new Regex[]
        {
            new Regex(@"(?<ID>VIRR)",RegexOptions.Compiled),
            new Regex(@"(?<ID>VIRRD)",RegexOptions.Compiled),
            new Regex(@"(?<ID>MERSI)",RegexOptions.Compiled),
            new Regex(@"(?<ID>AVHRR)",RegexOptions.Compiled),
            new Regex(@"(?<ID>MODIS)",RegexOptions.Compiled),
            new Regex(@"(?<ID>MOD)",RegexOptions.Compiled),
            new Regex(@"(?<ID>FDI)",RegexOptions.Compiled),
            new Regex(@"(?<ID>REG)",RegexOptions.Compiled),
            new Regex(@"(?<ID>VISSR)",RegexOptions.Compiled),
            new Regex(@"(?<ID>MYD\d+)",RegexOptions.Compiled),
        };

        public static DataIdentify Match(string fname)
        {
            if (string.IsNullOrEmpty(fname))
                return null;
            fname = Path.GetFileName(fname).ToUpper();
            DataIdentify it = new DataIdentify();
            it.Satellite = GetSatellite(fname);
            it.Sensor = GetSensor(fname);
            it.OrbitDateTime = GetOrbitDateTime(fname);
            return it;
        }

        private static string GetSatellite(string fname)
        {
            foreach (Regex reg in SATELLITES)
            {
                Match m = reg.Match(fname);
                if (m.Success)
                {
                    //if (reg.ToString() == @"(?<ID>MYD\d+)")
                    //    return "AQUA";
                    //else if (reg.ToString() == @"(?<ID>MOD\d+)")
                    //    return "TERRA";
                    if (reg.ToString() == @"(?<ID>MYD\d+)")
                        return "EOSA";
                    else if (reg.ToString() == @"(?<ID>MOD\d+)")
                        return "EOST";
                    else if (reg.ToString() == @"(?<ID>TERRA)")
                        return "EOST";
                    else if (reg.ToString() == @"(?<ID>AQUA)")
                        return "EOSA";
                    else
                        return m.Groups["ID"].Value;
                }
            }
            return null;
        }

        /// <summary>
        /// _yyyyMMdd_hhmm_
        /// yyyyMMdd_hhmm
        /// _yyyy_MM_dd_hh_mm
        /// </summary>
        private static Regex[] DateTimeReg = new Regex[]
        {
            new Regex(@"_(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})_(?<hour>\d{2})(?<min>\d{2})_", RegexOptions.Compiled),
            new Regex(@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})_(?<hour>\d{2})(?<min>\d{2})", RegexOptions.Compiled),
            new Regex(@"_(?<year>\d{4})_(?<month>\d{2})_(?<day>\d{2})_(?<hour>\d{2})_(?<min>\d{2})_", RegexOptions.Compiled),
            new Regex(@"_(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})(?<hour>\d{2})(?<min>\d{2})(?<sec>\d{2})_", RegexOptions.Compiled),
            new Regex(@"_(?<year>\d{4})_(?<month>\d{2})_(?<day>\d{2})_(?<hour>\d{2})_(?<min>\d{2})_", RegexOptions.Compiled),
            new Regex(@"_(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})(?<hour>\d{2})(?<min>\d{2})(?<sec>\d{2})", RegexOptions.Compiled),
            new Regex(@".\S{1}[A|Z](?<year>\d{4})(?<days>\d{3}).", RegexOptions.Compiled),
        };

        public static DateTime GetOrbitDateTime(string fname)
        {
            foreach (Regex rex in DateTimeReg)
            {
                Match m = rex.Match(fname);
                if (m.Success)
                {
                    DateTime dt = DateTime.MinValue;
                    if (rex.ToString() == @".\S{1}[A|Z](?<year>\d{4})(?<days>\d{3}).")
                    {
                        dt = dt.AddYears(int.Parse(m.Groups["year"].Value) - 1);
                        dt = dt.AddDays(int.Parse(m.Groups["days"].Value) - 1);
                    }
                    else
                        DateTime.TryParse(string.Format("{0}-{1}-{2} {3}:{4}:{5}",
                                m.Groups["year"].Value,
                                m.Groups["month"].Value,
                                m.Groups["day"].Value,
                                m.Groups["hour"].Value,
                                m.Groups["min"].Value,
                                m.Groups["sec"].Success ? m.Groups["sec"].Value : "00"), out dt);
                    return dt;
                }
            }
            return DateTime.MinValue;
        }

        private static string GetSensor(string fname)
        {
            foreach (Regex reg in SENSORS)
            {
                Match m = reg.Match(fname);
                if (m.Success)
                {
                    if (m.Groups["ID"].Value == "FDI" || m.Groups["ID"].Value == "REG")
                        return "VISSR";
                    if (reg.ToString() == @"(?<ID>MYD\d+)")
                        return "MODIS";
                    if (reg.ToString() == @"(?<ID>MOD)")
                        return "MODIS";
                    return m.Groups["ID"].Value;
                }
            }
            return null;
        }
    }
}
