using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections;
using System.IO;

namespace GeoDo.RSS.MIF.Core
{
    public class BandnameRefTableParser
    {
        private XDocument _doc;

        public BandnameRefTableParser()
        {
            TryLoadFromFile();
            if (_doc == null)
            {
                TryLoadFromResource();
            }
        }

        private void TryLoadFromResource()
        {
            string resName = "GeoDo.RSS.MIF.Core.Extracting.ArgEnvironment.BandnameHelper.GeoDo.RSS.MIF.Core.Cnfg.xml";
            _doc = XDocument.Load(this.GetType().Assembly.GetManifestResourceStream(resName));
        }

        private void TryLoadFromFile()
        {
            string file = System.AppDomain.CurrentDomain.BaseDirectory + "SystemData\\GeoDo.RSS.MIF.Core.Cnfg.xml";
            if (File.Exists(file))
            {
                _doc = XDocument.Load(file);
            }
        }

        public BandnameRefTable[] Parse()
        {
            if (_doc == null)
                return null;
            XElement root = _doc.Root;
            IEnumerable<XElement> elements = root.Elements();
            if (elements == null || elements.Count() == 0)
                return null;
            List<BandnameRefTable> tables = new List<BandnameRefTable>();
            BandnameRefTable[] tableArray;
            foreach (XElement element in elements)
            {
                if (element.Name == "BandnameRefTable")
                {
                    tableArray = CreatBandnameRefTable(element);
                    if (tableArray != null || tableArray.Length == 0)
                        tables.AddRange(tableArray);
                }
            }
            return tables.Count > 0 ? tables.ToArray() : null;
        }

        //private BandnameRefTable CreatBandnameRefTable(XElement element)
        //{
        //    string satellite = element.Attribute("satellite").Value;
        //    string sensor = element.Attribute("sensor").Value;
        //    if (String.IsNullOrEmpty(satellite) || String.IsNullOrEmpty(sensor))
        //        return null;
        //    IEnumerable<XElement> eles = element.Elements();
        //    List<BandnameItem> items = new List<BandnameItem>();
        //    BandnameItem item;
        //    if (eles == null && eles.Count() == 0)
        //        return null;
        //    foreach (XElement ele in eles)
        //    {
        //        item = CreatBandnameItem(ele);
        //        if (item != null)
        //            items.Add(item);
        //    }
        //    if (items == null || items.Count == 0)
        //        return null;
        //    return new BandnameRefTable(satellite, sensor, items.ToArray());
        //}

        //新的，支持在一个节点中加入多个卫星传感器。
        private BandnameRefTable[] CreatBandnameRefTable(XElement element)
        {
            string satellites = element.Attribute("satellite").Value;
            string sensors = element.Attribute("sensor").Value;
            if (String.IsNullOrEmpty(satellites) || String.IsNullOrEmpty(sensors))
                return null;
            string[] satelliteSplit = satellites.Split(',');
            string[] sensorSplit = sensors.Split(',');

            IEnumerable<XElement> eles = element.Elements();
            List<BandnameItem> items = new List<BandnameItem>();
            BandnameItem item;
            if (eles == null && eles.Count() == 0)
                return null;
            foreach (XElement ele in eles)
            {
                item = CreatBandnameItem(ele);
                if (item != null)
                    items.Add(item);
            }
            if (items == null || items.Count == 0)
                return null;
            List<BandnameRefTable> tables = new List<BandnameRefTable>();
            foreach (string satellite in satelliteSplit)
            {
                if (string.IsNullOrWhiteSpace(satellite))
                    continue;
                foreach (string sensor in sensorSplit)
                {
                    if (string.IsNullOrWhiteSpace(sensor))
                        continue;
                    tables.Add(new BandnameRefTable(satellite, sensor, items.ToArray()));
                }
            }
            return tables.ToArray();
        }

        private BandnameItem CreatBandnameItem(XElement ele)
        {
            BandnameItem item = new BandnameItem();
            IEnumerable<XAttribute> atts = ele.Attributes();
            foreach (XAttribute att in atts)
            {
                switch (att.Name.ToString())
                {
                    case "index":
                        item.Index = int.Parse(ele.Attribute("index").Value);
                        break;
                    case "originalindex":
                        item.OriginalIndex = int.Parse(ele.Attribute("originalindex").Value);
                        break;
                    case "name":
                        if (!String.IsNullOrEmpty(ele.Attribute("name").Value))
                            item.Name = ele.Attribute("name").Value;
                        break;
                    case "resolution":
                        item.Resolution = AttributeToFloat(ele.Attribute("resolution").Value);
                        break;
                    case "centerwavenum":
                        item.CenterWaveNumber = AttributeToFloat(ele.Attribute("centerwavenum").Value);
                        break;
                    case "type":
                        item.Type = ele.Attribute("type").Value;
                        break;
                }
            }
            item.WaveLength = CreatWavelength(ele);
            return item;
        }

        private BandWaveLength CreatWavelength(XElement ele)
        {
            float min = AttributeToFloat(ele.Attribute("min").Value);
            float max = AttributeToFloat(ele.Attribute("max").Value);
            BandWaveLength wave;
            if (min != -1 && max != -1 && !String.IsNullOrEmpty(ele.Attribute("type").Value))
            {
                wave = new BandWaveLength(min, max);
                wave.WaveLengthType = ele.Attribute("type").Value;
                return wave;
            }
            return null;
        }

        private int AttributeToInt(string att)
        {
            int value;
            if (!String.IsNullOrEmpty(att))
            {
                bool ok = int.TryParse(att, out value);
                if (ok)
                    return value;
            }
            return -1;
        }

        private float AttributeToFloat(string att)
        {
            float value;
            if (!String.IsNullOrEmpty(att))
            {
                bool ok = float.TryParse(att, out value);
                if (ok)
                    return value;
            }
            return -1;
        }
    }
}
