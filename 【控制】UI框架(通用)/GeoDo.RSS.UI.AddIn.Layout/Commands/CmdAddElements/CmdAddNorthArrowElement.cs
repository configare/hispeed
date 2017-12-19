using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout.GDIPlus;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdAddNorthArrowElement : CmdAddElementBase
    {
        private Bitmap _bitmap = null;

        public CmdAddNorthArrowElement()
            : base()
        {
            _id = 6012;
            _name = "AddNorthArrowElement";
            _text = _toolTip = "添加指北针";
        }

        public override void Execute()
        {
            ILayoutViewer layout = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (layout == null)
                return;
            _bitmap = _smartSession.UIFrameworkHelper.GetImage("system:指北针2.bmp") as Bitmap;
            PictureElement pic = new PictureElement(_bitmap);
            layout.LayoutHost.LayoutRuntime.Layout.Elements.Add(pic);
            layout.LayoutHost.Render();
            TryRefreshLayerManager();
        }
    }
}
