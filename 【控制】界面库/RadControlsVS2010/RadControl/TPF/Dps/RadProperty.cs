using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Telerik.WinControls
{
    public delegate bool ValidateValueCallback(object value, RadObject instance);

    public class RadPropertyNotFoundException: ArgumentException
    {
        private readonly string propertyName;
        private readonly string typeName;

        internal RadPropertyNotFoundException(string propertyName, string typeName)
            : base(string.Format("No such property registered: {0}, {1}", propertyName, typeName))
        {
            this.propertyName = propertyName;
            this.typeName = typeName;
        }

        public string TypeName
        {
            get { return typeName; }
        }

        public string PropertyName
        {
            get { return propertyName; }
        }
    }

    /// <summary>
    /// Represents a property. Supports telerik dependency properties system by
    /// encapsulating a property of a certain RadElement instance. 
    /// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
    public class RadProperty
    {
        //Syhchronization semaphore
        internal static object Synchronized;

        internal InsertionSortMap _metadataMap;
        private string _name;
        private Type _propertyType;
        private Type _ownerType;
        private RadPropertyMetadata _defaultMetadata;
        private ValidateValueCallback _validateValueCallback;
        private static int GlobalIndexCount;
        private int _globalIndex;
		private bool? propertyTypeIsValueType = null;
        private static Type NullableType;
        private int nameHashCode;

        //public fields
        public static readonly object UnsetValue;

        //properties table
        private static Hashtable PropertyFromName;

        //list of RT registered properties
        internal static ItemStructList<RadProperty> RegisteredPropertyList;
        private FromNameKey key = null;

        #region FromNameKey class declaration
        internal class FromNameKey
        {
            // Fields
            private int _hashCode;
            private string _name;
            private Type _ownerType;

            // Methods
            public FromNameKey(string name, Type ownerType)
            {
                this._name = name;
                this._ownerType = ownerType;
                this._hashCode = this._name.GetHashCode() ^ this._ownerType.GetHashCode();
            }

            public override bool Equals(object o)
            {
                if ((o != null) && (o is RadProperty.FromNameKey))
                {
                    return this.Equals((RadProperty.FromNameKey)o);
                }
                return false;
            }

            public bool Equals(RadProperty.FromNameKey key)
            {
                if (this._name.Equals(key._name))
                {
                    return (this._ownerType == key._ownerType);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return this._hashCode;
            }

            public void UpdateNameKey(Type ownerType)
            {
                this._ownerType = ownerType;
                this._hashCode = this._name.GetHashCode() ^ this._ownerType.GetHashCode();
            }

            public override string ToString()
            {
                return this._ownerType.FullName + "." + this._name;
            }
        }
        #endregion

        static RadProperty()
        {
            RadProperty.UnsetValue = new object();
            RadProperty.RegisteredPropertyList = new ItemStructList<RadProperty>(0x180);
            RadProperty.PropertyFromName = new Hashtable();
            RadProperty.Synchronized = new object();
            RadProperty.NullableType = typeof(Nullable<>);
        }

        private RadProperty(string name, Type propertyType, Type ownerType, RadPropertyMetadata defaultMetadata, ValidateValueCallback validateValueCallback)
        {
            this._metadataMap = new InsertionSortMap();
            this._name = name;
            this._propertyType = propertyType;
            this._ownerType = ownerType;
            this._defaultMetadata = defaultMetadata;
            this._validateValueCallback = validateValueCallback;
            lock (RadProperty.Synchronized)
            {
                this._globalIndex = RadProperty.GetUniqueGlobalIndex(ownerType, name);
                this.nameHashCode = name.GetHashCode();
                RadProperty.RegisteredPropertyList.Add(this);
            }
        }

        public override int GetHashCode()
        {
            return this._globalIndex;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        private static void RegisterParameterValidation(string name, Type propertyType, Type ownerType)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (name.Length == 0)
            {
                throw new ArgumentException("name");
            }
            if (ownerType == null)
            {
                throw new ArgumentNullException("ownerType");
            }
            if (propertyType == null)
            {
                throw new ArgumentNullException("propertyType");
            }
        }

        public static RadProperty Register(string name, Type propertyType, Type ownerType, RadPropertyMetadata typeMetadata)
        {
            return RadProperty.Register(name, propertyType, ownerType, typeMetadata, null);
        }

        public static RadProperty Register(string name, Type propertyType, Type ownerType, RadPropertyMetadata typeMetadata, ValidateValueCallback validateValueCallback)
        {
            //RadProperty.RegisterParameterValidation(name, propertyType, ownerType);
            RadPropertyMetadata metadata1 = null;
            if ((typeMetadata != null) && typeMetadata.DefaultValueWasSet())
            {
                metadata1 = new RadPropertyMetadata(typeMetadata.DefaultValue);
            }
            RadProperty property1 = RadProperty.RegisterCommon(name, propertyType, ownerType, AttachedPropertyUsage.Self, metadata1, validateValueCallback);
            if (typeMetadata != null)
            {
                property1.OverrideMetadata(ownerType, typeMetadata);
            }
            return property1;
        }

        public static RadProperty RegisterAttached(string name, Type propertyType, Type ownerType, RadPropertyMetadata typeMetadata)
        {
            return RadProperty.RegisterAttached(name, propertyType, ownerType, typeMetadata, null);
        }

        public static RadProperty RegisterAttached(string name, Type propertyType, Type ownerType, RadPropertyMetadata typeMetadata, ValidateValueCallback validateValueCallback)
        {
            RegisterParameterValidation(name, propertyType, ownerType);
            return RegisterCommon(name, propertyType, ownerType, AttachedPropertyUsage.Anywhere, typeMetadata, validateValueCallback);
        }

        public RadProperty AddOwner(Type ownerType)
        {
            return this.AddOwner(ownerType, null);
        }

        public RadProperty AddOwner(Type ownerType, RadPropertyMetadata typeMetadata)
        {
            if (ownerType == null)
            {
                throw new ArgumentNullException("ownerType");
            }
            if (this._defaultMetadata.ReadOnly && (this._defaultMetadata.AttachedPropertyUsage != AttachedPropertyUsage.Self))
            {
                object[] objArray1 = new object[] { this.Name };
                throw new InvalidOperationException(string.Format("Cannot Add Owner For Attached ReadOnly Property: {0}", objArray1));
            }
            RadProperty.FromNameKey key1 = new RadProperty.FromNameKey(this.Name, ownerType);
            lock (RadProperty.Synchronized)
            {
                if (RadProperty.PropertyFromName.Contains(key1))
                {
                    object[] objArray2 = new object[] { this.Name, ownerType.Name };
                    throw new ArgumentException(string.Format("Property Already Registered {0}, {1}", objArray2));
                }
            }
            if ((typeMetadata == null) && (this._defaultMetadata.AttachedPropertyUsage != AttachedPropertyUsage.Self))
            {
                typeMetadata = this._defaultMetadata.Copy(this);
            }
            if (typeMetadata != null)
            {
                typeMetadata.SetAttachedPropertyUsage(AttachedPropertyUsage.Self);
                this.OverrideMetadata(ownerType, typeMetadata);
            }
            lock (RadProperty.Synchronized)
            {
                RadProperty.PropertyFromName[key1] = this;
            }
            return this;
        }

        public void OverrideMetadata(Type forType, RadPropertyMetadata typeMetadata)
        {
            RadObjectType type1;
            RadPropertyMetadata metadata1;
            this.SetupOverrideMetadata(forType, typeMetadata, out type1, out metadata1);
            if (metadata1.ReadOnly)
            {
                object[] objArray1 = new object[] { this.Name };
                throw new InvalidOperationException(string.Format("ReadOnlyOverrideNotAllowed {0}", objArray1));
            }

            this.ProcessOverrideMetadata(forType, typeMetadata, type1, metadata1);
        }

        private void ProcessOverrideMetadata(Type forType, RadPropertyMetadata typeMetadata, RadObjectType dType, RadPropertyMetadata baseMetadata)
        {
            lock (RadProperty.Synchronized)
            {
                if (RadProperty.UnsetValue == this._metadataMap[dType.Id])
                {
                    this._metadataMap[dType.Id] = typeMetadata;
                }
                else
                {
                    object[] objArray1 = new object[] { forType.Name };
                    throw new ArgumentException(string.Format("TypeMetadataAlreadyRegistered {0}", objArray1));
                }
            }

            typeMetadata.InvokeMerge(baseMetadata, this);
            typeMetadata.Seal(this, forType);
        }


        private void SetupOverrideMetadata(Type forType, RadPropertyMetadata typeMetadata, out RadObjectType dType, out RadPropertyMetadata baseMetadata)
        {
            if (forType == null)
            {
                throw new ArgumentNullException("forType");
            }
            if (typeMetadata == null)
            {
                throw new ArgumentNullException("typeMetadata");
            }
            if (typeMetadata.Sealed)
            {
                throw new ArgumentException(string.Format("TypeMetadataAlreadyInUse", new object[0]));
            }
            if (!typeof(RadObject).IsAssignableFrom(forType))
            {
                object[] objArray1 = new object[] { forType.Name };
                throw new ArgumentException(string.Format("TypeMustBeRadObjectDerived {0}", objArray1));
            }
            if (typeMetadata.IsDefaultValueModified)
            {
                RadProperty.ValidateMetadataDefaultValue(typeMetadata, this.PropertyType, this.ValidateValueCallback);
            }
            dType = RadObjectType.FromSystemType(forType);
            baseMetadata = this.GetMetadata(dType.BaseType);
            if (!baseMetadata.GetType().IsAssignableFrom(typeMetadata.GetType()))
            {
                throw new ArgumentException(string.Format("OverridingMetadataDoesNotMatchBaseMetadataType", new object[0]));
            }
        }

        private static void ValidateMetadataDefaultValue(RadPropertyMetadata defaultMetadata, Type propertyType, ValidateValueCallback validateValueCallback)
        {
            /*if (defaultMetadata.DefaultValue != DefaultValueOptions.Instance)
            {
                RadProperty.ValidateDefaultValueCommon(defaultMetadata.DefaultValue, propertyType, validateValueCallback);
            }*/
        }

        public Type PropertyType
        {
            get
            {
                return this._propertyType;
            }
        }		

		public bool PropertyTypeIsValueType
		{
			get
			{
				if (propertyTypeIsValueType == null || !propertyTypeIsValueType.HasValue)
				{
					propertyTypeIsValueType = this.PropertyType.IsValueType;
				}

				return propertyTypeIsValueType.Value;
			}
		}

        public string Name
        {
            get
            {
                return this._name;
            }
        }

        public Type OwnerType
        {
            get
            {
                return this._ownerType;
            }
        }

        public bool IsValidValue(object value, RadObject instance)
        {
            if (!RadProperty.IsValidType(value, this.PropertyType))
            {
                return false;
            }
            if (this.ValidateValueCallback != null)
            {
                return this.ValidateValueCallback(value, instance);
            }
            return true;
        }

        public bool IsValidType(object value)
        {
            return RadProperty.IsValidType(value, this.PropertyType);
        }

        internal static bool IsValidType(object value, Type propertyType)
        {
            if (value == null)
            {
                if (propertyType.IsValueType && (!propertyType.IsGenericType || (propertyType.GetGenericTypeDefinition() != RadProperty.NullableType)))
                {
                    return false;
                }
            }
            else if (!propertyType.IsInstanceOfType(value))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the hashcode of the Name string. Pre-computed for faster dictionary operations.
        /// </summary>
        internal int NameHash
        {
            get
            {
                return this.nameHashCode;
            }
        }

        public ValidateValueCallback ValidateValueCallback
        {
            get
            {
                return this._validateValueCallback;
            }
        }

        internal static int GetUniqueGlobalIndex(Type ownerType, string name)
        {
            if (RadProperty.GlobalIndexCount < FrugalMapBase.INVALIDKEY)
            {
                return RadProperty.GlobalIndexCount++;
            }
            if (ownerType != null)
            {
                throw new InvalidOperationException("Too many RadProperties to create a new property: " + ownerType.Name + "." + name);
            }

            throw new InvalidOperationException("Too many RadProperties to create a constant property");
        }


        private static RadProperty RegisterCommon(string name, Type propertyType, Type ownerType, AttachedPropertyUsage usage, RadPropertyMetadata defaultMetadata, ValidateValueCallback validateValueCallback)
        {
            RadProperty.FromNameKey key1 = new RadProperty.FromNameKey(name, ownerType);
            lock (RadProperty.Synchronized)
            {
                if (RadProperty.PropertyFromName.Contains(key1))
                {
                    object[] objArray1 = new object[] { name, ownerType.Name };
                    throw new ArgumentException(string.Format("Property already registered", objArray1));
                }
            }
            if (defaultMetadata == null)
            {
                defaultMetadata = RadProperty.AutoGeneratePropertyMetadata(propertyType, validateValueCallback, name, ownerType);
            }
            else
            {
                if (!defaultMetadata.DefaultValueWasSet())
                {
                    defaultMetadata.DefaultValue = RadProperty.AutoGenerateDefaultValue(propertyType);
                }
                //TODO:
                //RadProperty.ValidateMetadataDefaultValue(defaultMetadata, propertyType, validateValueCallback);
            }
            defaultMetadata.SetAttachedPropertyUsage(usage);
            RadProperty property1 = new RadProperty(name, propertyType, ownerType, defaultMetadata, validateValueCallback);
            defaultMetadata.Seal(property1, null);
            lock (RadProperty.Synchronized)
            {
                RadProperty.PropertyFromName[key1] = property1;
            }

            return property1;
        }

        private static RadPropertyMetadata AutoGeneratePropertyMetadata(Type propertyType, ValidateValueCallback validateValueCallback, string name, Type ownerType)
        {
            return new RadPropertyMetadata(RadProperty.AutoGenerateDefaultValue(propertyType));
        }

        private static object AutoGenerateDefaultValue(Type propertyType)
        {
            object obj1 = null;
            if (propertyType.IsValueType)
            {
                obj1 = Activator.CreateInstance(propertyType);
            }

            return obj1;
        }

        public RadPropertyMetadata GetMetadata(RadObject radObject)
        {
            if (radObject == null)
            {
                throw new ArgumentNullException("radObject");
            }
            return this.GetMetadata(radObject.RadObjectType);
        }

        public RadPropertyMetadata GetMetadata(RadObjectType radObjectType)
        {
            if (radObjectType != null)
            {
                int num2;
                object obj1;
                int num1 = this._metadataMap.Count - 1;
                if (num1 < 0)
                {
                    return this._defaultMetadata;
                }
                if (num1 == 0)
                {
                    this._metadataMap.GetKeyValuePair(num1, out num2, out obj1);
                    while (radObjectType.Id > num2)
                    {
                        radObjectType = radObjectType.BaseType;
                    }
                    if (num2 == radObjectType.Id)
                    {
                        return (RadPropertyMetadata)obj1;
                    }
                }
                else
                    if (radObjectType.Id != 0)
                    {
                        do
                        {
                            this._metadataMap.GetKeyValuePair(num1, out num2, out obj1);
                            num1--;
                            while ((radObjectType.Id < num2) && (num1 >= 0))
                            {
                                this._metadataMap.GetKeyValuePair(num1, out num2, out obj1);
                                num1--;
                            }
                            while (radObjectType.Id > num2)
                            {
                                radObjectType = radObjectType.BaseType;
                            }
                            if (num2 == radObjectType.Id)
                            {
                                return (RadPropertyMetadata)obj1;
                            }
                        }
                        while (num1 >= 0);
                    }
            }
            return this._defaultMetadata;
        }

        internal RadPropertyMetadata GetMetadata(RadObjectType type, out bool found)
        {
            RadPropertyMetadata metadata = GetMetadata(type);
            found = metadata != this._defaultMetadata;

            return metadata;
        }

        public int GlobalIndex
        {
            get
            {
                return this._globalIndex;
            }
        }

        public static RadProperty Find(string className, string propertyName)
        {
            RadProperty res = FindSafe(className, propertyName);
            if (res == null)
            {
                throw new RadPropertyNotFoundException(propertyName, className);
                //object[] objArray1 = new object[] { propertyName, className };
                //throw new ArgumentException(string.Format("No such property registered: {0}, {1}", objArray1));
            }

            return res;
        }

        public static RadProperty FindSafe(string className, string propertyName)
        {
            Type objectType = RadTypeResolver.Instance.GetTypeByName(className, false);
			if (objectType == null)
				return null;

            RadProperty.FromNameKey key1 = new RadProperty.FromNameKey(propertyName, objectType);
            lock (RadProperty.Synchronized)
            {
                if (RadProperty.PropertyFromName.Contains(key1))
                {
                    return (RadProperty)RadProperty.PropertyFromName[key1];
                }
                else
                {
                    return null;
                }
            }
        }

        public static RadProperty FindSafe(Type objectType, string propertyName)
        {
            RadProperty.FromNameKey key1 = new RadProperty.FromNameKey(propertyName, objectType);
            lock (RadProperty.Synchronized)
            {
                if (RadProperty.PropertyFromName.Contains(key1))
                {
                    return (RadProperty)RadProperty.PropertyFromName[key1];
                }
                else
                {
                    return null;
                }
            }
        }

        public static RadProperty Find(Type objectType, string propertyName)
        {
            RadProperty res = FindSafe(objectType, propertyName);
            if (res == null)
            {
                throw new RadPropertyNotFoundException(propertyName, objectType.FullName);
                //object[] objArray1 = new object[] { propertyName, objectType.FullName };
                //throw new ArgumentException(string.Format("No such property registered: {0}, {1}", objArray1));
            }

            return res;
        }

        public string FullName
        {
            get
            {
                return this.OwnerType.FullName + "." + this.Name;
            }
        }

        internal FromNameKey PropertyKey
        {
            get
            {
                if (this.key == null)
                {
                    this.key = new RadProperty.FromNameKey(this.Name, this.OwnerType);
                }

                return this.key;
            }
        }

		public System.ComponentModel.PropertyDescriptor FindClrProperty()
		{
			return TypeDescriptor.GetProperties(this.OwnerType).Find(this.Name, false);
		}
	}
}
