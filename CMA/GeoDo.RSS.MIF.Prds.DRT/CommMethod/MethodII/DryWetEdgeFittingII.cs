using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using System.IO;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public class DryWetEdgeFittingII
    {
        public static bool DoDryWetEdgeFitting(out DryWetEdgeArgs args, TVDIUCArgs ucArgs, ref string error)
        {
            args = null;
            if (!CheckHistograms(ucArgs, ref error))
                return false;
            NdviList[] histograms = NDVIHistograms(ucArgs);
            if (histograms == null)
                return false;
            args = DryWetEdgesArgsCalc(ucArgs, histograms);
            if (args == null)
            {
                error = "干湿边拟合失败,请调整NDVI值域范围后重试!";
                return false;
            }
            return true;
        }

        private static bool CheckHistograms(TVDIUCArgs ucArgs, ref string error)
        {
            error = "";
            if (string.IsNullOrEmpty(ucArgs.ECLstFile) || string.IsNullOrEmpty(ucArgs.NDVIFile) || ucArgs.TVDIParas == null || ucArgs.TVDIParas.LstFile == null || ucArgs.TVDIParas.NdviFile == null)
            {
                error = "直方图统计所需数据或参数设置不全.";
                return false;
            }
            return true;
        }

        public static NdviList[] NDVIHistograms(TVDIUCArgs ucArgs)
        {
            string error = string.Empty;
            if (!CheckHistograms(ucArgs, ref error))
                return null;
            string ndviFile = ucArgs.NDVIFile;
            int ndviZoom = ucArgs.TVDIParas.NdviFile.Zoom;
            int ndviMin = ucArgs.TVDIParas.NdviFile.Min;
            int ndviMax = ucArgs.TVDIParas.NdviFile.Max;
            int ndviBand = ucArgs.TVDIParas.NdviFile.Band;

            string lstFile = ucArgs.ECLstFile;
            int lstZoom = ucArgs.TVDIParas.LstFile.Zoom;
            int lstMin = ucArgs.TVDIParas.LstFile.Min;
            int lstMax = ucArgs.TVDIParas.LstFile.Max;
            IRasterDataProvider ndviPrd = null;
            IRasterDataProvider lstPrd = null;
            try
            {
                List<RasterMaper> rms = new List<RasterMaper>();
                ndviPrd = RasterDataDriver.Open(ndviFile) as IRasterDataProvider;
                if (ndviPrd.BandCount < ndviBand)
                    return null;
                RasterMaper ndviRm = new RasterMaper(ndviPrd, new int[] { ndviBand });
                rms.Add(ndviRm);

                lstPrd = RasterDataDriver.Open(lstFile) as IRasterDataProvider;
                if (lstPrd.BandCount < 1)
                    return null;
                RasterMaper lstRm = new RasterMaper(lstPrd, new int[] { 1 });
                rms.Add(lstRm);

                string outFileName = CreatOutFile();
                Int16 value0 = 0;
                Int16 value1 = 0;
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                {
                    int start = -1000, end = 1000;
                    int statLength = end - start + 1;
                    NdviList[] results = new NdviList[statLength];
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    RasterProcessModel<Int16, Int16> rfr = new RasterProcessModel<Int16, Int16>();
                    rfr.SetRaster(fileIns, fileOuts);
                    TVDIParaClass tvdiP = ucArgs.TVDIParas;
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                        {
                            int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                            if (rvInVistor[0].RasterBandsData == null || rvInVistor[1].RasterBandsData == null ||
                                rvInVistor[0].RasterBandsData[0] == null || rvInVistor[1].RasterBandsData[0] == null)
                                return;
                            for (int index = 0; index < dataLength; index++)
                            {
                                value0 = rvInVistor[0].RasterBandsData[0][index];
                                value1 = rvInVistor[1].RasterBandsData[0][index];
                                if (value0 < -1000 || value0 > 1000)
                                    continue;
                                if (results[value0 - (-1000)] == null)
                                    results[value0 - (-1000)] = new NdviList(value0);
                                results[value0 - (-1000)].Lst.Add(value1);
                            }
                        }));
                    rfr.Excute();
                    return results;
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

        private static string CreatOutFile()
        {
            //string dir = AppDomain.CurrentDomain.BaseDirectory + "\\TEMP";
            //if (!Directory.Exists(dir))
            //    Directory.CreateDirectory(dir);
            //return dir + "\\" + Guid.NewGuid().ToString() + ".dat";
            return MifEnvironment.GetFullFileName(Guid.NewGuid().ToString() + ".dat");
        }

        private static DryWetEdgeArgs DryWetEdgesArgsCalc(TVDIUCArgs ucArgs, NdviList[] NDVIHistograms)
        {
            List<Samples> minData = new List<Samples>();
            List<Samples> maxData = new List<Samples>();
            TVDIParaClass tvdiP = ucArgs.TVDIParas;
            ArgumentItem ndviArgs = tvdiP.NdviFile;
            int ndviCloudy = ndviArgs.Cloudy;
            ArgumentItem lstArgs = tvdiP.LstFile;
            try
            {
                int rangeSize = (int)Math.Ceiling((double)(ndviArgs.Max - ndviArgs.Min) / tvdiP.Length);//区间个数
                if (rangeSize == 0)
                    return null;
                int key = 0;
                float minLst = 0;
                float maxLst = 0;
                for (int rangeIndex = 0; rangeIndex < rangeSize; rangeIndex++)//每个步长区间，0~step;step~step*rangeIndex;step
                {
                    key = (int)(ndviArgs.Min + tvdiP.Length * rangeIndex - (-1000));
                    if (key >= NDVIHistograms.Length || NDVIHistograms[key] == null)
                        continue;
                    if (NDVIHistograms[key].Ndvi == ndviCloudy || NDVIHistograms[key].Ndvi == 0)
                        continue;
                    if (!ValidValueHelper.IsVaild(key, ndviArgs))
                        continue;
                    if (NDVIHistograms[key].Lst == null || NDVIHistograms[key].Lst.Count == 0)
                        continue;
                    NDVIHistograms[key].Lst.Sort();
                    NdviLstPCStat(NDVIHistograms[key].Lst, out minLst, out maxLst, ucArgs.TVDIParas.LstFile, tvdiP.LstFreq);
                    if (minLst == 0 && maxLst == 0)
                        continue;
                    minData.Add(new Samples(NDVIHistograms[key].Ndvi, minLst));
                    maxData.Add(new Samples(NDVIHistograms[key].Ndvi, maxLst));
                }
                GC.Collect();
                return NihePara(maxData, minData, tvdiP.FLimit, tvdiP.HGYZ);
            }
            finally
            {
            }
        }

        private static void NdviLstPCStat(List<Int16> ndviList, out float minLst, out float maxLst, ArgumentItem lstArgItem, int LstFreq)
        {
            minLst = maxLst = 0;
            int length = ndviList.Count;
            List<float> resultTemp = new List<float>();
            bool isEnough = false;
            int start = lstArgItem.Min, end = lstArgItem.Max;
            Int16[] result = new short[end - start + 1];
            int index = 0;
            for (int i = 0; i < length; i++)
            {
                if (ndviList[i] == 9000 || ndviList[i] == 9998 ||
                   ndviList[i] == 9997 || ndviList[i] == 9999)
                    continue;
                if (!ValidValueHelper.IsVaild(ndviList[i], lstArgItem))
                    continue;
                index = ndviList[i] - start;
                if (index < 0 || index >= result.Length)
                    continue;
                result[index] += 1;
                if (!isEnough && result[index] > LstFreq)
                {
                    minLst = ndviList[i];
                    isEnough = true;
                }
                if (result[index] > LstFreq)
                    maxLst = ndviList[i];
            }
        }

        private static DryWetEdgeArgs NihePara(List<Samples> maxData, List<Samples> minData, float fLimit, float hgYZ)
        {
            double aMin = 0, aMax = 0, bMin = 0, bMax = 0;
            double fEigenValue = 0;
            MathHelper.MethodOfLeastSquares(minData, out aMin, out bMin, out fEigenValue);
            if (!CalcAB(ref minData, ref aMin, ref bMin, ref fEigenValue, fLimit, hgYZ))
                return null;
            if (aMin == 0 && bMin == 0)
                return null;
            MathHelper.MethodOfLeastSquares(maxData, out aMax, out bMax, out fEigenValue);
            if (!CalcAB(ref maxData, ref aMax, ref bMax, ref fEigenValue, fLimit, hgYZ))
                return null;
            if (aMax == 0 && bMax == 0)
                return null;
            return new DryWetEdgeArgs(aMin, aMax, bMin, bMax);
        }

        private static bool CalcAB(ref List<Samples> data, ref double a, ref double b, ref double fEigenValue, float fLimit, float hgYZ)
        {
            double a1 = a;
            double b1 = b;
            double fEigenValue1 = fEigenValue;
            double a2 = 0;
            double b2 = 0;
            double fEigenValue2 = 0;
            int bWorking = 1;
            double fTmp = 0;
            while (bWorking == 1)
            {
                double fTmpCoe = 1.0 / (fEigenValue1 * fEigenValue1);//相关系数的平方倒数
                double fNumDif = ((double)data.Count - fTmpCoe) / data.Count;
                if (fNumDif > fLimit)
                {
                    bWorking = 1;
                    fTmp = fTmpCoe;
                }
                else
                {
                    fNumDif = ((double)data.Count - fTmpCoe / 2.0) / data.Count;
                    if (fNumDif > fLimit)
                    {
                        bWorking = 1;
                        fTmp = fTmpCoe / 2.0;
                    }
                    else
                        return true;
                }
                if (bWorking == 1)
                {
                    if (!DelSample(ref data, a1, b1, fTmp))
                        return false;
                    MathHelper.MethodOfLeastSquares(data, out a2, out b2, out fEigenValue2);
                    double fCoeDif = Math.Abs(b2 - b1) / b1;//
                    if (fCoeDif < hgYZ)//回归方程阈值
                    {
                        a = a2;
                        b = b2;
                        fEigenValue = fEigenValue2;
                        return true;
                    }
                    else
                    {
                        a1 = a2;
                        b1 = b2;
                        fEigenValue1 = fEigenValue2;
                        bWorking = 1;
                    }
                }
            }
            return false;
        }

        private static bool DelSample(ref List<Samples> data, double a, double b, double fTmp)
        {
            for (int i = 0; i < data.Count; i++)
                data[i].FDif = Convert.ToSingle(Math.Abs(data[i].Lst - (b * data[i].Ndvi + a)));
            data.Sort(ListSort);
            int length = (int)Math.Round(fTmp * 10, 0);
            if (data.Count < length)
                return false;
            for (int i = 0; i < length; i++)
                data.RemoveAt(data.Count - 1);
            return true;
        }

        private static int ListSort(Samples samplesLast, Samples samplesCurrent)
        {
            if (samplesLast.FDif > samplesCurrent.FDif)
                return -1;
            else if (samplesLast.FDif < samplesCurrent.FDif)
                return 1;
            else
                return 0;
        }

        private static RasterIdentify CreateRID(string ndviFile)
        {
            RasterIdentify rid = new RasterIdentify(ndviFile);
            rid.ThemeIdentify = "CMA";
            rid.SubProductIdentify = "0DWE";
            rid.ProductIdentify = "DRT";
            return rid;
        }
    }

    public class NdviList
    {
        public Int16 Ndvi = 0;
        public List<Int16> Lst = new List<Int16>();

        public NdviList()
        { }

        public NdviList(Int16 ndvi)
        {
            Ndvi = ndvi;
        }

        public NdviList(Int16 ndvi, List<Int16> lst) :
            this(ndvi)
        {
            Lst = lst;
        }
    }
}
