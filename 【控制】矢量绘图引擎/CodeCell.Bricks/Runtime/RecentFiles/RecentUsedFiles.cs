using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeCell.Bricks.Runtime
{
    public class RecentUsedFiles
    {
        public delegate void OnAddOneFileHandler(object sender,string filename);
        public delegate void OnRemoveOneFileHandler(object sender, string filename);
        private int _maxRecentFilesCount = 20;
        private List<string> _recentUsedFiles = new List<string>();
        public event OnAddOneFileHandler OnAddOneFile = null;
        public event OnRemoveOneFileHandler OnRemoveOneFile = null;
        private string _tempDir = null;

        public RecentUsedFiles(int maxRecentFilesCount,string tempDir)
        {
            _tempDir = tempDir;
            _maxRecentFilesCount = maxRecentFilesCount;
        }

        public string[] GetRecentUsedFiles()
        {
            return _recentUsedFiles.Count > 0 ? _recentUsedFiles.ToArray() : null;
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
                Log.WriterException("RecentUsedFiles", "AddFile", ex);
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
                if(fs.Count>0)
                    File.WriteAllLines(mainfestfile,fs.ToArray());
            }
            catch(Exception e)
            {
                Log.WriterException("RecentUsedFiles", "SaveToDisk", e);
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
                        _recentUsedFiles.Insert(0,f);
                        if (OnAddOneFile != null)
                            OnAddOneFile(this, f);
                    }
                }
            }
            catch (Exception e)
            {
                Log.WriterException("RecentUsedFiles", "LoadRecentUsedFiles", e);
            }
        }
    }
}
