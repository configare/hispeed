using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public abstract unsafe class DataProviderReader<T> : IDataProviderReader
    {
        protected IRasterDataProvider _dataProvider;
        protected int _tileSize;
        protected int[] _selectedBandNos;
        private IRgbStretcher<T> st1, st2, st3;
        private Func<T, byte> _stretcher;
        private IRgbStretcherProvider _stretcherProvider;
        private ColorMapTable<int> _colorMapTable;
        private string _colorTableName;
        private IFileDiskCacheManager _fileDiskCacheManager;
        private delegate void AsyncCacheHandler(T[][] buffers, TileIdentify tile, int[] selectedBandNos);
        private AsyncCacheHandler _asyncCacheHandler;
        private bool _enableDiskCache = true;

        public DataProviderReader(int tileSize, int[] selectedBandNos,
            IRasterDataProvider dataProvider,
            IRgbStretcherProvider stretcherProvider,
            string colorTableName
            )
        {
            _tileSize = tileSize;
            _selectedBandNos = selectedBandNos;
            _dataProvider = dataProvider;
            _stretcherProvider = stretcherProvider;
            _colorTableName = colorTableName;
            _asyncCacheHandler = new AsyncCacheHandler(DoAsyncCache);
            SetStretchers();
            ActiveFileDiskCacheManager();
        }

        public IFileDiskCacheManager FileDiskCacheManager
        {
            get { return _fileDiskCacheManager; }
        }

        private void ActiveFileDiskCacheManager()
        {
            _fileDiskCacheManager = FileDiskCacheManagerFactory.Active(_dataProvider);
            _enableDiskCache = _fileDiskCacheManager != null;
        }

        private void DoAsyncCache(T[][] buffers, TileIdentify tile, int[] selectedBandNos)
        {
            for (int i = 0; i < _selectedBandNos.Length; i++)
                _fileDiskCacheManager.PutTile<T>(tile, _selectedBandNos[i], buffers[i]);
        }

        public void UpdateSelectedBandNos(int[] selectedBandNos)
        {
            _selectedBandNos = selectedBandNos;
            SetStretchers();
        }

        private void SetStretchers()
        {
            if (_selectedBandNos.Length == 1)
            {
                if (_stretcherProvider != null)
                {
                    object obj = _stretcherProvider.GetStretcher(_dataProvider.fileName, _colorTableName, out _colorMapTable);
                    if (obj != null)
                        _stretcher = obj as Func<T, byte>;
                    else
                        st1 = _dataProvider.GetRasterBand(_selectedBandNos[0]).Stretcher as IRgbStretcher<T>;
                }
                else
                {
                    st1 = _dataProvider.GetRasterBand(_selectedBandNos[0]).Stretcher as IRgbStretcher<T>;
                }
            }
            else if (_selectedBandNos.Length == 3)
            {
                st1 = _dataProvider.GetRasterBand(_selectedBandNos[0]).Stretcher as IRgbStretcher<T>;
                st2 = _dataProvider.GetRasterBand(_selectedBandNos[1]).Stretcher as IRgbStretcher<T>;
                st3 = _dataProvider.GetRasterBand(_selectedBandNos[2]).Stretcher as IRgbStretcher<T>;
            }
        }

        public object GrayStretcher
        {
            get { return st1; }
        }

        private object _lockObj = new object();
        public TileBitmap CreateBitmapByTile(LevelDef level, int beginRow, int beginCol, int width, int height, TileIdentify tile)
        {
            lock (_lockObj)
            {
                T[][] buffers = null;
            //从磁盘中读取缓存的瓦片
            tryAgainLine:
                if (_enableDiskCache && _fileDiskCacheManager.IsExist(tile, _selectedBandNos))
                {
                    buffers = new T[_selectedBandNos.Length][];
                    for (int i = 0; i < _selectedBandNos.Length; i++)
                    {
                        buffers[i] = _fileDiskCacheManager.GetTile<T>(tile, _selectedBandNos[i]);
                        if (buffers[i] == null)//缓存已经从磁盘清除
                        {
                            goto tryAgainLine;
                        }
                    }
                }
                else
                {
                    bool isOK = ReadRasterFile(out buffers, beginRow, beginCol, width, height, tile);
                    if (!isOK)
                        return null;
                    Console.WriteLine("direct read , "+Environment.TickCount.ToString());
                    //将该瓦片保存在磁盘
                    if (_enableDiskCache)
                        _asyncCacheHandler.BeginInvoke(buffers, tile, _selectedBandNos, null, null);
                }
                TileBitmap tb = new TileBitmap();
                tb.Level = level;
                tb.Tile = tile;
                tb.Bitmap = BuildBitmap(buffers, level, tile);
                return tb;
            }
        }

        private Bitmap BuildBitmap(T[][] buffers, LevelDef level, TileIdentify tile)
        {
            using (IBitmapBuilder<T> builder = GetBitmapBuilder())
            {
                Bitmap bitmap = null;
                if (buffers.Length == 1)
                {
                    bitmap = new Bitmap(_tileSize, _tileSize, PixelFormat.Format8bppIndexed);
                    bitmap.Palette = BitmapBuilderFactory.GetDefaultGrayColorPalette();
                    if (st1 == null && _stretcher == null)
                        builder.Build(tile.Width, tile.Height, tile.OffsetX, tile.OffsetY, buffers[0], ref bitmap);
                    else
                    {
                        if (_stretcher != null)
                            builder.Build(tile.Width, tile.Height, tile.OffsetX, tile.OffsetY, buffers[0], _stretcher, ref bitmap);
                        else
                            builder.Build(tile.Width, tile.Height, tile.OffsetX, tile.OffsetY, buffers[0], st1.Stretcher, ref bitmap);
                    }
                    //
                    if (_colorMapTable != null)
                    {
                        ColorPalette plt = BitmapBuilderFactory.GetDefaultGrayColorPalette();
                        for (int i = 0; i < 256; i++)
                            plt.Entries[i] = Color.Black;
                        int idx = 1;
                        foreach (ColorMapItem<int> item in _colorMapTable.Items)
                        {
                            for (int v = item.MinValue; v < item.MaxValue; v++)
                                plt.Entries[idx] = item.Color;
                            idx++;
                        }
                        bitmap.Palette = plt;
                    }
                }
                else if (buffers.Length == 3)
                {
                    bitmap = new Bitmap(_tileSize, _tileSize, PixelFormat.Format24bppRgb);
                    if (st1 == null || st2 == null || st3 == null)
                        builder.Build(tile.Width, tile.Height, tile.OffsetX, tile.OffsetY, buffers[0], buffers[1], buffers[2], ref bitmap);
                    else
                        builder.Build(tile.Width, tile.Height, tile.OffsetX, tile.OffsetY, buffers[0], buffers[1], buffers[2], st1.Stretcher, st2.Stretcher, st3.Stretcher, ref bitmap);
                }
                return bitmap;
            }
        }

        private bool ReadRasterFile(out T[][] buffers, int offsetY, int offsetX, int xSize, int ySize, TileIdentify tile)
        {
            int size = tile.Width * tile.Height;
            int bands = _selectedBandNos.Length;
            buffers = new T[bands][];
            for (int i = 0; i < bands; i++)
            {
                buffers[i] = new T[size];
                GCHandle handle = GCHandle.Alloc(buffers[i], GCHandleType.Pinned);
                try
                {
                    //eg:1,2,1
                    if (_selectedBandNos.Length == 3)
                    {
                        for (int p = 0; p < i; p++)
                        {
                            if (_selectedBandNos[p] == _selectedBandNos[i])
                            {
                                buffers[i] = buffers[p];
                                goto nexti;
                            }
                        }
                    }
                    //disposed
                    if (_dataProvider == null || i >= _selectedBandNos.Length)
                        return false;
                    _dataProvider.GetRasterBand(_selectedBandNos[i]).Read(offsetX, offsetY, xSize, ySize, handle.AddrOfPinnedObject(), _dataProvider.DataType, tile.Width, tile.Height);
                nexti: ;
                }
                finally
                {
                    handle.Free();
                }
            }
            return true;
        }

        public Bitmap GetOverview(IRasterDataProvider dataProvider, int width, int height, int[] bands, Action<int> progress)
        {
            T[][] buffers = new T[bands.Length][];
            T[] rowBuffer = new T[width];
            GCHandle handle = GCHandle.Alloc(rowBuffer, GCHandleType.Pinned);
            try
            {
                IntPtr rowPtr = handle.AddrOfPinnedObject();
                int offset = 0;
                for (int b = 0; b < bands.Length; b++, offset += height)
                    buffers[b] = ReadBandBuffer(dataProvider, rowPtr, bands[b], width, height, offset, progress);
                //
                using (IBitmapBuilder<T> builder = GetBitmapBuilder())
                {
                    Bitmap bitmap = null;
                    if (buffers.Length == 1)
                    {
                        bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                        bitmap.Palette = BitmapBuilderFactory.GetDefaultGrayColorPalette();
                        if (st1 == null)
                            builder.Build(width, height, 0, 0, buffers[0], ref bitmap);
                        else
                            builder.Build(width, height, 0, 0, buffers[0], st1.Stretcher, ref bitmap);
                    }
                    else if (buffers.Length == 3)
                    {
                        bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                        if (st1 == null || st2 == null || st3 == null)
                            builder.Build(width, height, 0, 0, buffers[0], buffers[1], buffers[2], ref bitmap);
                        else
                            builder.Build(width, height, 0, 0, buffers[0], buffers[1], buffers[2], st1.Stretcher, st2.Stretcher, st3.Stretcher, ref bitmap);
                    }
                    return bitmap;
                }
            }
            finally
            {
                handle.Free();
            }
        }

        private T[] ReadBandBuffer(IRasterDataProvider dataProvider, IntPtr rowPtr, int bandNo, int width, int height, int progressOffset, Action<int> progress)
        {
            T[] buffer = new T[width * height];
            int rowSize = width * DataTypeHelper.SizeOf(dataProvider.DataType);
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                IntPtr dstptr = handle.AddrOfPinnedObject();
                int sample = Math.Min(_dataProvider.Width / width, _dataProvider.Height / height);
                IRasterBand band = dataProvider.GetRasterBand(bandNo);
                int offset = 0;
                for (int r = 0; r < height; r++, offset += rowSize)
                {
                    band.Read(0, r * sample, _dataProvider.Width, 1, rowPtr, dataProvider.DataType, width, 1);
                    IntPtr dst = IntPtr.Add(dstptr, offset);
                    WinAPI.MemoryCopy(dst, rowPtr, rowSize);
                    progress(progressOffset + r);
                }
                return buffer;
            }
            finally
            {
                handle.Free();
            }
        }


        protected abstract IBitmapBuilder<T> GetBitmapBuilder();

        public void SetColorMapTable(IColorMapTableGetter colorTableGetter)
        {
            if (colorTableGetter == null)
                return;
            _colorMapTable = colorTableGetter.ColorTable;
            _stretcher = colorTableGetter.Stretcher as Func<T, byte>;
        }

        public void Dispose()
        {
            _dataProvider = null;
            if (_fileDiskCacheManager != null)
            {
                _fileDiskCacheManager.Dispose();
                _fileDiskCacheManager = null;
            }
        }
    }

    public class DataProviderReaderByte : DataProviderReader<byte>
    {
        public DataProviderReaderByte(int tileSize, int[] selectedBandNos,
            IRasterDataProvider dataProvider, IRgbStretcherProvider stretcherProvider, string colorTableName)
            : base(tileSize, selectedBandNos, dataProvider, stretcherProvider, colorTableName)
        {
        }

        protected override IBitmapBuilder<byte> GetBitmapBuilder()
        {
            return BitmapBuilderFactory.CreateBitmapBuilderByte();
        }
    }

    public class DataProviderReaderUInt16 : DataProviderReader<UInt16>
    {
        public DataProviderReaderUInt16(int tileSize, int[] selectedBandNos,
            IRasterDataProvider dataProvider, IRgbStretcherProvider stretcherProvider, string colorTableName)
            : base(tileSize, selectedBandNos, dataProvider, stretcherProvider, colorTableName)
        {
        }

        protected override IBitmapBuilder<UInt16> GetBitmapBuilder()
        {
            return BitmapBuilderFactory.CreateBitmapBuilderUInt16();
        }
    }


    public class DataProviderReaderInt16 : DataProviderReader<Int16>
    {
        public DataProviderReaderInt16(int tileSize, int[] selectedBandNos,
            IRasterDataProvider dataProvider, IRgbStretcherProvider stretcherProvider, string colorTableName)
            : base(tileSize, selectedBandNos, dataProvider, stretcherProvider, colorTableName)
        {
        }

        protected override IBitmapBuilder<Int16> GetBitmapBuilder()
        {
            return BitmapBuilderFactory.CreateBitmapBuilderInt16();
        }
    }

    public class DataProviderReaderUInt32 : DataProviderReader<UInt32>
    {
        public DataProviderReaderUInt32(int tileSize, int[] selectedBandNos,
            IRasterDataProvider dataProvider, IRgbStretcherProvider stretcherProvider, string colorTableName)
            : base(tileSize, selectedBandNos, dataProvider, stretcherProvider, colorTableName)
        {
        }

        protected override IBitmapBuilder<UInt32> GetBitmapBuilder()
        {
            return BitmapBuilderFactory.CreateBitmapBuilderUInt32();
        }

    }

    public class DataProviderReaderInt32 : DataProviderReader<Int32>
    {
        public DataProviderReaderInt32(int tileSize, int[] selectedBandNos,
            IRasterDataProvider dataProvider, IRgbStretcherProvider stretcherProvider, string colorTableName)
            : base(tileSize, selectedBandNos, dataProvider, stretcherProvider, colorTableName)
        {
        }

        protected override IBitmapBuilder<Int32> GetBitmapBuilder()
        {
            return BitmapBuilderFactory.CreateBitmapBuilderInt32();
        }

    }

    public class DataProviderReaderUInt64 : DataProviderReader<UInt64>
    {
        public DataProviderReaderUInt64(int tileSize, int[] selectedBandNos,
            IRasterDataProvider dataProvider, IRgbStretcherProvider stretcherProvider, string colorTableName)
            : base(tileSize, selectedBandNos, dataProvider, stretcherProvider, colorTableName)
        {
        }

        protected override IBitmapBuilder<UInt64> GetBitmapBuilder()
        {
            return BitmapBuilderFactory.CreateBitmapBuilderUInt64();
        }
    }

    public class DataProviderReaderInt64 : DataProviderReader<Int64>
    {
        public DataProviderReaderInt64(int tileSize, int[] selectedBandNos,
            IRasterDataProvider dataProvider, IRgbStretcherProvider stretcherProvider, string colorTableName)
            : base(tileSize, selectedBandNos, dataProvider, stretcherProvider, colorTableName)
        {
        }

        protected override IBitmapBuilder<Int64> GetBitmapBuilder()
        {
            return BitmapBuilderFactory.CreateBitmapBuilderInt64();
        }
    }

    public class DataProviderReaderFloat : DataProviderReader<float>
    {
        public DataProviderReaderFloat(int tileSize, int[] selectedBandNos,
            IRasterDataProvider dataProvider, IRgbStretcherProvider stretcherProvider, string colorTableName)
            : base(tileSize, selectedBandNos, dataProvider, stretcherProvider, colorTableName)
        {
        }

        protected override IBitmapBuilder<float> GetBitmapBuilder()
        {
            return BitmapBuilderFactory.CreateBitmapBuilderFloat();
        }
    }

    public class DataProviderReaderDouble : DataProviderReader<double>
    {
        public DataProviderReaderDouble(int tileSize, int[] selectedBandNos,
            IRasterDataProvider dataProvider, IRgbStretcherProvider stretcherProvider, string colorTableName)
            : base(tileSize, selectedBandNos, dataProvider, stretcherProvider, colorTableName)
        {
        }

        protected override IBitmapBuilder<double> GetBitmapBuilder()
        {
            return BitmapBuilderFactory.CreateBitmapBuilderDouble();
        }
    }
}
