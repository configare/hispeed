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
    public class SubProductLBBackFileDRT : CmaDrtMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductLBBackFileDRT()
            : base()
        {
        }
        public SubProductLBBackFileDRT(SubProductDef subProductDef)
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
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "LBBackAlgorithm")
            {
                return BackBGFileAlg(progressTracker);
                
            }
            return null;
        }
        //生成亮温背景库算法
        //云水 怎么处理
        private IExtractResult BackBGFileAlg(Action<int, string> progressTracker)
        {
            //亮温数据放大倍数
            Int16 TBZoom =  Convert.ToInt16(_argumentProvider.GetArg("LBZoom"));
            int LBBandCH = (int)_argumentProvider.GetArg("LBBand");
            //亮温数据生成的背景库放大倍数
            Int16 TBBackZoom = Convert.ToInt16(_argumentProvider.GetArg("OutLBBackZoom"));
            //亮温输入文件
            string[] tbfiles = GetStringArray("LBFile");
            
            //NDVI输出文件--根据输入文件信息输出
            string outtbbackfile = GetFileName(tbfiles, _subProductDef.ProductDef.Identify, _identify, ".ldf", null);//默认位置输出
            
            List<RasterMaper> rasterInputMaps = new List<RasterMaper>();//从配置文件中读取需要待合成ldf数据，此处只有一个ldf文件
            foreach (string itemfile in tbfiles)
            {
                IRasterDataProvider inraster = RasterDataDriver.Open(itemfile) as IRasterDataProvider;
                rasterInputMaps.Add(new RasterMaper(inraster, new int[] { LBBandCH }));
            }
            IRasterDataProvider outLBbackRaster = null;

            try
            {
                //栅格数据映射
                RasterMaper[] fileIns = rasterInputMaps.ToArray();
                RasterMaper[] fileOuts;
                outLBbackRaster = CreateOutLDFRaster(outtbbackfile, rasterInputMaps.ToArray(),5);
                fileOuts = new RasterMaper[] { new RasterMaper(outLBbackRaster, new int[] { 1, 2, 3, 4, 5 }) };//输出为固定5个波段数据
                //创建处理模型
                RasterProcessModel<Int16, Int16> rfr = new RasterProcessModel<Int16, Int16>(progressTracker);
                rfr.SetRaster(fileIns, fileOuts);
                rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                {
                    int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;//输出长*宽
                   
                    //初始化各波段原始数据
                    Int16 maxNDVI = Int16.MinValue;
                    Int16 minNDVI = Int16.MinValue;
                    Int16 secmaxNDVI = Int16.MinValue;
                    Int16 secminNDVI = Int16.MinValue;
                    double averageNDVI = Int16.MinValue;
                    for (int index = 0; index < dataLength; index++)
                    {
                        List<Int16> listtemp = new List<Int16>();
                        List<Int16> listavg = new List<short>();
                        for (int fileindex = 0; fileindex < rvInVistor.Length; fileindex++)
                        {
                            listtemp.Add(rvInVistor[fileindex].RasterBandsData[0][index]);
                            listavg.Add(rvInVistor[fileindex].RasterBandsData[0][index]);
                        }
                        //调整排序
                        listtemp.Sort();//按照从小到大升序排列
                        listtemp = listtemp.Distinct().ToList();//去重
                        if (listtemp.Count == 1 && listtemp[0] == 0)
                        {
                             maxNDVI = 0;
                             minNDVI = 0;
                             secmaxNDVI = 0;
                             secminNDVI = 0;
                             averageNDVI = 0;
                        }
                        else
                        {
                            ////这里是否需要考虑有效范围
                            maxNDVI = listtemp[listtemp.Count - 1];//最大值 此处文件个数大于1
                            minNDVI = listtemp[0];//最小值 此处文件数大于1
                            secmaxNDVI = listtemp.Count > 1 ? listtemp[listtemp.Count - 2] : listtemp[listtemp.Count - 1];//次大值 
                            secminNDVI = listtemp.Count > 1 ? listtemp[1] : listtemp[0];//次小值
                            //平均值 
                            //去除 0
                            averageNDVI = listavg.Where(num => num != 0).Average(num => (int)(num));//平均值
                        }
                            //给输出Raster填值，此处为固定五个波段 并考虑放大倍数的处理
                            rvOutVistor[0].RasterBandsData[0][index] = (Int16)(maxNDVI * TBBackZoom / TBZoom);
                            rvOutVistor[0].RasterBandsData[1][index] = (Int16)(minNDVI * TBBackZoom / TBZoom);
                            rvOutVistor[0].RasterBandsData[2][index] = (Int16)(secmaxNDVI * TBBackZoom / TBZoom);
                            rvOutVistor[0].RasterBandsData[3][index] = (Int16)(secminNDVI * TBBackZoom / TBZoom);
                            rvOutVistor[0].RasterBandsData[4][index] = (Int16)(averageNDVI * TBBackZoom / TBZoom);
                       
                    }
                }));
                rfr.Excute();
                FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outtbbackfile, true);
                res.SetDispaly(false);
                return res;
            }
            catch (Exception ex)
            {
                PrintInfo("算法处理过程发生异常！\n"+ex.StackTrace);
                return null;
            }

            finally
            {
                if (outLBbackRaster != null)
                    outLBbackRaster.Dispose();
                if (outLBbackRaster != null)
                    outLBbackRaster.Dispose();

            }
        }
        /// <summary>
        /// 创建LDFRaster
        /// </summary>
        /// <param name="outFileName">LDF文件名称</param>
        /// <param name="inrasterMaper">输入文件参考</param>
        /// <param name="outbandcount">输出文件波段</param>
        /// <returns></returns>
        protected IRasterDataProvider CreateOutLDFRaster(string outFileName, RasterMaper[] inrasterMaper, int outbandcount)
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
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, outbandcount, enumDataType.Int16, optionString) as RasterDataProvider;
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