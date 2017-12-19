using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Core
{
    public interface IStatAnalysisEngine<T>
    {
        StatResultItem[] StatArea(string rasterFileName, string templateName, Func<T, bool> filter);
        StatResultItem[] StatArea(IRasterDataProvider raster, string templateName, Func<T, bool> filter);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rasterFileName"></param>
        /// <param name="shpFileName"></param>
        /// <param name="primaryFieldName">所选定的feature名</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        StatResultItem[] StatAreaCustom(string rasterFileName, string shpFileName, string[] FieldValues, string primaryFieldName, Func<T, bool> filter);
        StatResultItem[] StatAreaCustom(IRasterDataProvider raster, string shpFileName, string[] FieldValues, string primaryFieldName, Func<T, bool> filter);
        StatResultItem[] StatAreaCustom(string rasterFileName, Dictionary<string, int[]> aoi, Func<T, bool> filter);
        StatResultItem[] StatAreaCustom(IRasterDataProvider raster, Dictionary<string, int[]> aoi, Func<T, bool> filter);
        //weight
        StatResultItem[] StatArea(string rasterFileName, string templateName, Func<T, int, int> weight);
        StatResultItem[] StatArea(IRasterDataProvider raster, string templateName, Func<T, int, int> weight);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rasterFileName"></param>
        /// <param name="shpFileName"></param>
        /// <param name="primaryFieldName">所选定的feature名</param>
        /// <param name="weight"></param>
        /// <returns></returns>
        StatResultItem[] StatAreaCustom(string rasterFileName, string shpFileName, string[] FieldValues, string primaryFieldName, Func<T, int,int> weight);
        StatResultItem[] StatAreaCustom(IRasterDataProvider raster, string shpFileName, string[] FieldValues, string primaryFieldName, Func<T, int, int> weight);
        StatResultItem[] StatAreaCustom(string rasterFileName, Dictionary<string, int[]> aoi, Func<T, int, int> weight);
        StatResultItem[] StatAreaCustom(IRasterDataProvider raster, Dictionary<string, int[]> aoi, Func<T, int, int> weight);
        void Display(IStatResultWindow window, string title, StatResultItem[] resultItems);
    }
}
