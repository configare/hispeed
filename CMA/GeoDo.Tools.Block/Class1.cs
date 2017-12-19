using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GeoDo.Tools.Block
{
    /// <summary>
    /// TERRA_2010_03_25_03_09_GZ.MOD021KM.hdf
    /// FY3A_MERSI_GBAL_L1_20110426_0245_1000M_MS.HDF
    /// FY1D_AVHRR_HRPT_L1_ORB_MLT_NUL_20090831_2209_1100M_PJ.1B
    /// NOAA18_AVHRR_CHINA_L1_20090806_N3_1000M.1bd
    /// NA18_AVHRR_HRPT_L1_ORB_MLT_NUL_20120319_1845_1100M_PJ.L1B
    /// </summary>
    public class Class1
    {
        private static Regex[] DateTimeReg = new Regex[]
        {
            new Regex(@"_(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})_(?<hour>\d{2})(?<min>\d{2})_", RegexOptions.Compiled),
            new Regex(@"_(?<year>\d{4})_(?<month>\d{2})_(?<day>\d{2})_(?<hour>\d{2})_(?<min>\d{2})_", RegexOptions.Compiled),
            new Regex(@"_(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})(?<hour>\d{2})(?<min>\d{2})(?<sec>\d{2})_", RegexOptions.Compiled)
        };

        private static Regex Date = new Regex(@"_(?<date>\d{8})_", RegexOptions.Compiled);

        public static DateTime GetOrbitDateTime(string fname)
        {
            foreach (Regex rex in DateTimeReg)
            {
                Match m = rex.Match(fname);
                if (m.Success)
                {
                    DateTime dt;
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
    }
}
