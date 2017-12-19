using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    internal class   AIRSLog : IDisposable
    {
        private const string LogFilename = "AIRSDataProcess.{0}.log";
        private StreamWriter _stringWriter = null;

        public AIRSLog()
        {
            string logFile = AppDomain.CurrentDomain.BaseDirectory + "Log\\";
            if (!Directory.Exists(logFile))
            {
                try
                {
                    Directory.CreateDirectory(logFile);
                }
                catch
                {
                    logFile = AppDomain.CurrentDomain.BaseDirectory;
                }
            }
            logFile = logFile + string.Format(LogFilename, DateTime.Now.ToString("yyyyMMdd"));
            try
            {
                _stringWriter = new StreamWriter(logFile, true, Encoding.Default);
            }
            catch
            {
                _stringWriter = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\log\\" + Guid.NewGuid().ToString() + ".log", true, Encoding.Default);
            }
        }

        internal void Write(string msg)
        {
            if (_stringWriter != null)
            {
                _stringWriter.WriteLine(DateTime.Now.ToLongTimeString());
                _stringWriter.WriteLine(msg);
                _stringWriter.Flush();
            }
        }

        public void Dispose()
        {
            if (_stringWriter != null)
            {
                _stringWriter.Close();
                _stringWriter.Dispose();
            }
        }
    }

    public class AIRSLogFactory
    {
        public static void WriteLine(string msg)
        {
            try
            {
                using (AIRSLog log = new AIRSLog())
                {
                    log.Write(msg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("LogFactory:Write:" + ex.Message);
            }
        }
    }
}
