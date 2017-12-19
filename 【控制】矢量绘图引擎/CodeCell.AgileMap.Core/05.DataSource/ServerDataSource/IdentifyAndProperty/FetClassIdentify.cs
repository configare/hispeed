using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class FetClassIdentify:IPersistable
    {
        private string _id = null;
        private string _name = null;
        private string _description = null;

        public FetClassIdentify(string id, string name,string description)
        {
            _id = id;
            _name = name;
            _description = description;
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

        #region IPersistable Members

        public PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("FetClassIdentify");
            obj.AddAttribute("id", _id != null ? _id : string.Empty);
            obj.AddAttribute("name", _name != null ? _name : string.Empty);
            obj.AddAttribute("description", _description != null ? _description : string.Empty);
            return obj;
        }

        public static FetClassIdentify FromXElement(XElement xele)
        {
            if (xele == null)
                return null;
            string id = xele.Attribute("id").Value;
            string name = xele.Attribute("name").Value;
            string des = xele.Attribute("description").Value;
            return new FetClassIdentify(id, name, des);
        }

        #endregion
    }
}
