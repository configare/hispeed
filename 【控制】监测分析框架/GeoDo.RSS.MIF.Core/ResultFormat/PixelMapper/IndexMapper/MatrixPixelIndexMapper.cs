using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using System.Drawing;

namespace GeoDo.RSS.MIF.Core
{
    internal class MatrixPixelIndexMapper : IPixelIndexMapper
    {
        protected object _tag;
        protected string _name;
        protected byte[] _buffer;
        protected Size _size;
        protected CoordEnvelope _coordEnvelope;
        protected ISpatialReference _spatialRef;
        protected bool _display = true;
        protected string _outIdentify;

        public MatrixPixelIndexMapper(string name, int width, int height, CoordEnvelope coordEnvelope, ISpatialReference spatialRef)
        {
            _name = name;
            _buffer = new byte[width * height];
            _size = new System.Drawing.Size(width, height);
            _spatialRef = spatialRef;
            _coordEnvelope = coordEnvelope;
        }

        public void SetDispaly(bool display)
        {
            _display = display;
        }

        public bool Display
        {
            get
            {
                return _display;
            }
        }

        public void SetOutIdentify(string outIdentify)
        {
            _outIdentify = outIdentify;
        }

        public string OutIdentify
        {
            get { return _outIdentify; }
        }

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        public Size Size
        {
            get { return _size; }
        }

        public CoordEnvelope CoordEnvelope
        {
            get { return _coordEnvelope; }
        }

        public ISpatialReference SpatialRef
        {
            get { return _spatialRef; }
        }

        public string Name
        {
            get { return _name; }
        }

        public bool IsEmpty()
        {
            return false;
        }

        public int Count
        {
            get { return _buffer.Length; }
        }

        public IEnumerable<int> Indexes
        {
            get
            {
                int n = _buffer.Length;
                for (int i = 0; i < n; i++)
                    if (_buffer[i] == 1)
                        yield return i;

            }
        }

        public void Put(int index)
        {
            _buffer[index] = 1;
        }

        public void Put(int[] indexes)
        {
            if (indexes == null || indexes.Length == 0)
                return;
            int n = indexes.Length;
            for (int i = 0; i < n; i++)
                _buffer[indexes[i]] = 1;
        }

        public void Remove(int[] indexes)
        {
            if (indexes == null || indexes.Length == 0)
                return;
            int n = indexes.Length;
            for (int i = 0; i < n; i++)
                _buffer[indexes[i]] = 0;
        }

        public void Reset()
        {
            Array.Clear(_buffer, 0, _buffer.Length);
        }

        public void Dispose()
        {
            _buffer = null;
        }


        public bool Get(int index)
        {
            return _buffer[index] == 1;
        }
    }
}
