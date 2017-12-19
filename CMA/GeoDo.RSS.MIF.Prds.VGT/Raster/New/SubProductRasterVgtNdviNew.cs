using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using System.IO;
using System.Drawing;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    /// <summary>
    /// 使用新的栅格计算框架计算单时次数据举例
    /// </summary>
    public class SubProductRasterVgtNdvi : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private string _orbitFilesDirPath = AppDomain.CurrentDomain.BaseDirectory + "\\Workspace\\VGT\\待计算数据";

        public SubProductRasterVgtNdvi()
            : base()
        {
        }

        public SubProductRasterVgtNdvi(SubProductDef subProductDef)
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
            _contextMessage = contextMessage;
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            string algorithmName = _argumentProvider.GetArg("AlgorithmName").ToString();
            if (algorithmName != "NDVI")
            {
                PrintInfo("指定的算法\"" + algorithmName + "\"没有实现。");
                return null;
            }
            return CalcNDVI(progressTracker);
        }

        private IExtractResult CalcNDVI(Action<int, string> progressTracker)
        {
            //参数准备
            int bandV = (int)_argumentProvider.GetArg("Visible");        //可见光
            int bandNear = (int)_argumentProvider.GetArg("NearInfrared");//近红外
            if (bandV == -1 || bandNear == -1)
            {
                PrintInfo("通道序号设置不正确");
                return null;
            }
            int[] bandNos = new int[] { bandV, bandNear };
            double visibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double nearInfrared = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
            float zoom = (ushort)_argumentProvider.GetArg("resultZoom");
            zoom = zoom == 0f ? 1000 : zoom;
            string[] aois = _argumentProvider.GetArg("AOITemplate") as string[];
            string aoiTemplate = aois == null ? null : aois[0];
            string[] inputFiles = _argumentProvider.GetArg("RasterFile") as string[];
            if (inputFiles == null || inputFiles.Length == 0)
            {
                PrintInfo("没有设置输入数据");
                return null;
            }
            IExtractResultArray results = new ExtractResultArray("NDVI");
            foreach (string inputFile in inputFiles)
            {
                //计算NDVI
                IExtractResult ret = CalcNDVI(inputFile, bandNos, zoom, aoiTemplate, progressTracker);
                if (ret != null)
                    results.Add(ret as IExtractResultBase);
            }
            return results;
        }

        /// <summary>
        /// 单文件计算其NDVI
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="bandNos"></param>
        /// <param name="zoom"></param>
        /// <param name="aoiTemplate"></param>
        /// <param name="progressTracker"></param>
        /// <returns></returns>
        private IExtractResult CalcNDVI(string inputFile, int[] bandNos, float zoom, string aoiTemplate, Action<int, string> progressTracker)
        {
            RasterProcessModel<ushort, short> rfr = null;
            RasterMaper[] fileIns = null;
            RasterMaper[] fileOuts = null;
            try
            {
                //输入数据(LDF)
                IRasterDataProvider inRaster = RasterDataDriver.Open(inputFile) as IRasterDataProvider;
                if (inRaster == null)
                {
                    PrintInfo("读取栅格文件失败：" + inRaster);
                    return null;
                }
                //输出数据(NDVI)
                string outFileName = GetFileName(new string[] { inRaster.fileName }, _subProductDef.ProductDef.Identify, _identify, ".ldf", null);
                IRasterDataDriver dd = RasterDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
                string mapInfo = inRaster.CoordEnvelope.ToMapInfoString(new Size(inRaster.Width, inRaster.Height));
                RasterDataProvider outRaster = dd.Create(outFileName, inRaster.Width, inRaster.Height, 1, enumDataType.Int16, mapInfo) as RasterDataProvider;
                //栅格数据映射
                RasterMaper fileIn = new RasterMaper(inRaster, bandNos);
                RasterMaper fileOut = new RasterMaper(outRaster, new int[] { 1 });
                fileIns = new RasterMaper[] { fileIn };
                fileOuts = new RasterMaper[] { fileOut };
                //创建处理模型
                rfr = new RasterProcessModel<ushort, short>(progressTracker);
                rfr.SetRaster(fileIns, fileOuts);
                rfr.SetTemplateAOI(aoiTemplate);
                rfr.RegisterCalcModel(new RasterCalcHandler<ushort, short>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData != null)
                    {
                        if (rvInVistor == null)
                            return;
                        ushort[] inBand0 = rvInVistor[0].RasterBandsData[0];//第1个输入文件的第1个波段的各像素值
                        ushort[] inBand1 = rvInVistor[0].RasterBandsData[1];//第1个输入文件的第2个波段的各像素值
                        short[] ndvi = new short[inBand0.Length];
                        if (string.IsNullOrWhiteSpace(aoiTemplate))
                        {
                            for (int index = 0; index < inBand0.Length; index++)
                            {
                                //第1个输出文件的第1个波段存储NDVI值
                                rvOutVistor[0].RasterBandsData[0][index] = (short)((inBand1[index] - inBand0[index]) * zoom / (inBand1[index] + inBand0[index]));
                            }
                        }
                        else if (aoi != null && aoi.Length != 0)
                        {
                            int index;
                            for (int i = 0; i < aoi.Length; i++)
                            {
                                index = aoi[i];
                                //第1个输出文件的第1个波段存储NDVI值
                                rvOutVistor[0].RasterBandsData[0][index] = (short)((inBand1[index] - inBand0[index]) * zoom / (inBand1[index] + inBand0[index]));
                            }
                        }
                    }
                }));
                //执行
                rfr.Excute();
                FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                res.SetDispaly(false);
                return res;
            }
            finally
            {
                if (fileIns != null)
                {
                    for (int i = 0; i < fileIns.Length; i++)
                    {
                        fileIns[i].Raster.Dispose();
                    }
                }
                if (fileOuts != null)
                {
                    for (int i = 0; i < fileOuts.Length; i++)
                    {
                        fileOuts[i].Raster.Dispose();
                    }
                }
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
