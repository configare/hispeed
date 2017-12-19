using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.VectorDrawing;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 以输出为驱动的数据访问逻辑
    /// 和序列栅格数据计算模型
    /// </summary>
    public class RasterProcessModel<Tsrc, Tdst>
    {
        private RasterMaper[] _fileIn;
        private RasterMaper[] _fileOut;
        /// 偏移量,输入文件相对输出文件的。每个输入文件计算四个偏移量，起始行、起始列、结束行、结束列
        /// 初始化输入输出文件集时候自动计算好。
        private RasterOffset[] _fileInOffsets = null;
        private RasterOffset[] _fileOutOffsets = null;
        private Action<int, string> _progress = null;
        private RasterCalcHandler<Tsrc, Tdst> _calcModle = null;
        private RasterCalcHandlerFun<Tsrc, Tdst> _calcModleFun = null;
        private string _aoiValue;
        private string _aoiType;
        private int[] _aoiIndex;
        private int[] _aoiIndexValue = null;
        private Feature[] _aoiFeatures = null;
        private VirtualRaster[] _vRasterIn = null;
        private object _aoiContainer = null;


        public RasterProcessModel()
        { }

        public RasterProcessModel(Action<int, string> progress)
        {
            _progress = progress;
        }

        public void RegisterCalcModel(RasterCalcHandler<Tsrc, Tdst> calcModle)
        {
            _calcModle = calcModle;
        }

        public void RegisterCalcModel(RasterCalcHandlerFun<Tsrc, Tdst> calcModle)
        {
            _calcModleFun = calcModle;
        }

        /// <summary>
        /// 基于矢量界面ArgumentProvider的Aoi定义
        /// </summary>
        /// <param name="feature"></param>
        public void SetArgumentProviderAOI(int[] aoiIndex)
        {
            _aoiType = "AOIIndex";
            _aoiIndexValue = aoiIndex;
        }

        public void SetFeatureAOI(Feature[] features)
        {
            _aoiType = "features";
            _aoiFeatures = features;
        }

        /// <summary>
        /// 设置处理对象（输入和输出）
        /// 输入和输出文件范围可以不一致，但是必须投影方式一样，分辨率一致。
        /// </summary>
        /// <param name="fileIn">一组创建好的输入对象，具有范围定义（每个文件的范围可以不一致,暂时要求分辨率和输出文件保持一致）</param>
        /// <param name="fileOut">一组创建好的输出对象，具有范围定义（每个文件的范围,大小，分辨率必须一致）</param>
        public void SetRaster(RasterMaper[] fileIn, RasterMaper[] fileOut)
        {
            if (fileOut == null || fileIn == null || fileIn.Length == 0 || fileOut.Length == 0)//获取为空时候使用其他逻辑，比如输入的并集，但我觉得还是放在外面好。
                return;
            //1、检查所有文件投影坐标都一致（这里暂时不考虑栅格坐标）

            //2、检查所有输出文件范围、坐标、分辨率等都一致

            //3、计算每个输入文件的偏移（相对输出文件）
            _fileInOffsets = new RasterOffset[fileIn.Length];
            _fileOutOffsets = new RasterOffset[fileIn.Length];
            for (int i = 0; i < fileIn.Length; i++)
            {
                RasterOffset fileOffset;
                RasterOffset fileOutOffset;
                CalcFileOffset(fileIn[i].Raster, fileOut[0].Raster, out fileOffset, out fileOutOffset);
                _fileInOffsets[i] = fileOffset;
                _fileOutOffsets[i] = fileOutOffset;
            }
            //4、设置通道映射表
            CheckBandMap(fileIn);
            CheckBandMap(fileOut);
            _fileIn = fileIn;
            _fileOut = fileOut;

            //建立虚拟栅格
            VirtualRasterHeader vHeader = VirtualRasterHeader.Create(fileOut[0].Raster);
            VirtualRaster[] vRasters = new VirtualRaster[fileIn.Length];
            for (int i = 0; i < fileIn.Length; i++)
            {
                VirtualRaster vr = new VirtualRaster(fileIn[i].Raster, vHeader);
                vRasters[i] = vr;
            }
            _vRasterIn = vRasters;
        }

        /// <summary>
        /// 【暂未实现】
        /// 基于矢量的Aoi定义，与方法SetTemplateAOI互斥
        /// 格式:{"filename"}
        /// </summary>
        /// <param name="feature"></param>
        public void SetFeatureAOI(string feature)
        {
            _aoiType = "feature";
            _aoiValue = feature;
        }

        /// <summary>
        /// 设置已定义的AOI，来源于AOITemplateFactory.TemplateNames，与方法SetFeatureAOI互斥
        /// 如：
        /// "vector:太湖"
        /// "vector:海陆模版"
        /// "raster:行政区划"
        /// </summary>
        /// <param name="feature"></param>
        public void SetTemplateAOI(string templateName)
        {
            if (!string.IsNullOrWhiteSpace(templateName))
            {
                _aoiType = "template";
                _aoiValue = templateName;
            }
        }

        public void SetCustomAOI(string aoiName, object aoiContainer)
        {
            if (!string.IsNullOrWhiteSpace(aoiName))
            {
                _aoiType = "custom";
                _aoiValue = aoiName;
                _aoiContainer = aoiContainer;
            }
        }

        private void CheckBandMap(RasterMaper[] fileMap)
        {
            for (int i = 0; i < fileMap.Length; i++)
            {
                if (fileMap[i].BandMap == null || fileMap[i].BandMap.Length == 0)
                {
                    int bandCount = fileMap[i].Raster.BandCount;
                    int[] bands = new int[bandCount];
                    for (int j = 0; j < bandCount; j++)
                    {
                        bands[j] = j + 1;
                    }
                    fileMap[i].BandMap = bands;
                }
            }
        }

        #region 偏移量计算
        /// <summary>
        /// 计算inputRaster相对outputRaster的偏移。
        /// </summary>
        /// <param name="inputRaster"></param>
        /// <param name="outputRaster"></param>
        /// <param name="foIn"></param>
        /// <param name="foOut"></param>
        private void CalcFileOffset(IRasterDataProvider inputRaster, IRasterDataProvider outputRaster, out RasterOffset foIn, out RasterOffset foOut)
        {
            int tBeginRow = -1, tEndRow = -1, tBeginCol = -1, tEndCol = -1;
            int oBeginRow = -1, oEndRow = -1, oBeginCol = -1, oEndCol = -1;
            CoordEnvelope oEnvelope = inputRaster.CoordEnvelope;
            CoordEnvelope tEnvelope = outputRaster.CoordEnvelope;
            Size oSize = new Size(inputRaster.Width, inputRaster.Height);
            Size tSize = new Size(outputRaster.Width, outputRaster.Height);
            bool isInternal = RasterOffsetHelper.ComputeBeginEndRowCol(oEnvelope, oSize, tEnvelope, tSize,
                ref oBeginRow, ref oBeginCol, ref oEndRow, ref oEndCol,
                ref tBeginRow, ref tBeginCol, ref tEndRow, ref tEndCol);
            if (!isInternal)//不想交
            {
                foIn = null;
                foOut = null;
                return;
            }
            foIn = new RasterOffset();
            foIn.BeginX = oBeginCol;
            foIn.BeginY = oBeginRow;
            foIn.XSize = oEndCol - oBeginCol;
            foIn.YSize = oEndRow - oBeginRow;
            foOut = new RasterOffset();
            foOut.BeginX = tBeginCol;
            foOut.BeginY = tBeginRow;
            foOut.XSize = tEndCol - tBeginCol;
            foOut.YSize = tEndRow - tBeginRow;
        }

        #endregion

        /// <summary>
        /// 自动分行执行
        /// </summary>
        public void Excute()
        {
            Excute(default(Tsrc));
        }

        /// <summary>
        /// 自动分行执行
        /// </summary>
        /// <param name="fillValue">指定读取非相交区域数据填充值</param>
        public void Excute(Tsrc nullValue)
        {
            Excute(nullValue, 0);
        }

        /// <summary>
        /// 自动分行执行
        /// </summary>
        /// <param name="fillValue">指定读取非相交区域数据填充值</param>
        public void Excute(Tsrc nullValue, int rowCount)
        {
            try
            {
                //int height = _fileIn[0].Raster.Height;
                int height = _vRasterIn[0].VirtualHeader.Height;
                if (rowCount <= 0 || rowCount > height)
                    rowCount = CalcRowCount();
                int step = (int)Math.Ceiling(height * 1d / rowCount);
                float stepCounter = 0f;
                for (int i = 0; i < height; i += rowCount)
                {
                    if (_progress != null)
                        _progress((int)(((stepCounter++) / step) * 100), string.Format("{0}/{1}", stepCounter, step));
                    if (i >= height)
                        break;
                    if (i + rowCount > height)
                        rowCount = height - i;
                    ExcuteMultiRow(i, rowCount, nullValue);
                }
            }
            finally
            {
                Free();
            }
        }

        /// <summary>
        /// 2013
        /// </summary>
        /// <returns></returns>
        public int CalcRowCount()
        {
            int allByteBandCount = 2 * DataTypeHelper.SizeOf(_fileOut[0].Raster.DataType);//这一个波段为写出时需要申请的内存
            foreach (RasterMaper ras in _fileIn)
            {
                allByteBandCount += ras.Raster.BandCount * DataTypeHelper.SizeOf(ras.Raster.DataType);
            }
            foreach (RasterMaper ras in _fileOut)
            {
                allByteBandCount += ras.Raster.BandCount * DataTypeHelper.SizeOf(ras.Raster.DataType);
            }
            int typeSize = DataTypeHelper.SizeOf(_fileOut[0].Raster.DataType);
            long maxlong = _vRasterIn[0].VirtualHeader.Width * _vRasterIn[0].VirtualHeader.Height;
            long arrayMax = GetMaxArray(allByteBandCount / typeSize, maxlong);//可申请的最大数组大小

            arrayMax = arrayMax / typeSize;
            int rowCount = (int)(arrayMax / _vRasterIn[0].VirtualHeader.Width);
            if (rowCount > _vRasterIn[0].VirtualHeader.Height)
                rowCount = _vRasterIn[0].VirtualHeader.Height;
            return rowCount;

        }

        /// <summary>
        /// 当前可以申请的最大byte数组的大小
        /// </summary>
        /// <param name="arrayCount">byte数组个数</param>
        /// <param name="maxlong">期望的数据大小</param>
        /// <returns></returns>
        public static long GetMaxArray(int arrayCount, long maxlong)
        {
            long memLong = System.GC.GetTotalMemory(true);
            bool sessory = false;
            memLong = int.MaxValue / 2;
            if (memLong > maxlong)
                memLong = maxlong;
            int count = 0;
            double s = -1;
            byte[][] mems = new byte[arrayCount][];
            while (!sessory)
            {
                try
                {
                    for (int i = 0; i < arrayCount; i++)
                    {
                        mems[i] = new byte[memLong];
                        memLong = mems[i].LongLength;
                        s = mems[i][memLong - 1];
                    }
                    sessory = true;
                }
                catch
                {
                    memLong = memLong * 4 / 5;
                    count++;
                    sessory = false;
                }
            }
            Console.WriteLine(string.Format("catch {0} times", count));
            Console.WriteLine(string.Format("can use mem {0}*{1}byte,{2}MB", arrayCount, memLong, arrayCount * memLong / 1048576f));
            return memLong;
        }

        private void ExcuteMultiRow(int rowIndex, int rowCount, Tsrc nullValue)
        {
            //初始化AOI
            TrySetAOI(rowIndex, rowCount);
            //初始化数据访问器
            RasterVirtualVistor<Tsrc>[] fileVistor = new RasterVirtualVistor<Tsrc>[_fileIn.Length];
            RasterVirtualVistor<Tdst>[] fileOutVistor = new RasterVirtualVistor<Tdst>[_fileOut.Length];
            ReadInputData(rowIndex, rowCount, fileVistor, nullValue);
            ReadOutputData(rowIndex, rowCount, fileOutVistor);

            //处理逻辑
            if (_calcModle != null)
                _calcModle(fileVistor, fileOutVistor, _aoiIndex);
            bool isSave = true;
            if (_calcModleFun != null)
                isSave = _calcModleFun(fileVistor, fileOutVistor, _aoiIndex);
            //更新输出数据
            if (isSave)
                UpdateOutData(fileOutVistor, rowCount);
        }

        private void TrySetAOI(int rowIndex, int rowCount)
        {
            if (string.IsNullOrWhiteSpace(_aoiType) || string.IsNullOrWhiteSpace(_aoiValue))
                _aoiIndex = null;
            if (_aoiType == "template")
            {
                _aoiIndex = TryGetAoiFromTemplate(rowIndex, rowCount);
            }
            else if (_aoiType == "feature")
            {
                _aoiIndex = TryGetAoiFromFeature(rowIndex, rowCount);
            }
            else if (_aoiType == "custom")
            {
                _aoiIndex = TryGetAoiFromCustom(rowIndex, rowCount);
            }
            else if (_aoiType == "features")
            {
                _aoiIndex = TryGetAoiFromFeatures(rowIndex, rowCount);
            }
            else if (_aoiType == "AOIIndex")
            {
                _aoiIndex = TryGetAoiFromAOIIndex(rowIndex, rowCount);
            }
            else
                _aoiIndex = null;
        }

        private int[] TryGetAoiFromAOIIndex(int rowIndex, int rowCount)
        {
            if (_aoiIndexValue == null)
                return null;
            int startIndex = _fileOut[0].Raster.Width * rowIndex;
            int endIndex = _fileOut[0].Raster.Width * (rowIndex + rowCount - 1);
            if (_aoiIndexValue[0] > endIndex || _aoiIndexValue[_aoiIndexValue.Length - 1] < startIndex)
                return null;
            List<int> aoiIndex = new List<int>();
            for (int i = startIndex; i < endIndex; i++)
                aoiIndex.Add(i);
            var resultTemp = _aoiIndexValue.Intersect(aoiIndex.ToArray());
            if (resultTemp == null || resultTemp.Count() == 0)
                return null;
            int[] result = resultTemp.ToArray();
            if (startIndex != 0)
                for (int i = 0; i < result.Length; i++)
                    result[i] -= startIndex;
            return result;
        }

        private int[] GetMoreTemplateAOI(double minLon, double maxLon, double minLat, double maxLat, Size outSize)
        {
            string[] templateAOIS = _aoiValue.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            List<int> aoiIndex = new List<int>();
            int[] aoitemp = null;
            foreach (string aoi in templateAOIS)
            {
                aoitemp = AOITemplateFactory.MakeAOI(aoi, minLon, maxLon, minLat, maxLat, outSize);
                if (aoitemp != null)
                    aoiIndex.AddRange(aoitemp);
            }
            return aoiIndex.Count == 0 ? null : aoiIndex.ToArray();
        }

        private int[] TryGetAoiFromFeatures(int rowIndex, int rowCount)
        {
            double minLon = _fileOut[0].Raster.CoordEnvelope.MinX;
            double maxLon = _fileOut[0].Raster.CoordEnvelope.MaxX;
            double minLat = _fileOut[0].Raster.CoordEnvelope.MaxY - (rowIndex + rowCount) * _fileOut[0].Raster.ResolutionY;
            double maxLat = _fileOut[0].Raster.CoordEnvelope.MaxY - rowIndex * _fileOut[0].Raster.ResolutionY;
            Size outSize = new Size(_fileOut[0].Raster.Width, rowCount);
            int[] aoiIndex = AOITemplateFactory.MakeAOI(_aoiFeatures, minLon, maxLon, minLat, maxLat, outSize);
            return aoiIndex;
        }

        private int[] TryGetAoiFromCustom(int rowIndex, int rowCount)
        {
            int[] aoiIndex = null;
            if (_aoiContainer != null)
            {
                double minLon = _fileOut[0].Raster.CoordEnvelope.MinX;
                double maxLon = _fileOut[0].Raster.CoordEnvelope.MaxX;
                double minLat = _fileOut[0].Raster.CoordEnvelope.MaxY - (rowIndex + rowCount) * _fileIn[0].Raster.ResolutionY;
                double maxLat = _fileOut[0].Raster.CoordEnvelope.MaxY - rowIndex * _fileIn[0].Raster.ResolutionY;
                Size outSize = new Size(_fileIn[0].Raster.Width, rowCount);
                CoordEnvelope rowEnv = new CoordEnvelope(minLon, maxLon, minLat, maxLat);
                if ((_aoiContainer as AOIContainerLayer) != null)
                {
                    aoiIndex = GetAOI(rowEnv, (_aoiContainer as AOIContainerLayer), outSize);
                }
                else if ((_aoiContainer as CoordEnvelope) != null)
                {
                    aoiIndex = GetAOI(rowEnv, (_aoiContainer as CoordEnvelope), outSize);
                }
            }
            return aoiIndex;
        }

        public  int[] GetAOI(CoordEnvelope fileCorEnv, AOIContainerLayer aoiContainer, Size fileSize)
        {
            int[] retAOI = null, aoi = null;
            VectorAOIGenerator vg = new VectorAOIGenerator();
            Envelope fileEnv = new Envelope(fileCorEnv.MinX, fileCorEnv.MinY, fileCorEnv.MaxX, fileCorEnv.MaxY);
            foreach (object obj in aoiContainer.AOIs)
            {
                try
                {
                    aoi = vg.GetAOI(new ShapePolygon[] { (obj as Feature).Geometry as ShapePolygon }, fileEnv, fileSize);
                    if (aoi == null)
                        continue;
                    if (retAOI == null)
                        retAOI = aoi;
                    else
                        retAOI = GeoDo.RSS.RasterTools.AOIHelper.Merge(new int[][] { retAOI, aoi });
                }
                catch (System.Exception ex)
                {
                    continue;
                }
            }
            return retAOI;
        }

        public int[] GetAOI(CoordEnvelope cordenv, CoordEnvelope subenv, Size fileSize)
        {
            int[] retAOI = null;
            if (cordenv != null && cordenv.Width > 0 && cordenv.Height > 0)
            {
                    if (subenv != null && subenv.Width > 0 && subenv.Height > 0)
                    {
                        IRasterDataProvider dataPrd = _fileIn[0].Raster;
                        int xoffset = (int)((subenv.MinX - cordenv.MinX) / dataPrd.ResolutionX + 0.5);
                        int yoffset = (int)((cordenv.MaxY - subenv.MaxY) / dataPrd.ResolutionY + 0.5);
                        if (xoffset < 0)
                            xoffset = 0;
                        if (yoffset < 0)
                            yoffset = 0;
                        if (xoffset > dataPrd.Width)
                            xoffset = dataPrd.Width;
                        if (yoffset > dataPrd.Height)
                            yoffset = dataPrd.Height;
                        int width = (int)(subenv.Width / dataPrd.ResolutionY + 0.5);
                        int height = (int)(subenv.Height / dataPrd.ResolutionY + 0.5);
                        int i, j;
                        retAOI = new int[width * height];
                        for (i = 0; i < height; i++)
                        {
                            for (j = 0; j < width; j++)
                            {
                                retAOI[i * width + j] = dataPrd.Width * (yoffset + i) + (xoffset + j);
                            }
                        }
                    }
            }
            return retAOI;
        }

        private int[] TryGetAoiFromTemplate(int rowIndex, int rowCount)
        {
            double minLon = _fileOut[0].Raster.CoordEnvelope.MinX;
            double maxLon = _fileOut[0].Raster.CoordEnvelope.MaxX;
            double minLat = _fileOut[0].Raster.CoordEnvelope.MaxY - (rowIndex + rowCount) * _fileOut[0].Raster.ResolutionY;
            double maxLat = _fileOut[0].Raster.CoordEnvelope.MaxY - rowIndex * _fileOut[0].Raster.ResolutionY;
            Size outSize = new Size(_fileOut[0].Raster.Width, rowCount);
            int[] aoiIndex = null;
            if (_aoiValue.Contains(";") && _aoiValue.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Length != 1)
                aoiIndex = GetMoreTemplateAOI(minLon, maxLon, minLat, maxLat, outSize);
            else
                aoiIndex = AOITemplateFactory.MakeAOI(_aoiValue, minLon, maxLon, minLat, maxLat, outSize);
            return aoiIndex;
        }

        //private int[] TryGetAoiFromTemplate(int rowIndex, int rowCount)
        //{
        //    double minLon = _fileOut[0].Raster.CoordEnvelope.MinX;
        //    double maxLon = _fileOut[0].Raster.CoordEnvelope.MaxX;
        //    double minLat = _fileOut[0].Raster.CoordEnvelope.MaxY - (rowIndex + rowCount) * _fileOut[0].Raster.ResolutionY;
        //    double maxLat = _fileOut[0].Raster.CoordEnvelope.MaxY - rowIndex * _fileOut[0].Raster.ResolutionY;
        //    Size outSize = new Size(_fileOut[0].Raster.Width, rowCount);
        //    int[] aoiIndex = AOITemplateFactory.MakeAOI(_aoiValue, minLon, maxLon, minLat, maxLat, outSize);
        //    return aoiIndex;
        //}
        
        private int[] TryGetAoiFromFeature(int rowIndex, int rowCount)
        {
            return null;
        }

        private void ReadInputData(int outRow, int rowCount, RasterVirtualVistor<Tsrc>[] fileVistor, Tsrc nullValue)
        {
            for (int i = 0; i < _fileIn.Length; i++)
            {
                RasterVirtualVistor<Tsrc> fv = new RasterVirtualVistor<Tsrc>();
                fv.Raster = _fileIn[i].Raster;
                fv.BandMap = _fileIn[i].BandMap;
                fv.IndexY = outRow;
                fv.SizeY = rowCount;
                fv.SizeX = _fileIn[i].Raster.Width;
                //
                int[] bandMap = _fileIn[i].BandMap;
                IRasterBand[] bands = new RasterBand[bandMap.Length];
                for (int b = 0; b < bandMap.Length; b++)
                {
                    bands[b] = _fileIn[i].Raster.GetRasterBand(bandMap[b]);
                }
                fv.Bands = bands;
                Tsrc[][] bandsVirtureData = new Tsrc[bands.Length][];
                
                for (int j = 0; j < bands.Length; j++)
                {
                    int bandNo = bandMap[j];
                    Tsrc[] dstVirtualData = _vRasterIn[i].ReadData<Tsrc>(bandNo, 0, outRow, _vRasterIn[i].VirtualHeader.Width, rowCount, nullValue);
                    bandsVirtureData[j] = dstVirtualData;
                }
                fv.RasterBandsData = bandsVirtureData;
                fileVistor[i] = fv;
            }
        }

        internal void ReadOutputData(int outRow, int rowCount, RasterVirtualVistor<Tdst>[] fileVistor)
        {
            for (int i = 0; i < _fileOut.Length; i++)
            {
                RasterVirtualVistor<Tdst> fv = new RasterVirtualVistor<Tdst>();
                fv.BandMap = _fileOut[i].BandMap;
                fv.IndexY = outRow;
                fv.SizeX = _fileOut[i].Raster.Width;
                fv.SizeY = rowCount;
                fv.Raster = _fileOut[i].Raster;
                int[] bandMap = _fileOut[i].BandMap;
                IRasterBand[] bands = new RasterBand[bandMap.Length];
                for (int b = 0; b < bandMap.Length; b++)
                {
                    bands[b] = _fileOut[i].Raster.GetRasterBand(bandMap[b]);
                }
                fv.Bands = bands;
                //初始化源数据访问
                int outWidth = _vRasterIn[0].VirtualHeader.Width;
                int readOffsetx = 0;
                int readOffsetY = outRow;
                int xSize = outWidth;
                int ySize = rowCount;
                enumDataType type = _fileOut[i].Raster.DataType;
                //if (type == enumDataType.Int16)
                //{
                    Tdst[][] bandsVirtureData = new Tdst[bands.Length][];
                    for (int b = 0; b < bands.Length; b++)
                    {
                        Tdst[] bufferData = new Tdst[xSize * ySize];
                        GCHandle buffer = GetHandles(bufferData);
                        bands[b].Read(readOffsetx, readOffsetY, xSize, ySize, buffer.AddrOfPinnedObject(), type, xSize, ySize);
                        buffer.Free();
                        bandsVirtureData[b] = bufferData;
                    }
                    fv.RasterBandsData = bandsVirtureData;
                //}
                fileVistor[i] = fv;
            }
        }

        internal void UpdateOutData(RasterVirtualVistor<Tdst>[] fileOutVistor, int rowCount)
        {
            for (int i = 0; i < fileOutVistor.Length; i++)
            {
                IRasterDataProvider raster = fileOutVistor[i].Raster;
                int row = fileOutVistor[i].IndexY;
                int[] bandMap = fileOutVistor[i].BandMap;
                IRasterBand[] bands = fileOutVistor[i].Bands;
                Tdst[][] bandsData = fileOutVistor[i].RasterBandsData;
                int readOffsetx = 0;
                int readOffsetY = row;
                int xSize = raster.Width;
                int ySize = rowCount;
                for (int b = 0; b < bands.Length; b++)
                {
                    IRasterBand band = bands[b];
                    enumDataType type = band.DataType;
                    GCHandle buffer = GetHandles(bandsData[b]);
                    try
                    {
                        band.Write(readOffsetx, readOffsetY, xSize, ySize, buffer.AddrOfPinnedObject(), type, xSize, ySize);
                    }
                    finally
                    {
                        buffer.Free();
                    }
                }
            }
        }

        internal GCHandle GetHandles(Tsrc[] virtureInData)
        {
            return GCHandle.Alloc(virtureInData, GCHandleType.Pinned);
        }

        internal GCHandle GetHandles(Tdst[] virtureInData)
        {
            return GCHandle.Alloc(virtureInData, GCHandleType.Pinned);
        }

        private void Free()
        {
            _calcModle = null;
            _aoiType = "";
            _aoiValue = "";
            _fileIn = null;
            _fileOut = null;
        }
    }

    public class RasterVirtualVistor2<T>
    {
        public IRasterDataProvider Raster { get; internal set; }
        public T[][] RasterBandsData { get; internal set; }
        public int[] BandMap { get; internal set; }
        public IRasterBand[] Bands { get; internal set; }
        public int IndexY { get; internal set; }
        public int SizeY { get; internal set; }
        public int SizeX { get; internal set; }

        public void Dispose()
        {
            RasterBandsData = null;
        }
    }
}
