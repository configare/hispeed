using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CodeCell.Bricks.AppFramework
{
    public abstract class HookDefault : IHook
    {
        protected IApplication _application = null;
        protected ToolStripItem _control = null;
        protected ICommandHelper _commandHelper = null;
        protected Control _containerControl = null;

        public HookDefault(IApplication application, ToolStripItem control, ICommandHelper commandHelper)
        {
            _application = application;
            _control = control;
            _commandHelper = commandHelper;
        }

        #region IHook 成员

        public IApplication Application
        {
            get
            {
                return _application;
            }
        }

        public ToolStripItem Control
        {
            get { return _control; }
        }

        public ICommandHelper CommandHelper
        {
            get
            {
                return _commandHelper;
            }
        }

        public Control ContainerControl
        {
            get { return _containerControl; }
        }

        #endregion
    }
}
