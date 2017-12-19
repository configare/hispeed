using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.Grid;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Layout.GDIPlus;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.Core.VectorDrawing;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.AddIn.DataRef
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandRemoveAllVector : Command
    {
        //private ICanvas _canvas = null;
        //private ILayoutHost _host = null;

        public CommandRemoveAllVector()
        {
            _id = 4050;
            _name = "RemoveAllVector";
            _text = _toolTip = "移除所有矢量数据";
        }

        public override void Execute()
        {
            ICanvas _canvas = null; ILayoutHost _host = null;
            if (_smartSession.SmartWindowManager.ActiveViewer is ICanvasViewer)
            {
                GetCanvasByCanvasViewer(out _canvas);
                ICurrentRasterInteractiver cv = (_smartSession.SmartWindowManager.ActiveViewer as ICurrentRasterInteractiver);//LabelService也是ICanvasViewer上的矢量
                if (cv != null && cv.LabelService != null)
                    cv.LabelService.Reset();
            }
            else if (_smartSession.SmartWindowManager.ActiveViewer is ILayoutViewer)
            {
                GetCanvasByLayoutDataFrame(out _canvas, out _host);
            }
            if (_canvas == null)
                return;
            if (_canvas.LayerContainer == null)
                return;
            if (_canvas.LayerContainer.VectorHost != null)
            {
                RemoveAllVectorLayer(_canvas.LayerContainer.VectorHost as IVectorHostLayer);
            }
            Layer removeLayer = null;
            foreach (Layer layer in _canvas.LayerContainer.Layers)
            {
                if ((layer as GeoGridLayer) != null)
                    removeLayer = layer;
            }
            if (removeLayer != null)
                _canvas.LayerContainer.Layers.Remove(removeLayer);
            _canvas.Refresh(enumRefreshType.VectorLayer);
            if (_host != null)
                _host.Render(true);
        }

        private void RemoveAllVectorLayer(IVectorHostLayer hostLayer)
        {
            if (hostLayer == null)
                return;
            (hostLayer as IDisposable).Dispose();
            GC.Collect();
            //VectorHostLayer newHost = new VectorHostLayer(null);
            //_canvas.LayerContainer.Layers.Add(newHost);
            return;
            /*
            IMap map = hostLayer.Map as IMap;
            if (map == null || map.LayerContainer.FeatureLayers == null || map.LayerContainer.FeatureLayers.Length == 0)
                return;
            List<string> layerNames = new List<string>();
            foreach (CodeCell.AgileMap.Core.IFeatureLayer lyr in map.LayerContainer.FeatureLayers)
            {
                IFeatureClass fetclass = lyr.Class as IFeatureClass;
                if (fetclass == null)
                    continue;
                FileDataSource fds = fetclass.DataSource as FileDataSource;
                if (fds == null)
                    continue;
                ICachedVectorData data = GlobalCacher.VectorDataGlobalCacher.GetData(fds.FileUrl);
                if (data != null)
                    layerNames.Add(data.Identify);
            }
            map.LayerContainer.Clear(false);
            foreach(string fName in layerNames)
                GlobalCacher.VectorDataGlobalCacher.Release(fName);
            GC.Collect();
             * */
        }

        private void GetCanvasByCanvasViewer(out ICanvas _canvas)
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            _canvas = viewer.Canvas;
        }

        private void GetCanvasByLayoutDataFrame(out ICanvas _canvas, out ILayoutHost _host)
        {
            _canvas = null;
            _host = null;
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
