using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdLayoutScale :Command
    {
        public CmdLayoutScale()
            : base()
        {
            _id = 6001;
            _name = "LayoutScale";
            _text = _toolTip = "专题图缩放";
        }

        public override void Execute(string argument)
        {
            ILayoutViewer host = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (host != null)
            {
                argument = argument.Split('%')[0];
                host.LayoutHost.LayoutRuntime.Scale = float.Parse(argument)/100f;
                host.LayoutHost.Render();
            }
        }
    }
}
