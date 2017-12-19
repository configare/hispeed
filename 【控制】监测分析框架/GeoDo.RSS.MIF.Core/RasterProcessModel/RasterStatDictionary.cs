using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.MIF.Core
{
    public class RasterStatDictionary
    {
        private static SortedDictionary<string, string> _xjDic;
        private static SortedDictionary<string, string> _landTypeDic;

        public static SortedDictionary<string, string> GetDic(string templateName)
        {
            if (templateName == "China_XjRaster")
            {
                return XjDic;
            }
            else if (templateName == "China_LandRaster")
            {
                return LanTypeDic;
            }
            return null;
        }

        public static SortedDictionary<string, string> XjDic
        {
            get
            {
                if (_xjDic == null)
                    _xjDic = ParseXjDic();
                return _xjDic;
            }
        }

        public static SortedDictionary<string, string> LanTypeDic
        {
            get
            {
                if (_landTypeDic == null)
                    _landTypeDic = ParseLandTypeDic();
                return _landTypeDic;
            }
        }

        private static SortedDictionary<string, string> ParseXjDic()
        {
            string codeFile = AppDomain.CurrentDomain.BaseDirectory + "SystemData\\RasterTemplate\\" + "China_XjRaster_Code.txt";
            SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
            string[] lines = File.ReadAllLines(codeFile, Encoding.Default);
            string[] parts = null;
            foreach (string lne in lines)
            {
                parts = lne.Split('\t');
                dic.Add(parts[0], parts[1]);
            }
            return dic;
        }

        private static SortedDictionary<string, string> ParseLandTypeDic()
        {
            string codeFile = AppDomain.CurrentDomain.BaseDirectory + "SystemData\\RasterTemplate\\" + "China_LandRaster_Code.txt";
            SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
            string[] lines = File.ReadAllLines(codeFile, Encoding.Default);
            string[] parts = null;
            foreach (string lne in lines)
            {
                parts = lne.Split('=');
                dic.Add(parts[0], parts[1]);
            }
            return dic;
        }
    }
}
