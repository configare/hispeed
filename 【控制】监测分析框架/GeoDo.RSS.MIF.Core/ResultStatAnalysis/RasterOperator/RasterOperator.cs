using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.IO;

namespace GeoDo.RSS.MIF.Core
{
    public class RasterOperator<T> : IRasterOperator<T>
    {
        public RasterOperator()
        {
        }

        public int Count(IRasterDataProvider dataProvider, int[] aoi, Func<T, bool> filter)
        {
            IArgumentProvider argPrd = new ArgumentProvider(dataProvider, null);
            argPrd.AOI = aoi;
            IRasterPixelsVisitor<T> visitor = new RasterPixelsVisitor<T>(argPrd);
            int retCount = 0;
            visitor.VisitPixel(new int[] { 1 },
                (idx, values) =>
                {
                    if (filter(values[0]))
                        retCount++;
                });
            return retCount;
        }

        public int Count(string fname, int[] aoi, Func<T, bool> filter)
        {
            using (IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider)
            {
                return Count(prd, aoi, filter);
            }
        }

        public int Count(IRasterDataProvider dataProvider, int[] aoi, Func<T, int, int> weight)
        {
            IArgumentProvider argPrd = new ArgumentProvider(dataProvider, null);
            argPrd.AOI = aoi;
            IRasterPixelsVisitor<T> visitor = new RasterPixelsVisitor<T>(argPrd);
            int retCount = 0;
            visitor.VisitPixel(new int[] { 1 },
                (idx, values) =>
                {
                    retCount += weight(values[0], idx);
                });
            return retCount;
        }

        public int Count(string fname, int[] aoi, Func<T, int, int> weight)
        {
            using (IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider)
            {
                return Count(prd, aoi, weight);
            }
        }

        public double Area(string fname, int[] aoi, Func<T, bool> filter)
        {
            using (IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider)
            {
                return Area(prd, aoi, filter);
            }
        }

        public double Area(IRasterDataProvider dataProvider, int[] aoi, Func<T, bool> filter)
        {
            if (dataProvider.CoordEnvelope == null)
                return 0d;
            IArgumentProvider argPrd = new ArgumentProvider(dataProvider, null);
            argPrd.AOI = aoi;
            IRasterPixelsVisitor<T> visitor = new RasterPixelsVisitor<T>(argPrd);
            double area = 0;
            int row = 0;
            int width = dataProvider.Width;
            double maxLat = dataProvider.CoordEnvelope.MaxY;
            double res = dataProvider.ResolutionX;
            visitor.VisitPixel(new int[] { 1 },
                (idx, values) =>
                {
                    if (filter(values[0]))
                    {
                        row = idx / width;
                        area += ComputePixelArea(row, maxLat, res);
                    }
                }
                );
            return area;
        }

        public Dictionary<string, double> Area(IRasterDataProvider dataProvider, int[] aoi, Dictionary<string, Func<T, bool>> filterDic)
        {
            if (dataProvider.CoordEnvelope == null)
                return null;
            IArgumentProvider argPrd = new ArgumentProvider(dataProvider, null);
            argPrd.AOI = aoi;
            IRasterPixelsVisitor<T> visitor = new RasterPixelsVisitor<T>(argPrd);
            int row = 0;
            int width = dataProvider.Width;
            double maxLat = dataProvider.CoordEnvelope.MaxY;
            double res = dataProvider.ResolutionX;
            Dictionary<string, double> areas = new Dictionary<string, double>();
            int length = filterDic.Count;
            string[] keys = filterDic.Keys.ToArray();
            Func<T, bool>[] filters = filterDic.Values.ToArray();
            double[] statAreas = new double[length];
            visitor.VisitPixel(new int[] { 1 },
                (idx, values) =>
                {
                    for (int i = 0; i < length; i++)
                    {
                        if (filters[i](values[0]))
                        {
                            row = idx / width;
                            statAreas[i] += ComputePixelArea(row, maxLat, res);
                            break;
                        }
                    }
                }
                );
            for (int i = 0; i < length; i++)
            {
                areas.Add(keys[i], statAreas[i]);
            }
            return areas;
        }

