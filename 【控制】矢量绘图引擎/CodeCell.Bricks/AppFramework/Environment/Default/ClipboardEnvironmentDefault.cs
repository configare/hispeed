using System;
using System.Collections.Generic;
using System.Text;

namespace CodeCell.Bricks.AppFramework
{
    public class ClipboardEnvironmentDefault:IClipboardEnvironment
    {
        private List<object> _objects = new List<object>(1);

        internal ClipboardEnvironmentDefault()
        { 
        }

        #region IClipboardEnvironment ≥…‘±

        public bool IsEmpty
        {
            get 
            {
                return _objects == null || _objects.Count == 0;
            }
        }

        public int Count
        {
            get 
            {
                return _objects != null ? _objects.Count : 0;
            }
        }

        public IEnumerable<object> Datas
        {
            get 
            {
                return _objects;
            }
        }

        public void Clear()
        {
            if (_objects != null)
                _objects.Clear();
        }

        public object First
        {
            get 
            {
                return _objects != null && _objects.Count > 0 ? _objects[0] : null;
            }
        }

        public object[] GetDatas()
        {
            return _objects != null ? _objects.ToArray() : null;
        }

        public void PutData(object obj)
        {
            Clear();
            _objects.Add(obj);
        }

        public void PutData(object[] objects)
        {
            if (objects == null)
                return;
            Clear();
            foreach (object obj in objects)
                _objects.Add(obj);
        }

        #endregion
    }
}
