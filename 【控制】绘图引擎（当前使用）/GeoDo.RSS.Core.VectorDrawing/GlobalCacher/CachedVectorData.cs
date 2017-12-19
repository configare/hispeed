using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.Core.VectorDrawing
{
    internal class CachedVectorData : ICachedVectorData
    {
        private string _identify;
        private Feature[] _features;
        private enumCoordinateType _coordType = enumCoordinateType.Geographic;
        private GeoDo.Project.IProjectionTransform _coordTransform;

        public CachedVectorData(string shpFileName)
        {
            _identify = GetIdenfity(shpFileName);
            _features = ReadFeatures(shpFileName);
        }

        public CachedVectorData(string shpFileName, GeoDo.Project.IProjectionTransform coordTransform)
            : this(shpFileName)
        {
            if (coordTransform != null)
            {
                _coordTransform = coordTransform;
                Project(_features);
                _coordType = enumCoordinateType.Projection;
            }
        }

        private void Project(Feature[] features)
        {
            if (features == null || features.Length == 0)
                return;
            foreach (Feature fet in features)
                ProjectFeature(fet);
        }

        private void ProjectFeature(Feature fet)
        {
            Shape geometry = fet.Geometry;
            double[] xs = new double[1];
            double[] ys = new double[1];
            if (geometry is ShapePolygon)
            {
                ShapePolygon plygon = geometry as ShapePolygon;
                foreach (ShapeRing ring in plygon.Rings)
                {
                    foreach (ShapePoint pt in ring.Points)
                    {
                        xs[0] = pt.X;
                        ys[0] = pt.Y;
                        _coordTransform.Transform(xs, ys);
                        pt.X = xs[0];
                        pt.Y = ys[0];
                    }
                    ring.UpdateCentroid();
                    ring.UpdateEnvelope();
                }
                plygon.UpdateCentroid();
                plygon.UpdateEnvelope();
            }
            else if (geometry is ShapePolyline)
            {
                ShapePolyline plyline = geometry as ShapePolyline;
                foreach (ShapeLineString line in plyline.Parts)
                {
                    foreach (ShapePoint pt in line.Points)
                    {
                        xs[0] = pt.X;
                        ys[0] = pt.Y;
                        _coordTransform.Transform(xs, ys);
                        pt.X = xs[0];
                        pt.Y = ys[0];
                    }
                    line.UpdateCentroid();
                    line.UpdateEnvelope();
                }
                plyline.UpdateCentroid();
                plyline.UpdateEnvelope();
            }
            else if (geometry is ShapePoint)
            {
                ShapePoint pt = geometry as ShapePoint;
                xs[0] = pt.X;
                ys[0] = pt.Y;
                _coordTransform.Transform(xs, ys);
                pt.X = xs[0];
                pt.Y = ys[0];
                pt.UpdateCentroid();
                pt.UpdateEnvelope();
            }
            fet.Geometry.UpdateCentroid();
            fet.Geometry.UpdateEnvelope();
            //
            if (fet.Annotations != null && fet.Annotations.Length > 0)
            {
                foreach (LabelLocation loc in fet.Annotations)
                {
                    xs[0] = loc.Location.X;
                    ys[0] = loc.Location.Y;
                    _coordTransform.Transform(xs, ys);
                    loc.Location = new ShapePoint(xs[0], ys[0]);
                }
            }
            //
            fet.Projected = true;
        }

        internal static string GetIdenfity(string shpFileName)
        {
            if (string.IsNullOrEmpty(shpFileName))
                return null;
            return shpFileName.Replace(":", "_").Replace(@"\", "_").Replace("/", "_");
        }

        private Feature[] ReadFeatures(string shpFileName)
        {
            using (IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(shpFileName) as IVectorFeatureDataReader)
            {
                return dr.FetchFeatures();
            }
        }

        public string Identify
        {
            get { return _identify; }
        }

        public enumCoordinateType CoordType
        {
            get { return _coordType; }
        }

        public Feature[] GetFeatures(Envelope envelope)
        {
            if (_features == null || _features.Length == 0)
                return null;
            if (envelope == null)
                return _features;
            List<Feature> fets = new List<Feature>();
            foreach (Feature fet in _features)
            {
                if (envelope.IsInteractived(fet.Geometry.Envelope))
                    fets.Add(fet);
            }
            return fets.Count > 0 ? fets.ToArray() : null;
        }

        public void Dispose()
        {
            _coordTransform = null;
            if (_features != null && _features.Length != 0)
            {
                foreach (Feature feature in _features)
                {
                    feature.Dispose();
                }
                _features = null;
            }
        }
    }
}
