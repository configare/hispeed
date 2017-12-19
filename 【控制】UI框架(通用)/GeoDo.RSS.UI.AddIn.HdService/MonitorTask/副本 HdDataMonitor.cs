using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.UI.AddIn.HdService.HdDataServer;
using System.Collections;
using GeoDo.RSS.UI.AddIn.HdService.MonitorTask;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    public class HdDataMonitorNotify_Old:IDisposable
    {
        public event Action<string> MessageSend;
        public event Action<ChangedType, dynamic[]> DataChanged;

        private GeneralMonitoringTask<HdDataServer.OrbitInfo> MoOrbitInfo = null;
        private GeneralMonitoringTask<HdDataServer.ProjectionInfo> MoProjectionInfo = null;
        private GeneralMonitoringTask<HdDataServer.MosaicInfo> MoMosaicInfo = null;
        private GeneralMonitoringTask<HdDataServer.BlockInfo> MoBlockInfo = null;

        private HdDataProvider _dbProvider;
        private GeneralMonitoringTask2 tasks;

        public HdDataMonitorNotify_Old()
        {
            _dbProvider = new HdDataProvider();
            _dbProvider.getProjectionsCompleted += new EventHandler<getProjectionsCompletedEventArgs>(_dbProvider_getProjectionsCompleted);
            _dbProvider.getBlocksCompleted += new EventHandler<getBlocksCompletedEventArgs>(_dbProvider_getBlocksCompleted);

            tasks = new GeneralMonitoringTask2(
                new Action[] { 
                    new Action(getProjections)
                    //,new Action(getMosaicsAsync) 
                });
            tasks.ThreadExceptionEventHandler += new System.Threading.ThreadExceptionEventHandler(tasks_ThreadExceptionEventHandler);
            //if (HdDataProvider.Instance.CanUse())
            //{
            //    MoProjectionInfo = new GeneralMonitoringTask<ProjectionInfo>(
            //        (items, item) => { return ContainsFromId(items, item); },
            //        () => { return getProjectionInfo(); });

            //    MoMosaicInfo = new GeneralMonitoringTask<MosaicInfo>(
            //        (items, item) => { return ContainsFromId(items, item); },
            //        () => { return getMosaicInfo(); });

            //    MoBlockInfo = new GeneralMonitoringTask<BlockInfo>(
            //        (items, item) => { return ContainsFromId(items, item); },
            //        () => { return HdDataProvider.Instance.getBlocks(DateTime.Today, DateTime.Today.AddDays(1).AddSeconds(-1)); });
                
            //    this.MoProjectionInfo.MessageSend += new Action<string>(OnMessageSend);
            //    this.MoProjectionInfo.DataAdded += new Action<HdDataServer.ProjectionInfo[]>(OnDataAdded);
            //    this.MoProjectionInfo.DataRemoved += new Action<HdDataServer.ProjectionInfo[]>(OnDataRemoved);

            //    this.MoMosaicInfo.MessageSend += new Action<string>(OnMessageSend);
            //    this.MoMosaicInfo.DataAdded += new Action<HdDataServer.MosaicInfo[]>(OnDataAdded);
            //    this.MoMosaicInfo.DataRemoved += new Action<HdDataServer.MosaicInfo[]>(OnDataRemoved);

            //    this.MoBlockInfo.MessageSend += new Action<string>(OnMessageSend);
            //    this.MoBlockInfo.DataAdded += new Action<HdDataServer.BlockInfo[]>(OnDataAdded);
            //    this.MoBlockInfo.DataRemoved += new Action<HdDataServer.BlockInfo[]>(OnDataRemoved);
            //}
        }

        void tasks_ThreadExceptionEventHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            if(e.Exception!=null)
                OnMessageSend(e.Exception.Message);
        }

        void _dbProvider_getBlocksCompleted(object sender, getBlocksCompletedEventArgs e)
        {
            Complete<BlockInfo>(e.Result, _curBfiles);
        }

        List<dynamic> _curPfiles = new List<dynamic>();
        List<dynamic> _curBfiles = new List<dynamic>();

        void _dbProvider_getProjectionsCompleted(object sender, getProjectionsCompletedEventArgs e)
        {
            try
            {
                ProjectionInfo[] ps = e.Result;
                Complete<ProjectionInfo>(ps, _curPfiles);
            }
            catch(Exception ex)
            {
                if (ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message))
                    OnMessageSend(ex.InnerException.Message);
                else
                    OnMessageSend(ex.Message);
            }
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

        private void getProjectionsAsync()
        {
            _dbProvider.getProjectionsAsync(DateTime.Today, DateTime.Today.AddDays(1).AddSeconds(-1));
        }

        private void getProjections()
        {
            try
            {
                ProjectionInfo[] ps = getProjectionInfo();
                Complete<ProjectionInfo>(ps, _curPfiles);
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
            _dbProvider.getMosaicsAsync(DateTime.Today, DateTime.Today.AddDays(1).AddSeconds(-1));
        }

        private MosaicInfo[] getMosaicInfo()
        {
            return HdDataProvider.Instance.getMosaics(DateTime.Today, DateTime.Today.AddDays(1).AddSeconds(-1));
        }

        private ProjectionInfo[] getProjectionInfo()
        {
            return _dbProvider.getProjections(DateTime.Today, DateTime.Today.AddDays(1).AddSeconds(-1));
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
            //MoOrbitInfo.StartListen();
            //MoProjectionInfo.StartListen();
            //MoMosaicInfo.StartListen();
            //MoBlockInfo.StartListen();
        }

        public void Stop()
        {
            tasks.StopListen();
            //MoOrbitInfo.StopListen();
            //MoProjectionInfo.StopListen();
            //MoMosaicInfo.StopListen();
            //MoBlockInfo.StopListen();
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
            if(tasks!=null)
                tasks.Dispose();
            //if (MoOrbitInfo != null)
            //    MoOrbitInfo.Dispose();
            //if (MoProjectionInfo != null)
            //    MoProjectionInfo.Dispose();
            //if (MoMosaicInfo != null)
            //    MoMosaicInfo.Dispose();
            //if (MoBlockInfo != null)
            //    MoBlockInfo.Dispose();
        }
    }
}
