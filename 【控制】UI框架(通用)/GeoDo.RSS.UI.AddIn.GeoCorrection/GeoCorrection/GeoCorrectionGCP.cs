using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.UI.AddIn.GeoCorrection
{
    class GeoCorrectionGCP
    {
        private CoordPoint _imagePoint;
        private CoordPoint _baseDataPoint;

        public GeoCorrectionGCP()
        { }

        public GeoCorrectionGCP(CoordPoint imgPoint, CoordPoint basePoint)
        {
            _imagePoint = imgPoint;
            _baseDataPoint = basePoint;
        }

        public CoordPoint ImagePoint
        {
            get { return _imagePoint; }
            set { _imagePoint = value; }
        }

        public CoordPoint BasePoint
        {
            get { return _baseDataPoint; }
            set { _baseDataPoint = value; }
        }
    }
}
