using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.Core.UI
{
    public class RecentFilesManager:IRecentFilesManager
    {
        public delegate void OnAddOneFileHandler(object sender, string filename);
        public delegate void OnRemoveOneFileHandler(object sender, string filename);
        private int _maxRecentFilesCount = 20;
        private List<string> _recentUsedFiles = new List<string>();
        public event OnAddOneFileHandler OnAddOneFile = null;
        public event OnRemoveOneFileHandler OnRemoveOneFile = null;

        public RecentFilesManager(int maxRecentFilesCount)
        {
            _maxRecentFilesCount = maxRecentFilesCount;
            LoadRecentUsedFiles();
        }

        public string[] GetRecentUsedFiles()
        {
            return _recentUsedFiles.Count > 0 ? _recentUsedFiles.ToArray() : null;
        }

        public void Remove(string fname)
        {
            if(!string.IsNullOrEmpty(fname) && _recentUsedFiles.Contains(fname))
                _recentUsedFiles.Remove(fname);
        }

        public void AddFile(string filename)
        {
            try
            {
                if (_recentUsedFiles.Count > 0 && _recentUsedFiles.Count == _maxRecentFilesCount)
                {
                    string f = _recentUsedFiles[_recentUsedFiles.Count - 1];
                    _recentUsedFiles.RemoveAt(_recentUsedFiles.Count - 1);
                    if (OnRemoveOneFile != null)
                        OnRemoveOneFile(this, f);
                }
                if (_recentUsedFiles.Contains(filename))
                {
                    if (OnRemoveOneFile != null)
                        OnRemoveOneFile(this, filename);
                    _recentUsedFiles.Remove(filename);
                }
                _recentUsedFiles.Insert(0, filename);
                if (OnAddOneFile != null)
                {
                    OnAddOneFile(this, filename);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        public void SaveToDisk()
        {
            try
            {
                string mainfestfile = AppDomain.CurrentDomain.BaseDirectory + "RecentUsedFiles.mainfestfile";
                if (File.Exists(mainfestfile))
                    File.Delete(mainfestfile);
                List<string> fs = new List<string>();
                foreach (string f in _recentUsedFiles)
                    if (IsCanAddToRectent(f))
                        fs.Add(f);
                if (fs.Count > 0)
                    File.WriteAllLines(mainfestfile, fs.ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private bool IsCanAddToRectent(string filename)
        {
            string dir = Path.GetDirectoryName(filename);
            if (filename.ToUpper().Contains(dir.ToUpper()) && filename.ToUpper().EndsWith(".HDFRAW"))
                return false;
            return true;
        }

        public void LoadRecentUsedFiles()
        {
            try
            {
                string mainfestfile = AppDomain.CurrentDomain.BaseDirectory + "RecentUsedFiles.mainfestfile";
                if (File.Exists(mainfestfile))
                {
                    string[] lines = File.ReadAllLines(mainfestfile);
                    if (lines == null || lines.Length == 0)
                        return;
                    Array.Reverse(lines);
                    foreach (string f in lines)
                    {
                        if (!IsCanAddToRectent(f))
                            continue;
                        _recentUsedFiles.Insert(0, f);
                        if (OnAddOneFile != null)
                            OnAddOneFile(this, f);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
