using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Net;

namespace GeoDo.RSS.UI.AddIn.ReportService
{
    public class QuickReportXML
    {
        public static string tempQuickReportXML = "";
        public static string ReportArgsFile = "";

        public static bool WriteTempQuickReport(ReportTemplateInfo info)
        {
            string hostName = Dns.GetHostName();
            System.Net.IPAddress[] addressList = Dns.GetHostAddresses(hostName);
            string tempDir = AppDomain.CurrentDomain.BaseDirectory + "\\SystemData\\Report\\QuickReport\\";
            Directory.CreateDirectory(tempDir);
            tempQuickReportXML = tempDir + "\\" + addressList[addressList.Length - 1] + "_ReportTemplateInfo.xml";

            XElement root = new XElement("QuickTempletCommand");
            XElement children = new XElement("QuickTemplateIndexID");
            children.SetValue(info.ReportTemplateID);
            root.Add(children);
            root.Save(tempQuickReportXML);
            return true;
        }

        public static bool WriteQuickReportArgs(ReportTemplateInfo info, string reportTimeFile)
        {
            string hostName = Dns.GetHostName();
            System.Net.IPAddress[] addressList = Dns.GetHostAddresses(hostName);
            string tempDir = AppDomain.CurrentDomain.BaseDirectory + "\\SystemData\\Report\\QuickReport\\";
            Directory.CreateDirectory(tempDir);
            ReportArgsFile = tempDir + "\\" + addressList[addressList.Length - 1] + "_QuickReportArg.txt";

            List<string> fileContexts = new List<string>();
            fileContexts.Add(info.ReportSubProType);
            fileContexts.Add(info.ReportTemplateName);
            fileContexts.Add(info.ReportTemplateID.ToString());
            string reportStartTime = GetReportTime(reportTimeFile);
            fileContexts.Add(string.IsNullOrEmpty(reportStartTime) ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : reportStartTime.ToString());
            File.WriteAllLines(ReportArgsFile, fileContexts.ToArray(), Encoding.Default);
            return true;
        }

        private static string GetReportTime(string reportTimeFile)
        {
            if (File.Exists(reportTimeFile))
            {
                string[] reportTime = File.ReadAllLines(reportTimeFile, Encoding.Default);
                if (reportTime == null || reportTime.Length == 0)
                    return string.Empty;
                return reportTime[0];
            }
            return string.Empty;
        }

        public static bool UpdateReportArgsFile()
        {
            if (File.Exists(ReportArgsFile))
            {
                string[] contexts = File.ReadAllLines(ReportArgsFile,Encoding.Default);
                contexts[3] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                File.WriteAllLines(ReportArgsFile, contexts, Encoding.Default);
            }
            return true;
        }
    }
}
