using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents a class selector that can be serialized and deserialized. 
    /// Telerik class selectors are very similar to CSS class selectors.
    /// </summary>
    //[Serializable]
    public class XmlClassSelector : XmlSelectorBase
    {   
        /// <summary>
        /// Initializes a new instance of the XmlClassSelector class. 
        /// </summary>
        public XmlClassSelector()
        {
        }
        /// <summary>
        /// Initializes a new instance of the XmlClassSelector class using an element 
        /// given as a string.
        /// </summary>
        /// <param name="elementClass"></param>
        public XmlClassSelector(string elementClass)
        {
            this.elementClass = elementClass;
        }

        private string elementClass;
        /// <summary>
        /// Gets or sets a string value indicating the class.
        /// </summary>
        public string ElementClass
        {
            get { return elementClass; }
            set { elementClass = value; }
        }

        protected override IElementSelector CreateInstance()
        {
            return new ClassSelector(this.ElementClass);
        }
        /// <summary>
        /// Retrieves the string representation of the class.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "ClassSelector";
        }
        /// <summary>
        /// Retrieves a boolean value indicating whether <em>this</em> and the argument are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is XmlClassSelector)
            {
                XmlClassSelector selector = obj as XmlClassSelector;
                if (selector.elementClass == elementClass)
                    return true;
            }

            return false;
        }
        /// <summary>
        /// Serves as a hash function for the XmlClassSelector type.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
