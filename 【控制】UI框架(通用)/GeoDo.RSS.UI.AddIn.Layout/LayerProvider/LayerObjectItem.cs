using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Layout.DataFrm;
using GeoDo.RSS.Core.UI;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    public class LayerObjectItem:ILayerItem
    {
        protected ILayerObject _layerObject;
        private string _name;
        private bool _isSelected;
        private enumLayerTypes _layerType = enumLayerTypes.Unknow;

        public LayerObjectItem(ILayerObject layerObject)
        {
            _layerObject = layerObject;
            if (_layerObject != null)
            {
                if (layerObject.Tag is CodeCell.AgileMap.Core.FeatureLayer)
                    _layerType = enumLayerTypes.BaseVector;
                _name = (_layerObject.Text ?? string.Empty);
            }        
        }

        public string Name
        {
            get 
            { 
                return _layerObject != null ? (_layerObject.Text ?? string.Empty) : string.Empty; 
            }
            set
            {
                _name = value;
            }
        }

        public string Text
        {
            get { return Name; }
        }

        public Image Image
        {
            get { return null; }
        }

        public enumLayerTypes LayerType
        {
            get { return _layerType; }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }

        public bool IsVisible
        {
            get { return _layerObject.Visible; }
            set { _layerObject.Visible = value; }
        }

        public bool IsAllowSelectable
        {
            get { return false; }
        }

        public bool IsAllowVisiable
        {
            get { return true; }
        }

        public object Tag
        {
            get { return _layerObject.Tag; }
        }
    }
}
