using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.Smart.ValueComposite
{
    public class ValueCompositer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFiles">输入文件列表</param>
        /// <param name="outFileName">输出文件名</param>
        /// <param name="indentify">最值计算标示</param>
        /// <returns></returns>
        public bool Composite(string inputDirectory, string outFileName, string indentify, Action<int, string> progressCallback)
        {
            if (string.IsNullOrEmpty(outFileName) || !outFileName.ToUpper().EndsWith(".DAT"))
                return false;
            string[] inputFiles = Directory.GetFiles(inputDirectory,"*.dat");
            if (inputFiles == null || inputFiles.Length < 1)
                return false;
            if (inputFiles.Length == 1)
            {
                //复制outfile
                CreateOutputFileSameToInputFile(inputFiles[0],outFileName);
                return true;
            }
            else
            {
                if(string.IsNullOrEmpty(indentify)||(indentify.ToUpper()!="MIN"&&indentify.ToUpper()!="MAX"))
                    return false;
                ArgumentParser parser = new ArgumentParser();
                string resolutionStr = parser.ParseArgumentValue("resolution");
                float resolution = float.Parse(resolutionStr);
                string defCloudStr = parser.ParseArgumentValue("defCloudy");

                //输入文件准备
                List<RasterMaper> rms = new List<RasterMaper>();
                try
                {
                    for (int i = 0; i < inputFiles.Length; i++)
                    {
                        IRasterDataProvider inRaster = GeoDataDriver.Open(inputFiles[i]) as IRasterDataProvider;
                        if (inRaster.BandCount != 1)
                        {
                            progressCallback(0,"请选择正确的数据进行最大值合成。");
                            return false;
                        }
                        RasterMaper rm = new RasterMaper(inRaster, new int[] { 1 });
                        rms.Add(rm);
                    }
                    //输出文件准备（作为输入栅格并集处理）
                    using (IRasterDataProvider outRaster = CreateOutputFile(rms.ToArray(), outFileName, resolution))
                    {
                        //栅格数据映射
                        RasterMaper[] fileIns = rms.ToArray();
                        RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                        //创建处理模型
                        if (outRaster.DataType == enumDataType.Int16)
                        {
                            //参数准备
                            short[] cloundValues = GetNanValues("CloudyValue",enumDataType.Int16) as short[];
                            short[] waterValues = GetNanValues("WaterValue", enumDataType.Int16) as short[];
                            RasterProcessModel<short, short> rfr = null;
                            rfr = new RasterProcessModel<short, short>(progressCallback);
                            rfr.SetRaster(fileIns, fileOuts);
                            Int16 defCloud = Int16.Parse(defCloudStr);
                            Int16 result;
                            if (indentify.ToUpper() == "MAX")
                            {
                                rfr.RegisterCalcModel(new RasterCalcHandler<short, short>((rvInVistor, rvOutVistor, aoi) =>
                                {
                                    int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                                    short[] maxData = new short[dataLength];
                                    InitMaxData(ref maxData);
                                    foreach (RasterVirtualVistor<short> rv in rvInVistor)
                                    {
                                        short[] dt = rv.RasterBandsData[0];
                                        if (dt != null)
                                        {
                                            for (int index = 0; index < dataLength; index++)
                                            {
                                                if (VaildValueProcess.ProcessMaxNanValues(new Int16[] { dt[index], maxData[index] }, cloundValues, waterValues, defCloud, out result))
                                                {
                                                    maxData[index] = result;
                                                }
                                            }
                                        }
                                    }
                                    for (int index = 0; index < dataLength; index++)
                                    {
                                        rvOutVistor[0].RasterBandsData[0][index] = maxData[index];
                                    }
                                }));
                                //执行
                                rfr.Excute(Int16.MinValue);  
                            }
                            else
                            {
                                rfr.RegisterCalcModel(new RasterCalcHandler<short, short>((rvInVistor, rvOutVistor, aoi) =>
                                {
                                    int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                                    short[] minData = new short[dataLength];
                                    InitMinData(ref minData);
                                    foreach (RasterVirtualVistor<short> rv in rvInVistor)
                                    {
                                        short[] dt = rv.RasterBandsData[0];
                                        if (dt != null)
                                        {
                                            for (int index = 0; index < dataLength; index++)
                                            {
                                                if (VaildValueProcess.ProcessMinNanValues(new Int16[] { dt[index], minData[index] }, cloundValues, waterValues, defCloud, out result))
                                                {
                                                    minData[index] = result;
                                                }
                                            }
                                        }
                                    }
                                    for (int index = 0; index < dataLength; index++)
                                    {
                                        rvOutVistor[0].RasterBandsData[0][index] = minData[index];
                                    }
                                }));
                                //执行
                                rfr.Excute(Int16.MaxValue);
                            }
                            return true;
                        }
                        else if (outRaster.DataType == enumDataType.UInt16)
                        {
                            //参数准备
                            ushort[] cloundValues = GetNanValues("CloudyValue",enumDataType.UInt16) as ushort[];
                            ushort[] waterValues = GetNanValues("WaterValue", enumDataType.UInt16) as ushort[];
                            //创建处理模型
                            RasterProcessModel<ushort, ushort> rfr = null;
                            rfr = new RasterProcessModel<ushort, ushort>(progressCallback);
                            rfr.SetRaster(fileIns, fileOuts);
                            if (indentify.ToUpper() == "MAX")
                            {
                                rfr.RegisterCalcModel(new RasterCalcHandler<ushort, ushort>((rvInVistor, rvOutVistor, aoi) =>
                                {
                                    int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                                    ushort[] maxData = new ushort[dataLength];
                                    for (int i = 0; i < dataLength; i++)
                                    {
                                        maxData[i] = UInt16.MinValue;
                                    }
                                    for (int index = 0; index < dataLength; index++)
                                    {
                                        foreach (RasterVirtualVistor<ushort> rvs in rvInVistor)
                                        {
                                            ushort[] dt = rvs.RasterBandsData[0];
                                            if (dt == null)
                                                continue;
                                            if (IsMaxValue(dt[index], cloundValues, waterValues, maxData[index]))
                                                maxData[index] = dt[index];
                                        }
                                    }
                                    for (int index = 0; index < dataLength; index++)
                                    {
                                        rvOutVistor[0].RasterBandsData[0][index] = maxData[index];
                                    }
                                }));
                                //执行
                                rfr.Excute(UInt16.MinValue);
                            }
                            else
                            {
                                rfr.RegisterCalcModel(new RasterCalcHandler<ushort, ushort>((rvInVistor, rvOutVistor, aoi) =>
                                {
                                    int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                                    ushort[] minData = new ushort[dataLength];
                                    for (int i = 0; i < dataLength; i++)
                                    {
                                        minData[i] = UInt16.MaxValue;
                                    }
                                    foreach (RasterVirtualVistor<ushort> rv in rvInVistor)
                                    {
                                        ushort[] dt = rv.RasterBandsData[0];
                                        if (dt != null)
                                        {
                                            for (int index = 0; index < dataLength; index++)
                                            {
                                                if (IsMinValue(dt[index], cloundValues, waterValues, minData[index]))
                                                    minData[index] = dt[index];
                                            }
                                        }
                                    }
                                    for (int index = 0; index < dataLength; index++)
                                    {
                                        rvOutVistor[0].RasterBandsData[0][index] = minData[index];
                                    }
                                }));
                                //执行
                                rfr.Excute(UInt16.MaxValue);
                            }
                            return true;
                        }
                    }
                }
                finally
                {
                    foreach (RasterMaper rm in rms)
                    {
                        rm.Raster.Dispose();
                    }

                }
            }
            return false;
        }

        private bool IsMinValue(ushort pixelValue, ushort[] cloudValues, ushort[] waterValues, ushort minValue)
        {
            if (pixelValue == UInt16.MaxValue)
                return false;
            if (minValue == UInt16.MaxValue)
                return true;
            if (IsNanValue(minValue, waterValues))
                return false;
            if (IsNanValue(pixelValue, cloudValues))
                return false;
            if (IsNanValue(pixelValue, waterValues))
                return true;
            if (IsNanValue(minValue, cloudValues))
                return true;
            else
            {
                if (minValue > pixelValue)
                    return true;
                else
                    return false;
            }
        }

        private bool IsMaxValue(ushort pixelValue, ushort[] cloudValues, ushort[] waterValues, ushort maxValue)
        {
            //若maxData[index]为初始值，可先将云或水的值赋给maxData
            if (pixelValue == UInt16.MinValue)
                return false;
            if (maxValue == UInt16.MinValue)
                return true;
            if (IsNanValue(maxValue, waterValues))
                return false;
            if (IsNanValue(pixelValue, cloudValues))
                return false;
            else
            {
                if (IsNanValue(pixelValue, waterValues))
                {
                    return true;
                }
                if (IsNanValue(maxValue, cloudValues))
                {
                    return true;
                }
                else
                {
                    if (maxValue < pixelValue)
                        return true;
                    else
                        return false;
                }
            }
        }

        private bool IsNanValue(ushort pixelValue, ushort[] nanValues)
        {
            if (nanValues == null || nanValues.Length < 1)
            {
                return false;
            }
            else
            {
                foreach (short value in nanValues)
                {
                    if (pixelValue == value)
                        return true;
                }
            }
            return false;
        }

        private void CreateOutputFileSameToInputFile(string inputFileName,string outFileName)
        {
            File.Copy(inputFileName, outFileName);
        }

        private IRasterDataProvider CreateOutputFile(RasterMaper[] inputRasterMapers, string outFileName, float resolution)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = null;
            foreach (RasterMaper maper in inputRasterMapers)
            {
                if (outEnv == null)
                    outEnv = maper.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Union(maper.Raster.CoordEnvelope);
            }
            int width = (int)(Math.Round(outEnv.Width / resolution));
            int height = (int)(Math.Round(outEnv.Height / resolution));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, inputRasterMapers[0].Raster.DataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }

        protected object GetNanValues(string argumentName,enumDataType dataType)
        {
            ArgumentParser parser = new ArgumentParser();
            string nanValuestring = parser.ParseArgumentValue(argumentName);
            if (!string.IsNullOrEmpty(nanValuestring))
            {
                string[] valueStrings = nanValuestring.Split(new char[] { ',', '，' });
                if (valueStrings != null && valueStrings.Length > 0)
                {
                    if (dataType == enumDataType.Int16)
                    {
                        List<short> values = new List<short>();
                        short value;
                        for (int i = 0; i < valueStrings.Length; i++)
                        {
                            if (Int16.TryParse(valueStrings[i], out value))
                                values.Add(value);
                        }
                        if (values.Count > 0)
                        {
                            return values.ToArray();
                        }
                    }
                    else if (dataType == enumDataType.UInt16)
                    {
                        List<ushort> values = new List<ushort>();
                        ushort value;
                        for (int i = 0; i < valueStrings.Length; i++)
                        {
                            if (UInt16.TryParse(valueStrings[i], out value))
                                values.Add(value);
                        }
                        if (values.Count > 0)
                        {
                            return values.ToArray();
                        }
                    }
                }
            }
            return null;
        }

        private void InitMaxData(ref short[] maxData)
        {
            int length = maxData.Length;
            for (int i = 0; i < length; i++)
            {
                maxData[i] = Int16.MinValue;
            }
        }

        private void InitMinData(ref short[] maxData)
        {
            int length = maxData.Length;
            for (int i = 0; i < length; i++)
            {
                maxData[i] = Int16.MaxValue;
            }
        }
    }
}
