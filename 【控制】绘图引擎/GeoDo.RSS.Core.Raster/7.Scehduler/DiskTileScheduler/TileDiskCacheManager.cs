using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class ReadTask
    {
    }

    public class BandLevelTask : ReadTask
    {
        public int BandNo;
        public LevelId Level;
        public string FileName;
    }

    public class TileTask : ReadTask
    {
        public int BandNo;
        public TileId Tile;
        public string Key;
    }

    public class TileDiskCacheManager : ITileDiskCacheManager
    {
        private LevelTileComputer _tileComputer = null;
        private EventHandler _idleHandler = null;
        private Queue<BandLevelTask> _bandLevelQueue = new Queue<BandLevelTask>();
        private BackgroundWorker _tileReadWorker = null;
        private Stack<ReadTask> _taskStack = new Stack<ReadTask>();
        private bool _isStarted = false;
        private bool _workerIsFinisehd = false;
        private CacheNameHelper _cacheNameHelper = null;
        private string _cacheFolder = null;
        private TileReadedCallback _tileReadedCallback;
        private int _dataTypeSize;
        private int _tileSize;
        private IRasterDataProvider _dataProvider;
        //用于BandLevelTask长任务暂停
        private bool _isDoingLargerTask = false;
        private bool _needPauseLargerTask = false;
        //需要异步通知内存缓存的瓦片
        private List<string> _needAsyncNotifyTasks = new List<string>();
        //磁盘缓存块文件流对象
        private Dictionary<string, FileStream> _readFileStreams = new Dictionary<string, FileStream>();
        private int _tileBufferSize;

        public TileDiskCacheManager(IRasterDataProvider dataProvider, string cacheDir, string fname, LevelTileComputer tileComputer, TileReadedCallback callback)
        {
            _dataProvider = dataProvider;
            _dataTypeSize = DataTypeHelper.SizeOf(_dataProvider.DataType);
            _tileComputer = tileComputer;
            _tileSize = _tileComputer.TileSize;
            _tileBufferSize = _tileSize * _tileSize * _dataTypeSize;
            _tileReadedCallback = callback;
            _cacheNameHelper = new CacheNameHelper(cacheDir);
            TryCreateCacheFolder(fname);
            AttachIdleEvents();
            CreateTileReadWorker();
        }

        private void TryCreateCacheFolder(string fname)
        {
            _cacheFolder = _cacheNameHelper.GetCacheDirByFilename(fname);
            if (!Directory.Exists(_cacheFolder))
                Directory.CreateDirectory(_cacheFolder);
            else
            {
                TryLoadCacheFromDisk(_cacheFolder);
            }
        }

        private void TryLoadCacheFromDisk(string cacheFolder)
        {
            string[] fnames = Directory.GetFiles(cacheFolder, "*.cache");
            if (fnames == null || fnames.Length == 0)
                return;
            foreach (string fname in fnames)
            {
                int bandNo, levelNo;
                _cacheNameHelper.ParseCacheFilename(Path.GetFileNameWithoutExtension(fname), out bandNo, out levelNo);
                if (bandNo == -1 || levelNo == -1)
                    continue;
                FileStream fs = GetFileStreamForRead(fname, levelNo);
                if (fs != null)
                {
                    _readFileStreams.Add(_cacheNameHelper.GetResourceKey(bandNo, levelNo), fs);
                }
            }
        }

        private FileStream GetFileStreamForRead(string fname, int levelNo)
        {
            FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (fs.Length == _tileComputer.Levels[levelNo].TileCount * _tileBufferSize)
                return fs;
            fs.Dispose();
            return null;
        }

        private void CreateTileReadWorker()
        {
            _tileReadWorker = new BackgroundWorker();
            _tileReadWorker.WorkerSupportsCancellation = true;
            _tileReadWorker.DoWork += new DoWorkEventHandler(_tileReadWorker_DoWork);
            _tileReadWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_tileReadWorker_RunWorkerCompleted);
        }

        void _tileReadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _workerIsFinisehd = true;
        }

        private void AttachIdleEvents()
        {
            _idleHandler = new EventHandler(Application_Idle);
            Application.Idle += _idleHandler;
        }

        public bool IsRunning
        {
            get { return _isStarted && !_workerIsFinisehd; }
        }

        public void Start()
        {
            if (_isStarted)
                return;
            lock (_tileReadWorker)
            {
                if (!_tileReadWorker.IsBusy)
                    _tileReadWorker.RunWorkerAsync();
            }
            _isStarted = true;
            _workerIsFinisehd = false;
        }

        public void Stop()
        {
            _idleHandler -= _idleHandler;
            _tileReadWorker.CancelAsync();
            while (!_workerIsFinisehd)
                Thread.Sleep(10);
            _isStarted = false;
        }

        void _tileReadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
        cntDo:
            while (_taskStack.Count > 0)
            {
                ReadTask task = _taskStack.Pop();
                if (task != null)
                    DoTask(task);
            }
            if (_bandLevelQueue.Count > 0)
            {
                //从等待队列中拿出任务
                Application_Idle(null, null);
                goto cntDo;
            }
            //队列中已经没有任务,异步读取线程退出
            _workerIsFinisehd = true;
        }

        private void DoTask(ReadTask task)
        {
            if (task is TileTask)
                DoTileTask(task as TileTask);
            else
            {
                //DoBandLevelTask(task as BandLevelTask);
            }
        }

        private unsafe void DoBandLevelTask(BandLevelTask bandLevelTask)
        {
            _isDoingLargerTask = true;
            try
            {
                string fname = _cacheNameHelper.GetCacheFilename(_cacheFolder, bandLevelTask.BandNo, bandLevelTask.Level.No);
                using (FileStream fs = new FileStream(fname, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        IRasterBand band = _dataProvider.GetRasterBand(bandLevelTask.BandNo);
                        int tCount = bandLevelTask.Level.Tiles.Length;
                        TileId[] tiles = bandLevelTask.Level.Tiles;
                        TileId tile;
                        LevelId lv = bandLevelTask.Level;
                        byte[] data = new byte[_tileBufferSize];
                        fixed (byte* ptr = data)
                        {
                            IntPtr buffer = new IntPtr(ptr);
                            for (int i = 0; i < tCount; i++)
                            {
                                tile = tiles[i];
                                band.Read(
                                    (int)(_tileSize * tile.Col / lv.Scale),
                                    (int)(_tileSize * tile.Row / lv.Scale),
                                    (int)(tile.Width / lv.Scale),
                                    (int)(tile.Height / lv.Scale),
                                    buffer, _dataProvider.DataType, tile.Width, tile.Height);
                                bw.Write(data);
                                //
                                if (_needPauseLargerTask)
                                {
                                    Console.WriteLine("需要暂停长任务。");
                                    _needPauseLargerTask = false;
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                _isDoingLargerTask = false;
            }
        }

        private unsafe void CheckIsNeedAsyncNotify(int bandNo, TileId tile, byte[] data)
        {
            string key = GetTileTaskKey(bandNo, tile);
            if (_needAsyncNotifyTasks.Contains(key))
            {
                TileData tileData = new TileData();
                tileData.BandNo = bandNo;
                tileData.Tile = tile;
                tileData.Data = new byte[data.Length];
                fixed (byte* srcptr = data, dstptr = (byte[])tileData.Data)
                {
                    IntPtr src = new IntPtr(srcptr);
                    IntPtr dst = new IntPtr(dstptr);
                    WinAPI.MemoryCopy(src, dst, data.Length);
                }
                if (_tileReadedCallback != null)
                    _tileReadedCallback.BeginInvoke(tileData, null, null);
                _needAsyncNotifyTasks.Remove(key);
            }
        }

        private void DoTileTask(TileTask tileTask)
        {
            TileData data = TryReadFromCacheFile(tileTask);
            if (data != null)
            {
                if (_tileReadedCallback != null)
                    _tileReadedCallback.BeginInvoke(data, null, null);
            }
            else
            {
                /*
                 * 缓存文件不存在,选择以下方式：
                 * 1.直接从IRasterDataProvider中读取
                 * 2.加入待通知队列,相应的BandLevelTask完成后异步通知内存缓存管理对象
                 */
                //1.
                DirectTileFromDataProvider(tileTask);
                //2.
                //_needAsyncNotifyTasks.Add(tileTask.Key);
                //
                //Console.WriteLine(tileTask.Tile.ToString() + ",缓存文件不存在。");
            }
        }

        public void LoadSync(int bandNo, LevelId level)
        {
            if (level.TileCount == 0 || level.Tiles == null || level.Tiles.Length == 0)
                return;
            for (int i = 0; i < level.Tiles.Length; i++)
            {
                TileTask task = new TileTask();
                task.Tile = level.Tiles[i];
                task.BandNo = bandNo;
                task.Key = GetTileTaskKey(bandNo, level.Tiles[i]);
                DoTileTask(task);
            }
        }

        private unsafe void DirectTileFromDataProvider(TileTask tileTask)
        {
            byte[] data = new byte[_tileBufferSize];
            fixed (byte* ptr = data)
            {
                IntPtr buffer = new IntPtr(ptr);
                TileId tile = tileTask.Tile;
                LevelId lv = _tileComputer.Levels[tile.LevelNo];
                _dataProvider.GetRasterBand(tileTask.BandNo).Read(
                        (int)(_tileSize * tile.Col / lv.Scale),
                        (int)(_tileSize * tile.Row / lv.Scale),
                        (int)(tile.Width / lv.Scale),
                        (int)(tile.Height / lv.Scale),
                    buffer, _dataProvider.DataType, tile.Width, tile.Height);
                //
                TileData tileData = new TileData();
                tileData.BandNo = tileTask.BandNo;
                tileData.Tile = tile;
                tileData.Data = data;
                _tileReadedCallback(tileData);
            }
        }

        private TileData TryReadFromCacheFile(TileTask tileTask)
        {
            string fname = _cacheNameHelper.GetCacheFilename(_cacheFolder, tileTask.BandNo, tileTask.Tile.LevelNo);
            if (!File.Exists(fname))
                return null;
            int offset = tileTask.Tile.Index * _tileSize * _tileSize * _dataTypeSize;
            FileStream fs = null;
            //
            string key = _cacheNameHelper.GetResourceKey(tileTask.BandNo, tileTask.Tile.LevelNo);
            if (_readFileStreams.ContainsKey(key))
                fs = _readFileStreams[key];
            else
            {
                fs = new FileStream(fname, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                _readFileStreams.Add(key, fs);
            }
            //要请求的块还未写入文件
            if (fs.Length < offset)
                return null;
            //
            fs.Seek(offset, SeekOrigin.Begin);
            byte[] buffer = new byte[_tileSize * _tileSize * _dataTypeSize];
            fs.Read(buffer, 0, buffer.Length);
            //
            TileData data = new TileData();
            data.BandNo = tileTask.BandNo;
            data.Data = buffer;
            data.Tile = tileTask.Tile;
            return data;
        }

        void Application_Idle(object sender, EventArgs e)
        {
            //return;
            if (_bandLevelQueue.Count > 0)
            {
                BandLevelTask task = _bandLevelQueue.Dequeue();
                task.FileName = _cacheNameHelper.GetCacheFilename(_cacheFolder, task.BandNo, task.Level.No);
                //将任务插入到读取栈
                _taskStack.Push(task);
            }
            else
            {
                if (_workerIsFinisehd)
                {
                    _isStarted = false;
                    Start();
                }
            }
        }

        public void Request(int bandNo, TileId[] tiles)
        {
            if (tiles == null)
                return;
            int count = tiles.Length;
            for (int i = 0; i < count; i++)
            {
                TileTask task = new TileTask();
                task.Tile = tiles[i];
                task.BandNo = bandNo;
                task.Key = GetTileTaskKey(bandNo, tiles[i]);
                //插入读取队列
                _taskStack.Push(task);
            }
            //重新启动读取线程
            if (_workerIsFinisehd)
            {
                _isStarted = false;
                Start();
            }
            if (_isDoingLargerTask)
                _needPauseLargerTask = true;
        }

        public void Enqueue(int bandNo, LevelId level)
        {
            string key = _cacheNameHelper.GetResourceKey(bandNo, level.No);
            if (_readFileStreams.ContainsKey(key))
                return;
            BandLevelTask task = new BandLevelTask();
            task.BandNo = bandNo;
            task.Level = level;
            _bandLevelQueue.Enqueue(task);
        }

        private string GetTileTaskKey(int bandNo, TileId tile)
        {
            return string.Format("{0}_{1}_{2}_{3}", bandNo, tile.LevelNo, tile.Row, tile.Col);
        }

        public void Dispose()
        {
            Stop();
            Application.Idle -= _idleHandler;
            _bandLevelQueue.Clear();
            //
            _taskStack.Clear();
            //
            if (_readFileStreams != null)
            {
                foreach (FileStream fs in _readFileStreams.Values)
                    fs.Dispose();
                _readFileStreams.Clear();
                _readFileStreams = null;
            }
        }
    }
}
