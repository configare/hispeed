using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.Smart.Tools.MODLSTPro
{
    public class FileListItem
    {
        private string _fileName;
        private RasterIdentify _rid = null;
        private string _srcFilename = null;

        public FileListItem(string filename, RasterIdentify fins)
        {
            _fileName = filename;
            _rid = fins;
        }

        public string FileName
        {
            get { return _fileName; }
        }

        public RasterIdentify Rid
        {
            get { return _rid; }
            set { _rid = value; }
        }

        public string SrcFilename
        {
            get { return _srcFilename; }
            set { _srcFilename = value; }
        }
    }
}
