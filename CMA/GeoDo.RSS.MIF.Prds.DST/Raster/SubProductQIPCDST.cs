using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.DST
{
    //沙尘定量反演产品集合（光学厚度、粒子半径、载沙量）
    public class SubProductQIPCDST : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductQIPCDST(SubProductDef subProduct)
            : base(subProduct)
        {
            _name = subProduct.Name;
            _identify = subProduct.Identify;
            _isBinary = false;
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _contextMessage = contextMessage;
            string algorithmName=_argumentProvider.GetArg("AlgorithmName").ToString();
            if (string.IsNullOrEmpty(algorithmName))
                return null;
            else if (algorithmName == "MODQIPCAlgorithm")
                return ModisAlgorithm();
            else if (algorithmName == "VIRRQIPCAlgorithm")
                return VirrAlgorithm();
            return null;
        }

        private IExtractResult ModisAlgorithm()
        {
            int middleInfraredNo, farInfrared1No, farInfrared2No;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            middleInfraredNo = TryGetBandNo(bandNameRaster, "MiddleInfrared");
            farInfrared1No = TryGetBandNo(bandNameRaster, "FarInfrared1");
            farInfrared2No = TryGetBandNo(bandNameRaster, "FarInfrared2");
            if (middleInfraredNo == -1 || farInfrared1No == -1 || farInfrared2No == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            int[] bandNos = new int[] { middleInfraredNo, farInfrared1No, farInfrared2No };
            if (_argumentProvider.GetArg("BinaryFile") == null)
            {
                PrintInfo("获取判识结果文件失败。");
                return null;
            }
            string dblvFile = _argumentProvider.GetArg("BinaryFile").ToString();
            if (string.IsNullOrEmpty(dblvFile) || !File.Exists(dblvFile))
            {
                PrintInfo("获取判识结果文件失败。");
                return null;
            }
            float mbtd = (float)_argumentProvider.GetArg("mbtd");
            float mbt = (float)_argumentProvider.GetArg("mbt");
            
            int ntype = Int32.Parse(_argumentProvider.GetArg("ntype").ToString());
            //获取查找表参数
            LookupTableArgument argument = new LookupTableArgument();
            //进行光学厚度与沙尘粒子有效半径计算需要文件：1、原始影像数据；2、查算表数据
            List<RasterMaper> inputRms = new List<RasterMaper>();
            List<RasterMaper> outputRms = new List<RasterMaper>();
            try
            {
                //当前影像（待判识文件）
                RasterMaper rm = new RasterMaper(_argumentProvider.DataProvider, bandNos);
                inputRms.Add(rm);
                IRasterDataProvider dblvPrd = GeoDataDriver.Open(dblvFile) as IRasterDataProvider;
                RasterMaper dblvRm = new RasterMaper(dblvPrd, new int[] { 1 });
                inputRms.Add(dblvRm);
                RasterIdentify ri = new RasterIdentify(_argumentProvider.DataProvider.fileName);
                ri.ProductIdentify = "DST";
                ri.IsOutput2WorkspaceDir = true;
                IRasterDataProvider reRaster= CreateOutRaster(ri, dblvPrd, "0DRE");
                RasterMaper reRm = new RasterMaper(reRaster, new int[] { 1 });
                outputRms.Add(reRm);
                IRasterDataProvider optRaster = CreateOutRaster(ri, dblvPrd, "0OPT");
                RasterMaper optRm = new RasterMaper(optRaster, new int[] { 1 });
                outputRms.Add(optRm);
                IRasterDataProvider denRaster = CreateOutRaster(ri, dblvPrd, "DDEN");
                RasterMaper denRm = new RasterMaper(denRaster, new int[] { 1 });
                outputRms.Add(denRm);
                //栅格数据映射
                RasterMaper[] fileIns = inputRms.ToArray();
                RasterMaper[] fileOuts = outputRms.ToArray();
                //创建处理模型
                RasterProcessModel<short, float> rfr = new RasterProcessModel<short, float>();
                rfr.SetRaster(fileIns, fileOuts);
                #region
                rfr.RegisterCalcModel(new RasterCalcHandler<short, float>((rvInVistor, rvOutVistor, aoi) =>
                {
                    int nindex;
                    double ocdiff = 0, mindiff = 0, den = 0,gcon;
                    float den0, rpsize=0,rpsize0,rpmin = 0.01f, rpmax = 100;
                    double sgma=2.0, sgdust=2.5e-12;
                    double dr=(Math.Log10(rpmax)-Math.Log10(rpmin))/100;
                    if (rvInVistor[0].RasterBandsData[0] != null)
                    {
                        int length=rvInVistor[0].RasterBandsData[0].Length;
                        for (int i = 0; i < length; i++)
                        {
                            if (rvInVistor[1].RasterBandsData[0][i] == 1)
                            {
                                float bt31 = rvInVistor[0].RasterBandsData[1][i]/10f;
                                float btd3132 = (rvInVistor[0].RasterBandsData[1][i] - rvInVistor[0].RasterBandsData[2][i])/10f;
                                float btd2931 = (rvInVistor[0].RasterBandsData[0][i] - rvInVistor[0].RasterBandsData[1][i])/10f;
                                for (int m = 0; m < 50; m++)
                                {
                                    for (int n = 0; n < 29; n++)
                                    {
                                        ocdiff = Math.Pow(btd3132 - argument.Cbtd3132[m, n], 2) + Math.Pow((btd2931 - argument.Cbtd2931[m, n]), 2) * mbtd
                                                 + Math.Pow((bt31 - argument.Cbt31[m, n]), 2) * mbt;
                                        if (ocdiff<mindiff||(m == 0 && n == 0))
                                        {
                                            mindiff = ocdiff;
                                            rvOutVistor[0].RasterBandsData[0][i]=argument.Dref[n];
                                            rvOutVistor[1].RasterBandsData[0][i]=argument.Dopt[m];
                                        }
                                    }
                                }
                                //计算rvOutVistor[2].RasterBandsData[0][i]载沙量
                                if (rvOutVistor[0].RasterBandsData[0][i] <= 1)
                                    nindex = (int)(rvOutVistor[0].RasterBandsData[0][i] * 10);
                                else
                                    nindex = (int)(rvOutVistor[0].RasterBandsData[0][i]) + 9;
                                den0 = rvOutVistor[1].RasterBandsData[0][i] / argument.Ext55[nindex-1];
                                double ravg=Calravg(rvOutVistor[1].RasterBandsData[0][i],sgma);
                                double tcv = 0;
                                rpsize = rpmin;
                                for (int j = 0; j < 100; j++)
                                {
                                    //源代码中未定义type为1,2所需变量初始值 
                                    den=DType(ntype,rpsize,0,0,0,0,0,0,0,den0,ravg,sgma);
                                    rpsize0=rpsize;
                                    rpsize=(float)Math.Pow(10,(Math.Log10(rpsize)+dr));
                                    gcon = 4f / 3 * Math.PI * Math.Pow(rpsize0 ,3) * den * (rpsize - rpsize0);
                                    tcv += gcon;
                                }
                                float value=(float)((tcv * sgdust * 1.0e+9*10));
                                rvOutVistor[2].RasterBandsData[0][i] = value > 50f ? 50f : value;
                            }
                        }
                    }
                }));
                #endregion
                rfr.Excute();
                FileExtractResult reResult = new FileExtractResult("0DRE", reRaster.fileName, true);
                reResult.SetDispaly(false);
                FileExtractResult optResult = new FileExtractResult("0OPT", optRaster.fileName, true);
                optResult.SetDispaly(false);
                FileExtractResult denResult = new FileExtractResult("DDEN", denRaster.fileName, true);
                ExtractResultArray resultArray = new ExtractResultArray(_subProductDef.Identify);
                resultArray.Add(reResult);
                resultArray.Add(optResult);
                //生成载沙量
                resultArray.Add(denResult);
                return resultArray;
            }
            finally
            {
                for (int i = 1; i < inputRms.Count; i++)
                {
                    if (inputRms[i].Raster != null)
                        inputRms[i].Raster.Dispose();
                }
                foreach (RasterMaper rm in outputRms)
                {
                    if (rm.Raster != null)
                        rm.Raster.Dispose();
                }
            }
        }

        private IExtractResult VirrAlgorithm()
        {
            int farInfraredNo1, farInfraredNo2;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            farInfraredNo1 = TryGetBandNo(bandNameRaster, "FarInfrared1");
            farInfraredNo2 = TryGetBandNo(bandNameRaster, "FarInfrared2");
            if (farInfraredNo1 == -1 || farInfraredNo2 == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            int[] bandNos = new int[] { farInfraredNo1, farInfraredNo2 };
            if (_argumentProvider.GetArg("BinaryFile") == null)
            {
                PrintInfo("获取判识结果文件失败。");
                return null;
            }
            string dblvFile = _argumentProvider.GetArg("BinaryFile").ToString();
            if (string.IsNullOrEmpty(dblvFile) || !File.Exists(dblvFile))
            {
                PrintInfo("获取判识结果文件失败。");
                return null;
            }
            string angleFile = _argumentProvider.GetArg("AngleFile").ToString();
            if (string.IsNullOrEmpty(angleFile) || !File.Exists(angleFile))
            {
                PrintInfo("获取太阳天顶角文件失败。");
                return null;
            }
            //参数检查与获取
            string landTypeFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\DST\\GlobleLandCover.dat");
            if(!File.Exists(landTypeFile))
            {
                PrintInfo("土地类型文件不存在！");
                return null;
            }
            string tempFile=_argumentProvider.GetArg("TemperatureFile").ToString();
            if (string.IsNullOrEmpty(tempFile) || !File.Exists(tempFile))
            {
                PrintInfo("获取温度场文件失败。");
                return null;
            }
            string pressureFile=_argumentProvider.GetArg("PressureFile").ToString();
            if (string.IsNullOrEmpty(pressureFile) || !File.Exists(pressureFile))
            {
                PrintInfo("获取气压场文件失败。");
                return null;
            }
            //进行光学厚度与沙尘粒子有效半径计算需要文件：1、原始影像数据；2、查算表数据；3、太阳高度角数据
            List<RasterMaper> inputRms = new List<RasterMaper>();
            List<RasterMaper> outputRms = new List<RasterMaper>();
            try
            {
                //当前影像（待判识文件）
                RasterMaper rm = new RasterMaper(_argumentProvider.DataProvider, bandNos);
                inputRms.Add(rm);
                //判识结果文件
                IRasterDataProvider dblvPrd = GeoDataDriver.Open(dblvFile) as IRasterDataProvider;
                RasterMaper dblvRm = new RasterMaper(dblvPrd, new int[] { 1 });
                inputRms.Add(dblvRm);
                //太阳天顶角文件
                IRasterDataProvider angleDataPrd = GeoDataDriver.Open(angleFile) as IRasterDataProvider;
                RasterMaper anglerm = new RasterMaper(angleDataPrd, new int[] { 1 });
                inputRms.Add(anglerm);
                //土地类型文件
                IRasterDataProvider landTypeDataPrd=GeoDataDriver.Open(landTypeFile) as IRasterDataProvider;
                RasterMaper landTypeRm= new RasterMaper(landTypeDataPrd,new int[]{1});
                inputRms.Add(landTypeRm);
                //温度场文件
                IRasterDataProvider tempDataPrd=GeoDataDriver.Open(landTypeFile) as IRasterDataProvider;
                RasterMaper tempRm= new RasterMaper(tempDataPrd,new int[]{1});
                inputRms.Add(tempRm);
                //气压场文件
                IRasterDataProvider pressureDataPrd=GeoDataDriver.Open(pressureFile) as IRasterDataProvider;
                RasterMaper pressureRm=new RasterMaper(tempDataPrd,new int[]{1});
                inputRms.Add(pressureRm);
                RasterIdentify ri = new RasterIdentify(_argumentProvider.DataProvider.fileName);
                ri.ProductIdentify = "DST";
                ri.IsOutput2WorkspaceDir = true;
                IRasterDataProvider htRaster = CreateOutRaster(ri, dblvPrd, "0DHT");
                RasterMaper htRm = new RasterMaper(htRaster, new int[] { 1 });
                outputRms.Add(htRm);
                IRasterDataProvider optRaster = CreateOutRaster(ri, dblvPrd, "0OPT");
                RasterMaper optRm = new RasterMaper(optRaster, new int[] { 1 });
                outputRms.Add(optRm);
                //栅格数据映射
                RasterMaper[] fileIns = inputRms.ToArray();
                RasterMaper[] fileOuts = outputRms.ToArray();
                //创建处理模型
                RasterProcessModel<short, float> rfr = new RasterProcessModel<short, float>();
                rfr.SetRaster(fileIns, fileOuts);
                #region
                rfr.RegisterCalcModel(new RasterCalcHandler<short, float>((rvInVistor, rvOutVistor, aoi) =>
                {
                    int landType;
                    float t1 = 0, t2 = 0;
                    if (rvInVistor[0].RasterBandsData[0] != null)
                    {
                         int length=rvInVistor[0].RasterBandsData[0].Length;
                         for (int i = 0; i < length; i++)
                         {
                             if (rvInVistor[1].RasterBandsData[0][i] == 1)
                             {
                                 if(rvInVistor[4].RasterBandsData[0][i]>263)
                                 {
                                     t1 = rvInVistor[4].RasterBandsData[0][i] - 3;
                                     t2 = rvInVistor[4].RasterBandsData[0][i] + 3;
                                 }
                                 if (rvInVistor[4].RasterBandsData[0][i] < rvInVistor[0].RasterBandsData[0][i])
                                 {
                                     t1 = rvInVistor[0].RasterBandsData[0][i];
                                     t2 = rvInVistor[0].RasterBandsData[0][i] + 5;
                                 }
                                 //for(int m=0;m<)
                                 landType=rvInVistor[3].RasterBandsData[0][i];
                                 //water
                                 if (landType == 0 || landType >= 17)
                                 {

                                 }
                                 //desert
                                 else if (landType == 16)
                                 {
                                     
                                 }
                                 else
                                 {

                                 }
                             }
                         }
                    }
                }));
                #endregion
                rfr.Excute();
                FileExtractResult htResult = new FileExtractResult("0DHT", htRaster.fileName, true);
                htResult.SetDispaly(false);
                FileExtractResult optResult = new FileExtractResult("0OPT", optRaster.fileName, true);
                optResult.SetDispaly(false);
                ExtractResultArray resultArray = new ExtractResultArray(_subProductDef.Identify);
                resultArray.Add(htResult);
                resultArray.Add(optResult);
                return resultArray;
            }
            finally
            {
                for (int i = 1; i < inputRms.Count; i++)
                {
                    if (inputRms[i].Raster != null)
                        inputRms[i].Raster.Dispose();
                }
                foreach (RasterMaper rm in outputRms)
                {
                    if (rm.Raster != null)
                        rm.Raster.Dispose();
                }
            }
        }

        private double DType(int ntype, float r,float dnr0, float r0, float alph1,float aa, float alph2, float bb,float vv,float den0, double rm, double sgma)
        {
            double den;
            switch (ntype)
            {
                case 1:
                    {
                        den = dnr0 * Math.Pow((r0 / r), alph1);
                        break;
                    }
                case 2:
                    {
                        den=Math.Pow(aa*r,alph2)*Math.Exp(Math.Pow(-bb*r,vv));
                        break;
                    }
                case 3:
                default:
                    {
                        double cons=den0/(Math.Sqrt(2*Math.PI)*Math.Log(sgma));
                        den=cons*(1.0/r)*Math.Exp(-Math.Pow(Math.Log(r/rm),2)/(2*Math.Pow(Math.Log(sgma),2)));
                        break;
                    }
            }
            return den;
        }

        private double Calravg(float refData,double sgma)
        {
            return refData / (Math.Exp(2.5 * Math.Pow(Math.Log(sgma), 2)));
        }

        private IRasterDataProvider CreateOutRaster(RasterIdentify rasterIdentify, IRasterDataProvider rasterProvider, string subProductIdentify)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = rasterProvider.CoordEnvelope;
            float resX = rasterProvider.ResolutionX;
            float resY = rasterProvider.ResolutionY;
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            rasterIdentify.SubProductIdentify = subProductIdentify;
            string outFileName = rasterIdentify.ToWksFullFileName(".dat");
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, enumDataType.Float, mapInfo) as RasterDataProvider;
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
