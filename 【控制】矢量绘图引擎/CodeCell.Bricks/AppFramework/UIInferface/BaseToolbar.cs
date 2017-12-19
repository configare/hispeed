using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CodeCell.Bricks.AppFramework
{
    [Serializable]
    public class BaseToolbar:BaseItem,IToolbar
    {
        protected ICommand _currentTool = null;
        protected IItem[] _items = null;
        protected DockStyle _dockStyle = DockStyle.Top;
        protected IHook _hook = null;

        public void SetHook(IHook hook)
        {
            _hook = hook;
        }

        #region IToolbar 成员

        public virtual IItem[] Items
        {
            get 
            {
                return _items;
            }
        }

        public ICommand CurrentTool 
        {
            get
            {
                return _currentTool;
            }
            set
            {
                if (value != null)
                {
                    if (!(value is ICommand))
                    {
                        throw new Exception("IToolbar.CurrentTool属性只兼容ICommand类型的工具！");
                    }
                }
                _currentTool = value;
            }
        }

        public ICommand FindCommand(Guid id)
        {
            if (_items == null)
                return null;
            foreach (IItem it in _items)
            {
                if (!(it is ICommand))
                    continue;
                if (it.Id == id)
                    return it as ICommand;
            }
            return null;
        }

        public ICommand FindCommand(string name) 
        {
            if (name == null || _items == null)
                return null;
            foreach (IItem it in _items)
            {
                if (!(it is ICommand))
                    continue;
                if (it.Name == name)
                    return it as ICommand;
            }
            return null;
        }

        public ICommand FindCommand(IItem item) 
        {
            if (item == null)
                return null;
            return FindCommand(item.Id);
        }

        public virtual DockStyle DockStyle
        {
            get { return _dockStyle; }
        }

        #endregion
    }
}
