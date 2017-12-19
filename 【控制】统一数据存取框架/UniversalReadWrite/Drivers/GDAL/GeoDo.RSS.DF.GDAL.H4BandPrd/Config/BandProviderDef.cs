using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.RSS.DF.GDAL.H4BandPrd
{
    internal class BandProviderDef
    {
        public string Name = null;
        public string Satellite = null;
        public string Sensor = null;
        public List<IdentifyAttDef> IdentifyAttDefs = new List<IdentifyAttDef>();
        public List<DefaultBandDatasetDef> DefaultBandDatasetDefs = new List<DefaultBandDatasetDef>();

        public BandProviderDef(string name,string satellite,string sensor, List<IdentifyAttDef> identifyAttDefs, List<DefaultBandDatasetDef> defaultBandDatasetDefs)
        {
            Name = name;
            Satellite = satellite;
            Sensor = sensor;
            IdentifyAttDefs.AddRange(identifyAttDefs);
            DefaultBandDatasetDefs.AddRange(defaultBandDatasetDefs);
        }

        public static BandProviderDef FromXElement(XElement element)
        {
            if (element == null)
                return null;
            var nameatt = element.Attribute(XName.Get("name"));
            var satelliteatt = element.Attribute("satellite").Value;
            return new BandProviderDef(nameatt != null ? nameatt.Value : string.Empty,
                satelliteatt, element.Attribute("sensor").Value,
                GetIentifyAtts(element), GetDefaultDatasets(element));
        }

        private static List<DefaultBandDatasetDef> GetDefaultDatasets(XElement element)
        {
            var eles = element.Element(XName.Get("DefaultBandDatasets"));
            if (eles == null )
                return null;
            List<DefaultBandDatasetDef> datasets = new List<DefaultBandDatasetDef>();
            foreach (XElement ele in eles.Elements())
            {
                DefaultBandDatasetDef id = DefaultBandDatasetDef.FromXElement(ele);
                if (id != null)
                    datasets.Add(id);
            }
            return datasets;
        }

        private static List<IdentifyAttDef> GetIentifyAtts(XElement element)
        {
            var eles = element.Element(XName.Get("IdentifyAtts"));
            if (eles == null )
                return null;
            List<IdentifyAttDef> identifies = new List<IdentifyAttDef>();
            foreach (XElement ele in eles.Elements())
            {
                IdentifyAttDef id = IdentifyAttDef.FromXElement(ele);
                if (id != null)
                    identifies.Add(id);
            }
            return identifies;
        }
    }
}
