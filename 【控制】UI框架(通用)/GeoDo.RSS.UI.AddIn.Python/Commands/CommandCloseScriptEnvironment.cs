using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;
using Telerik.WinControls.UI.Docking;

namespace GeoDo.RSS.UI.AddIn.Python
{
    [Export(typeof(ICommand))]
    class CommandCloseScriptEnvironment:Command
    {
        CommandCloseScriptEnvironment()
            : base()
        {
            _id = 20007;
            _text = _toolTip = "关闭脚本工作区";
        }

        public override void Execute()
        {
            if ((_smartSession.SmartWindowManager.ActiveViewer is IPythonEditorWindow))
            {
                DialogResult dr = MessageBox.Show("是否确认保存当前编辑文件?", "请确认", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dr == DialogResult.Yes)
                {
                    _smartSession.CommandEnvironment.Get(20004).Execute("ALL");
                }
            }
            CloseSpecialWindows(typeof(PythonMonitorWindow));
            CloseSpecialWindows(typeof(PythonManagerWindow));
            CloseSpecialWindows(typeof(PythonEditorWindow));
        }

        private void CloseSpecialWindows(Type type)
        {
            ISmartWindow[] wnds = _smartSession.SmartWindowManager.GetSmartWindows((wnd) => { return wnd.GetType().Equals(type); });
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
