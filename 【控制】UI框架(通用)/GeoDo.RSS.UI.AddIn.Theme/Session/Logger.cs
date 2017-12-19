//=========================================
//
// 描 述：日志类，注意需要在日志记录的目录给用户写入权限!
//
//=========================================
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Runtime.CompilerServices;
using System.Web;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    /// <summary>
    /// 日志类
    /// </summary>
    public sealed class Logger
    {
        #region Member Variables
        /// <summary>
        /// 用于Trace的组织输出的类别名称
        /// </summary>
        private const string trace_sql = "\r\n***********************TRACE_SQL {0}*****************************\r\nTRACE_SQL";
        /// <summary>
        /// 用于Trace的组织输出的类别名称
        /// </summary>
        private const string trace_exception = "\r\n***********************TRACE_EXCEPTION {0}***********************";
        /// <summary>
        /// 当前日志的日期
        /// </summary>
        private static DateTime CurrentLogFileDate = DateTime.Now;
        /// <summary>
        /// 日志对象
        /// </summary>
        private static TextWriterTraceListener twtl;
        /// <summary>
        /// 日志根目录
        /// </summary>
        private static string log_root_directory = System.AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// 日志子目录
        /// </summary>
        private static string log_subdir;
        /// <summary>
        /// "      {0} = {1}"
        /// </summary>
        private const string FORMAT_TRACE_PARAM = "      {0} = {1}";
        /// <summary>
        /// 1   仅控制台输出
        /// 2   仅日志输出
        /// 3   控制台+日志输出
        /// </summary>
        private static readonly int flag = 2;         //可以修改成从配置文件读取
        private static int _errorTimes = 0;           //跟踪写日志失败次数,超过一定次数停止跟踪
        #endregion

        #region Constructor
        static Logger()
        {
            System.Diagnostics.Trace.AutoFlush = true;
            switch (flag)
            {
                case 1:
                    System.Diagnostics.Trace.Listeners.Add(new ConsoleTraceListener());
                    break;
                case 2:
                    System.Diagnostics.Trace.Listeners.Add(TWTL);
                    break;
                case 3:
                    System.Diagnostics.Trace.Listeners.Add(new ConsoleTraceListener());
                    System.Diagnostics.Trace.Listeners.Add(TWTL);
                    break;
            }
        }
        #endregion
        #region Method
        #region trace
        /// <summary>
        /// 异步错误日志
        /// </summary>
        /// <param name="value"></param>
        public static void Trace(Exception ex)
        {
            if (_errorTimes > 10)
                return;
            new AsyncLogException(BeginTraceError).BeginInvoke(ex, null, null);
        }
        /// <summary>
        /// 异步SQL日志
        /// </summary>
        /// <param name="cmd"></param>
        public static void Trace(SqlCommand cmd)
        {
            new AsyncLogSqlCommand(BeginTraceSqlCommand).BeginInvoke(cmd, null, null);
        }
        /// <summary>
        /// 异步SQL日志
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        public static void Trace(string sql, params SqlParameter[] parameter)
        {
            new AsyncLogSql(BeginTraceSql).BeginInvoke(sql, parameter, null, null);
        }
        #endregion
        #region delegate
        private delegate void AsyncLogException(Exception ex);
        private delegate void AsyncLogSqlCommand(SqlCommand cmd);
        private delegate void AsyncLogSql(string sql, params SqlParameter[] parameter);
        private static void BeginTraceError(Exception ex)
        {
            if (null != ex)
            {
                //检测日志日期
                StrategyLog();
                //输出日志头
                System.Diagnostics.Trace.WriteLine(string.Format(trace_exception, DateTime.Now));
                while (null != ex)
                {
                    System.Diagnostics.Trace.WriteLine(string.Format("{0} {1}\r\n{2}\r\nSource:{3}", ex.GetType().Name, ex.Message, ex.StackTrace, ex.Source));
                    ex = ex.InnerException;
                }
            }
        }
        private static void BeginTraceSqlCommand(SqlCommand cmd)
        {
            if (null != cmd)
            {
                SqlParameter[] parameter = new SqlParameter[cmd.Parameters.Count];
                cmd.Parameters.CopyTo(parameter, 0);
                BeginTraceSql(cmd.CommandText, parameter);
            }
        }
        private static void BeginTraceSql(string sql, params SqlParameter[] parameter)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                //检测日志日期
                StrategyLog();
                System.Diagnostics.Trace.WriteLine(sql, string.Format(trace_sql, DateTime.Now));
                if (parameter != null)
                {
                    foreach (SqlParameter param in parameter)
                    {
                        System.Diagnostics.Trace.WriteLine(string.Format(FORMAT_TRACE_PARAM, param.ParameterName, param.Value));
                    }
                }
            }
        }
        #endregion
        #region helper
        /// <summary>
        /// 根据日志策略生成日志
        /// </summary>
        private static void StrategyLog()
        {
            try
            {
                //判断日志日期
                if (DateTime.Compare(DateTime.Now.Date, CurrentLogFileDate.Date) != 0)
                {
                    DateTime currentDate = DateTime.Now.Date;
                    //生成子目录
                    BuiderDir(currentDate);
                    //更新当前日志日期
                    CurrentLogFileDate = currentDate;
                    System.Diagnostics.Trace.Flush();
                    //更改输出
                    if (twtl != null)
                        System.Diagnostics.Trace.Listeners.Remove(twtl);
                    System.Diagnostics.Trace.Listeners.Add(TWTL);
                }
            }
            catch
            {
                _errorTimes++;
            }
        }
        /// <summary>
        /// 根据年月生成子目录
        /// </summary>
        /// <param name="currentDate"></param>
        private static void BuiderDir(DateTime currentDate)
        {
            int year = currentDate.Year;
            int month = currentDate.Month;
            //年/月
            string subdir = string.Concat(@"Log\", year, '\\', month);
            string path = Path.Combine(log_root_directory, subdir);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            log_subdir = subdir;
        }
        #endregion
        #endregion
        #region Properties
        /// <summary>
        /// 日志文件路径
        /// </summary>
        /// <returns></returns>
        private static string GetLogFullPath
        {
            get
            {
                return string.Concat(log_root_directory, '\\', string.Concat(log_subdir, CurrentLogFileDate.ToShortDateString(), ".txt"));
            }
        }
        /// <summary>
        /// 跟踪输出日志文件
        /// </summary>
        private static TextWriterTraceListener TWTL
        {
            get
            {
                if (twtl == null)
                {
                    if (string.IsNullOrEmpty(log_subdir))
                        BuiderDir(DateTime.Now);
                    else
                    {
                        string logPath = GetLogFullPath;
                        if (!Directory.Exists(Path.GetDirectoryName(logPath)))
                            BuiderDir(DateTime.Now);
                    }
                    twtl = new TextWriterTraceListener(GetLogFullPath);
                }
                return twtl;
            }
        }
        #endregion
    }
}