        /// <summary>
        /// 统计判识结果的面积
        /// </summary>
        /// <param name="dataProvider">判识结果使用的栅格文件</param>
        /// <param name="extractedPixels">判识结果</param>
        /// <param name="aoi"></param>
        /// <param name="filterDic"></param>
        /// <returns></returns>
        public double Area(IRasterDataProvider dataProvider, IPixelIndexMapper extractedPixels, int[] aoi)
        {
            if (dataProvider.CoordEnvelope == null)
                return 0;
            IArgumentProvider argPrd = new ArgumentProvider(dataProvider, null);
            argPrd.AOI = aoi;
            IRasterPixelsVisitor<T> visitor = new RasterPixelsVisitor<T>(argPrd);
            int row = 0;
            int width = dataProvider.Width;
            double maxLat = dataProvider.CoordEnvelope.MaxY;
            double res = dataProvider.ResolutionX;
            double statArea = 0;
            visitor.VisitPixel(new int[] { 1 },
                (idx, values) =>
                {
                    if (extractedPixels.Get(idx))
                    {
                        statArea += ComputePixelArea(row, maxLat, res);
                    }
                });
            return statArea;
        }

        public static double ComputePixelArea(int row, double maxLat, double resolution)
        {
            double lat = maxLat - row * resolution;
            double a = 6378.137d;
            double c = 6356.7523142d;
            double latComputeFactor = 111.13d;
            double factor = Math.Pow(Math.Tan(lat * Math.PI / 180d), 2);
            double lon = resolution * 2 * Math.PI * a * c * Math.Sqrt(1 / (c * c + a * a * factor)) / 360d;
            return lon * latComputeFactor * resolution;
        }

        public IInterestedRaster<T> Times(IRasterDataProvider[] srcRasters, RasterIdentify rasterIdentify, Func<T, T, T> timesAction)
        {
            return Times(srcRasters, rasterIdentify, null, timesAction);
        }

        public IInterestedRaster<T> Times(IRasterDataProvider[] srcRasters, RasterIdentify rasterIdentify, Action<int, string> progressTracker, Func<T, T, T> timesAction)
        {
            CoordEnvelope dstEnvelope = GetEnvelopeBySize(srcRasters[srcRasters.Length - 1]);
            float minResolutionX = 0f;
            float minResolutionY = 0f;
            Size dstSize = Size.Empty;
            for (int i = 0; i < srcRasters.Length - 1; i++)
            {
                if (srcRasters[i].DataType != srcRasters[i + 1].DataType)
                    throw new ArgumentException("数据类型不一致无法进行频次统计！");
                dstEnvelope = dstEnvelope.Union(GetEnvelopeBySize(srcRasters[i]));
                minResolutionX = Math.Min(srcRasters[i].ResolutionX, srcRasters[i + 1].ResolutionX);
                minResolutionY = Math.Min(srcRasters[i].ResolutionY, srcRasters[i + 1].ResolutionY);
                dstSize = new Size((int)Math.Round(dstEnvelope.Width / minResolutionX), (int)Math.Round(dstEnvelope.Height / minResolutionY));
            }
            if (srcRasters.Length == 1)
            {
                minResolutionX = srcRasters[0].ResolutionX;
                minResolutionY = srcRasters[0].ResolutionY;
                dstEnvelope = srcRasters[0].CoordEnvelope.Clone();
                dstSize = new Size(srcRasters[0].Width, srcRasters[0].Height);
            }
            IInterestedRaster<T> dstRaster = CreateDstRaster(srcRasters[0], dstEnvelope, rasterIdentify, dstSize);
            IArgumentProvider argprd = new ArgumentProvider(new AlgorithmDef());
            int offsetX = 0, offsetY = 0;
            int dstIndex = 0;
            if (progressTracker != null)
                progressTracker(0, "开始进行统计...");
            int num = 0;
            foreach (IRasterDataProvider rst in srcRasters)
            {
                offsetX = (int)((rst.CoordEnvelope.MinX - dstEnvelope.MinX) / rst.ResolutionX);
                offsetY = (int)((dstEnvelope.MaxY - rst.CoordEnvelope.MaxY) / rst.ResolutionY);
                //by chennan 20120806
                IRasterDataProvider dstTempPrd = GetSubPrd<T>(dstRaster.HostDataProvider, rst, rasterIdentify, offsetX, offsetY);
                try
                {
                    IVirtualRasterDataProvider virtualDataProvider = new VirtualRasterDataProvider(new IRasterDataProvider[] { dstTempPrd, rst });
                    argprd.DataProvider = virtualDataProvider;
                    using (IRasterPixelsVisitor<T> visitor = new RasterPixelsVisitor<T>(argprd))
                    {
                        visitor.VisitPixel(new int[] { 1, 2 },
                            (idx, values) =>
                            {
                                dstIndex = (idx / dstTempPrd.Width + offsetY) * dstRaster.HostDataProvider.Width + (idx % dstTempPrd.Width + offsetX);
                                dstRaster.Put(dstIndex, timesAction(values[0], values[1]));
                            });
                    }
                    num++;
                    int persent = (int)(num * 100f / srcRasters.Length);
                    if (progressTracker != null)
                        progressTracker(persent, "统计完成" + persent + "%");
                }
                finally
                {
                    dstTempPrd.Dispose();
                    if (File.Exists(dstTempPrd.fileName))
                        File.Delete(dstTempPrd.fileName);
                }
                if (progressTracker != null)
                    progressTracker(100, "统计完成");
            }
            for (int i = 0; i < srcRasters.Length; i++)
                srcRasters[i].Dispose();
            return dstRaster;
        }

