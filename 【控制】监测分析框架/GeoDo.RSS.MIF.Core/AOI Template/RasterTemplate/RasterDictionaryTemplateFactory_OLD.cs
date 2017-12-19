using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public static class RasterDictionaryTemplateFactory_OLD
    {
        private static Dictionary<string, object> _rasterTemplates = new Dictionary<string, object>();
        public static string[] RasterTemplateNames = null;

        static RasterDictionaryTemplateFactory_OLD()
        {
            RasterTemplateNames = new string[] { "土地利用类型", "行政区划" };
        }

        public static IRasterTemplateProvider GetRasterTemplate(string templateName)
        {
            switch (templateName)
            {
                case "土地利用类型":
                    return GetLandRasterTemplate() as IRasterTemplateProvider;
                case "行政区划":
                    return GetXjRasterTemplate() as IRasterTemplateProvider;
            }
            return null;
        }

        public static IRasterDictionaryTemplate<byte> GetLandRasterTemplate()
        {
            if (_rasterTemplates.ContainsKey("landuse"))
                return _rasterTemplates["landuse"] as IRasterDictionaryTemplate<byte>;
            else
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory + "SystemData\\RasterTemplate\\";
                object obj = new LandRasterDictionaryTemplate(dir + "China_LandRaster.dat",
                    dir + "China_LandRaster_Code.txt");
                _rasterTemplates.Add("landuse", obj);
                return _rasterTemplates["landuse"] as IRasterDictionaryTemplate<byte>;
            }
        }

        public static IRasterDictionaryTemplate<int> GetXjRasterTemplate()
        {
            if (_rasterTemplates.ContainsKey("xj"))
                return _rasterTemplates["xj"] as IRasterDictionaryTemplate<int>;
            else
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory + "SystemData\\RasterTemplate\\";
                object obj = new XjRasterDictionaryTemplate(dir + "China_XjRaster.dat",
                    dir + "China_XjRaster_Code.txt");
                _rasterTemplates.Add("xj", obj);
                return _rasterTemplates["xj"] as IRasterDictionaryTemplate<int>;
            }
        }
    }
}
