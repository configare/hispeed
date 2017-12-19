using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;
using System.Drawing;
using System.Drawing.Imaging;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    /// <summary>
    /// 尚未使用该类
    /// </summary>
    public class CmaLayoutSubProduct:CmaMonitoringSubProduct
    {
        public CmaLayoutSubProduct()
            : base()
        { 
        }

        public CmaLayoutSubProduct(SubProductDef def)
            : base(def)
        { 
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            IArgumentProvider argPrd = _argumentProvider;
            ArgumentDef def = argPrd.GetArgDef("ImageFile");
            if(def.RefIdentify == "CurrentImage" && def.Defaultvalue == null)
            {
            }
            return null;
        }

        private string SaveAsCurrentImage()
        {
            ISmartSession session = SmartApp.SmartSession;
            ICanvasViewer cv = session.SmartWindowManager.ActiveCanvasViewer;
            if (cv == null)
                return null;
            IRasterDrawing drawing = cv.ActiveObject as IRasterDrawing;
            if (drawing == null)
                return null;
            Bitmap bm = drawing.GetBitmapUseOriginResolution();
            bm.Save("", ImageFormat.Bmp);
            //generate a world file
            //
            return string.Empty;
        }
    }
}
