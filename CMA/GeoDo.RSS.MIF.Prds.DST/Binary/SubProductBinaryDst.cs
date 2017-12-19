using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.MIF.Prds.Comm;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.DST
{
    public class SubProductBinaryDst : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductBinaryDst(SubProductDef subProductDef)
            : base(subProductDef)
        {
            _name = subProductDef.Name;
            _identify = subProductDef.Identify;
            _isBinary = true;
            _algorithmDefs = new List<AlgorithmDef>();
            _algorithmDefs.AddRange(subProductDef.Algorithms);
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            
            _contextMessage = contextMessage;
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.DataProvider == null)
                return null;
            IRasterDataProvider prd = _argumentProvider.DataProvider;
            IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("DST", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
            if (String.IsNullOrEmpty(_argumentProvider.GetArg("AlgorithmName").ToString()))
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            string algorith = _argumentProvider.GetArg("AlgorithmName").ToString();
            int bandCount = prd.BandCount;
            bool isOk = false;
            if (algorith == "FY3Land")
                isOk = DustExtractFY3Land(bandCount, ref result);
            else if (algorith == "FY3Sea")
                isOk = DustExtractFY3Sea(bandCount, ref result);
            else if (algorith == "NOAA17Land")
                isOk = DustExtractNoaa17Land(bandCount, ref result);
            else if (algorith == "NOAA17Sea")
                isOk = DustExtractNoaa17Sea(bandCount, ref result);
            else if (algorith == "NOAA18Land")
                isOk = DustExtractNoaa18Land(bandCount, ref result);
            else if (algorith == "NOAA18Sea")
                isOk = DustExtractNoaa18Sea(bandCount, ref result);
            else if (algorith == "EOSLand")
                isOk = DustExtractEosLand(bandCount, ref result);
            else if (algorith == "EOSSea")
                isOk = DustExtractEosSea(bandCount, ref result);
            else if (algorith == "ImportAlgorithm")
            {
                isOk = DustImportAlgorithm(ref result);
            }
            else if (algorith == "ThresholdAlgorithm_RGB")
            {
                isOk = DustRGBAlgorithm(ref result);
            }
            else if(algorith=="FY2Algorithm_YW")
            {
                isOk = DustFy3EYW_Algorithm(ref result);
            }
            else if (algorith == "FY2Algorithm")
            {
                FileExtractResult IDDI = null;
                isOk = DustExtractFY2(bandCount, ref result, ref IDDI, progressTracker);
                if (!isOk)
                    return null;
                if (IDDI != null)
                {
                    ExtractResultArray resultArray = new ExtractResultArray(_subProductDef.Identify);
                    resultArray.Add(result);
                    resultArray.Add(IDDI);
                    return resultArray;
                }
                return result as IExtractResult;
            }
            else if (algorith == "FY3Algorithm")
            {
                return (new ComplexExtracter().Extracting(_argumentProvider, ref result, _contextMessage, progressTracker));
            }
            else if (algorith == "ModisAlgorithm")
                isOk = DustExtractModis(bandCount, ref result);
            else
            {
                PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
                return null;
            }
            if (!isOk)
                return null;
            return result as IExtractResult;
        }
        /// <summary>
        /// 增加静止星业务
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool DustFy3EYW_Algorithm(ref IPixelIndexMapper result)
        {
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int VisibleCH = TryGetBandNo(bandNameRaster, "Visible");
            double VisibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");

            if (VisibleCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return false;
            }

            string express = string.Format(@"band{0}/" + VisibleZoom + @"f  > var_VisibleMin && band{0}/" + VisibleZoom + @"f  < var_VisibleMax", VisibleCH);
            try
            {
                int[] bandNos = new int[] { VisibleCH };
                if (_argumentProvider.DataProvider.DataType == enumDataType.Byte)
                {
                    IThresholdExtracter<byte> extracter = new SimpleThresholdExtracter<byte>();//这里如果用uint16时候，系统会出现崩溃
                    extracter.Reset(_argumentProvider, bandNos, express);
                    IRasterDataProvider prd = _argumentProvider.DataProvider;
                    result = PixelIndexMapperFactory.CreatePixelIndexMapper("FOG", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
                    extracter.Extract(result);
                    return true;
                }
                else
                {
                    PrintInfo("非静止星数据，数据类型不是byte的。");
                    return false;
                }
            }
            catch (Exception ex)
            {
                PrintInfo(ex.Message);
                return false;
            }
        }

        private bool DustRGBAlgorithm(ref IPixelIndexMapper result)
        {
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int RedCH = TryGetBandNo(bandNameRaster, "Red");
            double RedZoom = (double)_argumentProvider.GetArg("Red_Zoom");

            if (RedCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return false;
            }

            int GreenCH = TryGetBandNo(bandNameRaster, "Green");
            double GreenZoom = (double)_argumentProvider.GetArg("Green_Zoom");

            if (GreenCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return false;
            }

            int BlueCH = TryGetBandNo(bandNameRaster, "Blue");
            double BlueZoom = (double)_argumentProvider.GetArg("Blue_Zoom");

            if (BlueCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return false;
            }
            string express = string.Format(@"(band{0}/" + RedZoom + @"f  > var_RedMin && band{0}/" + RedZoom + @"f  < var_RedMax)&&
                                            (band{1}/" + GreenZoom + @"f  > var_GreenMin && band{1}/" + GreenZoom + @"f  < var_GreenMax)&&
                                            (band{2}/" + BlueZoom + @"f  > var_BlueMin && band{2}/" + BlueZoom + @"f  < var_BlueMax)", RedCH, GreenCH, BlueCH);
            try
            {
                int[] bandNos = new int[] { RedCH, GreenCH, BlueCH };
                if (_argumentProvider.DataProvider.DataType == enumDataType.Byte)
                {
                    IThresholdExtracter<byte> extracter = new SimpleThresholdExtracter<byte>();//这里如果用uint16时候，系统会出现崩溃
                    extracter.Reset(_argumentProvider, bandNos, express);
                    IRasterDataProvider prd = _argumentProvider.DataProvider;
                    result = PixelIndexMapperFactory.CreatePixelIndexMapper("DST", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
                    extracter.Extract(result);
                    return true;
                }
                else
                {
                    PrintInfo("非真彩色数据，数据类型不是byte的。");
                    return false;
                }
            }
            catch (Exception ex)
            {
                PrintInfo(ex.Message);
                return false;
            }
        }

        private bool DustImportAlgorithm(ref IPixelIndexMapper result)
        {
            if (_argumentProvider.GetArg("HistoryFile") == null)
                return false;
            else
            {
                result = _argumentProvider.GetArg("HistoryFile") as IPixelIndexMapper;
                return true;
            }
            //string dblvImportFile = _argumentProvider.GetArg("HistoryFile").ToString();
            //if (string.IsNullOrEmpty(dblvImportFile)||!File.Exists(dblvImportFile))
            //    return false;
            //RasterMaper[] fileIns = null;
            //RasterMaper[] fileOuts = null;
            //string tempRasterFile=null;
            //string outRasterFile=null;
            //try
            //{
            //    IRasterDataProvider dblvPrd = GeoDataDriver.Open(dblvImportFile) as IRasterDataProvider;
            //    //创建临时与当前影像大小一致的Int16类型文件
            //    IRasterDataProvider tempRaster = GetTempRaster(_argumentProvider.DataProvider,"LDF",enumDataType.Int16);
            //    IRasterDataProvider outRaster = GetTempRaster(dblvPrd, "MEM", enumDataType.Int16);
            //    tempRasterFile=tempRaster.fileName;
            //    outRasterFile=outRaster.fileName;
            //    List<RasterMaper> rms = new List<RasterMaper>();
            //    RasterMaper rm = new RasterMaper(tempRaster, new int[] { 1 });
            //    RasterMaper oldRm = new RasterMaper(dblvPrd, new int[] { 1 });
            //    rms.AddRange(new RasterMaper[] { rm, oldRm });
            //    //栅格数据映射
            //    fileIns = rms.ToArray();
            //    fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
            //    //创建处理模型
            //    RasterProcessModel<short, short> rfr = null;
            //    rfr = new RasterProcessModel<short, short>(null);
            //    rfr.SetRaster(fileIns, fileOuts);
            //    rfr.RegisterCalcModel(new RasterCalcHandler<short, short>((rvInVistor, rvOutVistor, aoi) =>
            //    {
            //        if (rvInVistor[0].RasterBandsData[0] != null)
            //        {
            //            for (int i = 0; i < rvInVistor[1].RasterBandsData[0].Length; i++)
            //            {
            //                if (rvInVistor[1].RasterBandsData[0][i] == 1 && rvInVistor[0].RasterBandsData[0][i] != -1)
            //                {
            //                    rvOutVistor[0].RasterBandsData[0][i] = 1;
            //                }
            //            }
            //        }
            //    }));
            //    rfr.Excute(-1);
            //    List<int> indexList = new List<int>();
            //    IRasterPixelsVisitor<short> visitor = new RasterPixelsVisitor<short>(new ArgumentProvider(outRaster, null));
            //    visitor.VisitPixel(new int[] { 1 },
            //        (idx, values) =>
            //        {
            //            if (values[0] == 1)
            //                indexList.Add(idx);
            //        });
            //    result.Put(indexList.ToArray());
            //    return true;
            //}
            //finally
            //{
            //    foreach (RasterMaper item in fileIns)
            //    {
            //        if (item.Raster != null)
            //            item.Raster.Dispose();
            //    }
            //    foreach (RasterMaper item in fileOuts)
            //    {
            //        if (item.Raster != null)
            //            item.Raster.Dispose();
            //    }
            //    if (File.Exists(tempRasterFile))
            //        File.Delete(tempRasterFile);
            //    if (File.Exists(outRasterFile))
            //        File.Delete(outRasterFile);
            //}
        }

        private IRasterDataProvider GetTempRaster(IRasterDataProvider currentRasterPrd,string driver,enumDataType dataType)
        {
            string outFileName = AppDomain.CurrentDomain.BaseDirectory + "TemporalFileDir//" + Path.GetFileName(currentRasterPrd.fileName);
            if (!Directory.Exists(Path.GetDirectoryName(outFileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(outFileName));
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName(driver) as IRasterDataDriver;
            CoordEnvelope outEnv = currentRasterPrd.CoordEnvelope.Clone();
            int width = currentRasterPrd.Width;
            int height = currentRasterPrd.Height;
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, dataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }

        private bool DustExtractFY2(int bandCount, ref IPixelIndexMapper result,ref FileExtractResult iddiResult, Action<int, string> progressTracker)
        {
            //步骤一：计算地表背景亮温
            int visiNo, midNo1, midNo2, farNo1, farNo2;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;//
            visiNo = TryGetBandNo(bandNameRaster, "Visible"); //(int)_argumentProvider.GetArg("Visible");
            midNo1 = TryGetBandNo(bandNameRaster, "MiddleInfrared1"); //(int)_argumentProvider.GetArg("MiddleInfrared1");
            midNo2 = TryGetBandNo(bandNameRaster, "MiddleInfrared2"); //(int)_argumentProvider.GetArg("MiddleInfrared2");
            farNo1 = TryGetBandNo(bandNameRaster, "FarInfrared11"); //(int)_argumentProvider.GetArg("FarInfrared11");
            farNo2 = TryGetBandNo(bandNameRaster, "FarInfrared12"); //(int)_argumentProvider.GetArg("FarInfrared12");
            if (visiNo == -1 || midNo1 == -1 || farNo1 == -1 || farNo2 == -1 || midNo2 == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return false;
            }
            int[] bandNos = new int[] { farNo1, farNo2, midNo1, midNo2, visiNo};
            DstFY2ExtractArgSet set = _argumentProvider.GetArg("Arguments") as DstFY2ExtractArgSet;
            if (set == null)
            {
                PrintInfo("获取阈值参数失败。");
                return false;
            }
            if (string.IsNullOrEmpty(set.BackFileName) || !File.Exists(set.BackFileName))
            {
                PrintInfo("未设置背景亮温文件或该文件不存在。");
                return false;
            }
            if (set.ArgValueArray==null)
            {
                PrintInfo("未设置阈值参数。");
                return false;
            }
            return DoExtract(bandNos,set, ref result,ref iddiResult, progressTracker);
        }

        private bool DoExtract(int[] bandNos,DstFY2ExtractArgSet set, ref IPixelIndexMapper result,ref FileExtractResult iddiResult,  Action<int, string> progressTracker)
        {
            List<RasterMaper> rms = new List<RasterMaper>();
            try
            {
                IRasterDataProvider inRaster = RasterDataDriver.Open(set.BackFileName) as IRasterDataProvider;
                int[] backBands=new int[inRaster.BandCount];
                for (int i = 0; i < inRaster.BandCount; i++)
                    backBands[i] = i + 1;
                RasterMaper brm = new RasterMaper(inRaster, backBands);
                rms.Add(brm);
                IRasterDataProvider iRaster = _argumentProvider.DataProvider;
                if (iRaster.BandCount < bandNos.Count())
                {
                    PrintInfo("请选择正确的数据进行判识。");
                    return false;
                }
                RasterMaper rm = new RasterMaper(iRaster, bandNos);
                rms.Add(rm);
                //添加角度文件
                string solZenFileName = Path.Combine(Path.GetDirectoryName(_argumentProvider.DataProvider.fileName),
                    Path.GetFileNameWithoutExtension(_argumentProvider.DataProvider.fileName) + ".NOMSunZenith.ldf");
                if(!File.Exists(solZenFileName))
                {
                    PrintInfo("未能获取影像太阳天顶角信息。");
                    return false;
                }
                IRasterDataProvider solzenRaster = RasterDataDriver.Open(solZenFileName) as IRasterDataProvider;
                RasterMaper solzenRm = new RasterMaper(solzenRaster, new int[] { 1 });
                rms.Add(solzenRm);
                //输出文件准备（作为输入栅格并集处理）
                RasterIdentify ri = GetRasterIdentifyID(_argumentProvider.DataProvider.fileName);
                string outFileName = ri.ToWksFullFileName(".dat");
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName,rm))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<short, float> rfr = new RasterProcessModel<short, float>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    if (set.IsNightExtract)
                    {
                        rfr.RegisterCalcModel(new RasterCalcHandler<short, float>((rvInVistor, rvOutVistor, aoi) =>
                        {
                            int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                            if (rvInVistor[0].RasterBandsData != null&&rvInVistor[1].RasterBandsData!=null)
                            {
                                for (int index = 0; index < dataLength; index++)
                                {
                                    //判识
                                    if (CheckIsNightDst(rvInVistor, index, set))
                                    {
                                        //IDDI
                                        rvOutVistor[0].RasterBandsData[0][index] = (rvInVistor[0].RasterBandsData[0][index] - rvInVistor[1].RasterBandsData[0][index])/10f;
                                    }
                                }
                            }

                        }));
                    }
                    else
                    {
                        rfr.RegisterCalcModel(new RasterCalcHandler<short, float>((rvInVistor, rvOutVistor, aoi) =>
                        {
                            int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                            if (rvInVistor[0].RasterBandsData[0] != null)
                            {
                                for (int index = 0; index < dataLength; index++)
                                {
                                    //判识
                                    if (CheckIsDayDst(rvInVistor,index, set))
                                    {
                                        //IDDI
                                        rvOutVistor[0].RasterBandsData[0][index] = (rvInVistor[0].RasterBandsData[0][index] - rvInVistor[1].RasterBandsData[0][index])/10f;
                                    }
                                }
                            }

                        }));
                    }
                    //执行
                    rfr.Excute();
                    //平滑处理
                    if (set.IsSmooth)
                    {
                        SmoothIDDIResult(outRaster);
                    }
                    iddiResult = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                    iddiResult.SetDispaly(false);
                    //根据IDDI生成判识结果
                    List<int> indexList = new List<int>();
                    IRasterPixelsVisitor<float> visitor = new RasterPixelsVisitor<float>(new ArgumentProvider(outRaster, null));
                    visitor.VisitPixel(new int[] { 1 },
                        (idx, values) =>
                        {
                            if (values[0] != 0)
                                indexList.Add(idx);
                        });
                    result.Put(indexList.ToArray());
                    result.Tag = new DstFeatureFY2Collection("沙尘辅助信息", GetFY2DisplayInfo(bandNos, inRaster, solzenRaster));
                    return true;
                }
            }
            finally
            {
                if (rms[0].Raster != null)
                    rms[0].Raster.Dispose();
                if (rms[2].Raster != null)
                    rms[2].Raster.Dispose();
            }
        }

        private Dictionary<int, DstFeatureFY2> GetDisplayInfo()
        {
            return null;
        }

        private void SmoothIDDIResult(IRasterDataProvider outRaster)
        {
            //int height = outRaster.Height;
            //int width = outRaster.Width;
            //if (height < 5 || width < 5)
            //    return;
            //int iaround;
            //for (int i = 3; i <= height - 2; i++)
            //{
            //    for (int j = 3; j < width - 2; j++)
            //    {
            //        if()
            //        iaround = 0;
            //    }
            //}
        }

        private bool CheckIsNightDst(RasterVirtualVistor<short>[] rvInVistor, int index, DstFY2ExtractArgSet set)
        {
            float tbd12 = (rvInVistor[1].RasterBandsData[0][index] - rvInVistor[1].RasterBandsData[1][index]) / 10f;
            if(tbd12>=set.ArgValueArray[0]&&tbd12<=set.ArgValueArray[1])
            {
                float iddi=(rvInVistor[0].RasterBandsData[0][index] - rvInVistor[1].RasterBandsData[0][index])/10f;
                if(iddi>=set.ArgValueArray[2]&&iddi<=set.ArgValueArray[3])
                {
                    float tbd13 = (rvInVistor[1].RasterBandsData[0][index] - rvInVistor[1].RasterBandsData[2][index])/10f;
                    if (tbd13 >= set.ArgValueArray[4] && tbd13 <= set.ArgValueArray[5])
                    {
                        float tbd14 = (rvInVistor[1].RasterBandsData[0][index] - rvInVistor[1].RasterBandsData[3][index])/10f;
                        if (tbd14 >= set.ArgValueArray[6] && tbd14 <= set.ArgValueArray[7])
                        {
                            float tbd2=rvInVistor[1].RasterBandsData[1][index]/10f;
                            if (tbd2 >= set.ArgValueArray[8] && tbd2 <= set.ArgValueArray[9])
                            {
                                float tbd3 = rvInVistor[1].RasterBandsData[2][index]/10f;
                                if (tbd3 >= set.ArgValueArray[10] && tbd3 <= set.ArgValueArray[11])
                                {
                                    float tbd4 = rvInVistor[1].RasterBandsData[3][index]/10f;
                                    if (tbd4 >= set.ArgValueArray[12] && tbd4 <= set.ArgValueArray[13])
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
             }
            return false;
        }

        private bool CheckIsDayDst(RasterVirtualVistor<short>[] rvInVistor, int index,DstFY2ExtractArgSet set)
        {
            float tbd12 = (rvInVistor[1].RasterBandsData[0][index] - rvInVistor[1].RasterBandsData[1][index]) / 10f;
            if (tbd12 >= set.ArgValueArray[0] && tbd12 <= set.ArgValueArray[1])
            {
                float iddi = (rvInVistor[0].RasterBandsData[0][index] - rvInVistor[1].RasterBandsData[0][index])/10f;
                if (iddi >= set.ArgValueArray[2] && iddi <= set.ArgValueArray[3])
                {
                    float tbd13 = (rvInVistor[1].RasterBandsData[0][index] - rvInVistor[1].RasterBandsData[2][index]) / 10f;
                    if (tbd13 >= set.ArgValueArray[4] && tbd13 <= set.ArgValueArray[5])
                    {
                        float tbd14 = (rvInVistor[1].RasterBandsData[0][index] - rvInVistor[1].RasterBandsData[3][index]) / 10f;
                        if (tbd14 >= set.ArgValueArray[6] && tbd14 <= set.ArgValueArray[7])
                        {
                            float tbd2 = rvInVistor[1].RasterBandsData[1][index] / 10f;
                            if (tbd2 >= set.ArgValueArray[8] && tbd2 <= set.ArgValueArray[9])
                            {
                                float tbd3 = rvInVistor[1].RasterBandsData[2][index] / 10f;
                                if (tbd3 >= set.ArgValueArray[10] && tbd3 <= set.ArgValueArray[11])
                                {
                                    float tbd4 = rvInVistor[1].RasterBandsData[3][index] / 10f;
                                    if (tbd4 >= set.ArgValueArray[12] && tbd4 <= set.ArgValueArray[13])
                                    {
                                        float tbdBK12 = (rvInVistor[0].RasterBandsData[0][index]-rvInVistor[0].RasterBandsData[1][index]) / 10f;
                                        if (tbd12 < tbdBK12) 
                                        {
                                            //可见光
                                            double cosValue=Math.Cos(DegreeToRadian(rvInVistor[2].RasterBandsData[0][index]/100f));
                                            float tbdVisref = rvInVistor[1].RasterBandsData[4][index]/1000f /(float)(cosValue);
                                            if(tbdVisref>=set.ArgValueArray[15]&&tbdVisref<=set.ArgValueArray[16])
                                            {
                                                float tbb4 = rvInVistor[1].RasterBandsData[3][index]/10f;
                                                float tbb1 = rvInVistor[1].RasterBandsData[0][index]/10f;
                                                double t2rb1 = T2R(tbb1, 3.75);
                                                double RefMidIR = (T2R(tbb4, 3.75) - t2rb1) / (10.72 * cosValue / 3.1416 - t2rb1);
                                                if (RefMidIR < set.ArgValueArray[14])
                                                   return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }       
            return false;
        }

        private double paramC1 = 1.1910439d / Math.Pow(10, 16);
        private double paramC2 = 1.438769d / Math.Pow(10, 2);

        private double T2R(float temp, double waveLength)
        {
            double a1 = paramC1 / (Math.Pow(waveLength / Math.Pow(10, 6), 4) * waveLength);
            double a2 = Math.Exp(paramC2 / ((waveLength / Math.Pow(10, 6)) * temp)) - 1;
            return a1 / a2;
        }

        private IRasterDataProvider CreateOutRaster(string outFileName, RasterMaper rasterMaper)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = rasterMaper.Raster.CoordEnvelope;
            float resX = rasterMaper.Raster.ResolutionX;
            float resY = rasterMaper.Raster.ResolutionY;
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, enumDataType.Float, mapInfo) as RasterDataProvider;
            return outRaster;
        }


        private RasterIdentify GetRasterIdentifyID(string fileName)
        {
            RasterIdentify rst = new RasterIdentify(fileName);
            rst.ProductIdentify = _subProductDef.ProductDef.Identify;
            rst.SubProductIdentify = "IDDI";
            rst.IsOutput2WorkspaceDir = true;
            return rst;
        }

        /// <summary>
        /// MODIS判识算法
        /// </summary>
        /// <param name="bandCount"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool DustExtractModis(int bandCount, ref IPixelIndexMapper result)
        {
            int midNo = -1, far1No = -1, far2No = -1;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;//
            midNo = TryGetBandNo(bandNameRaster, "MiddleInfrared");
            far1No = TryGetBandNo(bandNameRaster, "FarInfrared1");
            far2No = TryGetBandNo(bandNameRaster, "FarInfrared2");
            double midZoom = (double)_argumentProvider.GetArg("MiddleInfrared_Zoom");
            double far1Zoom = (double)_argumentProvider.GetArg("FarInfrared1_Zoom");
            double far2Zoom = (double)_argumentProvider.GetArg("FarInfrared2_Zoom");
            if (midNo == -1 || far1No == -1 || far2No == -1 || midZoom == -1 || far1Zoom == -1 || far2Zoom == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return false;
            }
            if (midNo > bandCount)
            {
                PrintInfo("中红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (far1No > bandCount)
            {
                PrintInfo("远红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (far2No > bandCount)
            {
                PrintInfo("远红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            string express = " ((band" + far1No + "/" + far1Zoom + "f-band" + far2No + "/" + far2Zoom
                + "f) < var_FarInfraredMax)&&((band" + midNo + "/" + midZoom + "f-band"
                + far1No + "/" + far1Zoom + "f) >= var_MiddleInfraredFarInfraredMin)";
            int[] bandNos = new int[] { midNo, far1No, far2No };
            SandDustExtract(express, bandNos, ref result);
            try
            {
                DstFeatureCollection featureInfos = new DstFeatureCollection("沙尘辅助信息", GetDisplayInfo(-1, -1, -1, far1No, far2No, 9999));
                result.Tag = featureInfos;
            }
            catch
            {
                result.Tag = new DstFeatureCollection("沙尘辅助信息计算失败", null);
            }
            return true;
        }

        /// <summary>
        /// FY-3|FY1D 陆地判识算法
        /// </summary>
        /// <param name="_argumentProvider"></param>
        private bool DustExtractFY3Land(int bandCount, ref IPixelIndexMapper result)
        {
            int visiNo = -1, farNo = -1, shortNo = -1;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;//
            visiNo = TryGetBandNo(bandNameRaster, "Visible");//(int)_argumentProvider.GetArg("Visible");
            shortNo = TryGetBandNo(bandNameRaster, "ShortInfrared");//(int)_argumentProvider.GetArg("ShortInfrared");
            farNo = TryGetBandNo(bandNameRaster, "FarInfrared"); //(int)_argumentProvider.GetArg("FarInfrared");
            double visiZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double farZoom = (double)_argumentProvider.GetArg("FarInfrared_Zoom");
            double shortZoom = (double)_argumentProvider.GetArg("ShortInfrared_Zoom");
            float ShortFarInfraredVar = (float)_argumentProvider.GetArg("ShortInfraredFarInfraredVar");

            if (visiNo == -1 || farNo == -1 || shortNo == -1 || visiZoom == -1 || farZoom == -1 || shortZoom == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return false;
            }
            if (visiNo > bandCount)
            {
                PrintInfo("可见光波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (shortNo > bandCount)
            {
                PrintInfo("短波红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (farNo > bandCount)
            {
                PrintInfo("远红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }

            string express = " ((band" + visiNo + "/" + visiZoom + "f) > var_VisibleMin) && (band" + visiNo + "/"
                             + visiZoom + "f < var_VisibleMax) && (band" + farNo + "/" + farZoom
                             + "f > var_FarInfraredMin) && (band" + farNo + "/" + farZoom
                             + "f < var_FarInfraredMax) && (band" + shortNo + "/" + shortZoom
                             + "f > var_ShortInfraredMin) && ((band" + shortNo + "/" + shortZoom
                             + "f - band" + visiNo + "/" + visiZoom + "f)>var_ShortInfraredVisibleMin)  && ((band"
                             + shortNo + "/" + shortZoom + "f - band" + farNo + "/" + farZoom
                             + "f + var_ShortInfraredFarInfraredVar)>var_ShortInfraredFarInfraredMin)";
            int[] bandNos = new int[] { visiNo, farNo, shortNo };
            SandDustExtract(express, bandNos, ref result);
            try
            {
                DstFeatureCollection featureInfos = new DstFeatureCollection("沙尘辅助信息", GetDisplayInfo(visiNo, shortNo, -1, -1, farNo, (float)(ShortFarInfraredVar * farZoom)));
                result.Tag = featureInfos;
            }
            catch
            {
                result.Tag = new DstFeatureCollection("沙尘辅助信息计算失败", null);
            }
            return true;
        }

        /// <summary>
        /// FY-3|FY1D 海洋判识算法
        /// </summary>
        /// <param name="_argumentProvider"></param>
        /// <param name="result"></param>
        private bool DustExtractFY3Sea(int bandCount, ref IPixelIndexMapper result)
        {
            int visiNo = -1, farNo = -1, shortNo = -1, nearNo = -1;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;//
            visiNo = TryGetBandNo(bandNameRaster, "Visible");//(int)_argumentProvider.GetArg("Visible");
            nearNo = TryGetBandNo(bandNameRaster, "NearInfrared");//(int)_argumentProvider.GetArg("NearInfrared");
            shortNo = TryGetBandNo(bandNameRaster, "ShortInfrared");//(int)_argumentProvider.GetArg("ShortInfrared");
            farNo = TryGetBandNo(bandNameRaster, "FarInfrared"); //(int)_argumentProvider.GetArg("FarInfrared");
            double visiZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double farZoom = (double)_argumentProvider.GetArg("FarInfrared_Zoom");
            double shortZoom = (double)_argumentProvider.GetArg("ShortInfrared_Zoom");
            double nearZoom = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
            if (visiNo == -1 || farNo == -1 || shortNo == -1 || nearNo == -1 || visiZoom == -1 || farZoom == -1 || shortZoom == -1 || nearZoom == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return false;
            }
            if (visiNo > bandCount)
            {
                PrintInfo("可见光波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (nearNo > bandCount)
            {
                PrintInfo("近红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (shortNo > bandCount)
            {
                PrintInfo("短波红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (farNo > bandCount)
            {
                PrintInfo("远红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }

            string express = "(band" + visiNo + " / " + visiZoom + "f > var_VisibleMin) && (band" + visiNo + " / "
                            + visiZoom + "f < var_VisibleMax) && (band" + farNo + " / " + farZoom + "f > var_FarInfraredMin) && (band"
                            + farNo + " / " + farZoom + "f < var_FarInfraredMax) &&" + "(band" + shortNo + " / "
                            + shortZoom + "f > var_ShortInfraredMin) && ((band" + shortNo + " / " + shortZoom
                            + "f - band" + nearNo + " / " + nearZoom + "f) > var_ShortInfraredNearInfraredMin) && ((band"
                            + visiNo + " / " + visiZoom + "f - band" + nearNo + " / " + nearZoom + "f) > var_VisibleNearInfraredMin)";
            int[] bandNos = new int[] { visiNo, farNo, shortNo, nearNo };
            SandDustExtract(express, bandNos, ref result);
            try
            {
                DstFeatureCollection featureInfos = new DstFeatureCollection("沙尘辅助信息", GetDisplayInfo(visiNo, shortNo, nearNo, -1, farNo, 9999));
                result.Tag = featureInfos;
            }
            catch
            {
                result.Tag = new DstFeatureCollection("沙尘辅助信息计算失败", null);
            }
            return true;
        }

        /// <summary>
        /// NOAA17陆地
        /// </summary>
        /// <param name="_argumentProvider"></param>
        /// <param name="result"></param>
        private bool DustExtractNoaa17Land(int bandCount, ref IPixelIndexMapper result)
        {
            int visiNo = -1, farNo = -1, shortNo = -1, nearNo = -1;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;//
            visiNo = TryGetBandNo(bandNameRaster, "Visible");//(int)_argumentProvider.GetArg("Visible");
            nearNo = TryGetBandNo(bandNameRaster, "NearInfrared");//(int)_argumentProvider.GetArg("NearInfrared");
            shortNo = TryGetBandNo(bandNameRaster, "ShortInfrared");//(int)_argumentProvider.GetArg("ShortInfrared");
            farNo = TryGetBandNo(bandNameRaster, "FarInfrared"); //(int)_argumentProvider.GetArg("FarInfrared");
            double visiZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double farZoom = (double)_argumentProvider.GetArg("FarInfrared_Zoom");
            double shortZoom = (double)_argumentProvider.GetArg("ShortInfrared_Zoom");
            double nearZoom = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
            float ShortFarInfraredVar = (float)_argumentProvider.GetArg("ShortInfraredFarInfraredVar");

            if (visiNo == -1 || farNo == -1 || shortNo == -1 || nearNo == -1 || visiZoom == -1 || farZoom == -1 || shortZoom == -1 || nearZoom == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return false;
            }
            if (visiNo > bandCount)
            {
                PrintInfo("可见光波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (nearNo > bandCount)
            {
                PrintInfo("近红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (shortNo > bandCount)
            {
                PrintInfo("短波红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (farNo > bandCount)
            {
                PrintInfo("远红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }

            string express = "((band" + visiNo + " / " + visiZoom + "f) > var_VisibleMin) && (band" + visiNo + " / " + visiZoom + "f < var_VisibleMax) && (band"
                           + farNo + " / " + farZoom + "f > var_FarInfraredMin) && (band" + farNo + " / " + farZoom + "f < var_FarInfraredMax) &&"
                           + "(band" + shortNo + " / " + shortZoom + "f > var_ShortInfraredMin) && (band" + nearNo + " / " + nearZoom + "f > var_NearInfraredMin) && (band"
                           + nearNo + " / " + nearZoom + "f < var_NearInfraredMax) && ((band" + shortNo + "/" + shortZoom + "f - band" + nearNo + "/" + nearZoom + "f) > var_ShortInfraredNearInfraredMin) &&"
                           + "((band" + shortNo + " / " + shortZoom + "f - band" + farNo + " / " + farZoom + "f + var_ShortInfraredFarInfraredVar) > var_ShortInfraredFarInfraredMin)";
            int[] bandNos = new int[] { visiNo, farNo, shortNo, nearNo };
            SandDustExtract(express, bandNos, ref result);
            try
            {
                DstFeatureCollection featureInfo = new DstFeatureCollection("沙尘辅助信息", GetDisplayInfo(visiNo, shortNo, nearNo, -1, farNo, (float)(ShortFarInfraredVar * farZoom)));
                result.Tag = featureInfo;
            }
            catch
            {
                result.Tag = new DstFeatureCollection("沙尘辅助信息计算失败", null);
            }
            return true;
        }

        /// <summary>
        /// NOAA17海洋
        /// </summary>
        /// <param name="_argumentProvider"></param>
        /// <param name="result"></param>
        private bool DustExtractNoaa17Sea(int bandCount, ref IPixelIndexMapper result)
        {
            int visiNo = -1, farNo = -1, shortNo = -1, nearNo = -1;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;//
            visiNo = TryGetBandNo(bandNameRaster, "Visible");//(int)_argumentProvider.GetArg("Visible");
            nearNo = TryGetBandNo(bandNameRaster, "NearInfrared");//(int)_argumentProvider.GetArg("NearInfrared");
            shortNo = TryGetBandNo(bandNameRaster, "ShortInfrared");//(int)_argumentProvider.GetArg("ShortInfrared");
            farNo = TryGetBandNo(bandNameRaster, "FarInfrared"); //(int)_argumentProvider.GetArg("FarInfrared");
            double visiZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double farZoom = (double)_argumentProvider.GetArg("FarInfrared_Zoom");
            double shortZoom = (double)_argumentProvider.GetArg("ShortInfrared_Zoom");
            double nearZoom = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
            float ShortFarInfraredVar = (float)_argumentProvider.GetArg("ShortInfraredFarInfraredVar");

            if (visiNo == -1 || farNo == -1 || shortNo == -1 || nearNo == -1 || visiZoom == -1 || farZoom == -1 || shortZoom == -1 || nearZoom == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return false;
            }
            if (visiNo > bandCount)
            {
                PrintInfo("可见光波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (nearNo > bandCount)
            {
                PrintInfo("近红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (shortNo > bandCount)
            {
                PrintInfo("短波红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (farNo > bandCount)
            {
                PrintInfo("远红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            string express = "((band" + visiNo + " / " + visiZoom + "f) > var_VisibleMin) && (band" + visiNo + " / " + visiZoom + "f < var_VisibleMax) && (band"
                           + farNo + " / " + farZoom + "f > var_FarInfraredMin) && (band" + farNo + " / " + farZoom + "f < var_FarInfraredMax) &&"
                           + "(band" + shortNo + " / " + shortZoom + "f > var_ShortInfraredMin) &&((band" + shortNo + "/" + shortZoom + "f - band"
                           + nearNo + "/" + nearZoom + "f)>var_ShortInfraredNearInfraredMin) &&((band" + visiNo + " / " + visiZoom + "f - band"
                           + nearNo + "/" + nearZoom + "f )>var_VisibleNearInfraredMin) &&((band" + shortNo + "/" + shortZoom + "f - band"
                           + farNo + " / " + farZoom + "f + var_ShortInfraredFarInfraredVar)>var_ShortInfraredFarInfraredMin)";
            int[] bandNos = new int[] { visiNo, farNo, shortNo, nearNo };
            SandDustExtract(express, bandNos, ref result);
            try
            {
                DstFeatureCollection featureInfo= new DstFeatureCollection("沙尘辅助信息", GetDisplayInfo(visiNo, shortNo, nearNo, -1, farNo, (float)(ShortFarInfraredVar * farZoom)));
                result.Tag = featureInfo;
            }
            catch
            {
                result.Tag = new DstFeatureCollection("沙尘辅助信息计算失败", null);
            }
            return true;
        }

        /// <summary>
        /// NOAA 16 18/FY 2C 2D 2E 陆地
        /// </summary>
        /// <param name="_argumentProvider"></param>
        /// <param name="result"></param>
        private bool DustExtractNoaa18Land(int bandCount, ref IPixelIndexMapper result)
        {
            int visiNo = -1, farNo = -1, midNo = -1;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;//
            visiNo = TryGetBandNo(bandNameRaster, "Visible");//(int)_argumentProvider.GetArg("Visible");
            midNo = TryGetBandNo(bandNameRaster, "MiddleInfrared");//(int)_argumentProvider.GetArg("NearInfrared");
            farNo = TryGetBandNo(bandNameRaster, "FarInfrared"); //(int)_argumentProvider.GetArg("FarInfrared");
            double visiZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double farZoom = (double)_argumentProvider.GetArg("FarInfrared_Zoom");
            double midZoom = (double)_argumentProvider.GetArg("MiddleInfrared_Zoom");

            if (visiNo == -1 || midNo == -1 || farNo == -1 || visiZoom == -1 || farZoom == -1 || midZoom == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return false;
            }
            if (visiNo > bandCount)
            {
                PrintInfo("可见光波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (midNo > bandCount)
            {
                PrintInfo("中波红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (farNo > bandCount)
            {
                PrintInfo("远红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }

            string express = "((band" + visiNo + " / " + visiZoom + "f) > var_VisibleMin) && (band" + visiNo + " / " + visiZoom + "f < var_VisibleMax) && (band"
                            + farNo + " / " + farZoom + "f > var_FarInfraredMin) && (band" + farNo + " / " + farZoom + "f < var_FarInfraredMax) && "
                            + "(band" + midNo + "/" + midZoom + "f > var_MiddleInfraredMin) &&((band" + midNo + " /" + midZoom + "f - band" + farNo + " /" + farZoom + "f)> var_MiddleInfraredFarInfraredMin)";
            int[] bandNos = new int[] { visiNo, farNo, midNo };
            SandDustExtract(express, bandNos, ref result);
            try
            {
                DstFeatureCollection featureInfo= new DstFeatureCollection("沙尘辅助信息", GetDisplayInfo(visiNo, -1, -1, midNo, farNo, 9999));
                result.Tag = featureInfo;
            }
            catch
            {
                result.Tag = new DstFeatureCollection("沙尘辅助信息计算失败", null);
            }
            return true;
        }

        /// <summary>
        /// NOAA 16 18/FY 2C 2D 2E 海洋
        /// </summary>
        /// <param name="_argumentProvider"></param>
        /// <param name="result"></param>
        private bool DustExtractNoaa18Sea(int bandCount, ref IPixelIndexMapper result)
        {
            int visiNo = -1, farNo = -1, midNo = -1;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;//
            visiNo = TryGetBandNo(bandNameRaster, "Visible");//(int)_argumentProvider.GetArg("Visible");
            midNo = TryGetBandNo(bandNameRaster, "MiddleInfrared");//(int)_argumentProvider.GetArg("NearInfrared");
            farNo = TryGetBandNo(bandNameRaster, "FarInfrared"); //(int)_argumentProvider.GetArg("FarInfrared");
            double visiZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double farZoom = (double)_argumentProvider.GetArg("FarInfrared_Zoom");
            double midZoom = (double)_argumentProvider.GetArg("MiddleInfrared_Zoom");

            if (visiNo == -1 || midNo == -1 || farNo == -1 || visiZoom == -1 || farZoom == -1 || midZoom == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return false;
            }
            if (visiNo > bandCount)
            {
                PrintInfo("可见光波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (midNo > bandCount)
            {
                PrintInfo("中波红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (farNo > bandCount)
            {
                PrintInfo("远红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }

            string express = "((band" + visiNo + " / " + visiZoom + "f) > var_VisibleMin) && (band" + visiNo + " / " + visiZoom + "f < var_VisibleMax) && (band"
                            + farNo + " / " + farZoom + "f > var_FarInfraredMin) && (band" + farNo + " / " + farZoom + "f < var_FarInfraredMax) && "
                            + "(band" + midNo + "/" + midZoom + "f > var_MiddleInfraredMin) &&((band" + midNo + "/" + midZoom + "f - band" + farNo + "/" + farZoom + "f )> var_MiddleInfraredFarInfraredMin)";
            int[] bandNos = new int[] { visiNo, farNo, midNo };
            SandDustExtract(express, bandNos, ref result);
            try
            {
                DstFeatureCollection featureInfo=new DstFeatureCollection("沙尘辅助信息", GetDisplayInfo(visiNo, -1, -1, midNo, farNo, 9999));
                result.Tag =featureInfo;
            }
            catch
            {
                result.Tag = new DstFeatureCollection("沙尘辅助信息计算失败", null); 
            }
            return true;
        }

        /// <summary>
        /// MODIS 陆地
        /// </summary>
        /// <param name="_argumentProvider"></param>
        /// <param name="result"></param>
        private bool DustExtractEosLand(int bandCount, ref IPixelIndexMapper result)
        {
            int visiNo = -1, shortNo = -1, midNo = -1, farNo = -1;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;//
            visiNo = TryGetBandNo(bandNameRaster, "Visible");//(int)_argumentProvider.GetArg("Visible");
            midNo = TryGetBandNo(bandNameRaster, "MiddleInfrared");//(int)_argumentProvider.GetArg("NearInfrared");
            farNo = TryGetBandNo(bandNameRaster, "FarInfrared"); //(int)_argumentProvider.GetArg("FarInfrared");
            shortNo = TryGetBandNo(bandNameRaster, "ShortInfrared");//(int)_argumentProvider.GetArg("ShortInfrared");
            double visiZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double shortZoom = (double)_argumentProvider.GetArg("ShortInfrared_Zoom");
            double midZoom = (double)_argumentProvider.GetArg("MiddleInfrared_Zoom");
            double farZoom = (double)_argumentProvider.GetArg("FarInfrared_Zoom");

            if (visiNo == -1 || midNo == -1 || shortNo == -1 || farNo == -1 || visiZoom == -1 || farZoom == -1 || shortZoom == -1 || midZoom == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return false;
            }
            if (visiNo > bandCount)
            {
                PrintInfo("可见光波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (midNo > bandCount)
            {
                PrintInfo("中波红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (shortNo > bandCount)
            {
                PrintInfo("短波红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (farNo > bandCount)
            {
                PrintInfo("远红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }

            string express = "((band" + visiNo + " / " + visiZoom + "f) > var_VisibleMin) && (band" + visiNo + " / " + visiZoom + "f < var_VisibleMax) && (band"
               + shortNo + "/" + shortZoom + "f > var_ShortInfraredMin) && (band" + shortNo + "/" + shortZoom + "f < var_ShortInfraredMax) && "
               + "(band" + midNo + "/" + midZoom + "f > var_MiddleInfraredMin) && (band" + midNo + "/" + midZoom + "f < var_MiddleInfraredMax) && (band"
               + farNo + " / " + farZoom + "f > var_FarInfraredMin) && (band" + farNo + " / " + farZoom + "f < var_FarInfraredMax)";

            int[] bandNos = new int[] { visiNo, shortNo, midNo, farNo };
            SandDustExtract(express, bandNos, ref result);
            try
            {
                DstFeatureCollection featureInfo = new DstFeatureCollection("沙尘辅助信息", GetDisplayInfo(visiNo, shortNo, -1, midNo, farNo, 9999));
                result.Tag = featureInfo;
            }
            catch
            {
                result.Tag = new DstFeatureCollection("沙尘辅助信息计算失败", null);
            }
            return true;
        }

        /// <summary>
        /// MODIS 海洋
        /// </summary>
        /// <param name="_argumentProvider"></param>
        /// <param name="result"></param>
        private bool DustExtractEosSea(int bandCount, ref IPixelIndexMapper result)
        {
            int visiNo = -1, shortNo = -1, midNo = -1, farNo = -1;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;//
            visiNo = TryGetBandNo(bandNameRaster, "Visible");//(int)_argumentProvider.GetArg("Visible");
            midNo = TryGetBandNo(bandNameRaster, "MiddleInfrared");//(int)_argumentProvider.GetArg("NearInfrared");
            farNo = TryGetBandNo(bandNameRaster, "FarInfrared"); //(int)_argumentProvider.GetArg("FarInfrared");
            shortNo = TryGetBandNo(bandNameRaster, "ShortInfrared");//(int)_argumentProvider.GetArg("ShortInfrared");
            if (visiNo == -1 || midNo == -1 || shortNo == -1 || farNo == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return false;
            }
            if (visiNo > bandCount)
            {
                PrintInfo("可见光波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (midNo > bandCount)
            {
                PrintInfo("中波红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (shortNo > bandCount)
            {
                PrintInfo("短波红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }
            if (farNo > bandCount)
            {
                PrintInfo("远红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return false;
            }

            double visiZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            if (visiZoom < float.Epsilon)
            {
                PrintInfo("可见光波段数值缩放参数设置错误。");
                return false;
            }
            double shortZoom = (double)_argumentProvider.GetArg("ShortInfrared_Zoom");
            if (shortZoom < float.Epsilon)
            {
                PrintInfo("短波红外波段数值缩放参数设置错误。");
                return false;
            }
            double midZoom = (double)_argumentProvider.GetArg("MiddleInfrared_Zoom");
            if (midZoom < float.Epsilon)
            {
                PrintInfo("中波红外波段数值缩放参数设置错误。");
                return false;
            }
            double farZoom = (double)_argumentProvider.GetArg("FarInfrared_Zoom");
            if (farZoom < float.Epsilon)
            {
                PrintInfo("远红外波段数值缩放参数设置错误。");
                return false;
            }

            string express = "((band" + visiNo + " / " + visiZoom + "f) > var_VisibleMin) && (band" + visiNo + " / " + visiZoom + "f < var_VisibleMax) && (band"
                           + shortNo + "/" + shortZoom + "f > var_ShortInfraredMin) && (band" + shortNo + "/" + shortZoom + "f < var_ShortInfraredMax) && "
                           + "(band" + midNo + "/" + midZoom + "f > var_MiddleInfraredMin) && (band" + midNo + "/" + midZoom + "f < var_MiddleInfraredMax) && (band"
                           + farNo + " / " + farZoom + "f > var_FarInfraredMin) && (band" + farNo + " / " + farZoom + "f < var_FarInfraredMax)";
            int[] bandNos = new int[] { visiNo, shortNo, midNo, farNo };
            SandDustExtract(express, bandNos, ref result);
            try
            {
                DstFeatureCollection featureInfo = new DstFeatureCollection("沙尘辅助信息", GetDisplayInfo(visiNo, shortNo, -1, midNo, farNo, 9999));
                result.Tag = featureInfo;
            }
            catch
            {
                result.Tag = new DstFeatureCollection("沙尘辅助信息计算失败", null);
            }
            return true;
        }

        private void SandDustExtract(string express, int[] bandNos, ref IPixelIndexMapper result)
        {
            IThresholdExtracter<UInt16> extracter = new SimpleThresholdExtracter<UInt16>();
            extracter.Reset(_argumentProvider, bandNos, express);
            extracter.Extract(result);
        }

        private Dictionary<int, DstFeatureFY2> GetFY2DisplayInfo(int[] bandNos,IRasterDataProvider backRaster,IRasterDataProvider solZenRaster)
        {
            if (_argumentProvider.DataProvider == null)
                return null;
            if (backRaster == null)
                return null;
            VirtualRasterDataProvider virtualPrd = new VirtualRasterDataProvider(new IRasterDataProvider[] {_argumentProvider.DataProvider,
                                                                                                           backRaster,solZenRaster});
            ArgumentProvider argPrd = new Core.ArgumentProvider(virtualPrd,null);
            Dictionary<int, DstFeatureFY2> features = new Dictionary<int, DstFeatureFY2>();
            DstFeatureFY2 tempSnw = null;
            RasterPixelsVisitor<Int16> rpVisitor = new RasterPixelsVisitor<Int16>(argPrd);
            if (bandNos == null || bandNos.Length != 5)
                return null;
            List<int> bandVisitorNo = new List<int>();
            bandVisitorNo.AddRange(bandNos);
            int bandIndex = _argumentProvider.DataProvider.BandCount + 1;
            bandVisitorNo.AddRange(new int[]{bandIndex,bandIndex+1});
            bandVisitorNo.Add(_argumentProvider.DataProvider.BandCount + backRaster.BandCount + 1);
            double cosValue,t2rb1;
            rpVisitor.VisitPixel(bandVisitorNo.ToArray(),
                (index, values) =>
                {
                    tempSnw = new DstFeatureFY2();
                    tempSnw.TBDBK12 = (values[5] - values[6]) / 10f;
                    tempSnw.TBD12 = (values[0] - values[1]) / 10f;
                    tempSnw.TBD13 = (values[0] - values[2])/10f;
                    tempSnw.TBD14 = (values[0] - values[3])/10f;
                    tempSnw.TBD2 = values[1]/10f;
                    tempSnw.TBD3 = values[2]/10f;
                    tempSnw.TBD4 = values[3]/10f;
                    tempSnw.IDDI = (values[5] - values[0])/10f;
                    cosValue=Math.Cos(DegreeToRadian(values[7]/100f));
                    tempSnw.Visref = (float)(values[4] / 1000f / cosValue);
                    t2rb1 = T2R(values[0]/10f, 3.75);
                    tempSnw.RefMidIR = (float)((T2R(values[3]/10f, 3.75) - t2rb1) / (10.72 * cosValue / 3.1416 - t2rb1));
                    features.Add(index, tempSnw);
                }
            );
            return features;
        }

        private Dictionary<int, DstFeature> GetDisplayInfo(int VisCH, int ShortInfraredCH, int NearInfraredCH, int MidInfraredCh,
                                                           int FarInfraredCH, float shortFarInfraredVar)
        {
            if (_argumentProvider.DataProvider == null)
                return null;
            Dictionary<int, DstFeature> features = new Dictionary<int, DstFeature>();
            DstFeature tempSnw = null;
            RasterPixelsVisitor<UInt16> rpVisitor = new RasterPixelsVisitor<UInt16>(_argumentProvider);
            int bandIndex = 0, shortInfraredNo = 0, nearInfraredNo = 0, midInfraredNo = 0, farInfraredNo = 0;
            List<int> CHList = new List<int>();
            if (VisCH != -1)
                CHList.Add(VisCH);
            if (ShortInfraredCH != -1)
            {
                CHList.Add(ShortInfraredCH);
                shortInfraredNo = ++bandIndex;
            }
            if (NearInfraredCH != -1)
            {
                CHList.Add(NearInfraredCH);
                nearInfraredNo = ++bandIndex;
            }
            if (MidInfraredCh != -1)
            {
                CHList.Add(MidInfraredCh);
                midInfraredNo = ++bandIndex;
            }
            if (FarInfraredCH != -1)
            {
                CHList.Add(FarInfraredCH);
                farInfraredNo = ++bandIndex;
            }

            rpVisitor.VisitPixel(CHList.ToArray(),
                (index, values) =>
                {
                    tempSnw = new DstFeature();
                    tempSnw.Vis = VisCH == -1 ? (UInt16)9999 : values[0];
                    tempSnw.ShortInfrared = ShortInfraredCH == -1 ? (UInt16)9999 : values[shortInfraredNo];
                    tempSnw.NearInfrared = NearInfraredCH == -1 ? (UInt16)9999 : values[nearInfraredNo];
                    tempSnw.MidInfrared = MidInfraredCh == -1 ? (UInt16)9999 : values[midInfraredNo];
                    tempSnw.FarInfrared = FarInfraredCH == -1 ? (UInt16)9999 : values[farInfraredNo];
                    tempSnw.ShortVis = VisCH == -1 || ShortInfraredCH == -1 ? (Int16)9999 : (Int16)(values[shortInfraredNo] - values[0]);
                    tempSnw.ShortNearInfrared = NearInfraredCH == -1 || ShortInfraredCH == -1 ? (Int16)9999 : (Int16)(values[shortInfraredNo] - values[nearInfraredNo]);
                    tempSnw.ShortFarInfrared = FarInfraredCH == -1 || ShortInfraredCH == -1 || shortFarInfraredVar == 9999 ? (Int16)9999 : (Int16)(values[shortInfraredNo] - (values[farInfraredNo] - shortFarInfraredVar));
                    tempSnw.MidFarInfrared = MidInfraredCh == -1 || FarInfraredCH == -1 ? (Int16)9999 : (Int16)(values[midInfraredNo] - values[farInfraredNo]);
                    features.Add(index, tempSnw);
                }
            );
            return features;
        }

        private double DegreeToRadian(double degrees)
        {
            return degrees * Math.PI / 180f;
        }

        //public override IExtractResult MakeExtProduct(IPixelIndexMapper piexd, Action<int, string> progressTracker)
        //{

        //    return null;
        //}

        private Envelope GetEnvelope(out Size size, IRasterDataProvider prd)
        {
            size = new System.Drawing.Size();
            size.Width = prd.Width;
            size.Height = prd.Height;
            return new Envelope(prd.CoordEnvelope.MinX, prd.CoordEnvelope.MinY, prd.CoordEnvelope.MaxX, prd.CoordEnvelope.MaxY);
        }
        public override IExtractResult MakeExtProduct(IPixelIndexMapper piexd, Action<int, string> progressTracker)
        {
            //生成判识结果文件
            IInterestedRaster<UInt16> iir = null;
            RasterIdentify id = new RasterIdentify(_argumentProvider.DataProvider.fileName);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "DST";
            id.SubProductIdentify = _identify;
            id.GenerateDateTime = DateTime.Now;
            iir = new InterestedRaster<UInt16>(id, piexd.Size, piexd.CoordEnvelope);
            int[] idxs = piexd.Indexes.ToArray();
            iir.Put(idxs, 1);
            //原始影像raster
            IRasterDataProvider sourceraster = _argumentProvider.DataProvider as IRasterDataProvider;
            List<RasterMaper> listRaster = new List<RasterMaper>();
            RasterMaper rmsoure = new RasterMaper(sourceraster, GetBandArray(sourceraster.BandCount));
            RasterMaper rmpiexd = new RasterMaper(iir.HostDataProvider, new int[] { 1 });
            int totalbandcount = sourceraster.BandCount;
            listRaster.Add(rmpiexd);
            listRaster.Add(rmsoure);
            try
            {
                string outFileName = GetFileName(new string[] { _argumentProvider.DataProvider.fileName }, _subProductDef.ProductDef.Identify, "SRDA", ".ldf", null);
                using (IRasterDataProvider outRaster = CreateOutM_BandRaster(outFileName, listRaster.ToArray(), totalbandcount))
                {
                    //波段总数

                    RasterMaper[] fileIns = listRaster.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, GetBandArray(totalbandcount)) };
                    //创建处理模型
                    RasterProcessModel<UInt16, UInt16> rfr = null;
                    rfr = new RasterProcessModel<UInt16, UInt16>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.SetFeatureAOI(_argumentProvider.AOIs);
                    rfr.RegisterCalcModel(new RasterCalcHandler<UInt16, UInt16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        if (rvInVistor[0].RasterBandsData == null || rvInVistor[1].RasterBandsData == null ||
                            rvInVistor[0].RasterBandsData[0] == null || rvInVistor[1].RasterBandsData[0] == null)
                            return;
                        // if (_argumentProvider.AOIs == null)
                        for (int index = 0; index < dataLength; index++)
                        {
                            if (rvInVistor[0].RasterBandsData[0][index] == 1)
                                for (int i = 0; i < totalbandcount; i++)
                                    rvOutVistor[0].RasterBandsData[i][index] = rvInVistor[1].RasterBandsData[i][index];
                        }
                    }
                    ));
                    //执行
                    rfr.Excute(0);
                }
                string dstfilename = outFileName.Replace(".ldf", ".dat");
                if (File.Exists(dstfilename))
                {
                    File.Delete(dstfilename);
                }
                File.Move(outFileName, dstfilename);
                FileExtractResult res = new FileExtractResult("DST", dstfilename, true);

                res.SetDispaly(false);
                return res;
            }
            finally
            {
                iir.Dispose();
                if (File.Exists(iir.FileName))
                    File.Delete(iir.FileName);
            }

        }

        private int[] GetBandArray(int bandcount)
        {
            List<int> listband = new List<int>();
            for (int i = 1; i <= bandcount; i++)
            {
                listband.Add(i);
            }
            return listband.ToArray();
        }
        protected IRasterDataProvider CreateOutM_BandRaster(string outFileName, RasterMaper[] inrasterMaper, int bandcount)
        {


            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("LDF") as IRasterDataDriver;


            CoordEnvelope outEnv = null;

            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv.Union(inRaster.Raster.CoordEnvelope);
            }
            float resX = inrasterMaper[0].Raster.ResolutionX;
            float resY = inrasterMaper[0].Raster.ResolutionY;
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            string[] optionString = new string[]{
                    "INTERLEAVE=BSQ",
                    "VERSION=LDF",
                    "WITHHDR=TRUE",
                    "SPATIALREF=" +inrasterMaper[0].Raster.SpatialRef==null?"":("SPATIALREF=" +inrasterMaper[0].Raster.SpatialRef.ToProj4String()),
                    "MAPINFO={" + 1 + "," + 1 + "}:{" + outEnv.MinX + "," + outEnv.MaxY + "}:{" + resX + "," + resY + "}"
                    };
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, bandcount, enumDataType.UInt16, optionString) as RasterDataProvider;
            return outRaster;
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
       
    }
}
