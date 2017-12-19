using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.Project;

namespace GeoDo.RSS.Core.DF
{
    /// <summary>
    /// 波段
    /// </summary>
    public interface IRasterBand:IDisposable,IComparable
    {
        /// <summary>
        /// [GET,SET]波段序号
        /// </summary>
        int BandNo { get; set; }
        /// <summary>
        /// [GET]栅格数据提供者
        /// </summary>
        IRasterDataProvider RasterDataProvider { get; }
        /// <summary>
        /// [GET]波段属性字典集合
        /// </summary>
        AttributeManager Attributes { get; }
        /// <summary>
        /// [GET,SET]波段描述
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// [GET,SET]量化位数(eg:8bits,10bits,...；注意与DataType的概念不同)
        /// </summary>
        int MeasureBits { get; set; }
        /// <summary>
        /// [GET]数据类型
        /// </summary>
        enumDataType DataType { get; }
        /// <summary>
        /// [GET]宽度(列数,Samples)
        /// </summary>
        int Width { get; }
        /// <summary>
        /// [GET]高度(行数,Lines)
        /// </summary>
        int Height { get; }
        /// <summary>
        /// [GET]横向分辨率(数据单位)
        /// </summary>
        float ResolutionX { get; }
        /// <summary>
        /// [GET]纵向分辨率(数据单位)
        /// </summary>
        float ResolutionY { get; }
        /// <summary>
        /// [数据范围(数据单位)]
        /// </summary>
        CoordEnvelope CoordEnvelope { get; }
        /// <summary>
        /// [GET,SET]原始值到灰度值的拉伸器
        /// </summary>
        object Stretcher { get; set; }
        /// <summary>
        /// [GET]无效数据填充值(注意：使用时应转换为DataType)
        /// </summary>
        double NoDataValue { get; }
        /// <summary>
        /// [GET]数据值偏移量(eg:为了存储时为正值，DataValue+=500)
        /// </summary>
        double DataOffset { get; }
        /// <summary>
        /// [GET]数据值缩放系数(eg:为了存储时为整数，DataValue*=1000)
        /// </summary>
        double DataScale { get; }
        /// <summary>
        /// [GET]空间参考(如果CoordType为Raster，则返回null)
        /// </summary>
        ISpatialReference SpatialRef { get; }
        /// <summary>
        /// [GET]栅格坐标(行列号)与数据坐标(地理或投影)转换器
        /// </summary>
        ICoordTransform CoordTransform { get; }
        /// <summary>
        /// [GET]按量化位数计算出的最小值
        /// </summary>
        long MinByMeasureBits { get; }
        /// <summary>
        /// [GET]按量化位数计算出的最大值
        /// </summary>
        long MaxByMeasureBits { get; }
        /// <summary>
        /// 从波段读取指定位置的数据块
        /// </summary>
        /// <exception cref="RequestBlockOutOfRasterException"></exception>
        /// <exception cref="BufferIsEmptyException"></exception>
        /// <exception cref="Exception"></exception>
        /// <param name="xOffset">列偏移</param>
        /// <param name="yOffset">行偏移</param>
        /// <param name="xSize">原始宽度</param>
        /// <param name="ySize">原始高度</param>
        /// <param name="buffer">缓存区指针(指针类型与dataType一致，Bytes =xBufferSize * yBufferSize * sizeof(dataType))</param>
        /// <param name="dataType">返回的数据类型(应与dataType属性一致)</param>
        /// <param name="xBufferSize">请求采样后的宽度(如果xBufferSize == xSize，则列方向不采样)</param>
        /// <param name="yBufferSize">请求采样后的高度(如果yBufferSize == ySize，则行方向不采样)</param>
        void  Read(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize);
        /// <summary>
        /// 向波段指定位置写入数据块
        /// </summary>
        /// <param name="xOffset">目标位置列偏移</param>
        /// <param name="yOffset">目标位置行偏移</param>
        /// <param name="xSize">目标位置宽度</param>
        /// <param name="ySize">目标位置高度</param>
        /// <param name="buffer">待写入数据缓存区指针</param>
        /// <param name="dataType">数据类型</param>
        /// <param name="xBufferSize">待写入数据块宽度(如果xBufferSize != xSize,则列方向将会被采样写入)</param>
        /// <param name="yBufferSize">待写入数据块高度(如果yBufferSize != ySize,则行方向将会被采样写入)</param>
        void Write(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize);
        /// <summary>
        /// 将波段中所有像元值更新为指定值
        /// </summary>
        /// <param name="noDataValue">指定值</param>
        void Fill(double noDataValue);
        /// <summary>
        /// 计算像元最大值、最小值
        /// </summary>
        /// <param name="min">[OUT]最小值</param>
        /// <param name="max">[OUT]最大值</param>
        /// <param name="isCanApprox">TRUE:可采样统计,FALSE:不可采样统计</param>
        /// <param name="progressCallback">进度回调委托,int=[0~100],string=Message</param>
        void ComputeMinMax(out double min, out double max,bool isCanApprox,Action<int,string> progressCallback);
        /// <summary>
        /// 在指定的值域内统计最小值、最小值
        /// </summary>
        /// <param name="begin">指定值域开始值(eg:2200)</param>
        /// <param name="end">指定值域结束值(eg:3500)</param>
        /// <param name="min">[OUT]最小值</param>
        /// <param name="max">[OUT]最大值</param>
        /// <param name="isCanApprox">TRUE:可采样统计,FALSE:不可采样统计</param>
        /// <param name="progressCallback">进度回调委托,int=[0~100],string=Message</param>
        void ComputeMinMax(double begin,double end,out double min, out double max, bool isCanApprox, Action<int, string> progressCallback);
        /// <summary>
        /// 计算波段统计值
        /// </summary>
        /// <param name="min">[OUT]最小值</param>
        /// <param name="max">[OUT]最大值</param>
        /// <param name="mean">[OUT]平均值</param>
        /// <param name="stddev">[OUT]标准方差</param>
        /// <param name="isCanApprox">[IN]是否采样统计</param>
        /// <param name="progressCallback">进度回调委托,int=[0~100],string=Message</param>
        void ComputeStatistics(out double min, out double max, out double mean, out double stddev,bool isCanApprox, Action<int, string> progressCallback);
        /// <summary>
        /// 在指定的值域内计算波段统计值
        /// </summary>
        /// <param name="begin">指定值域开始值(eg:2200)</param>
        /// <param name="end">指定值域结束值(eg:3500)</param>
        /// <param name="min">[OUT]最小值</param>
        /// <param name="max">[OUT]最大值</param>
        /// <param name="mean">[OUT]平均值</param>
        /// <param name="stddev">[OUT]标准方差</param>
        /// <param name="isCanApprox">[IN]是否采样统计</param>
        /// <param name="progressCallback">[NULL]进度回调委托,int=[0~100],string=Message</param>
        void ComputeStatistics(double begin,double end,out double min, out double max, out double mean, out double stddev,bool isCanApprox, Action<int, string> progressCallback);
        /// <summary>
        /// 计算直方图
        /// </summary>
        /// <param name="begin">指定值域开始值(eg:2200);如果begin和end参数同时为零，则表示值域未设置</param>
        /// <param name="end">指定值域结束值(eg:3500);如果begin和end参数同时为零，则表示值域未设置</param>
        /// <param name="buckets">采样数(即histogram.Length),为零表示不采样</param>
        /// <param name="histogram">[REF]直方图缓冲区</param>
        /// <param name="isIncludeOutOfRange">TRUE:在值域范围之外的值作为begin和end处理,FALSE:不做统计</param>
        /// <param name="isCanApprox">TRUE:可采样统计,FALSE:不可采样统计</param>
        /// <param name="progressCallback">[NULL]进度回调委托,int=[0~100],string=Message</param>
        void ComputeHistogram(double begin, double end, int buckets, int[] histogram, bool isIncludeOutOfRange, bool isCanApprox, Action<int, string> progressCallback);
    }
}
