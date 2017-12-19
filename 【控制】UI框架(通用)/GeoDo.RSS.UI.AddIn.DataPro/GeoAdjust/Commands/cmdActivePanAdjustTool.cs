using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class cmdActivePanAdjustTool:Command
    {
        public cmdActivePanAdjustTool()
            : base()
        {
            _id = 30001;
            _text = _name = "激活平移校正";
        }

        public override void Execute()
        {
            Execute("GeoAdjustByPan");
        }

        public override void Execute(string argument)
        {
            _smartSession.UIFrameworkHelper.SetVisible(argument, true);
            //_smartSession.UIFrameworkHelper.Insert(argument, 5);
            _smartSession.UIFrameworkHelper.SetLockBesideX(argument, true);
        }
    }
}
