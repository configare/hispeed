using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace CodeCell.Bricks.AppFramework
{
    public class BaseControlItem:BaseItem,IControlItem
    {
        protected Size _size = new Size(100, 20);
        protected object _control = null;
        protected bool _enabled = true;
        protected Image _image = null;
        protected bool _checked = false;
        protected bool _beginGroup = false;
        protected IHook _hook = null;
        protected IItem[] _items = null;


        public BaseControlItem()
        { 
        }

        public BaseControlItem(bool begingroup)
        {
            _beginGroup = begingroup;
        }

        #region IControlItem 成员

        public virtual System.Drawing.Size Size
        {
            get 
            {
                return _size;
            }
        }

        public virtual object Control
        {
            get 
            {
                return _control;
            }
        }

        #endregion

        #region ICommand 成员

        public virtual System.Drawing.Image Image
        {
            get 
            {
                return _image;
            }
        }

        public virtual bool BeginGroup
        {
            get 
            {
                return _beginGroup;
            }
        }

        public virtual bool Enabled
        {
            get 
            {
                return true;
            }
        }

        public virtual bool Checked
        {
            get
            {
                return _checked;
            }
        }

        public virtual void Click()
        {
            //
        }

        public virtual void Init(IHook hook)
        {
            _hook = hook;
        }

        public virtual IHook Hook
        {
            get 
            {
                return _hook;
            }
        }

        public virtual IItem[] Items 
        {
            get 
            {
                return _items;
            }
        }

        #endregion
    }
}
