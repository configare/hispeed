using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.Core.RasterDrawing
{
    internal class FileIndexer
    {
        protected class AccessedFile
        {
            public string FilePath;
            public DateTime LastTime;

            public AccessedFile(string filePath, DateTime lastTime)
            {
                FilePath = filePath;
                LastTime = lastTime;
            }
        }

        protected const string FILE_INDEXER_NAME = "cmart.cache";
        protected string _cacheDir;
        protected string _fileName;
        protected Dictionary<string, AccessedFile> _fullFileNames = new Dictionary<string, AccessedFile>();

        public FileIndexer(string cacheDir)
        {
            _cacheDir = cacheDir;
            _fileName = Path.Combine(cacheDir, FILE_INDEXER_NAME);
            if (File.Exists(_fileName))
            {
                LoadSmartCacheFile(_fileName);
            }
        }

        public string GetOldestCacheDir()
        {
            if (_fullFileNames.Count == 0)
                return null;
            DateTime dt = DateTime.Now ;
            string oldDir = null;
            foreach (string dir in _fullFileNames.Keys)
            {
                if (_fullFileNames[dir].LastTime < dt)
                {
                    dt = _fullFileNames[dir].LastTime;
                    oldDir = dir;
                }
            }
            if (oldDir == null) //count == 1
                oldDir = _fullFileNames.Keys.ToArray()[0];
            _fullFileNames.Remove(oldDir);
            return Path.Combine(_cacheDir, oldDir);
        }

        private void LoadSmartCacheFile(string fileName)
        {
            string[] allLines = File.ReadAllLines(fileName, Encoding.Default);
            if (allLines == null || allLines.Length == 0)
                return;
            foreach (string aFile in allLines)
            {
                string[] parts = aFile.Split(',');
                if (parts == null || parts.Length != 3 || _fullFileNames.ContainsKey(parts[0]))
                    continue;
                if (Directory.Exists(Path.Combine(_cacheDir, parts[0])) &&
                    File.Exists(parts[1]))
                {
                    _fullFileNames.Add(parts[0], new AccessedFile(parts[1], DateTime.Parse(parts[2])));
                }
            }
        }

        public string GetFileDir(string fName)
        {
            string retDir;
            if (_fullFileNames.Count > 0)
            {
                foreach (string aliasName in _fullFileNames.Keys)
                {
                    if (_fullFileNames[aliasName].FilePath == fName)
                    {
                        retDir = Path.Combine(_cacheDir, aliasName);
                        goto returnLine;
                    }
                }
            }
            string aName = Guid.NewGuid().ToString().ToUpper();
            _fullFileNames.Add(aName, new AccessedFile(fName,DateTime.Now));
            retDir = Path.Combine(_cacheDir, aName);
        returnLine:
            if (!Directory.Exists(retDir))
                Directory.CreateDirectory(retDir);
            return retDir;
        }

        public void Save()
        {
            if (_fullFileNames == null || _fullFileNames.Count == 0)
                return;
            List<string> allLines = new List<string>();
            foreach (string aliasName in _fullFileNames.Keys)
            {
                allLines.Add(aliasName + "," + _fullFileNames[aliasName].FilePath + "," + _fullFileNames[aliasName].LastTime.ToString());
            }
            File.WriteAllLines(_fileName, allLines.ToArray(), Encoding.Default);
        }
    }
}
