using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.DF.HDF4.Cloudsat
{
    public class CloudSatRasterBand : RasterBand
    {
        Dictionary<string, string> _attribute = null;
        private CloudsatDataProvider _rasterProvider = null;
        private H4SDS _sds = null;
        private string _bandNmae = "";

        public CloudSatRasterBand(IRasterDataProvider rasterDataProvider, H4SDS sds, int bandNo)
            : base(rasterDataProvider)
        {
            _rasterProvider = rasterDataProvider as CloudsatDataProvider;
            _sds = sds;
            _bandNo = bandNo;
            _attribute = _attributes.CreateAttributeDomain("Attribute");
            int rank = sds.Rank;
            int[] dims = sds.Dimsizes;
            if (rank == 1)
            {
                _width = 1;
                _height = dims[0];
            }
            else if (rank == 2)
            {
                _height = dims[0];
                _width = dims[1];
            }
            _bandNmae = sds.Name;
            _dataType = enumDataType.Int16;
        }

        protected override void DirectRead(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            if (xSize == xBufferSize && ySize == yBufferSize)
            {
                int[] start = new int[] { yOffset, xOffset };
                int[] edges = new int[] { ySize, xSize };
                _sds.Read(start, null, edges, buffer);
            }
            else
            {
            }
        }

        protected override void DirectWrite(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            throw new NotImplementedException();
        }

        public override void Fill(double noDataValue)
        {
            throw new NotImplementedException();
        }

        public override void ComputeMinMax(out double min, out double max, bool isCanApprox, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }

        public override void ComputeMinMax(double begin, double end, out double min, out double max, bool isCanApprox, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }

        public override void ComputeStatistics(out double min, out double max, out double mean, out double stddev, bool isCanApprox, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }

        public override void ComputeStatistics(double begin, double end, out double min, out double max, out double mean, out double stddev, bool isCanApprox, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }

        public override void ComputeHistogram(double begin, double end, int buckets, int[] histogram, bool isIncludeOutOfRange, bool isCanApprox, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }
    }
}
