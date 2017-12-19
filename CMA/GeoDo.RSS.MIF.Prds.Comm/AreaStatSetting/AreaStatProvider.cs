#region Version Info
/*========================================================================
* 功能概述：
* 面积统计分析设置项提供者。
* 创建者：DongW     时间：2013/8/14 13:42:07
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
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    /// <summary>
    /// 类名：AreaStatProvider
    /// 属性描述：
    /// 创建者：DongW        创建日期：2013/8/14 
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class AreaStatProvider
    {
        public const string CONFIG_FILE_NAME = @"SystemData\ProductArgs\AreaStatSetting.xml";

        public AreaStatProvider()
        {

        }

        public static AreaStatItem[] GetAreaStatItems()
        {
            XElement root = GetXmlRoot();
            if (root == null)
                return null;
            IEnumerable<XElement> items=root.Elements("StatItem");
            if (items == null || items.Count() == 0)
                return null;
            List<AreaStatItem> lstAreaStat = new List<AreaStatItem>();
            foreach (XElement item in items)
            {
                AreaStatItem statItem = ParseAreaStat(item);
                lstAreaStat.Add(statItem);
            }
            return lstAreaStat.ToArray();
        }

        public string GetDefaultStatFile(string itemName)
        {
            switch (itemName)
            {
                case "省界分类":
                    return @"数据引用\基础矢量\行政区划\面\省级行政区域_面.shp";
                case "市县分级":
                    return @"\SystemData\RasterTemplate\China_XjRaster.dat";
                case "土地分类":
                    return @"\数据引用\基础矢量\矢量模版\土地利用类型_合并.shp";
                default:
                    return null;
            }
        }

        private static AreaStatItem ParseAreaStat(XElement statElement)
        {
            if (statElement == null)
                return null;
            string name = statElement.Attribute("name").Value;
            if (!string.IsNullOrEmpty(name))
            {
                AreaStatItem statItem = new AreaStatItem(name);
                string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, statElement.Attribute("filename").Value);
                statItem.FileName = fileName;
                statItem.MenuName = statElement.Attribute("menuname").Value;
                if (statElement.Attribute("infofilename") != null)
                {
                    statItem.InfoFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,statElement.Attribute("infofilename").Value);
                }
                string fileType = statElement.Attribute("filetype").Value.ToLower();
                switch (fileType)
                {
                    case "raster": 
                        {
                            statItem.StatFileType = enumStatTemplateType.Raster;
                            break;
                        }
                    case "vector":
                        {
                            statItem.StatFileType = enumStatTemplateType.Vector;
                            break;
                        }
                }
                if (statElement.Attribute("statfield") != null)
                {
                    statItem.StatField = statElement.Attribute("statfield").Value;
                }
                if (statElement.Attribute("columnnames")!=null)
                {
                    string names = statElement.Attribute("columnnames").Value;
                    string[] nameArray = names.Split(new char[]{',','，'});
                    if (nameArray != null && nameArray.Length > 0)
                        statItem.ColumnNames = nameArray;
                }
                return statItem;
            }
            return null;
        }

        public static void SaveToXML(AreaStatItem[] statItems)
        {
            if (statItems == null || statItems.Length < 1)
                return;
            XElement root = GetXmlRoot();
            if (root == null)
                return;
            IEnumerable<XElement> items = root.Elements("StatItem");
            for (int i = 0; i < statItems.Length; i++)
            {
                if (statItems[i] == null || string.IsNullOrEmpty(statItems[i].Name))
                    continue;
                //监测是否已经存在该统计分析项
                //存在仅修改
                bool isSave = false;
                foreach (XElement item in items)
                {
                    if (statItems[i].Name == item.Attribute("name").Value.Trim())
                    {
                        item.SetAttributeValue("menuname", statItems[i].MenuName);
                        item.SetAttributeValue("filename", statItems[i].FileName);
                        item.SetAttributeValue("infofilename", statItems[i].InfoFileName);
                        item.SetAttributeValue("statfield", statItems[i].StatField);
                        string columnNames=GetColumnNamesString(statItems[i].ColumnNames);
                        item.SetAttributeValue("columnnames", columnNames);
                        isSave = true;
                        break;
                    }
                }
                if (!isSave)
                {
                    //新添加节点
                    string columnNames = GetColumnNamesString(statItems[i].ColumnNames);
                    root.Add(new XElement("StatItem", new XAttribute("name", statItems[i].Name),
                    new XAttribute("menuname", statItems[i].MenuName),
                    new XAttribute("filename", statItems[i].FileName),
                    new XAttribute("filetype", statItems[i].StatFileType),
                    new XAttribute("infofilename", statItems[i].InfoFileName),
                    new XAttribute("statfield", statItems[i].StatField),
                    new XAttribute("columnnames", columnNames)));
                }
            }
            string statFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CONFIG_FILE_NAME);
            root.Save(statFileName);
        }

        private static XElement GetXmlRoot()
        {
            string statFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CONFIG_FILE_NAME);
            if (!File.Exists(statFileName))
                return null;
            XElement root = XElement.Load(statFileName);
            return root;
        }

        public static string GetAreaStatItemMenuName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            AreaStatItem[] statItems = GetAreaStatItems();
            if (statItems == null || statItems.Length == 0)
                return null;
            for (int i = 0; i < statItems.Length; i++)
            {
                if (statItems[i].Name == name)
                    return statItems[i].MenuName;
            }
            return null;
        }

        //矢量文件返回形式为:   vector:fileName:statField
        //栅格文件返回形式为：  raster:fileName:infoFileName
        public static string GetAreaStatItemFileName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            AreaStatItem[] statItems = GetAreaStatItems();
            if (statItems == null || statItems.Length == 0)
                return null;
            for (int i = 0; i < statItems.Length; i++)
            {
                if (statItems[i].Name == name)
                {
                    string fName = Path.GetFileName(statItems[i].FileName);
                    if (statItems[i].StatFileType == enumStatTemplateType.Vector)
                    {
                        return "vector:" + fName + ":" + statItems[i].StatField;
                    }
                    else
                    {
                        string infoFile=Path.GetFileName(statItems[i].InfoFileName);
                        return "raster:" + fName + ":" + infoFile;
                    }
                }
            }
            return null;
        }

        private static string GetColumnNamesString(string[] nameArray)
        {
            if (nameArray == null || nameArray.Length < 1)
                return null;
            string namesStr = null;
            foreach (string item in nameArray)
            {
                namesStr = namesStr + item + ",";
            }
            return namesStr.Remove(namesStr.Length - 1);
        }

        public static string[] GetColumnNameArrayByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            AreaStatItem[] statItems = GetAreaStatItems();
            if (statItems == null || statItems.Length == 0)
                return null;
            for (int i = 0; i < statItems.Length; i++)
            {
                if (statItems[i].Name == name)
                {
                    return statItems[i].ColumnNames;
                }
            }
            return null;
        }
    }
}
