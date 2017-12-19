#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/28 10:11:38
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

namespace GeoDo.RSS.DF.FY1D
{
    /// <summary>
    /// 类名：FY1DBandNamesXmlParser
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/28 10:11:38
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    internal class FY1DBandNamesXmlParser : IDisposable
    {
        private string _cnfgFile = null;

        public FY1DBandNamesXmlParser(string cnfgfile)
        {
            _cnfgFile = cnfgfile;
        }

        public string[] GetBandNames(string satellite, string sensor)
        {
            string[] bandNames = null;
            List<string> bandNameList = new List<string>();
            XDocument doc = XDocument.Load(_cnfgFile);
            XElement root = doc.Root;
            var eleNames = root.Elements(XName.Get("BandnameRefTable"));
            foreach (var eleName in eleNames)
            {
                if (eleName.Attribute("satellite").Value.Equals(satellite) && eleName.Attribute("sensor").Value.Equals(sensor))
                {
                    foreach (XElement name in eleName.Elements())
                    {
                        bandNameList.Add(name.Attribute("name").Value);
                    }
                }
            }
            bandNames = bandNameList.ToArray();
            return bandNames;
        }

        public string[] GetBandNames(string satellite)
        {
            string[] bandNames = null;
            List<string> bandNameList = new List<string>();
            XDocument doc = XDocument.Load(_cnfgFile);
            XElement root = doc.Root;
            var eleNames = root.Elements(XName.Get("BandnameRefTable"));
            foreach (var eleName in eleNames)
            {
                if (eleName.Attribute("satellite").Value.Equals(satellite))
                {
                    foreach (XElement name in eleName.Elements())
                    {
                        bandNameList.Add(name.Attribute("name").Value);
                    }
                }
            }
            bandNames = bandNameList.ToArray();
            if (bandNameList.Count == 0)
            {
                bandNames = new string[] { "可见光", "近红外", "短波红外(3A或者3B)", "远红外", "远红外" };
            }
            return bandNames;
        }

        public void Dispose()
        {
        }
    }
}
