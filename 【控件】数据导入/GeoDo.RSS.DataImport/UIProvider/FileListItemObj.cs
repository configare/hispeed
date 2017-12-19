using System.IO;

namespace GeoDo.RSS.DI
{
    public class ImportFilesObj
    {
        private string _proName;
        private string _proIdentify;
        private string _subProName;
        private string _subProIdentify;
        private string _fileName;
        private string _dir;

        public ImportFilesObj(string proName, string subProName, string proIdentify, string subProIdentify, string filename, string dir)
        {
            _proName = proName;
            _subProName = subProName;
            _proIdentify = proIdentify;
            _subProIdentify = subProIdentify;
            _fileName = filename;
            _dir = dir;
        }

        public ImportFilesObj(string proIdentify, string subProIdentify, string filename, string dir)
        {
            _proIdentify = proIdentify;
            _subProIdentify = subProIdentify;
            _fileName = filename;
            _dir = dir;
        }

        public string ProName
        {
            get { return _proName; }
            set { _proName = value; }
        }

        public string SubProName
        {
            get { return _subProName; }
            set { _subProName = value; }
        }

        public string ProIdentify
        {
            get { return _proIdentify; }
            set { _proIdentify = value; }
        }

        public string SubProIdentify
        {
            get { return _subProIdentify; }
            set { _subProIdentify = value; }
        }

        public string FileName
        {
            get { return _fileName; }
        }

        public string Dir
        {
            get { return _dir; }
        }

        public string FullFilename
        {
            get
            {
                if (string.IsNullOrEmpty(_dir) || string.IsNullOrEmpty(_fileName))
                    return string.Empty;
                else
                    return Path.Combine(_dir, _fileName);
            }
        }
    }
}
