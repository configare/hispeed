using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using Telerik.WinControls.UI.Docking;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.Python
{
    [Export(typeof(ICommand))]
    public class CommandOpenScriptEditor : Command
    {
        public CommandOpenScriptEditor()
        {
            _id = 20003;
            _name = "PythonEditorWindow";
            _text = _toolTip = "脚本编辑窗口";
        }

        public override void Execute()
        {
            base.Execute();
        }

        //重载EXECUTE函数
        public override void Execute(string argument)
        {
            if (string.IsNullOrEmpty(argument))
            {
                Execute();
                return;
            }
            string[] parts = argument.Split('|');
            if (parts.Length != 3 && parts.Length != 2)
            {
                Execute();
                return;
            }
            CloseOpenedScripts();
            ISmartViewer wnd = new PythonEditorWindow();
            IPythonEditorWindow wnd2 = wnd as IPythonEditorWindow;
            wnd2.CreateTB(parts[2]);
            ToolWindow wnd3 = wnd as ToolWindow;
            wnd3.Tag = parts[2];
            wnd3.Text = parts[2];
            if (wnd == null)
                return;
            _smartSession.SmartWindowManager.DisplayWindow(wnd);
        }

        private void CloseOpenedScripts()
        {
            ISmartWindow[] wnds = _smartSession.SmartWindowManager.GetSmartWindows((wnd) => { return wnd.GetType().Equals(typeof(PythonEditorWindow)); });
            if (wnds == null || wnds.Length == 0)
                return;
            foreach (ISmartWindow wnd in wnds)
            {
                (wnd as DockWindow).Close();
                wnd.Free();
            }
        }
    }
}
