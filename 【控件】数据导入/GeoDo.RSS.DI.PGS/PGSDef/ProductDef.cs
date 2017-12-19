using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DI.PGS
{
    public class ProductDef
    {
        private string _name;
        private string _smartIdentify;
        private string _fileIdentify;
        private SubProductDef[] _subProDefs;

        public ProductDef(string name, string smartIdentify, string fileIdentify)
        {
            _name = name;
            _smartIdentify = smartIdentify;
            _fileIdentify = fileIdentify;
        }

        public ProductDef(string name, string samrtIdentify, string fileIdentify, SubProductDef[] subProDefs)
            : this(name, samrtIdentify, fileIdentify)
        {
            _subProDefs = subProDefs;
        }

        public string Name
        {
            get { return _name; }
        }

        public string SmartIdentify
        {
            get { return _smartIdentify; }
        }

        public string FileIdentify
        {
            get { return _fileIdentify; }
        }

        public SubProductDef GetSubProductBySmartIdentfy(string subProductSmartIdentify)
        {
            if (_subProDefs == null)
                return null;
            foreach (SubProductDef item in _subProDefs)
            {
                if (item.martIdentify.ToUpper() == subProductSmartIdentify.ToUpper())
                    return item;
            }
            return null;
        }
    }
}
