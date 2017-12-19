using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace CodeCell.Bricks.AppFramework
{
    public class BaseTool:BaseItem,ITool,IShortcutFilter
    {
        protected bool _beginGroup = false;
        protected bool _checked = false;
        protected bool _enabled = true;
        protected Image _image = null;
        protected IHook _hook = null;
        protected Cursor _cursor = Cursors.Default;

        public BaseTool()
        { 
        }

        public BaseTool(bool beginGroup)
        {
            _beginGroup = beginGroup;
        }

        #region ITool 成员

        public virtual Cursor Cursor
        {
            get 
            {
                return _cursor;
            }
        }

        public virtual bool MouseDown(object sender, MouseEventArgs e)
        {
            return true;
        }

        public virtual bool MouseMove(object sender, MouseEventArgs e)
        {
            return true;
        }

        public virtual bool MouseUp(object sender, MouseEventArgs e)
        {
            return true;
        }

        public virtual bool MouseWheel(object sender, MouseEventArgs e)
        {
            return true;
        }

        public virtual bool MouseDoubleClick(object sender, MouseEventArgs e)
        {
            return true;
        }

        public virtual void Render(object drawArg)
        { 
        }

        #endregion

        #region ICommand 成员

        public virtual Image Image
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
                return _enabled;
            }
        }

        /// <summary>
        /// 该属性无效
        /// </summary>
        public bool Checked
        {
            get 
            {
                return _checked;
            }
        }

        public virtual void Click()
        {
            
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

        #endregion

        #region IShortcutFilter 成员

        public virtual bool AcceptShortcut(PreviewKeyDownEventArgs previewKeyDownEventArgs)
        {
            return false;
        }

        #endregion
    }
}
