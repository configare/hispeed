using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RasterProject;
using GeoDo.Project;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.IO;

namespace GeoDo.FileProject
{
    /// <summary>
    /// FY2E圆盘数据，投影转换
    /// </summary>
    [Export(typeof(IFileProjector)), ExportMetadata("VERSION", "1")]
    public class FY2_NOMProjection : FileProjector
    {
        private SpatialReference _srcSpatialRef;
        //private string[] _agiles = new string[] { "NOMSatelliteZenith", "NOMSunGlintAngle", "NOMSunZenith"};//FY2几个角度的波段名称
        Fy2_NOM_PrjSettings _prjSettings;

        public FY2_NOMProjection()
            : base()
        {
            _name = "FY2NOM";
            _fullname = "FY2_NOM数据投影";
            _rasterProjector = new RasterProjector();
            _srcSpatialRef = new SpatialReference(new GeographicCoordSystem());
            _supportAngles = new object[] { "NOMSatelliteZenith", "NOMSunGlintAngle", "NOMSunZenith" };
        }

        public override bool IsSupport(string fileName)
        {
            return false;
        }

        public override void ComputeDstEnvelope(RSS.Core.DF.IRasterDataProvider srcRaster, Project.ISpatialReference dstSpatialRef, out RasterProject.PrjEnvelope maxPrjEnvelope, Action<int, string> progressCallback)
        {
            if (dstSpatialRef == null || dstSpatialRef.ProjectionCoordSystem == null)
            {
                maxPrjEnvelope = new PrjEnvelope(60, 150, -70, 70);
            }
            else
            {
                maxPrjEnvelope = null;
            }
        }

        public override FilePrjSettings CreateDefaultPrjSettings()
        {
            Fy2_NOM_PrjSettings prj = new Fy2_NOM_PrjSettings();
            prj.OutFormat = ".ldf";
            prj.OutResolutionX = 0.05f;
            prj.OutResolutionY = 0.05f;
            return prj;
        }

        protected override void DoRadiation(RSS.Core.DF.IRasterDataProvider srcImgRaster, int bandNo, ushort[] srcBandData, float[] solarZenithData, System.Drawing.Size srcBlockImgSize, System.Drawing.Size angleSize)
        {
            throw new NotImplementedException();
        }

