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

namespace GeoDo.Smart.MaxValueComposites
{

    public class VirtualData
    {
        private enumDataType _dataType;

        private List<IRasterBand> _bands = null;

        public enumDataType DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }
        public int Width;
        public int Height;
        public int BandCount
        {
            get { return _bands == null ? 0 : _bands.Count; }
        }

        public IRasterBand[] Bands
        {
            get { return _bands.ToArray(); }
        }

        public void AddBand(IRasterBand band)
        {
            if (_bands == null)
            {
                _bands = new List<IRasterBand>();
                _bands.Add(band);
                _dataType = band.DataType;
                Width = band.Width;
                Height = band.Height;
            }
            else
            {
                if (ValidBand(band))
                    _bands.Add(band);
            }
        }

        private bool ValidBand(IRasterBand band)
        {
            if (_dataType != band.DataType)
                return false;
            if (Width != band.Width)
                return false;
            if (Height != band.Height)
                return false;
            return true;
        }

        public T[][] ReadLine<T>(int yOffset)
        {
            T[][] buffers = BuildBuffers<T>(Width, BandCount);
            T[] pixelBuffer = new T[BandCount];
            GCHandle[] handles = GetHandles<T>(buffers);
            try
            {
                for (int bandIdx = 0; bandIdx < BandCount; bandIdx++)
                {
                    _bands[bandIdx].Read(0, yOffset, Width, 1, handles[bandIdx].AddrOfPinnedObject(), _dataType, Width, 1);
                }
                return buffers;
            }
            finally
            {
                FreeHandles<T>(handles);
            }
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
