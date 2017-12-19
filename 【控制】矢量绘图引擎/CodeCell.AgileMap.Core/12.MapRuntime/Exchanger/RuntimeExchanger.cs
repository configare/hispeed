using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Threading;
using System.ComponentModel;
using System.IO;
using System.Collections;
using CodeCell.Bricks.Runtime;


namespace CodeCell.AgileMap.Core
{
    internal class RuntimeExchanger:IDisposable,IRuntimeExchanger
    {
        private IFeatureRenderEnvironment _environment = null;
        private ILayerContainer _layerContainer = null;
        private bool _enabled = true;
        private List<IFeatureLayer> _gridableLayers = new List<IFeatureLayer>();
        private bool _isChecking = false;
        private IMapRefresh _mapRefresh = null;
        private int _finishedLayerCount = 0;
        private int cstReadingTaskCount = 0;
        private int _readingTaskCount = 0;
        private Stack<TaskState> _waitingReadingTaskes = new Stack<TaskState>();
        private List<int> _tempGridNos = new List<int>();
        private bool _autoRefreshWhileFinishOneGrid = true;
        private EventHandler _idleEventHandler = null;
        private IAsyncDataArrivedNotify _asyncDataArrivedNotify;

        public RuntimeExchanger(IFeatureRenderEnvironment environment,
            IMapRefresh mapRefresh,ILayerContainer featureLayerContainer,
            IAsyncDataArrivedNotify asyncDataArrivedNotify)
        {
            _mapRefresh = mapRefresh;
            _environment = environment;
            _layerContainer = featureLayerContainer;
            _asyncDataArrivedNotify = asyncDataArrivedNotify;
            _idleEventHandler = new EventHandler(Application_Idle);
            Application.Idle += _idleEventHandler;
            _environment.OnTransformChanged += new OnTransformChangedHandler(TransformChanged);
            _layerContainer.OnAddFeatureLayer += new OnAddLayerHandler(OnAddLayer);
            _layerContainer.OnRemoveFeatureLayer += new OnRemoveLayerHandler(OnRemoveLayer);
            cstReadingTaskCount = Environment.ProcessorCount;
        }

        public void RemoveLayer(IFeatureLayer lyr)
        {
            if (lyr != null && _gridableLayers.Contains(lyr))
                _gridableLayers.Remove(lyr);
        }

        public IAsyncDataArrivedNotify AsyncDataArrivedNotify
        {
            get { return _asyncDataArrivedNotify; }
            set { _asyncDataArrivedNotify = value; }
        }

        public bool AutoRefreshWhileFinishOneGrid
        {
            get { return _autoRefreshWhileFinishOneGrid; }
            set { _autoRefreshWhileFinishOneGrid = value; }
        }

        public int MaxTaskCount
        {
            get { return cstReadingTaskCount; }
            set 
            {
                if (value < 1)
                    return;
                cstReadingTaskCount = value;
            }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set 
            {
                if (_enabled != value)
                {
                    _enabled = value; 
                }
            }
        }

        void OnAddLayer(object sender, ILayer layer)
        {
            if (layer == null)
                return;
            if (layer.Class == null)
            {
                Log.WriterInfo("RuntimeExchanger", "OnAddLayer()", "矢量层\"" + layer.Name + "\"FeatureClass为空,无法加载到网格缓存队列中。");
                return;
            }
            if (!_gridableLayers.Contains(layer as IFeatureLayer))
            {
                IFeatureLayer flyr = layer as IFeatureLayer;
                if (flyr != null)
                {
                    _gridableLayers.Add(flyr);
                    ReStatFinishedCount();
                }
            }
        }

        void OnRemoveLayer(object sender, ILayer layer)
        {
            if (layer == null)
                return;
            if (_gridableLayers.Contains(layer as IFeatureLayer))
            {
                _gridableLayers.Remove(layer as IFeatureLayer);
                ReStatFinishedCount();
            }
        }

