using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Layout.Elements;
using GeoDo.RSS.Layout.GDIPlus;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdAddTitle : CmdAddElementBase
    {
        public CmdAddTitle()
            : base()
        {
            _id = 6008;
            _name = "AddText";
            _text = _toolTip = "添加标题";
        }

        public override void Execute()
        {
            Execute("双击修改专题标图");
        }

        public override void Execute(string argument)
        {
            ILayoutViewer layout = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (layout != null)
            {
                TextElement textEle = new TextElement(argument);
                layout.LayoutHost.LayoutRuntime.Layout.Elements.Add(textEle);          
                layout.LayoutHost.Render();
                textEle.IsSelected = true;
                layout.LayoutHost.Aligment(enumElementAligment.LeftRightMid);
                layout.LayoutHost.Render();
            }
            TryRefreshLayerManager();
        }
    }
}
