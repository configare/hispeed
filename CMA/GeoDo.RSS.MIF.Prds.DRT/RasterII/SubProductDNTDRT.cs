using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public class SubProductDNTDRT : CmaDrtMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductDNTDRT()
            : base()
        {
        }

        public SubProductDNTDRT(SubProductDef subProductDef)
            : base(subProductDef)
        {
            _identify = subProductDef.Identify;
            _name = subProductDef.Name;
            if (subProductDef.Algorithms != null)
            {
                _algorithmDefs = subProductDef.Algorithms.ToList();
            }
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "DntAlgorithm")
            {
                return DemAlgorithm(progressTracker);
            }
            return null;
        }

        private IExtractResult DemAlgorithm(Action<int, string> progressTracker)
        {
            int dayCh = (int)_argumentProvider.GetArg("DayBandCH");
            int nightCh = (int)_argumentProvider.GetArg("NightBandCH");
            string dayFile = _argumentProvider.GetArg("DTDayFile").ToString();
            string nightFile = _argumentProvider.GetArg("DTNightFile").ToString();
            Int16 dayMinValue = Convert.ToInt16(_argumentProvider.GetArg("DayMinT"));
            Int16 nightMinValue = Convert.ToInt16(_argumentProvider.GetArg("NightMinT"));
            Int16 cloudValue = Convert.ToInt16(_argumentProvider.GetArg("CloudValue"));
            Int16 validValue = Convert.ToInt16(_argumentProvider.GetArg("ValidValue"));

            if (string.IsNullOrEmpty(dayFile) || string.IsNullOrEmpty(nightFile) || dayCh == -1 || nightCh == -1)
            {
                PrintInfo("亮温差计算所用文件或通道未设置完全,请检查!");
                return null;
            }

            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider dayPrd = null;
            IRasterDataProvider nightPrd = null;
            try
            {
                dayPrd = RasterDataDriver.Open(dayFile) as IRasterDataProvider;
                if (dayPrd.BandCount < dayCh)
                {
                    PrintInfo("请选择正确的白天亮温文件通道值。");
                    return null;
                }
                RasterMaper dayRm = new RasterMaper(dayPrd, new int[] { dayCh });
                rms.Add(dayRm);

                nightPrd = RasterDataDriver.Open(nightFile) as IRasterDataProvider;
                if (nightPrd.BandCount < nightCh)
                {
                    PrintInfo("请选择正确的夜间亮温文件通道值。");
                    return null;
                }
                RasterMaper nightRm = new RasterMaper(nightPrd, new int[] { nightCh });
                rms.Add(nightRm);

                string outFileName = GetFileName(new string[] { dayFile, nightFile }, _subProductDef.ProductDef.Identify, _identify, ".dat", null);
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                {
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<short, short> rfr = null;
                    rfr = new RasterProcessModel<short, short>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    Int16 value0 = 0, value1 = 0;
                    rfr.RegisterCalcModel(new RasterCalcHandler<short, short>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        if (rvInVistor[0].RasterBandsData == null || rvInVistor[1].RasterBandsData == null||
                            rvInVistor[0].RasterBandsData[0] == null || rvInVistor[1].RasterBandsData[0]==null)
                            return;
                        for (int index = 0; index < dataLength; index++)
                        {
                            value0 = rvInVistor[0].RasterBandsData[0][index];
                            value1 = rvInVistor[1].RasterBandsData[0][index];
                            if (value0 == 0 && value1 == 0)
                                rvOutVistor[0].RasterBandsData[0][index] = 0;
                            else if (value0 < dayMinValue || value1 < nightMinValue)
                                rvOutVistor[0].RasterBandsData[0][index] = cloudValue;
                            else
                                rvOutVistor[0].RasterBandsData[0][index] = (short)(rvInVistor[0].RasterBandsData[0][index] - rvInVistor[1].RasterBandsData[0][index]);
                        }
                    }));
                    //执行
                    rfr.Excute();
                    FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                    res.SetDispaly(false);
                    return res;
                }
            }
            finally
            {
                if (dayPrd != null)
                    dayPrd.Dispose();
                if (nightPrd != null)
                    nightPrd.Dispose();
            }
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
