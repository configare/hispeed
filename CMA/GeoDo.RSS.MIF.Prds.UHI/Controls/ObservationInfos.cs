using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace GeoDo.RSS.MIF.Prds.UHI
{
    public class ObservationInfo
    { private string _name = string.Empty;
        private float _lon = float.MinValue;
        private float _lat = float.MinValue;
        private float _chazhi = float.MinValue;

        public ObservationInfo()
        { }

        public ObservationInfo(string name, float lon, float lat, float chazhi)
        {
            _name = name;
            _lon=lon;
            _lat = lat;
            _chazhi = chazhi;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public float Lon
        {
            get { return _lon; }
            set { _lon = value; }
        }

        public float Lat
        {
            get { return _lat; }
            set { _lat = value; }
        }

        public float chazhi
        {
            get { return _chazhi; }
            set { _chazhi = value; }
        }

        public static ObservationInfo[] ReadFile(string file)
        {
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
                return null;
            string[] content = File.ReadAllLines(file, Encoding.Default);
            if (content == null || content.Length == 0)
                return null;
            List<ObservationInfo> infos = new List<ObservationInfo>();
            ObservationInfo wi = null;
            string[] values = null;
            foreach (string item in content)
            {
                values = item.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (values == null || values.Length == 0 || values.Length != 4)
                    continue;
                float chazhi=0;
                float.TryParse(values[3],out chazhi);
                wi = new ObservationInfo(values[0], GetLonLat(values[1]), GetLonLat(values[2]), chazhi);
                if (wi.Lon == float.MinValue || wi.Lat == float.MinValue)
                    continue;
                infos.Add(wi);
            }
            return infos.Count==0?null:infos.ToArray();
        }

        //113°53′37.0575″
        private static string regStr = @"^(?<du>\d+)°\s*(?<fen>\d+)′\s*(?<miao>\d+(\.\d+)?)″$";
        private static Regex re = null;
        private static float GetLonLat(string lonlat)
        {
            re = new Regex(regStr);
            if(re.IsMatch(lonlat))
            {
                Match m = re.Match(lonlat);
                return float.Parse(m.Groups["du"].ToString()) + float.Parse(m.Groups["fen"].ToString()) / 60
                    + float.Parse(m.Groups["miao"].ToString()) / 3600;
            }
            return float.MinValue;
        }
    }
}
