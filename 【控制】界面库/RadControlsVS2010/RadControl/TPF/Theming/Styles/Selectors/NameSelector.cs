using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents a name selector. Name selectors are used to apply customization to the
    /// element having the specified name. This behavior is very similar to that of CSS id
    /// selectors.
    /// </summary>
    [Serializable]
    public class NameSelector : HierarchicalSelector
    {
        private string elementName;

        /// <summary>
        /// Gets or sets the element's name. Customization is applied only to the element
        /// having this name.
        /// </summary>
        public string ElementName
        {
            get { return elementName; }
            set { elementName = value; }
        }

        protected internal override bool CanUseCache
        {
            get { return false; }
        }

        protected override int GetKey()
        {
            if (string.IsNullOrEmpty(this.elementName))
            {
                return 0;
            }
             
            return this.elementName.GetHashCode();
        }

        public static int GetSelectorKey(string name)
        {
            return name.GetHashCode();
        }

        /// <summary>Initializes a new instance of the NameSelector class.</summary>
        public NameSelector()
        {
        }

        /// <summary>
        /// Initializes a new instance of the NameSelector class using the name of the
        /// element.
        /// </summary>
        public NameSelector(string elementName)
        {
            this.elementName = elementName;
        }

        protected override bool CanSelectOverride(RadElement element)
        {
            return element != null && string.CompareOrdinal(element.Name, this.elementName) == 0;
        }

        public override bool Equals(IElementSelector elementSelector)
        {
            NameSelector selector = elementSelector as NameSelector;
            return selector != null && selector.elementName == this.elementName;
        }

        protected override XmlElementSelector CreateSerializableInstance()
        {
            return new XmlNameSelector(this.elementName);
        }
        public override string ToString()
        {
            if (elementName == null)
            {
                return "Name == NotSpecified";
            }

            return "Name == " + this.elementName;
        }
    }     
}
