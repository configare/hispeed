using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.Grid;
using System.IO;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Layout.GDIPlus;

namespace GeoDo.RSS.UI.AddIn.DataRef
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandRemoveVector : Command
    {
        private ICanvas _canvas = null;
        private ILayoutHost _host = null;

        public CommandRemoveVector()
        {
            _id = 4051;
            _name = "RemoveRaster";
            _text = _toolTip = "分类移除矢量数据";
        }

        public override void Execute(string type)
        {
            if (_smartSession.SmartWindowManager.ActiveViewer is ICanvasViewer)
                GetCanvasByCanvasViewer();
            else if (_smartSession.SmartWindowManager.ActiveViewer is ILayoutViewer)
                GetCanvasByLayoutDataFrame();
            if (_canvas == null)
                return;
            if (_canvas.LayerContainer == null)
                return;
            if (_canvas.LayerContainer.VectorHost != null)
                (_canvas.LayerContainer.VectorHost as IVectorHostLayer).ClearAll();
            switch (type)
            {
                case "basic":
                    Layer removeLayer = null;
                    foreach (Layer layer in _canvas.LayerContainer.Layers)
                    {
                        if ((layer as GeoGridLayer) != null)
                            removeLayer = layer;
                    }
                    if (removeLayer != null)
                        _canvas.LayerContainer.Layers.Remove(removeLayer);
                    break;
                case "inters":
                    break;
                case "split":
                    break;
                case "data":
                    break;
            }
            _canvas.Refresh(enumRefreshType.VectorLayer);
            if (_host != null)
                _host.Render();
        }

        private void GetCanvasByCanvasViewer()
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            _canvas = viewer.Canvas;
        }

        private void GetCanvasByLayoutDataFrame()
        {
            ILayoutViewer view = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (view == null)
                return;
            _host = view.LayoutHost;
            if (_host == null)
                return;
            IDataFrame dataFrame = _host.ActiveDataFrame;
            if (dataFrame == null)
                return;
            _canvas = (dataFrame.Provider as IDataFrameDataProvider).Canvas;
        }
    }
}
