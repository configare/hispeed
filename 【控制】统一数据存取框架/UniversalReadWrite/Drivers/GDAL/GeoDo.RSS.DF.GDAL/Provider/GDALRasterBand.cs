using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.Project;
using GeoDo.RSS.Core.DF;
using OSGeo.GDAL;

namespace GeoDo.RSS.DF.GDAL
{
    public class GDALRasterBand : RasterBand, IGDALRasterBand
    {
        protected Band _band = null;
        private Action<int, string> _progressCallback = null;
        protected GDALDataset _dataset = null;

        public GDALRasterBand(IRasterDataProvider rasterDataProvider)
            : base(rasterDataProvider)
        {
        }

        public GDALRasterBand(IRasterDataProvider rasterDataProvider, Band band,GDALDataset dataset)
            : base(rasterDataProvider)
        {
            _band = band;
            _dataset = dataset;
            InitLocalFields();
        }

        private void InitLocalFields()
        {
            double val = 0;
            int hasVal = 0;
            _dataType = GDALHelper.GDALDataType2DataType(_band.DataType);
            _band.GetScale(out val, out hasVal);
            if (hasVal != 0)
                _dataScale = val;
            _band.GetOffset(out val, out hasVal);
            if (hasVal != 0)
                _dataOffset = val;
            GDALHelper.GetBandAttributes(_band, _attributes);
            _description = _band.GetDescription();
            //_measureBits = ?
            _width = _band.XSize;
            _height = _band.YSize;
            _band.GetNoDataValue(out val, out hasVal);
            if (hasVal != 0)
                _noDataValue = val;
            _spatialRef = _rasterDataProvider.SpatialRef;
            _coordTransofrm = _rasterDataProvider.CoordTransform;
        }

        private object lockObj = new object();
        protected override void DirectRead(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            lock (lockObj)
            {
                _band.ReadRaster(xOffset, yOffset, xSize, ySize, buffer, xBufferSize, yBufferSize, GDALHelper.DataType2GDALDataType(dataType), 0, 0);
            }
        }

        protected override void DirectWrite(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            _band.WriteRaster(xOffset, yOffset, xSize, ySize, buffer, xBufferSize, yBufferSize, GDALHelper.DataType2GDALDataType(dataType), 0, 0);
        }

        public override void Fill(double noDataValue)
        {
            _band.Fill(noDataValue, 0);
        }

        public override void ComputeMinMax(out double min, out double max, bool isCanApprox, Action<int, string> progressCallback)
        {
            double[] outs = new double[2];
            _band.ComputeRasterMinMax(outs, isCanApprox ? 1 : 0);
            min = outs[0];
            max = outs[1];
            if (progressCallback != null)
                progressCallback(100, string.Empty);
        }

        public override void ComputeMinMax(double begin, double end, out double min, out double max, bool isCanApprox, Action<int, string> progressCallback)
        {
            min = max = 0;
            if (begin < double.Epsilon && end < double.Epsilon)
            {
                ComputeMinMax(out min, out max, isCanApprox, progressCallback);
                return;
            }
            int interleave = isCanApprox ? GeoDo.RSS.Core.DF.Constants.DEFAULT_PIXELES_INTERLEAVE : 1;
            MaxMinValuesComputer.ComputeMinMax(_band, interleave, begin, end, out min, out max, isCanApprox, progressCallback);
        }

        public override void ComputeStatistics(out double min, out double max, out double mean, out double stddev, bool isCanApprox, Action<int, string> progressCallback)
        {
            try
            {
                _band.ComputeStatistics(isCanApprox, out min, out max, out mean, out stddev, GetGdalProgressCallback(progressCallback), null);
            }
            finally
            {
                _progressCallback = null;
            }
        }

        public override void ComputeStatistics(double begin, double end, out double min, out double max, out double mean, out double stddev, bool isCanApprox, Action<int, string> progressCallback)
        {
            if (begin < double.Epsilon && end < double.Epsilon)
            {
                ComputeStatistics(out min, out max, out mean, out stddev, isCanApprox, progressCallback);
                return;
            }
            int interleave = isCanApprox ? GeoDo.RSS.Core.DF.Constants.DEFAULT_PIXELES_INTERLEAVE : 1;
            StatValuesComputer.Stat(_band, interleave, begin, end, out min, out max, out mean, out stddev, isCanApprox, progressCallback);
        }

        public override void ComputeHistogram(double begin, double end, int buckets, int[] histogram, bool isIncludeOutOfRange, bool isCanApprox, Action<int, string> progressCallback)
        {
            try
            {
                _progressCallback = progressCallback;
                _band.GetHistogram(begin, end, buckets, histogram, isIncludeOutOfRange ? 1 : 0, isCanApprox ? 1 : 0, GetGdalProgressCallback(progressCallback), null);
            }
            finally
            {
                _progressCallback = null;
            }
        }

        private Gdal.GDALProgressFuncDelegate GetGdalProgressCallback(Action<int, string> progressCallback)
        {
            _progressCallback = progressCallback;
            return new Gdal.GDALProgressFuncDelegate(GdalProgressDelegate);
        }

        private int GdalProgressDelegate(double percent, IntPtr p1, IntPtr p2)
        {
            percent *= 100;
            if (_progressCallback != null)
                _progressCallback((int)percent, string.Empty);
            return GeoDo.RSS.Core.DF.Constants.IS_TRUE;
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_band != null)
            {
                _band.Dispose();
                _band = null;
            }
            if (_dataset != null)
            {
                _dataset.Dispose();
                _dataset = null;
            }
            _progressCallback = null;
        }
    }
}
