using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;

namespace GeoDo.RSS.MIF.Core
{
    public class MemPixelFeatureMapper<T> : IPixelFeatureMapper<T>
    {
        protected string _name;
        protected List<int> _indexes;
        protected List<T> _values;
        protected Size _size;
        protected CoordEnvelope _coordEnvelope;
        protected ISpatialReference _spatialRef;
        protected bool _display = true;
        protected string _outIdentify;

        public MemPixelFeatureMapper(string name, int capacity,Size size,CoordEnvelope coordEnvelope,ISpatialReference spatialRef)
        {
            _name = name;
            _indexes = new List<int>(capacity);
            _values = new List<T>(capacity);
            _size = size;
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

        public IEnumerable<int> Indexes
        {
            get
            {
                int n = _indexes.Count;
                for (int i = 0; i < n; i++)
                    yield return _indexes[i];
            }
        }

        public T GetValueByIndex(int index)
        {
            return _values[index];
        }

        public void Put(int index, T feature)
        {
            _indexes.Add(index);
            _values.Add(feature);
        }

        public void Reset()
        {
            _indexes.Clear();
            _values.Clear();
        }

        public void Dispose()
        {
            _indexes.Clear();
            _values.Clear();
        }
    }
}
