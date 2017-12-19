using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using GeoDo.Project;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.UHI
{
    public class SubProductBinaryLST : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

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
            if (_argumentProvider == null || _argumentProvider.DataProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "NOAA16Algorithm" ||
                _argumentProvider.GetArg("AlgorithmName").ToString() == "NOAA18Algorithm" ||
                _argumentProvider.GetArg("AlgorithmName").ToString() == "FY3Algorithm")
            {
                return ExtractLST();
            }
            else if (_argumentProvider.GetArg("AlgorithmName").ToString() == "CMAAlgorithm")
            {
                return ExtractCMA();
            }
            else
            {
                PrintInfo("指定的算法没有实现。");
                return null;
            }
        }

        private IExtractResult ExtractCMA()
        {
            return null;
        }

        private IExtractResult ExtractLST()
        {
            float curNDVI = 0f;
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
            if (VisibleCH == -1 || NearInfraredCH == -1 || MiddInfraredCH == -1 || FarInfrared11CH == -1 || FarInfrared12CH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            IRasterDataProvider currPrd = _argumentProvider.DataProvider;
            if (currPrd == null)
            {
                PrintInfo("计算城市热岛的主文件未提供,请检查!");
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

            float NearInfraredCLMMin = (float)_argumentProvider.GetArg("NearInfraredCLMMin");
            float FarInfrared11CLMMax = (float)_argumentProvider.GetArg("FarInfrared11CLMMax");
            float FarInfrared1112CLMMin = (float)_argumentProvider.GetArg("FarInfrared1112CLMMin");
            float FarInfrared11WaterMin = (float)_argumentProvider.GetArg("FarInfrared11WaterMin");
            float NDVIWaterMax = (float)_argumentProvider.GetArg("NDVIWaterMax");

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
            int[] bands = new int[] { VisibleCH, NearInfraredCH, MiddInfraredCH, FarInfrared11CH, FarInfrared12CH };
            for (int i = 0; i < bands.Length; i++)
            {
                if (bands[i] > currPrd.BandCount)
                {
                    PrintInfo("波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                    return null;
                }
            }

            int[] aoi = _argumentProvider.AOI;
            Size size = new Size(currPrd.Width, currPrd.Height);
            Rectangle aoiRect = AOIHelper.ComputeAOIRect(aoi, size);

            MemPixelFeatureMapper<UInt16>  result = new MemPixelFeatureMapper<UInt16>("0LST", 1000, size, currPrd.CoordEnvelope, currPrd.SpatialRef);
            rpVisitor.VisitPixel(aoiRect, aoi, bands,
             (index, values) =>
             {
                 if (values[0] == 0)
                     result.Put(index, 0);
                 else if (values[1] / NearInfraredZoom > NearInfraredCLMMin && values[3] / FarInfrared11Zoom < FarInfrared11CLMMax && Math.Abs(values[2] / MiddInfraredZoom - values[3] / FarInfrared11Zoom) > FarInfrared1112CLMMin)
                     result.Put(index, defCloudy);
                 else
                 {
                     curNDVI = GetNDVI(values, 1, 0);
                     if (values[3] / FarInfrared11Zoom > FarInfrared11WaterMin && curNDVI < NDVIWaterMax)
                     {
                         result.Put(index, defWater);
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

                         result.Put(index, (UInt16)((a0 + P * (values[3] / FarInfrared11Zoom + values[4] / FarInfrared12Zoom) / 2
                                           + M * (values[3] / FarInfrared11Zoom - values[4] / FarInfrared12Zoom) / 2) * lstZoom));
                     }
                 }
             });
            return GenrateIInterested(result, currPrd);
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

        private IFileExtractResult GenrateIInterested(MemPixelFeatureMapper<UInt16> result, IRasterDataProvider currPrd)
        {
            RasterIdentify id = new RasterIdentify(currPrd.fileName.ToUpper());
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "UHI";
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
    }
}
