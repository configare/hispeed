using System;
using System.Collections.Generic;
using System.IO;

namespace GeoDo.RSS.MIF.Core
{
    public class CatalogItem : ICatalogItem
    {
        protected string _fileName;
        protected CatalogItemInfo _info = new CatalogItemInfo();

        public CatalogItem(SDatinfo datinfo)
        {
            _fileName = datinfo.SourceFileName;
            var info = new CatalogItemInfo();
            info.Properties.Add("ProductIdentify", datinfo.ProductIdentify);
            info.Properties.Add("SubProductIdentify", datinfo.SubProductIdentify);
            info.Properties.Add("FileName", datinfo.FileName);
            info.Properties.Add("FileDir", datinfo.FileDir);
            info.Properties.Add("CatalogDef", datinfo.CatalogDef);
            info.Properties.Add("Satellite", datinfo.Satellite);
            info.Properties.Add("Sensor", datinfo.Sensor);
            info.Properties.Add("OrbitDateTime", datinfo.OrbitDateTime);
            info.Properties.Add("OrbitTimeGroup", datinfo.OrbitTimeGroup);
            info.Properties.Add("CatalogItemCN", datinfo.CatalogItemCN);
            info.Properties.Add("Region", datinfo.Region);
            info.Properties.Add("ExtInfos", datinfo.ExtInfos);
            info.Properties.Add("CycFlagCN", datinfo.CycFlagCN);
            info.Properties.Add("OrbitTimes", datinfo.OrbitTimes);
            _info = info;
        }

        public CatalogItem(string fname, SubProductCatalogDef catalogDef, params object[] args)
        {
            _fileName = fname;
            string infoFileName = GetInfoFileName(fname);
            if (!File.Exists(infoFileName))
                TryCreateInfoFromMainFile(fname, infoFileName, catalogDef);
            else
            {
                _info = new CatalogItemInfo(infoFileName);
                if (!_info.Properties.ContainsKey("CatalogDef") || string.IsNullOrEmpty(_info.Properties["CatalogDef"].ToString()) ||
                    !_info.Properties.ContainsKey("CycFlagCN"))
                    TryCreateInfoFromMainFile(fname, infoFileName, catalogDef);
            }
        }

        public CatalogItem(string fname, CatalogItemInfo info)
        {
            _fileName = fname;
            string infoFileName = GetInfoFileName(fname);
            info.SaveTo(infoFileName);
            _info = info;
        }

        /*
        <Attribute text="产品类别" identify="SubProductIdentify" format="" visible="true"/>
        <Attribute text="卫星" identify="Satellite" format="" visible="true"/>
        <Attribute text="传感器" identify="Sensor" format="" visible="true"/>
        <Attribute text="轨道时间" identify="OrbitDateTime" format="yyyy-MM-dd HH:mm:ss" visible="true"/>
        <Attribute text="原始文件" identify="SourceFile" format="" visible="true"/>
        <Attribute text="监测区域" identify="RegionIdentify" format="" visible="true"/>
        <Attribute text="描述" identify="Description" format="" visible="true"/>
        <Attribute text="轨道时间分组" identify="OrbitTimeGroup" format="yyyy-MM-dd" visible="true"/>
        <Attribute text="文件名" identify="FileName" format="" visible="true"/>
        <Attribute text="路径" identify="FileDir" format="" visible="true"/>
        <Attribute text="类别中文" identify="CatalogItemCN" format="" visible="false"/>
        <Attribute text="数据集定义" identify="CatalogDef" format="" visible="false"/>
        <Attribute text="扩展信息" identify="ExtInfos" format="" visible="false"/>
        <Attribute text="区域" identify="Region" format="" visible="false"/>
        <Attribute text="轨道时间段" identify="OrbitTimes" format="yyyy-MM-dd HH:mm:ss ; yyyy-MM-dd HH:mm:ss" visible="false">
         */
        private void TryCreateInfoFromMainFile(string fname, string infoFileName, SubProductCatalogDef catalogDef)
        {
            string fileName = Path.GetFileNameWithoutExtension(fname);
            string[] parts = fileName.Split('_');
            if (parts.Length < 2)
                return;
            RasterIdentify rst = new RasterIdentify(fname);
            CatalogItemInfo info = new CatalogItemInfo();
            info.Properties.Add("ProductIdentify", parts[0]);
            info.Properties.Add("SubProductIdentify", parts[1]);
            info.Properties.Add("FileName", Path.GetFileName(fname));
            info.Properties.Add("FileDir", Path.GetDirectoryName(fname));
            info.Properties.Add("CatalogDef", catalogDef == null ? "" : catalogDef.ClassString);
            info.Properties.Add("Satellite", rst.Satellite);
            info.Properties.Add("Sensor", rst.Sensor);
            info.Properties.Add("OrbitDateTime", GetDateFormart(rst.OrbitDateTime, catalogDef, "OrbitDateTime"));
            info.Properties.Add("OrbitTimeGroup", GetDateFormart(rst.OrbitDateTime, catalogDef, "OrbitTimeGroup"));
            info.Properties.Add("CatalogItemCN", GetCatalogCN(info, MifEnvironment.CatalogItemCNDic));
            RasterIdentify ri = new RasterIdentify(fname);
            info.Properties.Add("Region", GetRegionInfo(ri, fname));
            info.Properties.Add("ExtInfos", GetExtInfo(ri, fname));
            info.Properties.Add("CycFlagCN", string.IsNullOrEmpty(ri.CYCFlag) ? "\\" : GetCycFlagCN(info, ri.CYCFlag, MifEnvironment.CatalogItemCNDic));
            info.Properties.Add("OrbitTimes", GetOrbitTimes(ri));
            //.....
            info.SaveTo(infoFileName);
            _info = info;
        }

