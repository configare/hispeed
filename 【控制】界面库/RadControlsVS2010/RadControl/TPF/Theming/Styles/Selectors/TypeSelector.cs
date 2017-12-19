using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents a type selector. Type selectors are used to apply the same
    /// customization to all elements of the same type. Behavior is very similar to that 
    /// of the CSS type selectors.
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(XmlTypeSelector))]
    public class TypeSelector : HierarchicalSelector
    {
        private class NoSuchElement : VisualElement
        {
        }

        private Type elementType = typeof(NoSuchElement);

        /// <summary>Initializes a new instance of the TypeSelector class.</summary>
        public TypeSelector()
        {}

        protected internal override bool CanUseCache
        {
            get { return true; }
        }

        protected override int GetKey()
        {
            if (this.elementType == null)
            {
                return 0;
            }

            return this.elementType.GetHashCode();
        }

        /// <summary>
        /// Initializes a new instance of the TypeSelector class using the type that will be
        /// affected.
        /// </summary>
        public TypeSelector(Type elementType)
        {
            this.elementType = elementType;
        }

        /// <summary>Gets or sets the element type that will be affected by the Type selector.</summary>
        public Type ElementType
        {
            get
            {
                return elementType;
            }
            set
            {
                elementType = value;
            }
        }

        protected override bool CanSelectOverride(RadElement element)
        {
            return element != null && this.elementType == element.GetThemeEffectiveType();
        }

        public override bool Equals(IElementSelector elementSelector)
        {
            TypeSelector selector = elementSelector as TypeSelector;
            return selector != null && selector.elementType == this.elementType;
        }

		protected override LinkedList<RadElement> FindElements(System.Collections.IDictionary ChildrenHierarchyByElement)
		{
            return (LinkedList<RadElement>)ChildrenHierarchyByElement[this.elementType];
		}

        protected override XmlElementSelector CreateSerializableInstance()
        {
            return new XmlTypeSelector(XmlTheme.SerializeType(this.ElementType));
        }

        public override string ToString()
        {
            if (elementType == null)
            {
                return "ThemeEffectiveType == NotSpecified";
            }

            return "ThemeEffectiveType == " + this.elementType;
        }
    }
}
