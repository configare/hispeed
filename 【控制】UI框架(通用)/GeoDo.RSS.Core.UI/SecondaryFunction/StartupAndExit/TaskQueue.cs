using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public delegate void OnBeforeDoQueueTaskItemHandler(object sender,IQueueTaskItem taskItem);
    public delegate void OnAfterDoQueueTaskItemHandler(object sender,IQueueTaskItem taskItem,bool isSuccessed);

    public class TaskQueue:IDisposable
    {
        private List<IQueueTaskItem> _taskItems = null;
        private IProgressTracker _tracker = null;
        public OnBeforeDoQueueTaskItemHandler OnBeforeDoQueueTaskItemHandler = null;
        public OnAfterDoQueueTaskItemHandler OnAfterQueueTaskItemHandler = null;

        public TaskQueue(IProgressTracker tracker)
        {
            _tracker = null;
        }

        public TaskQueue(IQueueTaskItem[] taskitems,IProgressTracker tracker)
            :this(tracker)
        {
            if (_taskItems == null)
                _taskItems = new List<IQueueTaskItem>();
            _taskItems.AddRange(taskitems);
        }

        public void AddTask(IQueueTaskItem taskItem)
        {
            if (_taskItems == null)
                _taskItems = new List<IQueueTaskItem>();
            _taskItems.Add(taskItem);
        }

        public bool Exists(IQueueTaskItem taskItem)
        {
            if (_taskItems == null || _taskItems.Count == 0)
                return false;
            return _taskItems.Contains(taskItem);
        }

        public void Remove(IQueueTaskItem taskItem)
        {
            if (_taskItems == null || _taskItems.Count == 0)
                return;
            if (_taskItems.Contains(taskItem))
                _taskItems.Remove(taskItem);
        }

        public void Clear()
        {
            if (_taskItems != null)
                _taskItems.Clear();
        }

        public void DoAllTask()
        {
            if (_taskItems == null || _taskItems.Count == 0)
                return;
            _taskItems.Reverse();
            //
            if (_tracker != null)
                _tracker.StartTracking(string.Empty, _taskItems.Count);
            int idx = 0;
            //
            for (int i = _taskItems.Count - 1; i >= 0; i--)
            {
                IQueueTaskItem taskItem = _taskItems[i];
                bool isOK = false ;
                try
                {
                    string name = taskItem.Name != null ? taskItem.Name : taskItem.ToString();
                    if (_tracker != null)
                        _tracker.Tracking("正在" + name + "......", idx);
                    //
                    if (OnBeforeDoQueueTaskItemHandler != null)
                        OnBeforeDoQueueTaskItemHandler(this, _taskItems[i]);
                    _taskItems[i].Do(_tracker);
                    _taskItems.Remove(_taskItems[i]);
                    isOK = true;
                }
                catch 
                {
                    isOK = false;
                }
                if (OnAfterQueueTaskItemHandler != null)
                    OnAfterQueueTaskItemHandler(this, taskItem, isOK);
                idx++;
            }
        }

        public IQueueTaskItem[] GetFailedTaskItems()
        {
            if (_taskItems == null || _taskItems.Count == 0)
                return  null;
            return _taskItems.ToArray();
        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (_taskItems != null)
            {
                _taskItems.Clear();
                _taskItems = null;
            }
        }

        #endregion
    }
}
