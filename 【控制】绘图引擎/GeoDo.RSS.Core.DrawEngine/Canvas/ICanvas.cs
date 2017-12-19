using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace GeoDo.RSS.Core.DrawEngine
{
    /*
     * 1、采用投影坐标(米)进行绘制
     *    （1）打开空视图时，采用全球地理坐标(-180,180;-90,90)直接投影到投影坐标初始化视图；
     *    （2）打开地理坐标系统的数据时，将该数据的地理坐标范围转换为投影坐标范围对视图进行初始化；
     *    （3）打开投影坐标系统的数据时，直接使用该数据的坐标范围对视图进行初始化；
     *    （4）打开栅格图片(无坐标信息)时，将栅格的行列号作为投影坐标对视图进行初始化；
     *            左下角为(0,0)点, prjX = col,prjY = -row
     * 2、左下角为坐标原点(0,0),向上为正方向,向右为正方向
     */
    public interface ICanvas : IDisposable, ICanvasViewControl,IRenderBehavior,IControlMessageAccepter
    {
        bool IsLinking { get; set; }
        bool IsMoving { get;  }
        bool IsDrawScalePercent { get; set; }
        float ZoomFactor { get; }
        float Scale { get; set; }
        float PrimaryObjectScale { get; }
        float ResolutionX { get; }
        float ResolutionY { get; }
        void Refresh(enumRefreshType refreshType);
        void StrongRefresh();
        CanvasSetting CanvasSetting { get; }
        Control Container { get; }
        IControlLayer CurrentViewControl { get; set; }
        CoordEnvelope CurrentEnvelope { get; set; }
        ICoordinateTransform CoordTransform { get; }
        ILayerContainer LayerContainer { get; }
        IPerformanceWatch PerformanceWatch { get; set; }
        IDummyRenderModeSupport DummyRenderModeSupport { get; }
        IPrimaryDrawObject PrimaryDrawObject { get; set; }
        EventHandler OnEnvelopeChanged { get; set; }
        List<IPixelInfoSubscriber> PixelInfoSubscribers { get; }
        Bitmap ToBitmap();
        Bitmap ToBitmap(float minPrjX, float maxPrjX, float minPrjY, float maxPrjY);
        Bitmap FullRasterRangeToBitmap();
        Func<int[]> AOIGetter { get; set; }
        Func<int[]> MaskGetter { get; set; }
        void SetDefaultProjectionTransform(string spatialRef);
        void SetToChinaEnvelope();
        void SetToFullEnvelope();
        bool IsRasterCoord { get; }
        Action SomeTileIsArrived { get; set; }
    }
}
