using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CodeCell.AgileMap.Core
{
    /*
     * <Field name="CREATETIME" type="DATETIME" description="创建时间"/>
     */
    public class SystemField
    {
        private string _name = null;
        private string _fieldType = null;
        private string _description = null;
        internal const string cstDataTimeField = "DATETIME";

        public SystemField(XElement xelement)
        {
            _name = xelement.Attribute("name").Value;
            _fieldType = xelement.Attribute("type").Value;
            _description = xelement.Attribute("description").Value;
        }

        public string Name
        {
            get { return _name; }
        }

        public string FieldType
        {
            get { return _fieldType; }
        }

        public string Description
        {
            get { return _description; }
        }
    }
}
