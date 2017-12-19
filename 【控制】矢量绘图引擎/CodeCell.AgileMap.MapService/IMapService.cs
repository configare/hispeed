using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Drawing;

namespace CodeCell.AgileMap.MapService
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    public interface IMapService
    {
        [OperationContract]
        MapImage GetMapImage(double x,
            double y,
            double width,
            double height,
            int targetWidth,
            int targetHeight,string[] invisibleLayerIds);

        [OperationContract]
        MapImage GetMapImageByQuadkey(string quadkey,
            double x,
            double y,
            double width,
            double height,
            int targetWidth,
            int targetHeight);

        [OperationContract]
        string GeoEnvelope2PrjEnvelope(double minX, double maxX, double minY, double maxY);

        [OperationContract]
        PointF[] Geo2Prj(PointF[] geoPts);

        [OperationContract]
        PointF[] Prj2Geo(PointF[] prjPts);

        [OperationContract]
        MapInfo GetMapInfo();

        [OperationContract]
        LayerInfo[] GetLayerInfos();

        [OperationContract]
        FeatureInfo[] Query(string layerId,string geometry, string keywords);

        [OperationContract]
        FeatureInfo[] Identify(string[] layerIds, PointF prjPt, double tolerance);
    }
}
