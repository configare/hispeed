using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout.GDIPlus;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdAddCustomText : CmdAddElementBase
    {
        public CmdAddCustomText()
            : base()
        {
            _id = 6024;
            _name = "AddCustomText";
            _text = _toolTip = "添加自定义文本";
        }

        public override void Execute()
        {
            ILayoutViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            TextElement textEle = new TextElement();
            viewer.LayoutHost.LayoutRuntime.Layout.Elements.Add(textEle);
            viewer.LayoutHost.Render();
            TryRefreshLayerManager();
        }
    }
}
