using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.Bricks.ModelFabric
{
    public interface ITaskActivator
    {
        int Count { get; }
        ITask[] Tasks { get; }
        ITask AddTask(string scriptfilename, IVarProvider varProvider, bool isLoopExecute);
        int Interval { get; set; }
        void Start();
        void Stop();
        TaskActivator.TaskFinishedHandler TaskFinished { get; set; }
    }
}
