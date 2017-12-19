using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.DF.NOAA.BandPrd
{
    public abstract class SecondaryBand:RasterBand
    {
        public SecondaryBand(IRasterDataProvider rasterDataProvider)
            :base(rasterDataProvider)
        { 
        }
        
        public override void ComputeHistogram(double begin, double end, int buckets, int[] histogram, bool isIncludeOutOfRange, bool isCanApprox, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }

        public override void ComputeMinMax(double begin, double end, out double min, out double max, bool isCanApprox, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }

        public override void ComputeMinMax(out double min, out double max, bool isCanApprox, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }

        public override void ComputeStatistics(double begin, double end, out double min, out double max, out double mean, out double stddev, bool isCanApprox, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }

        public override void ComputeStatistics(out double min, out double max, out double mean, out double stddev, bool isCanApprox, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }

        protected override void DirectRead(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            throw new NotImplementedException();
        }

        protected override void DirectWrite(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            throw new NotImplementedException();
        }

        public override void Fill(double noDataValue)
        {
            throw new NotImplementedException();
        }
    }
}
