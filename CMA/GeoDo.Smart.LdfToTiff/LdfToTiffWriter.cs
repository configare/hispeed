#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/10/8 10:43:24
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
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.CA;

namespace GeoDo.Smart.LdfToTiff
{
    /// <summary>
    /// 类名：LdfToTiffWriter
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/10/8 10:43:24
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class LdfToTiffWriter
    {
        IRgbProcessor[] _grbProcessors = null;

        private bool Write(string inputFileName, string outputFileName, string bandNos,string imageHanceFile,Action<int, string> progressCallback)
        {
            if (string.IsNullOrEmpty(inputFileName) || !File.Exists(inputFileName))
                return false;
            if (string.IsNullOrEmpty(outputFileName))
                outputFileName = Path.Combine(Path.GetDirectoryName(inputFileName), Path.GetFileNameWithoutExtension(inputFileName) + ".tif");
            //解析图像增强方案
            if (!string.IsNullOrEmpty(imageHanceFile) && File.Exists(imageHanceFile))
                _grbProcessors = TryGetRgbProcess(imageHanceFile);
            using (IRasterDataProvider inRaster = GeoDataDriver.Open(inputFileName) as IRasterDataProvider)
            {
                RasterIdentify identify = new RasterIdentify(inputFileName);
                //设置波段选择及拉伸器
                int[] bandNums = TryGetBandSet(bandNos);
                if (bandNums == null || (bandNums.Length !=1&&bandNums.Length!=3))
                {
                    Console.WriteLine("输入波段号参数错误！");
                    return false;
                }
                foreach (int band in bandNums)
                {
                    if (band > inRaster.BandCount)
                        return false;
                }
                IRgbStretcher<UInt16>[] stretchers = new IRgbStretcher<UInt16>[bandNums.Length];
                for (int i = 0; i < stretchers.Length;i++ )
                    stretchers[i] = new LinearRgbStretcherUInt16(0, 1000, 0, 255);
                IRasterDataProvider outRaster = null;
                try
                {
                    if (progressCallback != null)
                        progressCallback(0, "开始进行文件格式转换...");
                    outRaster = CreatOutputRaster(outputFileName, inRaster, bandNums.Length, progressCallback);
                    int rowStep = ComputeRowStep(inRaster);
                    int stepCount = (int)((inRaster.Height) / rowStep + 0.5f);
                    int curStep = -1;
                    for (int i = 0; i < inRaster.Height;i+=rowStep)
                    {
                        if (i + rowStep > inRaster.Height)
                            rowStep = inRaster.Height - i;
                        long bufferSize = inRaster.Width * rowStep;
                        switch (inRaster.DataType)
                        {
                            case enumDataType.UInt16:
                                {
                                    curStep++;
                                    int curp = (int)((curStep * 1.0f / stepCount) * 100);
                                    if (progressCallback != null)
                                        progressCallback(curp, "转换完成" + curp + "%");
                                    UInt16[] dataBuffer = new UInt16[bufferSize];
                                    if (bandNums.Length == 3)
                                    {
                                        Byte[] outRDatabuffer = new Byte[bufferSize];
                                        Byte[] outGDatabuffer = new Byte[bufferSize];
                                        Byte[] outBDatabuffer = new Byte[bufferSize];
                                        unsafe
                                        {
                                            //读取RGB三个波段的值并做拉伸处理存入相应buffer数组
                                            fixed (UInt16* ptr = dataBuffer)
                                            {
                                                fixed (Byte* outRPtr = outRDatabuffer)
                                                {
                                                    fixed (Byte* outGPtr = outGDatabuffer)
                                                    {
                                                        fixed (Byte* outBPtr = outBDatabuffer)
                                                        {
                                                            IntPtr buffer = new IntPtr(ptr);
                                                            inRaster.GetRasterBand(bandNums[0]).Read(0, i, inRaster.Width, rowStep, buffer, inRaster.DataType, inRaster.Width, rowStep);
                                                            //R拉伸转换
                                                            IntPtr outRBuffer = new IntPtr(outRPtr);
                                                            byte* rptr0 = (byte*)outRBuffer;
                                                            for (int j = 0; j < dataBuffer.Length; j++)
                                                            {
                                                                stretchers[0].Stretch(dataBuffer[j], rptr0);
                                                                rptr0++;
                                                            }
                                                            inRaster.GetRasterBand(bandNums[1]).Read(0, i, inRaster.Width, rowStep, buffer, inRaster.DataType, inRaster.Width, rowStep);
                                                            //G拉伸转换
                                                            IntPtr outGBuffer = new IntPtr(outGPtr);
                                                            byte* gptr0 = (byte*)outGBuffer;
                                                            for (int j = 0; j < dataBuffer.Length; j++)
                                                            {
                                                                stretchers[0].Stretch(dataBuffer[j], gptr0);
                                                                gptr0++;
                                                            }
                                                            inRaster.GetRasterBand(bandNums[2]).Read(0, i, inRaster.Width, rowStep, buffer, inRaster.DataType, inRaster.Width, rowStep);
                                                            //B拉伸转换
                                                            IntPtr outBBuffer = new IntPtr(outBPtr);
                                                            byte* bptr0 = (byte*)outBBuffer;
                                                            for (int j = 0; j < dataBuffer.Length; j++)
                                                            {
                                                                stretchers[0].Stretch(dataBuffer[j], bptr0);
                                                                bptr0++;
                                                            }
                                                            //图像增强处理
                                                            RgbProcess(outRBuffer, outGBuffer, outBBuffer, outRDatabuffer.Length);
                                                            outRaster.GetRasterBand(1).Write(0, i, inRaster.Width, rowStep, outRBuffer, enumDataType.Byte, inRaster.Width, rowStep);
                                                            outRaster.GetRasterBand(2).Write(0, i, inRaster.Width, rowStep, outGBuffer, enumDataType.Byte, inRaster.Width, rowStep);
                                                            outRaster.GetRasterBand(3).Write(0, i, inRaster.Width, rowStep, outBBuffer, enumDataType.Byte, inRaster.Width, rowStep);
                                                        }
                                                    }
                                                }
                                            }     
                                        }
                                        break;
                                    }
                                    else if(bandNums.Length==1)
                                    {
                                        Byte[] outDatabuffer = new Byte[bufferSize];
                                        unsafe
                                        {
                                            //读取1个波段的值并做拉伸处理存入相应buffer数组
                                            fixed (UInt16* ptr = dataBuffer)
                                            {
                                                fixed (Byte* outRPtr = outDatabuffer)
                                                {
                                                    IntPtr buffer = new IntPtr(ptr);
                                                    inRaster.GetRasterBand(bandNums[0]).Read(0, i, inRaster.Width, rowStep, buffer, inRaster.DataType, inRaster.Width, rowStep);
                                                    //R拉伸转换
                                                    IntPtr outRBuffer = new IntPtr(outRPtr);
                                                    byte* rptr0 = (byte*)outRBuffer;
                                                    for (int j = 0; j < dataBuffer.Length; j++)
                                                    {
                                                        stretchers[0].Stretch(dataBuffer[j], rptr0);
                                                        rptr0++;
                                                    }
                                                    outRaster.GetRasterBand(1).Write(0, i, inRaster.Width, rowStep, outRBuffer, enumDataType.Byte, inRaster.Width, rowStep);
                                                }
                                           }
                                        }
                                    }
                                    break;
                                }
                            default: break;
                        }
                    }
                    if (progressCallback != null)
                        progressCallback(100, "文件" + inputFileName + "转换完成");
                    return true;
                }
                finally
                {
                    if (outRaster != null)
                        outRaster.Dispose();
                }
            }
        }

        private int[] TryGetBandSet(string bandNos)
        {
            if (string.IsNullOrEmpty(bandNos))
                return null;
            string[] numbers = bandNos.Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
            if (numbers == null || numbers.Length < 1)
                return null;
            List<int> bandList = new List<int>();
            int no;
            foreach (string band in numbers)
            {
                if (Int32.TryParse(band, out no))
                    bandList.Add(no);
            }
            return bandList.ToArray();
        }

        private unsafe void RgbProcess(IntPtr rptr,IntPtr gptr,IntPtr bptr,int length)
        {
            if (_grbProcessors == null || _grbProcessors.Length < 1)
                return;
            byte* rptr0 = (byte*)rptr;
            byte* gptr0 = (byte*)gptr;
            byte* bptr0 = (byte*)bptr;
            for (int i = 0; i < length; i++)
            {
                RgbProcess(ref *rptr0, ref *gptr0, ref *bptr0);
                rptr0++;
                gptr0++;
                bptr0++;
            }
        }

        private IRasterDataProvider CreatOutputRaster(string outputFName, IRasterDataProvider inRaster,int bandCount, Action<int, string> progressCallback)
        {
            if (inRaster == null)
                return null;
            if (progressCallback != null)
                progressCallback(1, "开始创建目标文件...");
            IRasterDataDriver driver = GeoDataDriver.GetDriverByName("GDAL") as IRasterDataDriver;
            List<string> options = new List<string>();
            options.AddRange(new string[]{"DRIVERNAME=GTiff",
                    "TFW=YES",
                    "TILED=YES"});
            if (inRaster.SpatialRef != null)
                options.Add("WKT=" + inRaster.SpatialRef.ToWKTString());
            if (inRaster.CoordEnvelope != null)
                options.Add("GEOTRANSFORM=" + string.Format("{0},{1},{2},{3},{4},{5}", inRaster.CoordEnvelope.MinX, inRaster.ResolutionX, 0, inRaster.CoordEnvelope.MaxY, 0, -inRaster.ResolutionY));
            IRasterDataProvider prdWrite = driver.Create(outputFName, inRaster.Width, inRaster.Height, bandCount, enumDataType.Byte, options.ToArray()) as IRasterDataProvider;
            if (progressCallback != null)
                progressCallback(20, "目标文件创建完成.");
            return prdWrite;
           
            //if (inRaster == null)
            //    return null;
            //IRasterDataDriver driver = GeoDataDriver.GetDriverByName("GDAL") as IRasterDataDriver;
            //string[] options = new string[]{
            //        "DRIVERNAME=GTiff",
            //        "TFW=YES",
            //        "TILED=YES",
            //        "WKT="+inRaster.SpatialRef.ToWKTString(),
            //        "GEOTRANSFORM="+string.Format("{0},{1},{2},{3},{4},{5}",inRaster.CoordEnvelope.MinX, inRaster.ResolutionX,0, inRaster.CoordEnvelope.MaxY,0, -inRaster.ResolutionY)
            //        }; 
            //IRasterDataProvider prdWrite = driver.Create(outputFName, inRaster.Width, inRaster.Height, 3, enumDataType.Byte, options) as IRasterDataProvider;
            //return prdWrite;
       
        }

        //计算所取行数
        private int ComputeRowStep(IRasterDataProvider srcRaster)
        {
            int row = (int)((1024 * 1024 * 1024f) / (3 * (2 + 3)) / srcRaster.Width);//1GB
            if (row == 0)
                row = 1;
            if (row > srcRaster.Height)
                row = srcRaster.Height;
            return row;
        }
        
        private static IRgbProcessor[] TryGetRgbProcess(string filename)
        {
            RgbProcessorStack processor = new RgbProcessorStack();
            processor.ReadXmlElement(filename);
            IRgbProcessor[] ps = processor.Processors.ToArray();
            return ps;
        }

        private void RgbProcess(ref byte r, ref byte g, ref byte b)
        {
            if (_grbProcessors == null || _grbProcessors.Length == 0)
                return;
            int count = _grbProcessors.Length;
            IRgbProcessor p = null;
            for (int i = count - 1; i >= 0; i--)//由于是按照先进后出的Stat形式保存的，故应反序以此处理
            {
                p = _grbProcessors[i];
                if (p is RgbProcessorByPixel)
                {
                    (p as RgbProcessorByPixel).ProcessRGB(ref b, ref g, ref r);
                }
            }
        }

        public bool Write(string inputFName, string outputFName, string searchPattern, string bandNos, string imageHanceFile, Action<int, string> progressCallback)
        {
            if (string.IsNullOrEmpty(inputFName))
                return false;
            //判断是文件夹还是文件
            if (IsDir(inputFName))
            {
                if (IsDir(outputFName))
                {
                    if (string.IsNullOrEmpty(searchPattern))
                        searchPattern = "*.*";
                    string[] files = Directory.GetFiles(inputFName, searchPattern);
                    if(files.Length<1)
                        return false;
                    string outFileName = null;
                    foreach(string file in files)
                    {
                        outFileName = Path.Combine(outputFName, Path.GetFileNameWithoutExtension(file) + ".tif");
                        Write(file, outFileName, bandNos, imageHanceFile,progressCallback);
                    }
                    return true;
                }
                //输出参数设置为文件名或其他
                else
                {
                    Console.WriteLine("输出文件夹名称不正确！");
                    return false;
                }
            }
            else
            {
                return Write(inputFName, outputFName, bandNos, imageHanceFile, progressCallback);
            }
        }

        public static bool IsDir(string filepath)
        {
            FileInfo fi = new FileInfo(filepath);
            FileAttributes fa = fi.Attributes;
            if ((int)fa == -1)
            {
                return false;
            }
            else if((fi.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                return true;
            return false;
        }
    }
}
