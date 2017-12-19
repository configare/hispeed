using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Layout.GDIPlus;
using GeoDo.Project;
using GeoDo.RSS.Core.RasterDrawing;

namespace GeoDo.RSS.UI.WinForm
{
    internal static class GlobaCacherlInitializer
    {
        public static void Init(ISmartSession session) 
        {
            CodeCell.AgileMap.Core.GlobalCacher.VectorDataGlobalCacher = new GeoDo.RSS.Core.VectorDrawing.VectorDataGlobalCacher();
            //CodeCell.AgileMap.Core.GlobalCacher.VectorDataGlobalCacher.IsEnabled = false;
            CodeCell.AgileMap.Core.GlobalCacher.VectorDataGlobalCacher.SetGllPrjChecker
                (
                   /*
                    * 只在全局缓存中放投影为等经纬度的矢量数据
                    */
                   () => 
                   {
                       if (IsOrbitData(session))
                           return false;
                       ISpatialReference spatialRef = GetSpatialRef(session);
                       return spatialRef == null || spatialRef.ProjectionCoordSystem == null;
                   }
                );
        }

        private static bool IsOrbitData(ISmartSession session)
        {
            ISmartViewer activeViewer = session.SmartWindowManager.ActiveViewer;
            if (activeViewer is ICanvasViewer)
            {
                ICanvasViewer cv = activeViewer as ICanvasViewer;
                IRasterDrawing draing = cv.ActiveObject as IRasterDrawing;
                if (draing == null || draing.DataProvider.DataIdentify == null)
                    return false;
                return draing.DataProvider.DataIdentify.IsOrbit;
            }
            return false;
        }

        private static ISpatialReference GetSpatialRef(ISmartSession session)
        {
            ISmartViewer activeViewer = session.SmartWindowManager.ActiveViewer;
            if (activeViewer is ICanvasViewer)
            {
                return (activeViewer as ICanvasViewer).Canvas.CoordTransform.SpatialRefOfViewer as ISpatialReference;
            }
            else if (activeViewer is ILayoutViewer)
            {
                IDataFrame df = (activeViewer as ILayoutViewer).LayoutHost.ActiveDataFrame;
                if (df == null)
                    return null;
                IDataFrameDataProvider prd = df.Provider as IDataFrameDataProvider;
                if (prd == null)
                    return null;
                return prd.Canvas.CoordTransform.SpatialRefOfViewer as ISpatialReference;
            }
            return null;
        }
    }
}
