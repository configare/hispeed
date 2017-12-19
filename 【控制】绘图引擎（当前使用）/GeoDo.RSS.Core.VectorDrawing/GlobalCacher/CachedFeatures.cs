using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.Core.VectorDrawing
{
    internal class CachedFeatures:ICachedFeatures
    {
        private string _identify;
        private Feature[] _features;

        public CachedFeatures(string identify, Feature[] features)
        {
            _identify = identify;
            _features = features;
        }

        public Feature[] Features
        {
            get { return _features; }
        }

        public string Identify
        {
            get { return _identify; }
        }
    }
}
