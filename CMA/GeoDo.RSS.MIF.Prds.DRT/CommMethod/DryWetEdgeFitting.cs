using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public class DryWetEdgeFitting
    {
        public static bool DoDryWetEdgeFitting(out DryWetEdgeArgs args, TVDIUCArgs ucArgs, ref string error)
        {
            args = null;
            if (!CheckHistograms(ucArgs, ref error))
                return false;
            Dictionary<float, List<float>> histograms = NDVIHistograms(ucArgs);
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

        public static Dictionary<float, List<float>> NDVIHistograms(TVDIUCArgs ucArgs)
        {
            string error = string.Empty;
            if (!CheckHistograms(ucArgs, ref error))
                return null;
            Dictionary<string, FilePrdMap> filePrdMap = new Dictionary<string, FilePrdMap>();
            filePrdMap.Add("NDVIFile", new FilePrdMap(ucArgs.NDVIFile, ucArgs.TVDIParas.NdviFile.Zoom, new VaildPra(ucArgs.TVDIParas.NdviFile.Min, ucArgs.TVDIParas.NdviFile.Max), new int[] { ucArgs.TVDIParas.NdviFile.Band }));
            filePrdMap.Add("LSTFile", new FilePrdMap(ucArgs.ECLstFile, ucArgs.TVDIParas.LstFile.Zoom, new VaildPra(ucArgs.TVDIParas.LstFile.Min, ucArgs.TVDIParas.LstFile.Max), new int[] { 1 }));

            ITryCreateVirtualPrd tryVPrd = new TryCreateVirtualPrdByMultiFile();
            IVirtualRasterDataProvider vrd = null;
            try
            {
                vrd = tryVPrd.CreateVirtualRasterPRD(ref filePrdMap);
                if (vrd == null)
                    throw new Exception("数据间无相交部分,无法创建虚拟数据提供者!");
                Dictionary<float, List<float>> result = new Dictionary<float, List<float>>();
                ArgumentProvider ap = new ArgumentProvider(vrd, null);
                RasterPixelsVisitor<float> rpVisitor = new RasterPixelsVisitor<float>(ap);
                TVDIParaClass tvdiP = ucArgs.TVDIParas;
                IPixelFeatureMapper<float> _result = new MemPixelFeatureMapper<float>("0DWE", 1000, new Size(vrd.Width, vrd.Height), vrd.CoordEnvelope, vrd.SpatialRef);
                float ndvizoom = tvdiP.NdviFile.Zoom;
                float lstZoom = tvdiP.LstFile.Zoom;
                rpVisitor.VisitPixel(new int[] { filePrdMap["NDVIFile"].StartBand,
                                                 filePrdMap["LSTFile"].StartBand},
                 (index, values) =>
                 {
                     if (values[1] >= tvdiP.LstFile.Min / lstZoom && values[1] <= tvdiP.LstFile.Max / lstZoom &&
                         values[0] >= tvdiP.NdviFile.Min / ndvizoom && values[0] <= tvdiP.NdviFile.Max / ndvizoom)
                     {
                         if (!result.ContainsKey(values[0]))
                             result.Add(values[0], new List<float>());
                         result[values[0]].Add(values[1]);
                     }
                 });
                return result.Count == 0 ? null : result;
            }
            finally
            {
                vrd.Dispose();
            }
        }

        private static DryWetEdgeArgs DryWetEdgesArgsCalc(TVDIUCArgs ucArgs, Dictionary<float, List<float>> NDVIHistograms)
        {
            List<Samples> minData = new List<Samples>();
            List<Samples> maxData = new List<Samples>();
            TVDIParaClass tvdiP = ucArgs.TVDIParas;
            ArgumentItem ndviArgs = tvdiP.NdviFile;
            ArgumentItem lstArgs = tvdiP.LstFile;
            try
            {
                int rangeSize = (int)Math.Ceiling((double)(ndviArgs.Max - ndviArgs.Min) / tvdiP.Length);//区间个数
                if (rangeSize == 0)
                    return null;
                float key = 0;
                float minLst = 0;
                float maxLst = 0;
                for (int rangeIndex = 0; rangeIndex < rangeSize; rangeIndex++)//每个步长区间，0~step;step~step*rangeIndex;step
                {
                    key = (float)(ndviArgs.Min + tvdiP.Length * rangeIndex);// / ndviArgs.Zoom;

                    if (!NDVIHistograms.ContainsKey(key))
                        continue;
                    if (key == 0 || !ValidValueHelper.IsVaild(key, ndviArgs))
                        continue;
                    if (NDVIHistograms[key] == null || NDVIHistograms[key].Count == 0)
                        continue;
                    NDVIHistograms[key].Sort();
                    minLst = 0;
                    maxLst = 0;
                    NdviLstPCStat(NDVIHistograms[key], out minLst, out maxLst, ucArgs.TVDIParas.LstFile, tvdiP.LstFreq);
                    if (minLst == 0 && maxLst == 0)
                        continue;
                    minData.Add(new Samples(key * ndviArgs.Zoom, minLst * lstArgs.Zoom));
                    maxData.Add(new Samples(key * ndviArgs.Zoom, maxLst * lstArgs.Zoom));
                }
                GC.Collect();
                return NihePara(maxData, minData, ndviArgs, tvdiP.LstFile, tvdiP.FLimit, tvdiP.HGYZ);
            }
            finally
            {
            }
        }

        private static void NdviLstPCStat(List<float> ndviList, out float minLst, out float maxLst, ArgumentItem lstArgItem, int LstFreq)
        {
            minLst = maxLst = 0;
            int length = ndviList.Count;
            List<float> resultTemp = new List<float>();
            Dictionary<float, int> freqDic = new Dictionary<float, int>();
            bool isEnough = false;
            for (int i = 0; i < length; i++)
            {
                if (!ValidValueHelper.IsVaild(ndviList[i], lstArgItem))
                    continue;
                if (!freqDic.ContainsKey(ndviList[i]))
                    freqDic.Add(ndviList[i], 0);
                freqDic[ndviList[i]] += 1;
                if (!isEnough && freqDic[ndviList[i]] > LstFreq)
                {
                    minLst = ndviList[i];
                    isEnough = true;
                }
                if (freqDic[ndviList[i]] > LstFreq)
                    maxLst = ndviList[i];
            }
        }

        private static DryWetEdgeArgs NihePara(List<Samples> maxData, List<Samples> minData, ArgumentItem ndviArgItem, ArgumentItem lstArgItem, float fLimit, float hgYZ)
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
}
