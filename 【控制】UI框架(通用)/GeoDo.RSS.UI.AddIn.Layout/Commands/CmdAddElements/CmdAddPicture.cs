using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout.GDIPlus;
using System.Windows.Forms;
using GeoDo.RSS.Layout;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdAddPicture : CmdAddElementBase
    {
        public CmdAddPicture()
            : base()
        {
            _id = 6009;
            _name = "AddPicture";
            _text = _toolTip = "添加图片";
        }

        public override void Execute()
        {
            ILayoutViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            ILayoutHost host = viewer.LayoutHost;
            if (host == null)
                return;
            ILayoutRuntime runtime = host.LayoutRuntime;
            if (runtime == null)
                return;
            float w = 0, h = 0;
             using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "JPEG(*.jpg)|*.jpg|BMP(*.bmp)|*.bmp|PNG(*.png)|*.png";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    PictureElement pic = new PictureElement(dlg.FileName);
                    w = pic.Size.Width;
                    h = pic.Size.Height;
                    runtime.Pixel2Layout(ref w, ref h);
                    pic.Size = new System.Drawing.SizeF(w,h);
                    viewer.LayoutHost.LayoutRuntime.Layout.Elements.Add(pic);
                    viewer.LayoutHost.Render();
                }
            }
             TryRefreshLayerManager();
        }
    }
}
