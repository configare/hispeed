using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.RSS.Layout.GDIPlus
{
    public class GxdDataFrame : GxdItem, IGxdDataFrame
    {
        protected string _name;
        protected GxdEnvelope _coordEnvelope;
        protected List<IGxdRasterItem> _rasterItems = new List<IGxdRasterItem>();
        protected IGxdVectorHost _vectorHost;
        protected string _spatialRef;
        protected XElement _gridXml;
        //其他可被文档化层的宿主层(例如:等值线)
        protected XElement _documentableLayersHostXml;

        public GxdDataFrame(string name, GxdEnvelope coordEnvelope,
            IGxdVectorHost vectorHost,
            string spatialRef,
            XElement gridXml,
            XElement documentableLayersHostXml)
        {
            _name = name;
            _coordEnvelope = coordEnvelope;
            _vectorHost = vectorHost;
            _spatialRef = spatialRef;
            _gridXml = gridXml;
            _documentableLayersHostXml = documentableLayersHostXml;
        }

        public string Name
        {
            get { return _name; }
        }

        public GxdEnvelope Envelope
        {
            get { return _coordEnvelope; }
            set { _coordEnvelope = value; }
        }

        public List<IGxdRasterItem> GxdRasterItems
        {
            get { return _rasterItems; }
        }

        public IGxdVectorHost GxdVectorHost
        {
            get { return _vectorHost; }
        }

        public string SpatialRef
        {
            get { return _spatialRef; }
            set { _spatialRef = value; }
        }

        public XElement GeoGridXml
        {
            get { return _gridXml; }
        }

        public XElement DocumentableLayersHostXml
        {
            get { return _documentableLayersHostXml; }
        }

        public override XElement ToXml()
        {
            XElement ele = new XElement("GxdDataFrame");
            ele.SetAttributeValue("name", _name ?? string.Empty);
            if (_coordEnvelope != null)
            {
                ele.SetAttributeValue("minx", _coordEnvelope.MinX);
                ele.SetAttributeValue("maxx", _coordEnvelope.MaxX);
                ele.SetAttributeValue("miny", _coordEnvelope.MinY);
                ele.SetAttributeValue("maxy", _coordEnvelope.MaxY);
            }
            ele.SetElementValue("SpatialRef", _spatialRef ?? string.Empty);
            if (_gridXml != null)
                ele.SetElementValue("GeoGrid", _gridXml.ToString());
            ele.SetElementValue("GxdRasterItems", GetGxdRasterItems());
            ele.SetElementValue("GxdVectorHost", _vectorHost != null ? _vectorHost.ToXml().Value : string.Empty);
            if (_documentableLayersHostXml != null)
                ele.Add(_documentableLayersHostXml);
            return ele;
        }

        private object GetGxdRasterItems()
        {
            if (_rasterItems == null || _rasterItems.Count == 0)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (IGxdRasterItem item in _rasterItems)
                sb.Append(item.ToXml().ToString());
            return sb.ToString();
        }
    }
}
