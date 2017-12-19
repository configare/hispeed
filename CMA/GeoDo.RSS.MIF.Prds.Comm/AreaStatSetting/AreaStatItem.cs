using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    public class AreaStatItem
    {
        private string _name;
        private string _fileName;
        private enumStatTemplateType _statType;
        private string _menuName;
        private string _infoFileName;
        private string _statField;
        private string[] _columnNames;
      
        public AreaStatItem(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public enumStatTemplateType StatFileType
        {
            get { return _statType; }
            set { _statType = value; }
        }

        public string InfoFileName
        {
            get { return _infoFileName; }
            set { _infoFileName = value; }
        }

        public string MenuName
        {
            get { return _menuName; }
            set { _menuName = value; }
        }

        public string StatField
        {
            get { return _statField; }
            set { _statField = value; }
        }

        public string[] ColumnNames
        {
            get { return _columnNames; }
            set { _columnNames = value; }
        }
    }
}
