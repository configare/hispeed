using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 通过栅格分类数据进行统计
    /// </summary>
    public class RasterStatByRaster
    {
        private double _perAreas;
        private Action<int, string> _progressTracker;
        private SortedDictionary<string, StatAreaItem> _multiFileStat = new SortedDictionary<string, StatAreaItem>();
        private static string RASTER_TEMPLATE = AppDomain.CurrentDomain.BaseDirectory + "SystemData\\RasterTemplate";

        public RasterStatByRaster(Action<int, string> progressTracker)
        {
            _progressTracker = progressTracker;
            _perAreas = AreaCountHelper.CalcArea(110, 35, 0.01, 0.01) * Math.Pow(10, -6);
        }

        private void UpdateProgress(int p, string text)
        {
            if (_progressTracker != null)
                _progressTracker(p, text);
        }

        #region 单文件统计
        /// <summary>
        /// 单文件统计面积
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rasterFile"></param>
        /// <param name="templateName"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public SortedDictionary<string, double> Stat<T>(string rasterFile, string templateName, Func<T, bool> func)
        {
            if (string.IsNullOrWhiteSpace(rasterFile))
                return null;
            IRasterDataProvider dRaster = null;
            try
            {
                dRaster = RasterDataDriver.Open(rasterFile) as IRasterDataProvider;
                if (dRaster == null)
                    return null;
                return Stat<T>(dRaster, templateName, func);
            }
            finally
            {
                if (dRaster != null)
                    dRaster.Dispose();
            }
        }

        /// <summary>
        /// 基于栅格分类的统计
        /// 输出按分类的覆盖面积
        /// </summary>
        /// <typeparam name="T">待统计数据类型</typeparam>
        /// <param name="raster">待统计数据</param>
        /// <param name="templateName">栅格分类模版（统计分类）,如县界\土地类型</param>
        /// <param name="func">统计条件</param>
        public SortedDictionary<string, double> Stat<T>(IRasterDataProvider raster, string templateName, Func<T, bool> func)
        {
            if (raster == null)
                return null;
            _perAreas = AreaCountHelper.CalcArea(raster.CoordEnvelope.Center.X, raster.CoordEnvelope.Center.Y, raster.ResolutionX, raster.ResolutionY) * Math.Pow(10, -6);
            string tempFile = FindRasterFullname(templateName);
            if (templateName == "China_LandRaster")
            {
                return StatRasterTemplate<T>(raster, tempFile, func);
            }
            else
            {
                SortedDictionary<string, double> statResult = StatRasterTemplate<T>(raster, tempFile, func);
                SortedDictionary<string, double> newDic = ToSSXLevel(statResult);//县级统计，向上推至省市级。
                return newDic;
            }
        }

        private SortedDictionary<string, double> StatRasterTemplate<T>(IRasterDataProvider raster, string tempFile, Func<T, bool> func)//计算最大覆盖
        {
            using (IRasterDataProvider tempRaster = RasterDataDriver.Open(tempFile) as IRasterDataProvider)
            {
                SortedDictionary<string, double> result = new SortedDictionary<string, double>();
                StatRasterTemplate(raster, tempRaster, func, ref result);
                return result;
            }
        }

        private void StatRasterTemplate<T>(IRasterDataProvider raster, IRasterDataProvider tempRaster, Func<T, bool> func, ref  SortedDictionary<string, double> statResult)//计算最大覆盖
        {
            try
            {
                //计算待统计栅格与分类栅格的相交区域,以创建相同的虚拟栅格。
                CoordEnvelope jxEnv = raster.CoordEnvelope.Intersect(tempRaster.CoordEnvelope);
                VirtualRasterHeader vHeader = VirtualRasterHeader.Create(jxEnv, raster.Width, raster.Height);
                VirtualRaster vRaster = new VirtualRaster(raster, vHeader);
                VirtualRaster template = new VirtualRaster(tempRaster, vHeader);
                //2、依据逻辑，计算初步统计结果,这一步对于大数据，可考虑分块处理

                T[] vdata = vRaster.ReadData<T>(1, 0, 0, vHeader.Width, vHeader.Height);
                int[] typeData = template.ReadData<int>(1, 0, 0, vHeader.Width, vHeader.Height);
                int length = typeData.Length;
                for (int i = 0; i < length; i++)
                {
                    UpdateProgress((int)(i * 100f / length), "正在计算统计数据");
                    //统计满足条件的栅格，并将计数值加入字典。
                    if (func(vdata[i]))
                    {
                        int tvalue = typeData[i];
                        AddUpToDic(statResult, tvalue.ToString(), _perAreas);
                    }
                }
                //3、直接返回初步统计结果，或者直接进一步处理
                //SortedDictionary<int, double> newDic = ToSSXLevel(statResult);
                //return newDic;
            }
            finally
            {
                UpdateProgress(100, "统计数据计算完毕");
            }
        }

        private SortedDictionary<string, double> ToSSXLevel(SortedDictionary<string, double> statResult)
        {
            SortedDictionary<string, double> newDic = new SortedDictionary<string, double>();
            string[] keys = statResult.Keys.ToArray();
            for (int i = 0; i < statResult.Count; i++)
            {
                string key = keys[i];
                if (key == "0")
                    continue;
                string keyString = key.ToString();
                if (keyString.Length != 6)
                {
                    AddUpToDic(newDic, key, statResult[key]);//省级单位
                }
                else if (keyString.Substring(2, 4) == "0000")
                {
                    AddUpToDic(newDic, key, statResult[key]);
                }
                else if (keyString.Substring(4, 2) == "00")
                {
                    AddUpToDic(newDic, key, statResult[key]);//市级单位
                    //将该值加至省级单位
                    string sj = keyString.Substring(0, 2) + "0000";
                    AddUpToDic(newDic, sj, statResult[key]);
                }
                else if (keyString.Substring(4, 2) != "00")
                {
                    AddUpToDic(newDic, key, statResult[key]);//县级级单位
                    //将该值加至市级单位
                    string shiJi = keyString.Substring(0, 4) + "00";
                    AddUpToDic(newDic, shiJi, statResult[key]);
                    //将该值加至省级单位
                    string shengJi = keyString.Substring(0, 2) + "0000";
                    AddUpToDic(newDic, shengJi, statResult[key]);
                }
            }
            return newDic;
        }

        private void AddUpToDic(IDictionary<string, double> newDic, string key, double addValue)
        {
            if (newDic.ContainsKey(key))
                newDic[key] += addValue;
            else
                newDic.Add(key, addValue);
        }

        private void AddUpToDic(IDictionary<string, double> newDic, string[] featureNames, string key, string key2, double addValue)
        {
            if (newDic.ContainsKey(key))
                newDic[key] += addValue;
            else
                newDic.Add(key, addValue);

            if (featureNames != null && featureNames.Contains(key2) && newDic.ContainsKey(key + ":" + featureNames[0]))
                newDic[key + ":" + featureNames[0]] += addValue;
            else if (!string.IsNullOrEmpty(key2) && newDic.ContainsKey(key + ":" + key2))
                newDic[key + ":" + key2] += addValue;
            else
            {
                if (featureNames != null && featureNames.Length != 0 && featureNames.Contains(key2))
                    newDic.Add(key + ":" + featureNames[0], addValue);
                else if (featureNames == null && !string.IsNullOrEmpty(key2))
                    newDic.Add(key + ":" + key2, addValue);
                else if (featureNames != null && featureNames.Length != 0 && !newDic.ContainsKey(key + ":" + featureNames[0]))
                    newDic.Add(key + ":" + featureNames[0], 0);
            }
        }

        #endregion

        #region 多文件面积统计,单条件[覆盖面积、累计面积]
        /// <summary>
        /// 多文件统计覆盖面积和累积面积
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rasterFiles"></param>
        /// <param name="templateName">China_XjRaster|China_LandRaster</param>
        /// <param name="func"></param>
        /// <returns></returns>
        public SortedDictionary<string, StatAreaItem> Stat<T>(string[] rasterFiles, string templateName, Func<T, bool> func, bool isCombinSameDay)
        {
            List<IRasterDataProvider> rasters = new List<IRasterDataProvider>();
            try
            {
                for (int i = 0; i < rasterFiles.Length; i++)
                {
                    IRasterDataProvider dRaster = RasterDataDriver.Open(rasterFiles[i]) as IRasterDataProvider;
                    if (dRaster != null)
                        rasters.Add(dRaster);
                }
                if (rasters.Count == 0)
                    return null;
                return Stat<T>(rasters.ToArray(), templateName, func, isCombinSameDay);
            }
            catch
            {
                return null;
            }
            finally
            {
                for (int i = 0; i < rasters.Count; i++)
                {
                    rasters[i].Dispose();
                }
            }
        }

        /// <summary>
        /// 覆盖面积：指的是覆盖到的面积，
        /// 累计面积：即累计覆盖面积，相同区域不同时次的累计计算
        /// </summary>
        /// <typeparam name="T">待统计数据类型</typeparam>
        /// <param name="raster">待统计数据</param>
        /// <param name="templateName">栅格分类模版（统计分类）China_XjRaster|China_LandRaster</param>
        /// <param name="func">统计条件</param>
        public SortedDictionary<string, StatAreaItem> Stat<T>(IRasterDataProvider[] rasters, string templateName, Func<T, bool> func, bool isCombinSameDay)
        {
            if (rasters == null || rasters.Length == 0)
                return null;
            string tempFile = FindRasterFullname(templateName);
            SortedDictionary<string, StatAreaItem> statResult = StatTemplate<T>(rasters, tempFile, func, isCombinSameDay);
            SortedDictionary<string, StatAreaItem> newDic = ToSSXLevel(statResult);//县级统计，向上推至省市级。
            return newDic;
        }

        private SortedDictionary<string, StatAreaItem> ToSSXLevel(SortedDictionary<string, StatAreaItem> statResult)
        {
            SortedDictionary<string, StatAreaItem> newDic = new SortedDictionary<string, StatAreaItem>();
            string[] keys = statResult.Keys.ToArray();
            for (int i = 0; i < statResult.Count; i++)
            {
                string key = keys[i];
                if (key == "0")
                    continue;
                string keyString = key.ToString();
                if (keyString.Length != 6)
                {
                    AddUpToDic(newDic, key, statResult[key]);//省级单位
                }
                else if (keyString.Substring(2, 4) == "0000")
                {
                    AddUpToDic(newDic, key, statResult[key]);
                }
                else if (keyString.Substring(4, 2) == "00")
                {
                    AddUpToDic(newDic, key, statResult[key]);//市级单位
                    //将该值加至省级单位
                    string sj = keyString.Substring(0, 2) + "0000";
                    AddUpToDic(newDic, sj, statResult[key]);
                }
                else if (keyString.Substring(4, 2) != "00")
                {
                    AddUpToDic(newDic, key, statResult[key]);//县级级单位
                    //将该值加至市级单位
                    string shiJi = keyString.Substring(0, 4) + "00";
                    AddUpToDic(newDic, shiJi, statResult[key]);
                    //将该值加至省级单位
                    string shengJi = keyString.Substring(0, 2) + "0000";
                    AddUpToDic(newDic, shengJi, statResult[key]);
                }
            }
            return newDic;
        }

        private void AddUpToDic(SortedDictionary<string, StatAreaItem> newDic, string key, StatAreaItem addValue)
        {
            if (newDic.ContainsKey(key))
            {
                newDic[key].Cover += addValue.Cover;
                newDic[key].GrandTotal += addValue.GrandTotal;
            }
            else
            {
                newDic.Add(key, new StatAreaItem(addValue.Cover, addValue.GrandTotal));
            }
        }

        private SortedDictionary<string, StatAreaItem> StatTemplate<T>(IRasterDataProvider[] rasters, string tempFile, Func<T, bool> func, bool isCombinSameDay)//计算最大覆盖
        {
            using (IRasterDataProvider tempRaster = RasterDataDriver.Open(tempFile) as IRasterDataProvider)
            {
                return StatTemplate<T>(rasters, tempRaster, func, isCombinSameDay);
            }
        }

        /// <summary>
        /// 覆盖面积：指的是覆盖到的面积，
        /// 累计面积：即累计覆盖面积，相同区域不同时次的累计计算
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rasters"></param>
        /// <param name="rasterTemplate"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private SortedDictionary<string, StatAreaItem> StatTemplate<T>(IRasterDataProvider[] rasters, IRasterDataProvider typeRaster, Func<T, bool> func, bool isCombinSameDay)
        {
            try
            {
                //01、计算所有输入栅格的的范围并集
                CoordEnvelope maxEnv = UnionEnv(rasters);
                //计算待统计栅格与分类栅格的相交区域,以创建相同的虚拟栅格。
                CoordEnvelope virtureEnv = maxEnv.Intersect(typeRaster.CoordEnvelope);
                VirtualRasterHeader vHeader = VirtualRasterHeader.Create(virtureEnv, rasters[0].ResolutionX, rasters[0].ResolutionX);

                VirtualRaster typelate = new VirtualRaster(typeRaster, vHeader);
                int[] tdata = typelate.ReadData<int>(1, 0, 0, vHeader.Width, vHeader.Height);


                //2、依据逻辑，计算初步统计结果,这一步对于大数据，可考虑分块处理
                int calcLength = vHeader.Width * vHeader.Height * rasters.Length;
                int dataLength = vHeader.Width * vHeader.Height;
                byte[] valueCalced = new byte[dataLength];

                SortedDictionary<string, StatAreaItem> result = new SortedDictionary<string, StatAreaItem>();
                if (isCombinSameDay)
                {
                    //预处理同天数据
                    Dictionary<DateTime, List<IRasterDataProvider>> dic = new Dictionary<DateTime, List<IRasterDataProvider>>();
                    foreach (IRasterDataProvider raster in rasters)
                    {
                        if (raster.DataIdentify.OrbitDateTime == DateTime.MinValue)
                        {
                            RasterIdentify identify = new RasterIdentify(raster.fileName);
                            raster.DataIdentify.OrbitDateTime = identify.OrbitDateTime;
                        }
                        List<IRasterDataProvider> lst;
                        if (dic.TryGetValue(raster.DataIdentify.OrbitDateTime, out lst))
                        {
                            dic[raster.DataIdentify.OrbitDateTime].Add(raster);
                        }
                        else
                        {
                            lst = new List<IRasterDataProvider>();
                            lst.Add(raster);
                            dic.Add(raster.DataIdentify.OrbitDateTime, lst);
                        }
                    }
                    foreach (DateTime dateKey in dic.Keys)
                    {
                        IRasterDataProvider[] rastersSameDay = dic[dateKey].ToArray();
                        if (rastersSameDay.Length == 1 || dateKey == DateTime.MinValue)
                        {
                            for (int i = 0; i < rastersSameDay.Length; i++)
                            {
                                VirtualRaster vRaster = new VirtualRaster(rastersSameDay[i], vHeader);
                                T[] datas = vRaster.ReadData<T>(1, 0, 0, vHeader.Width, vHeader.Height);
                                for (int j = 0; j < dataLength; j++)
                                {
                                    UpdateProgress((int)((j + (i * dataLength)) * 100f / calcLength), "正在计算统计数据");
                                    if (func(datas[j]))
                                    {
                                        string key = tdata[j].ToString();
                                        if (result.ContainsKey(key))//累计计数
                                            result[key].GrandTotal += 1;
                                        else
                                            result.Add(key, new StatAreaItem() { GrandTotal = 1 });
                                        if (valueCalced[j] == 0)//覆盖计数
                                        {
                                            valueCalced[j] = 1;
                                            result[key].Cover += 1;
                                        }
                                    }
                                }
                            }
                        }
                        else//同天数据处理,
                        {
                            byte[] sameDayCalced = new byte[vHeader.Width * vHeader.Height];
                            for (int i = 0; i < rasters.Length; i++)
                            {
                                VirtualRaster vRaster = new VirtualRaster(rasters[i], vHeader);
                                T[] datas = vRaster.ReadData<T>(1, 0, 0, vHeader.Width, vHeader.Height);
                                for (int j = 0; j < dataLength; j++)
                                {
                                    UpdateProgress((int)((j + (i * dataLength)) * 100f / calcLength), "正在计算统计数据");
                                    if (func(datas[j]))
                                    {
                                        if (sameDayCalced[j] == 0)//
                                        {
                                            sameDayCalced[j] = 1;
                                            string key = tdata[j].ToString();
                                            if (result.ContainsKey(key))//累计计数
                                                result[key].GrandTotal += 1;
                                            else
                                                result.Add(key, new StatAreaItem() { GrandTotal = 1 });
                                            if (valueCalced[j] == 0)//覆盖计数
                                            {
                                                valueCalced[j] = 1;
                                                result[key].Cover += 1;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < rasters.Length; i++)
                    {
                        VirtualRaster vRaster = new VirtualRaster(rasters[i], vHeader);
                        T[] datas = vRaster.ReadData<T>(1, 0, 0, vHeader.Width, vHeader.Height);
                        for (int j = 0; j < dataLength; j++)
                        {
                            UpdateProgress((int)((j + (i * dataLength)) * 100f / calcLength), "正在计算统计数据");
                            if (func(datas[j]))
                            {
                                string key = tdata[j].ToString();
                                if (result.ContainsKey(key))//累计计数
                                    result[key].GrandTotal += 1;
                                else
                                    result.Add(key, new StatAreaItem() { GrandTotal = 1 });
                                if (valueCalced[j] == 0)//覆盖计数
                                {
                                    valueCalced[j] = 1;
                                    result[key].Cover += 1;
                                }
                            }
                        }
                    }
                }

                double perAreas = AreaCountHelper.CalcArea(vHeader.CoordEnvelope.Center.X, vHeader.CoordEnvelope.Center.Y, vHeader.ResolutionX, vHeader.ResolutionY) * Math.Pow(10, -6);
                foreach (string key in result.Keys)
                {
                    result[key].GrandTotal *= perAreas;
                    result[key].Cover *= perAreas;
                }
                return result;
            }
            finally
            {
                UpdateProgress(100, "统计数据计算完毕");
            }
        }

        private SortedDictionary<string, StatAreaItem> StatTemplate<T>(IRasterDataProvider[] rasters, IRasterDataProvider tempRaster, Func<T, bool> func)
        {
            try
            {
                //01、计算所有输入栅格的的范围并集
                CoordEnvelope maxEnv = UnionEnv(rasters);
                //计算待统计栅格与分类栅格的相交区域,以创建相同的虚拟栅格
                CoordEnvelope virtureEnv = maxEnv.Intersect(tempRaster.CoordEnvelope);
                VirtualRasterHeader vHeader = VirtualRasterHeader.Create(virtureEnv, rasters[0].ResolutionX, rasters[0].ResolutionX);
                List<VirtualRaster> vRasters = new List<VirtualRaster>();
                for (int i = 0; i < rasters.Length; i++)
                {
                    VirtualRaster vRaster = new VirtualRaster(rasters[i], vHeader);
                    vRasters.Add(vRaster);
                }
                VirtualRaster template = new VirtualRaster(tempRaster, vHeader);
                int[] tdata = template.ReadData<int>(1, 0, 0, vHeader.Width, vHeader.Height);
                //2、依据逻辑，计算初步统计结果,这一步对于大数据，可考虑分块处理
                int calcLength = vHeader.Width * vHeader.Height * vRasters.Count;
                int dataLength = vHeader.Width * vHeader.Height;
                byte[] valueCalced = new byte[dataLength];
                SortedDictionary<string, StatAreaItem> result = new SortedDictionary<string, StatAreaItem>();
                for (int i = 0; i < vRasters.Count; i++)
                {
                    T[] datas = vRasters[i].ReadData<T>(1, 0, 0, vHeader.Width, vHeader.Height);
                    for (int j = 0; j < dataLength; j++)
                    {
                        UpdateProgress((int)((j + (i * dataLength)) * 100f / calcLength), "正在计算统计数据");
                        if (func(datas[j]))
                        {
                            string key = tdata[j].ToString();
                            if (result.ContainsKey(key))//累计计数
                                result[key].GrandTotal += 1;
                            else
                                result.Add(key, new StatAreaItem() { GrandTotal = 1 });
                            if (valueCalced[j] == 0)//覆盖计数
                            {
                                valueCalced[j] = 1;
                                result[key].Cover += 1;
                            }
                        }
                    }
                }

                double perAreas = AreaCountHelper.CalcArea(vHeader.CoordEnvelope.Center.X, vHeader.CoordEnvelope.Center.Y, vHeader.ResolutionX, vHeader.ResolutionY) * Math.Pow(10, -6);
                foreach (string key in result.Keys)
                {
                    result[key].GrandTotal *= perAreas;
                    result[key].Cover *= perAreas;
                }
                return result;
            }
            finally
            {
                UpdateProgress(100, "统计数据计算完毕");
            }
        }

        private CoordEnvelope UnionEnv(IRasterDataProvider[] raster)
        {
            CoordEnvelope env = raster[0].CoordEnvelope;
            for (int i = 1; i < raster.Length; i++)
            {
                env = raster[i].CoordEnvelope.Union(env);
            }
            return env;
        }

        #endregion

        private Dictionary<string, SortedDictionary<string, double>> StatRaster<T>(IRasterDataProvider raster, Dictionary<string, Func<T, bool>> multiFilter) where T : struct, IConvertible
        {
            return StatRaster<T>(raster, multiFilter, false, 1);
        }

        /// <summary>
        /// 多条件面积统计,[当前区域]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="raster"></param>
        /// <param name="multiFilter"></param>
        /// <returns></returns>
        private Dictionary<string, SortedDictionary<string, double>> StatRaster<T>(IRasterDataProvider raster, Dictionary<string, Func<T, bool>> multiFilter, bool weight, float weightZoom) where T : struct, IConvertible
        {
            try
            {
                int filterLength = multiFilter.Count;
                string[] filterKeys = multiFilter.Keys.ToArray();
                Func<T, bool>[] filters = new Func<T, bool>[filterLength];
                double[] areas = new double[filterLength];
                for (int i = 0; i < filterLength; i++)
                {
                    filters[i] = multiFilter[filterKeys[i]];
                }
                //计算待统计栅格与分类栅格的相交区域,以创建相同的虚拟栅格。
                CoordEnvelope jxEnv = raster.CoordEnvelope;
                VirtualRasterHeader vHeader = VirtualRasterHeader.Create(jxEnv, raster.Width, raster.Height);
                VirtualRaster vRaster = new VirtualRaster(raster, vHeader);
                //2、依据逻辑，计算初步统计结果,这一步对于大数据，可考虑分块处理
                T[] vdata = vRaster.ReadData<T>(1, 0, 0, vHeader.Width, vHeader.Height);
                int length = vdata.Length;
                for (int i = 0; i < length; i++)
                {
                    UpdateProgress((int)(i * 100f / length), "正在计算统计数据");
                    //统计满足条件的栅格，并将计数值加入字典。
                    for (int f = 0; f < filterLength; f++)
                    {
                        if (filters[f](vdata[i]))
                        {
                            areas[f] += weight ? _perAreas * Convert.ToSingle(vdata[i]) / weightZoom : _perAreas;
                            //break;
                        }
                    }
                }
                SortedDictionary<string, double> dic = new SortedDictionary<string, double>();
                for (int i = 0; i < filterLength; i++)
                {
                    dic.Add(filterKeys[i], areas[i]);
                }
                Dictionary<string, SortedDictionary<string, double>> result = new Dictionary<string, SortedDictionary<string, double>>();
                result.Add("当前区域", dic);
                return result;
            }
            finally
            {
                UpdateProgress(100, "统计数据计算完毕");
            }
        }

        #region 测试
        static SortedDictionary<string, StatAreaItem> dic = null;
        static SortedDictionary<string, StatAreaItem> dic2 = null;
        static Dictionary<string, SortedDictionary<string, double>> dic3 = null;
        public static void totest()
        {
            //RasterStatByVector<short>.test();
            //return;
            string dblv = @"D:\WorkFolder\Smart二期\1.联机开发@Smart\SMART\bin\Release\Workspace\FLD\2013-01-17\栅格产品\FLD_0MIX_洞庭湖流域._FY3A_MERSI_0250M_20120517023500.dat";
            dblv = @"D:\WorkFolder\Smart二期\1.联机开发@Smart\SMART\bin\Release\Workspace\FLD\2013-03-27\栅格产品\FLD_DBLV_FY3A_MERSI_1000M_20120328014000.dat";
            dblv = @"D:\WorkFolder\Smart二期\1.联机开发@Smart\SMART\bin\Release\Workspace\FLD\2013-03-30\栅格产品\FLD_DBLV_FY3B_VIRR_1000M_20120427054500.dat";
            string dblv2 = @"D:\WorkFolder\Smart二期\1.联机开发@Smart\SMART\bin\Release\Workspace\FLD\2013-03-30\栅格产品\FLD_DBLV_FY3B_VIRR_1000M_20120427054500.dat";
            dblv = @"D:\WorkFolder\Smart二期\1.联机开发@Smart\SMART\bin\Release\Workspace\DST\2013-03-30\栅格产品\DST_DBLV_FY3A_VIRR_1000M_20120322051500.dat";
            dblv2 = @"D:\WorkFolder\Smart二期\1.联机开发@Smart\SMART\bin\Release\Workspace\DST\2013-03-30\栅格产品\DST_DBLV_FY3B_VIRR_1000M_20120427054500.dat";
            dblv = @"D:\WorkFolder\Smart二期\1.联机开发@Smart\SMART\bin\Release\Workspace\FLD\2013-04-01\栅格产品\FLD_DBLV_洞庭湖流域._FY3A_MERSI_0250M_20120517023500.dat";
            dblv2 = @"E:\Smart\staterror\FLD_DBLV_EOST_MODIS_NULL_20110517025700.dat";
            dblv = @"E:\Smart\staterror\FLD_DBLV_洞庭湖流域._FY3A_MERSI_0250M_20120624022000.dat";
            dblv2 = @"D:\WorkFolder\Smart二期\1.联机开发@Smart\SMART\bin\Release\Workspace\DST\2013-04-08\栅格产品\DST_VISY_FY3A_VIRR_1000M_20120322051500.dat";
            string[] statf = new string[] { dblv };
            //string vct = "vector:省级行政区域_面.shp:NAME";

            int p = 0;
            Action<int, string> progress = (i, t) =>
            {
                if (p < i)
                {
                    p = i;
                    Console.WriteLine(i + t);
                }
            };
            Dictionary<string, Func<short, bool>> dics = new Dictionary<string, Func<short, bool>>();
            dics.Add("等于1", new Func<short, bool>((t) => { return t == 1; }));
            dics.Add("等于0", new Func<short, bool>((t) => { return t == 0; }));

            RasterStatByRaster rsm = new RasterStatByRaster(progress);
            dic3 = rsm.Stat<short>(dblv, "China_XjRaster", dics);

            string vct = "vector:土地利用类型_合并.shp:NAME";
            p = 0;
            dic2 = RasterStatFactory.Stat<short>(new string[] { dblv, dblv2 }, vct, new Func<short, bool>((t) => { return t == 1; }), progress, false);

            RasterStatFactory.Stat<short>(dblv, "raster:China_XjRaster", dics, progress);
        }
        #endregion

        #region 单文件、多条件、栅格分类，面积统计

        internal Dictionary<string, SortedDictionary<string, double>> Stat<T>(string rasterFile, string templateName, Dictionary<string, Func<T, bool>> multiFilter) where T : struct, IConvertible
        {
            return Stat<T>(rasterFile, templateName, multiFilter, false, 1);
        }

        internal Dictionary<string, SortedDictionary<string, double>> Stat<T>(string rasterFile, string templateName, Dictionary<string, Func<T, bool>> multiFilter, bool weight, float weightZoom, string vectorTemplate) where T : struct, IConvertible
        {
            if (string.IsNullOrWhiteSpace(rasterFile))
                return null;
            using (IRasterDataProvider dRaster = RasterDataDriver.Open(rasterFile) as IRasterDataProvider)
            {
                return Stat<T>(dRaster, templateName, multiFilter, weight, weightZoom, vectorTemplate);
            }
        }

        internal Dictionary<string, SortedDictionary<string, double>> Stat<T>(string rasterFile, string templateName, Dictionary<string, Func<T, bool>> multiFilter, bool weight, float weightZoom) where T : struct, IConvertible
        {
            return Stat<T>(rasterFile, templateName, multiFilter, weight, weightZoom, null);
        }

        private Dictionary<string, SortedDictionary<string, double>> Stat<T>(IRasterDataProvider raster, string templateName, Dictionary<string, Func<T, bool>> multiFilter) where T : struct, IConvertible
        {
            return Stat<T>(raster, templateName, multiFilter, false, 1);
        }

        private Dictionary<string, SortedDictionary<string, double>> Stat<T>(IRasterDataProvider raster, string templateName, Dictionary<string, Func<T, bool>> multiFilter, bool weight, float weightZoom) where T : struct, IConvertible
        {
            return Stat<T>(raster, templateName, multiFilter, weight, weightZoom, null);
        }

        private Dictionary<string, SortedDictionary<string, double>> Stat<T>(IRasterDataProvider raster, string templateName, Dictionary<string, Func<T, bool>> multiFilter, bool weight, float weightZoom, string vectorTemplate) where T : struct, IConvertible
        {
            if (raster == null)
                return null;
            if (raster == null)
                return null;
            _perAreas = AreaCountHelper.CalcArea(raster.CoordEnvelope.Center.X, raster.CoordEnvelope.Center.Y, raster.ResolutionX, raster.ResolutionY) * Math.Pow(10, -6);
            if (string.IsNullOrWhiteSpace(templateName))
            {
                return StatRaster(raster, multiFilter, weight, weightZoom);
            }
            string tempFile = FindRasterFullname(templateName);
            if (templateName == "China_LandRaster")
            {
                return StatByRasterTemplate<T>(raster, tempFile, multiFilter, weight, weightZoom);
            }
            else
            {
                Dictionary<string, SortedDictionary<string, double>> statResult = StatByRasterTemplate<T>(raster, tempFile, multiFilter, weight, weightZoom, vectorTemplate);
                //SortedDictionary<string, double> newDic = ToSSXLevel(statResult);   //县级统计，向上推至省市级。
                return statResult;
            }
        }
        #endregion

        private Dictionary<string, SortedDictionary<string, double>> StatByRasterTemplate<T>(IRasterDataProvider raster, string tempFile, Dictionary<string, Func<T, bool>> multiFilter, bool weight, float weightZoom) where T : struct,IConvertible
        {
            return StatByRasterTemplate<T>(raster, tempFile, multiFilter, weight, weightZoom, null);
        }

        private Dictionary<string, SortedDictionary<string, double>> StatByRasterTemplate<T>(IRasterDataProvider raster, string tempFile, Dictionary<string, Func<T, bool>> multiFilter, bool weight, float weightZoom, string vectorTemplate) where T : struct,IConvertible
        {
            using (IRasterDataProvider tempRaster = RasterDataDriver.Open(tempFile) as IRasterDataProvider)
            {
                Dictionary<string, SortedDictionary<string, double>> result = new Dictionary<string, SortedDictionary<string, double>>();
                result = StatByRasterTemplate(raster, tempRaster, multiFilter, weight, weightZoom, vectorTemplate);
                return result;
            }
        }

        private Dictionary<string, SortedDictionary<string, double>> StatByRasterTemplate<T>(IRasterDataProvider raster, IRasterDataProvider tempRaster, Dictionary<string, Func<T, bool>> multiFilter, bool weight, float weightZoom, string vectorTemplate) where T : struct,IConvertible
        {
            try
            {
                Dictionary<string, SortedDictionary<string, double>> result = new Dictionary<string, SortedDictionary<string, double>>();
                string[] filterKeys = multiFilter.Keys.ToArray();
                foreach (string kye in filterKeys)
                {
                    result.Add(kye, new SortedDictionary<string, double>());
                }
                //计算待统计栅格与分类栅格的相交区域,以创建相同的虚拟栅格。
                CoordEnvelope jxEnv = raster.CoordEnvelope.Intersect(tempRaster.CoordEnvelope);
                VirtualRasterHeader vHeader = VirtualRasterHeader.Create(jxEnv, raster.Width, raster.Height);
                VirtualRaster vRaster = new VirtualRaster(raster, vHeader);
                VirtualRaster template = new VirtualRaster(tempRaster, vHeader);
                //2、依据逻辑，计算初步统计结果,这一步对于大数据，可考虑分块处理
                T[] vdata = vRaster.ReadData<T>(1, 0, 0, vHeader.Width, vHeader.Height);
                int[] typeData = template.ReadData<int>(1, 0, 0, vHeader.Width, vHeader.Height);
                int length = typeData.Length;
                if (string.IsNullOrEmpty(vectorTemplate))
                    for (int i = 0; i < length; i++)
                    {
                        UpdateProgress((int)(i * 100f / length), "正在计算统计数据");
                        //统计满足条件的栅格，并将计数值加入字典。
                        foreach (string key in filterKeys)
                        {
                            Func<T, bool> func = multiFilter[key];
                            if (func(vdata[i]))
                            {
                                int tvalue = typeData[i];
                                AddUpToDic(result[key], tvalue.ToString(), weight ? _perAreas * Convert.ToSingle(vdata[i]) / weightZoom : _perAreas);
                            }
                        }
                    }
                else
                {
                    Size size = new Size(raster.Width, raster.Height);
                    string[] typeNames = null;
                    byte[] typeValue = null;
                    string templateName;
                    string[] featureNames = null;
                    if (vectorTemplate.IndexOf(":") != -1)
                    {
                        templateName = vectorTemplate.Substring(0, vectorTemplate.IndexOf(":"));
                        featureNames = vectorTemplate.Substring(vectorTemplate.IndexOf(":") + 1).Split(new char[] { ',' });
                    }
                    else
                        templateName = vectorTemplate;
                    Action act2 = new Action(() => { typeValue = GetLanduseTypeRaster(raster, size, out typeNames, templateName); });
                    Parallel.Invoke(act2);

                    for (int i = 0; i < length; i++)
                    {
                        UpdateProgress((int)(i * 100f / length), "正在计算统计数据");
                        //统计满足条件的栅格，并将计数值加入字典。
                        foreach (string key in filterKeys)
                        {
                            Func<T, bool> func = multiFilter[key];
                            if (func(vdata[i]))
                            {
                                int tvalue = typeData[i];
                                AddUpToDic(result[key], featureNames, tvalue.ToString(), typeNames[typeValue[i]], weight ? _perAreas * Convert.ToSingle(vdata[i]) / weightZoom : _perAreas);
                            }
                        }
                    }
                }
                return result;
            }
            finally
            {
                UpdateProgress(100, "统计数据计算完毕");
            }
        }

        private byte[] GetLanduseTypeRaster(IRasterDataProvider dataProvider, Size size, out string[] landuseTypes, string vectorTemplate)
        {
            VectorAOITemplate temp = VectorAOITemplateFactory.GetAOITemplate(vectorTemplate);
            return temp.GetRaster(GetEnvelope(dataProvider), size, "NAME", out landuseTypes);
        }

        private CodeCell.AgileMap.Core.Envelope GetEnvelope(IRasterDataProvider dataProvider)
        {
            return new CodeCell.AgileMap.Core.Envelope(dataProvider.CoordEnvelope.MinX,
                dataProvider.CoordEnvelope.MinY,
                dataProvider.CoordEnvelope.MaxX,
                dataProvider.CoordEnvelope.MaxY);
        }

        public static string FindRasterFullname(string rasterTemplate)
        {
            if (Directory.Exists(RASTER_TEMPLATE))
            {
                List<string> fnameList = new List<string>();
                fnameList.AddRange(Directory.GetFiles(RASTER_TEMPLATE, "*.dat", SearchOption.AllDirectories));
                fnameList.AddRange(Directory.GetFiles(RASTER_TEMPLATE, "*.ldf", SearchOption.AllDirectories));
                fnameList.AddRange(Directory.GetFiles(RASTER_TEMPLATE, "*.tif", SearchOption.AllDirectories));
                if (fnameList.Count > 0)
                {
                    foreach (string f in fnameList)
                    {
                        if (Path.GetFileName(f).ToUpper() == rasterTemplate.ToUpper())
                        {
                            return Path.GetFullPath(f);
                        }
                    }
                }
            }
            return String.Empty;
        }
    }

    public class RasterTemplateFactory
    {
        public static string[] RasterTemplateNames = new string[] { "土地利用类型", "行政区划" };
        private static IRasterDataProvider _rasterTemplates = null;

        public static IRasterDataProvider GetXjRasterTemplate()
        {
            if (_rasterTemplates != null)
                return _rasterTemplates;
            else
            {
                string file = AppDomain.CurrentDomain.BaseDirectory + "SystemData\\RasterTemplate\\" + "China_XjRaster.dat";
                //string codetxt = "China_XjRaster_Code.txt";
                //_rasterTemplates = new RasterTemplate<int>(file) ;
                return _rasterTemplates;
            }
        }
    }
}
