using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    /// <summary>
    /// AOI转换为矢量(文件或矢量对象)
    /// 矢量属性名称及属性值：NAME = 'AOI'
    /// 导出矢量的几何形状为多边形
    /// </summary>
    public interface IAOI2ShapeFile
    {
        /// <summary>
        /// AOI导出为矢量文件
        /// </summary>
        /// <param name="geoEnvelope">影像地理范围</param>
        /// <param name="size">影像尺寸</param>
        /// <param name="aoi">AOI索引数组</param>
        /// <param name="shpFileName">矢量文件名</param>
        void Export(Envelope geoEnvelope, Size size, int[] aoi, string shpFileName);
        /// <summary>
        /// AOI转化为矢量对象
        /// </summary>
        /// <param name="geoEnvelope">影像地理范围</param>
        /// <param name="size">影像尺寸</param>
        /// <param name="aoi">AOI索引数组</param>
        /// <returns>返回矢量特征(几何形状为多边形)</returns>
        Feature Export(Envelope geoEnvelope, Size size, int[] aoi);
    }
}
