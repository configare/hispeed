using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.MIF.Core
{
    public static class AOITemplateFactory
    {
        public static string[] TemplateNames = null;

        static AOITemplateFactory()
        {
            TemplateNames = new string[] { "vector:太湖", "vector:巢湖", "vector:滇池", "vector:鄱阳湖", "vector:洞庭湖", "vector:海陆模版", "vector:海陆模版_反",
                "vector:贝尔湖", "vector:呼伦湖","vector:丹江口水库","vector:官厅水库","vector:密云水库", "raster:土地利用类型", "raster:行政区划" , "vector:西藏常用湖泊",
                 "vector:纳木错", "vector:色林错", "vector:羊卓雍错", "vector:普莫雍错", "vector:当惹雍错", "vector:扎日南木错", "vector:佩枯错", "vector:塔若错", "vector:玛旁雍错", "vector:拉昂错"};
        }

        /// <summary>
        /// 按照范围提取模版的AOIs
        /// </summary>
        /// <param name="templateName">
        /// vector:TemplateName = [raster|vector]:templateType
        /// raster: TemplateName = [raster|vector]:templateType:featureName
        /// 如：raster:行政区划:河北省
        /// </param> 
        /// <param name="minLon"></param>
        /// <param name="maxLon"></param>
        /// <param name="minLat"></param>
        /// <param name="maxLat"></param>
        /// <param name="outSize"></param>
        /// <returns></returns>
        public static int[] MakeAOI(string templateName, double minLon, double maxLon, double minLat, double maxLat, Size outSize)
        {
            if (String.IsNullOrEmpty(templateName))
                return null;
            string[] parts = templateName.Split(':');
            if (parts.Length != 3 && parts.Length != 2)
                return null;
            object template = GetTemplateByArrays(parts[0], parts[1]);
            if (template is IRasterTemplateProvider)
                return (template as IRasterTemplateProvider).GetAOI(parts[2], minLon, maxLon, minLat, maxLat, outSize);
            if (template is VectorAOITemplate)
            {
                Envelope envelop = new Envelope(minLon, minLat, maxLon, maxLat);
                return (template as VectorAOITemplate).GetAOI(envelop, outSize);
            }
            return null;
        }

        public static int[] MakeAOI(Feature[] fets, double minLon, double maxLon, double minLat, double maxLat, Size outSize)
        {
            if (fets == null || fets.Length == 0)
                return null;
            Envelope envelop = new Envelope(minLon, minLat, maxLon, maxLat);
            return GetAOI(fets, envelop, outSize);
        }

        public static int[] GetAOI(Feature[] fets, Envelope dstEnvelope, Size size)
        {
            if (fets == null || fets.Length == 0)
                return null;
            List<ShapePolygon> geometryslist = new List<ShapePolygon>();
            ShapePolygon temp = null;
            foreach (Feature item in fets)
            {
                temp = item.Geometry as ShapePolygon;
                if (temp != null)
                    geometryslist.Add(temp);
            }
            if (geometryslist.Count == 0)
                return null;
            using (IVectorAOIGenerator gen = new VectorAOIGenerator())
            {
                int[] aoi = gen.GetAOI(geometryslist.ToArray(), dstEnvelope, size);
                return aoi;
            }
        }

        private static object GetTemplateByArrays(string templateType, string templateName)
        {
            //templateType:raster,templateName:土地利用类型
            if (templateType.ToLower() == "vector")
                return GetVectorTemplateByArray(templateName);
            if (templateType.ToLower() == "raster")
                return GetRasterTemplateByArray(templateName);
            return null;
        }

        private static object GetRasterTemplateByArray(string templateName)
        {
            string[] rasterNames = RasterDictionaryTemplateFactory.RasterTemplateNames;
            foreach (string name in rasterNames)
            {
                if (name == templateName)
                    return RasterDictionaryTemplateFactory.CreateRasterTemplate(templateName);
            }
            return null;
        }

        private static object GetVectorTemplateByArray(string templateName)
        {
            string[] vectorNames = VectorAOITemplateFactory.VectorTemplateNames;
            foreach (string name in vectorNames)
            {
                if (name == templateName)
                    return VectorAOITemplateFactory.GetAOITemplate(templateName);
            }
            return null;
        }
    }
}
