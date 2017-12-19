using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public interface IRecentFilesManager
    {
        string[] GetRecentUsedFiles();
        void AddFile(string fileName);
        void SaveToDisk();
        void LoadRecentUsedFiles();
        void Remove(string fileName);
    }
}
