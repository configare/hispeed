using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    internal static class GetOtherExtractResult
    {
        public static IExtractResult GetExtractResult(IArgumentProvider argProvider, Dictionary<int, PixelFeature> features, IPixelIndexMapper candidateFirPixels, IContextMessage contextMessage, Action<int, string> progressTracker)
        {
            if (features == null || features.Count == 0)
            {
                PrintInfo(contextMessage, "当前影像无火点信息!");
                return null;
            }
            IExtractResultArray array = new ExtractResultArray("火点判识");
            if (progressTracker != null)
                progressTracker.Invoke(75, "正在生成强度文件,请稍候...");
            //强度栅格文件
            IFileExtractResult intensityFile = (new IntensityRasterGenerator()).Generate(argProvider, features);
            if (intensityFile != null)
            {
                array.Add(intensityFile);
                intensityFile.SetDispaly(false);
                intensityFile.SetOutIdentify("0FPG");
            }
            //内存判识二值图
            //array.Add(candidateFirPixels);
            if (progressTracker != null)
                progressTracker.Invoke(80, "正在生成火区列表,请稍候...");
            //计算火区
            using (FireAreaInfoListGenerator fireArea = new FireAreaInfoListGenerator())
            {
                Dictionary<int, FireAreaFeature> fireAFeatures = fireArea.GetFireArea(argProvider, candidateFirPixels, features);
                IFileExtractResult fALTFile = fireArea.Generator(argProvider, fireAFeatures);
                //fireArea.ExportILSTToExcel(fALTFile.FileName);
                if (fALTFile != null)
                    array.Add(fALTFile);
                //环保部信息列表
                IFileExtractResult fALHFile = fireArea.GeneratorHB(argProvider, fireAFeatures);
                if (fALHFile != null)
                    array.Add(fALHFile);

                //火情信息快报
                IFileExtractResult fALKFile = fireArea.GeneratorKB(argProvider, fireAFeatures);
                if (fALKFile != null)
                    array.Add(fALKFile);

                if (progressTracker != null)
                    progressTracker.Invoke(95, "正在生成火点列表,请稍候...");
                //火点信息列表
                using (FirePixelInfoListGenerator p = new FirePixelInfoListGenerator())
                {
                    IFileExtractResult pLSTFile = p.Generator(argProvider, features);
                    if (pLSTFile != null)
                        array.Add(pLSTFile);
                    //return array;
                }
                //环保部火点信息列表
                using (FirePixelInfoListGenerator p = new FirePixelInfoListGenerator())
                {
                    IFileExtractResult pLSTFile = p.GeneratorHB(argProvider, features);
                    if (pLSTFile != null)
                        array.Add(pLSTFile);
                    return array;
                }
            }
        }

        private static void PrintInfo(IContextMessage contextMessage, string info)
        {
            if (contextMessage != null)
                contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
    }
}
