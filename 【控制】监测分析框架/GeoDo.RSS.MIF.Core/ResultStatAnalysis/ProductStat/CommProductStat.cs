using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using CodeCell.Bricks.UIs;
using CodeCell.AgileMap.Core;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace GeoDo.RSS.MIF.Core
{

    public class CommProductStat
    {
        public static StatResultItem[] AreaStat<T>(string productName, string fname, ref string title, object aoiObj, Func<T, bool> filter)
        {
            if (string.IsNullOrEmpty(fname))
                return null;
            IStatAnalysisEngine<T> exe = new StatAnalysisEngine<T>();
            if (aoiObj == null)
            {
                title = productName + "按当前区域面积统计";
                return AreaStatByType<T>(productName, fname, "当前区域", title, filter);
            }
            else
            {
                if (aoiObj as Dictionary<string, int[]> != null)
                {
                    Dictionary<string, int[]> aoi = aoiObj as Dictionary<string, int[]>;
                    title = productName + "自定义面积统计";
                    return AreaStatCustom<T>(productName, fname, title, aoi, filter);
                }
                else
                {
                    title = productName + "按" + aoiObj.ToString() + "面积统计";
                    return AreaStatByType<T>(productName, fname, aoiObj.ToString(), title, filter);
                }
            }
        }

        /// <summary>
        /// 按类型进行进行面积统计
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="productName">产品名称:大雾、海冰、沙尘 ect</param>
        /// <param name="session"></param>
        /// <param name="statType">统计类型：当前区域 | 省级行政区划 | 土地利用类型</param>
        /// <param name="title">图表标题</param>
        /// <param name="filter">过滤条件 如:(v)=>{return v==1;}</param>
        private static StatResultItem[] AreaStatByType<T>(string productName, string fname, string statType, string title, Func<T, bool> filter)
        {
            IStatAnalysisEngine<T> exe = new StatAnalysisEngine<T>();
            if (string.IsNullOrEmpty(title))
                title = productName + "按" + statType + "面积统计";
            if (string.IsNullOrEmpty(fname))
                return null;
            StatResultItem[] items = null;
            IRasterDataProvider prd = null;
            try
            {
                prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
                if (statType == "当前区域")
                    items = AreaStatCurrentRegion<T>(prd, title, filter);
                else
                    items = exe.StatArea(prd, statType, filter);
                return items;
            }
            catch
            {
                throw new ArgumentException("选择的文件：“" + Path.GetFileName(fname) + "”无法进行面积统计。");
            }
            finally
            {
                if (prd != null)
                {
                    prd.Dispose();
                    prd = null;
                }
            }
        }

        private static StatResultItem[] AreaStatCurrentRegion<T>(IRasterDataProvider prd, string title, Func<T, bool> filter)
        {
            RasterOperator<T> oper = new RasterOperator<T>();
            int count = oper.Count(prd, null, filter);
            StatResultItem sri = new StatResultItem();
            sri.Name = "当前区域";
            //精细面积计算
            //sri.Value = oper.Area(prd, null, filter);
            double lon = prd.CoordEnvelope.Center.X;
            double lat = prd.CoordEnvelope.Center.Y;
            sri.Value = Math.Round(count * AreaCountHelper.CalcArea(lon, lat, prd.ResolutionX, prd.ResolutionX) / Math.Pow(10, 6), 3);
            return new StatResultItem[] { sri };
        }

        /// <summary>
        /// 自定义统计
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="productName">产品名称:大雾、海冰、沙尘 ect</param>
        /// <param name="session"></param>
        /// <param name="title">图表标题</param>
        /// <param name="filter">过滤条件 如:(v)=>{return v==1;}</param>
        private static StatResultItem[] AreaStatCustom<T>(string productName, string fname, string title, Dictionary<string, int[]> aoi, Func<T, bool> filter)
        {
            IStatAnalysisEngine<T> exe = new StatAnalysisEngine<T>();
            if (string.IsNullOrEmpty(title))
                title = productName + "面积统计";
            if (string.IsNullOrEmpty(fname))
                return null;
            StatResultItem[] items = null;
            IRasterDataProvider prd = null;
            try
            {
                prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
                if (aoi == null || aoi.Count == 0)
                    return null;
                items = exe.StatAreaCustom(prd, aoi, filter);
                return items;
            }
            catch
            {
                throw new ArgumentException("选择的文件：“" + Path.GetFileName(fname) + "”无法进行自定义面积统计。");
            }
            finally
            {
                if (prd != null)
                {
                    prd.Dispose();
                    prd = null;
                }
            }
        }

        #region weight

        public static StatResultItem[] AreaStat<T>(string productName, string fname, ref string title, object aoiObj, Func<T, int,int> weight)
        {
            if (string.IsNullOrEmpty(fname))
                return null;
            IStatAnalysisEngine<T> exe = new StatAnalysisEngine<T>();
            if (aoiObj == null)
            {
                title = productName + "按当前区域面积统计";
                return AreaStatByType<T>(productName, fname, "当前区域", title, weight);
            }
            else
            {
                if (aoiObj as Dictionary<string, int[]> != null)
                {
                    Dictionary<string, int[]> aoi = aoiObj as Dictionary<string, int[]>;
                    title = productName + "自定义面积统计";
                    return AreaStatCustom<T>(productName, fname, title, aoi, weight);
                }
                else
                {
                    title = productName + "按" + aoiObj.ToString() + "面积统计";
                    return AreaStatByType<T>(productName, fname, aoiObj.ToString(), title, weight);
                }
            }
        }

        /// <summary>
        /// 按类型进行进行面积统计
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="productName">产品名称:大雾、海冰、沙尘 ect</param>
        /// <param name="session"></param>
        /// <param name="statType">统计类型：当前区域 | 省级行政区划 | 土地利用类型</param>
        /// <param name="title">图表标题</param>
        /// <param name="weight">过滤条件 如:(v)=>{return v==1;}</param>
        private static StatResultItem[] AreaStatByType<T>(string productName, string fname, string statType, string title, Func<T, int, int> weight)
        {
            IStatAnalysisEngine<T> exe = new StatAnalysisEngine<T>();
            if (string.IsNullOrEmpty(title))
                title = productName + "按" + statType + "面积统计";
            if (string.IsNullOrEmpty(fname))
                return null;
            StatResultItem[] items = null;
            IRasterDataProvider prd = null;
            try
            {
                prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
                if (statType == "当前区域")
                    items = AreaStatCurrentRegion<T>(prd, title, weight);
                else
                    items = exe.StatArea(prd, statType, weight);
                return items;
            }
            catch
            {
                throw new ArgumentException("选择的文件：“" + Path.GetFileName(fname) + "”无法进行面积统计。");
            }
            finally
            {
                if (prd != null)
                {
                    prd.Dispose();
                    prd = null;
                }
            }
        }

        private static StatResultItem[] AreaStatCurrentRegion<T>(IRasterDataProvider prd, string title, Func<T, int, int> weight)
        {
            RasterOperator<T> oper = new RasterOperator<T>();
            int count = oper.Count(prd, null, weight);
            StatResultItem sri = new StatResultItem();
            sri.Name = "当前区域";
            //精细面积计算
            //sri.Value = oper.Area(prd, null, filter);
            double lon = prd.CoordEnvelope.Center.X;
            double lat = prd.CoordEnvelope.Center.Y;
            sri.Value = Math.Round(count * AreaCountHelper.CalcArea(lon, lat, prd.ResolutionX, prd.ResolutionX) / Math.Pow(10, 6), 3);
            return new StatResultItem[] { sri };
        }

        /// <summary>
        /// 自定义统计
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="productName">产品名称:大雾、海冰、沙尘 ect</param>
        /// <param name="session"></param>
        /// <param name="title">图表标题</param>
        /// <param name="weight">过滤条件 如:(v)=>{return v==1;}</param>
        private static StatResultItem[] AreaStatCustom<T>(string productName, string fname, string title, Dictionary<string, int[]> aoi, Func<T, int, int> weight)
        {
            IStatAnalysisEngine<T> exe = new StatAnalysisEngine<T>();
            if (string.IsNullOrEmpty(title))
                title = productName + "面积统计";
            if (string.IsNullOrEmpty(fname))
                return null;
            StatResultItem[] items = null;
            IRasterDataProvider prd = null;
            try
            {
                prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
                if (aoi == null || aoi.Count == 0)
                    return null;
                items = exe.StatAreaCustom(prd, aoi, weight);
                return items;
            }
            catch
            {
                throw new ArgumentException("选择的文件：“" + Path.GetFileName(fname) + "”无法进行自定义面积统计。");
            }
            finally
            {
                if (prd != null)
                {
                    prd.Dispose();
                    prd = null;
                }
            }
        }

        #endregion
    }
}