        private void ReStatFinishedCount()
        {
            _finishedLayerCount = 0;
            foreach (IFeatureLayer lyr in _gridableLayers)
            {
                if (lyr == null)
                    continue;
                IFeatureClass fetclass = lyr.Class as IFeatureClass;
                if ((fetclass.DataSource as IFeatureDataSource).ReadIsFinished)
                    _finishedLayerCount++;
            }
        }

        void TransformChanged(object sender, Matrix newMatrix, Matrix oldMatrix)
        {
            BeginCheckUnloadedGrids();
            TryStartReadingTask();
        }

        void Application_Idle(object sender, EventArgs e)
        {
            TryStartReadingTask();
            BeginCheckUnloadedGrids();
        }

        public void RaiseCheckingFromOutside()
        {
            BeginCheckUnloadedGrids();
            TryStartReadingTask();
        }

        private bool _tryStartReadingTasking = false;
        private void TryStartReadingTask()
        {
            if (_tryStartReadingTasking)
                return;
            _tryStartReadingTasking = true;
            while (_waitingReadingTaskes.Count > 0 && _readingTaskCount < cstReadingTaskCount)
            {
                TaskState ts = _waitingReadingTaskes.Pop();
                if (ThreadPool.QueueUserWorkItem(new WaitCallback(DoReadGrid), ts))
                {
                    Interlocked.Increment(ref _readingTaskCount);
                }
            }
            _tryStartReadingTasking = false;
        }

        void BeginCheckUnloadedGrids()
        {
            //前一次检查还未结束
            if (_isChecking)
                return;
            _isChecking = true;
            try
            {
                if (!_enabled)
                    return;
                //已经全部加载
                if (_finishedLayerCount == _gridableLayers.Count)
                    return;
                //如果还有读取任务则不做检查
                if (_waitingReadingTaskes.Count > 0)
                    return;
                TryCheckUnloadedGrids();
            }
            finally 
            {
                _isChecking = false;
            }
        }

        private void TryCheckUnloadedGrids()
        {
            Envelope rect = _environment.ExtentOfProjectionCoord;
            if (rect == null || _gridableLayers == null)
                return;
            int layerCount = _gridableLayers.Count;
            if (layerCount == 0)
                return;
            for (int i = 0; i < layerCount; i++)
            {
                if (i > _gridableLayers.Count - 1)
                    continue;
                IFeatureLayer lyr = _gridableLayers[i];
                if (lyr == null)
                    continue;
                if (lyr.Disposed || lyr.Class.Disposed)
                    continue;
                //如果图层未初始化
                if (!lyr.IsReady)
                    continue;
                //如果图层当前不可见,则不触发读取操作
                if (!(lyr as ILayerDrawable).Visible)
                    continue;
                //在不显示的比例尺下不触发读取操作
                if (!lyr.VisibleAtScale(_environment.CurrentScale))
                    continue;
                //正在鼠标交互时停止检查
                //if (_environment.IsMouseBusy)
                //    return;
                TryCheckUnloadedGrids(lyr,rect);
            }
        }

        private void TryCheckUnloadedGrids(IFeatureLayer layer,Envelope currentExtent)
        {
            if (layer == null || currentExtent == null)
                return;
            IFeatureClass fetclass = layer.Class as IFeatureClass;
            //所有网格已经全部在内存，无需读取
            if ((fetclass.DataSource as IFeatureDataSource).ReadIsFinished)
                return;
            if ((fetclass.DataSource as IFeatureDataSource).GridStateIndicator.GetUnFlagedCount()  ==0)
            {
                TryEndRead(fetclass.DataSource as IGridReader,fetclass.Name);
                _finishedLayerCount++;
                return;
            }
            Envelope validExtent = currentExtent.IntersectWith(fetclass.FullEnvelope);
            //
            if (validExtent == null)
                return;
            //将可使区域转换为数据源的坐标类型
            ProjectEnvelope(validExtent,fetclass);
            //计算未读出的网格编号
            _tempGridNos.Clear();
            (fetclass.DataSource as IFeatureDataSource).GridStateIndicator.GetDiffGridNos(validExtent, _tempGridNos);
            if (_tempGridNos == null || _tempGridNos.Count == 0)
                return;
            //往加载网格队列中添加任务
            for (int i = 0; i < _tempGridNos.Count; i++)
            {
                (fetclass.DataSource as IFeatureDataSource).GridStateIndicator.Flaging(_tempGridNos[i]);
                _waitingReadingTaskes.Push(new TaskState(fetclass, _tempGridNos[i]));
            }
        }

