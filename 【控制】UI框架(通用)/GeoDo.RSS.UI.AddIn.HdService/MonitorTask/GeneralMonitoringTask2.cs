using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    public class GeneralMonitoringTask2
    {
        private Timer scTimer = null;
        private CustomTaskScheduler cts = new CustomTaskScheduler(1);
        private bool isStarting;
        private List<Action> _actions = new List<Action>();
        private bool _isWorkering = false;
        private Exception _listenEnception;
        public event ThreadExceptionEventHandler ThreadExceptionEventHandler;

        public Exception ListenException
        {
            get { return _listenEnception; }
        }

        /// <summary>
        /// 定时执行任务
        /// </summary>
        public GeneralMonitoringTask2(Action[] actions)
        {
            _actions.AddRange(actions);
        }

        public bool IsStarting
        {
            get { return isStarting; }
            set { isStarting = value; }
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

        public void StopListen()
        {
            try
            {
                if (_isWorkering)
                    source.Cancel();
                if (!isStarting)
                    return;
                isStarting = false;
                scTimer.Dispose();
            }
            catch (Exception ex)
            {
                _listenEnception = ex;
                if (ThreadExceptionEventHandler != null)
                    ThreadExceptionEventHandler(this, new ThreadExceptionEventArgs(ex));
            }
        }

        CancellationTokenSource source;

        private void ListenWorker(object obj)
        {
            try
            {
                if (!isStarting || _isWorkering)
                    return;
                _isWorkering = true;
                //Task workerTask = new Task(() => { TaskWorker(); });
                //workerTask.Start(cts);
                source = new CancellationTokenSource();
                var token = source.Token;
                Task.Factory.StartNew(() =>
                {
                    TaskWorker();
                }, token)
                    .ContinueWith((t) =>
                    {
                        if (t.Exception == null)
                            return;
                        Console.WriteLine("I have observed a {0}", t.Exception.InnerException.GetType().Name);
                    }, TaskContinuationOptions.OnlyOnFaulted);
            }
            catch (Exception ex)
            {
                _listenEnception = ex;
                if (ThreadExceptionEventHandler != null)
                    ThreadExceptionEventHandler(this, new ThreadExceptionEventArgs(ex));
            }
            finally
            {
                _isWorkering = false;
            }
        }

        private void TaskWorker()
        {
            if (_actions.Count != 0)
            {
                for (int i = 0; i < _actions.Count; i++)
                {
                    if (cts.IsDispose)
                        return;
                    _actions[i]();
                }
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
