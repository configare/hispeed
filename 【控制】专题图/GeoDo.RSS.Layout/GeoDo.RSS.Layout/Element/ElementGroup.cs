using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GeoDo.RSS.Layout
{
    public class ElementGroup : Element, IElementGroup
    {
        protected List<IElement> _elements = new List<IElement>();

        public ElementGroup()
            : base()
        {
        }

        [DisplayName("元素集合"),Category("数据")]
        public List<IElement> Elements
        {
            get { return _elements; }
        }

        public IElement FindParent(IElement element)
        {
            IElement parent = null;
            FindParent(element, this, ref parent);
            return parent;
        }

        private void FindParent(IElement element, IElementGroup root, ref IElement retElement)
        {
            if (root.Elements.Contains(element))
            {
                retElement = root;
                return;
            }
            foreach (IElement sub in root.Elements)
                if (sub is IElementGroup)
                    FindParent(element, sub as IElementGroup, ref retElement);
        }

        public IElement GetByName(string name)
        {
            foreach (IElement ele in _elements)
                if (ele.Name != null && ele.Name == name)
                    return ele;
            return null;
        }

        public void Adjust(IElement element, int index)
        {
            if (!_elements.Contains(element))
                return;
            int idx = _elements.IndexOf(element);
            _elements.Remove(element);
            if (index < 1)
                _elements.Insert(0, element);
            else if (index >= _elements.Count)
                _elements.Add(element);
            else
                _elements.Insert(idx, element);
        }

        public bool IsEmpty()
        {
            return _elements.Count == 0;
        }

        public override void Render(object sender, IDrawArgs drawArgs)
        {
            if (_elements == null || _elements.Count == 0)
                return;
            foreach (IElement ele in _elements)
                if (ele.Visible)
                    ele.Render(sender, drawArgs);
        }

        public override void Dispose()
        {
            if (_elements == null || _elements.Count == 0)
                return;
            foreach (IElement ele in _elements)
                ele.Dispose();
            _elements = null;
            base.Dispose();
        }
    }
}
