using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class InstanceIdentify:IPersistable
    {
        private string _name = null;
        private string _descriptioin = null;
        private int _id = -1;

        public InstanceIdentify(int id, string name, string description)
        {
            _id = id;
            _name = name;
            _descriptioin = description;
        }

        public int Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Description
        {
            get { return _descriptioin; }
        }

        #region IPersistable Members

        public PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("InstanceIdentify");
            obj.AddAttribute("id", _id.ToString());
            obj.AddAttribute("name", _name != null ? _name : string.Empty);
            obj.AddAttribute("description", _descriptioin != null ? _descriptioin : string.Empty);
            return obj;
        }

        public static InstanceIdentify FromXElement(XElement xele)
        {
            if (xele == null)
                return null;
            int id = int.Parse(xele.Attribute("id").Value);
            string name = xele.Attribute("name").Value;
            string des = xele.Attribute("description").Value;
            return new InstanceIdentify(id, name, des);
        }

        #endregion
    }
}