        private void DoReadGrid(object state)
        {
            TaskState taskstate = state as TaskState;
            if (taskstate.FeatureClass.Disposed || taskstate.FeatureClass.DataSource == null)
                return;
            bool isLoaded = false;
            try
            {
                IGridReader reader = taskstate.FeatureClass.DataSource as IGridReader;
                IGrid grid = null;
                if (!reader.IsReady)
                    reader.BeginRead();
                grid = reader.ReadGrid(taskstate.GridNo);
                if (grid != null)
                {
                    taskstate.FeatureClass.AddGrid(grid);
                    isLoaded = true;
                }
            }
            catch (Exception ex)
            {
                Log.WriterException("RuntimeExchanger", "DoReadGrid", ex);
            }
            finally
            {
                if (taskstate.FeatureClass.DataSource != null)
                {
                    if (isLoaded)
                        (taskstate.FeatureClass.DataSource as IFeatureDataSource).GridStateIndicator.Flaged(taskstate.GridNo);
                    else//read grid is failed,next time to read
                        (taskstate.FeatureClass.DataSource as IFeatureDataSource).GridStateIndicator.UnFlaged(taskstate.GridNo);
                }
                Interlocked.Decrement(ref _readingTaskCount);
                if (isLoaded && _autoRefreshWhileFinishOneGrid)
                    _mapRefresh.ReRender();
                if (_asyncDataArrivedNotify != null)
                    _asyncDataArrivedNotify.SomeDataIsArrived();
                //int n = taskstate.FeatureClass.Grids != null ? taskstate.FeatureClass.Grids.Length : 0;
                //Console.WriteLine("Finished:" + n.ToString());
            }
        }

        private void ProjectEnvelope(Envelope validExtent, IFeatureClass fetclass)
        {
            if (validExtent == null || fetclass.OriginalCoordinateType == enumCoordinateType.Projection)
                return;
            ShapePoint[] pts = validExtent.Points;
            _environment.CoordinateTransform.PrjCoord2GeoCoord(pts);
            //
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;
            foreach (ShapePoint pt in pts)
            {
                if (pt.X < minX)
                    minX = pt.X;
                if (pt.Y < minY)
                    minY = pt.Y;
                if (pt.X > maxX)
                    maxX = pt.X;
                if (pt.Y > maxY)
                    maxY = pt.Y;
            }
            validExtent.MinX = minX;
            validExtent.MinY = minY;
            validExtent.MaxX = maxX;
            validExtent.MaxY = maxY;
        }

        private void TryEndRead(IGridReader reader,string name)
        {
            if (reader == null)
                return;
            try
            {
                reader.EndRead();
            }
            catch(Exception ex) 
            {
                Log.WriterException("RuntimeExchanger", "TryEndRead", ex);
            }
        }

        private class TaskState
        {
            public IFeatureClass FeatureClass = null;
            public int GridNo = -1;

            public TaskState(IFeatureClass fetclass, int gridNo)
            {
                FeatureClass = fetclass;
                GridNo = gridNo;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _enabled = false;
            _gridableLayers.Clear();
            //_gridableLayers = null;
            Application.Idle -= _idleEventHandler;

            _environment.OnTransformChanged -= new OnTransformChangedHandler(TransformChanged);
            _layerContainer.OnAddFeatureLayer -= new OnAddLayerHandler(OnAddLayer);
            _layerContainer.OnRemoveFeatureLayer -= new OnRemoveLayerHandler(OnRemoveLayer);
        }

        #endregion
    }
}
