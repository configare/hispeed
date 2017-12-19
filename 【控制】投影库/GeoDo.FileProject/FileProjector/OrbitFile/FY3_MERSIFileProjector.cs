using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.DF;
using GeoDo.RasterProject;
using GeoDo.Project;
using System.Drawing;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;

namespace GeoDo.FileProject
{
    [Export(typeof(IFileProjector)), ExportMetadata("VERSION", "1")]
    internal class FY3_MERSIFileProjector : FileProjector
    {
        protected const double DEG_TO_RAD_P100 = 0.000174532925199432955; // (PI/180)/100;
        //反射通道(可见光/近红外)定标，文件属性
        protected const string VIR_Cal_Coeff = "VIR_Cal_Coeff";
        protected const string SolarZenith = "SolarZenith";       //太阳天顶角数据集
        //发射通道(热红外通道)定标
        protected const double C1 = 1.1910659 / 100000;
        protected const double C2 = 1.438833;
        protected const double V = 875.1379;

        private ISpatialReference _srcSpatialRef = null;

        #region Session
        private float[] _vir_Cal_Coeff = null;      //反射通道，19个通道的三个系数，排列为第一个通道的k0，k1，k2；第二个通道的k0，k1，k2；......
        #endregion

        private IRasterDataDriver _outLdfDriver = null; //输出ldf文件驱动
        private string _szDataFilename;                             //太阳高度角文件
        private IRasterDataProvider _longitudeRaster = null;
        private IRasterDataProvider _latitudeRaster = null;
        private IRasterBand _longitudeBand = null;     //用于投影的经纬度通道
        private IRasterBand _latitudeBand = null;
        private int _readyProgress = 0;
        private string _dataType = "1KM";              //1KM、QKM
        private FY3_MERSI_PrjSettings _prjSettings;
        private IRasterDataProvider _angleDataProvider = null;
        private PrjBand[] _prjBands = null;

        public FY3_MERSIFileProjector()
            : base()
        {
            _name = "FY3_MERSI";
            _fullname = "FY3_MERSI轨道数据投影";
            _rasterProjector = new RasterProjector();
            _srcSpatialRef = new SpatialReference(new GeographicCoordSystem());
            _outLdfDriver = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            _left = 10;
            _right = 10;
        }

        public override bool IsSupport(string fileName)
        {
            return false;
        }

        public override void Project(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback)
        {
            try
            {
                ReadyArgs(srcRaster, prjSettings, dstSpatialRef, progressCallback);
                IRasterDataProvider outwriter = null;
                try
                {
                    //string[] options = new string[]{
                    //        "INTERLEAVE=BSQ",
                    //        "VERSION=LDF",
                    //        "WITHHDR=TRUE",
                    //        "SPATIALREF=" + _dstSpatialRef.ToProj4String(),
                    //        "MAPINFO={" + 1 + "," + 1 + "}:{" + _prjSettings.OutEnvelope.MinX + "," + _prjSettings.OutEnvelope.MaxY + "}:{" + _outResolutionX + "," + _outResolutionY + "}"
                    //        ,"BANDNAMES="+ BandNameString(_prjSettings.OutBandNos)    
                    //};
                    List<string> options = new List<string>();
                    options.Add("INTERLEAVE=BSQ");
                    options.Add("VERSION=LDF");
                    options.Add("WITHHDR=TRUE");
                    options.Add("SPATIALREF=" + _dstSpatialRef.ToProj4String());
                    options.Add("MAPINFO={" + 1 + "," + 1 + "}:{" + _prjSettings.OutEnvelope.MinX + "," + _prjSettings.OutEnvelope.MaxY + "}:{" + _outResolutionX + "," + _outResolutionY + "}");
                    options.Add("SENSOR=MERSI");
                    if (srcRaster.DataIdentify != null)
                    {
                        string satellite = srcRaster.DataIdentify.Satellite;
                        DateTime dt = srcRaster.DataIdentify.OrbitDateTime;
                        bool asc = srcRaster.DataIdentify.IsAscOrbitDirection;
                        if (!string.IsNullOrWhiteSpace(satellite))
                        {
                            options.Add("SATELLITE=" + satellite);
                        }
                        if (dt != DateTime.MinValue && dt != DateTime.MaxValue)
                            options.Add("DATETIME=" + dt.ToString("yyyy/MM/dd HH:mm"));
                        options.Add("ORBITDIRECTION=" + (asc ? "ASC" : "DESC"));
                    }
                    List<string> op1 = new List<string>(options);
                    op1.Add("BANDNAMES=" + BandNameString(_prjSettings.OutBandNos));
                    outwriter = CreateOutFile(_outfilename, _dstBandCount, _dstSize, op1.ToArray());
                    ReadyAngleFiles(_angleDataProvider, _outfilename, _prjSettings, _dstSize, options.ToArray());
                    ProjectToLDF(srcRaster, outwriter, 0, progressCallback);
                }
                catch (IOException ex)
                {
                    if (ex.Message == "磁盘空间不足。\r\n" && File.Exists(_outfilename))
                        File.Delete(_outfilename);
                    throw ex;
                }
                finally
                {
                    if (outwriter != null)
                    {
                        outwriter.Dispose();
                        outwriter = null;
                    }
                }
            }
            catch
            {
                EndSession();
                TryDeleteCurCatch();
                throw;
            }
            finally
            {
                if (_curSession == null)
                {
                    EndSession();
                    if (prjSettings.IsClearPrjCache)
                        TryDeleteCurCatch();
                }
            }
        }

