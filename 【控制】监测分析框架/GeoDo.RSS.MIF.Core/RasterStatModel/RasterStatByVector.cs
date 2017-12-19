using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using CodeCell.AgileMap.Core;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 通过矢量要素执行统计
    /// </summary>
    public class RasterStatByVector<T>
    {
        private Action<int, string> _progress = null;
        public RasterStatByVector(Action<int, string> progress)
        {
            _progress = progress;
        }

        #region 单条件统计 单文件、多文件
        /// <summary>
        /// 单文件只统计覆盖面积
        /// 多文件添加统计累计面积
        /// </summary>
        /// <param name="rasterFiles"></param>
        /// <param name="shpFullname"></param>
        /// <param name="shpPrimaryField"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        //public SortedDictionary<string, StatAreaItem> CountByVector(string[] rasterFiles, string shpFullname, string shpPrimaryField, Func<T, bool> filter)
        //{
        //    return CountByVector(rasterFiles, shpFullname, shpPrimaryField, filter, false);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rasterFiles"></param>
        /// <param name="shpFullname"></param>
        /// <param name="shpPrimaryField"></param>
        /// <param name="filter"></param>
        /// <param name="isCombinSameDay">
        /// 统计天数，同一天的数据先合并最大,如果没识别出日期，则按照独立日期算</param>
        /// <returns></returns>
        public SortedDictionary<string, StatAreaItem> CountByVector(string[] rasterFiles, string shpFullname, string shpPrimaryField, Func<T, bool> filter, bool isCombinSameDay)
        {
            List<IRasterDataProvider> rasters = new List<IRasterDataProvider>();
            try
            {
                foreach (string file in rasterFiles)
                {
                    IRasterDataProvider raster = GeoDataDriver.Open(file) as IRasterDataProvider;
                    if (raster != null)
                    {
                        if (raster.DataIdentify.OrbitDateTime == DateTime.MinValue)
                        {
                            RasterIdentify identify = new RasterIdentify(file);
                            raster.DataIdentify.OrbitDateTime = identify.OrbitDateTime;
                        }
                        rasters.Add(raster);
                    }
                }
                if (rasters.Count == 0)
                    return null;
                IRasterDataProvider[] orderRaster = rasters.OrderBy((rast) => { return rast.DataIdentify.OrbitDateTime; }).ToArray();

                return CountByVector(orderRaster, shpFullname, shpPrimaryField, filter, isCombinSameDay);
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
        /// 单文件只统计覆盖面积
        /// 多文件添加统计累计面积
        /// </summary>
        /// <param name="rasters"></param>
        /// <param name="shpFullname"></param>
        /// <param name="shpPrimaryField"></param>
        /// <param name="filter"></param>
        /// <returns>每种矢量分类的统计值</returns>
        public SortedDictionary<string, StatAreaItem> CountByVector(IRasterDataProvider[] rasters, string shpFullname, string shpPrimaryField, Func<T, bool> filter, bool isCombinSameDay)
        {
            try
            {
                if (rasters == null || rasters.Length == 0)
                    return null;
                //1、准备待分类统计矢量。
                Feature[] features;
                using (VectorAOITemplate vector = new VectorAOITemplate(shpFullname, (f) => { return f != null && f.Geometry is ShapePolygon; }))
                {
                    features = vector.GetFeatures();
                    if (string.IsNullOrWhiteSpace(shpPrimaryField))
                        shpPrimaryField = features[0].FieldNames[0];
                }
                SortedDictionary<string, StatAreaItem> statResult = new SortedDictionary<string, StatAreaItem>();
                //遍历所有矢量面
                int featureLength = features.Length;
                for (int i = 0; i < featureLength; i++)
                {
                    ShapePolygon shpPolygon = features[i].Geometry as ShapePolygon;
                    if (shpPolygon == null)
                        continue;
                    string fieldValue = features[i].GetFieldValue(shpPrimaryField);
                    UpdateProgress((int)(i * 100f / featureLength), fieldValue);
                    if (string.IsNullOrWhiteSpace(fieldValue))
                        continue;
                    //统计当前矢量面下所有文件的最大覆盖面积和累计面积。
                    StatAreaItem result = null;
                    if (rasters.Length == 1)
                        result = CountByVector(rasters[0], shpPolygon, filter);
                    else
                        result = CountByVector(rasters, shpPolygon, filter, isCombinSameDay);
                    if (result != null)
                        statResult.Add(fieldValue, result);
                }
                return statResult;
            }
            finally
            {
                UpdateProgress(100, "统计完成");
            }
        }

        /// <summary>
        /// 是否超过误差极限
        /// 如果误差，超过r1*r1Limit，则返回true，否则返回false
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <param name="r1Limit"></param>
        /// <returns></returns>
        private bool ErrorLimit(float r1, float r2, float r1Limit)
        {
            float absX = Math.Abs(r1 - r2);
            if (absX > r1 * r1Limit)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 2014年1月3日修改
        /// 内容：
        /// 分辨率使用最高的分辨率地方处理更新，防止发生类似0.00998分辨率的数据代替了0.01分辨率的数据。
        /// </summary>
        /// <param name="rasters"></param>
        /// <param name="filter"></param>
        /// <param name="statResult"></param>
        /// <param name="shpPolygon"></param>
        /// <param name="fieldValue"></param>
        private StatAreaItem CountByVector(IRasterDataProvider[] rasters, ShapePolygon shpPolygon, Func<T, bool> filter, bool isCombinSameDay)
        {
            //分辨率使用最高的分辨率
            float vResolutionX = rasters[0].ResolutionX;
            float vResolutionY = rasters[0].ResolutionY;
            foreach (IRasterDataProvider raster in rasters)
            {
                if (vResolutionX > raster.ResolutionX)
                {
                    if (ErrorLimit(vResolutionX, raster.ResolutionX, 0.1f))
                    {
                        vResolutionX = raster.ResolutionX;
                    }
                }
                if (vResolutionY > raster.ResolutionY)
                {
                    if (ErrorLimit(vResolutionY, raster.ResolutionY, 0.1f))
                    {
                        vResolutionY = raster.ResolutionY;
                    }
                }
            }

            //求所有栅格的最大合并区域与矢量区域的交集
            CoordEnvelope rasterUnionEnv = GetUnionEnvelope(rasters);
            Envelope polygonEnv = shpPolygon.Envelope;
            CoordEnvelope polygonCoordEnv = new CoordEnvelope(polygonEnv.MinX, polygonEnv.MaxX, polygonEnv.MinY, polygonEnv.MaxY);
            CoordEnvelope vrEnv = polygonCoordEnv.Intersect(rasterUnionEnv);
            if (vrEnv == null || vrEnv.IsEmpty())
                return null;
            Size vrSize = RasterRectOffset.GetRasterSize(vrEnv, vResolutionX, vResolutionY);
            //这里如果统计类似土地类型这样每种矢量面覆盖区域很大的数据时，容易在创建感兴趣区域时候造成溢出，故需要分行处理。
            int blockCount = 1;
            int blockRowCount = vrSize.Height;
            int maxLength = 5000 * 6000;
            if (vrSize.Width * vrSize.Height > maxLength)
            {
                blockCount = (int)Math.Ceiling(vrSize.Width * vrSize.Height * 1f / maxLength);
                blockRowCount = (int)Math.Ceiling(vrSize.Height * 1f / blockCount);
            }
            StatAreaItem statResult = new StatAreaItem();
            for (int rowBegin = 0; rowBegin < vrSize.Height; rowBegin += blockRowCount)
            {
                if (rowBegin >= vrSize.Height)//数据已经到头
                    break;
                if (rowBegin + blockRowCount > vrSize.Height)
                    blockRowCount = vrSize.Height - rowBegin;
                //i到i+blockCount行.
                double minX = vrEnv.MinX;
                double maxX = vrEnv.MaxX;
                double minY = vrEnv.MaxY - (rowBegin + blockRowCount) * vResolutionX;
                double maxY = vrEnv.MaxY - rowBegin * vResolutionY;
                CoordEnvelope blockEnv = new CoordEnvelope(minX, maxX, minY, maxY);
                //Size blockSize = new Size(vrSize.Width, blockRowCount);
                //创建虚拟数据头（地理范围为栅格和当前块的交集）
                VirtualRasterHeader vHeader = VirtualRasterHeader.Create(blockEnv, vResolutionX, vResolutionY);
                Size vSize = new Size(vHeader.Width, vHeader.Height);
                //创建AOI（基于虚拟数据的地理范围）
                int[] aoi = GetAoi(shpPolygon, vHeader);
                if (aoi == null)
                    continue;
                //2、遍历所有栅格数据
                byte[] coverCalced = new byte[vHeader.Width * vHeader.Height];//该区域的覆盖度已经统计过了
                if (isCombinSameDay)
                {
                    //预处理同天数据
                    Dictionary<DateTime, List<IRasterDataProvider>> dic = new Dictionary<DateTime, List<IRasterDataProvider>>();
                    foreach (IRasterDataProvider raster in rasters)
                    {
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
                            foreach (IRasterDataProvider raster in rastersSameDay)
                            {
                                CoordEnvelope intersectEnv = raster.CoordEnvelope.Intersect(blockEnv);
                                if (intersectEnv == null || intersectEnv.IsEmpty())       //不相交
                                    continue;
                                VirtualRaster vRaster = new VirtualRaster(raster, vHeader);
                                T[] datas = vRaster.ReadData<T>(1, 0, 0, vHeader.Width, vHeader.Height);
                                //测试当前分块数据读取是否正确
                                //TestWriteData<T>(datas, vHeader.Width, vHeader.Height, vRaster.VirtualHeader.CoordEnvelope);
                                for (int j = 0; j < aoi.Length; j++)
                                {
                                    int index = aoi[j];
                                    T data = datas[index];
                                    if (filter(data))//满足条件
                                    {
                                        //累计计数
                                        statResult.GrandTotal += 1;
                                        //覆盖计数
                                        if (coverCalced[index] == 0)
                                        {
                                            coverCalced[index] = 1;
                                            statResult.Cover += 1;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            byte[] sameDayCalced = new byte[vHeader.Width * vHeader.Height];
                            //同一天的计算出最大覆盖值
                            for (int i = 0; i < rastersSameDay.Length; i++)
                            {
                                IRasterDataProvider raster = rastersSameDay[i];
                                CoordEnvelope intersectEnv = raster.CoordEnvelope.Intersect(blockEnv);
                                if (intersectEnv == null || intersectEnv.IsEmpty())       //不相交
                                    continue;
                                VirtualRaster vRaster = new VirtualRaster(raster, vHeader);
                                T[] datas = vRaster.ReadData<T>(1, 0, 0, vHeader.Width, vHeader.Height);
                                for (int j = 0; j < aoi.Length; j++)
                                {
                                    int index = aoi[j];
                                    T data = datas[index];
                                    if (filter(data))//满足条件
                                    {
                                        if (sameDayCalced[index] == 0)//当天该像元数据是否已经统计过
                                        {
                                            sameDayCalced[index] = 1;
                                            statResult.GrandTotal += 1; //累计,一天只累计一次
                                            //覆盖计数
                                            if (coverCalced[index] == 0)
                                            {
                                                coverCalced[index] = 1; //最大覆盖，所有天只覆盖一次
                                                statResult.Cover += 1;
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
                    foreach (IRasterDataProvider raster in rasters)
                    {
                        CoordEnvelope intersectEnv = raster.CoordEnvelope.Intersect(blockEnv);
                        if (intersectEnv == null || intersectEnv.IsEmpty())       //不相交
                            continue;
                        VirtualRaster vRaster = new VirtualRaster(raster, vHeader);
                        T[] datas = vRaster.ReadData<T>(1, 0, 0, vHeader.Width, vHeader.Height);
                        //测试当前分块数据读取是否正确
                        //TestWriteData<T>(datas, vHeader.Width, vHeader.Height, vRaster.VirtualHeader.CoordEnvelope);
                        for (int j = 0; j < aoi.Length; j++)
                        {
                            int index = aoi[j];
                            T data = datas[index];
                            if (filter(data))//满足条件
                            {
                                //累计计数
                                statResult.GrandTotal += 1;
                                //覆盖计数
                                if (coverCalced[index] == 0)
                                {
                                    coverCalced[index] = 1;
                                    statResult.Cover += 1;
                                }
                            }
                        }
                    }
                }
                double perAreas = AreaCountHelper.CalcArea(shpPolygon.Centroid.X, shpPolygon.Centroid.Y, vResolutionX, vResolutionY) * Math.Pow(10, -6);
                statResult.Cover *= perAreas;
                statResult.GrandTotal *= perAreas;
            }
            if (statResult.Cover == 0 && statResult.GrandTotal == 0)
                return null;
            return statResult;
        }

        private CoordEnvelope GetUnionEnvelope(IRasterDataProvider[] rasters)
        {
            CoordEnvelope evp = rasters[0].CoordEnvelope.Clone();
            for (int i = 1; i < rasters.Length; i++)
                evp = evp.Union(rasters[i].CoordEnvelope);
            return evp;
        }

        /// <summary>
        /// 统计一个栅格在一个矢量面的面积
        /// </summary>
        private StatAreaItem CountByVector(IRasterDataProvider raster, ShapePolygon shpPolygon, Func<T, bool> filter)
        {
            float vResolutionX = raster.ResolutionX;
            float vResolutionY = raster.ResolutionY;
            Envelope polygonEnv = shpPolygon.Envelope;
            CoordEnvelope polygonCoordEnv = new CoordEnvelope(polygonEnv.MinX, polygonEnv.MaxX, polygonEnv.MinY, polygonEnv.MaxY);
            CoordEnvelope intersectEnv = raster.CoordEnvelope.Intersect(polygonCoordEnv);
            if (intersectEnv == null)       //不相交
                return null;
            //创建虚拟栅格(相交区域)
            VirtualRasterHeader vHeader = VirtualRasterHeader.Create(intersectEnv, vResolutionX, vResolutionY);
            Size aoiSize = new Size(vHeader.Width, vHeader.Height);
            //创建AOI
            int[] aoi = GetAoi(shpPolygon, vHeader);
            if (aoi == null)
                return null;
            VirtualRaster vRaster = new VirtualRaster(raster, vHeader);
            //这里如果统计类似土地类型等每种矢量面覆盖区域很大的数据时候，容易在创建感兴趣区域时候造成溢出，故需要分行处理
            T[] datas = vRaster.ReadData<T>(1, 0, 0, vHeader.Width, vHeader.Height);
            //测试当前分块数据读取是否正确
            //TestWriteData<T>(datas, vHeader.Width, vHeader.Height, vRaster.VirtualHeader.CoordEnvelope);
            double statValue = 0d;
            for (int j = 0; j < aoi.Length; j++)
            {
                int index = aoi[j];
                T data = datas[index];
                if (filter(data))       //满足条件
                {
                    //单文件不处理累计。
                    statValue += 1;
                }
            }
            if (statValue == 0)
                return null;
            double perAreas = AreaCountHelper.CalcArea(shpPolygon.Centroid.X, shpPolygon.Centroid.Y, raster.ResolutionX, raster.ResolutionY) * Math.Pow(10, -6);
            return new StatAreaItem(statValue * perAreas, statValue * perAreas);
        }

        /// <summary>
        /// 根据矢量模版统计栅格文件
        /// </summary>
        /// <param name="raster"></param>
        /// <param name="shpFullname"></param>
        /// <param name="shpPrimaryField"></param>
        /// <param name="operatorFun">
        /// Func<in T1, in T2, out TResult>输入T1为原始栅格值，输入T2（double）为当前统计结果，返回TResult(double)新的统计结果/>
        /// </param>
        /// <returns></returns>
        public SortedDictionary<string, StatAreaItem> CountByVector(IRasterDataProvider raster, string shpFullname, string shpPrimaryField, Func<T, double, double> operatorFun)
        {
            try
            {
                if (raster == null)
                    return null;
                //1、准备待分类统计矢量。
                Feature[] features;
                using (VectorAOITemplate vector = new VectorAOITemplate(shpFullname, (f) => { return f != null && f.Geometry is ShapePolygon; }))
                {
                    features = vector.GetFeatures();
                    if (string.IsNullOrWhiteSpace(shpPrimaryField))
                        shpPrimaryField = features[0].FieldNames[0];
                }
                SortedDictionary<string, StatAreaItem> statResult = new SortedDictionary<string, StatAreaItem>();
                //遍历所有矢量面
                int featureLength = features.Length;
                for (int i = 0; i < featureLength; i++)
                {
                    ShapePolygon shpPolygon = features[i].Geometry as ShapePolygon;
                    if (shpPolygon == null)
                        continue;
                    string fieldValue = features[i].GetFieldValue(shpPrimaryField);
                    UpdateProgress((int)(i * 100f / featureLength), fieldValue);
                    if (string.IsNullOrWhiteSpace(fieldValue))
                        continue;
                    //统计当前矢量面下所有文件的最大覆盖面积和累计面积。
                    StatAreaItem result = null;
                    float vResolutionX = raster.ResolutionX;
                    float vResolutionY = raster.ResolutionY;
                    Envelope polygonEnv = shpPolygon.Envelope;
                    CoordEnvelope polygonCoordEnv = new CoordEnvelope(polygonEnv.MinX, polygonEnv.MaxX, polygonEnv.MinY, polygonEnv.MaxY);
                    CoordEnvelope intersectEnv = raster.CoordEnvelope.Intersect(polygonCoordEnv);
                    if (intersectEnv == null)       //不相交
                        continue;
                    //创建虚拟栅格(相交区域)
                    VirtualRasterHeader vHeader = VirtualRasterHeader.Create(intersectEnv, vResolutionX, vResolutionY);
                    Size aoiSize = new Size(vHeader.Width, vHeader.Height);
                    //创建AOI
                    int[] aoi = GetAoi(shpPolygon, vHeader);
                    if (aoi == null)
                        continue;
                    VirtualRaster vRaster = new VirtualRaster(raster, vHeader);
                    //这里如果统计类似土地类型等每种矢量面覆盖区域很大的数据时候，容易在创建感兴趣区域时候造成溢出，故需要分行处理
                    T[] datas = vRaster.ReadData<T>(1, 0, 0, vHeader.Width, vHeader.Height);
                    double statValue = 0d;
                    for (int j = 0; j < aoi.Length; j++)
                    {
                        int index = aoi[j];
                        T data = datas[index];
                        statValue = operatorFun(data, statValue);
                    }
                    if (statValue == 0)
                        continue;
                    result = new StatAreaItem(statValue, statValue);
                    if (result != null)
                        statResult.Add(fieldValue, result);
                }
                return statResult;
            }
            finally
            {
                UpdateProgress(100, "统计完成");
            }
        }

        /// <summary>
        /// 根据矢量模版统计栅格文件
        /// </summary>
        /// <param name="raster"></param>
        /// <param name="shpFullname"></param>
        /// <param name="shpPrimaryField"></param>
        /// <param name="operatorFun">
        /// Func<in T1, in T2,in T3 out TResult>输入T1为原始栅格值，输入T2（int）为栅格索引值，输入T3用于区别方法参数,返回TResult(double)统计结果/>
        /// </param>
        /// <returns></returns>
        public SortedDictionary<string, StatAreaItem> CountByVector(IRasterDataProvider raster, string shpFullname, string shpPrimaryField, Func<T, int, double, double> operatorFun)
        {
            try
            {
                if (raster == null)
                    return null;
                //1、准备待分类统计矢量。
                Feature[] features;
                using (VectorAOITemplate vector = new VectorAOITemplate(shpFullname, (f) => { return f != null && f.Geometry is ShapePolygon; }))
                {
                    features = vector.GetFeatures();
                    if (string.IsNullOrWhiteSpace(shpPrimaryField))
                        shpPrimaryField = features[0].FieldNames[0];
                }
                SortedDictionary<string, StatAreaItem> statResult = new SortedDictionary<string, StatAreaItem>();
                //遍历所有矢量面
                int featureLength = features.Length;
                for (int i = 0; i < featureLength; i++)
                {
                    ShapePolygon shpPolygon = features[i].Geometry as ShapePolygon;
                    if (shpPolygon == null)
                        continue;
                    string fieldValue = features[i].GetFieldValue(shpPrimaryField);
                    UpdateProgress((int)(i * 100f / featureLength), fieldValue);
                    if (string.IsNullOrWhiteSpace(fieldValue))
                        continue;
                    //统计当前矢量面下所有文件的最大覆盖面积和累计面积。
                    StatAreaItem result = null;
                    float vResolutionX = raster.ResolutionX;
                    float vResolutionY = raster.ResolutionY;
                    Envelope polygonEnv = shpPolygon.Envelope;
                    CoordEnvelope polygonCoordEnv = new CoordEnvelope(polygonEnv.MinX, polygonEnv.MaxX, polygonEnv.MinY, polygonEnv.MaxY);
                    CoordEnvelope intersectEnv = raster.CoordEnvelope.Intersect(polygonCoordEnv);
                    if (intersectEnv == null)       //不相交
                        continue;
                    //创建虚拟栅格(相交区域)
                    VirtualRasterHeader vHeader = VirtualRasterHeader.Create(intersectEnv, vResolutionX, vResolutionY);
                    Size aoiSize = new Size(vHeader.Width, vHeader.Height);
                    //创建AOI
                    int[] aoi = GetAoi(shpPolygon, vHeader);
                    if (aoi == null)
                        continue;
                    VirtualRaster vRaster = new VirtualRaster(raster, vHeader);
                    //这里如果统计类似土地类型等每种矢量面覆盖区域很大的数据时候，容易在创建感兴趣区域时候造成溢出，故需要分行处理
                    T[] datas = vRaster.ReadData<T>(1, 0, 0, vHeader.Width, vHeader.Height);
                    double statValue = 0d;
                    for (int j = 0; j < aoi.Length; j++)
                    {
                        int index = aoi[j];
                        T data = datas[index];
                        statValue = operatorFun(data, index, statValue);
                    }
                    if (statValue == 0)
                        continue;
                    result = new StatAreaItem(statValue, statValue);
                    if (result != null)
                        statResult.Add(fieldValue, result);
                }
                return statResult;
            }
            finally
            {
                UpdateProgress(100, "统计完成");
            }
        }

        #endregion

        #region 多文件，分类统计最大面积，最大面积百分比
        /// <summary>
        /// 分类统计最大面积，最大面积百分比。
        /// <p>目前仅支持按省等，每个矢量面范围较小的统计。
        /// <p>暂时不支持较高分辨率的土地类型等尺度较大的矢量统计
        /// </summary>
        /// <param name="rasters"></param>
        /// <param name="shpFullname"></param>
        /// <param name="shpPrimaryField"></param>
        /// <param name="filter"></param>
        /// <returns>
        /// key为每种矢量面分类
        /// value为double[文件长度+2]：前面为每个文件的统计面积值，最后两个存储最大值和最大值占当前统计分类的百分比
        /// </returns>
        public Dictionary<string, double[]> MaxCountByVector(string[] rasterFiles, string shpFullname, string shpPrimaryField, Func<T, bool> filter)
        {
            List<IRasterDataProvider> rasters = new List<IRasterDataProvider>();
            try
            {
                foreach (string file in rasterFiles)
                {
                    IRasterDataProvider raster = GeoDataDriver.Open(file) as IRasterDataProvider;
                    if (raster != null)
                        rasters.Add(raster);
                }
                if (rasters.Count == 0)
                    return null;
                return MaxCountByVector(rasters.ToArray(), shpFullname, shpPrimaryField, filter);
            }
            finally
            {
                for (int i = 0; i < rasters.Count; i++)
                {
                    rasters[i].Dispose();
                }
            }
        }

        public Dictionary<string, double[]> MaxCountByVector(IRasterDataProvider[] rasters, string shpFullname, string shpPrimaryField, Func<T, bool> filter)
        {
            try
            {
                if (rasters == null || rasters.Length == 0)
                    return null;
                //1、准备待分类统计矢量。
                Feature[] features = GetShapePolygonFeature(shpFullname, ref shpPrimaryField);
                if (features == null || features.Length == 0)
                    return null;
                //2、统计最大面积值、最大面积时次和其所占百分比
                Dictionary<string, double[]> statResult = new Dictionary<string, double[]>();
                int rasterLength = rasters.Length;
                int length = features.Length;
                int pCount = rasterLength * length;
                for (int i = 0; i < length; i++)
                {
                    ShapePolygon shpPolygon = features[i].Geometry as ShapePolygon;
                    if (shpPolygon == null)
                        continue;
                    string fieldValue = features[i].GetFieldValue(shpPrimaryField);
                    if (string.IsNullOrWhiteSpace(fieldValue))
                        continue;
                    double maxArea = 0;
                    double maxAreaIndex = 0;
                    double[] areas = new double[rasterLength + 2]; //前面为每个文件的统计值，最后两个存储最大值，和最大值占当前统计分类百分比
                    for (int j = 0; j < rasterLength; j++)
                    {
                        UpdateProgress((int)((i * rasterLength + j) * 100f / pCount), "统计面积最大值，及百分比," + shpPolygon + ",第" + (j + 1) + "个文件");
                        IRasterDataProvider raster = rasters[j];
                        StatAreaItem area = CountByVector(raster, shpPolygon, filter);
                        if (area == null)
                            areas[j] = 0;
                        else
                            areas[j] = area.Cover;
                        if (maxArea < areas[j])
                        {
                            maxArea = areas[j];
                            maxAreaIndex = j;
                        }
                    }
                    if (maxArea == 0)//所有文件面积值都为0,即无交集
                        continue;
                    areas[rasters.Length] = maxArea;
                    double polygonArea = GetShapePolygonArea(rasters, shpPolygon);
                    if (polygonArea != 0)
                        areas[rasters.Length + 1] = maxArea * 100d / polygonArea;
                    statResult.Add(fieldValue, areas);
                }
                return statResult;
            }
            finally
            {
                UpdateProgress(100, "统计面积最大值，及百分比完成");
            }
        }

        private static double GetShapePolygonArea(IRasterDataProvider[] rasters, ShapePolygon shpPolygon)
        {
            float resolutionX = rasters[0].ResolutionX;
            float resolutionY = rasters[0].ResolutionY;
            double perAreas = AreaCountHelper.CalcArea(shpPolygon.Centroid.X, shpPolygon.Centroid.Y, resolutionX, resolutionY) * Math.Pow(10, -6);
            Size size = new Size(RasterRectOffset.GetInteger(shpPolygon.Envelope.Width / resolutionX), RasterRectOffset.GetInteger(shpPolygon.Envelope.Height / resolutionY));
            int[] aois = GetAoi(shpPolygon, shpPolygon.Envelope, size);
            double polygonArea = aois == null ? 0 : aois.Length * perAreas;
            return polygonArea;
        }

        /// <summary>
        /// 单文件统计面积和占矢量面的百分比
        /// </summary>
        /// <param name="raster"></param>
        /// <param name="shpPolygon"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private double[] CountAndPercentByVector(IRasterDataProvider raster, ShapePolygon shpPolygon, Func<T, bool> filter)
        {
            float vResolutionX = raster.ResolutionX;
            float vResolutionY = raster.ResolutionY;
            Envelope polygonEnv = shpPolygon.Envelope;
            CoordEnvelope polygonCoordEnv = new CoordEnvelope(polygonEnv.MinX, polygonEnv.MaxX, polygonEnv.MinY, polygonEnv.MaxY);
            CoordEnvelope intersectEnv = raster.CoordEnvelope.Intersect(polygonCoordEnv);
            if (intersectEnv == null)       //不相交
                return null;
            //创建虚拟栅格
            VirtualRasterHeader vHeader = VirtualRasterHeader.Create(intersectEnv, vResolutionX, vResolutionY);
            Size aoiSize = new Size(vHeader.Width, vHeader.Height);
            //创建AOI
            int[] aoi = GetAoi(shpPolygon, vHeader);
            if (aoi == null)
                return null;
            VirtualRaster vRaster = new VirtualRaster(raster, vHeader);
            //这里如果统计类似土地类型等每种矢量面覆盖区域很大的数据时候，容易在创建感兴趣区域时候造成溢出，故需要分行处理
            T[] datas = vRaster.ReadData<T>(1, 0, 0, vHeader.Width, vHeader.Height);
            //单位面积
            double perAreas = AreaCountHelper.CalcArea(intersectEnv.Center.X, intersectEnv.Center.Y, vResolutionX, vResolutionY) * Math.Pow(10, -6);
            double statValue = 0d;
            for (int j = 0; j < aoi.Length; j++)
            {
                int index = aoi[j];
                T data = datas[index];
                if (filter(data))       //满足条件
                {
                    //单文件不处理累计。
                    statValue += perAreas;
                }
            }
            return new double[] { statValue, aoi.Length * perAreas };
        }

        #endregion

        /// <summary>
        /// 单文件,分段(多条件)
        /// 面积统计以及其占各矢量面的百分比
        /// </summary>
        /// <param name="rasterFile">待统计栅格</param>
        /// <param name="shpFullname">待统计矢量（面）文件</param>
        /// <param name="shpPrimaryField">待统计矢量分类字段</param>
        /// <param name="filters">多个统计条件</param>
        /// <returns>
        /// 字典key为要统计的矢量面
        /// value为该矢量面下每个条件的统计值，每个统计值包含两项：（面积和面积占矢量面百分比）
        /// </returns>
        /// 
        public SortedDictionary<string, double[][]> CountAndPercentByVector(string rasterFile, string shpFullname, string shpPrimaryField, Dictionary<string, Func<T, bool>> filters)
        {
            return CountAndPercentByVector(rasterFile, shpFullname, shpPrimaryField, filters, false, 1);
        }

        public SortedDictionary<string, double[][]> CountAndPercentByVector(string rasterFile, string shpFullname, string shpPrimaryField, Dictionary<string, Func<T, bool>> filters, bool weight, float weightZoom)
        {
            using (IRasterDataProvider raster = GeoDataDriver.Open(rasterFile) as IRasterDataProvider)
            {
                if (raster == null)
                    return null;
                return CountAndPercentByVector(raster, shpFullname, shpPrimaryField, filters, weight, weightZoom);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="raster"></param>
        /// <param name="shpFullname"></param>
        /// <param name="shpPrimaryField"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public SortedDictionary<string, double[][]> CountAndPercentByVector(IRasterDataProvider raster, string shpFullname, string shpPrimaryField, Dictionary<string, Func<T, bool>> filters)
        {
            return CountAndPercentByVector(raster, shpFullname, shpPrimaryField, filters, false, 1);
        }

        public SortedDictionary<string, double[][]> CountAndPercentByVector(IRasterDataProvider raster, string shpFullname, string shpPrimaryField, Dictionary<string, Func<T, bool>> filters, bool weight, float weightZoom)
        {
            try
            {
                if (raster == null)
                    return null;
                //1、准备待分类统计矢量。
                Feature[] features;
                using (VectorAOITemplate vector = new VectorAOITemplate(shpFullname, (f) => { return f != null && f.Geometry is ShapePolygon; }))
                {
                    features = vector.GetFeatures();
                    if (string.IsNullOrWhiteSpace(shpPrimaryField))
                        shpPrimaryField = features[0].FieldNames[0];
                }
                SortedDictionary<string, double[][]> statResult = new SortedDictionary<string, double[][]>();
                //遍历所有矢量面
                int featureLength = features.Length;
                for (int i = 0; i < featureLength; i++)
                {
                    ShapePolygon shpPolygon = features[i].Geometry as ShapePolygon;
                    if (shpPolygon == null)
                        continue;
                    string fieldValue = features[i].GetFieldValue(shpPrimaryField);
                    UpdateProgress((int)(i * 100f / featureLength), fieldValue);
                    if (string.IsNullOrWhiteSpace(fieldValue))
                        continue;
                    double[][] stats = CountAndPercentByVector(raster, shpPolygon, filters, weight, weightZoom);
                    statResult.Add(fieldValue, stats);
                }
                return statResult;
            }
            finally
            {
                UpdateProgress(100, "统计完成");
            }
        }

        /// <summary>
        /// 单文件,分段(多条件)
        /// 面积统计以及其占各矢量面的百分比
        /// </summary>
        /// <param name="raster">待统计栅格</param>
        /// <param name="shpPolygon">待统计矢量面</param>
        /// <param name="filters">多个统计条件</param>
        /// <returns></returns>
        /// 
        private double[][] CountAndPercentByVector(IRasterDataProvider raster, ShapePolygon shpPolygon, Dictionary<string, Func<T, bool>> filters)
        {
            return CountAndPercentByVector(raster, shpPolygon, filters, false, 1);
        }

        private double[][] CountAndPercentByVector(IRasterDataProvider raster, ShapePolygon shpPolygon, Dictionary<string, Func<T, bool>> filters, bool weight, float weightZoom)
        {
            float vResolutionX = raster.ResolutionX;
            float vResolutionY = raster.ResolutionY;
            Envelope polygonEnv = shpPolygon.Envelope;
            CoordEnvelope polygonCoordEnv = new CoordEnvelope(polygonEnv.MinX, polygonEnv.MaxX, polygonEnv.MinY, polygonEnv.MaxY);
            CoordEnvelope intersectEnv = raster.CoordEnvelope.Intersect(polygonCoordEnv);
            if (intersectEnv == null)       //不相交
                return null;
            //创建虚拟栅格
            VirtualRasterHeader vHeader = VirtualRasterHeader.Create(intersectEnv, vResolutionX, vResolutionY);
            Size aoiSize = new Size(vHeader.Width, vHeader.Height);
            //创建AOI
            int[] aoi = GetAoi(shpPolygon, vHeader);
            if (aoi == null)
                return null;
            VirtualRaster vRaster = new VirtualRaster(raster, vHeader);
            //这里如果统计类似土地类型等每种矢量面覆盖区域很大的数据时候，容易在创建感兴趣区域时候造成溢出，故需要分行处理,目前尚未分行处理，故暂不支持较大面区域的统计
            T[] datas = vRaster.ReadData<T>(1, 0, 0, vHeader.Width, vHeader.Height);
            //单位面积
            double perAreas = AreaCountHelper.CalcArea(intersectEnv.Center.X, intersectEnv.Center.Y, vResolutionX, vResolutionY) * Math.Pow(10, -6);
            List<double[]> stats = new List<double[]>();
            foreach (string key in filters.Keys)
            {
                double statValue = 0d;
                Func<T, bool> filter = filters[key];
                if (filter == null)
                    stats.Add(null);
                for (int j = 0; j < aoi.Length; j++)
                {
                    int index = aoi[j];
                    T data = datas[index];
                    if (filter(data))       //满足条件
                    {
                        statValue += weight ? perAreas * Convert.ToSingle(data) / weightZoom : perAreas;
                    }
                }
                stats.Add(new double[] { statValue, aoi.Length * perAreas });
            }
            return stats.ToArray();
        }

        internal static GCHandle GetHandles(T[] virtureInData)
        {
            return GCHandle.Alloc(virtureInData, GCHandleType.Pinned);
        }

        #region 分段面积（多条件）统计 单文件
        /// <summary>
        /// 分段面积统计
        /// </summary>
        /// <param name="rasterFile"></param>
        /// <param name="shpFilename"></param>
        /// <param name="fieldName"></param>
        /// <param name="multiFilter"></param>
        /// <returns></returns>
        internal Dictionary<string, SortedDictionary<string, double>> CountByVector(string rasterFile, string shpFilename, string shpfieldName, Dictionary<string, Func<T, bool>> multiFilter)
        {
            IRasterDataProvider raster = null;
            try
            {
                raster = GeoDataDriver.Open(rasterFile) as IRasterDataProvider;
                if (raster == null)
                    return null;
                return CountByVector(raster, shpFilename, shpfieldName, multiFilter);
            }
            finally
            {
                if (raster != null)
                    raster.Dispose();
            }
        }

        /// <summary>
        /// 分段面积统计
        /// </summary>
        /// <param name="rasterFile"></param>
        /// <param name="shpFilename"></param>
        /// <param name="fieldName"></param>
        /// <param name="multiFilter"></param>
        /// <returns></returns>
        internal Dictionary<string, SortedDictionary<string, double>> CountByVector(IRasterDataProvider raster, string shpFilename, string shpPrimaryField, Dictionary<string, Func<T, bool>> multiFilter)
        {
            try
            {
                //1、准备待分类统计矢量。
                Feature[] features;
                using (VectorAOITemplate vector = new VectorAOITemplate(shpFilename, (f) => { return f != null && f.Geometry is ShapePolygon; }))
                {
                    features = vector.GetFeatures();
                    if (string.IsNullOrWhiteSpace(shpPrimaryField))
                        shpPrimaryField = features[0].FieldNames[0];
                }
                Dictionary<string, SortedDictionary<string, double>> statResult = new Dictionary<string, SortedDictionary<string, double>>();

                //遍历所有矢量面
                for (int i = 0; i < features.Length; i++)
                {
                    SortedDictionary<string, double> areadic = new SortedDictionary<string, double>();
                    ShapePolygon shpPolygon = features[i].Geometry as ShapePolygon;
                    if (shpPolygon == null)
                        continue;
                    string fieldValue = features[i].GetFieldValue(shpPrimaryField);
                    UpdateProgress((int)(i * 100f / features.Length), fieldValue);
                    if (string.IsNullOrWhiteSpace(fieldValue))
                        continue;
                    CoordEnvelope ploygonEnv = GetShpCoordEnvelope(shpPolygon);
                    CoordEnvelope intersectEnv = raster.CoordEnvelope.Intersect(ploygonEnv);
                    if (intersectEnv == null)//栅格与当前面不相交
                        continue;
                    //使用交集地理范围作为虚拟栅格范围
                    VirtualRasterHeader vHeader = CreateVirtualRasterHeader(raster, intersectEnv);
                    int[] aoi = GetAoi(shpPolygon, vHeader);
                    if (aoi == null || aoi.Length == 0)
                        continue;
                    double perAreas = AreaCountHelper.CalcArea(vHeader.CoordEnvelope.Center.X, vHeader.CoordEnvelope.Center.Y, vHeader.ResolutionX, vHeader.ResolutionY) * Math.Pow(10, -6);
                    VirtualRaster vRaster = new VirtualRaster(raster, vHeader);
                    T[] datas = vRaster.ReadData<T>(1, 0, 0, vHeader.Width, vHeader.Height);
                    //测试当前分块数据读取是否正确
                    //TestWriteData<T>(datas, vHeader.Width, vHeader.Height, vRaster.VirtualHeader.CoordEnvelope);
                    for (int j = 0; j < aoi.Length; j++)
                    {
                        int index = aoi[j];
                        T data = datas[index];
                        //统计各条件的值
                        foreach (string filterkey in multiFilter.Keys)
                        {
                            Func<T, bool> filter = multiFilter[filterkey];
                            if (filter(data))//满足条件
                            {
                                if (areadic.ContainsKey(filterkey))
                                    areadic[filterkey] += perAreas;
                                else
                                    areadic.Add(filterkey, perAreas);
                                break;
                            }
                        }
                    }
                    if (areadic.Count != 0)
                        statResult.Add(fieldValue, areadic);
                }
                return statResult;
            }
            finally
            {
                UpdateProgress(100, "统计完成");
            }
        }
        #endregion

        /// <summary>
        ///  平均值统计（分行政区域或其他）：
        ///  =行政区划内各像元值相加/行政区划像素个数
        /// </summary>
        /// <param name="raster"></param>
        /// <param name="shpFullFilename"></param>
        /// <param name="shpPrimaryField"></param>
        /// <param name="operatorFun"></param>
        /// <returns>
        /// key：矢量名称（行政区域或其他等）
        /// value：行政区划内各像元值相加/行政区划像素个数
        /// </returns>
        public SortedDictionary<string, double[]> SumAndCountByVector(string rasterFilename, string shpFullFilename, string shpPrimaryField, Func<T, bool> func)
        {
            using (IRasterDataProvider raster = GeoDataDriver.Open(rasterFilename) as IRasterDataProvider)
            {
                if (raster == null)
                    return null;
                return SumAndCountByVector(raster, shpFullFilename, shpPrimaryField, func);
            }
        }

        public SortedDictionary<string, double[]> SumAndCountByVector(IRasterDataProvider raster, string shpFullFilename, string shpPrimaryField, Func<T, bool> func)
        {
            try
            {
                //1、准备待分类统计矢量
                Feature[] features;
                using (VectorAOITemplate vector = new VectorAOITemplate(shpFullFilename, (f) => { return f != null && f.Geometry is ShapePolygon; }))
                {
                    features = vector.GetFeatures();
                    if (string.IsNullOrWhiteSpace(shpPrimaryField))
                        shpPrimaryField = features[0].FieldNames[0];
                }
                SortedDictionary<string, double[]> statResult = new SortedDictionary<string, double[]>();
                //遍历所有矢量面
                for (int i = 0; i < features.Length; i++)
                {
                    SortedDictionary<string, double> areadic = new SortedDictionary<string, double>();
                    ShapePolygon shpPolygon = features[i].Geometry as ShapePolygon;
                    if (shpPolygon == null)
                        continue;
                    string fieldValue = features[i].GetFieldValue(shpPrimaryField);
                    UpdateProgress((int)(i * 100f / features.Length), fieldValue);
                    if (string.IsNullOrWhiteSpace(fieldValue))
                        continue;
                    double[] stat = SumAndCountByVector(raster, shpPolygon, func);
                    statResult.Add(fieldValue, stat);
                }
                return statResult;
            }
            finally
            {
                UpdateProgress(100, "统计完成");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="raster"></param>
        /// <param name="shpPolygon"></param>
        /// <param name="funcDataFilter"></param>
        /// <returns>
        /// SortedDictionary<string, double[3]>
        /// key:各矢量名字
        /// value:double[0],满足条件的像元和，double[1]，满足条件的像元个数，double[2]，当前矢量面的像元个数。
        /// </returns>
        private double[] SumAndCountByVector(IRasterDataProvider raster, ShapePolygon shpPolygon, Func<T, bool> funcDataFilter)
        {
            float vResolutionX = raster.ResolutionX;
            float vResolutionY = raster.ResolutionY;
            Envelope polygonEnv = shpPolygon.Envelope;
            CoordEnvelope polygonCoordEnv = new CoordEnvelope(polygonEnv.MinX, polygonEnv.MaxX, polygonEnv.MinY, polygonEnv.MaxY);
            CoordEnvelope intersectEnv = raster.CoordEnvelope.Intersect(polygonCoordEnv);
            if (intersectEnv == null)       //不相交
                return null;
            //创建虚拟栅格
            VirtualRasterHeader vHeader = VirtualRasterHeader.Create(intersectEnv, vResolutionX, vResolutionY);
            Size aoiSize = new Size(vHeader.Width, vHeader.Height);
            //创建AOI
            int[] aoi = GetAoi(shpPolygon, vHeader);
            if (aoi == null)
                return null;
            VirtualRaster vRaster = new VirtualRaster(raster, vHeader);
            //这里如果统计类似土地类型等每种矢量面覆盖区域很大的数据时候，容易在创建感兴趣区域时候造成溢出，故需要分行处理,目前尚未分行处理，故暂不支持较大面区域的统计
            T[] datas = vRaster.ReadData<T>(1, 0, 0, vHeader.Width, vHeader.Height);
            //单位面积
            //double perAreas = AreaCountHelper.CalcArea(intersectEnv.Center.X, intersectEnv.Center.Y, vResolutionX, vResolutionY) * Math.Pow(10, -6);
            SortedDictionary<string, double[]> stats = new SortedDictionary<string, double[]>();
            //foreach (string key in filters.Keys)
            //{
            //    Func<T, bool> filter = filters[key];
            //    if (filter == null)
            //stats.Add(null);
            int filterCount = 0;
            dynamic sumValue = 0d;
            for (int j = 0; j < aoi.Length; j++)
            {
                int index = aoi[j];
                T data = datas[index];
                if (funcDataFilter(data))
                {
                    sumValue = sumValue + data;
                    filterCount++;
                }
            }
            //}
            return new double[] { sumValue, filterCount, aoi.Length };
        }

        private static Feature[] GetShapePolygonFeature(string shpFullname, ref string shpPrimaryField)
        {
            Feature[] features;
            using (VectorAOITemplate vector = new VectorAOITemplate(shpFullname, (f) => { return f != null && f.Geometry is ShapePolygon; }))
            {
                features = vector.GetFeatures();
                if (string.IsNullOrWhiteSpace(shpPrimaryField))
                    shpPrimaryField = features[0].FieldNames[0];
            }
            return features;
        }

        private static int[] GetAoi(ShapePolygon shpPolygon, VirtualRasterHeader vHeader)
        {
            Envelope env = new Envelope(vHeader.CoordEnvelope.MinX, vHeader.CoordEnvelope.MinY, vHeader.CoordEnvelope.MaxX, vHeader.CoordEnvelope.MaxY);
            Size size = new Size(vHeader.Width, vHeader.Height);
            return GetAoi(shpPolygon, env, size);
        }

        private static int[] GetAoi(ShapePolygon shpPolygon, CoordEnvelope coordEnv, Size size)
        {
            return GetAoi(shpPolygon, new Envelope(coordEnv.MinX, coordEnv.MinY, coordEnv.MaxX, coordEnv.MaxY), size);
        }

        private static int[] GetAoi(ShapePolygon shpPolygon, Envelope env, Size size)
        {
            try
            {
                int[] aoi = null;
                using (VectorAOIGenerator vectorGen = new VectorAOIGenerator())
                {
                    aoi = vectorGen.GetAOI(new ShapePolygon[] { shpPolygon }, env, size);
                }
                return aoi;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static VirtualRasterHeader CreateVirtualRasterHeader(IRasterDataProvider raster, CoordEnvelope vEnv)
        {
            float vResolutionX = raster.ResolutionX;
            float vResolutionY = raster.ResolutionY;
            return VirtualRasterHeader.Create(vEnv, vResolutionX, vResolutionY);
        }

        private static CoordEnvelope GetShpCoordEnvelope(ShapePolygon shpPolygon)
        {
            Envelope polygonEnv = shpPolygon.Envelope;
            CoordEnvelope vEnv = new CoordEnvelope(polygonEnv.MinX, polygonEnv.MaxX, polygonEnv.MinY, polygonEnv.MaxY);
            return vEnv;
        }

        private void UpdateProgress(int p, string text)
        {
            if (_progress != null)
                _progress(p, text);
        }

        private void TestWriteData(T[] virtureInData, int w, int h, CoordEnvelope env)
        {
            IRasterDataDriver dd = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            string mapInfo = env.ToMapInfoString(new Size(w, h));
            using (RasterDataProvider tr = dd.Create(@"E:\Smart\ldf\FY3A_MERSI_海南省_GLL_L1_20120911_D_0250M_MS_NDVI.dat", w, h, 1, enumDataType.Int16, mapInfo) as RasterDataProvider)
            {
                tr.GetRasterBand(1).Fill(-999);
                GCHandle buffer = GetHandles(virtureInData);
                tr.GetRasterBand(1).Write(0, 0, w, h, buffer.AddrOfPinnedObject(), enumDataType.Int16, w, h);
                buffer.Free();
            }
        }

        public static void test()
        {
            int p = 0;
            string dblv = @"D:\WorkFolder\Smart二期\1.联机开发@Smart\SMART\bin\Release\Workspace\FLD\2013-01-17\栅格产品\FLD_0MIX_洞庭湖流域._FY3A_MERSI_0250M_20120517023500.dat";
            dblv = @"D:\WorkFolder\Smart二期\1.联机开发@Smart\SMART\bin\Release\Workspace\FLD\2013-04-01\栅格产品\FLD_DBLV_洞庭湖流域._FY3A_MERSI_0250M_20120517023500.dat";
            RasterStatByVector<short> rsm = new RasterStatByVector<short>(
                (i, t) =>
                {
                    if (p < i)
                    {
                        p = i;
                        Console.WriteLine(i + t);
                    }
                });
            rsm.CountByVector(new string[] { dblv }, "省级行政区域_面.shp", "Name", (t) => { return t == 1; }, false);
        }


    }
}
