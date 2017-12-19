using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using GeoDo.FileProject;
using GeoDo.RasterProject;
using GeoDo.Tools.Projection;
using System.Text.RegularExpressions;
using GeoDo.Tools;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;

namespace GeoDo.Smart.BatchProjectionMosaic
{
    class FileIdentify
    {
        public FileIdentify(string filename)
        {
            FileName = filename;
            Identify = new RasterIdentify(filename);
        }

        public string FileName;
        public RasterIdentify Identify;
    }

    /// <summary>
    /// 根据配置监控一个目录，生成用于投影的参数xml，并执行投影动作
    /// </summary>
    public class Execute
    {
        private Queue<string> _taskQueue = null;
        private string _searchOption = "*.*";
        private string _path;
        private Timer _timer;
        private Timer _processTimer;
        private bool _processing = false;
        private List<string> _processedFiles = null;
        private InputArg _argMode;
        private bool _isFileList = false;
        private string[] _fileList = null;

        public Execute()
        {
            _taskQueue = new Queue<string>();
            _processedFiles = new List<string>();
        }

        public void StartBatch(string path, string searchOption)
        {
            _argMode = InputArg.ParseXml(path);
            if (_argMode == null)
            {
                Console.WriteLine("请提供参数文件");
                return;
            }
            else
            {
                //判断InputFilename参数值是否为文件列表
                string[] fileList = _argMode.InputFilename.Split(new char[]{';'},StringSplitOptions.RemoveEmptyEntries);
                if (fileList.Length > 1)
                {
                    foreach (string file in fileList)
                    {
                        if (!File.Exists(file))
                        {
                            Console.WriteLine("文件" + file + "不存在！");
                            return;
                        }
                    }
                    _isFileList = true;
                    _fileList = fileList;
                }
                else
                {
                    Match match = InputFileRex.Match(_argMode.InputFilename);
                    if (match.Groups["dir"].Success)
                        _path = match.Groups["dir"].Value;
                    if (match.Groups["filter"].Success)
                        _searchOption = match.Groups["filter"].Value;
                    if (string.IsNullOrWhiteSpace(_path) || !Directory.Exists(_path))
                    {
                        Console.WriteLine("要扫描的文件路径为空,或者该路径不存在" + _path);
                        return;
                    }
                    PrintInfo("启动扫描路径" + _path);
                    if (!string.IsNullOrWhiteSpace(_searchOption))
                        PrintInfo(" 文件过滤为" + _searchOption);
                }
            }          
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
            string[] fs =null;
            if (_isFileList)
                fs = _fileList;
            else
            {
                if (string.IsNullOrWhiteSpace(_searchOption))
                    fs = Directory.GetFiles(_path);
                else
                    fs = Directory.GetFiles(_path, _searchOption);
            }
            if (fs == null || fs.Length == 0)
                return;
            //初步筛选扫描到的文件。
            List<FileIdentify> newFiles = new List<FileIdentify>();
            foreach (string f in fs)
            {
                if (_processedFiles.Contains(f))//过滤已经处理过的数据
                    continue;
                FileIdentify fileIdentify = new FileIdentify(f);
                if (fileIdentify.Identify == null)
                {
                    PrintInfo("获取文件标识失败：" + Path.GetFileName(f));
                    continue;
                }
                newFiles.Add(fileIdentify);
            }
            if (newFiles.Count == 0)
                return;
            PrintInfo("扫描新的文件加入待处理队列：");
            StringBuilder bat = new StringBuilder();
            FileIdentify[] files = newFiles.OrderBy((identify) => { return identify.Identify.OrbitDateTime; }).ToArray();//按轨道市时间排序            
            foreach (FileIdentify file in files)
            {
                string xml = BulidArgXml(file);
                if (!string.IsNullOrWhiteSpace(xml) && File.Exists(xml))
                {
                    TaskQueueEnqueue(xml);
                    _processedFiles.Add(file.FileName);
                    bat.AppendLine();
                    PrintInfo(file.FileName);
                }
            }
        }

        DateTime? _orbitDataTime = null;

        private string BulidArgXml(FileIdentify file)
        {
            try
            {
                InputArg arg = _argMode.Copy();
                arg.InputFilename = file.FileName;
                if (file.Identify.OrbitDateTime == DateTime.MinValue)//没解析出来时间。
                {
                    PrintInfo("获取轨道时间失败：" + file.FileName);
                }
                else if (_orbitDataTime ==  null||(Math.Abs((_orbitDataTime.Value - file.Identify.OrbitDateTime).TotalMinutes) >= 20))
                {
                    _orbitDataTime = file.Identify.OrbitDateTime;
                    arg.PervObservationDate = _orbitDataTime.Value.ToString("yyyyMMdd");
                    arg.PervObservationTime = _orbitDataTime.Value.ToString("HHmm");
                    arg.OrbitIdentify = _orbitDataTime.Value.ToString("HHmm");
                }
                else
                {
                    arg.PervObservationDate = _orbitDataTime.Value.ToString("yyyyMMdd");
                    arg.PervObservationTime = _orbitDataTime.Value.ToString("HHmm");
                    arg.OrbitIdentify = _orbitDataTime.Value.ToString("HHmm");
                }
                string xml = Path.Combine(arg.OutputDir, "batchArgs\\" + Path.GetFileName(file.FileName) + ".xml");
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
            string processString = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "GeoDo.Tools.Projection.exe");
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
                GeoDo.Tools.LogFactory.WriteLine("处理程序异常 " + ex.Message);
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
