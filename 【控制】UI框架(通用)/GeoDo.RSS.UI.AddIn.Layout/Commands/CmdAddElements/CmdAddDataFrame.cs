using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout.DataFrm;
using System.Drawing;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.VectorDrawing;
using GeoDo.RSS.Layout.GDIPlus;

namespace GeoDo.RSS.UI.AddIn.Layout
{
     [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdAddDataFrame : CmdAddElementBase
    {
         public CmdAddDataFrame()
             : base()
         {
             _id = 6033;
             _name = "AddDaraFrame";
             _text = _toolTip = "添加数据框";
         }

         public override void Execute()
         {
             ILayoutViewer view = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
             if (view == null)
                 return;
             ILayoutHost host = view.LayoutHost;
             if (host == null)
                 return;               
             if (host.LayoutRuntime == null)
                 return;
             if (host.LayoutRuntime.Layout == null)
                 return;
             if (host.LayoutRuntime.Layout.Elements == null || host.LayoutRuntime.Layout.Elements.Count == 0)
                 return;
             IDataFrame df = new DataFrame(host);
             IDataFrameDataProvider provider = df.Provider as IDataFrameDataProvider;
             if (provider == null)
                 return;
             ICanvas c = provider.Canvas;
             if (c != null)
             {
                 if (c.CanvasSetting != null)
                     if (c.CanvasSetting.RenderSetting != null)
                         c.CanvasSetting.RenderSetting.BackColor = Color.White;
                 IVectorHostLayer vhost = new VectorHostLayer(null);
                 c.LayerContainer.Layers.Add(vhost as GeoDo.RSS.Core.DrawEngine.ILayer);
             }
             host.LayoutRuntime.Layout.Elements.Insert(1, df);
             host.Render();             
             host.ActiveDataFrame = df;
             TryRefreshLayerManager();
         }
    }
}
