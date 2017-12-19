using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.UI.AddIn.Windows;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    [Export(typeof(ICommand))]
    public class CommandSearchCondition : GeoDo.RSS.Core.UI.Command
    {
        public CommandSearchCondition()
        {
            _id = 39003;
            _name = "CommandSearchCondition";
            _text = "栅格数据查询条件";
            _toolTip = "栅格数据查询条件";
        }

        public override void Execute()
        {
            Execute("");
        }

        public override void Execute(string argument)
        {
            ISmartWindow smartWindow = _smartSession.SmartWindowManager.GetSmartWindow((w) => { return w.GetType() == typeof(pageSearchCondition); });
            if (smartWindow == null)
            {
                smartWindow = new pageSearchCondition();
                (smartWindow as ToolWindowBase).Init(_smartSession);
            }
            (smartWindow as pageSearchCondition).SetArgument(argument);
            _smartSession.SmartWindowManager.DisplayWindow(smartWindow, new WindowPosition(System.Windows.Forms.DockStyle.Left, false));
        }
    }
}
