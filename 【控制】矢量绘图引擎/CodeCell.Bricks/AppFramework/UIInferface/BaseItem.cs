using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CodeCell.Bricks.AppFramework
{
    [Serializable]
    public abstract class BaseItem:IItem
    {
        protected string _name = string.Empty;
        protected string _text = string.Empty;
        protected string _tooltips = string.Empty;
        protected Guid _id = Guid.NewGuid();
        protected bool _visible = true;
        protected bool _allowDock = true;
        protected ToolStripItemDisplayStyle _displayStyle = ToolStripItemDisplayStyle.Image;
        protected IItem[] _children = null;
        
        public BaseItem()
        { 
        }

        #region IItem ≥…‘±

        public bool AllowDock
        {
            get { return _allowDock; }
            set { _allowDock = value; }
        }

        public IItem[] Children
        {
            get { return _children; }
            set { _children = value; }
        }

        public virtual string ToolTips
        {
            get 
            {
                return _tooltips;
            }
        }

        public virtual string Name
        {
            get
            {
                return _name;
            }
            set { _name = value; }
        }

        public virtual string Text 
        {
            get 
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }

        public virtual Guid Id
        {
            get
            {
                return _id;
            }
        }

        public virtual bool Visible
        {
            get 
            {
                return _visible;
            }
            set
            {
                _visible = value;
            }
        }

        public virtual ToolStripItemDisplayStyle DisplayStyle
        {
            get 
            {
                return _displayStyle;
            }
        }

        #endregion
    }
}
