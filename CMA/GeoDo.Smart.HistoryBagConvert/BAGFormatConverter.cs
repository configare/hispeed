#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/10/15 14:50:24
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
using System.Text.RegularExpressions;
using System.Drawing;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;

namespace GeoDo.Smart.HistoryBagConvert
{
    /// <summary>
    /// 类名：BAGFormatConverter
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/10/15 14:50:24
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class BAGFormatConverter
    {
        public bool Convert(string inputDir,string outputDir,Action<int, string> progressCallback)
        {
            if (string.IsNullOrEmpty(inputDir)||!Directory.Exists(inputDir))
                return false;
            if (string.IsNullOrEmpty(outputDir))
                outputDir = inputDir;
            string[] inputFiles=Directory.GetFiles(inputDir,"*.dat");
            if(inputFiles==null||inputFiles.Length<1)
                return false;
            if (progressCallback != null)
                progressCallback(0, "开始转换...");
            float dst = 100f / (inputFiles.Length);
            float precent = 0f;
            foreach(string file in inputFiles)
            {
                if (progressCallback != null)
                    progressCallback((int)(precent), "转换完成" + (int)(precent) + "%");
                ToConvert(file, outputDir, progressCallback);
                precent += dst;
            }
            if (progressCallback != null)
                progressCallback(100, "转换完成.");
            return true;
        }

        public void ToConvert(string inputFileName, string outputDir,Action<int, string> progressCallback)
        {
            float[] ndviValue = GetNDVIValue(inputFileName);
            if (ndviValue != null && ndviValue.Length > 0)
            {
                //保存相应的判识结果文件以及像元覆盖度文件
                SaveToDBLVFile(inputFileName, outputDir, ndviValue);
                SaveToBPCDFile(inputFileName, outputDir, ndviValue);
            }
        }

        private unsafe void SaveToBPCDFile(string inputFileName, string outputDir, float[] ndviValue)
        {
            int width, height;
            width = height = GetFileSize(inputFileName);
            string bpcdFilename = Path.Combine(outputDir, GetFileName(inputFileName, "BAG", "BPCD"));
            float[] ndviArgs = ExtractNDVIwAndNDVIb(inputFileName);
            //未能解析到端元值，使用默认的（-0.2,0.8）;
            if (float.IsNaN(ndviArgs[0]) || float.IsNaN(ndviArgs[1]))
            {
                ndviArgs[0] = -0.2f;
                ndviArgs[1] = 0.8f;
            }
            double dst = ndviArgs[1] - ndviArgs[0];
            using (IRasterDataProvider outProvider = CreatOutputRaster(bpcdFilename, enumDataType.Float, width, height))
            {
                float[] bpcdValues = new float[width];
                fixed (float* outPtr = bpcdValues)
                {
                    //写数据
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            if (ndviValue[j+i*width] == -9999)
                                bpcdValues[j] = -9999f;
                            else
                                bpcdValues[j] = (float)((ndviValue[j + i * width] - ndviArgs[0]) / dst);
                        }
                        IntPtr outBuffer = new IntPtr(outPtr);
                        outProvider.GetRasterBand(1).Write(0, i, width, 1, outBuffer, enumDataType.Float, width, 1);
                    }
                }
            }
        }

        private unsafe void SaveToDBLVFile(string inputFileName, string outputDir,float[] ndviValue)
        {
            int width, height;
            width = height = GetFileSize(inputFileName);
            string dblvFilename = Path.Combine(outputDir, GetFileName(inputFileName, "BAG", "DBLV"));
            using (IRasterDataProvider outProvider = CreatOutputRaster(dblvFilename, enumDataType.Int16, width, height))
            {
                Int16[] dblvValues = new Int16[width];
                fixed (Int16* outPtr = dblvValues)
                {
                    //写数据
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            if (ndviValue[i*width+j] == -9999)
                                dblvValues[j] = 0;
                            else
                                dblvValues[j] = 1;
                        }
                        IntPtr outBuffer = new IntPtr(outPtr);
                        outProvider.GetRasterBand(1).Write(0, i, width, 1, outBuffer, enumDataType.Int16, width, 1);
                    }
                }
            }
        }

        private IRasterDataProvider CreatOutputRaster(string outFileName, enumDataType dataType, int width, int height)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = null;
            switch (width)
            {
                case 400: outEnv = new CoordEnvelope(119.6875, 120.6875, 30.7375,31.7375);
                    break;
                case 500: outEnv = new CoordEnvelope(119.776253, 121.026253, 30.67375, 31.92375);
                    break;
                case 600: outEnv = new CoordEnvelope(119.651253, 121.151253, 30.54875, 32.04875);
                    break;
            }
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, dataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }

        private float[] GetNDVIValue(string inputFileName)
        {
            //读取待转换NDVI文件
            int samples = GetFileSize(inputFileName);
            Int16[] ndvi = LoadChannelValueForBSQ(inputFileName, samples);
            float[] ndviValues = new float[ndvi.Length];
            for (int i = 0; i < ndvi.Length; i++)
            {
                ndviValues[i] = (ndvi[i] == -9999) ? -9999 : (ndvi[i] / 1000f);
            }
            return ndviValues; 
        }

        private unsafe Int16[] LoadChannelValueForBSQ(string fileName,int samples)
        {
            int offset = 0;
            List<Int16> blockValues = new List<Int16>();
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fs);
                fs.Seek(offset, SeekOrigin.Begin);
                int recordCount = samples;
                int nPerBand = samples * recordCount;
                Int16[] values = new Int16[nPerBand];
                for (int iPixel = 0; iPixel < nPerBand; iPixel++)
                    values[iPixel] = binaryReader.ReadInt16();
                return values;
            }
        }

        private int GetFileSize(string inputFileName)
        {
            if (string.IsNullOrEmpty(inputFileName))
                return 0;
            FileInfo fInfo = new FileInfo(inputFileName);
            if (fInfo.Length == (600 * 600 * 2))
                return 600;
            else if (fInfo.Length == (400 * 400 * 2))
                return 400;
            else if (fInfo.Length == (500 * 500 * 2))
                return 500;
            else
                throw new Exception("根据文件[" + inputFileName + "]的大小无法判断行数与列数！");
        }

        private float[] ExtractNDVIwAndNDVIb(string filename)
        {
            float ndviWater, ndviBlue;
            string exp = @"_(?<min>[-]?\d+)_(?<max>[-]?\d+)\.dat$";
            Match m = Regex.Match(filename, exp);
            if (m.Success)
            {
                ndviWater = float.Parse(m.Groups["min"].Value) / 1000f;
                ndviBlue = float.Parse(m.Groups["max"].Value) / 1000f;
            }
            else
            {
                ndviWater = float.NaN;
                ndviBlue = float.NaN;
            }
            return new float[] { ndviWater, ndviBlue };
        }

        public string GetFileName(string file, string productIdentify, string outputIdentify)
        {
            string outFileName = productIdentify + "_" + outputIdentify + "_NULL_NULL_0250M_";
            string orbitTime = ExtractOrbitTime(file);
            if (!string.IsNullOrEmpty(orbitTime))
            {
                outFileName += orbitTime;
            }
            else
                outFileName += "NULL";
            outFileName += ".dat";
            return outFileName;
        }

        private string ExtractOrbitTime(string filename)
        {
            filename = Path.GetFileName(filename);
            string exp = @"(?<time>\d{12})";
            Match m = Regex.Match(filename, exp);
            if (m.Success)
            {
                return m.Groups["time"].Value;
            }
            else
            {
                string dateExp = @"_(?<year>\d{4})_(?<month>\d{2})_(?<day>\d{2})_(?<hour>\d{2})_(?<minite>\d{2})";
                Match dateM = Regex.Match(filename, dateExp);
                if (dateM.Success)
                {
                    string timeStr = dateM.Groups["year"].Value + dateM.Groups["month"].Value + dateM.Groups["day"].Value
                        + dateM.Groups["hour"].Value + dateM.Groups["minite"].Value;
                    return timeStr;
                }
            }
            return null;
        }
    }
}
