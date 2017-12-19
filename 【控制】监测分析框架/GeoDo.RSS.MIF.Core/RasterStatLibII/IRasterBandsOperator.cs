using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 栅格运算接口
    /// 注：1.不同区域(源栅格在目标外部、相交、内部)、不同分辨率在内部处理
    ///     2.不同数据类型不做处理
    /// </summary>
    public interface IRasterBandsOperator<T>
    {
        /// <summary>
        /// 覆盖
        /// 操作方法：
        /// dstRaster[iPixel] = srcRasters[0..n][iPixel]
        /// 主要通途：对不同区域的多幅栅格拼接为中国区域后执行面积统计
        /// </summary>
        /// <param name="dstRaster"></param>
        /// <param name="srcRasters"></param>
        void Cover(IRasterBand dstRaster, IRasterBand[] srcRasters);

        /// <summary>
        /// 栅格值相加
        /// 操作方法:
        /// dstRaster[iPixel]+=srcRasters[0..n][iPixel];
        /// 主要用途：频次统计
        /// </summary>
        /// <param name="dstRaster"></param>
        /// <param name="srcRasters"></param>
        void Add(IRasterBand dstRaster, IRasterBand[] srcRasters);

        /// <summary>
        /// 波段运算-减
        /// </summary>
        /// <param name="dstRaster"></param>
        /// <param name="raster1"></param>
        /// <param name="raster2"></param>
        void Subtract(IRasterBand dstRaster, IRasterBand raster1, IRasterBand raster2);

        /// <summary>
        /// 波段运算-乘
        /// </summary>
        /// <param name="dstRaster"></param>
        /// <param name="raster1"></param>
        /// <param name="raster2"></param>
        void Multiply(IRasterBand dstRaster, IRasterBand raster1, IRasterBand raster2);

        /// <summary>
        /// 滚动
        /// 操作方法:
        /// if(srcRasters[j][iPixel] == TrueValue)
        ///    dstRaster[iPixel] = j;
        /// 主要用途：周期合成
        /// </summary>
        /// <param name="dstRaster"></param>
        /// <param name="srcRasters"></param>
        void Roll(IRasterBand dstRaster, IRasterBand[] srcRasters);

        /// <summary>
        /// 比较
        /// 操作方法:
        /// dstRaster[iPixel] = comparer(raster1[iPixel],raster2[iPixel]);
        /// 主要用途：相邻时次火点状态比较(新生火点、持续火点、熄灭火点)
        /// </summary>
        /// <param name="dstRaster"></param>
        /// <param name="raster1"></param>
        /// <param name="raster2"></param>
        void Compare(IRasterBand dstRaster, IRasterBand raster1, IRasterBand raster2,object comparer);

        /// <summary>
        /// 最大值
        /// 操作方法:dstRaster[iPixel] = Math.Max(srcRasters[0..n][iPixel]);
        /// 主要用途：最大值合成
        /// </summary>
        /// <param name="dstRaster"></param>
        /// <param name="srcRasters"></param>
        void Max(IRasterBand dstRaster, IRasterBand[] srcRasters);

        /// <summary>
        /// 最大值
        /// 操作方法:dstRaster[iPixel] = Math.Max(srcRasters[0..n][iPixel]);
        /// 主要用途：最小值合成
        /// </summary>
        /// <param name="dstRaster"></param>
        /// <param name="srcRasters"></param>
        void Min(IRasterBand dstRaster, IRasterBand[] srcRasters);

        /// <summary>
        /// 平均值合成
        /// 操作方法:dstRaster[iPixel] = Avg(srcRasters[0..n][iPixel]);
        /// 主要用途：平均值合成
        /// </summary>
        /// <param name="dstRaster"></param>
        /// <param name="srcRasters"></param>
        void Avg(IRasterBand dstRaster, IRasterBand[] srcRasters);

        /// <summary>
        /// 距平
        /// 操作方法:dstRaster[iPixel] = dstRaster[iPixel] - Avg(srcRasters[0..n][iPixel])
        /// 主要用途：距平计算
        /// </summary>
        /// <param name="dstRaster"></param>
        /// <param name="srcRasters"></param>
        void DisAvg(IRasterBand dstRaster,IRasterBand primaryRaster, IRasterBand[] srcRasters);
        
        /// <summary>
        /// 像元面积计算
        /// 返回float型的IRasterDataProvider
        /// </summary>
        /// <param name="dstRaster"></param>
        void PixelArea(IRasterBand dstRaster);//Envelope
    }
}