        public override IRasterDataProvider Project(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, IRasterDataProvider dstRaster, int beginBandIndex, Action<int, string> progressCallback)
        {
             if (dstRaster == null)
                return null;
            try
            {
                ReadyArgs(srcRaster, prjSettings, dstRaster.SpatialRef, progressCallback);
                _prjSettings.OutPathAndFileName = dstRaster.fileName;
                _outfilename = dstRaster.fileName;
                _outResolutionX = dstRaster.ResolutionX;
                _outResolutionY = dstRaster.ResolutionY;
                _dstEnvelope = _prjSettings.OutEnvelope = new PrjEnvelope(
                    dstRaster.CoordEnvelope.MinX, dstRaster.CoordEnvelope.MaxX,
                    dstRaster.CoordEnvelope.MinY, dstRaster.CoordEnvelope.MaxY);
                try
                {
                    Size outSize = new Size(dstRaster.Width, dstRaster.Height);
                    //角度输出，其中的BANDNAME需要在ReadyAngleFiles()方法中获取
                    string[] angleOptions = new string[] { 
                            "INTERLEAVE=BSQ",
                            "VERSION=LDF",
                            "WITHHDR=TRUE",
                            "SPATIALREF=" + _dstSpatialRef.ToProj4String(),
                            "MAPINFO={" + 1 + "," + 1 + "}:{" + _prjSettings.OutEnvelope.MinX + "," + _prjSettings.OutEnvelope.MaxY + "}:{" + _outResolutionX + "," + _outResolutionY + "}"
                    };
                    ReadyAngleFiles(_angleDataProvider, _outfilename, _prjSettings, outSize, angleOptions);
                    ProjectToLDF(srcRaster, dstRaster, beginBandIndex, progressCallback);
                    return dstRaster;
                }
                catch (IOException ex)
                {
                    if (ex.Message == "磁盘空间不足。\r\n" && File.Exists(_outfilename))
                        File.Delete(_outfilename);
                    throw ex;
                }
                finally
                {
                    if (dstRaster != null)
                    {
                        dstRaster.Dispose();
                        dstRaster = null;
                    }
                }
            }
            catch
            {
                EndSession();
                TryDeleteCurCatch();
                throw;
            }
            finally
            {
                if (_curSession == null)
                {
                    EndSession();
                    if (prjSettings.IsClearPrjCache)
                        TryDeleteCurCatch();
                }
            }
        }

