using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Telerik.WinControls
{
    /// <summary>
    /// Supports metadata for each class inherited from <see cref="RadObject"/>
    /// </summary>
    public class RadObjectType
    {
        static RadObjectType()
        {
            RadObjectType.DTypeFromCLRType = new Hashtable();
            RadObjectType.DTypeCount = 0;
            RadObjectType._lock = new object();
        }

        private RadObjectType()
        {
        }

        public static RadObjectType FromSystemType(Type systemType)
        {
            RadObjectType type1;
            if (systemType == null)
            {
                throw new ArgumentNullException("systemType");
            }
            if (!typeof(RadObject).IsAssignableFrom(systemType))
            {
                throw new ArgumentException("DTypeNotSupportForSystemType");
            }
            Queue queue1 = null;
            lock (RadObjectType._lock)
            {
                type1 = RadObjectType.FromSystemTypeRecursive(systemType, ref queue1);
                goto Label_0071;
            }
        Label_0061:
            RuntimeHelpers.RunClassConstructor((RuntimeTypeHandle)queue1.Dequeue());
        Label_0071:
            if ((queue1 == null) || (queue1.Count <= 0))
            {
                return type1;
            }
            goto Label_0061;
        }


        private static RadObjectType FromSystemTypeRecursive(Type systemType, ref Queue encounteredTypes)
        {
            RadObjectType type1 = (RadObjectType)RadObjectType.DTypeFromCLRType[systemType];
            if (type1 == null)
            {
                if (encounteredTypes == null)
                {
                    encounteredTypes = new Queue(10);
                }
                type1 = new RadObjectType();
                type1._systemType = systemType;
                RadObjectType.DTypeFromCLRType[systemType] = type1;
                if (systemType != typeof(RadObject))
                {
                    type1._baseDType = RadObjectType.FromSystemTypeRecursive(systemType.BaseType, ref encounteredTypes);
                }
                type1._id = RadObjectType.DTypeCount++;
                encounteredTypes.Enqueue(systemType.TypeHandle);
            }
            return type1;
        }


        public override int GetHashCode()
        {
            return this._id;
        }

        public bool IsInstanceOfType(RadObject radObject)
        {
            if (radObject != null)
            {
                RadObjectType type1 = radObject.RadObjectType;
                do
                {
                    if (type1.Id == this.Id)
                    {
                        return true;
                    }
                    type1 = type1._baseDType;
                }
                while (type1 != null);
            }
            return false;
        }


        public bool IsSubclassOf(RadObjectType radObjectType)
        {
            if (radObjectType != null)
            {
                for (RadObjectType type1 = this._baseDType; type1 != null; type1 = type1._baseDType)
                {
                    if (type1.Id == radObjectType.Id)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public RadObjectType BaseType
        {
            get
            {
                return this._baseDType;
            }
        }


        public int Id
        {
            get
            {
                return this._id;
            }
        }

        public string Name
        {
            get
            {
                return this.SystemType.Name;
            }
        }


        public Type SystemType
        {
            get
            {
                return this._systemType;
            }
        }


        private RadObjectType _baseDType;
        private int _id;
        private static object _lock;
        private Type _systemType;
        private static int DTypeCount;
        private static Hashtable DTypeFromCLRType;

        internal RadProperty[] GetRadProperties()
        {
            ArrayList list = new ArrayList();

            //foreach (RadProperty property in RadProperty.RegisteredPropertyList.List)
            for (int i = 0; i < RadProperty.RegisteredPropertyList.List.Length; i++)
            {
                RadProperty property = RadProperty.RegisteredPropertyList.List[i];
                if (property != null && property.OwnerType == this.SystemType)
                    list.Add(property);
            }

            return (RadProperty[])list.ToArray(typeof(RadProperty));
        }
    }
}