        protected void DoRadiation(IRasterBand rasterBand, ushort[] srcBandData, float[] solarZenithData, System.Drawing.Size srcBlockImgSize, System.Drawing.Size angleSize)
        {
            //if (!_isRadiation)
            //    return;
            int bandNo = rasterBand.BandNo; //1,2,3,4是亮温，5是可见光，6是云信息
            if (bandNo <= _bandNoValueMap.Length)
            {
                float[] valueMap = _bandNoValueMap[bandNo - 1];
                if (valueMap == null)
                    return;
                if (_isSensorZenith && _sensorZenithData != null) //"临边变暗订正"。
                {
                    float sensorZenith;
                    double deltaT;
                    float temperatureBB;
                    for (int i = 0; i < srcBandData.Length; i++)
                    {
                        if (srcBandData[i] < valueMap.Length)//采用查找表的方式获取亮温值。
                        {
                            temperatureBB = valueMap[srcBandData[i]];
                            if (bandNo < 5)
                            {
                                sensorZenith = _sensorZenithData[i] * 0.01f;
                                deltaT = temperatureBB + (Math.Pow(Math.E, 0.00012d * sensorZenith * sensorZenith) - 1) * (0.1072d * temperatureBB - 26.81d);
                                srcBandData[i] = (UInt16)(deltaT * 10);
                            }
                            else
                                srcBandData[i] = (UInt16)(valueMap[srcBandData[i]] * 10);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < srcBandData.Length; i++)
                    {
                        if (srcBandData[i] < valueMap.Length)//采用查找表的方式获取亮温值。
                        {
                            srcBandData[i] = (UInt16)(valueMap[srcBandData[i]] * 10);
                        }
                    }
                }
            }
        }

        protected short[] _sensorZenithData = null;
        protected override void TryReadZenithData(int xOffset, int yOffset, int blockWidth, int blockHeight)
        {
            //亮温临边变暗订正,读取卫星天顶角数据。
            if (_isSensorZenith && _sensorSenithBand != null)
            {
                _sensorZenithData = ReadRanToDeg(_sensorSenithBand, xOffset, yOffset, blockWidth, blockHeight);//原始数据为弧度数据，这里需要处理下
            }
        }

        protected override void ReleaseZenithData()
        {
            _sensorZenithData = null;
        }

        protected override void ReadLocations(RSS.Core.DF.IRasterDataProvider srcRaster, out double[] xs, out double[] ys, out System.Drawing.Size locationSize)
        {
            IRasterBand longitudeBand = null;
            IRasterBand latitudeBand = null;
            try
            {
                ReadLocations(srcRaster, out longitudeBand, out latitudeBand);
                ReadBandData(longitudeBand, out xs, out locationSize);
                ReadBandData(latitudeBand, out ys, out locationSize);
                TryApplyGeoInterceptSlope(xs, ys);
            }
            finally
            {
                if (longitudeBand != null)
                    longitudeBand.Dispose();
                if (latitudeBand != null)
                    latitudeBand.Dispose();
            }
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

        public override void Project(RSS.Core.DF.IRasterDataProvider srcRaster, FilePrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback)
        {
            try
            {
                if (prjSettings == null)
                    prjSettings = CreateDefaultPrjSettings();
                ReadyArgs(srcRaster, prjSettings, dstSpatialRef, progressCallback);
                using (IRasterDataProvider dstRaster = CreateOutFile(prjSettings, dstSpatialRef, srcRaster.DataIdentify))
                {
                    if (dstRaster == null)
                        return;
                    string[] options = LdfOptions(prjSettings, dstRaster.SpatialRef, _outResolutionX, _outResolutionY, srcRaster.DataIdentify);
                    ReadyAngleFiles(srcRaster, _outfilename, prjSettings, _dstSize, options);
                    _dstDataBands = GetDstRasterBand(dstRaster, prjSettings, 0);
                    List<IRasterBand> srcbands = new List<IRasterBand>();
                    List<IRasterBand> dstbands = new List<IRasterBand>();
                    for (int i = 0; i < _rasterDataBands.Length; i++)
                    {
                        if (_rasterDataBands[i].BandNo != 5)
                        {
                            srcbands.Add(_rasterDataBands[i]);
                            dstbands.Add(_dstDataBands[i]);
                        }
                    }
                    if (dstSpatialRef == null || dstSpatialRef.ProjectionCoordSystem == null)
                        _srcImgResolution = 0.05f;
                    else
                        _srcImgResolution = 5000f;
                    //先处理5km的通道
                    //_orbitBlock = new Block { xOffset = 0, yBegin = 0, xEnd = srcbands[0].Width - 1, yEnd = srcbands[0].Height - 1 };
                    if (dstbands.Count != 0)
                        ProjectToLDF(srcbands.ToArray(), dstbands.ToArray(), 0, progressCallback);

                    //处理1km的通道
                    srcbands.Clear();
                    dstbands.Clear();
                    if (dstSpatialRef == null || dstSpatialRef.ProjectionCoordSystem == null)
                        _srcImgResolution = 0.01f;
                    else
                        _srcImgResolution = 1000f;
                    for (int i = 0; i < _rasterDataBands.Length; i++)
                    {
                        if (_rasterDataBands[i].BandNo == 5)
                        {
                            srcbands.Add(_rasterDataBands[i]);
                            dstbands.Add(_dstDataBands[i]);
                        }
                    }
                    if (srcbands.Count == 1)
                    {
                        //_orbitBlock = new Block { xOffset = 0, yBegin = 0, xEnd = srcbands[0].Width - 1, yEnd = srcbands[0].Height - 1 };
                        ProjectToLDF(srcbands.ToArray(), dstbands.ToArray(), 0, progressCallback);
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
                EndSession();
                if (prjSettings.IsClearPrjCache)
                    TryDeleteCurCatch();
            }
        }

        public override void EndSession()
        {
            base.EndSession();
            if (_rasterDataBands != null && _rasterDataBands.Length != 0)
            {
                for (int i = 0; i < _rasterDataBands.Length; i++)
                {
                    if (_rasterDataBands[i] != null)
                    {
                        _rasterDataBands[i].Dispose();
                        _rasterDataBands[i] = null;
                    }
                }
                _rasterDataBands = null;
            }
            if (_dstDataBands != null && _dstDataBands.Length != 0)
            {
                for (int i = 0; i < _dstDataBands.Length; i++)
                {
                    if (_dstDataBands[i] != null)
                    {
                        _dstDataBands[i].Dispose();
                        _dstDataBands[i] = null;
                    }
                }
                _dstDataBands = null;
            }
        }

        protected void ProjectToLDF(IRasterBand[] srcBands, IRasterBand[] dstBands, int beginBandIndex, Action<int, string> progressCallback)
        {
            //progressCallback = progressCallback;
            if (srcBands == null || srcBands[0].Width == 0 || srcBands[0].Height == 0)
                throw new Exception("投影数据失败：无法读取源数据,或者源数据高或宽为0。");
            Size srcImgSize = new Size(srcBands[0].Width, srcBands[0].Height);
            Size outSize = _dstEnvelope.GetSize(_outResolutionX, _outResolutionY);
            float bufferResolutionX = 0f;
            float bufferResolutionY = 0f;
            float outXScale = _srcImgResolution / _outResolutionX; 
            float outYScale = _srcImgResolution / _outResolutionY; 
            if (outXScale > 1.5f || outYScale > 1.5f)
            {
                bufferResolutionX = _srcImgResolution;
                bufferResolutionY = _srcImgResolution;
            }
            else
            {
                bufferResolutionX = _outResolutionX;
                bufferResolutionY = _outResolutionY;
            }
            int blockXCount;
            int blockYCount;
            int blockWidth;
            int blockHeight;
            //后面投影需要的内存：（double）经纬度数据、（int16）原始通道数据、（int16）投影后通道、（int16）其他（如角度数据等）
            GetBlockNumber(outSize, _srcLocationSize, outXScale, outYScale, out blockXCount, out blockYCount, out blockWidth, out blockHeight);
            int imgLocationRatioX = srcImgSize.Width / _srcLocationSize.Width;
            int imgLocationRatioY = srcImgSize.Height / _srcLocationSize.Height;
            progressCount = blockYCount * blockXCount * (_dstBandCount + (_angleBands == null ? 0 : _angleBands.Length) + (_extSrcBands == null ? 0 : _extSrcBands.Length));
            progress = 0;
            percent = 0;
            Size bufferSize;

            #region 在需要分块的情况下，采样经纬度数据集
            int bC = 1;
            int tmpWidth = 0;
            int tmpHeight = 0;
            double[] tmpxs = null;
            double[] tmpys = null;
            if (blockYCount * blockXCount > 1 && (_xs == null || _ys == null))
            {
                bC = (int)Math.Sqrt(blockXCount * blockYCount) + 1;
                tmpWidth = _srcLocationSize.Width / bC;
                tmpHeight = _srcLocationSize.Height / bC;
                tmpxs = ReadSampleDatas(_longitudeBand, 0, 0, tmpWidth, tmpHeight);
                tmpys = ReadSampleDatas(_latitudeBand, 0, 0, tmpWidth, tmpHeight);
                TryApplyGeoInterceptSlope(tmpxs, tmpys);
                _rasterProjector.Transform(SpatialReference.GetDefault(), tmpxs, tmpys, _dstSpatialRef);
            }
            #endregion

            for (int blockXNo = 0; blockXNo < blockXCount; blockXNo++)
            {
                for (int blockYNo = 0; blockYNo < blockYCount; blockYNo++)
                {
                    //起始偏移，结束偏移
                    int beginX = blockWidth * blockXNo;
                    int beginY = blockHeight * blockYNo;
                    if (beginX >= outSize.Width || beginY >= outSize.Height)
                        continue;
                    if (beginX + blockWidth > outSize.Width)
                        blockWidth = outSize.Width - beginX;
                    if (beginY + blockHeight > outSize.Height)
                        blockHeight = outSize.Height - beginY;

                    //当前块的四角范围
                    double blockMinX = _dstEnvelope.MinX + beginX * _outResolutionX;
                    double blockMaxX = blockMinX + blockWidth * _outResolutionX;
                    double blockMaxY = _dstEnvelope.MaxY - beginY * _outResolutionY;
                    double blockMinY = blockMaxY - blockHeight * _outResolutionY;
                    PrjEnvelope blockEnvelope = new PrjEnvelope(blockMinX, blockMaxX, blockMinY, blockMaxY);
                    bufferSize = blockEnvelope.GetSize(bufferResolutionX, bufferResolutionY);
                    //根据当前输出块,反算出对应的源数据块(轨道)起始行列，为了减小后面需要读取的源数据大小
                    Block curOrbitblock = null;
                    //开始获取当前分块的经纬度数据集，计算轨道数据范围偏移
                    double[] srcBlockXs;
                    double[] srcBlockYs;
                    if (blockYCount == 1 && blockXCount == 1)                           //没分块的情况
                    {
                        curOrbitblock = _orbitBlock.Clone() as Block;
                        if (curOrbitblock.xOffset < _left)
                            curOrbitblock.xOffset = _left;
                        if (curOrbitblock.xEnd > _srcLocationSize.Width - 1 - _right)
                            curOrbitblock.xEnd = _srcLocationSize.Width - 1 - _right;
                        if (curOrbitblock.Width == _srcLocationSize.Width && curOrbitblock.Height == _srcLocationSize.Height)
                        {
                            if (_xs != null && _ys != null)
                            {
                                srcBlockXs = _xs;
                                srcBlockYs = _ys;
                            }
                            else
                            {
                                srcBlockXs = ReadBlockDatas(_longitudeBand, curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height);
                                srcBlockYs = ReadBlockDatas(_latitudeBand, curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height);

                                TryApplyGeoInterceptSlope(srcBlockXs, srcBlockYs);
                                _rasterProjector.Transform(SpatialReference.GetDefault(), srcBlockXs, srcBlockYs, _dstSpatialRef);
                            }
                        }
                        else
                        {
                            if (_xs != null && _ys != null)
                            {
                                GetBlockDatas(_xs, _ys, _srcLocationSize.Width, _srcLocationSize.Height, curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height, out srcBlockXs, out srcBlockYs);
                            }
                            else
                            {
                                srcBlockXs = ReadBlockDatas(_longitudeBand, curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height);
                                srcBlockYs = ReadBlockDatas(_latitudeBand, curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height);
                                TryApplyGeoInterceptSlope(srcBlockXs, srcBlockYs);
                                _rasterProjector.Transform(SpatialReference.GetDefault(), srcBlockXs, srcBlockYs, _dstSpatialRef);
                            }
                        }
                    }
                    else
                    {
                        if (_xs != null && _ys != null)
                        {
                            GetEnvelope(_xs, _ys, _srcLocationSize.Width, _srcLocationSize.Height, blockEnvelope, out curOrbitblock);
                        }
                        else
                        {
                            //计算偏移。
                            GetEnvelope(tmpxs, tmpys, tmpWidth, tmpHeight, blockEnvelope, out curOrbitblock);
                            curOrbitblock = curOrbitblock.Zoom(bC, bC);
                        }
                        if (curOrbitblock.Width <= 0 || curOrbitblock.Height <= 0)      //当前分块不在图像内部
                        {
                            progress += _dstBandCount;
                            continue;
                        }
                        if (curOrbitblock.xOffset < _left)
                            curOrbitblock.xOffset = _left;
                        if (curOrbitblock.xEnd > _srcLocationSize.Width - 1 - _right)
                            curOrbitblock.xEnd = _srcLocationSize.Width - 1 - _right;
                        if (_xs != null && _ys != null)
                        {
                            GetBlockDatas(_xs, _ys, _srcLocationSize.Width, _srcLocationSize.Height, curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height, out srcBlockXs, out srcBlockYs);
                        }
                        else
                        {
                            srcBlockXs = ReadBlockDatas(_longitudeBand, curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height);
                            srcBlockYs = ReadBlockDatas(_latitudeBand, curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height);
                            TryApplyGeoInterceptSlope(srcBlockXs, srcBlockYs);
                            _rasterProjector.Transform(SpatialReference.GetDefault(), srcBlockXs, srcBlockYs, _dstSpatialRef);
                        }
                    }
                    int srcBlockJdWidth = curOrbitblock.Width;
                    int srcBlockJdHeight = curOrbitblock.Height;
                    int srcBlockImgWidth = curOrbitblock.Width * imgLocationRatioX;
                    int srcBlockImgHeight = curOrbitblock.Height * imgLocationRatioY;
                    Size srcBlockLocationSize = new Size(srcBlockJdWidth, srcBlockJdHeight);
                    Size srcBlockImgSize = new Size(srcBlockImgWidth, srcBlockImgHeight);
                    //亮温订正，天顶角修正：下面获取用到的部分经纬度和太阳高度角修正系数数据,下面修改为从临时文件直接读取。
                    float[] solarZenithData = null;
                    if (_isRadiation && _isSolarZenith)
                    {
                        if (_solarZenithCacheRaster != null)    //太阳天顶角数据
                            ReadBandData(out solarZenithData, _solarZenithCacheRaster, 1, curOrbitblock.xOffset, curOrbitblock.yBegin, srcBlockJdWidth, srcBlockJdHeight);
                        //亮温临边变暗订正,读取卫星天顶角数据。
                        //if (_isSensorZenith)
                        //    ReadBandData(out _sensorZenithData, _sensorSenithBand, curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height);
                        TryReadZenithData(curOrbitblock.xOffset, curOrbitblock.yBegin, curOrbitblock.Width, curOrbitblock.Height);
                    }

                    //计算当前分块的投影查算表
                    UInt16[] dstRowLookUpTable = new UInt16[bufferSize.Width * bufferSize.Height];
                    UInt16[] dstColLookUpTable = new UInt16[bufferSize.Width * bufferSize.Height];
                    if (imgLocationRatioX == 1)
                        _rasterProjector.ComputeIndexMapTable(srcBlockXs, srcBlockYs, srcBlockImgSize, bufferSize, blockEnvelope, _maxPrjEnvelope,
                            out dstRowLookUpTable, out dstColLookUpTable, null);
                    else
                        _rasterProjector.ComputeIndexMapTable(srcBlockXs, srcBlockYs, srcBlockLocationSize, srcBlockImgSize, bufferSize, blockEnvelope, //_maxPrjEnvelope,
                            out dstRowLookUpTable, out dstColLookUpTable, null, 0);
                    //执行投影
                    UInt16[] srcBandData = null;
                    UInt16[] dstBandData = new UInt16[bufferSize.Width * bufferSize.Height];
                    for (int i = 0; i < srcBands.Length; i++)  //读取原始通道值，投影到目标区域
                    {
                        if (progressCallback != null)
                        {
                            progress++;
                            percent = (int)(progress * 100 / progressCount);
                            progressCallback(percent, string.Format("投影完成{0}%", percent));
                        }
                        ReadImgBand(srcBands[i], curOrbitblock.xOffset * imgLocationRatioX, curOrbitblock.yBegin * imgLocationRatioY, srcBlockImgWidth, srcBlockImgHeight,
                            new Size(srcBlockImgWidth, srcBlockImgHeight), out srcBandData);
                        //ReadImgBand(out srcBandData, i, curOrbitblock.xOffset * imgLocationRatioX, curOrbitblock.yBegin * imgLocationRatioY, srcBlockImgWidth, srcBlockImgHeight);                        
                        Size angleSize = new Size(srcBlockJdWidth, srcBlockJdHeight);

                        DoRadiation(srcBands[i], srcBandData, solarZenithData, srcBlockImgSize, angleSize);

                        _rasterProjector.Project<UInt16>(srcBandData, srcBlockImgSize, dstRowLookUpTable, dstColLookUpTable, bufferSize, dstBandData, 0, null);
                        srcBandData = null;
                        IRasterBand band = dstBands[i];
                        unsafe
                        {
                            fixed (UInt16* ptr = dstBandData)
                            {
                                IntPtr bufferPtr = new IntPtr(ptr);
                                int blockOffsetY = blockHeight * blockYNo;
                                int blockOffsetX = blockWidth * blockXNo;
                                band.Write(blockOffsetX, blockOffsetY, blockWidth, blockHeight, bufferPtr, enumDataType.UInt16, bufferSize.Width, bufferSize.Height);
                            }
                        }
                    }
                    srcBandData = null;
                    dstBandData = null;
                    GC.Collect();
                    ReleaseZenithData();
                    Size srcBufferSize = new Size(srcBlockImgWidth, srcBlockImgHeight);
                    ProjectAngle(bufferSize, srcBufferSize, blockWidth, blockHeight, blockYNo, blockXNo, curOrbitblock, dstRowLookUpTable, dstColLookUpTable, progressCallback);
                    ProjectExtBands(bufferSize, srcBufferSize, blockWidth, blockHeight, blockYNo, blockXNo, curOrbitblock, dstRowLookUpTable, dstColLookUpTable, progressCallback);
                    dstRowLookUpTable = null;
                    dstColLookUpTable = null;
                }
            }
        }

        private void ReadBandNoValueMap(IRasterDataProvider srcRaster)
        {
            _bandNoValueMap = new float[5][];
            _bandNoValueMap[0] = ReadBandValueMap(srcRaster, "CALChannelIR1");
            _bandNoValueMap[1] = ReadBandValueMap(srcRaster, "CALChannelIR2");
            _bandNoValueMap[2] = ReadBandValueMap(srcRaster, "CALChannelIR3");
            _bandNoValueMap[3] = ReadBandValueMap(srcRaster, "CALChannelIR4");
            _bandNoValueMap[4] = ReadBandValueMap(srcRaster, "CALChannelVIS");
        }

        private float[] ReadBandValueMap(IRasterDataProvider srcRaster, string dataset)
        {
            float[] value = null;
            IRasterBand[] bands = srcRaster.BandProvider.GetBands(dataset);
            if (bands != null && bands.Length != 0)
            {
                Size size;
                ReadBandData(bands[0], out value, out size);
            }
            return value;
        }
        
        private float[][] _bandNoValueMap;

        private void ReadyLocations(IRasterDataProvider srcRaster, ISpatialReference dstSpatialRef, Size srcSize,
            out double[] xs, out double[] ys, out PrjEnvelope maxPrjEnvelope, Action<int, string> progressCallback)
        {
            if (progressCallback != null)
                progressCallback(1, "读取并插值经度数据集");
            Size locationSize;
            ReadLocations(srcRaster, out xs, out ys, out locationSize);
            TryResetLonlatForLeftRightInvalid(xs, ys, locationSize);
            _srcLocationSize = locationSize;
            if (xs == null || xs == null)
                throw new Exception("读取经纬度数据失败");
            if (progressCallback != null)
                progressCallback(3, "预处理经纬度数据集");
            _rasterProjector.ComputeDstEnvelope(_srcSpatialRef, xs, ys, srcSize, dstSpatialRef, out maxPrjEnvelope, progressCallback);
        }

        public override IRasterDataProvider Project(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, IRasterDataProvider dstRaster, int beginBandIndex, Action<int, string> progressCallback)
        {
            try
            {
                ReadyArgs(srcRaster, prjSettings, dstRaster.SpatialRef, progressCallback);
                string[] angleOptions = LdfOptions(prjSettings, dstRaster.SpatialRef, _outResolutionX, _outResolutionY, srcRaster.DataIdentify);
                ReadyAngleFiles(srcRaster, _outfilename, prjSettings, _dstSize, angleOptions);
                ProjectToLDF(srcRaster, dstRaster, beginBandIndex, progressCallback);
                return dstRaster;
            }
            catch (IOException ex)
            {
                if (ex.Message == "磁盘空间不足。\r\n" && File.Exists(_outfilename))
                    File.Delete(_outfilename);
                TryDeleteCurCatch();
                throw ex;
            }
            catch
            {
                EndSession();
                TryDeleteCurCatch();
                throw;
            }
            finally
            {
                if (dstRaster != null)
                {
                    dstRaster.Dispose();
                    dstRaster = null;
                }
                EndSession();
                if (prjSettings.IsClearPrjCache)
                    TryDeleteCurCatch();
            }
        }

        private void DoSession(IRasterDataProvider srcRaster, ISpatialReference dstSpatialRef, Fy2_NOM_PrjSettings prjSettings, Action<int, string> progressCallback)
        {
            if (_curSession == null || _curSession != srcRaster || _isBeginSession)
            {
                Size geoSize = new Size(srcRaster.Width, srcRaster.Height);
                ReadyLocations(srcRaster, dstSpatialRef, geoSize, out _xs, out _ys, out _maxPrjEnvelope, progressCallback);
                if (prjSettings.OutEnvelope == null)
                {
                    prjSettings.OutEnvelope = _maxPrjEnvelope;
                }
                if (progressCallback != null)
                    progressCallback(4, "准备亮温计算参数");
                if (progressCallback != null)
                    progressCallback(5, "准备亮温计算参数");
                if (prjSettings.IsSensorZenith)//执行临边变暗订正
                {
                    ReadySensorZenith(srcRaster);
                }
                //亮温值映射表
                ReadBandNoValueMap(srcRaster);
                _isBeginSession = false;
            }
        }

        private void ReadySensorZenith(IRasterDataProvider srcRaster)
        {
            _sensorSenithRaster = srcRaster;
            IRasterBand[] bands = srcRaster.BandProvider.GetBands("NOMSatelliteZenith");//卫星天顶角
            if (bands != null || bands.Length != 1)
                _sensorSenithBand = bands[0];
        }

        private void ReadyArgs(IRasterDataProvider srcRaster, FilePrjSettings prjSettings, ISpatialReference dstSpatialRef, Action<int, string> progressCallback)
        {
            if (dstSpatialRef == null)
                dstSpatialRef = _srcSpatialRef;
            if (string.IsNullOrWhiteSpace(prjSettings.OutFormat))
                prjSettings.OutFormat = "LDF";
            if (prjSettings.OutResolutionX == 0 || prjSettings.OutResolutionY == 0)
            {
                if (dstSpatialRef.ProjectionCoordSystem == null)
                {
                    prjSettings.OutResolutionX = 0.05F;
                    prjSettings.OutResolutionY = 0.05F;
                }
                else
                {
                    prjSettings.OutResolutionX = 5000F;
                    prjSettings.OutResolutionY = 5000F;
                }
            }
            if (prjSettings.OutBandNos == null || prjSettings.OutBandNos.Length == 0)
            {
                prjSettings.OutBandNos = new int[] { 1, 2, 3, 4, 5, 6 };
            }

            if (dstSpatialRef == null || dstSpatialRef.ProjectionCoordSystem == null)
                _srcImgResolution = 0.05f;
            else
                _srcImgResolution = 5000f;

            _prjSettings = prjSettings as Fy2_NOM_PrjSettings;
            ReadExtArgs(prjSettings);
            DoSession(srcRaster, dstSpatialRef, prjSettings as Fy2_NOM_PrjSettings, progressCallback);
            _isRadiation = _prjSettings.IsRadiation;
            _isSolarZenith = _prjSettings.IsSolarZenith;
            _isSensorZenith = _prjSettings.IsSensorZenith;

            _outResolutionX = prjSettings.OutResolutionX;
            _outResolutionY = prjSettings.OutResolutionY;
            _dstEnvelope = prjSettings.OutEnvelope;
            _dstBandCount = prjSettings.OutBandNos.Length;
            _outfilename = prjSettings.OutPathAndFileName;
            _dstSize = prjSettings.OutEnvelope.GetSize(_outResolutionX, _outResolutionY);
            _orbitBlock = new Block { xOffset = 0, yBegin = 0, xEnd = srcRaster.Width - 1, yEnd = srcRaster.Height - 1 };

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
            }

            _rasterDataBands = GetSrcRasterBand(srcRaster, prjSettings);
        }

        private void ReadExtArgs(FilePrjSettings prjSettings)
        {
            try
            {
                if (prjSettings.ExtArgs != null && prjSettings.ExtArgs.Length != 0)
                {
                    foreach (object arg in prjSettings.ExtArgs)
                    {
                        if (arg is Dictionary<string, double>)
                        {
                            Dictionary<string, double> exAtg = arg as Dictionary<string, double>;
                            if (exAtg.ContainsKey("GeoSlope"))
                                _geoSlope = exAtg["GeoSlope"];
                            if (exAtg.ContainsKey("GeoIntercept"))
                                _geoIntercept = exAtg["GeoIntercept"];
                        }
                    }
                }
            }
            catch
            {
            }
        }

        ///针对FY2E，投影波段，设置为1,2,3,4,5[NOMChannelVIS],6[NOMCloudClassification]
        private IRasterBand[] GetSrcRasterBand(IRasterDataProvider srcRaster, FilePrjSettings prjSettings)
        {
            IRasterBand[] bands = new IRasterBand[prjSettings.OutBandNos.Length];
            int[] bandNos = prjSettings.OutBandNos;
            if (bandNos.Length == 1 && bandNos[0] == 5)
            {
                IRasterBand[] bs = srcRaster.BandProvider.GetBands("NOMChannelVIS1KM");
                if (bs != null)
                {
                    bands[0] = bs[0];
                    bands[0].BandNo = 5;
                    _srcImgResolution = 0.01f;
                }
            }
            else
            {
                for (int i = 0; i < bandNos.Length; i++)
                {
                    if (bandNos[i] <= 4)
                        bands[i] = srcRaster.GetRasterBand(bandNos[i]);
                    else if (bandNos[i] == 5)
                    {
                        IRasterBand[] bs = srcRaster.BandProvider.GetBands("NOMChannelVIS1KM");
                        if (bs != null)
                        {
                            bands[i] = bs[0];
                            bands[i].BandNo = 5;
                        }
                    }
                    else if (bandNos[i] == 6)
                    {
                        IRasterBand[] bs = srcRaster.BandProvider.GetBands("NOMCloudClassification");
                        if (bs != null)
                        {
                            bands[i] = bs[0];
                            bands[i].BandNo = 6;
                        }
                    }
                }
            }
            return bands;
        }

        private IRasterBand[] GetDstRasterBand(IRasterDataProvider dstRaster, FilePrjSettings prjSettings, int beginBandIndex)
        {
            IRasterBand[] bands = new IRasterBand[prjSettings.OutBandNos.Length];
            int[] bandNos = prjSettings.OutBandNos;
            for (int i = beginBandIndex; i < bandNos.Length + beginBandIndex; i++)
            {
                if (dstRaster is IBandNameRaster)
                {
                    int newBand = -1;
                    if ((dstRaster as IBandNameRaster).TryGetBandNoFromBandName(bandNos[i], out newBand))
                        bands[i] = dstRaster.GetRasterBand(newBand);
                    else
                        bands[i] = dstRaster.GetRasterBand(bandNos[i]);

                }
                else
                    bands[i] = dstRaster.GetRasterBand(bandNos[i]);
            }
            return bands;
        }

        private IRasterDataProvider CreateOutFile(FilePrjSettings prjSettings, ISpatialReference dstSpatial, DataIdentify dataIdentify)
        {
            float resolutionX = prjSettings.OutResolutionX;
            float resolutionY = prjSettings.OutResolutionY;
            string[] options = null;
            string outformat = string.IsNullOrWhiteSpace(prjSettings.OutFormat) ? ".ldf" : prjSettings.OutFormat.ToLower();
            string driver = "";
            if (outformat == ".tif" || outformat == ".tiff")
            {
                driver = "GDAL";
                options = new string[]{
                    "DRIVERNAME=GTiff",
                    "TFW=YES",
                    "WKT=" + dstSpatial.ToWKTString(),
                    "GEOTRANSFORM=" + string.Format("{0},{1},{2},{3},{4},{5}",prjSettings.OutEnvelope.MinX, resolutionX,0, prjSettings.OutEnvelope.MaxY,0, -resolutionY)
                    };
            }
            else
            {
                driver = "LDF";
                options = LdfOptions(prjSettings, dstSpatial, resolutionX, resolutionY, dataIdentify);
                List<string> opts = new List<string>(options);
                opts.Add("BANDNAMES=" + BandNameString(prjSettings.OutBandNos));
                options = opts.ToArray();

            }
            Size outSize = prjSettings.OutEnvelope.GetSize(resolutionX, resolutionY);
            int bandCount = prjSettings.OutBandNos.Length;
            string filename = prjSettings.OutPathAndFileName;
            return CreateOutFile(driver, filename, bandCount, outSize, enumDataType.Int16, options);
        }

        private string[] LdfOptions(FilePrjSettings prjSettings, ISpatialReference dstSpatial, float resolutionX, float resolutionY, DataIdentify dataIdentify)
        {
            List<string> options = new List<string>();
            options.Add("INTERLEAVE=BSQ");
            options.Add("VERSION=LDF");
            options.Add("SPATIALREF=" + dstSpatial.ToProj4String());
            options.Add("MAPINFO={" + 1 + "," + 1 + "}:{" + prjSettings.OutEnvelope.MinX + "," + prjSettings.OutEnvelope.MaxY + "}:{" + resolutionX + "," + resolutionY + "}");
            options.Add("SENSOR=NOM");
            if (dataIdentify != null)
            {
                string satellite = dataIdentify.Satellite;
                DateTime dt = dataIdentify.OrbitDateTime;
                bool asc = dataIdentify.IsAscOrbitDirection;
                if (!string.IsNullOrWhiteSpace(satellite))
                {
                    options.Add("SATELLITE=" + satellite);
                }
                if (dt != DateTime.MinValue && dt != DateTime.MaxValue)
                    options.Add("DATETIME=" + dt.ToString("yyyy/MM/dd HH:mm"));
            }
            return options.ToArray();
        }

        internal IRasterDataProvider CreateOutFile(string driver, string outfilename, int dstBandCount, Size outSize, enumDataType dataType, string[] options)
        {
            CheckAndCreateDir(Path.GetDirectoryName(outfilename));
            IRasterDataDriver outdrv = GeoDataDriver.GetDriverByName(driver) as IRasterDataDriver;
            return outdrv.Create(outfilename, outSize.Width, outSize.Height, dstBandCount, dataType, options) as IRasterDataProvider;
        }

        internal override void ReadAgileBand(IRasterBand band, int xOffset, int yOffset, int xSize, int ySize, Size bufferSize, out short[] bandData)
        {
            float[] buffeData = new float[bufferSize.Width * bufferSize.Height];
            unsafe
            {
                fixed (float* ptr = buffeData)
                {
                    IntPtr bufferptr = new IntPtr(ptr);
                    band.Read(xOffset, yOffset, xSize, ySize, bufferptr, enumDataType.Float, bufferSize.Width, bufferSize.Height);
                }
            }
            bandData = new short[bufferSize.Width * bufferSize.Height];
            for (int i = 0; i < buffeData.Length; i++)
            {
                bandData[i] = (short)(buffeData[i] * 5729.577951308232);//弧度转角度Radians = Degrees * Pi/ 180;
            }
        }

        internal short[] ReadRanToDeg(IRasterBand band, int xOffset, int yOffset, int blockWidth, int blockHeight)
        {
            if (band == null)
                return null;
            try
            {
                float[] bufferData = new float[blockWidth * blockHeight];
                unsafe
                {
                    fixed (float* ptr = bufferData)
                    {
                        IntPtr bufferptr = new IntPtr(ptr);
                        band.Read(xOffset, yOffset, blockWidth, blockHeight, bufferptr, enumDataType.Float, blockWidth, blockHeight);
                    }
                }
                short[] bandData = new short[blockWidth * blockHeight];
                for (int i = 0; i < bufferData.Length; i++)
                {
                    bandData[i] = (short)(bufferData[i] * 5729.577951308232);//弧度转角度Radians = Degrees * Pi/ 180;
                }
                return bandData;
            }
            finally
            {
            }
        }
    }
}
