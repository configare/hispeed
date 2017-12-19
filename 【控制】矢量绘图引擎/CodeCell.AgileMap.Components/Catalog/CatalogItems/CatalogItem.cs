using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.AgileMap.Components
{
    public abstract class CatalogItem:ICatalogItem
    {
        protected string _name = string.Empty;
        protected string _description = string.Empty;
        protected Image _image = null;
        protected ICatalogItem _parent = null;
        protected List<ICatalogItem> _children = null;
        protected object _tag = null;
        protected List<ContextOprItem> _oprItems = new List<ContextOprItem>();
        protected ICatalogItemRefresh _refresh = null;
        protected bool _isLoaded = false;

        public CatalogItem()
        {
            Init();
        }

        public CatalogItem(string name, object tag)
            :this()
        {
            _name = name;
            _tag = tag;
        }

        public CatalogItem(string name, object tag, string description)
            :this(name,tag)
        {
            _description = description;
        }

        public CatalogItem(string name, object tag, string description, Image image)
            : this(name, tag, description)
        {
            _image = image;
        }

        internal void SetPartent(ICatalogItem parent)
        {
            _parent = parent;
        }

        internal void SetCatalogItemRefresh(ICatalogItemRefresh refresh)
        {
            _refresh = refresh;
        }

        protected virtual void Init()
        {
        }

        #region ICatalogEntity Members

        public string Name
        {
            get { return _name != null ? _name : string.Empty; }
        }

        public string Description
        {
            get { return _description != null ? _description : string.Empty; }
        }

        public Image Image
        {
            get { return _image; }
        }

        public int ChildCount
        {
            get { return _children != null ? _children.Count : 0; }
        }

        public ICatalogItem[] Children
        {
            get { return _children != null ? _children.ToArray() : null; }
        }

        public ICatalogItem Parent
        {
            get { return _parent; }
        }

        public object Tag
        {
            get { return _tag; }
        }

        public void AddChild(ICatalogItem catalogEntity)
        {
            if (catalogEntity == null)
                return;
            if (catalogEntity.Parent != null)
                catalogEntity.Parent.Remove(catalogEntity);
            if (_children == null)
                _children = new List<ICatalogItem>(1);
            _children.Add(catalogEntity);
            (catalogEntity as CatalogItem).SetPartent(this);
        }

        public void Remove(ICatalogItem catalogEntity)
        {
            if (catalogEntity == null)
                return;
            (catalogEntity as CatalogItem).SetPartent(null);
            if (_children == null)
                return;
            _children.Remove(catalogEntity);
        }

        public virtual ContextOprItem[] ContextOprItems
        {
            get 
            {
                return _oprItems != null ? _oprItems.ToArray() : null;
            }
        }

        public virtual enumContextKeys DefaultKey
        {
            get { return enumContextKeys.None; }
        }

        public virtual void Click(enumContextKeys key)
        {
            if (key == enumContextKeys.Refresh)
            {
                _isLoaded = false;
                if(_children != null)
                    _children.Clear();
                LoadChildren();
            }
        }

        public virtual void Refresh()
        {
            _refresh.Refresh(this);
        }

        #endregion

        internal virtual void LoadChildren()
        {
            _isLoaded = true;
        }
    }
}
