using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class SpatialReference:ISpatialReference,ISpatialRefFormat
    {
        private string _name = null;
        private IGeographicCoordSystem _geoCoordSystem = null;
        private IProjectionCoordSystem _prjCoordSystem = null;
        private CoordinateDomain _coordinateDomain = null;
        private int _oracleSpatialSRID = 4326;//WGS-84
        public const int WGS84SRID = 4326;

        public SpatialReference(IGeographicCoordSystem geoCoordSystem)
        {
            _geoCoordSystem = geoCoordSystem;
            _name = _geoCoordSystem.Name;
            _oracleSpatialSRID = geoCoordSystem.ToOracleSpatialSRID();
        }

        public SpatialReference(IGeographicCoordSystem geoCoordSystem, IProjectionCoordSystem prjCoordSystem)
            :this(geoCoordSystem)
        {
            _prjCoordSystem = prjCoordSystem;
            if (_prjCoordSystem != null)
            {
                _name = _prjCoordSystem.NameDes;
                _oracleSpatialSRID = _prjCoordSystem.ToOracleSpatialSRID();
            }
        }

        public static ISpatialReference GetDefault()
        {
            return new SpatialReference(new GeographicCoordSystem());
        }

        #region ISpatialReference Members

        [Browsable(false)]
        public int OracleSpatialSRID
        {
            get { return _oracleSpatialSRID; }
        }

        [DisplayName("名称")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [DisplayName("值域"),Browsable(false)]
        public CoordinateDomain CoordinateDomain
        {
            get { return _coordinateDomain; }
            set { _coordinateDomain = value; }
        }

        [DisplayName("地理坐标系统"), TypeConverter(typeof(ExpandableObjectConverter))]
        public IGeographicCoordSystem GeographicsCoordSystem
        {
            get { return _geoCoordSystem; }
        }

        [DisplayName("投影坐标系统"), TypeConverter(typeof(ExpandableObjectConverter))]
        public IProjectionCoordSystem ProjectionCoordSystem
        {
            get { return _prjCoordSystem; }
        }

        public string ToProj4String()
        {
            if (_prjCoordSystem == null)
                return _geoCoordSystem.ToProj4String();
            else
                return _prjCoordSystem.ToProj4String() +_geoCoordSystem.ToProj4ArgsString();
        }

        public bool IsSame(ISpatialReference spatialRef)
        {
            if (spatialRef == null)
                return false;
            if (_prjCoordSystem != null && spatialRef.ProjectionCoordSystem == null)
                return false;
            return _geoCoordSystem.IsSame(spatialRef.GeographicsCoordSystem) &&
                      _prjCoordSystem.IsSame(spatialRef.ProjectionCoordSystem);
        }

        #endregion

        /*
         * PROJCS["World_Mercator",
                        GEOGCS["GCS_WGS_1984",
                               DATUM["D_WGS_1984",
                                     SPHEROID["WGS_1984",6378137.0,298.257223563]
                                    ],
                               PRIMEM["Greenwich",0.0],
                               UNIT["Degree",0.0174532925199433]
                              ],
                        PROJECTION["Mercator"],
                        PARAMETER["False_Easting",0.0],
                        PARAMETER["False_Northing",0.0],
                        PARAMETER["Central_Meridian",0.0],
                        PARAMETER["Standard_Parallel_1",0.0],
                        UNIT["Meter",1.0]
                      ]
         */
        public override string ToString()
        {
            if (_prjCoordSystem == null)
                return _geoCoordSystem.ToString();
            else
            {
                return "PROJCS[\"" + _name + "\"," + _geoCoordSystem.ToString() + "," + _prjCoordSystem.ToString() + "]";
            }
        }

        public string ToWKTString()
        {
            return ToString();
        }

        #region ISpatialRefFormat Members

        public string FormatToString()
        {
            if (_prjCoordSystem == null)
                return (_geoCoordSystem as ISpatialRefFormat).FormatToString();
            else
            {
                return "PROJCS[\"" + _name + "\",\n" +
                          "        "+(_geoCoordSystem as ISpatialRefFormat).FormatToString() + ",\n" + 
                          (_prjCoordSystem as ISpatialRefFormat).FormatToString() +"\n"+
                          "      ]";
            }
        }

        #endregion
    }
}