        unsafe private IRasterDataProvider GetSubPrd<T>(IRasterDataProvider dstPrd, IRasterDataProvider rst, RasterIdentify rasterIdentify, int offsetX, int offsetY)
        {
            string ExtInfos = rasterIdentify.ExtInfos;
            rasterIdentify.ExtInfos = DateTime.Now.ToString("HHmmss");
            IInterestedRaster<T> dst = new InterestedRaster<T>(rasterIdentify, new Size(rst.Width, rst.Height), rst.CoordEnvelope, rst.SpatialRef);
            switch (dstPrd.DataType)
            {
                case enumDataType.Int16:
                    Int16[] dataBlock = new Int16[rst.Width * rst.Height];
                    fixed (Int16* buffer = dataBlock)
                    {
                        IntPtr ptr = new IntPtr(buffer);
                        dstPrd.Read(offsetX, offsetY, rst.Width, rst.Height, ptr, dstPrd.DataType, rst.Width, rst.Height, 1, new int[] { 1 }, enumInterleave.BSQ);
                        IRasterBand bp = dst.HostDataProvider.GetRasterBand(1);
                        bp.Write(0, 0, rst.Width, rst.Height, ptr, enumDataType.Int16, rst.Width, rst.Height);
                    }
                    break;
                case enumDataType.UInt16:
                    UInt16[] dataBlockUint = new UInt16[rst.Width * rst.Height];
                    fixed (UInt16* buffer = dataBlockUint)
                    {
                        IntPtr ptr = new IntPtr(buffer);
                        dstPrd.Read(offsetX, offsetY, rst.Width, rst.Height, ptr, dstPrd.DataType, rst.Width, rst.Height, 1, new int[] { 1 }, enumInterleave.BSQ);
                        IRasterBand bp = dst.HostDataProvider.GetRasterBand(1);
                        bp.Write(0, 0, rst.Width, rst.Height, ptr, enumDataType.UInt16, rst.Width, rst.Height);
                    }
                    break;
            }
            rasterIdentify.ExtInfos = ExtInfos;
            return dst.HostDataProvider;
        }

        private CoordEnvelope GetEnvelopeBySize(IRasterDataProvider prd)
        {
            return new CoordEnvelope(prd.CoordEnvelope.MinX, prd.CoordEnvelope.MinX + prd.Width * prd.ResolutionX, prd.CoordEnvelope.MinY, prd.CoordEnvelope.MinY + prd.Width * prd.ResolutionY);
        }

        public IInterestedRaster<T> Times(string[] fnames, RasterIdentify rasterIdentify, Func<T, T, T> timesAction)
        {
            return Times(fnames, rasterIdentify, null, timesAction);
        }

        public IInterestedRaster<T> Times(string[] fnames, RasterIdentify rasterIdentify, Action<int, string> progressTracker, Func<T, T, T> timesAction)
        {
            if (fnames == null || fnames.Length == 0 || timesAction == null)
                return null;
            if (rasterIdentify == null)
                rasterIdentify = CreateEmptyRasterIdentify();
            IRasterDataProvider[] srcRasters = null;
            try
            {
                srcRasters = new IRasterDataProvider[fnames.Length];
                for (int i = 0; i < srcRasters.Length; i++)
                {
                    srcRasters[i] = GeoDataDriver.Open(fnames[i]) as IRasterDataProvider;
                }
                return Times(srcRasters, rasterIdentify, progressTracker, timesAction);
            }
            finally
            {
                if (srcRasters != null)
                    foreach (IRasterDataProvider prd in srcRasters)
                        prd.Dispose();
            }
        }

