using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.UI.AddIn.HdService.HdDataServer;
using System.Collections;
using GeoDo.RSS.UI.AddIn.HdService.MonitorTask;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    public class HdDataMonitorNotify : IDisposable
    {
        public event Action<string> MessageSend;
        public event Action<ChangedType, dynamic[]> DataChanged;
        List<dynamic> _curProjectionfiles = new List<dynamic>();
        List<dynamic> _curBlockfiles = new List<dynamic>();
        List<dynamic> _curMosaics = new List<dynamic>();
        private HdDataProvider _dbProvider;
        private GeneralMonitoringTask2 tasks;

        public HdDataMonitorNotify()
        {
            _dbProvider = new HdDataProvider();
            _dbProvider.getProjectionsCompleted += new EventHandler<getProjectionsCompletedEventArgs>(_dbProvider_getProjectionsCompleted);
            _dbProvider.getBlocksCompleted += new EventHandler<getBlocksCompletedEventArgs>(_dbProvider_getBlocksCompleted);
            _dbProvider.getMosaicsCompleted += new EventHandler<getMosaicsCompletedEventArgs>(_dbProvider_getMosaicsCompleted);

            tasks = new GeneralMonitoringTask2(
                new Action[] { 
                    //new Action(getProjections)
                    //,new Action(getMosaics) 
                    //,new Action(getBlocks) 
                    new Action(getProjectionsAsync) 
                    ,new Action(getMosaicsAsync) 
                    ,new Action(getBlocksAsync)
                });
            tasks.ThreadExceptionEventHandler += new System.Threading.ThreadExceptionEventHandler(tasks_ThreadExceptionEventHandler);
        }

        private DateTime MonitBeginTime
        {
            get { return DateTime.Today; }
        }

        private DateTime MonitEndTime
        {
            get { return DateTime.Today.AddDays(1).AddSeconds(-1); }
        }

        private void Complete<T>(dynamic[] files, List<dynamic> _curfiles)
        {
            List<dynamic> addFiles = new List<dynamic>();
            List<dynamic> removeFiles = new List<dynamic>();
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
                    if (!ContainsFromId(_curfiles.ToArray(), file))
                        addFiles.Add(file);
                }
                foreach (T file in _curfiles)
                {
                    if (!ContainsFromId(files, file))
                        removeFiles.Add(file);
                }
                _curfiles.Clear();
                _curfiles.AddRange(files);
            }
            if (addFiles.Count != 0)
                OnDataAdded(addFiles.ToArray());
            if (removeFiles.Count != 0)
                OnDataRemoved(removeFiles.ToArray());
        }

        void tasks_ThreadExceptionEventHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            if (e.Exception != null)
                OnMessageSend(e.Exception.Message);
        }

        private void getProjections()
        {
            try
            {
                ProjectionInfo[] ps = _dbProvider.getProjections(MonitBeginTime, MonitEndTime);
                Complete<ProjectionInfo>(ps, _curProjectionfiles);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message))
                    OnMessageSend(ex.InnerException.Message);
                else
                    OnMessageSend(ex.Message);
            }
        }

        private void getProjectionsAsync()
        {
            _dbProvider.getProjectionsAsync(MonitBeginTime, MonitEndTime);
        }

        private void _dbProvider_getProjectionsCompleted(object sender, getProjectionsCompletedEventArgs e)
        {
            try
            {
                ProjectionInfo[] ps = e.Result;
                Complete<ProjectionInfo>(ps, _curProjectionfiles);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message))
                    OnMessageSend(ex.InnerException.Message);
                else
                    OnMessageSend(ex.Message);
            }
        }

        private void getBlocks()
        {
            try
            {
                BlockInfo[] infos = _dbProvider.getBlocks(MonitBeginTime, MonitEndTime);
                Complete<BlockInfo>(infos, _curBlockfiles);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message))
                    OnMessageSend(ex.InnerException.Message);
                else
                    OnMessageSend(ex.Message);
            }
        }

        private void getBlocksAsync()
        {
            _dbProvider.getBlocksAsync(MonitBeginTime, MonitEndTime);
        }

        private void _dbProvider_getBlocksCompleted(object sender, getBlocksCompletedEventArgs e)
        {
            try
            {
                Complete<BlockInfo>(e.Result, _curBlockfiles);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message))
                    OnMessageSend(ex.InnerException.Message);
                else
                    OnMessageSend(ex.Message);
            }
        }

        private void getMosaics()
        {
            try
            {
                MosaicInfo[] infos = _dbProvider.getMosaics(DateTime.Today, DateTime.Today.AddDays(1).AddSeconds(-1));
                Complete<MosaicInfo>(infos, _curMosaics);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message))
                    OnMessageSend(ex.InnerException.Message);
                else
                    OnMessageSend(ex.Message);
            }
        }

        private void getMosaicsAsync()
        {
            _dbProvider.getMosaicsAsync(DateTime.Today, DateTime.Today.AddDays(1).AddMilliseconds(-1));
        }

        private void _dbProvider_getMosaicsCompleted(object sender, getMosaicsCompletedEventArgs e)
        {
            try
            {
                MosaicInfo[] ps = e.Result;
                Complete<MosaicInfo>(ps, _curMosaics);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message))
                    OnMessageSend(ex.InnerException.Message);
                else
                    OnMessageSend(ex.Message);
            }
        }

        void OnDataRemoved(dynamic[] obj)
        {
            if (DataChanged != null)
                DataChanged(ChangedType.Remove, obj);
        }

        void OnDataAdded(dynamic[] obj)
        {
            if (DataChanged != null)
                DataChanged(ChangedType.Add, obj);
        }

        void OnMessageSend(string obj)
        {
            if (MessageSend != null)
                MessageSend(obj);
        }

        public void Start()
        {
            tasks.StartListen();
        }

        public void Stop()
        {
            tasks.StopListen();
        }

        private bool ContainsFromId(dynamic[] items, dynamic item)
        {
            if (items == null || items.Length == 0)
                return false;
            foreach (dynamic c in items)
            {
                if (item.id == c.id)
                    return true;
            }
            return false;
        }

        public void Dispose()
        {
            if (tasks != null)
                tasks.Dispose();
        }
    }
}
