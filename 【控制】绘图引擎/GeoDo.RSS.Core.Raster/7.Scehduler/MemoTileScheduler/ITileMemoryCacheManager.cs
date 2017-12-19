using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public interface ITileMemoryCacheManager:IDisposable,IMemoryCacheControler
    {
        /// <summary>
        /// 获取当前选中波段
        /// </summary>
        int[] Bands { get; }
        /// <summary>
        /// 激活内存瓦片调度器
        /// </summary>
        void Start();
        /// <summary>
        /// 停止内存瓦片调度器(停止磁盘瓦片调度器)
        /// </summary>
        void Stop();
        /// <summary>
        /// [GET]标识调度器是否正在运行
        /// </summary>
        bool IsRunning { get; }
        /// <summary>
        /// 更新窗口大小(画布大小,像素坐标)
        /// </summary>
        /// <param name="size"></param>
        void UpdateWindowSize(Size size);
        /// <summary>
        /// 更新当前视窗区域对应对应在影像上的行列号
        /// </summary>
        /// <param name="rasterScale">当前视窗区域影像缩放比(用于确定瓦片Level)</param>
        /// <param name="beginX">开始列号</param>
        /// <param name="beginY">开始行号</param>
        /// <param name="xSize">宽度(列数)</param>
        /// <param name="ySize">高度(行数)</param>
        void UpdateRasterEnvelope(float rasterScale, int beginX, int beginY,int xSize,int ySize);
        /// <summary>
        /// 更新当前选中波段,同步更新Bands属性
        /// </summary>
        /// <param name="bands"></param>
        void ChangeBands(int[] bands);
        /// <summary>
        /// 获取视窗区域可见的瓦片
        /// </summary>
        /// <returns></returns>
        TileData[][] GetVisibleTileDatas();
        /// <summary>
        /// 同步加载指定Level的所有瓦片
        /// </summary>
        /// <param name="levelNo"></param>
        void LoadBySync(int levelNo);
        /// <summary>
        /// 同步加载最粗分辨率Level的所有瓦片
        /// </summary>
        void LoadBySync_MaxLevel();
        LevelTileComputer TileComputer { get; }
        Rectangle RasterEnvelopeOfWnd { get; }
    }
}
