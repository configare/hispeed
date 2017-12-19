using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents a class selector. Class selectors are used to apply the same
    /// customization to all elements that belong to the same class. This 
    /// behavior is very similar to that of the CSS class selectors.
    /// </summary>
    public class ClassSelector : HierarchicalSelector
    {
        private string elementClass;

        /// <summary>Gets or sets a value indicating the class name.</summary>
        public string ElementClass
        {
            get { return elementClass; }
            set { elementClass = value; }
        }

        /// <summary>Initializes a new instance of the class selector class.</summary>
        public ClassSelector()
        {
        }

        protected internal override bool CanUseCache
        {
            get { return true; }
        }
        
        protected override int GetKey()
        {
            if (string.IsNullOrEmpty(this.elementClass))
            {
                return 0;
            }

            return GetSelectorKey(this.elementClass).GetHashCode();
        }

        public static int GetSelectorKey(string elementClass)
        {
            return ("Class=" + elementClass).GetHashCode();
        }

        public override bool Equals(IElementSelector elementSelector)
        {
            ClassSelector selector = elementSelector as ClassSelector;
            return selector != null && selector.elementClass == this.elementClass;
        }

        /// <summary>
        /// Initializes a new instance of the class selector class using string for the class
        /// name.
        /// </summary>
        public ClassSelector(string elementClass)
        {
            this.elementClass = elementClass;
        }

        protected override bool CanSelectOverride(RadElement element)
        {
            return element != null && string.Compare(element.Class, this.elementClass, true) == 0;
        }

        protected override XmlElementSelector CreateSerializableInstance()
        {
            return new XmlClassSelector(this.elementClass);
        }

		public override LinkedList<RadElement> GetSelectedElements(RadElement element)
		{
			return base.GetSelectedElements(element);
		}

		protected override LinkedList<RadElement> FindElements(System.Collections.IDictionary ChildrenHierarchyByElement)
		{
			return (LinkedList<RadElement>)ChildrenHierarchyByElement[this.elementClass];
		}

        public override string ToString()
        {
            if (elementClass == null)
            {
                return "Class == NotSpecified";
            }

            return "Class == " + this.elementClass;
        }
    }
}
