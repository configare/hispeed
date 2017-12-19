using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace GeoDo.Project
{
    [Serializable]
    public class SpatialReference : ISpatialReference, ISpatialRefFormat, ICloneable
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
            : this(geoCoordSystem)
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

        [DisplayName("值域"), Browsable(false)]
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
            if (_prjCoordSystem != null && _prjCoordSystem.Name.EsriName == "MODIS Sinusoidal")
                return "+proj=sinu +lon_0=0 +x_0=0 +y_0=0 +a=6371007.181 +b=6371007.181 +units=m +no_defs";

            else 
            if (_prjCoordSystem == null)
                return _geoCoordSystem.ToProj4String();
            else
                return _prjCoordSystem.ToProj4String() + _geoCoordSystem.ToProj4ArgsString();
        }

        public string ToEnviProjectionInfoString()
        {
            if (_prjCoordSystem == null)
            {
                return "projection info = {1}";
            }
            EnviPrjInfoArgDef enviDef = null;
            using (IPrjStdsMapTableParser map = new PrjStdsMapTableParser())
                enviDef = map.GetEnviPrjInfoArgDef(int.Parse(_prjCoordSystem.Name.ENVIName));
            if (enviDef == null)
                throw new ArgumentException("ENVI空间参考参数格式设置错误!");
            StringBuilder sb = new StringBuilder();
            string[] paras = enviDef.Args;
            foreach (string para in paras)
            {
                NameValuePair v = _prjCoordSystem.GetParaByName(para);
                if (v == null)
                    sb.Append(GetEnviParaDefaultValue(para).ToString() + ",");
                else
                    sb.Append(v.Value + ",");
            }
            Datum datum = _geoCoordSystem.Datum != null ? _geoCoordSystem.Datum : new Datum();
            return "projection info = {" + _prjCoordSystem.Name.ENVIName + "," + sb.ToString() + datum.Name + "," + (Name ?? string.Empty) + ",Meters}";
        }

        public void ToEnviProjectionInfoString(out float[] argValues, out string units)
        {
            EnviPrjInfoArgDef enviDef = null;
            using (IPrjStdsMapTableParser map = new PrjStdsMapTableParser())
                enviDef = map.GetEnviPrjInfoArgDef(int.Parse(_prjCoordSystem.Name.ENVIName));
            if (enviDef == null)
                throw new ArgumentException("ENVI空间参考参数格式设置错误!");
            StringBuilder sb = new StringBuilder();
            string[] paras = enviDef.Args;
            List<float> retargValues = new List<float>();
            foreach (string para in paras)
            {
                NameValuePair v = _prjCoordSystem.GetParaByName(para);
                if (v == null)
                    retargValues.Add((float)GetEnviParaDefaultValue(para));
                else
                    retargValues.Add((float)v.Value);
            }
            argValues = retargValues.ToArray();
            units = "Meters";
        }

        private double GetEnviParaDefaultValue(string para)
        {
            switch (para)
            {
                case "a":
                    if (_geoCoordSystem != null)
                        return _geoCoordSystem.Datum.Spheroid.SemimajorAxis;
                    return (new Datum()).Spheroid.SemimajorAxis;
                case "b":
                case "r":
                    if (_geoCoordSystem != null)
                        return _geoCoordSystem.Datum.Spheroid.SemiminorAxis;
                    return (new Datum()).Spheroid.SemiminorAxis;
                case "lat0":
                case "lon0":
                case "x0":
                case "y0":
                case "sp1":
                case "sp2":
                    return 0d;
                case "k0":
                    return 1d;
                default:
                    return 0d;
            }
        }

        public bool IsSame(ISpatialReference spatialRef)
        {
            if (spatialRef == null)
                return false;
            if (_prjCoordSystem != null && spatialRef.ProjectionCoordSystem == null)
                return false;
            if (_geoCoordSystem != null && spatialRef.GeographicsCoordSystem == null)
                return false;
            if (_geoCoordSystem == null && spatialRef.GeographicsCoordSystem == null &&
                _prjCoordSystem == null && spatialRef.ProjectionCoordSystem == null)
                return false;
            if (_prjCoordSystem == null && spatialRef.ProjectionCoordSystem != null)
                return false;
            if (_prjCoordSystem == null && spatialRef.ProjectionCoordSystem == null)
                return _geoCoordSystem.IsSame(spatialRef.GeographicsCoordSystem);
            else
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
                          "        " + (_geoCoordSystem as ISpatialRefFormat).FormatToString() + ",\n" +
                          (_prjCoordSystem as ISpatialRefFormat).FormatToString() + "\n" +
                          "      ]";
            }
        }

        #endregion

        public static ISpatialReference FromPrjFile(string prjFile)
        {
            return SpatialReferenceFactory.GetSpatialReferenceByPrjFile(prjFile);
        }

        public static ISpatialReference FromWkt(string wktstring, enumWKTSource wktSource)
        {
            return SpatialReferenceFactory.GetSpatialReferenceByWKT(wktstring, wktSource);
        }

        public static ISpatialReference FromProj4(string prj4string)
        {
            return SpatialReferenceFactory.GetSpatialReferenceByProj4String(prj4string);
        }

        public object Clone()
        {
            SpatialReference spRef = null;
            if (_prjCoordSystem == null)
                spRef = new SpatialReference(_geoCoordSystem.Clone() as IGeographicCoordSystem);
            else
                spRef = new SpatialReference(_geoCoordSystem.Clone() as IGeographicCoordSystem, _prjCoordSystem.Clone() as IProjectionCoordSystem);
            spRef.Name = _name;
            if (_coordinateDomain != null)
                spRef.CoordinateDomain = _coordinateDomain.Clone() as CoordinateDomain;
            spRef._oracleSpatialSRID = _oracleSpatialSRID;
            return spRef;
        }
    }
}
