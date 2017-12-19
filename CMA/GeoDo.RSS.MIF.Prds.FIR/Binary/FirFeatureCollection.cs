using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    internal class FirFeatureCollection : ICursorDisplayedFeatures
    {
        private Dictionary<int, PixelFeature> _features;
        private string _name;

        public FirFeatureCollection(string name,Dictionary<int,PixelFeature> features )
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
