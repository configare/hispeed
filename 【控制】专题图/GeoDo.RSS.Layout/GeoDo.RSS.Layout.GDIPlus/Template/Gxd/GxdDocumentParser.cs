using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.RSS.Layout.GDIPlus
{
    /*
<GxDocument version="1">
  <GxdLaytouTemplate name="" fullname="">
  </GxdLaytouTemplate>
  <GxdDataFrames>
    <GxdDataFrame name="" envelope="">
      <GxdRasterItems>
        <GxdRasterItem text="" filename="">
          <RasterItemArgument></RasterItemArgument>
        </GxdRasterItem>
      </GxdRasterItems>
      <GxdVectorHost>
      </GxdVectorHost>
    </GxdDataFrame>
  </GxdDataFrames>
</GxDocument>
     */
    public class GxdDocumentParser
    {
        public IGxdDocument Parse(string fname)
        {
            XDocument doc = XDocument.Load(fname);
            GxdTemplateHost template = GetGxdTemplate(doc.Root.Element("GxdLaytouTemplate"));
            List<IGxdDataFrame> dataFrames = GetGxdDataFrames(doc.Root.Element("GxdDataFrames"));
            IGxdDocument gxdDoc = new GxdDocument(template,dataFrames);
            gxdDoc.FullPath = fname;
            return gxdDoc;
        }

        private List<IGxdDataFrame> GetGxdDataFrames(XElement xElement)
        {
            if (xElement == null)
                return null;
            var eles = xElement.Elements("GxdDataFrame");
            if (eles == null)
                return null;
            List<IGxdDataFrame> dfs = new List<IGxdDataFrame>();
            foreach (XElement ele in eles)
            {
                IGxdDataFrame df = GetGxdDataFrame(ele);
                if (df != null)
                    dfs.Add(df);
            }
            return dfs;
        }

        private IGxdDataFrame GetGxdDataFrame(XElement ele)
        {
            string name = GetStringAttribute(ele, "name");
            double minx = GetDoubleAttribute(ele, "minx");
            double maxx = GetDoubleAttribute(ele, "maxx");
            double miny = GetDoubleAttribute(ele, "miny");
            double maxy = GetDoubleAttribute(ele, "maxy");
            GxdEnvelope evp = null;
            if(!(double.IsNaN(minx) || double.IsNaN(maxx) || double.IsNaN(miny) || double.IsNaN(maxy)))
                evp = new GxdEnvelope(minx,maxx,miny,maxy);
            List<IGxdRasterItem> gxItems = null;
            if (ele.Element("GxdRasterItems") != null)
            {
                var eles = ele.Element("GxdRasterItems").Elements("GxdRasterItem");
               if (eles != null)
                {
                    gxItems = new List<IGxdRasterItem>();
                    foreach (XElement e in eles)
                    {
                        IGxdRasterItem item = GetRasterItem(e);
                        if (item != null)
                            gxItems.Add(item);
                    }
                }
            }
            string spatialRef = null;
            XElement prjEle = ele.Element("SpatialRef");
            if (prjEle != null)
            {
                spatialRef = prjEle.Value;
            }
            IGxdVectorHost vhost = GetGxdVectorHost(ele.Element("GxdVectorHost"));
            IGxdDataFrame df = new GxdDataFrame(name, evp, vhost, 
                spatialRef,
                ele.Element("GeoGrid"),
                ele.Element("DocumentableLayersHost"));
            if (gxItems != null)
                df.GxdRasterItems.AddRange(gxItems.ToArray());
            return df;
        }

        private double GetDoubleAttribute(XElement ele, string attName)
        {
            var e = ele.Attribute(attName);
            if (e == null)
                return 0;
            double d;
            if(double.TryParse(e.Value,out d))
                return d ;
            return double.NaN;
        }

        private GxdEnvelope GetGxdEnvelope(XElement ele, string attName)
        {
            var e = ele.Attribute(attName);
            if (e == null)
                return null;
            return GxdEnvelope.From(e.Value);
        }

        /*
         <GxdVectorHost>
         </GxdVectorHost>
         */
        private IGxdVectorHost GetGxdVectorHost(XElement xElement)
        {
            if (xElement == null)
                return null;
            if (xElement.Element("Map") != null)
            {
                GxdVectorHost vhost = new GxdVectorHost(xElement.Element("Map").ToString());
                return vhost;
            }
            return null;
        }

        /*
        <GxdRasterItem text="" filename="">
          <RasterItemArgument></RasterItemArgument>
        </GxdRasterItem>
         */
        private IGxdRasterItem GetRasterItem(XElement e)
        {
            string text = GetStringAttribute(e, "text");
            string filename = GetStringAttribute(e, "filename");
            if (string.IsNullOrEmpty(filename))
                return null;
            XElement ele = e.Element("RasterItemArgument");
            object arg = null;
            if (ele != null)
                arg = ele.Value;
            string fileargs = GetStringAttribute(e, "fileargs");
            string[] fileargArray = fileargs.Split(';');
            string colorTableName = GetStringElement(ele, "ColorTableName");
            return new GxdRasterItem(filename, arg, fileargArray, colorTableName);
        }

        private string[] GetStringArrayElement(XElement ele, string name)
        {
            if (ele == null || string.IsNullOrEmpty(name))
                return null;
            IEnumerable<XElement> attrs = ele.Elements(name);
            if (attrs == null || attrs.Count() == 0)
                return null;
            List<string> values = new List<string>();
            foreach (XElement attr in attrs)
            {
                values.Add(ele.Element(name).Value ?? string.Empty);
            }
            return values.ToArray();
        }

        private string GetStringElement(XElement ele, string attName)
        {
            if (ele == null || string.IsNullOrEmpty(attName))
                return string.Empty;
            if (ele.Element(attName) != null)
                return ele.Element(attName).Value ?? string.Empty;
            return string.Empty;
        }

        private GxdTemplateHost GetGxdTemplate(XElement ele)
        {
            if (ele == null)
                return null;
            string name = GetStringAttribute(ele, "name");
            string fullname = GetStringAttribute(ele, "fullname");
            string tempContent = null;
            if (ele.Element("Layout") != null)
                tempContent = ele.Element("Layout").ToString();
            ILayoutTemplate temp = LayoutTemplate.CreateFrom(tempContent);
            if (temp != null)
            {
                GxdTemplateHost host = new GxdTemplateHost(temp);
                return host;
            }
            return null;
        }

        private string GetStringAttribute(XElement ele, string attName)
        {
            if (ele == null || string.IsNullOrEmpty(attName))
                return string.Empty;
            if (ele.Attribute(attName) != null)
                return ele.Attribute(attName).Value ?? string.Empty;
            return string.Empty;
        }
    }
}
