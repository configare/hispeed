using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace GeoDo.RSS.DI.MVG
{
    public class SubProductDef
    {
        private string _name;
        private string _smartIdentify;
        private string _fileIdentify;
        private TabelDef[] _tableDefs;
        private int _tableDefCount = 0;

        public SubProductDef(string name, string smartIdentify, string fileIdentify)
        {
            _name = name;
            _smartIdentify = smartIdentify;
            _fileIdentify = fileIdentify;
        }

        public SubProductDef(string name, string smartIdentify, string fileIdentify, TabelDef[] tableDef)
            : this(name, smartIdentify, fileIdentify)
        {
            _tableDefs = tableDef;
            if (_tableDefs != null)
                _tableDefCount = tableDef.Length;
        }

        /// <summary>
        /// 指定类型数据对照字典表
        /// </summary>
        /// <typeparam name="T1">文件数据值</typeparam>
        /// <typeparam name="T2">Smart数据值</typeparam>
        /// <returns></returns>
        public Dictionary<T1, T2> GetTableDic<T1, T2>()
        {
            if (_tableDefs == null || _tableDefs.Length == 0)
                return null;
            Dictionary<T1, T2> resultDic = new Dictionary<T1, T2>();
            TypeConverter t1Converter = TypeDescriptor.GetConverter(typeof(T1));
            TypeConverter t2Converter = TypeDescriptor.GetConverter(typeof(T2));
            foreach (TabelDef item in _tableDefs)
            {
                resultDic.Add((T1)t1Converter.ConvertFromString(item.FileVale.ToString()),
                              (T2)t2Converter.ConvertFromString(item.SmartValue.ToString()));
            }
            return resultDic.Count == 0 ? null : resultDic;
        }

        public string Name
        {
            get { return _name; }
        }

        public string martIdentify
        {
            get { return _smartIdentify; }
        }

        public string FileIdentify
        {
            get { return _fileIdentify; }
        }

        public int TableDefCount
        {
            get { return _tableDefCount; }
        }
    }
}
