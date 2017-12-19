using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using Telerik.WinControls.UI.Docking;

namespace GeoDo.RSS.UI.AddIn.Windows
{
    [Export(typeof(ICommand))]
    public class CommandOpenContextMessage : CommandToolWindow
    {
        public CommandOpenContextMessage()
        {
            _id = 9006;
            _name = "ContextMessageWindow";
            _text = _toolTip = "上下文消息窗口";
        }

        public override void Execute()
        {
            Execute(null);
        }

        public override void Execute(string argument)
        {
            ISmartToolWindow wnd = _smartSession.SmartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(_id);
            if (wnd != null)
                _smartSession.SmartWindowManager.DisplayWindow(wnd, new WindowPosition(System.Windows.Forms.DockStyle.Bottom, false));
            if (!string.IsNullOrEmpty(argument))
            {
                bool isHidden = false;
                if(bool.TryParse(argument, out isHidden)&&isHidden)
                  (wnd as ToolWindow).DockState = DockState.AutoHide;
            }
        }
    }
}
