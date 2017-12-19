using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    /// <summary>
    /// 栅格数据提供者(相当于GDAL中的GDALDataset)
    /// </summary>
    public interface IRasterDataProvider : IGeoDataProvider,IOverviewGenerator
    {
        /// <summary>
        /// [GET]数据提供者属性字典集合(相当于GDAL's GDALDataset)
        /// </summary>
        AttributeManager Attributes { get; }
        /// <summary>
        /// [GET]数据类型(eg:Int16,UInt16,...)
        /// </summary>
        enumDataType DataType { get; }
        /// <summary>
        /// [GET]波段总数
        /// </summary>
        int BandCount { get; }
        /// <summary>
        /// [GET]数据集像素宽度(如果各个波段维度不一致，则返回Int.NaN)
        /// </summary>
        int Width { get; }
        /// <summary>
        /// [GET]数据集像素高度(如果各个波段维度不一致，则返回Int.NaN)
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
        /// 数据坐标范围(最小外包矩形，数据单位）
        /// </summary>
        CoordEnvelope CoordEnvelope { get; }
        /// <summary>
        /// [GET]栅格坐标(行列号)与数据坐标(地理或投影)转换器
        /// </summary>
        ICoordTransform CoordTransform { get; }
        /// <summary>
        /// 获取与数据提供者相关的所有文件(eg:raster.raw,raster.hdr;road.shp,road.shx,road.dbf)
        /// </summary>
        /// <returns>文件名路径数组(绝对路径)</returns>
        string[] GetFileList();
        /// <summary>
        /// 卫星传感器标识
        /// </summary>
        DataIdentify DataIdentify { get; }
        /// <summary>
        /// 获取波段的拉伸器(原始值=>灰度值)
        /// </summary>
        /// <param name="bandNo"></param>
        /// <returns></returns>
        object GetStretcher(int bandNo);
        int[] GetDefaultBands();
        /// <summary>
        /// 获取波段对象
        /// </summary>
        /// <param name="bandNo">波段序号(开始于1)</param>
        /// <returns>波段对象</returns>
        IRasterBand GetRasterBand(int bandNo);
        /// <summary>
        /// 波段提供者
        /// 对于类似HDF的科学数据集适用
        /// </summary>
        IBandProvider BandProvider { get; }
        /// <summary>
        /// 新增波段(如果新增成功BandCount=BandCount+1，否则BandCount不变)
        /// </summary>
        /// <param name="dataType">波段数据类型</param>
        void AddBand(enumDataType dataType);
        /// <summary>
        /// 从数据提供者(数据集)中读取指定位置的数据块
        /// </summary>
        /// <exception cref="BandIndexOutOfRangeException"></exception>
        /// <exception cref="RasterBandsIsEmptyException"></exception>
        /// <exception cref="InterleaveIsNotSupportException"></exception>
        /// <exception cref="RequestBlockOutOfRasterException"></exception>
        /// <exception cref="BufferIsEmptyException"></exception>
        /// <exception cref="Exception"></exception>
        /// <param name="xOffset">列偏移</param>
        /// <param name="yOffset">行偏移</param>
        /// <param name="xSize">原始宽度</param>
        /// <param name="ySize">原始高度</param>
        /// <param name="buffer">缓存区指针(指针类型与dataType一致，Bytes =xBufferSize * yBufferSize * bandCount * sizeof(dataType))</param>
        /// <param name="dataType">返回的数据类型(应与dataType属性一致)</param>
        /// <param name="xBufferSize">请求采样后的宽度(如果xBufferSize == xSize，则列方向不采样)</param>
        /// <param name="yBufferSize">请求采样后的高度(如果yBufferSize == ySize，则行方向不采样)</param>
        /// <param name="bandCount">请求的波段总数</param>
        /// <param name="bandMap">请求的波段序号数组(eg:int[]{6,2,1})</param>
        /// <param name="interleave">请求的像素布局方式(BIP,BSQ,BIL)</param>
        void Read(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer,enumDataType dataType, int xBufferSize, int yBufferSize, int bandCount, int[] bandMap,enumInterleave interleave);
        /// <summary>
        /// 向数据提供者(数据集)的指定位置写入数据块
        /// </summary>
        /// <param name="xOffset">目标位置列偏移</param>
        /// <param name="yOffset">目标位置行偏移</param>
        /// <param name="xSize">目标位置宽度</param>
        /// <param name="ySize">目标位置高度</param>
        /// <param name="buffer">待写入数据缓存区指针</param>
        /// <param name="dataType">数据类型</param>
        /// <param name="xBufferSize">待写入数据块宽度(如果xBufferSize != xSize,则列方向将会被采样写入)</param>
        /// <param name="yBufferSize">待写入数据块高度(如果yBufferSize != ySize,则行方向将会被采样写入)</param>
        /// <param name="bandCount">波段数</param>
        /// <param name="bandMap">待写入的波段序号(eg:int[] { 6 , 2 , 1 })</param>
        /// <param name="interleave">缓存区像素布局方式</param>
        void Write(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize, int bandCount, int[] bandMap, enumInterleave interleave);
        /// <summary>
        /// 轨道投影变换控制器
        /// </summary>
        IOrbitProjectionTransformControl OrbitProjectionTransformControl { get; }
        /// <summary>
        /// 记录用户对象
        /// </summary>
        object Tag { get; set; }
        bool IsOverviewsBuilded { get; }
        bool IsSupprtOverviews { get; }
        void BuildOverviews(int[] levels, Action<int, string> progressTracker);
        void BuildOverviews(Action<int, string> progressTracker);
        void ResetStretcher();
    }
}
