using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO;

namespace GeoDo.RSS.DF.FY2NOM
{
    public class FY2NOMBand : RasterBand
    {
        private string _fileName;
        private string _bandName;
        private IRasterDataProvider _provider;

        public FY2NOMBand(IRasterDataProvider rasterDataProvider, string datasetName)
            : base(rasterDataProvider)
        {
            _fileName = rasterDataProvider.fileName;
            _width = rasterDataProvider.Width;
            _height = rasterDataProvider.Height;
            _dataType = enumDataType.Double;
            _bandName = datasetName;
            _provider = rasterDataProvider;
        }

        protected override void DirectRead(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            switch (_bandName)
            {
                case "Longitude": DirectReadGeo(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize, 1); 
                    return;
                case "Latitude": DirectReadGeo(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize, 2);
                    return;
                //case "SolarZenith": DirectReadAngle(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize, 0); break;
                //case "SatelliteZenith": DirectReadAngle(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize, 1); break;
                //case "RelativeAzimuth": DirectReadAngle(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize, 2); break;
            }
        }

        private void DirectReadGeo(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize, int bandno)
        {
            string latlonFilename = TryFindGeoFile();
            if (!File.Exists(latlonFilename))
                return;
            using (IRasterDataProvider raster = RasterDataDriver.Open(latlonFilename) as IRasterDataProvider)
            {
                raster.GetRasterBand(bandno).Read(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
            }
        }


        private string TryFindGeoFile()
        {
            string sate = _provider.DataIdentify.Satellite;
            switch (sate)
            {
                case "FY2C":
                    return System.AppDomain.CurrentDomain.BaseDirectory + "SystemData\\FY2NOM\\" + "FY2C_latlon22882288.raw";
                case "FY2D":
                    return System.AppDomain.CurrentDomain.BaseDirectory + "SystemData\\FY2NOM\\" + "FY2D_latlon22882288.raw";
                case "FY2E":
                    return System.AppDomain.CurrentDomain.BaseDirectory + "SystemData\\FY2NOM\\" + "FY2E_latlon22882288.raw";
                default:
                    return System.AppDomain.CurrentDomain.BaseDirectory + "SystemData\\FY2NOM\\" + sate + "_latlon22882288.raw";
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
