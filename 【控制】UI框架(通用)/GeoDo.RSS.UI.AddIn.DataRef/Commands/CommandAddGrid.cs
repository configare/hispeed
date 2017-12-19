using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.IO;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.Grid;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Layout.GDIPlus;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.DataRef
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandAddGrid : Command
    {
        public CommandAddGrid()
            : base()
        {
            _id = 4041;
            _name = "AddGrid";
            _text = _toolTip = "添加经纬网格";
        }

        /// <summary>
        /// 添加经纬网格
        /// </summary>
        /// <param name="argument">经纬网格间隔："1"</param>
        public override void Execute(string argument)
        {
            ICanvas canvas = null;
            ILayoutHost host = null;
            if (_smartSession.SmartWindowManager.ActiveViewer is ICanvasViewer)
                canvas = GetCanvasByCanvasViewer();
            else if (_smartSession.SmartWindowManager.ActiveViewer is ILayoutViewer)
                canvas = GetCanvasByLayoutDataFrame(out host);
            if (canvas == null)
                return;
            //无坐标的图片
            if (canvas.IsRasterCoord)
                return;
            //判断是否已经有经纬网格层的存在
            double intervel = 1f;
            bool ok = double.TryParse(argument, out intervel);
            if (!ok)
                return;
            List<ILayer> layers = canvas.LayerContainer.Layers;
            GeoGridLayer gridLayer = null;
            foreach (ILayer layer in layers)
            {
                if (layer is GeoGridLayer)
                    gridLayer = (layer as GeoGridLayer);
                else
                    continue;
            }
            //如果没有经纬网格的存在则新建经纬网格，并添加
            if (gridLayer == null)
            {
                gridLayer = new GeoGridLayer();
                gridLayer.GridSpan = intervel;
                canvas.LayerContainer.Layers.Add(gridLayer);
            }
            //如果已经有经纬网格的存在则修改经纬网格的属性即可
            else
                gridLayer.GridSpan = intervel;
            canvas.Refresh(enumRefreshType.All);
            if (host != null)
            {
                //专题图中默认网格颜色为蓝色
                gridLayer.GridColor = Color.Blue;
                //刷新专题图
                host.Render(true);
            }
            //
            TryRefreshLayerManager();
        }

        public void TryRefreshLayerManager()
        {
            ISmartWindow layerManager = _smartSession.SmartWindowManager.GetSmartWindow((wnd) => { return wnd is ILayerManager; });
            if (layerManager != null)
                (layerManager as ILayerManager).Update();
        }

        private ICanvas GetCanvasByCanvasViewer()
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            return viewer.Canvas;
        }

        private ICanvas GetCanvasByLayoutDataFrame(out ILayoutHost host)
        {
            host = null;
            ILayoutViewer view = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (view == null)
                return null;
            host = view.LayoutHost;
            if (host == null)
                return null;
            IDataFrame dataFrame = host.ActiveDataFrame;
            if (dataFrame == null)
                return null;
            return (dataFrame.Provider as IDataFrameDataProvider).Canvas;
        }
    }
}
