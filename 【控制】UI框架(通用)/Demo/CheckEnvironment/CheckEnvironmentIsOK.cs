using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.AppEnv
{
    public class CheckEnvironmentIsOK:IDisposable,IProgressTracker
    {
        private List<IQueueTaskItem> _taskItems = null;
        private IStartProgress _startProgress = null; 

        public CheckEnvironmentIsOK(IStartProgress startProgress)
        {
            _startProgress = startProgress;
            _taskItems = new List<IQueueTaskItem>();
            _taskItems.Add(new MustFileIsExistChecker());
            _taskItems.Add(new MustDirIsExistChecker());
            //_taskItems.Add(new AppArgsIsSetedChecker());
        }

        public bool Check()
        {
            foreach (IQueueTaskItem task in _taskItems)
            {
                task.Do(this);
                IStartChecker c = task as IStartChecker;
                if (!c.IsOK)
                {
                    ExceptionHandler.ShowExceptionWnd(c.Exception, c.Message);
                    return c.IsCanContinue;
                }
            }
            return true;
        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (_taskItems != null)
            {
                _taskItems.Clear();
            }
        }

        #endregion

        #region IProgressTracker 成员

        public bool IsBusy
        {
            get { return false; }
        }

        public void StartTracking(string text, int estimateTotalTime)
        {
            _startProgress.PrintStartInfo(text);
        }

        public void StartTracking(string text)
        {
            _startProgress.PrintStartInfo(text);
        }

        public void StopTracking()
        {
            
        }

        public void Tracking(string text, int currentTime)
        {
            _startProgress.PrintStartInfo(text);
        }

        public void Tracking(string text)
        {
            _startProgress.PrintStartInfo(text);
        }

        #endregion
    }
}
