using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace GeoDo.Project
{
    /*
     * Angular Unit: Degree (0.017453292519943299)
       Prime Meridian: Greenwich (0.000000000000000000)
       Datum: D_WGS_1984
            Spheroid: WGS_1984
                Semimajor Axis: 6378137.000000000000000000
                Semiminor Axis: 6356752.314245179300000000
                Inverse Flattening: 298.257223563000030000
     */
    [Serializable]
    public class GeographicCoordSystem : IGeographicCoordSystem, ISpatialRefFormat, ICloneable
    {
        private string _name = "GCS_WGS_1984";
        private AngularUnit _angularUnit = new AngularUnit("Degree", 0.017453292519943299d);
        private PrimeMeridian _primeMeridian = new PrimeMeridian("Greenwich", 0d);
        private Datum _datum = new Datum();
        private int _oracleSpatialSRID = 0;

        public GeographicCoordSystem()//WGS-84
        {
            _oracleSpatialSRID = 4326;
        }

        public GeographicCoordSystem(WktItem wktItem)
        {
            _name = wktItem.Value.Split(',')[0].Replace("\"", string.Empty);
            _angularUnit = new AngularUnit(wktItem.GetWktItem("UNIT"));
            _primeMeridian = new PrimeMeridian(wktItem.GetWktItem("PRIMEM"));
            _datum = new Datum(wktItem.GetWktItem("DATUM"));
            _oracleSpatialSRID = int.MinValue;
        }

        public GeographicCoordSystem(string name, AngularUnit angularUnit, PrimeMeridian primeMeridian, Datum datum)
        {
            _name = name;
            _angularUnit = angularUnit;
            _primeMeridian = primeMeridian;
            _datum = datum;
            _oracleSpatialSRID = int.MinValue;
        }

        public int ToOracleSpatialSRID()
        {
            return _oracleSpatialSRID;
        }

        #region IGeographicsCoordSystem Members

        [DisplayName("名称")]
        public string Name
        {
            get { return _name; }
        }

        [DisplayName("单位")]
        public AngularUnit AngularUnit
        {
            get { return _angularUnit; }
        }

        [DisplayName("中央经线")]
        public PrimeMeridian PrimeMeridian
        {
            get { return _primeMeridian; }
        }

        [DisplayName("椭球体")]
        public Datum Datum
        {
            get { return _datum; }
        }

        public bool IsSame(IGeographicCoordSystem geoCoordSys)
        {
            return _angularUnit != null &&
                      _primeMeridian != null &&
                      _datum != null &&
                      _angularUnit.IsSame(geoCoordSys.AngularUnit) &&
                      _primeMeridian.IsSame(geoCoordSys.PrimeMeridian) &&
                      _datum.IsSame(geoCoordSys.Datum);
        }

        public string ToProj4String()
        {
            string proj4 = "+proj=latlong +datum={0} +a={1} +b={2} +f={3} +nodefs";
            return string.Format(proj4,
                                           ToProj4Datum(_datum),
                                           _datum.Spheroid.SemimajorAxis,
                                           _datum.Spheroid.SemiminorAxis,
                                           1 / _datum.Spheroid.InverseFlattening);
        }

        //作为投影坐标的一部分
        public string ToProj4ArgsString()
        {
            string proj4 = "+datum={0} +a={1} +b={2} +f={3} +nodefs";
            return string.Format(proj4,
                                           ToProj4Datum(_datum),
                                           _datum.Spheroid.SemimajorAxis,
                                           _datum.Spheroid.SemiminorAxis,
                                           1 / _datum.Spheroid.InverseFlattening);
        }

        private string ToProj4Datum(Datum datum)
        {
            using (OGCWkt2Proj4Mapper dm = new OGCWkt2Proj4Mapper())
            {
                return dm.ToProj4Datum(datum);
            }
        }

        #endregion

        /*
         * GEOGCS[  "GCS_WGS_1984",
                           DATUM[ "D_WGS_1984",
                                        SPHEROID["WGS_1984",6378137.0,298.257223563]
                                      ],
                           PRIMEM["Greenwich",0.0],
                           UNIT["Degree",0.0174532925199433]
                       ]
         */

        public override string ToString()
        {
            return "GEOGCS[\"" + _name + "\"," +
                      _datum.ToString() + "," +
                      _primeMeridian.ToString() + "," +
                      _angularUnit.ToString() + "]";
        }

        #region ISpatialRefFormat Members

        public string FormatToString()
        {
            return "GEOGCS[\"" + _name + "\",\n" +
                      "               " + (_datum as ISpatialRefFormat).FormatToString() + ",\n" +
                      "               " + _primeMeridian.ToString() + ",\n" +
                      "               " + _angularUnit.ToString() + "]";
        }

        #endregion

        public object Clone()
        {
            /*
             *  private string _name = "GCS_WGS_1984";
                private AngularUnit _angularUnit = new AngularUnit("Degree", 0.017453292519943299d);
                private PrimeMeridian _primeMeridian = new PrimeMeridian("Greenwich", 0d);
                private Datum _datum = new Datum();
                private int _oracleSpatialSRID = 0;
             */
            GeographicCoordSystem geoSystem = new GeographicCoordSystem();
            geoSystem._name = _name;
            geoSystem._oracleSpatialSRID = _oracleSpatialSRID;
            if (_angularUnit != null)
                geoSystem._angularUnit = _angularUnit.Clone() as AngularUnit;
            if (_primeMeridian != null)
                geoSystem._primeMeridian = _primeMeridian.Clone() as PrimeMeridian;
            if (_datum != null)
                geoSystem._datum = _datum.Clone() as Datum;
            return geoSystem;
        }
    }

}
