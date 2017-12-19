using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using GeoDo.Project;

namespace GeoDo.RSS.DF.GDAL.HDF4Universal
{
    public class Hdf4RasterDataProvider : GDALRasterDataProvider
    {
        private object[] _args = null;
        private string[] _datasets = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">传入数据文件名</param>
        /// <param name="header1024"></param>
        /// <param name="driver"></param>
        /// <param name="args">扩展参数</param>
        /// 可扩展的参数信息，每对参数都采用key=value的形式
        /// 1、子数据集用它在数据文件里的序号代替
        /// 2、投影方式采用proj4字符串的形式
        /// 3、数据投影信息，如数据地理范围、空间分辨率等
        public Hdf4RasterDataProvider(string fileName, byte[] header1024, IGeoDataDriver driver, params object[] args)
            : base(fileName, header1024, driver, enumDataProviderAccess.ReadOnly)
        {
            _args = args;

            TryParseArgs();

            TryCreateBands(fileName, header1024, _datasets);

            _bandCount = _rasterBands.Count;

            TryGetDatTypeOfProvider();

            TryGetSizeOfProvider();

            TryCreateSpatialRef();
            TryCreateCoordTransform();
            TrySetEnvelopeAndResolutions();

        }

        private new void TryCreateCoordTransform()
        {
            if (_spatialRef == null)
                _coordTransform = CoordTransoformFactory.GetCoordTransform(null, null, _width, _height);
            else
            {
                _coordTransform = CoordTransoformFactory.GetCoordTransform(
                    new Point(0, 0),
                    new Point(_width, _height),
                    new double[] { _coordEnvelope.MinX, _coordEnvelope.MaxY },
                    new double[] { _coordEnvelope.MaxX, _coordEnvelope.MinY });
            }
        }
        private void TryCreateSpatialRef()
        {
            try
            {
                if (_spatialRef == null)
                    _coordType = enumCoordType.Raster;
                else
                    _coordType = _spatialRef.ProjectionCoordSystem != null ? enumCoordType.PrjCoord : enumCoordType.GeoCoord;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void TryParseArgs()
        {
            if (_args == null || _args.Length == 0)
                return;
            Dictionary<string, string> opDic = new Dictionary<string, string>();
            foreach (object option in _args)
            {
                string param = option.ToString();
                int k = param.IndexOf('=');
                string key = param.Substring(0, k).ToLower().Trim();
                string value = param.Substring(k + 1).Trim();
                opDic.Add(key, value);
            }
            foreach (string key in opDic.Keys)
            {
                string value = opDic[key];
                switch (key)
                {
                    case "datasets":
                        _datasets = GetDataSets(value);
                        break;
                    case "geoinfo":
                        TryParseGeoInfo(value);
                        break;
                    case "proj4":
                        TryParsePrjInfo(value);
                        break;
                    default:
                        break;
                }
            }
        }
        private void TryParseGeoInfo(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            string[] vals = value.Split(',');
            if (vals.Length != 6)
                return;
            double minX, maxX, miny, maxY;
            float resolutionX, resolutionY;
            if (double.TryParse(vals[0], out minX)
                && double.TryParse(vals[1], out maxX)
                && double.TryParse(vals[2], out miny)
                && double.TryParse(vals[3], out maxY)
                && float.TryParse(vals[4], out resolutionX)
                && float.TryParse(vals[5], out resolutionY)
                )
            {
                _resolutionX = resolutionX;
                _resolutionY = resolutionY;
                _coordEnvelope = new CoordEnvelope(minX, maxX, miny, maxY);
            }
        }

        private void TryParsePrjInfo(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            ISpatialReference spa = SpatialReference.FromProj4(value);

            if (spa != null)
                _spatialRef = spa;
        }

        private void TryCreateBands(string fileName, byte[] header1024, string[] datasets)
        {
            _bandProvider = new BandProviderHDF4Universal(datasets);
            if (!_bandProvider.IsSupport(fileName, header1024, null))
                return;
            _bandProvider.Init(fileName, enumDataProviderAccess.ReadOnly, this);
            IRasterBand[] bands = _bandProvider.GetDefaultBands();
            if (bands != null)
            {
                if (_rasterBands == null)
                    _rasterBands = new List<IRasterBand>();
                _rasterBands.AddRange(bands);
            }
        }

        private string[] GetDataSets(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            return value.Split(',');
        }

        private void TryGetDatTypeOfProvider()
        {
            if (_rasterBands != null && _rasterBands.Count > 0)
            {
                _dataType = _rasterBands[0].DataType;
                for (int i = 0; i < _rasterBands.Count - 1; i++)
                {
                    if (_rasterBands[i].DataType != _rasterBands[i + 1].DataType)
                    {
                        _dataType = enumDataType.Atypism;
                        break;
                    }
                }
            }
        }

        private void TryGetSizeOfProvider()
        {
            if (_rasterBands != null && _rasterBands.Count > 0)
            {
                for (int i = 0; i < _rasterBands.Count - 1; i++)
                {
                    if (_rasterBands[i].Width != _rasterBands[i + 1].Width || _rasterBands[i].Height != _rasterBands[i + 1].Height)
                    {
                        _width = _height = 0;
                        return;
                    }
                }
                _width = _rasterBands[0].Width;
                _height = _rasterBands[0].Height;
            }
        }
    }
}
