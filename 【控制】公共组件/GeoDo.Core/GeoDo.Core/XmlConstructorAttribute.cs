using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Core
{
    public class XmlConstructorAttribute : Attribute
    {
        private string[] _varNameOfCtorArgs;

        public XmlConstructorAttribute(string[] varNameOfCtorArgs)
        {
            _varNameOfCtorArgs = varNameOfCtorArgs;
        }

        public string[] VarNameOfCtorArgs
        {
            get { return _varNameOfCtorArgs != null && _varNameOfCtorArgs.Length > 0 ? _varNameOfCtorArgs : null; }
        }
    }
}
