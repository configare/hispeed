using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;
using System.Drawing;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 计算逻辑,这里可以写根据输入数据、输出数据，写处理逻辑
    /// (这里的源数据，目标数据已经是同像元点的数据)
    /// </summary>
    /// <param name="fileVistor"></param>
    /// <param name="fileOutVistor"></param>   
    public delegate void RasterCalcHandler<T1, T2>(RasterVirtualVistor<T1>[] fileInVistor, RasterVirtualVistor<T2>[] fileOutVistor, int[] aoiIndex);
    public delegate bool RasterCalcHandlerFun<T1, T2>(RasterVirtualVistor<T1>[] fileInVistor, RasterVirtualVistor<T2>[] fileOutVistor, int[] aoiIndex);
    
    public class RasterVirtualVistor<T>
    {
        public IRasterDataProvider Raster { get; internal set; }
        public T[][] RasterBandsData { get; internal set; }
        public int[] BandMap { get; internal set; }
        public IRasterBand[] Bands { get; internal set; }
        public int IndexY { get; internal set; }
        public int SizeY { get; internal set; }
        public int SizeX { get; internal set; }

        public void Dispose()
        {
            RasterBandsData = null;
        }
    }

    public class RasterOffset
    {
        public int BeginX;
        public int BeginY;
        public int XSize;
        public int YSize;
    }

    public class RasterMaper
    {
        public RasterMaper(IRasterDataProvider raster, int[] bandMap)
        {
            Raster = raster;
            BandMap = bandMap;
        }

        /// <summary>
        /// 
        /// </summary>
        public IRasterDataProvider Raster { get;private set; }
        /// <summary>
        /// 文件的波段映射表（从1开始的）
        /// </summary>
        public int[] BandMap { get; internal set; }
    }
}
