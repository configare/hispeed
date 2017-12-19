using System;
using System.Collections.Generic;
using System.Text;

namespace GeoVis.GeoCore
{
    /// <summary>
    /// added by leaf in 2010.9.26
    /// </summary>
    public interface ITransformer
    {
        GeoCoord TransformPoints(GeoCoord pt);
        GeoRegion TransformPoints(GeoRegion region);
        void TransformPoints(GeoCoord[] pt);

        GeoCoord InvTransformPoints(GeoCoord pt);
        GeoRegion InvTransformPoints(GeoRegion region);
        void InvTransformPoints(GeoCoord[] pt); //逆变化
    }

}
