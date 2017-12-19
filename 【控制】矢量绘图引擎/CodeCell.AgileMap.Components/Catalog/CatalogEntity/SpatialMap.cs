using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CodeCell.AgileMap.Components
{
    /*
     *  <Field name="ID" type="VARCHAR(64)" description="GUID"/>
        <Field name="NAME" type="VARCHAR(64)" description="地图名称"/>
        <Field name="CONFIGXML" type="TEXT(1024000)" description="配置内容,1MB大小,存贮MCD文件的内容"/>
        <Field name="REFDATASOURCES" type="TEXT(102400)" description="地图引用的数据源列表，$分隔符分开,10KB。是MCD文件中的DataSource的快捷方式。"/>
        <Field name="DESCRIPTION" type="VARCHAR(256)" description="地图描述"/>
     */
    public class SpatialMap:CatalogEntityBase
    {
        private string _configXml = null;
        private string _refdatasources = null;

        public SpatialMap()
            : base()
        { 
        }

        [AttToFieldMap("ConfigXml", typeof(string))]
        public string ConfigXml
        {
            get { return _configXml; }
            set 
            {
                if (value != null && value != _configXml)
                {
                    _configXml = value;
                    TryExtractRefDataSources();
                }
            }
        }

        [AttToFieldMap("RefDataSources", typeof(string))]
        public string RefDataSources
        {
            get
            {
                return _refdatasources;
            }
        }

        public override bool IsEmpty()
        {
            return base.IsEmpty() && _configXml == null && _refdatasources == null;
        }

        private void TryExtractRefDataSources()
        { 
        }

        protected override ICatalogEntityClass GetCatalogEntityClass()
        {
            return new CatalogEntityClassMap(_connString);
        }
    }
}
