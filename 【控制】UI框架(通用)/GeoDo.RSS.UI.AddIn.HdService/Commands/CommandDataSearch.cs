using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    [Export(typeof(ICommand))]
    public class CommandDataSearch : GeoDo.RSS.Core.UI.Command
    {
        public CommandDataSearch()
        {
            _id = 39000;
            _name = "queryPage";
            _text = "首页";
            _toolTip = "首页";
        }

        public override void Execute()
        {
            Execute("");
        }

        public override void Execute(string argument)
        {
            ISmartWindow smartWindow = _smartSession.SmartWindowManager.GetSmartWindow((w) => { return w.GetType() == typeof(queryPage); });
            if (smartWindow == null)
            {
                smartWindow = new queryPage(_smartSession);
            }
            _smartSession.SmartWindowManager.DisplayWindow(smartWindow);
            (smartWindow as queryPage).SetQuery(argument);
            _smartSession.SmartWindowManager.DisplayWindow(smartWindow);
        }
    }
}
