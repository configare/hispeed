using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace CodeCell.AgileMap.Core
{
    public interface IMapRuntimeHost:IMapRefresh
    {
        Envelope ExtentGeo { get; }
        /// <summary>
        /// 投影服务,地理坐标与投影坐标转换
        /// null:等经纬度投影(地理坐标)
        /// </summary>
        IProjectionTransform ProjectionTransform { get; }
        /// <summary>
        /// 当前应用使用的坐标类型(地理坐标，投影坐标)
        /// </summary>
        enumCoordinateType CoordinateType { get; }
        /// <summary>
        /// 窗口范围，使用当前应用单位(地图单位,可能是meters或者degress)
        /// </summary>
        Envelope FocusEnvelope { get; }
        /// <summary>
        /// 当前画布大小，像素单位
        /// </summary>
        Size CanvasSize { get; }
        /// <summary>
        /// 画布大小改变时
        /// </summary>
        EventHandler OnCanvasSizeChanged { get; set; }
        /// <summary>
        /// 使用缓冲图片
        /// </summary>
        bool UseDummyMap { get; }
        /// <summary>
        /// 定位服务
        /// </summary>
        ILocationService LocationService { get; }
        /// <summary>
        /// 刷新宿主控件
        /// </summary>
        void RefreshContainer();
        /// <summary>
        /// 异步调用方式通知宿主一次渲染结束
        /// </summary>
        /// <param name="notify"></param>
        void DoBeginInvoke(OnRenderIsFinishedHandler notify);
    }
}
