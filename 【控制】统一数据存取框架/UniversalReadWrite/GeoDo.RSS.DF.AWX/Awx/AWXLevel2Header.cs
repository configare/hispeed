using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.DF.AWX
{
    public abstract class AWXLevel2Header
    {
        public string Satellite { get; set; }
        /// <summary>
        /// 通道号
        /// 静止数据：1：红外通道（10.3-11.3）;2：水汽通道 (6.3-7.6);3：红外分裂窗通道 (11.5-12.5);4：可见光通道 (0.5-0.9);5：中红外通道（3.5-4.0）
        /// 极轨数据，0：表示三通道图像，图像数据按R通道、G通道、B通道的顺序排列；1～5：表示单通道图像，且表示实际的卫星通道；101～119：TOVS HIRS通道；201～204：TOVS MSU通道
        /// </summary>
        public short ChannelNumber { get; set; }
        /// <summary>
        /// 图像宽度，列数
        /// </summary>
        public short ImageWidth { get; set; }
        /// <summary>
        /// 图像高度，行数
        /// </summary>
        public short ImageHeight { get; set; }
        public short ImageUpperLeftScanlineNumber { get; set; }
        public short ImageUpperLeftPixelNumber { get; set; }
        public short SamplingRate { get; set; }
        /// <summary>
        /// 投影方式, 0-未投影,1-兰布托投影,2-麦卡托投影,3-极射赤面投影,4-等经纬度投影【正轴等距离圆柱投影】, 5-等面积投影,6-其他投影
        /// </summary>
        public short ProjectMode { get; set; }
        /// <summary>
        /// 投影中心纬度,度×100
        /// </summary>
        public short ProjectCenterLatitude { get; set; }
        /// <summary>
        /// 投影中心经度,度×100
        /// </summary>
        public short ProjectCenterLongitude { get; set; }
        /// <summary>
        /// 投影标准纬度1（或标准经度）,度×100
        /// 对兰勃托投影（有两个标准纬度）有效，
        /// 对麦卡托投影（只有一个标准纬度）有效
        /// 对极射投影，表示标准经度
        /// 对等经纬度投影无意义
        /// </summary>
        public short ProjectStandardLatitude1 { get; set; }
        /// <summary>
        /// 标准投影纬度2，仅对兰勃托投影有效
        /// 对等经纬度投影无意义
        /// </summary>
        public short ProjectStandardLatitude2 { get; set; }
        /// <summary>
        /// 地理范围（北纬）,
        /// </summary>
        public short GeographicalScopeNorthLatitude { get; set; }
        /// <summary>
        /// 地理范围（南纬）
        /// </summary>
        public short GeographicalScopeSouthLatitude { get; set; }
        /// <summary>
        /// 地理范围（西经）
        /// </summary>
        public short GeographicalScopeWestLongitude { get; set; }
        /// <summary>
        /// 地理范围（东经）
        /// </summary>
        public short GeographicalScopeEastLongitude { get; set; }
        /// <summary>
        /// 投影水平分辨率	公里×100
        /// </summary>
        public short ProjectHorizontalResolution { get; set; }
        /// <summary>
        /// 投影垂直分辨率	公里×100
        /// </summary>
        public short ProjectVerticalResolution { get; set; }
        public short GeographyGridSuperposeMark { get; set; }
        public short GeographyGridSuperposeValue { get; set; }
        public short ToningTableDataBlockLength { get; set; }
        public short CalibrationDataBlockLength { get; set; }
        public short LocationDataBlockLength { get; set; }
        public short Reserve { get; set; }

        public abstract void Read(Stream stream);
        public abstract void Write(HdrFile hdr, RasterIdentify id);
        public abstract void WriteFile(string hdr);
    }
}
