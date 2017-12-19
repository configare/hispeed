using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.DST
{
    internal class DstFeatureFY2Collection : ICursorDisplayedFeatures
    {
        private Dictionary<int, DstFeatureFY2> _features;
        private string _name;

        public DstFeatureFY2Collection(string name, Dictionary<int, DstFeatureFY2> features)
        {
            _name = name;
            _features = features;
        }

        public string Name
        {
            get { return _name; }
        }

        public string GetCursorInfo(int pixelIndex)
        {
            if (_features == null)
                return string.Empty;
            if (_features.ContainsKey(pixelIndex))
                return _features[pixelIndex].ToString();
            return string.Empty;
        }
    }
}
