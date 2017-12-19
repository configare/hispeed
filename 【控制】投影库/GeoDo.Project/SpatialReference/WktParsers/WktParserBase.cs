using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Project
{
    internal abstract class WktParserBase:ISpatialReferenceStringParser
    {
        private WktItem _rootWktItem = null;
        private const string cstPrjCoordSystem = "PROJCS";
        private const string cstGeoCoordSystem = "GEOGCS";
        protected enumWKTSource _wktSource = enumWKTSource.Unknow;

        public WktParserBase(enumWKTSource source)
        {
            _wktSource = source;
        }

        #region ISpatialReferenceStringParser Members

        public ISpatialReference Parse(string wkt)
        {
            _rootWktItem = new WktSpliter().Split(wkt);  //2ms
            if (_rootWktItem == null)
                return null;
            bool isPrj = IsProjectionCoordSystem();
            bool isGeo = IsGeographicCoordSystem();
            if (!isPrj && !isGeo)
            {
                throw new Exception("本系统只支持地理坐标系统与投影坐标系统,坐标系统\"" + _rootWktItem.Name + "\"不受支持。");
            }
            IGeographicCoordSystem _geoCoordSystem = null; 
            IProjectionCoordSystem _prjCoordSystem = null;
            _geoCoordSystem = ParseGeoCoordSystem(_rootWktItem.GetWktItem(cstGeoCoordSystem)); 
            _prjCoordSystem = ParseProjectionCoordSystem(_rootWktItem.GetWktItem(cstPrjCoordSystem));  //8ms
            return new SpatialReference(_geoCoordSystem, _prjCoordSystem);
        }

        protected abstract IProjectionCoordSystem ParseProjectionCoordSystem(WktItem wktItem);

        protected IGeographicCoordSystem ParseGeoCoordSystem(WktItem wktItem)
        {
            return new GeographicCoordSystem(wktItem);
        }

        #endregion

        protected bool IsProjectionCoordSystem()
        {
            return _rootWktItem.Name.ToUpper() == cstPrjCoordSystem;
        }

        protected bool IsGeographicCoordSystem()
        {
            return _rootWktItem.Name.ToUpper() == cstGeoCoordSystem;
        }
    }
}
