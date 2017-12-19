using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace CodeCell.AgileMap.MapService
{
    [DataContract]
    public class MapInfo
    {
        private string _name = null;
        private string _spatialRef = null;

        public MapInfo(string name, string spatialRef)
        {
            _name = name;
            _spatialRef = spatialRef;
        }

        [DataMember]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [DataMember]
        public string SpatialRef
        {
            get { return _spatialRef; }
            set { _spatialRef = value; }
        }
    }
}