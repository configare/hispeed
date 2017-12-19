using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.MIF.Core
{
    public class StatAnalysisEngine<T> : IStatAnalysisEngine<T>
    {
        public StatResultItem[] StatArea(string rasterFileName, string templateName, Func<T, bool> filter)
        {
            using (IRasterDataProvider raster = GeoDataDriver.Open(rasterFileName) as IRasterDataProvider)
            {
                return StatArea(raster, templateName, filter);
            }
        }

        public StatResultItem[] StatArea(IRasterDataProvider raster, string templateName, Func<T, bool> filter)
        {
            if (filter == null)
                return null;
            StatResultItem[] resultItems = null;
            switch (templateName)
            {
                case "分级行政区划":
                    resultItems = ApplyStatByRasterTemplate(raster, templateName, filter);
                    break;
                case "省级行政区划":
                    resultItems = ApplyStatByVectorTemplate(raster, "省级行政区域_面.shp", "NAME", filter);
                    break;
                case "土地利用类型":
                    resultItems = ApplyStatByVectorTemplate(raster, "土地利用类型_合并.shp", "NAME", filter);
                    break;
                case "省级行政区+土地利用类型":
                    resultItems = DoStatByAdminAndLanduseType(raster, filter);
                    break;
                case "洞庭分区":
                    resultItems = ApplyStatByVectorTemplate(raster, "洞庭分区.shp", "NAME", filter);
                    break;
                case "西藏常用湖泊":
                    resultItems = ApplyStatByVectorTemplate(raster, "西藏常用湖泊区域.shp", "NAME", filter);
                    break;
            }
            if (resultItems == null || resultItems.Length == 0)
                return null;
            return resultItems;
        }

        private StatResultItem[] DoStatByAdminAndLanduseType(IRasterDataProvider raster, Func<T, bool> filter)
        {
            SataAdminAndLandUse<T> stat = new SataAdminAndLandUse<T>();
            Dictionary<string, Dictionary<string, double>> result = stat.StatArea(raster, filter);
            if (result == null)
                return null;
            List<StatResultItem> items = new List<StatResultItem>();
            foreach (string admin in result.Keys)
            {
                if (result[admin] == null)
                    continue;
                foreach (string landuse in result[admin].Keys)
                {
                    if (result[admin][landuse] == 0)
                        continue;
                    StatResultItem it = new StatResultItem();
                    it.Name = admin + "_" + landuse;
                    it.Value = result[admin][landuse];
                    items.Add(it);
                }
            }
            return items.Count > 0 ? items.ToArray() : null;
        }

        public StatResultItem[] StatAreaCustom(string rasterFileName, string templateName, string[] FiledNames, string primaryFieldName, Func<T, bool> filter)
        {
            using (IRasterDataProvider raster = GeoDataDriver.Open(rasterFileName) as IRasterDataProvider)
            {
                return StatAreaCustom(raster, templateName, FiledNames, primaryFieldName, filter);
            }
        }

        public StatResultItem[] StatAreaCustom(IRasterDataProvider raster, string templateName, string[] FiledNames, string primaryFieldName, Func<T, bool> filter)
        {
            if (filter == null)
                return null;
            StatResultItem[] results = null;
            switch (templateName)
            {
                case "分级行政区划":
                    results = ApplyStatByRasterTemplate(raster, templateName, filter);
                    break;
                case "省级行政区划":
                    results = ApplyStatByVectorTemplate(raster, "省级行政区域_面.shp", primaryFieldName, filter);
                    break;
                case "土地利用类型":
                    results = ApplyStatByVectorTemplate(raster, "土地利用类型_合并.shp", primaryFieldName, filter);
                    break;
                default:
                    results = ApplyStatByVectorProvider(raster, templateName, primaryFieldName, filter);
                    break;
            }
            if (results == null || results.Length == 0)
                return null;
            List<StatResultItem> primaryItems = new List<StatResultItem>();
            StatResultItem item = null;
            foreach (string name in FiledNames)
            {
                item = MatchItemInResults(name, results);
                if (item != null)
                    primaryItems.Add(item);
            }
            if (primaryItems.Count == 0)
                return null;
            return primaryItems.ToArray();
        }

        public StatResultItem[] StatAreaCustom(string rasterFilename, Dictionary<string, int[]> aoi, Func<T, bool> filter)
        {
            using (IRasterDataProvider raster = GeoDataDriver.Open(rasterFilename) as IRasterDataProvider)
            {
                return StatAreaCustom(raster, aoi, filter);
            }
        }

        public StatResultItem[] StatAreaCustom(IRasterDataProvider raster, Dictionary<string, int[]> aoi, Func<T, bool> filter)
        {
            if (filter == null)
                return null;
            StatResultItem[] results = null;
            results = ApplyStatByVectorProvider(raster, aoi, filter);
            if (results == null || results.Length == 0)
                return null;
            List<StatResultItem> primaryItems = new List<StatResultItem>();
            StatResultItem item = null;
            foreach (string name in aoi.Keys)
            {
                item = MatchItemInResults(name, results);
                if (item != null)
                    primaryItems.Add(item);
            }
            if (primaryItems.Count == 0)
                return null;
            return primaryItems.ToArray();
        }

        public void Display(IStatResultWindow window, string title, StatResultItem[] resultItems)
        {
            IStatResult results = StatResultItemToIStatResult.ItemsToResults(resultItems);
            window.Add(true, title, results, true, 1);
        }

        private StatResultItem MatchItemInResults(string name, StatResultItem[] results)
        {
            StatResultItem result = null;
            foreach (StatResultItem item in results)
            {
                if (name == item.Name)
                    return item;
            }
            result = new StatResultItem();
            result.Name = name;
            result.Value = 0;
            return result;
        }

        /// <summary>
        /// 通过AOI自定义统计面积
        /// </summary>
        /// <param name="rasterFileName"></param>
        /// <param name="aoi"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private StatResultItem[] ApplyStatByVectorProvider(IRasterDataProvider raster, Dictionary<string, int[]> aoi, Func<T, bool> filter)
        {
            IAOITemplateStat<T> stat = new AOITemplateStat<T>();
            return ResultsWithoutZero(stat.CountByVector(raster, aoi, filter));
        }

        /// <summary>
        /// 通过传入自定义矢量文件进行统计
        /// </summary>
        /// <param name="raster">要进行统计的栅格数据提供者</param>
        /// <param name="shpFullname">自定义矢量文件的完整路径</param>
        /// <param name="filter">统计条件</param>
        /// <returns></returns>
        private StatResultItem[] ApplyStatByVectorProvider(IRasterDataProvider raster, string shpFullname, string primaryFieldName, Func<T, bool> filter)
        {
            IAOITemplateStat<T> stat = new AOITemplateStat<T>();
            return ResultsWithoutZero(stat.CountByVector(raster, shpFullname, primaryFieldName, filter));
        }

        private StatResultItem[] ApplyStatByVectorTemplate(IRasterDataProvider raster, string shpTemplateName, string primaryFieldName, Func<T, bool> filter)
        {
            string shpFullname = VectorAOITemplate.FindVectorFullname(shpTemplateName);
            IAOITemplateStat<T> stat = new AOITemplateStat<T>();
            return ResultsWithoutZero(stat.CountByVector(raster, shpFullname, primaryFieldName, filter));
        }

        private StatResultItem[] ApplyStatByRasterTemplate(IRasterDataProvider raster, string templateName, Func<T, bool> filter)
        {
            IAOITemplateStat<T> stat = new AOITemplateStat<T>();
            templateName = "China_XjRaster";
            return ResultsWithoutZero(stat.CountByRaster(raster, templateName, filter));
        }

        private StatResultItem[] ResultsWithoutZero(StatResultItem[] items)
        {
            if (items == null || items.Length == 0)
                return null;
            List<StatResultItem> results = new List<StatResultItem>();
            foreach (StatResultItem item in items)
            {
                if (item.Value != 0)
                    results.Add(item);
            }
            if (results == null || results.Count == 0)
                return null;
            return results.ToArray();
        }

        #region weight

        public StatResultItem[] StatArea(string rasterFileName, string templateName, Func<T, int, int> weight)
        {
            using (IRasterDataProvider raster = GeoDataDriver.Open(rasterFileName) as IRasterDataProvider)
            {
                return StatArea(raster, templateName, weight);
            }
        }

        public StatResultItem[] StatArea(IRasterDataProvider raster, string templateName, Func<T, int, int> weight)
        {
            if (weight == null)
                return null;
            StatResultItem[] resultItems = null;
            switch (templateName)
            {
                case "分级行政区划":
                    resultItems = ApplyStatByRasterTemplate(raster, templateName, weight);
                    break;
                case "省级行政区划":
                    resultItems = ApplyStatByVectorTemplate(raster, "省级行政区域_面.shp", "NAME", weight);
                    break;
                case "土地利用类型":
                    resultItems = ApplyStatByVectorTemplate(raster, "土地利用类型_合并.shp", "NAME", weight);
                    break;
                case "省级行政区+土地利用类型":
                    resultItems = DoStatByAdminAndLanduseType(raster, weight);
                    break;
            }
            if (resultItems == null || resultItems.Length == 0)
                return null;
            return resultItems;
        }

        public StatResultItem[] StatAreaCustom(string rasterFileName, string templateName, string[] FiledNames, string primaryFieldName, Func<T, int, int> weight)
        {
            using (IRasterDataProvider raster = GeoDataDriver.Open(rasterFileName) as IRasterDataProvider)
            {
                return StatAreaCustom(raster, templateName, FiledNames, primaryFieldName, weight);
            }
        }

        public StatResultItem[] StatAreaCustom(IRasterDataProvider raster, string templateName, string[] FiledNames, string primaryFieldName, Func<T, int, int> weight)
        {
            if (weight == null)
                return null;
            StatResultItem[] results = null;
            switch (templateName)
            {
                case "分级行政区划":
                    results = ApplyStatByRasterTemplate(raster, templateName, weight);
                    break;
                case "省级行政区划":
                    results = ApplyStatByVectorTemplate(raster, "省级行政区域_面.shp", primaryFieldName, weight);
                    break;
                case "土地利用类型":
                    results = ApplyStatByVectorTemplate(raster, "土地利用类型_合并.shp", primaryFieldName, weight);
                    break;
                default:
                    results = ApplyStatByVectorProvider(raster, templateName, primaryFieldName, weight);
                    break;
            }
            if (results == null || results.Length == 0)
                return null;
            List<StatResultItem> primaryItems = new List<StatResultItem>();
            StatResultItem item = null;
            foreach (string name in FiledNames)
            {
                item = MatchItemInResults(name, results);
                if (item != null)
                    primaryItems.Add(item);
            }
            if (primaryItems.Count == 0)
                return null;
            return primaryItems.ToArray();
        }

        public StatResultItem[] StatAreaCustom(string rasterFilename, Dictionary<string, int[]> aoi, Func<T, int, int> weight)
        {
            using (IRasterDataProvider raster = GeoDataDriver.Open(rasterFilename) as IRasterDataProvider)
            {
                return StatAreaCustom(raster, aoi, weight);
            }
        }

        public StatResultItem[] StatAreaCustom(IRasterDataProvider raster, Dictionary<string, int[]> aoi, Func<T, int, int> weight)
        {
            if (weight == null)
                return null;
            StatResultItem[] results = null;
            results = ApplyStatByVectorProvider(raster, aoi, weight);
            if (results == null || results.Length == 0)
                return null;
            List<StatResultItem> primaryItems = new List<StatResultItem>();
            StatResultItem item = null;
            foreach (string name in aoi.Keys)
            {
                item = MatchItemInResults(name, results);
                if (item != null)
                    primaryItems.Add(item);
            }
            if (primaryItems.Count == 0)
                return null;
            return primaryItems.ToArray();
        }

        private StatResultItem[] DoStatByAdminAndLanduseType(IRasterDataProvider raster, Func<T, int, int> weight)
        {
            SataAdminAndLandUse<T> stat = new SataAdminAndLandUse<T>();
            Dictionary<string, Dictionary<string, double>> result = stat.StatArea(raster, weight);
            if (result == null)
                return null;
            List<StatResultItem> items = new List<StatResultItem>();
            foreach (string admin in result.Keys)
            {
                if (result[admin] == null)
                    continue;
                foreach (string landuse in result[admin].Keys)
                {
                    if (result[admin][landuse] == 0)
                        continue;
                    StatResultItem it = new StatResultItem();
                    it.Name = admin + "_" + landuse;
                    it.Value = result[admin][landuse];
                    items.Add(it);
                }
            }
            return items.Count > 0 ? items.ToArray() : null;
        }

        /// <summary>
        /// 通过AOI自定义统计面积
        /// </summary>
        /// <param name="rasterFileName"></param>
        /// <param name="aoi"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        private StatResultItem[] ApplyStatByVectorProvider(IRasterDataProvider raster, Dictionary<string, int[]> aoi, Func<T, int, int> weight)
        {
            IAOITemplateStat<T> stat = new AOITemplateStat<T>();
            return ResultsWithoutZero(stat.CountByVector(raster, aoi, weight));
        }

        /// <summary>
        /// 通过传入自定义矢量文件进行统计
        /// </summary>
        /// <param name="raster">要进行统计的栅格数据提供者</param>
        /// <param name="shpFullname">自定义矢量文件的完整路径</param>
        /// <param name="weight">统计条件</param>
        /// <returns></returns>
        private StatResultItem[] ApplyStatByVectorProvider(IRasterDataProvider raster, string shpFullname, string primaryFieldName, Func<T, int, int> weight)
        {
            IAOITemplateStat<T> stat = new AOITemplateStat<T>();
            return ResultsWithoutZero(stat.CountByVector(raster, shpFullname, primaryFieldName, weight));
        }

        private StatResultItem[] ApplyStatByVectorTemplate(IRasterDataProvider raster, string shpTemplateName, string primaryFieldName, Func<T, int, int> weight)
        {
            string shpFullname = VectorAOITemplate.FindVectorFullname(shpTemplateName);
            IAOITemplateStat<T> stat = new AOITemplateStat<T>();
            return ResultsWithoutZero(stat.CountByVector(raster, shpFullname, primaryFieldName, weight));
        }

        private StatResultItem[] ApplyStatByRasterTemplate(IRasterDataProvider raster, string templateName, Func<T, int, int> weight)
        {
            IAOITemplateStat<T> stat = new AOITemplateStat<T>();
            templateName = "China_XjRaster";
            return ResultsWithoutZero(stat.CountByRaster(raster, templateName, weight));
        }


        #endregion
    }
}
