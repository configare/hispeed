using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Telerik.WinControls
{
    //[Serializable]
    //[XmlInclude(typeof(XmlCondition))]
    //[XmlInclude(typeof(XmlGeneralSelector))]
    //[XmlInclude(typeof(XmlTypeSelector))]
    //[XmlInclude(typeof(XmlNameSelector))]
    //[XmlInclude(typeof(XmlClassSelector))]
    //[XmlInclude(typeof(XmlRoutedEventCondition))]
    //[XmlInclude(typeof(XmlSimpleCondition))]
    //[XmlInclude(typeof(XmlSimpleCondition))]
    public abstract class XmlElementSelector
    {
        public IElementSelector Deserialize()
        {
            IElementSelector selector = this.CreateInstance();
            this.DeserializeProperties(selector);

            return selector;
        }

        protected abstract void DeserializeProperties(IElementSelector selector);

        protected abstract IElementSelector CreateInstance();

        public override string ToString()
        {
            return "ElementSelector";
        }
    }
}
