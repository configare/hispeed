using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public class ParseLAIConfigXml
    {
        static string _laixml = AppDomain.CurrentDomain.BaseDirectory + @"\SystemData\ProductArgs\VGT\LAIConfig.xml";
        static string _dataset = null;
        static int redNO, irNO, swirNO;
        static string latstr, lonstr, szastr, saastr, vzastr, vaastr;
        static int szaNo, saaNo, vzaNo, vaaNo;
        static string _configstr = null;

        public static bool ParseXML(string satellite,string sensor,bool isOrbit =true)
        {
            if (!File.Exists(_laixml))
                return false;
            XDocument _doc = XDocument.Load(_laixml);
            if (_doc == null)
                return false;
            if (isOrbit)
                _configstr = "LAIHDFConfig";
            else
                _configstr = "LAIPrjConfig";
            XElement root = _doc.Root;
            IEnumerable<XElement> elements = root.Elements();
            foreach (XElement ele in elements)
            {
                if (ele.Name == _configstr)
                {
                    return ParseXMLElement(ele, satellite,sensor,isOrbit);
                }
            }
            return false;
        }

        private static bool ParseXMLElement(XElement root, string satellite, string sensor, bool isHDF)
        {
            IEnumerable<XElement> elements = root.Elements();
            XElement subele,geoele;
            if (isHDF)
            {
                #region parse"LAIHDFconfig"
                foreach (XElement ele in elements)
                {
                    if (ele.Name == "Types" && ele.Attribute("satellite").Value == satellite && ele.Attribute("sensor").Value == sensor)
                    {
                        subele = ele.Element("L2_LSR");
                        if (subele.Attributes("Dataset").Count() != 1
                            || subele.Attributes("Redbandno").Count() != 1
                            || subele.Attributes("IRbandno").Count() != 1
                            || subele.Attributes("SWIRbandno").Count() != 1)
                            return false;
                        _dataset = subele.Attribute("Dataset").Value;
                        if (!int.TryParse(subele.Attribute("Redbandno").Value, out redNO))
                            return false;
                        if (!int.TryParse(subele.Attribute("IRbandno").Value, out irNO))
                            return false;
                        if (!int.TryParse(subele.Attribute("SWIRbandno").Value, out swirNO))
                            return false;
                        geoele = ele.Element("L1_GEO");
                        if (geoele.Attributes("Latitude").Count() != 1
                            || geoele.Attributes("Longitude").Count() != 1
                            || geoele.Attributes("SolarZenith").Count() != 1
                            || geoele.Attributes("SolarAzimuth").Count() != 1
                            || geoele.Attributes("SensorZenith").Count() != 1
                            || geoele.Attributes("SensorAzimuth").Count() != 1)
                            return false;
                        latstr = geoele.Attribute("Latitude").Value;
                        lonstr = geoele.Attribute("Longitude").Value;
                        szastr = geoele.Attribute("SolarZenith").Value;
                        saastr = geoele.Attribute("SolarAzimuth").Value;
                        vzastr = geoele.Attribute("SensorZenith").Value;
                        vaastr = geoele.Attribute("SensorAzimuth").Value;
                        return true;
                    }
                }
#endregion
            }
            else
            {
                #region parse"LAIPrjconfig"
                foreach (XElement ele in elements)
                {
                    if (ele.Name == "Types" && ele.Attribute("satellite").Value == satellite && ele.Attribute("sensor").Value == sensor)
                    {
                        subele = ele.Element("L2_LSR_Prj");
                        if (subele.Attributes("Redbandno").Count() != 1
                            || subele.Attributes("IRbandno").Count() != 1
                            || subele.Attributes("SWIRbandno").Count() != 1)
                            return false;
                        if (!int.TryParse(subele.Attribute("Redbandno").Value, out redNO))
                            return false;
                        if (!int.TryParse(subele.Attribute("IRbandno").Value, out irNO))
                            return false;
                        if (!int.TryParse(subele.Attribute("SWIRbandno").Value, out swirNO))
                            return false;
                        geoele = ele.Element("L1_Angles_Prj");
                        if (geoele.Attributes("SolarZenithBandNo").Count() != 1
                            || geoele.Attributes("SolarAzimuthBandNo").Count() != 1
                            || geoele.Attributes("SensorZenithBandNo").Count() != 1
                            || geoele.Attributes("SensorAzimuthBandNo").Count() != 1)
                            return false;
                        if (!int.TryParse(geoele.Attribute("SolarZenithBandNo").Value, out szaNo))
                            return false;
                        if (!int.TryParse(geoele.Attribute("SolarAzimuthBandNo").Value, out saaNo))
                            return false;
                        if (!int.TryParse(geoele.Attribute("SensorZenithBandNo").Value, out vzaNo))
                            return false;
                        if (!int.TryParse(geoele.Attribute("SensorAzimuthBandNo").Value, out vaaNo))
                            return false;
                        return true;
                    }
                }
                #endregion
            }
            return false;
        }
        public static string GeoDataSets
        {
            get { return szastr+","+ saastr+","+ vzastr+","+ vaastr+","+lonstr+","+latstr; }
        }

        public static string DataSet
        {
            get{return _dataset;}
        }

        public static int  RedNO
        {
            get { return redNO; }
        }

        public static int IRNO
        {
            get { return irNO; }
        }

        public static int SWIRNO
        {
            get { return swirNO; }
        }

        public static int SZANO
        {
            get { return szaNo; }
        }

        public static int SAANO
        {
            get { return saaNo; }
        }

        public static int VZANO
        {
            get { return vzaNo; }
        }

        public static int VAANO
        {
            get { return vaaNo; }
        }
    }
}
