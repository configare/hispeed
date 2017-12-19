using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout;
using System.IO;
using GeoDo.RSS.Layout.GDIPlus;
using System.Drawing.Imaging;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdSaveTemplate : GeoDo.RSS.Core.UI.Command
    {
        public CmdSaveTemplate()
            : base()
        {
            _id = 6029;
            _name = "SaveTemplate";
            _text = _toolTip = "保存模版";
        }

        public override void Execute()
        {
            ILayoutViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            ILayoutHost host = viewer.LayoutHost;
            if (host == null)
                return;
            ILayoutTemplate template = host.Template;
            if (template == null || string.IsNullOrEmpty(template.FullPath))
                _smartSession.CommandEnvironment.Get(6006).Execute();
            else
            {
                template.SaveTo(template.FullPath);
                using (System.Drawing.Bitmap bm = viewer.LayoutHost.ExportToBitmap(PixelFormat.Format32bppArgb, new System.Drawing.Size(165, 165)))
                {
                    bm.Save(template.FullPath.ToLower().Replace(".gxt",".png"), ImageFormat.Png);
                }
            }
        }
    }
}
