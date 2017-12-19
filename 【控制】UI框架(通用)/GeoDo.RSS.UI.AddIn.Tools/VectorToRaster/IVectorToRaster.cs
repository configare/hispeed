using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    public interface IVectorToRaster
    {
        void ProcessVectorToRaster(string shpFileName, string shpPrimaryField, enumDataType dataType, double resolution, string rasterFileName);

        void ProcessVectorToRaster(Feature[] features, string shpPrimaryField, enumDataType dataType, double resolution, CoordEnvelope envelope, string rasterFileName);

        void ProcessVectorToRaster(Feature[] features, string shpPrimaryField, IRasterDataProvider rasterProvider);
    }
}
