using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using GeoDo.Project;

namespace GeoDo.RSS.MIF.Core
{
    internal class MemPixelIndexMapper : IPixelIndexMapper
    {
        protected object _tag;
        protected string _name;
        protected List<int> _indexes;
        protected Size _size;
        protected CoordEnvelope _coordEnvelope;
        protected ISpatialReference _spatialRef;
        protected bool _display = true;
        protected string _outIdentify;

        public MemPixelIndexMapper(string name, int capacity, Size size, CoordEnvelope coordEnvelope, ISpatialReference spatialRef)
        {
            _name = name;
            _indexes = new List<int>(capacity);
            _size = size;
            _spatialRef = spatialRef;
            _coordEnvelope = coordEnvelope;
        }

        public void SetDispaly(bool display)
        {
            _display = display;
        }

        public void SetOutIdentify(string outIdentify)
        {
            _outIdentify = outIdentify;
        }

        public string OutIdentify
        {
            get { return _outIdentify; }
        }

        public bool Display
        {
            get
            {
                return _display;
            }
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
            get { return _indexes.Count; }
        }

        public IEnumerable<int> Indexes
        {
            get
            {
                int n = _indexes.Count;
                for (int i = 0; i < n; i++)
                    yield return _indexes[i];
            }
        }

        public void Put(int index)
        {
            _indexes.Add(index);
        }

        public void Put(int[] indexes)
        {
            if (indexes == null || indexes.Length == 0)
                return;
            _indexes.AddRange(indexes);
        }

        public void Remove(int[] indexes)
        {
            if (indexes == null || indexes.Length == 0)
                return;
            for (int i = 0; i < indexes.Length; i++)
                if (_indexes.Contains(indexes[i]))
                    _indexes.Remove(indexes[i]);
        }

        public void Reset()
        {
            _indexes.Clear();
        }

        public void Dispose()
        {
            _indexes.Clear();
            _tag = null;
        }


        public bool Get(int index)
        {
            int n = _indexes.Count;
            for (int i = 0; i < n; i++)
                if (_indexes[i] == index)
                    return true;
            return false;
        }
    }
}
