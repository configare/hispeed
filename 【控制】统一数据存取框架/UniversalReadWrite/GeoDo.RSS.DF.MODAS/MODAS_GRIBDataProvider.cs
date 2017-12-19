#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/25 8:55:15
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
using System.IO;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.GRIB;
using GeoDo.Project;

namespace GeoDo.RSS.DF.MODAS
{
    public class MODAS_GRIBDataProvider : RasterDataProvider,
        IGRIBDataProvider, IMODAS_GRIBDataProvider
    {
        protected GRIB_Definition _definition;
        protected FileStream _fs;
        protected StreamReader _stReader;
        protected int _crtDeep;
        protected const int LAYER_COUNT = 51;
        protected string _currentDeepValue = "0";
        protected string _currentFeatureName = "温度";
        protected string[] _deepValues;
        protected string[] _featureNames;
        protected string[] _requestedFeatureNames;
        protected bool _isReadToRasterDataProvider = false;

        public MODAS_GRIBDataProvider(string fileName, IGeoDataDriver driver, string deepValue, string featureName)
            : base(fileName, driver)
        {
            SetDefinition();
            //如果指定了深度，则按照栅格数据集读取
            if (!string.IsNullOrWhiteSpace(deepValue))
            {
                TryReadToRasterDataProvider(deepValue, featureName, fileName);
            }
        }

        private void TryReadToRasterDataProvider(string deepValue, string featureName, string fileName)
        {
            _currentDeepValue = deepValue;
            if (featureName != null && featureName != "所有要素")
            {
                _currentFeatureName = featureName;
                _requestedFeatureNames = new string[] { featureName };
            }
            //
            _resolutionX = _definition.LonResolution;
            _resolutionY = _definition.LatResolution;
            _coordEnvelope = _definition.GetCoordEnvelope();
            _spatialRef = SpatialReference.GetDefault();
            string[] fetNames = null;
            if (_requestedFeatureNames == null)
            {
                fetNames = _featureNames;
                _bandCount = _featureNames.Length;
            }
            else
            {
                fetNames = _requestedFeatureNames;
                _bandCount = _requestedFeatureNames.Length;
            }
            _dataType = enumDataType.Float;
            _coordType = enumCoordType.GeoCoord;
            _dataTypeSize = DataTypeHelper.SizeOf(_dataType);
            _filelist = new string[] { fileName };
            _fileName = fileName;
            _width = _definition.Width;
            _height = _definition.Height;
            _rasterBands = new List<IRasterBand>(_bandCount);
            ArrayRasterDataProviderBuilder builder = new ArrayRasterDataProviderBuilder();
            IArrayRasterDataProvider prd = null;
            for (int b = 0; b < _bandCount; b++)
            {
                this.CurrentFeatureName = fetNames[b];
                prd = builder.Build(_definition, this.Read());
                _rasterBands.Add(prd.GetRasterBand(1));
            }
            //
            _isReadToRasterDataProvider = true;
        }

        public bool IsReadToRasterDataProvider
        {
            get { return _isReadToRasterDataProvider; }
        }

        private void SetDefinition()
        {
            //[-10 52] = 62 / 0.125 = 496 height
            //[99 150] = 51 / 0.125 = 408 width
            _definition = new GRIB_Definition(99, -10, 0.125f, 0.125f, 409, 497, _currentFeatureName, 9999);
            _deepValues = new string[] 
            {
                "0","5","10","15","20",
                "25","30","35","50","75",
                "100","125","150","175","200",
                "250","300","350","400","450",
                "500","600","700","800","900",
                "1000","1100","1200","1300","1400",
                "1500","1600","1700","1750","1800",
                "1900","2000","2500","3000","3500",
                "4000","4500","5000","5500","6000",
                "6500","7000","7500","8000","8500",
                "9000"
            };
            _featureNames = new string[] 
            {
                "温度","盐度","密度","声速","海流(u分量)","海流(v分量)","海流海面高度"
            };

            _fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read);
            _stReader = new StreamReader(_fs, Encoding.ASCII);
        }

        public string[] DeepValues
        {
            get { return _deepValues; }
        }

        public string[] FeatureNames
        {
            get { return _featureNames; }
        }

