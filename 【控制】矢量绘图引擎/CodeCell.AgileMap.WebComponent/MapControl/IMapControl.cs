using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace CodeCell.AgileMap.WebComponent
{
    public interface IMapControl
    {
        /// <summary>
        /// 获取画布
        /// </summary>
        Canvas Canvas { get; }
        /// <summary>
        /// 获取当前地图对象
        /// </summary>
        Map Map { get; }
        /// <summary>
        /// 地图服务对象代理
        /// </summary>
        IMapServerAgent MapServerAgent { get; set; }
        /// <summary>
        /// 获取或设置当前工具(根据内置工具的类型)
        /// </summary>
        enumMapTools CurrentMapToolType { get; set; }
        /// <summary>
        /// 获取或设置当前工具
        /// </summary>
        IMapTool CurrentMapTool { get; set; }
        /// <summary>
        /// 查询系统工具
        /// </summary>
        /// <param name="maptool"></param>
        /// <returns></returns>
        IMapCommand FindSystemMapTool(enumMapTools maptooltype);
        /// <summary>
        /// 地图视窗(采用投影坐标)
        /// </summary>
        PrjRectangleF Viewport { get; set; }
        /// <summary>
        /// 设置地图视窗(使用地理坐标)
        /// </summary>
        /// <param name="minLon"></param>
        /// <param name="maxLon"></param>
        /// <param name="minLat"></param>
        /// <param name="maxLat"></param>
        void SetViewportByGeo(double minLon, double maxLon, double minLat, double maxLat);
        /// <summary>
        /// 设置地图视窗(使用投影坐标系统)
        /// </summary>
        /// <param name="prjX"></param>
        /// <param name="prjY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        void SetViewportByPrj(PrjRectangleF viewport);
        /// <summary>
        /// 将投影坐标转换为像素坐标
        /// </summary>
        /// <param name="prjPt"></param>
        /// <returns></returns>
        Point Prj2Pixel(Point prjPt);
        /// <summary>
        /// 将像素坐标转换为投影坐标
        /// </summary>
        /// <param name="pixelPt"></param>
        /// <returns></returns>
        Point Pixel2Prj(Point pixelPt);
        /// <summary>
        /// 投影坐标到像素坐标的表换矩阵
        /// </summary>
        Transform Prj2PixelTransform { get; }
        /// <summary>
        /// 平移地图
        /// </summary>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        void Offset(double offsetX, double offsetY);
        /// <summary>
        /// 刷新地图
        /// </summary>
        void RefreshMap();
        /// <summary>
        /// 获取或者查询结果容器
        /// </summary>
        IQueryResultContainer QueryResultContainer { get; set; }
        /// <summary>
        /// 当前坐标提示
        /// </summary>
        ICurrentCoordDisplayer CurrentCoordDisplayer { get; set; }
        /// <summary>
        /// 当可视区域发生变化时
        /// </summary>
        OnMapControlViewportChangedHandler OnMapControlViewportChanged { get; set; }
        /// <summary>
        /// 当前比例尺
        /// </summary>
        double Scale { get; }
        /// <summary>
        /// 当前分辨率
        /// </summary>
        double Resolution { get; }
        /// <summary>
        /// 定位到指定的地理坐标
        /// </summary>
        /// <param name="geoPt">地理坐标</param>
        void Goto(Point geoPt);
        /// <summary>
        /// 漫游到指定的位置（按指定的半径计算可视区域）
        /// </summary>
        /// <param name="geoPt"></param>
        /// <param name="radius"></param>
        void PanTo(Point geoPt, double radius);
        /// <summary>
        /// 拾取要素
        /// </summary>
        /// <param name="fets">要素容器</param>
        /// <param name="prjPt">拾取点</param>
        /// <param name="tolerance">容差(米)</param>
        void Identify(ObservableCollection<Feature> fets, Point prjPt, double tolerance);
    }
}
