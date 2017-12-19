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
    public class cmdSvdStat:Command
    {
        public cmdSvdStat()
            : base()
        {
            _id = 35203;
            _text = _name = "SVD分解";
        }

        public override void Execute()
        {

            ISmartWindow smartWindow = _smartSession.SmartWindowManager.GetSmartWindow((w) => { return w.GetType() == typeof(dockSvd); });
            if (smartWindow == null)
            {
                smartWindow = new dockSvd(_smartSession);
                _smartSession.SmartWindowManager.DisplayWindow(smartWindow);
            }
            else
                _smartSession.SmartWindowManager.DisplayWindow(smartWindow);
            return;
        }
    }
}
