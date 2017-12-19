using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;
using System.Drawing;
using GeoDo.RSS.Core.VectorDrawing;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.DataRef
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandDefaultBkgrd : Command
    {
        public CommandDefaultBkgrd()
            : base()
        {
            _id = 4044;
            _text = _toolTip = "应用默认地图背景";
        }

        public override void Execute()
        {
            ICanvasViewer cv = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (cv == null)
                return;
            ILayer layer = cv.Canvas.LayerContainer.GetByName("海陆背景");
            if (layer != null)
                return;
            string fname = AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\矢量模版\海陆模版.shp";
            if (!File.Exists(fname))
            {
                Console.WriteLine("文件\"" + fname + "\"未找到,无法应用默认背景。");
                return;
            }
            IBackgroundLayer lyr = new BackgroundLayer(fname);
            cv.Canvas.LayerContainer.Layers.Add(lyr);
            cv.Canvas.Refresh(enumRefreshType.All);
        }
    }
}
