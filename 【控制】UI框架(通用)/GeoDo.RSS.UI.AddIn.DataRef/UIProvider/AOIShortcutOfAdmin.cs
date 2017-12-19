using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.UI.AddIn.DataRef
{
    /// <summary>
    /// 行政区划AOI快捷方式
    /// </summary>
    internal class AOIShortcutOfAdmin
    {
        //矢量文件名
        private string _shpFileName = "省级行政区域_面.shp";
        //显示字段名
        private string _displayFieldName = "NAME";

        public AOIShortcutOfAdmin()
        {
            //以后修改为配置文件
            string dir = AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量";
            string[] fnames = Directory.GetFiles(dir, _shpFileName,SearchOption.AllDirectories);
            if (fnames == null || fnames.Length == 0)
            {
                _shpFileName = null;
                return;
            }
            _shpFileName = fnames[0];
        }

        public string[] GetDisplayFieldNames()
        {
            if (_displayFieldName == null || _shpFileName == null || !File.Exists(_shpFileName))
                return null;
            using (IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(_shpFileName) as IVectorFeatureDataReader)
            {
                Feature[] fets = dr.FetchFeatures();
                if (fets == null || fets.Length == 0)
                    return null;
                if (fets[0].FieldNames == null || !fets[0].FieldNames.Contains(_displayFieldName))
                    return null;
                List<string> names = new List<string>();
                foreach (Feature f in fets)
                    names.Add(f.GetFieldValue(_displayFieldName));
                return names.Count > 0 ? names.ToArray() : null;
            }
        }

        public Feature GetFeatureByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            using (IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(_shpFileName) as IVectorFeatureDataReader)
            {
                return dr.FetchFeature((fet) => { return fet.GetFieldValue(_displayFieldName) == name; });
            }
        }
    }
}
