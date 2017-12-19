using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.Core.RasterDrawing
{
    internal unsafe class ReadPixelHelper: IDisposable
    {
        private IRasterDataProvider _dataProvider = null;
        private int _bandCount = 0;
        private int _dataTypeSize = 0;

        public ReadPixelHelper(IRasterDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            _bandCount = _dataProvider.BandCount;
            _dataTypeSize = DataTypeHelper.SizeOf(_dataProvider.DataType);
        }

        /// <summary>
        /// 读取当前像素点的所有通道值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public T[] Read<T>(int x, int y)
        {
            T[] values = new T[_bandCount];
            IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(values, 0);
            for (int i = 1; i <= _bandCount; i++, ptr = IntPtr.Add(ptr, _dataTypeSize))
            {
                _dataProvider.GetRasterBand(i).Read(x, y, 1, 1, ptr, _dataProvider.DataType, 1, 1);
            }
            return values;
        }

        public void Read(int x, int y, double* buffer)
        {
            switch (_dataProvider.DataType)
            {
                case enumDataType.Byte:
                    byte[] valuesByte = Read<byte>(x, y);
                    for (int i = 0; i < _bandCount; i++, buffer++)
                        *buffer = valuesByte[i];
                    break;
                case enumDataType.UInt16:
                    UInt16[] values = Read<UInt16>(x, y);
                    for (int i = 0; i < _bandCount; i++, buffer++)
                        *buffer = values[i];
                    break;
                case enumDataType.Int16:
                     Int16[] valuesInt16 = Read<Int16>(x, y);
                    for (int i = 0; i < _bandCount; i++, buffer++)
                        *buffer = valuesInt16[i];
                    break;
                case enumDataType.UInt32:
                    UInt32[] valuesUInt32 = Read<UInt32>(x, y);
                    for (int i = 0; i < _bandCount; i++, buffer++)
                        *buffer = valuesUInt32[i];
                    break;
                case enumDataType.Int32:
                    Int32[] valuesInt32 = Read<Int32>(x, y);
                    for (int i = 0; i < _bandCount; i++, buffer++)
                        *buffer = valuesInt32[i];
                    break;
                case enumDataType.UInt64:
                    UInt64[] valuesUInt64 = Read<UInt64>(x, y);
                    for (int i = 0; i < _bandCount; i++, buffer++)
                        *buffer = valuesUInt64[i];
                    break;
                case enumDataType.Int64:
                    Int64[] valuesInt64 = Read<Int64>(x, y);
                    for (int i = 0; i < _bandCount; i++, buffer++)
                        *buffer = valuesInt64[i];
                    break;
                case enumDataType.Float:
                    float[] valuesFloat = Read<float>(x, y);
                    for (int i = 0; i < _bandCount; i++, buffer++)
                        *buffer = valuesFloat[i];
                    break;
                case enumDataType.Double:
                    double[] valuesDouble = Read<double>(x, y);
                    for (int i = 0; i < _bandCount; i++, buffer++)
                        *buffer = valuesDouble[i];
                    break;
            }
        }

        public void Dispose()
        {
            _dataProvider = null;
        }
    }
}
