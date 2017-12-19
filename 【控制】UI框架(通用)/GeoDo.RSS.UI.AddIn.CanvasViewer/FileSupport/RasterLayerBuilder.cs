using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.UI;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    public static class RasterLayerBuilder
    {
        public static IRasterLayer CreateAndLoadRasterLayerForGeoEye(ISmartSession session, ICanvas canvas, string fname, string componentID, params object[] args)
        {
            IRasterDrawing drawing = new RasterDrawing(fname, canvas, GetStretcherProvider(args), componentID);
            //此处应走配置文件
            drawing.SelectedBandNos = GetDefaultBands(drawing);
            if (args != null && args.Length == 1 && args[0] is int[])
                drawing.SelectedBandNos = (int[])args[0];
            IRasterLayer rstLayer = new RasterLayer(drawing);
            canvas.LayerContainer.Layers.Add(rstLayer);
            canvas.PrimaryDrawObject = drawing;
            canvas.CurrentEnvelope = drawing.OriginalEnvelope;
            int w = drawing.DataProvider.Width;
            int h = drawing.DataProvider.Height;
            //小图像不显示进度条
            int times = drawing.GetOverviewLoadTimes();
            if (times == -1)
            {
                drawing.StartLoading(null);
                return rstLayer;
            }
            string tipstring = "正在读取文件\"" + Path.GetFileName(fname) + "\"...";
            try
            {
                session.ProgressMonitorManager.DefaultProgressMonitor.Reset(tipstring, times);
                session.ProgressMonitorManager.DefaultProgressMonitor.Start(false);
                drawing.StartLoading((t, p) =>
                {
                    session.ProgressMonitorManager.DefaultProgressMonitor.Boost(p, tipstring);
                });
            }
            finally
            {
                session.ProgressMonitorManager.DefaultProgressMonitor.Finish();
            }
            return rstLayer;
        }

        public static IRasterLayer CreateAndLoadRasterLayerForHJ(ISmartSession session, ICanvas canvas, string fname, params object[] args)
        {
            IRasterDrawing drawing = new RasterDrawing(fname, canvas, GetStretcherProvider(args));
            //此处应走配置文件
            drawing.SelectedBandNos = GetDefaultBands(drawing);
            if (args != null && args.Length == 1 && args[0] is int[])
                drawing.SelectedBandNos = (int[])args[0];
            IRasterLayer rstLayer = new RasterLayer(drawing);
            canvas.LayerContainer.Layers.Add(rstLayer);
            canvas.PrimaryDrawObject = drawing;
            canvas.CurrentEnvelope = drawing.OriginalEnvelope;
            int w = drawing.DataProvider.Width;
            int h = drawing.DataProvider.Height;
            //小图像不显示进度条
            int times = drawing.GetOverviewLoadTimes();
            if (times == -1)
            {
                drawing.StartLoading(null);
                return rstLayer;
            }
            string tipstring = "正在读取文件\"" + Path.GetFileName(fname) + "\"...";
            try
            {
                session.ProgressMonitorManager.DefaultProgressMonitor.Reset(tipstring, times);
                session.ProgressMonitorManager.DefaultProgressMonitor.Start(false);
                drawing.StartLoading((t, p) =>
                {
                    session.ProgressMonitorManager.DefaultProgressMonitor.Boost(p, tipstring);
                });
            }
            finally
            {
                session.ProgressMonitorManager.DefaultProgressMonitor.Finish();
            }
            return rstLayer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="canvas"></param>
        /// <param name="fname"></param>
        /// <param name="args">
        /// 支持各种类型的多个参数
        /// 其中string[]类型的数据会作为RasterDrawing的options
        /// </param>
        /// <returns></returns>
        public static IRasterLayer CreateAndLoadRasterLayer(ISmartSession session, ICanvas canvas, string fname, params object[] args)
        {
            //bool isImage = IsImage(fname);
            //IRasterDrawing drawing = new RasterDrawing(fname, canvas, isImage ? null : GetStretcherProvider(args));
            string[] options = GetOptions(args);
            IRasterDrawing drawing = new RasterDrawing(fname, canvas, GetStretcherProvider(args), options);
            //此处应走配置文件
            drawing.SelectedBandNos = GetDefaultBands(drawing);
            if (args != null && args.Length == 1 && args[0] is int[])
                drawing.SelectedBandNos = (int[])args[0];
            IRasterLayer rstLayer = new RasterLayer(drawing);
            canvas.LayerContainer.Layers.Add(rstLayer);
            canvas.PrimaryDrawObject = drawing;
            canvas.CurrentEnvelope = drawing.OriginalEnvelope;
            int w = drawing.DataProvider.Width;
            int h = drawing.DataProvider.Height;
            //小图像不显示进度条
            int times = drawing.GetOverviewLoadTimes();
            if (times == -1)
            {
                drawing.StartLoading(null);
                return rstLayer;
            }
            string tipstring = "正在读取文件\"" + Path.GetFileName(fname) + "\"...";
            try
            {
                session.ProgressMonitorManager.DefaultProgressMonitor.Reset(tipstring, times);
                session.ProgressMonitorManager.DefaultProgressMonitor.Start(false);
                drawing.StartLoading((t, p) =>
                {
                    session.ProgressMonitorManager.DefaultProgressMonitor.Boost(p, tipstring);
                });
            }
            finally
            {
                session.ProgressMonitorManager.DefaultProgressMonitor.Finish();
            }
            return rstLayer;
        }

        private static string[] GetOptions(object[] args)
        {
            if (args == null || args.Length == 0)
                return null;
            foreach (object obj in args)
                if (obj is string[])
                    return obj as string[];
            return null;
        }

        private static IRgbStretcherProvider GetStretcherProvider(params object[] args)
        {
            if (args == null || args.Length == 0)
                return null;
            foreach (object obj in args)
                if (obj is IRgbStretcherProvider)
                    return obj as IRgbStretcherProvider;
            return null;
        }

        private static int[] GetDefaultBands(IRasterDrawing drawing)
        {
            IRasterDataProvider prd = drawing.DataProvider;
            if (prd == null)
                return null;
            int[] defaultBands = prd.GetDefaultBands();
            //if (defaultBands == null || IsImage(prd.fileName))
            if (defaultBands == null)
                defaultBands = new int[] { 1, 2, 3 };
            return defaultBands;
        }

        private static string[] _imageFormat = new string[] { ".BMP", ".PNG", ".JPG", ".JPEG" };
        private static bool IsImage(string fname)
        {
            string ext = Path.GetExtension(fname);
            foreach (string format in _imageFormat)
            {
                if (format == ext.ToUpper())
                    return true;
            }
            return false;
        }
    }
}
