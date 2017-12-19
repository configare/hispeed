using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public class ElevationCorrectionsII
    {
        public static bool DoElevationCorrections(TVDIUCArgs ucArgs, ref string error)
        {
            if (string.IsNullOrEmpty(ucArgs.LSTFile) || string.IsNullOrEmpty(ucArgs.DEMFile) || ucArgs.TVDIParas == null || ucArgs.TVDIParas.LstFile == null)
            {
                error = "陆表高温高程订正所需数据或参数设置不全.";
                return false;
            }

            string lstFile = ucArgs.LSTFile;
            string demFile = ucArgs.DEMFile;
            float lstZoom = ucArgs.TVDIParas.LstFile.Zoom;
            int lstMin = ucArgs.TVDIParas.LstFile.Min;
            int lstMax = ucArgs.TVDIParas.LstFile.Max;
            int lstBand = ucArgs.TVDIParas.LstFile.Band;

            IRasterDataProvider lstPrd = null;
            IRasterDataProvider demPrd = null;
            try
            {
                List<RasterMaper> rms = new List<RasterMaper>();
                lstPrd = RasterDataDriver.Open(lstFile) as IRasterDataProvider;
                if (lstPrd.BandCount < lstBand)
                {
                    error = "选择的文件不正确，请重新选择。";
                    return false;
                }
                RasterMaper lstRm = new RasterMaper(lstPrd, new int[] { lstBand });
                rms.Add(lstRm);

                demPrd = RasterDataDriver.Open(demFile) as IRasterDataProvider;
                if (demPrd.BandCount < 1)
                {
                    error = "选择的文件不正确，请重新选择。";
                    return false;
                }
                RasterMaper demRm = new RasterMaper(demPrd, new int[] { 1 });
                rms.Add(demRm);

                //输出文件准备（作为输入栅格并集处理）          
                string outFileName = CreatOutFileName(lstFile, demFile);
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<Int16, Int16> rfr = new RasterProcessModel<Int16, Int16>();
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        int value0 = 0;
                        int value1 = 0;
                        if (rvInVistor[0].RasterBandsData == null || rvInVistor[1].RasterBandsData == null ||
                            rvInVistor[0].RasterBandsData[0] == null || rvInVistor[1].RasterBandsData[0] == null)
                            return;
                            for (int index = 0; index < dataLength; index++)
                        {
                            value0 = rvInVistor[0].RasterBandsData[0][index];
                            value1 = rvInVistor[1].RasterBandsData[0][index];
                            if (value1 == -9999)
                            {
                                rvOutVistor[0].RasterBandsData[0][index] = 9999;//海洋
                                continue;
                            }
                            if (value1 == -9000)
                            {
                                rvOutVistor[0].RasterBandsData[0][index] = 9000;//非中国区域陆地
                                continue;
                            }
                            if (value1 >= 6000)
                            {
                                rvOutVistor[0].RasterBandsData[0][index] = 0; //6000之内的LST数据
                                continue;
                            }
                            if (value0 == ucArgs.TVDIParas.LstFile.Cloudy)
                            {
                                rvOutVistor[0].RasterBandsData[0][index] = 9998; //云区
                                continue;
                            }
                            if (value0 == 12)
                            {
                                rvOutVistor[0].RasterBandsData[0][index] = 9997;//无数据区域
                                continue;
                            }
                            if (value0 == 0)
                            {
                                rvOutVistor[0].RasterBandsData[0][index] = 0;
                                continue;
                            }
                            if (value1 == 0)
                            {
                                rvOutVistor[0].RasterBandsData[0][index] = 0;
                                continue;
                            }
                            rvOutVistor[0].RasterBandsData[0][index] = (Int16)(((double)value0 / lstZoom - 273 + 0.006 * value1) * lstZoom);
                        }
                    }));
                    //执行
                    rfr.Excute();
                    //FileExtractResult res = new FileExtractResult("0LEC", outFileName, true);
                    //res.SetDispaly(false);
                }
                ucArgs.ECLstFile = outFileName;
                return true;
            }
            finally
            {
                if (lstPrd != null)
                    lstPrd.Dispose();
                if (demPrd != null)
                    demPrd.Dispose();
            }
        }

        private static string CreatOutFileName(string lstFile, string demFile)
        {
            RasterIdentify ri = new RasterIdentify(lstFile);
            return MifEnvironment.GetFullFileName(ri.ToWksFileName(".dat"));
        }

        private static RasterIdentify CreateRID(string lstFile)
        {
            RasterIdentify rid = new RasterIdentify(lstFile);
            rid.ThemeIdentify = "CMA";
            rid.SubProductIdentify = "0LEC";
            rid.ProductIdentify = "DRT";
            return rid;
        }

        private static IRasterDataProvider CreateOutRaster(string outFileName, RasterMaper[] rasterMaper)
        {
            using (IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver)
            {
                CoordEnvelope outEnv = null;
                foreach (RasterMaper inRaster in rasterMaper)
                {
                    if (outEnv == null)
                        outEnv = inRaster.Raster.CoordEnvelope;
                    else
                        outEnv.Union(inRaster.Raster.CoordEnvelope);
                }
                float resX = rasterMaper[0].Raster.ResolutionX;
                float resY = rasterMaper[0].Raster.ResolutionY;
                int width = (int)(Math.Round(outEnv.Width / resX));
                int height = (int)(Math.Round(outEnv.Height / resY));
                string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
                RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, rasterMaper[0].Raster.DataType, mapInfo) as RasterDataProvider;
                return outRaster;
            }
        }
    }
}
