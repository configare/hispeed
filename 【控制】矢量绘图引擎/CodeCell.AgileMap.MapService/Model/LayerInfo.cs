using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CodeCell.AgileMap.Core;
using System.Runtime.Serialization;

namespace CodeCell.AgileMap.MapService
{
    [DataContract]
    public class LayerInfo
    {
        private string _id = null;
        private string _name = null;
        private enumShapeType _shapeType = enumShapeType.NullShape;

        public LayerInfo(string name,string id, enumShapeType shapeType)
        {
            _name = name;
            _id = id;
            _shapeType = shapeType;
        }

        [DataMember]
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [DataMember]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [DataMember]
        public enumShapeType ShapeType
        {
            get { return _shapeType; }
            set { _shapeType = value; }
        }
    }
}