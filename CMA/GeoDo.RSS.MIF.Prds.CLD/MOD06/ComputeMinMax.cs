using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Drawing;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using System.IO;


namespace GeoDo.RSS.MIF.Prds.CLD
{
    public class ComputeMinMaxAvg
    {
        private string _logf ="";

        public ComputeMinMaxAvg(string log)
        {
            _logf = log;
        }

        /// <summary>
        /// 最大值统计
        /// </summary>
        /// <param name="filenames">输入数据，ldf格式</param>
        /// <param name="res"></param>
        /// <param name="outname">输出数据，dat格式</param>
        /// <param name="access"></param>
        /// <returns></returns>
        public void CLDParaPeriodicMaxStat(string[] filenames,string outname, /*string access,*/int  invalidValue)
        {
            float resolution=0;///数据分辨率
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider outRaster = null;
            try
            {
                enumDataType dataType =enumDataType.Unknow;
                for (int i = 0; i < filenames.Length; i++)
                {
                    IRasterDataProvider inRaster = GeoDataDriver.Open(filenames[i]) as IRasterDataProvider;
                    dataType = inRaster.DataType;
                    resolution = inRaster.ResolutionX;
                    RasterMaper rm = new RasterMaper(inRaster, new int[] { 1 });
                    rms.Add(rm);
                }
                string outfilename = outname;//输出文件名
                outRaster = CreateOutRaster(outfilename, dataType, rms.ToArray(), resolution);
                //栅格数据映射  
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                if (dataType == enumDataType.Float)
                {
                    RasterProcessModel<float, float> rfr = null;
                    //创建结果数据
                    rfr = new RasterProcessModel<float, float>();
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<float, float>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        float[] maxtemp = new float[dataLength];
                        //为避免最大值为空，先把float的最小值附给maxtemp
                        for (int index = 0; index < dataLength; index++)
                        {
                            maxtemp[index] = float.MinValue;
                        }
                        foreach (RasterVirtualVistor<float> rv in rvInVistor)
                        {
                            if (rv.RasterBandsData == null)
                                continue;
                            float[] dt = rv.RasterBandsData[0];//获取波段数据
                            if (dt != null)
                            {
                                for (int index = 0; index < dataLength; index++)
                                {
                                    if (rv.RasterBandsData[0][index] != invalidValue && maxtemp[index] < rv.RasterBandsData[0][index])//最大值不能等于无效值
                                    {
                                        maxtemp[index] = rv.RasterBandsData[0][index];
                                    }
                                }
                            }
                        }

                        for (int index = 0; index < dataLength; index++)
                        {
                            if (maxtemp[index] == float.MinValue)
                            {
                                maxtemp[index] = invalidValue;
                            }
                            rvOutVistor[0].RasterBandsData[0][index] = maxtemp[index];
                        }
                    }));
                    rfr.Excute();
                }
                else 
                {
                    RasterProcessModel<Int32, Int32> rfr = null;
                    //创建结果数据
                    rfr = new RasterProcessModel<Int32, Int32>();
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int32, Int32>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        Int32[] maxtemp = new Int32[dataLength];
                        //为避免最大值为空，先把float的最小值附给maxtemp
                        for (int index = 0; index < dataLength; index++)
                        {
                            maxtemp[index] = Int32.MinValue;
                        }
                        foreach (RasterVirtualVistor<Int32> rv in rvInVistor)
                        {
                            if (rv.RasterBandsData == null)
                                continue;
                            Int32[] dt = rv.RasterBandsData[0];//获取波段数据
                            if (dt != null)
                            {
                                for (int index = 0; index < dataLength; index++)
                                {
                                    if (rv.RasterBandsData[0][index] != invalidValue && maxtemp[index] < rv.RasterBandsData[0][index])//最大值不能等于无效值
                                    {
                                        maxtemp[index] = rv.RasterBandsData[0][index];
                                    }
                                }
                            }
                        }

                        for (int index = 0; index < dataLength; index++)
                        {
                            if (maxtemp[index] ==Int32.MinValue)
                            {
                                maxtemp[index] = (Int32)invalidValue;
                            }
                            rvOutVistor[0].RasterBandsData[0][index] = maxtemp[index];
                        }
                    }));
                    rfr.Excute();

                }

            }
            catch (Exception ex)
            {
                LogFactory.WriteLine(_logf, "统计" + outname + "时，出现错误:" + ex.Message);
            }
            finally
            {
                foreach (RasterMaper rm in rms)
                {
                    rm.Raster.Dispose();
                }
                if (outRaster != null)
                    outRaster.Dispose();
            }
        }

        /// <summary>
        /// 云参数最小值统计，只统计非填充值的数据，这是合理的
        /// </summary>
        /// <param name="filenames"></param>
        /// <param name="res"></param>
        /// <param name="outname"></param>
        /// <param name="access"></param>
        public void CLDParaPeriodicMinStat(string[] filenames, string outname, int  invalidValue)
        {
            float resolution=0 ;
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider outRaster = null;
            try
            {
                enumDataType dataType =enumDataType.Unknow;
                for (int i = 0; i < filenames.Length; i++)
                {
                    IRasterDataProvider inRaster = GeoDataDriver.Open(filenames[i]) as IRasterDataProvider;
                    dataType = inRaster.DataType;
                    resolution = inRaster.ResolutionX;
                    RasterMaper rm = new RasterMaper(inRaster, new int[] { 1 });
                    rms.Add(rm);
                }
                string outfilename = outname;
                outRaster = CreateOutRaster(outfilename, dataType, rms.ToArray(), resolution);
                //栅格数据映射  
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                // 创建处理模型
                if (dataType == enumDataType.Float)
                {
                    RasterProcessModel<float, float> rfr = null;
                    rfr = new RasterProcessModel<float, float>();
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<float, float>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        float[] mintemp = new float[dataLength];
                        //为避免最小值为空，先把float的最大值附给mintemp
                        for (int index = 0; index < dataLength; index++)
                        {
                            mintemp[index] = float.MaxValue;//值得商榷，是否合理
                        }
                        foreach (RasterVirtualVistor<float> rv in rvInVistor)
                        {
                            for (int index = 0; index < dataLength; index++)
                            {
                                //rv.RasterBandsData[0][index] ==invalidValue，mintemp值不变。
                                if (rv.RasterBandsData[0][index] != invalidValue && mintemp[index] > rv.RasterBandsData[0][index])
                                {
                                    mintemp[index] = rv.RasterBandsData[0][index];
                                }
                            }
                        }
                        for (int index = 0; index < dataLength; index++)
                        {
                            if (mintemp[index] == float.MaxValue)
                            {
                                mintemp[index] = invalidValue;
                            }
                            rvOutVistor[0].RasterBandsData[0][index] = mintemp[index];
                        }
                    }));
                    rfr.Excute();
                }
                else
                {
                    RasterProcessModel<Int32, Int32> rfr = null;
                    rfr = new RasterProcessModel<Int32, Int32>();
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int32, Int32>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        Int32[] mintemp = new Int32[dataLength];
                        //为避免最小值为空，先把float的最大值附给mintemp
                        for (int index = 0; index < dataLength; index++)
                        {
                            mintemp[index] = Int32.MaxValue;//值得商榷，是否合理
                        }
                        foreach (RasterVirtualVistor<Int32> rv in rvInVistor)
                        {
                            for (int index = 0; index < dataLength; index++)
                            {
                                //rv.RasterBandsData[0][index] ==invalidValue，mintemp值不变。
                                if (rv.RasterBandsData[0][index] != invalidValue && mintemp[index] > rv.RasterBandsData[0][index])
                                {
                                    mintemp[index] = rv.RasterBandsData[0][index];
                                }
                            }
                        }
                        for (int index = 0; index < dataLength; index++)
                        {
                            if (mintemp[index] == Int32.MaxValue)
                            {
                                mintemp[index] = (Int32)invalidValue;
                            }
                            rvOutVistor[0].RasterBandsData[0][index] = mintemp[index];
                        }
                    }));
                    rfr.Excute();
                }
            }
            catch (Exception ex)
            {
                LogFactory.WriteLine(_logf, "统计" + outname + "时，出现错误:" + ex.Message);
            }
            finally
            {
                foreach (RasterMaper rm in rms)
                {
                    rm.Raster.Dispose();
                }
                if (outRaster != null)
                    outRaster.Dispose();
            }
        }

        /// <summary>
        /// 参数平均值统计，不统计填充值
        /// </summary>
        /// <param name="filenames"></param>
        /// <param name="res"></param>
        /// <param name="outname"></param>
        /// <param name="access"></param>
        public void CLDParaPeriodicAvgStat(string[] filenames, string outname,  int  invalidValue)
        {
            float resolution =0;
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider outRaster = null;
            try
            {
                enumDataType dataType = enumDataType.Unknow;
                for (int i = 0; i < filenames.Length; i++)
                {
                    IRasterDataProvider inRaster = GeoDataDriver.Open(filenames[i]) as IRasterDataProvider;
                    dataType = inRaster.DataType;
                    resolution = inRaster.ResolutionX;
                    RasterMaper rm = new RasterMaper(inRaster, new int[] { 1 });
                    rms.Add(rm);
                }
                string outfilename = outname;
                outRaster = CreateOutRaster(outfilename, dataType, rms.ToArray(), resolution);
                //栅格数据映射  
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                //创建处理模型
                if (dataType==enumDataType.Float)
                {
                    RasterProcessModel<float, float> rfr = null;
                    rfr = new RasterProcessModel<float, float>();
                    rfr.SetRaster(fileIns, fileOuts);
                    //针对输入文件
                    rfr.RegisterCalcModel(new RasterCalcHandler<float, float>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        int n = rvInVistor.Count(); //输入文件个数
                        float[,] save = new float[n + 1, dataLength];// 第一维是文件的顺序号，第二维是文件的数值
                        short[,] flag = new short[n + 1, dataLength];
                        int j = 0; //记录输入文件的序号，从0开始
                        foreach (RasterVirtualVistor<float> rv in rvInVistor)
                        {
                            if (rv.RasterBandsData == null)
                                continue;
                            float[] dt = rv.RasterBandsData[0];
                            if (dt != null)
                            {
                                for (int index = 0; index < dataLength; index++)
                                {
                                    save[j, index] = rv.RasterBandsData[0][index];
                                    if (rv.RasterBandsData[0][index] != invalidValue)//掩膜无效值
                                    {
                                        flag[j, index] = 1;
                                    }
                                    else
                                        flag[j, index] = 0;
                                }
                            }
                            j++;
                        }
                        //针对输出文件
                        float[] sumData = new float[dataLength];
                        short[] sumDataFlag = new short[dataLength];
                        try
                        {
                            for (int m = 0; m < n; m++)//文件数
                            {
                                for (int q = 0; q < dataLength; q++)//每个文件中的数据个数
                                {
                                    if (flag[m, q]==1)
                                    {
                                        sumData[q] += save[m, q];
                                        sumDataFlag[q] += flag[m, q];
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogFactory.WriteLine(_logf, "统计" + outname + "时，出现错误:" + ex.Message);
                        }
                        for (int index = 0; index < dataLength; index++)
                        {
                            if (sumDataFlag[index] == 0)
                            {
                                rvOutVistor[0].RasterBandsData[0][index] = invalidValue;
                            }
                            else
                            {
                                rvOutVistor[0].RasterBandsData[0][index] = sumData[index] / Convert.ToSingle(sumDataFlag[index]);
                            }
                        }
                    }));
                    rfr.Excute();
                }
                else
                {
                    RasterProcessModel<Int32, Int32> rfr = null;
                    rfr = new RasterProcessModel<Int32, Int32>();
                    rfr.SetRaster(fileIns, fileOuts);
                    //针对输入文件
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int32, Int32>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        int n = rvInVistor.Count(); //输入文件个数
                        Int32[,] save = new Int32[n , dataLength];// 第一维是文件的顺序号，第二维是文件的数值
                        short[,] flag = new short[n , dataLength];
                        int j = 0; //记录输入文件的序号，从0开始
                        foreach (RasterVirtualVistor<Int32> rv in rvInVistor)
                        {
                            if (rv.RasterBandsData == null)
                                continue;
                            Int32[] dt = rv.RasterBandsData[0];
                            if (dt != null)
                            {
                                for (int index = 0; index < dataLength; index++)
                                {
                                    save[j, index] = rv.RasterBandsData[0][index];
                                    if (rv.RasterBandsData[0][index] != invalidValue)//掩膜无效值
                                    {
                                        flag[j, index] = 1;
                                    }
                                    else
                                        flag[j, index] = 0;
                                }
                            }
                            j++;
                        }
                        //针对输出文件
                        Int32[] sumData = new Int32[dataLength];
                        Int32[] sumDataFlag = new Int32[dataLength];
                        try
                        {
                            for (int m = 0; m < n; m++)//文件数
                            {
                                for (int q = 0; q < dataLength; q++)//每个文件中的数据个数
                                {
                                    if (flag[m, q] == 1)
                                    {
                                        sumData[q] += save[m, q];
                                        sumDataFlag[q] += flag[m, q];
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogFactory.WriteLine(_logf, "统计" + outname + "时，出现错误:" + ex.Message);
                        }
                        for (int index = 0; index < dataLength; index++)
                        {
                            if (sumDataFlag[index] == 0)
                            {
                                rvOutVistor[0].RasterBandsData[0][index] = (Int32)invalidValue;
                            }
                            else
                            {
                                rvOutVistor[0].RasterBandsData[0][index] = sumData[index] / Convert.ToInt16(sumDataFlag[index]);
                            }
                        }
                    }));
                    rfr.Excute();
                }

            }
            catch (Exception ex)
            {
                LogFactory.WriteLine(_logf, "统计"+outname+"时，出现错误:"+ex.Message);
            }
            finally
            {
                foreach (RasterMaper rm in rms)
                {
                    rm.Raster.Dispose();
                }
                if (outRaster != null)
                    outRaster.Dispose();
            }
        }

        /// <summary>
        /// 创建输出删格数据
        /// </summary>
        /// <param name="outFileName"></param>
        /// <param name="inrasterMaper"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        protected IRasterDataProvider CreateOutRaster(string outFileName, enumDataType dataType, RasterMaper[] inrasterMaper, float resolution)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Intersect(inRaster.Raster.CoordEnvelope);
            }
            float resX, resY;
            if (resolution != 0f)
            {
                resX = resolution;
                resY = resolution;
            }
            else
            {
                resX = inrasterMaper[0].Raster.ResolutionX;
                resY = inrasterMaper[0].Raster.ResolutionY;
            }
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, dataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }

    }
}
