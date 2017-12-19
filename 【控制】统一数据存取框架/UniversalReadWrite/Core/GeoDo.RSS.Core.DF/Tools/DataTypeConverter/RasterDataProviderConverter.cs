using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.Core.DF
{
    public class RasterDataProviderConverter : IRasterDataProviderConverter
    {
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void MemoryCopy(IntPtr pDest, IntPtr pSrc, int length);

        public IRasterDataProvider ConvertDataType<TSrc, TDst>(string srcFileName, enumDataType dstDataType, string dstFileName, Func<TSrc, TDst> converter)
        {
            IRasterDataProvider srcDataProvider = GeoDataDriver.Open(srcFileName) as IRasterDataProvider;
            if (srcDataProvider == null)
                new Exception("创建源数据提供者时发生错误！");
            try
            {
                return ConvertDataType<TSrc, TDst>(srcDataProvider, dstDataType, dstFileName,converter);
            }
            finally 
            {
                srcDataProvider.Dispose();
            }
        }

        public IRasterDataProvider ConvertDataType<TSrc, TDst>(IRasterDataProvider srcDataProvider, enumDataType dstDataType, string dstFileName, Func<TSrc, TDst> converter)
        {
            if (srcDataProvider == null || string.IsNullOrEmpty(dstFileName))
                return null;
            IRasterDataProvider dstDataProvider = null;
            try
            {
                dstDataProvider = CreateDstDataProvider(dstDataType, srcDataProvider, dstFileName);
                if (dstDataProvider == null)
                    new Exception("创建目标数据提供者时发生未知错误！");
            }
            catch (Exception ex)
            {
                throw new Exception("创建目标数据提供者时发生错误!", ex);
            }
            //
            int blockRows = 100;
            int blockCount = (int)Math.Ceiling((float)srcDataProvider.Height / blockRows); //总块数
            int bRow = 0, eRow = 0;
            int height = srcDataProvider.Height;
            int width = srcDataProvider.Width;
            int bufferRowCount = 0;
            TSrc[] srcBuffer = new TSrc[blockRows * srcDataProvider.Width];
            TDst[] dstBuffer = new TDst[blockRows * srcDataProvider.Width];
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
                            dstBuffer[i] = converter(srcBuffer[i]);
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

        private IRasterDataProvider CreateDstDataProvider(enumDataType dstDataType, IRasterDataProvider srcDataProvider, string dstFileName)
        {
            IRasterDataDriver driver = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            if (driver == null)
                throw new Exception("获取LDF栅格数据驱动时发生未知错误!");
            if (!dstFileName.ToLower().EndsWith(".ldf"))
                dstFileName += ".ldf";
            return driver.Create(dstFileName, srcDataProvider.Width,
                srcDataProvider.Height,
                srcDataProvider.BandCount,
                dstDataType,
                "SPATIALREF=" + GetSpatialRefString(srcDataProvider),
                GetMapInfoString(srcDataProvider.CoordEnvelope, srcDataProvider.Width, srcDataProvider.Height));
        }

        private string GetSpatialRefString(IRasterDataProvider srcDataProvider)
        {
            return srcDataProvider.SpatialRef != null ? srcDataProvider.SpatialRef.ToProj4String() : string.Empty;
        }

        private string GetMapInfoString(CoordEnvelope coordEnvelope, int width, int height)
        {
            return coordEnvelope != null ? coordEnvelope.ToMapInfoString(new Size(width, height)) : string.Empty;
        }
    }
}
