using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DF
{
    public class HdrMapInfo
    {
        private string _name = "Geographic Lat/Lon";
        private Point _baseRowColNumber = Point.Empty;
        private HdrGeoPointCoord _baseMapCoordinateXY = new HdrGeoPointCoord();
        private HdrGeoPointCoord _xyResolution = new HdrGeoPointCoord();
        private string _coordinateType = "WGS-84";
        private string _units = "Degrees";

        public HdrMapInfo()
        {

        }

        public bool IsEmpty()
        {
            if (_baseMapCoordinateXY == null || _baseRowColNumber == null)
                return true;
            //if (_baseMapCoordinateXY.Longitude<double.Epsilon && _baseMapCoordinateXY.Latitude <double.Epsilon)//by luoke 不应当由此限制
            //    return true;
            return false;
        }

        public string Name
        {
            get { return _name; }
            set { if (!(string.IsNullOrEmpty(value))) _name = value; }
        }

        public Point BaseRowColNumber
        {
            get { return _baseRowColNumber; }
            set { _baseRowColNumber = value; }
        }

        public HdrGeoPointCoord BaseMapCoordinateXY
        {
            get { return _baseMapCoordinateXY; }
            set { _baseMapCoordinateXY = value; }
        }

        public HdrGeoPointCoord XYResolution
        {
            get { return _xyResolution; }
            set { _xyResolution = value; }
        }

        public string CoordinateType
        {
            get { return _coordinateType; }
            set { _coordinateType = value; }
        }

        public string Units
        {
            get { return _units; }
            set { _units = value; }
        }
    }
}
