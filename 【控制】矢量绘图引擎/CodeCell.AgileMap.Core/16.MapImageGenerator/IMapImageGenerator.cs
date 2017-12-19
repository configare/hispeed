using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.AgileMap.Core
{
    public interface IMapImageGenerator
    {
        /// <summary>
        /// 设置或获取绘制使用的空间参考
        /// </summary>
        ISpatialReference SpatialReference { get; set; }
        /// <summary>
        /// 应用地图
        /// </summary>
        /// <param name="map"></param>
        void ApplyMap(IMap map);
        /// <summary>
        /// 获取当前地图
        /// </summary>
        IMap Map { get; }
        /// <summary>
        /// 获取坐标系统转换对象
        /// </summary>
        IProjectionTransform ProjectionTransform { get; }
        /// <summary>
        /// 生成地图图片
        /// </summary>
        /// <param name="rectPrj">投影坐标范围(请求的坐标范围，与实际返回图片的范围可能不一致)</param>
        /// <param name="targetSize">目标图片大小</param>
        /// <param name="img">生成的图片，为了共享内存，该对象需要外部创建，在内部Clear后填充</param>
        /// <returns>生成图片的实际坐标范围</returns>
        RectangleF GetMapImage(RectangleF rectPrj, Size targetSize, ref Image img);
        /// <summary>
        /// 将地理坐标范围的外接矩形转换纬投影坐标系统的矩形
        /// </summary>
        /// <param name="geoEnvelope">外接矩形(地理坐标系统)</param>
        /// <returns>外接矩形(投影坐标系统)</returns>
        RectangleF GeoEnvelope2Viewport(Envelope geoEnvelope);
    }
}
