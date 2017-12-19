using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.EC_GRIB
{
    internal class PDSClass
    {
        internal int PDSLength { get; set; }
        internal string OriginatingCenter { get; set; }
        internal enumThreeSection ThreeSection { get; set; }
        internal string Parameter { get; set; }
        internal string Units { get; set; }
        internal string LevelType { get; set; }
        internal int LevelValue { get; set; }
        internal string TimeString { get; set; }
        internal int DecimalFactor { get; set; }

        internal PDSClass(FileStream fs, BinaryReader br)
        {
            if (br == null)
                return;
            PDSLength = MathHelper.Bytes2Int(br.ReadBytes(3));  //01-03,Length in bytes of PDS
            if (PDSLength == 0)
                return;
            //04	Parameter table version number - currently 2 for international exchange
            fs.Seek(1, SeekOrigin.Current);
            //05	Originating Center
            OriginatingCenter = GribHelper.GetOringinatingCenter(br.ReadByte());
            //06	Generating process or model ID (center dependent)
            //07	Grid identification - used for fixed grid types, GDS is used for specific grid definition
            //08	Flag specifying the presence or absence of a GDS or a BMS
            fs.Seek(2, SeekOrigin.Current);
            ThreeSection = GribHelper.GetThreeSection(br.ReadByte());
            //09	Parameter and units
            object[] objs = GribHelper.GetParameterAndUnits(br.ReadByte());
            if (objs != null && objs.Length != 0)
            {
                Parameter = objs[0].ToString();
                Units = objs[1].ToString();
            }
            //10	Level or Layer Type
            //11-12	Level or Layer values
            objs = GribHelper.GetLevelOrLayer(br.ReadByte(), MathHelper.Bytes2Int(br.ReadBytes(2)));
            //if (objs != null && objs.Length != 0)
            //{
            //    LevelType = objs[0].ToString();
            //    LevelValue = (int)objs[1];
            //}

            //13	Reference Time - Year of century
            //14	Month of year
            //15	Day of month
            //16	Hour of day
            //17	Minute of hour
            byte[] byteVlues = br.ReadBytes(5);
            DateTime time = GribHelper.GetDateTime(byteVlues);
            TimeString = time.ToString("yyyy-MM-dd hh:mm");
            //18	Forecast time unit
            //19	P1 - Period of time (Number of time units). 0 for analysis or initialized analysis.
            //20	P2 - Period of time (Number of time units) or time interval between successive analyses, successive initialized analyses, or forecasts, undergoing averaging or accumulation.
            //21	Time range indicator

            //22-23	Number included in average, when octet 21 (Table 5) indicates an average or accumulation; otherwise set to zero.
            //24	Number Missing from averages or accumulations.
            //25	Century of Initial (Reference) time (=20 until Jan. 1, 2001, 21 afterwards)
            //26	Identification of sub-center
            //27-28	The decimal scale factor D. A negative value is indicated by setting the high order bit (bit No. 1) in octet 27 to 1 (on).
            fs.Seek(9, SeekOrigin.Current);
            DecimalFactor = MathHelper.Bytes2Int(br.ReadBytes(2));
            //29-40	Reserved (need not be present)
            //41-nnn	Reserved for originating center use.
            fs.Seek(PDSLength - 28, SeekOrigin.Current);
        }
    }

    public enum enumThreeSection
    {
        GDS,
        BMS
    }
}
