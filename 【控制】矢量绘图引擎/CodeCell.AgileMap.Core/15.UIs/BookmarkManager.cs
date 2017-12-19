using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeCell.AgileMap.Core
{
    public class BookmarkManager : IBookmarkManager
    {
        private List<Bookmark> _bookmarks = new List<Bookmark>();
        private string _storefile = null;

        public BookmarkManager()
        {
            _storefile = AppDomain.CurrentDomain.BaseDirectory + "bookmark.txt";
            Init();
        }

        public BookmarkManager(string storefile)
        {
            _storefile = storefile;
            Init();
        }

        private void Init()
        {
            if (!File.Exists(_storefile))
                return;
            using (FileStream fs = new FileStream(_storefile, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                {
                    while (!sr.EndOfStream)
                    {
                        Bookmark bk = Bookmark.FromString(sr.ReadLine());
                        if (bk != null)
                            _bookmarks.Add(bk);
                    }
                }
            }
        }

        public IList<Bookmark> Bookmarks
        {
            get { return _bookmarks; }
        }

        public void Remove(Bookmark bookmark)
        {
            if (_bookmarks.Contains(bookmark))
                _bookmarks.Remove(bookmark);
        }

        public void Remove(string name)
        {
            if (name == null)
                return;
            foreach (Bookmark bk in _bookmarks)
                if (bk.Name == name)
                {
                    _bookmarks.Remove(bk);
                    return;
                }
        }

        public void Add(Bookmark bookmark)
        {
            if (bookmark == null || !_bookmarks.Contains(bookmark))
                _bookmarks.Add(bookmark);
        }

        public void Add(string name, Envelope location)
        {
            if (name == null || location == null)
                return;
            _bookmarks.Add(new Bookmark(name, location));
        }

        public void Save()
        {
            if (_bookmarks.Count == 0)
                return;
            StringBuilder sb = new StringBuilder();
            foreach (Bookmark bk in _bookmarks)
                sb.AppendLine(bk.ToString());
            using (FileStream fs = new FileStream(_storefile, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                {
                    sw.Write(sb.ToString());
                }
            }
        }
    }
}
