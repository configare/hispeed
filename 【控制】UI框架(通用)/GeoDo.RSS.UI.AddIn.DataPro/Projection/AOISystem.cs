using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using CodeCell.AgileMap.Core;
using System.Drawing;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    /// <summary>
    /// 提取自AOIProvider
    /// 仅支持通过已定义坐标（投影坐标或者地理坐标）数据的AOI转换。
    /// </summary>
    public class AOISystem
    {
        private IRasterDataProvider _dataProvider;

        public AOISystem()
        { }

        public int[] GetIndexes()
        {
            return null;
        }

        public void SetProvider(IRasterDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            
        }

        private int[] GetIndexes(Feature feature)
        {
            //GeoDo.RSS.Core.DrawEngine.ICoordinateTransform tran = _canvas.CoordTransform;

            //GeoDo.RSS.Core.DF.CoordEnvelope coordEvp = _dataProvider.CoordEnvelope.Clone();
            //Envelope evp = null;
            //if (feature.Projected)//使用投影坐标矢量化
            //{
            //    if (_dataProvider.CoordType == enumCoordType.PrjCoord)
            //        evp = new Envelope(coordEvp.MinX, coordEvp.MinY, coordEvp.MaxX, coordEvp.MaxY);
            //    else if (_dataProvider.CoordType == enumCoordType.GeoCoord)
            //    {
            //        GeoDo.RSS.Core.DrawEngine.CoordEnvelope prjEvp = new Core.DrawEngine.CoordEnvelope(coordEvp.MinX, coordEvp.MaxX, coordEvp.MinY, coordEvp.MaxY);
            //        tran.Geo2Prj(prjEvp);
            //        evp = new Envelope(prjEvp.MinX, prjEvp.MinY, prjEvp.MaxX, prjEvp.MaxY);
            //    }
            //}
            //else//使用地理坐标栅格化
            //{
            //    if (_dataProvider.CoordType == enumCoordType.GeoCoord)
            //        evp = new Envelope(coordEvp.MinX, coordEvp.MinY, coordEvp.MaxX, coordEvp.MaxY);
            //    else if (_dataProvider.CoordType == enumCoordType.PrjCoord)
            //    {
            //        double geoX1, geoY1, geoX2, geoY2;
            //        tran.Prj2Geo(coordEvp.MinX, coordEvp.MaxY, out geoX1, out geoY1);
            //        tran.Prj2Geo(coordEvp.MaxX, coordEvp.MinY, out geoX2, out geoY2);
            //        evp = new Envelope(geoX1, geoY2, geoX2, geoY1);
            //    }
            //}

            //IProjectionTransform trans = ProjectionTransformFactory.GetProjectionTransform(_dataProvider.SpatialRef,);

            //Size size = new Size(_dataProvider.Width, _dataProvider.Height);
            //using (IVectorAOIGenerator gen = new VectorAOIGenerator())
            //{
            //    return gen.GetAOI(new ShapePolygon[] { feature.Geometry as ShapePolygon }, evp, size);
            //}
            return null;
        }
    }
}