        private object GetOrbitTimes(RasterIdentify rid)
        {
            string defFormat = "yyyy-MM-dd HH:mm:ss";
            if (rid.ObritTiems == null || rid.ObritTiems.Length <= 1)
                return "\\";
            else
            {
                string result = rid.ObritTiems[0].ToString(defFormat) + ";" + rid.ObritTiems[1].ToString(defFormat);
                return result;
            }
        }

        private object GetExtInfo(RasterIdentify ri, string filename)
        {
            string result = "\\";
            string extInfo = string.IsNullOrEmpty(ri.ExtInfos) ? "" : ri.ExtInfos;
            if (string.IsNullOrEmpty(extInfo))
                return result;
            else if (extInfo.EndsWith("_"))
                extInfo = extInfo.Substring(0, extInfo.Length - 1);
            return result.Replace("\\", "") + extInfo;
        }

        private object GetRegionInfo(RasterIdentify ri, string filename)
        {
            string result = "\\";
            string regionInfo = string.IsNullOrEmpty(ri.RegionIdentify) ? "" : ri.RegionIdentify;
            if (string.IsNullOrEmpty(regionInfo))
                return result;
            else if (regionInfo.EndsWith("_"))
                regionInfo = regionInfo.Substring(0, regionInfo.Length - 1);
            return result.Replace("\\", "") + regionInfo;
        }

        private string GetCatalogCN(CatalogItemInfo info, Dictionary<string, Dictionary<string, string>> catalogCNDic)
        {
            if (!info.Properties.ContainsKey("ProductIdentify") || info.Properties["ProductIdentify"] == null)
                return string.Empty;
            string result = string.Empty;
            if (catalogCNDic == null || catalogCNDic.Count == 0)
                result = info.Properties["SubProductIdentify"].ToString();
            else if (catalogCNDic.ContainsKey(info.Properties["ProductIdentify"].ToString().ToUpper()))
            {
                if (info.Properties["SubProductIdentify"] == null || !info.Properties.ContainsKey("SubProductIdentify"))
                    return string.Empty;
                Dictionary<string, string> temp = catalogCNDic[info.Properties["ProductIdentify"].ToString().ToUpper()];
                if (temp.ContainsKey(info.Properties["SubProductIdentify"].ToString().ToUpper()))
                    result = temp[info.Properties["SubProductIdentify"].ToString().ToUpper()];
                else
                    result = info.Properties["SubProductIdentify"].ToString();
            }
            return result;
        }

        private string GetCycFlagCN(CatalogItemInfo info, string cycFlag, Dictionary<string, Dictionary<string, string>> catalogCNDic)
        {
            if (!info.Properties.ContainsKey("ProductIdentify") || info.Properties["ProductIdentify"] == null)
                return cycFlag;
            string result = cycFlag;
            if (catalogCNDic.ContainsKey(info.Properties["ProductIdentify"].ToString().ToUpper()))
            {
                Dictionary<string, string> temp = catalogCNDic[info.Properties["ProductIdentify"].ToString().ToUpper()];
                if (temp.ContainsKey(cycFlag.ToUpper()))
                    result = temp[cycFlag.ToUpper()];
            }
            return result;
        }

        private string GetDateFormart(DateTime dt, SubProductCatalogDef catalogDef, string identify)
        {
            string defFormat = "yyyy-MM-dd HH:mm:ss";
            if (catalogDef == null)
                return dt.ToString(defFormat);
            CatalogAttriteDef cad = catalogDef.GetCatalogAttrDefByIdentify(identify);
            if (cad == null || string.IsNullOrEmpty(cad.Format))
                return dt.ToString(defFormat);
            return dt.ToString(cad.Format);
        }

        public string FileName
        {
            get { return _fileName; }
        }

        public CatalogItemInfo Info
        {
            get { return _info; }
            set { _info = value; }
        }

        public static string GetInfoFileName(string fname)
        {
            if (string.IsNullOrEmpty(fname))
                return null;
            return Path.Combine(Path.GetDirectoryName(fname), Path.GetFileNameWithoutExtension(fname) + ".INFO");
        }
    }
}
