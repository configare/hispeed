using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public class Bookmark
    {
        private string _name = null;
        private Envelope _location = null;

        public Bookmark() 
        {
        }

        public Bookmark(string name, Envelope location)
        {
            _name = name;
            _location = location;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Envelope Location
        {
            get { return _location; }
            set { _location = value; }
        }

        public override string ToString()
        {
            if (_location == null)
                return string.Empty;
            return (_name ?? string.Empty) + "=" + _location.ToString();
        }

        public static Bookmark FromString(string bookmark)
        {
            if (string.IsNullOrEmpty(bookmark))
                return null;
            string[] parts = bookmark.Split('=');
            if (parts == null || parts.Length != 2)
                return null;
            Envelope location = null;
            if (Envelope.TryParse(parts[1], out location))
            {
                return new Bookmark(parts[0], location);
            }
            return null;
        }
    }
}
