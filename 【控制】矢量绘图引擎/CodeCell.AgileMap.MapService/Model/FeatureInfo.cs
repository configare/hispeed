using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.MapService
{
    [DataContract]
    public class FeatureInfo
    {
        private int _oid = -1;
        private Dictionary<string, string> _properties = new Dictionary<string, string>();
        private Envelope _envelope = null;

        public FeatureInfo()
        { 
        }

        public FeatureInfo(int oid)
        {
            _oid = oid;
        }

        [DataMember]
        public int OID
        {
            get { return _oid; }
            set { _oid = value; }
        }

        [DataMember]
        public Dictionary<string, string> Properties
        {
            get { return _properties; }
            set { _properties = value; }
        }

        [DataMember]
        public Envelope Envelope
        {
            get { return _envelope; }
            set { _envelope = value; }
        }
    }
}