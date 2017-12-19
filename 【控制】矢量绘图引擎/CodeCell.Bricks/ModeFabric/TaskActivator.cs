using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;
using CodeCell.Bricks.Runtime;

namespace CodeCell.Bricks.ModelFabric
{
    public sealed class TaskActivator:ITaskActivator
    {
        //同时执行的任务列表
        private List<ITask> _taskes = null;
        //日志记录服务
        private ILog _log = null;
        //进度提示服务
        private IProgressTracker _tracker = null;
        //任务的激活周期
        private int _interval = 6 * 60 * 1000;//6分钟
        //触发激活任务动作的记时器
        private System.Timers.Timer _timer = null;
        //查找Action所在程序集的路径
        private string[] _assemblySearchDirs = null;
        private const string errActivedTasksIsEmpty = "没有活动任务,服务器已休眠。";
        private const string errActiveOneTask = "任务\"{0}\"已激活,等待执行。";
        private const string errBeginOneTask = "任务\"{0}\"已开始执行。";
        private const string errEndOneTask = "任务\"{0}\"已执行完毕,等待下次执行。";
        private const string errAddTaskToThreadPool = "将任务\"{0}\"加入线程池失败,详细错误:";
        //正在运行中Task ID集合
        private List<int> _runningTaskIds = new List<int>();

        public delegate void TaskFinishedHandler(string taskName);
        public TaskFinishedHandler TaskFinished = null;

        public TaskActivator(string[] assemblySearchDirs)
        {
            _assemblySearchDirs = assemblySearchDirs;
        }

        public TaskActivator(ILog log, IProgressTracker tracker, string[] assemblies)
            :this(assemblies)
        {
            _log = log;
            _tracker = tracker;
        }

        public TaskActivator(ILog log, IProgressTracker tracker, int interval, string[] assemblies)
            :this(assemblies)
        {
            _log = log;
            _tracker = tracker;
            _interval = interval;
        }

        private void Do()
        {
            if (_taskes == null || _taskes.Count == 0)
            {
                LogWarning(errActivedTasksIsEmpty);
                return;
            }
            int n = _taskes.Count;
            for (int i = n - 1; i >= 0; i--)
            {
                ITask task = _taskes[i];
                //判断是否可以并发
                bool isCanIntercurrent = (task as Task).IsCanIntercurrent;
                if (!isCanIntercurrent)
                    if (_runningTaskIds.Contains((task as Task).Id))//不允许并发
                        continue;
                //激活任务
                LogInfo(string.Format(errActiveOneTask, task.Name));
                //加入线程池执行
                try
                {
                    //ExecuteTask(task);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ExecuteTask), task);
                }
                catch (OutOfMemoryException ex)
                {
                    LogError(string.Format(errAddTaskToThreadPool, task.Name));
                    LogException(ex);
                }
            }
        }

        private void ExecuteTask(object taskobj)
        {
            //lock (taskobj)
            {
                ITask task = taskobj as ITask;
                int id = (task as Task).Id;
                bool isCanIntercurrent = (task as Task).IsCanIntercurrent;
                //
                if (!isCanIntercurrent)
                {
                    if (_runningTaskIds.Contains(id))//不允许并发
                        return;
                    else
                        _runningTaskIds.Add(id);
                }
                //
                LogInfo(string.Format(errBeginOneTask, task.Name));
                //
                task.Execute();
                //
                LogInfo(string.Format(errEndOneTask, task.Name));
                //如果不允许循环执行则移除任务
                if (!(task as Task).IsLoopExecute)
                {
                    lock (_taskes)
                    {
                        _taskes.Remove(task);
                    }
                }
                //从正在执行的任务列表移除
                if (!isCanIntercurrent)
                {
                    if (_runningTaskIds.Contains(id))
                        _runningTaskIds.Remove(id);
                    if (TaskFinished != null)
                        TaskFinished(task.Name);
                }
            }
        }

        #region ITaskActivator 成员

        public int Interval
        {
            get { return _interval; }
            set 
            {
                if (_timer != null)
                    _timer.Interval = value;
                _interval = value;
            }
        }

        public int Count
        {
            get { return _taskes != null ? _taskes.Count : 0; }
        }

        public ITask[] Tasks
        {
            get { return _taskes != null && _taskes.Count > 0 ? _taskes.ToArray() : null; }
        }

        public ITask AddTask(string scriptfilename, IVarProvider varProvider, bool isLoopExecute)
        {
            using (TaskScriptParser parser = new TaskScriptParser(_assemblySearchDirs))
            {
                ArgAutoBingdingEnvironment env = null;
                ITask task = parser.FromTaskScriptFile(scriptfilename, out env);
                if (task != null)
                {
                    (task as Task).SetArgAutoBingdingEnvironment(env);
                    (task as Task).IsLoopExecute = isLoopExecute;
                    (task as Task).SetVarProvider(varProvider);
                    (task as Task).SetLog(_log);
                    (task as Task).SetTracker(_tracker);
                    if (_taskes == null)
                        _taskes = new List<ITask>();
                    _taskes.Add(task);
                    return task;
                }
            }
            return null;
        }

        public void Start()
        {
            if (_timer == null)
                BuildTimer();
            if (!_timer.Enabled)
                _timer.Start();
        }

        private void BuildTimer()
        {
            _timer = new System.Timers.Timer(_interval);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
        }

        private object _lockObject = new object();
        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_lockObject)
            {
                Do();
            }
        }

        public void Stop()
        {
            if (_timer != null && _timer.Enabled)
                _timer.Stop();
        }

        #endregion

        private void LogException(Exception ex)
        {
            if (_log != null)
                _log.WriterException(ex);
        }

        private void LogInfo(string info)
        {
            if (_log != null)
                _log.WriterInfo(info);
        }

        private void LogWarning(string info)
        {
            if (_log != null)
                _log.WriterWarning(info);
        }

        private void LogError(string errorInfo)
        {
            if (_log != null)
                _log.WriterError(errorInfo);
        }

        #region ITaskActivator 成员


        TaskActivator.TaskFinishedHandler ITaskActivator.TaskFinished
        {
            get { return TaskFinished; }
            set { TaskFinished = value; }
        }

        #endregion
    }
}
