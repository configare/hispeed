using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.UI.AddIn.DataRef
{
    public class BookMarkGroup
    {
        private string _groupName = string.Empty;
        private Dictionary<string, CoordEnvelope> _bookMarks = null;

        public BookMarkGroup(string groupName, Dictionary<string, CoordEnvelope> bookMarks)
        {
            _groupName = groupName;
            _bookMarks = bookMarks;
        }

        public string GroupName
        {
            get { return _groupName; }
            set { _groupName = value; }
        }

        /// <summary>
        /// 地理坐标
        /// </summary>
        public Dictionary<string, CoordEnvelope> BookMarks
        {
            get { return _bookMarks; }
            set { _bookMarks = value; }
        }
    }
}
