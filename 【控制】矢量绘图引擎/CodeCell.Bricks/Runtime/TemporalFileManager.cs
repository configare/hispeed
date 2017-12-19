using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeCell.Bricks.Runtime
{
    public class TemporalFileManager:/*UnviersalObject,*/ IQueueTaskItem
    {
        private readonly string _temporalFileDir = null;
        private List<string> _temoralFilenames = null;

        public TemporalFileManager()
        {
            _temporalFileDir = AppDomain.CurrentDomain.BaseDirectory + "TemporalFileDir";
            if (!Directory.Exists(_temporalFileDir))
                TryCreateDir(_temporalFileDir);
            LoadPreviousLostTemporalFiles();
        }

        public TemporalFileManager(string temporalFileDir)
        {
            _temporalFileDir = temporalFileDir;
            if (!Directory.Exists(_temporalFileDir))
                TryCreateDir(_temporalFileDir);
            LoadPreviousLostTemporalFiles();
        }

        private void LoadPreviousLostTemporalFiles()
        {
            string[] fname = Directory.GetFiles(_temporalFileDir);
            if (fname != null || fname.Length > 0)
            {
                _temoralFilenames = new List<string>();
                foreach (string f in fname)
                    _temoralFilenames.Add(f);
            }
        }

        private void TryCreateDir(string temporalFileDir)
        {
            try
            {
                Directory.CreateDirectory(temporalFileDir);
            }
            catch 
            {
                throw;
            }
        }

        public string NextTemporalFilename(string extName,string[] secondaryExtnames)
        {
            if (_temoralFilenames == null)
                _temoralFilenames = new List<string>();
            string guid = _temporalFileDir + "\\"+ Guid.NewGuid().ToString();
            _temoralFilenames.Add(guid + extName);
            if (secondaryExtnames != null && secondaryExtnames.Length > 0)
                foreach (string extname in secondaryExtnames)
                    _temoralFilenames.Add(guid + extname);
            return guid + extName;
        }

        public static int NextRandomIndex = 0;
        public string NextTemporalFilename(string suggestFilename, string extName, string[] secondaryExtnames)
        {
            string mainFilename = _temporalFileDir + "\\"  +Path.GetFileNameWithoutExtension(suggestFilename);
            if (File.Exists(_temporalFileDir + "\\" +suggestFilename))
            {
                mainFilename += ("("+NextRandomIndex.ToString()+")");
                NextRandomIndex++;
            }
            _temoralFilenames.Add(mainFilename + extName);
            if (secondaryExtnames != null && secondaryExtnames.Length > 0)
                foreach (string extname in secondaryExtnames)
                    _temoralFilenames.Add(mainFilename + extname);
            return mainFilename + extName;
        }

        #region IQueueTaskItem 成员

        public string Name
        {
            get { return "清除临时文件"; }
        }

        public void Do(IProgressTracker tracker)
        {
            if (_temoralFilenames == null || _temoralFilenames.Count == 0)
                return;
            foreach (string fname in _temoralFilenames)
            {
                try
                {
                    File.Delete(fname);
                }
                catch(Exception ex)
                {
                    Log.WriterException(ex);
                }
            }
        }

        #endregion
    }
}
