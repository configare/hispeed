#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：罗战克     时间：2013-09-05 15:55:47
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.RSS.DF.GDAL.HDF5Universal
{
    /// <summary>
    /// 类名：FY3L2L3HdfProductDef
    /// 属性描述：
    /// 创建者：罗战克   创建日期：2013-09-05 15:55:47
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class FY3L2L3HdfProductDef
    {
        private string _identify;
        private string _name;
        private string _matchFilename;
        private string[] _matchDatasets;
        private Dictionary<string, string> _matchAttributes;
        private string[] _defaultDataSets;

        public FY3L2L3HdfProductDef()
        { }

        public string Identify
        {
            get { return _identify; }
            set { _identify = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string MatchFilename
        {
            get { return _matchFilename; }
            set { _matchFilename = value; }
        }

        public string[] MatchDatasets
        {
            get { return _matchDatasets; }
            set { _matchDatasets = value; }
        }

        public Dictionary<string, string> MatchAttributes
        {
            get { return _matchAttributes; }
            set { _matchAttributes = value; }
        }

        public string[] DefaultDataSets
        {
            get { return _defaultDataSets; }
            set { _defaultDataSets = value; }
        }

        internal static FY3L2L3HdfProductDef Parse(XElement ele)
        {
            string identify = ele.Attribute("identify").Value;
            if (string.IsNullOrWhiteSpace(identify))
                return null;
            string defaultDatasets = ele.Element("DefaultDatasets").Value;
            if (string.IsNullOrWhiteSpace(defaultDatasets))
                return null;
            string name = ele.Attribute("name").Value;
            XElement matchXml = ele.Element("MatchCondition");
            if (matchXml == null)
                return null;
            string matchFilename = matchXml.Attribute("filename").Value;
            string matchDatasets = matchXml.Element("DataSets").Value;
            XElement attributesXml = matchXml.Element("Attributes");
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (attributesXml != null)
            {
                IEnumerable<XElement> eles = attributesXml.Elements("Attribute");
                foreach (XElement aele in eles)
                {
                    string aname = aele.Attribute("name").Value;
                    string avalue = aele.Attribute("value").Value;
                    if (string.IsNullOrWhiteSpace(aname))
                        dic.Add(aname, avalue);
                }
            }
            if (string.IsNullOrWhiteSpace(matchFilename) && string.IsNullOrWhiteSpace(matchDatasets) && dic.Count == 0)
                return null;
            FY3L2L3HdfProductDef hdf = new FY3L2L3HdfProductDef();
            hdf.Identify = identify;
            hdf.Name = name;
            hdf.MatchFilename = matchFilename;
            hdf.MatchDatasets = ParseDatasets(matchDatasets);
            hdf.MatchAttributes = dic;
            hdf.DefaultDataSets = ParseDatasets(defaultDatasets);
            return hdf;
        }

        private static string[] ParseDatasets(string dstasets)
        {
            if (string.IsNullOrWhiteSpace(dstasets))
                return null;
            string[] dss = dstasets.Split(',');
            return dss;
        }
    }
}
