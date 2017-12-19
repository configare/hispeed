using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using GeoDo.RSS.Core.DrawEngine.GDIPlus;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandDisplayRuler : Command
    {
        public CommandDisplayRuler()
            : base()
        {
            _name = "CommandDisplayRuler";
            _text = _toolTip = "坐标标尺";
            _id = 7109;
        }

        public override void Execute()
        {
            Execute(null);
        }

        public override void Execute(string argument)
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return;
            var v = viewer.Canvas.LayerContainer.Layers.Where((lyr) => { return lyr is IRulerLayer; });
            if (v != null && v.Count() > 0)
            {
                ILayer[] lyrs = v.ToArray();
                viewer.Canvas.LayerContainer.Layers.Remove(lyrs[0]);
                viewer.Canvas.Refresh(enumRefreshType.All);
                return;
            }
            IRulerLayer rulerLayer = new RulerLayer();
            viewer.Canvas.LayerContainer.Layers.Add(rulerLayer as ILayer);
            viewer.Canvas.Refresh(Core.DrawEngine.enumRefreshType.All);
        }
    }
}
