using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.RasterTools
{
    public class AOIRasterFetcher<T>:IAOIRasterFetcher<T>
    {
        public void Fetch(IRasterDataProvider rasterDataProvider, int[] aoi, int[] bandNos, out T[][] banbValues)
        {
            if (rasterDataProvider == null || aoi == null || aoi.Length == 0 || bandNos == null || bandNos.Length == 0)
                throw new ArgumentException();
            int minBandNo = bandNos.Min();
            int maxBandNo = bandNos.Max();
            if (minBandNo < 1 || maxBandNo > rasterDataProvider.BandCount)
                throw new ArgumentException("BandNos");
            IRasterBand[] rasterBands = new IRasterBand[bandNos.Length];
            banbValues = new T[bandNos.Length][];
            for (int i = 0; i < bandNos.Length; i++)
            {
                rasterBands[i] = rasterDataProvider.GetRasterBand(bandNos[i]);
                banbValues[i] = new T[aoi.Length];
            }
            int width = rasterDataProvider.Width ;
            int count = aoi.Length ;
            int bandCount = rasterBands.Length ;
            T[] buffer = new T[1];
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            IntPtr bufferPtr = handle.AddrOfPinnedObject();
            try
            {
                int row =0,col =0;
                for (int i = 0; i < count; i++)
                {
                    row = aoi[i] / width ;
                    col = aoi[i] % width;
                    for (int b = 0; b < bandCount; b++)
                    {
                        rasterBands[b].Read(col, row, 1, 1, bufferPtr, rasterDataProvider.DataType, 1, 1);
                        banbValues[b][i] = buffer[0];
                    }
                }
            }
            finally 
            {
                handle.Free();
            }
        }
    }
}