        private void ReadyArgs(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback)
        {
            if (srcRaster.DataIdentify != null)
            {
                string satellite = srcRaster.DataIdentify.Satellite;
                //if (satellite == "FY3A")//FY3A MERSI250米数据已经经过处理，不需要再执行去条带操作
                //{
                //    _sacnLineWidth = 0;
                //}
            }
            float resolutionScale = 1f;
            _readyProgress = 0;
            if (progressCallback != null)
                progressCallback(_readyProgress++, "准备相关参数");
            _prjSettings = ArgsCheck(srcRaster, prjSettings);
            CheckIs0250(srcRaster);
            _dstSpatialRef = dstSpatialRef;
            switch (_dataType)
            {
                case "1KM":
                    //整轨投影时候去除左右锯齿，分块投影不需要
                    _left = 10;
                    _right = 10;
                    if (_prjSettings.OutEnvelope == null || _prjSettings.OutEnvelope == PrjEnvelope.Empty)//整轨投影
                    {
                        MemoryHelper.MemoryNeed(500, 1536);
                    }
                    else
                    {
                        MemoryHelper.MemoryNeed(400, 1536);//剩余900MB,已使用1.2GB
                    }
                    TryCreateDefaultArgs(srcRaster, _prjSettings, ref _dstSpatialRef);
                    DoSession(srcRaster, srcRaster, _dstSpatialRef, _prjSettings, progressCallback);
                    _angleDataProvider = srcRaster;       
                    break;
                case "QKM":
                    resolutionScale = 4f;
                    _left = 20;
                    _right = 20;
                    if (_prjSettings.OutEnvelope == null || _prjSettings.OutEnvelope == PrjEnvelope.Empty)
                    {
                        MemoryHelper.MemoryNeed(800, 1280);     //整幅投影对内存做限制，系统剩余内存不低于1.5GB，应用程序已使用内存不超过600MB
                    }
                    else
                    {
                        MemoryHelper.MemoryNeed(600, 1280);     //剩余900MB,已使用1.2GB
                    }
                    TryCreate0250DefaultArgs(srcRaster, _prjSettings, ref _dstSpatialRef);
                    DoSession(srcRaster, _prjSettings.SecondaryOrbitRaster, _dstSpatialRef, _prjSettings, progressCallback);
                    _angleDataProvider = _prjSettings.SecondaryOrbitRaster;
                    break;
                default:
                    break;
            }
            TrySetLeftRightInvalidPixel(_prjSettings.ExtArgs);
            if (_prjSettings.OutEnvelope == null || _prjSettings.OutEnvelope == PrjEnvelope.Empty)
            {
                _prjSettings.OutEnvelope = _maxPrjEnvelope;
                _orbitBlock = new Block { xOffset = 0, yBegin = 0, xEnd = _srcLocationSize.Width - 1, yEnd = _srcLocationSize.Height - 1 };
            }
            else
            {
                GetEnvelope(_xs, _ys, _srcLocationSize.Width, _srcLocationSize.Height, _prjSettings.OutEnvelope, out _orbitBlock);
                if (_orbitBlock == null || _orbitBlock.Width <= 0 || _orbitBlock.Height <= 0)
                    throw new Exception("数据不在目标区间内");
                float invalidPresent = (_orbitBlock.Width * _orbitBlock.Height * resolutionScale) / (_srcLocationSize.Width * _srcLocationSize.Height);
                if (invalidPresent < 0.0001f)
                    throw new Exception("数据占轨道数据比例太小,有效率" + invalidPresent * 100 + "%");
                if (invalidPresent > 0.60)
                    _orbitBlock = new Block { xOffset = 0, yBegin = 0, xEnd = _srcLocationSize.Width - 1, yEnd = _srcLocationSize.Height - 1 };
            }
            //地理坐标投影,下面简单的对地理坐标投影的范围作了限制，不大严谨，仅适合目前的状况。
            if (_dstSpatialRef.ProjectionCoordSystem == null && (prjSettings.OutEnvelope.MaxY > 80 || prjSettings.OutEnvelope.MaxY < -80))
                throw new Exception(string.Format("高纬度数据，不适合投影为等经纬度数据[{0}]", _maxPrjEnvelope));
            //以下参数用于投影
            _dstEnvelope = _prjSettings.OutEnvelope;
            if (!_dstEnvelope.IntersectsWith(_maxPrjEnvelope))
                throw new Exception("数据不在目标区间内");
            _outResolutionX = _prjSettings.OutResolutionX;
            _outResolutionY = _prjSettings.OutResolutionY;
            _outFormat = _prjSettings.OutFormat;
            _outfilename = _prjSettings.OutPathAndFileName;
            _dstProj4 = _dstSpatialRef.ToProj4String();
            _dstBandCount = _prjBands.Length;
            _dstSize = _prjSettings.OutSize;
            _isRadiation = _prjSettings.IsRadiation;
            _isSolarZenith = _prjSettings.IsSolarZenith;
            _isSensorZenith = _prjSettings.IsSensorZenith;
        }

        private static FY3_MERSI_PrjSettings ArgsCheck(IRasterDataProvider srcRaster, FilePrjSettings prjSettings)
        {
            if (srcRaster == null)
                throw new ArgumentNullException("srcRaster");
            if (prjSettings == null)
                throw new ArgumentNullException("prjSettings");
            FY3_MERSI_PrjSettings fy3prjSettings = prjSettings as FY3_MERSI_PrjSettings;
            return fy3prjSettings;
        }
 
        private void CheckIs0250(IRasterDataProvider srcRaster)
        {
            try
            {
                IBandProvider band = srcRaster.BandProvider;
                Dictionary<string, string> filaAttrs = band.GetAttributes();
                if (filaAttrs == null || !filaAttrs.ContainsKey("File Alias Name"))
                    throw new Exception("不能确认为合法的MERSI轨道数据，尝试获取文件属性File Alias Name的值为空");
                string fileAliasName = filaAttrs["File Alias Name"];
                if (string.IsNullOrWhiteSpace(fileAliasName))
                    throw new Exception("不能确认为合法的MERSI轨道数据，尝试获取文件属性File Alias Name的值为空");
                else if (fileAliasName == "MERSI_1KM_L1")
                    _dataType = "1KM";
                else if (fileAliasName == "MERSI_QKM_L1" || fileAliasName == "MERSI_250M_L1")
                    _dataType = "QKM";
                else
                    throw new Exception("不能确认为合法的MERSI轨道数据，文件属性File Alias Name的值为[" + fileAliasName + "]支持的是MERSI_1KM_L1或者MERSI_QKM_L1");
            }
            catch (Exception ex)
            {
                throw new Exception("不能确认为合法的MERSI轨道数据" + ex.Message, ex.InnerException);
            }
        }

