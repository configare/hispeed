using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public static class RasterDictionaryTemplateFactory
    {
        public static string[] RasterTemplateNames = null;

        static RasterDictionaryTemplateFactory()
        {
            RasterTemplateNames = new string[] { "土地利用类型", "行政区划" };
        }

        public static IRasterTemplateProvider CreateRasterTemplate(string templateName)
        {
            switch (templateName)
            {
                case "土地利用类型":
                    return CreateLandRasterTemplate() as IRasterTemplateProvider;
                case "行政区划":
                    return CreateXjRasterTemplate() as IRasterTemplateProvider;
            }
            return null;
        }

        public static IRasterDictionaryTemplate<byte> CreateLandRasterTemplate()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "SystemData\\RasterTemplate\\";
            object obj = new LandRasterDictionaryTemplate(dir + "China_LandRaster.dat",
                dir + "China_LandRaster_Code.txt");
            return obj as IRasterDictionaryTemplate<byte>;
        }

        public static IRasterDictionaryTemplate<int> CreateXjRasterTemplate()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "SystemData\\RasterTemplate\\";
            object obj = new XjRasterDictionaryTemplate(dir + "China_XjRaster.dat",
                dir + "China_XjRaster_Code.txt");
            return obj as IRasterDictionaryTemplate<int>;
        }
    }
}
