using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace GeoDo.RSS.Core.DF
{
    public unsafe class OverviewGenerator<T> : IOverviewGenerator
    {
        protected IRasterDataProvider _dataProvider;

        private byte* _ptr0 = null;
        private int _stride = 0;
        private int _dstWidth;
        private Func<T, byte> _s1, _s2, _s3;
        private T[] _blueBuffer, _greenBuffer, _redBuffer;

        public OverviewGenerator(IRasterDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public Size ComputeSize(int maxSize)
        {
            float factor = _dataProvider.Width / (float)_dataProvider.Height;
            float width = 0, height = 0;
            if (factor > 1)
            {
                width = maxSize;
                height = width / factor;
            }
            else
            {
                height = maxSize;
                width = height * factor;
            }
            return new Size((int)width, (int)height);
        }

        public void Generate(int[] bandNos, object[] stretchers,ref Bitmap bitmap)
        {
            if (bandNos == null || bandNos.Length == 0 || bitmap == null)
                return;
            if (bandNos.Length != 1 && bandNos.Length != 3)
                return;
            if (stretchers == null || stretchers.Length == 0)
                return;
            if (bandNos.Length != stretchers.Length)
                return;
            T[][] datas = GetBandDatas(bandNos, bitmap.Size);
            if (bandNos.Length == 1)
            {
                Func<T, byte> st = stretchers[0] as Func<T, byte>;
                FillBitmap(datas[0], st, ref bitmap);
            }
            else
            {
                Func<T, byte> stRed = stretchers[0] as Func<T, byte>;
                Func<T, byte> stGreen = stretchers[1] as Func<T, byte>;
                Func<T, byte> stBlue = stretchers[2] as Func<T, byte>;
                FillBitmap(datas[0], datas[1], datas[2], stRed, stGreen, stBlue, ref bitmap);
            }
        }

        public void Generate(int[] bandNos, ref Bitmap bitmap)
        {
            if (bandNos == null || bandNos.Length == 0 || bitmap == null)
                return;
            if (bandNos.Length != 1 && bandNos.Length != 3)
                return;
            T[][] datas = GetBandDatas(bandNos, bitmap.Size);
            Func<T, byte>[] sts = new Func<T, byte>[bandNos.Length];
            if (bandNos.Length == 1)
            {
                Func<T, byte> st = GetStretcherFromDataProvider(bandNos[0]);
                FillBitmap(datas[0], st, ref bitmap);
            }
            else
            {
                Func<T, byte> stRed = GetStretcherFromDataProvider(bandNos[0]);
                Func<T, byte> stGreen = GetStretcherFromDataProvider(bandNos[1]);
                Func<T, byte> stBlue = GetStretcherFromDataProvider(bandNos[2]);
                FillBitmap(datas[0], datas[1], datas[2], stRed, stGreen, stBlue, ref bitmap);
            }
        }

        private unsafe void FillBitmap(T[] redBuffer, T[] greenBuffer, T[] blueBuffer,
            Func<T, byte> redStretcher, Func<T, byte> greenStretcher, Func<T, byte> blueStretcher, ref Bitmap bitmap)
        {
            BitmapData pdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            try
            {
                _redBuffer = redBuffer;
                _greenBuffer = greenBuffer;
                _blueBuffer = blueBuffer;
                _s1 = redStretcher;
                _s2 = greenStretcher;
                _s3 = blueStretcher;
                _stride = pdata.Stride;
                _dstWidth = bitmap.Width;
                IntPtr intPtr = pdata.Scan0;
                _ptr0 = (byte*)intPtr;
                Parallel.For(0, bitmap.Height, row => { FillOneRow(row); });
            }
            finally
            {
                bitmap.UnlockBits(pdata);
                _redBuffer = null;
                _greenBuffer = null;
                _blueBuffer = null;
                _s1 = null;
                _s2 = null;
                _s3 = null;
            }
        }

        private unsafe void FillBitmap(T[] grayBuffer, Func<T, byte> grayStretcher, ref Bitmap bitmap)
        {
            BitmapData pdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            try
            {
                _redBuffer = grayBuffer;
                _s1 = grayStretcher;
                _stride = pdata.Stride;
                _dstWidth = bitmap.Width;
                IntPtr intPtr = pdata.Scan0;
                _ptr0 = (byte*)intPtr;
                Parallel.For(0, bitmap.Height, row => { BuildOneRowGray(row); });
            }
            finally
            {
                bitmap.UnlockBits(pdata);
                _redBuffer = null;
                _greenBuffer = null;
                _blueBuffer = null;
                _s1 = null;
                _s2 = null;
                _s3 = null;
            }
        }

        private void FillOneRow(int row)
        {
            byte* ptr = _ptr0 + row * _stride;
            int idx = row * _dstWidth;
            for (int col = 0; col < _dstWidth; col++, ptr += 3, idx++)
            {
                *ptr = _s3(_blueBuffer[idx]);
                *(ptr + 1) = _s2(_greenBuffer[idx]);
                *(ptr + 2) = _s1(_redBuffer[idx]);
            }
        }

        private void BuildOneRowGray(int row)
        {
            byte* ptr = _ptr0 + row * _stride;
            int idx = row * _dstWidth;
            for (int col = 0; col < _dstWidth; col++, ptr += 1, idx++)
            {
                *ptr = _s1(_redBuffer[idx]);
            }
        }

        private Func<T, byte> GetStretcherFromDataProvider(int bandNo)
        {
            IRgbStretcher<T> st = _dataProvider.GetRasterBand(bandNo).Stretcher as IRgbStretcher<T>;
            return st.Stretcher;
        }

        private T[][] GetBandDatas(int[] bandNos, Size size)
        {
            int bands = bandNos.Length;
            T[][] buffers = new T[bands][];
            for (int i = 0; i < bands; i++)
            {
                //eg:1,2,1
                if (bandNos.Length == 3)
                {
                    for (int p = 0; p < i; p++)
                    {
                        if (bandNos[p] == bandNos[i])
                        {
                            buffers[i] = buffers[p];
                            goto nexti;
                        }
                    }
                }
                buffers[i] = new T[size.Width * size.Height];
                GCHandle handle = GCHandle.Alloc(buffers[i], GCHandleType.Pinned);
                _dataProvider.GetRasterBand(bandNos[i]).Read(0, 0, _dataProvider.Width, _dataProvider.Height, handle.AddrOfPinnedObject(), _dataProvider.DataType, size.Width, size.Height);
                handle.Free();
            nexti: ;
            }
            return buffers;
        }
    }
}
