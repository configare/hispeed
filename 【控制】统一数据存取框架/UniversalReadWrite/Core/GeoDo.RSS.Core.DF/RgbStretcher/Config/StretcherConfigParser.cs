using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace GeoDo.RSS.Core.DF
{
    /// <summary>
    /// 优化，defaultbands节点优化为可以允许为多组
    /// </summary>
    public class StretcherConfigParser
    {
        public StretcherConfigItem[] Parser(out string autoStretchFileNameRulers)
        {
            autoStretchFileNameRulers = null;
            string fname = AppDomain.CurrentDomain.BaseDirectory + "SystemData\\DataStretchers.xml";
            if (!File.Exists(fname))
                return null;
            List<StretcherConfigItem> items = new List<StretcherConfigItem>();
            XDocument doc = XDocument.Load(fname);
            if(doc.Root.Element("AutoStretcher") != null)
                autoStretchFileNameRulers = doc.Root.Element("AutoStretcher").Attribute("filenameruler").Value;
            XElement root = doc.Root.Element("Stretchers");
            foreach (XElement ele in root.Elements("Stretcher"))
            {
                StretcherConfigItem it = GetStretcherConfigItem(ele);
                if (it != null)
                    items.Add(it);
            }
            return items.Count > 0 ? items.ToArray() : null;
        }

        //<Stretcher name="" statellite="FY3A,FY3B" sensor="VIRR,MERSI" bandno="1" stretcher="GeoDo.RSS.Core.DF.DLL,GeoDo.RSS.Core.DF.LinearRgbStretcher" 
        //mindata="0" maxdata="1000" mingray="0" maxgray="255"/>
        private StretcherConfigItem GetStretcherConfigItem(XElement ele)
        {
            StretcherConfigItem it = new StretcherConfigItem();
            //
            it.Name = ele.Attribute("name").Value;
            //
            if (ele.Attribute("isproduct") != null)
            {
                it.IsProduct = bool.Parse(ele.Attribute("isproduct").Value);
                if (ele.Attribute("productidentify") != null)
                    it.ProductIdentify = ele.Attribute("productidentify").Value;
                else if (ele.Attribute("subproductidentify") != null)
                    it.ProductIdentify = ele.Attribute("subproductidentify").Value;
            }
            //
            string satellites = ele.Attribute("satelliate").Value;
            string[] parts = satellites.Split(',');
            foreach (string p in parts)
                if (p != null)
                    it.Satellites.Add(p);
            //
            string sensors = ele.Attribute("sensor").Value;
            parts = sensors.Split(',');
            foreach (string p in parts)
                if (p != null)
                    it.Sensors.Add(p);
            //
            string bands = ele.Attribute("bandno").Value;
            parts = bands.Split(',');
            int b = 0;
            foreach (string p in parts)
            {
                if (p != null)
                {
                    if (int.TryParse(p, out b))
                        it.BandNo.Add(b);
                }
            }
            //
            it.StretcherClass = ele.Attribute("stretcher").Value;
            //
            double v = 0;
            string strv = null;
            strv = ele.Attribute("mindata").Value;
            if (double.TryParse(strv, out v))
                it.MinData = v;
            strv = ele.Attribute("maxdata").Value;
            if (double.TryParse(strv, out v))
                it.MaxData = v;
            //
            byte gray = 0;
            strv = ele.Attribute("mingray").Value;
            if (byte.TryParse(strv, out gray))
                it.MinGray = gray;
            strv = ele.Attribute("maxgray").Value;
            if (byte.TryParse(strv, out gray))
                it.MaxGray = gray;
            //
            bool.TryParse(ele.Attribute("isusemap").Value, out it.IsUseMap);
            bool.TryParse(ele.Attribute("isorbit").Value, out it.IsOribit);
            //
            string deBands = ele.Attribute("defaultbands").Value;
            if (deBands == null)
                return it;
            string[] dfs = deBands.Split('|');
            List<int[]> dfls = new List<int[]>();
            foreach (string df in dfs)
            {
                int[] defaultBands = GetBands(df);
                if (defaultBands != null)
                    dfls.Add(defaultBands);
            }
            if (dfls.Count > 0)
                it.DefaultBands = dfls[0];
            if (dfls.Count > 1)
                it.DefaultBandsExt = dfls[1];
            return it;
        }

        private int[] GetBands(string ps)
        {
            string[] parts = ps.Split(',');
            if (parts.Length != 3)
                return null;
            int[] defaultBands = new int[3];
            int c = 0;
            for (int i = 0; i < 3; i++)
            {
                if (int.TryParse(parts[i], out c))
                    defaultBands[i] = c;
            }
            return defaultBands;
        }
    }
}
