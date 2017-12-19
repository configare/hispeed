using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;
using System.Drawing;

namespace GeoDo.RSS.MIF.Core
{
    ///// <summary>
    ///// 以输出为驱动的数据访问逻辑
    ///// 和序列栅格数据计算模型
    ///// </summary>
    //public class RasterProcessModel_old<Tsrc, Tdst>
    //{
    //    private RasterMaper[] _fileIn;
    //    private RasterMaper[] _fileOut;
    //    /// 偏移量,输入文件相对输出文件的。每个输入文件计算四个偏移量，起始行、起始列、结束行、结束列
    //    /// 初始化输入输出文件集时候自动计算好。
    //    private RasterOffset[] _fileInOffsets = null;
    //    private RasterOffset[] _fileOutOffsets = null;
    //    private Action<int, string> _progress = null;
    //    private RasterCalcHandler<Tsrc, Tdst> _calcModle = null;
    //    private string _aoiValue;
    //    private string _aoiType;
    //    private int[] _aoiIndex;

    //    public RasterProcessModel_old()
    //    { }

    //    public RasterProcessModel_old(Action<int, string> progress)
    //    {
    //        _progress = progress;
    //    }

    //    public void RegisterCalcModel(RasterCalcHandler<Tsrc, Tdst> calcModle)
    //    {
    //        _calcModle = calcModle;
    //    }

    //    /// <summary>
    //    /// 设置处理对象（输入和输出）
    //    /// 输入和输出文件范围可以不一致，但是必须投影方式一样，分辨率一致。
    //    /// </summary>
    //    /// <param name="fileIn">一组创建好的输入对象，具有范围定义（每个文件的范围可以不一致,暂时要求分辨率和输出文件保持一致）</param>
    //    /// <param name="fileOut">一组创建好的输出对象，具有范围定义（每个文件的范围,大小，分辨率必须一致）</param>
    //    public void SetRaster(RasterMaper[] fileIn, RasterMaper[] fileOut)
    //    {
    //        if (fileOut == null || fileIn == null || fileIn.Length == 0 || fileOut.Length == 0)//获取为空时候使用其他逻辑，比如输入的并集，但我觉得还是放在外面好。
    //            return;
    //        //1、检查所有文件投影坐标都一致（这里暂时不考虑栅格坐标）

    //        //2、检查所有输出文件范围、坐标等都一致

    //        //3、计算每个输入文件的偏移（相对输出文件）
    //        _fileInOffsets = new RasterOffset[fileIn.Length];
    //        _fileOutOffsets = new RasterOffset[fileIn.Length];
    //        for (int i = 0; i < fileIn.Length; i++)
    //        {
    //            RasterOffset fileOffset;
    //            RasterOffset fileOutOffset;
    //            CalcFileOffset(fileIn[i].Raster, fileOut[0].Raster, out fileOffset, out fileOutOffset);
    //            _fileInOffsets[i] = fileOffset;
    //            _fileOutOffsets[i] = fileOutOffset;
    //        }
    //        //4、设置通道映射表
    //        CheckBandMap(fileIn);
    //        CheckBandMap(fileOut);
    //        _fileIn = fileIn;
    //        _fileOut = fileOut;
    //    }

    //    /// <summary>
    //    /// 【暂未实现】
    //    /// 基于矢量的Aoi定义，与方法SetTemplateAOI互斥
    //    /// 格式:{"filename"}
    //    /// </summary>
    //    /// <param name="feature"></param>
    //    public void SetFeatureAOI(string feature)
    //    {
    //        _aoiType = "feature";
    //        _aoiValue = feature;
    //    }

    //    /// <summary>
    //    /// 设置已定义的AOI，来源于AOITemplateFactory.TemplateNames，与方法SetFeatureAOI互斥
    //    /// 如：
    //    /// "vector:太湖"
    //    /// "vector:海陆模版"
    //    /// "raster:行政区划"
    //    /// </summary>
    //    /// <param name="feature"></param>
    //    public void SetTemplateAOI(string templateName)
    //    {
    //        if (!string.IsNullOrWhiteSpace(templateName))
    //        {
    //            _aoiType = "template";
    //            _aoiValue = templateName;
    //        }
    //    }

    //    private void CheckBandMap(RasterMaper[] fileMap)
    //    {
    //        for (int i = 0; i < fileMap.Length; i++)
    //        {
    //            if (fileMap[i].BandMap == null || fileMap[i].BandMap.Length == 0)
    //            {
    //                int bandCount = fileMap[i].Raster.BandCount;
    //                int[] bands = new int[bandCount];
    //                for (int j = 0; j < bandCount; j++)
    //                {
    //                    bands[j] = j + 1;
    //                }
    //                fileMap[i].BandMap = bands;
    //            }
    //        }
    //    }

