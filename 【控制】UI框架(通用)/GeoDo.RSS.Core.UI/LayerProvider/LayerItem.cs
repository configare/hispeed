using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.UI
{
    public class LayerItem : ILayerItem
    {
        private string _name = null;
        private string _text = null;
        private Image _image = null;
        private bool _isSelected = false;
        private bool _isVisible = false;
        private bool _isAllowSelectable = false;
        private bool _isAllowVisible = false;
        private enumLayerTypes _layerType = enumLayerTypes.Unknow;
        private object _tag = null;

        public LayerItem()
        {

        }

        public LayerItem(string name, string text)
        {
            _name = name;
            _text = text;
        }

        public LayerItem(string name, string text, Image image)
            : this(name, text)
        {
            _image = image;
        }

        public LayerItem(string name, string text, object tag)
            : this(name, text)
        {
            _tag = tag;
        }

        public LayerItem(string name, string text, Image image, object tag)
            : this(name, text, tag)
        {
            _image = image;
        }

        public LayerItem(string name, string text, Image image, object tag, bool isAllowSelectable, bool isAllowVisible)
            : this(name, text, image, tag)
        {
            _isAllowSelectable = isAllowSelectable;
            _isAllowVisible = IsAllowVisiable;
        }

        public string Name
        {
            get { return _name; }
            set { this._name = value; }
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
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
            }
        }

        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
            }
        }

        public bool IsAllowSelectable
        {
            get { return _isAllowSelectable; }
        }

        public bool IsAllowVisiable
        {
            get { return _isAllowVisible; }
        }

        public enumLayerTypes LayerType
        {
            get { return _layerType; }
            set { _layerType = value; }
        }

        public object Tag
        {
            get { return _tag; }
        }
    }
}
