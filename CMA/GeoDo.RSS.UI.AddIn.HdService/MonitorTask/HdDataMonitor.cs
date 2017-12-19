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
        private DateTime _monitBeginTime = DateTime.Parse(DateTime.Today.ToShortDateString()).AddHours(-8);
        private DateTime _monitEndTime = DateTime.Parse(DateTime.Today.AddDays(1).ToShortDateString()).AddSeconds(-1).AddHours(-8);

        public HdDataMonitorNotify(DateTime MonitBeginTime, DateTime MonitEndTime)
        {
            _dbProvider = new HdDataProvider();
            _monitBeginTime = MonitBeginTime;
            _monitEndTime = MonitEndTime;
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
            get { return _monitBeginTime; }
        }

        private DateTime MonitEndTime
        {
            get { return _monitEndTime; }
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

        void tasks_ThreadExceptionEventHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            try
            {
                if (e.Exception != null)
                {
                    OnMessageSend(e.Exception.Message);
                }
            }
            catch (Exception ex)
            {
                LogFactory.WriteLine("getBlocksAsync:" + ex.Message);
            }
        }

        private void getProjectionsAsync()
        {
            try
            {
                _dbProvider.getProjectionsAsync(MonitBeginTime, MonitEndTime);
            }
            catch (Exception ex)
            {
                LogFactory.WriteLine("getProjectionsAsync:" + ex.Message);
            }
        }

        private void _dbProvider_getProjectionsCompleted(object sender, getProjectionsCompletedEventArgs e)
        {
            try
            {
                ProjectionInfo[] ps = e.Result;
                Complete<ProjectionInfo>(ps, _curProjectionfiles);
                UpdateDBMessage("查询得到新的投影数据" + e.Result.Length + "条");
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
            try
            {
                _dbProvider.getBlocksAsync(MonitBeginTime, MonitEndTime);
            }
            catch (Exception ex)
            {
                LogFactory.WriteLine("getBlocksAsync:" + ex.Message);
            }
        }

        private void _dbProvider_getBlocksCompleted(object sender, getBlocksCompletedEventArgs e)
        {
            try
            {
                Complete<BlockInfo>(e.Result, _curBlockfiles);
                UpdateDBMessage("查询得到新的分幅数据" + e.Result.Length + "条");
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
            try
            {
                _dbProvider.getMosaicsAsync(_monitBeginTime, _monitEndTime);
            }
            catch (Exception ex)
            {
                LogFactory.WriteLine("getMosaicsAsync:" + ex.Message);
            }
        }

        private void _dbProvider_getMosaicsCompleted(object sender, getMosaicsCompletedEventArgs e)
        {
            try
            {
                MosaicInfo[] ps = e.Result;
                Complete<MosaicInfo>(ps, _curMosaics);
                UpdateDBMessage("查询得到新的拼接数据" + e.Result.Length + "条");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message))
                    OnMessageSend(ex.InnerException.Message);
                else
                    OnMessageSend(ex.Message);
            }
        }

        private void UpdateDBMessage(string msg)
        {
            OnMessageSend("最新一次访问数据库时间" + DateTime.Now.ToLongTimeString() + msg);
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

        public void ImmediatelyStart()
        {
            tasks.ImmediatelyStartListen();
        }

        public void Stop()
        {
            tasks.StopListen();
        }

        public void Dispose()
        {
            if (tasks != null)
                tasks.Dispose();
        }
    }
}
