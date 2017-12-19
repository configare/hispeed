using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    internal class Log : IDisposable
    {
        private const string LogFilename = "{0}.DataProcess.{1}.log";
        private StreamWriter _stringWriter = null;

        public Log(string name)
        {
            string logFile = AppDomain.CurrentDomain.BaseDirectory + "Log\\" + DateTime.Now.ToString("yyyyMMdd")+"\\";
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
            logFile = logFile + string.Format(LogFilename, name, DateTime.Now.ToString("yyyyMMdd"));
            try
            {
                _stringWriter = new StreamWriter(logFile, true, Encoding.Default);
            }
            catch
            {
                _stringWriter = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\log\\" +DateTime.Now.ToString("yyyyMMdd")+"\\"+ Guid.NewGuid().ToString() + ".log", true, Encoding.Default);
            }
        }

        internal void Write(string msg)
        {
            if (_stringWriter != null)
            {
                _stringWriter.Write(DateTime.Now.ToLongTimeString()+"\t");
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

    public class LogFactory
    {
        public static void WriteLine(string name, string msg)
        {
            try
            {
                using (Log log = new Log(name))
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
