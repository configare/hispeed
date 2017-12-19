using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using FastColoredTextBoxNS;
using System.IO;
using GeoDo.PythonEngine;

namespace GeoDo.RSS.UI.AddIn.Python
{
    [Export(typeof(ICommand))]
    class CommandExecuteScript:Command
    {
        public CommandExecuteScript()
            : base()
        {
            _id = 20008;
            _text = _toolTip = "执行脚本";
        }

        public override void Execute()
        {
            FastColoredTextBox ftb = getCurrentEditorView();
            if ((ftb != null))
            {
                string s = File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\SystemData\\Scripts\\install_list.py");
                (_smartSession.PythonEngine as IPythonEngine).Run(s + "   \r" + ftb.Text);
                ISmartWindow[] wnds = _smartSession.SmartWindowManager.GetSmartWindows((wnd) => { return wnd.GetType().Equals(typeof(PythonMonitorWindow)); });
                if (wnds == null || wnds.Length == 0)
                    return;
                ISmartViewer[] views = wnds.Cast<ISmartViewer>().ToArray();
                if (views.Count() > 0)
                {
                    (views[0] as IPythonMonitorWindow).NotityFetchLog();
                }
            }
        }

        FastColoredTextBox getCurrentEditorView()
        {
            IPythonEditorWindow ipw = _smartSession.SmartWindowManager.ActiveViewer as IPythonEditorWindow;
            if (ipw != null)
            {
                return ipw.getTB();
            }
            return null;
        }
    }
}
