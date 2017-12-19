using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

namespace CodeCell.Bricks.UIs
{
    public class LayerItem
    {
        public enum enumLayerTypes
        { 
            OrbitData,
            Raster,
            BaseVector,
            RoutineObser,
            DigitalNumberMeasure,
            Unknow
        }

        internal static ImageList _imageList = null;
        protected List<LayerItem> _children = null;
        protected string _name = null;
        protected Image _image = null;
        protected bool _enabled = true;
        protected bool _isCollapsed = false;
        protected LayerItem _parent = null;
        internal Rectangle _bounds = new Rectangle();
        internal Rectangle _groupCollpaseBounds = new Rectangle();
        internal Rectangle _visibleBounds = new Rectangle();
        internal Rectangle _editBounds = new Rectangle();
        protected enumLayerTypes _layerType = enumLayerTypes.Unknow;
        protected object _tag = null;
        protected bool _isFixed = false;
        protected bool _editable = false;

        static LayerItem()
        {
        }

        public LayerItem() 
        {
        }

        public LayerItem(string name)
        {
            _name = name;
        }

        public LayerItem(string name, enumLayerTypes layerType)
            :this(name)
        {
            _layerType = layerType;
        }

        public LayerItem(string name, Image image)
            : this(name)
        {
            _image = image;
        }

        public LayerItem(string name, enumLayerTypes layerType,Image image)
            : this(name,layerType)
        {
            _image = image;
        }

        public bool IsFixed
        {
            get { return _isFixed; }
            set { _isFixed = value; }
        }

        public enumLayerTypes LayerType
        {
            get { return _layerType; }
            set { _layerType = value; }
        }

        public string Name
        {
            get { return _name != null ?_name:this.ToString(); }
            set { _name = value; }
        }

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        public Image Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public bool Enabled 
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public bool IsCollpased
        {
            get { return _isCollapsed; }
            set { _isCollapsed = value; }
        }

        public bool Editable
        {
            get { return _editable; }
            set { _editable = value; }
        }

        public LayerItem Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public void Add(LayerItem child)
        {
            if (child == null)
                return;
            if (_children == null)
                _children = new List<LayerItem>(1);
            if (!_children.Contains(child))
            {
                _children.Add(child);
                child.Parent = this;
            }
        }

        public void Add(LayerItem[] child)
        {
            if (child == null || child.Length ==0)
                return;
            foreach (LayerItem it in child)
                Add(it);
        }

        public void Clear()
        {
            if(_children !=null)
                _children.Clear();
        }

        public void Insert(LayerItem item, int index)
        {
            if (_children.Contains(item))
                _children.Remove(item);
            if (item.Parent != null)
                item.Parent.Remove(item);
            _children.Insert(index,item);
            item.Parent = this;
        }

        public int IndexOf(LayerItem item)
        {
            if (_children == null)
                return -1;
            return _children.IndexOf(item);
        }

        public bool IsContains(LayerItem item)
        {
            if (item == null || _children == null )
                return false;
            return _children.Contains(item);
        }

        public bool IsContainsAll(LayerItem item)
        {
            bool isContains = false;
            IsContainsAll(this, item, ref isContains);
            return isContains;
        }

        private void IsContainsAll(LayerItem parent, LayerItem item,ref bool isContains)
        {
            if (parent == null || item == null)
                isContains = false;
            if (parent.IsContains(item))
            {
                isContains = true;
                return;
            }
            if (parent.ChildCount > 0)
            {
                foreach (LayerItem subItem in parent.Children)
                {
                    IsContainsAll(subItem, item, ref isContains);
                }
            }
        }

        public void Remove(LayerItem child)
        {
            if (_children != null && _children.Contains(child))
            {
                _children.Remove(child);
                child.Parent = null;
            }
        }

        public int ChildCount
        {
            get { return _children != null ? _children.Count : 0; }
        }

        public IEnumerable<LayerItem> Children
        {
            get { return _children; }
        }

        public LayerItem FindChild(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            return _children.Find(delegate(LayerItem child)
                                          {
                                              return child != null && child.Name == name;
                                          });
        }
    }
}
