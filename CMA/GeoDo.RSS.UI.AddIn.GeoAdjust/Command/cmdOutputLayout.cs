using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.RasterDrawing;
using System.Drawing.Imaging;
using GeoDo.RSS.UI.AddIn.Layout;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.GeoAdjust
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class cmdOutputLayout:Command
    {
        public cmdOutputLayout()
            : base()
        {
            _id = 30207;
            _text = _name = "导出专题图";
        }

        public override void Execute()
        {
            Execute("GeoAdjustByPan3");
        }

        public override void Execute(string argument)
        {
            ISmartWindow[] wnds = _smartSession.SmartWindowManager.GetSmartWindows((wnd) => { return wnd is ILayoutViewer; });
            if (wnds != null)
            {
                foreach (ISmartWindow wnd in wnds)
                {
                    ILayoutViewer viewer = wnd as ILayoutViewer;
                    if (viewer == null)
                        continue;
                    if (viewer.LayoutHost == null)
                        continue;
                    string defaultFname = string.Empty;
                    if ((viewer as LayoutViewer).Tag != null)
                        defaultFname = (viewer as LayoutViewer).Tag as string;
                    defaultFname = Path.Combine(Path.GetDirectoryName(defaultFname), Path.GetFileNameWithoutExtension(defaultFname) + ".jpg");
                    ImageFormat imgFormat = ImageFormat.Jpeg;
                    //
                    using (Bitmap bmp = viewer.LayoutHost.ExportToBitmap(PixelFormat.Format32bppArgb))
                    {
                        bmp.Save(defaultFname, imgFormat);
                    }
                }
            }
            _smartSession.UIFrameworkHelper.SetVisible(argument, false);
            _smartSession.UIFrameworkHelper.SetLockBesideX(argument, false);
        }
    }
}
