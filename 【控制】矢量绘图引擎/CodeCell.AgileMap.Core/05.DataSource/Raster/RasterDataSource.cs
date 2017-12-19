using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace CodeCell.AgileMap.Core
{
    public class RasterDataSource:IRasterDataSource,IPersistable
    {
        private IRasterReader _reader = null;
        private string _url = null;

        public RasterDataSource(string url)
        {
            _url = url;
            _reader = RasterReaderFactory.GetRasterReader(url);
        }

        public string Url
        {
            get { return _url; }
        }

        public IRasterReader Reader
        {
            get { return _reader; }
        }

        public string Name
        {
            get { return _url; }
        }

        public Envelope GetFullEnvelope()
        {
            return _reader.GetFullEnvelope();
        }

        public enumCoordinateType GetCoordinateType()
        {
            return _reader.GetCoordinateType();
        }

        public ISpatialReference GetSpatialReference()
        {
            return _reader.GetSpatialReference();
        }

        public bool IsReady
        {
            get { return true; }
        }

        public void BeginRead()
        {
         }

        public void EndRead()
        {
            
        }

        public void Dispose()
        {
            _reader.Dispose();
            _reader = null;
        }

        public PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("DataSource");
            obj.AddAttribute("type", Path.GetFileName(this.GetType().Assembly.Location) + "," + this.GetType().ToString());
            obj.AddAttribute("name", Name != null ? Name : string.Empty);
            obj.AddAttribute("url", _url != null ? Map.GetRelativeFilename(_url) : string.Empty);
            return obj;
        }

        public static IRasterDataSource FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            string name = null;
            if (ele.Attribute("name") != null)
                name = ele.Attribute("name").Value;
            string url = ele.Attribute("url").Value;
            string fname = MapFactory.GetFullFilename(url);
            IRasterDataSource ds = new RasterDataSource(fname);
            return ds;
        }

    }
}