        public IInterestedRaster<T> Compare(int[] aoi, string raster1, string raster2, Func<T, T, T> comparer, RasterIdentify rasterIdentify)
        {
            throw new NotImplementedException();
        }

        private IInterestedRaster<T> CreateDstRaster(IRasterDataProvider baseRaster, CoordEnvelope dstEnvelope, RasterIdentify rasterIdentify, Size dstSize)
        {
            ////修改不同区域生成结果文件大小不足问题 by chennan 20120806
            //IInterestedRaster<T> dst = new InterestedRaster<T>(rasterIdentify, new Size(baseRaster.Width, baseRaster.Height), dstEnvelope, baseRaster.SpatialRef);
            IInterestedRaster<T> dst = new InterestedRaster<T>(rasterIdentify, new Size(dstSize.Width, dstSize.Height), dstEnvelope, baseRaster.SpatialRef);
            return dst;
        }

        private RasterIdentify CreateEmptyRasterIdentify()
        {
            throw new NotImplementedException();
        }

        public bool ComputeMaxEnvelope(string[] fnames, out CoordEnvelope outEnvelope, out Size size)
        {
            outEnvelope = null;
            size = Size.Empty;
            if (fnames == null || fnames.Length == 0)
                return false;
            float resX = 0;
            float resY = 0;
            foreach (string f in fnames)
            {
                if (string.IsNullOrEmpty(f))
                    continue;
                try
                {
                    using (IRasterDataProvider prd = GeoDataDriver.Open(f) as IRasterDataProvider)
                    {
                        if (prd == null)
                            continue;
                        if (outEnvelope == null)
                        {
                            outEnvelope = prd.CoordEnvelope.Clone();
                            resX = prd.ResolutionX;
                            resY = prd.ResolutionY;
                        }
                        else
                            outEnvelope = outEnvelope.Union(prd.CoordEnvelope);
                    }
                }
                catch
                {
                    throw;
                }
            }
            size = new Size((int)(outEnvelope.Width / resX), (int)(outEnvelope.Height / resY));
            return true;
        }

        public bool ComputeMinEnvelope(string[] fnames, out CoordEnvelope outEnvelope, out Size size)
        {
            outEnvelope = null;
            size = Size.Empty;
            if (fnames == null || fnames.Length == 0)
                return false;
            float resX = 0;
            float resY = 0;
            foreach (string f in fnames)
            {
                if (string.IsNullOrEmpty(f))
                    continue;
                try
                {
                    using (IRasterDataProvider prd = GeoDataDriver.Open(f) as IRasterDataProvider)
                    {
                        if (prd == null)
                            continue;
                        if (outEnvelope == null)
                        {
                            outEnvelope = prd.CoordEnvelope.Clone();
                            resX = prd.ResolutionX;
                            resY = prd.ResolutionY;
                        }
                        else
                            outEnvelope = outEnvelope.Intersect(prd.CoordEnvelope);
                    }
                }
                catch
                {
                    throw;
                }
            }
            size = new Size((int)(outEnvelope.Width / resX), (int)(outEnvelope.Height / resY));
            return true;
        }

