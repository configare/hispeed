using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class NameMapItem
    {
        public string Name = null;
        public string WktName = null;
        public string EsriName = null;
        public string Proj4Name = null;
        public string EPSGName = null;
        public string GeoTiffName = null;

        public NameMapItem()
        {
        }

        public NameMapItem(string name, string esriName, string wktName, string proj4Name, string epsgName, string geoTiffName)
        {
            Name = name;
            EsriName = esriName;
            WktName = wktName;
            Proj4Name = proj4Name;
            EPSGName = epsgName;
            GeoTiffName = geoTiffName;
        }
    }
}
