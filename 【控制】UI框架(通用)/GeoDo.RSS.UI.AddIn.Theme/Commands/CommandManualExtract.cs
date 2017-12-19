using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.RasterDrawing;
using System.Drawing;
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.UI.AddIn.CMA
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandManualExtract : CommandProductBase
    {
        public CommandManualExtract()
            : base()
        {
            _id = 6602;
            _name = "ManualExtract";
            _text = _toolTip = "交互判识";
        }

        public override void Execute()
        {
            (_smartSession.MonitoringSession as IMonitoringSession).DoManualExtract();
        }

        public override void Execute(string argument)
        {
            Execute();
        }
    }
}
