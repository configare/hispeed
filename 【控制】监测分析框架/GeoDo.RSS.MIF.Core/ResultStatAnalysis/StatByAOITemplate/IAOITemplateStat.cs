using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Core
{
    public interface IAOITemplateStat<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rasterFileName"></param>
        /// <param name="shpFileVectorTemplate">矢量模板的完整路径</param>
        /// <param name="shpPrimaryField">主字段名</param>
        /// <param name="filter">过滤条件</param>
        /// <returns></returns>
        StatResultItem[] CountByVector(string rasterFileName, string shpFileVectorFullname, string shpPrimaryField, Func<T, bool> filter);
        StatResultItem[] CountByVector(IRasterDataProvider raster, string shpFileVectorTemplate, string shpPrimaryField, Func<T, bool> filter);
        StatResultItem[] CountByVector(IRasterDataProvider raster, Dictionary<string, int[]> aoi, Func<T, bool> filter);
        StatResultItem[] CountByVector(string rasterFileName, Dictionary<string, int[]> aoi, Func<T, bool> filter);
        StatResultItem[] CountByRaster(string rasterFileName, string templateName, Func<T, bool> filter);
        StatResultItem[] CountByRaster(IRasterDataProvider raster, string templateName, Func<T, bool> filter);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rasterFileName"></param>
        /// <param name="shpFileVectorTemplate">矢量模板的完整路径</param>
        /// <param name="shpPrimaryField">主字段名</param>
        /// <param name="weight">加权条件</param>
        /// <returns></returns>
        StatResultItem[] CountByVector(string rasterFileName, string shpFileVectorFullname, string shpPrimaryField, Func<T, int, int> weight);
        StatResultItem[] CountByVector(IRasterDataProvider raster, string shpFileVectorTemplate, string shpPrimaryField, Func<T, int, int> weight);
        StatResultItem[] CountByVector(IRasterDataProvider raster, Dictionary<string, int[]> aoi, Func<T, int, int> weight);
        StatResultItem[] CountByVector(string rasterFileName, Dictionary<string, int[]> aoi, Func<T, int, int> weight);
        StatResultItem[] CountByRaster(string rasterFileName, string templateName, Func<T, int, int> weight);
        StatResultItem[] CountByRaster(IRasterDataProvider raster, string templateName, Func<T, int, int> weight);
    }
}
