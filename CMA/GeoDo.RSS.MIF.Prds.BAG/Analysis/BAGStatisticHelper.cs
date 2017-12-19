using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace GeoDo.RSS.MIF.Prds.BAG
{
    public class BAGStatisticHelper
    {
        private const double EARTH_RADIUS = 6378.137;

        private static double rad(double d)
        {
            return d * Math.PI / 180.0;
        }

        private static double CalcDistance(double lng1, double lat1, double lng2, double lat2)
        {
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double a = radLat1 - radLat2;
            double b = rad(lng1) - rad(lng2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
             Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000 * 1000;
            return s;
        }

        protected static int cstMetersToKilometers = 1000000;
        protected static double metersPerLongitude = 0;
        protected static double metersPerLatitude = 0;

        //计算像素点面积
        public static double CalPixelArea(double solution)
        {
            metersPerLatitude = CalcDistance(119, 31, 118, 31);
            metersPerLongitude = CalcDistance(119, 31, 119, 32);
            return solution * metersPerLatitude * solution * metersPerLongitude / cstMetersToKilometers;
        }

        public static float[] GetCovertDegreeValue(string region)
        {
            string[] minmax = region.Replace("%", string.Empty).Split(new char[] { '~' });
            float min = float.Parse(minmax[0]) / 100f;
            if (min == 0f)
                min = -1;
            float max = float.Parse(minmax[1]) / 100f;
            return new float[] { min, max };
        }

        public static float[] GetCovertAreaValue(string region)
        {
            string[] minmax = region.Replace("%", string.Empty).Split(new char[] { '~' });
            float min = float.Parse(minmax[0]);
            float max = float.Parse(minmax[1]);
            return new float[] { min, max };
        }

        //获取AOITemplate配置
        public static Dictionary<string, string> GetAOITemplateList()
        {
            Dictionary<string, string> aoiTemplateList = new Dictionary<string, string>();
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "BAGConfig.xml"))
            {
                return null;
            }
            XDocument doc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "BAGConfig.xml");
            if (doc == null)
            {
                return null;
            }
            XElement root = doc.Element("DataSources");
            if (root == null)
            {
                return null;
            }
            foreach (XElement item in root.Elements("DataSource"))
            {
                string nameValue = item.Attribute("name").Value;
                string regionShapeFile = item.Attribute("regionShapeFile").Value;
                aoiTemplateList.Add(nameValue, regionShapeFile);
            }
            return aoiTemplateList;
        }
    }
}
