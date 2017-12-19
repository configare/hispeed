using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Core
{
    public class XmlPersistAttribute:Attribute
    {
        private bool _isCollectionProperty = false;
        private string _collectionItemName;
        private Type _collectionItemType;
        private Type _propertyConverter;
        private bool _isNeedPersisted = true;

        public XmlPersistAttribute(bool isNeedPersisted)
        {
            _isNeedPersisted = isNeedPersisted;
        }

        public XmlPersistAttribute(Type propertyConverter)
        {
            _propertyConverter = propertyConverter;
        }

        public XmlPersistAttribute(string collectionItemName,Type collectionItemType)
        {
            _collectionItemName = collectionItemName;
            _collectionItemType = collectionItemType;
        }

        public bool IsNeedPersisted
        {
            get { return _isNeedPersisted; }
        }

        public string CollectionItemName
        {
            get { return _collectionItemName; }
        }

        public Type CollectionItemType
        {
            get { return _collectionItemType; }
        }

        public bool IsCollectionProperty 
        {
            get { return _isCollectionProperty; }
        }

        public Type PropertyConverter
        {
            get { return _propertyConverter; }
        }
    }
}