        private void TryCreate0250DefaultArgs(IRasterDataProvider srcRaster, FY3_MERSI_PrjSettings prjSettings, ref ISpatialReference dstSpatialRef)
        {
            if (prjSettings.SecondaryOrbitRaster == null && prjSettings.IsSolarZenith)
                prjSettings.IsSolarZenith = false;  //throw new Exception("无法获取相应1KM轨道数据文件，无法做太阳天顶角订正");
            if (dstSpatialRef == null)
                dstSpatialRef = _srcSpatialRef;
            if (string.IsNullOrWhiteSpace(prjSettings.OutFormat))
                prjSettings.OutFormat = "LDF";
            if (dstSpatialRef.ProjectionCoordSystem == null)
                _srcImgResolution = 0.0025f;
            else
                _srcImgResolution = 250f;
            if (prjSettings.OutResolutionX == 0 || prjSettings.OutResolutionY == 0)
            {
                if (dstSpatialRef.ProjectionCoordSystem == null)
                {
                    prjSettings.OutResolutionX = 0.0025F;//地理坐标系
                    prjSettings.OutResolutionY = 0.0025F;
                }
                else
                {
                    prjSettings.OutResolutionX = 250F;//投影坐标系
                    prjSettings.OutResolutionY = 250F;
                }
            }
            if (prjSettings.OutBandNos == null || prjSettings.OutBandNos.Length == 0)
            {
                _prjBands = OrbitBandDefCollection.MERSI_0250_OrbitDefCollecges();
            }
            else
            {
                List<PrjBand> bands = new List<PrjBand>();
                PrjBand[] defbands = OrbitBandDefCollection.MERSI_0250_OrbitDefCollecges();
                foreach (int bandNo in prjSettings.OutBandNos)
                {
                    bands.Add(defbands[bandNo - 1]);
                }
                _prjBands = bands.ToArray();
            }
        }

        private void TryCreateDefaultArgs(IRasterDataProvider srcRaster, FY3_MERSI_PrjSettings prjSettings, ref ISpatialReference dstSpatialRef)
        {
            if (dstSpatialRef == null)
                dstSpatialRef = _srcSpatialRef;
            if (string.IsNullOrWhiteSpace(prjSettings.OutFormat))
                prjSettings.OutFormat = "LDF";
            if (dstSpatialRef.ProjectionCoordSystem == null)
            {
                _srcImgResolution = 0.01F;
            }
            else
            {
                _srcImgResolution = 1000F;
            }
            if (prjSettings.OutResolutionX == 0 || prjSettings.OutResolutionY == 0)
            {
                if (dstSpatialRef.ProjectionCoordSystem == null)
                {
                    prjSettings.OutResolutionX = 0.01F;
                    prjSettings.OutResolutionY = 0.01F;
                }
                else
                {
                    prjSettings.OutResolutionX = 1000F;
                    prjSettings.OutResolutionY = 1000F;
                }
            }
            if (prjSettings.OutBandNos == null || prjSettings.OutBandNos.Length == 0)
            {
                _prjBands = OrbitBandDefCollection.MERSI_1000_OrbitDefCollecges();
            }
            else
            {
                List<PrjBand> bands = new List<PrjBand>();
                PrjBand[] defbands = OrbitBandDefCollection.MERSI_1000_OrbitDefCollecges();
                foreach (int bandNo in prjSettings.OutBandNos)
                {
                    bands.Add(defbands[bandNo - 1]);
                }
                _prjBands = bands.ToArray();
            }
        }

        private void DoSession(IRasterDataProvider srcRaster, IRasterDataProvider locationRaster, ISpatialReference dstSpatialRef, FY3_MERSI_PrjSettings prjSettings, Action<int, string> progressCallback)
        {
            if (_curSession == null || _curSession != srcRaster || _isBeginSession)
            {
                Size srcImgSize = new Size(srcRaster.Width, srcRaster.Height);
                ReadyLocations(srcRaster, dstSpatialRef, prjSettings, out _srcLocationSize, out _maxPrjEnvelope, progressCallback);
                if (progressCallback != null)
                    progressCallback(4, "准备其他参数");
                if (prjSettings.IsRadiation)
                    ReadyRadiationArgs(srcRaster);
                if (prjSettings.IsSolarZenith) //&& prjSettings.IsRadiation
                {
                    _szDataFilename = GetSolarZenithCacheFilename(locationRaster.fileName);
                    if (!File.Exists(_szDataFilename))
                        ReadySolarZenithArgsToFile(locationRaster);
                    else
                        _solarZenithCacheRaster = GeoDataDriver.Open(_szDataFilename) as IRasterDataProvider;
                    if (prjSettings.IsSensorZenith)
                    {
                        ReadySensorZenith(locationRaster);
                    }
                }
                _rasterDataBands = TryCreateRasterDataBands(srcRaster, prjSettings, progressCallback);//待投影的波段
                _isBeginSession = false;
            }
        }

        private void ReadySensorZenith(IRasterDataProvider srcRaster)
        {
            _sensorSenithRaster = srcRaster;
            IRasterBand[] bands = srcRaster.BandProvider.GetBands("SensorZenith");
            if (bands != null || bands.Length != 1)
                _sensorSenithBand = bands[0];
        }
        