    //    #region 偏移量计算
    //    /// <summary>
    //    /// 计算inputRaster相对outputRaster的偏移。
    //    /// </summary>
    //    /// <param name="inputRaster"></param>
    //    /// <param name="outputRaster"></param>
    //    /// <param name="foIn"></param>
    //    /// <param name="foOut"></param>
    //    private void CalcFileOffset(IRasterDataProvider inputRaster, IRasterDataProvider outputRaster, out RasterOffset foIn, out RasterOffset foOut)
    //    {
    //        int tBeginRow = -1, tEndRow = -1, tBeginCol = -1, tEndCol = -1;
    //        int oBeginRow = -1, oEndRow = -1, oBeginCol = -1, oEndCol = -1;
    //        CoordEnvelope oEnvelope = inputRaster.CoordEnvelope;
    //        CoordEnvelope tEnvelope = outputRaster.CoordEnvelope;
    //        Size oSize = new Size(inputRaster.Width, inputRaster.Height);
    //        Size tSize = new Size(outputRaster.Width, outputRaster.Height);
    //        bool isInternal = RasterOffsetHelper.ComputeBeginEndRowCol(oEnvelope, oSize, tEnvelope, tSize,
    //            ref oBeginRow, ref oBeginCol, ref oEndRow, ref oEndCol,
    //            ref tBeginRow, ref tBeginCol, ref tEndRow, ref tEndCol);
    //        if (!isInternal)//不想交
    //        {
    //            foIn = null;
    //            foOut = null;
    //            return;
    //        }
    //        foIn = new RasterOffset();
    //        foIn.BeginX = oBeginCol;
    //        foIn.BeginY = oBeginRow;
    //        foIn.XSize = oEndCol - oBeginCol;
    //        foIn.YSize = oEndRow - oBeginRow;
    //        foOut = new RasterOffset();
    //        foOut.BeginX = tBeginCol;
    //        foOut.BeginY = tBeginRow;
    //        foOut.XSize = tEndCol - tBeginCol;
    //        foOut.YSize = tEndRow - tBeginRow;
    //    }

    //    #endregion

    //    public void Excute()
    //    {
    //        try
    //        {
    //            int height = _fileIn[0].Raster.Height;
    //            int rowCount = CalcRowCount();
    //            int step = height / rowCount;
    //            float stepCounter = 0f;
    //            for (int i = 0; i < height; i += rowCount)
    //            {
    //                if (_progress != null)
    //                    _progress((int)((stepCounter++ / step) * 100), "");
    //                if (i >= height)
    //                    break;
    //                if (i + rowCount > height)
    //                    rowCount = height - i;
    //                ExcuteMultiRow(i, rowCount);
    //            }
    //        }
    //        finally
    //        {
    //            Free();
    //        }
    //    }

    //    private int CalcRowCount()
    //    {
    //        int length = 0;
    //        foreach (RasterMaper ras in _fileIn)
    //        {
    //            length += ras.Raster.Width * DataTypeHelper.SizeOf(ras.Raster.DataType) * ras.BandMap.Length;
    //        }
    //        foreach (RasterMaper ras in _fileOut)
    //        {
    //            length += ras.Raster.Width * DataTypeHelper.SizeOf(ras.Raster.DataType) * ras.BandMap.Length;
    //        }
    //        int rowCount = 300 * 1024 * 1024 / length / 8;
    //        if (rowCount > _fileIn[0].Raster.Height)
    //            rowCount = _fileIn[0].Raster.Height;
    //        return rowCount;
    //    }

    //    private void ExcuteMultiRow(int rowIndex, int rowCount)
    //    {
    //        //初始化AOI
    //        TrySetAOI(rowIndex, rowCount);

    //        //初始化数据访问器
    //        RasterVirtualVistor<Tsrc>[] fileVistor = new RasterVirtualVistor<Tsrc>[_fileIn.Length];
    //        RasterVirtualVistor<Tdst>[] fileOutVistor = new RasterVirtualVistor<Tdst>[_fileOut.Length];
    //        InputData(rowIndex, rowCount, fileVistor);
    //        OutputData(rowIndex, rowCount, fileOutVistor);

    //        //处理逻辑
    //        if (_calcModle != null)
    //            _calcModle(fileVistor, fileOutVistor, _aoiIndex);

    //        //更新输出数据
    //        UpdateOutData(fileOutVistor, rowCount);
    //    }

