using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace CodeCell.Bricks.AppFramework
{
    [Serializable]
    public class BaseCommand:BaseItem,ICommand,IShortcutFilter
    {
        protected bool _beginGroup = false;
        protected bool _checked = false;
        protected bool _enabled = true;
        protected Image _image = null;
        protected IHook _hook = null;
  
        public BaseCommand()
        { 
        }

        public BaseCommand(bool beginGroup)
        {
            _beginGroup = beginGroup;
        }

        #region ICommand 成员

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
                return _enabled;
            }
        }

        public virtual bool Checked
        {
            get 
            {
                return _checked;
            }
            set { _checked = value; }
        }

        public virtual Image Image
        {
            get 
            {
                return _image;
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

        public IHook Hook 
        {
            get 
            {
                return _hook;
            }
        }

        #endregion


        #region IShortcutFilter 成员

        public virtual bool AcceptShortcut(PreviewKeyDownEventArgs previewKeyDownEventArgs)
        {
            return false;
        }

        #endregion
    }
}