        public string CurrentDeepValue
        {
            get { return _currentDeepValue; }
            set { _currentDeepValue = value; }
        }

        public string CurrentFeatureName
        {
            get { return _currentFeatureName; }
            set
            {
                _currentFeatureName = value;
                _definition.ValueName = value;
            }
        }


        public GRIB_Definition Definition
        {
            get { return _definition; }
        }

        public unsafe GRIB_Point[] Read()
        {
            try
            {
                string[] parts;
                char[] SPLIT_CHARS = new char[] { ' ' };
                GRIB_Point[] pts = new GRIB_Point[_definition.Width * _definition.Height];
                int deepIdx = Array.IndexOf(_deepValues, _currentDeepValue) + 1;
                int featureIdx = Array.IndexOf(_featureNames, _currentFeatureName) + 1; //skip deep column
                //只有水深为“0”的层才有"海流海面高度"
                if (_currentDeepValue != "0" && _currentFeatureName == "海流海面高度")
                    return pts;
                int step = LAYER_COUNT + 1;
                int firstPixel = deepIdx;
                string line = null;
                int rowIdx = 0;
                float v = 0;
                int pixelIdx = 0;
                fixed (GRIB_Point* ptr0 = pts)
                {
                    GRIB_Point* ptr = ptr0;
                    while (!_stReader.EndOfStream)
                    {
                        while (deepIdx > 0)
                        {
                            _stReader.ReadLine();
                            deepIdx--;
                        }
                        line = _stReader.ReadLine();
                        if (rowIdx % step == 0)
                        {
                            parts = line.Split(SPLIT_CHARS, StringSplitOptions.RemoveEmptyEntries);
                            float.TryParse(parts[featureIdx], out v);
                            ptr->Value = v;
                            ptr->Index = pixelIdx;
                            ptr++;
                            pixelIdx++;
                        }
                        rowIdx++;
                    }
                }
                return pts;
            }
            finally
            {
                _fs.Seek(0, SeekOrigin.Begin);
            }
        }

        public GRIB_Point[] Read(CoordEnvelope geoEnvelope)
        {
            throw new NotImplementedException();
        }

        public void Read(IntPtr buffer)
        {
            throw new NotImplementedException();
        }

        public void Read(IntPtr buffer, CoordEnvelope geoEnvelope)
        {
            throw new NotImplementedException();
        }

        public unsafe void StatMinMax(GRIB_Point[] pts, out GRIB_Point minPoint, out GRIB_Point maxPoint)
        {
            minPoint = new GRIB_Point();
            maxPoint = new GRIB_Point();
            if (pts == null || pts.Length == 0)
                return;
            minPoint.Value = float.MaxValue;
            maxPoint.Value = float.MinValue;
            float invalidValue = _definition.InvalidValue;
            fixed (GRIB_Point* ptr0 = pts)
            {
                GRIB_Point* ptr = ptr0;
                int n = pts.Length;
                if (float.IsNaN(invalidValue))
                {
                    for (int i = 0; i < n; i++, ptr++)
                    {
                        if (ptr->Value < minPoint.Value)
                        {
                            minPoint.Value = ptr->Value;
                            minPoint.Index = ptr->Index;
                        }
                        if (ptr->Value > maxPoint.Value)
                        {
                            maxPoint.Value = ptr->Value;
                            maxPoint.Index = ptr->Index;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < n; i++, ptr++)
                    {
                        if (Math.Abs(ptr->Value - invalidValue) < float.Epsilon)
                            continue;
                        if (ptr->Value < minPoint.Value)
                        {
                            minPoint.Value = ptr->Value;
                            minPoint.Index = ptr->Index;
                        }
                        if (ptr->Value > maxPoint.Value)
                        {
                            maxPoint.Value = ptr->Value;
                            maxPoint.Index = ptr->Index;
                        }
                    }
                }
            }
        }

        public IArrayRasterDataProvider ToArrayDataProvider(GRIB_Point[] points)
        {
            return new ArrayRasterDataProviderBuilder().Build(_definition, points);
        }

        public override void AddBand(enumDataType dataType)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            _stReader.Dispose();
            _fs.Dispose();
            base.Dispose();
        }
    }
}
