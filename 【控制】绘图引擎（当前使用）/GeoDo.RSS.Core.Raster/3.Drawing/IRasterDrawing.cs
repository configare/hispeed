using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.DrawEngine;
using System.Drawing;
using GeoDo.RSS.Core.CA;
using GeoDo.Project;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public unsafe interface IRasterDrawing:IPrimaryDrawObject, IDisposable,IGeoPanAdjust
    {
        string FileName { get; }
        int BandCount { get; }
        GeoDo.RSS.Core.DrawEngine.CoordEnvelope Envelope { get; }
        int[] SelectedBandNos { get; set; }
        IRasterDataProvider DataProvider { get; }
        IRasterDataProvider DataProviderCopy { get; }
        ITileBitmapProvider TileBitmapProvider { get; }
        IRgbStretcherProvider RgbStretcherProvider { get; }
        Bitmap Bitmap { get; }
        Bitmap GetBitmap(IDrawArgs drawArgs);
        int GetOverviewLoadTimes();
        void StartLoading(Action<int, int> progress);
        IRgbProcessorStack RgbProcessorStack { get; }
        Color GetColorAt(int x, int y);
        IProjectionTransform ProjectionTransform { get; }
        void ReadPixelValues(int x, int y, double* buffer);
        IList<ILoadingPrecentSubscriber> LoadingSubscribers { get; }
        bool IsActive { get; }
        void Active();
        void Deactive();
        /// <summary>
        /// 尝试创建轨道坐标投影对象（用于在轨道上显示矢量）
        /// </summary>
        void TryCreateOrbitPrjection();
        /// <summary>
        /// 应用颜色映射表(采用原始DN值)
        /// </summary>
        /// <param name="oColorTable"></param>
        void ApplyColorMapTable(ColorMapTable<double> oColorTable);
        void Reset();
    }
}
