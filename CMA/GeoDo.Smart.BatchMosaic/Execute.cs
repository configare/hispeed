using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
//using GeoDo.FileProject;
//using GeoDo.RasterProject;
//using GeoDo.Tools.Projection;
using System.Text.RegularExpressions;
using GeoDo.Tools.Mosaic;
using GeoDo.Tools;

namespace GeoDo.Smart.BatchMosaic
{
    /// <summary>
    /// 
    /// </summary>
    public class Execute
    {
        private Queue<string> _taskQueue = null;
        private string _searchOption = "*.*";
        private string _path;
        private Timer _timer;
        private Timer _processTimer;
        private bool _processing = false;
        private List<string> _processed = null;
        private MosaicInputArg _argMode;

        public Execute()
        {
            _taskQueue = new Queue<string>();
            _processed = new List<string>();
        }

        public void StartBatch(string path, string searchOption)
        {
            _argMode = MosaicInputArg.FromXml(path);
            if (_argMode == null)
            {
                Console.WriteLine("请提供参数文件");
                return;
            }
            else
            {
                Match match = InputFileRex.Match(_argMode.InputFilename);
                if (match.Groups["dir"].Success)
                    _path = match.Groups["dir"].Value;
                if (match.Groups["filter"].Success)
                    _searchOption = match.Groups["filter"].Value;
            }
            if (string.IsNullOrWhiteSpace(_path) || !Directory.Exists(_path))
            {
                Console.WriteLine("要扫描的文件路径为空,或者该路径不存在" + _path);
                return;
            }
            PrintInfo("启动扫描路径" + _path);
            if(!string.IsNullOrWhiteSpace(_searchOption))
                PrintInfo(" 文件过滤为" + _searchOption);
            _timer = new Timer(5 * 1000);
            _timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            _timer.Start();
            _processTimer = new Timer(5 * 1000);
            _processTimer.Elapsed += new ElapsedEventHandler(StartProcess);
            _processTimer.Start();
        }

        private Regex InputFileRex = new Regex(@"(?<dir>\S+)\s+(?<filter>\S*)", RegexOptions.Compiled);

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string[] fs;
            if (string.IsNullOrWhiteSpace(_searchOption))
                fs = Directory.GetFiles(_path);
            else
                fs = Directory.GetFiles(_path, _searchOption,SearchOption.AllDirectories);
            StringBuilder bat = new StringBuilder();
            foreach (string f in fs)
            {
                if (_processed.Contains(f))
                    continue;
                string xml = BulidArgXml(f);
                if (!string.IsNullOrWhiteSpace(xml) && File.Exists(xml))
                {
                    TaskQueueEnqueue(xml);
                    _processed.Add(f);
                    bat.AppendLine(f);
                }
            }
            if (bat.Length != 0)
            {
                PrintInfo("已扫描以下文件并加入处理队列：");
                PrintInfo(bat.ToString());
            }
        }

        private string BulidArgXml(string filename)
        {
            try
            {
                MosaicInputArg arg = _argMode.Copy();
                arg.InputFilename = filename;
                string xml = Path.Combine(arg.OutputDir, "BatchArgs\\" + Path.GetFileName(filename) + ".xml");
                arg.ToXml(xml);
                return xml;
            }
            catch(Exception ex)
            {
                PrintInfo("创建执行程序参数失败 "+ex.Message);
                return null;
            }
        }

        void TaskQueueEnqueue(string xml)
        {
            lock (_taskQueue)
            {
                _taskQueue.Enqueue(xml);
            }
        }

        string TaskQueueDequeue()
        {
            lock (_taskQueue)
            {
                if (_taskQueue.Count != 0)
                    return _taskQueue.Dequeue();
                return null;
            }
        }

        private void StartProcess(object sender, ElapsedEventArgs e)
        {
            if (_processing)
                return;
            string xml = TaskQueueDequeue();
            if (xml == null)
                return;
            _processing = true;
            Process p = null;
            string processString = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "GeoDo.Tools.Mosaic.exe");
            try
            {
                p = Process.Start(processString, xml);
                PrintInfo("启动处理程序" + p.StartTime.ToString());
                PrintInfo(p.ProcessName + " " + p.StartInfo.Arguments);
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                PrintInfo("处理程序异常 " + ex.Message);
                LogFactory.WriteLine(ex);
            }
            finally
            {
                PrintInfo("结束处理程序" + p.ExitTime.ToString());
                PrintInfo("");
                if (p != null)
                    p.Dispose();
                _processing = false;
            }
        }

        private void PrintInfo(string msg)
        {
            Console.WriteLine(msg);
        }
        
        public static void SimplePrj(string[] args)
        {
            Console.Clear();
            Console.ResetColor();
            string arg1, arg2;
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("参数不能为空");
                return;
            }

            if (args.Length == 1)
            {
                arg1 = args[0];
                arg2 = "";
            }
            else
            {
                arg1 = args[0];
                arg2 = args[1];
            }

            Execute c = new Execute();
            Console.WriteLine("启动批处理程序");
            c.StartBatch(arg1, arg2);

            string a = Console.ReadLine();
            while (a != "Q")
            {
                Console.WriteLine("按Q键退出执行");
                a = Console.ReadLine();
            }
        }
    }
}
