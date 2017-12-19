using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    public class LayerItemCanvasViewer : ILayerItem
    {
        private string _name = null;
        private string _text = null;
        private Image _image = null;
        private bool _isSelected = false;
        private bool _isAllowSelectable = false;
        private bool _isAllowVisible = false;
        private enumLayerTypes _layerType = enumLayerTypes.Unknow;
        private object _tag = null;

        public LayerItemCanvasViewer()
        {
        }

        public LayerItemCanvasViewer(enumLayerTypes layerType,Image image)
        {
            _layerType = layerType;
            _image = image;
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
                if (_tag is GeoDo.RSS.Core.DrawEngine.IRenderLayer)
                {
                    IRenderLayer lyr = _tag as IRenderLayer;
                    if (lyr != null)
                        return lyr.Visible;
                    return false;
                }
                else if (_tag is CodeCell.AgileMap.Core.ILayerDrawable)
                {
                    CodeCell.AgileMap.Core.ILayerDrawable lyr = _tag as CodeCell.AgileMap.Core.ILayerDrawable;
                    if (lyr != null)
                        return lyr.Visible;
                    return false;
                }
                else
                    return false;
            }
            set
            {
                if (_tag is GeoDo.RSS.Core.DrawEngine.IRenderLayer)
                {
                    IRenderLayer lyr = _tag as IRenderLayer;
                    if (lyr != null)
                        lyr.Visible = value;
                }
                else if (_tag is CodeCell.AgileMap.Core.ILayerDrawable)
                {
                    CodeCell.AgileMap.Core.ILayerDrawable lyr = _tag as CodeCell.AgileMap.Core.ILayerDrawable;
                    if (lyr != null)
                        lyr.Visible = value;
                }
                else if (_tag is GeoDo.RSS.Core.DrawEngine.IVectorHostLayer)
                {
                    //
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
