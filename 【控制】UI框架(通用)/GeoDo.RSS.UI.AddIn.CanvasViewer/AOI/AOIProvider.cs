using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    public class AOIProvider : IAOIProvider, IDisposable
    {
        protected ICanvas _canvas;
        protected IAOIContainerLayer _aoiContainer;
        protected ISelectedAOILayer _selectAOILayer;

        public AOIProvider(ICanvas canvas)
        {
            _canvas = canvas;
            _aoiContainer = _canvas.LayerContainer.GetByName("感兴趣区域") as IAOIContainerLayer;
            _selectAOILayer = _canvas.LayerContainer.GetByName("选中的感兴趣区域") as ISelectedAOILayer;
        }

        public bool IsEmpty()
        {
            return _aoiContainer == null || _aoiContainer.FirstAOI == null;
        }

        public void Reset()
        {
            _aoiContainer.Reset();
            _selectAOILayer.Reset();
        }

        public Size BitmapSize
        {
            get
            {
                IRasterDrawing rst = _canvas.PrimaryDrawObject as IRasterDrawing;
                return rst.Bitmap.Size;
            }
        }

        public int[] GetBitmapIndexes()
        {
            int[] retAOI = null;
            foreach (object obj in _aoiContainer.AOIs)
            {
                int[] aoi = null;
                if (obj is GeometryOfDrawed)
                    aoi = GetBitmapIndexes(obj as GeometryOfDrawed);
                else if (obj is Feature)
                    aoi = GetBitmapIndexes(obj as Feature);
                //
                if (aoi == null)
                    continue;
                if (retAOI == null)
                    retAOI = aoi;
                else
                    retAOI = AOIHelper.Merge(new int[][] { retAOI, aoi });
            }
            return retAOI;
        }

        public int[] GetBitmapIndexes(Feature feature)
        {
            IRasterDrawing rst = _canvas.PrimaryDrawObject as IRasterDrawing;
            GeoDo.RSS.Core.DrawEngine.CoordEnvelope coordEvp = rst.Envelope;
            Envelope evp = null;
            if (feature.Projected)
                evp = new Envelope(coordEvp.MinX, coordEvp.MinY, coordEvp.MaxX, coordEvp.MaxY);
            else
            {
                if (rst.DataProvider.DataIdentify != null && rst.DataProvider.DataIdentify.IsOrbit)
                    rst.DataProvider.OrbitProjectionTransformControl.Build();
                GeoDo.RSS.Core.DrawEngine.ICoordinateTransform tran = _canvas.CoordTransform;
                double geoX1, geoY1, geoX2, geoY2;
                tran.Prj2Geo(coordEvp.MinX, coordEvp.MaxY, out geoX1, out geoY1);
                tran.Prj2Geo(coordEvp.MaxX, coordEvp.MinY, out geoX2, out geoY2);
                evp = new Envelope(geoX1, geoY2, geoX2, geoY1);
            }
            Size size = rst.Bitmap.Size;
            using (IVectorAOIGenerator gen = new VectorAOIGenerator())
            {
                return gen.GetAOI(new ShapePolygon[] { feature.Geometry as ShapePolygon }, evp, size);
            }
        }

        private int[] GetBitmapIndexes(GeometryOfDrawed geometryOfDrawed)
        {
            PointF[] pts = geometryOfDrawed.RasterPoints.Clone() as PointF[];
            double prjX, prjY;
            GeoDo.RSS.Core.DrawEngine.ICoordinateTransform tran = _canvas.CoordTransform;
            for (int i = 0; i < pts.Length; i++)
            {
                tran.Raster2Prj((int)pts[i].Y, (int)pts[i].X, out prjX, out prjY);
                pts[i].X = (float)prjX;
                pts[i].Y = (float)prjY;
            }
            IRasterDrawing rst = _canvas.PrimaryDrawObject as IRasterDrawing;
            Size size = rst.Bitmap.Size;
            using (IVectorAOIGenerator gen = new VectorAOIGenerator())
            {
                Envelope evp = new Envelope(rst.Envelope.MinX, rst.Envelope.MinY, rst.Envelope.MaxX, rst.Envelope.MaxY);
                return gen.GetAOI(pts, geometryOfDrawed.Types, evp, size);
            }
        }

        public int[] GetIndexes()
        {
            int[] retAOI = null;
            foreach (object obj in _aoiContainer.AOIs)
            {
                int[] aoi = null;
                if (obj is GeometryOfDrawed)
                    aoi = GetIndexes(obj as GeometryOfDrawed);
                else if (obj is Feature)
                    aoi = GetIndexes(obj as Feature);
                //
                if (aoi == null)
                    continue;
                if (retAOI == null)
                    retAOI = aoi;
                else
                    retAOI = AOIHelper.Merge(new int[][] { retAOI, aoi });
            }
            return retAOI;
        }

        /// <summary>
        /// 矢量数据取AOI，
        /// 矢量与栅格坐标不一致时候，转矢量。
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        private int[] GetIndexes(Feature feature)
        {
            GeoDo.RSS.Core.DrawEngine.ICoordinateTransform tran = _canvas.CoordTransform;
            IRasterDrawing rasterDrawing = _canvas.PrimaryDrawObject as IRasterDrawing;
            GeoDo.RSS.Core.DF.CoordEnvelope coordEvp = rasterDrawing.DataProvider.CoordEnvelope.Clone();
            Size rasterSize = rasterDrawing.Size;
            Envelope rasterEnv = new Envelope(coordEvp.MinX, coordEvp.MinY, coordEvp.MaxX, coordEvp.MaxY);
            if (feature.Projected && rasterDrawing.DataProvider.CoordType == enumCoordType.GeoCoord)
            {
                using (ShapePolygon spPrj = ShapePolygonPrjToGeo(feature.Geometry as ShapePolygon))
                {
                    if (spPrj != null)
                    {
                        using (IVectorAOIGenerator gen = new VectorAOIGenerator())
                        {
                            return gen.GetAOI(new ShapePolygon[] { spPrj }, rasterEnv, rasterSize);
                        }
                    }
                }
            }
            else if (!feature.Projected && rasterDrawing.DataProvider.CoordType == enumCoordType.PrjCoord)
            {
                using (ShapePolygon spPrj = ShapePolygonGeoToPrj(feature.Geometry as ShapePolygon))
                {
                    if (spPrj != null)
                    {
                        using (IVectorAOIGenerator gen = new VectorAOIGenerator())
                        {
                            return gen.GetAOI(new ShapePolygon[] { spPrj }, rasterEnv, rasterSize);
                        }
                    }
                }
            }
            else
            {
                using (IVectorAOIGenerator gen = new VectorAOIGenerator())
                {
                    return gen.GetAOI(new ShapePolygon[] { feature.Geometry as ShapePolygon }, rasterEnv, rasterSize);
                }
            }
            return null;
        }

        private ShapePolygon ShapePolygonPrjToGeo(ShapePolygon shapePolygon)
        {
            if (shapePolygon == null)
                return null;
            GeoDo.RSS.Core.DrawEngine.ICoordinateTransform tran = _canvas.CoordTransform;
            double prjX;
            double prjY;
            int ringLength = shapePolygon.Rings.Length;
            ShapeRing[] ring = new ShapeRing[ringLength];
            int potsLength = 0;
            for (int i = 0; i < ringLength; i++)
            {
                if (shapePolygon.Rings[i].Points == null)
                    continue;
                potsLength = shapePolygon.Rings[i].Points.Length;
                ShapePoint[] shpPoint = new ShapePoint[potsLength];
                for (int j = 0; j < shapePolygon.Rings[i].Points.Length; j++)
                {
                    tran.Prj2Geo(shapePolygon.Rings[i].Points[j].X, shapePolygon.Rings[i].Points[j].Y, out prjX, out prjY);
                    ShapePoint point = new ShapePoint(prjX, prjY);
                    shpPoint[j] = point;
                }
                ring[i] = new ShapeRing(shpPoint);
            }
            ShapePolygon prjSp = new ShapePolygon(ring);
            return prjSp;
        }

        private ShapePolygon ShapePolygonGeoToPrj(ShapePolygon shapePolygon)
        {
            if (shapePolygon == null || shapePolygon.IsProjected)
                return null;
            GeoDo.RSS.Core.DrawEngine.ICoordinateTransform tran = _canvas.CoordTransform;
            double prjX;
            double prjY;
            int ringLength = shapePolygon.Rings.Length;
            ShapeRing[] ring = new ShapeRing[ringLength];
            int potsLength = 0;
            for (int i = 0; i < ringLength; i++)
            {
                if (shapePolygon.Rings[i].Points == null)
                    continue;
                potsLength = shapePolygon.Rings[i].Points.Length;
                ShapePoint[] shpPoint = new ShapePoint[potsLength];
                for (int j = 0; j < shapePolygon.Rings[i].Points.Length; j++)
                {
                    tran.Geo2Prj(shapePolygon.Rings[i].Points[j].X, shapePolygon.Rings[i].Points[j].Y, out prjX, out prjY);
                    ShapePoint point = new ShapePoint(prjX, prjY);
                    shpPoint[j] = point;
                }
                ring[i] = new ShapeRing(shpPoint);
            }
            ShapePolygon prjSp = new ShapePolygon(ring);
            return prjSp;
        }

        private int[] GetIndexes(GeometryOfDrawed geo)
        {
            using (IVectorAOIGenerator gen = new VectorAOIGenerator())
            {
                return gen.GetAOI(geo.RasterPoints.Clone() as PointF[], geo.Types, GetSize());
            }
        }

        public System.Drawing.Rectangle GetRasterRect()
        {
            IRasterDrawing rst = _canvas.PrimaryDrawObject as IRasterDrawing;
            if (rst == null)
                return Rectangle.Empty;
            GeoDo.RSS.Core.DrawEngine.CoordEnvelope retRect = null;
            foreach (object obj in _aoiContainer.AOIs)
            {
                GeoDo.RSS.Core.DrawEngine.CoordEnvelope rect = null;
                if (obj is GeometryOfDrawed)
                    rect = GetRasterRect(obj as GeometryOfDrawed);
                else if (obj is Feature)
                    rect = GetRasterRect(obj as Feature);
                if (rect == null)
                    continue;
                if (retRect == null)
                    retRect = rect;
                else
                    retRect = retRect.Union(rect);
            }
            return new Rectangle((int)retRect.MinX, (int)retRect.MinY, (int)retRect.Width, (int)retRect.Height);
        }

        private Core.DrawEngine.CoordEnvelope GetRasterRect(Feature feature)
        {
            throw new NotImplementedException();
        }

        private Core.DrawEngine.CoordEnvelope GetRasterRect(GeometryOfDrawed geometryOfDrawed)
        {
            int bRow = int.MaxValue, eRow = int.MinValue, bCol = int.MaxValue, eCol = int.MinValue;
            for (int i = 0; i < geometryOfDrawed.RasterPoints.Length; i++)
            {
                if (geometryOfDrawed.RasterPoints[i].X < bCol)
                    bCol = (int)geometryOfDrawed.RasterPoints[i].X;
                if (geometryOfDrawed.RasterPoints[i].X > eCol)
                    eCol = (int)geometryOfDrawed.RasterPoints[i].X;
                if (geometryOfDrawed.RasterPoints[i].Y < bRow)
                    bRow = (int)geometryOfDrawed.RasterPoints[i].Y;
                if (geometryOfDrawed.RasterPoints[i].Y > eRow)
                    eRow = (int)geometryOfDrawed.RasterPoints[i].Y;
            }
            return new Core.DrawEngine.CoordEnvelope(bCol, bRow, (eCol - bCol), (eRow - bRow));
        }

        public AOIItem[] GetAOIItems()
        {
            IRasterDrawing rst = _canvas.PrimaryDrawObject as IRasterDrawing;
            if (rst == null)
                return null;
            List<AOIItem> aoiItems = new List<AOIItem>();
            foreach (object obj in _aoiContainer.AOIs)
            {
                string name = string.Empty;
                GeoDo.RSS.Core.DrawEngine.CoordEnvelope rect = null;
                if (obj is GeometryOfDrawed)
                    rect = GetGeoRect(obj as GeometryOfDrawed);
                else if (obj is Feature)
                {
                    rect = GetGeoRect(obj as Feature, out name);
                }
                if (rect == null)
                    continue;
                aoiItems.Add(new AOIItem(name, rect));
            }
            return aoiItems.Count > 0 ? aoiItems.ToArray() : null;
        }

        public GeoDo.RSS.Core.DrawEngine.CoordEnvelope GetPrjRect()
        {
            IRasterDrawing rst = _canvas.PrimaryDrawObject as IRasterDrawing;
            if (rst == null)
                return null;
            GeoDo.RSS.Core.DrawEngine.CoordEnvelope retRect = null;
            foreach (object obj in _aoiContainer.AOIs)
            {
                string name;
                GeoDo.RSS.Core.DrawEngine.CoordEnvelope rect = null;
                if (obj is GeometryOfDrawed)
                    rect = GetPrjRect(obj as GeometryOfDrawed);
                else if (obj is Feature)
                    rect = GetPrjRect(obj as Feature, out name);
                if (rect == null)
                    continue;
                if (retRect == null)
                    retRect = rect;
                else
                    retRect = retRect.Union(rect);
            }
            return retRect;
        }

        private Core.DrawEngine.CoordEnvelope GetPrjRect(Feature feature, out string name)
        {
            name = string.Empty;
            if (feature.Geometry == null)
                return null;
            Envelope evp = feature.Geometry.Envelope.Clone() as Envelope;
            if (evp == null)
                return null;
            name = TryGetName(feature);
            if (feature.Projected)
                return new GeoDo.RSS.Core.DrawEngine.CoordEnvelope(evp.MinX, evp.MaxX, evp.MinY, evp.MaxY);
            else
            {
                double prjX1, prjY1, prjX2, prjY2;
                _canvas.CoordTransform.Geo2Prj(evp.MinX, evp.MaxY, out prjX1, out prjY1);
                _canvas.CoordTransform.Geo2Prj(evp.MaxX, evp.MinY, out prjX2, out prjY2);
                return new GeoDo.RSS.Core.DrawEngine.CoordEnvelope(prjX1, prjX2, prjY2, prjY1);
            }
        }

        private Core.DrawEngine.CoordEnvelope GetPrjRect(GeometryOfDrawed geometryOfDrawed)
        {
            int bRow = int.MaxValue, eRow = int.MinValue, bCol = int.MaxValue, eCol = int.MinValue;
            for (int i = 0; i < geometryOfDrawed.RasterPoints.Length; i++)
            {
                if (geometryOfDrawed.RasterPoints[i].X < bCol)
                    bCol = (int)geometryOfDrawed.RasterPoints[i].X;
                if (geometryOfDrawed.RasterPoints[i].X > eCol)
                    eCol = (int)geometryOfDrawed.RasterPoints[i].X;
                if (geometryOfDrawed.RasterPoints[i].Y < bRow)
                    bRow = (int)geometryOfDrawed.RasterPoints[i].Y;
                if (geometryOfDrawed.RasterPoints[i].Y > eRow)
                    eRow = (int)geometryOfDrawed.RasterPoints[i].Y;
            }
            IRasterDrawing rst = _canvas.PrimaryDrawObject as IRasterDrawing;
            DataIdentify id = rst.DataProvider.DataIdentify;
            if (id != null && id.IsOrbit)
            {
                rst.DataProvider.OrbitProjectionTransformControl.Build();
                GeoDo.RSS.Core.DF.CoordEnvelope evp = rst.DataProvider.OrbitProjectionTransformControl.OrbitProjectionTransform.ComputeEnvelope(bRow, bCol, eRow, eCol);
                return new Core.DrawEngine.CoordEnvelope(evp.MinX, evp.MaxX, evp.MinY, evp.MaxY);
            }
            else
            {
                double prjX1, prjY1, prjX2, prjY2;
                _canvas.CoordTransform.Raster2Prj(bRow, bCol, out prjX1, out prjY1);
                _canvas.CoordTransform.Raster2Prj(eRow, eCol, out prjX2, out prjY2);
                return new GeoDo.RSS.Core.DrawEngine.CoordEnvelope(prjX1, prjX2, prjY2, prjY1);
            }
        }

        public GeoDo.RSS.Core.DrawEngine.CoordEnvelope GetGeoRect()
        {
            IRasterDrawing rst = _canvas.PrimaryDrawObject as IRasterDrawing;
            if (rst == null)
                return null;
            GeoDo.RSS.Core.DrawEngine.CoordEnvelope retRect = null;
            foreach (object obj in _aoiContainer.AOIs)
            {
                string name;
                GeoDo.RSS.Core.DrawEngine.CoordEnvelope rect = null;
                if (obj is GeometryOfDrawed)
                    rect = GetGeoRect(obj as GeometryOfDrawed);
                else if (obj is Feature)
                    rect = GetGeoRect(obj as Feature, out name);
                if (rect == null)
                    continue;
                if (retRect == null)
                    retRect = rect;
                else
                    retRect = retRect.Union(rect);
            }
            return retRect;
        }

        public GeoDo.RSS.Core.DrawEngine.CoordEnvelope GetMinGeoRect()
        {
            IRasterDrawing rst = _canvas.PrimaryDrawObject as IRasterDrawing;
            if (rst == null)
                return null;
            GeoDo.RSS.Core.DrawEngine.CoordEnvelope retRect = GetGeoRect();
            if (retRect == null)
                return null;
            GeoDo.RSS.Core.DrawEngine.CoordEnvelope envelope = null;
            GeoDo.RSS.Core.DrawEngine.ICoordinateTransform tran = _canvas.CoordTransform;
            if (rst.DataProvider.CoordType == enumCoordType.GeoCoord)
                envelope = new GeoDo.RSS.Core.DrawEngine.CoordEnvelope(rst.DataProvider.CoordEnvelope.MinX, rst.DataProvider.CoordEnvelope.MaxX,
                                                                       rst.DataProvider.CoordEnvelope.MinY, rst.DataProvider.CoordEnvelope.MaxY);
            else if (rst.DataProvider.CoordType == enumCoordType.PrjCoord)
            {
                double geoX1, geoY1, geoX2, geoY2;
                tran.Prj2Geo(rst.DataProvider.CoordEnvelope.MinX, rst.DataProvider.CoordEnvelope.MaxY, out geoX1, out geoY1);
                tran.Prj2Geo(rst.DataProvider.CoordEnvelope.MaxX, rst.DataProvider.CoordEnvelope.MinY, out geoX2, out geoY2);
                envelope = new GeoDo.RSS.Core.DrawEngine.CoordEnvelope(geoX1, geoX2, geoY2, geoY1);
            }
            double minX = envelope.MinX > retRect.MinX ? envelope.MinX : retRect.MinX;
            double maxX = envelope.MaxX < retRect.MaxX ? envelope.MaxX : retRect.MaxX;
            double minY = envelope.MinY > retRect.MinY ? envelope.MinY : retRect.MinY;
            double maxY = envelope.MaxY < retRect.MaxY ? envelope.MaxY : retRect.MaxY;
            return new Core.DrawEngine.CoordEnvelope(minX, maxX, minY, maxY);
        }

        private GeoDo.RSS.Core.DrawEngine.CoordEnvelope GetGeoRect(Feature feature, out string name)
        {
            name = string.Empty;
            if (feature.Geometry == null)
                return null;
            Envelope evp = feature.Geometry.Envelope.Clone() as Envelope;
            if (evp == null)
                return null;
            name = TryGetName(feature);
            if (!feature.Projected)
                return new GeoDo.RSS.Core.DrawEngine.CoordEnvelope(evp.MinX, evp.MaxX, evp.MinY, evp.MaxY);
            else
            {
                double geoX1, geoY1, geoX2, geoY2;
                _canvas.CoordTransform.Prj2Geo(evp.MinX, evp.MaxY, out geoX1, out geoY1);
                _canvas.CoordTransform.Prj2Geo(evp.MaxX, evp.MinY, out geoX2, out geoY2);
                return new GeoDo.RSS.Core.DrawEngine.CoordEnvelope(geoX1, geoX2, geoY2, geoY1);
            }
        }

        private string TryGetName(Feature feature)
        {
            string[] fields = feature.FieldNames;
            if (fields == null || fields.Length == 0)
                return null;
            string[] names = new string[] { "CHINESE_CH", "CH_NAME", "NAME", "名称", "分幅编号" };
            foreach (string fld in fields)
            {
                if (Array.IndexOf<string>(names, fld.ToUpper()) >= 0)
                {
                    return feature.GetFieldValue(fld);
                }
            }
            return string.Empty;
        }

        private GeoDo.RSS.Core.DrawEngine.CoordEnvelope GetGeoRect(GeometryOfDrawed geometryOfDrawed)
        {
            int bRow = int.MaxValue, eRow = int.MinValue, bCol = int.MaxValue, eCol = int.MinValue;
            for (int i = 0; i < geometryOfDrawed.RasterPoints.Length; i++)
            {
                if (geometryOfDrawed.RasterPoints[i].X < bCol)
                    bCol = (int)geometryOfDrawed.RasterPoints[i].X;
                if (geometryOfDrawed.RasterPoints[i].X > eCol)
                    eCol = (int)geometryOfDrawed.RasterPoints[i].X;
                if (geometryOfDrawed.RasterPoints[i].Y < bRow)
                    bRow = (int)geometryOfDrawed.RasterPoints[i].Y;
                if (geometryOfDrawed.RasterPoints[i].Y > eRow)
                    eRow = (int)geometryOfDrawed.RasterPoints[i].Y;
            }
            IRasterDrawing rst = _canvas.PrimaryDrawObject as IRasterDrawing;
            DataIdentify id = rst.DataProvider.DataIdentify;
            if (id != null && id.IsOrbit)
            {
                rst.DataProvider.OrbitProjectionTransformControl.Build();
                GeoDo.RSS.Core.DF.CoordEnvelope evp = rst.DataProvider.OrbitProjectionTransformControl.OrbitProjectionTransform.ComputeEnvelope(bRow, bCol, eRow, eCol);
                return new Core.DrawEngine.CoordEnvelope(evp.MinX, evp.MaxX, evp.MinY, evp.MaxY);
            }
            else
            {
                double geoX1, geoY1, geoX2, geoY2;
                _canvas.CoordTransform.Raster2Geo(bRow, bCol, out geoX1, out geoY1);
                _canvas.CoordTransform.Raster2Geo(eRow, eCol, out geoX2, out geoY2);
                return new GeoDo.RSS.Core.DrawEngine.CoordEnvelope(geoX1, geoX2, geoY2, geoY1);
            }
        }

        private Size GetSize()
        {
            if (_canvas.PrimaryDrawObject == null)
                return Size.Empty;
            return _canvas.PrimaryDrawObject.Size;
        }

        private Envelope GetEnvelope()
        {
            if (_canvas.PrimaryDrawObject == null)
                return null;
            GeoDo.RSS.Core.DrawEngine.CoordEnvelope evp = _canvas.PrimaryDrawObject.OriginalEnvelope.Clone();
            return new Envelope(evp.MinX, evp.MinY, evp.MaxX, evp.MaxY);
        }

        public void Dispose()
        {
            _canvas = null;
        }
    }
}