    //    private void TrySetAOI(int rowIndex, int rowCount)
    //    {
    //        if (string.IsNullOrWhiteSpace(_aoiType) || string.IsNullOrWhiteSpace(_aoiValue))
    //            _aoiIndex = null;
    //        if (_aoiType == "template")
    //        {
    //            _aoiIndex = TryGetAoiFromTemplate(rowIndex, rowCount);
    //        }
    //        else if (_aoiType == "feature")
    //        {
    //            _aoiIndex = TryGetAoiFromFeature(rowIndex, rowCount);
    //        }
    //        else
    //            _aoiIndex = null;
    //    }

    //    private int[] TryGetAoiFromTemplate(int rowIndex, int rowCount)
    //    {
    //        double minLon = _fileOut[0].Raster.CoordEnvelope.MinX;
    //        double maxLon = _fileOut[0].Raster.CoordEnvelope.MaxX;
    //        double minLat = _fileOut[0].Raster.CoordEnvelope.MaxY - (rowIndex + rowCount) * _fileOut[0].Raster.ResolutionY;
    //        double maxLat = _fileOut[0].Raster.CoordEnvelope.MaxY - rowIndex * _fileOut[0].Raster.ResolutionY;
    //        Size outSize = new Size(_fileOut[0].Raster.Width, rowCount);
    //        int[] aoiIndex = AOITemplateFactory.MakeAOI(_aoiValue, minLon, maxLon, minLat, maxLat, outSize);
    //        return aoiIndex;
    //    }

    //    private int[] TryGetAoiFromFeature(int rowIndex, int rowCount)
    //    {
    //        return null;
    //    }

    //    private void InputData(int outRow, int rowCount, RasterVirtualVistor<Tsrc>[] fileVistor)
    //    {
    //        for (int i = 0; i < _fileIn.Length; i++)
    //        {
    //            RasterVirtualVistor<Tsrc> fv = new RasterVirtualVistor<Tsrc>();
    //            fv.Raster = _fileIn[i].Raster;
    //            fv.BandMap = _fileIn[i].BandMap;
    //            fv.IndexY = outRow;
    //            fv.SizeY = rowCount;
    //            fv.SizeX = _fileIn[i].Raster.Width;
    //            //
    //            int[] bandMap = _fileIn[i].BandMap;
    //            IRasterBand[] bands = new RasterBand[bandMap.Length];
    //            for (int b = 0; b < bandMap.Length; b++)
    //            {
    //                bands[b] = _fileIn[i].Raster.GetRasterBand(bandMap[b]);
    //            }
    //            fv.Bands = bands;
    //            //初始化源数据访问
    //            Tsrc[][] bandsVirtureData = new Tsrc[bands.Length][];
    //            RasterOffset offset = _fileInOffsets[i];
    //            int readOffsetX = offset.BeginX;
    //            int readOffsetY = offset.BeginY + outRow - _fileOutOffsets[i].BeginY;
    //            int readSizeX = offset.XSize;
    //            int readSizeY = rowCount + readOffsetY > _fileIn[i].Raster.Height ? _fileIn[i].Raster.Height - readOffsetY : rowCount;
    //            RasterOffset outOffset = _fileOutOffsets[i];
    //            int outOffsetX = outOffset.BeginX;
    //            int outSizeX = _fileIn[0].Raster.Width;
    //            int outSizeY = rowCount;

    //            if (readOffsetY >= _fileIn[i].Raster.Height || readOffsetY < 0)//源数据与目标已经没有交集
    //            {
    //                fv.RasterBandsData = null;
    //            }
    //            else
    //            {
    //                for (int b = 0; b < bands.Length; b++)
    //                {
    //                    enumDataType dataType = bands[b].DataType;
    //                    int dataTypeSize = DataTypeHelper.SizeOf(dataType);
    //                    //if (dataType == enumDataType.UInt16)
    //                    //{
    //                    Tsrc[] bufferInData = new Tsrc[readSizeX * readSizeY];
    //                    GCHandle buffer = GetHandles(bufferInData);
    //                    IntPtr bufferPtr = buffer.AddrOfPinnedObject();
    //                    //读取原始数据块
    //                    bands[b].Read(readOffsetX, readOffsetY, readSizeX, readSizeY, bufferPtr, dataType, readSizeX, readSizeY);
    //                    //复制到目标数据块大小
    //                    Tsrc[] dstVirtualData = new Tsrc[outSizeX * outSizeY];
    //                    if (readOffsetX >= 0 && readSizeX == outSizeX)
    //                    {
    //                        //Marshal.Copy(bufferPtr, dstVirtualData, outOffsetX, xInSize * yInSize);
    //                        //Buffer.BlockCopy(bufferInData, 0, dstVirtualData, outOffsetX, xInSize * yInSize * dataTypeSize);
    //                        Array.Copy(bufferInData, 0, dstVirtualData, outOffsetX, readSizeX * readSizeY);
    //                    }
    //                    else
    //                    {
    //                        //需要分行拷贝内存
    //                        for (int line = 0; line < readSizeY; line++)
    //                        {
    //                            int srcOffset = line * readSizeX * dataTypeSize;
    //                            int dstOffset = (outOffsetX + outSizeX * line) * dataTypeSize;
    //                            int count = readSizeX * dataTypeSize;
    //                            Buffer.BlockCopy(bufferInData, srcOffset, dstVirtualData, dstOffset, count);
    //                        }
    //                    }
    //                    buffer.Free();
    //                    bandsVirtureData[b] = dstVirtualData;
    //                    //}
    //                }
    //                fv.RasterBandsData = bandsVirtureData;
    //            }
    //            fileVistor[i] = fv;
    //        }
    //    }

