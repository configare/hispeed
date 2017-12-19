using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.DF;

namespace GeoDo.SMART.CSDatasets
{
    class Program
    {
        /// <summary>
        /// 角度文件固定后缀
        /// </summary>
        private static string[] AngleNames = { "SolarZenith", "SolarAzimuth", "SensorZenith", "SensorAzimuth" };
        static void Main(string[] args)
        {
            string argFile = args[0];
            //string argFile = @"D:\code\smartII\气象MAS二期\SMART\bin\Release\ClearSkyArg.txt";
            if (!File.Exists(argFile))
            {
                Console.WriteLine("参数配置文件不存在!");
                return;
            }
            Dictionary<string, string> argDic = GetArgsByFile(argFile);
            bool isangle = bool.Parse(argDic["AngleFile"]);
            if (argDic == null || argDic.Count == 0)
            {
                Console.WriteLine("参数配置文件内容不正确!");
                return;
            }
            string path = argDic["BaseDir"];
            string MatchFile =argDic["ExtendPar"];
            string[] fs = GetDirMainFiles(path, MatchFile);
            if (fs == null || fs.Length == 0)
            {
                Console.WriteLine("输入目录下无[" + MatchFile + "]数据文件!");
                return;
            }
            IRasterDataProvider prd = null;
            int[] orderCalcBands = null;
            Dictionary<int, int> orderCalcBandsDic = null;
            //int 输出文件波段索引 int 原始数据波段索引
            Dictionary<int, int> outbands = null;
            try
            {
                prd = RasterDataDriver.Open(fs[0]) as IRasterDataProvider;
                outbands = GetOutBands(prd, argDic);
                orderCalcBandsDic = GetCalcBands(prd, argDic);
                if (orderCalcBandsDic == null)
                {
                    Console.WriteLine("参数配置文件中表达式对应波段设置错误!");
                    return;
                }
                orderCalcBands = GetOrderCalcBands(orderCalcBandsDic);
            }
            finally
            {
                if (prd != null)
                    prd.Dispose();
            }
            Executer<UInt16> exe = new Executer<UInt16>();
            if (isangle && IsContainAngleFile(path, MatchFile))//界面勾选角度信息 并且文件夹中包含角度文件
            {
                exe.AngleProcessCSD(GetOutDir(fs[0], argDic), fs, orderCalcBandsDic.Keys.ToArray(), orderCalcBands, outbands, argDic["CalcModel"]);
            }
            else
            {
                exe.ProcessCSD(GetOutDir(fs[0], argDic), fs, orderCalcBandsDic.Keys.ToArray(), orderCalcBands, outbands, argDic["CalcModel"]);
            }


            Console.ReadLine();
        }

        /// <summary>
        /// 按对应波段先后顺序填写计算波段
        /// </summary>
        /// <param name="calcBands"></param>
        /// <returns></returns>
        private static int[] GetOrderCalcBands(Dictionary<int, int> calcBands)
        {
            List<int> orderCalcBands = new List<int>();
            foreach (int key in calcBands.Keys)
            {
                if (!orderCalcBands.Contains(calcBands[key]))
                    orderCalcBands.Add(calcBands[key]);
            }
            return orderCalcBands.ToArray();
        }

        private static Dictionary<string, string> GetArgsByFile(string argFile)
        {
            string[] context = File.ReadAllLines(argFile, Encoding.Default);
            if (context == null || context.Length == 0)
                return null;
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (Directory.Exists(context[0]))
                result.Add("BaseDir", context[0]);
            else
                return null;
            if (context.Length > 1 && Directory.Exists(context[1]))
                result.Add("OutDir", context[1]);
            else
                return null;
            if (context.Length > 2)
                result.Add("CalcModel", context[2]);
            else
                return result;
            if (context.Length > 3)
                result.Add("CalcBands", context[3]);
            if (context.Length > 4)
                result.Add("OutBands", context[4]);
            if (context.Length > 6)
                result.Add("AngleFile", context[6]);
            if (context.Length > 7)
                result.Add("ExtendPar", context[7]);
            return result;
        }

        private static Dictionary<int, int> GetCalcBands(IRasterDataProvider prd, Dictionary<string, string> argDic)
        {
            Dictionary<int, int> CalcBands = new Dictionary<int, int>();
            bool argIsOK = true;
            int tempBand = 0;
            if (argDic.ContainsKey("CalcBands"))
            {
                string[] split = argDic["CalcBands"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] temp = null;
                if (split != null && split.Length != 0)
                    for (int i = 0; i < split.Length; i++)
                    {
                        if (string.IsNullOrEmpty(split[i]) || !split[i].Contains("="))
                            continue;
                        temp = split[i].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                        tempBand = int.Parse(temp[1]);
                        if (tempBand > prd.BandCount)
                        {
                            argIsOK = false;
                            break;
                        }
                        if (!CalcBands.ContainsKey(int.Parse(temp[0].Replace("band", ""))))
                            CalcBands.Add(int.Parse(temp[0].Replace("band", "")), tempBand);
                    }
            }
            if (!argIsOK || CalcBands.Count == 0)
                return null;
            CalcBands.OrderBy(curr => curr.Key);
            return CalcBands;
        }

        private static Dictionary<int, int> GetOutBands(IRasterDataProvider prd, Dictionary<string, string> argDic)
        {
            Dictionary<int, int> bands = new Dictionary<int, int>();
            bool argIsOK = true;
            int tempBand = 0;
            int index = 0;
            if (argDic.ContainsKey("OutBands"))
            {
                if (argDic["OutBands"].ToUpper() != "ALL")
                {
                    string[] split = argDic["Bands"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (split != null && split.Length != 0)
                        for (int i = 0; i < split.Length; i++)
                        {
                            tempBand = int.Parse(split[i]);
                            if (tempBand > prd.BandCount)
                            {
                                argIsOK = false;
                                break;
                            }
                            bands.Add(index, tempBand);
                            index++;
                        }
                }
                else
                    argIsOK = false;
            }
            if (!argIsOK)
            {
                bands.Clear();
                for (int i = 0; i < prd.BandCount; i++)
                {
                    bands.Add(i + 1, i + 1);
                }
            }
            return bands.Count == 0 ? null : bands;
        }

        private static string GetOutDir(string srcFilename, Dictionary<string, string> argDic)
        {
            if (argDic.ContainsKey("OutDir"))
            {
                string dir = argDic["OutDir"];
                if (Directory.Exists(dir))
                    return dir;
            }
            return Path.GetFileName(srcFilename);
        }
        private static string[] GetDirMainFiles(string path, string matchexp)
        {
            
            string []extends = matchexp.Split('|');
            List<string> mainfiles = new List<string>();
            foreach(string itemexp in extends)
            {
                mainfiles.AddRange(Directory.GetFiles(path, itemexp));
            }
            return mainfiles.Where(o => !IsAngleFile(o)).ToArray();//查找不是角度文件的ldf文件
        }
        /// <summary>
        /// 判断文件是否是角度文件
        private static bool IsAngleFile(string inputfile)
        {
            foreach (string item in AngleNames)
            {
                if (inputfile.IndexOf(item) != -1)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool IsContainAngleFile(string path, string matchexp)
        {
            return GetDirMainFiles(path, matchexp).Length == Directory.GetFiles(path, matchexp).Length ? false : true;
        }
    }
}
