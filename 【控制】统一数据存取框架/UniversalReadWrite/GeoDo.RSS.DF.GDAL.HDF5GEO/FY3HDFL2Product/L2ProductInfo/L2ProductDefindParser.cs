using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.IO;

namespace GeoDo.RSS.DF.GDAL.HDF5GEO
{
    public class L2ProductDefindParser
    {
        private static string CnfgFile = "";

        public static L2ProductDefind GetL2ProductFirstDef(string l2ProFilename)
        {
            CnfgFile = AppDomain.CurrentDomain.BaseDirectory + "\\GeoDo.RSS.DF.L2Pro.Defind.xml";
            string regStr;
            Regex reg = null;
            XDocument doc = XDocument.Load(CnfgFile);
            XElement root = doc.Root;
            var elements = root.Elements(XName.Get("L2Product"));
            foreach (var element in elements)
            {
                if (!string.IsNullOrEmpty(element.Attribute("identify").Value))
                {
                    regStr = element.Attribute("identify").Value;
                    reg = new Regex(regStr);
                    if (reg.IsMatch(Path.GetFileName(l2ProFilename.ToUpper())))
                    {
                        return CrateL2ProductDef(element);
                    }
                }
            }
            return null;
        }

        public static L2ProductDefind[] GetL2ProductDefs(string l2ProFilename)
        {
            CnfgFile = AppDomain.CurrentDomain.BaseDirectory + "\\GeoDo.RSS.DF.L2Pro.Defind.xml";
            string regStr;
            Regex reg = null;
            XDocument doc = XDocument.Load(CnfgFile);
            XElement root = doc.Root;
            var elements = root.Elements(XName.Get("L2Product"));
            List<L2ProductDefind> result = new List<L2ProductDefind>();
            foreach (var element in elements)
            {
                if (!string.IsNullOrEmpty(element.Attribute("identify").Value))
                {
                    regStr = element.Attribute("identify").Value.ToUpper();
                    reg = new Regex(regStr);
                    if (reg.IsMatch(Path.GetFileName(l2ProFilename.ToUpper())))
                    {
                        result.Add(CrateL2ProductDef(element));
                    }
                }
            }
            return result.Count == 0 ? null : result.ToArray();
        }

        private static L2ProductDefind CrateL2ProductDef(XElement element)
        {
            L2ProductDefind result = new L2ProductDefind();
            result.Name = element.Attribute("name").Value;
            result.Desc = element.Attribute("desc").Value;
            result.Identify = element.Attribute("identify").Value;
            result.Product = element.Attribute("product").Value;
            XElement geoInfoElement = element.Element("GeoInfo");
            if (geoInfoElement != null)
            {
                GeoInfos geoInfo = new GeoInfos(geoInfoElement.Attribute("poj4").Value);
                GetGeoAtrrs(geoInfo, geoInfoElement);
                GetGeoDef(geoInfo, geoInfoElement);
                result.GeoInfo = geoInfo;
            }
            XElement proInfoElement = element.Element("ProInfo");
            if (proInfoElement != null)
            {
                ProInfos proInfo = new ProInfos();
                GetProDataSets(proInfo, proInfoElement);
                result.ProInfo = proInfo;
            }
            return result;
        }

        private static void GetGeoAtrrs(GeoInfos geoInfo, XElement geoInfoElement)
        {
            XElement geoAtrrs = geoInfoElement.Element("GeoAtrrs");
            if (geoAtrrs != null)
            {
                GeoAtrributes geoAtrr = null;
                if (!string.IsNullOrEmpty(geoAtrrs.Attribute("location").Value))
                {
                    geoAtrr = new GeoAtrributes();
                    geoAtrr.Location = (enumGeoAttType)Enum.Parse(typeof(enumGeoAttType), geoAtrrs.Attribute("location").Value);
                    geoAtrr.GeoDataset = geoAtrrs.Attribute("datasetname").Value;
                    geoAtrr.LeftTopLonAtrrName = geoAtrrs.Attribute("lefttoplon").Value;
                    geoAtrr.LeftTopLatAtrrName = geoAtrrs.Attribute("lefttoplat").Value;
                    geoAtrr.RightBottomLonAtrrName = geoAtrrs.Attribute("rightbottomlon").Value;
                    geoAtrr.RightBottomLatAtrrName = geoAtrrs.Attribute("rightbottomlat").Value;
                    geoAtrr.Unit = geoAtrrs.Attribute("unit").Value;
                }
                if (geoAtrr == null || (string.IsNullOrEmpty(geoAtrr.GeoDataset) && geoAtrr.Location == enumGeoAttType.Dataset) || string.IsNullOrEmpty(geoAtrr.LeftTopLonAtrrName) ||
                    string.IsNullOrEmpty(geoAtrr.LeftTopLatAtrrName) || string.IsNullOrEmpty(geoAtrr.RightBottomLonAtrrName) ||
                    string.IsNullOrEmpty(geoAtrr.RightBottomLatAtrrName))
                    geoInfo.GeoAtrrs = null;
                else
                    geoInfo.GeoAtrrs = geoAtrr;
            }
        }

        private static void GetGeoDef(GeoInfos geoInfo, XElement geoInfoElement)
        {
            XElement geoDefElement = geoInfoElement.Element("GeoDef");
            if (geoDefElement != null)
            {
                GeoDefs geoDef = null;
                geoDef = new GeoDefs();
                geoDef.LeftTopLon = GetDoubleAtrr(geoDefElement.Attribute("lefttoplon").Value);
                geoDef.LeftTopLat = GetDoubleAtrr(geoDefElement.Attribute("lefttoplat").Value);
                geoDef.RightBottomLon = GetDoubleAtrr(geoDefElement.Attribute("rightbottomlon").Value);
                geoDef.RightBottomLat = GetDoubleAtrr(geoDefElement.Attribute("rightbottomlat").Value);
                if (geoDef.LeftTopLon == double.MinValue || geoDef.LeftTopLat == double.MinValue ||
                    geoDef.RightBottomLon == double.MinValue || geoDef.RightBottomLat == double.MinValue)
                    geoInfo.GeoDef = null;
                else
                    geoInfo.GeoDef = geoDef;
            }
        }

        private static void GetProDataSets(ProInfos proInfo, XElement proInfoElement)
        {
            XElement proDataSetElement = proInfoElement.Element("ProDataSets");
            if (proDataSetElement != null)
            {
                proInfo.ProDatasets = proDataSetElement.Attribute("datasets").Value;
            }
        }

        private static double GetDoubleAtrr(string attribue)
        {
            double result = double.MinValue;
            if (string.IsNullOrEmpty(attribue))
                return result;
            if (double.TryParse(attribue, out result))
                return result;
            return double.MinValue;
        }

        public void Dispose()
        {
        }
    }
}
