using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.ReportService
{
    [Export(typeof(ICommand))]
    public class CommandToDb : GeoDo.RSS.Core.UI.Command
    {
        public CommandToDb()
        {
            _id = 400002;
            _name = "ReportPage";
            _text = "报告素材提交";
            _toolTip = "报告素材提交";
        }

        public override void Execute()
        {
            Execute("");
        }

        public override void Execute(string argument)
        {
            ISmartWindow smartWindow = _smartSession.SmartWindowManager.GetSmartWindow((w) => { return w.GetType() == typeof(ReportPage); });
            if (smartWindow == null)
                smartWindow = new ReportPage();
            _smartSession.SmartWindowManager.DisplayWindow(smartWindow);
            (smartWindow as ReportPage).UpdateSession(_smartSession);
            _smartSession.SmartWindowManager.DisplayWindow(smartWindow);
        }
    }
}
