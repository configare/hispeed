using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.ReportService
{
    public class GetQuickReportModel
    {
        public static ReportTemplateInfo[] GetReportTemplateInfo(string proIdentify, string outFilename)
        {
            List<ReportTemplateInfo> reportTmplate = new List<ReportTemplateInfo>();
            if (GetInfoFromDB(proIdentify, outFilename))
            {
                string[] contexts = File.ReadAllLines(outFilename, Encoding.Default);
                if (contexts == null || contexts.Length == 0)
                    return null;
                string[] split = null;
                foreach (string context in contexts)
                {
                    split = context.Split(new char[] { '\t' });
                    if (split == null || split.Length != 3)
                        continue;
                    reportTmplate.Add(new ReportTemplateInfo(split[0], split[1], decimal.Parse(split[2])));
                }
            }
            reportTmplate.Sort((curr, next) => { return curr.ReportSubProType.CompareTo(next.ReportSubProType); });
            return reportTmplate.Count == 0 ? null : reportTmplate.ToArray();
        }

        private static bool GetInfoFromDB(string proIdentify, string outFilename)
        {
            string toDBConsole = AppDomain.CurrentDomain.BaseDirectory + @"\SystemData\Report\GeoDo.Mms.Tools.InfosCollectionDemo.exe";
            if (File.Exists(toDBConsole))
            {
                System.Diagnostics.Process exep = new System.Diagnostics.Process();
                exep.StartInfo = new System.Diagnostics.ProcessStartInfo(toDBConsole);
                exep.StartInfo.Arguments = proIdentify + " \"" + outFilename + "\"";//设定参数
                exep.StartInfo.UseShellExecute = false;//不使用系统外壳程序启动
                exep.StartInfo.RedirectStandardInput = false;//不重定向输入
                exep.StartInfo.RedirectStandardOutput = true;//重定向输出，而不是默认的显示在dos控制台上
                exep.StartInfo.CreateNoWindow = true;//不创建窗口
                exep.Start();
                exep.WaitForExit();
                exep.Close();
            }
            return File.Exists(outFilename);
        }
    }
}
