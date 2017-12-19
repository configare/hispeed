using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public class FeatureSplitterByProjection : IFeatureSplitterByProjection
    {
        SplitRangesComputer _computer = null;
        SplitRangesComputer.Range latRange;
        SplitRangesComputer.Range[] lonRanges = null;
        double _centerLont = 0;
        double _minLat = -85;
        double _maxLat = 85;
        double _minLon = -180;
        double _maxLon = 180;
        bool _isNeedSplit = false;

        public FeatureSplitterByProjection(string spatialRef)
        {
            ISpatialReference spRef = SpatialReferenceFactory.GetSpatialReferenceByWKT(spatialRef, enumWKTSource.EsriPrjFile);
            if (spRef != null)
            {
                _computer = new SplitRangesComputer(spRef);
                _isNeedSplit = _computer.ComputeValidGeoRange(out _centerLont, out _minLat, out _maxLat,out _minLon,out _maxLon);
            }
        }

        public bool IsNeedSplit
        {
            get { return _isNeedSplit; }
        }

        public void Split(Feature feature, out Feature[] splittedFeatures)
        {
            splittedFeatures = Split(feature);
        }

        private Feature[] Split(Feature feature)
        {
            //if (feature.IsOutsideByLongitude(_minLon, _maxLon))
            //    return null;
            //if (feature.IsOutsideByLatitude(_minLat, _maxLat))
            //    return null;
            TryHandleLatitudeRange(feature);
            TryHandleLongitudeRange(feature);
            List<Feature> returnFeatures = new List<Feature>();
            Feature leftFeature,rightFeature;
            feature.SplitByLongitude(_centerLont, out leftFeature, out rightFeature);
            if (leftFeature != null)
                returnFeatures.Add(leftFeature);
            if (rightFeature != null)
                returnFeatures.Add(rightFeature);
            return returnFeatures.Count > 0 ? returnFeatures.ToArray() : null;
        }

        private void TryHandleLongitudeRange(Feature feature)
        {
            if (feature.Geometry is ShapePolygon)
            {
                ShapePolygon ply = feature.Geometry as ShapePolygon;
                foreach (ShapeRing ring in ply.Rings)
                {
                    foreach (ShapePoint pt in ring.Points)
                    {
                        if (pt.X > _maxLon)
                            pt.X = _maxLon;
                        else if (pt.X < _minLon)
                            pt.X = _minLon;
                    }
                }
            }
            else if(feature.Geometry is ShapePolyline)
            {
                ShapePolyline pline = feature.Geometry as ShapePolyline;
                foreach (ShapeLineString part in pline.Parts)
                {
                    foreach (ShapePoint pt in part.Points)
                    {
                        if (pt.X > _maxLon)
                            pt.X = _maxLon;
                        else if (pt.X < _minLon)
                            pt.X = _minLon;
                    }
                }
            }
            else if (feature.Geometry is ShapePoint)
            {
                ShapePoint pt = feature.Geometry as ShapePoint;
                if (pt.X > _maxLon)
                    pt.X = _maxLon;
                else if (pt.X < _minLon)
                    pt.X = _minLon;
            }
        }

        private void TryHandleLatitudeRange(Feature feature)
        {
            if (feature.Geometry is ShapePolygon)
            {
                ShapePolygon ply = feature.Geometry as ShapePolygon;
                foreach (ShapeRing ring in ply.Rings)
                {
                    foreach (ShapePoint pt in ring.Points)
                    {
                        if (pt.Y > _maxLat)
                            pt.Y = _maxLat;
                        else if (pt.Y < _minLat)
                            pt.Y = _minLat;
                    }
                }
            }
            else if (feature.Geometry is ShapePolyline)
            {
                ShapePolyline pline = feature.Geometry as ShapePolyline;
                foreach (ShapeLineString part in pline.Parts)
                {
                    foreach (ShapePoint pt in part.Points)
                    {
                        if (pt.Y > _maxLat)
                            pt.Y = _maxLat;
                        else if (pt.Y < _minLat)
                            pt.Y = _minLat;
                    }
                }
            }
            else if (feature.Geometry is ShapePoint)
            {
                ShapePoint pt = feature.Geometry as ShapePoint;
                if (pt.Y > _maxLat)
                    pt.Y = _maxLat;
                else if (pt.Y < _minLat)
                    pt.Y = _minLat;
            }
        }
    }
}
