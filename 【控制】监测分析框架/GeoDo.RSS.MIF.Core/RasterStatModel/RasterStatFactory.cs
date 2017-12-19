using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class RasterStatFactory
    {
        /// <summary>
        /// 单条件,面积及覆盖面积统计
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rasterFiles"></param>
        /// <param name="templateName">
        /// 如果是矢量：
        /// vector:省级行政区.shp:Name
        /// 如果是栅格：
        /// 直接China_XjRaster|China_LandRaster
        /// 或者
        /// raster:China_XjRaster
        /// raster:China_LandRaster
        /// 如果是空，则代表统计当前区域面积。
        /// </param>
        /// <param name="func"></param>
        /// <param name="progressTracker"></param>
        /// <returns></returns>
        public static SortedDictionary<string, StatAreaItem> Stat<T>(string[] rasterFiles, string templateName, Func<T, bool> func, Action<int, string> progressTracker, bool isCombinSameDay)
        {
            if (templateName.Contains("vector"))
            {
                string[] split = templateName.Split(':');
                string key = split[0];
                string shpFilename = split[1];
                string fieldName = split[2];
                RasterStatByVector<T> v = new RasterStatByVector<T>(progressTracker);
                return v.CountByVector(rasterFiles, shpFilename, fieldName, func, isCombinSameDay);
            }
            else
            {
                string value = templateName;
                if (templateName.Contains("raster"))
                {
                    string[] split = templateName.Split(':');
                    value = split[1];
                }
                RasterStatByRaster rsm = new RasterStatByRaster(progressTracker);
                return rsm.Stat<T>(rasterFiles, value, func, isCombinSameDay);
            }
        }


        /// <summary>
        /// 多条件,面积统计
        /// 每种条件为一列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rasterFile"></param>
        /// <param name="templateName">
        /// 为空则表示处理当前整个栅格
        /// </param>
        /// <param name="multiFilter"></param>
        /// <param name="progressTracker"></param>
        /// <returns></returns>
        public static Dictionary<string, SortedDictionary<string, double>> Stat<T>(
                                 string rasterFile,
                                 string templateName,
                                 Dictionary<string, Func<T, bool>> multiFilter,
                                 Action<int, string> progressTracker) where T : struct, IConvertible
        {
            return Stat<T>(rasterFile, templateName, multiFilter, progressTracker, false,1);
        }

        public static Dictionary<string, SortedDictionary<string, double>> Stat<T>(
            string rasterFile,
            string templateName,
            Dictionary<string, Func<T, bool>> multiFilter,
            Action<int, string> progressTracker, bool weight,float weightZoom,string vectorTemplate) where T : struct, IConvertible
        {
            if (string.IsNullOrWhiteSpace(templateName))
            {
                RasterStatByRaster stat = new RasterStatByRaster(progressTracker);
                return stat.Stat<T>(rasterFile, null, multiFilter, weight, weightZoom);
            }
            else if (templateName.Contains("vector"))
            {
                string[] split = templateName.Split(':');
                string key = split[0];
                string shpFilename = split[1];
                string fieldName = split[2];
                RasterStatByVector<T> v = new RasterStatByVector<T>(progressTracker);
                return v.CountByVector(rasterFile, shpFilename, fieldName, multiFilter);
            }
            else if (templateName.Contains("raster"))
            {
                string tmpValue = templateName;
                if (templateName.Contains("raster"))
                {
                    string[] split = templateName.Split(':');
                    tmpValue = split[1];
                }
                RasterStatByRaster stat = new RasterStatByRaster(progressTracker);
                return stat.Stat<T>(rasterFile, tmpValue, multiFilter, weight, weightZoom, vectorTemplate);
            }
            return null;
        }

        public static Dictionary<string, SortedDictionary<string, double>> Stat<T>(
          string rasterFile,
          string templateName,
          Dictionary<string, Func<T, bool>> multiFilter,
          Action<int, string> progressTracker, bool weight, float weightZoom) where T : struct, IConvertible
        {
            return Stat<T>(rasterFile, templateName, multiFilter, progressTracker, weight, weightZoom, null);
        }

        /// <summary>
        /// 单条件
        /// 分类面积及面积百分比统计
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rasterFiles"></param>
        /// <param name="templateName"></param>
        /// <param name="func"></param>
        /// <param name="progressTracker"></param>
        /// <returns></returns>
        public static Dictionary<string, double[]> StatPercent<T>(string[] rasterFiles, string templateName, Func<T, bool> func, Action<int, string> progressTracker)
        {
            if (templateName.Contains("vector"))
            {
                string[] split = templateName.Split(':');
                string key = split[0];
                string shpFilename = split[1];
                string fieldName = split[2];
                RasterStatByVector<T> v = new RasterStatByVector<T>(progressTracker);
                return v.MaxCountByVector(rasterFiles, shpFilename, fieldName, func);
            }
            return null;
        }

        /// <summary>
        /// 单文件,分段(多条件)
        /// 面积统计以及其占各矢量面的百分比
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rasterFile"></param>
        /// <param name="templateName"></param>
        /// <param name="funcs"></param>
        /// <param name="progressTracker"></param>
        /// <returns>
        /// SortedDictionary<string, double[n][2]>
        /// key:为要统计的矢量面
        /// value:为该矢量面下每个条件的统计值，每个统计值包含两项：（面积和面积占矢量面百分比）
        /// double[n]:n为条件，double[0],面积；double[1],面积占面百分比。
        /// </returns>
        public static SortedDictionary<string, double[][]> StatPercent<T>(string rasterFile, string templateName, Dictionary<string, Func<T, bool>> funcs, Action<int, string> progressTracker)
        {
            return StatPercent<T>(rasterFile, templateName, funcs, progressTracker, false, 1);
        }

        public static SortedDictionary<string, double[][]> StatPercent<T>(string rasterFile, string templateName, Dictionary<string, Func<T, bool>> funcs, Action<int, string> progressTracker, bool weight, float weightZoom)
        {
            if (templateName.Contains("vector"))
            {
                string[] split = templateName.Split(':');
                string key = split[0];
                string shpFilename = split[1];
                string shpFieldName = split[2];
                RasterStatByVector<T> v = new RasterStatByVector<T>(progressTracker);
                return v.CountAndPercentByVector(rasterFile, shpFilename, shpFieldName, funcs, weight, weightZoom);
            }
            else
            {
                throw new NotSupportedException("暂未实现除基于矢量面的分段面积百分比统计");
            }
        }

        /// <summary>
        ///  平均值统计（分行政区域或其他）：
        ///  =行政区划内各像元值相加/行政区划像素个数
        /// </summary>
        /// <param name="rasterFilename"></param>
        /// <param name="templateName"></param>
        /// <param name="func"></param>
        /// <param name="progressTracker"></param>
        /// <returns>
        /// SortedDictionary<string, double[3]>
        /// key:各矢量名字
        /// value:double[0],满足条件的像元和，double[1]，满足条件的像元个数，double[2]，当前矢量面的像元个数。
        /// </returns>
        public static SortedDictionary<string, double[]> SumAndCountByVector<T>(string rasterFile, string templateName, Func<T, bool> func, Action<int, string> progressTracker)
        {
            if (templateName.Contains("vector"))
            {
                string[] split = templateName.Split(':');
                string key = split[0];
                string shpFilename = split[1];
                string shpFieldName = split[2];
                RasterStatByVector<T> v = new RasterStatByVector<T>(progressTracker);
                return v.SumAndCountByVector(rasterFile, shpFilename, shpFieldName, func);
            }
            else
            {
                throw new NotSupportedException("暂未实现除基于矢量面的分段面积百分比统计");
            }
        }

    }
}
