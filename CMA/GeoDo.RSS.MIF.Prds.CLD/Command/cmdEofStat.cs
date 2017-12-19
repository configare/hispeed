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
    public class cmdEofStat:Command
    {
        public cmdEofStat()
            : base()
        {
            _id = 35205;
            _text = _name = "EOF分解";
        }

        public override void Execute()
        {
            ISmartWindow smartWindow = _smartSession.SmartWindowManager.GetSmartWindow((w) => { return w.GetType() == typeof(dockEof); });
            if (smartWindow == null)
            {
                smartWindow = new dockEof(_smartSession);
                _smartSession.SmartWindowManager.DisplayWindow(smartWindow);
            }
            else
                _smartSession.SmartWindowManager.DisplayWindow(smartWindow);
            return;
        }
    }
}
