using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;
using System.Drawing;
using GeoDo.Project;
using GeoDo.RSS.MIF.Core;
using System.IO;

namespace GeoDo.Smart.MaxCsr
{
    /// <summary>
    /// 时序数据集。
    /// </summary>
    public class VirtualData
    {
        private enumDataType _dataType;
        private List<IRasterBand[]> _vBands = null;
        private int _bandCount = 0;

        public enumDataType DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        /// <summary>
        /// 每个通道的数据宽
        /// </summary>
        public int Width;
        /// <summary>
        /// 每个通道的数据高
        /// </summary>
        public int Height;
        /// <summary>
        /// 每个时序的通道数
        /// </summary>
        public int BandCount
        {
            get
            {
                return _bandCount;
            }
        }

        /// <summary>
        /// 时序长度
        /// </summary>
        public int TimesCount
        {
            get { return _vBands == null ? 0 : _vBands.Count; }
        }

        public IRasterBand[][] Bands
        {
            get { return _vBands.ToArray(); }
        }

        public void AddTimeBands(IRasterBand[] bands)
        {
            if (bands == null || bands.Length == 0)
                return;
            if (_vBands == null)
            {
                _dataType = bands[0].DataType;
                Width = bands[0].Width;
                Height = bands[0].Height;

                _bandCount = bands.Length;
                if (ValidBands(bands))
                {
                    _vBands = new List<IRasterBand[]>();
                    _vBands.Add(bands);
                }
            }
            else
            {
                if (ValidBands(bands))
                    _vBands.Add(bands);
            }
        }

        private bool ValidBands(IRasterBand[] bands)
        {
            foreach (IRasterBand band in bands)
            {
                if (_dataType != band.DataType)
                    return false;
                if (Width != band.Width)
                    return false;
                if (Height != band.Height)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 读取所有时次、所有通道的一行数据
        /// 三维：时序、波段、一行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="yOffset">行索引</param>
        /// <returns>三维：时序、波段、一行</returns>
        public T[][][] ReadLine<T>(int yOffset)
        {
            List<T[][]> timesBuffers = new List<T[][]>();
            for (int time = 0; time < TimesCount; time++)
            {
                T[][] buffers = BuildBuffers<T>(Width, BandCount);  //一个时序的内存量
                T[] pixelBuffer = new T[BandCount];
                GCHandle[] handles = GetHandles<T>(buffers);
                try
                {
                    for (int bandIdx = 0; bandIdx < BandCount; bandIdx++)
                    {
                        _vBands[time][bandIdx].Read(0, yOffset, Width, 1, handles[bandIdx].AddrOfPinnedObject(), _dataType, Width, 1);
                    }
                    timesBuffers.Add(buffers);
                }
                finally
                {
                    FreeHandles<T>(handles);
                }
            }
            return timesBuffers.ToArray();
        }

        private T[][] BuildBuffers<T>(int width, int bandCount)
        {
            T[][] buffers = new T[bandCount][];
            for (int i = 0; i < bandCount; i++)
                buffers[i] = new T[width];
            return buffers;
        }

        private GCHandle[] GetHandles<T>(T[][] buffers)
        {
            GCHandle[] handles = new GCHandle[buffers.Length];
            for (int i = 0; i < buffers.Length; i++)
                handles[i] = GCHandle.Alloc(buffers[i], GCHandleType.Pinned);
            return handles;
        }

        private void FreeHandles<T>(GCHandle[] handles)
        {
            if (handles == null || handles.Length == 0)
                return;
            foreach (GCHandle h in handles)
                h.Free();
        }
    }
}
