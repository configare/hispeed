using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.UIs;


namespace CodeCell.AgileMap.Components
{
    internal class MapIdentifyTool : MapToolBase
    {
        public MapIdentifyTool()
            : base()
        {
        }

        protected override void MouseDown(IMapControl mapcontrol, MouseEventArgs e)
        {
            if (mapcontrol.MapRuntime.QueryResultContainer == null)
            {
                MsgBox.ShowInfo("没有设置查询结果容器,无法进行拾取操作!");
                return;
            }
            Point pt = e.Location;
            mapcontrol.CoordinateTransfrom.PixelCoord2PrjCoord(ref pt);
            ShapePoint prjPoint = new ShapePoint(pt.X, pt.Y);
            double tol = mapcontrol.CoordinateTransfrom.GetPixelTolerance(10);
            mapcontrol.MapRuntime.QueryResultContainer.Clear();
            Feature[] fets = Identify(mapcontrol, prjPoint, tol);
            if (fets != null)
            {
                mapcontrol.MapRuntime.QueryResultContainer.AddFeatures(fets.ToArray());
            }
            mapcontrol.MapRuntime.QueryResultContainer.ResultContainerVisible = true;
        }

        private Feature[] Identify(IMapControl  mapcontrol,ShapePoint prjPoint,double tolerance)
        {
            string[] layernames = GetIdentifyLayers(mapcontrol);
            if (layernames == null || layernames.Length == 0)
                return null;
            List<Feature> retfets = new List<Feature>();
            foreach (string lyrname in layernames)
            {
                IFeatureLayer layer = mapcontrol.Map.LayerContainer.GetLayerByName(lyrname) as IFeatureLayer;
                if (layer == null)
                    continue;
                Feature[] fets = layer.Identify(prjPoint, tolerance);
                if (fets != null)
                    retfets.AddRange(fets);
            }
            return retfets.Count > 0 ? retfets.ToArray() : null;
        }

        private string[] GetIdentifyLayers(IMapControl mapcontrol)
        {
            if (mapcontrol.Map.LayerContainer.Layers == null || mapcontrol.Map.LayerContainer.Layers.Length == 0)
                return null;
            List<string> layers = new List<string>();
            foreach (IFeatureLayer lyr in mapcontrol.Map.LayerContainer.Layers)
            {
                IFeatureClass fetclass = lyr.Class as IFeatureClass;
                if (fetclass.ShapeType == enumShapeType.Polygon)
                    layers.Add(lyr.Name);
            }
            return layers.Count > 0 ? layers.ToArray() : null;
            //return new string[] { "中国行政区" };
        }
    }
}
