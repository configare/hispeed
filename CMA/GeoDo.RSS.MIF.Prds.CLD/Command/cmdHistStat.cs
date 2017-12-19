using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class cmdHistStat : Command
    {
        public cmdHistStat()
            : base()
        {
            _id = 35201;
            _text = _name = "直方图统计";
        }

        public override void Execute()
        {

            ISmartWindow smartWindow = _smartSession.SmartWindowManager.GetSmartWindow((w) => { return w.GetType() == typeof(dockHist); });
            if (smartWindow == null)
            {
                smartWindow = new dockHist(_smartSession);
                _smartSession.SmartWindowManager.DisplayWindow(smartWindow);
            }
            else
                _smartSession.SmartWindowManager.DisplayWindow(smartWindow);
            return;

       }
    }
}
