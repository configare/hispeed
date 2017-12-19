using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

namespace CodeCell.Bricks.Runtime
{
    public static class Log
    {
        /*
         * 如果某个类是派生自WithAOPObj的，那么实例化自该类的实例的每个方法调用时，将自动给以下两个变量赋值
         * 在方法调用结束时将对以下两个变量赋值null
        */
        internal static string CurrentObjectName = null;
        internal static string CurrentMethodName = null;
        internal static ILogHandler _logHandler = null;
        internal static masLogHandlers _logHandlerType = masLogHandlers.LogFile;
        internal static object LockObj = new object();

        static Log()
        {
            LogHandler = masLogHandlers.LogFile;
        }

        public static masLogHandlers LogHandler
        {
            set
            {
                lock (LockObj)
                {
                    try
                    {
                        if (_logHandlerType != value || _logHandler == null)
                        {
                            if (_logHandler != null)
                                _logHandler.Dispose();
                            switch (value)
                            {
                                case masLogHandlers.LogFile:
                                    _logHandler = new LogHandlerToFile();
                                    break;
                                case masLogHandlers.WinEventViewer:
                                    _logHandler = new LogHandlerToWinEventViewer();
                                    break;
                                case masLogHandlers.Console:
                                    _logHandler = new LogHandlerToConsole();
                                    break;
                            }
                            _logHandlerType = value;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                            throw new Exception(ex.Message + ex.InnerException.Message);
                        else
                            throw new Exception(ex.Message);
                    }
                }
            }
            get { return _logHandlerType; }
        }

        public static void WriterWarning(string sWarning)
        {
            lock (LockObj)
            {
                WriterWarning(CurrentObjectName, CurrentMethodName, sWarning);
            }
        }

        public static void WriterError(string sError)
        {
            lock (LockObj)
            {
                WriterError(CurrentObjectName, CurrentMethodName, sError);
            }
        }

        public static void WriterInfo(string sInfo)
        {
            lock (LockObj)
            {
                WriterInfo(CurrentObjectName, CurrentMethodName, sInfo);
            }
        }

        public static void WriterException(Exception Ex)
        {
            lock (LockObj)
            {
                WriterException(CurrentObjectName, CurrentMethodName, Ex);
            }
        }

        public static void WriterWarning(string objectName,string methodName,string sWarning)
        {
            lock (LockObj)
            {
                _logHandler.WriterWarning(objectName, methodName, sWarning);
            }
        }

        public static void WriterError(string objectName, string methodName, string sError)
        {
            lock (LockObj)
            {
                _logHandler.WriterError(objectName, methodName, sError);
            }
        }

        public static void WriterInfo(string objectName, string methodName, string sInfo)
        {
            lock (LockObj)
            {
                _logHandler.WriterInfo(objectName, methodName, sInfo);
            }
        }

        public static void WriterException(string objectName, string methodName, Exception Ex)
        {
            lock (LockObj)
            {
                _logHandler.WriterException(objectName, methodName, Ex);
            }
        }
    }

    public interface ILog
    {
        void WriterWarning(string sWarning);
        void WriterError(string sError);
        void WriterInfo(string sInfo);
        void WriterException(Exception Ex);
    }
}
