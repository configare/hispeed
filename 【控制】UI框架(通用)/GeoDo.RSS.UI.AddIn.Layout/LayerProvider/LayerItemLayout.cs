using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    public class LayerItemLayout : ILayerItem
    {
        private string _name = null;
        private string _text = null;
        private Image _image = null;
        private bool _isSelected = false;
        private bool _isAllowSelectable = false;
        private bool _isAllowVisible = false;
        private enumLayerTypes _layerType = enumLayerTypes.Unknow;
        private object _tag = null;

        public LayerItemLayout()
        {

        }

        public LayerItemLayout(enumLayerTypes layerType)
        {
            _layerType = layerType;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string Text
        {
            get { return _text; }
        }

        public Image Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public enumLayerTypes LayerType
        {
            get { return _layerType; }
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
                if (_tag is GeoDo.RSS.Layout.IElement)
                {
                    IElement lyr = _tag as IElement;
                    if (lyr != null)
                        return lyr.Visible;
                }
                return false;
            }
            set
            {
                if (_tag is GeoDo.RSS.Layout.IElement)
                {
                    IElement lyr = _tag as IElement;
                    if (lyr != null)
                        lyr.Visible = value;
                }
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

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
    }
}
