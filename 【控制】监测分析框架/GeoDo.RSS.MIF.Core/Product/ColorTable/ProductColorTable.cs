using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class ProductColorTable
    {
        private string _identify = null;
        private string _subIdentify = null;
        private ProductColor[] _proColors = null;
        private string _description = null;
        private string _colorTableName;
        private string _labelText;

        public ProductColorTable(string identify, string subIdentify, string colorTableName, string labelText)
        {
            _identify = identify;
            _subIdentify = subIdentify;
            _colorTableName = colorTableName;
            _labelText = labelText;
        }

        public string ColorTableName
        {
            get { return _colorTableName; }
        }

        public string Identify
        {
            get { return _identify; }
        }

        public string SubIdentify
        {
            get { return _subIdentify; }
        }

        public ProductColor[] ProductColors
        {
            get { return _proColors; }
            set { _proColors = value; }
        }

        public string LabelText
        {
            get { return _labelText; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public ProductColor GetColor(float v)
        {
            foreach (ProductColor pc in _proColors)
                if (pc.IsContains(v))
                    return pc;
            return null;
        }
    }
}
