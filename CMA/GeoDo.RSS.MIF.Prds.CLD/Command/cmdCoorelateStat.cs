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
    public class cmdCorrelateStat:Command
    {
        public cmdCorrelateStat()
            : base()
        {
            _id = 35202;
            _text = _name = "相关系数计算";
        }

        public override void Execute()
        {

            ISmartWindow smartWindow = _smartSession.SmartWindowManager.GetSmartWindow((w) => { return w.GetType() == typeof(dockCorrelate); });
            if (smartWindow == null)
            {
                smartWindow = new dockCorrelate(_smartSession);
                _smartSession.SmartWindowManager.DisplayWindow(smartWindow);
            }
            else
                _smartSession.SmartWindowManager.DisplayWindow(smartWindow);
            return;
        }
    }
}
