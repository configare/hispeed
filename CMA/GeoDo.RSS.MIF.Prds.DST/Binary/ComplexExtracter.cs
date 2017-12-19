#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/11/6 13:54:47
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
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.DST
{
    /// <summary>
    /// 类名：ComplexExtracter
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/11/6 13:54:47
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class ComplexExtracter
    {
        private IContextMessage _contextMessage;
        private IArgumentProvider _argumentProvider;

        public IExtractResult Extracting(IArgumentProvider argProvider,ref IPixelIndexMapper result, IContextMessage contextMessage, Action<int, string> progressTracker)
        {
            _contextMessage = contextMessage;
            _argumentProvider = argProvider;
            int visi64No,visi47No,visi138No, nearNo, midNo, farNo11,farNo12, shortNo;
            IBandNameRaster bandNameRaster = argProvider.DataProvider as IBandNameRaster;
            visi64No = TryGetBandNo(bandNameRaster, "Visible64");
            nearNo = TryGetBandNo(bandNameRaster, "NearInfrared");
            midNo = TryGetBandNo(bandNameRaster, "MiddleInfrared");
            farNo11 = TryGetBandNo(bandNameRaster, "FarInfrared11");
            farNo12 = TryGetBandNo(bandNameRaster, "FarInfrared12");
            shortNo = TryGetBandNo(bandNameRaster, "ShortInfrared");
            visi47No = TryGetBandNo(bandNameRaster, "Visible47");
            visi138No = TryGetBandNo(bandNameRaster, "Visible138");
            if (visi64No == -1 || nearNo == -1 || midNo == -1 || farNo11 == -1 ||farNo12==-1|| shortNo == -1||visi47No==-1||visi138No==-1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            int[] bandNos = new int[] { visi64No, nearNo, midNo, farNo11,farNo12, shortNo, visi47No, visi138No };
            DstFY3ExtractArgSet set = argProvider.GetArg("Arguments") as DstFY3ExtractArgSet;
            if (set == null)
            {
                PrintInfo("获取阈值参数失败。");
                return null;
            }
            //参数检查与获取
            string landTypeFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\DST\\GlobleLandCover.dat");
            if(!File.Exists(landTypeFile))
            {
                PrintInfo("土地类型文件不存在！");
                return null;
            }
            if (argProvider.GetArg("LSTFile") == null || argProvider.GetArg("HeightFile") == null || argProvider.GetArg("AngleFile") == null)
                return null;
            string lstFileName = argProvider.GetArg("LSTFile").ToString();
            if (string.IsNullOrEmpty(lstFileName)||!File.Exists(lstFileName))
                return null;
            string heightFileName = argProvider.GetArg("HeightFile").ToString();
            if (string.IsNullOrEmpty(heightFileName) || !File.Exists(heightFileName))
                return null;
            string angleFileName = argProvider.GetArg("AngleFile").ToString();
            if (string.IsNullOrEmpty(angleFileName) || !File.Exists(angleFileName))
                return null;
            DstFY3ExtractArgSet extractArg = argProvider.GetArg("Arguments") as DstFY3ExtractArgSet;
            if (extractArg == null)
                return null;
            IRasterDataProvider dataPrd = argProvider.DataProvider;
            if (dataPrd == null)
                return null;
            return DoExtracte(ref result,extractArg, bandNos, landTypeFile, lstFileName, heightFileName,angleFileName);
        }

        private int TryGetBandNo(IBandNameRaster bandNameRaster, string argName)
        {
            int bandNo = (int)_argumentProvider.GetArg(argName);
            if (bandNameRaster != null)
            {
                int newbandNo = -1;
                if (bandNameRaster.TryGetBandNoFromBandName(bandNo, out newbandNo))
                    bandNo = newbandNo;
            }
            return bandNo;
        }

        private IExtractResult DoExtracte(ref IPixelIndexMapper result,DstFY3ExtractArgSet argument, int[] bandNos, string landTypeFile, string lstFile, string heightFile, string angleFile)
        {
            List<RasterMaper> inputRms = new List<RasterMaper>();
            List<RasterMaper> outputRms = new List<RasterMaper>();
            try
            {
                //当前影像（待判识文件）
                RasterMaper rm = new RasterMaper(_argumentProvider.DataProvider, bandNos);
                inputRms.Add(rm);
                //土地类型参数文件
                IRasterDataProvider landTypeDataPrd = GeoDataDriver.Open(landTypeFile) as IRasterDataProvider;
                RasterMaper ltrm = new RasterMaper(landTypeDataPrd, new int[] { 1 });
                inputRms.Add(ltrm);
                //陆表高温文件
                IRasterDataProvider lstDataPrd = GeoDataDriver.Open(lstFile) as IRasterDataProvider;
                RasterMaper lstrm = new RasterMaper(lstDataPrd, new int[] { 1 });
                inputRms.Add(lstrm);
                //高度文件
                IRasterDataProvider heightDataPrd = GeoDataDriver.Open(heightFile) as IRasterDataProvider;
                RasterMaper heightrm = new RasterMaper(heightDataPrd, new int[] { 1 });
                inputRms.Add(heightrm);
                //太阳天顶角文件
                IRasterDataProvider angleDataPrd=GeoDataDriver.Open(angleFile) as IRasterDataProvider;
                RasterMaper anglerm = new RasterMaper(angleDataPrd, new int[] { 1 });
                inputRms.Add(anglerm);
                RasterIdentify ri = new RasterIdentify(_argumentProvider.DataProvider.fileName);
                ri.ProductIdentify = "DST";
                ri.IsOutput2WorkspaceDir = true;
                //输出文件：通过检测序号
                IRasterDataProvider idOutRaster1 = CreateOutRaster(ri, rm, "PID1");
                RasterMaper id1Rm = new RasterMaper(idOutRaster1, new int[] { 1 });
                outputRms.Add(id1Rm);
                IRasterDataProvider idOutRaster2 = CreateOutRaster(ri, rm, "PID2");
                RasterMaper id2Rm = new RasterMaper(idOutRaster2, new int[] { 1 });
                outputRms.Add(id2Rm);
                //共通过检测数
                IRasterDataProvider testOutRaster = CreateOutRaster(ri, rm, "PTST");
                RasterMaper testRm = new RasterMaper(testOutRaster, new int[] { 1 });
                outputRms.Add(testRm);
                //沙尘分数
                IRasterDataProvider scoreOutRaster = CreateOutRaster(ri, rm, "PSRE");
                RasterMaper scoreRm = new RasterMaper(scoreOutRaster, new int[] { 1 });
                outputRms.Add(scoreRm);
                //栅格数据映射
                RasterMaper[] fileIns = inputRms.ToArray();
                RasterMaper[] fileOuts = outputRms.ToArray();
                //创建处理模型
                RasterProcessModel<short, Int16> rfr = new RasterProcessModel<short, Int16>();
                rfr.SetRaster(fileIns, fileOuts);
                #region 
                rfr.RegisterCalcModel(new RasterCalcHandler<short, Int16>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData != null)
                    {
                        int length = rvInVistor[0].RasterBandsData[0].Length;
                        for (int i = 0; i < length; i++)
                        {
                            //判断条件：太阳天顶角（0~80）
                            //角度扩大100倍存储
                            Int16 angle=rvInVistor[4].RasterBandsData[0][i];
                            if (angle >= 8000 || angle <= 0)
                            {
                                rvOutVistor[1].RasterBandsData[0][i] = 1;
                                continue;
                            }
                            else
                            {
                                short landType = rvInVistor[1].RasterBandsData[0][i];
                                short height = rvInVistor[3].RasterBandsData[0][i];
                                short ref64 = rvInVistor[0].RasterBandsData[0][i];
                                short refBand2 = rvInVistor[0].RasterBandsData[1][i];
                                short tbb37 = rvInVistor[0].RasterBandsData[2][i];
                                short tbb11 = rvInVistor[0].RasterBandsData[3][i];
                                short tbb12 = rvInVistor[0].RasterBandsData[4][i];
                                short ref164 = rvInVistor[0].RasterBandsData[5][i];
                                short ref47 = rvInVistor[0].RasterBandsData[6][i];
                                short ref138 = rvInVistor[0].RasterBandsData[7][i];
                                int btd11_12 = tbb11 - tbb12;
                                int btd11_37 = tbb11 - tbb37;
                                float r47_64 = (float)ref47 / ref64;
                                float lstValue = (float)(rvInVistor[2].RasterBandsData[0][i] * 0.02);
                                float ndvi = (float)(refBand2 - ref64) / (refBand2 + ref64) * 1000;
                                float ndsi = (float)(ref64 - ref164) / (ref64 + ref164) * 1000;
                                float nddi = (float)(ref164 - ref47) / (ref164 + ref47) * 1000;
                                double idsi = ComputeIDSI(ref164, tbb11, tbb12);
                                double iddi = lstValue - tbb11 / 10f;
                                //水
                                if (landType == 0 || landType == 254 || landType == 255)
                                {
                                    if (tbb11 > 2400 & tbb11 < 2900 & ref64 > 200 & ref64 < 300 & btd11_12 < -10 & btd11_37 < -200 & btd11_37 > -800)  // 沙尘回溯
                                    {
                                        rvOutVistor[1].RasterBandsData[0][i] = 7;
                                        rvOutVistor[3].RasterBandsData[0][i] = 22;
                                        rvOutVistor[2].RasterBandsData[0][i] = 12;
                                        continue;
                                    }
                                    rvOutVistor[1].RasterBandsData[0][i] = 6;
                                    //条件一
                                    if (ref64 > argument.WaterArg.Ref650[0] && ref64 < argument.WaterArg.Ref650[3])
                                    {
                                        rvOutVistor[0].RasterBandsData[0][i] += 1;
                                        rvOutVistor[2].RasterBandsData[0][i] = 1;
                                        if (ref64 >= argument.WaterArg.Ref650[1] && ref64 <= argument.WaterArg.Ref650[2])
                                            rvOutVistor[3].RasterBandsData[0][i] += 2;
                                        else
                                            rvOutVistor[3].RasterBandsData[0][i] += 1;
                                        //条件二
                                        if (tbb11 > argument.WaterArg.TBB11[0] && tbb11 < argument.WaterArg.TBB11[3])
                                        {
                                            rvOutVistor[0].RasterBandsData[0][i] += 1;
                                            rvOutVistor[2].RasterBandsData[0][i] = 2;
                                            if (tbb11 >= argument.WaterArg.TBB11[1] && tbb11 <= argument.WaterArg.TBB11[2])
                                                rvOutVistor[3].RasterBandsData[0][i] += 2;
                                            else
                                                rvOutVistor[3].RasterBandsData[0][i] += 1;
                                            //条件三
                                            if (tbb37 > argument.WaterArg.TBB37[0] && tbb37 < argument.WaterArg.TBB37[3])
                                            {
                                                rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                rvOutVistor[2].RasterBandsData[0][i] = 3;
                                                if (tbb37 >= argument.WaterArg.TBB37[1] && tbb37 <= argument.WaterArg.TBB37[2])
                                                    rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                else
                                                    rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                //条件四
                                                if (btd11_12 > argument.WaterArg.BTD11_12[0] && btd11_12 < argument.WaterArg.BTD11_12[3])
                                                {
                                                    rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                    rvOutVistor[2].RasterBandsData[0][i] = 4;
                                                    if (btd11_12 >= argument.WaterArg.BTD11_12[1] && btd11_12 <= argument.WaterArg.BTD11_12[2])
                                                        rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                    else
                                                        rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                    //条件五
                                                    if (btd11_37 > argument.WaterArg.BTD11_37[0] && btd11_37 < argument.WaterArg.BTD11_37[3])
                                                    {
                                                        rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                        rvOutVistor[2].RasterBandsData[0][i] = 5;
                                                        if (btd11_37 >= argument.WaterArg.BTD11_37[1] && btd11_37 <= argument.WaterArg.BTD11_37[2])
                                                            rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                        else
                                                            rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                        //可选条件6-12：
                                                        if (r47_64 > argument.WaterArg.R47_64[0] && r47_64 < argument.WaterArg.R47_64[4])
                                                        {
                                                            rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                            rvOutVistor[2].RasterBandsData[0][i] = 6;
                                                            if (r47_64 >= argument.WaterArg.R47_64[1] && r47_64 <= argument.WaterArg.R47_64[2])
                                                                rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                            else
                                                                rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                        }
                                                        if (ndvi > argument.WaterArg.NDVI[0] && ndvi < argument.WaterArg.NDVI[3])
                                                        {
                                                            rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                            rvOutVistor[2].RasterBandsData[0][i] = 7;
                                                            if (ndvi >= argument.WaterArg.NDVI[1] && ndvi <= argument.WaterArg.NDVI[2])
                                                                rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                            else
                                                                rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                        }
                                                        if (ndsi > argument.WaterArg.NDSI[0] && ndsi < argument.WaterArg.NDSI[3])
                                                        {
                                                            rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                            rvOutVistor[2].RasterBandsData[0][i] = 8;
                                                            if (ndsi >= argument.WaterArg.NDSI[1] && ndsi <= argument.WaterArg.NDSI[2])
                                                                rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                            else
                                                                rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                        }
                                                        if (nddi > argument.WaterArg.NDDI[0] && nddi < argument.WaterArg.NDDI[3])
                                                        {
                                                            rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                            rvOutVistor[2].RasterBandsData[0][i] = 9;
                                                            if (nddi >= argument.WaterArg.NDDI[1] && nddi <= argument.WaterArg.NDDI[2])
                                                                rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                            else
                                                                rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                        }
                                                        if (ref47 > argument.WaterArg.Ref470[0] && ref47 < argument.WaterArg.Ref470[3])
                                                        {
                                                            rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                            rvOutVistor[2].RasterBandsData[0][i] = 10;
                                                            if (ref47 >= argument.WaterArg.NDDI[1] && ref47 <= argument.WaterArg.NDDI[2])
                                                                rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                            else
                                                                rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                        }
                                                        if (idsi > argument.WaterArg.IDSI[0] && idsi < argument.WaterArg.IDSI[3])
                                                        {
                                                            rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                            rvOutVistor[2].RasterBandsData[0][i] = 11;
                                                            if (idsi >= argument.WaterArg.IDSI[1] && idsi <= argument.WaterArg.IDSI[2])
                                                                rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                            else
                                                                rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                        }
                                                        if (ref138 > argument.WaterArg.Ref1380[0] && ref138 < argument.WaterArg.Ref1380[3])
                                                        {
                                                            rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                            rvOutVistor[2].RasterBandsData[0][i] = 12;
                                                            if (ref138 >= argument.WaterArg.Ref1380[1] && ref138 <= argument.WaterArg.Ref1380[2])
                                                                rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                            else
                                                                rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    
                                }
                                else
                                {
                                    //裸土、城市
                                    if (landType == 13 || landType == 16 || (ndvi <= 200 && iddi > 3))
                                    {
                                        //冰雪下垫面或陆地上非沙尘的判识指标
                                        if (landType == 15||ndvi>200)
                                        {
                                            rvOutVistor[1].RasterBandsData[0][i] = 2;
                                            continue;
                                        }
                                        rvOutVistor[1].RasterBandsData[0][i] = 1;
                                        //条件一
                                        if (ref64 > argument.BareLandArg.Ref650[0] && ref64 < argument.BareLandArg.Ref650[3])
                                        {
                                            rvOutVistor[0].RasterBandsData[0][i] += 1;
                                            rvOutVistor[2].RasterBandsData[0][i] = 1;
                                            if (ref64 >= argument.BareLandArg.Ref650[1] && ref64 <= argument.BareLandArg.Ref650[2])
                                                rvOutVistor[3].RasterBandsData[0][i] += 2;
                                            else
                                                rvOutVistor[3].RasterBandsData[0][i] += 1;
                                            //条件二 根据高度不同或不同土地类型TBB11参数不同
                                            bool isPassTBB11 = false;
                                            if (landType == 13)
                                            {
                                                if (tbb11 > argument.VegetationArg.TBB11[0] && tbb11 < argument.VegetationArg.TBB11[3])
                                                {
                                                    isPassTBB11 = true;
                                                    rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                    rvOutVistor[2].RasterBandsData[0][i] = 2;
                                                    if (tbb11 >= argument.VegetationArg.TBB11[1] && tbb11 <= argument.VegetationArg.TBB11[2])
                                                        rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                    else
                                                        rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                }
                                            }
                                            else if (height > 1500)
                                            {
                                                if (tbb11 > argument.BareLandArg.TBB11[0] - 0.06 * height && tbb11 < argument.BareLandArg.TBB11[3] - 0.06 * height)
                                                {
                                                    isPassTBB11 = true;
                                                    rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                    rvOutVistor[2].RasterBandsData[0][i] = 2;
                                                    if (tbb11 >= argument.BareLandArg.TBB11[1] && tbb11 <= argument.BareLandArg.TBB11[2])
                                                        rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                    else
                                                        rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                }
                                            }
                                            else if (height >= 500)
                                            {
                                                if (tbb11 > argument.BareLandArg.TBB11[0] && tbb11 < argument.BareLandArg.TBB11[3])
                                                {
                                                    isPassTBB11 = true;
                                                    rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                    rvOutVistor[2].RasterBandsData[0][i] = 2;
                                                    if (tbb11 >= argument.BareLandArg.TBB11[1] && tbb11 <= argument.BareLandArg.TBB11[2])
                                                        rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                    else
                                                        rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                }
                                            }
                                            else
                                            {
                                                if (tbb11 > argument.BareLandArg.TBB11[0] + 0.06 * height && tbb11 < argument.BareLandArg.TBB11[3] + 0.06 * height)
                                                {
                                                    isPassTBB11 = true;
                                                    rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                    rvOutVistor[2].RasterBandsData[0][i] = 2;
                                                    if (tbb11 >= argument.BareLandArg.TBB11[1] && tbb11 <= argument.BareLandArg.TBB11[2])
                                                        rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                    else
                                                        rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                }
                                            }
                                            //条件三
                                            if (tbb37 > argument.BareLandArg.TBB37[0] && tbb37 < argument.BareLandArg.TBB37[3] && isPassTBB11)
                                            {
                                                rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                rvOutVistor[2].RasterBandsData[0][i] = 3;
                                                if (tbb37 >= argument.BareLandArg.TBB37[1] && tbb37 <= argument.BareLandArg.TBB37[2])
                                                    rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                else
                                                    rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                //条件四
                                                if (btd11_12 > argument.BareLandArg.BTD11_12[0] && btd11_12 < argument.BareLandArg.BTD11_12[3])
                                                {
                                                    rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                    rvOutVistor[2].RasterBandsData[0][i] = 4;
                                                    if (btd11_12 >= argument.BareLandArg.BTD11_12[1] && btd11_12 <= argument.BareLandArg.BTD11_12[2])
                                                        rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                    else
                                                        rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                    //条件五
                                                    if (btd11_37 > argument.BareLandArg.BTD11_37[0] && btd11_37 < argument.BareLandArg.BTD11_37[3])
                                                    {
                                                        rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                        rvOutVistor[2].RasterBandsData[0][i] = 5;
                                                        if (btd11_37 >= argument.BareLandArg.BTD11_37[1] && btd11_37 <= argument.BareLandArg.BTD11_37[2])
                                                            rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                        else
                                                            rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                        //条件六
                                                        if (r47_64 > argument.BareLandArg.R47_64[0] && r47_64 < argument.BareLandArg.R47_64[3])
                                                        {
                                                            rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                            rvOutVistor[2].RasterBandsData[0][i] = 6;
                                                            if (r47_64 >= argument.BareLandArg.R47_64[1] && r47_64 <= argument.BareLandArg.R47_64[2])
                                                                rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                            else
                                                                rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                        }
                                                        //条件七
                                                        if (ndvi > argument.BareLandArg.NDVI[0] && ndvi < argument.BareLandArg.NDVI[3])
                                                        {
                                                            rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                            rvOutVistor[2].RasterBandsData[0][i] = 7;
                                                            if (ndvi >= argument.BareLandArg.NDVI[1] && ndvi <= argument.BareLandArg.NDVI[2])
                                                                rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                            else
                                                                rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                        }
                                                        //条件八
                                                        if (ndsi > argument.BareLandArg.NDSI[0] && ndsi < argument.BareLandArg.NDSI[3])
                                                        {
                                                            rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                            rvOutVistor[2].RasterBandsData[0][i] = 8;
                                                            if (ndsi >= argument.BareLandArg.NDSI[1] && ndsi <= argument.BareLandArg.NDSI[2])
                                                                rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                            else
                                                                rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                        }
                                                        //条件九
                                                        if (nddi > argument.BareLandArg.NDDI[0] && nddi < argument.BareLandArg.NDDI[3])
                                                        {
                                                            rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                            rvOutVistor[2].RasterBandsData[0][i] = 9;
                                                            if (nddi >= argument.BareLandArg.NDDI[1] && nddi <= argument.BareLandArg.NDDI[2])
                                                                rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                            else
                                                                rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                        }
                                                        //条件十
                                                        if (ref47 > argument.BareLandArg.Ref470[0] && ref47 < argument.BareLandArg.Ref470[3])
                                                        {
                                                            rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                            rvOutVistor[2].RasterBandsData[0][i] = 10;
                                                            if (ref47 >= argument.BareLandArg.Ref470[1] && ref47 <= argument.BareLandArg.Ref470[2])
                                                                rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                            else
                                                                rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                        }
                                                        //条件11
                                                        if (idsi > argument.BareLandArg.IDSI[0] && idsi < argument.BareLandArg.IDSI[3])
                                                        {
                                                            rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                            rvOutVistor[2].RasterBandsData[0][i] = 11;
                                                            if (idsi >= argument.BareLandArg.IDSI[1] && idsi <= argument.BareLandArg.IDSI[2])
                                                                rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                            else
                                                                rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                        }
                                                        //条件12
                                                        if (ref138 > argument.BareLandArg.Ref1380[0] && ref138 < argument.BareLandArg.Ref1380[3])
                                                        {
                                                            rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                            rvOutVistor[2].RasterBandsData[0][i] = 12;
                                                            if (ref138 >= argument.BareLandArg.Ref1380[1] && ref138 <= argument.BareLandArg.Ref1380[2])
                                                                rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                            else
                                                                rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //植被
                                    else if (landType < 20 && landType != 15 && ndvi < 400)
                                    {
                                        //冰雪下垫面等三种不为沙尘情况
                                        if ((ndsi > 200 && ref64 > 200) || (r47_64 > 1200) || (ndvi > 400))
                                        {
                                            rvOutVistor[1].RasterBandsData[0][i] = 2;
                                            continue;
                                        }
                                        //沙尘回溯
                                        if (tbb11 > 2400 && tbb11 < 2850 && ref64 > 180 && ref64 < 500 && btd11_12 < -8 && r47_64 < 1000)
                                        {
                                            rvOutVistor[1].RasterBandsData[0][i] = 5;
                                            rvOutVistor[2].RasterBandsData[0][i] = 12;
                                            rvOutVistor[3].RasterBandsData[0][i] = 22;
                                            continue;
                                        }
                                        rvOutVistor[1].RasterBandsData[0][i] = 4;
                                        if (ref64 > argument.VegetationArg.Ref650[0] && ref64 < argument.VegetationArg.Ref650[3])
                                        {
                                            rvOutVistor[0].RasterBandsData[0][i] += 1;
                                            rvOutVistor[2].RasterBandsData[0][i] = 1;
                                            if (ref64 >= argument.VegetationArg.Ref650[1] && ref64 <= argument.VegetationArg.Ref650[2])
                                                rvOutVistor[3].RasterBandsData[0][i] += 2;
                                            else
                                                rvOutVistor[3].RasterBandsData[0][i] += 1;
                                        }
                                        //条件二 根据高度不同或不同土地类型TBB11参数不同
                                        bool isPassTBB11 = false;
                                        if (height > 2000)
                                        {
                                            if (tbb11 > argument.VegetationArg.TBB11[0] - 0.06 * height && tbb11 < argument.VegetationArg.TBB11[3] - 0.06 * height)
                                            {
                                                isPassTBB11 = true;
                                                rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                rvOutVistor[2].RasterBandsData[0][i] = 2;
                                                if (tbb11 >= argument.BareLandArg.TBB11[1] && tbb11 <= argument.BareLandArg.TBB11[2])
                                                    rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                else
                                                    rvOutVistor[3].RasterBandsData[0][i] += 1;
                                            }
                                        }
                                        else if (height >= 500)
                                        {
                                            if (tbb11 > argument.VegetationArg.TBB11[0] && tbb11 < argument.VegetationArg.TBB11[3])
                                            {
                                                isPassTBB11 = true;
                                                rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                rvOutVistor[2].RasterBandsData[0][i] = 2;
                                                if (tbb11 >= argument.VegetationArg.TBB11[1] && tbb11 <= argument.VegetationArg.TBB11[2])
                                                    rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                else
                                                    rvOutVistor[3].RasterBandsData[0][i] += 1;
                                            }
                                        }
                                        else
                                        {
                                            if (tbb11 > argument.VegetationArg.TBB11[0] + 0.06 * height && tbb11 < argument.VegetationArg.TBB11[3] + 0.06 * height)
                                            {
                                                isPassTBB11 = true;
                                                rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                rvOutVistor[2].RasterBandsData[0][i] = 2;
                                                if (tbb11 >= argument.VegetationArg.TBB11[1] && tbb11 <= argument.VegetationArg.TBB11[2])
                                                    rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                else
                                                    rvOutVistor[3].RasterBandsData[0][i] += 1;
                                            }
                                        }
                                        //条件三
                                        if (tbb37 > argument.VegetationArg.TBB37[0] && tbb37 < argument.VegetationArg.TBB37[3] && isPassTBB11)
                                        {
                                            rvOutVistor[0].RasterBandsData[0][i] += 1;
                                            rvOutVistor[2].RasterBandsData[0][i] = 3;
                                            if (tbb37 >= argument.VegetationArg.TBB37[1] && tbb37 <= argument.VegetationArg.TBB37[2])
                                                rvOutVistor[3].RasterBandsData[0][i] += 2;
                                            else
                                                rvOutVistor[3].RasterBandsData[0][i] += 1;
                                            //条件四
                                            if (btd11_12 > argument.VegetationArg.BTD11_12[0] && btd11_12 < argument.VegetationArg.BTD11_12[3])
                                            {
                                                rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                rvOutVistor[2].RasterBandsData[0][i] = 4;
                                                if (btd11_12 >= argument.VegetationArg.BTD11_12[1] && btd11_12 <= argument.VegetationArg.BTD11_12[2])
                                                    rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                else
                                                    rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                //条件五
                                                if (btd11_37 > argument.VegetationArg.BTD11_37[0] && btd11_37 < argument.VegetationArg.BTD11_37[3])
                                                {
                                                    rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                    rvOutVistor[2].RasterBandsData[0][i] = 5;
                                                    if (btd11_37 >= argument.VegetationArg.BTD11_37[1] && btd11_37 <= argument.VegetationArg.BTD11_37[2])
                                                        rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                    else
                                                        rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                    //条件六
                                                    if (r47_64 > argument.VegetationArg.R47_64[0] && r47_64 < argument.VegetationArg.R47_64[3])
                                                    {
                                                        rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                        rvOutVistor[2].RasterBandsData[0][i] = 6;
                                                        if (r47_64 >= argument.VegetationArg.R47_64[1] && r47_64 <= argument.VegetationArg.R47_64[2])
                                                            rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                        else
                                                            rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                    }
                                                    //条件七
                                                    if (ndvi > argument.VegetationArg.NDVI[0] && ndvi < argument.VegetationArg.NDVI[3])
                                                    {
                                                        rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                        rvOutVistor[2].RasterBandsData[0][i] = 7;
                                                        if (ndvi >= argument.VegetationArg.NDVI[1] && ndvi <= argument.VegetationArg.NDVI[2])
                                                            rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                        else
                                                            rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                    }
                                                    //条件八
                                                    if (ndsi > argument.VegetationArg.NDSI[0] && ndsi < argument.VegetationArg.NDSI[3])
                                                    {
                                                        rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                        rvOutVistor[2].RasterBandsData[0][i] = 8;
                                                        if (ndsi >= argument.VegetationArg.NDSI[1] && ndsi <= argument.VegetationArg.NDSI[2])
                                                            rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                        else
                                                            rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                    }
                                                    //条件九
                                                    if (nddi > argument.VegetationArg.NDDI[0] && nddi < argument.VegetationArg.NDDI[3])
                                                    {
                                                        rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                        rvOutVistor[2].RasterBandsData[0][i] = 9;
                                                        if (nddi >= argument.VegetationArg.NDDI[1] && nddi <= argument.VegetationArg.NDDI[2])
                                                            rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                        else
                                                            rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                    }
                                                    //条件十
                                                    if (ref47 > argument.VegetationArg.Ref470[0] && ref47 < argument.VegetationArg.Ref470[3])
                                                    {
                                                        rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                        rvOutVistor[2].RasterBandsData[0][i] = 10;
                                                        if (ref47 >= argument.VegetationArg.Ref470[1] && ref47 <= argument.VegetationArg.Ref470[2])
                                                            rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                        else
                                                            rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                    }
                                                    //条件11
                                                    if (idsi > argument.VegetationArg.IDSI[0] && idsi < argument.VegetationArg.IDSI[3])
                                                    {
                                                        rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                        rvOutVistor[2].RasterBandsData[0][i] = 11;
                                                        if (idsi >= argument.VegetationArg.IDSI[1] && idsi <= argument.VegetationArg.IDSI[2])
                                                            rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                        else
                                                            rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                    }
                                                    //条件12
                                                    if (ref138 > argument.VegetationArg.Ref1380[0] && ref138 < argument.VegetationArg.Ref1380[3])
                                                    {
                                                        rvOutVistor[0].RasterBandsData[0][i] += 1;
                                                        rvOutVistor[2].RasterBandsData[0][i] = 12;
                                                        if (ref138 >= argument.VegetationArg.Ref1380[1] && ref138 <= argument.VegetationArg.Ref1380[2])
                                                            rvOutVistor[3].RasterBandsData[0][i] += 2;
                                                        else
                                                            rvOutVistor[3].RasterBandsData[0][i] += 1;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }));
                #endregion
                rfr.Excute();
                //根据Test生成判识结果
                List<int> indexList = new List<int>();
                IRasterPixelsVisitor<short> visitor = new RasterPixelsVisitor<short>(new ArgumentProvider(testOutRaster, null));
                visitor.VisitPixel(new int[] { 1 },
                    (idx, values) =>
                    {
                        if (values[0] >= 5)
                            indexList.Add(idx);
                    });
                if (argument.isSmooth)
                {
                    result.Put(new ExtractResultSmoother().Smooth(testOutRaster.Width, testOutRaster.Height, indexList.ToArray()));
                }
                else
                    result.Put(indexList.ToArray());
                //result.Tag = new DstFeatureFY3Collection("沙尘辅助信息", GetFY3DisplayInfo(bandNos, heightDataPrd, angleDataPrd));
                FileExtractResult testResult = new FileExtractResult("DBLV", testOutRaster.fileName, true);
                testResult.SetDispaly(false);
                FileExtractResult scoreResult = new FileExtractResult("DBLV", scoreOutRaster.fileName, true);
                scoreResult.SetDispaly(false);
                FileExtractResult id1Result = new FileExtractResult("DBLV", idOutRaster1.fileName, true);
                id1Result.SetDispaly(false);
                FileExtractResult id2Result = new FileExtractResult("DBLV", idOutRaster2.fileName, true);
                id2Result.SetDispaly(false);
                ExtractResultArray resultArray = new ExtractResultArray("DBLV");
                resultArray.Add(result);
                resultArray.Add(testResult);
                resultArray.Add(scoreResult);
                resultArray.Add(id1Result);
                resultArray.Add(id2Result);
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

        private Dictionary<int, DstFeatureFY3> GetFY3DisplayInfo(int[] bandNos, IRasterDataProvider heightRaster, IRasterDataProvider solZenRaster)
        {
            if (_argumentProvider.DataProvider == null)
                return null;
            if (heightRaster == null)
                return null;
            //VirtualRasterDataProvider virtualPrd = new VirtualRasterDataProvider(new IRasterDataProvider[] {_argumentProvider.DataProvider,
                                                                                                           //heightRaster,solZenRaster});
            ArgumentProvider argPrd = new Core.ArgumentProvider(_argumentProvider.DataProvider, null);
            Dictionary<int, DstFeatureFY3> features = new Dictionary<int, DstFeatureFY3>();
            DstFeatureFY3 tempSnw = null;
            RasterPixelsVisitor<Int16> rpVisitor = new RasterPixelsVisitor<Int16>(argPrd);
            //if (bandNos == null || bandNos.Length != 5)
            //    return null;
            //List<int> bandVisitorNo = new List<int>();
            //bandVisitorNo.AddRange(bandNos);
            //int bandIndex = _argumentProvider.DataProvider.BandCount + 1;
            //bandVisitorNo.AddRange(new int[] { bandIndex, bandIndex + 1 });
            //bandVisitorNo.Add(_argumentProvider.DataProvider.BandCount + heightRaster.BandCount + 1);
            rpVisitor.VisitPixel(bandNos.ToArray(),
                (index, values) =>
                {
                    try
                    {
                        tempSnw = new DstFeatureFY3();
                        //tempSnw.SolarZenith = values[bandIndex] / 100f;
                        //tempSnw.Height = values[bandIndex-1];
                        tempSnw.Ref650 = values[0];
                        tempSnw.TBB11 = values[3];
                        tempSnw.TBB37 = values[2];
                        tempSnw.BTD11_12 = values[3] - values[4];
                        tempSnw.BTD11_37 = values[3] - values[2];
                        tempSnw.R47_64 = (float)values[6] / values[0];
                        tempSnw.NDVI = (float)(values[1] - values[0]) / (values[1] + values[0]) * 1000;
                        tempSnw.NDSI = (float)(values[0] - values[5]) / (values[0] + values[5]) * 1000;
                        tempSnw.NDDI = (float)(values[5] - values[6]) / (values[5] + values[6]) * 1000;
                        tempSnw.Ref470 = values[6];
                        tempSnw.IDSI = ComputeIDSI(values[5], values[3], values[4]);
                        tempSnw.Ref1380 = values[7];
                        features.Add(index, tempSnw);
                    }
                    catch
                    {
                        
                    }
                }
            );
            return features;
        }


        private double ComputeIDSI(short ref164, short tbb11, short tbb12)
        {
            double icsd = 10 * (Math.Exp(0.8 * (ref164 / 1000f)) - 1);
            float tbbR = ((tbb12 / 10f - 1) / (tbb11 / 10f) - 1) * 10;
            double idsi = icsd * Math.Exp(tbbR) * 10;
            if (idsi > 100)
                idsi = 100;
            return idsi;
        }

        private IRasterDataProvider CreateOutRaster(RasterIdentify rasterIdentify, RasterMaper rasterMaper, string subProductIdentify)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = rasterMaper.Raster.CoordEnvelope;
            float resX = rasterMaper.Raster.ResolutionX;
            float resY = rasterMaper.Raster.ResolutionY;
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            rasterIdentify.SubProductIdentify = subProductIdentify;
            string outFileName = rasterIdentify.ToWksFullFileName(".dat");
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, enumDataType.Int16, mapInfo) as RasterDataProvider;
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
