using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using CodeCell.Bricks.Runtime;

namespace CodeCell.AgileMap.Core
{
   public class MapFactory
   {
        internal static string CurrentFilename = null;

        static MapFactory()
        {
        }

        public static string GetFullFilename(string filename)
        {
            string mcd = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory+ "state.agilemap", Encoding.Default);
            return RelativePathHelper.GetFullPath(mcd, filename);
            //return RelativePathHelper.GetFullPath(CurrentFilename, filename);
        }

        public static IMap LoadMapFrom(string filename)
        {
            CurrentFilename = filename;
            //在WCF中，MapFactory会调用一次初始化一次，因此写文件
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory+ "state.agilemap", filename,Encoding.Default);
            if (string.IsNullOrEmpty(filename))
                return null;
            if (!File.Exists(filename))
                throw new FileNotFoundException("制定的地图配置文件\""+filename+"\"不存在。");
            try
            {
                return ParseMapFromXml(filename.Clone() as string);
            }
            catch (Exception ex)
            {
                Log.WriterException("MapFactory", "LoadMapFrom", ex);
                throw;
            }
        }

        private static IMap ParseMapFromXml(string filename)
        {
            PersistObject.BeginParse();
            try
            {
                XDocument doc = XDocument.Load(filename);
                MapAuthor author = GetMapAuthor(doc);
                MapVersion version = GetMapVersion(doc);
                MapArguments arg = GetMapArguments(doc);
                ConflictDefinition csym = null, clabel = null;
                GetConflictDef(doc,out csym,out clabel);
                ILayer[] layers = GetFeatureLayer(doc);
                string mapname = doc.Element("Map").Attribute("name").Value;
                Map map = new Map(mapname, filename, version, author,arg, layers);
                map.SetConflictDefinition(csym,clabel);
                return map;
            }
            finally 
            {
                PersistObject.EndParse();
            }
        }

        private static void GetConflictDef(XDocument doc,out ConflictDefinition sym,out ConflictDefinition label)
        {
            sym = null;
            label = null;
            var result = doc.Element("Map").Element("ConflictDefinitionSymbol");
            if (result == null)
                return;
            sym = ConflictDefinition.FromXElement(result.Element("ConflictDefinition") as XElement);
            result = doc.Element("Map").Element("ConflictDefinitionLabel");
            label = ConflictDefinition.FromXElement(result.Element("ConflictDefinition") as XElement);
            return;
        }

        private static ILayer[] GetFeatureLayer(XDocument doc)
        {
            List<ILayer> layers = new List<ILayer>();
            var result = doc.Element("Map").Element("Layers").Elements();
            if (result == null)
                return null;
            foreach (XElement ele in result)
            {
                string name = ele.Name.LocalName.ToUpper();
                ILayer lyr = null ;
                if (name == "LAYER" || name == "FEATURELAYER")
                {
                    try
                    {
                        lyr = FeatureLayer.FromXElement(ele);
                    }
                    catch(Exception ex) 
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                //else if (name == "RASTERLAYER")
                //    lyr = RasterLayer.FromXElement(ele);
                if (lyr != null)
                {
                    layers.Add(lyr);
                }
            }
            return layers.Count > 0 ? layers.ToArray() : null;
        }

        private static MapArguments GetMapArguments(XDocument doc)
        {
            var result = doc.Element("Map").Element("MapArguments");
            if (result == null)
                return null;
            return MapArguments.FromXElement(result as XElement);
        }

        private static MapVersion GetMapVersion(XDocument doc)
        {
            var result = doc.Element("Map").Element("Version");
            if (result == null)
                return null;
            return MapVersion.FromXElement(result as XElement);
        }

        private static MapAuthor GetMapAuthor(XDocument doc)
        {
            var result = doc.Element("Map").Element("Author");
            if (result == null)
                return null;
            return MapAuthor.FromXElement(result as XElement);
        }
    }
}
