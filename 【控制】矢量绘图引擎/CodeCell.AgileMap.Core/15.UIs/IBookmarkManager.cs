using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IBookmarkManager
    {
        IList<Bookmark> Bookmarks { get; }
        void Remove(Bookmark bookmark);
        void Remove(string name);
        void Add(Bookmark bookmark);
        void Add(string name, Envelope location);
        void Save();
    }
}
