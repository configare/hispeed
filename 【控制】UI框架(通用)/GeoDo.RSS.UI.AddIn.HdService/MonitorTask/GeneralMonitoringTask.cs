using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    public class GeneralMonitoringTask<T>
    {
        public event Action<T[]> DataAdded;
        public event Action<T[]> DataRemoved;
        public event Action<string> MessageSend;

        private Timer scTimer = null;
        private CustomTaskScheduler cts = new CustomTaskScheduler(1);
        private bool isStarting;
        private Func<T[], T, bool> DataContains;
        private Func<T[]> FindDatasFunc;

        public bool IsStarting
        {
            get { return isStarting; }
            set { isStarting = value; }
        }

        /// <summary>
        /// 定时执行任务
        /// </summary>
        /// <param name="dataContains">监控数据判断条件,用于判断数据是否已经存在的条件</param>
        /// <param name="findDatasFunc">监控数据源,用于获取数据</param>
        public GeneralMonitoringTask(Func<T[], T, bool> dataContains, Func<T[]> findDatasFunc)
        {
            DataContains = dataContains;
            FindDatasFunc = findDatasFunc;
        }

        public void StartListen()
        {
            if (isStarting)
                return;
            isStarting = true;
            scTimer = new Timer(ListenWorker, null, 2 * 1000, 20 * 1000);
        }

        public void ChangeTimer(long dueTime, long period)
        {
            scTimer.Change(dueTime, period);
        }

        public void StopAsync()
        { 
        }

        public void StopListen()
        {
            try
            {
                if (!isStarting)
                    return;
                isStarting = false;
                scTimer.Dispose();
                if (MessageSend != null)
                    MessageSend("停止监控");
            }
            catch (Exception ex)
            {
                Console.WriteLine("停止监控出现异常：" + ex.Message);
            }
        }

        private bool listeing = false;

        private void ListenWorker(object obj)
        {
            try
            {
                if (!isStarting || listeing)
                    return;
                listeing = true;
                Task workerTask = new Task(() => { TaskWorker(); });
                workerTask.Start(cts);
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据获取出现异常：" + ex.Message);
            }
            finally
            {
                listeing = false;
            }
        }

        private List<T> _curfiles = new List<T>();

        public List<T> Curfiles
        {
            get { return _curfiles; }
        }

        private void TaskWorker()
        {
            if (FindDatasFunc == null)
            {
                MessageSend("监控数据源没有提供或不存在" + FindDatasFunc);
                return;
            }
            if (DataContains == null)
            {
                MessageSend("监控数据判断条件没有提供或不存在" + DataContains);
                return;
            }
            try
            {
                T[] files = FindDatasFunc();
                List<T> addFiles = new List<T>();
                List<T> removeFiles = new List<T>();
                if (files == null || files.Length == 0)
                {
                    removeFiles.AddRange(_curfiles);
                    _curfiles.Clear();
                    addFiles.Clear();
                }
                else
                {
                    foreach (T file in files)
                    {
                        if (!DataContains(_curfiles.ToArray(), file))
                            addFiles.Add(file);
                    }
                    foreach (T file in _curfiles)
                    {
                        if (!DataContains(files, file))
                            removeFiles.Add(file);
                    }
                    _curfiles.Clear();
                    _curfiles.AddRange(files);
                }
                if (addFiles.Count != 0)
                    DataAdded(addFiles.ToArray());
                if (removeFiles.Count != 0)
                    DataRemoved(removeFiles.ToArray());
            }
            catch (Exception ex)
            {
                MessageSend(ex.Message);
            }
        }

        public void Dispose()
        {
            if (isStarting)
                StopListen();
            cts.Dispose();
        }
    }
}
