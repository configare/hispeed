using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.MIF.Core
{
    public class TryCreateVirtualPrdByMultiFile : ITryCreateVirtualPrd
    {
        public IVirtualRasterDataProvider CreateVirtualRasterPRD(ref Dictionary<string, FilePrdMap> filePrdMap)
        {
            bool sucess = GetSameSizeFilePrdMap(ref filePrdMap, false);
            if (!sucess)
                return null;
            IRasterDataProvider dstDataProvider = null;
            List<IRasterDataProvider> prdList = new List<IRasterDataProvider>();
            IRasterDataProviderConverter converter = new RasterDataProviderConverter();
            FilePrdMap fpm = null;
            string dstPath = GetTemplateFilePath();
            string dstFilename;
            foreach (string key in filePrdMap.Keys)
            {
                fpm = filePrdMap[key];
                dstFilename = dstPath + Guid.NewGuid() + ".ldf";
                switch (fpm.Prd.DataType)
                {
                    case enumDataType.Byte:
                        dstDataProvider = converter.ConvertDataType<Byte, float>(fpm.Prd.fileName, enumDataType.Float, dstFilename, (v) => { return (float)(v / fpm.Zoom); });
                        break;
                    case enumDataType.Double:
                        dstDataProvider = converter.ConvertDataType<Double, float>(fpm.Prd.fileName, enumDataType.Float, dstFilename, (v) => { return (float)(v / fpm.Zoom); });
                        break;
                    case enumDataType.Int16:
                        dstDataProvider = converter.ConvertDataType<Int16, float>(fpm.Prd.fileName, enumDataType.Float, dstFilename, (v) => { return (float)(v / fpm.Zoom); });
                        break;
                    case enumDataType.Int32:
                        dstDataProvider = converter.ConvertDataType<Int32, float>(fpm.Prd.fileName, enumDataType.Float, dstFilename, (v) => { return (float)(v / fpm.Zoom); });
                        break;
                    case enumDataType.Int64:
                        dstDataProvider = converter.ConvertDataType<Int64, float>(fpm.Prd.fileName, enumDataType.Float, dstFilename, (v) => { return (float)(v / fpm.Zoom); });
                        break;
                    case enumDataType.UInt16:
                        dstDataProvider = converter.ConvertDataType<UInt16, float>(fpm.Prd.fileName, enumDataType.Float, dstFilename, (v) => { return (float)(v / fpm.Zoom); });
                        //dstDataProvider = converter.ConvertDataType<UInt16, UInt16>(fpm.Prd.fileName, enumDataType.Float, dstFilename, (v) => { return (UInt16)(v / fpm.Zoom); });
                        break;
                    case enumDataType.UInt32:
                        dstDataProvider = converter.ConvertDataType<UInt32, float>(fpm.Prd.fileName, enumDataType.Float, dstFilename, (v) => { return (float)(v / fpm.Zoom); });
                        break;
                    case enumDataType.UInt64:
                        dstDataProvider = converter.ConvertDataType<UInt64, float>(fpm.Prd.fileName, enumDataType.Float, dstFilename, (v) => { return (float)(v / fpm.Zoom); });
                        break;
                    case enumDataType.Float:
                        dstDataProvider = converter.ConvertDataType<float, float>(fpm.Prd.fileName, enumDataType.Float, dstFilename, (v) => { return (float)(v / fpm.Zoom); });
                        break;
                }
                //dstDataProvider = CreatDstDataProvider(fpm, dstFilename);
                fpm.Prd.Dispose();
                fpm.Prd = dstDataProvider;
                fpm.Filename = dstDataProvider.fileName;
                prdList.Add(dstDataProvider);
            }
            return new VirtualRasterDataProvider(prdList.ToArray());
        }

        private IRasterDataProvider CreatDstDataProvider(FilePrdMap map, string dstFilename)
        {
            using (IRasterDataProvider srcDataProvider = map.Prd)
            {
                IRasterDataDriver driver = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
                if (driver == null)
                    throw new Exception("获取LDF栅格数据驱动时发生未知错误!");
                if (!dstFilename.ToLower().EndsWith(".ldf"))
                    dstFilename += ".ldf";
                enumDataType dstDataType = srcDataProvider.DataType;
                IRasterDataProvider dstDataProvider = driver.Create(dstFilename, srcDataProvider.Width,
                    srcDataProvider.Height,
                    srcDataProvider.BandCount,
                    dstDataType,
                    "SPATIALREF=" + GetSpatialRefString(srcDataProvider),
                    GetMapInfoString(srcDataProvider.CoordEnvelope, srcDataProvider.Width, srcDataProvider.Height));

                int blockRows = 100;
                int blockCount = (int)Math.Ceiling((float)srcDataProvider.Height / blockRows); //总块数
                int bRow = 0, eRow = 0;
                int height = srcDataProvider.Height;
                int width = srcDataProvider.Width;
                int bufferRowCount = 0;
                UInt16[] srcBuffer = new UInt16[blockRows * srcDataProvider.Width];
                UInt16[] dstBuffer = new UInt16[blockRows * srcDataProvider.Width];
                GCHandle srcHandle = GCHandle.Alloc(srcBuffer, GCHandleType.Pinned);
                GCHandle dstHandle = GCHandle.Alloc(dstBuffer, GCHandleType.Pinned);
                try
                {
                    for (int b = 1; b <= srcDataProvider.BandCount; b++)
                    {
                        IRasterBand srcBand = srcDataProvider.GetRasterBand(b);
                        IRasterBand dstBand = dstDataProvider.GetRasterBand(b);

                        bRow = 0;
                        //
                        for (int blocki = 0; blocki < blockCount; blocki++, bRow += blockRows)
                        {
                            eRow = Math.Min(height, bRow + blockRows);
                            bufferRowCount = eRow - bRow;
                            srcBand.Read(0, bRow, width, bufferRowCount, srcHandle.AddrOfPinnedObject(), srcDataProvider.DataType, width, bufferRowCount);
                            //
                            int count = bufferRowCount * width;
                            for (int i = 0; i < count; i++)
                                dstBuffer[i] = srcBuffer[i];
                            //
                            dstBand.Write(0, bRow, width, bufferRowCount, dstHandle.AddrOfPinnedObject(), dstDataType, width, bufferRowCount);
                        }
                    }
                }
                finally
                {
                    srcHandle.Free();
                    dstHandle.Free();
                }
                return dstDataProvider;
            }
        }

        private unsafe bool GetSameSizeFilePrdMap(ref Dictionary<string, FilePrdMap> filePrdMap, bool interpolation)
        {
            if (filePrdMap == null || filePrdMap.Count == 0)
                return false;
            FilePrdMap fpm = null;
            int startBand = 1;
            CoordEnvelope dstEnvelope = null;
            double dstResoltion = 0;
            Size dstSize = Size.Empty;
            string dstPath = GetTemplateFilePath();
            bool same = GetDstRegionInfos(ref filePrdMap, interpolation, out dstEnvelope, out dstResoltion, out dstSize);
            if (dstEnvelope == null)
                return false;
            if (!same)
            {
                int offsetX = 0;
                int offsetY = 0;
                string dstFilename;
                List<int> bandNumTemp = new List<int>();
                using (IRasterDataDriver drv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver)
                {
                    foreach (string key in filePrdMap.Keys)
                    {
                        fpm = filePrdMap[key];
                        //if (!fpm.SameRegion)
                        //{
                        IRasterBand band = null;
                        offsetX = GetOffset(fpm.Prd.CoordEnvelope.MinX, dstEnvelope.MinX, fpm.Prd.ResolutionX);
                        offsetY = GetOffset(fpm.Prd.CoordEnvelope.MaxY, dstEnvelope.MaxY, fpm.Prd.ResolutionY);
                        dstFilename = dstPath + Guid.NewGuid() + ".ldf";
                        IRasterDataProvider prdWriter = drv.Create(dstFilename, dstSize.Width, dstSize.Height, fpm.BandNums.Length,
                             fpm.Prd.DataType, "INTERLEAVE=BSQ", "VERSION=LDF", "WITHHDR=TRUE", "SPATIALREF=" + GetSpatialRefString(fpm.Prd),
                             GetMapInfoString(dstEnvelope, dstSize.Width, dstSize.Height)) as IRasterDataProvider;
                        switch (fpm.Prd.DataType)
                        {
                            case enumDataType.Byte:
                                for (int i = 0; i < fpm.BandNums.Length; i++)
                                {
                                    byte[] dataBlock = new byte[dstSize.Width * dstSize.Height];
                                    fixed (byte* buffer = dataBlock)
                                    {
                                        IntPtr ptr = new IntPtr(buffer);
                                        ReadWriteCustomizingRegion(fpm, dstEnvelope, dstSize, prdWriter, offsetX, offsetY, dstFilename, bandNumTemp, drv, band, i, ptr);
                                    }
                                }
                                break;
                            case enumDataType.Double:
                                for (int i = 0; i < fpm.BandNums.Length; i++)
                                {
                                    double[] dataBlock = new double[dstSize.Width * dstSize.Height];
                                    fixed (double* buffer = dataBlock)
                                    {
                                        IntPtr ptr = new IntPtr(buffer);
                                        ReadWriteCustomizingRegion(fpm, dstEnvelope, dstSize, prdWriter, offsetX, offsetY, dstFilename, bandNumTemp, drv, band, i, ptr);
                                    }
                                }
                                break;
                            case enumDataType.Float:
                                for (int i = 0; i < fpm.BandNums.Length; i++)
                                {
                                    float[] dataBlock = new float[dstSize.Width * dstSize.Height];
                                    fixed (float* buffer = dataBlock)
                                    {
                                        IntPtr ptr = new IntPtr(buffer);
                                        ReadWriteCustomizingRegion(fpm, dstEnvelope, dstSize, prdWriter, offsetX, offsetY, dstFilename, bandNumTemp, drv, band, i, ptr);
                                    }
                                }
                                break;
                            case enumDataType.Int16:
                                for (int i = 0; i < fpm.BandNums.Length; i++)
                                {
                                    Int16[] dataBlock = new Int16[dstSize.Width * dstSize.Height];
                                    fixed (Int16* buffer = dataBlock)
                                    {
                                        IntPtr ptr = new IntPtr(buffer);
                                        ReadWriteCustomizingRegion(fpm, dstEnvelope, dstSize, prdWriter, offsetX, offsetY, dstFilename, bandNumTemp, drv, band, i, ptr);
                                    }
                                }
                                break;
                            case enumDataType.Int32:
                                for (int i = 0; i < fpm.BandNums.Length; i++)
                                {
                                    Int32[] dataBlock = new Int32[dstSize.Width * dstSize.Height];
                                    fixed (Int32* buffer = dataBlock)
                                    {
                                        IntPtr ptr = new IntPtr(buffer);
                                        ReadWriteCustomizingRegion(fpm, dstEnvelope, dstSize, prdWriter, offsetX, offsetY, dstFilename, bandNumTemp, drv, band, i, ptr);
                                    }
                                }
                                break;
                            case enumDataType.Int64:
                                for (int i = 0; i < fpm.BandNums.Length; i++)
                                {
                                    Int64[] dataBlock = new Int64[dstSize.Width * dstSize.Height];
                                    fixed (Int64* buffer = dataBlock)
                                    {
                                        IntPtr ptr = new IntPtr(buffer);
                                        ReadWriteCustomizingRegion(fpm, dstEnvelope, dstSize, prdWriter, offsetX, offsetY, dstFilename, bandNumTemp, drv, band, i, ptr);
                                    }
                                }
                                break;
                            case enumDataType.UInt16:
                                for (int i = 0; i < fpm.BandNums.Length; i++)
                                {
                                    UInt16[] dataBlock = new UInt16[dstSize.Width * dstSize.Height];
                                    fixed (UInt16* buffer = dataBlock)
                                    {
                                        IntPtr ptr = new IntPtr(buffer);
                                        ReadWriteCustomizingRegion(fpm, dstEnvelope, dstSize, prdWriter, offsetX, offsetY, dstFilename, bandNumTemp, drv, band, i, ptr);
                                    }
                                }
                                break;
                            case enumDataType.UInt32:
                                for (int i = 0; i < fpm.BandNums.Length; i++)
                                {
                                    UInt32[] dataBlock = new UInt32[dstSize.Width * dstSize.Height];
                                    fixed (UInt32* buffer = dataBlock)
                                    {
                                        IntPtr ptr = new IntPtr(buffer);
                                        ReadWriteCustomizingRegion(fpm, dstEnvelope, dstSize, prdWriter, offsetX, offsetY, dstFilename, bandNumTemp, drv, band, i, ptr);
                                    }
                                }
                                break;
                            case enumDataType.UInt64:
                                for (int i = 0; i < fpm.BandNums.Length; i++)
                                {
                                    UInt64[] dataBlock = new UInt64[dstSize.Width * dstSize.Height];
                                    fixed (UInt64* buffer = dataBlock)
                                    {
                                        IntPtr ptr = new IntPtr(buffer);
                                        ReadWriteCustomizingRegion(fpm, dstEnvelope, dstSize, prdWriter, offsetX, offsetY, dstFilename, bandNumTemp, drv, band, i, ptr);
                                    }
                                }
                                break;
                        }
                        fpm.StartBand = startBand;
                        fpm.BandCount = fpm.BandNums.Length;
                        fpm.BandNums = bandNumTemp.ToArray();
                        bandNumTemp.Clear();
                        fpm.Filename = dstFilename;
                        fpm.SameRegion = true;
                        fpm.Prd.Dispose();
                        prdWriter.Dispose();
                        fpm.Prd = GeoDataDriver.Open(dstFilename) as IRasterDataProvider;
                        startBand += fpm.BandCount;
                        //}
                        //else
                        //    fpm.StartBand = startBand;
                    }
                }
            }
            return true;
        }

        unsafe private void ReadWriteCustomizingRegion(FilePrdMap fpm, CoordEnvelope dstEnvelope, Size dstSize, IRasterDataProvider prdWriter, int offsetX, int offsetY, string dstFilename, List<int> bandNumTemp, IRasterDataDriver drv, IRasterBand band, int i, IntPtr ptr)
        {
            fpm.Prd.Read(offsetX, offsetY, dstSize.Width, dstSize.Height, ptr, fpm.Prd.DataType, dstSize.Width, dstSize.Height, 1,
                new int[] { fpm.BandNums[i] }, enumInterleave.BSQ);

            band = prdWriter.GetRasterBand(i + 1);
            band.Write(0, 0, band.Width, band.Height, ptr, fpm.Prd.DataType, band.Width, band.Height);

            bandNumTemp.Add(i + 1);
        }

        private string GetSpatialRefString(IRasterDataProvider srcDataProvider)
        {
            return srcDataProvider.SpatialRef != null ? srcDataProvider.SpatialRef.ToProj4String() : string.Empty;
        }

        private string GetMapInfoString(CoordEnvelope coordEnvelope, int width, int height)
        {
            return coordEnvelope != null ? coordEnvelope.ToMapInfoString(new Size(width, height)) : string.Empty;
        }

        private int GetOffset(double src, double dst, float srcResolution)
        {
            return (int)Math.Floor(Math.Abs(src - dst) / srcResolution);
        }


        private bool GetDstRegionInfos(ref Dictionary<string, FilePrdMap> filePrdMap, bool interpolation, out CoordEnvelope dstEnvelope,
                                       out double dstResoltion, out Size dstSize)
        {
            IRasterDataProvider prd = null;
            string file;
            FilePrdMap fpm = null;
            dstEnvelope = null;
            dstResoltion = 0;
            int dstWidth = 0;
            int dstHeight = 0;
            dstSize = Size.Empty;
            int startBand = 1;
            bool same = true;
            CoordEnvelope prdEnvelope = null;
            float lonRegion = 0f;
            float latRegion = 0f;
            foreach (string key in filePrdMap.Keys)
            {
                fpm = filePrdMap[key];
                file = fpm.Filename;
                prd = GeoDataDriver.Open(file) as IRasterDataProvider;
                fpm.Prd = prd;
                fpm.StartBand += startBand;
                if (!SameRegion(prd, dstEnvelope, dstResoltion, dstWidth, dstHeight))
                {
                    lonRegion = prd.Width * prd.ResolutionX;
                    latRegion = prd.Height * prd.ResolutionY;
                    prdEnvelope = new CoordEnvelope(prd.CoordEnvelope.MinX, prd.CoordEnvelope.MinX + lonRegion, prd.CoordEnvelope.MinY, prd.CoordEnvelope.MinY + latRegion);
                    //修改输出图像大小与源图像大小不符问题  by chennan 20120802
                    //dstEnvelope = prd.CoordEnvelope.Intersect(dstEnvelope == null ? prd.CoordEnvelope : dstEnvelope);
                    dstEnvelope = prdEnvelope.Intersect(dstEnvelope == null ? prdEnvelope : dstEnvelope);
                    //by chennan 20120922 修改文件没有任何相交时报错问题
                    if (dstEnvelope == null)
                    {
                        dstEnvelope = null;
                        return false;
                    }
                    //
                    fpm.SameRegion = false;
                    same = false;
                    //dstResoltion = GetSimilarResolution(interpolation ? Math.Min(prd.ResolutionX, dstResoltion) : Math.Max(prd.ResolutionX, dstResoltion));
                    dstResoltion = interpolation ? Math.Min(prd.ResolutionX, dstResoltion) : Math.Max(prd.ResolutionX, dstResoltion);
                    dstWidth = (int)Math.Floor(dstEnvelope.Width / dstResoltion);
                    dstHeight = (int)Math.Floor(dstEnvelope.Height / dstResoltion);
                }
                fpm.BandCount = fpm.Prd.BandCount;
                startBand += fpm.BandCount;
            }
            dstSize = new Size(dstWidth, dstHeight);
            return same;
        }

        /// <summary>
        /// 获取近似分辨率，目前支持等经纬度 0.001 0.005 0025
        /// </summary>
        /// <param name="resoltion"></param>
        /// <returns></returns>
        private double GetSimilarResolution(double resoltion)
        {
            double halfResolution = resoltion + resoltion / 2;
            string halfResolutionStr = halfResolution.ToString();
            if (halfResolution > 0.01 && halfResolution < 0.02)
                return double.Parse(halfResolutionStr.Substring(halfResolutionStr.LastIndexOf("."), 3));
            if (halfResolution > 0.005 && halfResolution < 0.01)
                return double.Parse(halfResolutionStr.Substring(halfResolutionStr.LastIndexOf("."), 4));
            if (halfResolution > 0.0025 && halfResolution < 0.005)
                return double.Parse(halfResolutionStr.Substring(halfResolutionStr.LastIndexOf("."), 5));
            return resoltion;
        }

        private bool SameRegion(IRasterDataProvider prd, CoordEnvelope envelope, double resoltion, int width, int height)
        {
            if (Math.Abs(GetSimilarResolution(prd.ResolutionX) - resoltion) >= resoltion)
                return false;
            if (Math.Abs(prd.CoordEnvelope.MinX - envelope.MinX) > resoltion || Math.Abs(prd.CoordEnvelope.MaxY - envelope.MaxY) > resoltion)
                return false;
            if (prd.Width != width || prd.Height != height)
                return false; ;
            return true;
        }

        private string GetTemplateFilePath()
        {
            return AppDomain.CurrentDomain.BaseDirectory + "Temp\\";
        }
    }
}
