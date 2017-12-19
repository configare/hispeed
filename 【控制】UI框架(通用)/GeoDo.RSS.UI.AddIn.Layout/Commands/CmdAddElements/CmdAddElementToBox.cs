using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    public delegate void AddElementCustemHandler(object sender,IElement[] elements);

    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdAddElementToBox : CmdAddElementBase
    {
        private AddElementCustemHandler _elementAdded = null;
            
        public CmdAddElementToBox()
            : base()
        {
            _id = 6037;
            _name = "AddElementToBox";
            _text = _toolTip = "添加元素到整饰工具箱";
        }

        public override void Execute()
        {
            ILayoutViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            if (viewer.LayoutHost == null)
                return;
            ILayoutRuntime runTime = viewer.LayoutHost.LayoutRuntime;
            if (runTime == null)
                return;
            IElement[] elements = runTime.Selection;
            if (elements == null || elements.Length == 0)
                return;
            TryRefreshLayerManager();
        }
    }
}
