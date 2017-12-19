using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;
using System.Drawing;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 根据矢量生成AOI索引数组
    /// </summary>
    public interface IVectorAOIGenerator:IDisposable
    {
        /// <summary>
        /// 矢量栅格化
        /// </summary>
        /// <param name="geometrys"></param>
        /// <param name="dstEnvelope"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        byte[] GetRaster(ShapePolygon[] geometrys, Envelope dstEnvelope, Size size);
        /// <summary>
        /// 计算AOI索引数组(根据地理坐标或者投影坐标)
        /// </summary>
        /// <param name="geometrys"></param>
        /// <param name="dstEnvelope"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        int[] GetAOI(ShapePolygon[] geometrys, Envelope dstEnvelope, Size size);
        /// <summary>
        /// 将绘制的几何形状生成AOI索引数组（根据地理坐标或者投影坐标）
        /// </summary>
        /// <param name="points"></param>
        /// <param name="types"></param>
        /// <param name="dstEnvelope"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        int[] GetAOI(PointF[] coordPoints, byte[] types, Envelope dstEnvelope, Size size);
        /// 将绘制的几何形状生成AOI索引数组（根据栅格坐标(行列号坐标)）
        /// </summary>
        /// <param name="points"></param>
        /// <param name="bytes"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        int[] GetAOI(PointF[] rasterPoints, byte[] bytes, Size size);
    }
}
