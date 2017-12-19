using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.UI.AddIn.DataRef
{
    public class DataRefNameValues
    {
        private static SortedDictionary<string, string> _nameValues;

        static DataRefNameValues()
        {
            _nameValues = new SortedDictionary<string, string>();
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\行政区划\面\中国边界.shp", "0.国境");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\行政区划\面\中国行政区.shp", "1.中国行政区");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\行政区划\面\省级行政区域_面.shp", "2.省级行政区");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\行政区划\面\市级行政区域_面.shp", "3.市级行政区");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\行政区划\面\县级行政区域_面.shp", "4.县级行政区");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\行政区划\面\国家.shp", "5.国家");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\行政区划\面\大洲.shp", "6.大洲");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\行政区划\面\中国周边海陆模板.shp", "");

            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\行政区划\线\中国边界_线.shp", "0.国境线");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\行政区划\线\中国行政区_线.shp", "1.中国行政区");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\行政区划\线\省级行政区域_线.shp", "2.省界线");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\行政区划\线\市级行政区域_线.shp", "3.市界线");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\行政区划\线\县级行政区域_线.shp", "4.县界线");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\行政区划\线\国家_线.shp", "5.国界线");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\行政区划\线\大洲_线.shp", "6.洲界线");

            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\行政区划\点\中国首府_PT.shp", "0.首府");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\行政区划\点\中国省会.shp", "1.省会");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\行政区划\点\中国地级市_点.shp", "2.市政府");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\行政区划\点\中国县市_点.shp", "3.县政府");

            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\河流湖泊\全球\全球湖泊_面.shp", "1.全球湖泊(面)");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\河流湖泊\全球\全球湖泊(线).shp", "2.全球湖泊(线)");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\河流湖泊\全球\全球河流_线.shp", "3.全球河流");

            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\河流湖泊\中国\中国湖泊.shp", "1.中国湖泊(面)");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\河流湖泊\中国\中国湖泊(线).shp", "2.中国湖泊(线)");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\河流湖泊\中国\中国湖泊_标注.shp", "3.中国湖泊(标注)");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\河流湖泊\中国\中国_河流 湖泊_线.shp", "4.中国_河流 湖泊");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\河流湖泊\中国\中国河流.shp", "5.中国河流");
            _nameValues.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\河流湖泊\中国\中国主要河流_线.shp", "6.中国主要河流");
        }

        public static SortedDictionary<string, string> NameValues
        {
            get
            {
                return _nameValues;
            }
        }

        public static string TryFindValue(string name)
        {
            if (!_nameValues.ContainsKey(name))
                return null;
            return _nameValues[name];
        }
    }
}
