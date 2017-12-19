#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：admin     时间：2013-08-16 15:32:00
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using System.Drawing;
using OSGeo.GDAL;
using GeoDo.RSS.RasterTools;

namespace GeoDo.RSS.DF.GDAL.HDF5Universal
{
    /// <summary>
    /// 类名：HdfRasterDataProvider
    /// 属性描述：
    /// 创建者：luozhanke   创建日期：2013-08-16 15:32:00
    /// 修改者：罗战克    修改日期：2013-12-25
    /// 修改描述：修改HDF数据集只有一个数据集，一个通道时候读取不了的问题
    /// 备注：
    /// </summary>
    public class HdfRasterDataProvider : RasterDataProvider//GDALRasterDataProvider
    {
        private object[] _args = null;
        private string[] _datasets = null;
        private string _lonDatasetName = null;
        private string _latDatasetName = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="header1024"></param>
        /// <param name="driver"></param>
        /// <param name="args">
        /// 可扩展的参数信息，每对参数都采用key=value的形式。
        /// 1、数据集，采用逗号分隔的形式加入默认所选择的波段。
        /// 例如：datasets=VIRR_1Km_LST[,...]
        /// 2、投影方式,采用proj4字符串的形式
        /// 例如：proj4=
        /// 3、其他待扩展的参数信息
        /// 坐标信息：geoinfo=1,2,3,4,5,6//其中1，2，3,4，5,6分辨代表：minX,maxX,minY,maxY,resolutionX,resolutionY;
        /// 经纬度数据集：geodatasets=Longitude,Latitude//该数据对轨道数据有效。
        /// </param>
        public HdfRasterDataProvider(string fileName, byte[] header1024, IGeoDataDriver driver, params object[] args)
            : base(fileName, driver)
        //: base(fileName, driver)
        {
            _args = args;
            TryParseArgs();
            using (Dataset _dataset = Gdal.Open(fileName, Access.GA_ReadOnly))
            {
                GDALHelper.GetDatasetAttributes(_dataset, _attributes);
                //数据集仅有一个数据集，一个通道
                if (_dataset.RasterCount == 1)
                {
                    //GDAL对于只有一个数据集的HDF数据，SUBDATASETS属性值是空的，这里尝试手动设置下
                    TrySetGDALSUBDATASETS(_datasets);
                }
            }

            //...可以在此添加投影等信息
            TryCreateBandProvider(fileName, header1024, _datasets, _attributes.GetAttributeDomain("SUBDATASETS"));

            //_bandCount = _dataset.RasterCount;

            _bandCount = _rasterBands.Count;

            TryGetDatTypeOfProvider();

            TryGetSizeOfProvider();

            TryCreateSpatialRef();
            TryCreateCoordTransform();
            TrySetEnvelopeAndResolutions();
        }

        private void TrySetGDALSUBDATASETS(string[] datasets)
        {
            _attributes.GetAttributeDomain("SUBDATASETS").Add("SUBDATASET_1_NAME",string.Format("HDF5:\"{0}\"://{1}", _fileName, datasets[0]));
        }

        private void TryCreateCoordTransform()
        {
            if (_spatialRef == null)
                _coordTransform = CoordTransoformFactory.GetCoordTransform(null, null, _width, _height);
            else
            {
                _coordTransform = CoordTransoformFactory.GetCoordTransform(
                    new Point(0, 0),
                    new Point(_width, _height),
                    new double[] {_coordEnvelope.MinX,_coordEnvelope.MaxY},
                    new double[] {_coordEnvelope.MaxX,_coordEnvelope.MinY});
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

        #region ParseArgs
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
                    case "geodatasets":
                        TryParseGeoDatasets(value);
                        break;
                    default:
                        break;
                }
            }
        }

        private void TryParseGeoDatasets(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            string[] lonlat = value.Split(',');
            if (lonlat.Length != 2)
                return;
            _lonDatasetName = lonlat[0];
            _latDatasetName = lonlat[1];
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
        #endregion

        private void TryCreateBandProvider(string fileName, byte[] header1024, string[] datasets, Dictionary<string, string> datasetNames)
        {
            _bandProvider = new BandProviderHDF5Universal(datasets, _lonDatasetName, _latDatasetName);
            if (!_bandProvider.IsSupport(fileName, header1024, datasetNames))
                return;
            _bandProvider.Init(fileName, enumDataProviderAccess.ReadOnly, this);
            IRasterBand[] bands = _bandProvider.GetDefaultBands();
            if (bands != null)
            {
                if (_rasterBands == null)
                    _rasterBands = new List<IRasterBand>();
                else
                    _rasterBands.Clear();
                _rasterBands.AddRange(bands);
            }
        }

        private string[] GetDataSets(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            return value.Split(',');
        }

        //private ISpatialReference GetSpatialRef(string value)
        //{
        //    throw new NotImplementedException();
        //}

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

        public override void AddBand(enumDataType dataType)
        {
            throw new NotImplementedException();
        }

        public override object GetStretcher(int bandNo)
        {
            IRasterBand band = GetRasterBand(bandNo);
            if (band == null)
                return null;
            return PercentXStretcher(bandNo, 0.01f);
            //return TryGetDefaultStretcher(band);
        }

        private object TryGetDefaultStretcher(IRasterBand band)
        {
            double minValue, maxValue;
            if (_dataType == enumDataType.Byte)
            {
                minValue = 0;
                maxValue = 255;
            }
            else
            {
                band.ComputeMinMax(out minValue, out maxValue, true, null);
            }

            return RgbStretcherFactory.CreateStretcher(_dataType, minValue, maxValue);
        }

        public object PercentXStretcher(int bandNo, float percent)
        {
            Dictionary<int, RasterQuickStatResult> results;
            IRasterQuickStatTool stat = new RasterQuickStatTool();
            results = stat.Compute(this, null, new int[] { bandNo }, null);
            RasterQuickStatResult rst = results[bandNo];
            //
            object[] stretchers = GetStretcher(this.DataType, new RasterQuickStatResult[] { rst }, percent);
            return stretchers[0];
        }

        private object[] GetStretcher(enumDataType dataType, RasterQuickStatResult[] resutls, float percent)
        {
            int bandCount = resutls.Length;
            //x%像元临界个数
            int criticalCount = (int)(percent * resutls[0].HistogramResult.PixelCount);
            object[] sts = new object[bandCount];
            //low
            for (int i = 0; i < bandCount; i++)
            {
                //边界DN值
                double lowValue = 0, heightValue = 0;
                //累计个数
                double accCount = 0;
                int bucks = resutls[i].HistogramResult.ActualBuckets;
                //lowValue
                for (int k = 0; k < bucks; k++)
                {
                    if (accCount > criticalCount)
                        break;
                    accCount += resutls[i].HistogramResult.Items[k];
                    lowValue = resutls[i].HistogramResult.MinDN + k * resutls[i].HistogramResult.Bin;
                }
                //HighValue
                accCount = 0;
                for (int k = bucks - 1; k >= 0; k--)
                {
                    if (accCount > criticalCount)
                        break;
                    accCount += resutls[i].HistogramResult.Items[k];
                    heightValue = resutls[i].HistogramResult.MinDN + k * resutls[i].HistogramResult.Bin;
                }
                //
                sts[i] = RgbStretcherFactory.CreateStretcher(dataType, lowValue, heightValue);
            }
            return sts;
        }
    }
}
