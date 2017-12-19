using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public class XmlNameSelector : XmlSelectorBase
    {
        public XmlNameSelector()
        {
        }
        public XmlNameSelector(string elementName)
        {
            this.elementName = elementName;
        }

        private string elementName;

        public string ElementName
        {
            get { return elementName; }
            set { elementName = value; }
        }

        protected override IElementSelector CreateInstance()
        {
            return new NameSelector(this.ElementName);
        }

        public override string ToString()
        {
            return "NameSelector";
        }
        public override bool Equals(object obj)
        {
            if (obj is XmlNameSelector)
            {
                XmlNameSelector selector = obj as XmlNameSelector;
                if (selector.elementName == elementName)
                    return true;
            }

            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }   
}
