using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CodeCell.Bricks.Runtime
{
    public enum masLogHandlers
    { 
        Console,
        LogFile,
        WinEventViewer
    }

    internal interface ILogHandler:IDisposable
    {
        void WriterWarning(string objectName, string methodName, string sWarning);
        void WriterError(string objectName, string methodName, string sError);
        void WriterInfo(string objectName, string methodName, string sInfo);
        void WriterException(string objectName, string methodName, Exception Ex);
    }

    internal abstract class LogHandlerBase : ILogHandler, IDisposable
    {
        private const string MsgFormat = "    日志类型      :{0}\r\n    日志来源(类)  :{1}\r\n    日志来源(方法):{2}\r\n    日志内容      :{3}";
        private const string MsgFormatEx = "    日志类型      :{0}\r\n    日志来源(类)  :{1}\r\n    日志来源(方法):{2}\r\n    日志内容      :{3}\r\n    调用堆栈      :{4}";

        #region ILogHandler Members

        public void WriterWarning(string objectName, string methodName, string sWarning)
        {
            if (string.IsNullOrEmpty(objectName))
                objectName = string.Empty;
            if (string.IsNullOrEmpty(methodName))
                methodName = string.Empty;
            if (string.IsNullOrEmpty(sWarning))
                sWarning = string.Empty;
            WriteString(string.Format(MsgFormat, "警告", objectName, methodName, sWarning));
        }

        public void WriterError(string objectName, string methodName, string sError)
        {
            if (string.IsNullOrEmpty(objectName))
                objectName = string.Empty;
            if (string.IsNullOrEmpty(methodName))
                methodName = string.Empty;
            if (string.IsNullOrEmpty(sError))
                sError = string.Empty;
            WriteString(string.Format(MsgFormat, "错误", objectName, methodName, sError));
        }

        public void WriterInfo(string objectName, string methodName, string sInfo)
        {
            if (string.IsNullOrEmpty(objectName))
                objectName = string.Empty;
            if (string.IsNullOrEmpty(methodName))
                methodName = string.Empty;
            if (string.IsNullOrEmpty(sInfo))
                sInfo = string.Empty;
            WriteString(string.Format(MsgFormat, "信息", objectName, methodName, sInfo));
        }

        public void WriterException(string objectName, string methodName, Exception Ex)
        {
            if (string.IsNullOrEmpty(objectName))
                objectName = string.Empty;
            if (string.IsNullOrEmpty(methodName))
                methodName = string.Empty;
            if (Ex == null)
                Ex = new Exception(string.Empty);
            WriteString(string.Format(MsgFormatEx, "异常", objectName, methodName, Ex.Message,Ex.StackTrace));
        }

        #endregion

        private static object _lockObj = new object();
        protected void WriteString(string msg)
        {
            //lock (_lockObj)
            {
                WriteStringToTarget("[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "]\r\n" + msg);
            }
        }

        protected abstract void WriteStringToTarget(string msg);

        #region IDisposable Members

        public abstract void Dispose();

        #endregion
    }

    internal class LogHandlerToFile : LogHandlerBase 
    {
        private const string LogFilename = "AgileMap.{0}.log";
        private StreamWriter _stringWriter = null;

        public LogHandlerToFile()
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
            logFile =logFile+string.Format(LogFilename, DateTime.Now.ToString("yyyyMMdd"));
            try
            {
                _stringWriter = new StreamWriter(logFile, true, Encoding.Default);
            }
            catch
            {
                _stringWriter = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory+"\\log\\"+Guid.NewGuid().ToString() + ".log", true, Encoding.Default);
            }
        }

        protected override void WriteStringToTarget(string msg)
        {
            if (_stringWriter != null)
            {
                _stringWriter.WriteLine(msg);
                _stringWriter.Flush();
            }
        }

        public override void Dispose()
        {
            if (_stringWriter != null)
            {
                _stringWriter.Close();
                _stringWriter.Dispose();
            }
        }
    }

    internal class LogHandlerToConsole:LogHandlerBase
    {
        protected override void WriteStringToTarget(string msg)
        {
            Console.WriteLine( msg);
        }

        public override void Dispose()
        {
            ;
        }
    }

    internal class LogHandlerToWinEventViewer : LogHandlerBase
    {
        protected override void WriteStringToTarget(string msg)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
