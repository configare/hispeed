using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class ExtractResultArray : IExtractResultArray
    {
        private List<IExtractResultBase> _pixelMappers = null;
        private string _name;

        public ExtractResultArray(string name)
        {
            _name = name;
        }

        public IExtractResultBase[] PixelMappers
        {
            get
            {
                return _pixelMappers == null || _pixelMappers.Count == 0 ? null : _pixelMappers.ToArray();
            }
        }

        public string Name
        { get { return _name; } }

        public bool Add(IExtractResultBase pixelMapper)
        {
            if (_pixelMappers == null)
                _pixelMappers = new List<IExtractResultBase>();
            _pixelMappers.Add(pixelMapper);
            return true;
        }
    }
}
