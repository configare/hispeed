using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    internal class OGCWkt2Proj4Mapper:IDisposable
    {
        private Dictionary<string, string> _datums = null;
        private List<NameMapItem> _prjNameMapItems = null;
        private List<NameMapItem> _prjParameterItems = null;

        public OGCWkt2Proj4Mapper()
        {
            InitDatums();
            InitPrjs();
            InitPrjParameters();
        }

        private void InitPrjParameters()
        {
            _prjParameterItems = new List<NameMapItem>();
            _prjParameterItems.Add(new NameMapItem("Latitude of first standard parallel", "Standard_Parallel_1", "standard_parallel_1", "lat_1", "1", "StdParallel2"));
            _prjParameterItems.Add(new NameMapItem("Latitude of second standard parallel", "Standard_Parallel_2", "standard_parallel_2", "lat_2", "2", "StdParallel2"));
            _prjParameterItems.Add(new NameMapItem("Latitude of natural origin", "Latitude_Of_Origin", "latitude_of_origin", "lat_0", "1", "NatOriginLat")); 
            _prjParameterItems.Add(new NameMapItem("Longitude of natural origin", "Central_Meridian", "central_meridian", "lon_0", null, null));
            _prjParameterItems.Add(new NameMapItem("False Easting", "False_Easting", "false_easting", "x_0", "6", "FalseEasting"));
            _prjParameterItems.Add(new NameMapItem("False Northing", "False_Northing", "false_northing", "y_0", "7", "FalseNorthing "));
            _prjParameterItems.Add(new NameMapItem("Scale factor at natural origin", null, "scale_factor", "k_0", "5", "ScaleAtNatOrigin ")); //new:K_0 old:k
        }

        private void InitPrjs()
        {
            _prjNameMapItems = new List<NameMapItem>();
                                                                                //Name         EsriName    WKT                  Proj4     EGSP   GeoTiff
            _prjNameMapItems.Add(new NameMapItem("Mercator", "Mercator", "Mercator_2SP", "merc", "9805", null));
            _prjNameMapItems.Add(new NameMapItem("Transverse Mercator", "Transverse_Mercator", "Transverse_Mercator", "tmerc", "9807", "CT_TransverseMercator (1)"));
            _prjNameMapItems.Add(new NameMapItem("Albers Equal-Area Conic", "Albers", "Albers_Conic_Equal_Area ", "aea", "9822", "CT_AlbersEqualArea (11)"));
            _prjNameMapItems.Add(new NameMapItem("Gauss-Kruger", "Gauss_Kruger", "Transverse_Mercator", "tmerc", "9807", "CT_TransverseMercator (1)"));
        }

        private void InitDatums()
        {
            _datums = new Dictionary<string, string>();
            _datums.Add("D_WGS_1984", "WGS84");
        }

        public NameMapItem GetParameterFromEsriName(string esriName)
        {
            foreach (NameMapItem it in _prjParameterItems)
            {
                if (it.EsriName.ToUpper() == esriName.ToUpper())
                    return it;
            }
            return null;
        }

        public NameMapItem GetParameterFromWKTName(string wktName)
        {
            foreach (NameMapItem it in _prjParameterItems)
            {
                if (it.WktName.ToUpper() == wktName.ToUpper())
                    return it;
            }
            return null;
        }

        public NameMapItem GetPrjNameFromEsriName(string esriName)
        {
            foreach (NameMapItem it in _prjNameMapItems)
            {
                if (it.EsriName.ToUpper() == esriName.ToUpper())
                    return it;
            }
            return null;
        }

        public NameMapItem GetPrjNameFromWKTName(string wktName)
        {
            foreach (NameMapItem it in _prjNameMapItems)
            {
                if (it.WktName.ToUpper() == wktName.ToUpper())
                    return it;
            }
            return null;
        }

        public  string ToProj4Datum(Datum datum)
        {
            foreach (string wkt in _datums.Keys)
            {
                if (wkt.ToUpper() == datum.Name.ToUpper())
                    return _datums[wkt];
            }
            return "WGS84";
        }

        #region IDisposable Members

        public void Dispose()
        {
            _datums.Clear();
            _datums = null;
            _prjNameMapItems.Clear();
            _prjNameMapItems = null;
            _prjParameterItems.Clear();
            _prjParameterItems = null;
        }

        #endregion
    }
}
