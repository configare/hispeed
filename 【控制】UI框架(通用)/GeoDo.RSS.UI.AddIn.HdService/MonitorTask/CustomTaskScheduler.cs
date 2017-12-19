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
    public class CustomTaskScheduler : TaskScheduler, IDisposable
    {
        Thread[] _threads = null;
        private BlockingCollection<Task> _tasks = new BlockingCollection<Task>();
        int _concurrencyLevel;

        private object _syncObj;

        public CustomTaskScheduler(int concurrencyLevel)
        {
            _threads = new Thread[concurrencyLevel];
            this._concurrencyLevel = concurrencyLevel;
            for (int i = 0; i < concurrencyLevel; i++)
            {
                _threads[i] = new Thread(() =>
                {
                    foreach (Task task in _tasks.GetConsumingEnumerable())
                        this.TryExecuteTask(task);

                });
                _threads[i].Start();
            }
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(_syncObj, ref lockTaken);
                if (lockTaken)
                {
                    return _tasks.ToArray();
                }
                else
                    throw new NotSupportedException();
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(_syncObj);
            }
        }

        protected override void QueueTask(Task task)
        {
            _tasks.Add(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (_threads.Contains(Thread.CurrentThread))
                return TryExecuteTask(task);
            return false;
        }

        public override int MaximumConcurrencyLevel
        {
            get
            {
                return _concurrencyLevel;
            }
        }

        public bool IsDispose = false;

        public void Dispose()
        {
            IsDispose = true;
            if (_tasks == null)        //防止重入
                return;
            _tasks.CompleteAdding();
            foreach (Thread t in _threads)
                t.Join();
            _tasks.Dispose();
            _tasks = null;
        }
    }
}
