using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using GeoDo.Project;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 判识结果栅格数据集
    /// </summary>
    public interface IInterestedRaster<T> : IExtractResult, IDisposable,IExtHeaderSetter
    {
        /// <summary>
        /// 文件名(根据产品标识生成的)
        /// </summary>
        string FileName { get; }
        /// <summary>
        /// [GET,SET]标识
        /// </summary>
        RasterIdentify Identify { get;  }
        /// <summary>
        /// [GET,SET]名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// [GET,SET]显示名称
        /// </summary>
        string Text { get; set; }
        /// <summary>
        /// [GET]判识结果(栅格)//T[,]
        /// </summary>
        IRasterBand RasterValues { get; }
        IRasterDataProvider HostDataProvider { get; }
        /// <summary>
        /// [GET]栅格大小
        /// </summary>
        Size Size { get; } 
        /// <summary>
        /// [GET,SET]空间参考
        /// </summary>
        ISpatialReference SpatialRef { get; set; }
        /// <summary>
        /// [GET,SET]坐标范围
        /// </summary>
        CoordEnvelope CoordEnvelope { get; set; }
        /// <summary>
        /// 复位(判识结果清零)
        /// </summary>
        void Reset();
        /// <summary>
        /// 设置初始值
        /// </summary>
        /// <param name="defaultValue"></param>
        void Put(double defaultValue);
        /// <summary>
        /// 写判识结果
        /// </summary>
        /// <param name="indexes">真值像元索引</param>
        /// <param name="trueValue">真值填充值</param>
        void Put(int[] indexes, T trueValue);
        /// <summary>
        /// 写判识结果
        /// </summary>
        /// <param name="indexes">像元索引值数组</param>
        /// <param name="features">像元特征值数组</param>
        void Put(int[] indexes, T[] features);
        /// <summary>
        /// 写判识结果
        /// </summary>
        /// <param name="result"></param>
        void Put(IPixelFeatureMapper<T> result);
        /// <summary>
        /// 写判识结果
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="value"></param>
        void Put(int idx, T value);
        /// <summary>
        /// 计数（用于粗略计算面积）
        /// </summary>
        /// <param name="aoi"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        int Count(int[] aoi, Func<T, bool> filter);
        /// <summary>
        /// 累计
        /// </summary>
        /// <param name="aoi"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        int Count(int[] aoi, Func<T, int> weight);
        /// <summary>
        /// 准确计算面积
        /// </summary>
        /// <param name="aoi"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        double Area(int[] aoi, Func<T, bool> filter); 
        /// <summary>
        /// 设置扩展头
        /// </summary>
        /// <typeparam name="TExtHanderStruct"></typeparam>
        /// <param name="extHeader"></param>
        void SetExtHeader<TExtHanderStruct>(TExtHanderStruct extHeader);
        /// <summary>
        /// 获取扩展头
        /// </summary>
        /// <typeparam name="TExtHanderStruct"></typeparam>
        /// <returns></returns>
        TExtHanderStruct GetExtHeader<TExtHanderStruct>();
    }
}
