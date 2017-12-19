using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.AddIn.CMA
{
    [Export(typeof(GeoDo.RSS.Core.UI.ICommand))]
    public class CommProductThemeGraphRegionSetting : Command
    {
        public CommProductThemeGraphRegionSetting()
        {
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override void Execute()
        {
            base.Execute();
        }
    }
}
