using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Core
{
    public class XmlContextVarAttribute:Attribute
    {
        private string _varName;

        public XmlContextVarAttribute(string varName)
        {
            _varName = varName;
        }

        public string VarName
        {
            get { return _varName; }
        }
    }
}