        public override void EndSession()
        {
            base.EndSession();
            _xs = null;
            _ys = null;
            _vir_Cal_Coeff = null;
            if (_solarZenithCacheRaster != null)
            {
                _solarZenithCacheRaster.Dispose();
                _solarZenithCacheRaster = null;
            }
            if (_outLdfDriver != null)
            {
                _outLdfDriver.Dispose();
                _outLdfDriver = null;
            }
            if (_longitudeBand != null)
            {
                _longitudeBand.Dispose();
                _longitudeBand = null;
            }
            if (_latitudeBand != null)
            {
                _latitudeBand.Dispose();
                _latitudeBand = null;
            }
            if (_latitudeRaster != null)
            {
                _latitudeRaster.Dispose();
                _latitudeRaster = null;
            }
            if (_longitudeRaster != null)
            {
                _longitudeRaster.Dispose();
                _longitudeRaster = null;
            }
        }

        private IRasterBand[] TryCreateRasterDataBands(IRasterDataProvider srcRaster, FY3_MERSI_PrjSettings fy3prjSettings, Action<int, string> progressCallback)
        {
            IBandProvider srcbandpro = srcRaster.BandProvider as IBandProvider;
            List<IRasterBand> rasterBands = new List<IRasterBand>();
            for (int i = 0; i < _prjBands.Length; i++)
            {
                if (progressCallback != null)
                    progressCallback(_readyProgress++, "准备第" + i + "个输入数据通道");
                PrjBand bandMap = _prjBands[i];
                IRasterBand[] latBands = srcbandpro.GetBands(bandMap.DataSetName);
                IRasterBand band = latBands[bandMap.DataSetIndex];
                rasterBands.Add(band);
            }
            return rasterBands.ToArray();
        }

