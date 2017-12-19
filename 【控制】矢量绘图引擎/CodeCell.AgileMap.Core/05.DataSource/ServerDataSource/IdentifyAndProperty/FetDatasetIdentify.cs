using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class FetDatasetIdentify
    {
        private string _id = null;
        private string _name = null;
        private string _description = null;
        private List<FetClassIdentify> _fetclassIds = null;

        public FetDatasetIdentify(string id, string name, string description,FetClassIdentify[] fetclassIds)
        {
            _id = id;
            _name = name;
            _description = description;
            if (fetclassIds != null && fetclassIds.Length > 0)
                _fetclassIds = new List<FetClassIdentify>(fetclassIds);
        }

        public string Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Description
        {
            get { return _description; }
        }

        public FetClassIdentify[] FetClassIds
        {
            get { return _fetclassIds != null && _fetclassIds.Count > 0 ? _fetclassIds.ToArray() : null; }
        }
    }
}
