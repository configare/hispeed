using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    /// <summary>
    /// 每间隔指定的时间执行一次
    /// </summary>
    public class SmpTaskScheduler : IDisposable
    {
        public event Action<string[]> FileAdded;
        public event Action<string[]> FileRemoved;
        public event Action<string> MessageSend;

        private Timer scTimer = null;
        private CustomTaskScheduler sc = new CustomTaskScheduler(1);
        private bool isStarting;

        public bool IsStarting
        {
            get { return isStarting; }
            set { isStarting = value; }
        }

        private string _moniDir = @"D:\\Moni";

        public string MoniDir
        {
            get { return _moniDir; }
            set { _moniDir = value; }
        }

        public SmpTaskScheduler()
        {
        }

        public void StartListen()
        {
            //ThreadStart start = new ThreadStart(ListenWorker);
            //workerThread = new Thread(start);
            //workerThread.IsBackground = true;
            //workerThread.Start();
            isStarting = true;
            scTimer = new Timer(ListenWorker, null, 1 * 1000, 10 * 1000);
        }

        public void StopListen()
        {
            try
            {
                isStarting = false;
                scTimer.Dispose();
            }
            catch
            {
            }
        }

        private void ListenWorker(object obj)
        {
            try
            {
                Task workerTask = new Task(() => { TaskWorker(); });
                workerTask.Start(sc);
            }
            catch
            {
            }
        }

        private List<string> _curfiles = new List<string>();

        private void TaskWorker()
        {
            if (MessageSend != null)
                MessageSend("启动数据监听");
            if (!Directory.Exists(_moniDir))
            {
                MessageSend("监控地址不存在或无法访问" + _moniDir);
                return;
            }
            string[] files = Directory.GetFiles(_moniDir, "*.overview.png");
            List<string> addFiles = new List<string>();
            List<string> removeFiles = new List<string>();
            if (files == null || files.Length == 0)
            {
                removeFiles.AddRange(_curfiles);
                _curfiles.Clear();
                addFiles.Clear();
            }
            else
            {
                foreach (string file in files)
                {
                    if (!_curfiles.Contains(file))
                        addFiles.Add(file);
                }
                foreach (string file in _curfiles)
                {
                    if (!files.Contains(file))
                        removeFiles.Add(file);
                }
                _curfiles.Clear();
                _curfiles.AddRange(files);
            }
            if (addFiles.Count != 0)
                FileAdded(addFiles.ToArray());
            if (removeFiles.Count != 0)
                FileRemoved(removeFiles.ToArray());
        }

        public void Dispose()
        {
            if (isStarting)
                StopListen();
            sc.Dispose();
        }
    }
}
