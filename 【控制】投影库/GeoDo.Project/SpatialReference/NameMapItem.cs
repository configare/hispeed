using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Project
{
    [Serializable]
    public class NameMapItem:ICloneable
    {
        public string Name = null;
        public string WktName = null;
        public string EsriName = null;
        public string Proj4Name = null;
        public string EPSGName = null;
        public string ENVIName = null;
        public string GeoTiffName = null;

        public NameMapItem()
        {
        }

        public NameMapItem(string name, string esriName, string wktName, string proj4Name, string epsgName,string enviName, string geoTiffName)
        {
            Name = name;
            EsriName = esriName;
            WktName = wktName;
            Proj4Name = proj4Name;
            EPSGName = epsgName;
            ENVIName = enviName;
            GeoTiffName = geoTiffName;
        }

        public object Clone()
        {
            NameMapItem v = new NameMapItem();
            v.ENVIName = this.ENVIName;
            v.EPSGName = this.EPSGName;
            v.EsriName = this.EsriName;
            v.GeoTiffName = this.GeoTiffName;
            v.Name = this.Name;
            v.Proj4Name = this.Proj4Name;
            v.WktName = this.WktName;
            return v;
        }
    }
}
