using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.Runtime;

namespace CodeCell.AgileMap.MapService
{
    public class QueryExecutor
    {
        private static IMapImageGenerator _gen = null;

        public QueryExecutor(IMapImageGenerator gen)
        {
            _gen = gen;
        }

        public FeatureInfo[] Query(string layerId, string geometry, string keywords)
        {
            List<IFeatureLayer> lyrs = GetQueryLayers(layerId);
            if (lyrs == null || lyrs.Count == 0)
                return null;
            Feature[] fets = Query(lyrs, geometry, keywords);
            return FeaturesToFeatureInfo(fets);
        }

        public static FeatureInfo[] FeaturesToFeatureInfo(Feature[] fets)
        {
            List<FeatureInfo> fetInfos = new List<FeatureInfo>(fets.Length);
            foreach (Feature fet in fets)
            {
                FeatureInfo info = new FeatureInfo(fet.OID);
                info.Envelope = fet.Geometry.Envelope.Clone() as Envelope;
                if (fet.Geometry.IsProjected)
                    ProjectEnvelope(info.Envelope);
                info.Properties = GetFeatureProperties(fet);
                fetInfos.Add(info);
            }
            return fetInfos.ToArray();
        }

        private static void ProjectEnvelope(Envelope evp)
        {
            if (evp == null)
                return;
            ShapePoint[] pts = evp.Points;
            _gen.ProjectionTransform.InverTransform(pts);
            //
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;
            foreach (ShapePoint pt in pts)
            {
                if (pt.X < minX)
                    minX = pt.X;
                if (pt.Y < minY)
                    minY = pt.Y;
                if (pt.X > maxX)
                    maxX = pt.X;
                if (pt.Y > maxY)
                    maxY = pt.Y;
            }
            evp.MinX = minX;
            evp.MinY = minY;
            evp.MaxX = maxX;
            evp.MaxY = maxY;
        }

        private static Dictionary<string, string> GetFeatureProperties(Feature fet)
        {
            if (fet.FieldNames == null || fet.FieldNames.Length == 0 || fet.FieldValues == null || fet.FieldValues.Length == 0)
                return null;
            Dictionary<string, string> ps = new Dictionary<string, string>();
            int n = Math.Min(fet.FieldNames.Length, fet.FieldValues.Length);
            for (int i = 0; i < n; i++)
                ps.Add(fet.FieldNames[i], fet.FieldValues[i] ?? string.Empty);
            return ps;
        }

        private Feature[] Query(List<IFeatureLayer> lyrs, string geometry, string keywords)
        {
            QueryFilter filter = new QueryFilter(keywords);
            List<Feature> retFeatures = new List<Feature>();
            foreach (IFeatureLayer lyr in lyrs)
            {
                //Log.WriterWarning("Query:"+lyr.Name);
                IFeatureClass fetclass = lyr.Class as IFeatureClass;
                Feature[] fets = (fetclass.DataSource as IFeatureDataSource).Query(filter);
                if (fets != null && fets.Length > 0)
                {
                    retFeatures.AddRange(fets);
                    //Log.WriterWarning("Count:" + fets.Length.ToString());
                }
            }
            return retFeatures.ToArray();
        }

        private List<IFeatureLayer> GetQueryLayers(string layerId)
        {
            //Log.WriterWarning("LayerId:" + layerId);
            if (layerId == null)
                return new List<IFeatureLayer>(_gen.Map.LayerContainer.FeatureLayers);
            IFeatureLayer lyr = _gen.Map.LayerContainer.GetLayerById(layerId) as IFeatureLayer;
            if (lyr != null)
                return new List<IFeatureLayer>(new IFeatureLayer[] { lyr });
            //Log.WriterWarning("FeatureLayer is empty!");
            return null;
        }
    }
}