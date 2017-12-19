using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.GeoAdjust
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class cmdActivePanAdjustTool3 : Command
    {
        public cmdActivePanAdjustTool3()
            : base()
        {
            _id = 30201;
            _text = _name = "激活平移校正3";
        }

        public override void Execute()
        {
            Execute("GeoAdjustByPan3");
        }

        public override void Execute(string argument)
        {
            _smartSession.UIFrameworkHelper.SetVisible(argument, true);
            _smartSession.UIFrameworkHelper.SetLockBesideX(argument, true);
        }
    }
}
