using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
//using GeoDo.RSS.DF.LDF;

namespace GeoDo.Smart.RasterSmooth
{
    /// <summary>
    /// 数据平滑处理
    /// </summary>
    public class Smooth
    {
        public Smooth()
        { }

        public bool CheckRaster(string[] filesIn, int bandIndex, string dir, int outInvalid,
            out IRasterDataProvider[] rasterIn, out IRasterDataProvider[] rasterOut, out string msgInfos)
        {
            rasterIn = null;
            rasterOut = null;
            string[] errorFiles;
            bool isok = FileCheck(filesIn, bandIndex, out rasterIn, out errorFiles, out msgInfos);
            if (isok)
            {
                IRasterDataProvider raster = rasterIn[0];
                return InitOutFile(raster, filesIn, dir, outInvalid, out rasterOut);
            }
            else
                return false;
        }

        public bool SmoothFiles(IRasterDataProvider[] rasterIn, IRasterDataProvider[] rasterOut,
            int iMax, int iMin, int iCore, int iInvalid, int inInvalid, int outInvalid, double absValue, int bandIndex, Action<int, string> progress, out string msgInfos)
        {
            msgInfos = null;
            return RawFileSmooth(rasterIn, rasterOut, iMax, iMin, iCore, iInvalid, inInvalid, outInvalid, absValue, bandIndex, progress);
        }

        private bool FileCheck(string[] files, int bandIndex, out IRasterDataProvider[] rasterIn, out string[] errorFiles, out string msgInfos)
        {
            rasterIn = null;
            errorFiles = null;
            msgInfos = null;
            List<string> errorFileList = new List<string>();
            StringBuilder msgString = new StringBuilder();
            if (files == null || files.Length == 0)
            {
                msgInfos = "输入文件列表为空";
                return false;
            }
            if (files == null || files.Length == 0)
            {
                msgInfos = "输出文件列表为空";
                return false;
            }
            if (files.Length < 10)
            {
                msgInfos = "输入的有效文件数至少要有10个";
                return false;
            }
            List<IRasterDataProvider> rasterInlst = new List<IRasterDataProvider>();
            foreach (string file in files)
            {
                IRasterDataProvider raster = RasterDataDriver.Open(file) as IRasterDataProvider;
                if (raster == null)
                {
                    errorFileList.Add(file);
                    msgString.AppendLine("不支持的文件" + file);
                    return false;
                }
                if (raster.BandCount < bandIndex)
                {
                    msgInfos = "数据的通道数小于设置要平滑的通道号";
                    return false;
                }
                string msg = null;
                if (FileIsCorrect(rasterInlst, raster, out msg))
                {
                    rasterInlst.Add(raster);
                }
                else
                {
                    raster.Dispose();
                    errorFileList.Add(file);
                    msgString.AppendLine(msg);
                }
            }
            rasterIn = rasterInlst.ToArray();
            errorFiles = errorFileList.ToArray();
            msgInfos = msgString.ToString();
            if (errorFiles == null || errorFiles.Length == 0)
                return true;
            else
                return false;
        }

        private bool FileIsCorrect(List<IRasterDataProvider> rasterIn, IRasterDataProvider raster, out string msginfos)
        {
            msginfos = "";
            if (rasterIn.Count == 0)
            {
                return true;
            }
            else
            {
                IRasterDataProvider r1 = rasterIn[0];
                if (r1.DataType != raster.DataType)
                {
                    msginfos = "数据类型不一致";
                    return false;
                }
                if (r1.BandCount != raster.BandCount)
                {
                    msginfos = "数据通道数不一致";
                    return false;
                }
                if (r1.Height != raster.Height || r1.Width != raster.Width)
                {
                    msginfos = "数据大小不一致";
                    return false;
                }
            }
            return true;
        }

