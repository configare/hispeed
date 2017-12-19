using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using GeoDo.RSS.Core.DF;

namespace GeoDo.Smart.MaxCsr
{
    class Program
    {
        static void Main(string[] args)
        {
            //string argFile = AppDomain.CurrentDomain.BaseDirectory + "\\ClearSkyArg.txt";
            string argFile = args[0];
            if (!File.Exists(argFile))
            {
                Console.WriteLine("参数配置文件不存在!");
                return;
            }
            Dictionary<string, string> argDic = GetArgsByFile(argFile);
            if (argDic == null || argDic.Count == 0)
            {
                Console.WriteLine("参数配置文件内容不正确!");
                return;
            }
            string path = argDic["BaseDir"];    //@"E:\NDVI"
            string[] fs = Directory.GetFiles(path, "*.ldf");
            if (fs == null || fs.Length == 0)
            {
                Console.WriteLine("输入目录下无数据文件!");
                return;
            }
            Executer s = new Executer();
            IRasterDataProvider prd = RasterDataDriver.Open(fs[0]) as IRasterDataProvider;
            int[] bands = GetBands(prd, argDic);
            int[] ndviBands = GetNDVIBands(prd, argDic);
            if (ndviBands == null)
            {
                Console.WriteLine("参数配置文件中NDVI计算通道错误!");
                return;
            }
            s.StatFiles(fs, bands, GetOutDir(prd, argDic), ndviBands, (p, pstring) => { Console.WriteLine(pstring); });
            Console.ReadLine();
        }

        private static int[] GetNDVIBands(IRasterDataProvider prd, Dictionary<string, string> argDic)
        {
            List<int> NDVIBands = new List<int>();
            bool argIsOK = true;
            int tempBand = 0;
            if (argDic.ContainsKey("NDVIBands"))
            {
                string[] split = argDic["NDVIBands"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (split != null && split.Length != 0)
                    for (int i = 0; i < split.Length; i++)
                    {
                        tempBand = int.Parse(split[i]);
                        if (tempBand >= prd.BandCount)
                        {
                            argIsOK = false;
                            break;
                        }
                        NDVIBands.Add(tempBand);
                    }
            }
            if (!argIsOK || NDVIBands.Count != 2)
                return null;
            return NDVIBands.ToArray();
        }

        private static string GetOutDir(IRasterDataProvider prd, Dictionary<string, string> argDic)
        {
            if (argDic.ContainsKey("OutDir"))
            {
                string dir = argDic["OutDir"];
                if (Directory.Exists(dir))
                    return dir;
            }
            return Path.GetFileName(prd.fileName);
        }

        private static Dictionary<string, string> GetArgsByFile(string argFile)
        {
            string[] context = File.ReadAllLines(argFile);
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
                result.Add("NDVIBands", context[2]);
            else
                return result;
            if (context.Length > 3)
                result.Add("Bands", context[3]);
            return result;
        }

        private static int[] GetBands(IRasterDataProvider prd, Dictionary<string, string> argDic)
        {
            List<int> bands = new List<int>();
            bool argIsOK = true;
            int tempBand = 0;
            if (argDic.ContainsKey("Bands"))
            {
                if (argDic["Bands"].ToUpper() != "ALL")
                {
                    string[] split = argDic["Bands"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (split != null && split.Length != 0)
                        for (int i = 0; i < split.Length; i++)
                        {
                            tempBand = int.Parse(split[i]);
                            if (tempBand >= prd.BandCount)
                            {
                                argIsOK = false;
                                break;
                            }
                            bands.Add(tempBand);
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
                    bands.Add(i + 1);
                }
            }
            return bands.ToArray();
        }
    }
}
