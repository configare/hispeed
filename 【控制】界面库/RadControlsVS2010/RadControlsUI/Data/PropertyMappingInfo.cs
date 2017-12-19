using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI.Data
{
    class PropertyMappingInfo : IPropertyMappingInfo
    {
        protected List<PropertyMapping> mappings = new List<PropertyMapping>();

        public List<PropertyMapping> Mappings
        {
            get
            {
                return mappings;
            }
        }

        #region IPropertyMappingInfo Members

        public PropertyMapping FindByLogicalItemProperty(string logicalProperty)
        {
            return this.mappings.Find(delegate(PropertyMapping p)
            {
                return p.LogicalItemProperty == logicalProperty;
            });
        }

        public PropertyMapping FindByDataSourceProperty(string dataSourceProperty)
        {
            return this.mappings.Find(delegate(PropertyMapping p)
            {
                return p.DataSourceItemProperty == dataSourceProperty;
            });
        }

        #endregion

        #region IEnumerable<PropertyMapping> Members

        public IEnumerator<PropertyMapping> GetEnumerator()
        {
            return this.mappings.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.mappings.GetEnumerator();
        }

        #endregion
    }
}
