using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.Drawing;
using GeoDo.Project;
using GeoDo.RSS.MIF.Prds.Comm;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.MIF.Prds.LST
{
    public class SubProductBinaryLST : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private List<int> _indexiex = new List<int>();
        private float _tempzoom = 1000;


        public SubProductBinaryLST(SubProductDef subProductDef)
            : base(subProductDef)
        {
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
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            //修改 增加11 12 14 卫星算法
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "NOAA16Algorithm" ||
                _argumentProvider.GetArg("AlgorithmName").ToString() == "NOAA18Algorithm" ||
                _argumentProvider.GetArg("AlgorithmName").ToString() == "NOAA11Algorithm" ||
                _argumentProvider.GetArg("AlgorithmName").ToString() == "NOAA12Algorithm" ||
                _argumentProvider.GetArg("AlgorithmName").ToString() == "NOAA14Algorithm" ||
                _argumentProvider.GetArg("AlgorithmName").ToString() == "NOAA7Algorithm" ||
                _argumentProvider.GetArg("AlgorithmName").ToString() == "NOAA9Algorithm" ||
                _argumentProvider.GetArg("AlgorithmName").ToString() == "FY3Algorithm")
            {
                Dictionary<string, string[]> pathDic = _argumentProvider.GetArg("OrbitFileSelectType") as Dictionary<string, string[]>;
                if (pathDic == null || pathDic.Keys.Contains("CurrentRaster")) //选择当前影像进行计算
                    return ComputeByCurrentRaster(progressTracker);
                if (pathDic.Keys.Contains("DirectoryPath")) //选择局地文件夹路径
                    return ExtractLST(pathDic["DirectoryPath"][0]);
                if (pathDic.Keys.Contains("FileNames")) //选择多个文件进行计算
                    return ComputeByFiles(pathDic["FileNames"]);
                return null;
            }
            else if (_argumentProvider.GetArg("AlgorithmName").ToString() == "FY2DAlgorithm" ||
                     _argumentProvider.GetArg("AlgorithmName").ToString() == "FY2EAlgorithm")
            {
                Dictionary<string, string[]> pathDic = _argumentProvider.GetArg("OrbitFileSelectType") as Dictionary<string, string[]>;
                if (pathDic == null || pathDic.Keys.Contains("CurrentRaster")) //选择当前影像进行计算
                    return ComputeByFilesGeostationary(progressTracker, new string[] { _argumentProvider.DataProvider.fileName });
                if (pathDic.Keys.Contains("DirectoryPath")) //选择局地文件夹路径
                    return ExtractLSTGeostationary(progressTracker, pathDic["DirectoryPath"][0]);
                if (pathDic.Keys.Contains("FileNames")) //选择多个文件进行计算
                    return ComputeByFilesGeostationary(progressTracker, pathDic["FileNames"]);
                return null;
            }
            else
            {
                PrintInfo("指定的算法没有实现。");
                return null;
            }
        }

        #region 极轨星计算

        private IExtractResult ComputeByCurrentRaster(Action<int, string> progressTracker)
        {
            MemPixelFeatureMapper<UInt16> resultTemp = null;
            float curNDVI = 0f;
            IRasterDataProvider currPrd = _argumentProvider.DataProvider;
            if (currPrd == null)
                return null;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;//
            int VisibleCH = TryGetBandNo(bandNameRaster, "Visible"); //(int)_argumentProvider.GetArg("Visible");
            int NearInfraredCH = TryGetBandNo(bandNameRaster, "NearInfrared"); //(int)_argumentProvider.GetArg("NearInfrared");
            int MiddInfraredCH = TryGetBandNo(bandNameRaster, "MiddInfrared"); //(int)_argumentProvider.GetArg("MiddInfrared");
            int FarInfrared11CH = TryGetBandNo(bandNameRaster, "FarInfrared11"); //(int)_argumentProvider.GetArg("FarInfrared11");
            int FarInfrared12CH = TryGetBandNo(bandNameRaster, "FarInfrared12"); //(int)_argumentProvider.GetArg("FarInfrared12");
            double VisibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double NearInfraredZoom = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
            double MiddInfraredZoom = (double)_argumentProvider.GetArg("MiddInfrared_Zoom");
            double FarInfrared11Zoom = (double)_argumentProvider.GetArg("FarInfrared11_Zoom");
            double FarInfrared12Zoom = (double)_argumentProvider.GetArg("FarInfrared12_Zoom");
            bool isAutoCloud = (bool)_argumentProvider.GetArg("isAutoCloud");

            if (VisibleCH == -1 || NearInfraredCH == -1 || MiddInfraredCH == -1 || FarInfrared11CH == -1 || FarInfrared12CH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            bool useCLMFile = true;
            string clmRasterFile = _argumentProvider.GetArg("clmFile").ToString();
            if (string.IsNullOrEmpty(clmRasterFile) || !File.Exists(clmRasterFile))
                useCLMFile = false;

            double lstZoom = (double)_argumentProvider.GetArg("LSTZoom");
            UInt16 defCloudy = (UInt16)_argumentProvider.GetArg("defCloudy");
            UInt16 defWater = (UInt16)_argumentProvider.GetArg("defWater");

            float a0 = (float)_argumentProvider.GetArg("A0");
            float alfa = (float)_argumentProvider.GetArg("alfa");
            float beta = (float)_argumentProvider.GetArg("beta");
            float gama = (float)_argumentProvider.GetArg("gama");
            float alfa2 = (float)_argumentProvider.GetArg("alfa2");
            float beta2 = (float)_argumentProvider.GetArg("beta2");

            float b4m = (float)_argumentProvider.GetArg("b4m");
            float b4n = (float)_argumentProvider.GetArg("b4n");
            float b5m = (float)_argumentProvider.GetArg("b5m");
            float b5n = (float)_argumentProvider.GetArg("b5n");

            float b4Water = (float)_argumentProvider.GetArg("b4Water");
            float b5Water = (float)_argumentProvider.GetArg("b5Water");
            float b4Soil = (float)_argumentProvider.GetArg("b4Soil");
            float b5Soil = (float)_argumentProvider.GetArg("b5Soil");
            float b4VGT = (float)_argumentProvider.GetArg("b4VGT");
            float b5VGT = (float)_argumentProvider.GetArg("b5VGT");

            float NearInfraredCLMMin = float.Parse(_argumentProvider.GetArg("NearInfraredCLMMin").ToString());
            float FarInfrared11CLMMax = float.Parse(_argumentProvider.GetArg("FarInfrared11CLMMax").ToString());
            float FarInfrared1112CLMMin = float.Parse(_argumentProvider.GetArg("FarInfrared1112CLMMin").ToString());
            float FarInfrared11WaterMin = float.Parse(_argumentProvider.GetArg("FarInfrared11WaterMin").ToString());
            float NDVIWaterMax = float.Parse(_argumentProvider.GetArg("NDVIWaterMax").ToString());

            ArgumentProvider ap = new ArgumentProvider(currPrd, null);
            RasterPixelsVisitor<UInt16> rpVisitor = new RasterPixelsVisitor<UInt16>(ap);
            float b4Emiss = 0;
            float b5Emiss = 0;
            double pv = 0;
            //4 5 通道比辐射率均值 差值
            float EmissMean = 0;
            float DeltaEmiss = 0;
            //LST计算系数 P M
            double P = 0;
            double M = 0;

            UInt16 tempLstValue = 0;

            int[] aoi = _argumentProvider.AOI;
            Size size = new Size(currPrd.Width, currPrd.Height);
            Rectangle aoiRect = AOIHelper.ComputeAOIRect(aoi, size);
            if (progressTracker != null)
                progressTracker.Invoke(5, "开始计算LST,请稍候...");
            float progressJG = 60f / (size.Width * size.Height);
            resultTemp = new MemPixelFeatureMapper<UInt16>("DBLV", 1000, size, currPrd.CoordEnvelope, currPrd.SpatialRef);
            MemPixelFeatureMapper<float> ndviTemp = new MemPixelFeatureMapper<float>("NDVI", 1000, size, currPrd.CoordEnvelope, currPrd.SpatialRef);
            rpVisitor.VisitPixel(aoiRect, aoi, new int[] { VisibleCH, NearInfraredCH, MiddInfraredCH, FarInfrared11CH, FarInfrared12CH },
             (index, values) =>
             {
                 if (progressTracker != null)
                     progressTracker.Invoke((int)(5 + progressJG * index), "正在计算LST,请稍候...");
                 curNDVI = GetNDVI(values, 1, 0);
                 ndviTemp.Put(index, curNDVI);
                 if (values[0] == 0)
                 {
                     resultTemp.Put(index, 0);
                     ndviTemp.Put(index, 0);
                 }
                 else if (isAutoCloud && (values[1] / NearInfraredZoom > NearInfraredCLMMin && values[3] / FarInfrared11Zoom < FarInfrared11CLMMax && Math.Abs(values[2] / MiddInfraredZoom - values[3] / FarInfrared11Zoom) > FarInfrared1112CLMMin))
                 {
                     resultTemp.Put(index, defCloudy);
                     ndviTemp.Put(index, defCloudy / _tempzoom);
                 }
                 else
                 {
                     if (values[3] / FarInfrared11Zoom > FarInfrared11WaterMin && curNDVI < NDVIWaterMax)
                     {
                         resultTemp.Put(index, defWater);
                         ndviTemp.Put(index, defWater / _tempzoom);
                         b4Emiss = b4Water;
                         b5Emiss = b5Water;
                     }
                     else
                     {
                         if (curNDVI < 0.2)
                         {
                             b4Emiss = b4Soil;
                             b5Emiss = b5Soil;
                         }
                         else if (curNDVI > 0.5)
                         {
                             b4Emiss = b4VGT;
                             b5Emiss = b5VGT;
                         }
                         else if (curNDVI >= 0.2 && curNDVI <= 0.5)
                         {
                             pv = Math.Pow((curNDVI - 0.2) / (0.5 - 0.2), 2);
                             b4Emiss = (float)(b4m * pv + b4n);
                             b5Emiss = (float)(b5m * pv + b5n);
                         }

                         EmissMean = (b4Emiss + b5Emiss) / 2;
                         DeltaEmiss = (b4Emiss - b5Emiss);

                         P = 1 + alfa * (1 - EmissMean) / EmissMean + beta * DeltaEmiss / Math.Pow(EmissMean, 2);
                         M = gama + alfa2 * (1 - EmissMean) / EmissMean + beta2 * DeltaEmiss / Math.Pow(EmissMean, 2);

                         tempLstValue = (UInt16)((a0 + P * (values[3] / FarInfrared11Zoom + values[4] / FarInfrared12Zoom) / 2
                                                 + M * (values[3] / FarInfrared11Zoom - values[4] / FarInfrared12Zoom) / 2) * lstZoom);
                         resultTemp.Put(index, tempLstValue);
                     }
                 }
             });
            AddClmProcess(resultTemp, currPrd, true, progressTracker);
            if (progressTracker != null)
                progressTracker.Invoke(65, "开始进行LST修正,请稍候...");
            IFileExtractResult qcFile = null;
            IExtractResult newResult = TryModifyLstZYP(resultTemp, currPrd, defCloudy, defWater, size, ndviTemp, progressTracker, out qcFile);

            if (progressTracker != null)
                progressTracker.Invoke(95, "开始存储结果数据,请稍候...");
            IExtractResultArray arrary = new ExtractResultArray("LST");
            if (newResult != null)
            {
                arrary.Add(newResult as IFileExtractResult);
                arrary.Add(qcFile);
                return arrary;
            }
            else
            {
                IFileExtractResult dblvFile = GenrateIInterested(resultTemp, currPrd);
                if (dblvFile == null)
                    return null;
                arrary.Add(dblvFile as IExtractResultBase);
                GetLSQC(resultTemp, currPrd, defCloudy, defWater, size, ndviTemp, progressTracker, out qcFile);
                if (qcFile != null)
                    arrary.Add(qcFile);
                return arrary;
            }
        }

        private IFileExtractResult GetLSQC(MemPixelFeatureMapper<UInt16> resultTemp, IRasterDataProvider currPrd, UInt16 defCloudy, UInt16 defWater, Size size, MemPixelFeatureMapper<float> ndviTemp, Action<int, string> progressTracker, out IFileExtractResult qcFileResult)
        {
            qcFileResult = null;
            string argument = _argumentProvider.GetArg("DataCorrection") as string;
            if (string.IsNullOrEmpty(argument))
                return null;
            string[] argumentArry = argument.Split(new char[] { ';' });
            if (argumentArry.Length != 5)
                return null;
            string computeFile = argumentArry[0];
            if (string.IsNullOrEmpty(computeFile))
                return null;
            if (!string.IsNullOrEmpty(computeFile) && File.Exists(computeFile))
            {
                //读取000文件
                Dictionary<float[], float> txtValues = new Dictionary<float[], float>();
                txtValues = ReadTxtValues(computeFile);//常规观测数据
                if (txtValues != null && txtValues.Count > 0)
                {
                    //根据观测数据值获取待拟合x[],y[],ordValue为原始观测数据值
                    double[] lstValue, ndviValue, ordValue;
                    ObservationValue[] observationValues = null;//有效的常规观测数据。。。差值数据
                    GetFittingValues(txtValues, null, ndviTemp, resultTemp, "",
                        out ndviValue, out lstValue, out ordValue, out observationValues, defCloudy, defWater);
                    if (lstValue != null && observationValues != null
                        && lstValue.Length > 0 && observationValues.Length > 0)
                    {
                        bool isCreateQC = (bool)_argumentProvider.GetArg("isCreateQC");
                        if (isCreateQC)
                        {
                            List<string> rowInfo = new List<string>();
                            List<string[]> rows = new List<string[]>();
                            string[] columns = new string[] { "经度", "纬度", "站点", "0cm地温", "LST", "NDVI值", "地温与LST差异值" };
                            for (int i = 0; i < lstValue.Length; i++)
                            {
                                rowInfo.AddRange(new string[] { observationValues[i].Lon.ToString(), observationValues[i].Lat.ToString(), observationValues[i].station.ToString(), ToSSD(observationValues[i].Value).ToString(),
                                                                ToSSD(lstValue[i]).ToString(), ToDouble( observationValues[i].OtherValue,1000f).ToString(),(ToSSD(observationValues[i].Value) - ToSSD(lstValue[i])).ToString() });
                                rows.Add(rowInfo.ToArray());
                                rowInfo.Clear();
                            }
                            rows.Sort((bef, last) => Math.Abs(float.Parse(bef[6])) >= Math.Abs(float.Parse(last[6])) ? 1 : -1);
                            string title = "观测点精度检验信息图表\n";
                            IStatResult accuracyInfo = new StatResult(title, columns, rows.ToArray());
                            string xlsxFile = StatResultToFile(new string[] { currPrd.fileName.ToUpper() }, accuracyInfo, "LST", "LSQC", title, "", 1, 4, 2, false, 1, 2);
                            qcFileResult = new FileExtractResult("LST", xlsxFile);
                            qcFileResult.SetDispaly(false);
                        }
                    }
                }
            }
            return qcFileResult != null && File.Exists(qcFileResult.FileName) ? qcFileResult : null;
        }

        private IExtractResult TryModifyLstZYP(MemPixelFeatureMapper<UInt16> resultTemp, IRasterDataProvider currPrd, UInt16 defCloudy, UInt16 defWater, Size size, MemPixelFeatureMapper<float> ndviTemp, Action<int, string> progressTracker, out IFileExtractResult qcFileResult)
        {
            bool isRecompute = (bool)_argumentProvider.GetArg("isReCompute");
            qcFileResult = null;
            //进行LST结果订正
            if (isRecompute)
            {
                string argument = _argumentProvider.GetArg("DataCorrection") as string;
                if (string.IsNullOrEmpty(argument))
                    return null;
                string[] argumentArry = argument.Split(new char[] { ';' });
                if (argumentArry.Length != 5)
                    return null;
                string computeFile = argumentArry[0];
                string ndviMaxFile = argumentArry[1];
                string segFile = argumentArry[2];
                string csvFile = argumentArry[3];
                ushort minLST;
                if (!ushort.TryParse(argumentArry[4], out minLST))
                    return null;
                if (string.IsNullOrEmpty(ndviMaxFile))
                {
                    if (progressTracker != null)
                        progressTracker.Invoke(95, "正在写出临时NDVI数据,请稍候...");
                    ndviMaxFile = SaveNDVITempFile(ndviTemp, "TNDVI", 1000);
                }
                if (progressTracker != null)
                    progressTracker.Invoke(96, "继续进行LST修正,请稍候...");
                if (string.IsNullOrEmpty(computeFile) || !File.Exists(segFile)
                    || string.IsNullOrEmpty(ndviMaxFile) || !File.Exists(ndviMaxFile))
                    return null;
                List<float[]> ndviSeg = ReadSegValues(segFile);//ndvi分段区间
                FileExtractResult res = null;
                if (!string.IsNullOrEmpty(computeFile) && File.Exists(computeFile))
                {
                    //读取000文件
                    Dictionary<float[], float> txtValues = new Dictionary<float[], float>();
                    txtValues = ReadTxtValues(computeFile);//常规观测数据
                    if (txtValues != null && txtValues.Count > 0)
                    {
                        //根据观测数据值获取待拟合x[],y[],ordValue为原始观测数据值
                        double[] lstValue, ndviValue, ordValue;
                        ObservationValue[] observationValues = null;//有效的常规观测数据。。。差值数据
                        GetFittingValues(txtValues, null, ndviTemp, resultTemp, ndviMaxFile,
                            out ndviValue, out lstValue, out ordValue, out observationValues, defCloudy, defWater);
                        if (lstValue != null && observationValues != null
                            && lstValue.Length > 0 && observationValues.Length > 0)
                        {
                            List<double> detaTs = new List<double>();
                            for (int t = 0; t < ndviSeg.Count; t++)
                            {
                                float[] segs = ndviSeg[t];
                                if (segs == null || segs.Length != 2)
                                    detaTs.Add(0);
                                double dtSum = 0;
                                double dtCount = 0;
                                for (int i = 0; i < lstValue.Length; i++)
                                {
                                    double ndvi = observationValues[i].OtherValue;
                                    if (ndvi >= segs[0] && ndvi < segs[1] && lstValue[i] > minLST)
                                    {
                                        dtSum += observationValues[i].DetaT;
                                        dtCount++;
                                    }
                                }
                                if (dtCount == 0)
                                    detaTs.Add(0);
                                else
                                    detaTs.Add(dtSum / dtCount);
                            }
                            if (lstValue != null)
                            {
                                int orgDataLength = 0;
                                bool isResuleValue = false;
                                //保存lst原始结果
                                string lstfName = SaveTempFile(resultTemp, "DBLV");
                                //输出文件准备（作为输入栅格并集处理）
                                RasterIdentify ri = GetRasterIdentifyID(new string[] { currPrd.fileName });
                                string outFileName = ri.ToWksFullFileName(".dat");
                                //输入文件准备
                                List<RasterMaper> rms = new List<RasterMaper>();
                                try
                                {
                                    IRasterDataProvider lstPrd = RasterDataDriver.Open(lstfName) as IRasterDataProvider;
                                    RasterMaper rmLst = new RasterMaper(lstPrd, new int[] { 1 });
                                    rms.Add(rmLst);
                                    IRasterDataProvider ndviPrd = RasterDataDriver.Open(ndviMaxFile) as IRasterDataProvider;
                                    RasterMaper rmNdvi = new RasterMaper(ndviPrd, new int[] { 1 });
                                    rms.Add(rmNdvi);
                                    using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                                    {
                                        //栅格数据映射
                                        RasterMaper[] fileIns = rms.ToArray();
                                        RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                                        //创建处理模型
                                        RasterProcessModel<UInt16, UInt16> rfr = null;
                                        rfr = new RasterProcessModel<UInt16, UInt16>(progressTracker);
                                        rfr.SetRaster(fileIns, fileOuts);
                                        UInt16 tempValue = 0;
                                        UInt16 tempNdvi = 0;
                                        rfr.RegisterCalcModel(new RasterCalcHandler<UInt16, UInt16>((rvInVistor, rvOutVistor, aoi) =>
                                        {
                                            int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                                            for (int index = 0; index < dataLength; index++)
                                            {
                                                tempValue = rvInVistor[0].RasterBandsData[0][index];
                                                if (tempValue == 0 || tempValue == defCloudy || tempValue == defWater)//无效区域，不修正
                                                    rvOutVistor[0].RasterBandsData[0][index] = tempValue;
                                                else
                                                {
                                                    tempNdvi = rvInVistor[1].RasterBandsData[0][index];

                                                    for (int t = 0; t < ndviSeg.Count; t++)
                                                    {
                                                        float[] segs = ndviSeg[t];
                                                        if (tempNdvi >= segs[0] && tempNdvi < segs[1])
                                                        {
                                                            rvOutVistor[0].RasterBandsData[0][index] = (ushort)(tempValue - detaTs[t]);
                                                            if (_indexiex.Contains(orgDataLength + index))
                                                            {
                                                                GetObservationbByIndex(observationValues, orgDataLength + index).resuleValue = (ushort)(tempValue - detaTs[t]);
                                                                isResuleValue = true;
                                                            }
                                                        }
                                                    }
                                                    if (!isResuleValue && _indexiex.Contains(orgDataLength + index))
                                                    {
                                                        GetObservationbByIndex(observationValues, orgDataLength + index).resuleValue = (ushort)(tempValue);
                                                        rvOutVistor[0].RasterBandsData[0][index] = (ushort)(tempValue);
                                                    }
                                                    isResuleValue = false;
                                                }
                                            }
                                            orgDataLength += dataLength;
                                        }));
                                        //执行
                                        rfr.Excute();
                                        res = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                                        res.SetDispaly(false);
                                    }
                                }
                                finally
                                {
                                    foreach (RasterMaper rm in rms)
                                    {
                                        rm.Raster.Dispose();
                                    }
                                }


                                //Dictionary<string, FilePrdMap> filePrdMap = new Dictionary<string, FilePrdMap>();
                                //filePrdMap.Add("lstfName", new FilePrdMap(lstfName, 1, new VaildPra(short.MinValue, short.MaxValue), new int[] { 1 }));
                                //filePrdMap.Add("ndviMaxFile", new FilePrdMap(ndviMaxFile, 1, new VaildPra(short.MinValue, short.MaxValue), new int[] { 1 }));
                                //ITryCreateVirtualPrd tryVPrd = new TryCreateVirtualPrdByMultiFile();

                                ////int indextemp = 0;
                                //IVirtualRasterDataProvider vrd = null;
                                //try
                                //{
                                //    vrd = tryVPrd.CreateVirtualRasterPRD(ref filePrdMap);
                                //    if (vrd == null)
                                //        throw new Exception("数据间无相交部分,无法创建虚拟数据提供者!");

                                //    ArgumentProvider ap = new ArgumentProvider(vrd, null);

                                //    RasterPixelsVisitor<float> lstVisitor = new RasterPixelsVisitor<float>(ap);
                                //    Size vrdSize = new Size(vrd.Width, vrd.Height);
                                //    CoordEnvelope coord = vrd.CoordEnvelope;
                                //    //size,currPrd.CoordEnvelope,跟着两个比较下，可能有差异
                                //    MemPixelFeatureMapper<UInt16> result = new MemPixelFeatureMapper<UInt16>("DBLV", 1000, vrdSize, coord, currPrd.SpatialRef);
                                //    float progressJG = 30f / (size.Width * size.Height);
                                //    lstVisitor.VisitPixel(new int[] { 1, 2 },
                                //    (index, values) =>
                                //    {
                                //        if (progressTracker != null)
                                //            progressTracker.Invoke((int)(65 + progressJG * index), "正在修正LST,请稍候...");
                                //        if (values[0] == 0 || values[0] == defCloudy || values[0] == defWater)//无效区域，不修正
                                //            result.Put(index, (UInt16)values[0]);
                                //        else
                                //        {
                                //            for (int t = 0; t < ndviSeg.Count; t++)
                                //            {
                                //                float[] segs = ndviSeg[t];
                                //                if (values[1] >= segs[0] && values[1] < segs[1])
                                //                {
                                //                    result.Put(index, (ushort)(values[0] - detaTs[t]));
                                //                    if (_indexiex.Contains(index))
                                //                    {
                                //                        GetObservationbByIndex(observationValues, index).resuleValue = (ushort)(values[0] - detaTs[t]);
                                //                        //indextemp++;
                                //                    }
                                //                    return;
                                //                }
                                //            }
                                //            if (_indexiex.Contains(index))
                                //                GetObservationbByIndex(observationValues, index).resuleValue = (ushort)(values[0]);
                                //            result.Put(index, (ushort)(values[0]));
                                //        }
                                //    });
#if LSTTEST
                                bool isCreateQC = (bool)_argumentProvider.GetArg("isCreateQC");
                                if (isCreateQC)
                                {
                                    #region 填写速率加强后应用

                                    List<string> rowInfo = new List<string>();
                                    List<string[]> rows = new List<string[]>();
                                    string[] columns = new string[] { "经度", "纬度", "站点", "0cm地温", "LST计算值", "LST", "地温与计算差异值", "地温与LST差异值", "NDVI值", "LST计算平均值", "土地利用类型" };
                                    for (int i = 0; i < lstValue.Length; i++)
                                    {
                                        rowInfo.AddRange(new string[] { observationValues[i].Lon.ToString(), observationValues[i].Lat.ToString(), observationValues[i].station.ToString(), ToSSD(observationValues[i].Value).ToString(),  ToSSD(lstValue[i]).ToString(),  ToSSD(observationValues[i].resuleValue).ToString(), 
                                             ToDouble(observationValues[i].DetaT,10f).ToString(),(ToSSD(observationValues[i].Value)-ToSSD(observationValues[i].resuleValue)).ToString(),ToDouble( observationValues[i].OtherValue,1000f).ToString(),ToSSD( observationValues[i].avgLst).ToString(), ""});
                                        rows.Add(rowInfo.ToArray());
                                        rowInfo.Clear();
                                    }
                                    rows.Sort((bef, last) => Math.Abs(float.Parse(bef[7])) >= Math.Abs(float.Parse(last[7])) ? 1 : -1);
                                    string title = "观测点纠正精度检验信息图表\n";
                                    //for (int t = 0; t < detaTs.Count; t++)
                                    //{
                                    //    float[] segs = ndviSeg[t];
                                    //    title += string.Format("区间>={0}到<{1}的DetaT是{2}\n", segs[0], segs[1], detaTs[t]);
                                    //}
                                    IStatResult accuracyInfo = new StatResult(title, columns, rows.ToArray());
                                    string xlsxFile = StatResultToFile(new string[] { currPrd.fileName.ToUpper() }, accuracyInfo, "LST", "RLQC", title, "", 1, 4, 3, false, 1, 2);
                                    qcFileResult = new FileExtractResult("LST", xlsxFile);
                                    qcFileResult.SetDispaly(false);
                                }

                                    #endregion

#endif
                                return res;
                                //return GenrateIInterested(result, vrd, new RasterIdentify(currPrd.fileName.ToUpper()));

                                //}
                                //finally
                                //{
                                //    if (vrd != null)
                                //        vrd.Dispose();
                                //}
                            }

                        }
                    }
                }
            }
            return null;
        }

        private ObservationValue GetObservationbByIndex(ObservationValue[] observationValues, int index)
        {
            if (observationValues == null || observationValues.Length == 0)
                return null;
            foreach (ObservationValue item in observationValues)
            {
                if (item.index == index)
                    return item;
            }
            return null;
        }

        private object ToDouble(double srcValue, float zoom)
        {
            return Math.Round(srcValue / zoom, 1);
        }

        private double ToSSD(double temprature)
        {
            return temprature == 0 ? 0 : Math.Round((temprature - 2730) / 10f, 1);
        }

        private string GetCSVFilename(RasterIdentify rasterIdentify, string subIdentify)
        {
            rasterIdentify.ThemeIdentify = "CMA";
            rasterIdentify.ProductIdentify = "LST";
            rasterIdentify.SubProductIdentify = subIdentify;
            return rasterIdentify.ToWksFullFileName(".csv");
        }

        //读取分段，0-0.5。。。
        private List<float[]> ReadSegValues(string segFile)
        {
            if (string.IsNullOrEmpty(segFile) || !File.Exists(segFile))
                return null;
            float min, max;
            string[] pointInfos = File.ReadAllLines(segFile, Encoding.Default);
            List<float[]> valuesDic = new List<float[]>();
            for (int i = 0; i < pointInfos.Length; i++)
            {
                string[] values = pointInfos[i].Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length != 2)
                    continue;
                if (float.TryParse(values[0], out min) && float.TryParse(values[1], out max))
                {
                    valuesDic.Add(new float[] { min * 1000, max * 1000 });
                }
                else
                    continue;
            }
            return valuesDic;
        }

        private void GetFittingValues(Dictionary<float[], float> txtValues, float[] ab, MemPixelFeatureMapper<float> ndviTemp, MemPixelFeatureMapper<ushort> resultTemp,
            string ndviMaxValue,
            out double[] newNdviValue, out double[] lstValue, out double[] ordNDVIValue, out ObservationValue[] observationValues, UInt16 cloudy, UInt16 water)
        {
            _indexiex.Clear();
            List<double> lstList = new List<double>();
            List<double> ndviList = new List<double>();
            List<double> ordNDVIList = new List<double>();
            List<ObservationValue> observationValueList = new List<ObservationValue>();
            newNdviValue = null;
            lstValue = null;
            ordNDVIValue = null;
            observationValues = null;
            int[] index;
            float[] ndvi = new float[1];
            short[] lst = new short[9];
            if (txtValues.Count() < 1)
                return;
            //保存两个文件
            string ndviTempFile = SaveTempFile(ndviTemp, "NDVI");
            string lstTempFile = SaveTempFile(resultTemp, "0LST");
            IRasterDataProvider ndviPrd = null;
            IRasterDataProvider lstPrd = null;
            IRasterDataProvider ndviMaxPrd = null;
            short[] ndviMax = new short[1];
            try
            {
                ndviPrd = GeoDataDriver.Open(ndviTempFile) as IRasterDataProvider;
                lstPrd = GeoDataDriver.Open(lstTempFile) as IRasterDataProvider;
                if (!string.IsNullOrWhiteSpace(ndviMaxValue))
                    ndviMaxPrd = GeoDataDriver.Open(ndviMaxValue) as IRasterDataProvider;
                foreach (float[] key in txtValues.Keys)
                {
                    float tempValue = (txtValues[key] + 273) * 10;
                    //计算经纬度位置的索引号
                    index = GetRasterIndex(key, _argumentProvider.DataProvider);
                    if (index == null)
                        continue;
                    //if(index[1]-1>0
                    GCHandle lstGC = GCHandle.Alloc(lst, GCHandleType.Pinned);
                    lstPrd.GetRasterBand(1).Read(index[1], index[0], 1, 1, lstGC.AddrOfPinnedObject(), enumDataType.UInt16, 1, 1);
                    //释放内存
                    //...
                    if (lst[0] == 0 || lst[0] == cloudy || lst[0] == water)
                        continue;
                    lstList.Add(lst[0]);
                    GCHandle ndviGC = GCHandle.Alloc(ndvi, GCHandleType.Pinned);
                    ndviPrd.GetRasterBand(1).Read(index[1], index[0], 1, 1, ndviGC.AddrOfPinnedObject(), enumDataType.Float, 1, 1);
                    //释放内存
                    //...
                    ordNDVIList.Add(ndvi[0]);
                    if (ab != null)
                        ndviList.Add(tempValue + (ndvi[0] * ab[0] + ab[1]) * 10);
                    ObservationValue ov = new ObservationValue { Lat = key[0], Lon = key[1], Value = tempValue, station = (int)Math.Floor(key[2]) };
                    if (ndviMaxPrd != null)
                    {
                        int[] ndviMaxIndex = GetRasterIndex(key, ndviMaxPrd);
                        GCHandle ndviMaxGC = GCHandle.Alloc(ndviMax, GCHandleType.Pinned);
                        ndviMaxPrd.GetRasterBand(1).Read(ndviMaxIndex[1], ndviMaxIndex[0], 1, 1, ndviMaxGC.AddrOfPinnedObject(), enumDataType.Int16, 1, 1);
                        ov.OtherValue = ndviMax[0];
                        ov.DetaT = lst[0] - tempValue;
                        //读取周围点，算平均值
                        int region = 5;
                        int r = region / 2;
                        short[] lstT = new short[9];
                        double sum = 0;
                        int count = 0;
                        for (int row = 0 - r; row <= r; row++)
                        {
                            for (int col = 0 - r; col <= r; col++)
                            {
                                if (index[0] + row < 0 || index[1] + col < 0 || index[0] + row >= lstPrd.Height || index[1] + col >= lstPrd.Width)
                                    continue;
                                GCHandle lstGCT = GCHandle.Alloc(lstT, GCHandleType.Pinned);
                                lstPrd.GetRasterBand(1).Read(index[1] + col, index[0] + row, 1, 1, lstGCT.AddrOfPinnedObject(), enumDataType.UInt16, 1, 1);
                                if (lstT[0] == 0 || lstT[0] == cloudy || lstT[0] == water)
                                    continue;
                                sum += lstT[0];
                                count++;
                            }
                        }
                        ov.avgLst = Math.Round(sum / count, 0);
                        ov.index = index[0] * _argumentProvider.DataProvider.Width + index[1];
                        //
                        _indexiex.Add(ov.index);
                    }
                    observationValueList.Add(ov);
                }
                if (lstList.Count > 0)
                {
                    newNdviValue = ndviList.ToArray();
                    lstValue = lstList.ToArray();
                    ordNDVIValue = ordNDVIList.ToArray();
                    observationValues = observationValueList.ToArray();
                }
            }
            finally
            {
                if (ndviPrd != null)
                    ndviPrd.Dispose();
                if (lstPrd != null)
                    lstPrd.Dispose();
            }
        }

        private string SaveTempFile<T>(MemPixelFeatureMapper<T> tempMapper, string identify)
        {
            IRasterDataProvider dataPrd = _argumentProvider.DataProvider;
            RasterIdentify id = new RasterIdentify();
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "LST";
            id.SubProductIdentify = identify;
            using (IInterestedRaster<T> iir = new InterestedRaster<T>(id, new Size(dataPrd.Width, dataPrd.Height), dataPrd.CoordEnvelope.Clone(), dataPrd.SpatialRef))
            {
                iir.Put(tempMapper);
                return iir.FileName;
            }
        }

        private string SaveNDVITempFile(MemPixelFeatureMapper<float> tempMapper, string identify, float zoom)
        {
            IRasterDataProvider dataPrd = _argumentProvider.DataProvider;
            RasterIdentify id = new RasterIdentify();
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "LST";
            id.SubProductIdentify = identify;
            using (IInterestedRaster<UInt16> iir = new InterestedRaster<UInt16>(id, new Size(dataPrd.Width, dataPrd.Height), dataPrd.CoordEnvelope.Clone(), dataPrd.SpatialRef))
            {
                float temp = 0;
                iir.Put(0);
                int[] indexTemp = tempMapper.Indexes.ToArray();
                for (int index = 0; index < indexTemp.Length; index++)
                {
                    temp = tempMapper.GetValueByIndex(index) * zoom;
                    iir.Put(indexTemp[index], temp < 0 ? (UInt16)0 : (UInt16)(temp));
                }
                return iir.FileName;
            }
        }

        public static void lsfitlinear(double[] y, double[,] x, out double[] beta)
        {
            int nobs = y.Length;
            if (nobs != x.GetLength(0))
                throw new ArgumentException("ols: y and x must have same length");
            int nvar = x.GetLength(1);
            double[,] x1 = new double[nobs, nvar + 1];
            for (int i = 0; i < nvar; i++)
            {
                for (int j = 0; j < nobs; j++)
                    x1[j, i] = x[j, i];
            }
            for (int i = 0; i < nobs; i++)
                x1[i, nvar] = 1;
            int info;
            alglib.lsfitreport rep;
            alglib.lsfitlinear(y, x1, nobs, nvar + 1, out info, out beta, out rep);
        }

        //计算指定经纬度位置索引号location[0]-纬度 locatioon[1]-经度
        private int[] GetRasterIndex(float[] location, IRasterDataProvider dataPrd)
        {
            if (location[0] > 90 || location[0] < -90)
                return null;
            if (location[1] < -180 || location[1] > 180)
                return null;
            if (dataPrd == null)
                return null;
            double minX = dataPrd.CoordEnvelope.MinX;
            double maxY = dataPrd.CoordEnvelope.MaxY;
            int lineIndex = (int)((maxY - location[0]) / dataPrd.ResolutionX + 0.5f);
            int columnIndex = (int)((location[1] - minX) / dataPrd.ResolutionY + 0.5f);
            if (lineIndex < 0 || columnIndex < 0 || lineIndex >= dataPrd.Height || columnIndex >= dataPrd.Width)
                return null;
            return new int[] { lineIndex, columnIndex };
        }

        //读取观测文件数据
        private Dictionary<float[], float> ReadTxtValues(string computeFile)
        {
            if (string.IsNullOrEmpty(computeFile) || !File.Exists(computeFile))
                return null;
            float lat, lon, station;
            string[] pointInfos = File.ReadAllLines(computeFile, Encoding.Default);
            if (pointInfos == null || pointInfos.Length < 3)
                return null;
            Dictionary<float[], float> valuesDic = new Dictionary<float[], float>();
            for (int i = 2; i < pointInfos.Length; i++)
            {
                string[] values = pointInfos[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length != 5)
                    continue;
                float tempValue;
                if (float.TryParse(values[1], out lon) && float.TryParse(values[2], out lat) && float.TryParse(values[4], out tempValue) && float.TryParse(values[0], out station))
                {
                    valuesDic.Add(new float[] { lat, lon, station }, tempValue);
                }
                else
                    continue;
            }
            return valuesDic;
        }

        internal class ObservationValue
        {
            public double Lon;
            public double Lat;
            public int station;
            public double Value;
            public double OtherValue;
            public double resuleValue;
            public double avgLst;
            public double avgResultLst;
            public int index;

            public float DetaT { get; set; }
        }

        private IExtractResult ExtractLST(string dirPath)
        {
            string[] files = GetFiles(dirPath);
            return ComputeByFiles(files);
        }

        private IExtractResult ComputeByFiles(string[] files)
        {
            MemPixelFeatureMapper<UInt16> resultTemp = null;
            IExtractResultArray result = new ExtractResultArray("DBLV");
            IRasterDataProvider currPrd = null;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;//
            int VisibleCH = TryGetBandNo(bandNameRaster, "Visible"); //(int)_argumentProvider.GetArg("Visible");
            int NearInfraredCH = TryGetBandNo(bandNameRaster, "NearInfrared"); //(int)_argumentProvider.GetArg("NearInfrared");
            int MiddInfraredCH = TryGetBandNo(bandNameRaster, "MiddInfrared"); //(int)_argumentProvider.GetArg("MiddInfrared");
            int FarInfrared11CH = TryGetBandNo(bandNameRaster, "FarInfrared11"); //(int)_argumentProvider.GetArg("FarInfrared11");
            int FarInfrared12CH = TryGetBandNo(bandNameRaster, "FarInfrared12"); //(int)_argumentProvider.GetArg("FarInfrared12");
            double VisibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double NearInfraredZoom = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
            double MiddInfraredZoom = (double)_argumentProvider.GetArg("MiddInfrared_Zoom");
            double FarInfrared11Zoom = (double)_argumentProvider.GetArg("FarInfrared11_Zoom");
            double FarInfrared12Zoom = (double)_argumentProvider.GetArg("FarInfrared12_Zoom");
            bool isAutoCloud = (bool)_argumentProvider.GetArg("isAutoCloud");

            if (VisibleCH == -1 || NearInfraredCH == -1 || MiddInfraredCH == -1 || FarInfrared11CH == -1 || FarInfrared12CH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            bool useCLMFile = true;
            string clmRasterFile = _argumentProvider.GetArg("clmFile").ToString();
            if (string.IsNullOrEmpty(clmRasterFile) || !File.Exists(clmRasterFile))
                useCLMFile = false;

            double lstZoom = (double)_argumentProvider.GetArg("LSTZoom");
            UInt16 defCloudy = (UInt16)_argumentProvider.GetArg("defCloudy");
            UInt16 defWater = (UInt16)_argumentProvider.GetArg("defWater");

            float a0 = (float)_argumentProvider.GetArg("A0");
            float alfa = (float)_argumentProvider.GetArg("alfa");
            float beta = (float)_argumentProvider.GetArg("beta");
            float gama = (float)_argumentProvider.GetArg("gama");
            float alfa2 = (float)_argumentProvider.GetArg("alfa2");
            float beta2 = (float)_argumentProvider.GetArg("beta2");

            float b4m = (float)_argumentProvider.GetArg("b4m");
            float b4n = (float)_argumentProvider.GetArg("b4n");
            float b5m = (float)_argumentProvider.GetArg("b5m");
            float b5n = (float)_argumentProvider.GetArg("b5n");

            float b4Water = (float)_argumentProvider.GetArg("b4Water");
            float b5Water = (float)_argumentProvider.GetArg("b5Water");
            float b4Soil = (float)_argumentProvider.GetArg("b4Soil");
            float b5Soil = (float)_argumentProvider.GetArg("b5Soil");
            float b4VGT = (float)_argumentProvider.GetArg("b4VGT");
            float b5VGT = (float)_argumentProvider.GetArg("b5VGT");

            float NearInfraredCLMMin = float.Parse(_argumentProvider.GetArg("NearInfraredCLMMin").ToString());
            float FarInfrared11CLMMax = float.Parse(_argumentProvider.GetArg("FarInfrared11CLMMax").ToString());
            float FarInfrared1112CLMMin = float.Parse(_argumentProvider.GetArg("FarInfrared1112CLMMin").ToString());
            float FarInfrared11WaterMin = float.Parse(_argumentProvider.GetArg("FarInfrared11WaterMin").ToString());
            float NDVIWaterMax = float.Parse(_argumentProvider.GetArg("NDVIWaterMax").ToString());

            //by chennan 上海 批量生产的AOI如何设置？？？
            for (int i = 0; i < files.Length; i++)
            {
                float curNDVI = 0f;
                currPrd = GeoDataDriver.Open(files[i], null) as IRasterDataProvider;
                if (currPrd != null)
                    try
                    {
                        ArgumentProvider ap = new ArgumentProvider(currPrd, null);
                        RasterPixelsVisitor<UInt16> rpVisitor = new RasterPixelsVisitor<UInt16>(ap);
                        float b4Emiss = 0;
                        float b5Emiss = 0;
                        double pv = 0;
                        //4 5 通道比辐射率均值 差值
                        float EmissMean = 0;
                        float DeltaEmiss = 0;
                        //LST计算系数 P M
                        double P = 0;
                        double M = 0;
                        resultTemp = new MemPixelFeatureMapper<UInt16>("DBLV", 1000, new Size(currPrd.Width, currPrd.Height), currPrd.CoordEnvelope, currPrd.SpatialRef);
                        rpVisitor.VisitPixel(new int[] { VisibleCH, NearInfraredCH, MiddInfraredCH, FarInfrared11CH, FarInfrared12CH },
                         (index, values) =>
                         {
                             if (values[0] == 0)
                             {
                                 resultTemp.Put(index, 0);
                                 curNDVI = 0;
                             }
                             else if (isAutoCloud && (values[1] / NearInfraredZoom > NearInfraredCLMMin && values[3] / FarInfrared11Zoom < FarInfrared11CLMMax && Math.Abs(values[2] / MiddInfraredZoom - values[3] / FarInfrared11Zoom) > FarInfrared1112CLMMin))
                             {
                                 resultTemp.Put(index, defCloudy);
                                 curNDVI = defCloudy / _tempzoom;
                             }
                             else
                             {
                                 curNDVI = GetNDVI(values, 1, 0);
                                 if (values[3] / FarInfrared11Zoom > FarInfrared11WaterMin && curNDVI < NDVIWaterMax)
                                 {
                                     resultTemp.Put(index, defWater);
                                     curNDVI = defWater / _tempzoom;
                                     b4Emiss = b4Water;
                                     b5Emiss = b5Water;
                                 }
                                 else
                                 {
                                     if (curNDVI < 0.2)
                                     {
                                         b4Emiss = b4Soil;
                                         b5Emiss = b5Soil;
                                     }
                                     else if (curNDVI > 0.5)
                                     {
                                         b4Emiss = b4VGT;
                                         b5Emiss = b5VGT;
                                     }
                                     else if (curNDVI >= 0.2 && curNDVI <= 0.5)
                                     {
                                         pv = Math.Pow((curNDVI - 0.2) / (0.5 - 0.2), 2);
                                         b4Emiss = (float)(b4m * pv + b4n);
                                         b5Emiss = (float)(b5m * pv + b5n);
                                     }

                                     EmissMean = (b4Emiss + b5Emiss) / 2;
                                     DeltaEmiss = (b4Emiss - b5Emiss);

                                     P = 1 + alfa * (1 - EmissMean) / EmissMean + beta * DeltaEmiss / Math.Pow(EmissMean, 2);
                                     M = gama + alfa2 * (1 - EmissMean) / EmissMean + beta2 * DeltaEmiss / Math.Pow(EmissMean, 2);

                                     resultTemp.Put(index, (UInt16)((a0 + P * (values[3] / FarInfrared11Zoom + values[4] / FarInfrared12Zoom) / 2
                                                       + M * (values[3] / FarInfrared11Zoom - values[4] / FarInfrared12Zoom) / 2) * lstZoom));
                                 }
                             }
                         });
                        AddClmProcess(resultTemp, currPrd, true, null);
                        result.Add(GenrateIInterested(resultTemp, currPrd));
                    }
                    finally
                    {
                        currPrd.Dispose();
                    }
            }
            return result;
        }

        #endregion

        #region 静止星计算

        private IExtractResult ComputeByCurrentRasterGeostationary(Action<int, string> progressTracker, IRasterDataProvider currPrd, bool progressControl)
        {
            if (currPrd == null)
                return null;
            int FarInfrared11CH = -1, FarInfrared12CH = -1, CloudCH = -1;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            if (!progressControl)
            {
                FarInfrared11CH = TryGetBandNo(bandNameRaster, "FarInfrared11");//(int)_argumentProvider.GetArg("FarInfrared11");
                FarInfrared12CH = TryGetBandNo(bandNameRaster, "FarInfrared12");//(int)_argumentProvider.GetArg("FarInfrared12");
                CloudCH = (int)_argumentProvider.GetArg("CloudBand");
            }
            else
            {
                FarInfrared11CH = 1;
                FarInfrared12CH = 2;
                CloudCH = 3;
            }
            int NDVICH = (int)_argumentProvider.GetArg("NDVIBand");
            double FarInfrared11Zoom = (double)_argumentProvider.GetArg("FarInfrared11_Zoom");
            double FarInfrared12Zoom = (double)_argumentProvider.GetArg("FarInfrared12_Zoom");
            double NDVIBandZoom = (double)_argumentProvider.GetArg("NDVIBand_Zoom");
            double CloudZoom = (double)_argumentProvider.GetArg("CloudBand_Zoom");

            if (CloudCH == -1 || NDVICH == -1 || FarInfrared11CH == -1 || FarInfrared12CH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            string NDVIFile = GetStringArg("NDVIFile");
            if (string.IsNullOrEmpty(NDVIFile))
            {
                PrintInfo("获取NDVI文件失败。");
                return null;
            }
            float NDVIZoom = (float)_argumentProvider.GetArg("NDVIZoom");
            List<Int16> CloudyMark = GetArrayByStringArg("Cloudy");
            if (CloudyMark == null || CloudyMark.Count == 0)
            {
                PrintInfo("获取云标识失败。");
                return null;
            }
            List<Int16> WaterMark = GetArrayByStringArg("Water");
            if (WaterMark == null || WaterMark.Count == 0)
            {
                PrintInfo("获取水标识失败。");
                return null;
            }

            bool useCLMFile = true;
            string clmRasterFile = _argumentProvider.GetArg("clmFile").ToString();
            if (string.IsNullOrEmpty(clmRasterFile) || !File.Exists(clmRasterFile))
                useCLMFile = false;

            double lstZoom = (double)_argumentProvider.GetArg("LSTZoom");
            UInt16 defCloudy = (UInt16)_argumentProvider.GetArg("defCloudy");
            UInt16 defWater = (UInt16)_argumentProvider.GetArg("defWater");

            float a0 = (float)_argumentProvider.GetArg("A0");
            float alfa = (float)_argumentProvider.GetArg("alfa");
            float beta = (float)_argumentProvider.GetArg("beta");
            float gama = (float)_argumentProvider.GetArg("gama");
            float alfa2 = (float)_argumentProvider.GetArg("alfa2");
            float beta2 = (float)_argumentProvider.GetArg("beta2");

            float b4m = (float)_argumentProvider.GetArg("b4m");
            float b4n = (float)_argumentProvider.GetArg("b4n");
            float b5m = (float)_argumentProvider.GetArg("b5m");
            float b5n = (float)_argumentProvider.GetArg("b5n");

            float b4Water = (float)_argumentProvider.GetArg("b4Water");
            float b5Water = (float)_argumentProvider.GetArg("b5Water");
            float b4Soil = (float)_argumentProvider.GetArg("b4Soil");
            float b5Soil = (float)_argumentProvider.GetArg("b5Soil");
            float b4VGT = (float)_argumentProvider.GetArg("b4VGT");
            float b5VGT = (float)_argumentProvider.GetArg("b5VGT");

            float FarInfrared11CLMMax = float.Parse(_argumentProvider.GetArg("FarInfrared11CLMMax").ToString());
            float FarInfrared1112CLMMin = float.Parse(_argumentProvider.GetArg("FarInfrared1112CLMMin").ToString());
            float FarInfrared11WaterMin = float.Parse(_argumentProvider.GetArg("FarInfrared11WaterMin").ToString());
            float NDVIWaterMax = float.Parse(_argumentProvider.GetArg("NDVIWaterMax").ToString());

            ArgumentProvider ap = new ArgumentProvider(currPrd, null);
            RasterPixelsVisitor<UInt16> rpVisitor = new RasterPixelsVisitor<UInt16>(ap);
            float b4Emiss = 0;
            float b5Emiss = 0;
            double pv = 0;
            //4 5 通道比辐射率均值 差值
            float EmissMean = 0;
            float DeltaEmiss = 0;
            //LST计算系数 P M
            double P = 0;
            double M = 0;

            //int[] aoi = _argumentProvider.AOI;
            //Size size = new Size(currPrd.Width, currPrd.Height);
            //Rectangle aoiRect = AOIHelper.ComputeAOIRect(aoi, size);

            //输入文件准备
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider ndviPrd = null;
            try
            {
                if (currPrd.BandCount < CloudCH)
                {
                    PrintInfo("未投影静止星云产品数据集。");
                    return null;
                }
                RasterMaper rasterRm = new RasterMaper(currPrd, new int[] { FarInfrared11CH, FarInfrared12CH, CloudCH });
                rms.Add(rasterRm);
                ndviPrd = GeoDataDriver.Open(NDVIFile) as IRasterDataProvider;
                if (ndviPrd.BandCount < NDVICH)
                {
                    PrintInfo("请选择正确的植被指数文件进行静止星陆表温度计算。");
                    return null;
                }
                RasterMaper ndviRm = new RasterMaper(ndviPrd, new int[] { NDVICH });
                rms.Add(ndviRm);

                //输出文件准备
                string outFileName = GetFileNameDefTime(new string[] { currPrd.fileName, NDVIFile }, _subProductDef.ProductDef.Identify, _identify, ".dat", null, currPrd.fileName);
                using (IRasterDataProvider outRaster = CreateOutRasterDat(outFileName, rms.ToArray()))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    RasterProcessModel<Int16, UInt16> rfr = new RasterProcessModel<Int16, UInt16>(progressControl ? null : progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    Int16 valueNdvi = 0, valueFar11 = 0, valueFar12 = 0, valueCloud = 0;
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, UInt16>((rvInVistor, rvOutVistor, aoi) =>
                        {
                            int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                            if (rvInVistor[0].RasterBandsData == null || rvInVistor[1].RasterBandsData == null ||
                                rvInVistor[0].RasterBandsData[0] == null || rvInVistor[1].RasterBandsData[0] == null)
                                return;
                            for (int index = 0; index < dataLength; index++)
                            {
                                valueFar11 = rvInVistor[0].RasterBandsData[0][index];
                                valueFar12 = rvInVistor[0].RasterBandsData[1][index];
                                valueCloud = rvInVistor[0].RasterBandsData[2][index];
                                valueNdvi = rvInVistor[1].RasterBandsData[0][index];
                                if (isCloudy(CloudyMark, valueCloud) || isCloudy(CloudyMark, valueNdvi))
                                    rvOutVistor[0].RasterBandsData[0][index] = defCloudy;
                                else if (isWater(WaterMark, valueNdvi))
                                    rvOutVistor[0].RasterBandsData[0][index] = defWater;
                                else if (valueNdvi == Int16.MinValue || valueFar11 == Int16.MinValue || valueFar12 == Int16.MinValue)
                                    rvOutVistor[0].RasterBandsData[0][index] = 0;
                                else if (valueFar11 == 0)
                                    rvOutVistor[0].RasterBandsData[0][index] = 0;
                                else
                                {
                                    if (valueFar11 / FarInfrared11Zoom > FarInfrared11WaterMin && valueNdvi < NDVIWaterMax)
                                    {
                                        rvOutVistor[0].RasterBandsData[0][index] = defWater;
                                        b4Emiss = b4Water;
                                        b5Emiss = b5Water;
                                    }
                                    else
                                    {
                                        if (valueNdvi < 0.2)
                                        {
                                            b4Emiss = b4Soil;
                                            b5Emiss = b5Soil;
                                        }
                                        else if (valueNdvi > 0.5)
                                        {
                                            b4Emiss = b4VGT;
                                            b5Emiss = b5VGT;
                                        }
                                        else if (valueNdvi >= 0.2 && valueNdvi <= 0.5)
                                        {
                                            pv = Math.Pow((valueNdvi - 0.2) / (0.5 - 0.2), 2);
                                            b4Emiss = (float)(b4m * pv + b4n);
                                            b5Emiss = (float)(b5m * pv + b5n);
                                        }

                                        EmissMean = (b4Emiss + b5Emiss) / 2;
                                        DeltaEmiss = (b4Emiss - b5Emiss);

                                        P = 1 + alfa * (1 - EmissMean) / EmissMean + beta * DeltaEmiss / Math.Pow(EmissMean, 2);
                                        M = gama + alfa2 * (1 - EmissMean) / EmissMean + beta2 * DeltaEmiss / Math.Pow(EmissMean, 2);

                                        rvOutVistor[0].RasterBandsData[0][index] = (UInt16)((a0 + P * (valueFar11 / FarInfrared11Zoom + valueFar12 / FarInfrared12Zoom) / 2
                                                         + M * (valueFar11 / FarInfrared11Zoom - valueFar12 / FarInfrared12Zoom) / 2) * lstZoom);
                                    }
                                }
                            }
                        }));
                    rfr.Excute(Int16.MinValue);
                    FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                    res.SetDispaly(false);
                    return res;
                }
            }
            finally
            {
                if (ndviPrd != null)
                    ndviPrd.Dispose();
            }
        }

        private IExtractResult ExtractLSTGeostationary(Action<int, string> progressTracker, string dirPath)
        {
            string[] files = GetFiles(dirPath);
            return ComputeByFilesGeostationary(progressTracker, files);
        }

        private IExtractResult ComputeByFilesGeostationary(Action<int, string> progressTracker, string[] files)
        {
            if (files == null || files.Length == 0)
                return null;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;//
            int CloudCH = TryGetBandNo(bandNameRaster, "CloudBand"); //(int)_argumentProvider.GetArg("CloudBand");
            int FarInfrared11CH = TryGetBandNo(bandNameRaster, "FarInfrared11"); //(int)_argumentProvider.GetArg("FarInfrared11");
            int FarInfrared12CH = TryGetBandNo(bandNameRaster, "FarInfrared12"); //(int)_argumentProvider.GetArg("FarInfrared12");
            double FarInfrared11Zoom = (double)_argumentProvider.GetArg("FarInfrared11_Zoom");
            double FarInfrared12Zoom = (double)_argumentProvider.GetArg("FarInfrared12_Zoom");
            double CloudZoom = (double)_argumentProvider.GetArg("CloudBand_Zoom");

            if (CloudCH == -1 || FarInfrared11CH == -1 || FarInfrared12CH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            IRasterDataProvider currPrd = null;
            IExtractResultArray array = new ExtractResultArray("LST");
            Dictionary<string, string[]> fileGroup = FileSortAndGroup(files);
            if (fileGroup == null)
                return null;
            string outFilename;
            int interval = 100 / (fileGroup.Count + 1);
            int index = 0;
            foreach (string key in fileGroup.Keys)
            {
                outFilename = CompositeGeostationaryData(progressTracker, fileGroup[key], ref index, interval);
                if (outFilename == null)
                    continue;
                currPrd = GeoDataDriver.Open(outFilename, null) as IRasterDataProvider;
                if (currPrd != null)
                    try
                    {
                        IExtractResult oneFile = ComputeByCurrentRasterGeostationary(progressTracker, currPrd, true);
                        if (oneFile == null)
                            continue;
                        array.Add(oneFile as IFileExtractResult);
                    }
                    finally
                    {
                        currPrd.Dispose();
                    }
            }
            return array;
        }

        private string CompositeGeostationaryData(Action<int, string> progressTracker, string[] groupFiles, ref int progressIndex, int interval)
        {
            int groupLength = groupFiles.Length;
            string outFilename = MifEnvironment.GetFullFileName(Path.GetFileNameWithoutExtension(groupFiles[groupLength - 1]) + ".ldf");
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;//
            int CloudCH = TryGetBandNo(bandNameRaster, "CloudBand"); //(int)_argumentProvider.GetArg("CloudBand");
            int FarInfrared11CH = TryGetBandNo(bandNameRaster, "FarInfrared11"); //(int)_argumentProvider.GetArg("FarInfrared11");
            int FarInfrared12CH = TryGetBandNo(bandNameRaster, "FarInfrared12"); //(int)_argumentProvider.GetArg("FarInfrared12");
            double FarInfrared11Zoom = (double)_argumentProvider.GetArg("FarInfrared11_Zoom");
            double FarInfrared12Zoom = (double)_argumentProvider.GetArg("FarInfrared12_Zoom");
            double CloudZoom = (double)_argumentProvider.GetArg("CloudBand_Zoom");
            UInt16 defCloudy = (UInt16)_argumentProvider.GetArg("defCloudy");

            //输入文件准备
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider currPrd = null;
            IRasterDataProvider nextPrd = null;
            try
            {
                for (int i = 0; i < groupLength; i++)
                {
                    IRasterDataProvider inRaster = RasterDataDriver.Open(groupFiles[i]) as IRasterDataProvider;
                    if (inRaster.BandCount < CloudCH)
                    {
                        PrintInfo("未投影静止星云产品数据集。");
                        continue;
                    }
                    RasterMaper rm = new RasterMaper(inRaster, new int[] { FarInfrared11CH, FarInfrared12CH, CloudCH });
                    rms.Add(rm);
                }
                if (rms.Count == 0)
                    return null;
                using (IRasterDataProvider outRaster = CreateOutRasterLDF(outFilename, rms.ToArray(), 3))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1, 2, 3 }) };
                    RasterProcessModel<UInt16, UInt16> rfr = new RasterProcessModel<UInt16, UInt16>(null);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<UInt16, UInt16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        UInt16[] maxFar11Data = new UInt16[dataLength];
                        byte[] nonCloudData = new byte[dataLength];//0代表有云 1代表无云
                        UInt16 valueCloud = 0, valueFar11 = 0;
                        foreach (RasterVirtualVistor<UInt16> rvs in rvInVistor)
                        {
                            for (int index = 0; index < dataLength; index++)
                            {
                                valueFar11 = rvs.RasterBandsData[0][index];
                                valueCloud = rvs.RasterBandsData[2][index];
                                if (valueCloud > 10 && valueCloud != 255)
                                {
                                    if (nonCloudData[index] == 0)
                                        rvOutVistor[0].RasterBandsData[2][index] = defCloudy;
                                    else
                                        continue;
                                }
                                else
                                {
                                    if (nonCloudData[index] == 0)
                                    {
                                        rvOutVistor[0].RasterBandsData[0][index] = rvs.RasterBandsData[0][index];
                                        rvOutVistor[0].RasterBandsData[1][index] = rvs.RasterBandsData[1][index];
                                        rvOutVistor[0].RasterBandsData[2][index] = rvs.RasterBandsData[2][index];
                                        maxFar11Data[index] = rvs.RasterBandsData[0][index];
                                        nonCloudData[index] = 1;
                                    }
                                    else
                                    {
                                        if (maxFar11Data[index] < valueFar11)
                                        {
                                            rvOutVistor[0].RasterBandsData[0][index] = rvs.RasterBandsData[0][index];
                                            rvOutVistor[0].RasterBandsData[1][index] = rvs.RasterBandsData[1][index];
                                            rvOutVistor[0].RasterBandsData[2][index] = rvs.RasterBandsData[2][index];
                                            maxFar11Data[index] = rvs.RasterBandsData[0][index];
                                            nonCloudData[index] = 1;
                                        }
                                        else
                                            continue;
                                    }
                                }
                            }
                        }
                    }));
                    rfr.Excute(UInt16.MinValue);
                    FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outFilename, false);
                    res.SetDispaly(false);
                    return outFilename;
                }
            }
            finally
            {
                progressIndex++;
                if (progressTracker != null)
                    progressTracker(progressIndex * interval, "");
                if (currPrd != null)
                    currPrd.Dispose();
                if (nextPrd != null)
                    nextPrd.Dispose();
            }

        }

        #endregion

        private Dictionary<string, string[]> FileSortAndGroup(string[] files)
        {
            if (files == null || files.Length == 0)
                return null;
            string dateMatch = "yyyyMMdd";
            Dictionary<DateTime, string> sortTemp = (new RasterIdentify()).SortByOrbitDate(files);
            Dictionary<string, string[]> result = new Dictionary<string, string[]>();
            List<string> fileTemp = new List<string>();
            string groupStandard = sortTemp.Keys.First().ToString(dateMatch);
            foreach (DateTime key in sortTemp.Keys)
            {
                if (groupStandard == key.ToString(dateMatch))
                {
                    fileTemp.Add(sortTemp[key]);
                }
                else
                {
                    result.Add(groupStandard, fileTemp.ToArray());
                    fileTemp = new List<string>();
                    fileTemp.Add(sortTemp[key]);
                    groupStandard = key.ToString(dateMatch);
                }
            }
            if (fileTemp.Count != 0)
                result.Add(groupStandard, fileTemp.ToArray());
            return result.Count == 0 ? null : result;
        }

        private bool isCloudy(List<Int16> mark, Int16 value)
        {
            return isContaints(mark, value);
        }

        private bool isWater(List<Int16> mark, Int16 value)
        {
            return isContaints(mark, value);
        }

        private bool isContaints(List<Int16> mark, Int16 value)
        {
            if (mark.Count == 0)
                return false;
            return mark.Contains(value);
        }

        private string GetStringArg(string argContent)
        {
            object obj = _argumentProvider.GetArg(argContent);
            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                return string.Empty;
            return obj.ToString();
        }

        private List<Int16> GetArrayByStringArg(string argContent)
        {
            string content = GetStringArg(argContent);
            if (string.IsNullOrEmpty(argContent))
                return null;
            string[] tempSprit = content.Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
            if (tempSprit == null || tempSprit.Length == 0)
                return null;
            List<Int16> result = new List<Int16>();
            int length = tempSprit.Length;
            Int16 listTemp = 0;
            for (int i = 0; i < length; i++)
            {
                if (Int16.TryParse(tempSprit[i], out listTemp))
                    result.Add(listTemp);
            }
            return result.Count == 0 ? null : result;
        }

        private string[] GetFiles(string dirPath)
        {
            ObritFileFinder off = new ObritFileFinder();
            string extInfo = null;
            string[] files = null;
            if (string.IsNullOrEmpty(dirPath))
                return null;
            string flag = _argumentProvider.GetArg("orbitDataIdentify").ToString();
            files = off.Find(string.Empty, ref extInfo, "Dir=" + dirPath + ",Flag=" + flag + ",Sort=asc,ProFlag=true,FindRegion=all");
            if (files == null || files.Length == 0)
            {
                PrintInfo("计算陆表高温的主文件未提供!");
                return null;
            }
            return files;
        }

        private IFileExtractResult GenrateIInterested(MemPixelFeatureMapper<UInt16> result, IRasterDataProvider currPrd)
        {
            RasterIdentify id = new RasterIdentify(currPrd.fileName.ToUpper());
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "LST";
            id.SubProductIdentify = "DBLV";
            id.IsOutput2WorkspaceDir = true;
            using (IInterestedRaster<UInt16> iir = new InterestedRaster<UInt16>(id, new Size(currPrd.Width, currPrd.Height), currPrd.CoordEnvelope.Clone(), currPrd.SpatialRef))
            {
                iir.Put(result);
                IFileExtractResult fileResult = new FileExtractResult("DBLV", iir.FileName);
                fileResult.SetDispaly(false);
                return fileResult;
            }
        }

        private IFileExtractResult GenrateIInterested(MemPixelFeatureMapper<UInt16> result, IRasterDataProvider currPrd, RasterIdentify riCopy)
        {
            if (riCopy == null)
                return null;
            riCopy.ThemeIdentify = "CMA";
            riCopy.ProductIdentify = "LST";
            riCopy.SubProductIdentify = "DBLV";
            riCopy.IsOutput2WorkspaceDir = true;
            using (IInterestedRaster<UInt16> iir = new InterestedRaster<UInt16>(riCopy, new Size(currPrd.Width, currPrd.Height), currPrd.CoordEnvelope.Clone(), currPrd.SpatialRef))
            {
                iir.Put(result);
                IFileExtractResult fileResult = new FileExtractResult("DBLV", iir.FileName);
                fileResult.SetDispaly(false);
                return fileResult;
            }
        }

        private RasterIdentify GetRasterIdentifyID(string[] fileNames)
        {
            RasterIdentify rst = new RasterIdentify(fileNames);
            object obj = _argumentProvider.GetArg("isReCompute");
            bool isRecompute = false;
            if (obj != null)
                isRecompute = (bool)_argumentProvider.GetArg("isReCompute");
            rst.ProductIdentify = _subProductDef.ProductDef.Identify;
            rst.SubProductIdentify = isRecompute ? "DBHL" : _subProductDef.Identify;
            rst.IsOutput2WorkspaceDir = true;
            return rst;
        }

        private IRasterDataProvider CreateOutRaster(string outFileName, RasterMaper[] rasterMaper)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in rasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Union(inRaster.Raster.CoordEnvelope);
            }
            float resX = rasterMaper[0].Raster.ResolutionX;
            float resY = rasterMaper[0].Raster.ResolutionY;
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, enumDataType.Int16, mapInfo) as RasterDataProvider;
            return outRaster;
        }

        private IFileExtractResult GenrateIInterested(MemPixelFeatureMapper<Int16> result, IRasterDataProvider currPrd, string subProductIndentify)
        {
            RasterIdentify id = new RasterIdentify(currPrd.fileName.ToUpper());
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "LST";
            id.SubProductIdentify = subProductIndentify;
            id.IsOutput2WorkspaceDir = true;
            using (IInterestedRaster<Int16> iir = new InterestedRaster<Int16>(id, new Size(currPrd.Width, currPrd.Height), currPrd.CoordEnvelope.Clone(), currPrd.SpatialRef))
            {
                iir.Put(result);
                IFileExtractResult fileResult = new FileExtractResult(subProductIndentify, iir.FileName);
                fileResult.SetDispaly(false);
                return fileResult;
            }
        }

        private float GetNDVI(UInt16[] values, int nearInfraredCH, int visibleCH)
        {
            return (values[nearInfraredCH] + values[visibleCH]) == 0 ? 0f : (float)(values[nearInfraredCH] - values[visibleCH]) / (values[nearInfraredCH] + values[visibleCH]);
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }

        private IRasterDataProvider CreateOutRasterLDF(string outFileName, RasterMaper[] inrasterMaper, int bandcount)
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
            ISpatialReference spatialRef = inrasterMaper[0].Raster.SpatialRef;
            string bandNames = CreateBandNames(bandcount);
            string[] options = new string[]{
                            "INTERLEAVE=BSQ",
                            "VERSION=LDF",
                            "WITHHDR=TRUE",
                            "SPATIALREF=" + spatialRef.ToProj4String(),
                            "MAPINFO={" + 1 + "," + 1 + "}:{" + outEnv.MinX + "," + outEnv.MaxY + "}:{" + resX + "," + resY + "}",
                            "BANDNAMES="+ bandNames
                        };
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, bandcount, enumDataType.UInt16, options) as RasterDataProvider;
            return outRaster;
        }

        private IRasterDataProvider CreateOutRasterDat(string outFileName, RasterMaper[] rasterMaper)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in rasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Union(inRaster.Raster.CoordEnvelope);
            }
            float resX = rasterMaper[0].Raster.ResolutionX;
            float resY = rasterMaper[0].Raster.ResolutionY;
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, enumDataType.Int16, mapInfo) as RasterDataProvider;
            return outRaster;
        }

        private string CreateBandNames(int bandcount)
        {
            string result = null;
            for (int i = 1; i <= bandcount; i++)
            {
                result += "band " + i + ",";
            }
            return result.Substring(0, result.Length - 1);
        }

        private float[] ComputeAK(float[] xValue, float[] yValue)
        {
            if (xValue.Length != yValue.Length)
                return null;
            if (xValue.Length < 2 || yValue.Length < 2)
                return null;
            if (xValue[0] == xValue[1])
                return null;
            float a = (yValue[0] - yValue[1]) / (xValue[0] - xValue[1]);
            float b = yValue[0] - a * xValue[0];
            return new float[] { a, b };
        }

        #region 增加交互云修正

        private void AddClmProcess(MemPixelFeatureMapper<UInt16> resultTemp, IRasterDataProvider currPrd, bool searchClm, Action<int, string> progressTracker)
        {
            UInt16 defCloudy = (UInt16)_argumentProvider.GetArg("defCloudy");
            int cloudCH = (int)_argumentProvider.GetArg("cloudCH");
            bool isAppCloud = (bool)_argumentProvider.GetArg("isAppCloud");
            if (!isAppCloud)
                return;
            string lstfName = SaveTempFile(resultTemp, "DBLV");
            string clmFile = searchClm ? GetClmFile(currPrd) : currPrd.fileName;
            if (string.IsNullOrEmpty(clmFile))
                return;
            //输入文件准备
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider clmPrd = null;
            IRasterDataProvider lstPrd = null;
            try
            {
                lstPrd = GeoDataDriver.Open(lstfName) as IRasterDataProvider;

                RasterMaper rasterRm = new RasterMaper(lstPrd, new int[] { 1 });
                rms.Add(rasterRm);
                clmPrd = GeoDataDriver.Open(clmFile) as IRasterDataProvider;
                if (clmPrd.BandCount < cloudCH)
                {
                    PrintInfo("请选择正确的云数据通道进行计算。");
                    return;
                }
                RasterMaper ndviRm = new RasterMaper(clmPrd, new int[] { cloudCH });
                rms.Add(ndviRm);

                //输出文件准备
                string outFileName = MifEnvironment.GetFullFileName(Guid.NewGuid().ToString() + ".dat");
                using (IRasterDataProvider outRaster = CreateOutRasterDat(outFileName, rms.ToArray()))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    RasterProcessModel<UInt16, UInt16> rfr = new RasterProcessModel<UInt16, UInt16>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<UInt16, UInt16>((rvInVistor, rvOutVistor, aoi) =>
                        {
                            int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                            int y = rvInVistor[0].IndexY * rvInVistor[0].Raster.Width;
                            for (int index = 0; index < dataLength; index++)
                            {
                                if (rvInVistor[1].RasterBandsData[0][index] != 0)
                                    resultTemp.Put(y + index, defCloudy);

                            }
                        }));
                    rfr.Excute(UInt16.MinValue);
                }
            }
            finally
            {
                if (clmPrd != null)
                    clmPrd.Dispose();
                if (lstPrd != null)
                    lstPrd.Dispose();
            }
        }

        private string GetClmFile(IRasterDataProvider currPrd)
        {
            RasterIdentify rid = new RasterIdentify(Path.GetFileName(currPrd.fileName));
            rid.ProductIdentify = "LST";
            rid.SubProductIdentify = "0CLM";
            string clmFile = rid.ToWksFullFileName(".dat");
            if (File.Exists(clmFile))
                return clmFile;
            return null;
        }

        #endregion
    }
}