        public IInterestedRaster<T> CycleTimes(IRasterDataProvider[] srcRasters, RasterIdentify rasterIdentify, Func<int, T, T, T> iTimesGetter, Action<int, string> progress)
        {
            ////修改不同区域生成结果文件大小不足问题 by chennan 20120806
            //CoordEnvelope dstEnvelope = srcRasters[srcRasters.Length - 1].CoordEnvelope;
            CoordEnvelope dstEnvelope = GetEnvelopeBySize(srcRasters[srcRasters.Length - 1]);
            float minResolutionX = 0f;
            float minResolutionY = 0f;
            Size dstSize = Size.Empty; ;
            for (int i = 0; i < srcRasters.Length - 1; i++)
            {
                if (srcRasters[i].DataType != srcRasters[i + 1].DataType)
                    throw new ArgumentException("数据类型不一致无法进行频次统计！");
                dstEnvelope = dstEnvelope.Union(GetEnvelopeBySize(srcRasters[i]));
                // by chennan 20120806
                minResolutionX = Math.Min(srcRasters[i].ResolutionX, srcRasters[i + 1].ResolutionX);
                minResolutionY = Math.Min(srcRasters[i].ResolutionY, srcRasters[i + 1].ResolutionY);
                dstSize = new Size((int)Math.Floor(dstEnvelope.Width / minResolutionX), (int)Math.Floor(dstEnvelope.Height / minResolutionY));
            }
            //by chennan 20121025 修改单文件无法进行周期统计
            if (srcRasters.Length == 1)
            {
                minResolutionX = srcRasters[0].ResolutionX;
                minResolutionY = srcRasters[0].ResolutionY;
                dstEnvelope = srcRasters[0].CoordEnvelope.Clone();
                dstSize = new Size(srcRasters[0].Width, srcRasters[0].Height);
            }
            IInterestedRaster<T> dstRaster = CreateDstRaster(srcRasters[0], dstEnvelope, rasterIdentify, dstSize);
            IArgumentProvider argprd = new ArgumentProvider(new AlgorithmDef());
            int offsetX = 0, offsetY = 0;
            int dstIndex = 0;
            int index = 0;
            foreach (IRasterDataProvider rst in srcRasters)
            {
                if (progress != null)
                    progress((int)(index++ * 100f / srcRasters.Length), "正在执行周期统计");
                CoordEnvelope inc = rst.CoordEnvelope.Intersect(dstEnvelope);
                if (inc == null || inc.IsEmpty())
                    continue;
                int iCycle = Array.IndexOf<IRasterDataProvider>(srcRasters, rst) + 1;
                offsetX = (int)((rst.CoordEnvelope.MinX - dstEnvelope.MinX) / rst.ResolutionX);
                offsetY = (int)((dstEnvelope.MaxY - rst.CoordEnvelope.MaxY) / rst.ResolutionY);
                //by chennan 20120806
                IRasterDataProvider dstTempPrd = GetSubPrd<T>(dstRaster.HostDataProvider, rst, rasterIdentify, offsetX, offsetY);
                try
                {
                    IVirtualRasterDataProvider virtualDataProvider = new VirtualRasterDataProvider(new IRasterDataProvider[] { dstTempPrd, rst });
                    argprd.DataProvider = virtualDataProvider;
                    using (IRasterPixelsVisitor<T> visitor = new RasterPixelsVisitor<T>(argprd))
                    {
                        visitor.VisitPixel(new int[] { 1, 2 },
                            (idx, values) =>
                            {
                                dstIndex = (idx / dstTempPrd.Width + offsetY) * dstRaster.HostDataProvider.Width + (idx % dstTempPrd.Width + offsetX);
                                dstRaster.Put(dstIndex, iTimesGetter(iCycle, values[0], values[1]));
                            });
                    }
                }
                finally
                {
                    dstTempPrd.Dispose();
                    if (File.Exists(dstTempPrd.fileName))
                        File.Delete(dstTempPrd.fileName);
                }
            }
            for (int i = 0; i < srcRasters.Length; i++)
                srcRasters[i].Dispose();
            return dstRaster;
        }

        public IInterestedRaster<T> CycleTimes(string[] fnames, RasterIdentify rasterIdentify, Func<int, T, T, T> iTimesGetter, Action<int, string> progress)
        {
            if (fnames == null || fnames.Length == 0 || iTimesGetter == null)
                return null;
            if (rasterIdentify == null)
                rasterIdentify = CreateEmptyRasterIdentify();
            IRasterDataProvider[] srcRasters = null;
            try
            {
                srcRasters = new IRasterDataProvider[fnames.Length];
                for (int i = 0; i < srcRasters.Length; i++)
                {
                    srcRasters[i] = GeoDataDriver.Open(fnames[i]) as IRasterDataProvider;
                }
                return CycleTimes(srcRasters, rasterIdentify, iTimesGetter, progress);
            }
            finally
            {
                if (srcRasters != null)
                    foreach (IRasterDataProvider prd in srcRasters)
                        prd.Dispose();
                srcRasters = null;
            }
        }
    }
}