        private bool InitOutFile(IRasterDataProvider rasterIn, string[] inFiles, string outDir, int defaultValue, out IRasterDataProvider[] rasterOuts)
        {
            rasterOuts = null;
            try
            {
                string fileExt = "";
                IRasterDataDriver driver = null;
                string[] optionString = null;
                if (rasterIn.Driver.Name == "MEM")//判识结果
                {
                    optionString = new string[]{
                    "SPATIALREF=" +rasterIn.SpatialRef==null?"":("SPATIALREF=" + rasterIn.SpatialRef.ToProj4String()),
                    "MAPINFO={" + 1 + "," + 1 + "}:{" + rasterIn.CoordEnvelope.MinX + "," + rasterIn.CoordEnvelope.MaxY + "}:{" + rasterIn.ResolutionX + "," + rasterIn.ResolutionY + "}"
                    };
                    driver = GeoDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
                    fileExt = ".DAT";
                }
                else
                {
                    optionString = new string[]{
                    "INTERLEAVE=BSQ",
                    "VERSION=LDF",
                    "WITHHDR=TRUE",
                    "SPATIALREF=" +rasterIn.SpatialRef==null?"":("SPATIALREF=" + rasterIn.SpatialRef.ToProj4String()),
                    "MAPINFO={" + 1 + "," + 1 + "}:{" + rasterIn.CoordEnvelope.MinX + "," + rasterIn.CoordEnvelope.MaxY + "}:{" + rasterIn.ResolutionX + "," + rasterIn.ResolutionY + "}"
                    };
                    driver = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
                    fileExt = ".LDF";
                }
                string[] fileOuts = new string[inFiles.Length];
                for (int i = 0; i < inFiles.Length; i++)
                {
                    fileOuts[i] = Path.Combine(Path.GetDirectoryName(inFiles[0]) + "/out", Path.ChangeExtension(Path.GetFileName(inFiles[i]), fileExt));
                }
                rasterOuts = new IRasterDataProvider[inFiles.Length];
                for (int i = 0; i < inFiles.Length; i++)
                {
                    IRasterDataProvider rasterOut = driver.Create(fileOuts[i], rasterIn.Width, rasterIn.Height, 1, rasterIn.DataType, optionString);
                    if (rasterOut == null)
                        return false;
                    rasterOuts[i] = rasterOut;
                    double nodata = 0d;
                    double.TryParse(defaultValue.ToString(), out nodata);
                    rasterOut.GetRasterBand(1).Fill(nodata);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private unsafe bool RawFileSmooth(IRasterDataProvider[] rasterIn, IRasterDataProvider[] rasterOut,
            int iMax, int iMin, int iCore, int iInvalid, int inInvalid, int outInvalid, double absValue, int bandIndex, Action<int, string> progress)
        {
            try
            {
                IRasterDataProvider raster = rasterIn[0];
                enumDataType dataType = raster.DataType;
                switch (dataType)
                {
                    case enumDataType.Int16:
                        return SmoothInt16(rasterIn, rasterOut, iMax, iMin, iCore, iInvalid, inInvalid, outInvalid, absValue, bandIndex, progress);
                    case enumDataType.UInt16:
                        return SmoothUInt16(rasterIn, rasterOut, iMax, iMin, iCore, iInvalid, inInvalid, outInvalid, absValue, bandIndex, progress);
                    default:
                        if (progress != null)
                            progress(0, "不支持的数据类型");
                        return false;
                }
            }
            finally
            {
            }
        }

        private unsafe bool SmoothUInt16(IRasterDataProvider[] rasterIn, IRasterDataProvider[] rasterOut, 
            int iMax, int iMin, int iCore, int iInvalid, int inInvalid, int outInvalid, double absValue, int bandIndex, Action<int, string> progress)
        {
            try
            {
                int height = rasterIn[0].Height;
                int fileCount = rasterIn.Length;      //文件个数
                int width = rasterIn[0].Width;        //每次处理像元数
                int processRows = 20;
                int processPiexs = width * processRows;
                //按行，每次处理一行。10行？
                for (int row = 0; row < height; row += processRows)
                {
                    if (row + processRows >= height)
                        processRows = height - row;
                    UInt16[] outdatas = new UInt16[fileCount * processPiexs];
                    UInt16[] inDatas = new UInt16[fileCount * processPiexs];
                    if (progress != null)
                        progress((int)(row * 100f / height), string.Format("{0}/{1}", row, height));
                    //读数据
                    for (int fileIndex = 0; fileIndex < fileCount; fileIndex++)
                    {
                        UInt16[] inDataBuffer = new UInt16[processPiexs];
                        unsafe
                        {
                            fixed (UInt16* ptr = inDataBuffer)
                            {
                                IntPtr buffer = new IntPtr(ptr);
                                rasterIn[fileIndex].GetRasterBand(bandIndex).Read(0, row, width, processRows, buffer, enumDataType.UInt16, width, processRows);
                            }
                        }
                        for (int pixel = 0; pixel < width * processRows; pixel++)
                        {
                            inDatas[fileCount * pixel + fileIndex] = inDataBuffer[pixel];
                        }
                    }
                    //平滑处理(按像素)
                    for (int pixel = 0; pixel < processPiexs; pixel++)
                    {
                        UInt16[] inDataBuffer = new UInt16[fileCount];
                        UInt16[] outDataBuffer = new UInt16[fileCount];
                        Array.Copy(inDatas, pixel * fileCount, inDataBuffer, 0, fileCount);
                        fixed (UInt16* indat = inDataBuffer)
                        {
                            fixed (UInt16* outdat = outDataBuffer)
                            {
                                SmoothAPI.SmoothAPIUInt16(indat, outdat, fileCount, 1, iMax, iMin, iCore, iInvalid, inInvalid, outInvalid, absValue);
                            }
                        }
                        Array.Copy(outDataBuffer, 0, outdatas, pixel * fileCount, fileCount);
                    }
                    //输出文件(按文件)
                    for (int i = 0; i < fileCount; i++)
                    {
                        UInt16[] databuffer = new UInt16[processPiexs];
                        for (int j = 0; j < processPiexs; j++)
                        {
                            databuffer[j] = outdatas[j * fileCount + i];
                        }
                        unsafe
                        {
                            fixed (UInt16* ptr = databuffer)
                            {
                                IntPtr buffer = new IntPtr(ptr);
                                rasterOut[i].GetRasterBand(bandIndex).Write(0, row, width, processRows, buffer, enumDataType.UInt16, width, processRows);
                            }
                        }
                    }
                }
                return true;
            }
            finally
            {
            }
        }
        
        private unsafe bool SmoothInt16(IRasterDataProvider[] rasterIn, IRasterDataProvider[] rasterOut,
            int iMax, int iMin, int iCore, int iInvalid, int inInvalid, int outInvalid, double absValue, int bandIndex, Action<int, string> progress)
        {
            try
            {
                int height = rasterIn[0].Height;
                int fileCount = rasterIn.Length;      //文件个数
                int width = rasterIn[0].Width;        //每次处理像元数
                int processRows = 20;
                int processPiexs = width * processRows;
                //按行，每次处理一行。10行？
                    int allCount = (height * width);
                for (int row = 0; row < height; row += processRows)
                {
                    if (row + processRows >= height)
                        processRows = height - row;
                    Int16[] outdatas = new Int16[fileCount * processPiexs];
                    Int16[] inDatas = new Int16[fileCount * processPiexs];
                    //if (progress != null)
                    //    progress((int)(row * 100f / height), string.Format("{0}/{1}", row, height));
                    //读数据
                    for (int fileIndex = 0; fileIndex < fileCount; fileIndex++)
                    {
                        Int16[] inDataBuffer = new Int16[processPiexs];
                        unsafe
                        {
                            fixed (Int16* ptr = inDataBuffer)
                            {
                                IntPtr buffer = new IntPtr(ptr);
                                rasterIn[fileIndex].GetRasterBand(bandIndex).Read(0, row, width, processRows, buffer, enumDataType.UInt16, width, processRows);
                            }
                        }
                        for (int pixel = 0; pixel < width * processRows; pixel++)
                        {
                            inDatas[fileCount * pixel + fileIndex] = inDataBuffer[pixel];
                        }
                    }
                    //平滑处理(按像素)
                    for (int pixel = 0; pixel < processPiexs; pixel++)
                    //Parallel.For(0,processPiexs,(pixel)=>
                    {
                        if (progress != null)
                        {
                            int percent = (int)((row * width + pixel) * 100f / allCount);
                            progress(percent, string.Format("{0}/{1}", (row * width + pixel), allCount));
                        }
                        Int16[] inDataBuffer = new Int16[fileCount];
                        Int16[] outDataBuffer = new Int16[fileCount];
                        Array.Copy(inDatas, pixel * fileCount, inDataBuffer, 0, fileCount);
                        fixed (Int16* indat = inDataBuffer)
                        {
                            fixed (Int16* outdat = outDataBuffer)
                            {
                                SmoothAPI.SmoothAPIInt16(indat, outdat, fileCount, 1, iMax, iMin, iCore, iInvalid, inInvalid, outInvalid, absValue);
                            }
                        }
                        Array.Copy(outDataBuffer, 0, outdatas, pixel * fileCount, fileCount);
                    };//);
                    //输出文件(按文件)
                    for (int i = 0; i < fileCount; i++)
                    {
                        Int16[] databuffer = new Int16[processPiexs];
                        for (int j = 0; j < processPiexs; j++)
                        {
                            databuffer[j] = outdatas[j * fileCount + i];
                        }
                        unsafe
                        {
                            fixed (Int16* ptr = databuffer)
                            {
                                IntPtr buffer = new IntPtr(ptr);
                                rasterOut[i].GetRasterBand(bandIndex).Write(0, row, width, processRows, buffer, enumDataType.Int16, width, processRows);
                            }
                        }
                    }
                }
                return true;
            }
            finally
            {
            }
        }
    }
}
