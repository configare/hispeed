using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.MIF.Core
{
    public interface IRasterOperator<T>
    {
        //计算外包矩形
        bool ComputeMaxEnvelope(string[] fnames, out CoordEnvelope outEnvelope, out Size size);
        bool ComputeMinEnvelope(string[] fnames, out CoordEnvelope outEnvelope, out Size size);
        /// <summary>
        /// 周期合成
        /// </summary>
        /// <param name="fnames"></param>
        /// <param name="rasterIdentify"></param>
        /// <param name="iTimesGetter"></param>
        /// <returns></returns>
        IInterestedRaster<T> CycleTimes(string[] fnames, RasterIdentify rasterIdentify, Func<int, T, T, T> iTimesGetter, Action<int, string> progress);
        IInterestedRaster<T> CycleTimes(IRasterDataProvider[] srcRasters, RasterIdentify rasterIdentify, Func<int, T, T, T> iTimesGetter, Action<int, string> progress);
        IInterestedRaster<T> Times(string[] fnames, RasterIdentify rasterIdentify,Action<int,string> progressTracker, Func<T, T, T> timesAction);
        IInterestedRaster<T> Times(string[] fnames, RasterIdentify rasterIdentify, Func<T, T, T> timesAction);
        IInterestedRaster<T> Times(IRasterDataProvider[] rasters, RasterIdentify rasterIdentify, Func<T, T, T> timesAction);
        IInterestedRaster<T> Times(IRasterDataProvider[] rasters, RasterIdentify rasterIdentify, Action<int, string> progressTracker, Func<T, T, T> timesAction);
        IInterestedRaster<T> Compare(int[] aoi, string raster1, string raster2, Func<T, T, T> comparer, RasterIdentify rasterIdentify);
        int Count(IRasterDataProvider dataProvider, int[] aoi, Func<T, bool> filter);
        int Count(string fname, int[] aoi, Func<T, bool> filter);
        int Count(IRasterDataProvider dataProvider, int[] aoi, Func<T, int, int> weight);
        int Count(string fname, int[] aoi, Func<T, int, int> weight);
        double Area(string fname, int[] aoi, Func<T, bool> filter);
        double Area(IRasterDataProvider dataProvider, int[] aoi, Func<T, bool> filter);
        Dictionary<string, double> Area(IRasterDataProvider dataProvider, int[] aoi, Dictionary<string, Func<T, bool>> filters);
    }
}
