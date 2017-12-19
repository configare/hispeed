using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    public class TaskDataProcess : IListen, IDisposable
    {
        public event Action DataUpdate;
        public event Action<string> MessageSend;

        private Thread workerThread;
        private Task workerTask;

        public TaskDataProcess()
        {
            this.StartListen();
        }

        public void StartListen()
        {
            ThreadStart start = new ThreadStart(ListenWorker);
            workerThread = new Thread(start);
            workerThread.IsBackground = true;
            workerThread.Start();

            workerTask = new Task(() => { ListenWorker(); });
            CustomTaskScheduler sc = new CustomTaskScheduler(1);
            workerTask.Start(sc);
        }

        public void StopListen()
        {
            try
            {
                workerThread.Abort();
            }
            catch
            {
            }
        }

        private void ListenWorker()
        {
            try
            {
                if (MessageSend != null)
                    MessageSend("启动数据监听");
                
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 一种简单的可控并发粒度的TaskScheduler的实现
    /// 当我们使用.net 4.0中的任务并行库的时候，有时候我们是需要自己控制并发粒度（调度线程数）的，这个时候往往就需要我们自己写TaskScheduler了，一个简单的实现如下：
    /// PS：当前在Parallel.ForEach或Parallel.For等数据并发函数中可以通过ParallelOptions.MaxDegreeOfParallelism来控制并发粒度，但无法控制调度顺序。也可以通过类似这样的TaskScheduler来改变调度顺序。
    /// </summary>
    public sealed class SimpleTaskScheduler : TaskScheduler, IDisposable
    {
        BlockingCollection<Task> _tasks = new BlockingCollection<Task>();
        List<Thread> _threads = new List<Thread>();

        public SimpleTaskScheduler(int initNumberOfThreads = 3)
        {
            if (initNumberOfThreads < 1)
                throw new ArgumentOutOfRangeException();

            _threads.AddRange(Enumerable.Range(0, initNumberOfThreads).Select(_ => CreateThread()));
        }

        Thread CreateThread()
        {
            var thread = new Thread(() =>
            {
                foreach (var t in _tasks.GetConsumingEnumerable())
                {
                    TryExecuteTask(t);
                }
            });

            thread.IsBackground = true;
            thread.Start();
            return thread;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            //这个函数好像没有调过，返回null也不影响功能
            return _tasks.ToArray();
        }

        protected override void QueueTask(Task task)
        {
            _tasks.Add(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return TryExecuteTask(task);
        }

        public override int MaximumConcurrencyLevel { get { return _threads.Count; } }

        #region IDisposable 成员

        public void Dispose()
        {
            if (_tasks == null)        //防止重入
                return;

            _tasks.CompleteAdding();
            _threads.ForEach(t => t.Join());

            _tasks.Dispose();
            _tasks = null;
        }

        #endregion
    }
}