    //    internal void OutputData(int outRow, int rowCount, RasterVirtualVistor<Tdst>[] fileVistor)
    //    {
    //        for (int i = 0; i < _fileOut.Length; i++)
    //        {
    //            RasterVirtualVistor<Tdst> fv = new RasterVirtualVistor<Tdst>();
    //            fv.BandMap = _fileOut[i].BandMap;
    //            fv.IndexY = outRow;
    //            fv.SizeX = _fileOut[i].Raster.Width;
    //            fv.SizeY = rowCount;
    //            fv.Raster = _fileOut[i].Raster;
    //            int[] bandMap = _fileOut[i].BandMap;
    //            IRasterBand[] bands = new RasterBand[bandMap.Length];
    //            for (int b = 0; b < bandMap.Length; b++)
    //            {
    //                bands[b] = _fileOut[i].Raster.GetRasterBand(bandMap[b]);
    //            }
    //            fv.Bands = bands;
    //            //初始化源数据访问
    //            int outWidth = _fileIn[0].Raster.Width;
    //            int readOffsetx = 0;
    //            int readOffsetY = outRow;
    //            int xSize = outWidth;
    //            int ySize = rowCount;
    //            enumDataType type = _fileOut[i].Raster.DataType;
    //            //if (type == enumDataType.Int16)
    //            //{
    //            Tdst[][] bandsVirtureData = new Tdst[bands.Length][];
    //            for (int b = 0; b < bands.Length; b++)
    //            {
    //                Tdst[] bufferData = new Tdst[xSize * ySize];
    //                GCHandle buffer = GetHandles(bufferData);
    //                bands[b].Read(readOffsetx, readOffsetY, xSize, ySize, buffer.AddrOfPinnedObject(), type, xSize, ySize);
    //                buffer.Free();
    //                bandsVirtureData[b] = bufferData;
    //            }
    //            fv.RasterBandsData = bandsVirtureData;
    //            //}
    //            fileVistor[i] = fv;
    //        }
    //    }

    //    internal void UpdateOutData(RasterVirtualVistor<Tdst>[] fileOutVistor, int rowCount)
    //    {
    //        for (int i = 0; i < fileOutVistor.Length; i++)
    //        {
    //            IRasterDataProvider raster = fileOutVistor[i].Raster;
    //            int row = fileOutVistor[i].IndexY;
    //            int[] bandMap = fileOutVistor[i].BandMap;
    //            IRasterBand[] bands = fileOutVistor[i].Bands;
    //            Tdst[][] bandsData = fileOutVistor[i].RasterBandsData;
    //            int readOffsetx = 0;
    //            int readOffsetY = row;
    //            int xSize = raster.Width;
    //            int ySize = rowCount;
    //            for (int b = 0; b < bands.Length; b++)
    //            {
    //                IRasterBand band = bands[b];
    //                enumDataType type = band.DataType;
    //                GCHandle buffer = GetHandles(bandsData[b]);
    //                band.Write(readOffsetx, readOffsetY, xSize, ySize, buffer.AddrOfPinnedObject(), type, xSize, ySize);
    //                buffer.Free();
    //            }
    //        }
    //    }

    //    internal GCHandle GetHandles(Tsrc[] virtureInData)
    //    {
    //        return GCHandle.Alloc(virtureInData, GCHandleType.Pinned);
    //    }

    //    internal GCHandle GetHandles(Tdst[] virtureInData)
    //    {
    //        return GCHandle.Alloc(virtureInData, GCHandleType.Pinned);
    //    }

    //    private void Free()
    //    {
    //        _calcModle = null;
    //        _aoiType = "";
    //        _aoiValue = "";
    //        _fileIn = null;
    //        _fileOut = null;
    //    }
    //}
}
