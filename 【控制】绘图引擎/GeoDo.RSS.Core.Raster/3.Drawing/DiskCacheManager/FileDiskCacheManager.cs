using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.DrawEngine;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class FileDiskCacheManager : IFileDiskCacheManager, IDisposable
    {
        protected IRasterDataProvider _dataProvider;
        protected bool _isInsideDataProvider = true;
        protected string _fDir;
        protected TileSetting _tileSetting;
        private byte[] _tileBuffer;
        private GCHandle _tileBufferHandle;
        private int _dataTypeSize;
        private List<long> _cachedFiles = new List<long>();

        public FileDiskCacheManager(TileSetting tileSetting, string fName, string fDir)
        {
            _tileSetting = tileSetting;
            _dataProvider = GeoDataDriver.Open(fName) as IRasterDataProvider;
            _fDir = fDir;
            _dataTypeSize = DataTypeHelper.SizeOf(_dataProvider.DataType);
            _tileBuffer = new byte[_tileSetting.TileSize * _tileSetting.TileSize * _dataTypeSize];
            _tileBufferHandle = GCHandle.Alloc(_tileBuffer, GCHandleType.Pinned);
            LoadCachedFiles();
        }

        public FileDiskCacheManager(TileSetting tileSetting, IRasterDataProvider dataProvider, string fDir)
        {
            _tileSetting = tileSetting;
            _isInsideDataProvider = false;
            _dataProvider = dataProvider;
            _fDir = fDir;
            _dataTypeSize = DataTypeHelper.SizeOf(_dataProvider.DataType);
            _tileBuffer = new byte[_tileSetting.TileSize * _tileSetting.TileSize * _dataTypeSize];
            _tileBufferHandle = GCHandle.Alloc(_tileBuffer, GCHandleType.Pinned);
            LoadCachedFiles();
        }

        private void LoadCachedFiles()
        {
            //string[] fnames = Directory.GetFiles(_fDir, "*.raw");
            //if (fnames == null || fnames.Length == 0)
            //    return;
            //foreach (string f in fnames)
            //{
            //    _cachedFiles.Add(long.Parse(Path.GetFileNameWithoutExtension(f)));
            //}
        }

        public string CacheDir
        {
            get { return _fDir; }
        }

        public void Start()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            CreateEmptyLevelBuffer();
            //CreateEmptyTileBuffer();
            //CreateBufferByLevel();
            //CreateBufferByTile();
            //CreateBufferByLevelWholeFile();
            //TestReadFromLevelBuffer();
            //TestReadFromTileBuffer();
            //TestReadFromLevelBufferWhole();
            sw.Stop();
            Console.WriteLine("Lost time : " + sw.ElapsedMilliseconds.ToString());
        }

        private void CreateEmptyTileBuffer()
        {
            int minLevel = 1;
            TileComputer tileComputer = new TileComputer(_tileSetting.TileSize, _tileSetting.SampleRatio);
            LevelDef[] levels = tileComputer.GetLevelDefs(_dataProvider.Width, _dataProvider.Height);
            int levelCount = levels.Length;
            byte[] buffer = new byte[_tileSetting.TileSize * _tileSetting.TileSize * _dataTypeSize];
            for (int i = levelCount - 1; i >= minLevel; i--)
            {
                for (int b = 1; b <= _dataProvider.BandCount; b++)
                {
                    TileIdentify[] ts = levels[i].TileIdentities;
                    foreach (TileIdentify t in ts)
                    {
                        string fName = GetTileBufferFileName(t, b) + "_" + t.Width.ToString() + "_" + t.Height + ".RAW";
                        using (FileStream fs = new FileStream(fName, FileMode.Create))
                        {
                            fs.SetLength(t.Width * t.Height * _dataTypeSize);
                        }
                    }
                }
            }
        }

        private void CreateEmptyLevelBuffer()
        {
            int minLevel = 1;
            TileComputer tileComputer = new TileComputer(_tileSetting.TileSize, _tileSetting.SampleRatio);
            LevelDef[] levels = tileComputer.GetLevelDefs(_dataProvider.Width, _dataProvider.Height);
            int levelCount = levels.Length;
            byte[] buffer = new byte[_tileSetting.TileSize * _tileSetting.TileSize * _dataTypeSize];
            for (int i = levelCount - 1; i >= minLevel; i--)
            {
                for (int b = 1; b <= _dataProvider.BandCount; b++)
                {
                    string fName = GetLevelBufferFileName(b, levels[i]);
                    using (FileStream fs = new FileStream(fName, FileMode.Create))
                    {
                        fs.SetLength((long)levels[i].Size.Width * (long)levels[i].Size.Height * (long)_dataTypeSize);
                    }
                }
            }
        }

        private void TestReadFromLevelBufferWhole()
        {
            int idx = 0;
            int minLevel = 1;
            TileComputer tileComputer = new TileComputer(_tileSetting.TileSize, _tileSetting.SampleRatio);
            LevelDef[] levels = tileComputer.GetLevelDefs(_dataProvider.Width, _dataProvider.Height);
            int levelCount = levels.Length;
            byte[] buffer = new byte[_tileSetting.TileSize * _tileSetting.TileSize * _dataTypeSize];
            for (int i = levelCount - 1; i >= minLevel; i--)
            {
                for (int b = 1; b <= _dataProvider.BandCount; b++)
                {
                    string fName = GetLevelBufferFileNameWhole(b);
                    using (FileStream fs = new FileStream(fName, FileMode.Open))
                    {
                        TileIdentify[] ts = levels[i].TileIdentities;
                        foreach (TileIdentify t in ts)
                        {
                            long offset = GetOffsetByLevelBufferWhole(levels, i, t);
                            fs.Seek(offset, SeekOrigin.Begin);
                            fs.Read(buffer, 0, t.Width * t.Height * _dataTypeSize);
                            //Console.WriteLine((idx++).ToString());
                        }
                    }
                }
            }
        }

        private long GetOffsetByLevelBufferWhole(LevelDef[] levels, int iLevel, TileIdentify t)
        {
            int levelCount = levels.Length;
            long offset = 0;
            LevelDef lv;
            for (int i = levelCount - 1; i >= 0; i--)
            {
                if (i == iLevel)
                    break;
                lv = levels[i];
                offset = lv.Size.Width * lv.Size.Height * _dataTypeSize + t.BeginRow * lv.Size.Width + t.BeginCol;
            }
            return offset;
        }

        private void TestReadFromTileBuffer()
        {
            int idx = 0;
            int minLevel = 1;
            TileComputer tileComputer = new TileComputer(_tileSetting.TileSize, _tileSetting.SampleRatio);
            LevelDef[] levels = tileComputer.GetLevelDefs(_dataProvider.Width, _dataProvider.Height);
            int levelCount = levels.Length;
            byte[] buffer = new byte[_tileSetting.TileSize * _tileSetting.TileSize * _dataTypeSize];
            for (int i = levelCount - 1; i >= minLevel; i--)
            {
                for (int b = 1; b <= _dataProvider.BandCount; b++)
                {
                    TileIdentify[] ts = levels[i].TileIdentities;
                    foreach (TileIdentify t in ts)
                    {
                        string fName = GetTileBufferFileName(t, b) + "_" + t.Width.ToString() + "_" + t.Height + ".RAW";
                        using (FileStream fs = new FileStream(fName, FileMode.Open))
                        {
                            //buffer = File.ReadAllBytes(fName);
                            fs.Read(buffer, 0, t.Width * t.Height * _dataTypeSize);
                        }
                        //Console.WriteLine((idx++).ToString());
                    }
                }
            }
        }

        private void TestReadFromLevelBuffer()
        {
            int idx = 0;
            int minLevel = 1;
            TileComputer tileComputer = new TileComputer(_tileSetting.TileSize, _tileSetting.SampleRatio);
            LevelDef[] levels = tileComputer.GetLevelDefs(_dataProvider.Width, _dataProvider.Height);
            int levelCount = levels.Length;
            byte[] buffer = new byte[_tileSetting.TileSize * _tileSetting.TileSize * _dataTypeSize];
            for (int i = levelCount - 1; i >= minLevel; i--)
            {
                for (int b = 1; b <= _dataProvider.BandCount; b++)
                {
                    string fName = GetLevelBufferFileName(b, levels[i]);
                    using (FileStream fs = new FileStream(fName, FileMode.Open))
                    {
                        TileIdentify[] ts = levels[i].TileIdentities;
                        foreach (TileIdentify t in ts)
                        {
                            fs.Seek(t.BeginRow * t.Width + t.BeginCol, SeekOrigin.Begin);
                            fs.Read(buffer, 0, t.Width * t.Height * _dataTypeSize);
                            //Console.WriteLine((idx++).ToString());
                        }
                    }
                }
            }
        }

        private void CreateBufferByTile()
        {
            int[] defaultBandNos, otherBandNos;
            GetBandNos(out defaultBandNos, out otherBandNos);
            TileComputer tileComputer = new TileComputer(_tileSetting.TileSize, _tileSetting.SampleRatio);
            LevelDef[] levels = tileComputer.GetLevelDefs(_dataProvider.Width, _dataProvider.Height);
            int levelCount = levels.Length;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            CreateBufferByTile(levels, 1, defaultBandNos);
            CreateBufferByTile(levels, 1, otherBandNos);
            sw.Stop();
            Console.WriteLine("Lost time : " + sw.ElapsedMilliseconds.ToString());
        }

        private void CreateBufferByTile(LevelDef[] levels, int minLevel, int[] bandNos)
        {
            if (bandNos == null || bandNos.Length == 0)
                return;
            int levelCount = levels.Length;
            for (int i = levelCount - 1; i >= minLevel; i--)
            {
                foreach (int b in bandNos)
                {
                    CreateBufferByTile(levels[i], b);
                }
            }
        }

        private void CreateBufferByTile(LevelDef levelDef, int bandNo)
        {
            TileIdentify[] tiles = levelDef.TileIdentities;
            if (tiles == null || tiles.Length == 0)
                return;
            byte[] buffer = new byte[_tileSetting.TileSize * _tileSetting.TileSize * _dataTypeSize];
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            IntPtr ptr = handle.AddrOfPinnedObject();
            float scale = levelDef.Scale;
            try
            {
                int oBeginRow, oBeginCol, oWidth, oHeight;
                IRasterBand raster = _dataProvider.GetRasterBand(bandNo);
                foreach (TileIdentify tile in tiles)
                {
                    string fName = GetTileBufferFileName(tile, bandNo) + "_" + tile.Width.ToString() + "_" + tile.Height + ".RAW";
                    if (File.Exists(fName) && (new FileInfo(fName)).Length == tile.Width * tile.Height * _dataTypeSize)
                        continue;
                    oBeginRow = (int)(tile.BeginRow / scale);
                    oBeginCol = (int)(tile.BeginCol / scale);
                    oWidth = (int)(tile.Width / scale);
                    oHeight = (int)(tile.Height / scale);
                    raster.Read(oBeginCol, oBeginRow, oWidth, oHeight, ptr, raster.DataType, tile.Width, tile.Height);
                    using (FileStream fs = new FileStream(fName, FileMode.Create))
                    {
                        fs.Write(buffer, 0, tile.Width * tile.Height * _dataTypeSize);
                    }
                }
            }
            finally
            {
                handle.Free();
            }
        }

        private void CreateBufferByLevelWholeFile()
        {
            int[] defaultBandNos, otherBandNos;
            GetBandNos(out defaultBandNos, out otherBandNos);
            TileComputer tileComputer = new TileComputer(_tileSetting.TileSize, _tileSetting.SampleRatio);
            LevelDef[] levels = tileComputer.GetLevelDefs(_dataProvider.Width, _dataProvider.Height);
            int levelCount = levels.Length;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            CreateBufferByLevelWholeFile(levels, 1, defaultBandNos);
            CreateBufferByLevelWholeFile(levels, 1, otherBandNos);
            sw.Stop();
            Console.WriteLine("Lost time : " + sw.ElapsedMilliseconds.ToString());
        }

        private void CreateBufferByLevelWholeFile(LevelDef[] levels, int minLevel, int[] bandNos)
        {
            if (bandNos == null || bandNos.Length == 0)
                return;
            foreach (int b in bandNos)
            {
                string fName = GetLevelBufferFileNameWhole(b);
                using (FileStream fs = new FileStream(fName, FileMode.Create))
                {
                    int levelCount = levels.Length;
                    for (int i = levelCount - 1; i >= minLevel; i--)
                    {
                        CreateLevelBufferWholeFile(fs, b, levels[i]);
                    }
                }
            }
        }

        private void CreateLevelBufferWholeFile(FileStream fs, int bandNo, LevelDef levelDef)
        {
            Size lvSize = levelDef.Size;
            float scale = levelDef.Scale;
            int rowsOfBlock = lvSize.Height;
            int blockCount = 0;
            byte[] buffer = CreateBuffer(lvSize.Width, ref rowsOfBlock, out blockCount);
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            IntPtr bufferPointer = handle.AddrOfPinnedObject();
            try
            {
                int bRow = 0, eRow = 0, actualRows = 0;
                int oBeginRow = 0, oEndRow = 0;
                IRasterBand raster = _dataProvider.GetRasterBand(bandNo);
                for (int i = 0; i < blockCount; i++, bRow += rowsOfBlock)
                {
                    eRow = Math.Min(bRow + rowsOfBlock, lvSize.Height);
                    actualRows = eRow - bRow;
                    oBeginRow = (int)(bRow / scale);
                    oEndRow = (int)(eRow / scale);
                    raster.Read(0, oBeginRow, raster.Width, oEndRow - oBeginRow,
                                bufferPointer, raster.DataType,
                                lvSize.Width, actualRows);
                    fs.Write(buffer, 0, actualRows * lvSize.Width * _dataTypeSize);
                }
            }
            finally
            {
                handle.Free();
            }
        }

        private void CreateBufferByLevel()
        {
            int[] defaultBandNos, otherBandNos;
            GetBandNos(out defaultBandNos, out otherBandNos);
            TileComputer tileComputer = new TileComputer(_tileSetting.TileSize, _tileSetting.SampleRatio);
            LevelDef[] levels = tileComputer.GetLevelDefs(_dataProvider.Width, _dataProvider.Height);
            int levelCount = levels.Length;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            CreateBufferByLevel(levels, 1, defaultBandNos);
            CreateBufferByLevel(levels, 1, otherBandNos);
            sw.Stop();
            Console.WriteLine("Lost time : " + sw.ElapsedMilliseconds.ToString());
        }

        private void CreateBufferByLevel(LevelDef[] levels, int minLevel, int[] bandNos)
        {
            if (bandNos == null || bandNos.Length == 0)
                return;
            int levelCount = levels.Length;
            for (int i = levelCount - 1; i >= minLevel; i--)
            {
                foreach (int b in bandNos)
                {
                    CreateLevelBuffer(b, levels[i]);
                }
            }
        }

        private void CreateLevelBuffer(int bandNo, LevelDef levelDef)
        {
            Size lvSize = levelDef.Size;
            string fName = GetLevelBufferFileName(bandNo, levelDef);
            if (File.Exists(fName) && (new FileInfo(fName)).Length == lvSize.Width * lvSize.Height * _dataTypeSize)
                return;
            float scale = levelDef.Scale;
            int rowsOfBlock = lvSize.Height;
            int blockCount = 0;
            byte[] buffer = CreateBuffer(lvSize.Width, ref rowsOfBlock, out blockCount);
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            IntPtr bufferPointer = handle.AddrOfPinnedObject();
            try
            {
                using (FileStream fs = new FileStream(fName, FileMode.Create))
                {
                    int bRow = 0, eRow = 0, actualRows = 0;
                    int oBeginRow = 0, oEndRow = 0;
                    IRasterBand raster = _dataProvider.GetRasterBand(bandNo);
                    for (int i = 0; i < blockCount; i++, bRow += rowsOfBlock)
                    {
                        eRow = Math.Min(bRow + rowsOfBlock, lvSize.Height);
                        actualRows = eRow - bRow;
                        oBeginRow = (int)(bRow / scale);
                        oEndRow = (int)(eRow / scale);
                        raster.Read(0, oBeginRow, raster.Width, oEndRow - oBeginRow,
                                    bufferPointer, raster.DataType,
                                    lvSize.Width, actualRows);
                        fs.Write(buffer, 0, actualRows * lvSize.Width * _dataTypeSize);
                    }
                }
            }
            finally
            {
                handle.Free();
            }
        }

        private byte[] CreateBuffer(int width, ref int height, out int blockCount)
        {
            int oHeight = height;
            blockCount = 0;
            byte[] buffer;
        tryAgainLine:
            try
            {
                buffer = new byte[width * height * _dataTypeSize];
            }
            catch (OutOfMemoryException)
            {
                height /= 2;
                goto tryAgainLine;
            }
            blockCount = (int)Math.Ceiling(oHeight / (double)height);
            return buffer;
        }

        private string GetLevelBufferFileNameWhole(int bandNo)
        {
            return Path.Combine(_fDir, string.Format("WHOLE_B{0}.cache", bandNo));
        }

        private string GetLevelBufferFileName(int bandNo, LevelDef levelDef)
        {
            return Path.Combine(_fDir, string.Format("B{0}_L{1}", bandNo, levelDef.LevelNo));
        }

        private void GetBandNos(out int[] defaultBandNos, out int[] otherBandNos)
        {
            defaultBandNos = _dataProvider.GetDefaultBands();
            List<int> bandNos = new List<int>();
            for (int b = 1; b <= _dataProvider.BandCount; b++)
            {
                if (defaultBandNos != null && Array.IndexOf(defaultBandNos, b) >= 0)
                    continue;
                bandNos.Add(b);
            }
            otherBandNos = bandNos.Count > 0 ? bandNos.ToArray() : null;
        }

        public void Stop()
        {
        }

        private string GetTileBufferFileName(TileIdentify tile, int bandNo)
        {
            return Path.Combine(_fDir, string.Format("{0}{1}.RAW", bandNo, tile.TileNo));
        }

        public bool IsExist(TileIdentify tile, int[] selectedBandNos)
        {
            foreach (int b in selectedBandNos)
            {
                long id = long.Parse(b.ToString() + tile.TileNo.ToString());
                if (!_cachedFiles.Contains(id))
                    return false;
            }
            Console.WriteLine("existed : " + tile.TileNo.ToString());
            return true;
        }

        public T[] GetTile<T>(TileIdentify tile, int bandNo)
        {
            long id = long.Parse(bandNo.ToString() + tile.TileNo.ToString());
            if (!_cachedFiles.Contains(id))
                return null;
            int size = tile.Width * tile.Height;
            int bytesSize = size * _dataTypeSize;
            T[] dstBuffer = new T[size];
            try
            {
                string fname = GetTileBufferFileName(tile, bandNo);
                using (FileStream fs = new FileStream(fname, FileMode.Open))
                {
                    fs.Read(_tileBuffer, 0, bytesSize);
                    GCHandle dstHandle = GCHandle.Alloc(dstBuffer, GCHandleType.Pinned);
                    try
                    {
                        WinAPI.MemoryCopy(dstHandle.AddrOfPinnedObject(), _tileBufferHandle.AddrOfPinnedObject(), bytesSize);
                    }
                    finally
                    {
                        dstHandle.Free();
                    }
                    return dstBuffer;
                }
            }
            catch (FileNotFoundException)
            {
                NotifyCacheIsCleared();
                return null;
            }
        }

        //程序运行过程中缓存文件被删除
        private void NotifyCacheIsCleared()
        {
            _cachedFiles.Clear();
        }

        public void PutTile<T>(TileIdentify tile, int bandNo, T[] buffer)
        {
            long id = long.Parse(bandNo.ToString() + tile.TileNo.ToString());
            if (_cachedFiles.Contains(id))
                return;
            string fname = GetTileBufferFileName(tile, bandNo);
            int len = tile.Width * tile.Height * DataTypeHelper.SizeOf(_dataProvider.DataType);
            byte[] dstBuffer = new byte[len];
            GCHandle srcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            GCHandle dstHandle = GCHandle.Alloc(dstBuffer, GCHandleType.Pinned);
            try
            {
                WinAPI.MemoryCopy(dstHandle.AddrOfPinnedObject(), srcHandle.AddrOfPinnedObject(), len);
            tryAgainLine:
                try
                {
                    File.WriteAllBytes(fname, dstBuffer);
                }
                catch (DirectoryNotFoundException)//缓存目录被删除
                {
                    Directory.CreateDirectory(_fDir);
                    goto tryAgainLine;
                }
                catch (IOException ioex)
                {
                    if (ioex.Message.Contains("磁盘空间"))
                        FileDiskCacheManagerFactory.TryFreeDiskspace(_fDir);
                    return;
                }
                _cachedFiles.Add(id);
            }
            finally
            {
                srcHandle.Free();
                dstHandle.Free();
            }
        }

        public void Dispose()
        {
            _tileBufferHandle.Free();
            if (_isInsideDataProvider)
            {
                _dataProvider.Dispose();
            }
            _tileBuffer = null;
        }
    }
}