        private void ReadyLocations(IRasterDataProvider srcRaster, ISpatialReference dstSpatialRef, FY3_MERSI_PrjSettings fy3prjSettings,
            out Size locationSize, out PrjEnvelope maxPrjEnvelope, Action<int, string> progressCallback)
        {
            if (progressCallback != null)
                progressCallback(_readyProgress++, "读取经纬度数据集");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            double[] xs = null;
            double[] ys = null;
            //Size locationSize;
            ReadLocations(srcRaster, out xs, out ys, out locationSize);
            TryResetLonlatForLeftRightInvalid(xs, ys, locationSize);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds+"ms....1");
            if (progressCallback != null)
                progressCallback(_readyProgress++, "预处理经纬度数据集");
            sw.Restart();
            _rasterProjector.ComputeDstEnvelope(_srcSpatialRef, xs, ys, locationSize, dstSpatialRef, out maxPrjEnvelope, progressCallback);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + "ms....2");
            _xs = xs;
            _ys = ys;
        }

        protected override void ReadLocations(IRasterDataProvider srcRaster, out double[] xs, out double[] ys, out Size locationSize)
        {
            IRasterBand longitudeBand = null;
            IRasterBand latitudeBand = null;
            ReadLocations(srcRaster, out longitudeBand, out latitudeBand);
            ReadBandData(longitudeBand, out xs, out locationSize);
            ReadBandData(latitudeBand, out ys, out locationSize);
        }

        private void ReadLocations(IRasterDataProvider srcRaster, out IRasterBand longitudeBand, out IRasterBand latitudeBand)
        {
            IBandProvider srcbandpro = srcRaster.BandProvider as IBandProvider;
            IRasterBand[] lonsBands = srcbandpro.GetBands("Longitude");
            IRasterBand[] latBands = srcbandpro.GetBands("Latitude");
            if (lonsBands == null || latBands == null || lonsBands.Length == 0 || latBands.Length == 0 || lonsBands[0] == null || latBands[0] == null)
                throw new Exception("获取经纬度数据失败");
            longitudeBand = lonsBands[0];
            latitudeBand = latBands[0];
        }

        public class DnInterceptSlope
        {
            public float Intercept = 0f;
            public float Slope = 1f;
        }

        private Dictionary<string, DnInterceptSlope[]> _refSbDnIS = new Dictionary<string, DnInterceptSlope[]>();

        private void DoRadiation(IRasterDataProvider srcImgRaster, ushort[] srcBlockBandData, Size srcBlockDataSize, string dsName, int dsIndex, float[] solarZenithData, Size srcLocationSize)
        {
            if(!_prjSettings.IsRadiation)
                return;
            bool isSolarZenith = _prjSettings.IsSolarZenith;

            switch (dsName)
            {
                case "EV_250_Aggr.1KM_RefSB"://定标系数为VIS_Cal_Coeff 中的前四组,k0，k1，k2
                    {
                        float k0 = _vir_Cal_Coeff[dsIndex * 3];
                        float k1 = _vir_Cal_Coeff[dsIndex * 3 + 1];
                        float k2 = _vir_Cal_Coeff[dsIndex * 3 + 2];

                        ReadDnIS(srcImgRaster, dsName);
                        float intercept = _dsIntercept[dsIndex];
                        float slope = _dsSlope[dsIndex];

                        RefSBRadiation(srcBlockBandData, k0, k1, k2, intercept, slope, solarZenithData, isSolarZenith);
                    }
                    break;
                case "EV_1KM_RefSB":        //定标系数为VIS_Cal_Coeff 中的后15 组
                    {
                        float k0 = _vir_Cal_Coeff[12 + dsIndex * 3];
                        float k1 = _vir_Cal_Coeff[12 + dsIndex * 3 + 1];
                        float k2 = _vir_Cal_Coeff[12 + dsIndex * 3 + 2];

                        ReadDnIS(srcImgRaster, dsName);
                        float intercept = _dsIntercept[dsIndex];
                        float slope = _dsSlope[dsIndex];

                        RefSBRadiation(srcBlockBandData, k0, k1, k2, intercept, slope, solarZenithData, isSolarZenith);
                    }
                    break;
                case "EV_250_RefSB_b1"://_vir_Cal_Coeff
                    {
                        float k0 = _vir_Cal_Coeff[dsIndex * 0];
                        float k1 = _vir_Cal_Coeff[dsIndex * 0 + 1];
                        float k2 = _vir_Cal_Coeff[dsIndex * 0 + 2];

                        ReadDnIS(srcImgRaster, dsName);
                        float intercept = _dsIntercept[dsIndex];
                        float slope = _dsSlope[dsIndex];

                        RefSBRadiation(srcBlockBandData, k0, k1, k2, intercept, slope, solarZenithData, isSolarZenith, srcBlockDataSize, srcLocationSize);
                    }
                    break;
                case "EV_250_RefSB_b2":
                    {
                        float k0 = _vir_Cal_Coeff[dsIndex * 1];
                        float k1 = _vir_Cal_Coeff[dsIndex * 1 + 1];
                        float k2 = _vir_Cal_Coeff[dsIndex * 1 + 2];

                        ReadDnIS(srcImgRaster, dsName);
                        float intercept = _dsIntercept[dsIndex];
                        float slope = _dsSlope[dsIndex];

                        RefSBRadiation(srcBlockBandData, k0, k1, k2, intercept, slope, solarZenithData, isSolarZenith, srcBlockDataSize, srcLocationSize);
                    }
                    break;
                case "EV_250_RefSB_b3":
                    {
                        float k0 = _vir_Cal_Coeff[dsIndex * 2];
                        float k1 = _vir_Cal_Coeff[dsIndex * 2 + 1];
                        float k2 = _vir_Cal_Coeff[dsIndex * 2 + 2];

                        ReadDnIS(srcImgRaster, dsName);
                        float intercept = _dsIntercept[dsIndex];
                        float slope = _dsSlope[dsIndex];

                        RefSBRadiation(srcBlockBandData, k0, k1, k2, intercept, slope, solarZenithData, isSolarZenith, srcBlockDataSize, srcLocationSize);
                    }
                    break;
                case "EV_250_RefSB_b4":
                    {
                        float k0 = _vir_Cal_Coeff[dsIndex * 3];
                        float k1 = _vir_Cal_Coeff[dsIndex * 3 + 1];
                        float k2 = _vir_Cal_Coeff[dsIndex * 3 + 2];

                        ReadDnIS(srcImgRaster, dsName);
                        float intercept = _dsIntercept[dsIndex];
                        float slope = _dsSlope[dsIndex];

                        RefSBRadiation(srcBlockBandData, k0, k1, k2, intercept, slope, solarZenithData, isSolarZenith, srcBlockDataSize, srcLocationSize);
                    }
                    break;
                case "EV_250_Aggr.1KM_Emissive":    //0~4095
                case "EV_250_Emissive":
                    {   //亮温=10*(c2*v/log(1+c1*v*v*v/(观测值/100.0)))
                        double c2v = C2 * V;
                        double c1v3 = C1 * V * V * V * 100.0;
                        RadiationEmissive(srcBlockBandData, c2v, c1v3);
                    }
                    break;
                default:
                    break;
            }
        }
        
        private float[] _dsIntercept;
        private float[] _dsSlope;

        private void ReadDnIS(IRasterDataProvider srcImgRaster, string dsName)
        {
            if (srcImgRaster == null)
                throw new ArgumentNullException("srcRaster", "获取亮温转换参数失败：参数srcRaster为空");
            if (dsName == null)
                throw new ArgumentNullException("dataSetName", "获取亮温转换参数失败：参数srcRaster为空");
            try
            {
                IBandProvider srcbandpro = srcImgRaster.BandProvider as IBandProvider;
                int count = srcbandpro.GetBands(dsName).Length;
                _dsIntercept = ReadDataSetAttrToFloat(srcbandpro, dsName, "Intercept", count);
                _dsSlope = ReadDataSetAttrToFloat(srcbandpro, dsName, "Slope", count);
            }
            catch (Exception ex)
            {
                throw new Exception("获取亮温转换参数失败:" + ex.Message, ex.InnerException);
            }
        }

        private float[] ReadDataSetAttrToFloat(IBandProvider srcbandpro, string dataSetName, string attrName, int length)
        {
            float[] value = new float[length];
            IDictionary<string, string> dsAtts = srcbandpro.GetDatasetAttributes(dataSetName);
            string refSbCalStr = dsAtts[attrName];
            string[] refSbCals = refSbCalStr.Split(',');
            if (refSbCals.Length >= length)
            {
                for (int i = 0; i < length; i++)
                {
                    value[i] = float.Parse(refSbCals[i]);
                }
                return value;
            }
            else
                return null;
        }

        protected short[] _sensorZenithData = null;
        protected override void TryReadZenithData(int xOffset, int yOffset, int blockWidth, int blockHeight)
        {
            //亮温临边变暗订正,读取卫星天顶角数据。
            if (_isSensorZenith && _sensorSenithBand != null)
            {
                _sensorZenithData = ReadBandData(_sensorSenithBand, xOffset, yOffset, blockWidth, blockHeight);
            }
        }

        protected override void ReleaseZenithData()
        {
            _sensorZenithData = null;
        }

        //MERSI 辐射通道定标
        private void RadiationEmissive(ushort[] bandData, double c2v, double c1v3)
        {
            Parallel.For(0, bandData.Length, (index) =>
            {
                double temperatureBB;
                double sensorZenith;
                double deltaT;
                UInt16 dn = bandData[index];
                temperatureBB = (c2v / Math.Log(1 + c1v3 / dn));
                //添加"临边变暗订正"
                if (_isSensorZenith && _sensorZenithData != null)
                {
                    sensorZenith = _sensorZenithData[index] * 0.01d;//原始数据放大了100倍
                    deltaT = temperatureBB + (Math.Pow(Math.E, 0.00012 * sensorZenith * sensorZenith) - 1) * (0.1072 * temperatureBB - 26.81);
                    bandData[index] = (UInt16)(deltaT * 10);
                }
                else
                    bandData[index] = (UInt16)(temperatureBB * 10);
                if (bandData[index] > 6500)
                    bandData[index] = 0;

            });
        }

        //MERSI 0250M 反射通道定标.高度角数据和数据尺寸不一致,
        private void RefSBRadiation(ushort[] bandData, float k0, float k1, float k2, float intercept, float slope, float[] solarZenithData, bool isSolarZenith, Size dataSize, Size angleSize)
        {
            int height = dataSize.Height;
            int width = dataSize.Width;
            if (isSolarZenith)
            {
                float scoreX = (float)dataSize.Width / angleSize.Width;
                float scoreY = (float)dataSize.Height / angleSize.Height;
                Parallel.For(0, dataSize.Height, (row) =>
                {
                    int rOffset = row * width;
                    for (int col = 0; col < dataSize.Width; col++)
                    {
                        int index = rOffset + col;
                        float dn = slope * (bandData[index] - intercept);   //dn值（原始观测值）调整
                        double radiation = k0 + k1 * dn + k2 * dn * dn;//(定标)计算反射率,理论上讲反射率应当是0-100
                        int szCol = (int)(col / scoreX);
                        int szRow = (int)(row / scoreY);
                        double solarZenith = solarZenithData[szRow * angleSize.Width + szCol];//;
                        bandData[index] = (UInt16)(radiation * solarZenith);
                        if (bandData[index] > 65000)
                            bandData[index] = 0;
                    }
                });
            }
            else
            {
                Parallel.For(0, dataSize.Height, (row) =>
                {
                    for (int col = 0; col < dataSize.Width; col++)
                    {
                        int index = row * width + col;
                        float dn = slope * (bandData[index] - intercept);   //dn值（原始观测值）调整
                        double radiation = k0 + k1 * dn + k2 * dn * dn;//(定标)计算反射率,理论上讲反射率应当是0-100
                        bandData[index] = (UInt16)(10 * radiation);//放大到0-1000
                        if (bandData[index] > 65000)
                            bandData[index] = 0;
                    }
                });
            }
        }

        //可见光反射率计算,角度信息和数据尺寸一致。
        private void RefSBRadiation(ushort[] srcBandData, float k0, float k1, float k2, float intercept, float slope, float[] solarZenithData, bool isSolarZenith)
        {
            if (isSolarZenith)//天顶角订正
            {
                Parallel.For(0, srcBandData.Length, (index) =>
                {
                    float bandData = slope * (srcBandData[index] - intercept);   //dn值（原始观测值）调整回恢复
                    double radiation = k0 + k1 * bandData + k2 * bandData * bandData;
                    double solarZenith = solarZenithData[index];
                    srcBandData[index] = (UInt16)(radiation * solarZenith);
                    if (srcBandData[index] > 65000)
                        srcBandData[index] = 0;
                });
            }
            else
            {
                Parallel.For(0, srcBandData.Length, (index) =>
                {
                    float bandData = slope * (srcBandData[index] - intercept);   //dn值（原始观测值）调整回恢复
                    double radiation = k0 + k1 * bandData + k2 * bandData * bandData;
                    srcBandData[index] = (UInt16)(10 * radiation);      //放大到0-1000
                    if (srcBandData[index] > 65000)
                        srcBandData[index] = 0;
                });
            }
        }
        
        private void ReadySolarZenithArgsToFile(IRasterDataProvider srcRaster)
        {
            if (srcRaster == null)
                throw new ArgumentNullException("srcRaster", "获取太阳天顶角数据失败");
            try
            {
                IBandProvider srcbandpro = srcRaster.BandProvider as IBandProvider;
                Size srcSize = new System.Drawing.Size(srcRaster.Width, srcRaster.Height);
                short[] readSolarZenithData = ReadDataSetToInt16(srcbandpro, srcSize, SolarZenith, 0);
                int length = srcRaster.Width * srcRaster.Height;
                float[] saveSolarZenithData = new float[length];
                Parallel.For(0, length, index =>
                {
                    if (readSolarZenithData[index] > 0 && readSolarZenithData[index] < 18000)
                        saveSolarZenithData[index] = (float)(10.0f / Math.Cos(readSolarZenithData[index] * DEG_TO_RAD_P100));
                    else
                        saveSolarZenithData[index] = 0;
                });
                _solarZenithCacheRaster = WriteData(saveSolarZenithData, _szDataFilename, srcSize.Width, srcSize.Height);
                saveSolarZenithData = null;
                readSolarZenithData = null;
            }
            catch (Exception ex)
            {
                throw new Exception("获取太阳天顶角数据失败", ex.InnerException);
            }
        }
        
        private void ReadyRadiationArgs(IRasterDataProvider srcRaster)
        {
            if (srcRaster == null)
                throw new ArgumentNullException("srcRaster", "获取亮温转换参数失败：参数srcRaster为空");
            try
            {
                IBandProvider srcbandpro = srcRaster.BandProvider as IBandProvider;
                _vir_Cal_Coeff = ReadFileAttributeToFloat(srcbandpro, VIR_Cal_Coeff, 57);
            }
            catch (Exception ex)
            {
                throw new Exception("获取亮温转换参数失败:"+ex.Message, ex.InnerException);
            }
        }

        private float[] ReadFileAttributeToFloat(IBandProvider srcbandpro, string AttrName, int length)
        {
            float[] value = new float[length];
            Dictionary<string, string> dsAtts = srcbandpro.GetAttributes();
            string refSbCalStr = dsAtts[AttrName];
            string[] refSbCals = refSbCalStr.Split(',');
            if (refSbCals.Length >= length)
            {
                for (int i = 0; i < length; i++)
                {
                    value[i] = float.Parse(refSbCals[i]);
                }
                return value;
            }
            else
                return null;
        }

        private Int16[] ReadDataSetToInt16(IBandProvider srcbandpro, Size srcSize, string dataSetName, int bandIndex)
        {
            Int16[] data = new Int16[srcSize.Width * srcSize.Height];
            IRasterBand[] rasterBands = srcbandpro.GetBands(dataSetName);
            using (IRasterBand rasterBand = rasterBands[0])
            {
                unsafe
                {
                    fixed (Int16* ptr = data)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        rasterBand.Read(0, 0, srcSize.Width, srcSize.Height, bufferPtr, enumDataType.Int16, srcSize.Width, srcSize.Height);
                    }
                }
            }
            return data;
        }

        private float[] ReadDataSetToSingle(IBandProvider srcbandpro, Size srcSize, string dataSetName, int bandIndex)
        {
            Single[] data = new Single[srcSize.Width * srcSize.Height];
            IRasterBand[] rasterBands = srcbandpro.GetBands(dataSetName);
            using (IRasterBand rasterBand = rasterBands[0])
            {
                unsafe
                {
                    fixed (Single* ptr = data)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        rasterBand.Read(0, 0, srcSize.Width, srcSize.Height, bufferPtr, enumDataType.Float, srcSize.Width, srcSize.Height);
                    }
                }
            }
            return data;
        }
        
        private void ReadBandData(IRasterBand band, out double[] bandData, out Size srcSize)
        {
            int width = band.Width;
            int height = band.Height;
            srcSize = new Size(width, height);
            bandData = new Double[width * height];
            unsafe
            {
                fixed (Double* ptrLong = bandData)
                {
                    IntPtr bufferPtrLong = new IntPtr(ptrLong);
                    band.Read(0, 0, width, height, bufferPtrLong, enumDataType.Double, width, height);
                }
            } 
        }

        public override FilePrjSettings CreateDefaultPrjSettings()
        {
            return new FY3_VIRR_PrjSettings();
        }

        public override void ComputeDstEnvelope(IRasterDataProvider srcRaster, ISpatialReference dstSpatialRef, out PrjEnvelope maxPrjEnvelope, Action<int, string> progressCallback)
        {
            if (srcRaster != null)
            {
                Size srcSize = new Size(srcRaster.Width, srcRaster.Height);
                Size srcLocationSize;
                ReadyLocations(srcRaster, dstSpatialRef,null, out srcLocationSize, out maxPrjEnvelope, progressCallback);
            }
            else
            {
                maxPrjEnvelope = PrjEnvelope.Empty;
            }
        }
        
        protected override void DoRadiation(IRasterDataProvider srcImgRaster, int i, ushort[] srcBandData, float[] solarZenithData, Size srcBlockImgSize, Size angleSize)
        {
            if (_isRadiation)
            {
                string ds = _prjBands[i].DataSetName;
                int dsIndex = _prjBands[i].DataSetIndex;
                DoRadiation(srcImgRaster, srcBandData, srcBlockImgSize, ds, dsIndex, solarZenithData, angleSize);
            }
        }
    }
}
