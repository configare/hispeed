using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.GDAL.HDF5GEO
{
    public class L2ProductDefind
    {
        public string Name;
        public string Desc;
        public string Identify;
        public string Product;
        public GeoInfos GeoInfo;
        public ProInfos ProInfo;

        public L2ProductDefind()
        {

        }
    }

    public class GeoInfos
    {
        public string Proj4Str;
        public GeoAtrributes GeoAtrrs;
        public GeoDefs GeoDef;

        public GeoInfos(string proj4Str)
        {
            Proj4Str = proj4Str;
        }

        public GeoInfos(string proj4Str, GeoAtrributes geoAtrrs) :
            this(proj4Str)
        {
            GeoAtrrs = geoAtrrs;
        }

        public GeoInfos(string proj4Str, GeoDefs geoDef) :
            this(proj4Str)
        {
            GeoDef = geoDef;
        }
    }

    public class GeoAtrributes
    {
        public enumGeoAttType Location = enumGeoAttType.File;
        public string GeoDataset;
        public string LeftTopLonAtrrName;
        public string LeftTopLatAtrrName;
        public string RightBottomLonAtrrName;
        public string RightBottomLatAtrrName;
        public string Unit;

        public GeoAtrributes()
        {
        }

        public GeoAtrributes(enumGeoAttType location, string geoDatset)
        {
            GeoDataset = geoDatset;
            Location = location;
        }

        public GeoAtrributes(enumGeoAttType location, string geoDatset, string leftTopLonAtrrName, string leftTopLatAtrrName, string rightBottomLonAtrrName, string rightBottomLatAtrrName, string unit) :
            this(location, geoDatset)
        {
            LeftTopLonAtrrName = leftTopLonAtrrName;
            LeftTopLatAtrrName = leftTopLatAtrrName;
            RightBottomLonAtrrName = rightBottomLonAtrrName;
            RightBottomLatAtrrName = rightBottomLatAtrrName;
            Unit = unit;
        }
    }

    public class GeoDefs
    {
        public double LeftTopLon;
        public double LeftTopLat;
        public double RightBottomLon;
        public double RightBottomLat;

        public GeoDefs()
        {
        }

        public GeoDefs(double leftTopLon, double leftTopLat, double rightBottomLon, double rightBottomLat)
        {
            LeftTopLon = leftTopLon;
            LeftTopLat = leftTopLat;
            RightBottomLon = rightBottomLon;
            RightBottomLat = rightBottomLat;
        }

    }

    public class ProInfos
    {
        public string ProDatasets;

        public ProInfos()
        {
        }

        public ProInfos(string proDataSets)
        {
            ProDatasets = proDataSets;
        }
    }

    public enum enumGeoAttType
    {
        File,
        Dataset
    }
}
