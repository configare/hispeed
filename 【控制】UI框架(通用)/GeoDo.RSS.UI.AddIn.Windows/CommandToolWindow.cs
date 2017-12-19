using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using Telerik.WinControls.UI.Docking;
using Telerik.WinControls.UI;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.Windows
{
    public class CommandToolWindow : Command
    {
        public CommandToolWindow()
        {
        }

        public override void Execute()
        {
            ISmartToolWindow wnd = _smartSession.SmartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(_id);
            if (wnd != null)
                _smartSession.SmartWindowManager.DisplayWindow(wnd,new WindowPosition(System.Windows.Forms.DockStyle.Left,true));
            DisplayAfter(wnd);
        }

        public override void Execute(string argument)/*Left:true:9000*/
        {
            if (string.IsNullOrEmpty(argument))
            {
                Execute();
                return;
            }
            string[] parts = argument.Split(':');
            if (parts.Length != 3 && parts.Length != 2)
            {
                Execute();
                return;
            }
            ISmartToolWindow wnd = _smartSession.SmartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(_id);
            if (wnd == null)
                return;
            DockStyle pos = DockStyle.Left;
            bool isfloat = false;
            switch (parts[0].ToUpper())
            {
                case "LEFT":
                    pos = DockStyle.Left;
                    break;
                case "RIGHT":
                    pos = DockStyle.Right;
                    break;
                case "TOP":
                    pos = DockStyle.Top;
                    break;
                case "BOTTOM":
                    pos = DockStyle.Bottom;
                    break;
                case "FILL":
                    pos = DockStyle.Fill;
                    break;
                case "FLOAT":
                    isfloat = true;
                    break;
            }
            bool isSplited = true;
            bool.TryParse(parts[1], out isSplited);
            int parentId = 0;
            if (parts.Length == 3)
                int.TryParse(parts[2], out parentId);
            if (!isfloat)
                _smartSession.SmartWindowManager.DisplayWindow(wnd, new WindowPosition(pos, isSplited));
            else
                _smartSession.SmartWindowManager.DisplayWindow(wnd);
            DisplayAfter(wnd);
        }

        protected  virtual void DisplayAfter(ISmartToolWindow wnd)
        {
        }
    }
}
