using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.UI
{
    public class LayerItemGroup : ILayerItem, ILayerItemGroup
    {
        private string _name = null;
        private string _text = null;
        private Image _image = null;
        private enumLayerTypes _layerType = enumLayerTypes.Unknow;
        private bool _isSelected = false;
        private bool _isVisible = false;
        private bool _isAllowSelectable = false;
        private bool _isAllowVisible = false;
        private object _tag = null;
        private List<ILayerItem> _items = new List<ILayerItem>();

        public LayerItemGroup()
        {

        }

        public LayerItemGroup(string name, string text)
        {
            _name = name;
            _text = text;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Text
        {
            get { return _text; }
        }

        public Image Image
        {
            get { return _image; }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }

        public bool IsAllowSelectable
        {
            get { return _isAllowSelectable; }
        }

        public bool IsAllowVisible
        {
            get { return _isAllowVisible; }
        }

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        public List<ILayerItem> Items
        {
            get { return _items; }
        }

        public bool IsAllowVisiable
        {
            get { return _isAllowVisible; }
        }

        public ILayerItem FindParent(ILayerItem item)
        {
            foreach (ILayerItem layer in _items)
            {
                if (item.Equals(layer))
                    return this;
                if (layer is ILayerItemGroup)
                {
                    ILayerItem parent = (layer as ILayerItemGroup).FindParent(item);
                    if (parent != null)
                        return parent;
                }
            }
            return null;
        }

        public enumLayerTypes LayerType
        {
            get { return _layerType; }
        }
    }
}
