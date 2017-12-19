using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class FileExtractResult:IFileExtractResult
    {
        protected string _fileName;
        protected string _name;
        protected bool _display = true;
        protected string _outIdentify;
        protected bool _add2Workspace = true;

        public FileExtractResult(string name,string fileName)
        {
            _name = name;
            _fileName = fileName;
        }

        public FileExtractResult(string name, string fileName,bool isAdd2Workspace)
        {
            _name = name;
            _fileName = fileName;
            _add2Workspace = isAdd2Workspace;
        }

        public void SetDispaly(bool display)
        {
            _display = display;
        }

        public bool Add2Workspace
        {
            get { return _add2Workspace; }
            set { _add2Workspace = value; }
        }

        public bool Display
        {
            get
            {
                return _display;
            }
        }

        public string Name
        {
            get { return _name; }
        }

        public string FileName
        {
            get { return _fileName; }
        }

        public void SetOutIdentify(string outIdentify)
        {
            _outIdentify = outIdentify;
        }
      
        public string OutIdentify
        {
            get { return _outIdentify; }
        }
    }
}
