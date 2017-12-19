using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using OSGeo.GDAL;
using System.Windows.Forms;
using GeoDo.Project;
using System.IO;

namespace GeoDo.RSS.DF.GDAL
{
    /// <summary>
    /// 修改日志
    /// 修改日期：2012.2.12
    /// 修改人：罗战克
    /// 修改内容：通过_projectionRef初始化_spatialRef时候，采用enumWKTSource.GDAL出现错误，修改为采用enumWKTSource.EsriPrjFile
    /// </summary>
    public class GDALRasterDataProvider : RasterDataProvider, IGDALRasterDataProvider
    {
        protected Dataset _dataset = null;
        protected string _projectionRef = null;
        protected const string GDAL_SUBDATASETS_NAME = "SUBDATASETS";

        public GDALRasterDataProvider(string fileName, byte[] header1024, IGeoDataDriver driver, enumDataProviderAccess access)
            : base(fileName, driver)
        {
            _driver = driver;
            CallGDALBefore();
            Access gdalAccess = access == enumDataProviderAccess.ReadOnly ? Access.GA_ReadOnly : Access.GA_Update;
            _dataset = Gdal.Open(fileName, gdalAccess);
            GDALHelper.GetDatasetAttributes(_dataset, _attributes);
            if (header1024 == null)
                header1024 = GetHeader1024Bytes(fileName);
            TryGetBandProviderAndGetDefaultBands(gdalAccess, header1024, _attributes.GetAttributeDomain(GDAL_SUBDATASETS_NAME));
            _width = 0;  //GDAL: _dataset.RasterXSize 默认为512
            _height = 0; //GDAL: _dataset.RasterYSize 默认为512
            _filelist = _dataset.GetFileList();
            _fileName = _fileName;
            //读取波段列表
            _bandCount = _dataset.RasterCount;
            for (int i = 1; i <= _bandCount; i++)
                _rasterBands.Add(new GDALRasterBand(this, _dataset.GetRasterBand(i), new GDALDataset(_dataset)));
            _bandCount = _rasterBands.Count;
            for (int i = 1; i <= _bandCount; i++)
                _rasterBands[i - 1].BandNo = i;
            TryGetDatTypeOfProvider();
            TryGetSizeOfProvider();
            TryCreateSpatialRef();
            TryCreateCoordTransform();
            TrySetEnvelopeAndResolutions();
            //
            //TryGetDataIdentity();
        }

        private static byte[] GetHeader1024Bytes(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                byte[] buffer = new byte[1024];
                fs.Read(buffer, 0, 1024);
                return buffer;
            }
        }

        protected virtual void CallGDALBefore()
        {
        }

        private void TryCreateSpatialRef()
        {
            try
            {
                _projectionRef = Dataset.GetProjectionRef();
                if (!string.IsNullOrWhiteSpace(_projectionRef))
                    _spatialRef = SpatialReferenceFactory.GetSpatialReferenceByWKT(_projectionRef, enumWKTSource.EsriPrjFile);
                if (string.IsNullOrWhiteSpace(_projectionRef))
                    TryReadSpatialRefFromSecondaryFile();
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

        private void TryReadSpatialRefFromSecondaryFile()
        {
            double[] GTs = null;
            _spatialRef = SpatialRefFinderBySecondaryFile.TryGetSpatialRef(_fileName,out GTs);
            if (_spatialRef != null)
                _projectionRef = _spatialRef.ToWKTString();
            if (GTs != null && GTs.Length == 6)
            {
                _coordTransform = CoordTransoformFactory.GetCoordTransform(_spatialRef, GTs, _width, _height);
            }
        }

        protected virtual void TryCreateCoordTransform()
        {
            //_coordTransform已由函数TryReadSpatialRefFromSecondaryFile创建
            if (_coordTransform != null)
                return;
            double[] GTs = new double[6];
            _dataset.GetGeoTransform(GTs);
            _coordTransform = CoordTransoformFactory.GetCoordTransform(_spatialRef, GTs, _width, _height);
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

        protected virtual void TryGetBandProviderAndGetDefaultBands(Access access, byte[] header1024, Dictionary<string, string> datasetNames)
        {
            _bandProvider = BandProviderFactory.GetBandProvider(_fileName, header1024, access, this, datasetNames);
            if (_bandProvider != null)
            {
                if (_bandProvider.DataIdentify != null)
                {
                    _bandProvider.DataIdentify.CopyAttributesToIfNull(_dataIdentify);
                    _dataIdentify.IsOrbit = _bandProvider.DataIdentify.IsOrbit;
                    _dataIdentify.IsAscOrbitDirection = _bandProvider.DataIdentify.IsAscOrbitDirection;
                    _dataIdentify.CopyAttributesToIfNull(_bandProvider.DataIdentify);//互相复制未获取的属性
                }
                IRasterBand[] bands = _bandProvider.GetDefaultBands();
                if (bands != null && bands.Length > 0)
                {
                    if (_rasterBands == null)
                        _rasterBands = new List<IRasterBand>(bands);
                    else
                        _rasterBands.AddRange(bands);
                }
            }
        }

        internal Dataset Dataset
        {
            get { return _dataset; }
        }

        public override void AddBand(enumDataType dataType)
        {
        }

        public override bool IsSupprtOverviews
        {
            get
            {
                return true;
            }
        }

        Action<int, string> _progressTracker = null;
        public override void BuildOverviews(int[] levels, Action<int, string> progressTracker)
        {
            if (_dataset == null)
                return;
            if (levels == null)
                levels = GetLevels();
            if (levels.Length == 0)
                return;
            _progressTracker = progressTracker;
            Gdal.SetConfigOption("USE_RRD", "YES");
            _dataset.BuildOverviews("NEAREST", levels, new Gdal.GDALProgressFuncDelegate(ProgressFunc), string.Empty);
        }

        public override void BuildOverviews(Action<int, string> progressTracker)
        {
            int[] levels = GetLevels();
            BuildOverviews(levels, progressTracker);
        }

        private int[] GetLevels()
        {
            int width = _width;
            int height = _height;
            int pixelNum = width * height;    //图像中的总像元个数  
            int topNum = 512 * 512;             //顶层金字塔大小:512 * 512
            int curNum = pixelNum / 4;
            int[] anLevels = new int[1024];
            int nLevelCount = 0;                 //金字塔级数
            do
            {
                anLevels[nLevelCount] = Convert.ToInt32(Math.Pow(2.0, nLevelCount + 2));
                nLevelCount++;
                curNum /= 4;
            }
            while (curNum > topNum);
            int[] levels = new int[nLevelCount];
            for (int lvIdx = 0; lvIdx < nLevelCount; lvIdx++)
            {
                levels[lvIdx] = anLevels[lvIdx];
            }
            return levels;
        }

        public int ProgressFunc(double completePercent, IntPtr msg, IntPtr data)
        {
            if (_progressTracker != null)
                _progressTracker((int)(completePercent * 100), System.Runtime.InteropServices.Marshal.PtrToStringAnsi(msg));
            return 1;
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_dataset != null)
            {
                _dataset.Dispose();
                _dataset = null;
            }
        }
    }
}
