using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public class LocationIcon
    {
        private string _text = null;
        private Feature _feature = null;

        public LocationIcon(string text,Feature feature)
        {
            _text = text;
            _feature = feature;
        }

        public string Text
        {
            get { return _text; }
        }

        public Feature Feature
        {
            get { return _feature; }
        }
    }
}
